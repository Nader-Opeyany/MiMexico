using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MiMexicoWeb.Data;
using MiMexicoWeb.Models;
using MiMexicoWeb.Models.ViewModel;
using Stripe;
using Stripe.Checkout;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
//SMS Imports
using System;
using System.Collections.Generic;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace MiMexicoWeb.Areas.Customer.Controllers
{
    
    public class CartController : Controller
    {
        private readonly ApplicationDBContext _db;
        public ShoppingCartVM ShoppingCartVM;
        public OrderDetails OrderDetail;
        public OrderHeader OrderHeader;
        internal DbSet<ShoppingCart> dbSet;
        internal DbSet<Meat> dbSetMeat;
        internal DbSet<OrderHeader> dbSetOrderHeader;
        internal DbSet<OrderDetails> dbSetOrderDetails;
        private IConfiguration _configuration;

        [BindProperty]
        public ShoppingCartVM ShoppingCartViewModel { get; set; }

        public CartController(ApplicationDBContext db, IConfiguration iconfig)
        {
            _db = db;
            this.dbSet = _db.Set<ShoppingCart>();
            this.dbSetMeat = _db.Set<Meat>();
            this.dbSetOrderHeader = _db.Set<OrderHeader>();
            this.dbSetOrderDetails = _db.Set<OrderDetails>();
            _configuration = iconfig;


        }

        public IActionResult Index()
        {

            var cookie = Request.Cookies["firstRequest"];
            int currentShoppingCartNumber = int.Parse(cookie);
            var includedProperties = "Item";
            IQueryable<ShoppingCart> query = dbSet;
            IQueryable<ShoppingCart> tempQuery = dbSet;

            query = query.Where(u => u.shoppingCartID == currentShoppingCartNumber);

            foreach (var includedProperty in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includedProperty);
            }

            IEnumerable<ShoppingCart> ShoppingCartList = query.ToList();
            int length = ShoppingCartList.Count();
            ShoppingCart[] shoppingCarts = new ShoppingCart[length];
            ShoppingCart blankCart = new ShoppingCart();
            blankCart.Id = -1;
            int i = 0;
            int tempId = -1;
            foreach(var c in ShoppingCartList)
            {
                shoppingCarts[i] = c;
                i++;
            }
            foreach (var c in ShoppingCartList)
            {
                for (i = 0; i < length; i++)
                {
                    if ((c.Id != -1 && shoppingCarts[i].Id != -1) && (c.Item.name == shoppingCarts[i].Item.name) && c.meatId == shoppingCarts[i].meatId &&(c.Id != shoppingCarts[i].Id))
                    {
                        int j = 0;
                        tempId = shoppingCarts[i].Id;

                        while(j < length)
                        {
                            if(c.Id == shoppingCarts[j].Id)
                            {
                                shoppingCarts[j] = blankCart;
                                
                                break;
                            }
                            j++;
                        }
                            
                        
                        tempQuery = query.Where(x => x.Id == c.Id);
                        var cart = tempQuery.FirstOrDefault();
                        if (cart != null)
                        {
                            cart.quantity += shoppingCarts[i].quantity;
                            shoppingCarts[i] = blankCart;
                            _db.SaveChanges();
                        }
                        Remove(tempId);
                    }

                }
            }
            //Sort of works but not really
            IQueryable<ShoppingCart> secondQuery = dbSet;
            secondQuery = secondQuery.Where(u => u.shoppingCartID == currentShoppingCartNumber);

            foreach (var includedProperty in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                secondQuery = secondQuery.Include(includedProperty);
            }
            ShoppingCartList = secondQuery.ToList();

            IQueryable<Meat> queryMeat = dbSetMeat;
            

            IEnumerable<Meat> Meats = queryMeat.ToList();


            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = ShoppingCartList,
                MeatList = Meats.ToList(),
                OrderHeader = new()

            };

            foreach(var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBaseonQuantity(cart.Price, cart.quantity);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Item.price * cart.quantity);
            }

            return View(ShoppingCartVM);
        }
        [HttpGet]
        public IActionResult Summary()
        {

            var cookie = Request.Cookies["firstRequest"];
            int currentShoppingCartNumber = int.Parse(cookie);

            var includedProperties = "Item";
            IQueryable<ShoppingCart> query = dbSet;

            query = query.Where(u => u.shoppingCartID == currentShoppingCartNumber);

            foreach (var includedProperty in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includedProperty);
            }

            IEnumerable<ShoppingCart> ShoppingCartList = query.ToList();

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = ShoppingCartList.ToList(),
                OrderHeader=new()
            };  

            ShoppingCartVM.OrderHeader.OrderHeaderId = currentShoppingCartNumber;

            foreach (var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBaseonQuantity(cart.Price, cart.quantity);
                ShoppingCartVM.OrderHeader.OrderTotal += (cart.Item.price * cart.quantity);
            }

            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ActionName("Summary")]
        [ValidateAntiForgeryToken]
        public IActionResult Summary(ShoppingCartVM viewModel)
        {
            var cookie = Request.Cookies["firstRequest"];
            int currentShoppingCartNumber = int.Parse(cookie);
            double orderTotal = 0;
            IQueryable<ShoppingCart> query = dbSet;
            IQueryable<Meat> meatQuery = dbSetMeat;
            var includedProperties = "Item";
            query = query.Where(u => u.shoppingCartID == currentShoppingCartNumber);

            foreach (var includedProperty in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includedProperty);
            }
            IEnumerable<ShoppingCart> ShoppingCartList = query.ToList();

            viewModel.ListCart = ShoppingCartList;


            //START HERE

            viewModel.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
            viewModel.OrderHeader.OrderStatus = SD.StatusPending;
            viewModel.OrderHeader.PaymentDate = DateTime.Now;
            viewModel.OrderHeader.OrderHeaderId = currentShoppingCartNumber;

            foreach (var item in ShoppingCartList)
            {
                item.Price = GetPriceBaseonQuantity(item.Price, item.quantity);
                viewModel.OrderHeader.OrderTotal += (item.Item.price * item.quantity);
            }

            if (viewModel.OrderHeader.Name == null && viewModel.OrderHeader.PhoneNumber == null)
            {
                ViewBag.Message = "Please Provide Your Name And Phone Number";
                return View(viewModel);
            }
            if (viewModel.OrderHeader.Name == null)
            {
                ViewBag.Message = "Please Provide Your Name";
                return View(viewModel);
            }
            if (viewModel.OrderHeader.PhoneNumber == null)
            {
                ViewBag.Message = "Please Provide Your Phone Number";
                return View(viewModel);
            }


            viewModel.OrderHeader.PhoneNumber = new string(viewModel.OrderHeader.PhoneNumber.Where(c => char.IsDigit(c)).ToArray());

            if (viewModel.OrderHeader.PhoneNumber.Length != 10)
            {
                ViewBag.Message = "Invalid Phone Number. Please Insert Your Phone Number As Either (916)123-4567 Or 9161234567";
                return View(viewModel);
            }

            _db.OrderHeaders.Add(viewModel.OrderHeader);
            _db.SaveChanges();

            //OrderHeader = new OrderHeader()
            //{
            //    OrderHeaderId = currentShoppingCartNumber,
            //    Name = viewModel.OrderHeader.Name,
            //    PhoneNumber = viewModel.OrderHeader.PhoneNumber,
            //    OrderTotal = orderTotal
            //};
            //_db.Add(OrderHeader);
            //_db.SaveChanges();

            

            TwilioSettings t = new TwilioSettings();
            t.AccountSID = _configuration.GetValue<string>("Twilio:AccountSID");
            t.AuthToken = _configuration.GetValue<string>("Twilio:AuthToken");


            TwilioClient.Init(t.AccountSID, t.AuthToken);

            var message = MessageResource.Create(
                body: "Your meal is ready!",
                from: new Twilio.Types.PhoneNumber("+18304452606"),
                to: new Twilio.Types.PhoneNumber(viewModel.OrderHeader.PhoneNumber)
            );

            //Stripe settings
            var domain = "https://localhost:7121/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain+$"customer/cart/OrderConfirmation?id={viewModel.OrderHeader.Id}",
                CancelUrl = domain+$"customer/cart/index",
            };

            foreach (var item in viewModel.ListCart)
            {

                var sessionLineItem = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Item.price * 100),//20.00 -> 2000
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Item.name
                        },

                    },
                    Quantity = item.quantity,
                };
                options.LineItems.Add(sessionLineItem);

            }

            var service = new SessionService();
            Session session = service.Create(options);
            viewModel.OrderHeader.SessionId = session.Id;
            viewModel.OrderHeader.PaymentIntentId = session.PaymentIntentId;
            _db.SaveChanges();
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);


        }

        public IActionResult OrderConfirmation(int id)
        {
            IQueryable<OrderHeader> query = dbSetOrderHeader;
            query = query.Where(u => u.Id == id);
            OrderHeader orderHeader = query.FirstOrDefault();
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            IQueryable<Meat> meatQuery = dbSetMeat;
            IQueryable<ShoppingCart> orderDetailCarts = dbSet;

            var cookie = Request.Cookies["firstRequest"];
            int currentShoppingCartNumber = int.Parse(cookie);
            var includedProperties = "Item";
            orderDetailCarts = orderDetailCarts.Where(u => u.shoppingCartID == currentShoppingCartNumber);

            foreach (var includedProperty in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                orderDetailCarts = orderDetailCarts.Include(includedProperty);
            }
            IEnumerable<ShoppingCart> OrderDetailCartList = orderDetailCarts.ToList();

            if (session.PaymentStatus.ToLower() == "paid")
            {
                orderHeader.OrderStatus = SD.StatusApproved;
                orderHeader.PaymentStatus = SD.PaymentStatusApproved;
                _db.SaveChanges();

                foreach (var cart in OrderDetailCartList)
                {
                    meatQuery = dbSetMeat;
                    Meat tempMeat = new Meat()
                    {
                        name = "No Meat"
                    };
                    if (cart.Item.Beverage == false)
                    {
                        meatQuery = meatQuery.Where(x => x.id == cart.meatId);
                        tempMeat = meatQuery.First();
                    }


                    OrderDetail = new OrderDetails()
                    {
                        OrderId = orderHeader.Id,
                        itemId = cart.Item.id,
                        item = cart.Item,
                        Count = cart.quantity,
                        Price = GetPriceBaseonQuantity(cart.Item.price, cart.quantity),
                        meatId = cart.meatId,
                        MeatName = tempMeat.name
                    };
                    _db.Add(OrderDetail);
                    _db.SaveChanges();
                }
            }
            IQueryable<ShoppingCart> queryShoppingCart = dbSet;
            queryShoppingCart = queryShoppingCart.Where(u => u.shoppingCartID == currentShoppingCartNumber);

            foreach (var includedProperty in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includedProperty);
            }
            IEnumerable<ShoppingCart> ShoppingCartList = queryShoppingCart.ToList();

            List<ShoppingCart> shoppingCarts = ShoppingCartList.ToList();
            dbSet.RemoveRange(shoppingCarts);
            _db.SaveChanges();
            return View(id);


        }

        public IActionResult Plus(int cartId)
        {

            IQueryable<ShoppingCart> query = dbSet;
            query = query.Where(u => u.Id == cartId);
            
            var cart = query.FirstOrDefault();

            cart.quantity += 1;
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
            
        }

        public IActionResult Minus(int cartId)
        {
            IQueryable<ShoppingCart> query = dbSet;
            query = query.Where(u => u.Id == cartId);
            var cart = query.FirstOrDefault();
            if(cart.quantity <=1)
            {
                _db.ShoppingCarts.Remove(cart);
            }
            else
            {
                cart.quantity -= 1;
            }
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));

        }

        public IActionResult Remove(int cartId)
        {
            IQueryable<ShoppingCart> query = dbSet;
            query = query.Where(u => u.Id == cartId);
            var cart = query.FirstOrDefault();

            _db.ShoppingCarts.Remove(cart);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBaseonQuantity(double quantity, double price)
        {
            double total = quantity * price;
            return total;
        }
    }
}

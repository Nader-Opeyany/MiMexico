using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiMexicoWeb.Data;
using MiMexicoWeb.Models;
using MiMexicoWeb.Models.ViewModel;
using System.Drawing.Text;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

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


        public CartController(ApplicationDBContext db)
        {
            _db = db;
            this.dbSet = _db.Set<ShoppingCart>();
            this.dbSetMeat = _db.Set<Meat>();
            this.dbSetOrderHeader = _db.Set<OrderHeader>();
            this.dbSetOrderDetails = _db.Set<OrderDetails>();
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
        [ValidateAntiForgeryToken]
        public IActionResult Summary(ShoppingCartVM viewModel)
        {
            var cookie = Request.Cookies["firstRequest"];
            int currentShoppingCartNumber = int.Parse(cookie);
            double orderTotal = 0;
            IQueryable<ShoppingCart> query = dbSet;
            var includedProperties = "Item";
            query = query.Where(u => u.shoppingCartID == currentShoppingCartNumber);

            foreach (var includedProperty in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includedProperty);
            }
            IEnumerable<ShoppingCart> ShoppingCartList = query.ToList();
            foreach(var item in ShoppingCartList)
            {
                item.Price = GetPriceBaseonQuantity(item.Price, item.quantity);
                orderTotal += (item.Item.price * item.quantity);
            }
            viewModel.OrderHeader.PhoneNumber = new string(viewModel.OrderHeader.PhoneNumber.Where(c => char.IsDigit(c)).ToArray());

            OrderHeader = new OrderHeader()
            {
                OrderHeaderId = currentShoppingCartNumber,
                Name = viewModel.OrderHeader.Name,
                PhoneNumber = viewModel.OrderHeader.PhoneNumber,
                OrderTotal = orderTotal
            };
            _db.Add(OrderHeader);
            _db.SaveChanges();

            foreach(var cart in ShoppingCartList)
            {
                OrderDetail = new OrderDetails()
                {
                    OrderId = OrderHeader.Id,
                    itemId = cart.Item.id,
                    item = cart.Item,
                    Count = cart.quantity,
                    Price = GetPriceBaseonQuantity(cart.Item.price, cart.quantity)
                };
                _db.Add(OrderDetail);
                _db.SaveChanges();
            }

            return RedirectToAction("Summary");
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

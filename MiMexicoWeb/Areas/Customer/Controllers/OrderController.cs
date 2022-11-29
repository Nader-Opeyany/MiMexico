using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using MiMexicoWeb.Areas.Admin.Controllers;
using MiMexicoWeb.Data;
using MiMexicoWeb.Migrations;
using MiMexicoWeb.Models;
using MiMexicoWeb.Models.ViewModel;
using NuGet.Protocol.Plugins;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;

//SMS Imports
using System;
using System.Collections.Generic;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace MiMexicoWeb.Areas.Customer.Controllers
{
    public class OrderController : Controller
    {

        private readonly ILogger<ShoppingCartController> _logger;
        private ShoppingCart shoppingCart;
        private readonly ApplicationDBContext _db;
        private int shoppingCartNumber;
        //private readonly HttpOptionsAttribute _options;
        //private Sender thisSender;
        //private TextWriter textWriter;
        internal DbSet<Item> dbSet;

        public OrderController(ApplicationDBContext db, ILogger<ShoppingCartController> logger)
        {
            _db = db;
            _logger = logger;
            shoppingCart = new ShoppingCart();
            this.dbSet = _db.Set<Item>();
            //_options = options;
            //TextWriter textWriter = null;

        }

        /*        public IActionResult Order()
                {
                    IEnumerable<OrderClass> objFromOrderList = _db.SimpleOrderTable;
                    return View(objFromOrderList);
                }*/


        [HttpGet]
        public IActionResult AddItem(int itemId)
        {

            string includedProperties = "Meat,Condiment";
            IQueryable<Item> query = dbSet;
            query = query.Where(u => u.id == itemId);
            foreach (var includedProprty in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includedProprty);
            }

            Item foodItem = query.FirstOrDefault();

            if (!HttpContext.Request.Cookies.ContainsKey("firstRequest"))
            {
               
                shoppingCartNumber = setShoppingCartNumber();
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.MinValue;
               
                HttpContext.Response.Cookies.Append("firstRequest", shoppingCartNumber.ToString());
                ItemViewModel IVMObj = new()
                {
                    ShoppingCart = new()
                    {
                        quantity = 1,
                        itemId = itemId,
                        Item = foodItem,
                        shoppingCartID = shoppingCartNumber
                    },
                    MeatList = _db.Meats.Select(i => new SelectListItem()
                    {
                        Text = i.name,
                        Value = i.id.ToString(),

                    }),
                    CondimentList = _db.Condiments.Select(i => new SelectListItem()
                    {
                        Text = i.name,
                        Value = i.id.ToString()
                    })
                };
                ShoppingCart cartObj = new()
                {
                    quantity = 1,
                    itemId = itemId,
                    Item = foodItem,
                    shoppingCartID = shoppingCartNumber
                };
                return View(IVMObj);
            }
            else
            {
                var cookie = Request.Cookies["firstRequest"];
                int currentShoppingCartNumber = int.Parse(cookie);
                ItemViewModel IVMObj = new()
                {
                    ShoppingCart = new()
                    {
                        quantity = 1,
                        itemId = itemId,
                        Item = foodItem,
                        shoppingCartID = currentShoppingCartNumber
                    },
                    MeatList = _db.Meats.Select(i => new SelectListItem()
                    {
                        Text = i.name,
                        Value = i.id.ToString(),

                    }),
                    CondimentList = _db.Condiments.Select(i => new SelectListItem()
                    {
                        Text = i.name,
                        Value = i.id.ToString()
                    })
                };
                ShoppingCart cartObj = new()
                {
                    quantity = 1,
                    itemId = itemId,
                    Item = foodItem,
                    shoppingCartID = shoppingCartNumber
                };
                return View(IVMObj);

            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddItem(ItemViewModel viewModelCart)
        {

            _db.ShoppingCarts.Add(viewModelCart.ShoppingCart);
            _db.SaveChanges();

            return RedirectToAction("Order", "Order");
        }



        [HttpGet]
        public IActionResult Order()
        {
            string includeProperties = "Meat,Condiment";
            IQueryable<Item> query = dbSet;
            foreach(var includedProperety in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includedProperety);
            }

            IEnumerable<Item> itemList = query.ToList();

            // this would return item list view

            //ItemViewModel itemViewModel = new()
            //{
            //    ShoppingCart = new(),
            //    ItemList = _db.Items.Select(i => new SelectListItem()
            //    {
            //        Text = i.name,
            //        Value = i.id.ToString(),
            //    }),
            //    MeatList = _db.Meats.Select(i => new SelectListItem()
            //    {
            //        Text = i.name,
            //        Value = i.id.ToString(),
            //    }),
            //    CondimentList = _db.Condiments.Select(i => new SelectListItem()
            //    {
            //        Text = i.name,
            //        Value = i.id.ToString()
            //    })


            //};

            if (itemList == null)
            {
                return View(itemList);
            }

            return View(itemList);
        }

        //POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Order(ItemViewModel obj)
        {
            if (ModelState.IsValid)
            {
                _db.Add(obj.ShoppingCart);
               _db.SaveChanges();
                //TempData["success"] = "Prodcut creating successully";
                //return RedirectToAction("Landing", "Home");
           }
          return View("Order");

        }


        // GET: OrderController
        //public ActionResult Index()
        //{
        //    return View(currentView);
        //}

        public int setShoppingCartNumber()
        {
            shoppingCartNumber += 1;
            return shoppingCartNumber;
        }

        public ActionResult AddItem(OrderClass model, string orderItem, int orderQuantity)
        {
            model.order += orderQuantity.ToString() + " " + orderItem +"| ";

            return View(model);
        }

        public ActionResult SetFirstName(OrderClass model, string firstName)
        {
            model.customerFirstName = firstName;

            return View(model);
        }

        public ActionResult SetLastName(OrderClass model, string lastName)
        {
            model.customerLastName = lastName;

            return View(model);
        }

        public ActionResult AddPrice(OrderClass model, int price)
        {
            model.price += price;

            return View(model);
        }

        public ActionResult SubtractPrice(OrderClass model, int price)
        {
            model.price -= price;

            return View(model);
        }

        public ActionResult SetSpecialInstruction(OrderClass model, string instruction)
        {
            model.specialInstructions = instruction;

            return View(model);
        }

        //Twilio SMS method
        public IActionResult SMSMessage()
        {
            // Find your Account SID and Auth Token at twilio.com/console
            // and set the environment variables. See http://twil.io/secure
            string accountSid = "AC824ff7cac7a8d45d476474f86f5eab20";
            string authToken = "25965ce9b53f037f5cbd11c09368f243";

            TwilioClient.Init(accountSid, authToken);

            var message = MessageResource.Create(
                body: "Your meal is ready!",
                from: new Twilio.Types.PhoneNumber("+18304452606"),
                to: new Twilio.Types.PhoneNumber("+1(xxx)xxx-xxxx")
            );

            Console.WriteLine(message.Sid);

            return RedirectToAction("Order", "Order");

        }
    }
}

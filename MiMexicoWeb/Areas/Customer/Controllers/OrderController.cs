using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using MiMexicoWeb.Areas.Admin.Controllers;
using MiMexicoWeb.Data;
using MiMexicoWeb.Migrations;
using MiMexicoWeb.Models;
using MiMexicoWeb.Models.ViewModel;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Net.Http.Headers;

namespace MiMexicoWeb.Areas.Customer.Controllers
{
    public class OrderController : Controller
    {

        private readonly ILogger<ShoppingCartController> _logger;
        private ShoppingCart shoppingCart;
        private readonly ApplicationDBContext _db;
        internal DbSet<Item> dbSet;

        public OrderController(ApplicationDBContext db, ILogger<ShoppingCartController> logger)
        {
            _db = db;
            _logger = logger;
            shoppingCart = new ShoppingCart();
            this.dbSet = _db.Set<Item>();
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

            ShoppingCart cartObj = new()
            {
                quantity = 1,
                itemId = itemId,
                Item = foodItem
            };


            return View(cartObj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddItem(ShoppingCart shoppingCart)
        {

            //string includedProperties = "Meat,Condiment";
            //IQueryable<Item> query = dbSet;
            //query = query.Where(u => u.id == id);
            //foreach (var includedProprty in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            //{
            //    query = query.Include(includedProprty);
            //}

            //Item foodItem = query.FirstOrDefault();

            //ShoppingCart cartObj = new()
            //{
            //    quantity = 1,
            //    Item = foodItem
            //};

            _db.ShoppingCarts.Add(shoppingCart);
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

            //if(id == null || id == 0)
            //{
                return View(itemList);
            //}

            //return View(itemViewModel);
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
    }
}

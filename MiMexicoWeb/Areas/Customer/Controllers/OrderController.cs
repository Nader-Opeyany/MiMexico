using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.Internal;
using MiMexicoWeb.Data;
using MiMexicoWeb.Models;

namespace MiMexicoWeb.Areas.Customer.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDBContext _db;

        public OrderController(ApplicationDBContext db)
        {
            _db = db;
        }

        /*        public IActionResult Order()
                {
                    IEnumerable<OrderClass> objFromOrderList = _db.SimpleOrderTable;
                    return View(objFromOrderList);
                }*/

        // GET
        [HttpGet]
        public IActionResult Order()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Order(OrderClass obj)
        {
            if (ModelState.IsValid)
            {
                _db.SimpleOrderTable.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Landing", "Home");
            }
            return View(obj);

        }

        /*        public IActionResult Index(OrderClass obj)
                {
                    _db.SimpleOrderTable.Add(obj);
                    _db.SaveChanges();
                }*/



        public OrderClass currentView = new OrderClass();

        // GET: OrderController
        public ActionResult Index()
        {
            return View(currentView);
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
    }
}

using Microsoft.AspNetCore.Mvc;
using MiMexicoWeb.Data;
using MiMexicoWeb.Models;
using System.Diagnostics;




namespace MiMexicoWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private OrderClass orderClass;
        private readonly ApplicationDBContext _db;

        public HomeController(ApplicationDBContext db, ILogger<HomeController> logger)
        {
            _db = db;
            _logger = logger;
            orderClass = new OrderClass();
        }
        // GET
        //[HttpGet]
        //public IActionResult Order()
        //{
        //    return View();
        //}

        //// POST
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult Order(OrderClass obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.SimpleOrderTable.Add(obj);
        //        _db.SaveChanges();
        //        return RedirectToAction("Landing", "Home");
        //    }
        //    return View(obj);

        //}
        public IActionResult Menu()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }

        public IActionResult Landing()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
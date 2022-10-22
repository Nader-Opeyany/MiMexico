using Microsoft.AspNetCore.Mvc;
using MiMexicoWeb.Data;
using MiMexicoWeb.Models;

namespace MiMexicoWeb.Areas.Admin.Controllers
{
    public class OrderController : Controller { 
    
        private readonly ApplicationDBContext _db;

        public OrderController(ApplicationDBContext db)
        {
            _db = db;
        }

        //GET
        public IActionResult Order()
        {
            var objProductList = _db.SimpleOrderTable.ToList();
            return View();
        }

/*        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Order() 
        {
            return RedirectToAction("Order");
        }*/
    }
}

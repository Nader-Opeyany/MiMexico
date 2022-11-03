using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiMexicoWeb.Areas.Admin.Controllers;
using MiMexicoWeb.Data;
using MiMexicoWeb.Models;
namespace MiMexicoWeb.Areas.Customer.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ILogger<ShoppingCartController> _logger;
        private ShoppingCart shoppingCartClass;
        private readonly ApplicationDBContext _db;
        internal DbSet<ShoppingCart> dbSet;

        public ShoppingCartController(ApplicationDBContext db, ILogger<ShoppingCartController> logger)
        {
            _db = db;
            dbSet = _db.Set<ShoppingCart>();
            _logger = logger;
            shoppingCartClass = new ShoppingCart();
        }
        public IActionResult Index()
        {
            IEnumerable<ShoppingCart> objCartList = _db.ShoppingCarts.ToList();
            return View(objCartList);
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ShoppingCart obj)
        {
            if (ModelState.IsValid)
            {
                _db.ShoppingCarts.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Order", "Home");
            }
            return View(obj);
        }

        // GET
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            IQueryable<ShoppingCart> query = dbSet;
            query = query.Where(u => u.Id == id);

            var cartFromDbFirst = query.FirstOrDefault();

            if (cartFromDbFirst == null)
            {
                return NotFound();
            }

            return View(cartFromDbFirst);
        }

        // POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            IQueryable<ShoppingCart> query = dbSet;
            query = query.Where(u => u.Id == id);
            var obj = query.FirstOrDefault();

            if (obj == null)
            {
                return NotFound();
            }

            dbSet.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Item deleted sucessfuly";
            return RedirectToAction("Index");
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiMexicoWeb.Data;
using MiMexicoWeb.Models;

namespace MiMexicoWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class MeatController : Controller
    {
        private readonly ILogger<MeatController> _logger;
        private Meat meatClass;
        private readonly ApplicationDBContext _db;
        internal DbSet<Meat> dbSet;
        public MeatController(ApplicationDBContext db, ILogger<MeatController> logger)
        {
            _db = db;
            dbSet = _db.Set<Meat>();
            _logger = logger;
            meatClass = new Meat();
        }
        public IActionResult Index()
        {
            IEnumerable<Meat> objMeatList = _db.Meats.ToList();
            return View(objMeatList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Meat obj)
        {
            if (ModelState.IsValid)
            {
                _db.Meats.Add(obj);
                _db.SaveChanges();
                return RedirectToAction("Landing", "Home");
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

            IQueryable<Meat> query = dbSet;
            query = query.Where(u => u.id == id);

            var meatFromDbFirst = query.FirstOrDefault();

            if (meatFromDbFirst == null)
            {
                return NotFound();
            }

            return View(meatFromDbFirst);
        }

        // POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            IQueryable<Meat> query = dbSet;
            query = query.Where(u => u.id == id);
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

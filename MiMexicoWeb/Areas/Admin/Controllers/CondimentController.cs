using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiMexicoWeb.Data;
using MiMexicoWeb.Models;

namespace MiMexicoWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class CondimentController : Controller
    {
        private readonly ILogger<CondimentController> _logger;
        private Condiment condimentClass;
        private readonly ApplicationDBContext _db;
        internal DbSet<Condiment> dbSet;

        public CondimentController(ApplicationDBContext db, ILogger<CondimentController> logger)
        {
            _db = db;
            dbSet = _db.Set<Condiment>();
            _logger = logger;
            condimentClass = new Condiment();
        }
        public IActionResult Index()
        {
            IEnumerable<Condiment> objCondimentList = _db.Condiments.ToList();
            return View(objCondimentList);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Condiment obj)
        {
            if (ModelState.IsValid)
            {
                _db.Condiments.Add(obj);
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

            IQueryable<Condiment> query = dbSet;
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
            IQueryable<Condiment> query = dbSet;
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

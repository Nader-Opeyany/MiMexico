using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MiMexicoWeb.Data;
using MiMexicoWeb.Migrations;
using MiMexicoWeb.Models;
using MiMexicoWeb.Models.ViewModel;
using System.Linq.Expressions;

namespace MiMexicoWeb.Areas.Admin.Controllers
{
    public class ItemController : Controller
    {
        private readonly ILogger<ItemController> _logger;
        private Item itemClass;
        private readonly ApplicationDBContext _db;
        internal DbSet<Item> dbSet;

        public ItemController(ApplicationDBContext db, ILogger<ItemController> logger)
        {
            _db = db;
            _logger = logger;
            itemClass = new Item();
            this.dbSet = _db.Set<Item>(); 
        }

        public IActionResult Index()
        {
            return View();
        }

        // Get
        [HttpGet]
        public IActionResult Create(int? id)
        {

            ItemViewModel itemViewModel = new()
            {
                Item = new(),
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

            if (id == null || id == 0)
            {
                return View(itemViewModel);
            }
            else
            {

            }
            return View(itemViewModel);
        }

        // Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ItemViewModel obj)
        {
            if (ModelState.IsValid)
            {
                _db.Add(obj.Item);
                _db.SaveChanges();
                TempData["success"] = "Prodcut created Sucessfully";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Create");
        }

        #region API CALLS
        [HttpGet]

        public IActionResult GetAll()
        {

            IQueryable<Item> query = dbSet;

            var includedProperties = "Meat";
            foreach(var includeProp in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProp);
            }

            var productList = query.ToList();

            return Json(new { data = productList });
        }



        #endregion
    }
}
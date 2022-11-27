using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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
        private readonly IWebHostEnvironment _hostEnvironment;
        internal DbSet<Item> dbSet;


        public ItemController(ApplicationDBContext db, ILogger<ItemController> logger, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _logger = logger;
            itemClass = new Item();
            this.dbSet = _db.Set<Item>();
            _hostEnvironment = hostEnvironment;
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

            if (id == 0 || id == null)
            {
                return View(itemViewModel);
            }
            else
            {
                IQueryable<Item> query = dbSet;
                query = query.Where(u => u.id == id);
                itemViewModel.Item = query.FirstOrDefault();
                return View(itemViewModel);
            }
        }

        // Post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(ItemViewModel obj, IFormFile file)
        {

            if (ModelState.IsValid)
            {
                string wwwRoothPath = _hostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRoothPath, @"images\Items");
                    var extension = Path.GetExtension(file.FileName);

                    if (obj.Item.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(wwwRoothPath, obj.Item.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileSteams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        file.CopyTo(fileSteams);
                    }
                    obj.Item.ImageUrl = @"\images\items" + fileName + extension;
                }
                if (obj.Item.id == 0)
                {
                    _db.Add(obj.Item);
                }
                else
                {
                    _db.Update(obj.Item);
                }
                _db.SaveChanges();
                TempData["success"] = "Prodcut created Sucessfully";
                return RedirectToAction("Index");
            }

            //_db.Add(obj.Item);
            //_db.SaveChanges();
            //TempData["success"] = "Prodcut created Sucessfully";
            //return RedirectToAction("Index");
            return View(obj);

        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {
            string includedProperties = "Meat,Condiment";
            IQueryable<Item> query = dbSet;
            foreach (var includedProperty in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includedProperty);
            }
            var itemList = query.ToList();
            return Json(new { data = itemList });
        }

        //POST
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var obj = _db.Items.FirstOrDefault(u => u.id == id);
            if(obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _db.Items.Remove(obj);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });
        }


        #endregion

    }
}



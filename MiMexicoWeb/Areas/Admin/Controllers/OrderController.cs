using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MiMexicoWeb.Data;
using MiMexicoWeb.Models;
using System.Diagnostics;

namespace MiMexicoWeb.Areas.Admin.Controllers
{

    public class OrderController : Controller { 
    
        private readonly ApplicationDBContext _db;
        private readonly DbSet<OrderHeader> dbSet;
        private readonly DbSet<OrderDetails> dbSetDetails;

        public OrderController(ApplicationDBContext db)
        {
            _db = db;
            this.dbSet = _db.Set<OrderHeader>();
            this.dbSetDetails = _db.Set<OrderDetails>();
        }

        public IActionResult Index()
        {
            return View();
        }

        //GET
        public IActionResult Order()
        {
            var objProductList = _db.SimpleOrderTable.ToList();
            return View();
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll()
        {

            //OrderViewModel orderViewModel = new OrderViewModel()
            //{
            //    OrderDetails = new List<OrderDetails>(),
            //    OrderHeader = new OrderHeader()
            //};

            string includedProperties = "item";
            //IEnumerable<OrderDetails> orderDetails;
            IQueryable<OrderDetails> query = dbSetDetails;

            foreach (var includedProperty in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includedProperty);
            }
           
            query.ToList();

            return Json(new { data = query });
        }

        //POST
        
        public IActionResult Delete(int? id)
        {
            string includedProperties = "item";
            IEnumerable<OrderDetails> orderDetails;
            IQueryable<OrderDetails> query = dbSetDetails;
            var items = query.Where(u => u.Id != id);

            var obj = _db.OrderDetails.FirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            _db.OrderDetails.Remove(obj);
            _db.SaveChanges();
            return Json(new { success = true, message = "Delete Successful" });


            //foreach (var includedProperty in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            //{
            //    query = query.Include(includedProperty);
            //}
            //orderDetails = items.ToList();

            //switch (status)
            //{
            //    case "inprocess":
            //        orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusPending);
            //        break;
            //    case "completed":
            //        orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.PaymentStatusApproved);
            //        break;
            //    default:
            //        //orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
            //        break;

            //}

            //return Json(new { data = items });
        }
        #endregion
    }
}

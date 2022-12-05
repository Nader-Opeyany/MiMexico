using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MiMexicoWeb.Data;
using MiMexicoWeb.Models;
using System.Diagnostics;
//SMS Imports
using System;
using System.Collections.Generic;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace MiMexicoWeb.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class OrderController : Controller { 
    
        private readonly ApplicationDBContext _db;
        private readonly DbSet<OrderHeader> dbSet;
        private readonly DbSet<OrderDetails> dbSetDetails;
        private IConfiguration _configuration;


        public OrderController(ApplicationDBContext db, IConfiguration iconfig)
        {
            _db = db;
            this.dbSet = _db.Set<OrderHeader>();
            this.dbSetDetails = _db.Set<OrderDetails>();
            _configuration = iconfig;
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

        //Send SMS Twilio SMS
        public IActionResult SendSMS(int? id)
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

            IQueryable<OrderHeader> orderHeader = dbSet;
            
            orderHeader = orderHeader.Where(u => u.Id == obj.OrderId);
            OrderHeader user = orderHeader.FirstOrDefault();

            //Twilio SMS
            TwilioSettings t = new TwilioSettings();
            t.AccountSID = _configuration.GetValue<string>("Twilio:AccountSID");
            t.AuthToken = _configuration.GetValue<string>("Twilio:AuthToken");
            t.PhoneNumber = _configuration.GetValue<string>("Twilio:PhoneNumber");


            TwilioClient.Init(t.AccountSID, t.AuthToken);

            var message = MessageResource.Create(
                body: "Hello " + user.Name + ", your order is now ready for pickup.",
                from: new Twilio.Types.PhoneNumber(t.PhoneNumber),
                to: new Twilio.Types.PhoneNumber(user.PhoneNumber)
            );

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}

/*using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiMexicoWeb.Models;

namespace MiMexicoWeb.Areas.Customer.Controllers
{
    public class OrderController : Controller
    {

        public OrderClass currentView = new OrderClass();

        // GET: OrderController
        public ActionResult Index()
        {
            return View(currentView);
        }

       /* public ActionResult AddItem(OrderClass model, string orderItem, int orderQuantity)
        {
            for(int i = 0; i < 50; i++ )
            {
                if(model.order[i] != null)
                {
                    model.order[i] = orderQuantity.ToString() + " " + orderItem;
                }
            }

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
*/
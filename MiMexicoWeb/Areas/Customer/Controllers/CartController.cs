using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiMexicoWeb.Data;
using MiMexicoWeb.Models;
using MiMexicoWeb.Models.ViewModel;
using System.Drawing.Text;

namespace MiMexicoWeb.Areas.Customer.Controllers
{
    
    public class CartController : Controller
    {
        private readonly ApplicationDBContext _db;
        public ShoppingCartVM ShoppingCartVM;
        internal DbSet<ShoppingCart> dbSet;

        public CartController(ApplicationDBContext db)
        {
            _db = db;
            this.dbSet = _db.Set<ShoppingCart>();
        }
        public IActionResult Index()
        {
            var includedProperties = "Item";
            IQueryable<ShoppingCart> query = dbSet;

            foreach(var includedProperty in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includedProperty);
            }

            IEnumerable<ShoppingCart> ShoppingCartList = query.ToList();

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = ShoppingCartList.ToList(),
            };
            
            foreach(var cart in ShoppingCartVM.ListCart)
            {
                cart.Price = GetPriceBaseonQuantity(cart.Price, cart.quantity);
                ShoppingCartVM.CartTotal += (cart.Item.price * cart.quantity);
            }

            return View(ShoppingCartVM);
        }

        public IActionResult Summary()
        {
            //var includedProperties = "Item";
            //IQueryable<ShoppingCart> query = dbSet;

            //foreach (var includedProperty in includedProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            //{
            //    query = query.Include(includedProperty);
            //}

            //IEnumerable<ShoppingCart> ShoppingCartList = query.ToList();

            //ShoppingCartVM = new ShoppingCartVM()
            //{
            //    ListCart = ShoppingCartList.ToList(),
            //};

            //foreach (var cart in ShoppingCartVM.ListCart)
            //{
            //    cart.Price = GetPriceBaseonQuantity(cart.Price, cart.quantity);
            //    ShoppingCartVM.CartTotal += (cart.Item.price * cart.quantity);
            //}

            return View();
        }

        public IActionResult Plus(int cartId)
        {

            IQueryable<ShoppingCart> query = dbSet;
            query = query.Where(u => u.Id == cartId);
            
            var cart = query.FirstOrDefault();

            cart.quantity += 1;
            _db.SaveChanges();

            return RedirectToAction(nameof(Index));
            
        }

        public IActionResult Minus(int cartId)
        {
            IQueryable<ShoppingCart> query = dbSet;
            query = query.Where(u => u.Id == cartId);
            var cart = query.FirstOrDefault();
            if(cart.quantity <=1)
            {
                _db.ShoppingCarts.Remove(cart);
            }
            else
            {
                cart.quantity -= 1;
            }
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));

        }

        public IActionResult Remove(int cartId)
        {
            IQueryable<ShoppingCart> query = dbSet;
            query = query.Where(u => u.Id == cartId);
            var cart = query.FirstOrDefault();

            _db.ShoppingCarts.Remove(cart);
            _db.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private double GetPriceBaseonQuantity(double quantity, double price)
        {
            double total = quantity * price;
            return total;
        }
    }
}

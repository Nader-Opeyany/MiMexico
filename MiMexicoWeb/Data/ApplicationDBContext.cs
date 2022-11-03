using Microsoft.EntityFrameworkCore;
using MiMexicoWeb.Models;
namespace MiMexicoWeb.Data

{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options ) : base(options)
        {

        }

        public DbSet<Meat> Meats { get; set; }

        //Creating Table
        public DbSet<OrderClass> SimpleOrderTable { get; set; }

        public DbSet<Condiment> Condiments { get; set; }

        public DbSet<Item> Items { get; set; }

        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
    }
}

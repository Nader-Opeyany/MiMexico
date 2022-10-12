using Microsoft.EntityFrameworkCore;
using MiMexicoWeb.Models;
namespace MiMexicoWeb.Data

{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options ) : base(options)
        {

        }

        //Creating Table
        public DbSet<OrderClass> SimpleOrderTable { get; set; }
    }
}

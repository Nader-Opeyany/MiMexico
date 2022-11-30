using Microsoft.EntityFrameworkCore;
using MiMexicoWeb.Data;

namespace MiMexicoWeb.Dbintializer
{
    public class DbInitializer : IDbInitializer
    {

        private readonly ApplicationDBContext _db;

        public DbInitializer(ApplicationDBContext db)
        {
            _db = db; 
        }
        public void Initialize()
        {
            //migrations if they are not applied
            try
            {
                if(_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception e)
            {

            }
        }

    }
}

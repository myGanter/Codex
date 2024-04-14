using Microsoft.EntityFrameworkCore;

namespace DemoWeb.DataAccess
{
    public class DemoWebSQLiteContext : DbContext
    {
        public DemoWebSQLiteContext(DbContextOptions<DemoWebSQLiteContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}

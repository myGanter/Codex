using CodexCQRS.AspNet.Tests.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CodexCQRS.AspNet.Tests.DataAccess
{
    internal class CodexAspNetTestsSQLiteContext : DbContext
    {
        private static object _locker = new object();

        public DbSet<SaveChangesDecoratorModel> SaveChangesDecoratorModels { get; set; }

        public DbSet<TransactionDecoratorModel> TransactionDecoratorModels { get; set; }

        public CodexAspNetTestsSQLiteContext(DbContextOptions<CodexAspNetTestsSQLiteContext> options)
            : base(options)
        {
            lock (_locker)
            {
                Database.EnsureCreated();
            }
        }
    }
}

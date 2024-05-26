using CodexCQRS.AspNet.Tests.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace CodexCQRS.AspNet.Tests.DataAccess
{
    internal class CodexAspNetTestsSQLiteContext : DbContext
    {
        public DbSet<SaveChangesDecoratorModel> SaveChangesDecoratorModels { get; set; }

        public DbSet<TransactionDecoratorModel> TransactionDecoratorModels { get; set; }

        public CodexAspNetTestsSQLiteContext(DbContextOptions<CodexAspNetTestsSQLiteContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}

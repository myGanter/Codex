using Codex.AspNet.Tests.DataAccess.Models;
using Microsoft.EntityFrameworkCore;

namespace Codex.AspNet.Tests.DataAccess
{
    internal class CodexAspNetTestsSQLiteContext : DbContext
    {
        public DbSet<SaveChangesDecoratorModel> SaveChangesDecoratorModels { get; set; }

        public CodexAspNetTestsSQLiteContext(DbContextOptions<CodexAspNetTestsSQLiteContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}

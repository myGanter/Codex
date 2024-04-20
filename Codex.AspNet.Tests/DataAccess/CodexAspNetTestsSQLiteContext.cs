using Microsoft.EntityFrameworkCore;

namespace Codex.AspNet.Tests.DataAccess
{
    internal class CodexAspNetTestsSQLiteContext : DbContext
    {
        public CodexAspNetTestsSQLiteContext(DbContextOptions<CodexAspNetTestsSQLiteContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}

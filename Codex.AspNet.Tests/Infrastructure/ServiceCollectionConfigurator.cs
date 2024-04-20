using Codex.AspNet.Tests.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Codex.AspNet.Tests.Infrastructure
{
    internal static class ServiceCollectionConfigurator
    {
        public static IServiceCollection CreateAndConfigureServiceCollection()
        {
            var services = new ServiceCollection();

            services.AddCodex();

            services.AddDbContext<DbContext, CodexAspNetTestsSQLiteContext>(x => x.UseSqlite("Data Source=.\\CodexAspNetTests.db"));

            return services;
        }

        public static IServiceProvider CreateServiceProvider(this IServiceCollection sc)
        {
            return new DefaultServiceProviderFactory().CreateServiceProvider(sc);
        }
    }
}

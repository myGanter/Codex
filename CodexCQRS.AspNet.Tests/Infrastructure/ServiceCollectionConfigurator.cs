﻿using CodexCQRS.AspNet.EntityFrameworkCore;
using CodexCQRS.AspNet.Tests.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CodexCQRS.AspNet.Tests.Infrastructure
{
    internal static class ServiceCollectionConfigurator
    {
        public static IServiceCollection CreateAndConfigureServiceCollection()
        {
            var services = new ServiceCollection();

            services.AddCodex();
            services.AddCodexEntityFrameworkCore();

            services.AddDbContext<DbContext, CodexAspNetTestsSQLiteContext>(x => x.UseSqlite("Data Source=.\\CodexAspNetTests.db"));

            return services;
        }

        public static IServiceProvider CreateServiceProvider(this IServiceCollection sc)
        {
            return new DefaultServiceProviderFactory().CreateServiceProvider(sc);
        }
    }
}

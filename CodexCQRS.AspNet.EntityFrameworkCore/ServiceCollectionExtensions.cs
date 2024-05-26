using CodexCQRS.AspNet.EntityFrameworkCore.Decorators;
using CodexCQRS.AspNet.EntityFrameworkCore.Services;
using Microsoft.Extensions.DependencyInjection;

namespace CodexCQRS.AspNet.EntityFrameworkCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCodexEntityFrameworkCore(this IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            AddStandardDecorators(services);
            AddServices(services);

            return services;
        }

        private static void AddServices(IServiceCollection services)
        {
            services.AddScoped<TransactionService>();
        }

        private static void AddStandardDecorators(IServiceCollection services)
        {
            services.AddDecorator(typeof(SaveChangesDecorator<>));
            services.AddDecorator(typeof(SaveChangesDecorator<,,>));
            services.AddDecorator(typeof(AsyncSaveChangesDecorator<>));
            services.AddDecorator(typeof(AsyncSaveChangesDecorator<,,>));

            services.AddDecorator(typeof(BeginTransactionDecorator<>));
            services.AddDecorator(typeof(BeginTransactionDecorator<,,>));
            services.AddDecorator(typeof(AsyncBeginTransactionDecorator<>));
            services.AddDecorator(typeof(AsyncBeginTransactionDecorator<,,>));

            services.AddDecorator(typeof(CommitTransactionDecorator<>));
            services.AddDecorator(typeof(CommitTransactionDecorator<,,>));
            services.AddDecorator(typeof(AsyncCommitTransactionDecorator<>));
            services.AddDecorator(typeof(AsyncCommitTransactionDecorator<,,>));
        }
    }
}

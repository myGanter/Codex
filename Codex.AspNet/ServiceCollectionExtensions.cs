using Microsoft.Extensions.DependencyInjection;
using Codex.Dispatcher;
using Codex.AspNet.Infrastructure;
using Codex.Exceptions;
using Codex.CQRS;
using Codex.AspNet.Decorators;
using Codex.AspNet.Services;

namespace Codex.AspNet
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCodex(this IServiceCollection services)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            services.AddScoped<IDiAdapter, MicrosoftDiAdapter>();
            services.AddScoped<IDispatcher, Dispatcher.Dispatcher>();

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

            services.AddDecorator(typeof(ValidationDecorator<>));
            services.AddDecorator(typeof(ValidationDecorator<,>));
            services.AddDecorator(typeof(AsyncValidationDecorator<>));
            services.AddDecorator(typeof(AsyncValidationDecorator<,>));

            services.AddDecorator(typeof(BeginTransactionDecorator<>));
            services.AddDecorator(typeof(BeginTransactionDecorator<,,>));
            services.AddDecorator(typeof(AsyncBeginTransactionDecorator<>));
            services.AddDecorator(typeof(AsyncBeginTransactionDecorator<,,>));

            services.AddDecorator(typeof(CommitTransactionDecorator<>));
            services.AddDecorator(typeof(CommitTransactionDecorator<,,>));
            services.AddDecorator(typeof(AsyncCommitTransactionDecorator<>));
            services.AddDecorator(typeof(AsyncCommitTransactionDecorator<,,>));
        }

        public static IServiceCollection AddDecorator(this IServiceCollection services, Type decoratorType)
        {
            if (services is null)
                throw new ArgumentNullException(nameof(services));

            if (decoratorType is null)
                throw new ArgumentNullException(nameof(decoratorType));

            if (decoratorType.IsInterface)
                throw new DecorateInitException("The decorator type should be an implementation, not an interface.");

            var iHandlerType = typeof(IHandler<>);
            var iAsyncHandlerType = typeof(IAsyncHandler<>);
            var iResultHandlerType = typeof(IHandler<,,>);
            var iResultAsyncHandlerType = typeof(IAsyncHandler<,,>);

            var interfaces = decoratorType.GetInterfaces();

            Type? handlerInterface = interfaces.FirstOrDefault(x => (x.Name == iResultAsyncHandlerType.Name && x.Namespace == iResultAsyncHandlerType.Namespace) ||
                (x.Name == iResultHandlerType.Name && x.Namespace == iResultHandlerType.Namespace) ||
                (x.Name == iAsyncHandlerType.Name && x.Namespace == iAsyncHandlerType.Namespace) ||
                (x.Name == iHandlerType.Name && x.Namespace == iHandlerType.Namespace));

            if (handlerInterface is null)
                throw new DecorateInitException("The passed type does not implement any of the handler interfaces.");

            var searchType = typeof(IHandlerDecorator<>).MakeGenericType(handlerInterface);
            var decoratorHandlerInterface = interfaces.FirstOrDefault(x => x == searchType);

            if (decoratorHandlerInterface is null)
                throw new DecorateInitException($"The decorator type must implement {searchType.FullName}.");

            services.AddTransient(decoratorType);

            return services;
        }
    }
}

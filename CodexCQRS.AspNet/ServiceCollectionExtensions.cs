using CodexCQRS.AspNet.Decorators;
using CodexCQRS.AspNet.Infrastructure;
using CodexCQRS.CQRS;
using CodexCQRS.Dispatcher;
using CodexCQRS.Dtos;
using CodexCQRS.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace CodexCQRS.AspNet
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

            return services;
        }

        private static void AddStandardDecorators(IServiceCollection services)
        {
            services.AddDecorator(typeof(ValidationDecorator<>));
            services.AddDecorator(typeof(ValidationDecorator<,>));
            services.AddDecorator(typeof(AsyncValidationDecorator<>));
            services.AddDecorator(typeof(AsyncValidationDecorator<,>));
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


        public static IServiceCollection AddHandler<THandler, TDto>(this IServiceCollection services)
            where THandler : class, IHandler<TDto>
        {
            services.AddTransient<IHandler<TDto>, THandler>();

            return services;
        }

        public static IServiceCollection AddAsyncHandler<THandler, TDto>(this IServiceCollection services)
            where THandler : class, IAsyncHandler<TDto>
        {
            services.AddTransient<IAsyncHandler<TDto>, THandler>();

            return services;
        }

        public static IServiceCollection AddHandler<THandler, TDto, TOut, TError>(this IServiceCollection services)
            where THandler : class, IHandler<TDto, TOut, TError>
            where TError : class
            where TDto : IDtoContract<TOut, TError>
        {
            services.AddTransient<IHandler<TDto, TOut, TError>, THandler>();

            return services;
        }

        public static IServiceCollection AddAsyncHandler<THandler, TDto, TOut, TError>(this IServiceCollection services)
            where THandler : class, IAsyncHandler<TDto, TOut, TError>
            where TError : class
            where TDto : IDtoContract<TOut, TError>
        {
            services.AddTransient<IAsyncHandler<TDto, TOut, TError>, THandler>();

            return services;
        }


        public static IServiceCollection AddHandler<THandler, TInterfaceHandler, TDto>(this IServiceCollection services)
            where THandler : class, IHandler<TDto>, TInterfaceHandler
            where TInterfaceHandler : class, IHasDecoratedHandler<TDto>
        {
            services.AddHandler<THandler, TDto>();

            services.AddTransient<TInterfaceHandler>(sp =>
            {
                var dispatcher = sp.GetRequiredService<IDispatcher>();
                var handler = dispatcher.BuildHandler<TDto>();

                if (handler.SourceHandler is TInterfaceHandler sourceHandler)
                {
                    sourceHandler.DecoratedHandler = handler.DecoratedHandler;

                    return sourceHandler;
                }
                else
                {
                    throw CreateCastTypeException(handler.SourceHandler.GetType(), typeof(TInterfaceHandler));
                }
            });

            return services;
        }

        public static IServiceCollection AddAsyncHandler<THandler, TInterfaceHandler, TDto>(this IServiceCollection services)
            where THandler : class, IAsyncHandler<TDto>, TInterfaceHandler
            where TInterfaceHandler : class, IHasAsyncDecoratedHandler<TDto>
        {
            services.AddAsyncHandler<THandler, TDto>();

            services.AddTransient<TInterfaceHandler>(sp =>
            {
                var dispatcher = sp.GetRequiredService<IDispatcher>();
                var handler = dispatcher.BuildAsyncHandler<TDto>();

                if (handler.SourceHandler is TInterfaceHandler sourceHandler)
                {
                    sourceHandler.DecoratedAsyncHandler = handler.DecoratedHandler;

                    return sourceHandler;
                }
                else
                {
                    throw CreateCastTypeException(handler.SourceHandler.GetType(), typeof(TInterfaceHandler));
                }
            });

            return services;
        }

        public static IServiceCollection AddHandler<THandler, TInterfaceHandler, TDto, TOut, TError>(this IServiceCollection services)
            where THandler : class, IHandler<TDto, TOut, TError>, TInterfaceHandler
            where TError : class
            where TDto : IDtoContract<TOut, TError>
            where TInterfaceHandler : class, IHasDecoratedHandler<TDto, TOut, TError>
        {
            services.AddHandler<THandler, TDto, TOut, TError>();

            services.AddTransient<TInterfaceHandler>(sp =>
            {
                var dispatcher = sp.GetRequiredService<IDispatcher>();
                var handler = dispatcher.BuildResultHandler<TDto, TOut, TError>();

                if (handler.SourceHandler is TInterfaceHandler sourceHandler)
                {
                    sourceHandler.DecoratedResultHandler = handler.DecoratedHandler;

                    return sourceHandler;
                }
                else
                {
                    throw CreateCastTypeException(handler.SourceHandler.GetType(), typeof(TInterfaceHandler));
                }
            });

            return services;
        }

        public static IServiceCollection AddAsyncHandler<THandler, TInterfaceHandler, TDto, TOut, TError>(this IServiceCollection services)
            where THandler : class, IAsyncHandler<TDto, TOut, TError>, TInterfaceHandler
            where TError : class
            where TDto : IDtoContract<TOut, TError>
            where TInterfaceHandler : class, IHasAsyncDecoratedHandler<TDto, TOut, TError>
        {
            services.AddAsyncHandler<THandler, TDto, TOut, TError>();

            services.AddTransient<TInterfaceHandler>(sp =>
            {
                var dispatcher = sp.GetRequiredService<IDispatcher>();
                var handler = dispatcher.BuildAsyncResultHandler<TDto, TOut, TError>();

                if (handler.SourceHandler is TInterfaceHandler sourceHandler)
                {
                    sourceHandler.DecoratedAsyncResultHandler = handler.DecoratedHandler;

                    return sourceHandler;
                }
                else
                {
                    throw CreateCastTypeException(handler.SourceHandler.GetType(), typeof(TInterfaceHandler));
                }
            });

            return services;
        }

        private static DispatchException CreateCastTypeException(Type type1, Type type2)
        {
            return new DispatchException($"Cannot cast type {type1.FullName} to type {type2.FullName}.");
        }
    }
}

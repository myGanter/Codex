using CodexCQRS.AspNet.Decorators;
using CodexCQRS.AspNet.Dtos;
using CodexCQRS.AspNet.Tests.Infrastructure;
using CodexCQRS.Cache;
using CodexCQRS.CQRS;
using Microsoft.Extensions.DependencyInjection;

namespace CodexCQRS.AspNet.Tests.SpecificHandlerInterfaceTest
{
    public class UnitTestSpecificHandlerInterface
    {
        private readonly IServiceCollection _serviceCollection;

        static UnitTestSpecificHandlerInterface()
        {
            DecoratorsPipeLine.FromHandler<InputDto1>()
                .Before<IHandler<InputDto1>, ValidationDecorator<InputDto1>>();

            DecoratorsPipeLine.FromAsyncHandler<InputDto1>()
                .Before<IAsyncHandler<InputDto1>, AsyncValidationDecorator<InputDto1>>();

            DecoratorsPipeLine.FromHandler<InputDto1, OutputDto, ErrorDto>()
                .Before<IHandler<InputDto1, OutputDto, ErrorDto>, ValidationDecorator<InputDto1, OutputDto>>();

            DecoratorsPipeLine.FromAsyncHandler<InputDto1, OutputDto, ErrorDto>()
                .Before<IAsyncHandler<InputDto1, OutputDto, ErrorDto>, AsyncValidationDecorator<InputDto1, OutputDto>>();
        }

        public UnitTestSpecificHandlerInterface()
        {
            _serviceCollection = ServiceCollectionConfigurator.CreateAndConfigureServiceCollection();

            _serviceCollection.AddHandler<Handler<InputDto1>, IConcreteHandler<InputDto1>, InputDto1>();
            _serviceCollection.AddHandler<ResultHandler<InputDto1>, IConcreteResultHandler<InputDto1>, InputDto1, OutputDto, ErrorDto>();
            _serviceCollection.AddAsyncHandler<AsyncHandler<InputDto1>, IConcreteAsyncHandler<InputDto1>, InputDto1>();
            _serviceCollection.AddAsyncHandler<AsyncResultHandler<InputDto1>, IConcreteAsyncResultHandler<InputDto1>, InputDto1, OutputDto, ErrorDto>();

            _serviceCollection.AddHandler<Handler<InputDto2>, IConcreteHandler<InputDto2>, InputDto2>();
            _serviceCollection.AddHandler<ResultHandler<InputDto2>, IConcreteResultHandler<InputDto2>, InputDto2, OutputDto, ErrorDto>();
            _serviceCollection.AddAsyncHandler<AsyncHandler<InputDto2>, IConcreteAsyncHandler<InputDto2>, InputDto2>();
            _serviceCollection.AddAsyncHandler<AsyncResultHandler<InputDto2>, IConcreteAsyncResultHandler<InputDto2>, InputDto2, OutputDto, ErrorDto>();
        }

        [Fact]
        public void CheckConcreteHandler()
        {
            //With decorator
            Wrap<IConcreteHandler<InputDto1>>(_serviceCollection, handler =>
            {
                Assert.NotNull(handler);
                Assert.IsType<Handler<InputDto1>>(handler);
                Assert.IsType<ValidationDecorator<InputDto1>>(handler.DecoratedHandler);
            });

            //Without decorator
            Wrap<IConcreteHandler<InputDto2>>(_serviceCollection, handler =>
            {
                Assert.NotNull(handler);
                Assert.IsType<Handler<InputDto2>>(handler);
                Assert.IsType<Handler<InputDto2>>(handler.DecoratedHandler);
                Assert.Equal(handler, handler.DecoratedHandler);
            });
        }

        [Fact]
        public void CheckConcreteResultHandler()
        {
            //With decorator
            Wrap<IConcreteResultHandler<InputDto1>>(_serviceCollection, handler =>
            {
                Assert.NotNull(handler);
                Assert.IsType<ResultHandler<InputDto1>>(handler);
                Assert.IsType<ValidationDecorator<InputDto1, OutputDto>>(handler.DecoratedResultHandler);
            });

            //Without decorator
            Wrap<IConcreteResultHandler<InputDto2>>(_serviceCollection, handler =>
            {
                Assert.NotNull(handler);
                Assert.IsType<ResultHandler<InputDto2>>(handler);
                Assert.IsType<ResultHandler<InputDto2>>(handler.DecoratedResultHandler);
                Assert.Equal(handler, handler.DecoratedResultHandler);
            });
        }

        [Fact]
        public void CheckConcreteAsyncHandler()
        {
            //With decorator
            Wrap<IConcreteAsyncHandler<InputDto1>>(_serviceCollection, handler =>
            {
                Assert.NotNull(handler);
                Assert.IsType<AsyncHandler<InputDto1>>(handler);
                Assert.IsType<AsyncValidationDecorator<InputDto1>>(handler.DecoratedAsyncHandler);
            });

            //Without decorator
            Wrap<IConcreteAsyncHandler<InputDto2>>(_serviceCollection, handler =>
            {
                Assert.NotNull(handler);
                Assert.IsType<AsyncHandler<InputDto2>>(handler);
                Assert.IsType<AsyncHandler<InputDto2>>(handler.DecoratedAsyncHandler);
                Assert.Equal(handler, handler.DecoratedAsyncHandler);
            });
        }

        [Fact]
        public void CheckConcreteAsyncResultHandler()
        {
            //With decorator
            Wrap<IConcreteAsyncResultHandler<InputDto1>>(_serviceCollection, handler =>
            {
                Assert.NotNull(handler);
                Assert.IsType<AsyncResultHandler<InputDto1>>(handler);
                Assert.IsType<AsyncValidationDecorator<InputDto1, OutputDto>>(handler.DecoratedAsyncResultHandler);
            });

            //Without decorator
            Wrap<IConcreteAsyncResultHandler<InputDto2>>(_serviceCollection, handler =>
            {
                Assert.NotNull(handler);
                Assert.IsType<AsyncResultHandler<InputDto2>>(handler);
                Assert.IsType<AsyncResultHandler<InputDto2>>(handler.DecoratedAsyncResultHandler);
                Assert.Equal(handler, handler.DecoratedAsyncResultHandler);
            });
        }

        private static void Wrap<THandler>(IServiceCollection serviceCollection, Action<THandler?> clbk)
        {
            var sp = serviceCollection.CreateServiceProvider();
            var handler = sp.GetService<THandler>();

            try
            {
                clbk?.Invoke(handler);
            }
            finally
            {
                ((IDisposable)sp).Dispose();
            }
        }
    }
}

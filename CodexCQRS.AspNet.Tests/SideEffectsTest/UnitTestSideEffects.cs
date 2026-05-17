using CodexCQRS.AspNet.Dtos;
using CodexCQRS.AspNet.Tests.Infrastructure;
using CodexCQRS.Cache;
using CodexCQRS.Dispatcher;
using CodexCQRS.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace CodexCQRS.AspNet.Tests.SideEffectsTest
{
    public class UnitTestSideEffects
    {
        private readonly IServiceCollection _serviceCollection;

        public UnitTestSideEffects()
        {
            _serviceCollection = ServiceCollectionConfigurator.CreateAndConfigureServiceCollection();

            _serviceCollection.AddAsyncHandler<AsyncHandler1, IConcreteAsyncHandler1, InputDto, OutputDto, ErrorDto>();
            _serviceCollection.AddDecorator(typeof(AsyncDecorator1));

            _serviceCollection.AddAsyncHandler<AsyncHandler2, IConcreteAsyncHandler2, InputDto, OutputDto, ErrorDto>();
            _serviceCollection.AddDecorator(typeof(AsyncDecorator2));
        }

        static UnitTestSideEffects()
        {
            DecoratorsPipeLine.FromHandlerType(typeof(AsyncHandler1))
                .Before(typeof(AsyncDecorator1));

            DecoratorsPipeLine.FromHandlerType(typeof(AsyncHandler2))
                .Before(typeof(AsyncDecorator2));
        }

        [Fact]
        public void CheckConcreteHandlersWithSharedHandlerInterfaceAndDTO()
        {
            //Exception: Cannot cast type AsyncHandler2 to type IConcreteAsyncHandler1.
            //The error occurs because when registering dependencies, we overwrite the registration of (IAsyncHandler<InputDto, OutputDto, ErrorDto> -> AsyncHandler1) with (IAsyncHandler<InputDto, OutputDto, ErrorDto> -> AsyncHandler2)
            //This happens because both AsyncHandler1 and AsyncHandler2 implement the same interface IAsyncHandler<InputDto, OutputDto, ErrorDto>
            Assert.Throws<DispatchException>(() =>
            {
                Wrap<IConcreteAsyncHandler1>(_serviceCollection, handler =>
                { });
            });            

            Wrap<IConcreteAsyncHandler2>(_serviceCollection, handler =>
            {
                Assert.NotNull(handler);
                Assert.IsType<AsyncHandler2>(handler);
                Assert.IsType<AsyncDecorator2>(handler.DecoratedAsyncResultHandler);
            });
        }

        [Fact]
        public void CheckHandlersThatHaveSharedHandlerInterfaceAndDTO()
        {
            var dto = new InputDto();
            var expected = $"{nameof(AsyncDecorator2)}"
                + $"{nameof(AsyncHandler2)}";

            Wrap<IDispatcher>(_serviceCollection, dispatcher =>
            {
                //Two handler implementations have one common handler interface and dto
                //When registering dependencies, we overrode the registration of AsyncHandler1 with the registration of AsyncHandler2
                //Therefore, when dispatching via a dto, we will always call the implementation and decorators for AsyncHandler2
                dispatcher.DispatchResultAsync<InputDto, OutputDto, ErrorDto>(dto).Wait();

                Assert.Equal(expected, dto.PipeLineLog);
            });
        }

        private static void Wrap<TService>(IServiceCollection serviceCollection, Action<TService?> clbk)
        {
            var sp = serviceCollection.CreateServiceProvider();
            var handler = sp.GetService<TService>();

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

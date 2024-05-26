using CodexCQRS.AspNet.Decorators;
using CodexCQRS.AspNet.Dtos;
using CodexCQRS.AspNet.Tests.Infrastructure;
using CodexCQRS.Cache;
using CodexCQRS.CQRS;
using CodexCQRS.Dispatcher;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace CodexCQRS.AspNet.Tests.ValidationDecoratorsTest
{
    public class UnitTestValidationDecorators
    {
        private readonly IServiceCollection _serviceCollection;

        static UnitTestValidationDecorators()
        {
            DecoratorsPipeLine.FromHandlerType(typeof(Handler1))
                .Before(typeof(ValidationDecorator<>));

            DecoratorsPipeLine.FromHandlerType(typeof(Handler2))
                .After(typeof(ValidationDecorator<>));

            DecoratorsPipeLine.FromHandlerType(typeof(AsyncHandler1))
               .Before(typeof(AsyncValidationDecorator<>));

            DecoratorsPipeLine.FromHandlerType(typeof(AsyncHandler2))
               .After(typeof(AsyncValidationDecorator<>));

            DecoratorsPipeLine.FromHandlerType(typeof(ResultHandler1))
                .Before(typeof(ValidationDecorator<,>));

            DecoratorsPipeLine.FromHandlerType(typeof(ResultHandler2))
                .After(typeof(ValidationDecorator<,>));

            DecoratorsPipeLine.FromHandlerType(typeof(AsyncResultHandler1))
                .Before(typeof(AsyncValidationDecorator<,>));

            DecoratorsPipeLine.FromHandlerType(typeof(AsyncResultHandler2))
                .After(typeof(AsyncValidationDecorator<,>));
        }

        public UnitTestValidationDecorators()
        {
            _serviceCollection = ServiceCollectionConfigurator.CreateAndConfigureServiceCollection();

            _serviceCollection.AddTransient<IHandler<InputDto1>, Handler1>();
            _serviceCollection.AddTransient<IHandler<InputDto2>, Handler2>();
            _serviceCollection.AddTransient<IAsyncHandler<InputDto1>, AsyncHandler1>();
            _serviceCollection.AddTransient<IAsyncHandler<InputDto2>, AsyncHandler2>();
            _serviceCollection.AddTransient<IHandler<InputDto1, OutputDto, ErrorDto>, ResultHandler1>();
            _serviceCollection.AddTransient<IHandler<InputDto2, OutputDto, ErrorDto>, ResultHandler2>();
            _serviceCollection.AddTransient<IAsyncHandler<InputDto1, OutputDto, ErrorDto>, AsyncResultHandler1>();
            _serviceCollection.AddTransient<IAsyncHandler<InputDto2, OutputDto, ErrorDto>, AsyncResultHandler2>();
        }

        [Fact]
        public void CheckSuccessBeforeValidateHandler()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto1() { Name = "Test", Value = "123" };

                dispatcher.Dispatch(dto);
            });
        }

        [Fact]
        public void CheckDeniedBeforeValidateHandler()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto1() { Name = null, Value = "123" };

                Assert.Throws<ValidationException>(() => dispatcher.Dispatch(dto));
            });
        }

        [Fact]
        public void CheckSuccessAfterValidateHandler()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto2() { Name = "Test", Value = "123" };

                dispatcher.Dispatch(dto);
            });
        }

        [Fact]
        public void CheckDeniedAfterValidateHandler()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto2() { Name = null, Value = "123" };

                Assert.Throws<ValidationException>(() => dispatcher.Dispatch(dto));
            });
        }

        [Fact]
        public void CheckDeniedAfterValidateHandlerIfHandlerReturnError()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto2() { Name = "Test", Value = "123", IsReturnError = true };

                Assert.Throws<Exception>(() => dispatcher.Dispatch(dto));
            });
        }

        [Fact]
        public void CheckSuccessBeforeAsyncValidateHandler()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto1() { Name = "Test", Value = "123" };

                dispatcher.DispatchAsync(dto).Wait();
            });
        }

        [Fact]
        public void CheckDeniedBeforeAsyncValidateHandler()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto1() { Name = null, Value = "123" };

                Assert.Throws<AggregateException>(() => dispatcher.DispatchAsync(dto).Wait());
            });
        }

        [Fact]
        public void CheckSuccessAfterAsyncValidateHandler()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto2() { Name = "Test", Value = "123" };

                dispatcher.DispatchAsync(dto).Wait();
            });
        }

        [Fact]
        public void CheckDeniedAfterAsyncValidateHandler()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto2() { Name = null, Value = "123" };

                Assert.Throws<AggregateException>(() => dispatcher.DispatchAsync(dto).Wait());
            });
        }

        [Fact]
        public void CheckDeniedAfterAsyncValidateHandlerIfHandlerReturnError()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto2() { Name = "Test", Value = "123", IsReturnError = true };

                Assert.Throws<AggregateException>(() => dispatcher.DispatchAsync(dto).Wait());
            });
        }

        [Fact]
        public void CheckSuccessBeforeResultValidateHandler()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto1() { Name = "Test", Value = "123" };

                var result = dispatcher.DispatchResult<InputDto1, OutputDto, ErrorDto>(dto);

                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Result);
            });
        }

        [Fact]
        public void CheckDeniedBeforeResultValidateHandler()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto1() { Name = null, Value = "123" };

                var result = dispatcher.DispatchResult<InputDto1, OutputDto, ErrorDto>(dto);

                Assert.False(result.IsSuccess);
                Assert.Equal(ErrorDto.ValidationTextError, result.Error!.Error);
            });
        }

        [Fact]
        public void CheckSuccessAfterResultValidateHandler()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto2() { Name = "Test", Value = "123" };

                var result = dispatcher.DispatchResult<InputDto2, OutputDto, ErrorDto>(dto);

                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Result);
            });
        }

        [Fact]
        public void CheckDeniedAfterResultValidateHandler()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto2() { Name = null, Value = "123" };

                var result = dispatcher.DispatchResult<InputDto2, OutputDto, ErrorDto>(dto);

                Assert.False(result.IsSuccess);
                Assert.Equal(ErrorDto.ValidationTextError, result.Error!.Error);
            });
        }

        [Fact]
        public void CheckDeniedAfterResultValidateHandlerIfHandlerReturnError()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto2() { Name = "Test", Value = "123", IsReturnError = true };

                var result = dispatcher.DispatchResult<InputDto2, OutputDto, ErrorDto>(dto);

                Assert.False(result.IsSuccess);
                Assert.Equal(ErrorDto.TeapotTextError, result.Error!.Error);
            });
        }

        [Fact]
        public void CheckSuccessBeforeAsyncResultValidateHandler()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto1() { Name = "Test", Value = "123" };

                var result = dispatcher.DispatchResultAsync<InputDto1, OutputDto, ErrorDto>(dto).Result;

                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Result);
            });
        }

        [Fact]
        public void CheckDeniedBeforeAsyncResultValidateHandler()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto1() { Name = null, Value = "123" };

                var result = dispatcher.DispatchResultAsync<InputDto1, OutputDto, ErrorDto>(dto).Result;

                Assert.False(result.IsSuccess);
                Assert.Equal(ErrorDto.ValidationTextError, result.Error!.Error);
            });
        }

        [Fact]
        public void CheckSuccessAfterAsyncResultValidateHandler()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto2() { Name = "Test", Value = "123" };

                var result = dispatcher.DispatchResultAsync<InputDto2, OutputDto, ErrorDto>(dto).Result;

                Assert.True(result.IsSuccess);
                Assert.NotNull(result.Result);
            });
        }

        [Fact]
        public void CheckDeniedAfterAsyncResultValidateHandler()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto2() { Name = null, Value = "123" };

                var result = dispatcher.DispatchResultAsync<InputDto2, OutputDto, ErrorDto>(dto).Result;

                Assert.False(result.IsSuccess);
                Assert.Equal(ErrorDto.ValidationTextError, result.Error!.Error);
            });
        }

        [Fact]
        public void CheckDeniedAfterResultAsyncValidateHandlerIfHandlerReturnError()
        {
            Wrap(_serviceCollection, dispatcher =>
            {
                var dto = new InputDto2() { Name = "Test", Value = "123", IsReturnError = true };

                var result = dispatcher.DispatchResultAsync<InputDto2, OutputDto, ErrorDto>(dto).Result;

                Assert.False(result.IsSuccess);
                Assert.Equal(ErrorDto.TeapotTextError, result.Error!.Error);
            });
        }

        private static void Wrap(IServiceCollection serviceCollection, Action<IDispatcher> clbk)
        {
            var sp = serviceCollection.CreateServiceProvider();
            var dispatcher = sp.GetService<IDispatcher>()!;

            try
            {
                clbk?.Invoke(dispatcher);
            }
            finally
            {
                ((IDisposable)sp).Dispose();
            }
        }
    }
}

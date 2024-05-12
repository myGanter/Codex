using Codex.AspNet.Dtos;
using Codex.AspNet.EntityFrameworkCore.Decorators;
using Codex.AspNet.Tests.DataAccess.Models;
using Codex.AspNet.Tests.Infrastructure;
using Codex.Cache;
using Codex.CQRS;
using Codex.Dispatcher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Codex.AspNet.Tests.SaveChangesDecoratorsTest
{
    public class UnitTestSaveChangesDecorators
    {
        private readonly IServiceCollection _serviceCollection;

        static UnitTestSaveChangesDecorators()
        {
            DecoratorsPipeLine.FromHandlerType(typeof(Handler))
                .After(typeof(SaveChangesDecorator<>));

            DecoratorsPipeLine.FromHandlerType(typeof(ResultHandler))
                .After(typeof(SaveChangesDecorator<,,>));

            DecoratorsPipeLine.FromHandlerType(typeof(AsyncHandler))
                .After(typeof(AsyncSaveChangesDecorator<>));

            DecoratorsPipeLine.FromHandlerType(typeof(AsyncResultHandler))
                .After(typeof(AsyncSaveChangesDecorator<,,>));
        }

        public UnitTestSaveChangesDecorators()
        {
            _serviceCollection = ServiceCollectionConfigurator.CreateAndConfigureServiceCollection();

            _serviceCollection.AddTransient<IHandler<InputDto>, Handler>();
            _serviceCollection.AddTransient<IAsyncHandler<InputDto>, AsyncHandler>();
            _serviceCollection.AddTransient<IHandler<InputDto, OutputDto, ErrorDto>, ResultHandler>();
            _serviceCollection.AddTransient<IAsyncHandler<InputDto, OutputDto, ErrorDto>, AsyncResultHandler>();
        }

        [Fact]
        public void CheckSaveModelForHandler()
        {
            var id = Guid.Parse("c6e72250-ba03-4562-9063-a6583e75631d");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<SaveChangesDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<SaveChangesDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto { Id = id };

                dispatcher.Dispatch(dto);

                model = dbContext.Set<SaveChangesDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.NotNull(model);
            });
        }

        [Fact]
        public void CheckUnSaveModelForHandler()
        {
            var id = Guid.Parse("ac33a93a-1bfa-4651-9e43-e443cfeeaf9f");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<SaveChangesDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<SaveChangesDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto { Id = id, IsReturnError = true };

                Assert.Throws<Exception>(() => dispatcher.Dispatch(dto));

                model = dbContext.Set<SaveChangesDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.Null(model);
            });
        }

        [Fact]
        public void CheckSaveModelForResultHandler()
        {
            var id = Guid.Parse("62346cad-9111-407e-85a2-4c56c3f55449");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<SaveChangesDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<SaveChangesDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto { Id = id };

                var result = dispatcher.DispatchResult<InputDto, OutputDto, ErrorDto>(dto);

                model = dbContext.Set<SaveChangesDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.NotNull(model);
            });
        }

        [Fact]
        public void CheckUnSaveModelForResultHandler()
        {
            var id = Guid.Parse("60d79e8a-2df5-4e35-af6d-dab60f96cf40");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<SaveChangesDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<SaveChangesDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto { Id = id, IsReturnError = true };

                var result = dispatcher.DispatchResult<InputDto, OutputDto, ErrorDto>(dto);

                model = dbContext.Set<SaveChangesDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.False(result.IsSuccess);
                Assert.Equal(ErrorDto.TeapotTextError, result.Error!.Error);
                Assert.Null(model);
            });
        }

        [Fact]
        public void CheckSaveModelForAsyncHandler()
        {
            var id = Guid.Parse("3d1aa881-fd9f-4acf-8f45-e3580495c8dc");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<SaveChangesDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<SaveChangesDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto { Id = id };

                dispatcher.DispatchAsync(dto).Wait();

                model = dbContext.Set<SaveChangesDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.NotNull(model);
            });
        }

        [Fact]
        public void CheckUnSaveModelForAsyncHandler()
        {
            var id = Guid.Parse("9ada3f05-013b-48a7-ac83-79f46a85558d");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<SaveChangesDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<SaveChangesDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto { Id = id, IsReturnError = true };

                Assert.Throws<AggregateException>(() => dispatcher.DispatchAsync(dto).Wait());

                model = dbContext.Set<SaveChangesDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.Null(model);
            });
        }

        [Fact]
        public void CheckSaveModelForResultAsyncResultHandler()
        {
            var id = Guid.Parse("c997af94-1bb0-4441-9394-e6dcf146ff54");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<SaveChangesDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<SaveChangesDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto { Id = id };

                var result = dispatcher.DispatchResultAsync<InputDto, OutputDto, ErrorDto>(dto).Result;

                model = dbContext.Set<SaveChangesDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.NotNull(model);
            });
        }

        [Fact]
        public void CheckUnSaveModelForResultAsyncResultHandler()
        {
            var id = Guid.Parse("73cf5c3b-9937-4762-b679-bad1cae7f09c");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<SaveChangesDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<SaveChangesDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto { Id = id, IsReturnError = true };

                var result = dispatcher.DispatchResultAsync<InputDto, OutputDto, ErrorDto>(dto).Result;

                model = dbContext.Set<SaveChangesDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.False(result.IsSuccess);
                Assert.Equal(ErrorDto.TeapotTextError, result.Error!.Error);
                Assert.Null(model);
            });
        }

        private static void Wrap(IServiceCollection serviceCollection, Action<IDispatcher, DbContext> clbk)
        {
            var sp = serviceCollection.CreateServiceProvider();
            var dispatcher = sp.GetService<IDispatcher>()!;
            var dbContext = sp.GetService<DbContext>()!;

            try
            {
                clbk?.Invoke(dispatcher, dbContext);
            }
            finally
            {
                ((IDisposable)sp).Dispose();
            }
        }
    }
}

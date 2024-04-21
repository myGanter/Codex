using Codex.AspNet.Decorators;
using Codex.AspNet.Dtos;
using Codex.AspNet.Tests.DataAccess.Models;
using Codex.AspNet.Tests.Infrastructure;
using Codex.Cache;
using Codex.CQRS;
using Codex.Dispatcher;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Codex.AspNet.Tests.TransactionDecoratorsTest
{
    public class UnitTestTransactionDecorators
    {
        private readonly IServiceCollection _serviceCollection;

        static UnitTestTransactionDecorators()
        {
            DecoratorsPipeLine.FromHandlerType(typeof(Handler1))
                .Before(typeof(BeginTransactionDecorator<>))
                .After(typeof(CommitTransactionDecorator<>));
            DecoratorsPipeLine.FromHandlerType(typeof(AsyncHandler1))
               .Before(typeof(AsyncBeginTransactionDecorator<>))
               .After(typeof(AsyncCommitTransactionDecorator<>));
            DecoratorsPipeLine.FromHandlerType(typeof(ResultHandler1))
                .Before(typeof(BeginTransactionDecorator<,,>))
                .After(typeof(CommitTransactionDecorator<,,>));
            DecoratorsPipeLine.FromHandlerType(typeof(AsyncResultHandler1))
               .Before(typeof(AsyncBeginTransactionDecorator<,,>))
               .After(typeof(AsyncCommitTransactionDecorator<,,>));

            DecoratorsPipeLine.FromHandlerType(typeof(Handler2))
                .Before(typeof(BeginTransactionDecorator<>))
                .Before(typeof(BeginTransactionDecorator<>))
                .After(typeof(CommitTransactionDecorator<>))
                .After(typeof(CommitTransactionDecorator<>));
            DecoratorsPipeLine.FromHandlerType(typeof(AsyncHandler2))
               .Before(typeof(AsyncBeginTransactionDecorator<>))
               .Before(typeof(AsyncBeginTransactionDecorator<>))
               .After(typeof(AsyncCommitTransactionDecorator<>))
               .After(typeof(AsyncCommitTransactionDecorator<>));
            DecoratorsPipeLine.FromHandlerType(typeof(ResultHandler2))
                .Before(typeof(BeginTransactionDecorator<,,>))
                .Before(typeof(BeginTransactionDecorator<,,>))
                .After(typeof(CommitTransactionDecorator<,,>))
                .After(typeof(CommitTransactionDecorator<,,>));
            DecoratorsPipeLine.FromHandlerType(typeof(AsyncResultHandler2))
               .Before(typeof(AsyncBeginTransactionDecorator<,,>))
               .Before(typeof(AsyncBeginTransactionDecorator<,,>))
               .After(typeof(AsyncCommitTransactionDecorator<,,>))
               .After(typeof(AsyncCommitTransactionDecorator<,,>));

            DecoratorsPipeLine.FromHandlerType(typeof(Handler3))
                .Before(typeof(BeginTransactionDecorator<>));
            DecoratorsPipeLine.FromHandlerType(typeof(AsyncHandler3))
               .Before(typeof(AsyncBeginTransactionDecorator<>));
            DecoratorsPipeLine.FromHandlerType(typeof(ResultHandler3))
                .Before(typeof(BeginTransactionDecorator<,,>));
            DecoratorsPipeLine.FromHandlerType(typeof(AsyncResultHandler3))
               .Before(typeof(AsyncBeginTransactionDecorator<,,>));
        }

        public UnitTestTransactionDecorators()
        {
            _serviceCollection = ServiceCollectionConfigurator.CreateAndConfigureServiceCollection();

            _serviceCollection.AddTransient<IHandler<InputDto1>, Handler1>();
            _serviceCollection.AddTransient<IHandler<InputDto2>, Handler2>();
            _serviceCollection.AddTransient<IHandler<InputDto3>, Handler3>();
            _serviceCollection.AddTransient<IAsyncHandler<InputDto1>, AsyncHandler1>();
            _serviceCollection.AddTransient<IAsyncHandler<InputDto2>, AsyncHandler2>();
            _serviceCollection.AddTransient<IAsyncHandler<InputDto3>, AsyncHandler3>();
            _serviceCollection.AddTransient<IHandler<InputDto1, OutputDto, ErrorDto>, ResultHandler1>();
            _serviceCollection.AddTransient<IHandler<InputDto2, OutputDto, ErrorDto>, ResultHandler2>();
            _serviceCollection.AddTransient<IHandler<InputDto3, OutputDto, ErrorDto>, ResultHandler3>();
            _serviceCollection.AddTransient<IAsyncHandler<InputDto1, OutputDto, ErrorDto>, AsyncResultHandler1>();
            _serviceCollection.AddTransient<IAsyncHandler<InputDto2, OutputDto, ErrorDto>, AsyncResultHandler2>();
            _serviceCollection.AddTransient<IAsyncHandler<InputDto3, OutputDto, ErrorDto>, AsyncResultHandler3>();
        }

        [Fact]
        public void CheckSaveModelForHandler1Transaction()
        {
            var id = Guid.Parse("ee68cde3-8fb5-4b45-8735-833df8a53734");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<TransactionDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto1() { Id = id };

                dispatcher.Dispatch(dto);
            });

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.NotNull(model);
            });
        }

        [Fact]
        public void CheckSaveModelForAsyncHandler1Transaction()
        {
            var id = Guid.Parse("768d3a88-3841-4460-a7e9-564434908952");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<TransactionDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto1() { Id = id };

                dispatcher.DispatchAsync(dto).Wait();
            });

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.NotNull(model);
            });
        }

        [Fact]
        public void CheckSaveModelForResultHandler1Transaction()
        {
            var id = Guid.Parse("491eedf8-5207-4c67-82b1-1daf5c339769");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<TransactionDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto1() { Id = id };

                var result = dispatcher.DispatchResult<InputDto1, OutputDto, ErrorDto>(dto);

                Assert.True(result.IsSuccess);
            });

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.NotNull(model);
            });
        }

        [Fact]
        public void CheckSaveModelForAsyncResultHandler1Transaction()
        {
            var id = Guid.Parse("68280431-3994-4238-8520-3981ce2adcf4");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<TransactionDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto1() { Id = id };

                var result = dispatcher.DispatchResultAsync<InputDto1, OutputDto, ErrorDto>(dto).Result;

                Assert.True(result.IsSuccess);
            });

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.NotNull(model);
            });
        }

        [Fact]
        public void CheckSaveModelForHandler2Transaction()
        {
            var id = Guid.Parse("beb08c87-2434-42bb-8446-b156b242393a");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<TransactionDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto2() { Id = id };

                dispatcher.Dispatch(dto);
            });

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.NotNull(model);
            });
        }

        [Fact]
        public void CheckSaveModelForAsyncHandler2Transaction()
        {
            var id = Guid.Parse("f02aebbe-95dd-42db-8081-a63d89c822bf");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<TransactionDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto2() { Id = id };

                dispatcher.DispatchAsync(dto).Wait();
            });

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.NotNull(model);
            });
        }

        [Fact]
        public void CheckSaveModelForResultHandler2Transaction()
        {
            var id = Guid.Parse("62e8a148-6fb5-4a18-923a-fb1981423123");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<TransactionDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto2() { Id = id };

                var result = dispatcher.DispatchResult<InputDto2, OutputDto, ErrorDto>(dto);

                Assert.True(result.IsSuccess);
            });

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.NotNull(model);
            });
        }

        [Fact]
        public void CheckSaveModelForAsyncResultHandler2Transaction()
        {
            var id = Guid.Parse("8be36972-eb29-4703-ab9f-aeed39928eff");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<TransactionDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto2() { Id = id };

                var result = dispatcher.DispatchResultAsync<InputDto2, OutputDto, ErrorDto>(dto).Result;

                Assert.True(result.IsSuccess);
            });

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.NotNull(model);
            });
        }

        [Fact]
        public void CheckUnSaveModelForHandlerNotCommitTransaction()
        {
            var id = Guid.Parse("a860c355-4285-416e-bda8-020f2ce1a6f1");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<TransactionDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto3() { Id = id };

                dispatcher.Dispatch(dto);
            });

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.Null(model);
            });
        }

        [Fact]
        public void CheckUnSaveModelForAsyncHandlerNotCommitTransaction()
        {
            var id = Guid.Parse("c409582b-468d-4586-a7b8-be42076f7269");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<TransactionDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto3() { Id = id };

                dispatcher.DispatchAsync(dto).Wait();
            });

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.Null(model);
            });
        }

        [Fact]
        public void CheckUnSaveModelForResultHandlerNotCommitTransaction()
        {
            var id = Guid.Parse("9729b7b2-78a3-4376-a029-953ddd5a27b0");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<TransactionDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto3() { Id = id };

                var result = dispatcher.DispatchResult<InputDto3, OutputDto, ErrorDto>(dto);

                Assert.True(result.IsSuccess);
            });

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.Null(model);
            });
        }

        [Fact]
        public void CheckUnSaveModelForAsyncResultHandlerNotCommitTransaction()
        {
            var id = Guid.Parse("e84c9135-ffb8-4e49-985f-a0dbe59541da");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<TransactionDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto3() { Id = id };

                var result = dispatcher.DispatchResultAsync<InputDto3, OutputDto, ErrorDto>(dto).Result;

                Assert.True(result.IsSuccess);
            });

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.Null(model);
            });
        }

        [Fact]
        public void CheckUnSaveModelForHandlerIfError()
        {
            var id = Guid.Parse("6b375aa4-d66a-46a0-a274-9858a8f870be");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<TransactionDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto1() { Id = id, IsReturnError = true };

                Assert.Throws<Exception>(() => dispatcher.Dispatch(dto));
            });

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.Null(model);
            });
        }

        [Fact]
        public void CheckUnSaveModelForAsyncHandlerIfError()
        {
            var id = Guid.Parse("7bc2b652-5bf3-4904-9865-16bc3df500b0");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<TransactionDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto1() { Id = id, IsReturnError = true };

                Assert.Throws<AggregateException>(() => dispatcher.DispatchAsync(dto).Wait());
            });

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.Null(model);
            });
        }

        [Fact]
        public void CheckUnSaveModelForResultHandlerIfError()
        {
            var id = Guid.Parse("7ace020d-67eb-4980-94bf-c86ba1671052");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<TransactionDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto1() { Id = id, IsReturnError = true };

                var result = dispatcher.DispatchResult<InputDto1, OutputDto, ErrorDto>(dto);

                Assert.False(result.IsSuccess);
            });

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                Assert.Null(model);
            });
        }

        [Fact]
        public void CheckUnSaveModelForAsyncResultHandlerIfError()
        {
            var id = Guid.Parse("7640bcc2-ec8e-46c5-a632-88cadc324878");

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

                if (model is not null)
                {
                    dbContext.Set<TransactionDecoratorModel>().Remove(model);
                    dbContext.SaveChanges();
                }

                var dto = new InputDto1() { Id = id, IsReturnError = true };

                var result = dispatcher.DispatchResultAsync<InputDto1, OutputDto, ErrorDto>(dto).Result;

                Assert.False(result.IsSuccess);
            });

            Wrap(_serviceCollection, (dispatcher, dbContext) =>
            {
                var model = dbContext.Set<TransactionDecoratorModel>()
                    .FirstOrDefault(x => x.Id == id);

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
                ((IAsyncDisposable)sp).DisposeAsync().AsTask().Wait();
            }
        }
    }
}

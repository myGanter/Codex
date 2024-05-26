using CodexCQRS.Cache;
using CodexCQRS.CQRS;
using CodexCQRS.Exceptions;
using CodexCQRS.Tests.Infrastructure;

namespace CodexCQRS.Tests.DecoratorsPipeLineTest
{
    [TestCaseOrderer(ordererTypeName: "CodexCQRS.Tests.Infrastructure.PriorityOrderer",
        ordererAssemblyName: "CodexCQRS.Tests")]
    public class UnitTestDecoratorsPipeLine
    {
        [Fact]
        public void CheckDynamicArgumentNullExceptionWhenCreatePipeline()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                DecoratorsPipeLine.FromHandlerType(null);
            });

            Assert.Throws<ArgumentNullException>(() =>
            {
                DecoratorsPipeLine.FromHandlerType(typeof(TestHandler<>))
                    .Before(null);
            });
        }

        [Fact]
        public void CheckDynamicInterfaceExceptionWhenCreatePipeline()
        {
            Assert.Throws<DecorateInitException>(() =>
            {
                DecoratorsPipeLine.FromHandlerType(typeof(IHandler<>));
            });
        }

        [Fact]
        public void CheckDynamicGenericExceptionWhenCreatePipeline()
        {
            Assert.Throws<DecorateInitException>(() =>
            {
                DecoratorsPipeLine.FromHandlerType(typeof(TestHandler<TestDto<int>>));
            });
        }

        [Fact]
        public void CheckDynamicHandlerImplementExceptionWhenCreatePipeline()
        {
            Assert.Throws<DecorateInitException>(() =>
            {
                DecoratorsPipeLine.FromHandlerType(typeof(List<>));
            });
        }

        [Fact]
        public void CheckDynamicInterfaceExceptionWhenCreateAfterBeforePipeline()
        {
            Assert.Throws<DecorateInitException>(() =>
            {
                DecoratorsPipeLine.FromHandlerType(typeof(TestHandler<>))
                    .Before(typeof(IHandlerDecorator<>));

                DecoratorsPipeLine.FromHandlerType(typeof(TestHandler<>))
                    .After(typeof(IHandlerDecorator<>));
            });
        }

        [Fact]
        public void CheckDynamicGenericExceptionWhenCreateAfterBeforePipeline()
        {
            Assert.Throws<DecorateInitException>(() =>
            {
                DecoratorsPipeLine.FromHandlerType(typeof(TestHandler<>))
                    .Before(typeof(IHandlerDecorator<IHandler<TestDto<int>>>));

                DecoratorsPipeLine.FromHandlerType(typeof(TestHandler<>))
                    .After(typeof(IHandlerDecorator<IHandler<TestDto<int>>>));
            });
        }

        [Fact]
        public void CheckDynamicTypeMappingExceptionWhenCreateAfterBeforePipeline()
        {
            Assert.Throws<DecorateInitException>(() =>
            {
                DecoratorsPipeLine.FromHandlerType(typeof(TestHandler<>))
                    .Before(typeof(TestAsyncDecorator<>));

                DecoratorsPipeLine.FromHandlerType(typeof(TestHandler<>))
                    .After(typeof(TestAsyncDecorator<>));
            });
        }

        [Fact]
        public void CheckDynamicGenericMappingExceptionWhenCreateAfterBeforePipeline()
        {
            Assert.Throws<DecorateInitException>(() =>
            {
                DecoratorsPipeLine.FromHandlerType(typeof(TestHandler<>))
                    .Before(typeof(BadTestAsyncDecorator1<,>));
            });
        }

        [Fact]
        public void CheckSuccessCreateVoidHandlerPipeline()
        {
            DecoratorsPipeLine.FromHandler<TestDto<int>>()
                .Before<IHandler<TestDto<int>>, TestDecorator<TestDto<int>>>()
                .Before<IHandler<TestDto<int>>, TestDecorator<TestDto<int>>>()
                .After<IHandler<TestDto<int>>, TestDecorator<TestDto<int>>>()
                .After<IHandler<TestDto<int>>, TestDecorator<TestDto<int>>>();
        }

        [Fact]
        public void CheckSuccessCreateAsyncTaskHandlerPipeline()
        {
            DecoratorsPipeLine.FromAsyncHandler<TestDto<int>>()
                .Before<IAsyncHandler<TestDto<int>>, TestAsyncDecorator<TestDto<int>>>()
                .Before<IAsyncHandler<TestDto<int>>, TestAsyncDecorator<TestDto<int>>>()
                .After<IAsyncHandler<TestDto<int>>, TestAsyncDecorator<TestDto<int>>>()
                .After<IAsyncHandler<TestDto<int>>, TestAsyncDecorator<TestDto<int>>>();
        }

        [Fact]
        public void CheckSuccessCreateResultHandlerPipeline()
        {
            DecoratorsPipeLine.FromHandler<TestDto<int, int>, int, TestErrorResult>()
                .Before<IHandler<TestDto<int, int>, int, TestErrorResult>, TestDecorator<TestDto<int, int>, int, TestErrorResult>>()
                .Before<IHandler<TestDto<int, int>, int, TestErrorResult>, TestDecorator<TestDto<int, int>, int, TestErrorResult>>()
                .After<IHandler<TestDto<int, int>, int, TestErrorResult>, TestDecorator<TestDto<int, int>, int, TestErrorResult>>()
                .After<IHandler<TestDto<int, int>, int, TestErrorResult>, TestDecorator<TestDto<int, int>, int, TestErrorResult>>();
        }

        [Fact]
        public void CheckSuccessAsyncResultHandlerPipeline()
        {
            DecoratorsPipeLine.FromAsyncHandler<TestDto<int, int>, int, TestErrorResult>()
                .Before<IAsyncHandler<TestDto<int, int>, int, TestErrorResult>, TestAsyncDecorator<TestDto<int, int>, int, TestErrorResult>>()
                .Before<IAsyncHandler<TestDto<int, int>, int, TestErrorResult>, TestAsyncDecorator<TestDto<int, int>, int, TestErrorResult>>()
                .After<IAsyncHandler<TestDto<int, int>, int, TestErrorResult>, TestAsyncDecorator<TestDto<int, int>, int, TestErrorResult>>()
                .After<IAsyncHandler<TestDto<int, int>, int, TestErrorResult>, TestAsyncDecorator<TestDto<int, int>, int, TestErrorResult>>();
        }

        [Fact]
        public void CheckSuccessDynamicCreateVoidHandlerPipeline()
        {
            DecoratorsPipeLine.FromHandlerType(typeof(TestHandler<>))
                .After(typeof(TestDecorator<>))
                .After(typeof(TestDecorator<>))
                .Before(typeof(TestDecorator<>))
                .Before(typeof(TestDecorator<>));
        }

        [Fact]
        public void CheckSuccessDynamicCreateAsyncTaskHandlerPipeline()
        {
            DecoratorsPipeLine.FromHandlerType(typeof(TestAsyncHandler<>))
                .After(typeof(TestAsyncDecorator<>))
                .After(typeof(TestAsyncDecorator<>))
                .Before(typeof(TestAsyncDecorator<>))
                .Before(typeof(TestAsyncDecorator<>));
        }

        [Fact]
        public void CheckSuccessDynamicCreateResultHandlerPipeline()
        {
            DecoratorsPipeLine.FromHandlerType(typeof(TestHandler<,,>))
                .After(typeof(TestDecorator<,,>))
                .After(typeof(TestDecorator<,,>))
                .Before(typeof(TestDecorator<,,>))
                .Before(typeof(TestDecorator<,,>));
        }

        [Fact]
        public void CheckSuccessDynamicAsyncResultHandlerPipeline()
        {
            DecoratorsPipeLine.FromHandlerType(typeof(TestAsyncHandler<,,>))
                .After(typeof(TestAsyncDecorator<,,>))
                .After(typeof(TestAsyncDecorator<,,>))
                .Before(typeof(TestAsyncDecorator<,,>))
                .Before(typeof(TestAsyncDecorator<,,>));
        }

        [Fact]
        [TestPriority(1)]
        public void CheckExceptionWhenCreateVoidHandlerPipeline()
        {
            Assert.Throws<DecorateInitException>(() =>
            {
                DecoratorsPipeLine.FromHandler<TestDto<int>>()
                    .Before<IHandler<TestDto<int>>, TestDecorator<TestDto<int>>>()
                    .After<IHandler<TestDto<int>>, TestDecorator<TestDto<int>>>();

                DecoratorsPipeLine.FromHandler<TestDto<int>>()
                    .After<IHandler<TestDto<int>>, TestDecorator<TestDto<int>>>();
            });
        }

        [Fact]
        [TestPriority(1)]
        public void CheckExceptionWhenCreateAsyncTaskHandlerPipeline()
        {
            Assert.Throws<DecorateInitException>(() =>
            {
                DecoratorsPipeLine.FromAsyncHandler<TestDto<int>>()
                    .Before<IAsyncHandler<TestDto<int>>, TestAsyncDecorator<TestDto<int>>>()
                    .After<IAsyncHandler<TestDto<int>>, TestAsyncDecorator<TestDto<int>>>();

                DecoratorsPipeLine.FromAsyncHandler<TestDto<int>>()
                    .After<IAsyncHandler<TestDto<int>>, TestAsyncDecorator<TestDto<int>>>();
            });
        }

        [Fact]
        [TestPriority(1)]
        public void CheckExceptionWhenCreateResultHandlerPipeline()
        {
            Assert.Throws<DecorateInitException>(() =>
            {
                DecoratorsPipeLine.FromHandler<TestDto<int, int>, int, TestErrorResult>()
                    .Before<IHandler<TestDto<int, int>, int, TestErrorResult>, TestDecorator<TestDto<int, int>, int, TestErrorResult>>()
                    .After<IHandler<TestDto<int, int>, int, TestErrorResult>, TestDecorator<TestDto<int, int>, int, TestErrorResult>>();

                DecoratorsPipeLine.FromHandler<TestDto<int, int>, int, TestErrorResult>()
                    .After<IHandler<TestDto<int, int>, int, TestErrorResult>, TestDecorator<TestDto<int, int>, int, TestErrorResult>>();
            });
        }

        [Fact]
        [TestPriority(1)]
        public void CheckExceptionWhenCreateAsyncResultHandlerPipeline()
        {
            Assert.Throws<DecorateInitException>(() =>
            {
                DecoratorsPipeLine.FromAsyncHandler<TestDto<int, int>, int, TestErrorResult>()
                    .Before<IAsyncHandler<TestDto<int, int>, int, TestErrorResult>, TestAsyncDecorator<TestDto<int, int>, int, TestErrorResult>>()
                    .After<IAsyncHandler<TestDto<int, int>, int, TestErrorResult>, TestAsyncDecorator<TestDto<int, int>, int, TestErrorResult>>();

                DecoratorsPipeLine.FromAsyncHandler<TestDto<int, int>, int, TestErrorResult>()
                    .After<IAsyncHandler<TestDto<int, int>, int, TestErrorResult>, TestAsyncDecorator<TestDto<int, int>, int, TestErrorResult>>();
            });
        }

        [Fact]
        [TestPriority(1)]
        public void CheckDynamicExistExceptionWhenCreateVoidHandlerPipeline()
        {
            Assert.Throws<DecorateInitException>(() =>
            {
                DecoratorsPipeLine.FromHandlerType(typeof(TestHandler<>))
                    .Before(typeof(TestDecorator<>));

                DecoratorsPipeLine.FromHandlerType(typeof(TestHandler<>))
                    .Before(typeof(TestDecorator<>));
            });
        }

        [Fact]
        [TestPriority(1)]
        public void CheckDynamicExistExceptionWhenCreateAsyncTaskHandlerPipeline()
        {
            Assert.Throws<DecorateInitException>(() =>
            {
                DecoratorsPipeLine.FromHandlerType(typeof(TestAsyncHandler<>))
                    .Before(typeof(TestAsyncDecorator<>));

                DecoratorsPipeLine.FromHandlerType(typeof(TestAsyncHandler<>))
                    .Before(typeof(TestAsyncDecorator<>));
            });
        }

        [Fact]
        [TestPriority(1)]
        public void CheckDynamicExistExceptionWhenCreateResultHandlerPipeline()
        {
            Assert.Throws<DecorateInitException>(() =>
            {
                DecoratorsPipeLine.FromHandlerType(typeof(TestHandler<,,>))
                    .Before(typeof(TestDecorator<,,>));

                DecoratorsPipeLine.FromHandlerType(typeof(TestHandler<,,>))
                    .Before(typeof(TestDecorator<,,>));
            });
        }

        [Fact]
        [TestPriority(1)]
        public void CheckDynamicExistExceptionWhenCreateAsyncResultHandlerPipeline()
        {
            Assert.Throws<DecorateInitException>(() =>
            {
                DecoratorsPipeLine.FromHandlerType(typeof(TestAsyncHandler<,,>))
                    .Before(typeof(TestAsyncDecorator<,,>));

                DecoratorsPipeLine.FromHandlerType(typeof(TestAsyncHandler<,,>))
                    .Before(typeof(TestAsyncDecorator<,,>));
            });
        }
    }
}

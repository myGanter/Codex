using CodexCQRS.Cache;
using CodexCQRS.CQRS;
using CodexCQRS.Dispatcher;
using CodexCQRS.Exceptions;
using CodexCQRS.Tests.Infrastructure;

namespace CodexCQRS.Tests.DispatcherTest
{
    [TestCaseOrderer(ordererTypeName: "CodexCQRS.Tests.Infrastructure.PriorityOrderer",
        ordererAssemblyName: "CodexCQRS.Tests")]
    public class UnitTestDispatcher
    {
        private static readonly TestDiAdapter _diAdapter;
        private static readonly IDispatcher _dispatcher;

        static UnitTestDispatcher()
        {
            _diAdapter = new TestDiAdapter();
            _dispatcher = new Dispatcher.Dispatcher(_diAdapter);

            _diAdapter
                .From<IHandler<TestDto<uint>>>(() => new TestHandler<TestDto<uint>>())
                .From<IHandler<TestDto<double>>>(() => new TestHandler<TestDto<double>>())
                .From(() => new TestDecorator1<TestDto<uint>>())
                .From(() => new TestDecorator2<TestDto<uint>>())
                .From(() => new TestDecorator1<TestDto<double>>())
                .From(() => new TestDecorator2<TestDto<double>>())

                .From<IAsyncHandler<TestDto<uint>>>(() => new TestAsyncHandler<TestDto<uint>>())
                .From<IAsyncHandler<TestDto<double>>>(() => new TestAsyncHandler<TestDto<double>>())
                .From(() => new TestAsyncDecorator1<TestDto<uint>>())
                .From(() => new TestAsyncDecorator2<TestDto<uint>>())
                .From(() => new TestAsyncDecorator1<TestDto<double>>())
                .From(() => new TestAsyncDecorator2<TestDto<double>>())

                .From<IHandler<TestDto<uint, uint>, uint, TestErrorResult>>(() => new TestResultHandler<TestDto<uint, uint>, uint>())
                .From<IHandler<TestDto<double, double>, double, TestErrorResult>>(() => new TestResultHandler<TestDto<double, double>, double>())
                .From(() => new TestResultDecorator1<TestDto<uint, uint>, uint>())
                .From(() => new TestResultDecorator2<TestDto<uint, uint>, uint>())
                .From(() => new TestResultDecorator1<TestDto<double, double>, double>())
                .From(() => new TestResultDecorator2<TestDto<double, double>, double>())

                .From<IAsyncHandler<TestDto<uint, uint>, uint, TestErrorResult>>(() => new TestAsyncResultHandler<TestDto<uint, uint>, uint>())
                .From<IAsyncHandler<TestDto<double, double>, double, TestErrorResult>>(() => new TestAsyncResultHandler<TestDto<double, double>, double>())
                .From(() => new TestAsyncResultDecorator1<TestDto<uint, uint>, uint>())
                .From(() => new TestAsyncResultDecorator2<TestDto<uint, uint>, uint>())
                .From(() => new TestAsyncResultDecorator1<TestDto<double, double>, double>())
                .From(() => new TestAsyncResultDecorator2<TestDto<double, double>, double>());


            DecoratorsPipeLine.FromHandler<TestDto<uint>>()
                .Before<IHandler<TestDto<uint>>, TestDecorator2<TestDto<uint>>>()
                .Before<IHandler<TestDto<uint>>, TestDecorator1<TestDto<uint>>>()
                .Before<IHandler<TestDto<uint>>, TestDecorator2<TestDto<uint>>>()
                .After<IHandler<TestDto<uint>>, TestDecorator1<TestDto<uint>>>()
                .After<IHandler<TestDto<uint>>, TestDecorator2<TestDto<uint>>>()
                .After<IHandler<TestDto<uint>>, TestDecorator2<TestDto<uint>>>();

            DecoratorsPipeLine.FromHandlerType(typeof(TestHandler<>))
                .Before(typeof(TestDecorator1<>))
                .Before(typeof(TestDecorator2<>))
                .Before(typeof(TestDecorator1<>))
                .After(typeof(TestDecorator2<>))
                .After(typeof(TestDecorator2<>))
                .After(typeof(TestDecorator1<>));


            DecoratorsPipeLine.FromAsyncHandler<TestDto<uint>>()
                .Before<IAsyncHandler<TestDto<uint>>, TestAsyncDecorator2<TestDto<uint>>>()
                .Before<IAsyncHandler<TestDto<uint>>, TestAsyncDecorator1<TestDto<uint>>>()
                .Before<IAsyncHandler<TestDto<uint>>, TestAsyncDecorator2<TestDto<uint>>>()
                .After<IAsyncHandler<TestDto<uint>>, TestAsyncDecorator1<TestDto<uint>>>()
                .After<IAsyncHandler<TestDto<uint>>, TestAsyncDecorator2<TestDto<uint>>>()
                .After<IAsyncHandler<TestDto<uint>>, TestAsyncDecorator2<TestDto<uint>>>();

            DecoratorsPipeLine.FromHandlerType(typeof(TestAsyncHandler<>))
                .Before(typeof(TestAsyncDecorator1<>))
                .Before(typeof(TestAsyncDecorator2<>))
                .Before(typeof(TestAsyncDecorator1<>))
                .After(typeof(TestAsyncDecorator2<>))
                .After(typeof(TestAsyncDecorator2<>))
                .After(typeof(TestAsyncDecorator1<>));


            DecoratorsPipeLine.FromHandler<TestDto<uint, uint>, uint, TestErrorResult>()
                .Before<IHandler<TestDto<uint, uint>, uint, TestErrorResult>, TestResultDecorator2<TestDto<uint, uint>, uint>>()
                .Before<IHandler<TestDto<uint, uint>, uint, TestErrorResult>, TestResultDecorator1<TestDto<uint, uint>, uint>>()
                .Before<IHandler<TestDto<uint, uint>, uint, TestErrorResult>, TestResultDecorator2<TestDto<uint, uint>, uint>>()
                .After<IHandler<TestDto<uint, uint>, uint, TestErrorResult>, TestResultDecorator1<TestDto<uint, uint>, uint>>()
                .After<IHandler<TestDto<uint, uint>, uint, TestErrorResult>, TestResultDecorator2<TestDto<uint, uint>, uint>>()
                .After<IHandler<TestDto<uint, uint>, uint, TestErrorResult>, TestResultDecorator2<TestDto<uint, uint>, uint>>();

            DecoratorsPipeLine.FromHandlerType(typeof(TestResultHandler<,>))
                .Before(typeof(TestResultDecorator1<,>))
                .Before(typeof(TestResultDecorator2<,>))
                .Before(typeof(TestResultDecorator1<,>))
                .After(typeof(TestResultDecorator2<,>))
                .After(typeof(TestResultDecorator2<,>))
                .After(typeof(TestResultDecorator1<,>));


            DecoratorsPipeLine.FromAsyncHandler<TestDto<uint, uint>, uint, TestErrorResult>()
                .Before<IAsyncHandler<TestDto<uint, uint>, uint, TestErrorResult>, TestAsyncResultDecorator2<TestDto<uint, uint>, uint>>()
                .Before<IAsyncHandler<TestDto<uint, uint>, uint, TestErrorResult>, TestAsyncResultDecorator1<TestDto<uint, uint>, uint>>()
                .Before<IAsyncHandler<TestDto<uint, uint>, uint, TestErrorResult>, TestAsyncResultDecorator2<TestDto<uint, uint>, uint>>()
                .After<IAsyncHandler<TestDto<uint, uint>, uint, TestErrorResult>, TestAsyncResultDecorator1<TestDto<uint, uint>, uint>>()
                .After<IAsyncHandler<TestDto<uint, uint>, uint, TestErrorResult>, TestAsyncResultDecorator2<TestDto<uint, uint>, uint>>()
                .After<IAsyncHandler<TestDto<uint, uint>, uint, TestErrorResult>, TestAsyncResultDecorator2<TestDto<uint, uint>, uint>>();

            DecoratorsPipeLine.FromHandlerType(typeof(TestAsyncResultHandler<,>))
                .Before(typeof(TestAsyncResultDecorator1<,>))
                .Before(typeof(TestAsyncResultDecorator2<,>))
                .Before(typeof(TestAsyncResultDecorator1<,>))
                .After(typeof(TestAsyncResultDecorator2<,>))
                .After(typeof(TestAsyncResultDecorator2<,>))
                .After(typeof(TestAsyncResultDecorator1<,>));
        }

        [Fact]
        public void CheckDispatch() 
        {
            var dto = new TestDto<uint>();
            var dtoObj = (object)new TestDto<uint>();
            var expected = $"{nameof(TestDecorator<TestDto<uint>>)}2"
                + $"{nameof(TestDecorator<TestDto<uint>>)}1"
                + $"{nameof(TestDecorator<TestDto<uint>>)}2"
                + $"{nameof(TestHandler<TestDto<uint>>)}"
                + $"{nameof(TestDecorator<TestDto<uint>>)}1"
                + $"{nameof(TestDecorator<TestDto<uint>>)}2"
                + $"{nameof(TestDecorator<TestDto<uint>>)}2";

            _dispatcher.Dispatch(dto);
            _dispatcher.Dispatch(dtoObj);

            Assert.Equal(dto.PipeLineLog, expected);
            Assert.Equal(((TestDto<uint>)dtoObj).PipeLineLog, expected);
            Assert.Throws<ArgumentNullException>(() => _dispatcher.Dispatch(null));
            Assert.Throws<DispatchException>(() => _dispatcher.Dispatch(new TestDto<float>()));
        }

        [Fact]
        public void CheckDynamicDispatch()
        {
            var dto = new TestDto<double>();
            var expected = $"{nameof(TestDecorator<TestDto<double>>)}1"
                + $"{nameof(TestDecorator<TestDto<double>>)}2"
                + $"{nameof(TestDecorator<TestDto<double>>)}1"
                + $"{nameof(TestHandler<TestDto<double>>)}"
                + $"{nameof(TestDecorator<TestDto<double>>)}2"
                + $"{nameof(TestDecorator<TestDto<double>>)}2"
                + $"{nameof(TestDecorator<TestDto<double>>)}1";

            _dispatcher.Dispatch(dto);

            Assert.Equal(dto.PipeLineLog, expected);
        }

        [Fact]
        public void CheckDispatchWithoutDecorators()
        {
            var dto = new TestDto<uint>();
            var expected = $"{nameof(TestHandler<TestDto<uint>>)}";

            _dispatcher.Dispatch(dto, false);

            Assert.Equal(dto.PipeLineLog, expected);
        }

        [Fact]
        public void CheckAsyncDispatch()
        {
            var dto = new TestDto<uint>();
            var dtoObj = (object)new TestDto<uint>();
            var expected = $"{nameof(TestAsyncDecorator<TestDto<uint>>)}2"
                + $"{nameof(TestAsyncDecorator<TestDto<uint>>)}1"
                + $"{nameof(TestAsyncDecorator<TestDto<uint>>)}2"
                + $"{nameof(TestAsyncHandler<TestDto<uint>>)}"
                + $"{nameof(TestAsyncDecorator<TestDto<uint>>)}1"
                + $"{nameof(TestAsyncDecorator<TestDto<uint>>)}2"
                + $"{nameof(TestAsyncDecorator<TestDto<uint>>)}2";

            _dispatcher.DispatchAsync(dto).Wait();
            _dispatcher.DispatchAsync(dtoObj).Wait();

            Assert.Equal(dto.PipeLineLog, expected);
            Assert.Equal(((TestDto<uint>)dtoObj).PipeLineLog, expected);
            Assert.Throws<AggregateException>(() => _dispatcher.DispatchAsync(null).Wait());
            Assert.Throws<AggregateException>(() => _dispatcher.DispatchAsync(new TestDto<float>()).Wait());
        }

        [Fact]
        public void CheckDynamicAsyncDispatch()
        {
            var dto = new TestDto<double>();
            var expected = $"{nameof(TestAsyncDecorator<TestDto<double>>)}1"
                + $"{nameof(TestAsyncDecorator<TestDto<double>>)}2"
                + $"{nameof(TestAsyncDecorator<TestDto<double>>)}1"
                + $"{nameof(TestAsyncHandler<TestDto<double>>)}"
                + $"{nameof(TestAsyncDecorator<TestDto<double>>)}2"
                + $"{nameof(TestAsyncDecorator<TestDto<double>>)}2"
                + $"{nameof(TestAsyncDecorator<TestDto<double>>)}1";

            _dispatcher.DispatchAsync(dto).Wait();

            Assert.Equal(dto.PipeLineLog, expected);
        }

        [Fact]
        public void CheckAsyncDispatchWithoutDecorators()
        {
            var dto = new TestDto<uint>();
            var expected = $"{nameof(TestAsyncHandler<TestDto<uint>>)}";

            _dispatcher.DispatchAsync(dto, false, default).Wait();

            Assert.Equal(dto.PipeLineLog, expected);
        }

        [Fact]
        public void CheckDispatchResult()
        {
            var dto = new TestDto<uint, uint>();
            var dtoObj = (object)new TestDto<uint, uint>();
            var expected = $"{nameof(TestResultDecorator<TestDto<uint, uint>, uint>)}2"
                + $"{nameof(TestResultDecorator<TestDto<uint, uint>, uint>)}1"
                + $"{nameof(TestResultDecorator<TestDto<uint, uint>, uint>)}2"
                + $"{nameof(TestResultHandler<TestDto<uint, uint>, uint>)}"
                + $"{nameof(TestResultDecorator<TestDto<uint, uint>, uint>)}1"
                + $"{nameof(TestResultDecorator<TestDto<uint, uint>, uint>)}2"
                + $"{nameof(TestResultDecorator<TestDto<uint, uint>, uint>)}2";

            _dispatcher.DispatchResult<TestDto<uint, uint>, uint, TestErrorResult>(dto);
            _dispatcher.DispatchResult(dtoObj);

            Assert.Equal(dto.PipeLineLog, expected);
            Assert.Equal(((TestDto<uint>)dtoObj).PipeLineLog, expected);
            Assert.Throws<ArgumentNullException>(() => _dispatcher.DispatchResult(null));
            Assert.Throws<DispatchException>(() => _dispatcher.DispatchResult(new TestDto<float>()));
            Assert.Throws<DispatchException>(() => _dispatcher.DispatchResult(new TestDto<float, float>()));
        }

        [Fact]
        public void CheckDynamicDispatchResult()
        {
            var dto = new TestDto<double, double>();
            var expected = $"{nameof(TestResultDecorator<TestDto<double, double>, double>)}1"
                + $"{nameof(TestResultDecorator<TestDto<double, double>, double>)}2"
                + $"{nameof(TestResultDecorator<TestDto<double, double>, double>)}1"
                + $"{nameof(TestResultHandler<TestDto<double, double>, double>)}"
                + $"{nameof(TestResultDecorator<TestDto<double, double>, double>)}2"
                + $"{nameof(TestResultDecorator<TestDto<double, double>, double>)}2"
                + $"{nameof(TestResultDecorator<TestDto<double, double>, double>)}1";

            _dispatcher.DispatchResult<TestDto<double, double>, double, TestErrorResult>(dto);

            Assert.Equal(dto.PipeLineLog, expected);
        }

        [Fact]
        public void CheckDispatchResultWithoutDecorators()
        {
            var dto = new TestDto<uint, uint>();
            var expected = $"{nameof(TestResultHandler<TestDto<uint, uint>, uint>)}";

            _dispatcher.DispatchResult<TestDto<uint, uint>, uint, TestErrorResult>(dto, false);

            Assert.Equal(dto.PipeLineLog, expected);
        }

        [Fact]
        public void CheckDispatchAsyncResult()
        {
            var dto = new TestDto<uint, uint>();
            var dtoObj = (object)new TestDto<uint, uint>();
            var expected = $"{nameof(TestAsyncResultDecorator<TestDto<uint, uint>, uint>)}2"
                + $"{nameof(TestAsyncResultDecorator<TestDto<uint, uint>, uint>)}1"
                + $"{nameof(TestAsyncResultDecorator<TestDto<uint, uint>, uint>)}2"
                + $"{nameof(TestAsyncResultHandler<TestDto<uint, uint>, uint>)}"
                + $"{nameof(TestAsyncResultDecorator<TestDto<uint, uint>, uint>)}1"
                + $"{nameof(TestAsyncResultDecorator<TestDto<uint, uint>, uint>)}2"
                + $"{nameof(TestAsyncResultDecorator<TestDto<uint, uint>, uint>)}2";

            _dispatcher.DispatchResultAsync<TestDto<uint, uint>, uint, TestErrorResult>(dto).Wait();
            _dispatcher.DispatchResultAsync(dtoObj).Wait();

            Assert.Equal(dto.PipeLineLog, expected);
            Assert.Equal(((TestDto<uint>)dtoObj).PipeLineLog, expected);
            Assert.Throws<AggregateException>(() => _dispatcher.DispatchResultAsync(null).Wait());
            Assert.Throws<AggregateException>(() => _dispatcher.DispatchResultAsync(new TestDto<float>()).Wait());
            Assert.Throws<AggregateException>(() => _dispatcher.DispatchResultAsync(new TestDto<float, float>()).Wait());
        }

        [Fact]
        public void CheckDynamicDispatchAsyncResult()
        {
            var dto = new TestDto<double, double>();
            var expected = $"{nameof(TestAsyncResultDecorator<TestDto<double, double>, double>)}1"
                + $"{nameof(TestAsyncResultDecorator<TestDto<double, double>, double>)}2"
                + $"{nameof(TestAsyncResultDecorator<TestDto<double, double>, double>)}1"
                + $"{nameof(TestAsyncResultHandler<TestDto<double, double>, double>)}"
                + $"{nameof(TestAsyncResultDecorator<TestDto<double, double>, double>)}2"
                + $"{nameof(TestAsyncResultDecorator<TestDto<double, double>, double>)}2"
                + $"{nameof(TestAsyncResultDecorator<TestDto<double, double>, double>)}1";

            _dispatcher.DispatchResultAsync<TestDto<double, double>, double, TestErrorResult>(dto).Wait();

            Assert.Equal(dto.PipeLineLog, expected);
        }

        [Fact]
        public void CheckDispatchAsyncResultWithoutDecorators()
        {
            var dto = new TestDto<uint, uint>();
            var expected = $"{nameof(TestAsyncResultHandler<TestDto<uint, uint>, uint>)}";

            _dispatcher.DispatchResultAsync<TestDto<uint, uint>, uint, TestErrorResult>(dto, false, default).Wait();

            Assert.Equal(dto.PipeLineLog, expected);
        }
    }
}

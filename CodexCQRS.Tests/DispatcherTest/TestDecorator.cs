using CodexCQRS.CQRS;
using CodexCQRS.Dtos;

namespace CodexCQRS.Tests.DispatcherTest
{
    internal class TestDecorator<T> : HandlerDecorator<T>
        where T : TestDto
    {
        private string _text;

        public TestDecorator(string postfix) 
        {
            _text = nameof(TestDecorator<T>) + postfix;
        }

        protected override void DecorateAction(T dto)
        {
            dto.PipeLineLog += _text;
        }
    }

    internal class TestDecorator1<T> : TestDecorator<T>
        where T : TestDto
    {
        public TestDecorator1()
            : base("1")
        { }
    }

    internal class TestDecorator2<T> : TestDecorator<T>
        where T : TestDto
    {
        public TestDecorator2()
            : base("2")
        { }
    }

    internal class TestAsyncDecorator<T> : AsyncHandlerDecorator<T>
        where T : TestDto
    {
        private string _text;

        public TestAsyncDecorator(string postfix)
        {
            _text = nameof(TestAsyncDecorator<T>) + postfix;
        }

        protected override async Task DecorateActionAsync(T dto, CancellationToken token)
        {
            dto.PipeLineLog += _text;

            await Task.CompletedTask;
        }
    }

    internal class TestAsyncDecorator1<T> : TestAsyncDecorator<T>
        where T : TestDto
    {
        public TestAsyncDecorator1()
            : base("1")
        { }
    }

    internal class TestAsyncDecorator2<T> : TestAsyncDecorator<T>
        where T : TestDto
    {
        public TestAsyncDecorator2()
            : base("2")
        { }
    }

    internal class TestResultDecorator<TDto, TOut> : HandlerDecorator<TDto, TOut, TestErrorResult>
        where TDto : TestDto, IDtoContract<TOut, TestErrorResult>
    {
        private string _text;

        public TestResultDecorator(string postfix)
        {
            _text = nameof(TestResultDecorator<TDto, TOut>) + postfix;
        }

        protected override ResultOr<TOut, TestErrorResult> DecorateAction(DecorateInDto<TDto, TOut, TestErrorResult> dto)
        {
            dto.In.PipeLineLog += _text;

            return default(TOut);
        }
    }

    internal class TestResultDecorator1<TDto, TOut> : TestResultDecorator<TDto, TOut>
        where TDto : TestDto, IDtoContract<TOut, TestErrorResult>
    {
        public TestResultDecorator1()
            : base("1")
        { }
    }

    internal class TestResultDecorator2<TDto, TOut> : TestResultDecorator<TDto, TOut>
        where TDto : TestDto, IDtoContract<TOut, TestErrorResult>
    {
        public TestResultDecorator2()
            : base("2")
        { }
    }

    internal class TestAsyncResultDecorator<TDto, TOut> : AsyncHandlerDecorator<TDto, TOut, TestErrorResult>
        where TDto : TestDto, IDtoContract<TOut, TestErrorResult>
    {
        private string _text;

        public TestAsyncResultDecorator(string postfix)
        {
            _text = nameof(TestAsyncResultDecorator<TDto, TOut>) + postfix;
        }

        protected override async Task<ResultOr<TOut, TestErrorResult>> DecorateActionAsync(DecorateInDto<TDto, TOut, TestErrorResult> dto, CancellationToken token)
        {
            dto.In.PipeLineLog += _text;

            return await Task.FromResult(default(TOut));
        }
    }

    internal class TestAsyncResultDecorator1<TDto, TOut> : TestAsyncResultDecorator<TDto, TOut>
        where TDto : TestDto, IDtoContract<TOut, TestErrorResult>
    {
        public TestAsyncResultDecorator1()
            : base("1")
        { }
    }

    internal class TestAsyncResultDecorator2<TDto, TOut> : TestAsyncResultDecorator<TDto, TOut>
        where TDto : TestDto, IDtoContract<TOut, TestErrorResult>
    {
        public TestAsyncResultDecorator2()
            : base("2")
        { }
    }
}

using Codex.CQRS;
using Codex.Dtos;

namespace Codex.Tests.DecoratorsPipeLineTest
{
    internal class BadTestAsyncDecorator1<T1, T2> : AsyncHandlerDecorator<TestDto<int, int>, int, TestErrorResult>
    {
        protected override Task<ResultOr<int, TestErrorResult>> DecorateActionAsync(DecorateInDto<TestDto<int, int>, int, TestErrorResult> dto, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }

    internal class TestDecorator<TDto> : HandlerDecorator<TDto>
    {
        protected override void DecorateAction(TDto dto)
        {
            throw new NotImplementedException();
        }
    }

    internal class TestAsyncDecorator<TDto> : AsyncHandlerDecorator<TDto>
    {
        protected override Task DecorateActionAsync(TDto dto, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }

    internal class TestDecorator<TDto, TOut, TError> : HandlerDecorator<TDto, TOut, TError>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        protected override ResultOr<TOut, TError> DecorateAction(DecorateInDto<TDto, TOut, TError> dto)
        {
            throw new NotImplementedException();
        }
    }

    internal class TestAsyncDecorator<TDto, TOut, TError> : AsyncHandlerDecorator<TDto, TOut, TError>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        protected override Task<ResultOr<TOut, TError>> DecorateActionAsync(DecorateInDto<TDto, TOut, TError> dto, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}

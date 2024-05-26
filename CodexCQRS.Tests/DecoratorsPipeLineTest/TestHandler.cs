using CodexCQRS.CQRS;
using CodexCQRS.Dtos;

namespace CodexCQRS.Tests.DecoratorsPipeLineTest
{
    internal class TestHandler<TDto> : IHandler<TDto>
    {
        public void Handle(TDto dto)
        {
            throw new NotImplementedException();
        }
    }

    internal class TestAsyncHandler<TDto> : IAsyncHandler<TDto>
    {
        public Task HandleAsync(TDto dto, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }
    }

    internal class TestHandler<TDto, TOut, TError> : IHandler<TDto, TOut, TError>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        public ResultOr<TOut, TError> Handle(TDto dto)
        {
            throw new NotImplementedException();
        }
    }

    internal class TestAsyncHandler<TDto, TOut, TError> : IAsyncHandler<TDto, TOut, TError>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        public Task<ResultOr<TOut, TError>> HandleAsync(TDto dto, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}

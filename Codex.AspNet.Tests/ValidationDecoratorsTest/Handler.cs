using Codex.AspNet.Dtos;
using Codex.CQRS;
using Codex.Dtos;

namespace Codex.AspNet.Tests.ValidationDecoratorsTest
{
    internal class Handler1 : Handler<InputDto1>
    { }

    internal class Handler2 : Handler<InputDto2>
    { }

    internal class ResultHandler1 : ResultHandler<InputDto1>
    { }

    internal class ResultHandler2 : ResultHandler<InputDto2>
    { }

    internal class AsyncHandler1 : AsyncHandler<InputDto1>
    { }

    internal class AsyncHandler2 : AsyncHandler<InputDto2>
    { }

    internal class AsyncResultHandler1 : AsyncResultHandler<InputDto1>
    { }

    internal class AsyncResultHandler2 : AsyncResultHandler<InputDto2>
    { }

    abstract class Handler<T> : IHandler<T>
        where T : InputDto
    {
        public void Handle(T dto)
        {
            if (dto.IsReturnError)
                throw new Exception("Handler throw.");
        }
    }

    abstract class ResultHandler<T> : IHandler<T, OutputDto, ErrorDto>
        where T : InputDto
    {
        public ResultOr<OutputDto, ErrorDto> Handle(T dto)
        {
            if (dto.IsReturnError)
                return ErrorDto.TeapotError("ResultHandler throw.");

            return new OutputDto();
        }
    }

    abstract class AsyncHandler<T> : IAsyncHandler<T>
        where T : InputDto
    {
        public async Task HandleAsync(T dto, CancellationToken token = default)
        {
            if (dto.IsReturnError)
                throw new Exception("AsyncHandler throw.");

            await Task.CompletedTask;
        }
    }

    abstract class AsyncResultHandler<T> : IAsyncHandler<T, OutputDto, ErrorDto>
        where T : InputDto
    {
        public async Task<ResultOr<OutputDto, ErrorDto>> HandleAsync(T dto, CancellationToken token = default)
        {
            if (dto.IsReturnError)
                return ErrorDto.TeapotError("AsyncResultHandler throw.");

            return await Task.FromResult(new OutputDto());
        }
    }
}

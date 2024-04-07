using Codex.CQRS;
using Codex.Dtos;

namespace Codex.Tests.DispatcherTest
{
    internal class TestHandler<TDto> : IHandler<TDto>
        where TDto : TestDto
    {
        public void Handle(TDto dto)
        {
            dto.PipeLineLog += nameof(TestHandler<TDto>);
        }
    }

    internal class TestAsyncHandler<TDto> : IAsyncHandler<TDto>
        where TDto : TestDto
    {
        public async Task HandleAsync(TDto dto, CancellationToken token = default)
        {
            dto.PipeLineLog += nameof(TestAsyncHandler<TDto>);

            await Task.CompletedTask;
        }
    }

    internal class TestResultHandler<TDto, TOut> : IHandler<TDto, TOut, TestErrorResult>
         where TDto : TestDto, IDtoContract<TOut, TestErrorResult>
    {
        public ResultOr<TOut, TestErrorResult> Handle(TDto dto)
        {
            dto.PipeLineLog += nameof(TestResultHandler<TDto, TOut>);

            return default(TOut);
        }
    }

    internal class TestAsyncResultHandler<TDto, TOut> : IAsyncHandler<TDto, TOut, TestErrorResult>
        where TDto : TestDto, IDtoContract<TOut, TestErrorResult>
    {
        public async Task<ResultOr<TOut, TestErrorResult>> HandleAsync(TDto dto, CancellationToken token)
        {
            dto.PipeLineLog += nameof(TestAsyncResultHandler<TDto, TOut>);

            return await Task.FromResult(default(TOut));
        }
    }
}

using CodexCQRS.AspNet.Dtos;
using CodexCQRS.CQRS;
using CodexCQRS.Dtos;

namespace CodexCQRS.AspNet.Tests.SideEffectsTest
{
    internal interface IConcreteAsyncHandler1 : IAsyncHandler<InputDto, OutputDto, ErrorDto>,
        IHasAsyncDecoratedHandler<InputDto, OutputDto, ErrorDto>
    { }

    internal class AsyncHandler1 : IConcreteAsyncHandler1
    {
        public IAsyncHandler<InputDto, OutputDto, ErrorDto> DecoratedAsyncResultHandler { get; set; } = null!;

        public async Task<ResultOr<OutputDto, ErrorDto>> HandleAsync(InputDto dto, CancellationToken token = default)
        {
            dto.PipeLineLog += $"{nameof(AsyncHandler1)}";

            return await Task.FromResult(new OutputDto());
        }
    }

    internal class AsyncDecorator1 : AsyncHandlerDecorator<InputDto, OutputDto, ErrorDto>
    {
        protected override async Task<ResultOr<OutputDto, ErrorDto>> DecorateActionAsync(DecorateInDto<InputDto, OutputDto, ErrorDto> dto, CancellationToken token)
        {
            dto.In.PipeLineLog += $"{nameof(AsyncDecorator1)}";

            return await Task.FromResult(dto.Out);
        }
    }

    internal interface IConcreteAsyncHandler2 : IAsyncHandler<InputDto, OutputDto, ErrorDto>,
        IHasAsyncDecoratedHandler<InputDto, OutputDto, ErrorDto>
    { }

    internal class AsyncHandler2 : IConcreteAsyncHandler2
    {
        public IAsyncHandler<InputDto, OutputDto, ErrorDto> DecoratedAsyncResultHandler { get; set; } = null!;

        public async Task<ResultOr<OutputDto, ErrorDto>> HandleAsync(InputDto dto, CancellationToken token = default)
        {
            dto.PipeLineLog += $"{nameof(AsyncHandler2)}";

            return await Task.FromResult(new OutputDto());
        }
    }

    internal class AsyncDecorator2 : AsyncHandlerDecorator<InputDto, OutputDto, ErrorDto>
    {
        protected override async Task<ResultOr<OutputDto, ErrorDto>> DecorateActionAsync(DecorateInDto<InputDto, OutputDto, ErrorDto> dto, CancellationToken token)
        {
            dto.In.PipeLineLog += $"{nameof(AsyncDecorator2)}";

            return await Task.FromResult(dto.Out);
        }
    }
}

using CodexCQRS.AspNet.Dtos;
using CodexCQRS.CQRS;
using CodexCQRS.Dtos;

namespace CodexCQRS.AspNet.Tests.SpecificHandlerInterfaceTest
{
    internal interface IConcreteHandler<TInputDto> : IHandler<TInputDto>,
        IHasDecoratedHandler<TInputDto>
        where TInputDto: InputDto
    { }

    internal class Handler<TInputDto> : IConcreteHandler<TInputDto>
        where TInputDto : InputDto
    {
        public IHandler<TInputDto> DecoratedHandler { get; set; } = null!;

        public void Handle(TInputDto dto)
        { }
    }

    internal interface IConcreteAsyncHandler<TInputDto> : IAsyncHandler<TInputDto>,
        IHasAsyncDecoratedHandler<TInputDto>
        where TInputDto : InputDto
    { }

    internal class AsyncHandler<TInputDto> : IConcreteAsyncHandler<TInputDto>
        where TInputDto : InputDto
    {
        public IAsyncHandler<TInputDto> DecoratedAsyncHandler { get; set; } = null!;

        public Task HandleAsync(TInputDto dto, CancellationToken ct)
            => Task.CompletedTask;
    }

    internal interface IConcreteResultHandler<TInputDto> : IHandler<TInputDto, OutputDto, ErrorDto>,
        IHasDecoratedHandler<TInputDto, OutputDto, ErrorDto>
        where TInputDto : InputDto
    { }

    internal class ResultHandler<TInputDto> : IConcreteResultHandler<TInputDto>
        where TInputDto : InputDto
    {
        public IHandler<TInputDto, OutputDto, ErrorDto> DecoratedResultHandler { get; set; } = null!;

        public ResultOr<OutputDto, ErrorDto> Handle(TInputDto dto)
            => new OutputDto();
    }

    internal interface IConcreteAsyncResultHandler<TInputDto> : IAsyncHandler<TInputDto, OutputDto, ErrorDto>,
        IHasAsyncDecoratedHandler<TInputDto, OutputDto, ErrorDto>
        where TInputDto : InputDto
    { }

    internal class AsyncResultHandler<TInputDto> : IConcreteAsyncResultHandler<TInputDto>
        where TInputDto : InputDto
    {
        public IAsyncHandler<TInputDto, OutputDto, ErrorDto> DecoratedAsyncResultHandler { get; set; } = null!;

        public async Task<ResultOr<OutputDto, ErrorDto>> HandleAsync(TInputDto dto, CancellationToken ct)
            => await Task.FromResult(new OutputDto());
    }
}

using Codex.Dtos;

namespace Codex.CQRS
{
    public interface IHandler<in TDto>
    {
        void Handle(TDto dto);
    }

    public interface IAsyncHandler<in TDto>
    {
        Task HandleAsync(TDto dto, CancellationToken token = default);
    }

    public interface IHandler<in TDto, TOut, TError>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        ResultOr<TOut, TError> Handle(TDto dto);
    }

    public interface IAsyncHandler<in TDto, TOut, TError>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        Task<ResultOr<TOut, TError>> HandleAsync(TDto dto, CancellationToken token = default);
    }
}

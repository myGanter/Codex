using CodexCQRS.Dtos;

namespace CodexCQRS.CQRS
{
    public interface IHandler<in TDto>
    {
        public abstract void Handle(TDto dto);
    }

    public interface IAsyncHandler<in TDto>
    {
        public abstract Task HandleAsync(TDto dto, CancellationToken token = default);
    }

    public interface IHandler<in TDto, TOut, TError>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        public abstract ResultOr<TOut, TError> Handle(TDto dto);
    }

    public interface IAsyncHandler<in TDto, TOut, TError>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        public abstract Task<ResultOr<TOut, TError>> HandleAsync(TDto dto, CancellationToken token = default);
    }
}

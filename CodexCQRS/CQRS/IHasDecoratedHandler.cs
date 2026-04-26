using CodexCQRS.Dtos;

namespace CodexCQRS.CQRS
{
    public interface IHasDecoratedHandler<TDto>
    {
        public abstract IHandler<TDto> DecoratedHandler { get; set; }
    }

    public interface IHasAsyncDecoratedHandler<TDto>
    {
        public abstract IAsyncHandler<TDto> DecoratedAsyncHandler { get; set; }
    }

    public interface IHasDecoratedHandler<TDto, TOut, TError>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        public abstract IHandler<TDto, TOut, TError> DecoratedResultHandler { get; set; }
    }

    public interface IHasAsyncDecoratedHandler<TDto, TOut, TError>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        public abstract IAsyncHandler<TDto, TOut, TError> DecoratedAsyncResultHandler { get; set; }
    }
}

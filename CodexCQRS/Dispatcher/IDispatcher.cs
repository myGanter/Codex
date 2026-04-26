using CodexCQRS.CQRS;
using CodexCQRS.Dtos;

namespace CodexCQRS.Dispatcher
{
    public interface IDispatcher
    {
        public abstract void Dispatch<TDto>(TDto dto, bool buildDecorator = true);

        public abstract void Dispatch(object dto, bool buildDecorator = true);

        public abstract (IHandler<TDto> SourceHandler, IHandler<TDto> DecoratedHandler) BuildHandler<TDto>();

        public abstract Task DispatchAsync<TDto>(TDto dto, bool buildDecorator = true, CancellationToken token = default);

        public abstract Task DispatchAsync(object dto, bool buildDecorator = true, CancellationToken token = default);

        public abstract (IAsyncHandler<TDto> SourceHandler, IAsyncHandler<TDto> DecoratedHandler) BuildAsyncHandler<TDto>();

        public abstract ResultOr<TOut, TError> DispatchResult<TDto, TOut, TError>(TDto dto, bool buildDecorator = true)
            where TError : class
            where TDto : IDtoContract<TOut, TError>;

        public abstract object DispatchResult(object dto, bool buildDecorator = true);

        public abstract (IHandler<TDto, TOut, TError> SourceHandler, IHandler<TDto, TOut, TError> DecoratedHandler) BuildResultHandler<TDto, TOut, TError>()
            where TError : class
            where TDto : IDtoContract<TOut, TError>;

        public abstract Task<ResultOr<TOut, TError>> DispatchResultAsync<TDto, TOut, TError>(TDto dto, bool buildDecorator = true, CancellationToken token = default)
            where TError : class
            where TDto : IDtoContract<TOut, TError>;

        public abstract Task<object> DispatchResultAsync(object dto, bool buildDecorator = true, CancellationToken token = default);

        public abstract (IAsyncHandler<TDto, TOut, TError> SourceHandler, IAsyncHandler<TDto, TOut, TError> DecoratedHandler) BuildAsyncResultHandler<TDto, TOut, TError>()
            where TError : class
            where TDto : IDtoContract<TOut, TError>;
    }
}

using CodexCQRS.CQRS;
using CodexCQRS.Dtos;

namespace CodexCQRS.Dispatcher
{
    public interface IDispatcher
    {
        void Dispatch<TDto>(TDto dto, bool buildDecorator = true);

        void Dispatch(object dto, bool buildDecorator = true);

        (IHandler<TDto> SourceHandler, IHandler<TDto> DecoratedHandler) BuildHandler<TDto>();

        Task DispatchAsync<TDto>(TDto dto, bool buildDecorator = true, CancellationToken token = default);

        Task DispatchAsync(object dto, bool buildDecorator = true, CancellationToken token = default);

        (IAsyncHandler<TDto> SourceHandler, IAsyncHandler<TDto> DecoratedHandler) BuildAsyncHandler<TDto>();

        ResultOr<TOut, TError> DispatchResult<TDto, TOut, TError>(TDto dto, bool buildDecorator = true)
            where TError : class
            where TDto : IDtoContract<TOut, TError>;

        object DispatchResult(object dto, bool buildDecorator = true);

        (IHandler<TDto, TOut, TError> SourceHandler, IHandler<TDto, TOut, TError> DecoratedHandler) BuildResultHandler<TDto, TOut, TError>()
            where TError : class
            where TDto : IDtoContract<TOut, TError>;

        Task<ResultOr<TOut, TError>> DispatchResultAsync<TDto, TOut, TError>(TDto dto, bool buildDecorator = true, CancellationToken token = default)
            where TError : class
            where TDto : IDtoContract<TOut, TError>;

        Task<object> DispatchResultAsync(object dto, bool buildDecorator = true, CancellationToken token = default);

        (IAsyncHandler<TDto, TOut, TError> SourceHandler, IAsyncHandler<TDto, TOut, TError> DecoratedHandler) BuildAsyncResultHandler<TDto, TOut, TError>()
            where TError : class
            where TDto : IDtoContract<TOut, TError>;
    }
}

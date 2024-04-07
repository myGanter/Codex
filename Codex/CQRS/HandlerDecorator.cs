using Codex.Dtos;

namespace Codex.CQRS
{
    public abstract class HandlerDecorator<TDto> :
        IHandler<TDto>,
        IHandlerDecorator<IHandler<TDto>>
    {
        bool IHandlerDecorator<IHandler<TDto>>.IsAfter { get; set; }

        IHandler<TDto> IHandlerDecorator<IHandler<TDto>>.DecorateHandler { get; set; }

        public virtual void Handle(TDto dto)
        {
            var decorator = (IHandlerDecorator<IHandler<TDto>>)this;

            if (!decorator.IsAfter)
                DecorateAction(dto);

            decorator.DecorateHandler.Handle(dto);

            if (decorator.IsAfter)
                DecorateAction(dto);
        }

        protected abstract void DecorateAction(TDto dto);
    }

    public abstract class AsyncHandlerDecorator<TDto> : 
        IAsyncHandler<TDto>,
        IHandlerDecorator<IAsyncHandler<TDto>>
    {
        bool IHandlerDecorator<IAsyncHandler<TDto>>.IsAfter { get; set; }

        IAsyncHandler<TDto> IHandlerDecorator<IAsyncHandler<TDto>>.DecorateHandler { get; set; }

        public virtual async Task HandleAsync(TDto dto, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var decorator = (IHandlerDecorator<IAsyncHandler<TDto>>)this;

            if (!decorator.IsAfter)
                await DecorateActionAsync(dto, token);

            await decorator.DecorateHandler.HandleAsync(dto, token);

            if (decorator.IsAfter)
                await DecorateActionAsync(dto, token);
        }

        protected abstract Task DecorateActionAsync(TDto dto, CancellationToken token);
    }

    public abstract class HandlerDecorator<TDto, TOut, TError> :
        IHandler<TDto, TOut, TError>,
        IHandlerDecorator<IHandler<TDto, TOut, TError>>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        bool IHandlerDecorator<IHandler<TDto, TOut, TError>>.IsAfter { get; set; }

        IHandler<TDto, TOut, TError> IHandlerDecorator<IHandler<TDto, TOut, TError>>.DecorateHandler { get; set; }

        public virtual ResultOr<TOut, TError> Handle(TDto dto)
        {
            var decorator = (IHandlerDecorator<IHandler<TDto, TOut, TError>>)this;

            if (!decorator.IsAfter)
            {
                var result = DecorateAction(new DecorateInDto<TDto, TOut, TError>(dto, default));

                if (!result.IsSuccess)
                    return result;
            }

            var handlerResult = decorator.DecorateHandler.Handle(dto);

            if (decorator.IsAfter)
            {
                handlerResult = DecorateAction(new DecorateInDto<TDto, TOut, TError>(dto, handlerResult));
            }

            return handlerResult;
        }

        protected abstract ResultOr<TOut, TError> DecorateAction(DecorateInDto<TDto, TOut, TError> dto);
    }

    public abstract class AsyncHandlerDecorator<TDto, TOut, TError> :
        IAsyncHandler<TDto, TOut, TError>,
        IHandlerDecorator<IAsyncHandler<TDto, TOut, TError>>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        bool IHandlerDecorator<IAsyncHandler<TDto, TOut, TError>>.IsAfter { get; set; }

        IAsyncHandler<TDto, TOut, TError> IHandlerDecorator<IAsyncHandler<TDto, TOut, TError>>.DecorateHandler { get; set; }

        public virtual async Task<ResultOr<TOut, TError>> HandleAsync(TDto dto, CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            var decorator = (IHandlerDecorator<IAsyncHandler<TDto, TOut, TError>>)this;

            if (!decorator.IsAfter)
            {
                var result = await DecorateActionAsync(new DecorateInDto<TDto, TOut, TError>(dto, default), token);

                if (!result.IsSuccess)
                    return result;
            }

            var handlerResult = await decorator.DecorateHandler.HandleAsync(dto, token);

            if (decorator.IsAfter)
            {
                handlerResult = await DecorateActionAsync(new DecorateInDto<TDto, TOut, TError>(dto, handlerResult), token);
            }

            return handlerResult;
        }

        protected abstract Task<ResultOr<TOut, TError>> DecorateActionAsync(DecorateInDto<TDto, TOut, TError> dto, CancellationToken token);
    }
}

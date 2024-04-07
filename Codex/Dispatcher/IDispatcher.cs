﻿using Codex.Dtos;

namespace Codex.Dispatcher
{
    public interface IDispatcher
    {
        void Dispatch<TDto>(TDto dto, bool buildDecorator = true);

        void Dispatch(object dto, bool buildDecorator = true);

        Task DispatchAsync<TDto>(TDto dto, CancellationToken token = default, bool buildDecorator = true);

        Task DispatchAsync(object dto, CancellationToken token = default, bool buildDecorator = true);

        ResultOr<TOut, TError> DispatchResult<TDto, TOut, TError>(TDto dto, bool buildDecorator = true)
            where TError : class
            where TDto : IDtoContract<TOut, TError>;

        object DispatchResult(object dto, bool buildDecorator = true);

        Task<ResultOr<TOut, TError>> DispatchResultAsync<TDto, TOut, TError>(TDto dto, CancellationToken token = default, bool buildDecorator = true)
            where TError : class
            where TDto : IDtoContract<TOut, TError>;

        Task<object> DispatchResultAsync(object dto, CancellationToken token = default, bool buildDecorator = true);
    }
}

namespace Codex.Dtos
{
    public readonly struct ResultOr<TResult, TError>
        where TError : class
    {
        public readonly TResult? Result { get; init; }

        public readonly TError? Error { get; init; }

        public bool IsSuccess { get => Error is null; }

        public ResultOr(TResult value)
        {
            Result = value;
        }

        public ResultOr(TError error) 
        {
            Error = error;
        }

        public static implicit operator ResultOr<TResult, TError>(TResult value) 
            => new ResultOr<TResult, TError>(value);

        public static implicit operator ResultOr<TResult, TError>(TError error)
            => new ResultOr<TResult, TError>(error);

        #region Methods
        public void MatchVoid(Action<TResult?> onSuccess, Action<TError> onError)
        {
            if (IsSuccess)
                onSuccess(Result);
            else
                onError(Error);
        }

        public T Match<T>(Func<TResult?, T> onSuccess, Func<TError, T> onError)
        {
            if (IsSuccess)
                return onSuccess(Result);

            return onError(Error);
        }

        public ResultOr<T, TError> Match<T>(Func<TResult?, ResultOr<T, TError>> onSuccess, Func<TError, ResultOr<T, TError>> onError)
        {
            if (IsSuccess)
                return onSuccess(Result);

            return onError(Error);
        }

        public ResultOr<T, TError> Match<T>(Func<TResult?, ResultOr<T, TError>> onSuccess, Action<TError>? onErrorWithoutResult = null)
        {
            if (IsSuccess)
                return onSuccess(Result);

            onErrorWithoutResult?.Invoke(Error);

            return Error;
        }

        //Async versions

        public Task MatchVoidAsync(Func<TResult?, Task> onSuccessAsync, Func<TError, Task> onErrorAsync)
        {
            if (IsSuccess)
                return onSuccessAsync(Result);
            
            return onErrorAsync(Error);
        }

        public Task<T> MatchAsync<T>(Func<TResult?, Task<T>> onSuccessAsync, Func<TError, Task<T>> onErrorAsync)
        {
            if (IsSuccess)
                return onSuccessAsync(Result);

            return onErrorAsync(Error);
        }

        public Task<ResultOr<T, TError>> MatchAsync<T>(Func<TResult?, Task<ResultOr<T, TError>>> onSuccessAsync, Func<TError, Task<ResultOr<T, TError>>> onErrorAsync)
        {
            if (IsSuccess)
                return onSuccessAsync(Result);

            return onErrorAsync(Error);
        }

        public async Task<ResultOr<T, TError>> MatchAsync<T>(Func<TResult?, Task<ResultOr<T, TError>>> onSuccessAsync, Func<TError, Task>? onErrorWithoutResultAsync = null)
        {
            if (IsSuccess)
                return await onSuccessAsync(Result);

            if (onErrorWithoutResultAsync is not null)
                await onErrorWithoutResultAsync.Invoke(Error);

            return Error;
        }
        #endregion
    }
}

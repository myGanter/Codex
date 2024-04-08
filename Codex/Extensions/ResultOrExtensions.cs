using Codex.Dtos;

namespace Codex.Extensions
{
    public static class ResultOrExtensions
    {
        public static async Task MatchVoidAsync<TResult, TError>(this Task<ResultOr<TResult, TError>> target,
            Action<TResult?> onSuccess, Action<TError> onError)
            where TError : class
        {
            (await target).MatchVoid(onSuccess, onError);
        }

        public static async Task<T> MatchAsync<TResult, TError, T>(this Task<ResultOr<TResult, TError>> target,
            Func<TResult?, T> onSuccess, Func<TError, T> onError)
            where TError : class
        {
            return (await target)
                .Match(onSuccess, onError);
        }

        public static async Task<ResultOr<T, TError>> MatchAsync<TResult, TError, T>(this Task<ResultOr<TResult, TError>> target,
            Func<TResult?, ResultOr<T, TError>> onSuccess, Func<TError, ResultOr<T, TError>> onError)
            where TError : class
        {
            return (await target)
                .Match(onSuccess, onError);
        }

        public static async Task<ResultOr<T, TError>> MatchAsync<TResult, TError, T>(this Task<ResultOr<TResult, TError>> target,
            Func<TResult?, ResultOr<T, TError>> onSuccess, Action<TError>? onErrorWithoutResult = null)
            where TError : class
        {
            return (await target)
                .Match(onSuccess, onErrorWithoutResult);
        }


        public static async Task MatchVoidAsync<TResult, TError>(this Task<ResultOr<TResult, TError>> target,
            Func<TResult?, Task> onSuccessAsync, Func<TError, Task> onErrorAsync)
            where TError : class
        {
            await (await target)
                .MatchVoidAsync(onSuccessAsync, onErrorAsync);
        }

        public static async Task<T> MatchAsync<TResult, TError, T>(this Task<ResultOr<TResult, TError>> target, 
            Func<TResult?, Task<T>> onSuccessAsync, Func<TError, Task<T>> onErrorAsync)
            where TError : class
        {
            return await (await target)
                .MatchAsync(onSuccessAsync, onErrorAsync);
        }

        public static async Task<ResultOr<T, TError>> MatchAsync<TResult, TError, T>(this Task<ResultOr<TResult, TError>> target,
            Func<TResult?, Task<ResultOr<T, TError>>> onSuccessAsync, Func<TError, Task<ResultOr<T, TError>>> onErrorAsync)
            where TError : class
        {
            return await (await target)
                .MatchAsync(onSuccessAsync, onErrorAsync);
        }

        public static async Task<ResultOr<T, TError>> MatchAsync<TResult, TError, T>(this Task<ResultOr<TResult, TError>> target, 
            Func<TResult?, Task<ResultOr<T, TError>>> onSuccessAsync, Func<TError, Task>? onErrorWithoutResultAsync = null)
            where TError : class
        {
            return await (await target)
                .MatchAsync(onSuccessAsync, onErrorWithoutResultAsync);
        }
    }
}

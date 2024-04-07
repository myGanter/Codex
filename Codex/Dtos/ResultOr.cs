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
    }
}

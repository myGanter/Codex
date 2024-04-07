namespace Codex.Dtos
{
    public class ResultOr<TResult, TError>
        where TError : class
    {
        public TResult? Result { get; set; }

        public TError? Error { get; set; }

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

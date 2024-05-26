namespace CodexCQRS.Dtos
{
    public readonly struct DecorateInDto<TDto, TOut, TError>        
        where TError : class
    {
        public readonly TDto In { get; init; }

        public readonly ResultOr<TOut, TError> Out { get; init; }
    
        public DecorateInDto(TDto dto, ResultOr<TOut, TError> result)
        {
            In = dto; 
            Out = result;
        }
    }
}

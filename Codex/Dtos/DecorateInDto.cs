namespace Codex.Dtos
{
    public class DecorateInDto<TDto, TOut, TError>        
        where TError : class
    {
        public TDto In { get; set; }

        public ResultOr<TOut, TError>? Out { get; set; }
    
        public DecorateInDto(TDto dto, ResultOr<TOut, TError>? result)
        {
            In = dto; 
            Out = result;
        }
    }
}

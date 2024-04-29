namespace Codex.Dtos
{
    public interface IDtoContract<in TOut, in TError>
        where TError : class
    { }
}

namespace Codex.CQRS
{
    public interface IHandlerDecorator<THandler>
    {
        bool IsAfter { get; set; }

        THandler DecorateHandler { get; set; }
    }
}

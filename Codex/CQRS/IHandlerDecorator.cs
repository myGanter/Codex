namespace Codex.CQRS
{
    public interface IHandlerDecorator<THandler>
    {
        bool IsAfter { get; internal set; }

        THandler DecorateHandler { get; internal set; }
    }
}

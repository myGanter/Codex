namespace CodexCQRS.CQRS
{
    public interface IHandlerDecorator<THandler>
    {
        public abstract bool IsAfter { get; set; }

        public abstract THandler DecorateHandler { get; set; }
    }
}

namespace CodexCQRS.Dispatcher
{
    public interface IDiAdapter
    {
        public abstract object? Create(Type type);

        public abstract TService? Create<TService>()
            where TService : class;
    }
}

namespace Codex.Dispatcher
{
    public interface IDiAdapter
    {
        object? Create(Type type);

        TService? Create<TService>()
            where TService : class;
    }
}

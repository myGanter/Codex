using CodexCQRS.Dispatcher;

namespace CodexCQRS.Tests.Infrastructure
{
    internal static class TestDiAdapterExt 
    {
        public static TestDiAdapter From<TType>(this TestDiAdapter adapter, Func<TType> factory)
            where TType : class
        {
            if (adapter is null)
                throw new ArgumentNullException(nameof(adapter));

            adapter.Add(factory);

            return adapter;
        }
    }

    internal class TestDiAdapter : IDiAdapter
    {
        private readonly Dictionary<Type, Func<object>> _factoryCache;

        public TestDiAdapter()
        {
            _factoryCache = new Dictionary<Type, Func<object>>();
        }

        public void Add<TType>(Func<TType> factory)
            where TType : class
        {
            if (factory is null)
                throw new ArgumentNullException(nameof(factory));

            if (_factoryCache.ContainsKey(typeof(TType)))
                throw new Exception($"Type {typeof(TType).FullName} alrady exists.");

            _factoryCache.Add(typeof(TType), factory);
        }

        public object? Create(Type type)
        {
            if (!_factoryCache.ContainsKey(type))
                return null;

            return _factoryCache[type]();
        }

        public TService? Create<TService>()
            where TService : class
        {
            if (!_factoryCache.ContainsKey(typeof(TService)))
                return null;

            return (TService)_factoryCache[typeof(TService)]();
        }
    }
}

using System.Collections.ObjectModel;
using System.Collections.Concurrent;

namespace CodexCQRS.Cache
{
    internal static class StaticDictionaryHashSetCache<TKey, TValue>
        where TKey : notnull
    {
        private static readonly Dictionary<TKey, HashSet<TValue>> _values;

        private static readonly ConcurrentDictionary<TKey, Lazy<ReadOnlyCollection<TValue>>> _readValues;
        
        private static readonly ReadWriteLocker _locker;

        private static readonly ReadOnlyCollection<TValue> _emptyValue;

        static StaticDictionaryHashSetCache()
        {
            _values = new Dictionary<TKey, HashSet<TValue>>();
            _locker = new ReadWriteLocker();
            _readValues = new ConcurrentDictionary<TKey, Lazy<ReadOnlyCollection<TValue>>>();
            _emptyValue = new ReadOnlyCollection<TValue>(new List<TValue>());
        }

        private static void InitReadValues(TKey key)
        {
            _readValues[key] = new Lazy<ReadOnlyCollection<TValue>>(() =>
            {
                using (_locker.ReadLock())
                {
                    return _values[key].ToList().AsReadOnly();
                }
            }, true);
        }

        public static ReadOnlyCollection<TValue> Get(TKey key) 
            => _readValues[key].Value;

        public static bool TryGet(TKey key, out ReadOnlyCollection<TValue> value)
        {
            if (_readValues.TryGetValue(key, out var lazyValue))
            {
                value = lazyValue.Value;
                return true;
            }

            value = _emptyValue;
            return false;
        }

        public static bool ContainsKey(TKey key) 
            => _readValues.ContainsKey(key);

        public static void Add(TKey key, TValue value)
        {
            using (_locker.WriteLock())
            {
                if (!_values.ContainsKey(key))
                {
                    _values.Add(key, new HashSet<TValue>());
                }

                var hs = _values[key];

                if (!hs.Contains(value))
                {
                    hs.Add(value);

                    InitReadValues(key);
                }
            }
        }

        public static void Remove(TKey key)
        {
            using (_locker.WriteLock())
            {
                _values.Remove(key);
                _readValues.Remove(key, out var value);
            }
        }
    }
}

using System.Collections.ObjectModel;

namespace CodexCQRS.Cache
{
    internal static class StaticKeyHashSetCache<TKey, TValue>
    {
        private static readonly HashSet<TValue> _values;

        private static readonly ReadWriteLocker _locker;

        private static Lazy<ReadOnlyCollection<TValue>> _readValues;

        public static ReadOnlyCollection<TValue> Values => _readValues.Value;

        static StaticKeyHashSetCache()
        {
            _values = new HashSet<TValue>();
            _locker = new ReadWriteLocker();

            InitReadValues();
        }

        private static void InitReadValues()
        {
            _readValues = new Lazy<ReadOnlyCollection<TValue>>(() =>
            {
                using (_locker.ReadLock())
                {
                    return _values.ToList().AsReadOnly();
                }
            }, true);
        }

        public static void AddRange(IEnumerable<TValue> values)
        {
            using (_locker.WriteLock())
            {
                foreach (var val in values)
                {
                    if (!_values.Contains(val))
                    {
                        _values.Add(val);                        
                    }
                }

                InitReadValues();
            }
        }

        public static void Add(TValue value) 
        {
            using (_locker.WriteLock())
            {
                if (!_values.Contains(value))
                {
                    _values.Add(value);

                    InitReadValues();
                }                
            }
        }

        public static void Remove(TValue value)
        {
            using (_locker.WriteLock())
            {
                _values.Remove(value);

                InitReadValues();
            }
        }
    }
}

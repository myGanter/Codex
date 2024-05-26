namespace CodexCQRS.Cache
{
    public class ReadWriteLocker : IDisposable
    {
        private class WriteLockToken : IDisposable
        {
            private readonly ReaderWriterLockSlim _locker;

            public WriteLockToken(ReaderWriterLockSlim locker)
            {
                _locker = locker;
                locker.EnterWriteLock();
            }

            public void Dispose() => _locker.ExitWriteLock();
        }

        private class ReadLockToken : IDisposable
        {
            private readonly ReaderWriterLockSlim _locker;

            public ReadLockToken(ReaderWriterLockSlim locker)
            {
                _locker = locker;
                locker.EnterReadLock();
            }

            public void Dispose() => _locker.ExitReadLock();
        }

        private readonly ReaderWriterLockSlim _locker;

        public ReadWriteLocker()
        {
            _locker = new ReaderWriterLockSlim();
        }

        public IDisposable ReadLock() => new ReadLockToken(_locker);
        public IDisposable WriteLock() => new WriteLockToken(_locker);

        public void Dispose() => _locker.Dispose();
    }
}

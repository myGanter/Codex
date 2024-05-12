using Codex.Cache;
using Codex.Dispatcher;
using Codex.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Codex.AspNet.EntityFrameworkCore.Services
{
    internal class TransactionService : IAsyncDisposable
    {
        private class EmptyTransaction : IDbContextTransaction
        {
            public Guid TransactionId => throw new NotImplementedException();

            public void Commit()
            { }

            public Task CommitAsync(CancellationToken cancellationToken = default)
                => Task.CompletedTask;

            public void Rollback()
            { }

            public Task RollbackAsync(CancellationToken cancellationToken = default)
                => Task.CompletedTask;

            public void Dispose()
            { }

            public ValueTask DisposeAsync()
                => ValueTask.CompletedTask;
        }

        private readonly DbContext _context;
        private readonly Stack<IDbContextTransaction> _transactionStore;
        private readonly ReadWriteLocker _locker;

        public TransactionService(DbContext context)
        {
            _context = context;
            _transactionStore = new Stack<IDbContextTransaction>();
            _locker = new ReadWriteLocker();
        }

        public void BeginTransaction()
        {
            using (_locker.WriteLock())
            {
                _transactionStore.Push(_transactionStore.Count == 0 ?
                    _context.Database.BeginTransaction() :
                    new EmptyTransaction());
            }
        }

        public async Task BeginTransactionAsync(CancellationToken token)
        {
            using (_locker.WriteLock())
            {
                _transactionStore.Push(_transactionStore.Count == 0 ?
                    await _context.Database.BeginTransactionAsync(token) :
                    new EmptyTransaction());
            }
        }

        public void CommitTransaction()
        {
            using (_locker.WriteLock())
            {
                if (_transactionStore.Count > 0)
                {
                    var transaction = _transactionStore.Pop();

                    transaction.Commit();

                    transaction.Dispose();
                }
            }
        }

        public async Task CommitTransactionAsync(CancellationToken token)
        {
            using (_locker.WriteLock())
            {
                if (_transactionStore.Count > 0)
                {
                    var transaction = _transactionStore.Pop();

                    await transaction.CommitAsync(token);

                    await transaction.DisposeAsync();
                }
            }
        }

        public async ValueTask DisposeAsync()
        {
            while (_transactionStore.Count > 0)
            {
                var transaction = _transactionStore.Pop();

                await transaction.DisposeAsync();
            }

            _locker.Dispose();
        }

        public static TransactionService CreateService(IDiAdapter diAdapter)
        {
            return diAdapter.Create<TransactionService>() ?? throw new DispatchException("Not all Codex services are registered.");
        }
    }
}

using Codex.Dispatcher;
using Codex.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Codex.AspNet.Services
{
    internal class TransactionService : IAsyncDisposable
    {
        private readonly DbContext _context;
        private readonly Stack<IDbContextTransaction> _transactionStore;

        public TransactionService(DbContext context)
        {
            _context = context;
            _transactionStore = new Stack<IDbContextTransaction>();
        }

        public void BeginTransaction()
        {
            _transactionStore.Push(_context.Database.BeginTransaction());
        }

        public async Task BeginTransactionAsync(CancellationToken token)
        {
            _transactionStore.Push(await _context.Database.BeginTransactionAsync(token));
        }

        public void CommitTransaction()
        {
            if (_transactionStore.Count > 0)
            {
                var transaction = _transactionStore.Pop();

                transaction.Commit();

                transaction.Dispose();
            }
        }

        public async Task CommitTransactionAsync(CancellationToken token)
        {
            if (_transactionStore.Count > 0)
            {
                var transaction = _transactionStore.Pop();

                await transaction.CommitAsync(token);

                await transaction.DisposeAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            while (_transactionStore.Count > 0)
            {
                var transaction = _transactionStore.Pop();

                await transaction.DisposeAsync();
            }
        }

        public static TransactionService CreateService(IDiAdapter diAdapter)
        {
            return diAdapter.Create<TransactionService>() ?? throw new DispatchException("Not all Codex services are registered.");
        }
    }
}

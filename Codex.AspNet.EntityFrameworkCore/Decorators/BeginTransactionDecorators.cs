using Codex.AspNet.EntityFrameworkCore.Services;
using Codex.CQRS;
using Codex.Dispatcher;
using Codex.Dtos;

namespace Codex.AspNet.EntityFrameworkCore.Decorators
{
    public class BeginTransactionDecorator<TDto> : HandlerDecorator<TDto>
    {
        private readonly TransactionService _transactionService;

        public BeginTransactionDecorator(IDiAdapter diAdapter)
        {
            _transactionService = TransactionService.CreateService(diAdapter);
        }

        protected override void DecorateAction(TDto dto)
        {
            _transactionService.BeginTransaction();
        }
    }

    public class AsyncBeginTransactionDecorator<TDto> : AsyncHandlerDecorator<TDto>
    {
        private readonly TransactionService _transactionService;

        public AsyncBeginTransactionDecorator(IDiAdapter diAdapter)
        {
            _transactionService = TransactionService.CreateService(diAdapter);
        }

        protected override Task DecorateActionAsync(TDto dto, CancellationToken token)
        {
            return _transactionService.BeginTransactionAsync(token);
        }
    }

    public class BeginTransactionDecorator<TDto, TOut, TError> : HandlerDecorator<TDto, TOut, TError>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        private readonly TransactionService _transactionService;

        public BeginTransactionDecorator(IDiAdapter diAdapter)
        {
            _transactionService = TransactionService.CreateService(diAdapter);
        }

        protected override ResultOr<TOut, TError> DecorateAction(DecorateInDto<TDto, TOut, TError> dto)
        {
            return dto.Out.Match(x =>
            {
                _transactionService.BeginTransaction();

                return dto.Out;
            });
        }
    }

    public class AsyncBeginTransactionDecorator<TDto, TOut, TError> : AsyncHandlerDecorator<TDto, TOut, TError>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        private readonly TransactionService _transactionService;

        public AsyncBeginTransactionDecorator(IDiAdapter diAdapter)
        {
            _transactionService = TransactionService.CreateService(diAdapter);
        }

        protected override Task<ResultOr<TOut, TError>> DecorateActionAsync(DecorateInDto<TDto, TOut, TError> dto, CancellationToken token)
        {
            return dto.Out.MatchAsync(async x =>
            {
                await _transactionService.BeginTransactionAsync(token);

                return dto.Out;
            });
        }
    }
}

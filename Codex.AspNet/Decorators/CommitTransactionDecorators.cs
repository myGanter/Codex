﻿using Codex.AspNet.Services;
using Codex.CQRS;
using Codex.Dispatcher;
using Codex.Dtos;

namespace Codex.AspNet.Decorators
{
    public class CommitTransactionDecorator<TDto> : HandlerDecorator<TDto>
    {
        private readonly TransactionService _transactionService;

        public CommitTransactionDecorator(IDiAdapter diAdapter)
        {
            _transactionService = TransactionService.CreateService(diAdapter);
        }

        protected override void DecorateAction(TDto dto)
        {
            _transactionService.CommitTransaction();
        }
    }

    public class AsyncCommitTransactionDecorator<TDto> : AsyncHandlerDecorator<TDto>
    {
        private readonly TransactionService _transactionService;

        public AsyncCommitTransactionDecorator(IDiAdapter diAdapter)
        {
            _transactionService = TransactionService.CreateService(diAdapter);
        }

        protected override Task DecorateActionAsync(TDto dto, CancellationToken token)
        {
            return _transactionService.CommitTransactionAsync(token);
        }
    }

    public class CommitTransactionDecorator<TDto, TOut, TError> : HandlerDecorator<TDto, TOut, TError>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        private readonly TransactionService _transactionService;

        public CommitTransactionDecorator(IDiAdapter diAdapter)
        {
            _transactionService = TransactionService.CreateService(diAdapter);
        }

        protected override ResultOr<TOut, TError> DecorateAction(DecorateInDto<TDto, TOut, TError> dto)
        {
            return dto.Out.Match(x =>
            {
                _transactionService.CommitTransaction();

                return dto.Out;
            });
        }
    }

    public class AsyncCommitTransactionDecorator<TDto, TOut, TError> : AsyncHandlerDecorator<TDto, TOut, TError>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        private readonly TransactionService _transactionService;

        public AsyncCommitTransactionDecorator(IDiAdapter diAdapter)
        {
            _transactionService = TransactionService.CreateService(diAdapter);
        }

        protected override Task<ResultOr<TOut, TError>> DecorateActionAsync(DecorateInDto<TDto, TOut, TError> dto, CancellationToken token)
        {
            return dto.Out.MatchAsync(async x =>
            {
                await _transactionService.CommitTransactionAsync(token);

                return dto.Out;
            });
        }
    }
}
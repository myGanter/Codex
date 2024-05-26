using CodexCQRS.CQRS;
using CodexCQRS.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CodexCQRS.AspNet.EntityFrameworkCore.Decorators
{
    public class SaveChangesDecorator<TDto> : HandlerDecorator<TDto>
    {
        private readonly DbContext _context;

        public SaveChangesDecorator(DbContext context)
        {
            _context = context;
        }

        protected override void DecorateAction(TDto dto)
        {
            if (_context.ChangeTracker.HasChanges())
                _context.SaveChanges();
        }
    }

    public class AsyncSaveChangesDecorator<TDto> : AsyncHandlerDecorator<TDto>
    {
        private readonly DbContext _context;

        public AsyncSaveChangesDecorator(DbContext context)
        {
            _context = context;
        }

        protected override async Task DecorateActionAsync(TDto dto, CancellationToken token)
        {
            if (_context.ChangeTracker.HasChanges())
                await _context.SaveChangesAsync(token);
        }
    }

    public class SaveChangesDecorator<TDto, TOut, TError> : HandlerDecorator<TDto, TOut, TError>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        private readonly DbContext _context;

        public SaveChangesDecorator(DbContext context)
        {
            _context = context;
        }

        protected override ResultOr<TOut, TError> DecorateAction(DecorateInDto<TDto, TOut, TError> dto)
        {
            return dto.Out.Match(x =>
            {
                if (_context.ChangeTracker.HasChanges())
                    _context.SaveChanges();

                return dto.Out;
            });
        }
    }

    public class AsyncSaveChangesDecorator<TDto, TOut, TError> : AsyncHandlerDecorator<TDto, TOut, TError>
        where TError : class
        where TDto : IDtoContract<TOut, TError>
    {
        private readonly DbContext _context;

        public AsyncSaveChangesDecorator(DbContext context)
        {
            _context = context;
        }

        protected override async Task<ResultOr<TOut, TError>> DecorateActionAsync(DecorateInDto<TDto, TOut, TError> dto, CancellationToken token)
        {
            return await dto.Out.MatchAsync(async x =>
            {
                if (_context.ChangeTracker.HasChanges())
                    await _context.SaveChangesAsync(token);

                return dto.Out;
            });
        }
    }
}

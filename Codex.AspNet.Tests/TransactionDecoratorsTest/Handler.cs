using Codex.AspNet.Dtos;
using Codex.AspNet.Tests.DataAccess.Models;
using Codex.CQRS;
using Codex.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Codex.AspNet.Tests.TransactionDecoratorsTest
{
    internal class Handler1 : Handler<InputDto1>
    {
        public Handler1(DbContext dbContext) : base(dbContext)
        { }
    }

    internal class Handler2 : Handler<InputDto2>
    {
        public Handler2(DbContext dbContext) : base(dbContext)
        { }
    }

    internal class Handler3 : Handler<InputDto3>
    {
        public Handler3(DbContext dbContext) : base(dbContext)
        { }
    }

    internal class AsyncHandler1 : AsyncHandler<InputDto1>
    {
        public AsyncHandler1(DbContext dbContext) : base(dbContext)
        { }
    }

    internal class AsyncHandler2 : AsyncHandler<InputDto2>
    {
        public AsyncHandler2(DbContext dbContext) : base(dbContext)
        { }
    }

    internal class AsyncHandler3 : AsyncHandler<InputDto3>
    {
        public AsyncHandler3(DbContext dbContext) : base(dbContext)
        { }
    }

    internal class ResultHandler1 : ResultHandler<InputDto1>
    {
        public ResultHandler1(DbContext dbContext) : base(dbContext)
        { }
    }

    internal class ResultHandler2 : ResultHandler<InputDto2>
    {
        public ResultHandler2(DbContext dbContext) : base(dbContext)
        { }
    }

    internal class ResultHandler3 : ResultHandler<InputDto3>
    {
        public ResultHandler3(DbContext dbContext) : base(dbContext)
        { }
    }

    internal class AsyncResultHandler1 : AsyncResultHandler<InputDto1>
    {
        public AsyncResultHandler1(DbContext dbContext) : base(dbContext)
        { }
    }

    internal class AsyncResultHandler2 : AsyncResultHandler<InputDto2>
    {
        public AsyncResultHandler2(DbContext dbContext) : base(dbContext)
        { }
    }

    internal class AsyncResultHandler3 : AsyncResultHandler<InputDto3>
    {
        public AsyncResultHandler3(DbContext dbContext) : base(dbContext)
        { }
    }

    abstract class Handler<T> : IHandler<T>
        where T : InputDto
    {
        private readonly DbContext _dbContext;

        public Handler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Handle(T dto)
        {
            _dbContext.Set<TransactionDecoratorModel>()
                .Add(new TransactionDecoratorModel() { Id = dto.Id });

            _dbContext.SaveChanges();

            if (dto.IsReturnError)
                throw new Exception("Handler throw.");
        }
    }

    abstract class AsyncHandler<T> : IAsyncHandler<T>
        where T : InputDto
    {
        private readonly DbContext _dbContext;

        public AsyncHandler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task HandleAsync(T dto, CancellationToken token = default)
        {
            _dbContext.Set<TransactionDecoratorModel>()
                .Add(new TransactionDecoratorModel() { Id = dto.Id });

            await _dbContext.SaveChangesAsync();

            if (dto.IsReturnError)
                throw new Exception("AsyncHandler throw.");
        }
    }

    abstract class ResultHandler<T> : IHandler<T, OutputDto, ErrorDto>
        where T : InputDto
    {
        private readonly DbContext _dbContext;

        public ResultHandler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultOr<OutputDto, ErrorDto> Handle(T dto)
        {
            _dbContext.Set<TransactionDecoratorModel>()
                .Add(new TransactionDecoratorModel() { Id = dto.Id });

            _dbContext.SaveChanges();

            if (dto.IsReturnError)
                return ErrorDto.TeapotError("ResultHandler throw.");

            return new OutputDto();
        }
    }

    abstract class AsyncResultHandler<T> : IAsyncHandler<T, OutputDto, ErrorDto>
        where T : InputDto
    {
        private readonly DbContext _dbContext;

        public AsyncResultHandler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultOr<OutputDto, ErrorDto>> HandleAsync(T dto, CancellationToken token = default)
        {
            _dbContext.Set<TransactionDecoratorModel>()
                .Add(new TransactionDecoratorModel() { Id = dto.Id });

            await _dbContext.SaveChangesAsync();

            if (dto.IsReturnError)
                return ErrorDto.TeapotError("ResultHandler throw.");

            return new OutputDto();
        }
    }
}

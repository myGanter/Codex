using CodexCQRS.AspNet.Dtos;
using CodexCQRS.AspNet.Tests.DataAccess.Models;
using CodexCQRS.CQRS;
using CodexCQRS.Dtos;
using Microsoft.EntityFrameworkCore;

namespace CodexCQRS.AspNet.Tests.SaveChangesDecoratorsTest
{
    internal class Handler : IHandler<InputDto>
    {
        private readonly DbContext _dbContext;

        public Handler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Handle(InputDto dto)
        {
            _dbContext.Set<SaveChangesDecoratorModel>()
                .Add(new SaveChangesDecoratorModel() { Id = dto.Id });

            if (dto.IsReturnError)
                throw new Exception("Handler throw.");            
        }
    }

    internal class ResultHandler : IHandler<InputDto, OutputDto, ErrorDto>
    {
        private readonly DbContext _dbContext;

        public ResultHandler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public ResultOr<OutputDto, ErrorDto> Handle(InputDto dto)
        {
            _dbContext.Set<SaveChangesDecoratorModel>()
                .Add(new SaveChangesDecoratorModel() { Id = dto.Id });

            if (dto.IsReturnError)
                return ErrorDto.TeapotError("ResultHandler throw.");            

            return new OutputDto();
        }
    }

    internal class AsyncHandler : IAsyncHandler<InputDto>
    {
        private readonly DbContext _dbContext;

        public AsyncHandler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task HandleAsync(InputDto dto, CancellationToken token = default)
        {
            _dbContext.Set<SaveChangesDecoratorModel>()
                .Add(new SaveChangesDecoratorModel() { Id = dto.Id });

            if (dto.IsReturnError)
                throw new Exception("AsyncHandler throw.");            

            await Task.CompletedTask;
        }
    }

    internal class AsyncResultHandler : IAsyncHandler<InputDto, OutputDto, ErrorDto>
    {
        private readonly DbContext _dbContext;

        public AsyncResultHandler(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ResultOr<OutputDto, ErrorDto>> HandleAsync(InputDto dto, CancellationToken token = default)
        {
            _dbContext.Set<SaveChangesDecoratorModel>()
                .Add(new SaveChangesDecoratorModel() { Id = dto.Id });

            if (dto.IsReturnError)
                return ErrorDto.TeapotError("AsyncResultHandler throw.");            

            return await Task.FromResult(new OutputDto());
        }
    }
}

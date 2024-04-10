using Codex.AspNet.Dtos;
using Codex.AspNet.Helpers;
using Codex.CQRS;
using Codex.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Codex.AspNet.Decorators
{
    public class ValidationDecorator<TDto> : HandlerDecorator<TDto>
    {
        protected override void DecorateAction(TDto dto)
        {
            var validationResult = ValidateHalper.GetErrorValidationResultOrNull(dto);

            if (validationResult is not null)
                throw new ValidationException(ValidateHalper.GetDescriptionValidationText(validationResult));
        }
    }

    public class AsyncValidationDecorator<TDto> : AsyncHandlerDecorator<TDto>
    {
        protected override Task DecorateActionAsync(TDto dto, CancellationToken token)
        {
            var validationResult = ValidateHalper.GetErrorValidationResultOrNull(dto);

            if (validationResult is not null)
                throw new ValidationException(ValidateHalper.GetDescriptionValidationText(validationResult));

            return Task.CompletedTask;
        }
    }

    public class ValidationDecorator<TDto, TOut> : HandlerDecorator<TDto, TOut, ErrorDto>
        where TDto : IDtoContract<TOut, ErrorDto>
    {
        protected override ResultOr<TOut, ErrorDto> DecorateAction(DecorateInDto<TDto, TOut, ErrorDto> dto)
        {
            var validationResult = ValidateHalper.GetErrorValidationResultOrNull(dto.In);

            if (validationResult is not null)
                return ErrorDto.MapTo(validationResult);

            return dto.Out;
        }
    }

    public class AsyncValidationDecorator<TDto, TOut> : AsyncHandlerDecorator<TDto, TOut, ErrorDto>
        where TDto : IDtoContract<TOut, ErrorDto>
    {
        protected override Task<ResultOr<TOut, ErrorDto>> DecorateActionAsync(DecorateInDto<TDto, TOut, ErrorDto> dto, CancellationToken token)
        {
            var validationResult = ValidateHalper.GetErrorValidationResultOrNull(dto.In);

            if (validationResult is not null)
                return Task.FromResult(new ResultOr<TOut, ErrorDto>(ErrorDto.MapTo(validationResult)));

            return Task.FromResult(dto.Out);
        }
    }
}

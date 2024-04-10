using Codex.AspNet.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Codex.AspNet.Helpers
{
    public static class ValidateHalper
    {
        public static List<ValidationResult>? GetErrorValidationResultOrNull<TDto>(TDto dto)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var context = new ValidationContext(dto);
            var result = new List<ValidationResult>();

            if (!Validator.TryValidateObject(dto, context, result, true))
                return result;

            return null;
        }

        public static string GetDescriptionValidationText(IEnumerable<ValidationResult> validationResults)
        {
            if (validationResults is null)
                throw new ArgumentNullException(nameof(validationResults));

            return $"{ErrorDto.ValidationTextError}: [{string.Join(", ", validationResults.Where(x => x is not null).Select(x => x.ErrorMessage))}]";
        }
    }
}

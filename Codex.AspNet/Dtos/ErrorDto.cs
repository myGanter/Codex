using System.ComponentModel.DataAnnotations;

namespace Codex.AspNet.Dtos
{
    public class ErrorDto
    {
        public string Error { get; init; }

        public string? Description { get; init; }

        public Dictionary<string, string[]>? MemberErrors { get; init; }

        public ErrorDto(string error, string? description = null, Dictionary<string, string[]>? memberErrors = null)
        {
            Error = error;
            Description = description;
            MemberErrors = memberErrors;
        }

        public const string ValidationTextError = "Validation error";
        public const string NotFoundTextError = "Not found";
        public const string ServerTextError = "Server error";
        public const string ForbiddenTextError = "Forbidden";
        public const string TeapotTextError = "I’m a teapot";

        public static ErrorDto ValidationError(string description) => new ErrorDto(ValidationTextError, description);

        public static ErrorDto NotFoundError(string description) => new ErrorDto(NotFoundTextError, description);

        public static ErrorDto ServerError(string description) => new ErrorDto(ServerTextError, description);

        public static ErrorDto ForbiddenError(string description) => new ErrorDto(ForbiddenTextError, description);

        public static ErrorDto TeapotError(string description) => new ErrorDto(TeapotTextError, description);

        public static ErrorDto MapTo(IEnumerable<ValidationResult>? validationResults) => 
            new ErrorDto(ValidationTextError, "The input Dto has an incorrect value.",
                validationResults?.Where(x => x.MemberNames is not null)
                .SelectMany(x => x.MemberNames, (x, y) => new { Member = y, ErrorText = x.ErrorMessage ?? string.Empty })
                .GroupBy(x => x.Member, x => x.ErrorText)
                .ToDictionary(x => x.Key, x => x.ToArray()));
    }
}

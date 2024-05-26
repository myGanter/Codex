using CodexCQRS.AspNet.Dtos;
using CodexCQRS.Dtos;
using System.ComponentModel.DataAnnotations;

namespace CodexCQRS.AspNet.Tests.ValidationDecoratorsTest
{
    internal class InputDto1 : InputDto
    { }

    internal class InputDto2 : InputDto 
    { }

    abstract class InputDto : IDtoContract<OutputDto, ErrorDto>
    {
        [Required]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"[+-]?([0-9]*[.])?[0-9]+", ErrorMessage = "Value has an incorrect format.")]
        public string Value { get; set; }

        public bool IsReturnError {  get; set; }
    }
}

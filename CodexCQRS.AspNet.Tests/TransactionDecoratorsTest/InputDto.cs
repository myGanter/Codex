using CodexCQRS.AspNet.Dtos;
using CodexCQRS.Dtos;

namespace CodexCQRS.AspNet.Tests.TransactionDecoratorsTest
{
    internal class InputDto1 : InputDto
    {}

    internal class InputDto2 : InputDto
    { }

    internal class InputDto3 : InputDto
    { }

    abstract class InputDto : IDtoContract<OutputDto, ErrorDto>
    {
        public Guid Id { get; set; }

        public bool IsReturnError { get; set; }
    }
}

using CodexCQRS.AspNet.Dtos;
using CodexCQRS.Dtos;

namespace CodexCQRS.AspNet.Tests.SpecificHandlerInterfaceTest
{
    internal class InputDto1 : InputDto
    { }

    internal class InputDto2 : InputDto 
    { }

    internal class InputDto : IDtoContract<OutputDto, ErrorDto>
    { }
}

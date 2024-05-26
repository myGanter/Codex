using CodexCQRS.AspNet.Dtos;
using CodexCQRS.Dtos;

namespace CodexCQRS.AspNet.Tests.SaveChangesDecoratorsTest
{
    internal class InputDto : IDtoContract<OutputDto, ErrorDto>
    {
        public Guid Id { get; set; }

        public bool IsReturnError { get; set; }
    }
}

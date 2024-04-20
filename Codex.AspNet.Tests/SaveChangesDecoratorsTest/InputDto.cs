using Codex.AspNet.Dtos;
using Codex.Dtos;

namespace Codex.AspNet.Tests.SaveChangesDecoratorsTest
{
    internal class InputDto : IDtoContract<OutputDto, ErrorDto>
    {
        public Guid Id { get; set; }

        public bool IsReturnError { get; set; }
    }
}

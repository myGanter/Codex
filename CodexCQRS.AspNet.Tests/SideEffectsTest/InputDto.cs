using CodexCQRS.AspNet.Dtos;
using CodexCQRS.Dtos;

namespace CodexCQRS.AspNet.Tests.SideEffectsTest
{
    internal class InputDto : IDtoContract<OutputDto, ErrorDto>
    { 
        public string PipeLineLog { get; set; }

        public InputDto()
        {
            PipeLineLog = string.Empty;
        }
    }
}

using CodexCQRS.Dtos;

namespace CodexCQRS.Tests.DispatcherTest
{
    internal class TestDto
    {
        public string PipeLineLog { get; set; }

        public TestDto()
        {
            PipeLineLog = string.Empty;
        }
    }

    internal class TestDto<T> : TestDto
    {     
    }

    internal class TestDto<TOut, T> : TestDto<T>, IDtoContract<TOut, TestErrorResult>
    {
        public TestDto()
            : base()
        { }
    }
}

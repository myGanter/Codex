using Codex.Dtos;

namespace Codex.Tests.DecoratorsPipeLineTest
{
    internal class TestDto<T>
    { }

    internal class TestDto<T, TOut> : IDtoContract<TOut, TestErrorResult>
    { }
}

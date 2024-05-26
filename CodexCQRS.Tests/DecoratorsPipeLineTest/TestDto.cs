using CodexCQRS.Dtos;

namespace CodexCQRS.Tests.DecoratorsPipeLineTest
{
    internal class TestDto<T>
    { }

    internal class TestDto<T, TOut> : IDtoContract<TOut, TestErrorResult>
    { }
}

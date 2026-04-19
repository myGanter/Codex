using System.Reflection;
using Xunit.Sdk;
using Xunit.v3;

namespace CodexCQRS.Tests.Infrastructure
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class TestPriorityAttribute : Attribute
    {
        public int Priority { get; private set; }

        public TestPriorityAttribute(int priority) => Priority = priority;
    }

    public class PriorityOrderer : ITestCaseOrderer
    {
        public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases) 
            where TTestCase : notnull, ITestCase
        {
            var sortedMethods = new SortedDictionary<int, List<TTestCase>>();

            foreach (TTestCase testCase in testCases)
            {
                var xUnitTestMethod = testCase.TestMethod as XunitTestMethod;
                if (xUnitTestMethod is null)
                    throw new Exception("TestMethod is not XunitTestMethod!");

                int priority = xUnitTestMethod.Method
                    .GetCustomAttributes(typeof(TestPriorityAttribute))
                    .Cast<TestPriorityAttribute>()
                    .FirstOrDefault()?.Priority ?? 0;

                if (!sortedMethods.TryGetValue(priority, out var list))
                {
                    list = new List<TTestCase>();
                    sortedMethods[priority] = list;
                }

                list.Add(testCase);
            }
            
            return sortedMethods.Keys
                .SelectMany(priority => sortedMethods[priority]
                    .OrderBy(testCase => testCase.TestMethod?.MethodName))
                .ToArray();            
        }         
    }
}

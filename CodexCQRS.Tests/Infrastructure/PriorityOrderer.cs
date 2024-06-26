﻿using Xunit.Abstractions;
using Xunit.Sdk;

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
        public IEnumerable<TTestCase> OrderTestCases<TTestCase>(IEnumerable<TTestCase> testCases) 
            where TTestCase : ITestCase
        {
            string assemblyName = typeof(TestPriorityAttribute).AssemblyQualifiedName!;
            var sortedMethods = new SortedDictionary<int, List<TTestCase>>();

            foreach (TTestCase testCase in testCases)
            {
                int priority = testCase.TestMethod.Method
                    .GetCustomAttributes(assemblyName)
                    .FirstOrDefault()
                    ?.GetNamedArgument<int>(nameof(TestPriorityAttribute.Priority)) ?? 0;

                if (!sortedMethods.TryGetValue(priority, out var list))
                {
                    list = new List<TTestCase>();
                    sortedMethods[priority] = list;
                }

                list.Add(testCase);
            }

            foreach (TTestCase testCase in sortedMethods.Keys
                .SelectMany(priority => sortedMethods[priority]
                    .OrderBy(testCase => testCase.TestMethod.Method.Name)))
            {
                yield return testCase;
            }
        }         
    }
}

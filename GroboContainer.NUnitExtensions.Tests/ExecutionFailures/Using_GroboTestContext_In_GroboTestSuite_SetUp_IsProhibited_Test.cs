using System.Reflection;

using FluentAssertions;

using GroboContainer.NUnitExtensions.Impl.TestContext;

using NUnit.Framework;

namespace GroboContainer.NUnitExtensions.Tests.ExecutionFailures
{
    public class WithGroboTestContext : GroboTestSuiteWrapperAttribute
    {
        public override void SetUp(string suiteName, Assembly testAssembly, IEditableGroboTestContext suiteContext)
        {
            GroboTestContext.Current.SuiteName().Should().Be(suiteName);
        }
    }
    
    [GroboTestSuite, WithGroboTestContext]
    [Explicit("Intentionally fails with 'Unable to get TestContext for test' error")]
    public class Using_GroboTestContext_In_GroboTestSuite_SetUp_IsProhibited_ExplicitTest
    {
        [Test]
        public void Test()
        {
        }
    }

    public class Using_GroboTestContext_In_GroboTestSuite_SetUp_IsProhibited_Test
    {
        [Test]
        public void Test()
        {
            var testResults = TestRunner.RunTestClass<Using_GroboTestContext_In_GroboTestSuite_SetUp_IsProhibited_ExplicitTest>();
            var result = testResults[nameof(Using_GroboTestContext_In_GroboTestSuite_SetUp_IsProhibited_ExplicitTest.Test)];
            result.Message.Should().Contain("Unable to get TestContext for test");
        }
    }
}
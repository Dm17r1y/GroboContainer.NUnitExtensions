using System;
using System.Reflection;

using GroboContainer.NUnitExtensions.Impl.TestContext;

using JetBrains.Annotations;

using NUnit.Framework;

namespace GroboContainer.NUnitExtensions.Tests
{
    public class WithDebugLogPerSuite : EdiTestSuiteWrapperAttribute
    {
        public override sealed void SetUp([NotNull] string suiteName, [NotNull] Assembly testAssembly, [NotNull] IEditableEdiTestContext suiteContext)
        {
            suiteContext.AddItem("SuiteDebugId", Guid.NewGuid());
            suiteContext.AddItem("TestSuiteName", suiteName);
            EdiTestMachineryTrace.Log(string.Format("SuiteWrapper.SetUp() for {0}", suiteName), suiteContext);
        }

        public override sealed void TearDown([NotNull] string suiteName, [NotNull] Assembly testAssembly, [NotNull] IEditableEdiTestContext suiteContext)
        {
            EdiTestMachineryTrace.Log(string.Format("SuiteWrapper.TearDown() for {0}", suiteName), suiteContext);
            Assert.That(suiteName, Is.EqualTo(suiteContext.GetContextItem<string>("TestSuiteName")));
            Assert.That(suiteContext.RemoveItem("TestSuiteName"), Is.True);
        }
    }
}
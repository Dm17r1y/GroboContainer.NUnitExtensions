using System.Reflection;

using GroboContainer.NUnitExtensions.Impl.TestContext;

namespace GroboContainer.NUnitExtensions.Tests.Container
{
    [WithDebugLogPerSuite]
    public class WithServiceDependingOnString : EdiTestSuiteWrapperAttribute
    {
        public WithServiceDependingOnString(string p)
        {
            this.p = p;
        }

        public override void SetUp(string suiteName, Assembly testAssembly, IEditableEdiTestContext suiteContext)
        {
            EdiTestMachineryTrace.Log(string.Format("WithServiceDependingOnString(p={0}).SetUp()", p), suiteContext);
            suiteContext.Container.Configurator.ForAbstraction<IServiceDependingOnString>().UseInstances(new ServiceDependingOnString(p));
        }

        protected override string TryGetIdentity()
        {
            return p;
        }

        private readonly string p;
    }
}
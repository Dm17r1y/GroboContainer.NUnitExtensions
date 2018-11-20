using GroboContainer.Core;

using NUnit.Framework;

using SKBKontur.Catalogue.NUnit.Extensions.CommonWrappers.ForSuite;
using SKBKontur.Catalogue.NUnit.Extensions.EdiTestMachinery;

namespace SKBKontur.Catalogue.Core.Tests.NUnitExtensionTests.EdiTestMachinery.Container
{
    [EdiTestFixture, WithContainerPerSuite]
    public class ServiceDependingOnString_Configuration_ViaTestFixtureSetUpMethod_Test : EdiTestMachineryTestBase
    {
        [EdiTestFixtureSetUp]
        public void TestFixtureSetUp(IContainer container/*, IEdiTestContextData suiteContext*/) // todo [edi-test]: add suiteContext param to TestFixtureSetUp() method
        {
            //EdiTestMachineryTrace.Log(string.Format("TestFixtureSetUp() for {0}", suiteContext.GetItem("TestSuiteName")));
            EdiTestMachineryTrace.Log(string.Format("TestFixtureSetUp() for {0}", "SKBKontur.Catalogue.Core.Tests.NUnitExtensionTests.EdiTestMachinery.Container.ServiceDependingOnString_Configuration_ViaTestFixtureSetUpMethod_Test"));
            container.Configurator.ForAbstraction<IServiceDependingOnString>().UseInstances(new ServiceDependingOnString("2"));
        }

        [EdiSetUp]
        public void SetUp()
        {
            serviceDependingOnString = EdiTestContext.Current.Container.Get<IServiceDependingOnString>();
        }

        [Test]
        public void Test01()
        {
            serviceDependingOnString.Hoo(1);
            AssertEdiTestMachineryTrace(new[]
                {
                    string.Format("SuiteWrapper.SetUp() for {0}", EdiTestContext.Current.SuiteName()),
                    string.Format("TestFixtureSetUp() for {0}", EdiTestContext.Current.SuiteName()),
                    string.Format("MethodWrapper.SetUp() for {0}::{1}", EdiTestContext.Current.SuiteName(), EdiTestContext.Current.TestName()),
                    "ServiceDependingOnString.Hoo(p=2, q=1)",
                });
        }

        [Test]
        public void Test02()
        {
            serviceDependingOnString.Hoo(2);
            AssertEdiTestMachineryTrace(new[]
                {
                    "SuiteWrapper.SetUp() for SKBKontur.Catalogue.Core.Tests.NUnitExtensionTests.EdiTestMachinery.Container.ServiceDependingOnString_Configuration_ViaTestFixtureSetUpMethod_Test",
                    "TestFixtureSetUp() for SKBKontur.Catalogue.Core.Tests.NUnitExtensionTests.EdiTestMachinery.Container.ServiceDependingOnString_Configuration_ViaTestFixtureSetUpMethod_Test",
                    "MethodWrapper.SetUp() for SKBKontur.Catalogue.Core.Tests.NUnitExtensionTests.EdiTestMachinery.Container.ServiceDependingOnString_Configuration_ViaTestFixtureSetUpMethod_Test::SKBKontur.Catalogue.Core.Tests.NUnitExtensionTests.EdiTestMachinery.Container.ServiceDependingOnString_Configuration_ViaTestFixtureSetUpMethod_Test.Test01",
                    "ServiceDependingOnString.Hoo(p=2, q=1)",
                    "MethodWrapper.TearDown() for SKBKontur.Catalogue.Core.Tests.NUnitExtensionTests.EdiTestMachinery.Container.ServiceDependingOnString_Configuration_ViaTestFixtureSetUpMethod_Test::SKBKontur.Catalogue.Core.Tests.NUnitExtensionTests.EdiTestMachinery.Container.ServiceDependingOnString_Configuration_ViaTestFixtureSetUpMethod_Test.Test01",
                    "MethodWrapper.SetUp() for SKBKontur.Catalogue.Core.Tests.NUnitExtensionTests.EdiTestMachinery.Container.ServiceDependingOnString_Configuration_ViaTestFixtureSetUpMethod_Test::SKBKontur.Catalogue.Core.Tests.NUnitExtensionTests.EdiTestMachinery.Container.ServiceDependingOnString_Configuration_ViaTestFixtureSetUpMethod_Test.Test02",
                    "ServiceDependingOnString.Hoo(p=2, q=2)",
                }); // NB! ���������� �� ���������� ������� ������� ������ ������ ������ ������
        }

        private IServiceDependingOnString serviceDependingOnString;
#pragma warning disable 649
        // todo [edi-test]: inject fields after TestFixtureSetUp() call
        //[Injected]
        //private readonly IServiceDependingOnString serviceDependingOnString;
#pragma warning restore 649
    }
}
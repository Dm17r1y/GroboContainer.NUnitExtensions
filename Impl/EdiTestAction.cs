using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using JetBrains.Annotations;

using NUnit.Framework;

using SKBKontur.Catalogue.NUnit.Extensions.EdiTestMachinery.Impl.TestContext;
using SKBKontur.Catalogue.NUnit.Extensions.TestEnvironments.ExceptionUtils;
using SKBKontur.Catalogue.Objects;
using SKBKontur.Catalogue.ServiceLib.Logging;

namespace SKBKontur.Catalogue.NUnit.Extensions.EdiTestMachinery.Impl
{
    public static class EdiTestAction
    {
        public static void BeforeTest([NotNull] TestDetails testDetails)
        {
            EnsureAppDomainIntialization();

            var test = testDetails.Method;
            test.EnsureNunitAttributesAbscence();
            var fixtureType = test.GetFixtureType();
            var testFixture = testDetails.Fixture;
            if(fixtureType != testFixture.GetType())
                throw new InvalidProgramStateException(string.Format("TestFixtureType mismatch for: {0}", test.GetMethodName()));

            var suiteName = test.GetSuiteName();
            var suiteDescriptor = suiteDescriptors.GetOrAdd(suiteName, x => new SuiteDescriptor(fixtureType.Assembly));
            var suiteContext = suiteDescriptor.SuiteContext;
            foreach(var suiteWrapper in test.GetSuiteWrappers())
            {
                if(suiteDescriptor.SetUpedSuiteWrappers.Contains(suiteWrapper))
                    continue;
                suiteWrapper.SetUp(suiteName, suiteDescriptor.TestAssembly, suiteContext);
                suiteDescriptor.SetUpedSuiteWrappers.Add(suiteWrapper);
            }

            if(setUpedFixtures.Add(fixtureType))
            {
                InjectFixtureFields(suiteContext, testFixture);
                var fixtureSetUpMethod = test.FindFixtureSetUpMethod();
                if(fixtureSetUpMethod != null)
                {
                    if(suiteName != fixtureType.FullName)
                        throw new InvalidProgramStateException(string.Format("EdiTestFixtureSetUp method is only allowed inside EdiTestFixure suite. Test: {0}", test.GetMethodName()));
                    InvokeWrapperMethod(fixtureSetUpMethod, testFixture, suiteContext.GetContainer());
                }
            }

            var testName = testDetails.FullName;
            var methodContext = new EdiTestMethodContextData();
            foreach(var methodWrapper in test.GetMethodWrappers())
                methodWrapper.SetUp(testName, suiteContext, methodContext);

            EdiTestContextHolder.SetCurrentTestContext(testName, suiteContext, methodContext);

            InvokeWrapperMethod(test.FindSetUpMethod(), testFixture);
        }

        public static void AfterTest([NotNull] TestDetails testDetails)
        {
            var test = testDetails.Method;
            var suiteName = test.GetSuiteName();
            SuiteDescriptor suiteDescriptor;
            if(!suiteDescriptors.TryGetValue(suiteName, out suiteDescriptor))
                throw new InvalidProgramStateException(string.Format("Suite context is not set for: {0}", suiteName));

            InvokeWrapperMethod(test.FindTearDownMethod(), testDetails.Fixture);

            // todo [edi-test]: duplicate call fails when [TestSuite] is defined mltiple times for the given test
            var methodContext = EdiTestContextHolder.ResetCurrentTestContext();

            var testName = testDetails.FullName;
            foreach(var methodWrapper in Enumerable.Reverse(test.GetMethodWrappers()))
                methodWrapper.TearDown(testName, suiteDescriptor.SuiteContext, methodContext);

            methodContext.Destroy();
        }

        private static void InvokeWrapperMethod([CanBeNull] MethodInfo wrapperMethod, [NotNull] object testFixture, params object[] @params)
        {
            if(wrapperMethod == null)
                return;
            try
            {
                wrapperMethod.Invoke(testFixture, @params);
            }
            catch(TargetInvocationException exception)
            {
                exception.RethrowInnerException();
            }
        }

        private static void InjectFixtureFields([NotNull] EdiTestSuiteContextData suiteContext, [NotNull] object testFixture)
        {
            foreach(var fieldInfo in testFixture.GetType().GetFieldsForInjection())
                fieldInfo.SetValue(testFixture, suiteContext.GetContainer().Get(fieldInfo.FieldType));
        }

        private static void EnsureAppDomainIntialization()
        {
            if(appDomainIsIntialized)
                return;
            AppDomain.CurrentDomain.DomainUnload += (sender, args) => OnAppDomainUnload();
            appDomainIsIntialized = true;
        }

        private static void OnAppDomainUnload()
        {
            Log.For("EdiTestMachinery").InfoFormat("Suites to tear down: {0}", string.Join(", ", suiteDescriptors.Select(x => x.Key)));
            foreach(var kvp in suiteDescriptors.OrderByDescending(x => x.Value.Order))
            {
                var suiteName = kvp.Key;
                var suiteDescriptor = kvp.Value;
                foreach(var suiteWrapper in Enumerable.Reverse(suiteDescriptor.SetUpedSuiteWrappers))
                    suiteWrapper.TearDown(suiteName, suiteDescriptor.TestAssembly, suiteDescriptor.SuiteContext);
                suiteDescriptor.SuiteContext.Destroy();
            }
            Log.For("EdiTestMachinery").InfoFormat("App domain cleanup is finished");
        }

        private static bool appDomainIsIntialized;
        private static readonly HashSet<Type> setUpedFixtures = new HashSet<Type>();
        private static readonly ConcurrentDictionary<string, SuiteDescriptor> suiteDescriptors = new ConcurrentDictionary<string, SuiteDescriptor>();

        private class SuiteDescriptor
        {
            public SuiteDescriptor([NotNull] Assembly testAssembly)
            {
                Order = order++;
                TestAssembly = testAssembly;
                SuiteContext = new EdiTestSuiteContextData();
                SetUpedSuiteWrappers = new List<EdiTestSuiteWrapperAttribute>();
            }

            public int Order { get; private set; }

            [NotNull]
            public Assembly TestAssembly { get; private set; }

            [NotNull]
            public EdiTestSuiteContextData SuiteContext { get; private set; }

            [NotNull]
            public List<EdiTestSuiteWrapperAttribute> SetUpedSuiteWrappers { get; private set; }

            private static int order;
        }
    }
}
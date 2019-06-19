// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Linq;
using System.Windows.Forms.IntegrationTests.Common;
using Xunit;

namespace System.Windows.Forms.Maui.IntegrationTests
{
    /// <summary>
    /// This class runs a maui executable, which contains one or more scenarios.
    /// We want to be able to represent each scenario as a seperate xUnit test, but it's not
    /// possible to run them independently. The workaround is to have a MauiTestRunner execute all
    /// the scenarios once and store the results, then each xUnit test just parses the results
    /// it cares about and asserts appropriately.
    /// </summary>
    public abstract class WinformsMauiTestBase
    {
        /// <summary>
        /// The test runner, static because we only want one shared across all xUnit tests in this class
        /// </summary>
        private static readonly MauiTestRunner s_testRunner = new MauiTestRunner();

        /// <summary>
        /// The test results, again static because we only want one
        /// </summary>
        private static TestResult s_testResult;

        /// <summary>
        /// The lock object, used to make sure we only run once in case tests are running concurrently
        /// </summary>
        private static readonly object s_lock = new object();

        /// <summary>
        /// The project name, used to build the path to the maui executable
        /// Derived classes must override this to match their project names
        /// </summary>
        protected abstract string MauiProjectName { get; }

        /// <summary>
        /// The test result, exposed to the derived classes
        /// </summary>
        protected static TestResult TestResult { get => s_testResult; }

        /// <summary>
        /// Scenario theory data, used to drive unit tests in derived classes
        /// </summary>
        public static TheoryData<string> ScenarioTheoryData
        {
            get
            {
                var data = new TheoryData<string>();
                foreach (var scenarioName in TestResult.ScenarioGroup.Scenarios.Select(x => x.Name))
                {
                    data.Add(scenarioName);
                }

                return data;
            }
        }

        public WinformsMauiTestBase()
        {
            // if we have no results, execute the tests
            lock (s_lock)
            {
                if (s_testResult == null)
                {
                    var exePath = TestHelpers.GetExePath(MauiProjectName);
                    s_testResult = s_testRunner.RunTest(exePath);
                    Assert.NotNull(TestResult);
                    Assert.NotNull(TestResult.ScenarioGroup);
                }
            }
        }

        /// <summary>
        /// Validate a specified scenario passed.
        /// The derived classes will call this with their scenario names
        /// </summary>
        /// <param name="scenarioName">The name of the scenario</param>
        protected void ValidateScenarioPassed(string scenarioName)
        {
            var scenario = TestResult.ScenarioGroup.Scenarios.SingleOrDefault(x => x.Name == scenarioName);
            Assert.NotNull(scenario);
            Assert.NotNull(scenario.Result);
            Assert.Equal("Pass", scenario.Result.Type);
        }
    }
}

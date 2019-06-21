// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
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
        /// The test results, indexed by project name
        /// </summary>
        private static Dictionary<string, TestResult> s_testResults = new Dictionary<string, TestResult>();

        /// <summary>
        /// Run the maui test for the specified project name and store
        /// the results for later
        /// </summary>
        /// <param name="projectName">The name of the project to run</param>
        protected static void RunMauiTest(string projectName)
        {
            if (string.IsNullOrEmpty(projectName))
                throw new ArgumentNullException(nameof(projectName));

            var exePath = TestHelpers.GetExePath(projectName);
            var result = s_testRunner.RunTest(exePath);
            Assert.NotNull(result);
            Assert.NotNull(result.ScenarioGroup);

            s_testResults[projectName] = result;
        }

        /// <summary>
        /// Get the scenario theory data, used to drive unit tests in derived classes
        /// </summary>
        /// <param name="projectName">The name of the project</param>
        /// <returns>The theory data</returns>
        public static TheoryData<string> GetScenarioTheoryData(string projectName)
        {
            var data = new TheoryData<string>();

            s_testResults.TryGetValue(projectName, out var testResult);
            if (testResult == null)
                return null;

            foreach (var scenarioName in testResult.ScenarioGroup.Scenarios.Select(x => x.Name))
            {
                data.Add(scenarioName);
            }

            return data;
        }

        /// <summary>
        /// Validate a specified scenario passed.
        /// The derived classes will call this with their scenario names
        /// </summary>
        /// <param name="projectName">The name of the maui project</param>
        /// <param name="scenarioName">The name of the scenario</param>
        protected void ValidateScenarioPassed(string projectName, string scenarioName)
        {
            var scenario = s_testResults[projectName].ScenarioGroup.Scenarios.SingleOrDefault(x => x.Name == scenarioName);
            Assert.NotNull(scenario);
            Assert.NotNull(scenario.Result);
            Assert.Equal("Pass", scenario.Result.Type);
        }
    }
}

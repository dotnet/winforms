// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Windows.Forms.IntegrationTests.Common;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.Maui.IntegrationTests
{
    /// <summary>
    ///  This class performs common operations for maui xUnit tests.
    /// </summary>
    public static class MauiTestHelper
    {
        /// <summary>
        ///  The test runner
        /// </summary>
        private static readonly MauiTestRunner s_testRunner = new();

        /// <summary>
        ///  The test results, indexed by project name
        /// </summary>
        private static readonly Dictionary<string, TestResult> s_testResults = new();

        /// <summary>
        ///  Get the scenarios that will run for a specified maui project
        /// </summary>
        /// <param name="projectName">The project</param>
        /// <returns>The scenarios that ran</returns>
        public static IEnumerable<string> GetScenarios(string projectName)
        {
            List<string> scenarios = new();

            // get the path to the maui test executable
            var exePath = TestHelpers.GetExePath(projectName);

            // make sure the dll exists too, since that's the assembly we really have to load
            var dllPath = exePath.Replace(".exe", ".dll");
            if (!File.Exists(dllPath))
                throw new FileNotFoundException("Unable to get Maui scenarios, file not found", dllPath);

            // Load the assembly. Note we have to load the dll instead of the exe because
            // of an architecture mismatch. Otherwise the line below will throw a BadImageFormatException
            var assembly = Assembly.LoadFrom(dllPath);

            // Look for all methods on all types with the '[Scenario(true)]' attribute
            foreach (var type in assembly.DefinedTypes)
            {
                foreach (var methodInfo in type.GetMethods())
                {
                    var attributes = methodInfo.GetCustomAttributes();
                    foreach (dynamic attribute in attributes)
                    {
                        if (attribute.TypeId.Name != "ScenarioAttribute")
                            continue;

                        if (!attribute.IsScenario)
                            continue;

                        // if we get here, the method has [Scenario(true)]
                        scenarios.Add(methodInfo.Name);
                    }
                }
            }

            return scenarios;
        }

        /// <summary>
        ///  Validate a specified scenario passed.
        /// </summary>
        /// <param name="projectName">The name of the maui project</param>
        /// <param name="scenarioName">The name of the scenario</param>
        public static void ValidateScenarioPassed(string projectName, string scenarioName, ITestOutputHelper output = null)
        {
            // if the test hasn't run yet for the specified projectName, run it
            if (!s_testResults.ContainsKey(projectName))
            {
                RunMauiTest(projectName);
            }

            var scenario = s_testResults[projectName].ScenarioGroup.Scenarios.SingleOrDefault(x => x.Name == scenarioName);

            Assert.NotNull(scenario);
            Assert.NotNull(scenario.Result);

            if (output is not null && scenario.Result.Type != "Pass" && scenario.Text?.Length > 0)
            {
                output.WriteLine($"Log:{string.Join("\r\n", scenario.Text)}");
            }

            Assert.Equal("Pass", scenario.Result.Type);
        }

        /// <summary>
        ///  Run the maui test for the specified project name and store the results
        /// </summary>
        /// <param name="projectName">The name of the project to run</param>
        private static void RunMauiTest(string projectName)
        {
            if (string.IsNullOrEmpty(projectName))
                throw new ArgumentNullException(nameof(projectName));

            if (s_testResults.ContainsKey(projectName))
                throw new InvalidOperationException($"Maui test for {projectName} has already run. Please double-check the project name in your test class.");

            var exePath = TestHelpers.GetExePath(projectName);
            var result = s_testRunner.RunTest(exePath);
            Assert.NotNull(result);
            Assert.NotNull(result.ScenarioGroup);

            s_testResults[projectName] = result;
        }
    }
}

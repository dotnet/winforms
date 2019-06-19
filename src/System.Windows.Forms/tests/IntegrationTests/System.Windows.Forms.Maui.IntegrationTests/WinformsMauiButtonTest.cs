// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Maui.IntegrationTests
{
    /// <summary>
    /// This class runs a maui executable, which contains one or more scenarios.
    /// We want to be able to represent each scenario as a seperate xUnit test, but it's not
    /// possible to run them independently. The workaround is to have a MauiTestRunner execute all
    /// the scenarios once and store the results, then feed the scenario names in as member data.
    /// </summary>
    public class WinformsMauiButtonTest : WinformsMauiTestBase
    {
        private const string ProjectName = "MauiButtonTest";

        /// <summary>
        /// The project name, used to build the path to the maui executable
        /// Derived classes must override this to match their project names
        /// </summary>
        protected override string MauiProjectName { get => ProjectName; }

        [Theory]
        [MemberData(nameof(ScenarioTheoryData))]
        public void WinformsMauiTest_ButtonTest(string scenarioName)
        {
            ValidateScenarioPassed(scenarioName);
        }
    }
}

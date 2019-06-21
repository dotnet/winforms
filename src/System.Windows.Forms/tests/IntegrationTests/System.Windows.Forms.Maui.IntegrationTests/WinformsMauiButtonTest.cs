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
    /// 
    /// The only trick is that MemberData is resolved before any constructors are called. This means
    /// our scenario names will not be available yet. The solution to this is to create a custom 
    /// MemberDataAttribute which executes custom code before returning the expected data. 
    /// See CustomMemberDataAttribute.cs for more info.
    /// </summary>
    public class WinformsMauiButtonTest : WinformsMauiTestBase
    {
        public const string ProjectName = "MauiButtonTest";

        static WinformsMauiButtonTest()
        {
            // run the test to get the results in the base class
            RunMauiTest(ProjectName);
        }

        [Theory]
        [CustomMemberData(nameof(GetScenarioTheoryData), ProjectName)]
        public void WinformsMauiTest_ButtonTest(string scenarioName)
        {
            ValidateScenarioPassed(ProjectName, scenarioName);
        }
    }
}

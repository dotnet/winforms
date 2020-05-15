// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Maui.IntegrationTests
{
    /// <summary>
    ///  This class runs a maui executable, which contains one or more scenarios.
    ///
    ///  We want to be able to represent each scenario as a seperate xUnit test, but it's not
    ///  possible to run them independently. The workaround is to have a MauiTestRunner execute all
    ///  the scenarios once and store the results, then feed the scenario names in as member data.
    ///
    ///  However, MemberData is resolved before any constructors (even static) are called.
    ///  This means the scenario names will not be available yet.
    ///
    ///  The solution to this is to inherit from MemberDataAttribute and execute custom code
    ///  (running the maui test) before returning the expected data. See MauiMemberDataAttribute.cs for more info.
    ///
    ///  Also [Collection("Maui")] is used put all maui tests in the same collection, which makes them run sequentially
    ///  instead of in parallel. This is to migitate race conditions of multiple forms open at once.
    /// </summary>
    [Collection("Maui")]
    public class WinformsMauiMonthCalendarTests
    {
        private const string ProjectName = "MauiMonthCalendarTests";

        [Theory]
        [MauiData(ProjectName)]
        public void MauiMonthCalendarTest(string scenarioName)
        {
            MauiTestHelper.ValidateScenarioPassed(ProjectName, scenarioName);
        }
    }
}

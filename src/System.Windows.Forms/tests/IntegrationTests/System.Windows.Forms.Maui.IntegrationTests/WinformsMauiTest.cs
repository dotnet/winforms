// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms.IntegrationTests.Common;
using System.Xml.Linq;
using Xunit;

namespace System.Windows.Forms.Maui.IntegrationTests
{
    public class WinformsMauiTest
    {
        // todo: use https://xunit.net/docs/shared-context.html#class-fixture to run the test once
        // then make each check of the log file the actual fact to make the tests as they are supposed to be
        // make it implement IDisposable so dispose can rename the results file

        [Fact]
        public void WinformsMauiTest_ButtonTest()
        {
            const string projectName = "MauiButtonTest";
            string exePath = TestHelpers.GetExePath(projectName);

            // run the maui test exe, making sure to set the cwd of the process
            // (so the results.log gets generated in the right place)
            Process process = TestHelpers.StartProcess(exePath, true);
            process.WaitForExit();

            Assert.True(process.HasExited);
            Assert.Equal(0, process.ExitCode);

            // check the maui results.log for failures
            var logDir = Path.GetDirectoryName(exePath);
            var logPath = Path.Combine(logDir, "results.log");

            // the log is an xml file that looks like this:
            // <Testcase>
            //   <ScenarioGroup>
            //     <Scenario>
            //       <Result type="Pass/Fail">
            //   <FinalResults type="Pass/Fail">
            var element = XElement.Load(logPath);

            // get the result of each scenario and make sure it's a pass
            var scenarios = element.Descendants("Scenario");
            foreach (var scenario in scenarios)
            {
                var resultNode = scenario.Descendants("Result");
                var result = resultNode.Select(x => x.Attribute("type").Value).First();
                Assert.Equal("Pass", result);
            }
            
            // todo: parse each test, and write to log, not just the total
        }
    }
}

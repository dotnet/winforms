// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Xunit;

namespace System.Windows.Forms.IntegrationTests
{
    public class WinformsControlsTest
    {
        public string GetPathToTestFromBin(string testName)
        {
            string buildType = "Release";

#if DEBUG
            buildType = "Debug";
#endif

            return $"{testName}\\{buildType}\\netcoreapp3.0\\{testName}.exe";
        }

        [Fact]
        public void WinformsMauiTest_ButtonTest()
        {
            // run the maui test exe
            var relativeExePath = GetPathToTestFromBin("MauiButtonTest");
            Process process = TestHelpers.StartProcess(relativeExePath);
            process.WaitForExit();

            Assert.True(process.HasExited);
            Assert.Equal(0, process.ExitCode);

            // check the maui results.log for failures
            var logDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var logPath = Path.Combine(logDir, "results.log");

            // the log is an xml file that looks like this:
            // <Testcase>
            //   <FinalResults type="Pass/Fail" total="X" fail="Y">
            var element = XElement.Load(logPath);
            var result = element.Descendants("FinalResults").Select(x => x.Attribute("type").Value).First();
            Assert.Equal("Pass", result);
        }
    }
}

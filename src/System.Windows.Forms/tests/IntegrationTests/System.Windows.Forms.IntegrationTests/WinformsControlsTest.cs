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
    public class WinformsControlsTestTests : IFunctionalPathTest
    {
        public string GetPathToTestFromBin()
        {
            string buildType = "Release";

#if DEBUG
            buildType = "Debug";
#endif

            return "WinformsControlsTest\\" + buildType + "\\netcoreapp3.0\\WinformsControlsTest.exe";
        }

        public string GetPathToMauiTestFromBin(string testName)
        {
            string buildType = "Release";

#if DEBUG
            buildType = "Debug";
#endif

            return $"{testName}\\{buildType}\\netcoreapp3.0\\{testName}.exe";
        }

        [Fact]
        public void WinformsControlsTest_OpenAndClose()
        {
            Process process = TestHelpers.StartProcess(GetPathToTestFromBin());

            Assert.NotNull(process);
            Assert.NotNull(Process.GetProcessById(process.Id));
            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_ButtonsTest()
        {
            Process process = TestHelpers.StartProcess(GetPathToTestFromBin());

            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_CalendarTest()
        {
            Process process = TestHelpers.StartProcess(GetPathToTestFromBin());
            TestHelpers.PressTabsOnProcess(process, 1);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }


        // [Fact]
        // Commenting out until this gets fixed
        // public void WinformsControlsTest_TreeViewAndImageListTest()
        // {
        //     var process = TestHelpers.StartProcess(GetPathToTestFromBin());
        //     TestHelpers.PressTabsOnProcess(process, 2);
        //     TestHelpers.PressEnterOnProcess(process);

        //     Assert.False(process.HasExited);

        //     process.Kill();
        //     process.WaitForExit();

        //     Assert.True(process.HasExited);
        // }

        [Fact]
        public void WinformsControlsTest_ContentAlignmentTest()
        {
            Process process = TestHelpers.StartProcess(GetPathToTestFromBin());
            TestHelpers.PressTabsOnProcess(process, 3);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_MultipleControlsTest()
        {
            Process process = TestHelpers.StartProcess(GetPathToTestFromBin());
            TestHelpers.PressTabsOnProcess(process, 4);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_DataGridViewTest()
        {
            Process process = TestHelpers.StartProcess(GetPathToTestFromBin());
            TestHelpers.PressTabsOnProcess(process, 5);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_MenusTest()
        {
            Process process = TestHelpers.StartProcess(GetPathToTestFromBin());
            TestHelpers.PressTabsOnProcess(process, 6);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_PanelsTest()
        {
            Process process = TestHelpers.StartProcess(GetPathToTestFromBin());
            TestHelpers.PressTabsOnProcess(process, 7);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_SplitterTest()
        {
            Process process = TestHelpers.StartProcess(GetPathToTestFromBin());
            TestHelpers.PressTabsOnProcess(process, 8);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_ComboBoxesTest()
        {
            Process process = TestHelpers.StartProcess(GetPathToTestFromBin());
            TestHelpers.PressTabsOnProcess(process, 9);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_MDIParentTest()
        {
            Process process = TestHelpers.StartProcess(GetPathToTestFromBin());
            TestHelpers.PressTabsOnProcess(process, 10);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_PropertyGridTest()
        {
            Process process = TestHelpers.StartProcess(GetPathToTestFromBin());
            TestHelpers.PressTabsOnProcess(process, 11);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_PropertyGrid_CollectionEditorTest()
        {
            Process process = TestHelpers.StartProcess(GetPathToTestFromBin());
            TestHelpers.PressTabsOnProcess(process, 11);
            TestHelpers.PressEnterOnProcess(process);

            TestHelpers.PressTabOnProcess(process);
            TestHelpers.PressRightOnProcess(process); // once   
            TestHelpers.PressRightOnProcess(process); // twice
            TestHelpers.PressTabsOnProcess(process, 2);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        // [Fact]
        // Commenting this out until this is fixed
        // public void WinformsControlsTest_ListViewTest()
        // {
        //     var process = TestHelpers.StartProcess(GetPathToTestFromBin());
        //     TestHelpers.PressTabsOnProcess(process, 12);
        //     TestHelpers.PressEnterOnProcess(process);

        //     Assert.False(process.HasExited);

        //     process.Kill();
        //     process.WaitForExit();

        //     Assert.True(process.HasExited);
        // }

        [Fact]
        public void WinformsControlsTest_DateTimePickerTest()
        {
            Process process = TestHelpers.StartProcess(GetPathToTestFromBin());
            TestHelpers.PressTabsOnProcess(process, 13);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_ThreadExceptionDialogTest()
        {
            Process process = TestHelpers.StartProcess(GetPathToTestFromBin());
            TestHelpers.PressTabsOnProcess(process, 15);
            TestHelpers.PressEnterOnProcess(process);

            TestHelpers.PressTabsOnProcess(process, 2);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsMauiTest_ButtonTest()
        {
            // run the maui test exe
            var relativeExePath = GetPathToMauiTestFromBin("MauiButtonTest");
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

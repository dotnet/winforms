// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.IO;
using System.Windows.Forms.IntegrationTests.Common;
using Xunit;

namespace System.Windows.Forms.IntegrationTests
{
    public class WinformsControlsTest
    {
        private const string ProjectName = "WinformsControlsTest";
        private readonly string _exePath;

        public WinformsControlsTest()
        {
            _exePath = TestHelpers.GetExePath(ProjectName);
        }

        [Fact]
        public void WinformsControlsTest_OpenAndClose()
        {
            Process process = TestHelpers.StartProcess(_exePath);

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
            Process process = TestHelpers.StartProcess(_exePath);

            TestHelpers.PressTabsOnProcess(process, MainFormControlsTabOrder.Buttons);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_CalendarTest()
        {
            Process process = TestHelpers.StartProcess(_exePath);
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
            Process process = TestHelpers.StartProcess(_exePath);
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
            Process process = TestHelpers.StartProcess(_exePath);
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
            Process process = TestHelpers.StartProcess(_exePath);
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
            Process process = TestHelpers.StartProcess(_exePath);
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
            Process process = TestHelpers.StartProcess(_exePath);
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
            Process process = TestHelpers.StartProcess(_exePath);
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
            Process process = TestHelpers.StartProcess(_exePath);
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
            Process process = TestHelpers.StartProcess(_exePath);
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
            Process process = TestHelpers.StartProcess(_exePath);
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
            Process process = TestHelpers.StartProcess(_exePath);
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
            Process process = TestHelpers.StartProcess(_exePath);
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
            Process process = TestHelpers.StartProcess(_exePath);
            TestHelpers.PressTabsOnProcess(process, 15);
            TestHelpers.PressEnterOnProcess(process);

            TestHelpers.PressTabsOnProcess(process, 2);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }
    }
}

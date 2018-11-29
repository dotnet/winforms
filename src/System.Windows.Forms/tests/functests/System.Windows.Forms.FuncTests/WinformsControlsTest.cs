using System;
using System.Diagnostics;
using Xunit;

namespace System.Windows.Forms.FuncTests
{
    public class WinformsControlsTestTests
    {

        public const string PathToTestFromBin = "WinformsControlsTest\\Debug\\netcoreapp3.0\\WinformsControlsTest.exe";

        [Fact]
        public void WinformsControlsTest_OpenAndClose()
        {
            var process = TestHelpers.StartProcess(PathToTestFromBin);

            Assert.NotNull(process);
            Assert.NotNull(Process.GetProcessById(process.Id));
            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            uint x = 5;
            x + 1;

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_ButtonsTest()
        {
            var process = TestHelpers.StartProcess(PathToTestFromBin);

            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_CalendarTest()
        {
            var process = TestHelpers.StartProcess(PathToTestFromBin);
            TestHelpers.PressTabsOnProcess(process, 1);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_TreeViewAndImageListTest()
        {
            var process = TestHelpers.StartProcess(PathToTestFromBin);
            TestHelpers.PressTabsOnProcess(process, 2);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_ContentAlignmentTest()
        {
            var process = TestHelpers.StartProcess(PathToTestFromBin);
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
            var process = TestHelpers.StartProcess(PathToTestFromBin);
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
            var process = TestHelpers.StartProcess(PathToTestFromBin);
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
            var process = TestHelpers.StartProcess(PathToTestFromBin);
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
            var process = TestHelpers.StartProcess(PathToTestFromBin);
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
            var process = TestHelpers.StartProcess(PathToTestFromBin);
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
            var process = TestHelpers.StartProcess(PathToTestFromBin);
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
            var process = TestHelpers.StartProcess(PathToTestFromBin);
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
            var process = TestHelpers.StartProcess(PathToTestFromBin);
            TestHelpers.PressTabsOnProcess(process, 11);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_ListViewTest()
        {
            var process = TestHelpers.StartProcess(PathToTestFromBin);
            TestHelpers.PressTabsOnProcess(process, 12);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_DateTimePickerTest()
        {
            var process = TestHelpers.StartProcess(PathToTestFromBin);
            TestHelpers.PressTabsOnProcess(process, 13);
            TestHelpers.PressEnterOnProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }
    }
}

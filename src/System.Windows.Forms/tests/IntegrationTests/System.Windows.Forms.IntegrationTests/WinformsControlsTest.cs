// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Windows.Forms.IntegrationTests.Common;
using Xunit;

namespace System.Windows.Forms.IntegrationTests
{
    public partial class WinformsControlsTest
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

        [Theory]
        [InlineData(MainFormControlsTabOrder.ButtonsButton)]
        [InlineData(MainFormControlsTabOrder.CalendarButton)]
        [InlineData(MainFormControlsTabOrder.MultipleControlsButton)]
        [InlineData(MainFormControlsTabOrder.ComboBoxesButton)]
        [InlineData(MainFormControlsTabOrder.DateTimePickerButton)]
        [InlineData(MainFormControlsTabOrder.FolderBrowserDialogButton)]
        [InlineData(MainFormControlsTabOrder.ThreadExceptionDialogButton)]
        [InlineData(MainFormControlsTabOrder.PrintDialogButton)]
        [InlineData(MainFormControlsTabOrder.DataGridViewButton)]
        [InlineData(MainFormControlsTabOrder.TreeViewButton)]
        [InlineData(MainFormControlsTabOrder.ContentAlignmentButton)]
        [InlineData(MainFormControlsTabOrder.MenusButton)]
        [InlineData(MainFormControlsTabOrder.PanelsButton)]
        [InlineData(MainFormControlsTabOrder.SplitterButton)]
        [InlineData(MainFormControlsTabOrder.MdiParentButton)]
        [InlineData(MainFormControlsTabOrder.PropertyGridButton)]
        [InlineData(MainFormControlsTabOrder.ListViewButton)]
        [InlineData(MainFormControlsTabOrder.FontNameEditorButton)]
        [InlineData(MainFormControlsTabOrder.CollectionEditorsButton)]
        [InlineData(MainFormControlsTabOrder.RichTextBoxesButton)]
        [InlineData(MainFormControlsTabOrder.PictureBoxesButton)]
        [InlineData(MainFormControlsTabOrder.FormBorderStylesButton)]
        [InlineData(MainFormControlsTabOrder.ToggleIconButton)]
        [InlineData(MainFormControlsTabOrder.FileDialogButton)]
        public void WinformsControlsTest_InnerForms_OpenAndClose(MainFormControlsTabOrder tabOrder)
        {
            Process process = TestHelpers.StartProcess(_exePath);
            TestHelpers.SendTabKeysToProcess(process, tabOrder);
            TestHelpers.SendEnterKeyToProcess(process);
            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_CalendarTest()
        {
            Process process = TestHelpers.StartProcess(_exePath);
            TestHelpers.SendTabKeysToProcess(process, MainFormControlsTabOrder.CalendarButton);
            TestHelpers.SendEnterKeyToProcess(process);
            TestHelpers.SendRightArrowKeyToProcess(process, switchToMainWindow: false);
            TestHelpers.SendRightArrowKeyToProcess(process, switchToMainWindow: false);
            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_ThreadExceptionDialogTest()
        {
            Process process = TestHelpers.StartProcess(_exePath);
            TestHelpers.SendTabKeysToProcess(process, MainFormControlsTabOrder.ThreadExceptionDialogButton);
            TestHelpers.SendEnterKeyToProcess(process);
            TestHelpers.SendTabKeysToProcess(process, 2);
            TestHelpers.SendEnterKeyToProcess(process);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_PropertyGrid_CollectionEditorTest()
        {
            Process process = TestHelpers.StartProcess(_exePath);
            TestHelpers.SendTabKeysToProcess(process, MainFormControlsTabOrder.PropertyGridButton);
            TestHelpers.SendEnterKeyToProcess(process);
            TestHelpers.SendTabKeyToProcess(process, switchToMainWindow: false);
            TestHelpers.SendRightArrowKeyToProcess(process, switchToMainWindow: false); // once
            TestHelpers.SendRightArrowKeyToProcess(process, switchToMainWindow: false); // twice
            TestHelpers.SendTabKeysToProcess(process, 2, switchToMainWindow: false);
            TestHelpers.SendEnterKeyToProcess(process, switchToMainWindow: false);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void WinformsControlsTest_FileDialogTest()
        {
            Process process = TestHelpers.StartProcess(_exePath);
            TestHelpers.SendTabKeysToProcess(process, MainFormControlsTabOrder.FileDialogButton);
            TestHelpers.SendEnterKeyToProcess(process);
            TestHelpers.SendEnterKeyToProcess(process, switchToMainWindow: false);
            TestHelpers.SendTabKeysToProcess(process, 1, switchToMainWindow: false);
            TestHelpers.SendEnterKeyToProcess(process, switchToMainWindow: false);
            TestHelpers.SendTabKeysToProcess(process, 1, switchToMainWindow: false);
            TestHelpers.SendEnterKeyToProcess(process, switchToMainWindow: false);
            TestHelpers.SendTabKeysToProcess(process, 1, switchToMainWindow: false);
            TestHelpers.SendEnterKeyToProcess(process, switchToMainWindow: false);
            TestHelpers.SendTabKeysToProcess(process, 1, switchToMainWindow: false);
            TestHelpers.SendEnterKeyToProcess(process, switchToMainWindow: false);
            TestHelpers.SendTabKeysToProcess(process, 1, switchToMainWindow: false);
            TestHelpers.SendEnterKeyToProcess(process, switchToMainWindow: false);
            TestHelpers.SendTabKeysToProcess(process, 1, switchToMainWindow: false);
            TestHelpers.SendEnterKeyToProcess(process, switchToMainWindow: false);

            Assert.False(process.HasExited);

            process.Kill();
            process.WaitForExit();

            Assert.True(process.HasExited);
        }

        [Fact]
        public void DataBindings_remove_should_unsubscribe_INotifyPropertyChanged_PropertyChanged_event()
        {
            var mainObject = new Mocks.MainObject();
            mainObject.Text = "Test text";
            Form form = new Form();
            TextBox textBox = new TextBox();
            Binding binding = new Binding("Text", mainObject, "Text");
            textBox.DataBindings.Add(binding);
            textBox.Parent = form;
            form.Show();

            // bindings set
            Assert.True(mainObject.IsPropertyChangedAssigned);

            // remove bindings
            textBox.DataBindings.Clear();

            // bindings unset
            Assert.False(mainObject.IsPropertyChangedAssigned);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

public class OpenFileDialogTests : ControlTestBase
{
    public OpenFileDialogTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    // Regression test for https://github.com/dotnet/winforms/issues/8108
    [WinFormsFact]
    public void OpenWithNonExistingInitDirectory_Success()
    {
        using DialogHostForm dialogOwnerForm = new();
        using OpenFileDialog dialog = new();
        dialog.InitialDirectory = Guid.NewGuid().ToString();
        Assert.Equal(DialogResult.Cancel, dialog.ShowDialog(dialogOwnerForm));
    }

    [WinFormsFact]
    public void OpenWithExistingInitDirectory_Success()
    {
        using DialogHostForm dialogOwnerForm = new();
        using OpenFileDialog dialog = new();
        dialog.InitialDirectory = Path.GetTempPath();
        Assert.Equal(DialogResult.Cancel, dialog.ShowDialog(dialogOwnerForm));
    }

    // Regression test for https://github.com/dotnet/winforms/issues/8414
    [WinFormsFact]
    public void ShowDialog_ResultWithMultiselect()
    {
        using var tempFile = TempFile.Create(0);
        using AcceptDialogForm dialogOwnerForm = new();
        using OpenFileDialog dialog = new();
        dialog.Multiselect = true;
        dialog.InitialDirectory = Path.GetDirectoryName(tempFile.Path);
        dialog.FileName = tempFile.Path;
        Assert.Equal(DialogResult.OK, dialog.ShowDialog(dialogOwnerForm));
        Assert.Equal(tempFile.Path, dialog.FileName);
    }

    // Regression test for https://github.com/dotnet/winforms/issues/12847
    [WinFormsFact]
    public void ShowDialog_Twice()
    {
        using OpenFileDialog dialog = new();
        using var tempFile = TempFile.Create(0);
        dialog.Multiselect = true;
        dialog.InitialDirectory = Path.GetDirectoryName(tempFile.Path);
        dialog.FileName = tempFile.Path;

        using RaceConditionDialogForm dialogOwnerForm = new(dialog);
        Assert.Equal(DialogResult.OK, dialog.ShowDialog(dialogOwnerForm));
    }

    private class RaceConditionDialogForm : AcceptDialogForm
    {
        private readonly OpenFileDialog _dialog;

        public RaceConditionDialogForm(OpenFileDialog dialog)
        {
            _dialog = dialog;
        }

        protected override void OnDialogIdle(HWND dialogHandle)
        {
            Assert.Equal(DialogResult.Cancel, _dialog.ShowDialog(this));
            base.OnDialogIdle(dialogHandle);
        }
    }

    private class AcceptDialogForm : DialogHostForm
    {
        protected override void OnDialogIdle(HWND dialogHandle)
        {
            Accept(dialogHandle);
        }
    }
}

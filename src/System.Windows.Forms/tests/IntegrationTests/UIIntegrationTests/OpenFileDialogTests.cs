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
    public void OpenFileDialogTests_OpenWithNonExistingInitDirectory_Success()
    {
        using DialogHostForm dialogOwnerForm = new();
        using OpenFileDialog dialog = new();
        dialog.InitialDirectory = Guid.NewGuid().ToString();
        Assert.Equal(DialogResult.Cancel, dialog.ShowDialog(dialogOwnerForm));
    }

    [WinFormsFact]
    public void OpenFileDialogTests_OpenWithExistingInitDirectory_Success()
    {
        using DialogHostForm dialogOwnerForm = new();
        using OpenFileDialog dialog = new();
        dialog.InitialDirectory = Path.GetTempPath();
        Assert.Equal(DialogResult.Cancel, dialog.ShowDialog(dialogOwnerForm));
    }

    // Regression test for https://github.com/dotnet/winforms/issues/8414
    [WinFormsFact]
    public void OpenFileDialogTests_ResultWithMultiselect()
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

    private class AcceptDialogForm : DialogHostForm
    {
        protected override void OnDialogIdle(HWND dialogHandle)
        {
            Accept(dialogHandle);
        }
    }
}

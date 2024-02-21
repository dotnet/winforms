// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

public class PrintDialogTests : ControlTestBase
{
    public PrintDialogTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    // Regression test for https://github.com/dotnet/winforms/issues/10920
    [WinFormsFact]
    public void PrintDialogTests_UseEXDialog_Cancel_Success()
    {
        using DialogHostForm dialogOwnerForm = new();
        using PrintDialog dialog = new();
        dialog.UseEXDialog = false;
        Assert.Equal(DialogResult.Cancel, dialog.ShowDialog(dialogOwnerForm));
    }

    [WinFormsFact]
    public void PrintDialogTests_UseEXDialog_Success()
    {
        using AcceptDialogForm dialogOwnerForm = new();
        using PrintDialog dialog = new();
        dialog.UseEXDialog = false;
        Assert.Equal(DialogResult.OK, dialog.ShowDialog(dialogOwnerForm));
    }

    private class AcceptDialogForm : DialogHostForm
    {
        protected override void OnDialogIdle(HWND dialogHandle)
        {
            Accept(dialogHandle);
        }
    }
}

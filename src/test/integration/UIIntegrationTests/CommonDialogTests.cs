// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Moq;

namespace System.Windows.Forms.UITests;

// Migrated from unit tests; see issue #4500.
public class CommonDialogTests
{
    [WinFormsTheory]
    [InlineData(true, DialogResult.OK)]
    [InlineData(false, DialogResult.Cancel)]
    public void ShowDialog_NonControlOwnerWithVisualStyles_ReturnsExpected(bool runDialogResult, DialogResult expectedDialogResult)
    {
        Application.EnableVisualStyles();

        using SubCommonDialog dialog = new()
        {
            RunDialogResult = runDialogResult
        };
        var owner = new Mock<IWin32Window>(MockBehavior.Strict);
        owner
            .Setup(o => o.Handle)
            .Returns(IntPtr.Zero);
        Assert.Equal(expectedDialogResult, dialog.ShowDialog(owner.Object));
    }

    [WinFormsTheory]
    [InlineData(true, DialogResult.OK)]
    [InlineData(false, DialogResult.Cancel)]
    public void ShowDialog_ControlOwnerWithVisualStyles_ReturnsExpected(bool runDialogResult, DialogResult expectedDialogResult)
    {
        Application.EnableVisualStyles();

        using SubCommonDialog dialog = new()
        {
            RunDialogResult = runDialogResult
        };
        using Control owner = new();
        Assert.Equal(expectedDialogResult, dialog.ShowDialog(owner));
    }

    [WinFormsTheory]
    [InlineData(true, DialogResult.OK)]
    [InlineData(false, DialogResult.Cancel)]
    public void ShowDialog_ControlOwnerWithHandleWithVisualStyles_ReturnsExpected(bool runDialogResult, DialogResult expectedDialogResult)
    {
        Application.EnableVisualStyles();

        using SubCommonDialog dialog = new()
        {
            RunDialogResult = runDialogResult
        };
        using Control owner = new();
        Assert.NotEqual(IntPtr.Zero, owner.Handle);
        Assert.Equal(expectedDialogResult, dialog.ShowDialog(owner));
    }

    private class SubCommonDialog : CommonDialog
    {
        public override void Reset()
        {
        }

        public bool RunDialogResult { get; set; }

        protected override bool RunDialog(IntPtr hwndOwner) => RunDialogResult;
    }
}

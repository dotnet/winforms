﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using Xunit.Abstractions;
using static Interop;

namespace System.Windows.Forms.UITests;

public class FolderBrowserDialogTests : ControlTestBase
{
    public FolderBrowserDialogTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    // Regression test for https://github.com/dotnet/winforms/issues/7981
    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void FolderBrowserDialog_ShowDialog(bool autoUpgradeEnabled)
    {
        using DialogForm dialogOwnerForm = new(User32.WM.ENTERIDLE, User32.WM.CLOSE);
        using FolderBrowserDialog dialog = new()
        {
            AutoUpgradeEnabled = autoUpgradeEnabled,
        };

        Assert.Equal(DialogResult.Cancel, dialog.ShowDialog(dialogOwnerForm));
    }
}

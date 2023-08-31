// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class SaveFileDialogTests
{
    [WinFormsFact]
    public void SaveFileDialog_Ctor_Default()
    {
        using var dialog = new SaveFileDialog();
        Assert.True(dialog.CheckWriteAccess);
        Assert.False(dialog.CreatePrompt);
        Assert.True(dialog.ExpandedMode);
        Assert.True(dialog.OverwritePrompt);
    }

    [WinFormsTheory]
    [BoolData]
    public void SaveFileDialog_CheckWriteAccess_Set_GetReturnsExpected(bool value)
    {
        using var dialog = new SaveFileDialog
        {
            CheckWriteAccess = value
        };
        Assert.Equal(value, dialog.CheckWriteAccess);

        // Set same.
        dialog.CheckWriteAccess = value;
        Assert.Equal(value, dialog.CheckWriteAccess);

        // Set different.
        dialog.CheckWriteAccess = !value;
        Assert.Equal(!value, dialog.CheckWriteAccess);
    }

    [WinFormsTheory]
    [BoolData]
    public void SaveFileDialog_CreatePrompt_Set_GetReturnsExpected(bool value)
    {
        using var dialog = new SaveFileDialog
        {
            CreatePrompt = value
        };
        Assert.Equal(value, dialog.CreatePrompt);

        // Set same.
        dialog.CreatePrompt = value;
        Assert.Equal(value, dialog.CreatePrompt);

        // Set different.
        dialog.CreatePrompt = !value;
        Assert.Equal(!value, dialog.CreatePrompt);
    }

    [WinFormsTheory]
    [BoolData]
    public void SaveFileDialog_ExpandedMode_Set_GetReturnsExpected(bool value)
    {
        using var dialog = new SaveFileDialog
        {
            ExpandedMode = value
        };
        Assert.Equal(value, dialog.ExpandedMode);

        // Set same.
        dialog.ExpandedMode = value;
        Assert.Equal(value, dialog.ExpandedMode);

        // Set different.
        dialog.ExpandedMode = !value;
        Assert.Equal(!value, dialog.ExpandedMode);
    }

    [WinFormsTheory]
    [BoolData]
    public void SaveFileDialog_OverwritePrompt_Set_GetReturnsExpected(bool value)
    {
        using var dialog = new SaveFileDialog
        {
            OverwritePrompt = value
        };
        Assert.Equal(value, dialog.OverwritePrompt);

        // Set same.
        dialog.OverwritePrompt = value;
        Assert.Equal(value, dialog.OverwritePrompt);

        // Set different.
        dialog.OverwritePrompt = !value;
        Assert.Equal(!value, dialog.OverwritePrompt);
    }

    [WinFormsFact]
    public void SaveFileDialog_Reset_Invoke_Success()
    {
        using var dialog = new SaveFileDialog
        {
            CheckWriteAccess = false,
            CreatePrompt = true,
            ExpandedMode = false,
            OverwritePrompt = false
        };

        dialog.Reset();
        Assert.True(dialog.CheckWriteAccess);
        Assert.False(dialog.CreatePrompt);
        Assert.True(dialog.ExpandedMode);
        Assert.True(dialog.OverwritePrompt);
    }
}

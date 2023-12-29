// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class OpenFileDialogTests
{
    [WinFormsFact]
    public void OpenFileDialog_Ctor_Default()
    {
        using OpenFileDialog dialog = new();
        Assert.True(dialog.CheckFileExists);
        Assert.False(dialog.Multiselect);
        Assert.False(dialog.ReadOnlyChecked);
        Assert.True(dialog.SelectReadOnly);
        Assert.False(dialog.ShowPreview);
        Assert.False(dialog.ShowReadOnly);
    }

    [WinFormsTheory]
    [BoolData]
    public void OpenFileDialog_CheckFileExists_Set_GetReturnsExpected(bool value)
    {
        using OpenFileDialog dialog = new()
        {
            CheckFileExists = value
        };
        Assert.Equal(value, dialog.CheckFileExists);

        // Set same.
        dialog.CheckFileExists = value;
        Assert.Equal(value, dialog.CheckFileExists);

        // Set different.
        dialog.CheckFileExists = !value;
        Assert.Equal(!value, dialog.CheckFileExists);
    }

    [WinFormsTheory]
    [BoolData]
    public void OpenFileDialog_Multiselect_Set_GetReturnsExpected(bool value)
    {
        using OpenFileDialog dialog = new()
        {
            Multiselect = value
        };
        Assert.Equal(value, dialog.Multiselect);

        // Set same.
        dialog.Multiselect = value;
        Assert.Equal(value, dialog.Multiselect);

        // Set different.
        dialog.Multiselect = !value;
        Assert.Equal(!value, dialog.Multiselect);
    }

    [WinFormsTheory]
    [BoolData]
    public void OpenFileDialog_ReadOnlyChecked_Set_GetReturnsExpected(bool value)
    {
        using OpenFileDialog dialog = new()
        {
            ReadOnlyChecked = value
        };
        Assert.Equal(value, dialog.ReadOnlyChecked);

        // Set same.
        dialog.ReadOnlyChecked = value;
        Assert.Equal(value, dialog.ReadOnlyChecked);

        // Set different.
        dialog.ReadOnlyChecked = !value;
        Assert.Equal(!value, dialog.ReadOnlyChecked);
    }

    [WinFormsTheory]
    [BoolData]
    public void OpenFileDialog_SelectReadOnly_Set_GetReturnsExpected(bool value)
    {
        using OpenFileDialog dialog = new()
        {
            SelectReadOnly = value
        };
        Assert.Equal(value, dialog.SelectReadOnly);

        // Set same.
        dialog.SelectReadOnly = value;
        Assert.Equal(value, dialog.SelectReadOnly);

        // Set different.
        dialog.SelectReadOnly = !value;
        Assert.Equal(!value, dialog.SelectReadOnly);
    }

    [WinFormsTheory]
    [BoolData]
    public void OpenFileDialog_ShowPreview_Set_GetReturnsExpected(bool value)
    {
        using OpenFileDialog dialog = new()
        {
            ShowPreview = value
        };
        Assert.Equal(value, dialog.ShowPreview);

        // Set same.
        dialog.ShowPreview = value;
        Assert.Equal(value, dialog.ShowPreview);

        // Set different.
        dialog.ShowPreview = !value;
        Assert.Equal(!value, dialog.ShowPreview);
    }

    [WinFormsTheory]
    [BoolData]
    public void OpenFileDialog_ShowReadOnly_Set_GetReturnsExpected(bool value)
    {
        using OpenFileDialog dialog = new()
        {
            ShowReadOnly = value
        };
        Assert.Equal(value, dialog.ShowReadOnly);

        // Set same.
        dialog.ShowReadOnly = value;
        Assert.Equal(value, dialog.ShowReadOnly);

        // Set different.
        dialog.ShowReadOnly = !value;
        Assert.Equal(!value, dialog.ShowReadOnly);
    }

    [WinFormsFact]
    public void OpenFileDialog_Reset_Invoke_Success()
    {
        using OpenFileDialog dialog = new()
        {
            CheckFileExists = false,
            Multiselect = true,
            ReadOnlyChecked = true,
            SelectReadOnly = false,
            ShowPreview = true,
            ShowReadOnly = true
        };

        dialog.Reset();
        Assert.True(dialog.CheckFileExists);
        Assert.False(dialog.Multiselect);
        Assert.False(dialog.ReadOnlyChecked);
        Assert.True(dialog.SelectReadOnly);
        Assert.False(dialog.ShowPreview);
        Assert.False(dialog.ShowReadOnly);
    }
}

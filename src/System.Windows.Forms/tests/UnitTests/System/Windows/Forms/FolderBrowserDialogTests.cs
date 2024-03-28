// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class FolderBrowserDialogTests
{
    [WinFormsFact]
    public void FolderBrowserDialog_Ctor_Default()
    {
        using FolderBrowserDialog dialog = new();
        Assert.True(dialog.AddToRecent);
        Assert.True(dialog.AutoUpgradeEnabled);
        Assert.Null(dialog.Container);
        Assert.Empty(dialog.Description);
        Assert.Equal(Environment.SpecialFolder.Desktop, dialog.RootFolder);
        Assert.Empty(dialog.InitialDirectory);
        Assert.False(dialog.OkRequiresInteraction);
        Assert.False(dialog.Multiselect);
        Assert.Empty(dialog.SelectedPath);
        Assert.Empty(dialog.SelectedPaths);
        Assert.Same(dialog.SelectedPaths, dialog.SelectedPaths);
        Assert.False(dialog.ShowHiddenFiles);
        Assert.True(dialog.ShowPinnedPlaces);
        Assert.True(dialog.ShowNewFolderButton);
        Assert.Null(dialog.Site);
        Assert.Null(dialog.Tag);
        Assert.False(dialog.UseDescriptionForTitle);
        Assert.Null(dialog.ClientGuid);
    }

    [WinFormsTheory]
    [BoolData]
    public void FolderBrowserDialog_AddToRecent_Set_GetReturnsExpected(bool value)
    {
        using FolderBrowserDialog dialog = new()
        {
            AddToRecent = value
        };
        Assert.Equal(value, dialog.AddToRecent);

        // Set same.
        dialog.AddToRecent = value;
        Assert.Equal(value, dialog.AddToRecent);

        // Set different.
        dialog.AddToRecent = !value;
        Assert.Equal(!value, dialog.AddToRecent);
    }

    [WinFormsTheory]
    [BoolData]
    public void FolderBrowserDialog_AutoUpgradeEnabled_Set_GetReturnsExpected(bool value)
    {
        using FolderBrowserDialog dialog = new()
        {
            AutoUpgradeEnabled = value
        };
        Assert.Equal(value, dialog.AutoUpgradeEnabled);

        // Set same.
        dialog.AutoUpgradeEnabled = value;
        Assert.Equal(value, dialog.AutoUpgradeEnabled);

        // Set different.
        dialog.AutoUpgradeEnabled = !value;
        Assert.Equal(!value, dialog.AutoUpgradeEnabled);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void FolderBrowserDialog_Description_Set_GetReturnsExpected(string value)
    {
        using FolderBrowserDialog dialog = new()
        {
            Description = value
        };
        Assert.Equal(value ?? string.Empty, dialog.Description);

        // Set same.
        dialog.Description = value;
        Assert.Equal(value ?? string.Empty, dialog.Description);
    }

    [WinFormsTheory]
    [BoolData]
    public void FolderBrowserDialog_Multiselect_Set_GetReturnsExpected(bool value)
    {
        using var dialog = new FolderBrowserDialog
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
    public void FolderBrowserDialog_OkRequiresInteraction_Set_GetReturnsExpected(bool value)
    {
        using FolderBrowserDialog dialog = new()
        {
            OkRequiresInteraction = value
        };
        Assert.Equal(value, dialog.OkRequiresInteraction);

        // Set same.
        dialog.OkRequiresInteraction = value;
        Assert.Equal(value, dialog.OkRequiresInteraction);

        // Set different.
        dialog.OkRequiresInteraction = !value;
        Assert.Equal(!value, dialog.OkRequiresInteraction);
    }

    [WinFormsTheory]
    [InlineData(Environment.SpecialFolder.Desktop)]
    [InlineData(Environment.SpecialFolder.StartMenu)]
    public void FolderBrowserDialog_RootFolder_Set_GetReturnsExpected(Environment.SpecialFolder value)
    {
        using FolderBrowserDialog dialog = new()
        {
            RootFolder = value
        };
        Assert.Equal(value, dialog.RootFolder);

        // Set same.
        dialog.RootFolder = value;
        Assert.Equal(value, dialog.RootFolder);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void FolderBrowserDialog_InitialDirectory_Set_GetReturnsExpected(string value)
    {
        using FolderBrowserDialog dialog = new()
        {
            InitialDirectory = value
        };
        Assert.Equal(value ?? string.Empty, dialog.InitialDirectory);

        // Set same.
        dialog.InitialDirectory = value;
        Assert.Equal(value ?? string.Empty, dialog.InitialDirectory);
    }

    [WinFormsTheory]
    [InlineData(null, new string[0])]
    [InlineData("", new string[] { "" })]
    [InlineData("selectedPath", new string[] { "selectedPath" })]
    public void FolderBrowserDialog_SelectedPath_Set_GetReturnsExpected(string value, string[] expectedSelectedPaths)
    {
        using FolderBrowserDialog dialog = new()
        {
            SelectedPath = value
        };
        Assert.Equal(value ?? string.Empty, dialog.SelectedPath);
        Assert.Equal(expectedSelectedPaths, dialog.SelectedPaths);
        if (expectedSelectedPaths.Length > 0)
        {
            Assert.Equal(dialog.SelectedPaths, dialog.SelectedPaths);
            Assert.Equal(dialog.SelectedPath, dialog.SelectedPaths[0]);
            Assert.NotSame(dialog.SelectedPaths, dialog.SelectedPaths);
        }
        else
        {
            Assert.Same(dialog.SelectedPaths, dialog.SelectedPaths);
        }

        // Set same.
        dialog.SelectedPath = value;
        Assert.Equal(value ?? string.Empty, dialog.SelectedPath);
        if (expectedSelectedPaths.Length > 0)
        {
            Assert.Equal(dialog.SelectedPaths, dialog.SelectedPaths);
            Assert.Equal(dialog.SelectedPath, dialog.SelectedPaths[0]);
            Assert.NotSame(dialog.SelectedPaths, dialog.SelectedPaths);
        }
        else
        {
            Assert.Same(dialog.SelectedPaths, dialog.SelectedPaths);
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void FolderBrowserDialog_ShowHiddenFiles_Set_GetReturnsExpected(bool value)
    {
        using FolderBrowserDialog dialog = new()
        {
            ShowHiddenFiles = value
        };
        Assert.Equal(value, dialog.ShowHiddenFiles);

        // Set same.
        dialog.ShowHiddenFiles = value;
        Assert.Equal(value, dialog.ShowHiddenFiles);

        // Set different.
        dialog.ShowHiddenFiles = !value;
        Assert.Equal(!value, dialog.ShowHiddenFiles);
    }

    [WinFormsTheory]
    [BoolData]
    public void FileDialog_ShowPinnedPlaces_Set_GetReturnsExpected(bool value)
    {
        using FolderBrowserDialog dialog = new()
        {
            ShowPinnedPlaces = value
        };
        Assert.Equal(value, dialog.ShowPinnedPlaces);

        // Set same.
        dialog.ShowPinnedPlaces = value;
        Assert.Equal(value, dialog.ShowPinnedPlaces);

        // Set different.
        dialog.ShowPinnedPlaces = !value;
        Assert.Equal(!value, dialog.ShowPinnedPlaces);
    }

    [WinFormsTheory]
    [BoolData]
    public void FolderBrowserDialog_ShowNewFolderButton_Set_GetReturnsExpected(bool value)
    {
        using FolderBrowserDialog dialog = new()
        {
            ShowNewFolderButton = value
        };
        Assert.Equal(value, dialog.ShowNewFolderButton);

        // Set same.
        dialog.ShowNewFolderButton = value;
        Assert.Equal(value, dialog.ShowNewFolderButton);

        // Set different.
        dialog.ShowNewFolderButton = !value;
        Assert.Equal(!value, dialog.ShowNewFolderButton);
    }

    [WinFormsTheory]
    [BoolData]
    public void FolderBrowserDialog_UseDescriptionForTitle_Set_GetReturnsExpected(bool value)
    {
        using FolderBrowserDialog dialog = new()
        {
            UseDescriptionForTitle = value
        };
        Assert.Equal(value, dialog.UseDescriptionForTitle);

        // Set same.
        dialog.UseDescriptionForTitle = value;
        Assert.Equal(value, dialog.UseDescriptionForTitle);

        // Set different.
        dialog.UseDescriptionForTitle = !value;
        Assert.Equal(!value, dialog.UseDescriptionForTitle);
    }

    [WinFormsFact]
    public void FolderBrowserDialog_Reset_Invoke_Success()
    {
        using FolderBrowserDialog dialog = new()
        {
            AddToRecent = false,
            AutoUpgradeEnabled = false,
            Description = "A description",
            Multiselect = true,
            RootFolder = Environment.SpecialFolder.CommonAdminTools,
            InitialDirectory = @"C:\",
            OkRequiresInteraction = true,
            SelectedPath = @"C:\",
            ShowHiddenFiles = true,
            ShowPinnedPlaces = false,
            ShowNewFolderButton = false,
            UseDescriptionForTitle = true,
            ClientGuid = new Guid("ad6e2857-4659-4791-aa59-efffa61d4594"),
        };

        dialog.Reset();

        Assert.True(dialog.AddToRecent);
        Assert.False(dialog.AutoUpgradeEnabled);
        Assert.Null(dialog.Container);
        Assert.Empty(dialog.Description);
        Assert.Equal(Environment.SpecialFolder.Desktop, dialog.RootFolder);
        Assert.Empty(dialog.InitialDirectory);
        Assert.False(dialog.OkRequiresInteraction);
        Assert.False(dialog.Multiselect);
        Assert.Empty(dialog.SelectedPath);
        Assert.Empty(dialog.SelectedPaths);
        Assert.Same(dialog.SelectedPaths, dialog.SelectedPaths);
        Assert.False(dialog.ShowHiddenFiles);
        Assert.True(dialog.ShowPinnedPlaces);
        Assert.True(dialog.ShowNewFolderButton);
        Assert.Null(dialog.Site);
        Assert.Null(dialog.Tag);
        Assert.True(dialog.UseDescriptionForTitle);
        Assert.Null(dialog.ClientGuid);
    }

    [WinFormsFact]
    public void FolderBrowserDialog_HelpRequest_AddRemove_Success()
    {
        using FolderBrowserDialog dialog = new();
        int callCount = 0;
        EventHandler handler = (sender, e) => callCount++;

        dialog.HelpRequest += handler;
        dialog.HelpRequest -= handler;
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("1d5a0215-fa19-4e3b-8ab9-06da88c28ae7")]
    public void FolderBrowserDialog_ClientGuid_Set_GetReturnsExpected(Guid value)
    {
        using FolderBrowserDialog dialog = new() { ClientGuid = value };
        Assert.Equal(value, dialog.ClientGuid);

        // Set same.
        dialog.ClientGuid = value;
        Assert.Equal(value, dialog.ClientGuid);
    }
}

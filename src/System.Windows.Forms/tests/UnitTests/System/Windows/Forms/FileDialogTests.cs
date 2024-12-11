// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Windows.Win32.UI.Controls.Dialogs;

namespace System.Windows.Forms.Tests;

public class FileDialogTests
{
    [WinFormsFact]
    public void FileDialog_Ctor_Default()
    {
        using SubFileDialog dialog = new();
        Assert.True(dialog.AddExtension);
        Assert.True(dialog.AddToRecent);
        Assert.True(dialog.AutoUpgradeEnabled);
        Assert.True(dialog.CanRaiseEvents);
        Assert.False(dialog.CheckFileExists);
        Assert.True(dialog.CheckPathExists);
        Assert.Null(dialog.Container);
        Assert.Empty(dialog.CustomPlaces);
        Assert.Same(dialog.CustomPlaces, dialog.CustomPlaces);
        Assert.False(dialog.DesignMode);
        Assert.Empty(dialog.DefaultExt);
        Assert.True(dialog.DereferenceLinks);
        Assert.NotNull(dialog.Events);
        Assert.Same(dialog.Events, dialog.Events);
        Assert.Empty(dialog.FileName);
        Assert.Empty(dialog.FileNames);
        Assert.Same(dialog.FileNames, dialog.FileNames);
        Assert.Equal(1, dialog.FilterIndex);
        Assert.Empty(dialog.InitialDirectory);
        Assert.NotEqual(IntPtr.Zero, dialog.Instance);
        Assert.False(dialog.OkRequiresInteraction);
        Assert.Equal(2052, dialog.Options);
        Assert.False(dialog.RestoreDirectory);
        Assert.False(dialog.ShowHelp);
        Assert.False(dialog.ShowHiddenFiles);
        Assert.False(dialog.SupportMultiDottedExtensions);
        Assert.True(dialog.ShowPinnedPlaces);
        Assert.Null(dialog.Site);
        Assert.Null(dialog.Tag);
        Assert.Empty(dialog.Title);
        Assert.True(dialog.ValidateNames);
        Assert.Null(dialog.ClientGuid);
    }

    [WinFormsFact]
    public void FileDialog_Ctor_Default_OverriddenReset()
    {
        using EmptyResetFileDialog dialog = new();
        Assert.False(dialog.AddExtension);
        Assert.True(dialog.AddToRecent);
        Assert.True(dialog.AutoUpgradeEnabled);
        Assert.True(dialog.CanRaiseEvents);
        Assert.False(dialog.CheckFileExists);
        Assert.False(dialog.CheckPathExists);
        Assert.Null(dialog.Container);
        Assert.Empty(dialog.CustomPlaces);
        Assert.Same(dialog.CustomPlaces, dialog.CustomPlaces);
        Assert.False(dialog.DesignMode);
        Assert.Empty(dialog.DefaultExt);
        Assert.True(dialog.DereferenceLinks);
        Assert.NotNull(dialog.Events);
        Assert.Same(dialog.Events, dialog.Events);
        Assert.Empty(dialog.FileName);
        Assert.Empty(dialog.FileNames);
        Assert.Same(dialog.FileNames, dialog.FileNames);
        Assert.Equal(0, dialog.FilterIndex);
        Assert.Empty(dialog.InitialDirectory);
        Assert.NotEqual(IntPtr.Zero, dialog.Instance);
        Assert.False(dialog.OkRequiresInteraction);
        Assert.Equal(0, dialog.Options);
        Assert.False(dialog.RestoreDirectory);
        Assert.False(dialog.ShowHelp);
        Assert.False(dialog.ShowHiddenFiles);
        Assert.True(dialog.ShowPinnedPlaces);
        Assert.False(dialog.SupportMultiDottedExtensions);
        Assert.Null(dialog.Site);
        Assert.Null(dialog.Tag);
        Assert.Empty(dialog.Title);
        Assert.True(dialog.ValidateNames);
        Assert.Null(dialog.ClientGuid);
    }

    [WinFormsFact]
    public void FileDialog_EventFileOk_Get_ReturnsExpected()
    {
        Assert.NotNull(SubFileDialog.EventFileOk);
        Assert.Same(SubFileDialog.EventFileOk, SubFileDialog.EventFileOk);
    }

    [WinFormsTheory]
    [BoolData]
    public void FileDialog_AddExtension_Set_GetReturnsExpected(bool value)
    {
        using SubFileDialog dialog = new()
        {
            AddExtension = value
        };
        Assert.Equal(value, dialog.AddExtension);
        Assert.Equal(2052, dialog.Options);

        // Set same.
        dialog.AddExtension = value;
        Assert.Equal(value, dialog.AddExtension);
        Assert.Equal(2052, dialog.Options);

        // Set different.
        dialog.AddExtension = !value;
        Assert.Equal(!value, dialog.AddExtension);
        Assert.Equal(2052, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 2052, 33556484)]
    [InlineData(false, 33556484, 2052)]
    public void FileDialog_AddToRecent_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFileDialog dialog = new()
        {
            AddToRecent = value
        };
        Assert.Equal(value, dialog.AddToRecent);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.AddToRecent = value;
        Assert.Equal(value, dialog.AddToRecent);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.AddToRecent = !value;
        Assert.Equal(!value, dialog.AddToRecent);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [BoolData]
    public void FileDialog_AutoUpgradeEnabled_Set_GetReturnsExpected(bool value)
    {
        using SubFileDialog dialog = new()
        {
            AutoUpgradeEnabled = value
        };
        Assert.Equal(value, dialog.AutoUpgradeEnabled);
        Assert.Equal(2052, dialog.Options);

        // Set same.
        dialog.AutoUpgradeEnabled = value;
        Assert.Equal(value, dialog.AutoUpgradeEnabled);
        Assert.Equal(2052, dialog.Options);

        // Set different.
        dialog.AutoUpgradeEnabled = !value;
        Assert.Equal(!value, dialog.AutoUpgradeEnabled);
        Assert.Equal(2052, dialog.Options);
    }

    [WinFormsTheory]
    [BoolData]
    public void FileDialog_CheckFileExists_Set_GetReturnsExpected(bool value)
    {
        using SubFileDialog dialog = new()
        {
            CheckFileExists = value
        };
        Assert.Equal(value, dialog.CheckFileExists);
        Assert.Equal(2052, dialog.Options);

        // Set same.
        dialog.CheckFileExists = value;
        Assert.Equal(value, dialog.CheckFileExists);
        Assert.Equal(2052, dialog.Options);

        // Set different.
        dialog.CheckFileExists = !value;
        Assert.Equal(!value, dialog.CheckFileExists);
        Assert.Equal(2052, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 2052, 4)]
    [InlineData(false, 4, 2052)]
    public void FileDialog_CheckPathExists_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFileDialog dialog = new()
        {
            CheckPathExists = value
        };
        Assert.Equal(value, dialog.CheckPathExists);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.CheckPathExists = value;
        Assert.Equal(value, dialog.CheckPathExists);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.CheckPathExists = !value;
        Assert.Equal(!value, dialog.CheckPathExists);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    [InlineData("1d5a0215-fa19-4e3b-8ab9-06da88c28ae7")]
    public void FileDialog_ClientGuid_Set_GetReturnsExpected(Guid value)
    {
        using SubFileDialog dialog = new()
        {
            ClientGuid = value
        };
        Assert.Equal(value, dialog.ClientGuid);

        // Set same.
        dialog.ClientGuid = value;
        Assert.Equal(value, dialog.ClientGuid);
    }

    [WinFormsTheory]
    [InlineData(null, "")]
    [InlineData(".", "")]
    [InlineData(".ext", "ext")]
    [InlineData("..ext", ".ext")]
    [InlineData("ext", "ext")]
    public void FileDialog_DefaultExt_Set_GetReturnsExpected(string value, string expected)
    {
        using SubFileDialog dialog = new()
        {
            DefaultExt = value
        };
        Assert.Equal(expected, dialog.DefaultExt);
        Assert.Equal(2052, dialog.Options);

        // Set same.
        dialog.DefaultExt = value;
        Assert.Equal(expected, dialog.DefaultExt);
        Assert.Equal(2052, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 2052, 1050628)]
    [InlineData(false, 1050628, 2052)]
    public void FileDialog_DereferenceLinks_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFileDialog dialog = new()
        {
            DereferenceLinks = value
        };
        Assert.Equal(value, dialog.DereferenceLinks);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.DereferenceLinks = value;
        Assert.Equal(value, dialog.DereferenceLinks);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.DereferenceLinks = !value;
        Assert.Equal(!value, dialog.DereferenceLinks);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(null, new string[0])]
    [InlineData("", new string[] { "" })]
    [InlineData("fileName", new string[] { "fileName" })]
    public void FileDialog_FileName_Set_GetReturnsExpected(string value, string[] expectedFileNames)
    {
        using SubFileDialog dialog = new()
        {
            FileName = value
        };
        Assert.Equal(value ?? string.Empty, dialog.FileName);
        Assert.Equal(expectedFileNames, dialog.FileNames);
        if (expectedFileNames.Length > 0)
        {
            Assert.Equal(dialog.FileNames, dialog.FileNames);
            Assert.NotSame(dialog.FileNames, dialog.FileNames);
        }
        else
        {
            Assert.Same(dialog.FileNames, dialog.FileNames);
        }

        Assert.Equal(2052, dialog.Options);

        // Set same.
        dialog.FileName = value;
        Assert.Equal(value ?? string.Empty, dialog.FileName);
        if (expectedFileNames.Length > 0)
        {
            Assert.Equal(dialog.FileNames, dialog.FileNames);
            Assert.NotSame(dialog.FileNames, dialog.FileNames);
        }
        else
        {
            Assert.Same(dialog.FileNames, dialog.FileNames);
        }

        Assert.Equal(2052, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("filter|filter")]
    [InlineData("filter|filter|filter|filter")]
    public void FileDialog_Filter_Set_GetReturnsExpected(string value)
    {
        using SubFileDialog dialog = new()
        {
            Filter = value
        };
        Assert.Equal(value ?? string.Empty, dialog.Filter);
        Assert.Equal(2052, dialog.Options);

        // Set same.
        dialog.Filter = value;
        Assert.Equal(value ?? string.Empty, dialog.Filter);
        Assert.Equal(2052, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData("filter")]
    [InlineData("filter|filter|filter")]
    public void FileDialog_Filter_SetInvalid_ThrowsArgumentException(string value)
    {
        using SubFileDialog dialog = new();
        Assert.Throws<ArgumentException>("value", () => dialog.Filter = value);
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void FileDialog_FilterIndex_Set_GetReturnsExpected(int value)
    {
        using SubFileDialog dialog = new()
        {
            FilterIndex = value
        };
        Assert.Equal(value, dialog.FilterIndex);
        Assert.Equal(2052, dialog.Options);

        // Set same.
        dialog.FilterIndex = value;
        Assert.Equal(value, dialog.FilterIndex);
        Assert.Equal(2052, dialog.Options);
    }

    [WinFormsTheory]
    [IntegerData<int>]
    public void FileDialog_InitialDirectory_Set_GetReturnsExpected(string value)
    {
        using SubFileDialog dialog = new()
        {
            InitialDirectory = value
        };
        Assert.Equal(value ?? string.Empty, dialog.InitialDirectory);
        Assert.Equal(2052, dialog.Options);

        // Set same.
        dialog.InitialDirectory = value;
        Assert.Equal(value ?? string.Empty, dialog.InitialDirectory);
        Assert.Equal(2052, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 2099204, 2052)]
    [InlineData(false, 2052, 2099204)]
    public void FileDialog_OkRequiresInteraction_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFileDialog dialog = new()
        {
            OkRequiresInteraction = value
        };
        Assert.Equal(value, dialog.OkRequiresInteraction);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.OkRequiresInteraction = value;
        Assert.Equal(value, dialog.OkRequiresInteraction);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.OkRequiresInteraction = !value;
        Assert.Equal(!value, dialog.OkRequiresInteraction);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 2060, 2052)]
    [InlineData(false, 2052, 2060)]
    public void FileDialog_RestoreDirectory_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFileDialog dialog = new()
        {
            RestoreDirectory = value
        };
        Assert.Equal(value, dialog.RestoreDirectory);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.RestoreDirectory = value;
        Assert.Equal(value, dialog.RestoreDirectory);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.RestoreDirectory = !value;
        Assert.Equal(!value, dialog.RestoreDirectory);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 2068, 2052)]
    [InlineData(false, 2052, 2068)]
    public void FileDialog_ShowHelp_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFileDialog dialog = new()
        {
            ShowHelp = value
        };
        Assert.Equal(value, dialog.ShowHelp);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.ShowHelp = value;
        Assert.Equal(value, dialog.ShowHelp);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.ShowHelp = !value;
        Assert.Equal(!value, dialog.ShowHelp);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 268437508, 2052)]
    [InlineData(false, 2052, 268437508)]
    public void FileDialog_ShowHiddenFiles_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFileDialog dialog = new()
        {
            ShowHiddenFiles = value
        };
        Assert.Equal(value, dialog.ShowHiddenFiles);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.ShowHiddenFiles = value;
        Assert.Equal(value, dialog.ShowHiddenFiles);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.ShowHiddenFiles = !value;
        Assert.Equal(!value, dialog.ShowHiddenFiles);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 2052, 264196)]
    [InlineData(false, 264196, 2052)]
    public void FileDialog_ShowPinnedPlaces_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFileDialog dialog = new()
        {
            ShowPinnedPlaces = value
        };
        Assert.Equal(value, dialog.ShowPinnedPlaces);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.ShowPinnedPlaces = value;
        Assert.Equal(value, dialog.ShowPinnedPlaces);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.ShowPinnedPlaces = !value;
        Assert.Equal(!value, dialog.ShowPinnedPlaces);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    [WinFormsTheory]
    [BoolData]
    public void FileDialog_SupportMultiDottedExtensions_Set_GetReturnsExpected(bool value)
    {
        using SubFileDialog dialog = new()
        {
            SupportMultiDottedExtensions = value
        };
        Assert.Equal(value, dialog.SupportMultiDottedExtensions);
        Assert.Equal(2052, dialog.Options);

        // Set same.
        dialog.SupportMultiDottedExtensions = value;
        Assert.Equal(value, dialog.SupportMultiDottedExtensions);
        Assert.Equal(2052, dialog.Options);

        // Set different.
        dialog.SupportMultiDottedExtensions = !value;
        Assert.Equal(!value, dialog.SupportMultiDottedExtensions);
        Assert.Equal(2052, dialog.Options);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void FileDialog_Title_Set_GetReturnsExpected(string value)
    {
        using SubFileDialog dialog = new()
        {
            Title = value
        };
        Assert.Equal(value ?? string.Empty, dialog.Title);
        Assert.Equal(2052, dialog.Options);

        // Set same.
        dialog.Title = value;
        Assert.Equal(value ?? string.Empty, dialog.Title);
        Assert.Equal(2052, dialog.Options);
    }

    [WinFormsTheory]
    [InlineData(true, 2052, 2308)]
    [InlineData(false, 2308, 2052)]
    public void FileDialog_ValidateNames_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
    {
        using SubFileDialog dialog = new()
        {
            ValidateNames = value
        };
        Assert.Equal(value, dialog.ValidateNames);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set same.
        dialog.ValidateNames = value;
        Assert.Equal(value, dialog.ValidateNames);
        Assert.Equal(expectedOptions, dialog.Options);

        // Set different.
        dialog.ValidateNames = !value;
        Assert.Equal(!value, dialog.ValidateNames);
        Assert.Equal(expectedOptionsAfter, dialog.Options);
    }

    public static IEnumerable<object[]> CancelEventArgs_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new CancelEventArgs() };
    }

    [WinFormsTheory]
    [MemberData(nameof(CancelEventArgs_TestData))]
    public void FileDialog_OnFileOk_Invoke_Success(CancelEventArgs eventArgs)
    {
        using SubFileDialog dialog = new();

        // No handler.
        dialog.OnFileOk(eventArgs);

        // Handler.
        int callCount = 0;
        CancelEventHandler handler = (sender, e) =>
        {
            Assert.Same(dialog, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        dialog.FileOk += handler;
        dialog.OnFileOk(eventArgs);
        Assert.Equal(1, callCount);

        // Should not call if the handler is removed.
        dialog.FileOk -= handler;
        dialog.OnFileOk(eventArgs);
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void FileDialog_Reset_Invoke_Success()
    {
        using SubFileDialog dialog = new()
        {
            AddExtension = false,
            AddToRecent = false,
            AutoUpgradeEnabled = false,
            CheckFileExists = true,
            CheckPathExists = false,
            ClientGuid = new Guid("ad6e2857-4659-4791-aa59-efffa61d4594"),
            DefaultExt = "DefaultExt",
            DereferenceLinks = false,
            FileName = "FileName",
            FilterIndex = 2,
            InitialDirectory = "InitialDirectory",
            OkRequiresInteraction = true,
            RestoreDirectory = true,
            ShowHelp = true,
            ShowHiddenFiles = true,
            ShowPinnedPlaces = false,
            SupportMultiDottedExtensions = true,
            Tag = "Tag",
            Title = "Title",
            ValidateNames = false
        };
        dialog.CustomPlaces.Add("path");

        dialog.Reset();
        Assert.True(dialog.AddExtension);
        Assert.True(dialog.AddToRecent);
        Assert.False(dialog.AutoUpgradeEnabled);
        Assert.True(dialog.CanRaiseEvents);
        Assert.False(dialog.CheckFileExists);
        Assert.True(dialog.CheckPathExists);
        Assert.Null(dialog.Container);
        Assert.Empty(dialog.CustomPlaces);
        Assert.Same(dialog.CustomPlaces, dialog.CustomPlaces);
        Assert.False(dialog.DesignMode);
        Assert.Empty(dialog.DefaultExt);
        Assert.True(dialog.DereferenceLinks);
        Assert.NotNull(dialog.Events);
        Assert.Same(dialog.Events, dialog.Events);
        Assert.Empty(dialog.FileName);
        Assert.Empty(dialog.FileNames);
        Assert.Same(dialog.FileNames, dialog.FileNames);
        Assert.Equal(1, dialog.FilterIndex);
        Assert.Empty(dialog.InitialDirectory);
        Assert.NotEqual(IntPtr.Zero, dialog.Instance);
        Assert.False(dialog.OkRequiresInteraction);
        Assert.Equal(2052, dialog.Options);
        Assert.False(dialog.RestoreDirectory);
        Assert.False(dialog.ShowHelp);
        Assert.False(dialog.ShowHiddenFiles);
        Assert.True(dialog.ShowPinnedPlaces);
        Assert.False(dialog.SupportMultiDottedExtensions);
        Assert.Null(dialog.Site);
        Assert.Equal("Tag", dialog.Tag);
        Assert.Empty(dialog.Title);
        Assert.True(dialog.ValidateNames);
        Assert.Null(dialog.ClientGuid);
    }

    [WinFormsTheory]
    [BoolData]
    public unsafe void FileDialog_RunDialog_NonVista_Success(bool result)
    {
        using SubFileDialog dialog = new()
        {
            AutoUpgradeEnabled = false
        };
        dialog.RunFileDialogAction = o =>
        {
            Assert.Equal(sizeof(OPENFILENAME), (int)o.lStructSize);
            Assert.Equal((HWND)(nint)1, o.hwndOwner);
            Assert.Equal(dialog.Instance, o.hInstance);
            ReadOnlySpan<char> filter = new(o.lpstrFilter.Value, o.lpstrFilter.StringListLength);
            Assert.True(filter.SequenceEqual(" \0*.*"));
            Assert.True(o.lpstrCustomFilter.IsNull);
            Assert.Equal(0u, o.nMaxCustFilter);
            Assert.Equal(1u, o.nFilterIndex);
            Assert.False(o.lpstrFile.IsNull);
            Assert.Equal(8192u, o.nMaxFile);
            Assert.True(o.lpstrFileTitle.IsNull);
            Assert.Equal(0u, o.nMaxFileTitle);
            Assert.True(o.lpstrInitialDir.IsNull);
            Assert.True(o.lpstrTitle.IsNull);
            Assert.Equal((OPEN_FILENAME_FLAGS)8914980, o.Flags);
            Assert.Equal(0, o.nFileOffset);
            Assert.Equal(0, o.nFileExtension);
            Assert.True(o.lpstrDefExt.IsNull);
            Assert.Equal((LPARAM)0, o.lCustData);
            Assert.False(o.lpfnHook is null);
            Assert.True(o.lpTemplateName.IsNull);
            Assert.True(o.pvReserved is null);
            Assert.Equal(0u, o.dwReserved);
            Assert.Equal((OPEN_FILENAME_FLAGS_EX)0, o.FlagsEx);
            return result;
        };
        Assert.Equal(result, dialog.RunDialog(1));
    }

    [WinFormsTheory]
    [BoolData]
    public unsafe void FileDialog_RunDialog_NonVistaAdvanced_Success(bool result)
    {
        using SubFileDialog dialog = new()
        {
            AddExtension = result,
            AddToRecent = false,
            AutoUpgradeEnabled = false,
            CheckFileExists = true,
            CheckPathExists = false,
            DefaultExt = "DefaultExt",
            DereferenceLinks = false,
            FileName = "FileName",
            FilterIndex = 2,
            InitialDirectory = "InitialDirectory",
            OkRequiresInteraction = true,
            RestoreDirectory = true,
            ShowHelp = true,
            ShowHiddenFiles = true,
            ShowPinnedPlaces = false,
            SupportMultiDottedExtensions = true,
            Tag = "Tag",
            Title = "Title",
            ValidateNames = false
        };

        dialog.RunFileDialogAction = o =>
        {
            Assert.Equal(sizeof(OPENFILENAME), (int)o.lStructSize);
            Assert.Equal((IntPtr)1, o.hwndOwner);
            Assert.Equal(dialog.Instance, o.hInstance);
            Assert.True(o.lpstrFilter.IsNull);
            Assert.True(o.lpstrCustomFilter.IsNull);
            Assert.Equal(0u, o.nMaxCustFilter);
            Assert.Equal(2u, o.nFilterIndex);
            Assert.False(o.lpstrFile.IsNull);
            Assert.Equal(8192u, o.nMaxFile);
            Assert.True(o.lpstrFileTitle.IsNull);
            Assert.Equal(0u, o.nMaxFileTitle);
            Assert.Equal("InitialDirectory", o.lpstrInitialDir.ToString());
            Assert.Equal("Title", o.lpstrTitle.ToString());
            Assert.Equal((OPEN_FILENAME_FLAGS)314310972, o.Flags);
            Assert.Equal(0u, o.nFileOffset);
            Assert.Equal(0u, o.nFileExtension);
            Assert.Equal(result ? "DefaultExt" : null, o.lpstrDefExt.ToString());
            Assert.Equal((LPARAM)0, o.lCustData);
            Assert.False(o.lpfnHook is null);
            Assert.True(o.lpTemplateName.IsNull);
            Assert.True(o.pvReserved is null);
            Assert.Equal(0u, o.dwReserved);
            Assert.Equal((OPEN_FILENAME_FLAGS_EX)0, o.FlagsEx);
            return result;
        };
        Assert.Equal(result, dialog.RunDialog(1));
    }

    [WinFormsTheory]
    [BoolData]
    public unsafe void FileDialog_RunDialog_ShowHelp_Success(bool result)
    {
        using SubFileDialog dialog = new()
        {
            ShowHelp = true
        };
        dialog.RunFileDialogAction = o =>
        {
            Assert.Equal(sizeof(OPENFILENAME), (int)o.lStructSize);
            Assert.Equal((HWND)(nint)1, o.hwndOwner);
            Assert.Equal(dialog.Instance, o.hInstance);
            ReadOnlySpan<char> filter = new(o.lpstrFilter.Value, o.lpstrFilter.StringListLength);
            Assert.True(filter.SequenceEqual(" \0*.*"));
            Assert.True(o.lpstrCustomFilter.IsNull);
            Assert.Equal(0u, o.nMaxCustFilter);
            Assert.Equal(1u, o.nFilterIndex);
            Assert.False(o.lpstrFile.IsNull);
            Assert.Equal(8192u, o.nMaxFile);
            Assert.True(o.lpstrFileTitle.IsNull);
            Assert.Equal(0u, o.nMaxFileTitle);
            Assert.True(o.lpstrInitialDir.IsNull);
            Assert.True(o.lpstrTitle.IsNull);
            Assert.Equal((OPEN_FILENAME_FLAGS)8914996, o.Flags);
            Assert.Equal(0u, o.nFileOffset);
            Assert.Equal(0u, o.nFileExtension);
            Assert.True(o.lpstrDefExt.IsNull);
            Assert.Equal((LPARAM)0, o.lCustData);
            Assert.False(o.lpfnHook is null);
            Assert.True(o.lpTemplateName.IsNull);
            Assert.True(o.pvReserved is null);
            Assert.Equal(0u, o.dwReserved);
            Assert.Equal((OPEN_FILENAME_FLAGS_EX)0, o.FlagsEx);
            return result;
        };
        Assert.Equal(result, dialog.RunDialog(1));
    }

    public static IEnumerable<object[]> ToString_TestData()
    {
        yield return new object[] { new SubFileDialog(), "System.Windows.Forms.Tests.FileDialogTests+SubFileDialog: Title: , FileName: " };
        yield return new object[] { new SubFileDialog { Title = "Title", FileName = "FileName" }, "System.Windows.Forms.Tests.FileDialogTests+SubFileDialog: Title: Title, FileName: FileName" };
    }

    [WinFormsTheory]
    [MemberData(nameof(ToString_TestData))]
    public void FileDialog_ToString_Invoke_ReturnsExpected(FileDialog dialog, string expected)
    {
        Assert.Equal(expected, dialog.ToString());
    }

    [WinFormsFact]
    public void FileDialog_GetMultiselectFiles_ReturnsExpected()
    {
        // Test with directory
        var accessor = typeof(FileDialog).TestAccessor();
        string buffer = "C:\\test\0testfile.txt\0testfile2.txt\0";
        string[] expected = ["C:\\test\\testfile.txt", "C:\\test\\testfile2.txt"];
        string[] result = accessor.CreateDelegate<GetMultiselectFiles>()(buffer);
        Assert.Equal(expected, result);

        // Test without directory
        buffer = "C:\\\0testfile.txt\0testfile2.txt\0";
        expected = ["C:\\testfile.txt", "C:\\testfile2.txt"];
        result = accessor.CreateDelegate<GetMultiselectFiles>()(buffer);
        Assert.Equal(expected, result);

        // Test single file with directory
        buffer = "C:\\test\\testfile.txt\0";
        expected = ["C:\\test\\testfile.txt"];
        result = accessor.CreateDelegate<GetMultiselectFiles>()(buffer);
        Assert.Equal(expected, result);

        // Test single file without directory
        buffer = "C:\\testfile.txt\0";
        expected = ["C:\\testfile.txt"];
        result = accessor.CreateDelegate<GetMultiselectFiles>()(buffer);
        Assert.Equal(expected, result);
    }

    private delegate string[] GetMultiselectFiles(ReadOnlySpan<char> fileBuffer);

    private unsafe class SubFileDialog : FileDialog
    {
#pragma warning disable IDE1006 // Naming Styles
        public static new readonly object EventFileOk = FileDialog.EventFileOk;
#pragma warning restore IDE1006

        public new bool CanRaiseEvents => base.CanRaiseEvents;

        public new bool DesignMode => base.DesignMode;

        public new EventHandlerList Events => base.Events;

        public new IntPtr Instance => base.Instance;

        public new int Options => base.Options;

        public Func<OPENFILENAME, bool> RunFileDialogAction { get; set; }

        private protected override bool RunFileDialog(OPENFILENAME* ofn)
        {
            return RunFileDialogAction(*ofn);
        }

        private protected override ComScope<IFileDialog> CreateVistaDialog() => default;

        private protected override string[] ProcessVistaFiles(IFileDialog* dialog) => null;

        public new void OnFileOk(CancelEventArgs e) => base.OnFileOk(e);

        public new bool RunDialog(IntPtr hWndOwner) => base.RunDialog(hWndOwner);
    }

    private class EmptyResetFileDialog : SubFileDialog
    {
        public override void Reset()
        {
        }
    }
}

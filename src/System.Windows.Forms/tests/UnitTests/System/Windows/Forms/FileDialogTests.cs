// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using WinForms.Common.Tests;
using Xunit;
using static Interop.Shell32;

namespace System.Windows.Forms.Tests
{
    public class FileDialogTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void FileDialog_Ctor_Default()
        {
            using var dialog = new SubFileDialog();
            Assert.True(dialog.AddExtension);
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
            Assert.Equal(2052, dialog.Options);
            Assert.False(dialog.RestoreDirectory);
            Assert.False(dialog.ShowHelp);
            Assert.False(dialog.SupportMultiDottedExtensions);
            Assert.Null(dialog.Site);
            Assert.Null(dialog.Tag);
            Assert.Empty(dialog.Title);
            Assert.True(dialog.ValidateNames);
            Assert.Null(dialog.ClientGuid);
        }

        [WinFormsFact]
        public void FileDialog_Ctor_Default_OverridenReset()
        {
            using var dialog = new EmptyResetFileDialog();
            Assert.False(dialog.AddExtension);
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
            Assert.Equal(0, dialog.Options);
            Assert.False(dialog.RestoreDirectory);
            Assert.False(dialog.ShowHelp);
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
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FileDialog_AddExtension_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new SubFileDialog
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
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FileDialog_AutoUpgradeEnabled_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new SubFileDialog
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
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FileDialog_CheckFileExists_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new SubFileDialog
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
            using var dialog = new SubFileDialog
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
            using var dialog = new SubFileDialog
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
            using var dialog = new SubFileDialog
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
            using var dialog = new SubFileDialog
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
            using var dialog = new SubFileDialog
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
            using var dialog = new SubFileDialog
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
            using var dialog = new SubFileDialog();
            Assert.Throws<ArgumentException>("value", () => dialog.Filter = value);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void FileDialog_FilterIndex_Set_GetReturnsExpected(int value)
        {
            using var dialog = new SubFileDialog
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
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void FileDialog_InitialDirectory_Set_GetReturnsExpected(string value)
        {
            using var dialog = new SubFileDialog
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
        [InlineData(true, 2060, 2052)]
        [InlineData(false, 2052, 2060)]
        public void FileDialog_RestoreDirectory_Set_GetReturnsExpected(bool value, int expectedOptions, int expectedOptionsAfter)
        {
            using var dialog = new SubFileDialog
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
            using var dialog = new SubFileDialog
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
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FileDialog_SupportMultiDottedExtensions_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new SubFileDialog
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
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void FileDialog_Title_Set_GetReturnsExpected(string value)
        {
            using var dialog = new SubFileDialog
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
            using var dialog = new SubFileDialog
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
            using var dialog = new SubFileDialog();

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
            using var dialog = new SubFileDialog
            {
                AddExtension = false,
                AutoUpgradeEnabled = false,
                CheckFileExists = true,
                CheckPathExists = false,
                ClientGuid = new Guid("ad6e2857-4659-4791-aa59-efffa61d4594"),
                DefaultExt = "DefaultExt",
                DereferenceLinks = false,
                FileName = "FileName",
                FilterIndex = 2,
                InitialDirectory = "InitialDirectory",
                RestoreDirectory = true,
                ShowHelp = true,
                SupportMultiDottedExtensions = true,
                Tag = "Tag",
                Title = "Title",
                ValidateNames = false
            };
            dialog.CustomPlaces.Add("path");

            dialog.Reset();
            Assert.True(dialog.AddExtension);
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
            Assert.Equal(2052, dialog.Options);
            Assert.False(dialog.RestoreDirectory);
            Assert.False(dialog.ShowHelp);
            Assert.False(dialog.SupportMultiDottedExtensions);
            Assert.Null(dialog.Site);
            Assert.Equal("Tag", dialog.Tag);
            Assert.Empty(dialog.Title);
            Assert.True(dialog.ValidateNames);
            Assert.Null(dialog.ClientGuid);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FileDialog_RunDialog_NonVista_Success(bool result)
        {
            using var dialog = new SubFileDialog
            {
                AutoUpgradeEnabled = false
            };
            dialog.RunFileDialogAction = o =>
            {
                Assert.Equal(Marshal.SizeOf<NativeMethods.OPENFILENAME_I>(), o.lStructSize);
                Assert.Equal((IntPtr)1, o.hwndOwner);
                Assert.Equal(dialog.Instance, o.hInstance);
                Assert.Equal(" \0*.*\0\0", o.lpstrFilter);
                Assert.Equal(IntPtr.Zero, o.lpstrCustomFilter);
                Assert.Equal(0, o.nMaxCustFilter);
                Assert.Equal(1, o.nFilterIndex);
                Assert.NotEqual(IntPtr.Zero, o.lpstrFile);
                Assert.Equal(8192, o.nMaxFile);
                Assert.Equal(IntPtr.Zero, o.lpstrFileTitle);
                Assert.Equal(260, o.nMaxFileTitle);
                Assert.Null(o.lpstrInitialDir);
                Assert.Null(o.lpstrTitle);
                Assert.Equal(8914980, o.Flags);
                Assert.Equal(0, o.nFileOffset);
                Assert.Equal(0, o.nFileExtension);
                Assert.Null(o.lpstrDefExt);
                Assert.Equal(IntPtr.Zero, o.lCustData);
                Assert.NotNull(o.lpfnHook);
                Assert.Null(o.lpTemplateName);
                Assert.Equal(IntPtr.Zero, o.pvReserved);
                Assert.Equal(0, o.dwReserved);
                Assert.Equal(0, o.FlagsEx);
                return result;
            };
            Assert.Equal(result, dialog.RunDialog((IntPtr)1));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FileDialog_RunDialog_NonVistaAdvanced_Success(bool result)
        {
            using var dialog = new SubFileDialog
            {
                AddExtension = result,
                AutoUpgradeEnabled = false,
                CheckFileExists = true,
                CheckPathExists = false,
                DefaultExt = "DefaultExt",
                DereferenceLinks = false,
                FileName = "FileName",
                FilterIndex = 2,
                InitialDirectory = "InitialDirectory",
                RestoreDirectory = true,
                ShowHelp = true,
                SupportMultiDottedExtensions = true,
                Tag = "Tag",
                Title = "Title",
                ValidateNames = false
            };
            dialog.RunFileDialogAction = o =>
            {
                Assert.Equal(Marshal.SizeOf<NativeMethods.OPENFILENAME_I>(), o.lStructSize);
                Assert.Equal((IntPtr)1, o.hwndOwner);
                Assert.Equal(dialog.Instance, o.hInstance);
                Assert.Null(o.lpstrFilter);
                Assert.Equal(IntPtr.Zero, o.lpstrCustomFilter);
                Assert.Equal(0, o.nMaxCustFilter);
                Assert.Equal(2, o.nFilterIndex);
                Assert.NotEqual(IntPtr.Zero, o.lpstrFile);
                Assert.Equal(8192, o.nMaxFile);
                Assert.Equal(IntPtr.Zero, o.lpstrFileTitle);
                Assert.Equal(260, o.nMaxFileTitle);
                Assert.Equal("InitialDirectory", o.lpstrInitialDir);
                Assert.Equal("Title", o.lpstrTitle);
                Assert.Equal(9961788, o.Flags);
                Assert.Equal(0, o.nFileOffset);
                Assert.Equal(0, o.nFileExtension);
                Assert.Equal(result ? "DefaultExt" : null, o.lpstrDefExt);
                Assert.Equal(IntPtr.Zero, o.lCustData);
                Assert.NotNull(o.lpfnHook);
                Assert.Null(o.lpTemplateName);
                Assert.Equal(IntPtr.Zero, o.pvReserved);
                Assert.Equal(0, o.dwReserved);
                Assert.Equal(0, o.FlagsEx);
                return result;
            };
            Assert.Equal(result, dialog.RunDialog((IntPtr)1));
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FileDialog_RunDialog_ShowHelp_Success(bool result)
        {
            using var dialog = new SubFileDialog
            {
                ShowHelp = true
            };
            dialog.RunFileDialogAction = o =>
            {
                Assert.Equal(Marshal.SizeOf<NativeMethods.OPENFILENAME_I>(), o.lStructSize);
                Assert.Equal((IntPtr)1, o.hwndOwner);
                Assert.Equal(dialog.Instance, o.hInstance);
                Assert.Equal(" \0*.*\0\0", o.lpstrFilter);
                Assert.Equal(IntPtr.Zero, o.lpstrCustomFilter);
                Assert.Equal(0, o.nMaxCustFilter);
                Assert.Equal(1, o.nFilterIndex);
                Assert.NotEqual(IntPtr.Zero, o.lpstrFile);
                Assert.Equal(8192, o.nMaxFile);
                Assert.Equal(IntPtr.Zero, o.lpstrFileTitle);
                Assert.Equal(260, o.nMaxFileTitle);
                Assert.Null(o.lpstrInitialDir);
                Assert.Null(o.lpstrTitle);
                Assert.Equal(8914996, o.Flags);
                Assert.Equal(0, o.nFileOffset);
                Assert.Equal(0, o.nFileExtension);
                Assert.Null(o.lpstrDefExt);
                Assert.Equal(IntPtr.Zero, o.lCustData);
                Assert.NotNull(o.lpfnHook);
                Assert.Null(o.lpTemplateName);
                Assert.Equal(IntPtr.Zero, o.pvReserved);
                Assert.Equal(0, o.dwReserved);
                Assert.Equal(0, o.FlagsEx);
                return result;
            };
            Assert.Equal(result, dialog.RunDialog((IntPtr)1));
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

        private class SubFileDialog : FileDialog
        {
            public new static readonly object EventFileOk = FileDialog.EventFileOk;

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new bool DesignMode => base.DesignMode;

            public new EventHandlerList Events => base.Events;

            public new IntPtr Instance => base.Instance;

            public new int Options => base.Options;

            public Func<NativeMethods.OPENFILENAME_I, bool> RunFileDialogAction { get; set; }

            private protected override bool RunFileDialog(NativeMethods.OPENFILENAME_I ofn)
            {
                return RunFileDialogAction(ofn);
            }

            private protected override IFileDialog CreateVistaDialog() => null;

            private protected override string[] ProcessVistaFiles(IFileDialog dialog) => null;

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
}

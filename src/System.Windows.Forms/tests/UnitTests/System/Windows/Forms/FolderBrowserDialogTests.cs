// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows.Forms.TestUtilities;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class FolderBrowserDialogTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void FolderBrowserDialog_Ctor_Default()
        {
            using var dialog = new FolderBrowserDialog();
            Assert.True(dialog.AddToRecent);
            Assert.True(dialog.AutoUpgradeEnabled);
            Assert.Null(dialog.Container);
            Assert.Empty(dialog.Description);
            Assert.Equal(Environment.SpecialFolder.Desktop, dialog.RootFolder);
            Assert.Empty(dialog.InitialDirectory);
            Assert.False(dialog.OkRequiresInteraction);
            Assert.Empty(dialog.SelectedPath);
            Assert.False(dialog.ShowHiddenFiles);
            Assert.True(dialog.ShowPinnedPlaces);
            Assert.True(dialog.ShowNewFolderButton);
            Assert.Null(dialog.Site);
            Assert.Null(dialog.Tag);
            Assert.False(dialog.UseDescriptionForTitle);
            Assert.Null(dialog.ClientGuid);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FolderBrowserDialog_AddToRecent_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new FolderBrowserDialog
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
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FolderBrowserDialog_AutoUpgradeEnabled_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new FolderBrowserDialog
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
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void FolderBrowserDialog_Description_Set_GetReturnsExpected(string value)
        {
            using var dialog = new FolderBrowserDialog
            {
                Description = value
            };
            Assert.Equal(value ?? string.Empty, dialog.Description);

            // Set same.
            dialog.Description = value;
            Assert.Equal(value ?? string.Empty, dialog.Description);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FolderBrowserDialog_OkRequiresInteraction_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new FolderBrowserDialog
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
            using var dialog = new FolderBrowserDialog
            {
                RootFolder = value
            };
            Assert.Equal(value, dialog.RootFolder);

            // Set same.
            dialog.RootFolder = value;
            Assert.Equal(value, dialog.RootFolder);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(Environment.SpecialFolder))]
        public void FolderBrowserDialog_RootFolder_SetInvalid_ThrowsInvalidEnumArgumentException(Environment.SpecialFolder value)
        {
            using var dialog = new FolderBrowserDialog();
            Assert.Throws<InvalidEnumArgumentException>("value", () => dialog.RootFolder = value);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void FolderBrowserDialog_InitialDirectory_Set_GetReturnsExpected(string value)
        {
            using var dialog = new FolderBrowserDialog
            {
                InitialDirectory = value
            };
            Assert.Equal(value ?? string.Empty, dialog.InitialDirectory);

            // Set same.
            dialog.InitialDirectory = value;
            Assert.Equal(value ?? string.Empty, dialog.InitialDirectory);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void FolderBrowserDialog_SelectedPath_Set_GetReturnsExpected(string value)
        {
            using var dialog = new FolderBrowserDialog
            {
                SelectedPath = value
            };
            Assert.Equal(value ?? string.Empty, dialog.SelectedPath);

            // Set same.
            dialog.SelectedPath = value;
            Assert.Equal(value ?? string.Empty, dialog.SelectedPath);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FolderBrowserDialog_ShowHiddenFiles_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new FolderBrowserDialog
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
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FileDialog_ShowPinnedPlaces_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new FolderBrowserDialog
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
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FolderBrowserDialog_ShowNewFolderButton_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new FolderBrowserDialog
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
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FolderBrowserDialog_UseDescriptionForTitle_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new FolderBrowserDialog
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
            using var dialog = new FolderBrowserDialog
            {
                AddToRecent = false,
                AutoUpgradeEnabled = false,
                Description = "A description",
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
            Assert.Empty(dialog.SelectedPath);
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
            using var dialog = new FolderBrowserDialog();
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
            using var dialog = new FolderBrowserDialog { ClientGuid = value };
            Assert.Equal(value, dialog.ClientGuid);

            // Set same.
            dialog.ClientGuid = value;
            Assert.Equal(value, dialog.ClientGuid);
        }
    }
}

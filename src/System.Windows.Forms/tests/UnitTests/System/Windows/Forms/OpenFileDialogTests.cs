// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.TestUtilities;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class OpenFileDialogTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void OpenFileDialog_Ctor_Default()
        {
            using var dialog = new OpenFileDialog();
            Assert.True(dialog.CheckFileExists);
            Assert.False(dialog.Multiselect);
            Assert.False(dialog.ReadOnlyChecked);
            Assert.True(dialog.SelectReadOnly);
            Assert.False(dialog.ShowPreview);
            Assert.False(dialog.ShowReadOnly);
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void OpenFileDialog_CheckFileExists_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new OpenFileDialog
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
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void OpenFileDialog_Multiselect_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new OpenFileDialog
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
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void OpenFileDialog_ReadOnlyChecked_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new OpenFileDialog
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
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void OpenFileDialog_SelectReadOnly_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new OpenFileDialog
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
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void OpenFileDialog_ShowPreview_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new OpenFileDialog
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
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void OpenFileDialog_ShowReadOnly_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new OpenFileDialog
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
            using var dialog = new OpenFileDialog
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
}

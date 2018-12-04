// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class FolderBrowserDialogTests
    {
        [Fact]
        public void FolderBrowserDialog_Constructor()
        {
            var dialog = new FolderBrowserDialog();

            Assert.Equal(string.Empty, dialog.SelectedPath);
            Assert.True(dialog.AutoUpgradeEnabled);
            Assert.Equal(string.Empty, dialog.Description);
            Assert.False(dialog.UseDescriptionForTitle);
        }

        [Fact]
        public void FolderBrowserDialog_Reset()
        {
            var dialog = new FolderBrowserDialog
            {
                Description = "A description",
                UseDescriptionForTitle = true,
                SelectedPath = @"C:\"
            };

            dialog.Reset();

            Assert.Equal(string.Empty, dialog.SelectedPath);
            Assert.True(dialog.AutoUpgradeEnabled);
            Assert.Equal(string.Empty, dialog.Description);
        }
    }
}

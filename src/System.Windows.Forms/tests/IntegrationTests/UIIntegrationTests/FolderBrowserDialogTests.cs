// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class FolderBrowserDialogTests : ControlTestBase
    {
        public FolderBrowserDialogTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsTheory]
        [InlineData(true)]
        [InlineData(false)]
        public void FolderBrowserDialog_ShowDialog(bool autoUpgradeEnabled)
        {
            using FolderBrowserDialog dialog = new()
            {
                AutoUpgradeEnabled = autoUpgradeEnabled,
            };

            bool dialogDismissed = false;

            using Timer timer = new();
            timer.Interval = 1_000;

            timer.Tick += (s, e) =>
            {
                dialogDismissed = true;
                timer.Stop();

                // Forcefully close the dialog
                Application.Exit();
            };

            timer.Start();
            dialog.ShowDialog();

            // The dialog has opened and closed successfully
            Assert.True(dialogDismissed);
        }
    }
}

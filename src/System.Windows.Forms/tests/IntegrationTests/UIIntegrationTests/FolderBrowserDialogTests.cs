// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WindowsInput;
using WindowsInput.Native;
using Xunit;
using Xunit.Abstractions;
using static Interop.Ole32;

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

            using Timer timer = new();
            timer.Interval = 1_000;
            int counter = 0;
            bool failedClose = false;
            timer.Tick += (s, e) =>
            {
                counter++;
                if (counter > 2)
                {
                    timer.Stop();
                    failedClose=true;
                    Application.Exit();
                }

                // Close the dialog
                new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
            };

            timer.Start();
            dialog.ShowDialog();

            // The dialog has opened and closed successfully
            Assert.False(failedClose, "Failed to close the dialog");
        }
    }
}

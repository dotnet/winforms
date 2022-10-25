// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WindowsInput;
using WindowsInput.Native;
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
            FolderBrowserDialog form = new()
            {
                AutoUpgradeEnabled = autoUpgradeEnabled,
            };

            Timer timer = new();
            timer.Interval = 1_000;
            int counter = 0;
            timer.Tick += (s, e) =>
            {
                counter++;
                if (counter > 2)
                    throw new TimeoutException("Failed to close the dialog");

                new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
            };

            timer.Start();
            form.ShowDialog();

            // The dialog has opened and closed successfully
            Assert.True(true);
        }
    }
}

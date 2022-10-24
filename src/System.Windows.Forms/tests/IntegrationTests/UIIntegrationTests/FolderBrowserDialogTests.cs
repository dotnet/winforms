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

        [WinFormsFact]
        public void FolderBrowserDialog_ShowDialog()
        {
            FolderBrowserDialog form = new()
            {
                AutoUpgradeEnabled = false,
            };

            Timer timer = new();
            timer.Interval = 3000;
            timer.Tick += (s, e) =>
            {
                ((Timer)s).Stop();
                new InputSimulator().Keyboard.KeyPress(VirtualKeyCode.ESCAPE);
            };

            timer.Start();
            form.ShowDialog();
        }

    }
}

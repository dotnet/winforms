// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using WindowsInput;
using WindowsInput.Native;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;
using static Interop;

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

            bool dialogClosed = false;
            timer.Tick += (s, e) =>
            {
                counter++;
                timer.Stop();

                User32.EnumWindows(FindAndCloseDialog);

                if (!dialogClosed)
                {
                    if (counter > 2)
                    {
                        Application.Exit();
                        return;
                    }

                    TestOutputHelper?.WriteLine($"Couldn't find the dialog ({counter}/3). Retrying...");
                    timer.Start();
                }
            };

            timer.Start();
            dialog.ShowDialog();

            // The dialog has opened and closed successfully
            Assert.True(dialogClosed, "Failed to close the dialog");
            return;

            unsafe BOOL FindAndCloseDialog(HWND hwnd)
            {
                uint processId;
                PInvoke.GetWindowThreadProcessId(hwnd, &processId);
                if (processId == PInvoke.GetCurrentProcessId() && PInvoke.IsWindowVisible(hwnd))
                {
                    TestOutputHelper?.WriteLine($"Process ID: {processId}");
                    TestOutputHelper?.WriteLine($"Dialog window found: 0x{hwnd.Value:X8}");

                    PInvoke.SendMessage(hwnd, User32.WM.CLOSE);

                    dialogClosed = true;
                }

                return true;
            }
        }
    }
}

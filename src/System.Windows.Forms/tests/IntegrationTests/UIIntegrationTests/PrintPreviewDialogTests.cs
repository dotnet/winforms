﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WindowsInput.Native;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class PrintPreviewDialogTests : ControlTestBase
    {
        public PrintPreviewDialogTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsFact]
        public async Task PrintPreviewDialog_Hotkey_Ctrl_1Async()
        {
            await RunTestAsync(async printPreviewDialog =>
            {
                await InputSimulator.SendAsync(
                    printPreviewDialog,
                    inputSimulator => inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_1));

                Assert.Equal(1, printPreviewDialog.PrintPreviewControl.Rows);
                Assert.Equal(1, printPreviewDialog.PrintPreviewControl.Columns);
            });
        }

        [WinFormsFact]
        public async Task PrintPreviewDialog_Hotkey_Ctrl_2Async()
        {
            await RunTestAsync(async printPreviewDialog =>
            {
                await InputSimulator.SendAsync(
                    printPreviewDialog,
                    inputSimulator => inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_2));

                Assert.Equal(1, printPreviewDialog.PrintPreviewControl.Rows);
                Assert.Equal(2, printPreviewDialog.PrintPreviewControl.Columns);
            });
        }

        [WinFormsFact]
        public async Task PrintPreviewDialog_Hotkey_Ctrl_3Async()
        {
            await RunTestAsync(async printPreviewDialog =>
            {
                await InputSimulator.SendAsync(
                    printPreviewDialog,
                    inputSimulator => inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_3));

                Assert.Equal(1, printPreviewDialog.PrintPreviewControl.Rows);
                Assert.Equal(3, printPreviewDialog.PrintPreviewControl.Columns);
            });
        }

        [WinFormsFact]
        public async Task PrintPreviewDialog_Hotkey_Ctrl_4Async()
        {
            await RunTestAsync(async printPreviewDialog =>
            {
                await InputSimulator.SendAsync(
                    printPreviewDialog,
                    inputSimulator => inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_4));

                Assert.Equal(2, printPreviewDialog.PrintPreviewControl.Rows);
                Assert.Equal(2, printPreviewDialog.PrintPreviewControl.Columns);
            });
        }

        [WinFormsFact]
        public async Task PrintPreviewDialog_Hotkey_Ctrl_5Async()
        {
            await RunTestAsync(async printPreviewDialog =>
            {
                await InputSimulator.SendAsync(
                    printPreviewDialog,
                    inputSimulator => inputSimulator.Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_5));

                Assert.Equal(2, printPreviewDialog.PrintPreviewControl.Rows);
                Assert.Equal(3, printPreviewDialog.PrintPreviewControl.Columns);
            });
        }

        private async Task RunTestAsync(Func<PrintPreviewDialog, Task> runTest)
        {
            await RunFormWithoutControlAsync(
                testDriverAsync: runTest,
                createForm: () =>
                {
                    return new PrintPreviewDialog()
                    {
                        Size = new(500, 300),
                    };
                });
        }
    }
}

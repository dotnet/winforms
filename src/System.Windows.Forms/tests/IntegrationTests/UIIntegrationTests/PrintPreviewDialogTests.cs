// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
            await RunTestAsync(printPreviewDialog =>
            {
                printPreviewDialog.Show();
                SendKeys.SendWait("^1");

                Assert.Equal(1, printPreviewDialog.PrintPreviewControl.Rows);
                Assert.Equal(1, printPreviewDialog.PrintPreviewControl.Columns);

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task PrintPreviewDialog_Hotkey_Ctrl_2Async()
        {
            await RunTestAsync(printPreviewDialog =>
            {
                printPreviewDialog.Show();
                SendKeys.SendWait("^2");

                Assert.Equal(1, printPreviewDialog.PrintPreviewControl.Rows);
                Assert.Equal(2, printPreviewDialog.PrintPreviewControl.Columns);

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task PrintPreviewDialog_Hotkey_Ctrl_3Async()
        {
            await RunTestAsync(printPreviewDialog =>
            {
                printPreviewDialog.Show();
                SendKeys.SendWait("^3");

                Assert.Equal(1, printPreviewDialog.PrintPreviewControl.Rows);
                Assert.Equal(3, printPreviewDialog.PrintPreviewControl.Columns);

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task PrintPreviewDialog_Hotkey_Ctrl_4Async()
        {
            await RunTestAsync(printPreviewDialog =>
            {
                printPreviewDialog.Show();
                SendKeys.SendWait("^4");

                Assert.Equal(2, printPreviewDialog.PrintPreviewControl.Rows);
                Assert.Equal(2, printPreviewDialog.PrintPreviewControl.Columns);

                return Task.CompletedTask;
            });
        }

        [WinFormsFact]
        public async Task PrintPreviewDialog_Hotkey_Ctrl_5Async()
        {
            await RunTestAsync(printPreviewDialog =>
            {
                printPreviewDialog.Show();
                SendKeys.SendWait("^5");

                Assert.Equal(2, printPreviewDialog.PrintPreviewControl.Rows);
                Assert.Equal(3, printPreviewDialog.PrintPreviewControl.Columns);

                return Task.CompletedTask;
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

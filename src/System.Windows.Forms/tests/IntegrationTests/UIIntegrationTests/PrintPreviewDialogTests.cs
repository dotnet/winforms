// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.UI.IntegrationTests.Infra;
using Xunit;

namespace System.Windows.Forms.UI.IntegrationTests
{
    [ConfigureJoinableTaskFactory]
    public class PrintPreviewDialogTests
    {
        [StaFact]
        public void Hotkey_Ctrl_1()
        {
            RunTest(textBox =>
            {
                PrintPreviewDialog printPreviewDialog = new();
                printPreviewDialog.Show();
                SendKeys.SendWait("^1");

                Assert.Equal(1, printPreviewDialog.PrintPreviewControl.Rows);

                Assert.Equal(1, printPreviewDialog.PrintPreviewControl.Columns);
            });
        }

        [StaFact]
        public void Hotkey_Ctrl_2()
        {
            RunTest(textBox =>
            {
                PrintPreviewDialog printPreviewDialog = new();
                printPreviewDialog.Show();
                SendKeys.SendWait("^2");

                Assert.Equal(1, printPreviewDialog.PrintPreviewControl.Rows);

                Assert.Equal(2, printPreviewDialog.PrintPreviewControl.Columns);
            });
        }

        [StaFact]
        public void Hotkey_Ctrl_3()
        {
            RunTest(textBox =>
            {
                PrintPreviewDialog printPreviewDialog = new();
                printPreviewDialog.Show();
                SendKeys.SendWait("^3");

                Assert.Equal(1, printPreviewDialog.PrintPreviewControl.Rows);

                Assert.Equal(3, printPreviewDialog.PrintPreviewControl.Columns);
            });
        }

        [StaFact]
        public void Hotkey_Ctrl_4()
        {
            RunTest(textBox =>
            {
                PrintPreviewDialog printPreviewDialog = new();
                printPreviewDialog.Show();
                SendKeys.SendWait("^4");

                Assert.Equal(2, printPreviewDialog.PrintPreviewControl.Rows);

                Assert.Equal(2, printPreviewDialog.PrintPreviewControl.Columns);
            });
        }

        [StaFact]
        public void Hotkey_Ctrl_5()
        {
            RunTest(textBox =>
            {
                PrintPreviewDialog printPreviewDialog = new();
                printPreviewDialog.Show();
                SendKeys.SendWait("^5");

                Assert.Equal(2, printPreviewDialog.PrintPreviewControl.Rows);

                Assert.Equal(3, printPreviewDialog.PrintPreviewControl.Columns);
            });
        }

        private void RunTest(Action<TextBox> runTest)
        {
            UITest.RunControl(
                createControl: form =>
                {
                    TextBox textBox = new()
                    {
                        Parent = form,
                    };

                    return textBox;
                },
                runTestAsync: async textBox =>
                {
                    // Wait for pending operations so the Control is loaded completely before testing it
                    await AsyncTestHelper.JoinPendingOperationsAsync(AsyncTestHelper.UnexpectedTimeout);

                    Assert.NotEqual(IntPtr.Zero, textBox.Handle);

                    runTest(textBox);
                });
        }
    }
}

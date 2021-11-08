// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data;
using System.Windows.Forms.UI.IntegrationTests.Infra;
using Xunit;

namespace System.Windows.Forms.UI.IntegrationTests
{
    [ConfigureJoinableTaskFactory]
    public class DataGridViewTests
    {
        // This test is rather specific: there should be a separate Form to reproduce the issue.
        // We need to show this Form ourselves, the issue won't reproduce with ReflectBase-inherited Form which is shown automatically while scenarios is being run.
        [StaFact]
        public void DataGridView_ToolTip_DoesNot_ThrowException()
        {
            RunTest(dataGridView =>
            {
                using DataTable dataTable = new();
                dataTable.Columns.Add(columnName: "name");
                dataTable.Rows.Add(values: "name1");

                // Create DataGridView with DataSource set to dataTable.
                dataGridView.ShowCellToolTips = true;
                dataGridView.DataSource = dataTable;

                // Create form and add dataGridView.
                using Form dialog = new();
                dialog.Controls.Add(dataGridView);

                dialog.Shown += (sender, args) =>
                {
                    // Move mouse cursor over any cell of the first row.
                    Cursor.Position = dataGridView.PointToScreen(dataGridView.GetCellDisplayRectangle(columnIndex: 0, rowIndex: 0, cutOverflow: false).Location);

                    // Close the dialog after a short delay.
                    _ = Task.Delay(millisecondsDelay: 100).ContinueWith((t) => dialog.Close(), TaskScheduler.FromCurrentSynchronizationContext());
                };

                dialog.ShowDialog();

                var exceptionThrown = false;

                try
                {
                    dataTable.AcceptChanges();
                }
                catch
                {
                    exceptionThrown = true;
                }

                Assert.False(exceptionThrown);
            });
        }

        private void RunTest(Action<DataGridView> runTest)
        {
            UITest.RunControl(
                createControl: form =>
                {
                    DataGridView dataGridView = new()
                    {
                        Parent = form,
                    };
                    return dataGridView;
                },
                runTestAsync: async dataGridView =>
                {
                    // Wait for pending operations so the Control is loaded completely before testing it
                    await AsyncTestHelper.JoinPendingOperationsAsync(AsyncTestHelper.UnexpectedTimeout);

                    runTest(dataGridView);
                });
        }
    }
}

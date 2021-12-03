// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class DataGridViewTests : ControlTestBase
    {
        public DataGridViewTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        // This test is rather specific: there should be a separate Form to reproduce the issue.
        // We need to show this Form ourselves, the issue won't reproduce with ReflectBase-inherited Form which is shown automatically while scenarios is being run.
        [WinFormsFact]
        public async Task DataGridView_ToolTip_DoesNot_ThrowExceptionAsync()
        {
            await RunTestAsync((form, dataGridView) =>
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
                return Task.CompletedTask;
            });
        }

        private async Task RunTestAsync(Func<Form, DataGridView, Task> runTest)
        {
            await RunSingleControlTestAsync(
                testDriverAsync: runTest,
                createControl: () =>
                {
                    DataGridView control = new();

                    return control;
                },
                createForm: () =>
                {
                    return new()
                    {
                        Size = new(500, 300),
                    };
                });
        }
    }
}

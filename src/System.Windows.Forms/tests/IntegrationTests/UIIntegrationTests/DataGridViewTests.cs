// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data;
using System.Drawing;
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

        [WinFormsFact]
        public async Task DataGridView_ToolTip_DoesNot_ThrowExceptionAsync()
        {
            await RunTestAsync(async (form, dataGridView) =>
            {
                using DataTable dataTable = new();
                dataTable.Columns.Add(columnName: "name");
                dataTable.Rows.Add(values: "name1");
                dataGridView.ShowCellToolTips = true;
                dataGridView.DataSource = dataTable;
                Point point = dataGridView.GetCellDisplayRectangle(columnIndex: 0, rowIndex: 0, cutOverflow: false).Location;

                // Move mouse cursor over any cell of the first row to trigger a tooltip.
                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Mouse.MoveMouseTo(point.X, point.Y));

                // Close the form to verify no exceptions thrown while showing the tooltip.
                // Regression test for https://github.com/dotnet/winforms/issues/5496
                form.Close();
                dataTable.AcceptChanges();
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

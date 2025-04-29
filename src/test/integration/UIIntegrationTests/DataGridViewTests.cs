// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Data;
using System.Drawing;
using FluentAssertions;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

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
            Rectangle cellRectangle = dataGridView.GetCellDisplayRectangle(columnIndex: 0, rowIndex: 0, cutOverflow: false);
            Point cellCenter = GetCenter(cellRectangle);
            Point targetPoint = ToVirtualPoint(dataGridView.PointToScreen(cellCenter));

            // Move mouse cursor over any cell of the first row to trigger a tooltip.
            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Mouse.MoveMouseTo(targetPoint.X, targetPoint.Y));

            // Close the form to verify no exceptions thrown while showing the tooltip.
            // Regression test for https://github.com/dotnet/winforms/issues/5496
            form.Close();
            dataTable.AcceptChanges();
        });
    }

    [WinFormsFact]
    public void DataGridView_ClosesFormWhileDataGridViewInEditMode_WithBindingSource()
    {
        Action action = () =>
        {
            DataGridView dataGridView = new();
            Form form = new();

            form.Controls.Add(dataGridView);
            dataGridView.Dock = DockStyle.Fill;

            DataGridViewTextBoxColumn nameColumn = new()
            {
                HeaderText = "Name",
                DataPropertyName = "Name"
            };
            DataGridViewTextBoxColumn ageColumn = new()
            {
                HeaderText = "Age",
                DataPropertyName = "Age"
            };

            dataGridView.Columns.AddRange([nameColumn, ageColumn]);

            DataRecordList dataRecordList =
            [
                new DataRecord { Name = "Alice", Age = 30 }
            ];

            IContainer components = new Container();
            using BindingSource bindingSource = new(components)
            {
                DataSource = dataRecordList
            };
            dataGridView.DataSource = bindingSource;

            form.Shown += (s, e) =>
            {
                dataGridView.CurrentCell = dataGridView.Rows[0].Cells[0];
                bool beganEdit = dataGridView.BeginEdit(true);

                _ = Task.Run(async () =>
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                    form.Invoke(() => form.Close());
                });
            };

            form.ShowDialog();
        };

        action.Should().NotThrow();
    }

    public class DataRecord
    {
        public string Name { get; set; } = "";
        public int Age { get; set; }
    }

    public class DataRecordList : BindingList<DataRecord> { };

    [WinFormsTheory]
    [InlineData("short value", false)]
    [InlineData("very long value that will be truncated by the DataGridViewCell", true)]
    public async Task DataGridView_MouseToolTip_Appears_IfTextIsTruncatedOnly(string cellValue, bool expected)
    {
        await RunTestAsync(async (form, dataGridView) =>
        {
            using DataTable dataTable = new();
            dataTable.Columns.Add(columnName: "name");
            dataTable.Rows.Add(values: cellValue);
            dataGridView.ShowCellToolTips = true;
            dataGridView.DataSource = dataTable;
            Rectangle cellRectangle = dataGridView.GetCellDisplayRectangle(columnIndex: 0, rowIndex: 0, cutOverflow: false);
            Point cellCenter = GetCenter(cellRectangle);
            Point targetPoint = ToVirtualPoint(dataGridView.PointToScreen(cellCenter));

            // Move mouse cursor over any cell of the first row to trigger a tooltip.
            // Wait 1 second to make sure that the toolTip appeared, it has some delay (500 ms by default).
            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Mouse.MoveMouseTo(targetPoint.X, targetPoint.Y).Sleep(TimeSpan.FromMilliseconds(1000)));

            // DataGridViewToolTip is private so use the reflection
            object toolTip = dataGridView.TestAccessor().Dynamic._toolTipControl;
            object? actual = toolTip.GetType().GetProperty("Activated")?.GetValue(toolTip);

            Assert.Equal(expected, actual);
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

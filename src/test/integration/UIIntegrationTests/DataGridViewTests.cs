// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms.IntegrationTests.Common;

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
    public async Task DataGridView_ClosesFormWhileDataGridViewInEditMode_WithBindingSource()
    {
        using Form form1 = new();
        using Form form2 = new();
        using DataGridView dataGridView = new();
        using IContainer components = new Container();
        using BindingSource bindingSource = new(components)
        {
            DataSource = TestDataSources.GetPersons()
        };

        dataGridView.DataSource = bindingSource;
        dataGridView.Dock = DockStyle.Fill;
        form2.Controls.Add(dataGridView);

        form1.Shown += (_, _) =>
        {
            form2.Shown += (_, _) =>
            {
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await form2.InvokeAsync(() =>
                        {
                            void handler(object? s, DataGridViewEditingControlShowingEventArgs e)
                            {
                                dataGridView.EditingControlShowing -= handler;
                                form2.Close();
                            }

                            dataGridView.EditingControlShowing += handler;
                            dataGridView.CurrentCell = dataGridView.Rows[0].Cells[1];
                            dataGridView.BeginEdit(true);
                        });
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(ex.Message);
                    }
                });
            };

            form2.ShowDialog(form1);
            form1.Close();
        };

        await Task.Run(form1.ShowDialog);
    }

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

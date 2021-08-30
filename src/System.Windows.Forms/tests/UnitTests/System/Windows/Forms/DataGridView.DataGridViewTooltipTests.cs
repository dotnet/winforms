// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataGridView_DataGridViewToolTipTests
    {
        [WinFormsFact]
        public void DataGridViewToolTip_Get_Not_ThrowsException()
        {
            DataTable dataTable = new();
            dataTable.Columns.Add(columnName: "name");
            dataTable.Rows.Add(values: "name1");

            // create DataGridView with DataSource set to dataTable
            DataGridView dataGridView = new();
            dataGridView.ShowCellToolTips = true;
            dataGridView.DataSource = dataTable;

            // create form and add dataGridView
            Form dialog = new();
            dialog.Controls.Add(dataGridView);

            // these two steps can also be done manually after the dialog is shown
            dialog.Shown += (sender, args) =>
            {
                // 1. move mouse cursor over any cell of the first row
                Cursor.Position = dataGridView.PointToScreen(dataGridView.GetCellDisplayRectangle(columnIndex: 0, rowIndex: 0, cutOverflow: false).Location);

                // 2. close the dialog after a short delay
                Task.Delay(millisecondsDelay: 100).ContinueWith((t) => dialog.Close(), TaskScheduler.FromCurrentSynchronizationContext());
            };

            dialog.ShowDialog();

            bool exceptionThrown = false;

            try
            {
                dataTable.AcceptChanges();
            }
            catch
            {
                exceptionThrown = true;
            }

            Assert.False(exceptionThrown, "Accepting changes on DataTable has thrown an InvalidOperationException because handle of the DataGridView used in Tooltip isn't created");
        }
    }
}

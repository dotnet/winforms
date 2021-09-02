// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Data;
using System.Windows.Forms.IntegrationTests.Common;
using ReflectTools;
using WFCTestLib.Log;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDataGridViewTests : ReflectBase
    {
        public MauiDataGridViewTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDataGridViewTests(args));
        }

        // This test is rather specific: there should be a separate Form to reproduce the issue.
        // We need to show this Form ourselves, the issue won't reproduce with ReflectBase-inherited Form which is shown automatically while scenarios is being run.
        [Scenario(true)]
        public ScenarioResult DataGridView_ToolTip_DoesNot_ThrowException(TParams p)
        {
            using DataTable dataTable = new();
            dataTable.Columns.Add(columnName: "name");
            dataTable.Rows.Add(values: "name1");

            // Create DataGridView with DataSource set to dataTable.
            using DataGridView dataGridView = new();
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
                Task.Delay(millisecondsDelay: 100).ContinueWith((t) => dialog.Close(), TaskScheduler.FromCurrentSynchronizationContext());
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

            return new ScenarioResult(!exceptionThrown);
        }
    }
}

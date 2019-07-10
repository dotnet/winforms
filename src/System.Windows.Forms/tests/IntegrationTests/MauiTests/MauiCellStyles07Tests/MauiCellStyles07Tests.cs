// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Data;
using System.Threading;
using System.Windows.Forms.IntegrationTests.Common;

//
// Testcase:    CellStyles07
// Description: UI manipulation
// Author:      ariroth
//
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiCellStyles07Tests : ReflectBase
    {

        #region Testcase setup
        public MauiCellStyles07Tests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiCellStyles07Tests(args));
        }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            grid = new DataGridView();
            p.ru.LogRandomValues = true;
        }

        DataGridView grid;
        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        // [Scenario("Apply cellstyle to row.  Set visible = false.  Set visible = true.  CellStyle is retained.")]
        [Scenario(true)]
        public ScenarioResult ToggleVisible(TParams p)
        {
            grid = DataGridViewUtils.GetDataGridView(false, 5, p.ru.GetRange(100, 1000));
            DataGridViewCellStyle style = DataGridViewUtils.GetRandomCellStyle(p);
            int rowNum = p.ru.GetRange(0, grid.RowCount - 1);
            grid.Rows[rowNum].DefaultCellStyle = style;
            grid.Rows[rowNum].Visible = false;
            grid.Rows[rowNum].Visible = true;
            return new ScenarioResult(DataGridViewUtils.CompareCellStyles(grid.Rows[rowNum].DefaultCellStyle, style), "Styles are not the same.", style, grid.Rows[rowNum].DefaultCellStyle, p.log);
        }

        [Scenario(true)]
        //[Scenario("Apply cellstyle to column.  Reorder column.  CellStyle is retained.")]
        public ScenarioResult ColumnStyleReorder(TParams p)
        {
            grid = DataGridViewUtils.GetDataGridView(false, 5, p.ru.GetRange(100, 1000));
            DataGridViewCellStyle style = DataGridViewUtils.GetRandomCellStyle(p);
            int colNum = p.ru.GetRange(0, 4);
            grid.Columns[colNum].DefaultCellStyle = style;
            DataGridViewUtils.ReorderColumn(grid, colNum);
            return new ScenarioResult(DataGridViewUtils.CompareCellStyles(grid.Columns[colNum].DefaultCellStyle, style), "Styles are not the same.", style, grid.Columns[colNum].DefaultCellStyle, p.log);
        }

        [Scenario(true)]
        //[Scenario("Apply cellstyle to row.  Reorder column.  CellStyle is retained.")]
        public ScenarioResult RowStyleReorder(TParams p)
        {
            grid = DataGridViewUtils.GetDataGridView(false, 5, p.ru.GetRange(100, 1000));
            DataGridViewCellStyle style = DataGridViewUtils.GetRandomCellStyle(p);
            int rowNum = p.ru.GetRange(0, grid.RowCount - 1);
            grid.Rows[rowNum].DefaultCellStyle = style;
            DataGridViewUtils.ReorderColumn(grid, p.ru.GetRange(0, grid.ColumnCount - 1));
            return new ScenarioResult(DataGridViewUtils.CompareCellStyles(grid.Rows[rowNum].DefaultCellStyle, style), "Styles are not the same.", style, grid.Rows[rowNum].DefaultCellStyle, p.log);
        }


        [Scenario(true)]
        //[Scenario("Apply cellstyle to a row in unbound table.  Sort Column.  CellStyle is retained.")]
        public ScenarioResult UnboundSort(TParams p)
        {
            grid = DataGridViewUtils.GetDataGridView(false, 5, p.ru.GetRange(100, 1000));
            DataGridViewCellStyle style = DataGridViewUtils.GetRandomCellStyle(p);
            DataGridViewRow row = grid.Rows[p.ru.GetRange(0, grid.RowCount - 1)];
            row.DefaultCellStyle = style;
            SafeMethods.DoAccessibleDefaultAction(grid.Columns[p.ru.GetRange(0, grid.ColumnCount - 1)].HeaderCell.AccessibilityObject);
            return new ScenarioResult(DataGridViewUtils.CompareCellStyles(row.DefaultCellStyle, style), "Styles are not the same.", style, row.DefaultCellStyle, p.log);
        }

        [Scenario(true)]
        //[Scenario("Apply cellstyle to a row in databound table.  Sort Column.  CellStyle is retained.")]
        public ScenarioResult BoundSort(TParams p)
        {
            grid = DataGridViewUtils.GetDataGridView(true, 5, p.ru.GetRange(100, 1000));
            // Hack to get around VSCurrent 430979
            this.Controls.Add(grid);
            DataGridViewCellStyle style = DataGridViewUtils.GetRandomCellStyle(p);
            p.log.WriteLine("Grid rows: {0}", grid.RowCount);
            DataGridViewRow row = grid.Rows[p.ru.GetRange(0, grid.RowCount - 1)];
            row.DefaultCellStyle = style;

            p.log.WriteLine(grid.Columns[p.ru.GetRange(0, grid.ColumnCount - 1)].HeaderCell.AccessibilityObject.DefaultAction);
            SafeMethods.DoAccessibleDefaultAction(grid.Columns[p.ru.GetRange(0, grid.ColumnCount - 1)].HeaderCell.AccessibilityObject);
            return new ScenarioResult(DataGridViewUtils.CompareCellStyles(row.DefaultCellStyle, style), "Styles are not the same.", style, row.DefaultCellStyle, p.log);
        }

        #endregion
    }
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Apply cellstyle to row.  Set visible = false.  Set visible = true.  CellStyle is retained.

//@ Apply cellstyle to column.  Reorder column.  CellStyle is retained.

//@ Apply cellstyle to row.  Reorder column.  CellStyle is retained.

//@ Apply cellstyle to a row in unbound table.  Sort Column.  CellStyle is retained.

//@ Apply cellstyle to a row in databound table.  Sort Column.  CellStyle is retained.


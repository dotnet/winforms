// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Windows.Forms;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Threading;
using System.Windows.Forms.IntegrationTests.Common;

//
// Testcase:    DGV_BasicRowDirtyStateNeeded
// Description: RowDirtyStateNeeded
// Author:      t-timw
//
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class DGV_BasicRowDirtyStateNeeded : ReflectBase
    {

        #region Testcase setup
        public DGV_BasicRowDirtyStateNeeded(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new DGV_BasicRowDirtyStateNeeded(args));
        }
        DataGridView grid;
        int eventCount = 0;

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            Controls.Remove(grid);
            grid = DataGridViewUtils.GetSimpleDataGridView();
            grid.RowDirtyStateNeeded += new QuestionEventHandler(grid_RowDirtyStateNeeded);
            Controls.Add(grid);

        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {

            eventCount = 0;
            grid.VirtualMode = true;

            return base.BeforeScenario(p, scenario);
        }

        #endregion
        [Scenario(false)]
        public ScenarioResult ChangeCurrentCellAndCheck(int rowIndex1, int columnIndex1, int rowIndex2, int columnIndex2, int expectedTimesFired)
        {
            grid.CurrentCell = grid.Rows[rowIndex1].Cells[columnIndex1];
            Application.DoEvents();
            eventCount = 0;
            grid.CurrentCell = grid.Rows[rowIndex2].Cells[columnIndex2];
            Application.DoEvents();
            return new ScenarioResult(expectedTimesFired, eventCount, "Event fired an unexpected number of times.", log);
        }
        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("Navigate between rows.  Verify event fires.")]
        [Scenario(true)]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            // Programmatically
            int rowIndex1 = p.ru.GetRange(0, grid.RowCount - 1);
            int columnIndex1 = p.ru.GetRange(0, grid.ColumnCount - 1);
            int rowIndex2;
            int columnIndex2 = p.ru.GetRange(0, grid.ColumnCount - 1);
            do
            {
                rowIndex2 = p.ru.GetRange(0, grid.RowCount - 1);
            } while (rowIndex1 == rowIndex2);

            result.IncCounters(ChangeCurrentCellAndCheck(rowIndex1, columnIndex1, rowIndex2, columnIndex2, 1));

            // With keyboard
            rowIndex1 = p.ru.GetRange(0, grid.RowCount - 1);
            columnIndex1 = p.ru.GetRange(0, grid.ColumnCount - 1);
            grid.CurrentCell = grid.Rows[rowIndex1].Cells[columnIndex1];
            Application.DoEvents();
            eventCount = 0;

            if (rowIndex1 == grid.RowCount - 1)
            {
                // Can't go down anymore
                SendKeys.SendWait("{UP}");
            }
            else
            {
                SendKeys.SendWait("{DOWN}");
            }
            Application.DoEvents();
            result.IncCounters(1, eventCount, "Event fired an unexpected number of times when navigating between rows with keyboard.", log);
            return result;
        }

        //[Scenario("Navigate within row.  Verify event doesn't fire.")]
        [Scenario(true)]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            // Programmatically
            int rowIndex1 = p.ru.GetRange(0, grid.RowCount - 1);
            int columnIndex1 = p.ru.GetRange(0, grid.ColumnCount - 1);
            int columnIndex2;
            do
            {
                columnIndex2 = p.ru.GetRange(0, grid.ColumnCount - 1);

            } while (columnIndex1 == columnIndex2);

            result.IncCounters(ChangeCurrentCellAndCheck(rowIndex1, columnIndex1, rowIndex1, columnIndex2, 0));

            // Keyboard
            eventCount = 0;
            if (grid.CurrentCell.ColumnIndex == grid.RowCount - 1)
            {
                // Already in rightmost cell, so can't go right
                SendKeys.SendWait("{LEFT}");
            }
            else
            {
                SendKeys.SendWait("{Right}");
            }
            Application.DoEvents();
            result.IncCounters(0, eventCount, "Event fired an unexpected number of times.", log);
            return result;
        }

        //[Scenario("VirtualMode = false.  Verify event doesn't fire.")]
        [Scenario(true)]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            grid.VirtualMode = false;
            // Programmatically
            int rowIndex1 = p.ru.GetRange(0, grid.RowCount - 1);
            int columnIndex1 = p.ru.GetRange(0, grid.ColumnCount - 1);
            int rowIndex2;
            int columnIndex2 = p.ru.GetRange(0, grid.ColumnCount - 1);
            do
            {
                rowIndex2 = p.ru.GetRange(0, grid.RowCount - 1);
            } while (rowIndex1 == rowIndex2);

            result.IncCounters(ChangeCurrentCellAndCheck(rowIndex1, columnIndex1, rowIndex2, columnIndex2, 0));

            // With keyboard
            rowIndex1 = p.ru.GetRange(0, grid.RowCount - 1);
            columnIndex1 = p.ru.GetRange(0, grid.ColumnCount - 1);
            grid.CurrentCell = grid.Rows[rowIndex1].Cells[columnIndex1];
            Application.DoEvents();
            eventCount = 0;

            if (rowIndex1 == grid.RowCount - 1)
            {
                // Can't go down anymore
                SendKeys.SendWait("{UP}");
            }
            else
            {
                SendKeys.SendWait("{DOWN}");
            }
            Application.DoEvents();
            result.IncCounters(0, eventCount, "Event fired an unexpected number of times when navigating between rows with keyboard.", log);
            return result;
        }

        #endregion

        void grid_RowDirtyStateNeeded(object sender, QuestionEventArgs e)
        {
            eventCount++;
        }
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Navigate between rows.  Verify event fires.

//@ Navigate within row.  Verify event doesn't fire.

//@ VirtualMode = false.  Verify event doesn't fire.


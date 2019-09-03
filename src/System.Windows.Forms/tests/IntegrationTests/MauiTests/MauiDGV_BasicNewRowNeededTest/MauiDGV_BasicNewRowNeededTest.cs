// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Windows.Forms;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using Maui.Core;
using System.Threading;
using System.Windows.Forms.IntegrationTests.Common;

//
// Testcase:    DGV_BasicNewRowNeeded
// Description: NewRowNeeded
// Author:      t-timw
//
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDGV_BasicNewRowNeededTest : ReflectBase
    {

        #region Testcase setup
        public MauiDGV_BasicNewRowNeededTest(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDGV_BasicNewRowNeededTest(args));
        }
        DataGridView grid;
        int eventCount = 0;
        DataGridViewRowEventArgs eventsArgs;
        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            ResetGrid();
            return base.BeforeScenario(p, scenario);
        }

        void ResetGrid()
        {
            ResetGrid(true);
        }
        void ResetGrid(bool isVirtual)
        {
            Controls.Remove(grid);
            grid = DataGridViewUtils.GetSimpleDataGridView();
            grid.NewRowNeeded += new DataGridViewRowEventHandler(grid_NewRowNeeded);
            Controls.Add(grid);
            grid.Focus();
            grid.VirtualMode = isVirtual;

            grid.CurrentCell = grid.Rows[0].Cells[0];
            Application.DoEvents();
            eventCount = 0;
            eventsArgs = null;
        }
        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("VirtualMode = false.  Verify event doesn't fire.")]
        [Scenario(true)]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            ResetGrid(false);
            // Try navigating to new row via keyboard
            SendKeys.SendWait("{down " + (grid.RowCount - 1).ToString() + "}");
            result.IncCounters(0, eventCount, "Event fired when virtual = false.", log);

            // Enter something into row before new row, and then hit enter
            ResetGrid(false);
            int columnIndex = p.ru.GetRange(0, grid.ColumnCount - 1);
            int rowIndex = grid.RowCount - 2; // Row before bottom row

            grid.EditMode = DataGridViewEditMode.EditOnEnter;
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];
            SendKeys.SendWait("Packing a security punch{Enter}");
            Application.DoEvents();
            result.IncCounters(0, eventCount, "Event fired when virtual = false.", log);

            // Try clicking on new row
            ResetGrid(false);
            // Drop some rows so that all rows are visible in grid.
            grid.RowCount = 2;
            grid.CurrentCell = null;

            // Click on cell in new row
            Rectangle cellRectangle = grid.GetCellDisplayRectangle(0, grid.RowCount - 1, false);
            Point clickPt = grid.PointToScreen(new Point(cellRectangle.X + cellRectangle.Width / 2, cellRectangle.Y + cellRectangle.Height / 2));

            Mouse.Click(clickPt.X, clickPt.Y);
            Application.DoEvents();
            result.IncCounters(0, eventCount, "Event fired when clicking on new row with VirtualMode = false.", log);

            // Click on new row's header
            grid.CurrentCell = null;

            cellRectangle = grid.GetCellDisplayRectangle(-1, grid.RowCount - 1, false);
            clickPt = grid.PointToScreen(new Point(cellRectangle.X + cellRectangle.Width / 2, cellRectangle.Y + cellRectangle.Height / 2));

            Mouse.Click(clickPt.X, clickPt.Y);
            Application.DoEvents();
            result.IncCounters(0, eventCount, "Event fired an unexpected number of times when clicking on cell in new row.", log);

            // Now click on a cell in new row.  Verify event doesn't fire.
            eventCount = 0;
            clickPt = grid.PointToScreen(new Point(cellRectangle.X + cellRectangle.Width / 2, cellRectangle.Y + cellRectangle.Height / 2));
            Mouse.Click(clickPt.X, clickPt.Y);
            Application.DoEvents();
            result.IncCounters(0, eventCount, "Event fired when clicking on new row with VirtualMode = false.", log);

            return result;
        }

        //[Scenario("VirtualMode = true. Navigate to the bottom row using arrow keys.  Verify event fires.")]
        [Scenario(true)]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            SendKeys.SendWait("{down " + (grid.RowCount - 1).ToString() + "}");
            result.IncCounters(1, eventCount, "Event fired an unexpected number of times.", log);
            result.IncCounters(grid.Rows[grid.RowCount - 1], eventsArgs.Row, "e.Row is incorrect.", log);

            return result;
        }

        //[Scenario("VirtualMode = true. Navigate to bottom row using PageDown.  Verify event fires.")]
        [Scenario(true)]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            grid.CurrentCell = grid.Rows[0].Cells[0];
            grid.CurrentCell.Selected = true;
            // pgdown grid.RowCount number of times to be sure that it is hit enough times to scroll to new row
            SendKeys.SendWait("{pgdn " + grid.RowCount.ToString() + "}");

            Application.DoEvents();

            result.IncCounters(1, eventCount, "Event fired an unexpected number of times.", log);
            result.IncCounters(grid.Rows[grid.RowCount - 1], eventsArgs.Row, "e.Row is incorrect.", log);

            return result;
        }

        //[Scenario("VirtualMode = true. Edit a cell in row before bottom row.  Press Enter. Verify event fires.")]
        [Scenario(true)]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            int columnIndex = p.ru.GetRange(0, grid.ColumnCount - 1);
            int rowIndex = grid.RowCount - 2; // Row before bottom row

            grid.EditMode = DataGridViewEditMode.EditOnEnter;
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];
            SendKeys.SendWait("Just watch the fireworks");
            Application.DoEvents();
            result.IncCounters(0, eventCount, "Event fired before pressing {Enter} to enter the new row.", log);
            SendKeys.SendWait("{Enter}");
            Application.DoEvents();
            result.IncCounters(1, eventCount, "Event fired an unexpected number of times.", log);
            // Expect e.Row to reference the new row
            result.IncCounters(grid.Rows[grid.RowCount - 1], eventsArgs.Row, "e.Row is incorrect.", log);

            return result;
        }

        //[Scenario("VirtualMode = true. Navigate to bottom row using mouse.  Click on row header cell and a regular cell. Verify event fires.")]
        [Scenario(true)]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            // Drop some rows so that all rows are visible in grid.
            grid.RowCount = 2;
            grid.CurrentCell = null;

            // Click on cell in new row
            Rectangle cellRectangle = grid.GetCellDisplayRectangle(0, grid.RowCount - 1, false);
            Point clickPt = grid.PointToScreen(new Point(cellRectangle.X + cellRectangle.Width / 2, cellRectangle.Y + cellRectangle.Height / 2));

            Mouse.Click(clickPt.X, clickPt.Y);
            Application.DoEvents();
            result.IncCounters(1, eventCount, "Event fired an unexpected number of times when clicking on cell in new row.", log);
            result.IncCounters(grid.Rows[grid.RowCount - 1], eventsArgs.Row, "e.Row is incorrect.", log);

            // Click on new row's header
            grid.CurrentCell = null;
            eventCount = 0;
            eventsArgs = null;

            cellRectangle = grid.GetCellDisplayRectangle(-1, grid.RowCount - 1, false);
            clickPt = grid.PointToScreen(new Point(cellRectangle.X + cellRectangle.Width / 2, cellRectangle.Y + cellRectangle.Height / 2));

            Mouse.Click(clickPt.X, clickPt.Y);
            Application.DoEvents();
            result.IncCounters(1, eventCount, "Event fired an unexpected number of times when clicking on cell in new row.", log);
            result.IncCounters(grid.Rows[grid.RowCount - 1], eventsArgs.Row, "e.Row is incorrect.", log);

            // Now click on a cell in new row.  Verify event doesn't fire.
            eventCount = 0;
            clickPt = grid.PointToScreen(new Point(cellRectangle.X + cellRectangle.Width / 2, cellRectangle.Y + cellRectangle.Height / 2));
            Mouse.Click(clickPt.X, clickPt.Y);
            Application.DoEvents();
            result.IncCounters(0, eventCount, "Event fired when clicking on cell in new row while the new row is selected.", log);

            return result;
        }

        #endregion

        void grid_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
        {
            eventCount++;
            eventsArgs = e;
        }
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ VirtualMode = false.  Verify event doesn't fire.

//@ VirtualMode = true. Navigate to the bottom row using arrow keys.  Verify event fires.

//@ VirtualMode = true. Navigate to bottom row using PageDown.  Verify event fires.

//@ VirtualMode = true. Edit a cell in row before bottom row.  Press Enter. Verify event fires.

//@ VirtualMode = true. Navigate to bottom row using mouse.  Click on row header cell and a regular cell. Verify event fires.


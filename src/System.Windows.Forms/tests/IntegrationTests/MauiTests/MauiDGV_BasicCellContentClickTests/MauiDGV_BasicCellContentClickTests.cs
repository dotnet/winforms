// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Windows.Forms;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Forms.IntegrationTests.Common;
using System.Threading;


//
// Testcase:    DGV_BasicCellContentClick
// Description: 
// Author:      t-timw
//
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDGV_BasicCellContentClickTests : ReflectBase
    {

        #region Testcase setup
        public MauiDGV_BasicCellContentClickTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDGV_BasicCellContentClickTests(args));
        }
        DataGridView grid;
        int eventCount = 0;
        int eventsRowIndex = -1;
        int eventsColumnIndex = -1;
        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            grid = DataGridViewUtils.GetSimpleDataGridView();

            Controls.Add(grid);
        }
        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            eventCount = 0;
            Controls.Remove(grid);
            grid = DataGridViewUtils.GetSimpleDataGridView();
            grid.CellContentClick += new DataGridViewCellEventHandler(grid_CellContentClick);
            Controls.Add(grid);
            return base.BeforeScenario(p, scenario);
        }


        #endregion
        [Scenario(false)]
        //[Scenario(true)]
        // Returns a given cell's coordinates wrt the screen.
        Point GetCellCoordsWrtScreen(DataGridViewCell cell)
        {
            DataGridView dgv = cell.DataGridView;
            // Get cell's coordinates wrt dgv
            Rectangle cellsRectWrtDgv = dgv.GetCellDisplayRectangle(cell.ColumnIndex, cell.RowIndex, false);

            // Rectangle with cell's points wrt screen
            cellsRectWrtDgv.Offset(dgv.PointToScreen(Point.Empty));

            return new Point(cellsRectWrtDgv.X, cellsRectWrtDgv.Y);
        }
        [Scenario(false)]
        //[Scenario(true)]
        // Click's the given cell's contents
        public void ClickCellContents(DataGridViewCell cell)
        {
            DataGridViewCellStyle oldStyle = grid.DefaultCellStyle;
            DataGridViewCellStyle scenariosStyle = new DataGridViewCellStyle();

            // Figure out where cell's content is
            Point cellPt = GetCellCoordsWrtScreen(cell);
            Rectangle contentRectangle = cell.ContentBounds;

            Point cellClickPt = cellPt;
            cellClickPt.X += contentRectangle.X + (contentRectangle.Width / 2);
            cellClickPt.Y += contentRectangle.Y + (contentRectangle.Height / 2);

            // Click the contents
            Maui.Core.Mouse.Click(cellClickPt.X, cellClickPt.Y);
            Application.DoEvents();
        }

        [Scenario(false)]
        //[Scenario(true)]


        // Checks the event arguments

        public ScenarioResult CheckEventArgs(int expectedRowIndex, int expectedColumnIndex)
        {
            ScenarioResult result = new ScenarioResult();

            result.IncCounters(expectedColumnIndex, eventsColumnIndex, "e.ColumnIndex is incorrect.", scenarioParams.log);
            result.IncCounters(expectedRowIndex, eventsRowIndex, "e.RowIndex is incorrect.", scenarioParams.log);

            return result;
        }
        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("1) Doesn't fire when clicking padding")]
        [Scenario(true)]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            DataGridViewCellStyle oldStyle = grid.DefaultCellStyle;
            DataGridViewCellStyle scenariosStyle = new DataGridViewCellStyle();
            // Make the cells padded.
            // increase size of the cell we're looking at to fit padding
            grid.Columns[0].Width += 20;
            grid.Rows[0].Height += 20;
            scenariosStyle.Padding = new Padding(15);
            grid.DefaultCellStyle = scenariosStyle;

            // Figure out where cell (0,0)'s padding is
            Point cellPt = GetCellCoordsWrtScreen(grid.Rows[0].Cells[0]);
            Point cellPaddingPt = cellPt;
            cellPaddingPt.X += grid.Columns[0].Width / 2;
            cellPaddingPt.Y += 8;

            // Move mouse to padding
            Maui.Core.Mouse.Click(cellPaddingPt.X, cellPaddingPt.Y);

            result.IncCounters(0, eventCount, "Unexpected value.", p.log);
            return result;
        }

        //[Scenario("2) Verify event doesn't fire when entering cell using keyboard")]
        [Scenario(true)]
        public ScenarioResult Scenario2(TParams p)
        {
            grid.Focus();
            SendKeys.SendWait("{right 1}");
            SendKeys.SendWait("{down 2}");
            return new ScenarioResult(0, eventCount, "Unexpected values.", p.log);
        }

        //[Scenario("3) Verify event fires when entering cell using mouseclick on contents")]
        [Scenario(true)]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            DataGridViewCellStyle oldStyle = grid.DefaultCellStyle;
            DataGridViewCellStyle scenariosStyle = new DataGridViewCellStyle();

            int rowIndex = 0;
            int colIndex = 1;
            DataGridViewCell cell = grid.Rows[rowIndex].Cells[colIndex];
            eventCount = 0;

            ClickCellContents(cell);
            result.IncCounters(1, eventCount, "Unexpected value.", p.log);
            result.IncCounters(CheckEventArgs(rowIndex, colIndex));
            return result;
        }

        //[Scenario("4) Verify event fires correctly when contents clicked while inside/outside of cell")]
        [Scenario(true)]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            // Click on contents of cell when another cell is selected
            int oldRowIndex = 0;
            int oldColIndex = 1;
            int newRowIndex = 0;
            int newColIndex = 0;
            DataGridViewCell oldCell = grid.Rows[oldRowIndex].Cells[oldColIndex];
            DataGridViewCell newCell = grid.Rows[newRowIndex].Cells[newColIndex];

            //Utilities.ActiveFreeze();
            ClickCellContents(oldCell);
            eventCount = 0;
            ClickCellContents(newCell);
            result.IncCounters(1, eventCount, "Unexpected value 1.", p.log);
            result.IncCounters(CheckEventArgs(newRowIndex, newColIndex));

            // Click on contents of header cell when it is currently selected
            DataGridViewCell cell = grid.Rows[0].HeaderCell;
            grid.Rows[0].HeaderCell.Value = "Foo";
            // Increrasing the width of the row header so that we don't get tooltips
            grid.RowHeadersWidth += 15;
            ClickCellContents(cell);
            eventCount = 0;
            ClickCellContents(cell);
            result.IncCounters(1, eventCount, "Unexpected value 2.", p.log);
            result.IncCounters(CheckEventArgs(0, -1));
            return result;
        }

        #endregion

        void grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            eventCount++;
            eventsRowIndex = e.RowIndex;
            eventsColumnIndex = e.ColumnIndex;
            Text = eventCount.ToString();
        }
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ 1) Doesn't fire when clicking padding

//@ 2) Verify event doesn't fire when entering cell using keyboard

//@ 3) Verify event fires when entering cell using mouseclick on contents

//@ 4) Verify event fires correctly when contents clicked while inside/outside of cell


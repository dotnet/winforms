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
// Testcase:    DGV_BasicCellValuePushed
// Description: 
// Author:      t-timw
//
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDGV_BasicCellValuePushedTests : ReflectBase
    {

        #region Testcase setup
        public MauiDGV_BasicCellValuePushedTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDGV_BasicCellValuePushedTests(args));
        }
        DataGridView grid;
        int eventCount = 0;
        int colIndex = -1;
        int rowIndex = -1;
        object eValue = null;
        string sendString = "aaa"; // String that will be sent using SendKeys.SendWait()
        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            grid = DataGridViewUtils.GetSimpleDataGridView();
            grid.CellValuePushed += new DataGridViewCellValueEventHandler(grid_CellValuePushed);
            grid.VirtualMode = true;
            Controls.Add(grid);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            eventCount = 0;
            sendString += "Aa"; // so that each string is different
            return base.BeforeScenario(p, scenario);
        }

        protected void reset()
        {
            eventCount = 0;
            colIndex = -1;
            rowIndex = -1;
            eValue = null;
        }


        #endregion
        [Scenario(false)]
        public ScenarioResult CheckEventFire(int expectedTimesFired, int columnIndex, int rowIndex, object value)
        {
            ScenarioResult result = new ScenarioResult();

            result.IncCounters(expectedTimesFired, eventCount, "Event fired an unexpected number of times.", log);
            result.IncCounters(columnIndex, colIndex, "e.ColumnIndex is incorrect.", log);
            result.IncCounters(rowIndex, rowIndex, "e.RowIndex is incorrect.", log);
            result.IncCounters(value, eValue, "e.Value is incorrect.", log);

            return result;
        }

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("Verify event fires when entering value cell for that particular cell and no others.")]
        [Scenario(true)]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            p.log.WriteLine("rowcount: " + grid.RowCount.ToString());
            int rowIndex = p.ru.GetRange(0, grid.RowCount - 1);
            int columnIndex = p.ru.GetRange(0, grid.ColumnCount - 1);

            // Enter value using keyboard
            SafeMethods.FindForm(grid);
            SafeMethods.Focus(grid);
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];
            result.IncCounters(0, eventCount, "Event fired immediately after entering edit mode.", log);
            reset();
            SendKeys.SendWait(sendString);
            Application.DoEvents();
            SendKeys.SendWait("{ENTER}");
            Application.DoEvents();
            result.IncCounters(CheckEventFire(1, columnIndex, rowIndex, sendString));

            // Try programmatically
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];
            reset();
            string newValue = p.ru.GetString(Int32.MaxValue);
            grid.CurrentCell.Value = newValue;
            Application.DoEvents();
            result.IncCounters(CheckEventFire(1, columnIndex, rowIndex, newValue));
            return result;
        }

        //[Scenario("Verify event doesn't fire when entering and exiting edit mode via <escape>")]
        [Scenario(true)]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            int rowIndex = p.ru.GetRange(0, grid.RowCount - 1);
            int columnIndex = p.ru.GetRange(0, grid.ColumnCount - 1);

            // Enter value using keyboard
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];
            SendKeys.SendWait(sendString + "{ESCAPE}");
            Application.DoEvents();
            result.IncCounters(eventCount == 0, log, BugDb.VSWhidbey, 328624, "Event fires when edit is cancelled.");

            // Try programmatically
            eventCount = 0;
            grid.BeginEdit(true);
            grid.CancelEdit();
            Application.DoEvents();
            result.IncCounters(eventCount == 0, log, BugDb.VSWhidbey, 328624, "Event fires when edit is cancelled.");

            return result;
        }

        //[Scenario("Verify event doesnt fire when leaving cell that was not in edit mode.")]
        [Scenario(true)]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            int rowIndex1 = p.ru.GetRange(0, grid.RowCount - 1);
            int columnIndex1 = p.ru.GetRange(0, grid.ColumnCount - 1);
            int rowIndex2;
            int columnIndex2;
            do
            {
                rowIndex2 = p.ru.GetRange(0, grid.RowCount - 1);
                columnIndex2 = p.ru.GetRange(0, grid.ColumnCount - 1);
            } while (rowIndex1 == rowIndex2 && columnIndex1 == columnIndex2);

            grid.CurrentCell = grid.Rows[rowIndex1].Cells[columnIndex1];
            eventCount = 0;
            grid.CurrentCell = grid.Rows[rowIndex2].Cells[columnIndex2];

            result.IncCounters(0, eventCount, "Event fired without entering edit mode.", log);


            return result;
        }

        //[Scenario("Verify event fires when leaving edit mode after inserting text and clearing it. ")]
        [Scenario(true)]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            int rowIndex = p.ru.GetRange(0, grid.RowCount - 1);
            int columnIndex = p.ru.GetRange(0, grid.ColumnCount - 1);
            p.log.WriteLine("Cell: " + columnIndex.ToString() + ", " + rowIndex.ToString());

            // Enter value using keyboard
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];
            result.IncCounters(0, eventCount, "Event fired immediately after entering edit mode.", log);
            reset();
            SendKeys.SendWait(sendString + "^a{DELETE}{ENTER}");
            Application.DoEvents();
            result.IncCounters(CheckEventFire(1, columnIndex, rowIndex, null));

            return result;
        }

        #endregion

        void grid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            eventCount++;
            scenarioParams.log.WriteLine("eventCount: " + eventCount.ToString());
            scenarioParams.log.WriteLine("cellpushed: " + e.ColumnIndex.ToString() + ", " + e.RowIndex.ToString());
            colIndex = e.ColumnIndex;
            rowIndex = e.RowIndex;
            eValue = e.Value;
        }
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify event fires when entering value cell for that particular cell and no others.
//@ Verify event doesn't fire when entering and exiting edit mode via &lt;escape&gt;
//@ Verify event doesnt fire when leaving cell that was not in edit mode.
//@ Verify event fires when leaving edit mode after inserting text and clearing it. 

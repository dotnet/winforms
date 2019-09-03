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
// Testcase:    DGV_BasicCellValueNeeded
// Description: 
// Author:      t-timw
//
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDGV_BasicCellValueNeededTests : ReflectBase
    {

        #region Testcase setup
        public MauiDGV_BasicCellValueNeededTests(string[] args) : base(args)
        {

            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDGV_BasicCellValueNeededTests(args));
        }
        DataGridView grid;
        int eventCount = 0;
        DataGridViewCellValueEventArgs eventsArgs;

        // Used by grid_CellValueNeeded2 to store the value that it set e.Value to
        object setValue;

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            grid = DataGridViewUtils.GetSimpleDataGridView();
            Controls.Add(grid);
            grid.CellValueNeeded += new DataGridViewCellValueEventHandler(grid_CellValueNeeded);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            grid.VirtualMode = true;
            grid.RowCount = 2;
            grid.ColumnCount = 1;
            return base.BeforeScenario(p, scenario);
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("Verify event fires on virtual cell when it is repainted.")]
        [Scenario(true)]
        public ScenarioResult TriggerRepaint(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            grid.VirtualMode = true;
            // Toggle selected to trigger painting
            Application.DoEvents();
            eventCount = 0;
            eventsArgs = null;
            grid.Rows[0].Cells[0].Selected ^= true;
            Application.DoEvents();
            result.IncCounters(1, eventCount, "Event fired an unexpected number of times when cell was being painted.", log);
            result.IncCounters(0, eventsArgs.RowIndex, "e.RowIndex is incorrect.", log);
            result.IncCounters(0, eventsArgs.ColumnIndex, "e.ColumnIndex is incorrect.", log);
            result.IncCounters(null, eventsArgs.Value, "e.Value is incorrect.", log);

            // Toggle once more
            eventCount = 0;
            eventsArgs = null;
            grid.Rows[0].Cells[0].Selected ^= true;
            Application.DoEvents();
            result.IncCounters(1, eventCount, "Event fired an unexpected number of times when cell was being painted.", log);
            result.IncCounters(0, eventsArgs.RowIndex, "e.RowIndex is incorrect.", log);
            result.IncCounters(0, eventsArgs.ColumnIndex, "e.ColumnIndex is incorrect.", log);
            result.IncCounters(null, eventsArgs.Value, "e.Value is incorrect.", log);

            // Access cell.Value
            eventCount = 0;
            eventsArgs = null;
            object value = grid.Rows[0].Cells[0].Value;
            result.IncCounters(1, eventCount, "Event fired an unexpected number of times when accessing cell.Value", log);
            result.IncCounters(0, eventsArgs.RowIndex, "e.RowIndex is incorrect.", log);
            result.IncCounters(0, eventsArgs.ColumnIndex, "e.ColumnIndex is incorrect.", log);
            result.IncCounters(null, eventsArgs.Value, "e.Value is incorrect.", log);

            return result;
        }


        //[Scenario("Verify event fires on virtual cell when selected. ")]
        [Scenario(true)]
        public ScenarioResult Scenario2(TParams p)
        {
            // Previous scenario toggles cell.Selected in order to repaint cell
            return TriggerRepaint(p);
        }

        //[Scenario("Access cell.Value and set e.Value.  Verify event fires and that change to e.Value sticks.")]
        [Scenario(true)]
        public ScenarioResult SCenario3(TParams p)
        {

            ScenarioResult result = new ScenarioResult();

            grid.CellValueNeeded -= new DataGridViewCellValueEventHandler(grid_CellValueNeeded);
            grid.CellValueNeeded += new DataGridViewCellValueEventHandler(grid_CellValueNeeded2);

            // Access cell's value
            eventCount = 0;
            DataGridViewCell cell = grid.Rows[0].Cells[0];
            string value = (string)cell.Value;
            string expectedValue = (string)setValue;


            result.IncCounters(1, eventCount, "Event fired an unexpected number of times when cell was being painted.", log);
            result.IncCounters(0, eventsArgs.RowIndex, "e.RowIndex is incorrect.", log);
            result.IncCounters(0, eventsArgs.ColumnIndex, "e.ColumnIndex is incorrect.", log);
            result.IncCounters(value, expectedValue, "e.Value is incorrect.", log);
            result.IncCounters(eventsArgs.Value, expectedValue, "e.Value is incorrect.", log);

            grid.CellValueNeeded += new DataGridViewCellValueEventHandler(grid_CellValueNeeded);
            grid.CellValueNeeded -= new DataGridViewCellValueEventHandler(grid_CellValueNeeded2);

            return result;
        }

        //[Scenario("Verify event doesn't fire when virtual = false")]
        [Scenario(true)]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            Application.DoEvents();
            eventCount = 0;
            grid.VirtualMode = false;

            object value = grid.Rows[0].Cells[0].Value;
            Application.DoEvents();
            result.IncCounters(0, eventCount, "Event fired when accessing a cell's value.", log);

            eventCount = 0;
            grid.Rows[0].Cells[0].Selected ^= true;
            Application.DoEvents();
            result.IncCounters(0, eventCount, "Event fired when toggling cell.Selected.", log);

            return result;
        }

        #endregion

        void grid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            eventCount++;
            eventsArgs = e;
        }
        void grid_CellValueNeeded2(object sender, DataGridViewCellValueEventArgs e)
        {
            eventsArgs = e;
            setValue = e.Value = scenarioParams.ru.GetString(10);
            eventCount++;

        }
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify event fires on virtual cell when it is repainted.

//@ Verify event fires on virtual cell when selected. 

//@ Access cell.Value and set e.Value.  Verify event fires and that change to e.Value sticks.

//@ Verify event doesn't fire when virtual = false


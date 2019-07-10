using System;
using System.Drawing;
using System.Windows.Forms;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Threading;
using System.Windows.Forms.IntegrationTests.Common;

//
// Testcase:    DGV_BasicRowErrorTextNeeded
// Description: 
// Author:      t-timw
//
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDGV_BasicRowErrorTextNeededTests : ReflectBase
    {

        #region Testcase setup
        public MauiDGV_BasicRowErrorTextNeededTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDGV_BasicRowErrorTextNeededTests(args));
        }
        DataGridView grid;
        int eventCount = 0;
        DataGridViewRowErrorTextNeededEventArgs eventsArgs;
        // Used by grid_RowErrorTextNeeded2 to store what the error text was set to.
        string setErrorText;

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            grid = DataGridViewUtils.GetSimpleDataGridView();
            Controls.Add(grid);
            grid.RowErrorTextNeeded += new DataGridViewRowErrorTextNeededEventHandler(grid_RowErrorTextNeeded);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            grid.VirtualMode = true;
            return base.BeforeScenario(p, scenario);
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("Verify event fires when accessing row's error text.")]
        [Scenario(true)]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            int rowIndex = p.ru.GetRange(0, grid.RowCount - 1);

            eventCount = 0;
            eventsArgs = null;

            string rowsErrorText = grid.Rows[rowIndex].ErrorText;
            result.IncCounters(1, eventCount, "Event fired an unexpected number of times while accessing row.ErrorText.", log);
            result.IncCounters(rowIndex, eventsArgs.RowIndex, "e.RowIndex is incorrect.", log);

            // Try setting the row's error text through the handler
            grid.RowErrorTextNeeded += new DataGridViewRowErrorTextNeededEventHandler(grid_RowErrorTextNeeded2);
            grid.RowErrorTextNeeded -= new DataGridViewRowErrorTextNeededEventHandler(grid_RowErrorTextNeeded);

            eventCount = 0;
            eventsArgs = null;
            rowsErrorText = grid.Rows[rowIndex].ErrorText;
            result.IncCounters(1, eventCount, "Event fired an unexpected number of times while accessing row.ErrorText.", log);
            result.IncCounters(setErrorText, rowsErrorText, "The change to e.ErrorText did not stick.", log);
            result.IncCounters(setErrorText, eventsArgs.ErrorText, "e.ErrorText is incorrect.", log);
            result.IncCounters(rowIndex, eventsArgs.RowIndex, "e.RowIndex is incorrect.", log);

            // Install old handler, uninstall new handler
            grid.RowErrorTextNeeded -= new DataGridViewRowErrorTextNeededEventHandler(grid_RowErrorTextNeeded2);
            grid.RowErrorTextNeeded += new DataGridViewRowErrorTextNeededEventHandler(grid_RowErrorTextNeeded);
            return result;
        }

        //[Scenario("Verify event doesn't fire for virtual = false")]
        [Scenario(true)]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            grid.VirtualMode = false;
            int rowIndex = p.ru.GetRange(0, grid.RowCount - 1);

            eventCount = 0;
            eventsArgs = null;

            string errorText = grid.Rows[rowIndex].ErrorText;
            result.IncCounters(0, eventCount, "Event fired when grid.VirtualMode = false.", log);

            return result;
        }

        #endregion

        void grid_RowErrorTextNeeded(object sender, DataGridViewRowErrorTextNeededEventArgs e)
        {
            eventCount++;
            eventsArgs = e;
        }
        void grid_RowErrorTextNeeded2(object sender, DataGridViewRowErrorTextNeededEventArgs e)
        {
            setErrorText = e.ErrorText = scenarioParams.ru.GetString(0, Int32.MaxValue);
            eventCount++;
            eventsArgs = e;
        }
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify event fires when accessing row's error text.

//@ Verify event doesn't fire for virtual = false


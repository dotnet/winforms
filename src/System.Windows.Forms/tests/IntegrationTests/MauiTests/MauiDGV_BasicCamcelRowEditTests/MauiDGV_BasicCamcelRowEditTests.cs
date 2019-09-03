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
// Testcase:    DGV_BasicCancelRowEdit
// Description: CancelRowEdit
// Author:      t-timw
//
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDGV_BasicCamcelRowEditTests : ReflectBase
    {

        #region Testcase setup
        public MauiDGV_BasicCamcelRowEditTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDGV_BasicCamcelRowEditTests(args));
        }
        DataGridView grid;
        int eventCount = 0;
        string sendString = "Vitamin A&D Fat free milk";

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            grid = DataGridViewUtils.GetSimpleDataGridView();
            grid.CancelRowEdit += new QuestionEventHandler(grid_CancelRowEdit);
            Controls.Add(grid);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            grid.EndEdit(); // In case the previous scenario left the grid in editing mode
            eventCount = 0;
            grid.VirtualMode = true;
            sendString += "Wilcox";
            return base.BeforeScenario(p, scenario);
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("VirtualMode = True. Enter edit mode on new row, <escape> to cancel.  Verify event fires. ")]
        [Scenario(true)]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            int columnIndex = p.ru.GetRange(0, grid.ColumnCount - 1);
            int rowIndex = grid.RowCount - 1;
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];

            grid.BeginEdit(true);
            SendKeys.SendWait(sendString + "{ESCAPE}");
            Application.DoEvents();
            result.IncCounters(1, eventCount, "Event fired an unexpected number of times.", log);

            // Try editing and cancelling edit in a row other than the new row
            columnIndex = p.ru.GetRange(0, grid.ColumnCount - 1);
            rowIndex = p.ru.GetRange(0, grid.RowCount - 2);
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];
            eventCount = 0;
            SendKeys.SendWait(sendString + "Try editing and cancelling edit in a row other than the new row" + "{ESCAPE}");
            Application.DoEvents();
            result.IncCounters(0, eventCount, "Event fired when editing a row other than new row.", log);
            return result;
        }

        //[Scenario("Verify event fires when hitting <escape> while not in editing mode.  See VSCurrent 495012")]
        [Scenario(true)]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            int columnIndex = p.ru.GetRange(0, grid.ColumnCount - 1);

            grid.CurrentCell = grid.Rows[grid.RowCount - 1].Cells[columnIndex];
            SendKeys.SendWait("{Escape}");
            Application.DoEvents();
            result.IncCounters(1, eventCount, "Event did not fire when hitting escape while not in editing mode.", log);

            return result;
        }

        //[Scenario("VirtualMode = False. Enter edit mode, <escape> to cancel.  Verify event doesn't fire.")]
        [Scenario(true)]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            grid.VirtualMode = false;
            int columnIndex = p.ru.GetRange(0, grid.ColumnCount - 1);
            int rowIndex = grid.RowCount - 1;
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];

            // Try for new row
            grid.BeginEdit(true);
            SendKeys.SendWait(sendString + "VirtualMode = False. Enter edit mode.{ESCAPE}");
            Application.DoEvents();
            result.IncCounters(0, eventCount, "Event fired an unexpected number of times. (Shouldnt fire when virtual = false)", log);

            // Try editing and cancelling edit in a row other than the new row
            columnIndex = p.ru.GetRange(0, grid.ColumnCount - 1);
            rowIndex = p.ru.GetRange(0, grid.RowCount - 1);
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];
            eventCount = 0;
            SendKeys.SendWait(sendString + "Try editing and cancelling edit in a row other than the new row{ESCAPE}");
            Application.DoEvents();
            result.IncCounters(0, eventCount, "Event fired when editing a row other than new row.", log);
            return result;
        }

        //[Scenario("Call CancelEdit.  Verify event fires.")]
        [Scenario(true)]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            int columnIndex = p.ru.GetRange(0, grid.ColumnCount - 1);
            int rowIndex = grid.RowCount - 1;
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];

            grid.BeginEdit(true);
            SendKeys.SendWait(sendString);
            grid.CancelEdit();
            Application.DoEvents();
            result.IncCounters(1, eventCount, "Event fired an unexpected number of times.", log);

            // Try editing and cancelling edit in a row other than the new row
            columnIndex = p.ru.GetRange(0, grid.ColumnCount - 1);
            rowIndex = p.ru.GetRange(0, grid.RowCount - 2);
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];
            eventCount = 0;
            SendKeys.SendWait(sendString + "Try editing and cancelling edit in a row other than the new row");
            grid.CancelEdit();

            Application.DoEvents();
            result.IncCounters(0, eventCount, "Event fired when editing a row other than new row.", log);
            return result;
        }

        #endregion

        void grid_CancelRowEdit(object sender, QuestionEventArgs e)
        {
            eventCount++;
        }
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ VirtualMode = True. Enter edit mode on new row, &lt;escape&gt; to cancel.  Verify event fires. 
//@ Verify event fires when hitting <escape> while not in editing mode.  See VSCurrent 495012
//@ VirtualMode = False. Enter edit mode, &lt;escape&gt; to cancel.  Verify event doesn't fire.
//@ Call CancelEdit.  Verify event fires.

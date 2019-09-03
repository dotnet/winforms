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
using System.Reflection;
using System.Threading;
using System.Windows.Forms.IntegrationTests.Common;

//
// Testcase:    DGV_BasicCellParsing
// Description: 
// Author:      t-timw
//
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDGV_BasicCellParsingTests : ReflectBase
    {

        #region Testcase setup
        public MauiDGV_BasicCellParsingTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDGV_BasicCellParsingTests(args));
        }
        DataGridView grid;
        int eventCount = 0;
        DataGridViewCellParsingEventArgs eventsArgs;
        String testString = "a";
        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            grid = DataGridViewUtils.GetSimpleDataGridView();
            Controls.Add(grid);
            grid.CellParsing += new DataGridViewCellParsingEventHandler(grid_CellParsing);
        }

        protected override bool BeforeScenario(TParams p, MethodInfo scenario)
        {
            testString += "1";
            return base.BeforeScenario(p, scenario);
        }

        #endregion

        [Scenario(false)]
        public ScenarioResult CheckEventFire(int expectedTimesFired, DataGridViewCellParsingEventArgs expectedArgs)
        {
            ScenarioResult result = new ScenarioResult();

            result.IncCounters(expectedTimesFired, eventCount, "Event fired an unexpected number of times.", log);
            if (expectedTimesFired > 0)
            {
                result.IncCounters(CompareEventArgs(expectedArgs, eventsArgs));
            }

            return result;
        }
        [Scenario(false)]
        public ScenarioResult CompareEventArgs(EventArgs expectedArgs, EventArgs actualArgs)
        {
            ScenarioResult result = new ScenarioResult();

            if (expectedArgs.GetType() != actualArgs.GetType())
            {
                result.IncCounters(expectedArgs.GetType(), actualArgs.GetType(), "The event arguments are of different types.", log);
            }
            PropertyInfo[] pi = expectedArgs.GetType().GetProperties();
            foreach (PropertyInfo p in pi)
            {
                if (p.Name == "CellStyle")
                {
                    result.IncCounters(DataGridViewUtils.CompareCellStyles((DataGridViewCellStyle)p.GetValue(expectedArgs, null), (DataGridViewCellStyle)p.GetValue(actualArgs, null)),
                                       "e.CellStyle is incorrect.", log);

                }
                else
                {
                    result.IncCounters(p.GetValue(expectedArgs, null), p.GetValue(actualArgs, null), "e." + p.Name + " is incorrect.", log);
                }
            }

            return result;
        }
        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("Change cell value and leave cell by mouseclick in another cell")]
        [Scenario(true)]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            int rowIndex = 0;
            int columnIndex = 0;
            DataGridViewCell cell = grid.Rows[rowIndex].Cells[columnIndex];
            grid.CurrentCell = cell;
            grid.BeginEdit(true);
            eventCount = 0;

            SendKeys.SendWait(testString);

            Rectangle cellRectangle = grid.GetCellDisplayRectangle(columnIndex, rowIndex + 1, false);
            Point clickPt = grid.PointToScreen(new Point(cellRectangle.X + (cellRectangle.Width / 2), cellRectangle.Y + (cellRectangle.Height / 2)));
            Mouse.Click(clickPt.X, clickPt.Y);

            Application.DoEvents();

            DataGridViewCellStyle expectedStyle = cell.GetInheritedStyle(null, rowIndex, true);
            DataGridViewCellParsingEventArgs expectedArgs = new DataGridViewCellParsingEventArgs(rowIndex, columnIndex, testString, cell.ValueType, expectedStyle);

            result.IncCounters(CheckEventFire(1, expectedArgs));
            return result;
        }

        //[Scenario("Change cell value and leave cell by keyboard ")]
        [Scenario(true)]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            int rowIndex = 0;
            int columnIndex = 0;
            DataGridViewCell cell = grid.Rows[rowIndex].Cells[columnIndex];

            grid.CurrentCell = cell;
            grid.BeginEdit(true);
            eventCount = 0;

            SendKeys.SendWait(testString);
            // Leave cell using TAB
            SendKeys.SendWait("{TAB}");

            Application.DoEvents();

            DataGridViewCellStyle expectedStyle = cell.GetInheritedStyle(null, rowIndex, true);
            DataGridViewCellParsingEventArgs expectedArgs = new DataGridViewCellParsingEventArgs(rowIndex, columnIndex, testString, cell.ValueType, expectedStyle);

            result.IncCounters(CheckEventFire(1, expectedArgs));
            return result;
        }

        //[Scenario("Change cell value and <ctrl>+<enter>")]
        [Scenario(true)]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            int rowIndex = 0;
            int columnIndex = 0;
            DataGridViewCell cell = grid.Rows[rowIndex].Cells[columnIndex];

            grid.CurrentCell = cell;
            grid.BeginEdit(true);
            eventCount = 0;

            SendKeys.SendWait(testString);
            SendKeys.SendWait("^{ENTER}");

            Application.DoEvents();

            DataGridViewCellStyle expectedStyle = cell.GetInheritedStyle(null, rowIndex, true);
            DataGridViewCellParsingEventArgs expectedArgs = new DataGridViewCellParsingEventArgs(rowIndex, columnIndex, testString, cell.ValueType, expectedStyle);

            result.IncCounters(CheckEventFire(1, expectedArgs));
            return result;
        }

        //[Scenario("Enter value in cell.  Change current cell programmatically while cell is still in editing mode.")]
        [Scenario(true)]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            int rowIndex = 0;
            int columnIndex = 0;
            DataGridViewCell cell = grid.Rows[rowIndex].Cells[columnIndex];

            grid.CurrentCell = cell;
            grid.BeginEdit(true);
            eventCount = 0;

            SendKeys.SendWait(testString);

            grid.CurrentCell = grid.Rows[rowIndex + 1].Cells[columnIndex];
            Application.DoEvents();

            DataGridViewCellStyle expectedStyle = cell.GetInheritedStyle(null, rowIndex, true);
            DataGridViewCellParsingEventArgs expectedArgs = new DataGridViewCellParsingEventArgs(rowIndex, columnIndex, testString, cell.ValueType, expectedStyle);

            result.IncCounters(CheckEventFire(1, expectedArgs));
            return result;
        }

        //[Scenario("Should not fire when enter and exit editing mode w/o changing cell value.")]
        [Scenario(true)]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            int rowIndex = 0;
            int columnIndex = 0;
            DataGridViewCell cell = grid.Rows[rowIndex].Cells[columnIndex];

            grid.CurrentCell = cell;
            eventCount = 0;
            grid.BeginEdit(true);
            grid.EndEdit(p.ru.GetEnumValue<DataGridViewDataErrorContexts>());

            Application.DoEvents();

            result.IncCounters(CheckEventFire(0, null));
            return result;
        }

        //[Scenario("should not fire for changing cell value programmatically")]
        [Scenario(true)]
        public ScenarioResult Scenario6(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            int rowIndex = 0;
            int columnIndex = 0;
            DataGridViewCell cell = grid.Rows[rowIndex].Cells[columnIndex];

            grid.CurrentCell = cell;
            grid.BeginEdit(true);
            eventCount = 0;

            cell.Value = testString;

            Application.DoEvents();

            result.IncCounters(CheckEventFire(0, null));
            return result;
        }

        #endregion

        void grid_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
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
//@ Change cell value and leave cell by mouseclick in another cell
//@ Change cell value and leave cell by keyboard 
//@ Change cell value and &lt;ctrl&gt;+&lt;enter&gt;
//@ Enter value in cell.  Change current cell programmatically while cell is still in editing mode.
//@ Should not fire when enter and exit editing mode w/o changing cell value.
//@ should not fire for changing cell value programmatically

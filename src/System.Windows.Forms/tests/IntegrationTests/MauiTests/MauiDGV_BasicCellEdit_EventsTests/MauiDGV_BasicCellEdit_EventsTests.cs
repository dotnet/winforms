// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Threading;
using WFCTestLib.Log;
using ReflectTools;
using Maui.Core;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Windows.Forms.IntegrationTests.Common;

//
// Testcase:    DGV_BasicCellEdit Events
// Description: CellBeginEdit, CellEndEdit
// Author:      t-timw
//
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDGV_BasicCellEdit_EventsTests : ReflectBase
    {

        #region Testcase setup
        public MauiDGV_BasicCellEdit_EventsTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDGV_BasicCellEdit_EventsTests(args));
        }
        DataGridView grid;
        bool pass = false;

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            ResetGrid();
        }

        #endregion
        #region Helpers
        #region EventInfo Stuff
        // EventInfo - Class to store an event and its arguments
        class EventInfo
        {
            public EventInfo(EventArgs e, string eventName)
            {
                this.EventArgs = e;
                this.eventName = eventName;
            }
            private EventArgs eventArgs;
            public EventArgs EventArgs
            {
                get
                {
                    return eventArgs;

                }
                set
                {
                    eventArgs = value;
                }
            }
            private string eventName;
            public string EventName
            {
                get
                {
                    return eventName;
                }
                set
                {
                    eventName = value;
                }
            }

        }

        List<EventInfo> events = new List<EventInfo>();

        void OutputEvents(List<EventInfo> eventList)
        {
            foreach (EventInfo ei in eventList)
            {
                scenarioParams.log.WriteLine(GetTextForEvent(ei));
            }
        }
        void AddEvent(List<EventInfo> eventList, EventArgs e, string eventName)
        {

            eventList.Add(new EventInfo(e, eventName));
        }
        string GetTextForEvent(EventInfo ei)
        {
            string text = GetTextForEvent(ei.EventArgs);

            return ei.EventName + ": " + text;

        }
        string GetTextForEvent(EventArgs e)
        {
            PropertyInfo[] pi = e.GetType().GetProperties();
            if (pi.Length == 0)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pi.Length - 1; i++)
            {
                sb.Append(pi[i].Name + ": " + pi[i].GetValue(e, null) + "; ");
            }
            sb.Append(pi[pi.Length - 1].Name + ": " + pi[pi.Length - 1].GetValue(e, null));
            return sb.ToString();
        }
        string GetTextForEvent(DataGridViewCellMouseEventArgs e)
        {
            return e.ColumnIndex + " " + e.RowIndex + " " + e.Location + " " + e.Button;
        }
        string GetTextForEvent(DataGridViewCellValidatingEventArgs e)
        {
            return e.ColumnIndex + " " + e.RowIndex + " " + e.FormattedValue + " " + e.Cancel;
        }
        public MouseFlags MouseButtonsToMouseFlags(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    return MouseFlags.LeftButton;
                    //break;
                case MouseButtons.Middle:
                    return MouseFlags.MiddleButton;
                    //break;
                case MouseButtons.Right:
                    return MouseFlags.RightButton;
                   //break;
                case MouseButtons.XButton1:
                    return MouseFlags.XButton1;
                    //break;
                case MouseButtons.XButton2:
                    return MouseFlags.XButton2;
                   // break;
                case MouseButtons.None:
                default:
                    throw new ArgumentException();
            }
        }
        //[Scenario(false)]
        [Scenario(false)]
        // Passes if the event args are: 1) of the same type and 2) their properties have equal values
        public ScenarioResult CompareEventArgs(EventArgs expectedArgs, EventArgs actualArgs)
        {
            ScenarioResult result = new ScenarioResult();
            // HACK: Special cases because there are no constructors for DataGridViewCellEventArgs and DataGridViewCellCancelEventArgs

            if (actualArgs is DataGridViewCellCancelEventArgs)
            {
                DataGridViewCellCancelEventArgs actualArgs2 = (DataGridViewCellCancelEventArgs)(actualArgs);
                DataGridViewCellMouseEventArgs expectedArgs2 = (DataGridViewCellMouseEventArgs)(expectedArgs);
                result.IncCounters(expectedArgs2.RowIndex, actualArgs2.RowIndex, "e.RowIndex doesn't match.", log);
                result.IncCounters(expectedArgs2.ColumnIndex, actualArgs2.ColumnIndex, "e.ColumnIndex doesn't match.", log);
                return result;
            }
            if (actualArgs is DataGridViewCellEventArgs)
            {
                result.IncCounters(((DataGridViewCellMouseEventArgs)expectedArgs).ColumnIndex, ((DataGridViewCellEventArgs)actualArgs).ColumnIndex,
                    "e.ColumnIndex doesn't match", log);
                result.IncCounters(((DataGridViewCellMouseEventArgs)expectedArgs).RowIndex, ((DataGridViewCellEventArgs)actualArgs).RowIndex,
                    "e.RowIndex doesn't match.", log);
                return result;
            }
            else
            {
                result.IncCounters(expectedArgs.GetType(), actualArgs.GetType(), "Event argument types dont match.", log);
                if (expectedArgs.GetType() != actualArgs.GetType())
                {
                    // can't compare properties of different types, so just return here
                    return result;
                }
            }

            PropertyInfo[] piArray = expectedArgs.GetType().GetProperties();
            foreach (PropertyInfo p in piArray)
            {
                if (p.Name == "X" || p.Name == "Y")
                {
                    int expectedValue = (int)p.GetValue(expectedArgs, null);
                    int actualValue = (int)p.GetValue(actualArgs, null);
                    result.IncCounters(expectedValue == actualValue, "Coordinates don't match.", log);
                }
                else if (p.Name == "Location")
                {
                    Point expectedLocation = (Point)p.GetValue(expectedArgs, null);
                    Point actualLocation = (Point)p.GetValue(actualArgs, null);
                    result.IncCounters(expectedLocation.X == actualLocation.X && expectedLocation.Y == actualLocation.Y, "e.Location doesn't match.", log);
                }
                else
                {
                    result.IncCounters(p.GetValue(expectedArgs, null).ToString(), p.GetValue(actualArgs, null).ToString(), "e." + p.Name + " doesn't match.", log);
                }
            }
            return result;
        }
        #endregion
        // Resets the datagridview
        public void ResetGrid()
        {
            ResetGrid(DataGridViewUtils.GetSimpleDataGridView());
        }

        public void ResetGrid(DataGridView newGrid)
        {
            Controls.Remove(grid);
            grid = newGrid;
            Controls.Add(newGrid);
            grid.Focus();

            InstallHandlers();
            ResetEvents();

        }
        public void InstallHandlers()
        {
            grid.CellBeginEdit += new DataGridViewCellCancelEventHandler(grid_CellBeginEdit);
            grid.CellEndEdit += new DataGridViewCellEventHandler(grid_CellEndEdit);
        }
        public void ResetEvents()
        {
            events.Clear();
        }

        // Helpers to check the firing of an event
        //[Scenario(false)]
        [Scenario(false)]
        public ScenarioResult CheckNumberEventsFired(int expectedNumber)
        {
            return new ScenarioResult(expectedNumber, events.Count, "An unexpected number of events fired.", log);
        }
        //[Scenario(false)]
        [Scenario(false)]
        public ScenarioResult CheckEventFire(int eventIndex, EventArgs expectedArgs, string eventName)
        {
            ScenarioResult result = new ScenarioResult();
            result.IncCounters(eventIndex < events.Count, "Looking for event (" + eventName + ") at invalid index.", log);
            if (!result.IsPassing)
            {
                // Just return here instead of trying to access a list member that doesn't exist.
                return result;
            }
            EventInfo ei = events[eventIndex];
            result.IncCounters(eventName, ei.EventName, "Event name is incorrect.", scenarioParams.log);
            result.IncCounters(CompareEventArgs(expectedArgs, ei.EventArgs));
            return result;
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("Verify that CellEndEdit fires when we CancelEdit programmatically, or with <escape>. ")]
        [Scenario(true)]
        public ScenarioResult CancelEditTest(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            p.log.WriteLine("Beginning edit and cancelling programmatically");
            int columnIndex = 0;
            int rowIndex = 0;
            ResetGrid();

            // Using DataGridViewCellMouseEventArgs because there is no way to create DataGridViewCellCancelEvent args.
            DataGridViewCellMouseEventArgs expectedArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex, 0, 0, new MouseEventArgs(0, 0, 0, 0, 0));
            grid.BeginEdit(true);
            result.IncCounters(CheckNumberEventsFired(1));
            result.IncCounters(CheckEventFire(0, expectedArgs, "CellBeginEdit"));
            grid.CancelEdit();
            // Don't expect any events to fire on grid.CancelEdit
            result.IncCounters(CheckNumberEventsFired(1));
            result.IncCounters(CheckEventFire(0, expectedArgs, "CellBeginEdit"));
            // Begin Edit, then cancel edit with <escape>
            ResetGrid();
            grid.BeginEdit(true);
            SendKeys.SendWait("{escape}");
            result.IncCounters(CheckNumberEventsFired(2));
            result.IncCounters(CheckEventFire(0, expectedArgs, "CellBeginEdit"));
            result.IncCounters(CheckEventFire(1, expectedArgs, "CellEndEdit"));
            return result;
        }

        //[Scenario("Verify that CellEndEdit fires when we EndEdit. ")]
        [Scenario(true)]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            ResetGrid();
            int rowIndex = 1;
            int columnIndex = 1;
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];
            grid.BeginEdit(true);
            grid.EndEdit(p.ru.GetEnumValue<DataGridViewDataErrorContexts>());

            Application.DoEvents();
            DataGridViewCellMouseEventArgs expectedArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex, 0, 0, new MouseEventArgs(0, 0, 0, 0, 0));
            result.IncCounters(CheckNumberEventsFired(2));
            result.IncCounters(CheckEventFire(0, expectedArgs, "CellBeginEdit"));
            result.IncCounters(CheckEventFire(1, expectedArgs, "CellEndEdit"));
            return result;
        }

        //[Scenario("Verify that CellEndEdit does not fire when CommitEdit.")]
        [Scenario(true)]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            ResetGrid();
            int rowIndex = 1;
            int columnIndex = 1;

            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];

            grid.BeginEdit(true);
            grid.CommitEdit(new DataGridViewDataErrorContexts());

            DataGridViewCellMouseEventArgs expectedArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex, 0, 0, new MouseEventArgs(0, 0, 0, 0, 0));

            result.IncCounters(CheckNumberEventsFired(1));
            result.IncCounters(CheckEventFire(0, expectedArgs, "CellBeginEdit"));

            return result;
        }

        //[Scenario("Verify that CellBeginEdit fires when editing textbox cell.")]
        [Scenario(true)]
        public ScenarioResult Scenario4(TParams p)
        {
            return CancelEditTest(p);
        }

        //[Scenario("Verify that CellBeginEdit fires when editing a cell via keyboard.")]
        [Scenario(true)]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            int rowIndex = 1;
            int columnIndex = 1;

            ResetGrid();

            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];

            // Send some keys to edit
            SendKeys.SendWait("ABCDD");

            DataGridViewCellMouseEventArgs expectedArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex, 0, 0, new MouseEventArgs(0, 0, 0, 0, 0));

            result.IncCounters(CheckNumberEventsFired(1));
            result.IncCounters(CheckEventFire(0, expectedArgs, "CellBeginEdit"));

            return result;
        }


        //[Scenario("Verify an InvalidOperationException is thrown when calling BeginEdit(bool selectAll) in CellBeginEdit")]
        [Scenario(true)]
        public ScenarioResult Scenario6(TParams p)
        {
            int rowIndex = 1;
            int columnIndex = 1;

            ResetGrid();
            grid.CellBeginEdit -= grid_CellBeginEdit;
            Application.DoEvents();
            grid.CellBeginEdit += new DataGridViewCellCancelEventHandler(grid_InvalidCellBeginEdit);

            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];

            // Send some keys to edit
            SendKeys.SendWait("ABCDD");
            Application.DoEvents();

            return new ScenarioResult(pass);
        }

        //[Scenario("Verify that event fires when editing combobox cell.")]
        [Scenario(true)]
        public ScenarioResult Scenario7(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            ResetGrid();
            int rowIndex = 0;
            int columnIndex = 0;
            grid.Rows[rowIndex].Cells[columnIndex] = new DataGridViewComboBoxCell();
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];
            grid.BeginEdit(true);

            DataGridViewCellMouseEventArgs expectedArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex, 0, 0, new MouseEventArgs(0, 0, 0, 0, 0));

            result.IncCounters(CheckNumberEventsFired(1));
            result.IncCounters(CheckEventFire(0, expectedArgs, "CellBeginEdit"));

            grid.EndEdit(p.ru.GetEnumValue<DataGridViewDataErrorContexts>());
            result.IncCounters(CheckNumberEventsFired(2));
            result.IncCounters(CheckEventFire(1, expectedArgs, "CellEndEdit"));

            return result;
        }

        #endregion
        #region Event Handlers
        void grid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            AddEvent(events, e, "CellBeginEdit");
        }

        void grid_InvalidCellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            try
            {
                grid.BeginEdit(true);
            }
            catch (InvalidOperationException)
            {
                pass = true;
            }
        }

        void grid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            AddEvent(events, e, "CellEndEdit");
        }
        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify that CellEndEdit fires when we CancelEdit programmatically, or with &lt;escape&gt;. 
//@ Verify that CellEndEdit fires when we EndEdit. 
//@ Verify that CellEndEdit does not fire when CommitEdit.
//@ Verify that CellBeginEdit fires when editing textbox cell.
//@ Verify that CellBeginEdit fires when editing a cell via keyboard.
//@ Verify an InvalidOperationException is thrown when calling BeginEdit(bool selectAll) in CellBeginEdit
//@ Verify that event fires when editing combobox cell.

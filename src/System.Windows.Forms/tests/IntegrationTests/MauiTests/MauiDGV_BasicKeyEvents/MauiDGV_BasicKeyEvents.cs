// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Windows.Forms;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms.IntegrationTests.Common;

//
// Testcase:    DGV_BasicKeyEvents
// Description: 
// Author:      t-timw
//
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDGV_BasicKeyEvents : ReflectBase
    {

        #region Testcase setup
        public MauiDGV_BasicKeyEvents(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDGV_BasicKeyEvents(args));
        }
        DataGridView grid;
        //int eventCount = 0;

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            grid = DataGridViewUtils.GetSimpleDataGridView();
            Controls.Add(grid);
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
        [Scenario(false)]
        // Passes if the event args are: 1) of the same type and 2) their properties have equal values
        public ScenarioResult CompareEventArgs(EventArgs expectedArgs, EventArgs actualArgs)
        {
            ScenarioResult result = new ScenarioResult();
            if (actualArgs is DataGridViewCellEventArgs)
            {
                // HACK: Special case because there is no way to create a DataGridViewCellEventArgs as expectedArgs
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
                    result.IncCounters(expectedValue == actualValue,
                        "Coordinates don't match.", log);
                }
                else if (p.Name == "Location")
                {
                    Point expectedLocation = (Point)p.GetValue(expectedArgs, null);
                    Point actualLocation = (Point)p.GetValue(actualArgs, null);
                    result.IncCounters(expectedLocation == actualLocation, "e.Location doesn't match.", log);
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

        // Overloaded to use a specific grid for the test
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
            grid.KeyPress += new KeyPressEventHandler(grid_KeyPress);
            grid.KeyDown += new KeyEventHandler(grid_KeyDown);
            grid.KeyUp += new KeyEventHandler(grid_KeyUp);
        }
        public void ResetEvents()
        {
            events.Clear();
        }
        // Helpers to check the firing of an event
        [Scenario(false)]
        public ScenarioResult CheckNumberEventsFired(int expectedNumber)
        {
            return new ScenarioResult(expectedNumber, events.Count, "An unexpected number of events fired.", log);
        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult CheckEventFire(int eventIndex, EventArgs expectedArgs, string eventName)
        {
            ScenarioResult result = new ScenarioResult();
            if (eventIndex >= events.Count)
            {
                result.IncCounters(eventIndex < events.Count, "Looking for (" + eventName + ") at  invalid index (" + eventIndex.ToString() + ")");
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
        //[Scenario("Press character key while cell is selected.  Verify no events fire.")]
        [Scenario(true)]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult result = new ScenarioResult();


            ResetGrid(DataGridViewUtils.GetDataGridViewWithImageControls(p, true));
            int rowIndex = 0;
            int columnIndex = 0;

            while (grid.SelectedCells.Count > 0)
            {
                grid.SelectedCells[0].Selected = false;
            }
            grid.EditMode = DataGridViewEditMode.EditProgrammatically;
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];
            grid.CurrentCell.Selected = true;

            SendKeys.SendWait("a");
            Application.DoEvents();

            KeyEventArgs expectedKeyEventArgs = new KeyEventArgs(Keys.A);
            KeyPressEventArgs expectedKeyPressEventArgs = new KeyPressEventArgs('a');

            result.IncCounters(CheckNumberEventsFired(3));
            result.IncCounters(CheckEventFire(0, expectedKeyEventArgs, "KeyDown"));
            result.IncCounters(CheckEventFire(1, expectedKeyPressEventArgs, "KeyPress"));
            result.IncCounters(CheckEventFire(2, expectedKeyEventArgs, "KeyUp"));

            return result;
        }

        //[Scenario("Press <escape> while cell is selected.  Verify events fire.")]
        [Scenario(true)]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult result = new ScenarioResult();


            ResetGrid(DataGridViewUtils.GetDataGridViewWithImageControls(p, true));
            int rowIndex = 0;
            int columnIndex = 0;

            while (grid.SelectedCells.Count > 0)
            {
                grid.SelectedCells[0].Selected = false;
            }
            grid.EditMode = DataGridViewEditMode.EditProgrammatically;
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];
            grid.CurrentCell.Selected = true;

            SendKeys.SendWait("{escape}");
            Application.DoEvents();

            KeyEventArgs expectedKeyEventArgs = new KeyEventArgs(Keys.Escape);
            KeyPressEventArgs expectedKeyPressEventArgs = new KeyPressEventArgs('\x1b');

            result.IncCounters(CheckNumberEventsFired(3));
            result.IncCounters(CheckEventFire(0, expectedKeyEventArgs, "KeyDown"));
            result.IncCounters(CheckEventFire(1, expectedKeyPressEventArgs, "KeyPress"));
            result.IncCounters(CheckEventFire(2, expectedKeyEventArgs, "KeyUp"));
            return result;
        }

        //[Scenario("Press character key while row is selected.  Verify events fire.")]
        [Scenario(true)]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult result = new ScenarioResult();


            ResetGrid(DataGridViewUtils.GetDataGridViewWithImageControls(p, true));

            while (grid.SelectedCells.Count > 0)
            {
                grid.SelectedCells[0].Selected = false;
            }
            grid.EditMode = DataGridViewEditMode.EditProgrammatically;
            grid.Rows[0].Selected = true;

            SendKeys.SendWait("a");
            Application.DoEvents();

            KeyEventArgs expectedKeyEventArgs = new KeyEventArgs(Keys.A);
            KeyPressEventArgs expectedKeyPressEventArgs = new KeyPressEventArgs('a');

            result.IncCounters(CheckNumberEventsFired(3));
            result.IncCounters(CheckEventFire(0, expectedKeyEventArgs, "KeyDown"));
            result.IncCounters(CheckEventFire(1, expectedKeyPressEventArgs, "KeyPress"));
            result.IncCounters(CheckEventFire(2, expectedKeyEventArgs, "KeyUp"));

            return result;
        }

        //[Scenario("Press character key while all cells are selected. Verify events fire.")]
        [Scenario(true)]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult result = new ScenarioResult();


            ResetGrid(DataGridViewUtils.GetDataGridViewWithImageControls(p, true));
            //int rowIndex = 0;
            //int columnIndex = 0;

            while (grid.SelectedCells.Count > 0)
            {
                grid.SelectedCells[0].Selected = false;
            }
            grid.EditMode = DataGridViewEditMode.EditProgrammatically;
            for (int i = 0; i < grid.RowCount; i++)
            {
                grid.Rows[i].Selected = true;
            }
            SendKeys.SendWait("a");
            Application.DoEvents();

            KeyEventArgs expectedKeyEventArgs = new KeyEventArgs(Keys.A);
            KeyPressEventArgs expectedKeyPressEventArgs = new KeyPressEventArgs('a');

            result.IncCounters(CheckNumberEventsFired(3));
            result.IncCounters(CheckEventFire(0, expectedKeyEventArgs, "KeyDown"));
            result.IncCounters(CheckEventFire(1, expectedKeyPressEventArgs, "KeyPress"));
            result.IncCounters(CheckEventFire(2, expectedKeyEventArgs, "KeyUp"));

            return result;
        }

        //[Scenario("Press key with no cells selected. Verify events fire.")]
        [Scenario(true)]
        public ScenarioResult Scenario6(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            ResetGrid();
            grid.CurrentCell = null;

            SendKeys.SendWait("a");
            Application.DoEvents();

            KeyEventArgs expectedKeyEventArgs = new KeyEventArgs(Keys.A);
            KeyPressEventArgs expectedKeyPressEventArgs = new KeyPressEventArgs('a');

            result.IncCounters(CheckNumberEventsFired(3));
            result.IncCounters(CheckEventFire(0, expectedKeyEventArgs, "KeyDown"));
            result.IncCounters(CheckEventFire(1, expectedKeyPressEventArgs, "KeyPress"));
            result.IncCounters(CheckEventFire(2, expectedKeyEventArgs, "KeyUp"));
            return result;
        }

        //[Scenario("Try special keys ( e.g. alt+a, ctrl+c )")]
        [Scenario(true)]
        public ScenarioResult Scenario7(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            ResetGrid();
            grid.CurrentCell = null;

            SendKeys.SendWait("%a");
            Application.DoEvents();

            KeyEventArgs expectedCharKeyEventArgs = new KeyEventArgs(Keys.A | Keys.Alt);
            KeyEventArgs expectedAltKeyDownEventArgs = new KeyEventArgs(Keys.Menu | Keys.Alt);
            KeyEventArgs expectedAltKeyUpEventArgs = new KeyEventArgs(Keys.Menu);

            result.IncCounters(CheckNumberEventsFired(4));
            result.IncCounters(CheckEventFire(0, expectedAltKeyDownEventArgs, "KeyDown"));
            result.IncCounters(CheckEventFire(1, expectedCharKeyEventArgs, "KeyDown"));
            result.IncCounters(CheckEventFire(2, expectedCharKeyEventArgs, "KeyUp"));
            result.IncCounters(CheckEventFire(3, expectedAltKeyUpEventArgs, "KeyUp"));

            // Try CTRL+c
            ResetGrid();
            grid.CurrentCell = null;

            SendKeys.SendWait("^c");
            Application.DoEvents();

            expectedCharKeyEventArgs = new KeyEventArgs(Keys.C | Keys.Control);
            KeyEventArgs expectedCtrlKeyDownEventArgs = new KeyEventArgs(Keys.Control | Keys.ControlKey);
            KeyEventArgs expectedCtrlKeyUpEventArgs = new KeyEventArgs(Keys.ControlKey);
            KeyPressEventArgs expectedKeyPressEventArgs = new KeyPressEventArgs('\x3');

            result.IncCounters(CheckEventFire(0, expectedCtrlKeyDownEventArgs, "KeyDown"));
            result.IncCounters(CheckEventFire(1, expectedCharKeyEventArgs, "KeyDown"));
            result.IncCounters(CheckEventFire(2, expectedKeyPressEventArgs, "KeyPress"));
            result.IncCounters(CheckEventFire(3, expectedCharKeyEventArgs, "KeyUp"));
            result.IncCounters(CheckEventFire(4, expectedCtrlKeyUpEventArgs, "KeyUp"));
            return result;
        }

        //[Scenario("Verify event doesn't fire when pressing keys while in editing mode. ")]
        [Scenario(true)]
        public ScenarioResult Scenario8(TParams p)
        {
            ScenarioResult result = new ScenarioResult();


            ResetGrid();
            //int rowIndex = 0;
            //int columnIndex = 0;

            while (grid.SelectedCells.Count > 0)
            {
                grid.SelectedCells[0].Selected = false;
            }

            grid.Rows[0].Cells[0].Selected = true;
            grid.CurrentCell = grid.Rows[0].Cells[0];

            grid.BeginEdit(true);
            SendKeys.SendWait("hsif");
            Application.DoEvents();

            KeyEventArgs expectedKeyEventArgs = new KeyEventArgs(Keys.A);
            KeyPressEventArgs expectedKeyPressEventArgs = new KeyPressEventArgs('a');

            result.IncCounters(CheckNumberEventsFired(0));

            return result;
        }

        #endregion

        void grid_KeyPress(object sender, KeyPressEventArgs e)
        {
            AddEvent(events, e, "KeyPress");
        }

        void grid_KeyDown(object sender, KeyEventArgs e)
        {
            AddEvent(events, e, "KeyDown");
        }

        void grid_KeyUp(object sender, KeyEventArgs e)
        {
            AddEvent(events, e, "KeyUp");
        }
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Press character key while cell is selected.  Verify no events fire.
//@ Press &lt;escape&gt; while cell is selected.  Verify events fire.
//@ Press character key while row is selected.  Verify events fire.
//@ Press character key while all cells are selected. Verify events fire.
//@ Press key with no cells selected. Verify events fire.
//@ Try special keys ( e.g. alt+a, ctrl+c )
//@ Verify event doesn't fire when pressing keys while in editing mode. 

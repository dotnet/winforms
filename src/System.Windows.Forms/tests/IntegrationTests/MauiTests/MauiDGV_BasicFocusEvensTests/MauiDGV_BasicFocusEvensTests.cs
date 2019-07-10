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
using Maui.Core;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms.IntegrationTests.Common;


//
// Testcase:    DGV_BasicFocusEvents
// Description: Enter, GotFocus, Leave, MouseEnter, MouseLeave, MouseHover, CellEnter, CellLeave
// Author:      t-timw
//
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDGV_BasicFocusEvensTests : ReflectBase
    {

        #region Testcase setup
        public MauiDGV_BasicFocusEvensTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDGV_BasicFocusEvensTests(args));
        }
        DataGridView grid;
        Button button;

        // Used for the the WaitForHover() method
        bool mouseHoverFired = false;

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, MethodInfo scenario)
        {
            ResetGrid();
            return base.BeforeScenario(p, scenario);
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
            log.WriteLine("Events [" + eventList.Count + "]");
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
                   // break;
                case MouseButtons.Right:
                    return MouseFlags.RightButton;
                    //break;
                case MouseButtons.XButton1:
                    return MouseFlags.XButton1;
                    //break;
                case MouseButtons.XButton2:
                    return MouseFlags.XButton2;
                    //break;
                case MouseButtons.None:
                default:
                    throw new ArgumentException();
            }
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
                    result.IncCounters(expectedLocation.X == actualLocation.Y, "e.Location doesn't match.", log);
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

            ResetButton();

            InstallHandlers();

            // Move mouse pointer to button so that it is in a predictable position.
            ClickButton(MouseClickType.SingleClick, MouseButtons.Left);
            Application.DoEvents();
            ResetEvents();
            mouseHoverFired = false;

        }
        public void InstallHandlers()
        {
            grid.GotFocus += new EventHandler(grid_GotFocus);
            grid.LostFocus += new EventHandler(grid_LostFocus);
            grid.Enter += new EventHandler(grid_Enter);
            grid.Leave += new EventHandler(grid_Leave);
            grid.MouseEnter += new EventHandler(grid_MouseEnter);
            grid.MouseLeave += new EventHandler(grid_MouseLeave);
            grid.MouseHover += new EventHandler(grid_MouseHover);
        }
        public void ResetButton()
        {
            if (grid == null)
            {
                ResetButton(0, 0);
            }
            else
            {
                ResetButton(grid.DisplayRectangle.X, grid.DisplayRectangle.Y + grid.DisplayRectangle.Height + 10);
            }
        }
        public void ResetButton(int x, int y)
        {
            Controls.Remove(button);
            button = new Button();
            button.Text = "Button";
            Controls.Add(button);

            button.Location = new Point(x, y);

            button.Show();
        }
        public void ClickButton(MouseClickType clickType, MouseButtons mouseButton)
        {
            // Point to click on wrt form.
            Point formPT = new Point(button.Location.X + button.Size.Width / 2, button.Location.Y + button.Size.Height / 2);
            // Point to click on wrt screen
            Point clickPt = this.PointToScreen(formPT);
            Maui.Core.Mouse.Click(clickType, MouseButtonsToMouseFlags(mouseButton), clickPt.X, clickPt.Y);
        }
        public void ResetEvents()
        {
            Application.DoEvents();
            events.Clear();
        }
        // Clicks a cell in the datagridview
        public void ClickCell()
        {
            Rectangle cellRectangle = grid.GetCellDisplayRectangle(0, 0, false);
            Point clickPt = grid.PointToScreen(new Point(cellRectangle.X + cellRectangle.Width / 2, cellRectangle.Y + cellRectangle.Height / 2));
            Mouse.Click(clickPt.X, clickPt.Y);
        }
        // Causes application to wait until either: 1) MouseHover fires, or 2) maxTime seconds have passed
        public void WaitHover()
        {
            int maxTime = 10;
            DateTime now = DateTime.Now;
            while (!mouseHoverFired && DateTime.Now < now.AddSeconds(maxTime))
            {
                Application.DoEvents();
            }
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
        //[Scenario("Change focus to the control when it doesn't have focus using mouse.  Verify Enter and GotFocus fires.")]
        [Scenario(true)]
        public ScenarioResult Scenario1(TParams p)
        {

            ScenarioResult result = new ScenarioResult();

            // Change focus from button->grid
            ResetGrid(new DataGridView());
            button.Focus();
            Application.DoEvents();
            ResetEvents();

            Point gridPt = new Point(grid.Width / 2, grid.Height / 2);
            Point clickPt = grid.PointToScreen(gridPt);

            Mouse.Click(clickPt.X, clickPt.Y);

            WaitHover();

            result.IncCounters(CheckNumberEventsFired(4));
            result.IncCounters(CheckEventFire(0, new EventArgs(), "MouseEnter"));
            result.IncCounters(CheckEventFire(1, new EventArgs(), "Enter"));
            result.IncCounters(CheckEventFire(2, new EventArgs(), "GotFocus"));
            result.IncCounters(CheckEventFire(3, new EventArgs(), "MouseHover"));
            return result;
        }

        //[Scenario("Change focus to the control when it doesn't have focus using keyboard. Verify Enter and GotFocus fires.")]
        [Scenario(true)]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            // Change focus from button->grid 
            ResetGrid(new DataGridView());

            // Puts focus on button
            button.Focus();
            Application.DoEvents();

            ResetEvents();

            // Bring focus back to grid
            SendKeys.SendWait("{tab}");
            Application.DoEvents();


            result.IncCounters(CheckNumberEventsFired(2));
            result.IncCounters(CheckEventFire(0, new EventArgs(), "Enter"));
            result.IncCounters(CheckEventFire(1, new EventArgs(), "GotFocus"));
            return result;
        }

        //[Scenario("Change focus to/from the control when it has focus using mouse.  Verify Enter and GotFocus fires.")]
        [Scenario(true)]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            // Change focus from grid->button

            ResetGrid(new DataGridView());

            grid.Focus();
            ResetEvents();

            ClickButton(MouseClickType.SingleClick, MouseButtons.Left);

            Application.DoEvents();
            result.IncCounters(CheckNumberEventsFired(2));
            result.IncCounters(CheckEventFire(0, new EventArgs(), "LostFocus"));
            result.IncCounters(CheckEventFire(1, new EventArgs(), "Leave"));

            // Change click grid when it already has focus.  
            ResetGrid(new DataGridView());
            grid.Focus();
            ResetEvents();

            Point gridPt = new Point(grid.DisplayRectangle.Width / 2, grid.DisplayRectangle.Height / 2);
            Point clickPt = grid.PointToScreen(gridPt);

            Mouse.Click(clickPt.X, clickPt.Y);
            Application.DoEvents();

            WaitHover();
            result.IncCounters(CheckNumberEventsFired(2));
            result.IncCounters(CheckEventFire(0, new EventArgs(), "MouseEnter"));
            result.IncCounters(CheckEventFire(1, new EventArgs(), "MouseHover"));
            return result;
        }

        //[Scenario("Change focus to/from the control when it has focus using keyboard. Verify Enter and GotFocus fires.")]
        [Scenario(true)]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            int rowIndex = grid.CurrentCell.RowIndex;
            int columnIndex = grid.CurrentCell.ColumnIndex;

            // Change focus from grid->button with grid focused
            grid.Focus();
            ResetEvents();
            SendKeys.SendWait("^{TAB}");

            result.IncCounters(CheckNumberEventsFired(2));
            result.IncCounters(CheckEventFire(0, new EventArgs(), "Leave"));
            result.IncCounters(CheckEventFire(1, new EventArgs(), "LostFocus"));

            // Move within grid with keyboard
            grid.Focus();
            ResetEvents();
            SendKeys.SendWait("{RIGHT}{LEFT}");
            result.IncCounters(CheckNumberEventsFired(0));
            return result;
        }

        //[Scenario("Change focus programatically.")]
        [Scenario(true)]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            // Change focus to grid when it already has focus.  Verify no events fire.
            ResetGrid();
            grid.Focus();
            ResetEvents();

            grid.Focus();
            result.IncCounters(CheckNumberEventsFired(0));

            // Change focus to grid when it doesn't have focus
            button.Focus();
            ResetEvents();

            grid.Focus();
            result.IncCounters(CheckNumberEventsFired(2));
            result.IncCounters(CheckEventFire(0, new EventArgs(), "Enter"));
            result.IncCounters(CheckEventFire(1, new EventArgs(), "GotFocus"));
            return result;
        }

        #endregion

        #region Event Handlers
        void grid_GotFocus(object sender, EventArgs e)
        {
            AddEvent(events, e, "GotFocus");
        }

        void grid_Enter(object sender, EventArgs e)
        {
            AddEvent(events, e, "Enter");
        }

        void grid_Leave(object sender, EventArgs e)
        {
            AddEvent(events, e, "Leave");
        }

        void grid_MouseEnter(object sender, EventArgs e)
        {
            AddEvent(events, e, "MouseEnter");
        }

        void grid_MouseLeave(object sender, EventArgs e)
        {
            AddEvent(events, e, "MouseLeave");
        }

        void grid_MouseHover(object sender, EventArgs e)
        {
            AddEvent(events, e, "MouseHover");
            mouseHoverFired = true;
        }

        void grid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            AddEvent(events, e, "CellEnter");
        }

        void grid_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            AddEvent(events, e, "CellLeave");
        }

        void grid_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            AddEvent(events, e, "CellMouseEnter");
        }

        void grid_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            AddEvent(events, e, "CellMouseLeave");
        }

        void grid_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            AddEvent(events, e, "CellMouseMove");
        }
        void grid_LostFocus(object sender, EventArgs e)
        {
            AddEvent(events, e, "LostFocus");
        }
        #endregion



    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Change focus to the control when it doesn't have focus using mouse.  Verify Enter and GotFocus fires.

//@ Change focus to the control when it doesn't have focus using keyboard. Verify Enter and GotFocus fires.

//@ Change focus to/from the control when it has focus using mouse.  Verify Enter and GotFocus fires.

//@ Change focus to/from the control when it has focus using keyboard. Verify Enter and GotFocus fires.

//@ Change focus programatically.

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using WFCTestLib.Log;
using ReflectTools;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Maui.Core;
using System.Threading;
using System.Windows.Forms.IntegrationTests.Common;

//
// Testcase:    DGV_BasicClickEvents
// Description: Tests all events involved in clicking on datagridview control
// Author:      t-timw
//
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDGV_BasicClickEventsTests : ReflectBase
    {

        #region Testcase setup
        public MauiDGV_BasicClickEventsTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDGV_BasicClickEventsTests(args));
        }
        DataGridView grid;
        private const int TOLERANCE = 1;

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
                   // break;
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
        // There is a tolerance built in if the values being compared are coordinate values (x, y)
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
                    result.IncCounters(expectedValue <= actualValue + TOLERANCE && expectedValue >= actualValue - TOLERANCE,
                        "Coordinates don't match.", log);
                }
                else if (p.Name == "Location")
                {
                    Point expectedLocation = (Point)p.GetValue(expectedArgs, null);
                    Point actualLocation = (Point)p.GetValue(actualArgs, null);
                    result.IncCounters(expectedLocation.X <= actualLocation.X + TOLERANCE &&
                        expectedLocation.X >= actualLocation.X - TOLERANCE &&
                        expectedLocation.Y <= actualLocation.Y + TOLERANCE &&
                        expectedLocation.Y >= actualLocation.Y - TOLERANCE, "e.Location doesn't match.", log);
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
            grid.MouseDown += new MouseEventHandler(grid_MouseDown);
            grid.CellMouseDown += new DataGridViewCellMouseEventHandler(grid_CellMouseDown);
            grid.Click += new EventHandler(grid_Click);
            grid.MouseClick += new MouseEventHandler(grid_MouseClick);
            grid.CellMouseClick += new DataGridViewCellMouseEventHandler(grid_CellMouseClick);
            grid.CellMouseUp += new DataGridViewCellMouseEventHandler(grid_CellMouseUp);
            grid.MouseUp += new MouseEventHandler(grid_MouseUp);
            grid.CellContentClick += new DataGridViewCellEventHandler(grid_CellContentClick);
            //grid.GotFocus += new EventHandler(grid_GotFocus);
            grid.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(grid_ColumnHeaderMouseClick);
            grid.ColumnHeaderMouseDoubleClick += new DataGridViewCellMouseEventHandler(grid_ColumnHeaderMouseDoubleClick);
            grid.RowHeaderMouseClick += new DataGridViewCellMouseEventHandler(grid_RowHeaderMouseClick);
            grid.RowHeaderMouseDoubleClick += new DataGridViewCellMouseEventHandler(grid_RowHeaderMouseDoubleClick);
            grid.CellClick += new DataGridViewCellEventHandler(grid_CellClick);
            grid.DoubleClick += new EventHandler(grid_DoubleClick);
            grid.MouseDoubleClick += new MouseEventHandler(grid_MouseDoubleClick);
            grid.CellMouseDoubleClick += new DataGridViewCellMouseEventHandler(grid_CellMouseDoubleClick);
            grid.CellDoubleClick += new DataGridViewCellEventHandler(grid_CellDoubleClick);
            grid.CellContentDoubleClick += new DataGridViewCellEventHandler(grid_CellContentDoubleClick);
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
            if (eventName == ei.EventName)
            {
                result.IncCounters(CompareEventArgs(expectedArgs, ei.EventArgs));
            }
            return result;
        }

        // Single Click Scenario Helper
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult ClickTextboxCellAndCheck(MouseClickType clickType, MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            int columnIndex = 0;
            int rowIndex = 0;

            ResetGrid();

            Rectangle contentRectangle = grid.Rows[rowIndex].Cells[columnIndex].ContentBounds;
            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, rowIndex, false));
            // Point (coords wrt screen) to click on
            Point clickPt = new Point(gridRectangle.Left + (contentRectangle.Right - contentRectangle.Left) / 2,
                gridRectangle.Top + (contentRectangle.Bottom - contentRectangle.Top) / 2);
            //Point clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2, gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);

            // To prevent entering in edit mode on the first click
            grid.CurrentCell = grid.Rows[rowIndex + 1].Cells[columnIndex];

            Mouse.Click(clickType, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);

            Application.DoEvents();

            // Construct expected event arguments to pass to the check method
            Point gridOffset = grid.PointToScreen(Point.Empty);
            Rectangle cellRectangle = grid.GetCellDisplayRectangle(rowIndex, columnIndex, false);
            MouseEventArgs expectedSingleMouseEventArgs = new MouseEventArgs(button, 1, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);
            MouseEventArgs expectedDoubleMouseEventArgs = new MouseEventArgs(button, 2, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);
            DataGridViewCellMouseEventArgs expectedSingleDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex,
                clickPt.X - gridOffset.X - cellRectangle.X, clickPt.Y - gridOffset.Y - cellRectangle.Y, expectedSingleMouseEventArgs);
            DataGridViewCellMouseEventArgs expectedDoubleDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex,
                clickPt.X - gridOffset.X - cellRectangle.X, clickPt.Y - gridOffset.Y - cellRectangle.Y, expectedDoubleMouseEventArgs);

            if (clickType == MouseClickType.SingleClick)
            {
                if (button == MouseButtons.Left)
                {
                    result.IncCounters(CheckNumberEventsFired(9));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleDGVCellMouseEventArgs, "CellClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "CellContentClick"));
                    result.IncCounters(CheckEventFire(7, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(8, expectedSingleMouseEventArgs, "MouseUp"));
                }
                else
                {
                    result.IncCounters(CheckNumberEventsFired(7));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(6, expectedSingleMouseEventArgs, "MouseUp"));
                }
            }
            else
            {
                if (button == MouseButtons.Left)
                {
                    result.IncCounters(CheckNumberEventsFired(18));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleDGVCellMouseEventArgs, "CellClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "CellContentClick"));
                    result.IncCounters(CheckEventFire(7, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(8, expectedSingleMouseEventArgs, "MouseUp"));
                    // Second click
                    result.IncCounters(CheckEventFire(9, expectedDoubleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(10, expectedDoubleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(11, expectedDoubleMouseEventArgs, "DoubleClick"));
                    result.IncCounters(CheckEventFire(12, expectedDoubleDGVCellMouseEventArgs, "CellDoubleClick"));
                    result.IncCounters(CheckEventFire(13, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                    result.IncCounters(CheckEventFire(14, expectedDoubleDGVCellMouseEventArgs, "CellMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(15, expectedDoubleDGVCellMouseEventArgs, "CellContentDoubleClick"));
                    result.IncCounters(CheckEventFire(16, expectedDoubleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(17, expectedSingleMouseEventArgs, "MouseUp"));
                }
                else
                {
                    result.IncCounters(CheckNumberEventsFired(14));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(6, expectedSingleMouseEventArgs, "MouseUp"));
                    // Second click
                    result.IncCounters(CheckEventFire(7, expectedDoubleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(8, expectedDoubleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(9, expectedDoubleMouseEventArgs, "DoubleClick"));
                    result.IncCounters(CheckEventFire(10, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                    result.IncCounters(CheckEventFire(11, expectedDoubleDGVCellMouseEventArgs, "CellMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(12, expectedDoubleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(13, expectedSingleMouseEventArgs, "MouseUp"));
                }
            }
            //OutputEvents(events);
            return result;
        }

        [Scenario(false)]
        [Obsolete]
        public ScenarioResult TripleClickTextBoxCellAndCheck(MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            int columnIndex = 0;
            int rowIndex = 0;

            ResetGrid();

            Rectangle contentRectangle = grid.Rows[rowIndex].Cells[columnIndex].ContentBounds;
            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, rowIndex, false));
            // Point (coords wrt screen) to click on
            Point clickPt = new Point(gridRectangle.Left + (contentRectangle.Right - contentRectangle.Left) / 2,
                gridRectangle.Top + (contentRectangle.Bottom - contentRectangle.Top) / 2);
            //Point clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2, gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);

            // To prevent entering in edit mode on the first click
            grid.CurrentCell = grid.Rows[rowIndex + 1].Cells[columnIndex];

            bool flag = true;

            try
            {
                Mouse.Click(MouseClickType.DoubleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
                Mouse.Click(MouseClickType.SingleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
            }
            catch (Exception)
            {
                flag = false;
            }

            result.IncCounters(flag, "Exception occurred during triple click");
            //OutputEvents(events);
            return result;
        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult DragSelectAndCheck(MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            int columnIndex = 0;
            int rowIndex = 0;
            int columnIndex2 = 1;
            int rowIndex2 = 1;

            ResetGrid();
            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, rowIndex, false));
            Rectangle gridRectangle2 = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex2, rowIndex2, false));
            // Point (coords wrt screen) to click on
            Point startDragPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2, gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);
            Point endDragPt = new Point(gridRectangle2.Left + (gridRectangle2.Right - gridRectangle2.Left) / 2, gridRectangle2.Top + (gridRectangle2.Bottom - gridRectangle2.Top) / 2);

            // Drag from startDragPt to endDragPt
            Mouse.ClickDrag(startDragPt.X, startDragPt.Y, endDragPt.X, endDragPt.Y);

            Application.DoEvents();

            // Construct expected event arguments to pass to the check method
            Point gridOffset = grid.PointToScreen(Point.Empty);
            // For the start cell:
            Rectangle cellRectangle = grid.GetCellDisplayRectangle(rowIndex, columnIndex, false);
            MouseEventArgs expectedMouseEventArgs = new MouseEventArgs(button, 1, startDragPt.X - gridOffset.X, startDragPt.Y - gridOffset.Y, 0);
            DataGridViewCellMouseEventArgs expectedDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex,
                startDragPt.X - gridOffset.X - cellRectangle.X, startDragPt.Y - gridOffset.Y - cellRectangle.Y, expectedMouseEventArgs);
            // For the end cell
            Rectangle cellRectangle2 = grid.GetCellDisplayRectangle(rowIndex2, columnIndex2, false);
            MouseEventArgs expectedMouseEventArgs2 = new MouseEventArgs(button, 1, endDragPt.X - gridOffset.X, endDragPt.Y - gridOffset.Y, 0);
            DataGridViewCellMouseEventArgs expectedDGVCellMouseEventArgs2 = new DataGridViewCellMouseEventArgs(columnIndex2, rowIndex2,
                endDragPt.X - gridOffset.X - cellRectangle2.X, endDragPt.Y - gridOffset.Y - cellRectangle2.Y, expectedMouseEventArgs2);

            result.IncCounters(CheckNumberEventsFired(6).IsPassing, log, BugDb.VSWhidbey, 276810, "CellContentClick is firing unnecessarily.");
            result.IncCounters(CheckEventFire(0, expectedMouseEventArgs, "MouseDown"));
            result.IncCounters(CheckEventFire(1, expectedDGVCellMouseEventArgs, "CellMouseDown"));
            result.IncCounters(CheckEventFire(2, expectedMouseEventArgs2, "Click"));
            result.IncCounters(CheckEventFire(3, expectedMouseEventArgs2, "MouseClick"));
            result.IncCounters(CheckEventFire(4, expectedDGVCellMouseEventArgs2, "CellMouseUp"));
            result.IncCounters(CheckEventFire(5, expectedMouseEventArgs2, "MouseUp"));
            return result;
        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult ClickTopLeftHeaderAndCheck(MouseClickType clickType, MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            int columnIndex = -1;
            int rowIndex = -1;

            ResetGrid();
            grid.CurrentCell = grid.Rows[1].Cells[0];
            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, rowIndex, false));
            // Point (coords wrt screen) to click on
            Point clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2,
                gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);

            // Single click on header cell
            Mouse.Click(clickType, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);

            Application.DoEvents();

            // Construct expected event arguments to pass to the check method
            Point gridOffset = grid.PointToScreen(Point.Empty);
            Rectangle cellRectangle = grid.GetCellDisplayRectangle(rowIndex, columnIndex, false);
            MouseEventArgs expectedSingleMouseEventArgs = new MouseEventArgs(button, 1, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);
            MouseEventArgs expectedDoubleMouseEventArgs = new MouseEventArgs(button, 2, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);

            DataGridViewCellMouseEventArgs expectedSingleDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex,
                clickPt.X - gridOffset.X - cellRectangle.X, clickPt.Y - gridOffset.Y - cellRectangle.Y, expectedSingleMouseEventArgs);
            DataGridViewCellMouseEventArgs expectedDoubleDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex,
                       clickPt.X - gridOffset.X - cellRectangle.X, clickPt.Y - gridOffset.Y - cellRectangle.Y, expectedDoubleMouseEventArgs);

            if (clickType == MouseClickType.SingleClick)
            {
                if (button == MouseButtons.Left)
                {
                    result.IncCounters(CheckNumberEventsFired(8));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleDGVCellMouseEventArgs, "CellClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(7, expectedSingleMouseEventArgs, "MouseUp"));
                }
                else
                {
                    result.IncCounters(CheckNumberEventsFired(7));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(6, expectedSingleMouseEventArgs, "MouseUp"));
                }

            }
            else
            {
                if (button == MouseButtons.Left)
                {
                    result.IncCounters(CheckNumberEventsFired(16));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleDGVCellMouseEventArgs, "CellClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(7, expectedSingleMouseEventArgs, "MouseUp"));
                    // Second click
                    result.IncCounters(CheckEventFire(8, expectedDoubleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(9, expectedDoubleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(10, expectedDoubleMouseEventArgs, "DoubleClick"));
                    result.IncCounters(CheckEventFire(11, expectedDoubleDGVCellMouseEventArgs, "CellDoubleClick"));
                    result.IncCounters(CheckEventFire(12, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                    result.IncCounters(CheckEventFire(13, expectedDoubleDGVCellMouseEventArgs, "CellMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(14, expectedDoubleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(15, expectedSingleMouseEventArgs, "MouseUp"));
                }
                else
                {
                    result.IncCounters(CheckNumberEventsFired(14));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(6, expectedSingleMouseEventArgs, "MouseUp"));
                    // Second click
                    result.IncCounters(CheckEventFire(7, expectedDoubleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(8, expectedDoubleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(9, expectedDoubleMouseEventArgs, "DoubleClick"));
                    result.IncCounters(CheckEventFire(10, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                    result.IncCounters(CheckEventFire(11, expectedDoubleDGVCellMouseEventArgs, "CellMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(12, expectedDoubleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(13, expectedSingleMouseEventArgs, "MouseUp"));
                }

            }

            return result;
        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult TripleClickTopLeftHeaderAndCheck(MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            int columnIndex = -1;
            int rowIndex = -1;

            ResetGrid();
            grid.CurrentCell = grid.Rows[1].Cells[0];
            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, rowIndex, false));
            // Point (coords wrt screen) to click on
            Point clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2,
                gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);

            // Triple click on header cell
            bool flag = true;

            try
            {
                Mouse.Click(MouseClickType.DoubleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
                Mouse.Click(MouseClickType.SingleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
            }
            catch (Exception)
            {
                flag = false;
            }

            Application.DoEvents();
            result.IncCounters(flag, "Exception occurred during triple click");

            return result;
        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult ClickColumnHeaderAndCheck(MouseClickType clickType, MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            int columnIndex = 0;

            ResetGrid();

            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, -1, false));
            // Point (coords wrt screen) to click on
            Point clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2, gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);

            Mouse.Click(clickType, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
            Application.DoEvents();

            // Construct expected event arguments to pass to the check method
            Point gridOffset = grid.PointToScreen(Point.Empty);
            Rectangle columnRectangle = grid.GetColumnDisplayRectangle(columnIndex, false);
            MouseEventArgs expectedSingleMouseEventArgs = new MouseEventArgs(button, 1, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);
            MouseEventArgs expectedDoubleMouseEventArgs = new MouseEventArgs(button, 2, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);

            DataGridViewCellMouseEventArgs expectedSingleDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, -1,
                clickPt.X - gridOffset.X - columnRectangle.X, clickPt.Y - gridOffset.Y - columnRectangle.Y, expectedSingleMouseEventArgs);
            DataGridViewCellMouseEventArgs expectedDoubleDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, -1,
                       clickPt.X - gridOffset.X - columnRectangle.X, clickPt.Y - gridOffset.Y - columnRectangle.Y, expectedDoubleMouseEventArgs);

            if (clickType == MouseClickType.SingleClick)
            {
                if (button == MouseButtons.Left)
                {
                    result.IncCounters(CheckNumberEventsFired(10));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleDGVCellMouseEventArgs, "CellClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "ColumnHeaderMouseClick"));
                    result.IncCounters(CheckEventFire(7, expectedSingleDGVCellMouseEventArgs, "CellContentClick"));
                    result.IncCounters(CheckEventFire(8, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(9, expectedSingleMouseEventArgs, "MouseUp"));
                }
                else
                {
                    result.IncCounters(CheckNumberEventsFired(8));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "ColumnHeaderMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(7, expectedSingleMouseEventArgs, "MouseUp"));

                }

            }
            else
            {
                if (button == MouseButtons.Left)
                {
                    result.IncCounters(CheckNumberEventsFired(20));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleDGVCellMouseEventArgs, "CellClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "ColumnHeaderMouseClick"));
                    result.IncCounters(CheckEventFire(7, expectedSingleDGVCellMouseEventArgs, "CellContentClick"));
                    result.IncCounters(CheckEventFire(8, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(9, expectedSingleMouseEventArgs, "MouseUp"));
                    // Second click
                    result.IncCounters(CheckEventFire(10, expectedDoubleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(11, expectedDoubleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(12, expectedDoubleMouseEventArgs, "DoubleClick"));
                    result.IncCounters(CheckEventFire(13, expectedDoubleDGVCellMouseEventArgs, "CellDoubleClick"));
                    result.IncCounters(CheckEventFire(14, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                    result.IncCounters(CheckEventFire(15, expectedDoubleDGVCellMouseEventArgs, "CellMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(16, expectedDoubleDGVCellMouseEventArgs, "ColumnHeaderMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(17, expectedDoubleDGVCellMouseEventArgs, "CellContentDoubleClick"));
                    result.IncCounters(CheckEventFire(18, expectedDoubleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(19, expectedSingleMouseEventArgs, "MouseUp"));
                }
                else
                {
                    result.IncCounters(CheckNumberEventsFired(16));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "ColumnHeaderMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(7, expectedSingleMouseEventArgs, "MouseUp"));
                    // Second click
                    result.IncCounters(CheckEventFire(8, expectedDoubleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(9, expectedDoubleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(10, expectedDoubleMouseEventArgs, "DoubleClick"));
                    result.IncCounters(CheckEventFire(11, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                    result.IncCounters(CheckEventFire(12, expectedDoubleDGVCellMouseEventArgs, "CellMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(13, expectedDoubleDGVCellMouseEventArgs, "ColumnHeaderMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(14, expectedDoubleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(15, expectedSingleMouseEventArgs, "MouseUp"));
                }

            }
            return result;
        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult TripleClickColumnHeaderAndCheck(MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            int columnIndex = 0;

            ResetGrid();

            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, -1, false));
            // Point (coords wrt screen) to click on
            Point clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2, gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);

            bool flag = true;

            try
            {
                Mouse.Click(MouseClickType.DoubleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
                Mouse.Click(MouseClickType.SingleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
                Application.DoEvents();
            }
            catch (Exception)
            {
                flag = false;
            }
            result.IncCounters(flag, "Exception thrown during triple click");

            return result;
        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult ClickEmptySpaceAndCheck(MouseClickType clickType, MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            // grid with no cells
            ResetGrid(new DataGridView());

            Point pointOnGrid = new Point(grid.Width / 2, grid.Height / 2);
            Point clickPt = grid.PointToScreen(pointOnGrid);

            Mouse.Click(clickType, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
            Application.DoEvents();

            // Construct expected event arguments to pass to the check method
            Point gridOffset = grid.PointToScreen(Point.Empty);
            MouseEventArgs expectedSingleMouseEventArgs = new MouseEventArgs(button, 1, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);
            MouseEventArgs expectedDoubleMouseEventArgs = new MouseEventArgs(button, 2, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);

            if (clickType == MouseClickType.SingleClick)
            {
                result.IncCounters(CheckNumberEventsFired(4));
                result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                result.IncCounters(CheckEventFire(1, expectedSingleMouseEventArgs, "Click"));
                result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "MouseClick"));
                result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseUp"));
            }
            else
            {
                result.IncCounters(CheckNumberEventsFired(8));
                result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                result.IncCounters(CheckEventFire(1, expectedSingleMouseEventArgs, "Click"));
                result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "MouseClick"));
                result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseUp"));
                result.IncCounters(CheckEventFire(4, expectedDoubleMouseEventArgs, "MouseDown"));
                result.IncCounters(CheckEventFire(5, expectedDoubleMouseEventArgs, "DoubleClick"));
                result.IncCounters(CheckEventFire(6, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                result.IncCounters(CheckEventFire(7, expectedSingleMouseEventArgs, "MouseUp"));

            }
            return result;
        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult TripleClickEmptySpaceAndCheck(MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            // grid with no cells
            ResetGrid(new DataGridView());

            Point pointOnGrid = new Point(grid.Width / 2, grid.Height / 2);
            Point clickPt = grid.PointToScreen(pointOnGrid);

            bool flag = true;

            try
            {
                Mouse.Click(MouseClickType.DoubleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
                Mouse.Click(MouseClickType.SingleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
            }
            catch (Exception)
            {
                flag = false;
            }
            Application.DoEvents();

            result.IncCounters(flag, "Exception occured during triple click");
            return result;
        }
        [Scenario(false)]
        public ScenarioResult ClickScrollBarAndCheck(MouseClickType clickType, MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();

            ResetGrid();
            grid.ScrollBars = ScrollBars.Both; // Make sure the scrollbars are visible
            Point gridPt = new Point(grid.Width - 10, grid.Height / 2);
            Point clickPt = grid.PointToScreen(gridPt);

            Mouse.Click(clickType, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);

            result.IncCounters(CheckNumberEventsFired(0));
            return result;

        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult TripleClickScrollBarAndCheck(MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();

            ResetGrid();
            grid.ScrollBars = ScrollBars.Both; // Make sure the scrollbars are visible
            Point gridPt = new Point(grid.Width - 10, grid.Height / 2);
            Point clickPt = grid.PointToScreen(gridPt);

            bool flag = true;

            try
            {
                Mouse.Click(MouseClickType.DoubleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
                Mouse.Click(MouseClickType.SingleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
            }
            catch (Exception)
            {
                flag = false;
            }

            result.IncCounters(flag, "Exception occured during triple click");
            return result;

        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult ClickButtonCellAndCheck(MouseClickType clickType, MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            ResetGrid();
            int rowIndex = 0;
            int columnIndex = 0;

            grid.Rows[rowIndex].Cells[columnIndex] = new DataGridViewButtonCell();

            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, rowIndex, false));
            // Point (coords wrt screen) to click on
            Point clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2, gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);

            Mouse.Click(clickType, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);

            Application.DoEvents();

            // Construct expected event arguments to pass to the check method
            Point gridOffset = grid.PointToScreen(Point.Empty);
            Rectangle cellRectangle = grid.GetCellDisplayRectangle(rowIndex, columnIndex, false);
            MouseEventArgs expectedSingleMouseEventArgs = new MouseEventArgs(button, 1, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);
            MouseEventArgs expectedDoubleMouseEventArgs = new MouseEventArgs(button, 2, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);
            DataGridViewCellMouseEventArgs expectedSingleDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex,
                clickPt.X - gridOffset.X - cellRectangle.X, clickPt.Y - gridOffset.Y - cellRectangle.Y, expectedSingleMouseEventArgs);
            DataGridViewCellMouseEventArgs expectedDoubleDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex,
                clickPt.X - gridOffset.X - cellRectangle.X, clickPt.Y - gridOffset.Y - cellRectangle.Y, expectedDoubleMouseEventArgs);


            if (clickType == MouseClickType.SingleClick)
            {
                if (button == MouseButtons.Left)
                {
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleDGVCellMouseEventArgs, "CellClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "CellContentClick"));
                    result.IncCounters(CheckEventFire(7, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(8, expectedSingleMouseEventArgs, "MouseUp"));

                }
                else
                {
                    result.IncCounters(CheckNumberEventsFired(7));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(6, expectedSingleMouseEventArgs, "MouseUp"));

                }

            }
            else // DoubleClick
            {
                if (button == MouseButtons.Left)
                {
                    result.IncCounters(CheckNumberEventsFired(18));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleDGVCellMouseEventArgs, "CellClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "CellContentClick"));
                    result.IncCounters(CheckEventFire(7, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(8, expectedSingleMouseEventArgs, "MouseUp"));
                    // Second click
                    result.IncCounters(CheckEventFire(9, expectedDoubleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(10, expectedDoubleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(11, expectedDoubleMouseEventArgs, "DoubleClick"));
                    result.IncCounters(CheckEventFire(12, expectedDoubleDGVCellMouseEventArgs, "CellDoubleClick"));
                    result.IncCounters(CheckEventFire(13, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                    result.IncCounters(CheckEventFire(14, expectedDoubleDGVCellMouseEventArgs, "CellMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(15, expectedDoubleDGVCellMouseEventArgs, "CellContentDoubleClick"));
                    result.IncCounters(CheckEventFire(16, expectedDoubleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(17, expectedSingleMouseEventArgs, "MouseUp"));
                }
                else
                {
                    result.IncCounters(CheckNumberEventsFired(14));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(6, expectedSingleMouseEventArgs, "MouseUp"));

                    // Second click
                    result.IncCounters(CheckEventFire(7, expectedDoubleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(8, expectedDoubleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(9, expectedDoubleMouseEventArgs, "DoubleClick"));
                    result.IncCounters(CheckEventFire(10, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                    result.IncCounters(CheckEventFire(11, expectedDoubleDGVCellMouseEventArgs, "CellMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(12, expectedDoubleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(13, expectedSingleMouseEventArgs, "MouseUp"));

                }

            }
            return result;
        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult TripleClickButtonCellAndCheck(MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            ResetGrid();
            int rowIndex = 0;
            int columnIndex = 0;

            grid.Rows[rowIndex].Cells[columnIndex] = new DataGridViewButtonCell();

            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, rowIndex, false));
            // Point (coords wrt screen) to click on
            Point clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2, gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);

            bool flag = true;

            try
            {
                Mouse.Click(MouseClickType.DoubleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
                Mouse.Click(MouseClickType.SingleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
            }
            catch (Exception)
            {
                flag = false;
            }
            Application.DoEvents();

            result.IncCounters(flag, "Exception occured during triple click");
            return result;
        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult TripleClickImageCellAndCheck(MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            ResetGrid(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewImageColumn)));

            int columnIndex = 0;
            int rowIndex = 0;


            grid.CurrentCell = grid.Rows[rowIndex + 1].Cells[columnIndex];
            Rectangle contentRectangle = grid.Rows[rowIndex].Cells[columnIndex].ContentBounds;
            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, rowIndex, false));
            // Point (coords wrt screen) to click on
            Point clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2,
                gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);

            Application.DoEvents();

            bool flag = true;

            try
            {
                Mouse.Click(MouseClickType.DoubleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
                Mouse.Click(MouseClickType.SingleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
            }
            catch (Exception)
            {
                flag = false;
            }
            Application.DoEvents();

            result.IncCounters(flag, "Exception occured during triple click");
            return result;
        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult ClickImageCellAndCheck(MouseClickType clickType, MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            ResetGrid(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewImageColumn)));

            int columnIndex = 0;
            int rowIndex = 0;


            grid.CurrentCell = grid.Rows[rowIndex + 1].Cells[columnIndex];
            Rectangle contentRectangle = grid.Rows[rowIndex].Cells[columnIndex].ContentBounds;
            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, rowIndex, false));
            // Point (coords wrt screen) to click on
            Point clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2,
                gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);

            Application.DoEvents();


            Mouse.Click(clickType, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);

            Application.DoEvents();
            // Construct expected event arguments to pass to the check method
            Point gridOffset = grid.PointToScreen(Point.Empty);
            Rectangle cellRectangle = grid.GetCellDisplayRectangle(rowIndex, columnIndex, false);
            MouseEventArgs expectedSingleMouseEventArgs = new MouseEventArgs(button, 1, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);
            MouseEventArgs expectedDoubleMouseEventArgs = new MouseEventArgs(button, 2, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);
            DataGridViewCellMouseEventArgs expectedSingleDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex,
                clickPt.X - gridOffset.X - cellRectangle.X, clickPt.Y - gridOffset.Y - cellRectangle.Y, expectedSingleMouseEventArgs);
            DataGridViewCellMouseEventArgs expectedDoubleDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex,
                clickPt.X - gridOffset.X - cellRectangle.X, clickPt.Y - gridOffset.Y - cellRectangle.Y, expectedDoubleMouseEventArgs);
            if (clickType == MouseClickType.SingleClick)
            {
                if (button == MouseButtons.Left)
                {
                    result.IncCounters(CheckNumberEventsFired(9));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleDGVCellMouseEventArgs, "CellClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "CellContentClick"));
                    result.IncCounters(CheckEventFire(7, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(8, expectedSingleMouseEventArgs, "MouseUp"));
                }
                else
                {
                    result.IncCounters(CheckNumberEventsFired(7));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(6, expectedSingleMouseEventArgs, "MouseUp"));
                }

            }
            else
            {
                if (button == MouseButtons.Left)
                {
                    result.IncCounters(CheckNumberEventsFired(18));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleDGVCellMouseEventArgs, "CellClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "CellContentClick"));
                    result.IncCounters(CheckEventFire(7, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(8, expectedSingleMouseEventArgs, "MouseUp"));

                    // Second click
                    result.IncCounters(CheckEventFire(9, expectedDoubleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(10, expectedDoubleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(11, expectedDoubleMouseEventArgs, "DoubleClick"));
                    result.IncCounters(CheckEventFire(12, expectedDoubleDGVCellMouseEventArgs, "CellDoubleClick"));
                    result.IncCounters(CheckEventFire(13, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                    result.IncCounters(CheckEventFire(14, expectedDoubleDGVCellMouseEventArgs, "CellMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(15, expectedDoubleDGVCellMouseEventArgs, "CellContentDoubleClick"));
                    result.IncCounters(CheckEventFire(16, expectedDoubleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(17, expectedSingleMouseEventArgs, "MouseUp"));
                }
                else
                {
                    result.IncCounters(CheckNumberEventsFired(14));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(6, expectedSingleMouseEventArgs, "MouseUp"));
                    // Second click
                    result.IncCounters(CheckEventFire(7, expectedDoubleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(8, expectedDoubleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(9, expectedDoubleMouseEventArgs, "DoubleClick"));
                    result.IncCounters(CheckEventFire(10, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                    result.IncCounters(CheckEventFire(11, expectedDoubleDGVCellMouseEventArgs, "CellMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(12, expectedDoubleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(13, expectedSingleMouseEventArgs, "MouseUp"));
                }

            }

            return result;
        }
        [Scenario(false)]
        [Obsolete]
        // NOTE: This helper only clicks on LinkCells with no content, because strange things happen when
        // clicking on a LinkCell's content.  This is most probably an issue with MAUI, so it is not a 
        // pressing issue.
        public ScenarioResult TripleClickLinkCellAndCheck(MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            ResetGrid(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewLinkColumn)));
            int rowIndex = 0;
            int columnIndex = 0;

            grid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.Rows[rowIndex].Cells[columnIndex].Value = "link cell";

            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, rowIndex, false));
            // Point (coords wrt screen) to click on
            Point clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2, gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);

            bool flag = true;

            try
            {
                Mouse.Click(MouseClickType.DoubleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
                Mouse.Click(MouseClickType.SingleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
            }
            catch (Exception)
            {
                flag = false;
            }
            Application.DoEvents();

            result.IncCounters(flag, "Exception occured during triple click");
            return result;
        }
        [Scenario(false)]
        [Obsolete]
        // NOTE: This helper only clicks on LinkCells with no content, because strange things happen when
        // clicking on a LinkCell's content.  This is most probably an issue with MAUI, so it is not a 
        // pressing issue.
        public ScenarioResult ClickLinkCellAndCheck(MouseClickType clickType, MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            ResetGrid(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewLinkColumn)));
            int rowIndex = 0;
            int columnIndex = 0;

            grid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.Rows[rowIndex].Cells[columnIndex].Value = "link cell";

            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, rowIndex, false));
            // Point (coords wrt screen) to click on
            Point clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2, gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);

            Mouse.Click(clickType, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
            for (int i = 0; i < 100; i++) Application.DoEvents();

            // Construct expected event arguments to pass to the check method
            Point gridOffset = grid.PointToScreen(Point.Empty);
            Rectangle cellRectangle = grid.GetCellDisplayRectangle(rowIndex, columnIndex, false);
            MouseEventArgs expectedSingleMouseEventArgs = new MouseEventArgs(button, 1, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);
            MouseEventArgs expectedDoubleMouseEventArgs = new MouseEventArgs(button, 2, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);
            DataGridViewCellMouseEventArgs expectedSingleDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex,
                clickPt.X - gridOffset.X - cellRectangle.X, clickPt.Y - gridOffset.Y - cellRectangle.Y, expectedSingleMouseEventArgs);
            DataGridViewCellMouseEventArgs expectedDoubleDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex,
                clickPt.X - gridOffset.X - cellRectangle.X, clickPt.Y - gridOffset.Y - cellRectangle.Y, expectedDoubleMouseEventArgs);

            if (clickType == MouseClickType.SingleClick)
            {
                if (button == MouseButtons.Left)
                {
                    result.IncCounters(CheckNumberEventsFired(9));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleDGVCellMouseEventArgs, "CellClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "CellContentClick"));
                    result.IncCounters(CheckEventFire(7, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(8, expectedSingleMouseEventArgs, "MouseUp"));
                }
                else
                {
                    result.IncCounters(CheckNumberEventsFired(7));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(6, expectedSingleMouseEventArgs, "MouseUp"));

                }

            }
            else // DoubleClick
            {
                if (button == MouseButtons.Left)
                {
                    result.IncCounters(CheckNumberEventsFired(18));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleDGVCellMouseEventArgs, "CellClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "CellContentClick"));
                    result.IncCounters(CheckEventFire(7, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(8, expectedSingleMouseEventArgs, "MouseUp"));
                    // Second click
                    result.IncCounters(CheckEventFire(9, expectedDoubleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(10, expectedDoubleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(11, expectedDoubleMouseEventArgs, "DoubleClick"));
                    result.IncCounters(CheckEventFire(12, expectedDoubleDGVCellMouseEventArgs, "CellDoubleClick"));
                    result.IncCounters(CheckEventFire(13, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                    result.IncCounters(CheckEventFire(14, expectedDoubleDGVCellMouseEventArgs, "CellMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(15, expectedDoubleDGVCellMouseEventArgs, "CellContentDoubleClick"));
                    result.IncCounters(CheckEventFire(16, expectedDoubleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(17, expectedSingleMouseEventArgs, "MouseUp"));

                }
                else
                {
                    result.IncCounters(CheckNumberEventsFired(14));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(6, expectedSingleMouseEventArgs, "MouseUp"));
                    // Second click
                    result.IncCounters(CheckEventFire(7, expectedDoubleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(8, expectedDoubleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(9, expectedDoubleMouseEventArgs, "DoubleClick"));
                    result.IncCounters(CheckEventFire(10, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                    result.IncCounters(CheckEventFire(11, expectedDoubleDGVCellMouseEventArgs, "CellMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(12, expectedDoubleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(13, expectedSingleMouseEventArgs, "MouseUp"));
                }

            }
            return result;
        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult TripleClickComboBoxCellAndCheck(MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            ResetGrid();
            int rowIndex = 0;
            int columnIndex = 0;

            grid.CurrentCell = grid.Rows[1].Cells[0];
            grid.Rows[rowIndex].Cells[columnIndex] = new DataGridViewComboBoxCell();

            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, rowIndex, false));
            // Point (coords wrt screen) to click on
            Point clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2, gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);

            bool flag = true;

            try
            {
                Mouse.Click(MouseClickType.DoubleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
                Mouse.Click(MouseClickType.SingleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
            }
            catch (Exception)
            {
                flag = false;
            }
            Application.DoEvents();

            result.IncCounters(flag, "Exception occured during triple click");

            Application.DoEvents();
            return result;
        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult ClickComboBoxCellAndCheck(MouseClickType clickType, MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            ResetGrid();
            int rowIndex = 0;
            int columnIndex = 0;

            grid.CurrentCell = grid.Rows[1].Cells[0];
            grid.Rows[rowIndex].Cells[columnIndex] = new DataGridViewComboBoxCell();

            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, rowIndex, false));
            // Point (coords wrt screen) to click on
            Point clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2, gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);

            Mouse.Click(clickType, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);

            Application.DoEvents();

            // Construct expected event arguments to pass to the check method
            Point gridOffset = grid.PointToScreen(Point.Empty);
            Rectangle cellRectangle = grid.GetCellDisplayRectangle(rowIndex, columnIndex, false);
            MouseEventArgs expectedSingleMouseEventArgs = new MouseEventArgs(button, 1, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);
            MouseEventArgs expectedDoubleMouseEventArgs = new MouseEventArgs(button, 2, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);
            DataGridViewCellMouseEventArgs expectedSingleDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex,
                clickPt.X - gridOffset.X - cellRectangle.X, clickPt.Y - gridOffset.Y - cellRectangle.Y, expectedSingleMouseEventArgs);
            DataGridViewCellMouseEventArgs expectedDoubleDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex,
                clickPt.X - gridOffset.X - cellRectangle.X, clickPt.Y - gridOffset.Y - cellRectangle.Y, expectedDoubleMouseEventArgs);

            if (clickType == MouseClickType.SingleClick)
            {
                if (button == MouseButtons.Left)
                {
                    result.IncCounters(CheckNumberEventsFired(8));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleDGVCellMouseEventArgs, "CellClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(7, expectedSingleMouseEventArgs, "MouseUp"));
                }
                else
                {
                    result.IncCounters(CheckNumberEventsFired(7));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(6, expectedSingleMouseEventArgs, "MouseUp"));
                }
            }
            else // DoubleClick
            {
                if (button == MouseButtons.Left)
                {
                    result.IncCounters(CheckNumberEventsFired(16));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleDGVCellMouseEventArgs, "CellClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(7, expectedSingleMouseEventArgs, "MouseUp"));
                    // Second click
                    result.IncCounters(CheckEventFire(8, expectedDoubleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(9, expectedDoubleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(10, expectedDoubleMouseEventArgs, "DoubleClick"));
                    result.IncCounters(CheckEventFire(11, expectedDoubleDGVCellMouseEventArgs, "CellDoubleClick"));
                    result.IncCounters(CheckEventFire(12, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                    result.IncCounters(CheckEventFire(13, expectedDoubleDGVCellMouseEventArgs, "CellMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(14, expectedDoubleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(15, expectedSingleMouseEventArgs, "MouseUp"));

                }
                else
                {
                    result.IncCounters(CheckNumberEventsFired(14));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(6, expectedSingleMouseEventArgs, "MouseUp"));

                    // Second click
                    result.IncCounters(CheckEventFire(7, expectedDoubleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(8, expectedDoubleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(9, expectedDoubleMouseEventArgs, "DoubleClick"));
                    result.IncCounters(CheckEventFire(10, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                    result.IncCounters(CheckEventFire(11, expectedDoubleDGVCellMouseEventArgs, "CellMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(12, expectedDoubleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(13, expectedSingleMouseEventArgs, "MouseUp"));

                }


            }
            return result;
        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult ClickRowHeaderAndCheck(MouseClickType clickType, MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            int columnIndex = -1; // Row Header cell
            int rowIndex = 0;

            ResetGrid();

            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, rowIndex, false));
            // Point (coords wrt screen) to click on
            Point clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2, gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);

            // Single click on header cell
            Mouse.Click(clickType, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);

            Application.DoEvents();

            // Construct expected event arguments to pass to the check method
            Point gridOffset = grid.PointToScreen(Point.Empty);
            Rectangle rowRectangle = grid.GetRowDisplayRectangle(rowIndex, false);

            MouseEventArgs expectedSingleMouseEventArgs = new MouseEventArgs(button, 1, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);
            MouseEventArgs expectedDoubleMouseEventArgs = new MouseEventArgs(button, 2, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);
            DataGridViewCellMouseEventArgs expectedSingleDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex,
                clickPt.X - gridOffset.X - rowRectangle.X, clickPt.Y - gridOffset.Y - rowRectangle.Y, expectedSingleMouseEventArgs);
            DataGridViewCellMouseEventArgs expectedDoubleDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex,
                clickPt.X - gridOffset.X - rowRectangle.X, clickPt.Y - gridOffset.Y - rowRectangle.Y, expectedDoubleMouseEventArgs);

            if (clickType == MouseClickType.SingleClick)
            {
                if (button == MouseButtons.Left)
                {
                    result.IncCounters(CheckNumberEventsFired(9));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleDGVCellMouseEventArgs, "CellClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "RowHeaderMouseClick"));
                    result.IncCounters(CheckEventFire(7, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(8, expectedSingleMouseEventArgs, "MouseUp"));
                }
                else
                {
                    result.IncCounters(CheckNumberEventsFired(8));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "RowHeaderMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(7, expectedSingleMouseEventArgs, "MouseUp"));

                }
            }
            else
            {
                if (button == MouseButtons.Left)
                {
                    result.IncCounters(CheckNumberEventsFired(18));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleDGVCellMouseEventArgs, "CellClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "RowHeaderMouseClick"));
                    result.IncCounters(CheckEventFire(7, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(8, expectedSingleMouseEventArgs, "MouseUp"));

                    // Second click
                    result.IncCounters(CheckEventFire(9, expectedDoubleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(10, expectedDoubleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(11, expectedDoubleMouseEventArgs, "DoubleClick"));
                    result.IncCounters(CheckEventFire(12, expectedDoubleDGVCellMouseEventArgs, "CellDoubleClick"));
                    result.IncCounters(CheckEventFire(13, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                    result.IncCounters(CheckEventFire(14, expectedDoubleDGVCellMouseEventArgs, "CellMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(15, expectedDoubleDGVCellMouseEventArgs, "RowHeaderMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(16, expectedDoubleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(17, expectedSingleMouseEventArgs, "MouseUp"));
                }
                else
                {
                    result.IncCounters(CheckNumberEventsFired(16));
                    result.IncCounters(CheckEventFire(0, expectedSingleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(1, expectedSingleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(2, expectedSingleMouseEventArgs, "Click"));
                    result.IncCounters(CheckEventFire(3, expectedSingleMouseEventArgs, "MouseClick"));
                    result.IncCounters(CheckEventFire(4, expectedSingleDGVCellMouseEventArgs, "CellMouseClick"));
                    result.IncCounters(CheckEventFire(5, expectedSingleDGVCellMouseEventArgs, "RowHeaderMouseClick"));
                    result.IncCounters(CheckEventFire(6, expectedSingleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(7, expectedSingleMouseEventArgs, "MouseUp"));

                    // Second click
                    result.IncCounters(CheckEventFire(8, expectedDoubleMouseEventArgs, "MouseDown"));
                    result.IncCounters(CheckEventFire(9, expectedDoubleDGVCellMouseEventArgs, "CellMouseDown"));
                    result.IncCounters(CheckEventFire(10, expectedDoubleMouseEventArgs, "DoubleClick"));
                    result.IncCounters(CheckEventFire(11, expectedDoubleMouseEventArgs, "MouseDoubleClick"));
                    result.IncCounters(CheckEventFire(12, expectedDoubleDGVCellMouseEventArgs, "CellMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(13, expectedDoubleDGVCellMouseEventArgs, "RowHeaderMouseDoubleClick"));
                    result.IncCounters(CheckEventFire(14, expectedDoubleDGVCellMouseEventArgs, "CellMouseUp"));
                    result.IncCounters(CheckEventFire(15, expectedSingleMouseEventArgs, "MouseUp"));

                }

            }
            return result;
        }
        [Scenario(false)]
        [Obsolete]
        public ScenarioResult EditCellAndCheck(MouseButtons button)
        {
            ScenarioResult result = new ScenarioResult();
            int columnIndex = 0;
            int rowIndex = 0;

            ResetGrid();
            grid.CurrentCell = grid.Rows[rowIndex].Cells[columnIndex];
            grid.BeginEdit(true);
            ResetEvents();
            Rectangle gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(columnIndex, rowIndex, false));
            // Point (coords wrt screen) to click on
            Point clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2, gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);


            // Single click on cell
            Mouse.Click(MouseClickType.SingleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);

            Application.DoEvents();

            // Construct expected event arguments to pass to the check method
            Point gridOffset = grid.PointToScreen(Point.Empty);
            Rectangle cellRectangle = grid.GetCellDisplayRectangle(rowIndex, columnIndex, false);
            MouseEventArgs expectedMouseEventArgs = new MouseEventArgs(button, 1, clickPt.X - gridOffset.X, clickPt.Y - gridOffset.Y, 0);
            DataGridViewCellMouseEventArgs expectedDGVCellMouseEventArgs = new DataGridViewCellMouseEventArgs(columnIndex, rowIndex,
                clickPt.X - gridOffset.X - cellRectangle.X, clickPt.Y - gridOffset.Y - cellRectangle.Y, expectedMouseEventArgs);

            if (button == MouseButtons.Left)
            {
                result.IncCounters(CheckNumberEventsFired(8));
                result.IncCounters(CheckEventFire(0, expectedMouseEventArgs, "MouseDown"));
                result.IncCounters(CheckEventFire(1, expectedDGVCellMouseEventArgs, "CellMouseDown"));
                result.IncCounters(CheckEventFire(2, expectedMouseEventArgs, "Click"));
                result.IncCounters(CheckEventFire(3, expectedDGVCellMouseEventArgs, "CellClick"));
                result.IncCounters(CheckEventFire(4, expectedMouseEventArgs, "MouseClick"));
                result.IncCounters(CheckEventFire(5, expectedDGVCellMouseEventArgs, "CellMouseClick"));
                result.IncCounters(CheckEventFire(6, expectedDGVCellMouseEventArgs, "CellMouseUp"));
                result.IncCounters(CheckEventFire(7, expectedMouseEventArgs, "MouseUp"));
            }
            else
            {
                result.IncCounters(CheckNumberEventsFired(4));
                result.IncCounters(CheckEventFire(0, expectedMouseEventArgs, "MouseDown"));
                result.IncCounters(CheckEventFire(1, expectedDGVCellMouseEventArgs, "CellMouseDown"));
                result.IncCounters(CheckEventFire(2, expectedDGVCellMouseEventArgs, "CellMouseUp"));
                result.IncCounters(CheckEventFire(3, expectedMouseEventArgs, "MouseUp"));
            }
            //if (button == MouseButtons.Right)
            //{
            //    //Need to click to get rid of context menu
            //    gridRectangle = grid.RectangleToScreen(grid.GetCellDisplayRectangle(0, 1, false));
            //    // Point (coords wrt screen) to click on
            //    clickPt = new Point(gridRectangle.Left + (gridRectangle.Right - gridRectangle.Left) / 2, gridRectangle.Top + (gridRectangle.Bottom - gridRectangle.Top) / 2);
            //    // Single click on cell
            //    Mouse.Click(MouseClickType.SingleClick, MouseButtonsToMouseFlags(button), clickPt.X, clickPt.Y);
            //    Application.DoEvents();
            //}
            return result;
        }
        #endregion
        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("1) Left/right/middle click on cell")]
        [Scenario(true)]
        [Obsolete]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            p.log.WriteLine("Left single click");
            result.IncCounters(ClickTextboxCellAndCheck(MouseClickType.SingleClick, MouseButtons.Left));
            p.log.WriteLine("Middle single click");
            result.IncCounters(ClickTextboxCellAndCheck(MouseClickType.SingleClick, MouseButtons.Middle));
            p.log.WriteLine("Right single click");
            result.IncCounters(ClickTextboxCellAndCheck(MouseClickType.SingleClick, MouseButtons.Right));
            p.log.WriteLine("Left double click");
            result.IncCounters(ClickTextboxCellAndCheck(MouseClickType.DoubleClick, MouseButtons.Left));
            p.log.WriteLine("Middle double click");
            result.IncCounters(ClickTextboxCellAndCheck(MouseClickType.DoubleClick, MouseButtons.Middle));
            p.log.WriteLine("Right double click");
            result.IncCounters(ClickTextboxCellAndCheck(MouseClickType.DoubleClick, MouseButtons.Right));
            p.log.WriteLine("Left triple click");
            result.IncCounters(TripleClickTextBoxCellAndCheck(MouseButtons.Left));
            p.log.WriteLine("Middle triple click");
            result.IncCounters(TripleClickTextBoxCellAndCheck(MouseButtons.Middle));
            p.log.WriteLine("Right triple click");
            result.IncCounters(TripleClickTextBoxCellAndCheck(MouseButtons.Right));
            return result;
        }

        //[Scenario("2) Click on top left header cell")]
        [Scenario(true)]
        [Obsolete]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            p.log.WriteLine("Left single click");
            result.IncCounters(ClickTopLeftHeaderAndCheck(MouseClickType.SingleClick, MouseButtons.Left));
            p.log.WriteLine("Right single click");
            result.IncCounters(ClickTopLeftHeaderAndCheck(MouseClickType.SingleClick, MouseButtons.Right));
            p.log.WriteLine("Middle single click");
            result.IncCounters(ClickTopLeftHeaderAndCheck(MouseClickType.SingleClick, MouseButtons.Middle));
            p.log.WriteLine("Left double click");
            result.IncCounters(ClickTopLeftHeaderAndCheck(MouseClickType.DoubleClick, MouseButtons.Left));
            p.log.WriteLine("Right double click");
            result.IncCounters(ClickTopLeftHeaderAndCheck(MouseClickType.DoubleClick, MouseButtons.Right));
            p.log.WriteLine("Middle double click");
            result.IncCounters(ClickTopLeftHeaderAndCheck(MouseClickType.DoubleClick, MouseButtons.Middle));
            p.log.WriteLine("Left triple click");
            result.IncCounters(TripleClickTopLeftHeaderAndCheck(MouseButtons.Left));
            p.log.WriteLine("Right triple click");
            result.IncCounters(TripleClickTopLeftHeaderAndCheck(MouseButtons.Right));
            p.log.WriteLine("Middle triple click");
            result.IncCounters(TripleClickTopLeftHeaderAndCheck(MouseButtons.Middle));
            return result;
        }

        //[Scenario("3) Click on column header cell.  Verify ColumnHeaderMouseClick event fires after CellMouseClick, and verify its args")]
        [Scenario(true)]
        [Obsolete]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            p.log.WriteLine("Left single click");
            result.IncCounters(ClickColumnHeaderAndCheck(MouseClickType.SingleClick, MouseButtons.Left));
            p.log.WriteLine("Right single click");
            result.IncCounters(ClickColumnHeaderAndCheck(MouseClickType.SingleClick, MouseButtons.Right));
            p.log.WriteLine("Middle single click");
            result.IncCounters(ClickColumnHeaderAndCheck(MouseClickType.SingleClick, MouseButtons.Middle));

            p.log.WriteLine("Left double click");
            result.IncCounters(ClickColumnHeaderAndCheck(MouseClickType.DoubleClick, MouseButtons.Left));
            p.log.WriteLine("Right double click");
            result.IncCounters(ClickColumnHeaderAndCheck(MouseClickType.DoubleClick, MouseButtons.Right));
            p.log.WriteLine("Middle double click");
            result.IncCounters(ClickColumnHeaderAndCheck(MouseClickType.DoubleClick, MouseButtons.Middle));

            p.log.WriteLine("Left triple click");
            result.IncCounters(TripleClickColumnHeaderAndCheck(MouseButtons.Left));
            p.log.WriteLine("Right triple click");
            result.IncCounters(TripleClickColumnHeaderAndCheck(MouseButtons.Right));
            p.log.WriteLine("Middle triple click");
            result.IncCounters(TripleClickColumnHeaderAndCheck(MouseButtons.Middle));
            return result;

        }

        //[Scenario("4) Click on row header cell.  Verify RowHeaderMouseClick event fires after CellMouseClick, and verify its args")]
        [Scenario(true)]
        [Obsolete]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            p.log.WriteLine("Left single click");
            result.IncCounters(ClickRowHeaderAndCheck(MouseClickType.SingleClick, MouseButtons.Left));
            p.log.WriteLine("Middle single click");
            result.IncCounters(ClickRowHeaderAndCheck(MouseClickType.SingleClick, MouseButtons.Middle));
            p.log.WriteLine("Right single click");
            result.IncCounters(ClickRowHeaderAndCheck(MouseClickType.SingleClick, MouseButtons.Right));

            p.log.WriteLine("Left double click");
            result.IncCounters(ClickRowHeaderAndCheck(MouseClickType.DoubleClick, MouseButtons.Left));
            //        result.IncCounters(ClickRowHeaderAndCheck(MouseClickType.DoubleClick, MouseButtons.Middle));
            //        result.IncCounters(ClickRowHeaderAndCheck(MouseClickType.DoubleClick, MouseButtons.Right));
            return result;
        }

        //[Scenario("5) Click on empty space in datagridview.  Verify only MouseDown, Click, MouseClick, and MouseUp events fire")]
        [Scenario(true)]
        [Obsolete]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            result.IncCounters(ClickEmptySpaceAndCheck(MouseClickType.SingleClick, MouseButtons.Left));
            result.IncCounters(ClickEmptySpaceAndCheck(MouseClickType.SingleClick, MouseButtons.Right));
            result.IncCounters(ClickEmptySpaceAndCheck(MouseClickType.SingleClick, MouseButtons.Middle));
            result.IncCounters(ClickEmptySpaceAndCheck(MouseClickType.DoubleClick, MouseButtons.Left));
            result.IncCounters(ClickEmptySpaceAndCheck(MouseClickType.DoubleClick, MouseButtons.Right));
            result.IncCounters(ClickEmptySpaceAndCheck(MouseClickType.DoubleClick, MouseButtons.Middle));
            result.IncCounters(TripleClickColumnHeaderAndCheck(MouseButtons.Left));
            result.IncCounters(TripleClickColumnHeaderAndCheck(MouseButtons.Right));
            result.IncCounters(TripleClickColumnHeaderAndCheck(MouseButtons.Middle));
            return result;
        }

        //[Scenario("6) Click on scrollbars  Verify no events fire")]
        [Scenario(true)]
        [Obsolete]
        public ScenarioResult Scenario6(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            result.IncCounters(ClickScrollBarAndCheck(MouseClickType.SingleClick, MouseButtons.Left));
            result.IncCounters(ClickScrollBarAndCheck(MouseClickType.SingleClick, MouseButtons.Middle));
            result.IncCounters(ClickScrollBarAndCheck(MouseClickType.DoubleClick, MouseButtons.Left));
            result.IncCounters(ClickScrollBarAndCheck(MouseClickType.DoubleClick, MouseButtons.Middle));
            result.IncCounters(TripleClickScrollBarAndCheck(MouseButtons.Left));
            result.IncCounters(TripleClickScrollBarAndCheck(MouseButtons.Middle));
            return result;
        }

        //[Scenario("7) Click on button cell")]
        [Scenario(true)]
        [Obsolete]
        public ScenarioResult Scenario7(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            p.log.WriteLine("Left single click");
            result.IncCounters(ClickButtonCellAndCheck(MouseClickType.SingleClick, MouseButtons.Left));
            p.log.WriteLine("Middle single click");
            result.IncCounters(ClickButtonCellAndCheck(MouseClickType.SingleClick, MouseButtons.Middle));
            p.log.WriteLine("Right single click");
            result.IncCounters(ClickButtonCellAndCheck(MouseClickType.SingleClick, MouseButtons.Right));
            p.log.WriteLine("Left double click");
            result.IncCounters(ClickButtonCellAndCheck(MouseClickType.DoubleClick, MouseButtons.Left));
            p.log.WriteLine("Middle double click");
            result.IncCounters(ClickButtonCellAndCheck(MouseClickType.DoubleClick, MouseButtons.Middle));
            p.log.WriteLine("Right double click");
            result.IncCounters(ClickButtonCellAndCheck(MouseClickType.DoubleClick, MouseButtons.Right));
            p.log.WriteLine("Left triple click");
            result.IncCounters(TripleClickButtonCellAndCheck(MouseButtons.Left));
            p.log.WriteLine("Middle triple click");
            result.IncCounters(TripleClickButtonCellAndCheck(MouseButtons.Middle));
            p.log.WriteLine("Right triple click");
            result.IncCounters(TripleClickButtonCellAndCheck(MouseButtons.Right));
            return result;
        }

        //[Scenario("8) Click on image cell")]
        [Scenario(true)]
        [Obsolete]
        public ScenarioResult Scenario8(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            p.log.WriteLine("Left single click");
            result.IncCounters(ClickImageCellAndCheck(MouseClickType.SingleClick, MouseButtons.Left));
            p.log.WriteLine("Right single click");
            result.IncCounters(ClickImageCellAndCheck(MouseClickType.SingleClick, MouseButtons.Right));
            p.log.WriteLine("Middle single click");
            result.IncCounters(ClickImageCellAndCheck(MouseClickType.SingleClick, MouseButtons.Middle));
            p.log.WriteLine("Left double click");
            result.IncCounters(ClickImageCellAndCheck(MouseClickType.DoubleClick, MouseButtons.Left));
            p.log.WriteLine("Right double click");
            result.IncCounters(ClickImageCellAndCheck(MouseClickType.DoubleClick, MouseButtons.Right));
            p.log.WriteLine("Middle double click");
            result.IncCounters(ClickImageCellAndCheck(MouseClickType.DoubleClick, MouseButtons.Middle));
            p.log.WriteLine("Left triple click");
            result.IncCounters(TripleClickImageCellAndCheck(MouseButtons.Left));
            p.log.WriteLine("Right triple click");
            result.IncCounters(TripleClickImageCellAndCheck(MouseButtons.Right));
            p.log.WriteLine("Middle triple click");
            result.IncCounters(TripleClickImageCellAndCheck(MouseButtons.Middle));
            return result;
        }

        //[Scenario("9) Click on link cell")]
        [Scenario(true)]
        [Obsolete]
        public ScenarioResult Scenario9(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            p.log.WriteLine("Left single click");
            result.IncCounters(ClickLinkCellAndCheck(MouseClickType.SingleClick, MouseButtons.Left));
            p.log.WriteLine("Right single click");
            result.IncCounters(ClickLinkCellAndCheck(MouseClickType.SingleClick, MouseButtons.Right));
            p.log.WriteLine("Middle single click");
            result.IncCounters(ClickLinkCellAndCheck(MouseClickType.SingleClick, MouseButtons.Middle));
            p.log.WriteLine("Left double click");
            result.IncCounters(ClickLinkCellAndCheck(MouseClickType.DoubleClick, MouseButtons.Left));
            p.log.WriteLine("Right double click");
            result.IncCounters(ClickLinkCellAndCheck(MouseClickType.DoubleClick, MouseButtons.Right));
            p.log.WriteLine("Middle double click");
            result.IncCounters(ClickLinkCellAndCheck(MouseClickType.DoubleClick, MouseButtons.Middle));
            p.log.WriteLine("Left triple click");
            result.IncCounters(TripleClickLinkCellAndCheck(MouseButtons.Left));
            p.log.WriteLine("Right triple click");
            result.IncCounters(TripleClickLinkCellAndCheck(MouseButtons.Right));
            p.log.WriteLine("Middle triple click");
            result.IncCounters(TripleClickLinkCellAndCheck(MouseButtons.Middle));
            return result;

        }

        //[Scenario("10) Click on combobox")]
        [Scenario(true)]
        [Obsolete]
        public ScenarioResult Scenario10(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            p.log.WriteLine("Left single click");
            result.IncCounters(ClickComboBoxCellAndCheck(MouseClickType.SingleClick, MouseButtons.Left));
            p.log.WriteLine("Middle single click");
            result.IncCounters(ClickComboBoxCellAndCheck(MouseClickType.SingleClick, MouseButtons.Middle));
            p.log.WriteLine("Right single click");
            result.IncCounters(ClickComboBoxCellAndCheck(MouseClickType.SingleClick, MouseButtons.Right));
            p.log.WriteLine("Left double click");
            result.IncCounters(ClickComboBoxCellAndCheck(MouseClickType.DoubleClick, MouseButtons.Left));
            p.log.WriteLine("Middle double click");
            result.IncCounters(ClickComboBoxCellAndCheck(MouseClickType.DoubleClick, MouseButtons.Middle));
            p.log.WriteLine("Right double click");
            result.IncCounters(ClickComboBoxCellAndCheck(MouseClickType.DoubleClick, MouseButtons.Right));
            p.log.WriteLine("Left triple click");
            result.IncCounters(TripleClickComboBoxCellAndCheck(MouseButtons.Left));
            p.log.WriteLine("Middle triple click");
            result.IncCounters(TripleClickComboBoxCellAndCheck(MouseButtons.Middle));
            p.log.WriteLine("Right triple click");
            result.IncCounters(TripleClickComboBoxCellAndCheck(MouseButtons.Right));
            return result;
        }

        //[Scenario("11) Enter editing mode for textbox cell.  Click on cell.  Same events as above")]
        [Scenario(true)]
        [Obsolete]
        public ScenarioResult Scenario11(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            p.log.WriteLine("Left click");
            result.IncCounters(EditCellAndCheck(MouseButtons.Left));
            p.log.WriteLine("Middle click");
            result.IncCounters(EditCellAndCheck(MouseButtons.Middle));
            //p.log.WriteLine("Right click");
            //result.IncCounters(EditCellAndCheck(MouseButtons.Right));
            return result;
        }

        //[Scenario("12) Click-drag to select cells - Verify CellClick, CellMouseClick, and CellContentClick don't fire (VSCurrent 276810")]
        [Scenario(true)]
        [Obsolete]
        public ScenarioResult Scenario12(TParams p)
        {

            return DragSelectAndCheck(MouseButtons.Left);
        }

        #endregion
        #region Event Handlers
        void grid_MouseDown(object sender, MouseEventArgs e)
        {
            AddEvent(events, e, "MouseDown");
        }


        void grid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            AddEvent(events, e, "CellMouseDown");
        }

        void grid_Click(object sender, EventArgs e)
        {
            AddEvent(events, e, "Click");
        }

        void grid_MouseClick(object sender, MouseEventArgs e)
        {
            AddEvent(events, e, "MouseClick");
        }

        void grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            AddEvent(events, e, "CellMouseClick");
        }

        void grid_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            AddEvent(events, e, "CellMouseUp");
        }

        void grid_MouseUp(object sender, MouseEventArgs e)
        {
            AddEvent(events, e, "MouseUp");
        }

        //void grid_GotFocus(object sender, EventArgs e)
        //{
        //    AddEvent(events, e, "GotFocus");
        //}

        void grid_DoubleClick(object sender, EventArgs e)
        {
            AddEvent(events, e, "DoubleClick");
        }

        void grid_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            AddEvent(events, e, "MouseDoubleClick");
        }

        void grid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            AddEvent(events, e, "CellMouseDoubleClick");
        }

        void grid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            AddEvent(events, e, "CellContentClick");
        }

        void grid_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            AddEvent(events, e, "CellContentDoubleClick");
        }

        void grid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            AddEvent(events, e, "CellDoubleClick");
        }

        void grid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            AddEvent(events, e, "ColumnHeaderMouseClick");
        }

        void grid_ColumnHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            AddEvent(events, e, "ColumnHeaderMouseDoubleClick");
        }

        void grid_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            AddEvent(events, e, "RowHeaderMouseClick");
        }
        void grid_RowHeaderMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            AddEvent(events, e, "RowHeaderMouseDoubleClick");
        }
        void grid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            AddEvent(events, e, "CellClick");
        }
        #endregion


    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ 1) Left/right/middle click on cell

//@ 2) Click on top left header cell

//@ 3) Click on column header cell.  Verify ColumnHeaderMouseClick event fires after CellMouseClick, and verify its args

//@ 4) Click on row header cell.  Verify RowHeaderMouseClick event fires after CellMouseClick, and verify its args

//@ 5) Click on empty space in datagridview.  Verify only MouseDown, Click, MouseClick, and MouseUp events fire

//@ 6) Click on scrollbars  Verify no events fire

//@ 7) Click on button cell

//@ 8) Click on image cell

//@ 9) Click on link cell

//@ 10) Click on combobox

//@ 11) Enter editing mode for textbox cell.  Click on cell.  Same events as above

//@ 12) Click-drag to select cells - Verify CellClick, CellMouseClick, and CellContentClick don't fire (VSCurrent 276810

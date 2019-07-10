using System;
using System.Drawing;
using System.Windows.Forms;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using Maui.Core;
using System.Windows.Forms.VisualStyles;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms.IntegrationTests.Common;

//
// Testcase:    DGV_BasicCellFormatting
// Description: 
// Author:      t-timw
//
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDGV_BasicCellFormattingTests : ReflectBase
    {

        #region Testcase setup
        public MauiDGV_BasicCellFormattingTests(string[] args) : base(args)
        {

            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDGV_BasicCellFormattingTests(args));
        }
        DataGridView grid;
        // Expect different numbers of event firings for visual styles enabled:
        // The header cells are formatted, and the non-header cells are formatted more often
        bool visualStylesOn = VisualStyleRenderer.IsSupported;

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            ResetGrid(DataGridViewUtils.GetSimpleDataGridView());
            return base.BeforeScenario(p, scenario);
        }

        void ResetGrid(DataGridView newGrid)
        {
            Controls.Remove(grid);
            grid = newGrid;
            Controls.Add(grid);
            grid.CellFormatting += new DataGridViewCellFormattingEventHandler(grid_CellFormatting);
            events.Clear();

        }
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
            int i = 0;
            foreach (EventInfo ei in eventList)
            {
                scenarioParams.log.WriteLine(i.ToString() + ": " + GetTextForEvent(ei));
                i++;
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
        #endregion
        // Assumes that cell is displayed
        void MoveMouseToCell(int rowIndex, int columnIndex)
        {
            Rectangle cellRectangle = grid.GetCellDisplayRectangle(columnIndex, rowIndex, false);
            Point clickPt = grid.PointToScreen(new Point(cellRectangle.X + cellRectangle.Width / 2, cellRectangle.Y + cellRectangle.Y / 2));

            Mouse.Move(clickPt.X, clickPt.Y);
        }
        //[Scenario(false)]
        [Scenario(true)]
        // Moves from one cell to another
        public void MoveBetweenCells(int rowIndex1, int columnIndex1, int rowIndex2, int columnIndex2)
        {
            MoveMouseToCell(rowIndex1, columnIndex1);

            Application.DoEvents();
            events.Clear();

            MoveMouseToCell(rowIndex2, columnIndex2);
            Application.DoEvents();
        }
        //[Scenario(false)]
        [Scenario(true)]
        public ScenarioResult CheckEventFire(int index, string expectedEventName, DataGridViewCellFormattingEventArgs expectedArgs)
        {
            ScenarioResult result = new ScenarioResult();

            EventInfo ei = events[index];
            DataGridViewCellFormattingEventArgs actualArgs = (DataGridViewCellFormattingEventArgs)ei.EventArgs;
            result.IncCounters(expectedEventName, ei.EventName, "Event name is incorrect.", log);
            result.IncCounters(expectedArgs.CellStyle.Equals(actualArgs.CellStyle), "e.CellStyle is incorrect.", log);
            result.IncCounters(expectedArgs.ColumnIndex, actualArgs.ColumnIndex, "e.ColumnIndex is incorrect.", log);
            result.IncCounters(expectedArgs.RowIndex, actualArgs.RowIndex, "e.RowIndex is incorrect.", log);
            result.IncCounters(expectedArgs.DesiredType, actualArgs.DesiredType, "e.DesiredType is incorrect.", log);
            result.IncCounters(expectedArgs.FormattingApplied, actualArgs.FormattingApplied, "e.FormattingApplied is incorrect.", log);
            return result;
        }
        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("Event doesn't fire when table is empty")]
        [Scenario(true)]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            ResetGrid(new DataGridView());
            events.Clear();
            Point movePt = grid.PointToScreen(new Point(grid.Width / 2, grid.Height / 2));

            Mouse.Move(movePt.X, movePt.Y);
            Application.DoEvents();

            result.IncCounters(0, events.Count, "Event fired when moving mouse over empty grid.", log);
            return result;
        }

        //[Scenario("Row header cell - doesn't fire")]
        [Scenario(true)]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            MoveBetweenCells(0, 0, 0, -1);
            //if (visualStylesOn)
            //{
            //    DataGridViewCell cell = grid.Rows[0].Cells[0]; // cell that the event fired for
            //    DataGridViewCellFormattingEventArgs expectedArgs = new DataGridViewCellFormattingEventArgs(0,0, cell.Value, cell.Value.GetType(), cell.GetInheritedStyle(null, 0, true));
            //    result.IncCounters(1, events.Count, "Event fired an unexpected number of times.", log);
            //    result.IncCounters(CheckEventFire(0, "CellFormatting", expectedArgs));
            //}
            //else
            //{
            result.IncCounters(0, events.Count, "Event fired an unexpected number of times.", log);
            //}
            return result;
        }

        //[Scenario("Column header cell - doesn't fire")]
        [Scenario(true)]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            MoveBetweenCells(0, 0, -1, 0);
            //if (visualStylesOn)
            //{
            //    DataGridViewCell cell = grid.Rows[0].Cells[0]; // cell that the event fired for
            //    DataGridViewCellFormattingEventArgs expectedArgs = new DataGridViewCellFormattingEventArgs(0, 0, cell.Value, cell.Value.GetType(), cell.GetInheritedStyle(null, 0, true));
            //    result.IncCounters(1, events.Count, "Event fired an unexpected number of times.", log);
            //    result.IncCounters(CheckEventFire(0, "CellFormatting", expectedArgs));
            //}
            //else
            //{
            result.IncCounters(0, events.Count, "Event fired an unexpected number of times.", log);
            //}
            return result;
        }

        //[Scenario("Top left header cell - doesn't fire")]
        [Scenario(true)]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            MoveBetweenCells(0, 0, -1, -1);
            //if (visualStylesOn)
            //{
            //    DataGridViewCell cell = grid.Rows[0].Cells[0]; // cell that the event fired for
            //    DataGridViewCellFormattingEventArgs expectedArgs = new DataGridViewCellFormattingEventArgs(0, 0, cell.Value, cell.Value.GetType(), cell.GetInheritedStyle(null, 0, true));
            //    result.IncCounters(1, events.Count, "Event fired an unexpected number of times.", log);
            //    result.IncCounters(CheckEventFire(0, "CellFormatting", expectedArgs));
            //}
            //else
            //{
            result.IncCounters(0, events.Count, "Event fired an unexpected number of times.", log);
            //}
            return result;
        }

        //[Scenario("Verify event fires for cells that are not headers")]
        [Scenario(true)]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            // Move from column header to cell right below 
            MoveBetweenCells(-1, 0, 0, 0);
            if (visualStylesOn)
            {
                //result.IncCounters(2, events.Count, "Event fired an unexpected number of times.", log);
                result.IncCounters(events.Count > 0 ? true : false, "Event did not fire.", log);

                // construct first cell's expected event args
                DataGridViewCell cell = grid.Rows[0].Cells[0]; // cell that the event fired for
                DataGridViewCellFormattingEventArgs expectedArgs = new DataGridViewCellFormattingEventArgs(0, 0, cell.Value, cell.Value.GetType(), cell.GetInheritedStyle(null, 0, true));
                result.IncCounters(CheckEventFire(0, "CellFormatting", expectedArgs));
                result.IncCounters(CheckEventFire(1, "CellFormatting", expectedArgs));

            }
            else
            {
                //result.IncCounters(1, events.Count, "Event fired an unexpected number of times.", log);
                result.IncCounters(events.Count > 0 ? true : false, "Event did not fire.", log);
                DataGridViewCell cell = grid.Rows[0].Cells[0];
                DataGridViewCellFormattingEventArgs expectedArgs = new DataGridViewCellFormattingEventArgs(0, 0, cell.Value, cell.Value.GetType(), cell.GetInheritedStyle(null, 0, true));
                result.IncCounters(CheckEventFire(0, "CellFormatting", expectedArgs));
            }


            // Move from (0,0) to (0,1)
            MoveBetweenCells(0, 0, 0, 1);
            if (visualStylesOn)
            {
                //result.IncCounters(3, events.Count, "Event fired an unexpected number of times.", log);
                result.IncCounters(events.Count > 0 ? true : false, "Event did not fire.", log);
                // construct first cell's (0,1) expected event args
                DataGridViewCell cell = grid.Rows[0].Cells[1]; // cell that the event fired for
                DataGridViewCellFormattingEventArgs expectedArgs = new DataGridViewCellFormattingEventArgs(1, 0, cell.Value, cell.Value.GetType(), cell.GetInheritedStyle(null, 0, true));
                result.IncCounters(CheckEventFire(0, "CellFormatting", expectedArgs));

                //// Construct second cell's (0,0) expected event args
                //DataGridViewCell cell2 = grid.Rows[0].Cells[0]; // cell that the event fired for
                //DataGridViewCellFormattingEventArgs expectedArgs2 = new DataGridViewCellFormattingEventArgs(0, 0, cell2.Value, cell2.Value.GetType(), cell2.GetInheritedStyle(null, 0, true));
                //result.IncCounters(CheckEventFire(1, "CellFormatting", expectedArgs2));

                //// Should fire once more for the first cell (0,1)
                //result.IncCounters(CheckEventFire(2, "CellFormatting", expectedArgs));
            }
            else
            {
                //result.IncCounters(1, events.Count, "Event fired an unexpected number of times.", log);
                result.IncCounters(events.Count > 0 ? true : false, "Event did not fire.", log);
                DataGridViewCell cell = grid.Rows[0].Cells[1]; // cell that the event fired for
                DataGridViewCellFormattingEventArgs expectedArgs = new DataGridViewCellFormattingEventArgs(1, 0, cell.Value, cell.Value.GetType(), cell.GetInheritedStyle(null, 0, true));

                result.IncCounters(CheckEventFire(0, "CellFormatting", expectedArgs));
            }
            return result;
        }

        #endregion

        void grid_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            AddEvent(events, e, "CellFormatting");
        }
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Event doesn't fire when table is empty

//@ Row header cell - doesn't fire

//@ Column header cell - doesn't fire

//@ Top left header cell - doesn't fire

//@ Verify event fires for cells that are not headers

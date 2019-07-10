using System;
using System.Drawing;
using System.Windows.Forms;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Collections;
using System.Threading;
using System.Windows.Forms.IntegrationTests.Common;

//
// Testcase:    DGV_BasicCellPainting
// Description: 
// Author:      t-timw
//

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDGV_BasicCellPaintingTests : ReflectBase
    {

        #region Testcase setup
        public MauiDGV_BasicCellPaintingTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDGV_BasicCellPaintingTests(args));
        }
        DataGridView grid;

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);

        }
        public void ResetGrid(DataGridView dgv)
        {
            Controls.Remove(grid);
            grid = dgv;
            grid.CellPainting += new DataGridViewCellPaintingEventHandler(grid_CellPainting);
            Controls.Add(grid);
            grid.Show();
            grid.Focus();
            Application.DoEvents();
            events.Clear();

        }

        protected override bool BeforeScenario(TParams p, MethodInfo scenario)
        {
            ResetGrid(DataGridViewUtils.GetSimpleDataGridView());

            return base.BeforeScenario(p, scenario);
        }

        #endregion
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
        //[Scenario(false)]
        [Scenario(true)]
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
        //==========================================
        // Helpers
        //==========================================
        // Helper to return an arraylist of all displayed (non-header) cells in the grid 
        // Assumes that the display indices of the rows and columns are in the
        // same relative order as the row and column indices
        public ArrayList GetAllDisplayedCells(DataGridView dgv)
        {
            ArrayList cells = new ArrayList();

            int firstDisplayedColumnIndex = grid.FirstDisplayedCell.ColumnIndex;
            int lastDisplayedColumnIndex = firstDisplayedColumnIndex + grid.DisplayedColumnCount(true) - 1;
            int firstDisplayedRowIndex = grid.FirstDisplayedCell.RowIndex;
            int lastDisplayedRowIndex = grid.FirstDisplayedCell.RowIndex + grid.DisplayedRowCount(true) - 1;

            for (int row = firstDisplayedRowIndex; row <= lastDisplayedRowIndex; row++)
            {
                for (int col = firstDisplayedColumnIndex; col <= lastDisplayedColumnIndex; col++)
                {
                    cells.Add(grid.Rows[row].Cells[col]);
                }
            }

            return cells;
        }
        //[Scenario(false)]
        [Scenario(true)]
        // Checks that a CellPainting event fired at least once for each of the cells in the expectedCells 
        public ScenarioResult CheckCellEventsFired(ArrayList expectedCells)
        {
            ScenarioResult result = new ScenarioResult();
            ArrayList cells = new ArrayList(expectedCells); // Create a copy so we don't have to modify the expectedCells array

            foreach (EventInfo ei in events)
            {
                DataGridViewCellPaintingEventArgs ea = (DataGridViewCellPaintingEventArgs)ei.EventArgs;
                if (ea.RowIndex != -1 && ea.ColumnIndex != -1)
                {
                    DataGridViewCell firedCell = grid.Rows[ea.RowIndex].Cells[ea.ColumnIndex];
                    // Check that we expected the event to fire for this cell
                    result.IncCounters(expectedCells.Contains(firedCell), "CellPainting fired for an unexpected cell [" + firedCell.RowIndex + ", " + firedCell.ColumnIndex + "]", log);
                    cells.Remove(firedCell); // Remove each cell that CellPainting has fired for
                }
            }
            result.IncCounters(0, cells.Count, "Event fired more than once for a cell", log);

            return result;
        }
        //[Scenario(false)]
        [Scenario(true)]
        // Checks that a CellPainting event fired at least once for each of the displayed header cells
        public ScenarioResult CheckHeaderCellEventsFired()
        {
            ScenarioResult result = new ScenarioResult();


            int firstDisplayedColumnIndex = grid.FirstDisplayedCell.ColumnIndex;
            int lastDisplayedColumnIndex = firstDisplayedColumnIndex + grid.DisplayedColumnCount(true) - 1;
            int firstDisplayedRowIndex = grid.FirstDisplayedCell.RowIndex;
            int lastDisplayedRowIndex = grid.FirstDisplayedCell.RowIndex + grid.DisplayedRowCount(true) - 1;

            for (int row = firstDisplayedRowIndex; row <= lastDisplayedRowIndex; row++)
            {
                for (int col = firstDisplayedColumnIndex; col <= lastDisplayedColumnIndex; col++)
                {
                    result.IncCounters(CellFired(row, col), "Event did not fire for cell [" + row.ToString() + ", " + col.ToString() + "]", log);
                }
            }

            return result;
        }
        // Returns true if the event fired at least once for the given cell coordinates
        public bool CellFired(int rowIndex, int columnIndex)
        {
            foreach (EventInfo ei in events)
            {
                DataGridViewCellPaintingEventArgs ea = (DataGridViewCellPaintingEventArgs)ei.EventArgs;
                if (ea.RowIndex == rowIndex && ea.ColumnIndex == columnIndex)
                {
                    return true;
                }
            }

            return false;
        }
        // returns the number of header cells displayed in this grid
        public int NumDisplayedHeaderCells()
        {
            return grid.DisplayedColumnCount(true) + grid.DisplayedRowCount(true) + 1;
        }
        // Gets a random displayed cell
        public DataGridViewCell GetRandomDisplayedCell(TParams p)
        {
            int firstDisplayedColumnIndex = grid.FirstDisplayedCell.ColumnIndex;
            int lastDisplayedColumnIndex = firstDisplayedColumnIndex + grid.DisplayedColumnCount(true) - 1;
            int firstDisplayedRowIndex = grid.FirstDisplayedCell.RowIndex;
            int lastDisplayedRowIndex = grid.FirstDisplayedCell.RowIndex + grid.DisplayedRowCount(true) - 1;

            int rowIndex = p.ru.GetRange(firstDisplayedRowIndex, lastDisplayedRowIndex);
            int columnIndex = p.ru.GetRange(firstDisplayedColumnIndex, lastDisplayedColumnIndex);

            return grid.Rows[rowIndex].Cells[columnIndex];
        }
        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("Verify fires for top left header cell when displayed")]
        [Scenario(true)]
        public ScenarioResult InvalidateGridAndCheck(TParams p)
        {
            ScenarioResult result = new ScenarioResult();
            events.Clear();
            grid.Invalidate();
            Application.DoEvents();
            ArrayList expectedCells = GetAllDisplayedCells(grid);
            result.IncCounters(expectedCells.Count + NumDisplayedHeaderCells(), events.Count, "Event fired an unexpected number of times.", log);
            result.IncCounters(CheckHeaderCellEventsFired());
            result.IncCounters(CheckCellEventsFired(expectedCells));
            return result;
        }

        //[Scenario("Verify fires for row header cells when displayed")]
        [Scenario(true)]
        public ScenarioResult Scenario2(TParams p)
        {
            return InvalidateGridAndCheck(p);
        }

        //[Scenario("Verify fires for column header cells when displayed")]
        [Scenario(true)]
        public ScenarioResult Scenario3(TParams p)
        {
            return InvalidateGridAndCheck(p);

        }

        //[Scenario("Verify fires for cell when displayed")]
        [Scenario(true)]
        public ScenarioResult CheckCellPaint(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            // Check for all cells inside covered in previous scenario
            result.IncCounters(InvalidateGridAndCheck(p));

            events.Clear();
            // Try repainting cell
            DataGridViewCell cell = GetRandomDisplayedCell(p);
            cell.Selected ^= true;
            Application.DoEvents();
            result.IncCounters(CellFired(cell.RowIndex, cell.ColumnIndex), "Event did not fire for cell [" + cell.RowIndex.ToString() + ", " + cell.ColumnIndex.ToString() + "] when toggling selected.", log);
            result.IncCounters(1, events.Count, "An unexpected number of events fired.", log);
            return result;

        }

        //[Scenario("Add row.  Verify fires for newly displayed cells (and row header)")]
        [Scenario(true)]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            // Use smaller grid so we can see added row
            DataGridView newGrid = DataGridViewUtils.GetSimpleDataGridView();
            newGrid.RowCount = newGrid.ColumnCount = 2;
            ResetGrid(newGrid);

            // Add cells programmatically
            grid.RowCount++;
            Application.DoEvents();

            ArrayList expectedCells = GetAllDisplayedCells(grid);
            // All cells get repainted if new row is added
            result.IncCounters(expectedCells.Count + NumDisplayedHeaderCells(), events.Count, "Event fired an unexpected number of times.", log);
            result.IncCounters(CheckHeaderCellEventsFired());
            result.IncCounters(CheckCellEventsFired(expectedCells));

            return result;
        }

        //[Scenario("Edit cell data.  Verify fires.")]
        [Scenario(true)]
        public ScenarioResult Scenario6(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            grid.CurrentCell = GetRandomDisplayedCell(p);
            Application.DoEvents();
            events.Clear();

            // Verify event doesn't fire when entering edit mode
            grid.BeginEdit(true);
            Application.DoEvents();
            result.IncCounters(1, events.Count, "Event did not fire when entering edit mode.", log);
            // Verify event fires after text is changed
            SendKeys.SendWait("s");
            Application.DoEvents();
            result.IncCounters(2, events.Count, "Event didn't fire when first edit is made.", log);
            result.IncCounters(CellFired(grid.CurrentCell.RowIndex, -1), "Event did not fire for the correct cell.", log);
            events.Clear();
            // Commit the edit (verify event fires)
            grid.CommitEdit(new DataGridViewDataErrorContexts());
            Application.DoEvents();
            result.IncCounters((events.Count == 2 || events.Count == 3), "An unexpected number of events fired.", log);
            result.IncCounters(CellFired(grid.CurrentCell.RowIndex, grid.CurrentCell.ColumnIndex), "Event did not fire for the cell [" + grid.CurrentCell.RowIndex.ToString() + ", " + grid.CurrentCell.ColumnIndex.ToString() + "]", log);
            result.IncCounters(CellFired(grid.CurrentCell.RowIndex, -1), "Event did not fire for the cell's row header", log);

            return result;
        }

        //[Scenario("Change style for cell. Verify fires.")]
        [Scenario(true)]
        public ScenarioResult Scenario7(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            DataGridViewCell cell = GetRandomDisplayedCell(p);
            DataGridViewCellStyle newStyle;
            do
            {
                newStyle = DataGridViewUtils.GetRandomCellStyle(p);
            } while (DataGridViewUtils.CompareCellStyles(newStyle, cell.Style));

            cell.Style = newStyle;
            Application.DoEvents();
            result.IncCounters(1, events.Count, "Event fired an unexpected number of times while toggling cell.Selected.", log);
            result.IncCounters(CellFired(cell.RowIndex, cell.ColumnIndex), "Event did not fire for the cell [" + grid.CurrentCell.RowIndex.ToString() + ", " + grid.CurrentCell.ColumnIndex.ToString() + "]", log);

            return result;
        }

        //[Scenario("Verify fires for selected cell.")]
        [Scenario(true)]
        public ScenarioResult Scenario8(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            // Toggle Select twice and verify event fires once for each toggle
            DataGridViewCell cell = GetRandomDisplayedCell(p);
            cell.Selected ^= true;
            Application.DoEvents();
            result.IncCounters(1, events.Count, "Event fired an unexpected number of times while toggling cell.Selected.", log);
            result.IncCounters(CellFired(cell.RowIndex, cell.ColumnIndex), "Event did not fire for the cell [" + grid.CurrentCell.RowIndex.ToString() + ", " + grid.CurrentCell.ColumnIndex.ToString() + "]", log);

            events.Clear();
            cell.Selected ^= true;
            Application.DoEvents();
            result.IncCounters(1, events.Count, "Event fired an unexpected number of times while toggling cell.Selected.", log);
            result.IncCounters(CellFired(cell.RowIndex, cell.ColumnIndex), "Event did not fire for the cell [" + grid.CurrentCell.RowIndex.ToString() + ", " + grid.CurrentCell.ColumnIndex.ToString() + "]", log);

            return result;
        }

        #endregion

        void grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // Have to copy the event args each time because the DataGridView reuses the object every time
            // cellpainting is raised.  
            AddEvent(events, new DataGridViewCellPaintingEventArgs(grid, e.Graphics, e.ClipBounds, e.CellBounds, e.RowIndex, e.ColumnIndex, e.State, e.Value, e.FormattedValue, e.ErrorText, e.CellStyle, e.AdvancedBorderStyle, e.PaintParts), "CellPainting");
        }
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify fires for top left header cell when displayed

//@ Verify fires for row header cells when displayed

//@ Verify fires for column header cells when displayed

//@ Verify fires for cell when displayed

//@ Add row.  Verify fires for newly displayed cells (and row header)

//@ Edit cell data.  Verify fires.

//@ Change style for cell. Verify fires.

//@ Verify fires for selected cell.
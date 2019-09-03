// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using Maui.Core;
using System.Windows.Forms.IntegrationTests.Common;
using System.Threading;

//
// Testcase:    AutoFill_Basic Behavior
// Description: Test basic AutoFill behavior.
// Author:      ariroth
//

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiAutoFill_BasicBehaviorTests : ReflectBase
    {

        #region Testcase setup
        public MauiAutoFill_BasicBehaviorTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiAutoFill_BasicBehaviorTests(args));
        }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            grid = new DataGridView();
            this.Controls.Add(grid);
            this.Text = p.ru.GetString(5);  // For the MAUI mouse stuff
        }

        private float CalculateTolerance(float val)
        {
            // Because of rounding issues, the amount that the column widths can be off by varies.  So, we compensate.

            if (val < TTINY_MAX)
                return val * TOLERANCE_TINY;

            if (val < TSMALL_MAX)
                return val * TOLERANCE_SMALL;

            if (val < TMEDIUM_MAX)
                return val * TOLERANCE_MEDIUM;

            return val * TOLERANCE_LARGE;
        }

        [Scenario(false)]
        private ScenarioResult DoWidthWeightScenarios(bool doWeight, TParams p)
        {
            grid.Columns.Clear();
            int colCount = p.ru.GetRange(3, 10);
            for (int i = 0; i < colCount; i++)
            {
                DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns.Add(c);
            }

            if (doWeight)
                // Change the weight -- we'll use column 1
                grid.Columns[1].FillWeight *= 2;
            else
                // Change the width -- we'll use column 1
                grid.Columns[1].Width *= 2;

            // Get the weights of the columns
            float totalWeights = 0.0F;
            for (int i = 0; i < colCount; i++)
                totalWeights += grid.Columns[i].FillWeight;

            ScenarioResult ret = new ScenarioResult();

            if (doWeight)
            {
                // Now calculate the expected widths of each column and make sure that it's accurate
                for (int i = 0; i < colCount; i++)
                {
                    DataGridViewColumn c = grid.Columns[i];
                    int expected = (int)((c.FillWeight / totalWeights) * grid.Width);
                    ret.IncCounters(Math.Abs(expected - c.Width) <= CalculateTolerance(c.Width) || (expected < MINIMUM_COLUMN_WIDTH && c.Width == MINIMUM_COLUMN_WIDTH),
                        "Column was the wrong width!", expected, c.Width, p.log);
                }
            }
            else
            {
                // Now calculate the expected weights of each column and make sure that it's accurate
                for (int i = 0; i < colCount; i++)
                {
                    DataGridViewColumn c = grid.Columns[i];
                    float expected = (c.Width * totalWeights) / grid.Width;
                    ret.IncCounters(Math.Abs(expected - c.FillWeight) <= CalculateTolerance(c.FillWeight), "Column has the wrong weight!", expected, c.FillWeight, p.log);
                }
            }

            return ret;
        }

        [Scenario(false)]
        private ScenarioResult DoAddRemoveScenarios(bool doAdd, TParams p)
        {
            grid.Columns.Clear();
            float totalFillWeights = 0.0F;
            int colCount = p.ru.GetRange(2, 10);
            for (int i = 0; i < colCount; i++)
            {
                DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns.Add(c);
                totalFillWeights += c.FillWeight;
            }

            if (doAdd)
            {
                // Add one specific column so that we can check
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns.Add(col);
                totalFillWeights += col.FillWeight;
            }
            else
            {
                // Remove the column
                int removeIndex = p.ru.GetRange(0, colCount - 1);
                totalFillWeights -= grid.Columns[removeIndex].FillWeight;
                grid.Columns.Remove(grid.Columns[removeIndex]);
            }

            int[] widthArray = new int[grid.ColumnCount];
            // Calculate the expected widths of each column
            for (int i = 0; i < grid.ColumnCount; i++)
                widthArray[i] = (int)((grid.Columns[i].FillWeight / totalFillWeights) * grid.Width);

            ScenarioResult ret = new ScenarioResult();
            for (int i = 0; i < widthArray.Length; i++)
                ret.IncCounters(Math.Abs(grid.Columns[i].Width - widthArray[i]) <= CalculateTolerance((float)widthArray[i]) || (widthArray[i] < 5 && grid.Columns[i].Width == 5), "Column width is incorrect!", p.log);

            return ret;
        }

        private DataGridView grid;
        private const int MINIMUM_COLUMN_WIDTH = 5;
        private const int DEFAULT_FILL_WEIGHT = 100;

        /* 
         * !!Important!!
         * 
         * These constants represent a grid of cutoff points beyond which
         * a column width has been incorrectly calculated.  In other words,
         * they are here to determine what is a rounding discrepancy and what
         * is a miscalculation.  The grid works out as follows:
         * 
         * Category			Maximum Column Width		Max % Difference Between Expected and Actual
         * --------			--------------------		--------------------------------------------
         * Tiny				10  pixels					20%
         * Small			20  pixels					10%
         * Medium			200 pixels					 6%
         * Large			Column.Width.MaxValue		 3%
         *
         */
        private const int TTINY_MAX = 10;
        private const int TSMALL_MAX = 20;
        private const int TMEDIUM_MAX = 200;
        private const float TOLERANCE_TINY = 0.20F;
        private const float TOLERANCE_SMALL = 0.10F;
        private const float TOLERANCE_MEDIUM = 0.06F;
        private const float TOLERANCE_LARGE = 0.03F;

        private const int OFFSET = 2;
        private const int COLUMN_COUNT = 10;
        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("Verify order of priority: AutoSize (not Fill), Fixed Size, Fill")]
        [Scenario(true)]
        public ScenarioResult OrderOfPriority(TParams p)
        {

            // The idea here is to make sure that the Fill column shrinks if we make
            // either of the other two columns larger, and that the Fixed column moves
            // over if we make the AutoSize column larger.

            DataGridViewTextBoxColumn autoCol = new DataGridViewTextBoxColumn();
            autoCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;

            DataGridViewTextBoxColumn fixedCol = new DataGridViewTextBoxColumn();
            fixedCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;

            DataGridViewTextBoxColumn fillCol = new DataGridViewTextBoxColumn();
            fillCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            grid.Columns.Add(autoCol);
            grid.Columns.Add(fixedCol);
            grid.Columns.Add(fillCol);

            // Get the initial widths
            int autoWidth = autoCol.Width;
            int fixedWidth = fixedCol.Width;
            int fillWidth = fillCol.Width;

            ScenarioResult ret = new ScenarioResult();

            // 1.) Make the auto column larger; verify Fixed doesn't change size but Fill does
            p.log.WriteLine("Resizing the AutoResize column...");
            grid[0, 0].Value = p.ru.GetString(100, 100);
            grid.AutoResizeColumn(0);   // Make sure it's autosized correctly
            ret.IncCounters(autoCol.Width > autoWidth, "AutoSize column did not get resized!", p.log);
            ret.IncCounters(fixedCol.Width == fixedWidth, "Fixed-width column got resized!", p.log);
            ret.IncCounters(fillCol.Width < fillWidth, "Fill column did not get resized!", p.log);

            // Reset the columns by shrinking the AutoFill column and adjusting the width
            // variables on the others
            grid[0, 0].Value = p.ru.GetString(5);   // Shrink the column
            fillWidth = fillCol.Width;
            autoWidth = autoCol.Width;
            fixedWidth = fixedCol.Width;

            // 2.) Make the fixed column larger; verify Auto doesn't change size but Fill does
            p.log.WriteLine("Resizing the Fixed column...");
            fixedCol.Width = p.ru.GetRange(500, 1000);
            ret.IncCounters(autoCol.Width == autoWidth, "AutoSize column got resized!", p.log);
            ret.IncCounters(fixedCol.Width > fixedWidth, "Fixed-width column did not get resized!", p.log);
            ret.IncCounters(fillCol.Width < fillWidth, "Fill column did not get resized!", p.log);

            // Reset the columns
            fixedCol.Width = fixedWidth;
            ret.IncCounters(autoCol.Width == autoWidth, "AutoSize column was not reset!", p.log);
            ret.IncCounters(fixedCol.Width == fixedWidth, "Fixed-width column was not reset!", p.log);
            ret.IncCounters(fillCol.Width == fillWidth, "Fill column was not reset!", p.log);

            // 3.) Make the fill column larger; verify the other two don't change size
            p.log.WriteLine("Resizing the Fill column...");
            // To make the Fill column bigger, you make the grid bigger
            grid.Width += p.ru.GetRange(50, 100);
            ret.IncCounters(autoCol.Width == autoWidth, "AutoSize column got resized!", p.log);
            ret.IncCounters(fixedCol.Width == fixedWidth, "Fixed-width column got resized!", p.log);
            ret.IncCounters(fillCol.Width > fillWidth, "Fill column did not get resized!", p.log);

            return ret;
        }

        //[Scenario("Verify column sizes are appropriately calculated: [(fill value of column) / (total fill values)] * (DGV.Width).  Use multiple columns with random fill values")]
        [Scenario(true)]
        public ScenarioResult VerifySizes(TParams p)
        {
            grid.Columns.Clear();
            grid.RowHeadersVisible = false;     // Turn this off for simplicity
            float totalFillWeights = 0.0F;
            for (int i = 0; i < COLUMN_COUNT; i++)
            {
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                col.FillWeight = p.ru.GetFloat(1, 6000);
                totalFillWeights += col.FillWeight;
                grid.Columns.Add(col);
            }

            ScenarioResult ret = new ScenarioResult();
            for (int i = 0; i < 5; i++)
            {
                DataGridViewColumn col = grid.Columns[i];
                int expectedWidth = (int)(((col.FillWeight) / totalFillWeights) * grid.Width);
                ret.IncCounters(Math.Abs(col.Width - expectedWidth) <= CalculateTolerance(expectedWidth) || (expectedWidth < 5 && col.Width == 5), "Column's width is not in keeping with the expected width!", expectedWidth, col.Width, p.log);
            }

            return ret;
        }

        //[Scenario("Verify total column widths == DGV.Width when (total columns) * (Minimum width) < available space")]
        [Scenario(true)]
        public ScenarioResult VerifyTotals(TParams p)
        {
            grid.Columns.Clear();
            grid.RowHeadersVisible = false;     // For simplicity
            int maxColumns = grid.Width / MINIMUM_COLUMN_WIDTH;
            int numColumns = COLUMN_COUNT;

            int totalWidths = 0;
            for (int i = 0; i < numColumns; i++)
            {
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns.Add(col);
            }

            // Get the sum of the widths
            for (int i = 0; i < numColumns; i++)
                totalWidths += grid.Columns[i].Width;

            // See VSCurrent 432611 for why we're checking to within 3 pixels.
            // Short answer: Won't Fix -- doesn't make the RTM Ship Bar
            return new ScenarioResult(Math.Abs(totalWidths - grid.Width) <= 3, "Total column widths do not equal grid's width within 3 pixels!", p.log);

        }

        [Scenario(true)]
        //[Scenario("Change FillWeights on an invisible DGV, then set Visible == true.  Verify changes have taken affect.")]
        public ScenarioResult ChangeWeights(TParams p)
        {
            grid.Columns.Clear();
            for (int i = 0; i < p.ru.GetRange(2, (grid.Width / MINIMUM_COLUMN_WIDTH) / 2); i++)
            {
                DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns.Add(c);
            }

            grid.Visible = false;

            int colChanged = p.ru.GetRange(0, grid.ColumnCount - 1);
            grid.Columns[colChanged].FillWeight *= 2;

            float totalWeights = 0.0F;
            for (int i = 0; i < grid.ColumnCount; i++)
                totalWeights += grid.Columns[i].FillWeight;

            // Calculate the expected widths for the changed column and the unchanged columns (changed == FillWeight changed)
            int expectedUnchanged = (int)((DEFAULT_FILL_WEIGHT / totalWeights) * grid.Width);
            int expectedChanged = (int)(((DEFAULT_FILL_WEIGHT * 2) / totalWeights) * grid.Width);

            grid.Visible = true;

            ScenarioResult ret = new ScenarioResult();
            for (int i = 0; i < grid.ColumnCount; i++)
            {
                if (i == colChanged)
                    ret.IncCounters(Math.Abs(grid.Columns[i].Width - expectedChanged) <= CalculateTolerance((float)expectedChanged) || (expectedUnchanged < 5 && grid.Columns[i].Width == 5),
                        "Changes did not take affect when grid was made visible!", p.log);
                else
                    ret.IncCounters(Math.Abs(grid.Columns[i].Width - expectedUnchanged) <= CalculateTolerance((float)expectedUnchanged) || (expectedUnchanged < 5 && grid.Columns[i].Width == 5),
                        "Changes did not take affect when grid was made visible!", p.log);
            }

            return ret;
        }

        [Scenario(true)]
        //[Scenario("Add column at runtime.  Verify widths change accordingly")]
        public ScenarioResult AddCol(TParams p)
        {
            return DoAddRemoveScenarios(true, p);
        }

        [Scenario(true)]
       // [Scenario("Reorder columns programmatically.  Verify no issues.")]
        public ScenarioResult ReorderProgrammatically(TParams p)
        {
            grid.Columns.Clear();
            int colCount = COLUMN_COUNT;
            float totalWeights = 0.0F;
            for (int i = 0; i < colCount; i++)
            {
                DataGridViewTextBoxColumn col = new DataGridViewTextBoxColumn();
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                col.FillWeight = p.ru.GetFloat(1, 6000);
                grid.Columns.Add(col);
                totalWeights += col.FillWeight;
            }

            int[] beforeWidths = new int[colCount];
            for (int i = 0; i < colCount; i++)
                beforeWidths[i] = grid.Columns[i].Width;

            DataGridViewUtils.ReorderColumn(grid, p.ru.GetRange(0, grid.ColumnCount - 1));

            ScenarioResult ret = new ScenarioResult();
            // Since the DisplayIndex of the column is changing only, the widths should remain the same
            for (int i = 0; i < colCount; i++)
                ret.IncCounters(beforeWidths[i] == grid.Columns[i].Width, "Column width changed on reorder!", p.log);

            return ret;
        }

        [Scenario(true)]
        //[Scenario("Change a Fill column's width programmatically.  Verify weight changes accordingly and that the DGV draws the columns correctly.")]
        public ScenarioResult ChangeFill(TParams p)
        {
            return DoWidthWeightScenarios(false, p);
        }

        [Scenario(true)]
       //[Scenario("Change a Fill column's weight programmatically.  Verify width changes accordingly and that the DGV draws the columns correctly.")]
        public ScenarioResult ChangeWeight(TParams p)
        {
            return DoWidthWeightScenarios(true, p);
        }

        [Scenario(true)]
        //[Scenario("Switch between Fill and non-Fill at both the column and grid level.  Verify no issues.")]
        public ScenarioResult SwitchFill(TParams p)
        {
            grid.Columns.Clear();
            int colCount = COLUMN_COUNT;
            grid.ColumnCount = colCount;

            ScenarioResult ret = new ScenarioResult();

            // Grid Level
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            for (int i = 0; i < colCount; i++)
                ret.IncCounters(grid.Columns[i].InheritedAutoSizeMode == DataGridViewAutoSizeColumnMode.Fill, "Column has wrong AutoSize mode!", p.log);

            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            for (int i = 0; i < colCount; i++)
                ret.IncCounters(grid.Columns[i].InheritedAutoSizeMode == DataGridViewAutoSizeColumnMode.None, "Column has wrong AutoSize mode!", p.log);

            // Column level
            DataGridViewColumn col = grid.Columns[p.ru.GetRange(0, colCount - 1)];
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            // If there are no issues, we passed
            ret.IncCounters(ScenarioResult.Pass);
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            // If there are no issues, we passed
            ret.IncCounters(ScenarioResult.Pass);

            return ret;
        }

        //Scenario("Verify inheritance: Column overrides grid")]
        [Scenario(true)]
        public ScenarioResult VerifyInheritance(TParams p)
        {
            grid.Columns.Clear();
            int colCount = COLUMN_COUNT;
            grid.ColumnCount = colCount;

            ScenarioResult ret = new ScenarioResult();

            // Grid Level
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            for (int i = 0; i < colCount; i++)
            {
                ret.IncCounters(grid.Columns[i].InheritedAutoSizeMode == DataGridViewAutoSizeColumnMode.Fill, "Column has wrong AutoSize mode!", p.log);
            }

            // Column level
            DataGridViewColumn col = grid.Columns[p.ru.GetRange(0, colCount - 1)];
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            ret.IncCounters(col.AutoSizeMode == DataGridViewAutoSizeColumnMode.None, "Grid is overriding column!", p.log);

            return ret;
        }

        [Scenario(true)]
       // [Scenario("Call AutoSize() on a filled column.  Verify exception")]
        public ScenarioResult CallAutoSize(TParams p)
        {
            grid.Columns.Clear();
            int colCount = COLUMN_COUNT;
            for (int i = 0; i < colCount; i++)
            {
                DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns.Add(c);
            }

            grid.AutoResizeColumn(p.ru.GetRange(0, colCount - 1));
            return ScenarioResult.Pass;
        }

        //[Scenario("Remove a column.  Verify column widths adjust accordingly in runtime.")]
        [Scenario(true)]
        public ScenarioResult RemoveCol(TParams p)
        {
            return DoAddRemoveScenarios(false, p);
        }

        [Scenario(true)]
       //[Scenario("Toggle visibility.  Verify weights do not change but widths change accordingly for all visible columns.")]
        public ScenarioResult ToggleVisibility(TParams p)
        {
            grid.Columns.Clear();
            float totalWeightAllCells = 0.0F;
            int colCount = COLUMN_COUNT;
            for (int i = 0; i < colCount; i++)
            {
                DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns.Add(c);
                totalWeightAllCells += c.FillWeight;
            }

            // Get the widths of all of the column assuming nothing is invisible (for use later)
            int[] allCellWidths = new int[colCount];
            for (int i = 0; i < colCount; i++)
                allCellWidths[i] = grid.Columns[i].Width;

            // Make a column invisible
            int toggleColumn = p.ru.GetRange(0, colCount - 1);
            grid.Columns[toggleColumn].Visible = false;

            // Now get the fillweight of all of the visible columns
            float totalWeightVisible = totalWeightAllCells - grid.Columns[toggleColumn].FillWeight;

            ScenarioResult ret = new ScenarioResult();

            // Figure out if the visible columns are all the appropriate size
            for (int i = 0; i < colCount; i++)
            {
                DataGridViewColumn c = grid.Columns[i];
                if (c.Visible)
                {
                    int expected = (int)((c.FillWeight / totalWeightVisible) * grid.Width);
                    ret.IncCounters(Math.Abs(c.Width - expected) <= CalculateTolerance((float)expected) || (expected < MINIMUM_COLUMN_WIDTH && c.Width == MINIMUM_COLUMN_WIDTH),
                        "Column is the wrong size!", p.log);
                }
            }

            // Make the column visible again
            grid.Columns[toggleColumn].Visible = true;

            // Figure out if the columns reverted to their normal size
            for (int i = 0; i < grid.ColumnCount; i++)
            {
                DataGridViewColumn c = grid.Columns[i];
                ret.IncCounters(Math.Abs(c.Width - allCellWidths[i]) <= CalculateTolerance((float)allCellWidths[i]) || (allCellWidths[i] < MINIMUM_COLUMN_WIDTH && c.Width == MINIMUM_COLUMN_WIDTH),
                    "Column is the wrong size!", p.log);
            }

            return ret;
        }

        [Scenario(true)]
        //[Scenario("Verify newly frozen Fill column automatically gets its fill mode changed to None.")]
        public ScenarioResult VerifyFrozen(TParams p)
        {
            grid.Columns.Clear();
            int colCount = COLUMN_COUNT;
            for (int i = 0; i < colCount; i++)
            {
                DataGridViewTextBoxColumn c = new DataGridViewTextBoxColumn();
                c.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                grid.Columns.Add(c);
            }

            int frozenCol = p.ru.GetRange(0, colCount - 1);
            p.log.WriteLine("Freezing column " + frozenCol.ToString());
            grid.Columns[frozenCol].Frozen = true;

            ScenarioResult ret = new ScenarioResult();
            for (int i = 0; i < colCount; i++)
            {
                p.log.WriteLine("Column " + i.ToString() + " Frozen: " + grid.Columns[i].Frozen);
                ret.IncCounters((grid.Columns[i].Frozen && grid.Columns[i].AutoSizeMode == DataGridViewAutoSizeColumnMode.None) ||
                    (!grid.Columns[i].Frozen && grid.Columns[i].AutoSizeMode == DataGridViewAutoSizeColumnMode.Fill),
                    "Frozen column's AutoSize is not None!", p.log);
            }
            return ret;
        }

        #endregion
    }
}


// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify order of priority: AutoSize (not Fill), Fixed Size, Fill

//@ Verify column sizes are appropriately calculated: [(fill value of column) / (total fill values)] * (DGV.Width).  Use multiple columns with random fill values

//@ Verify total column widths == DGV.Width when (total columns) * (Minimum width) < available space

//@ Change FillWeights on an invisible DGV, then set Visible == true.  Verify changes have taken affect.

//@ Add column at runtime.  Verify widths change accordingly

//@ Reorder columns programmatically.  Verify no issues.

//@ Change a Fill column's width programmatically.  Verify weight changes accordingly and that the DGV draws the columns correctly.

//@ Change a Fill column's weight programmatically.  Verify width changes accordingly and that the DGV draws the columns correctly.

//@ Switch between Fill and non-Fill at both the column and grid level.  Verify no issues.

//@ Verify inheritance: Column overrides grid

//@ Call AutoSize() on a filled column.  Verify ?

//@ Remove a column.  Verify column widths adjust accordingly in runtime.

//@ Toggle visibility.  Verify weights do not change but widths change accordingly for all visible columns.

//@ Verify newly frozen Fill column automatically gets its fill mode changed to None.


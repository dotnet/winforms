// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Forms.IntegrationTests.Common;
using System.Threading;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiSelectionNightliesTests : ReflectBase
    {
        // DataGridView object we will be testing
        private DataGridView dataGridView = null;

        public MauiSelectionNightliesTests(String[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiSelectionNightliesTests(args));
        }
#if MainClassNightlySelectionAutomation
	/**
	* Calls static method LaunchTest to start the test
	*/
	public static void Main(String[] args)
	{
		Application.Run(new NightlySelectionAutomation(args));
	}
#endif

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
        }

        private void ResetTest(DataGridView newDataGridView)
        {
            if (dataGridView != null)
                dataGridView.Dispose();

            dataGridView = newDataGridView;
            dataGridView.Size = new Size(200, 200);
            Controls.Add(dataGridView);

            foreach (DataGridViewColumn col in dataGridView.Columns)
                col.SortMode = DataGridViewColumnSortMode.Programmatic;
        }

        //==========================================
        // Test Methods
        //==========================================
        [Scenario(true)]
        public ScenarioResult SingleSelectionCellSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // In DataGridViewUtils columns 11 and 6 are made invisible
            // also Row 19 is made invisible - will have to expect InvalidOperationException
            // if setting CurrentCell || cell.Selected to cells from these columns/row

            // debug info
            p.log.WriteLine("initially CurrentCell: " + dataGridView.CurrentCellAddress);
            if (dataGridView.SelectedCells.Count > 0)
                p.log.WriteLine("initially SelectedCell: ({0}, {1})", dataGridView.SelectedCells[0].RowIndex, dataGridView.SelectedCells[0].ColumnIndex);
            else
                p.log.WriteLine("nothing is initially selected");

            // set static contexts so we are testing single cell select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;


            // verify that the current cell in the DataGridView is the only cell 
            // selected in the DataGridView
            for (int k = 0; k < dataGridView.Rows.Count; k++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    if (!dataGridView.Rows[k].Cells[j].Equals(dataGridView.CurrentCell))
                    {
                        scenRes.IncCounters(!dataGridView.Rows[k].Cells[j].Selected, " Cell (" + k + "," + j + ") is not the current cell and is selected.", p.log);
                    }
                }
            }

            // randomly choose a cell to set as the current cell
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);

            p.log.WriteLine("WorkingCell is located: row {0}, col {1}", randomRowIndex, randomColIndex);

            DataGridViewCell workingCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            // set current cell to a cell and verify
            try
            {
                // InvalidOperationException should be thrown when setting CurrentCell to cell from invisible row/column
                dataGridView.CurrentCell = workingCell;
                if (workingCell.ColumnIndex == 11 || workingCell.ColumnIndex == 6 || workingCell.RowIndex == 19)
                    scenRes.IncCounters(false, "Failed: no exception when setting CurrentCell to invisible cell", p.log);
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                if (randomColIndex == 1 || randomRowIndex == 0)
                    scenRes.IncCounters(false, p.log, WFCTestLib.Log.BugDb.VSWhidbey, 81799, "Unable to set DataGridView.CurrentCell: " + e);
                else
                    scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 115952, "ArgumentOutOfRangeException in ScrollIntoView()");
            }
            catch (InvalidOperationException ex)
            {
                scenRes.IncCounters((workingCell.ColumnIndex == 11 || workingCell.ColumnIndex == 6 || workingCell.RowIndex == 19), "UnexpectedException when setting Selected for visible cell: " + ex.ToString(), p.log);
            }
            if (!(workingCell.ColumnIndex == 11 || workingCell.ColumnIndex == 6 || workingCell.RowIndex == 19))
            {
                scenRes.IncCounters(workingCell.Equals(dataGridView.CurrentCell), "Unable to set/get DataGridView.CurrentCell to: " + workingCell, p.log);
            }


            // set cell.IsSelected to false and verify
            p.log.WriteLine("... set workingCell.Selected=false ...");
            workingCell.Selected = false;
            scenRes.IncCounters(!workingCell.Selected, "Unable to set/get cell.Selected to false", p.log);

            // set cell.IsSelected to true and verify
            try
            {
                p.log.WriteLine("... set workingCell.Selected=true ...");
                // InvalidOperationException should be thrown when setting Selected for cell in invisible column/row
                workingCell.Selected = true;
                if ((workingCell.ColumnIndex == 11 || workingCell.ColumnIndex == 6 || workingCell.RowIndex == 19))
                    //scenRes.IncCounters(false, "No Exception when setting Selected for invisible cell), p.log");
                scenRes.IncCounters(false, "No Exception when setting Selected for invisible cell), p.log", p.log);
                scenRes.IncCounters(workingCell.Selected, "Unable to set/get cell.Selected to true", p.log);
            }
            catch (ArgumentOutOfRangeException e)
            {
                e.ToString();
                scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 115952, "ArgumentOutOfRangeException in ScrollIntoView()");
            }
            catch (InvalidOperationException ex)
            {
                scenRes.IncCounters((workingCell.ColumnIndex == 11 || workingCell.ColumnIndex == 6 || workingCell.RowIndex == 19), "UnexpectedException when setting Selected for visible cell: " + ex.ToString(), p.log);
            }

            // return if workingCell is invisible
            if (workingCell.ColumnIndex == 11 || workingCell.ColumnIndex == 6 || workingCell.RowIndex == 19)
                return scenRes;

            // try to set cell.IsSelected for multiple cells and verify this is not possible
            p.log.WriteLine("before selecting more cells: SelectedCells.Count = " + dataGridView.SelectedCells.Count);
            if (dataGridView.SelectedCells.Count > 0)
                p.log.WriteLine("selected [0]th: ({0}, {1})", dataGridView.SelectedCells[0].RowIndex, dataGridView.SelectedCells[0].ColumnIndex);

            // obtain second test cell
            // to avoid exception exclude columns 11 & 6
            randomColIndex = getVisibleColumnIndex();

            // exclude 19 row - it's invisible according to DataGridViewUtils
            randomRowIndex = getVisibleRowIndex();

            p.log.WriteLine("--- will select cell ({0}, {1})", randomRowIndex, randomColIndex);
            DataGridViewCell testCellTwo = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            // obtain third test cell
            randomColIndex = getVisibleColumnIndex();
            randomRowIndex = getVisibleRowIndex();

            p.log.WriteLine("--- and cell ({0}, {1})", randomRowIndex, randomColIndex);
            DataGridViewCell testCellThree = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            // try to select the second and third test cells and verify not possible
            testCellTwo.Selected = true;
            testCellThree.Selected = true;
            scenRes.IncCounters(!(workingCell.Selected && testCellTwo.Selected && testCellThree.Selected) && !(workingCell.Selected && testCellTwo.Selected) && !(workingCell.Selected && testCellThree.Selected) && !(testCellTwo.Selected && testCellThree.Selected), "Able to set multiple cells as selected when multiple cell selection should not be possible.", p.log);
            if (!(!(workingCell.Selected && testCellTwo.Selected && testCellThree.Selected) && !(workingCell.Selected && testCellTwo.Selected) && !(workingCell.Selected && testCellThree.Selected) && !(testCellTwo.Selected && testCellThree.Selected)))
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81747, "Should not be able to select multiple cells when DataGridView.MultiSelect = false.");
            }
            scenRes.IncCounters(dataGridView.SelectedCells.Count == 1, "SelectedCells.Count = " + dataGridView.SelectedCells.Count + " instead of expected 1", p.log);
            scenRes.IncCounters(dataGridView.SelectedCells[0].Equals(testCellThree), "Selected cell is not the last one we selected", p.log);
            scenRes.IncCounters(dataGridView.CurrentCell.Equals(testCellThree), "CurrentCell is not the last one we selected", p.log);

            p.log.WriteLine("... set CurrentCell to ({0}, {1})", workingCell.RowIndex, workingCell.ColumnIndex);
            // move current cell from one cell to the next
            dataGridView.CurrentCell = workingCell;

            // get second cell to move currentCell to
            randomColIndex = getVisibleColumnIndex();
            randomRowIndex = getVisibleRowIndex();

            testCellTwo = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];
            p.log.WriteLine("... set CurrentCell to ({0}, {1})", randomRowIndex, randomColIndex);

            dataGridView.CurrentCell = testCellTwo;

            // verify that the currentCell has moved
            scenRes.IncCounters(testCellTwo.Equals(dataGridView.CurrentCell), "Unable to programmatically move the CurrentCell", p.log);
            if (!testCellTwo.Equals(dataGridView.CurrentCell))
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81871, "DataGridView.CurrentCell is not set to the most recent selected cell, which should be the case when DataGridView.MultiSelect = " + dataGridView.MultiSelect + " and DataGridView.SelectionMode = " + dataGridView.SelectionMode);
            }

            p.log.WriteLine("after setting currentCell SelectedCells.Count = " + dataGridView.SelectedCells.Count);
            scenRes.IncCounters(testCellTwo.Selected, "setting CurrentCell should change selection - did not", p.log);
            scenRes.IncCounters(!testCellThree.Selected, "setting CurrentCell should change selection - but did not", p.log);

            p.log.WriteLine("... verify the SelectedCells[0] is the only one selected ...");

            // verify that the current cell in the DataGridView is the only selected cell 
            for (int k = 0; k < dataGridView.Rows.Count; k++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    if (!dataGridView.Rows[k].Cells[j].Equals(testCellTwo))
                    {
                        scenRes.IncCounters(!dataGridView.Rows[k].Cells[j].Selected, "Cell (" + k + "," + j + ") is not SelectedCells[0] and is selected.", p.log);
                    }
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SingleSelectionColumnHeaderSelect_SingleColumnSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing column header select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

            // obtain random column index
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            try
            {
                // Should not be able to access the Selected property of 
                // a column's HeaderCell
                dataGridView.Columns[randomColIndex].HeaderCell.Selected = true;
                scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 81918, "Exception NOT thrown for set_Selected on HeaderCell of Column " + randomColIndex);
            }
            catch (System.InvalidOperationException e)
            {
                p.log.WriteLine("Expected exception: " + e.Message);
                scenRes.IncCounters(true);
            }
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SingleSelectionFullColumnSelect_SingleCellSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            //ResetTest(DataGridViewUtils.GetTypicalDataGridView (true));
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full column select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;

            // Choose a random cell
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            // obtain the random cell and assign it to be the current cell
            DataGridViewCell workingCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            try
            {
                dataGridView.CurrentCell = workingCell;
                if (randomRowIndex == 19 || randomColIndex == 6 || randomColIndex == 11)
                    return new ScenarioResult(false, "No exception when setting Current cell to invisible cell", p.log);
            }
            catch (System.ArgumentOutOfRangeException e)
            {
                if (randomRowIndex == 0 && randomColIndex == 1)
                    scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 81799, "Unable to set DataGridView.CurrentCell: " + e);
                else
                    scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 115952, "ArgumentOutOrRangeException in ScrollIntoView()");
            }
            catch (InvalidOperationException ex)
            {
                ex.ToString();
                return new ScenarioResult((randomRowIndex == 19 || randomColIndex == 6 || randomColIndex == 11), "unexpected exception when setting CurrentCell", p.log);
            }

            // check that assignment is correct
            scenRes.IncCounters(workingCell.Equals(dataGridView.Rows[randomRowIndex].Cells[randomColIndex]), "Value of the cell is " + workingCell + ", which is not its assigned value of " + dataGridView.Rows[randomRowIndex].Cells[randomColIndex], p.log);

            // select the current cell
            workingCell.Selected = true;

            // verify that the cell just selected is the current cell
            scenRes.IncCounters(workingCell.Equals(dataGridView.CurrentCell), "Cell just selected: " + workingCell + " is not equal to the DataGridView current cell: " + dataGridView.CurrentCell, p.log);

            // verify that the entire column containing this cell is selected
            scenRes.IncCounters(dataGridView.Columns[randomColIndex].Selected, "Unable to Select Column " + randomColIndex + " which contains selected cell (" + randomRowIndex + "," + randomColIndex + ")", p.log);
            if (!dataGridView.Columns[randomColIndex].Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Column is not programmatically selected when one of its Cells is selected.");
            }

            // verify that each cell in the column is selected
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[j].Cells[randomColIndex].Selected, "Cell (" + j + "," + workingCell.ColumnIndex + ") is not selected.", p.log);
                if (!dataGridView.Rows[j].Cells[randomColIndex].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Each cell in the column is not programmatically selected when one of its Cells is selected.");
                }
            }

            // verify that no other column is selected
            for (int k = 0; k < dataGridView.Columns.Count; k++)
            {
                // if we are not looking at the column we just selected
                if (k != randomColIndex)
                {
                    scenRes.IncCounters(!dataGridView.Columns[k].Selected, "Column " + k + " is selected when only column " + randomColIndex + " should be.", p.log);
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SingleSelectionFullColumnSelect_SingleCellNotSelected(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full column select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;

            // obtain a random cell
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            DataGridViewCell workingCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            // un-select the cell
            workingCell.Selected = false;

            // verify that the entire column containing this cell is NOT selected
            scenRes.IncCounters(!dataGridView.Columns[randomColIndex].Selected, "Column " + randomColIndex + " is still selected even though cell (" + randomRowIndex + "," + randomColIndex + ") is no longer selected", p.log);

            // verify that each cell in the column is NOT selected
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                scenRes.IncCounters(!dataGridView.Rows[j].Cells[randomColIndex].Selected, "Cell (" + j + "," + randomColIndex + " is still selected.", p.log);
                if (!!dataGridView.Rows[j].Cells[randomColIndex].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Each cell in the column is not programmatically selected when one of its Cells is selected.");
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SingleSelectionFullColumnSelect_SingleColumnSelected(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full column select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;

            DataGridViewCell origCurrentCell = dataGridView.CurrentCell;
            if (origCurrentCell == null)
                p.log.WriteLine("initially current cell is null");
            else
                p.log.WriteLine("initial current cell: " + dataGridView.CurrentCellAddress);

            // obtain random cell
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            DataGridViewColumn workingColumn = dataGridView.Columns[randomColIndex];

            p.log.WriteLine("will select column " + randomColIndex);
            workingColumn.Selected = true;

            /*if ((origCurrentCell != null) && randomRowIndex == 0 && randomColIndex == origCurrentCell.ColumnIndex)
                scenRes.IncCounters (DataGridView.Rows[0].Cells[randomColIndex].Equals (origCurrentCell), "Failed: Unexpected CurrentCell", p.log);
            else
            {
                scenRes.IncCounters (!DataGridView.Rows[0].Cells[randomColIndex].Equals (DataGridView.CurrentCell), p.log, BugDb.VSWhidbey, 81871, "First cell in the selected column: " + DataGridView.Rows[0].Cells[randomColIndex] + " is equal to the DataGridView's current cell: " + DataGridView.CurrentCell);
                if (origCurrentCell == null)
                    scenRes.IncCounters (DataGridView.CurrentCell == null, "didn't preserve origianl null-CurrentCell", p.log);
                else
                    scenRes.IncCounters (DataGridView.CurrentCell.Equals (origCurrentCell), "didn't preserve origianl CurrentCell", p.log);
            }*/

            // verify column is selected
            scenRes.IncCounters(dataGridView.Columns[randomColIndex].Selected, "Unable to Select Column " + randomColIndex, p.log);

            // verify that each cell in the column is selected
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[j].Cells[randomColIndex].Selected, "Cell (" + j + "," + randomColIndex + ") is not selected.", p.log);
                if (!dataGridView.Rows[j].Cells[randomColIndex].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Each cell in the column is not programmatically selected when one of its Cells is selected.");
                }
            }

            p.log.WriteLine("CurrentCell after column selection: " + dataGridView.CurrentCellAddress);

            // verify that the DataGridView's current cell is not set to the first cell in the column
            // original CurrentCell is preserved when selecting column/row
            if (origCurrentCell == null)
                scenRes.IncCounters(dataGridView.CurrentCell == null, "Failed: to preserve init null-CurrentCell", p.log);
            else
                scenRes.IncCounters(dataGridView.CurrentCell.Equals(origCurrentCell), "Failed: to preserve original CurrentCell", p.log);

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SingleSelectionFullColumnSelect_SingleColumnNotSelected(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full column select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;

            // obtain a random column
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            DataGridViewColumn workingColumn = dataGridView.Columns[randomColIndex];

            workingColumn.Selected = false;

            // verify column is not selected
            scenRes.IncCounters(!dataGridView.Columns[randomColIndex].Selected, "Unable to UN-Select (de-select) Column " + randomColIndex, p.log);

            // verify that each cell in the column is NOT selected
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                scenRes.IncCounters(!dataGridView.Rows[j].Cells[randomColIndex].Selected, "Cell (" + j + "," + randomColIndex + ") is STILL selected.", p.log);
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SingleSelectionFullColumnSelect_MultipleColumnSelectAttempt(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full column select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;

            DataGridViewCell origCurrentCell = dataGridView.CurrentCell;
            if (origCurrentCell == null)
                p.log.WriteLine("init CurrentCell is null");
            else
                p.log.WriteLine("init CurrentCell: (col, row)~({0}, {1})", origCurrentCell.ColumnIndex, origCurrentCell.RowIndex);

            // obtain random column index
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            DataGridViewColumn workingColumn = dataGridView.Columns[randomColIndex];
            DataGridViewColumn testColumnOne = null;
            DataGridViewColumn testColumnTwo = null;

            p.log.WriteLine("first column to select: " + randomColIndex);

            workingColumn.Selected = true;

            // verify that the DataGridView's current cell is now set to the first cell in the column
            if ((origCurrentCell != null) && randomColIndex == origCurrentCell.ColumnIndex && origCurrentCell.RowIndex == 0)
                scenRes.IncCounters(dataGridView.Rows[0].Cells[randomColIndex].Equals(dataGridView.CurrentCell), "Didn't preserve CurrentCell in 0-th row after selecting current column in FullColumnSelect", p.log);
            else
                scenRes.IncCounters(!dataGridView.Rows[0].Cells[randomColIndex].Equals(dataGridView.CurrentCell), "First cell in the selected column: " + dataGridView.Rows[0].Cells[randomColIndex] + " is not equal to the DataGridView's current cell: " + dataGridView.CurrentCell, p.log);
            scenRes.IncCounters(((origCurrentCell == null) && (dataGridView.CurrentCell == null)) || ((origCurrentCell != null) && dataGridView.CurrentCell.Equals(origCurrentCell)), p.log, BugDb.VSWhidbey, 81871, "Didn't preserve CurrentCell in FullColumnSelect");
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            testColumnOne = dataGridView.Columns[randomColIndex];
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            testColumnTwo = dataGridView.Columns[randomColIndex];

            p.log.WriteLine("2nd and 3rd columns to select: {0}, {1}", testColumnOne.Index, testColumnTwo.Index);
            // select the two additional cells
            testColumnOne.Selected = true;
            testColumnTwo.Selected = true;
            scenRes.IncCounters(dataGridView.SelectedColumns.Count == 1, p.log, BugDb.VSWhidbey, 81747, "can select multiple columns in MultiSelect=false&FullColumnSelect. SelectedColumns.Count = " + dataGridView.SelectedColumns.Count);
            scenRes.IncCounters(testColumnTwo.Selected, "Failed: to select column that was selected last", p.log);

            // verify the last column we tried to select is the selected column 
            if (dataGridView.SelectedColumns.Count == 1)
                scenRes.IncCounters(dataGridView.SelectedColumns[0].Equals(testColumnTwo), "Failed to select last column", p.log);

            // verify that the last column is tne only one selected
            bool pass = true;
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                if (i == testColumnTwo.Index)
                    pass = pass && dataGridView.Columns[i].Selected;
                else
                {
                    pass = pass && !dataGridView.Columns[i].Selected;
                    if (dataGridView.Columns[i].Selected)
                        p.log.WriteLine("column " + i + " is selected when it should not be");
                }
            }
            scenRes.IncCounters(pass, p.log, BugDb.VSWhidbey, 81747, "Can select multiple columns programmatically when Multiselect = false");

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SingleSelectionFullColumnSelect_ChangingSelectionBetweenTwoColumns(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full column select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;

            DataGridViewCell origCurrent = dataGridView.CurrentCell;
            if (origCurrent == null)
                p.log.WriteLine("init CurrentCell is null");
            else
                p.log.WriteLine("init CurrentCell: " + dataGridView.CurrentCellAddress);

            // obtain random index values
            //int randomRowIndex = p.ru.GetRange (0, DataGridView.Rows.Count - 1);
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            // set the first column as selected
            dataGridView.Columns[randomColIndex].Selected = true;

            // verify that the DataGridView's current cell is not set to the first cell in the column
            //scenRes.IncCounters (!DataGridView.Rows[0].Cells[randomColIndex].Equals (DataGridView.CurrentCell), "First cell in the selected column: " + DataGridView.Rows[0].Cells[randomColIndex] + " is not equal to the DataGridView's current cell: " + DataGridView.CurrentCell, p.log);
            //scenRes.IncCounters (((origCurrent == null) && DataGridView.CurrentCell == null) || (origCurrent != null) && (DataGridView.CurrentCell.Equals (origCurrent)), p.log, BugDb.VSWhidbey, 81871, "didn't preserve CurrentCell in FullRowSelect");
            scenRes.IncCounters(origCurrent.Equals(dataGridView.CurrentCell), "Failed: to preserve CurrentSell after column selection", p.log);

            // un-select the first column
            dataGridView.Columns[randomColIndex].Selected = false;
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            // set the second column as selected
            dataGridView.Columns[randomColIndex].Selected = true;

            // verify that the DataGridView's current cell is not set to the first cell in the column
            //scenRes.IncCounters (!DataGridView.Rows[0].Cells[randomColIndex].Equals (DataGridView.CurrentCell), "First cell in the selected column: " + DataGridView.Rows[0].Cells[randomColIndex] + " is not equal to the DataGridView's current cell: " + DataGridView.CurrentCell, p.log);
            //scenRes.IncCounters (((origCurrent == null) && DataGridView.CurrentCell == null) || (origCurrent != null) && (DataGridView.CurrentCell.Equals (origCurrent)), p.log, BugDb.VSWhidbey, 81871, "didn't preserve CurrentCell in FullRowSelect");
            scenRes.IncCounters(origCurrent.Equals(dataGridView.CurrentCell), "Failed: to preserve CurrentSell after column selection", p.log);

            // verify that the second column is indeed selected
            scenRes.IncCounters(dataGridView.Columns[randomColIndex].Selected, "Unable to move selection from one column to another when in MultiSelect = false, SelectionMode = FullColumn mode.", p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SingleSelectionFullColumnSelect_ChangingSelectionBetweenTwoCells(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full column select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;

            DataGridViewCell origCurrent = dataGridView.CurrentCell;

            // obtain a random cell
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            // set the first column as selected by using one of its cells
            DataGridViewCell firstCellSelected = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            try
            {
                firstCellSelected.Selected = true;
                if (randomRowIndex == 19 || randomColIndex == 6 || randomColIndex == 11)
                    return new ScenarioResult(false, "No exception when selecting invisible cell", p.log);
            }
            catch (InvalidOperationException ex)
            {
                return new ScenarioResult((randomRowIndex == 19 || randomColIndex == 6 || randomColIndex == 11), "Unexpected exception when selecting visible cell: " + ex.ToString(), p.log);
            }

            // verify that the DataGridView's current cell is now set to the cell selected in the column
            scenRes.IncCounters(firstCellSelected.Equals(dataGridView.CurrentCell), "Cell selected in the selected column: " + firstCellSelected + " is not equal to the DataGridView's current cell: " + dataGridView.CurrentCell, p.log);
            if (!firstCellSelected.Equals(dataGridView.CurrentCell))
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81871, "Incorrect value returned when DataGridView.CurrentCell is called programmatically -- first cell in the column should be returned.");
            }

            // un-select the first column by un-selecting one of its cells
            firstCellSelected.Selected = false;

            // set the second column as selected by using one of its cells
            randomRowIndex = getVisibleRowIndex();
            randomColIndex = getVisibleColumnIndex();

            DataGridViewCell secondCellSelected = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            secondCellSelected.Selected = true;

            // verify that the DataGridView's current cell is now set to the cell selected in the column
            scenRes.IncCounters(secondCellSelected.Equals(dataGridView.CurrentCell), "Cell selected in the selected column: " + secondCellSelected + " is not equal to the DataGridView's current cell: " + dataGridView.CurrentCell, p.log);

            // verify that the second column is indeed selected
            scenRes.IncCounters(dataGridView.Columns[secondCellSelected.ColumnIndex].Selected, "Unable to move selection from one column to another using cell selection when in MultiSelect = " + dataGridView.MultiSelect + " and SelectionMode = " + dataGridView.SelectionMode, p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SingleSelectionFullRowSelect_SingleCellSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full row select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // obtain a random cell
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            DataGridViewCell workingCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            try
            {
                workingCell.Selected = true;
                if (randomRowIndex == 19 || randomColIndex == 6 || randomColIndex == 11)
                    return new ScenarioResult(false, "no exception when selecting invisible cell", p.log);
            }
            catch (InvalidOperationException ex)
            {
                return new ScenarioResult((randomRowIndex == 19 || randomColIndex == 6 || randomColIndex == 11), "Unexpected exception when selecting visible cell: " + ex.ToString(), p.log);
            }

            // verify that the DataGridView's current cell is now set to the cell selected in the row
            scenRes.IncCounters(workingCell.Equals(dataGridView.CurrentCell), "Cell selected in the selected row: " + workingCell.RowIndex + " is not equal to the DataGridView's current cell: " + dataGridView.CurrentCell, p.log);
            if (!workingCell.Equals(dataGridView.CurrentCell))
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81871, "Incorrect value returned when DataGridView.CurrentCell is called programmatically.");
            }

            // verify that the entire row containing this cell is selected
            scenRes.IncCounters(dataGridView.Rows[workingCell.RowIndex].Selected, "Unable to Select Row " + workingCell.RowIndex + " which contains selected cell (" + workingCell.RowIndex + "," + workingCell.ColumnIndex + ")", p.log);
            if (!dataGridView.Rows[workingCell.RowIndex].Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Entire row is not selected when one of the cells within the row is selected, as should be the case.");
            }

            // verify that each cell in the row is selected
            for (int j = 0; j < dataGridView.Columns.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[workingCell.RowIndex].Cells[j].Selected, "Cell (" + workingCell.RowIndex + "," + j + ") is not selected.", p.log);
                if (!dataGridView.Rows[workingCell.RowIndex].Cells[j].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Each cell in the row is not programmatically selected when one of its Cells is selected.");
                }
            }

            // verify that no other row is selected
            for (int k = 0; k < dataGridView.Rows.Count; k++)
            {
                // if we are not looking at the row we just selected
                if (k != workingCell.RowIndex)
                {
                    scenRes.IncCounters(!dataGridView.Rows[k].Selected, "Row " + k + " is selected when only row " + workingCell.RowIndex + " should be.", p.log);
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SingleSelectionFullRowSelect_SingleCellNotSelected(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full row select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // obtain a random cell
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            DataGridViewCell currentCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            // un-select the cell
            currentCell.Selected = false;

            // verify that the entire row containing this cell is NOT selected
            scenRes.IncCounters(!dataGridView.Rows[randomRowIndex].Selected, "Row " + randomRowIndex + " is still selected even though cell (" + currentCell.RowIndex + "," + currentCell.ColumnIndex + ") is no longer selected", p.log);

            // verify that each cell in the row is NOT selected
            for (int j = 0; j < dataGridView.Columns.Count; j++)
            {
                scenRes.IncCounters(!dataGridView.Rows[currentCell.RowIndex].Cells[j].Selected, "Cell (" + currentCell.RowIndex + "," + j + " is still selected.", p.log);
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SingleSelectionFullRowSelect_SingleRowSelected(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full row select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            DataGridViewCell origCurrent = dataGridView.CurrentCell;

            // obtain a random cell
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            DataGridViewRow currentRow = dataGridView.Rows[randomRowIndex];

            currentRow.Selected = true;

            scenRes.IncCounters(origCurrent, dataGridView.CurrentCell, "didn't preserve CurrentCell in FullRowSelect", p.log);

            // verify row is selected
            scenRes.IncCounters(dataGridView.Rows[randomRowIndex].Selected, "Unable to Select Row " + randomRowIndex, p.log);

            // verify that each cell in the Row is selected
            for (int j = 0; j < dataGridView.Columns.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[randomRowIndex].Cells[j].Selected, "Cell (" + randomRowIndex + "," + j + ") is not selected.", p.log);
                if (!dataGridView.Rows[randomRowIndex].Cells[j].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Each cell in the row is not programmatically selected when one of its Cells is selected.");
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SingleSelectionFullRowSelect_SingleRowNotSelected(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full row select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // obtain a random row
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            DataGridViewRow currentRow = dataGridView.Rows[randomRowIndex];

            currentRow.Selected = false;

            // verify column is selected
            scenRes.IncCounters(!dataGridView.Rows[randomRowIndex].Selected, "Unable to UN-Select (de-select) Row " + randomRowIndex, p.log);

            // verify that each cell in the row is NOT selected
            for (int j = 0; j < dataGridView.Columns.Count; j++)
            {
                scenRes.IncCounters(!dataGridView.Rows[randomRowIndex].Cells[j].Selected, "Cell (" + randomRowIndex + "," + j + ") is STILL selected.", p.log);
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SingleSelectionFullRowSelect_MultipleRowSelectAttempt(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full row select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            DataGridViewCell origCurrent = dataGridView.CurrentCell;

            // obtain a random row
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            DataGridViewRow currentRow = dataGridView.Rows[randomRowIndex];
            DataGridViewRow testRowOne = null;
            DataGridViewRow testRowTwo = null;

            currentRow.Selected = true;

            // verify that the DataGridView's current cell is preserved when selecting a row
            scenRes.IncCounters(dataGridView.CurrentCell.Equals(origCurrent), "Failed: to preserve CurrentCell after selecting row " + randomRowIndex, p.log);

            // obtain two other random rows
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            testRowOne = dataGridView.Rows[randomRowIndex];
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            testRowTwo = dataGridView.Rows[randomRowIndex];
            testRowOne.Selected = true;
            testRowTwo.Selected = true;
            scenRes.IncCounters(dataGridView.SelectedRows.Count == 1, p.log, BugDb.VSWhidbey, 81747, "can select multiple rows in FullRowSelect with MultiSelect=false");
            scenRes.IncCounters(testRowTwo.Selected, "Failed: to select row that was selected last", p.log);

            // verify that the last column is tne only one selected
            bool pass = true;

            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                if (i == testRowTwo.Index)
                    pass = pass && dataGridView.Rows[i].Selected;
                else
                {
                    pass = pass && !dataGridView.Rows[i].Selected;
                    if (dataGridView.Rows[i].Selected)
                        p.log.WriteLine("row " + i + " is selected when it should not be");
                }
            }
            scenRes.IncCounters(pass, p.log, BugDb.VSWhidbey, 81747, "Can select multiple rows programmatically when Multiselect = false");

            if (dataGridView.SelectedRows.Count == 1)
                scenRes.IncCounters(dataGridView.SelectedRows[0].Equals(testRowTwo), "Failed: to select last row we tried to select", p.log);

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SingleSelectionFullRowSelect_ChangingSelectionBetweenTwoRows(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full row select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            DataGridViewCell origCurrent = dataGridView.CurrentCell;
            if (origCurrent == null)
                p.log.WriteLine("init CurrentCell is null");
            else
                p.log.WriteLine("init CurrentCell: " + dataGridView.CurrentCellAddress);

            // obtain a random row
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);

            // set the first row as selected
            dataGridView.Rows[randomRowIndex].Selected = true;

            // verify that the DataGridView's current cell is not set to the first cell selected in the row
            //scenRes.IncCounters (!DataGridView.Rows[randomRowIndex].Cells[0].Equals (DataGridView.CurrentCell), "First cell in the selected row: " + randomRowIndex + " is equal to the DataGridView's current cell: " + DataGridView.CurrentCell, p.log);
            //scenRes.IncCounters ((origCurrent == null && DataGridView.CurrentCell == null) || (origCurrent != null && origCurrent.Equals (DataGridView.CurrentCell)), p.log, BugDb.VSWhidbey, 81871, "didn't preserve CurrentCell in FullRowSelect");

            // verify that initial CurrentCell is preserved after selecting row
            scenRes.IncCounters(dataGridView.CurrentCell.Equals(origCurrent), "Failed: to preserve initial CurrentCell after selecting row", p.log);

            // un-select the first row
            dataGridView.Rows[randomRowIndex].Selected = false;

            // verify the first row is not selected
            scenRes.IncCounters(!dataGridView.Rows[randomRowIndex].Selected, "Unable to de-select a selected row: " + randomRowIndex + ".", p.log);

            // set the second row as selected
            int secondRandomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);

            dataGridView.Rows[secondRandomRowIndex].Selected = true;

            // verify that the DataGridView's current cell is not set to the first cell selected in the row
            //scenRes.IncCounters (!DataGridView.Rows[secondRandomRowIndex].Cells[0].Equals (DataGridView.CurrentCell), "First cell in the selected row: " + secondRandomRowIndex + " is equal to the DataGridView's current cell: " + DataGridView.CurrentCell, p.log);
            //scenRes.IncCounters ((origCurrent == null && DataGridView.CurrentCell == null) || (origCurrent != null && origCurrent.Equals (DataGridView.CurrentCell)), p.log, BugDb.VSWhidbey, 81871, "didn't preserve CurrentCell in FullRowSelect");

            // verify that initial CurrentCell is preserved after selecting row
            scenRes.IncCounters(dataGridView.CurrentCell.Equals(origCurrent), "Failed: to preserve initial CurrentCell after selecting row", p.log);

            // verify that the second row is indeed selected
            scenRes.IncCounters(dataGridView.Rows[secondRandomRowIndex].Selected, "Unable to move selection from one row to another when in MultiSelect = " + dataGridView.MultiSelect + " and SelectionMode = " + dataGridView.SelectionMode, p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SingleSelectionFullRowSelect_ChangingSelectionBetweenTwoCells(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full row select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            DataGridViewCell origCurrent = dataGridView.CurrentCell;
            p.log.WriteLine("initially CurrentCell: " + dataGridView.CurrentCellAddress);

            // obtain a random row
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            p.log.WriteLine("Cell to be selected: (col, row)~({0}, {1})", randomColIndex, randomRowIndex);
            // set the first row as selected by using one of its cells
            DataGridViewCell firstRow = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            try
            {
                firstRow.Selected = true;
                if (randomRowIndex == 19 || randomColIndex == 6 || randomColIndex == 11)
                    return new ScenarioResult(false, "no exception when selecting invisible cell", p.log);
            }
            catch (InvalidOperationException ex)
            {
                return new ScenarioResult((randomRowIndex == 19 || randomColIndex == 6 || randomColIndex == 11), "Unexpected exception when selecting visible cell: " + ex.ToString(), p.log);
            }

            p.log.WriteLine("new CurrentCell: " + dataGridView.CurrentCellAddress);

            // verify that the DataGridView's current cell is set to the cell selected in the row
            scenRes.IncCounters(firstRow.Equals(dataGridView.CurrentCell), "Setting Cell.Selected didn't set CurrentCell", p.log);
            //scenRes.IncCounters ((origCurrent == null && DataGridView.CurrentCell == null) || (origCurrent != null && origCurrent.Equals (DataGridView.CurrentCell)), p.log, BugDb.VSWhidbey, 81871, "didn't preserve CurrentCell in FullRowSelect");

            // un-select the first row by un-selecting one of its cells
            firstRow.Selected = false;

            // verify the first row is not selected
            scenRes.IncCounters(!dataGridView.Rows[firstRow.RowIndex].Selected, "Unable to de-select a selected row: " + firstRow.RowIndex + ".", p.log);

            // set the second row as selected by using one of its cells
            randomColIndex = getVisibleColumnIndex();
            randomRowIndex = getVisibleRowIndex();

            p.log.WriteLine("new cell to select: (col, row) ~ ({0},{1})", randomColIndex, randomRowIndex);
            DataGridViewCell secondRow = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            secondRow.Selected = true;

            // verify that the DataGridView's current cell is not set to the cell selected in the row
            scenRes.IncCounters(secondRow.Equals(dataGridView.CurrentCell), "CurrentCell was not set after setting Cell.Selected", p.log);

            // verify that the second row is indeed selected
            scenRes.IncCounters(secondRow.Selected, "Unable to move selection from one row to another using cell selection when in MultiSelect = " + dataGridView.MultiSelect + " and SelectionMode = " + dataGridView.SelectionMode, p.log);

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SingleSelectionRowHeaderSelect_SingleRowSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing row header select
            dataGridView.MultiSelect = false;
            dataGridView.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;

            // obtain a random row
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);

            try
            {
                // Should not be able to access the Selected property of 
                // a row's HeaderCell
                dataGridView.Rows[randomRowIndex].HeaderCell.Selected = true;
                scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 81894, "Exception NOT thrown as a result of setting the Selected property of the HeaderCell of Row ");
            }
            catch (System.InvalidOperationException e)
            {
                p.log.WriteLine("Exception: " + e.Message);
                scenRes.IncCounters(true);
            }
            catch (Exception e1)
            {
                e1.ToString();
                scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 81894, "Unexpected exception when setting Selected on HeaderCell of Row ");
            }
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionCellSelect_SingleCellSelection(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing single cell select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;

            // obtain a random row
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            // Select a single cell
            dataGridView.Rows[randomRowIndex].Cells[randomColIndex].Selected = true;

            // verify that this cell is selected
            scenRes.IncCounters(dataGridView.Rows[randomRowIndex].Cells[randomColIndex].Selected, "Cell (" + randomRowIndex + "," + randomColIndex + ") was NOT selected, as expected.", p.log);

            // verify that this is the only cell selected in the DataGridView
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                for (int k = 0; k < dataGridView.Columns.Count; k++)
                {
                    // all cells not equal to the current cell should not be selected
                    if (j != randomRowIndex && k != randomColIndex)
                    {
                        scenRes.IncCounters(!dataGridView.Rows[j].Cells[k].Selected, "Cell (" + j + "," + k + ") should NOT be selected.", p.log);
                    }
                }
            }

            // De-select the cell
            dataGridView.Rows[randomRowIndex].Cells[randomColIndex].Selected = false;

            // Verify that the cell was de-selected
            scenRes.IncCounters(!dataGridView.Rows[randomRowIndex].Cells[randomColIndex].Selected, "Cell: " + dataGridView.Rows[randomRowIndex].Cells[randomColIndex] + " should not longer be selected.", p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionCellSelect_TwoCellBoundaryMultipleSelection(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing single cell select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;

            // obtain a random row
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            // test selection of two cells
            // select the first cell
            DataGridViewCell firstCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            firstCell.Selected = true;

            // verify the first cell is selected
            scenRes.IncCounters(firstCell.Selected, "Cell (" + randomRowIndex + "," + randomColIndex + ") is NOT selected, as expected.", p.log);

            // select the second cell
            DataGridViewCell secondCell = null;

            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            secondCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];
            secondCell.Selected = true;

            // verify that the second cell is selected
            scenRes.IncCounters(secondCell.Selected, "Cell (" + secondCell.RowIndex + "," + secondCell.ColumnIndex + ") is NOT selected, as expected.", p.log);

            // verify that the first cell is still selected
            scenRes.IncCounters(firstCell.Selected, "Cell (" + firstCell.RowIndex + "," + firstCell.ColumnIndex + ") is NOT selected, as expected.", p.log);

            // verfity that the first cell is not the current cell
            scenRes.IncCounters(!firstCell.Equals(dataGridView.CurrentCell), "Cell: " + firstCell + " should NOT be set to the current cell: " + dataGridView.CurrentCell, p.log);

            // verify that no other cells in the DataGridView are selected
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                for (int k = 0; k < dataGridView.Columns.Count; k++)
                {
                    if ((j != firstCell.RowIndex && k != firstCell.ColumnIndex) && (j != secondCell.RowIndex && k != secondCell.ColumnIndex))
                    {
                        // this cell should not be selected
                        scenRes.IncCounters(!dataGridView.Rows[j].Cells[k].Selected, "Cell (" + j + "," + k + ") should NOT be selected.", p.log);
                    }
                }
            }

            // un-select the first cell
            firstCell.Selected = false;

            // verify that the first cell is unselected
            scenRes.IncCounters(!firstCell.Selected, "Cell (" + firstCell.RowIndex + "," + firstCell.ColumnIndex + ") should NOT be selected.", p.log);

            // un-select the second cell
            secondCell.Selected = false;

            // verify that the second cell is unselected
            scenRes.IncCounters(!secondCell.Selected, "Cell (" + secondCell.RowIndex + "," + secondCell.ColumnIndex + ") should NOT be selected.", p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionCellSelect_ThreeCellRandomMultipleSelection(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing single cell select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;

            // choose three cells randomly and select them at the same time
            int randomRowIndex = 0;
            int randomColIndex = 0;

            // select first random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell firstRandomCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            firstRandomCell.Selected = true;

            // verify first random cell is selected
            scenRes.IncCounters(firstRandomCell.Selected, "Unable to select Cell (" + randomRowIndex + "," + randomColIndex + ").", p.log);

            // select second random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell secondRandomCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            secondRandomCell.Selected = true;

            // verify second random cell is selected
            scenRes.IncCounters(secondRandomCell.Selected, "Unable to select Cell (" + randomRowIndex + "," + randomColIndex + ").", p.log);

            // verify first and second cell are still selected
            scenRes.IncCounters(firstRandomCell.Selected && secondRandomCell.Selected, "Unable to simultaneoulsy select cells: " + firstRandomCell + " and " + secondRandomCell, p.log);

            // select third random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell thirdRandomCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            thirdRandomCell.Selected = true;

            // verify third random cell is selected
            scenRes.IncCounters(thirdRandomCell.Selected, "Unable to select Cell (" + randomRowIndex + "," + randomColIndex + ").", p.log);

            // verify first, second and third random cells are selected at same time
            scenRes.IncCounters(firstRandomCell.Selected && secondRandomCell.Selected && thirdRandomCell.Selected, "Unable to simultaneoulsy select cells: " + firstRandomCell + " and " + secondRandomCell + " and " + thirdRandomCell, p.log);

            // verify that all other cells in the DataGridView are not selected
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    if ((i != firstRandomCell.RowIndex && j != firstRandomCell.ColumnIndex) && (i != secondRandomCell.RowIndex && j != secondRandomCell.ColumnIndex) && (i != thirdRandomCell.RowIndex && j != secondRandomCell.ColumnIndex))
                    {
                        scenRes.IncCounters(!dataGridView.Rows[i].Cells[j].Selected, "Cell (" + i + "," + j + ") should NOT be selected.", p.log);
                    }
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionCellSelect_FourCellContinuousMultipleSelection(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing single cell select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;

            // randomly choose whether we are going to select cells continuously across a row or a column
            bool doRowSelect = false;

            doRowSelect = p.ru.GetBoolean();

            int randomRowIndex = 0;
            int randomColIndex = 0;
            int randomRowStartIndex = 0;
            int randomColStartIndex = 0;
            DataGridViewCell firstCell = null;
            DataGridViewCell secondCell = null;
            DataGridViewCell thirdCell = null;
            DataGridViewCell fourthCell = null;

            if (doRowSelect)
            {
                // randomly select a row
                randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);

                // randomly select a starting point, carefull not to fall off end of DataGridView 
                // when chosing starting point
                randomColStartIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 5);

                // select first cell
                firstCell = dataGridView.Rows[randomRowIndex].Cells[randomColStartIndex];
                firstCell.Selected = true;

                // verify first cell is selected
                scenRes.IncCounters(firstCell.Selected, "Unable to select cell: " + firstCell, p.log);

                // select second cell
                secondCell = dataGridView.Rows[randomRowIndex].Cells[randomColStartIndex + 1];
                secondCell.Selected = true;

                // verify second cell is selected
                scenRes.IncCounters(secondCell.Selected, "Unable to select cell: " + secondCell, p.log);

                // verify first and second cells are selected at same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell, p.log);

                // select third cell
                thirdCell = dataGridView.Rows[randomRowIndex].Cells[randomColStartIndex + 2];
                thirdCell.Selected = true;

                // verify third cell is selected
                scenRes.IncCounters(thirdCell.Selected, "Unable to select cell: " + thirdCell, p.log);

                // verify first, second, and third cells are selected at the same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected && thirdCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell + " and " + thirdCell, p.log);

                // select fourth cell
                fourthCell = dataGridView.Rows[randomRowIndex].Cells[randomColStartIndex + 3];
                fourthCell.Selected = true;

                // verify fourth cell is selected
                scenRes.IncCounters(fourthCell.Selected, "Unable to select cell: " + fourthCell, p.log);

                // verify first, second, third and fourth cells are selected at the same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected && thirdCell.Selected && fourthCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell + " and " + thirdCell + " and " + fourthCell, p.log);
            }
            else
            {
                // randomly select a column
                randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

                // randomly select a starting point, carefull not to fall off end of DataGridView
                // when chosing starting point
                randomRowStartIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 5);

                // select first cell
                firstCell = dataGridView.Rows[randomRowStartIndex].Cells[randomColIndex];
                firstCell.Selected = true;

                // verify first cell is selected
                scenRes.IncCounters(firstCell.Selected, "Unable to select cell: " + firstCell, p.log);

                // select second cell
                secondCell = dataGridView.Rows[randomRowStartIndex + 1].Cells[randomColIndex];
                secondCell.Selected = true;

                // verify second cell is selected
                scenRes.IncCounters(secondCell.Selected, "Unable to select cell: " + secondCell, p.log);

                // verify first and second cells are selected at same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell, p.log);

                // select third cell
                thirdCell = dataGridView.Rows[randomRowStartIndex + 2].Cells[randomColIndex];
                thirdCell.Selected = true;

                // verify third cell is selected
                scenRes.IncCounters(thirdCell.Selected, "Unable to select cell: " + thirdCell, p.log);

                // verify first, second, and third cells are selected at the same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected && thirdCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell + " and " + thirdCell, p.log);

                // select fourth cell
                fourthCell = dataGridView.Rows[randomRowStartIndex + 3].Cells[randomColIndex];
                fourthCell.Selected = true;

                // verify fourth cell is selected
                scenRes.IncCounters(fourthCell.Selected, "Unable to select cell: " + fourthCell, p.log);

                // verify first, second, third and fourth cells are selected at the same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected && thirdCell.Selected && fourthCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell + " and " + thirdCell + " and " + fourthCell, p.log);
            }

            // verify that only these four cells are selected at the same time
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    if ((i != firstCell.RowIndex && j != firstCell.ColumnIndex) && (i != secondCell.RowIndex && j != secondCell.ColumnIndex) && (i != thirdCell.RowIndex && j != thirdCell.ColumnIndex) && (i != fourthCell.RowIndex && j != fourthCell.ColumnIndex))
                    {
                        // cell should not be selected
                        scenRes.IncCounters(!dataGridView.Rows[i].Cells[j].Selected, "Cell " + dataGridView.Rows[i].Cells[j] + " should NOT be selected.", p.log);
                    }
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionCellSelect_TopLeftHeaderCellSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing single cell select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;
            try
            {
                // Should not be able to access the Selected property of 
                // a DataGridView's TopLeftHeaderCell
                // select the top left header cell
                dataGridView.TopLeftHeaderCell.Selected = true;
                scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 81894, "No Exception when setting Selected property on TopLeftHeaderCell");
            }
            catch (System.InvalidOperationException e)
            {
                p.log.WriteLine("Exception: " + e.Message);
                scenRes.IncCounters(true);
            }
            catch (Exception ex)
            {
                ex.ToString();
                scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 81894, "Unexpected Exception when setting Selected property on TopLeftHeaderCell");
            }
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionColumnHeaderSelect_SingleCellSelection(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing column header select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

            // obtain a random row
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            // Select a single cell
            DataGridViewCell singleCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            singleCell.Selected = true;

            // verify that this cell is selected
            scenRes.IncCounters(singleCell.Selected, "Cell (" + singleCell.RowIndex + "," + singleCell.ColumnIndex + ") was NOT selected, as expected.", p.log);

            // verify that this is the only cell selected in the DataGridView
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                for (int k = 0; k < dataGridView.Columns.Count; k++)
                {
                    // all cells not equal to the current cell should not be selected
                    if (j != singleCell.RowIndex && k != singleCell.ColumnIndex)
                    {
                        scenRes.IncCounters(!dataGridView.Rows[j].Cells[k].Selected, "Cell (" + j + "," + k + ") should NOT be selected.", p.log);
                    }
                }
            }

            // Un-Select the cell
            singleCell.Selected = false;

            // verify that this cell is un-selected
            scenRes.IncCounters(!singleCell.Selected, "Cell (" + singleCell.RowIndex + "," + singleCell.ColumnIndex + ") should no longer be selected.", p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionColumnHeaderSelect_TwoCellBoundaryMultipleSelection(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing column header select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

            // obtain a random row
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            // select the first cell
            DataGridViewCell firstCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            firstCell.Selected = true;

            // verify the first cell is selected
            scenRes.IncCounters(firstCell.Selected, "Cell (" + firstCell.RowIndex + "," + firstCell.ColumnIndex + ") is NOT selected, as expected.", p.log);

            // select the second cell
            DataGridViewCell secondCell = null;

            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            secondCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];
            secondCell.Selected = true;

            // verify that the second cell is selected
            scenRes.IncCounters(secondCell.Selected, "Cell (" + secondCell.RowIndex + "," + secondCell.ColumnIndex + ") is NOT selected, as expected.", p.log);

            // verify that the first cell is still selected
            scenRes.IncCounters(firstCell.Selected, "Cell (" + firstCell.RowIndex + "," + firstCell.ColumnIndex + ") is NOT selected, as expected.", p.log);

            // verfity that the first cell is not the current cell
            scenRes.IncCounters(!firstCell.Equals(dataGridView.CurrentCell), "Cell: " + firstCell + " should NOT be set to the current cell: " + dataGridView.CurrentCell, p.log);

            // verify that no other cells in the DataGridView are selected
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                for (int k = 0; k < dataGridView.Columns.Count; k++)
                {
                    if ((j != firstCell.RowIndex && k != firstCell.ColumnIndex) && (j != secondCell.RowIndex && k != secondCell.ColumnIndex))
                    {
                        // this cell should not be selected
                        scenRes.IncCounters(!dataGridView.Rows[j].Cells[k].Selected, "Cell (" + j + "," + k + ") should NOT be selected.", p.log);
                    }
                }
            }

            // un-select the first cell
            firstCell.Selected = false;

            // verify that the first cell is unselected
            scenRes.IncCounters(!firstCell.Selected, "Cell (" + firstCell.RowIndex + "," + firstCell.ColumnIndex + ") should NOT be selected.", p.log);

            // un-select the second cell
            secondCell.Selected = false;

            // verify that the second cell is unselected
            scenRes.IncCounters(!secondCell.Selected, "Cell (" + secondCell.RowIndex + "," + secondCell.ColumnIndex + ") should NOT be selected.", p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionColumnHeaderSelect_ThreeCellRandomMultipleSelection(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing column header select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

            // choose three cells randomly and select them at the same time
            int randomRowIndex = 0;
            int randomColIndex = 0;

            // select first random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell firstRandomCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            firstRandomCell.Selected = true;

            // verify first random cell is selected
            scenRes.IncCounters(firstRandomCell.Selected, "Unable to select Cell (" + randomRowIndex + "," + randomColIndex + ").", p.log);

            // select second random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell secondRandomCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            secondRandomCell.Selected = true;

            // verify second random cell is selected
            scenRes.IncCounters(secondRandomCell.Selected, "Unable to select Cell (" + randomRowIndex + "," + randomColIndex + ").", p.log);

            // verify first and second cell are still selected
            scenRes.IncCounters(firstRandomCell.Selected && secondRandomCell.Selected, "Unable to simultaneoulsy select cells: " + firstRandomCell + " and " + secondRandomCell, p.log);

            // select third random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell thirdRandomCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            thirdRandomCell.Selected = true;

            // verify third random cell is selected
            scenRes.IncCounters(thirdRandomCell.Selected, "Unable to select Cell (" + randomRowIndex + "," + randomColIndex + ").", p.log);

            // verify first, second and third random cells are selected at same time
            scenRes.IncCounters(firstRandomCell.Selected && secondRandomCell.Selected && thirdRandomCell.Selected, "Unable to simultaneoulsy select cells: " + firstRandomCell + " and " + secondRandomCell + " and " + thirdRandomCell, p.log);

            // verify that all other cells in the DataGridView are not selected
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    if ((i != firstRandomCell.RowIndex && j != firstRandomCell.ColumnIndex) && (i != secondRandomCell.RowIndex && j != secondRandomCell.ColumnIndex) && (i != thirdRandomCell.RowIndex && j != secondRandomCell.ColumnIndex))
                    {
                        scenRes.IncCounters(!dataGridView.Rows[i].Cells[j].Selected, "Cell (" + i + "," + j + ") should NOT be selected.", p.log);
                    }
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionColumnHeaderSelect_FourCellContinuousMultipleSelection(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing column header select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;

            // randomly choose whether we are going to select cells continuously across a row or a column
            bool doRowSelect = false;

            doRowSelect = p.ru.GetBoolean();

            int randomRowIndex = 0;
            int randomColIndex = 0;
            int randomRowStartIndex = 0;
            int randomColStartIndex = 0;
            DataGridViewCell firstCell = null;
            DataGridViewCell secondCell = null;
            DataGridViewCell thirdCell = null;
            DataGridViewCell fourthCell = null;

            if (doRowSelect)
            {
                // randomly select a row
                randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);

                // randomly select a starting point, carefull not to fall off end of DataGridView 
                // when chosing starting point
                randomColStartIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 5);

                // select first cell
                firstCell = dataGridView.Rows[randomRowIndex].Cells[randomColStartIndex];
                firstCell.Selected = true;

                // verify first cell is selected
                scenRes.IncCounters(firstCell.Selected, "Unable to select cell: " + firstCell, p.log);

                // select second cell
                secondCell = dataGridView.Rows[randomRowIndex].Cells[randomColStartIndex + 1];
                secondCell.Selected = true;

                // verify second cell is selected
                scenRes.IncCounters(secondCell.Selected, "Unable to select cell: " + secondCell, p.log);

                // verify first and second cells are selected at same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell, p.log);

                // select third cell
                thirdCell = dataGridView.Rows[randomRowIndex].Cells[randomColStartIndex + 2];
                thirdCell.Selected = true;

                // verify third cell is selected
                scenRes.IncCounters(thirdCell.Selected, "Unable to select cell: " + thirdCell, p.log);

                // verify first, second, and third cells are selected at the same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected && thirdCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell + " and " + thirdCell, p.log);

                // select fourth cell
                fourthCell = dataGridView.Rows[randomRowIndex].Cells[randomColStartIndex + 3];
                fourthCell.Selected = true;

                // verify fourth cell is selected
                scenRes.IncCounters(fourthCell.Selected, "Unable to select cell: " + fourthCell, p.log);

                // verify first, second, third and fourth cells are selected at the same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected && thirdCell.Selected && fourthCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell + " and " + thirdCell + " and " + fourthCell, p.log);
            }
            else
            {
                // randomly select a column
                randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

                // randomly select a starting point, carefull not to fall off end of DataGridView
                // when chosing starting point
                randomRowStartIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 5);

                // select first cell
                firstCell = dataGridView.Rows[randomRowStartIndex].Cells[randomColIndex];
                firstCell.Selected = true;

                // verify first cell is selected
                scenRes.IncCounters(firstCell.Selected, "Unable to select cell: " + firstCell, p.log);

                // select second cell
                secondCell = dataGridView.Rows[randomRowStartIndex + 1].Cells[randomColIndex];
                secondCell.Selected = true;

                // verify second cell is selected
                scenRes.IncCounters(secondCell.Selected, "Unable to select cell: " + secondCell, p.log);

                // verify first and second cells are selected at same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell, p.log);

                // select third cell
                thirdCell = dataGridView.Rows[randomRowStartIndex + 2].Cells[randomColIndex];
                thirdCell.Selected = true;

                // verify third cell is selected
                scenRes.IncCounters(thirdCell.Selected, "Unable to select cell: " + thirdCell, p.log);

                // verify first, second, and third cells are selected at the same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected && thirdCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell + " and " + thirdCell, p.log);

                // select fourth cell
                fourthCell = dataGridView.Rows[randomRowStartIndex + 3].Cells[randomColIndex];
                fourthCell.Selected = true;

                // verify fourth cell is selected
                scenRes.IncCounters(fourthCell.Selected, "Unable to select cell: " + fourthCell, p.log);

                // verify first, second, third and fourth cells are selected at the same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected && thirdCell.Selected && fourthCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell + " and " + thirdCell + " and " + fourthCell, p.log);
            }

            // verify that only these four cells are selected at the same time
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    if ((i != firstCell.RowIndex && j != firstCell.ColumnIndex) && (i != secondCell.RowIndex && j != secondCell.ColumnIndex) && (i != thirdCell.RowIndex && j != thirdCell.ColumnIndex) && (i != fourthCell.RowIndex && j != fourthCell.ColumnIndex))
                    {
                        // cell should not be selected
                        scenRes.IncCounters(!dataGridView.Rows[i].Cells[j].Selected, "Cell " + dataGridView.Rows[i].Cells[j] + " should NOT be selected.", p.log);
                    }
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionColumnHeaderSelect_TopLeftHeaderCellSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing column header select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.ColumnHeaderSelect;
            try
            {
                // Should not be able to access the Selected property of 
                // a DataGridView's TopLeftHeaderCell
                // select the top left header cell
                dataGridView.TopLeftHeaderCell.Selected = true;
                scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 81894, "No exception when setting the Selected on TopLeftHeaderCell");
            }
            catch (System.InvalidOperationException e)
            {
                p.log.WriteLine("Exception: " + e.Message);
                scenRes.IncCounters(true);
            }
            catch (Exception e1)
            {
                scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 81894, "Unexpected exception when setting the Selected on TopLeftHeaderCell: " + e1.Message);
            }
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionFullColumnSelect_SingleCellSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full column select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;

            // select a random cell
            int randomRowIndex = 0;
            int randomColIndex = 0;
            DataGridViewCell currentCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            scenRes.IncCounters(currentCell.Equals(dataGridView.Rows[randomRowIndex].Cells[randomColIndex]), "Value of the current cell is " + currentCell + ", which is not its assigned value of " + dataGridView.Rows[randomRowIndex].Cells[randomColIndex], p.log);
            currentCell.Selected = true;

            // verify that the cell is selected
            scenRes.IncCounters(currentCell.Selected, "Unable to select the cell:" + currentCell, p.log);
            if (!currentCell.Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Selection of one cell when in DataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect should result in the column.Selected = true for that column, but currently does not.");
            }

            // verify that the entire column containing this cell is selected
            scenRes.IncCounters(dataGridView.Columns[currentCell.ColumnIndex].Selected, "Unable to Select Column " + currentCell.ColumnIndex + " which contains selected cell (" + currentCell.RowIndex + "," + currentCell.ColumnIndex + ")", p.log);
            if (!dataGridView.Columns[currentCell.ColumnIndex].Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Selection of one cell when in DataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect should result in the entire column being selected, but currently does not.");
            }

            // verify that each cell in the column is selected
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[j].Cells[currentCell.ColumnIndex].Selected, "Cell (" + j + "," + currentCell.ColumnIndex + ") is not selected.", p.log);
                if (!dataGridView.Rows[j].Cells[currentCell.ColumnIndex].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Each cell in the column is not programmatically selected when one of its Cells is selected.");
                }
            }

            // verify that no other column is selected
            for (int k = 0; k < dataGridView.Columns.Count; k++)
            {
                // if we are not looking at the column we just selected
                if (k != currentCell.ColumnIndex)
                {
                    scenRes.IncCounters(!dataGridView.Columns[k].Selected, "Column " + k + " is selected when only column " + currentCell.ColumnIndex + " should be.", p.log);
                }
            }

            // Unselect the column
            dataGridView.Columns[currentCell.ColumnIndex].Selected = false;

            // verify that the column was unselected
            scenRes.IncCounters(!dataGridView.Columns[currentCell.ColumnIndex].Selected, "Unable to DE-select column: " + currentCell.ColumnIndex, p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionFullColumnSelect_SingleColumnSelected(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full column select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;

            // obtain a random column
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            DataGridViewColumn currentColumn = dataGridView.Columns[randomColIndex];

            currentColumn.Selected = true;

            // verify column is selected
            scenRes.IncCounters(dataGridView.Columns[randomColIndex].Selected, "Unable to Select Column " + randomColIndex, p.log);

            // verify that each cell in the column is selected
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[j].Cells[randomColIndex].Selected, "Cell (" + j + "," + randomColIndex + ") is not selected.", p.log);
                if (!dataGridView.Rows[j].Cells[randomColIndex].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Each cell in the column is not programmatically selected when one of its Cells is selected.");
                }
            }

            // verify that no other column is selected
            for (int k = 0; k < dataGridView.Columns.Count; k++)
            {
                // if we are not looking at the column we just selected
                if (k != randomColIndex)
                {
                    scenRes.IncCounters(!dataGridView.Columns[k].Selected, "Column " + k + " is selected when only column " + randomColIndex + " should be.", p.log);
                }
            }

            // Unselect the column
            dataGridView.Columns[randomColIndex].Selected = false;

            // verify that the column was unselected
            scenRes.IncCounters(!dataGridView.Columns[randomColIndex].Selected, "Unable to DE-select column: " + randomColIndex, p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionFullColumnSelect_TwoColumnBoundaryMultipleSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full column select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;

            int randomRowIndex = 0;
            int randomColIndex = 0;
            DataGridViewCell firstColumnCell = null;
            DataGridViewCell secondColumnCell = null;

            // select the first column by random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            firstColumnCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];
            firstColumnCell.Selected = true;

            // verify the first column is selected
            scenRes.IncCounters(dataGridView.Columns[randomColIndex].Selected, "Unable to select column: " + randomColIndex, p.log);
            if (!dataGridView.Columns[randomColIndex].Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "When DataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect, selection of a cell in a column should result in column.Select = true for that column, which it currently does not.");
            }

            // verify the random cell is selected
            scenRes.IncCounters(firstColumnCell.Selected, "Unable to select cell " + firstColumnCell, p.log);
            if (!firstColumnCell.Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "When DataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect, selection of a cell in a column should result in all of the cells in that column being selected, which it currently does not.");
            }

            // select the second column by random cell
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            secondColumnCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];
            secondColumnCell.Selected = true;

            // verify the second column is selected
            scenRes.IncCounters(dataGridView.Columns[randomColIndex].Selected, "Unable to select column: " + dataGridView.Columns[randomColIndex], p.log);

            // verify the random cell is selected
            scenRes.IncCounters(secondColumnCell.Selected, "Unable to select cell " + secondColumnCell, p.log);

            // verify that both the first and second columns are selected
            scenRes.IncCounters(dataGridView.Columns[firstColumnCell.ColumnIndex].Selected && dataGridView.Columns[secondColumnCell.ColumnIndex].Selected, "Unable to simultaneously select two columns: " + firstColumnCell.ColumnIndex + " and " + secondColumnCell.ColumnIndex, p.log);

            // verify that every cell in both column is selected
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[j].Cells[firstColumnCell.ColumnIndex].Selected, "Unable to select cell " + dataGridView.Rows[j].Cells[firstColumnCell.ColumnIndex], p.log);
                scenRes.IncCounters(dataGridView.Rows[j].Cells[secondColumnCell.ColumnIndex].Selected, "Unable to select cell " + dataGridView.Rows[j].Cells[secondColumnCell.ColumnIndex], p.log);
                if (!dataGridView.Rows[j].Cells[firstColumnCell.ColumnIndex].Selected || !dataGridView.Rows[j].Cells[secondColumnCell.ColumnIndex].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Each cell in the column is not programmatically selected when one of its Cells is selected.");
                }
            }

            // de-select both columns
            dataGridView.Columns[firstColumnCell.ColumnIndex].Selected = false;
            dataGridView.Columns[secondColumnCell.ColumnIndex].Selected = false;

            // verify that both columns are no longer selected
            scenRes.IncCounters(!dataGridView.Columns[firstColumnCell.ColumnIndex].Selected, "Column " + dataGridView.Columns[firstColumnCell.ColumnIndex] + " was not de selected.", p.log);
            scenRes.IncCounters(!dataGridView.Columns[secondColumnCell.ColumnIndex].Selected, "Column " + dataGridView.Columns[secondColumnCell.ColumnIndex] + " was not de selected.", p.log);

            // verify that all cells in both columns are no longer selected
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                scenRes.IncCounters(!dataGridView.Rows[j].Cells[firstColumnCell.ColumnIndex].Selected, "Cell " + dataGridView.Rows[j].Cells[firstColumnCell.ColumnIndex] + " was not de selected.", p.log);
                scenRes.IncCounters(!dataGridView.Rows[j].Cells[secondColumnCell.ColumnIndex].Selected, "Cell " + dataGridView.Rows[j].Cells[secondColumnCell.ColumnIndex] + " was not de selected.", p.log);
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionFullColumnSelect_ThreeColumnRandomMultipleSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full column select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;

            // choose three cells randomly and select them at the same time
            int randomRowIndex = 0;
            int randomColIndex = 0;

            // select first random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell firstRandomColumnCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            firstRandomColumnCell.Selected = true;

            // verify the column is selected
            scenRes.IncCounters(dataGridView.Columns[firstRandomColumnCell.ColumnIndex].Selected, "Unable to select column " + dataGridView.Columns[firstRandomColumnCell.ColumnIndex], p.log);
            if (!dataGridView.Columns[firstRandomColumnCell.ColumnIndex].Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Selection of a single cell in a column should result in the column being selected when DataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect, but this is not currently the case.");
            }

            // verify first random cell is selected
            scenRes.IncCounters(firstRandomColumnCell.Selected, "Unable to select Cell " + firstRandomColumnCell, p.log);

            // verify each cell in the first selected column is selected
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[j].Cells[firstRandomColumnCell.ColumnIndex].Selected, "Unable to select cell " + dataGridView.Rows[j].Cells[firstRandomColumnCell.ColumnIndex], p.log);
                if (!dataGridView.Rows[j].Cells[firstRandomColumnCell.ColumnIndex].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Selection of a single cell in a column when DataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect should result in all of the cells in the column being selected, but this is currently not the case.");
                }
            }

            // select second random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell secondRandomColumnCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            secondRandomColumnCell.Selected = true;

            // verify that second column is selected
            scenRes.IncCounters(dataGridView.Columns[secondRandomColumnCell.ColumnIndex].Selected, "Unable to select column " + dataGridView.Columns[secondRandomColumnCell.ColumnIndex], p.log);

            // verify second random cell is selected
            scenRes.IncCounters(secondRandomColumnCell.Selected, "Unable to select Cell " + secondRandomColumnCell, p.log);

            // verify first and second cell are still selected
            scenRes.IncCounters(firstRandomColumnCell.Selected && secondRandomColumnCell.Selected, "Unable to simultaneoulsy select cells: " + firstRandomColumnCell + " and " + secondRandomColumnCell, p.log);

            // verify each cell in the first two selected columns are selected
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[j].Cells[firstRandomColumnCell.ColumnIndex].Selected, "Unable to select cell " + dataGridView.Rows[j].Cells[firstRandomColumnCell.ColumnIndex], p.log);
                scenRes.IncCounters(dataGridView.Rows[j].Cells[secondRandomColumnCell.ColumnIndex].Selected, "Unable to select cell " + dataGridView.Rows[j].Cells[secondRandomColumnCell.ColumnIndex], p.log);
                if (!dataGridView.Rows[j].Cells[firstRandomColumnCell.ColumnIndex].Selected || !dataGridView.Rows[j].Cells[secondRandomColumnCell.ColumnIndex].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Selection of a single cell in a column when DataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect should result in all of the cells in the column being selected, but this is currently not the case.");
                }
            }

            // select third random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell thirdRandomColumnCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            thirdRandomColumnCell.Selected = true;

            // verify third column is selected
            scenRes.IncCounters(dataGridView.Columns[thirdRandomColumnCell.ColumnIndex].Selected, "Unable to select column " + thirdRandomColumnCell, p.log);

            // verify third random cell is selected
            scenRes.IncCounters(thirdRandomColumnCell.Selected, "Unable to select Cell " + thirdRandomColumnCell, p.log);

            // verify first, second and third random cells are selected at same time
            scenRes.IncCounters(firstRandomColumnCell.Selected && secondRandomColumnCell.Selected && thirdRandomColumnCell.Selected, "Unable to simultaneoulsy select cells: " + firstRandomColumnCell + " and " + secondRandomColumnCell + " and " + thirdRandomColumnCell, p.log);

            // verify that all cells in the first, second and third selected columns are selected
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                scenRes.IncCounters(dataGridView.Rows[i].Cells[firstRandomColumnCell.ColumnIndex].Selected, "Unable to select cell " + dataGridView.Rows[i].Cells[firstRandomColumnCell.ColumnIndex], p.log);
                scenRes.IncCounters(dataGridView.Rows[i].Cells[secondRandomColumnCell.ColumnIndex].Selected, "Unable to select cell " + dataGridView.Rows[i].Cells[secondRandomColumnCell.ColumnIndex], p.log);
                scenRes.IncCounters(dataGridView.Rows[i].Cells[thirdRandomColumnCell.ColumnIndex].Selected, "Unable to select cell " + dataGridView.Rows[i].Cells[thirdRandomColumnCell.ColumnIndex], p.log);
                if (!dataGridView.Rows[i].Cells[firstRandomColumnCell.ColumnIndex].Selected || !dataGridView.Rows[i].Cells[secondRandomColumnCell.ColumnIndex].Selected || !dataGridView.Rows[i].Cells[thirdRandomColumnCell.ColumnIndex].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Selection of a single cell in a column when DataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect should result in all of the cells in the column being selected, but this is currently not the case.");
                }
            }

            // verify that all cells NOT in the first, second and third selected columns are NOT selected
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    if (j != firstRandomColumnCell.ColumnIndex && j != secondRandomColumnCell.ColumnIndex && j != thirdRandomColumnCell.ColumnIndex)
                    {
                        scenRes.IncCounters(!dataGridView.Rows[i].Cells[j].Selected, "Cell " + dataGridView.Rows[i].Cells[j] + " should NOT have been selected.", p.log);
                    }
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionFullColumnSelect_FourColumnContinuousRandomMultipleSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full column select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;

            // randomly select a column to start the continuous selection from,
            // carful not to fall off the end of the DataGridView
            int randomColStartIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 5);

            // random row index we will use for the cell we will select the column with
            int randomRowIndex = 0;

            // select first random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);

            DataGridViewCell firstRandomColumnCell = dataGridView.Rows[randomRowIndex].Cells[randomColStartIndex];

            firstRandomColumnCell.Selected = true;

            // verify the column is selected
            scenRes.IncCounters(dataGridView.Columns[firstRandomColumnCell.ColumnIndex].Selected, "Unable to select column " + dataGridView.Columns[firstRandomColumnCell.ColumnIndex], p.log);
            if (!dataGridView.Columns[firstRandomColumnCell.ColumnIndex].Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "When a cell in a column is selected and DataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect, this should result in column.Selected = true, which it currently does not.");
            }

            // verify first random cell is selected
            scenRes.IncCounters(firstRandomColumnCell.Selected, "Unable to select Cell " + firstRandomColumnCell, p.log);

            // verify each cell in the first selected column is selected
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[j].Cells[firstRandomColumnCell.ColumnIndex].Selected, "Unable to select cell " + dataGridView.Rows[j].Cells[firstRandomColumnCell.ColumnIndex], p.log);
                if (!dataGridView.Rows[j].Cells[firstRandomColumnCell.ColumnIndex].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "When a cell in a column is selected and DataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect, this should result in all of the cells in the column being selected, which it currently does not.");
                }
            }

            // select second random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);

            DataGridViewCell secondRandomColumnCell = dataGridView.Rows[randomRowIndex].Cells[randomColStartIndex + 1];

            secondRandomColumnCell.Selected = true;

            // verify that second column is selected
            scenRes.IncCounters(dataGridView.Columns[secondRandomColumnCell.ColumnIndex].Selected, "Unable to select column " + dataGridView.Columns[secondRandomColumnCell.ColumnIndex], p.log);

            // verify second random cell is selected
            scenRes.IncCounters(secondRandomColumnCell.Selected, "Unable to select Cell " + secondRandomColumnCell, p.log);

            // verify first and second cell are still selected
            scenRes.IncCounters(firstRandomColumnCell.Selected && secondRandomColumnCell.Selected, "Unable to simultaneoulsy select cells: " + firstRandomColumnCell + " and " + secondRandomColumnCell, p.log);

            // verify each cell in the first two selected columns are selected
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[j].Cells[firstRandomColumnCell.ColumnIndex].Selected, "Unable to select cell " + dataGridView.Rows[j].Cells[firstRandomColumnCell.ColumnIndex], p.log);
                scenRes.IncCounters(dataGridView.Rows[j].Cells[secondRandomColumnCell.ColumnIndex].Selected, "Unable to select cell " + dataGridView.Rows[j].Cells[secondRandomColumnCell.ColumnIndex], p.log);
                if (!dataGridView.Rows[j].Cells[firstRandomColumnCell.ColumnIndex].Selected || !dataGridView.Rows[j].Cells[secondRandomColumnCell.ColumnIndex].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "When a cell in a column is selected and DataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect, this should result in all of the cells in the column being selected, which it currently does not.");
                }
            }

            // select third random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);

            DataGridViewCell thirdRandomColumnCell = dataGridView.Rows[randomRowIndex].Cells[randomColStartIndex + 2];

            thirdRandomColumnCell.Selected = true;

            // verify third column is selected
            scenRes.IncCounters(dataGridView.Columns[thirdRandomColumnCell.ColumnIndex].Selected, "Unable to select column " + thirdRandomColumnCell, p.log);

            // verify third random cell is selected
            scenRes.IncCounters(thirdRandomColumnCell.Selected, "Unable to select Cell " + thirdRandomColumnCell, p.log);

            // verify first, second and third random cells are selected at same time
            scenRes.IncCounters(firstRandomColumnCell.Selected && secondRandomColumnCell.Selected && thirdRandomColumnCell.Selected, "Unable to simultaneoulsy select cells: " + firstRandomColumnCell + " and " + secondRandomColumnCell + " and " + thirdRandomColumnCell, p.log);

            // select fourth random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);

            DataGridViewCell fourthRandomColumnCell = dataGridView.Rows[randomRowIndex].Cells[randomColStartIndex + 3];

            fourthRandomColumnCell.Selected = true;

            // verify fourth column is selected
            scenRes.IncCounters(dataGridView.Columns[fourthRandomColumnCell.ColumnIndex].Selected, "Unable to select column " + fourthRandomColumnCell, p.log);

            // verify fourth random cell is selected
            scenRes.IncCounters(fourthRandomColumnCell.Selected, "Unable to select Cell " + fourthRandomColumnCell, p.log);

            // verify first, second, third, and fourth random cells are selected at same time
            scenRes.IncCounters(firstRandomColumnCell.Selected && secondRandomColumnCell.Selected && thirdRandomColumnCell.Selected && fourthRandomColumnCell.Selected, "Unable to simultaneoulsy select cells: " + firstRandomColumnCell + " and " + secondRandomColumnCell + " and " + thirdRandomColumnCell + " and " + fourthRandomColumnCell, p.log);

            // verify that all cells in the first, second and third selected columns are selected
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                scenRes.IncCounters(dataGridView.Rows[i].Cells[firstRandomColumnCell.ColumnIndex].Selected, "Unable to select cell " + dataGridView.Rows[i].Cells[firstRandomColumnCell.ColumnIndex], p.log);
                scenRes.IncCounters(dataGridView.Rows[i].Cells[secondRandomColumnCell.ColumnIndex].Selected, "Unable to select cell " + dataGridView.Rows[i].Cells[secondRandomColumnCell.ColumnIndex], p.log);
                scenRes.IncCounters(dataGridView.Rows[i].Cells[thirdRandomColumnCell.ColumnIndex].Selected, "Unable to select cell " + dataGridView.Rows[i].Cells[thirdRandomColumnCell.ColumnIndex], p.log);
                scenRes.IncCounters(dataGridView.Rows[i].Cells[fourthRandomColumnCell.ColumnIndex].Selected, "Unable to select cell " + dataGridView.Rows[i].Cells[fourthRandomColumnCell.ColumnIndex], p.log);
                if (!dataGridView.Rows[i].Cells[firstRandomColumnCell.ColumnIndex].Selected || !dataGridView.Rows[i].Cells[secondRandomColumnCell.ColumnIndex].Selected || !dataGridView.Rows[i].Cells[thirdRandomColumnCell.ColumnIndex].Selected || !dataGridView.Rows[i].Cells[fourthRandomColumnCell.ColumnIndex].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "When a cell in a column is selected and DataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect, this should result in all of the cells in the column being selected, which it currently does not.");
                }
            }

            // verify that all cells NOT in the first, second, third, and fourth selected columns are NOT selected
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    if (j != firstRandomColumnCell.ColumnIndex && j != secondRandomColumnCell.ColumnIndex && j != thirdRandomColumnCell.ColumnIndex && j != fourthRandomColumnCell.ColumnIndex)
                    {
                        scenRes.IncCounters(!dataGridView.Rows[i].Cells[j].Selected, "Cell " + dataGridView.Rows[i].Cells[j] + " should NOT have been selected.", p.log);
                    }
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionFullColumnSelect_TopLeftHeaderCellSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full column select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;
            try
            {
                // Should not be able to access the Selected property of 
                // a DataGridView's TopLeftHeaderCell
                // select the top left header cell
                dataGridView.TopLeftHeaderCell.Selected = true;
                scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 81894, "No exception when setting Selected on TopLeftHeaderCell");
            }
            catch (System.InvalidOperationException e)
            {
                p.log.WriteLine("Exception: " + e.Message);
                scenRes.IncCounters(true);
            }
            catch (Exception e1)
            {
                p.log.WriteLine("Unexpected Exception: " + e1);
                scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 81894, "Unexpected exception when setting Selected on TopLeftHeaderCell: " + e1.Message);
            }
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionRowHeaderSelect_SingleCellSelection(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing row header select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;

            // obtain a random cell
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            // Select a single cell
            DataGridViewCell singleCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            singleCell.Selected = true;

            // verify that this cell is selected
            scenRes.IncCounters(singleCell.Selected, "Cell (" + singleCell.RowIndex + "," + singleCell.ColumnIndex + ") was NOT selected, as expected.", p.log);

            dataGridView.Rows[0].Cells[0].Selected = false;
            // verify that this is the only cell selected in the DataGridView
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                for (int k = 0; k < dataGridView.Columns.Count; k++)
                {
                    // all cells not equal to the current cell should not be selected
                    if (j != singleCell.RowIndex && k != singleCell.ColumnIndex)
                    {
                        scenRes.IncCounters(!dataGridView.Rows[j].Cells[k].Selected, "Cell (" + j + "," + k + ") should NOT be selected.", p.log);
                    }
                }
            }

            // un-select the cell
            singleCell.Selected = false;

            // verfiy the cell is unselected
            scenRes.IncCounters(!singleCell.Selected, "Unable to un-select cell: " + singleCell, p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionRowHeaderSelect_TwoCellBoundaryMultipleSelection(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing row header select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;

            // obtain a random cell
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            // test selection of two cells
            // select the first cell
            DataGridViewCell firstCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            firstCell.Selected = true;

            // verify the first cell is selected
            scenRes.IncCounters(firstCell.Selected, "Cell (" + firstCell.RowIndex + "," + firstCell.ColumnIndex + ") is NOT selected, as expected.", p.log);
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell secondCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            secondCell.Selected = true;

            // verify that the second cell is selected
            scenRes.IncCounters(secondCell.Selected, "Cell (" + secondCell.RowIndex + "," + secondCell.ColumnIndex + ") is NOT selected, as expected.", p.log);

            // verify that the first cell is still selected
            scenRes.IncCounters(firstCell.Selected, "Cell (" + firstCell.RowIndex + "," + firstCell.ColumnIndex + ") is NOT selected, as expected.", p.log);

            // verfity that the first cell is not the current cell
            scenRes.IncCounters(!firstCell.Equals(dataGridView.CurrentCell), "Cell: " + firstCell + " should NOT be set to the current cell: " + dataGridView.CurrentCell, p.log);

            dataGridView.Rows[0].Cells[0].Selected = false;
            // verify that no other cells in the DataGridView are selected
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                for (int k = 0; k < dataGridView.Columns.Count; k++)
                {
                    if ((j != firstCell.RowIndex && k != firstCell.ColumnIndex) && (j != secondCell.RowIndex && k != secondCell.ColumnIndex))
                    {
                        // this cell should not be selected
                        scenRes.IncCounters(!dataGridView.Rows[j].Cells[k].Selected, "Cell (" + j + "," + k + ") should NOT be selected.", p.log);
                    }
                }
            }

            // un-select the first cell
            firstCell.Selected = false;

            // verify that the first cell is unselected
            scenRes.IncCounters(!firstCell.Selected, "Cell (" + firstCell.RowIndex + "," + firstCell.ColumnIndex + ") should NOT be selected.", p.log);

            // un-select the second cell
            secondCell.Selected = false;

            // verify that the second cell is unselected
            scenRes.IncCounters(!secondCell.Selected, "Cell (" + secondCell.RowIndex + "," + secondCell.ColumnIndex + ") should NOT be selected.", p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionRowHeaderSelect_ThreeCellRandomMultipleSelection(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing row header select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;

            // choose three cells randomly and select them at the same time
            int randomRowIndex = 0;
            int randomColIndex = 0;

            // select first random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell firstRandomCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            firstRandomCell.Selected = true;

            // verify first random cell is selected
            scenRes.IncCounters(firstRandomCell.Selected, "Unable to select Cell (" + randomRowIndex + "," + randomColIndex + ").", p.log);

            // select second random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell secondRandomCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            secondRandomCell.Selected = true;

            // verify second random cell is selected
            scenRes.IncCounters(secondRandomCell.Selected, "Unable to select Cell (" + randomRowIndex + "," + randomColIndex + ").", p.log);

            // verify first and second cell are still selected
            scenRes.IncCounters(firstRandomCell.Selected && secondRandomCell.Selected, "Unable to simultaneoulsy select cells: " + firstRandomCell + " and " + secondRandomCell, p.log);

            // select third random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell thirdRandomCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            thirdRandomCell.Selected = true;

            // verify third random cell is selected
            scenRes.IncCounters(thirdRandomCell.Selected, "Unable to select Cell (" + randomRowIndex + "," + randomColIndex + ").", p.log);

            // verify first, second and third random cells are selected at same time
            scenRes.IncCounters(firstRandomCell.Selected && secondRandomCell.Selected && thirdRandomCell.Selected, "Unable to simultaneoulsy select cells: " + firstRandomCell + " and " + secondRandomCell + " and " + thirdRandomCell, p.log);

            dataGridView.Rows[0].Cells[0].Selected = false;
            // verify that all other cells in the DataGridView are not selected
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    if ((i != firstRandomCell.RowIndex && j != firstRandomCell.ColumnIndex) && (i != secondRandomCell.RowIndex && j != secondRandomCell.ColumnIndex) && (i != thirdRandomCell.RowIndex && j != secondRandomCell.ColumnIndex))
                    {
                        scenRes.IncCounters(!dataGridView.Rows[i].Cells[j].Selected, "Cell (" + i + "," + j + ") should NOT be selected.", p.log);
                    }
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionRowHeaderSelect_FourCellContinuousMultipleSelection(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing row header select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;

            // randomly choose whether we are going to select cells continuously across a row or a column
            bool doRowSelect = false;

            doRowSelect = p.ru.GetBoolean();

            int randomRowIndex = 0;
            int randomColIndex = 0;
            int randomRowStartIndex = 0;
            int randomColStartIndex = 0;
            DataGridViewCell firstCell = null;
            DataGridViewCell secondCell = null;
            DataGridViewCell thirdCell = null;
            DataGridViewCell fourthCell = null;

            if (doRowSelect)
            {
                // randomly select a row
                randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);

                // randomly select a starting point, carefull not to fall off end of DataGridView 
                // when chosing starting point
                randomColStartIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 5);

                // select first cell
                firstCell = dataGridView.Rows[randomRowIndex].Cells[randomColStartIndex];
                firstCell.Selected = true;

                // verify first cell is selected
                scenRes.IncCounters(firstCell.Selected, "Unable to select cell: " + firstCell, p.log);

                // select second cell
                secondCell = dataGridView.Rows[randomRowIndex].Cells[randomColStartIndex + 1];
                secondCell.Selected = true;

                // verify second cell is selected
                scenRes.IncCounters(secondCell.Selected, "Unable to select cell: " + secondCell, p.log);

                // verify first and second cells are selected at same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell, p.log);

                // select third cell
                thirdCell = dataGridView.Rows[randomRowIndex].Cells[randomColStartIndex + 2];
                thirdCell.Selected = true;

                // verify third cell is selected
                scenRes.IncCounters(thirdCell.Selected, "Unable to select cell: " + thirdCell, p.log);

                // verify first, second, and third cells are selected at the same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected && thirdCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell + " and " + thirdCell, p.log);

                // select fourth cell
                fourthCell = dataGridView.Rows[randomRowIndex].Cells[randomColStartIndex + 3];
                fourthCell.Selected = true;

                // verify fourth cell is selected
                scenRes.IncCounters(fourthCell.Selected, "Unable to select cell: " + fourthCell, p.log);

                // verify first, second, third and fourth cells are selected at the same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected && thirdCell.Selected && fourthCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell + " and " + thirdCell + " and " + fourthCell, p.log);
            }
            else
            {
                // randomly select a column
                randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

                // randomly select a starting point, carefull not to fall off end of DataGridView
                // when chosing starting point
                randomRowStartIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 5);

                // select first cell
                firstCell = dataGridView.Rows[randomRowStartIndex].Cells[randomColIndex];
                firstCell.Selected = true;

                // verify first cell is selected
                scenRes.IncCounters(firstCell.Selected, "Unable to select cell: " + firstCell, p.log);

                // select second cell
                secondCell = dataGridView.Rows[randomRowStartIndex + 1].Cells[randomColIndex];
                secondCell.Selected = true;

                // verify second cell is selected
                scenRes.IncCounters(secondCell.Selected, "Unable to select cell: " + secondCell, p.log);

                // verify first and second cells are selected at same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell, p.log);

                // select third cell
                thirdCell = dataGridView.Rows[randomRowStartIndex + 2].Cells[randomColIndex];
                thirdCell.Selected = true;

                // verify third cell is selected
                scenRes.IncCounters(thirdCell.Selected, "Unable to select cell: " + thirdCell, p.log);

                // verify first, second, and third cells are selected at the same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected && thirdCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell + " and " + thirdCell, p.log);

                // select fourth cell
                fourthCell = dataGridView.Rows[randomRowStartIndex + 3].Cells[randomColIndex];
                fourthCell.Selected = true;

                // verify fourth cell is selected
                scenRes.IncCounters(fourthCell.Selected, "Unable to select cell: " + fourthCell, p.log);

                // verify first, second, third and fourth cells are selected at the same time
                scenRes.IncCounters(firstCell.Selected && secondCell.Selected && thirdCell.Selected && fourthCell.Selected, "Unable to simultaneously select cells: " + firstCell + " and " + secondCell + " and " + thirdCell + " and " + fourthCell, p.log);
            }

            dataGridView.Rows[0].Cells[0].Selected = false;
            // verify that only these four cells are selected at the same time
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    if ((i != firstCell.RowIndex && j != firstCell.ColumnIndex) && (i != secondCell.RowIndex && j != secondCell.ColumnIndex) && (i != thirdCell.RowIndex && j != thirdCell.ColumnIndex) && (i != fourthCell.RowIndex && j != fourthCell.ColumnIndex))
                    {
                        // cell should not be selected
                        scenRes.IncCounters(!dataGridView.Rows[i].Cells[j].Selected, "Cell " + dataGridView.Rows[i].Cells[j] + " should NOT be selected.", p.log);
                    }
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionRowHeaderSelect_TopLeftHeaderCellSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing row header select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.RowHeaderSelect;
            try
            {
                // Should not be able to access the Selected property of 
                // a DataGridView's TopLeftHeaderCell
                // select the top left header cell
                dataGridView.TopLeftHeaderCell.Selected = true;
                scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 81894, "No exception when setting Selected on TopLeftHeaderCell");
            }
            catch (System.InvalidOperationException e)
            {
                p.log.WriteLine("Exception: " + e.Message);
                scenRes.IncCounters(true);
            }
            catch (Exception e1)
            {
                scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 81894, "Unexpected exception when setting Selected on TopLeftHeaderCell:" + e1.Message);
            }
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionFullRowSelect_SingleCellSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full row select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // obtain a random cell
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            DataGridViewCell currentCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            scenRes.IncCounters(currentCell.Equals(dataGridView.Rows[randomRowIndex].Cells[randomColIndex]), "Value of the current cell is " + currentCell + ", which is not its assigned value of " + dataGridView.Rows[randomRowIndex].Cells[randomColIndex], p.log);
            currentCell.Selected = true;

            // verify that the entire row containing this cell is selected
            scenRes.IncCounters(dataGridView.Rows[randomRowIndex].Selected, "Unable to Select Row " + randomRowIndex + " which contains selected cell (" + currentCell.RowIndex + "," + currentCell.ColumnIndex + ")", p.log);
            if (!dataGridView.Rows[randomRowIndex].Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Selection of a cell within a row should result in row.Selected = true, but it does not currently.");
            }

            // verify that each cell in the row is selected
            for (int j = 0; j < dataGridView.Columns.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[currentCell.RowIndex].Cells[j].Selected, "Cell (" + currentCell.RowIndex + "," + j + ") is not selected.", p.log);
                if (!dataGridView.Rows[currentCell.RowIndex].Cells[j].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Selection of a cell within a row should result in all of the cells in the row being selected, but it does not currently.");
                }
            }

            // verify that no other row is selected
            for (int k = 0; k < dataGridView.Rows.Count; k++)
            {
                // if we are not looking at the row we just selected
                if (k != currentCell.RowIndex)
                {
                    scenRes.IncCounters(!dataGridView.Rows[k].Selected, "Row " + k + " is selected when only Row " + currentCell.RowIndex + " should be.", p.log);
                }
            }

            // Unselect the row
            dataGridView.Rows[randomRowIndex].Selected = false;

            // verify that the column was unselected
            scenRes.IncCounters(!dataGridView.Rows[randomRowIndex].Selected, "Unable to DE-select row: " + randomRowIndex, p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionFullRowSelect_SingleRowSelected(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full row select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            DataGridViewCell origCurrent = dataGridView.CurrentCell;

            if (origCurrent == null)
                p.log.WriteLine("initially current cell is null");
            else
                p.log.WriteLine("initial current cell: " + dataGridView.CurrentCellAddress);


            // obtain a random row
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            DataGridViewRow currentRow = dataGridView.Rows[randomRowIndex];

            currentRow.Selected = true;

            // verify that the DataGridView's current cell is preserved after selecting a row
            scenRes.IncCounters(dataGridView.CurrentCell.Equals(origCurrent), "Failed: to preserve CurrentCell after selecting row " + randomRowIndex, p.log);

            // verify row is selected
            scenRes.IncCounters(currentRow.Selected, "Unable to Select Row " + currentRow.Index, p.log);

            // verify that each cell in the row is selected
            for (int j = 0; j < dataGridView.Columns.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[currentRow.Index].Cells[j].Selected, "Cell (" + currentRow.Index + "," + j + ") is not selected.", p.log);
                if (!dataGridView.Rows[currentRow.Index].Cells[j].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "After a row is selected, each cell in the row should be selected as well when the selection mode is FullRowSelect.  This is not currently the case.");
                }
            }

            // verify that no other row is selected
            for (int k = 0; k < dataGridView.Rows.Count; k++)
            {
                // if we are not looking at the row we just selected
                if (k != currentRow.Index)
                {
                    scenRes.IncCounters(!dataGridView.Rows[k].Selected, "Row " + k + " is selected when only row " + currentRow.Index + " should be.", p.log);
                }
            }

            // Unselect the row
            currentRow.Selected = false;

            // verify that the row was unselected
            scenRes.IncCounters(!currentRow.Selected, "Unable to DE-select row: " + currentRow.Index, p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionFullRowSelect_TwoRowBoundaryMultipleSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full row select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            int randomRowIndex = 0;
            int randomColIndex = 0;
            DataGridViewCell firstRowCell = null;
            DataGridViewCell secondRowCell = null;

            // select the first column by random cell
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            firstRowCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];
            firstRowCell.Selected = true;

            // verify the first row is selected
            scenRes.IncCounters(dataGridView.Rows[randomRowIndex].Selected, "Unable to select Row: " + firstRowCell.RowIndex, p.log);
            if (!dataGridView.Rows[randomRowIndex].Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Should be able to select a row when one of its cells is select and the selection mode is FullRowSelect.  This functionality is not currently working.");
            }

            // verify the random cell is selected
            scenRes.IncCounters(firstRowCell.Selected, "Unable to select cell " + firstRowCell, p.log);

            // select the second Row by random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            secondRowCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];
            secondRowCell.Selected = true;

            // verify the second row is selected
            scenRes.IncCounters(dataGridView.Rows[randomRowIndex].Selected, "Unable to select Row: " + secondRowCell.RowIndex, p.log);
            if (!dataGridView.Rows[randomRowIndex].Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Should be able to select a row when one of its cells is select and the selection mode is FullRowSelect.  This functionality is not currently working.");
            }

            // verify the random cell is selected
            scenRes.IncCounters(secondRowCell.Selected, "Unable to select cell " + secondRowCell, p.log);

            // verify that both the first and second Rows are selected
            scenRes.IncCounters(dataGridView.Rows[firstRowCell.RowIndex].Selected && dataGridView.Rows[secondRowCell.RowIndex].Selected, "Unable to simultaneously select two Rows: " + dataGridView.Rows[firstRowCell.RowIndex] + " and " + dataGridView.Rows[secondRowCell.RowIndex], p.log);

            // verify that every cell in both Rows is selected
            for (int j = 0; j < dataGridView.Columns.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[firstRowCell.RowIndex].Cells[j].Selected, "Unable to select cell " + dataGridView.Rows[firstRowCell.RowIndex].Cells[j], p.log);
                scenRes.IncCounters(dataGridView.Rows[secondRowCell.RowIndex].Cells[j].Selected, "Unable to select cell " + dataGridView.Rows[secondRowCell.RowIndex].Cells[j], p.log);
                if (!dataGridView.Rows[firstRowCell.RowIndex].Cells[j].Selected || !dataGridView.Rows[secondRowCell.RowIndex].Cells[j].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "All of the cells in a row should be selected when its row is selected and the selection mode is FullRowSelect.  This functionality is not currently working.");
                }
            }

            // de-select both Rows
            dataGridView.Rows[firstRowCell.RowIndex].Selected = false;
            dataGridView.Rows[secondRowCell.RowIndex].Selected = false;

            // verify that both Rows are no longer selected
            scenRes.IncCounters(!dataGridView.Rows[firstRowCell.RowIndex].Selected, "Row " + firstRowCell.RowIndex + " was not de selected.", p.log);
            scenRes.IncCounters(!dataGridView.Rows[secondRowCell.RowIndex].Selected, "Row " + secondRowCell.RowIndex + " was not de selected.", p.log);

            // verify that all cells in both Rows are no longer selected
            for (int j = 0; j < dataGridView.Columns.Count; j++)
            {
                scenRes.IncCounters(!dataGridView.Rows[firstRowCell.RowIndex].Cells[j].Selected, "Cell " + dataGridView.Rows[firstRowCell.RowIndex].Cells[j] + " was not de selected.", p.log);
                scenRes.IncCounters(!dataGridView.Rows[secondRowCell.RowIndex].Cells[j].Selected, "Cell " + dataGridView.Rows[secondRowCell.RowIndex].Cells[j] + " was not de selected.", p.log);
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionFullRowSelect_ThreeRowRandomMultipleSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full row select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // choose three cells randomly and select them at the same time
            int randomRowIndex = 0;
            int randomColIndex = 0;

            // select first random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell firstRandomRowCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            firstRandomRowCell.Selected = true;

            // verify the row is selected
            scenRes.IncCounters(dataGridView.Rows[firstRandomRowCell.RowIndex].Selected, "Unable to select row " + firstRandomRowCell.RowIndex, p.log);
            if (!dataGridView.Rows[firstRandomRowCell.RowIndex].Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Should be able to select a row when one of its cells is select and the selection mode is FullRowSelect.  This functionality is not currently working.");
            }

            // verify first random cell is selected
            scenRes.IncCounters(firstRandomRowCell.Selected, "Unable to select Cell " + firstRandomRowCell, p.log);

            // verify each cell in the first selected row is selected
            for (int j = 0; j < dataGridView.Columns.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[j].Selected, "Unable to select cell " + dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[j], p.log);
                if (!dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[j].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "All of the cells in a row should be selected when its row is selected and the selection mode is FullRowSelect.  This functionality is not currently working.");
                }
            }

            // select second random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell secondRandomRowCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            secondRandomRowCell.Selected = true;

            // verify that second row is selected
            scenRes.IncCounters(dataGridView.Rows[secondRandomRowCell.RowIndex].Selected, "Unable to select row " + secondRandomRowCell.RowIndex, p.log);
            if (!dataGridView.Rows[secondRandomRowCell.RowIndex].Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Should be able to select a row when one of its cells is select and the selection mode is FullRowSelect.  This functionality is not currently working.");
            }

            // verify second random cell is selected
            scenRes.IncCounters(secondRandomRowCell.Selected, "Unable to select Cell " + secondRandomRowCell, p.log);

            // verify first and second cell are still selected
            scenRes.IncCounters(firstRandomRowCell.Selected && secondRandomRowCell.Selected, "Unable to simultaneoulsy select cells: " + firstRandomRowCell + " and " + secondRandomRowCell, p.log);

            // verify each cell in the first two selected rows are selected
            for (int j = 0; j < dataGridView.Columns.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[j].Selected, "Unable to select cell " + dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[j], p.log);
                scenRes.IncCounters(dataGridView.Rows[secondRandomRowCell.RowIndex].Cells[j].Selected, "Unable to select cell " + dataGridView.Rows[secondRandomRowCell.RowIndex].Cells[j], p.log);
                if (!dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[j].Selected || !dataGridView.Rows[secondRandomRowCell.RowIndex].Cells[j].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "All of the cells in a row should be selected when its row is selected and the selection mode is FullRowSelect.  This functionality is not currently working.");
                }
            }

            // select third random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell thirdRandomRowCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            thirdRandomRowCell.Selected = true;

            // verify third row is selected
            scenRes.IncCounters(dataGridView.Rows[thirdRandomRowCell.RowIndex].Selected, "Unable to select Row " + thirdRandomRowCell.RowIndex, p.log);

            // verify third random cell is selected
            scenRes.IncCounters(thirdRandomRowCell.Selected, "Unable to select Cell " + thirdRandomRowCell, p.log);

            // verify first, second and third random cells are selected at same time
            scenRes.IncCounters(firstRandomRowCell.Selected && secondRandomRowCell.Selected && thirdRandomRowCell.Selected, "Unable to simultaneoulsy select cells: " + firstRandomRowCell + " and " + secondRandomRowCell + " and " + thirdRandomRowCell, p.log);

            // verify that all cells in the first, second and third selected columns are selected
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                scenRes.IncCounters(dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[i].Selected, "Unable to select cell " + dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[i], p.log);
                scenRes.IncCounters(dataGridView.Rows[secondRandomRowCell.RowIndex].Cells[i].Selected, "Unable to select cell " + dataGridView.Rows[secondRandomRowCell.RowIndex].Cells[i], p.log);
                scenRes.IncCounters(dataGridView.Rows[thirdRandomRowCell.RowIndex].Cells[i].Selected, "Unable to select cell " + dataGridView.Rows[thirdRandomRowCell.RowIndex].Cells[i], p.log);
                if (!dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[i].Selected || !dataGridView.Rows[secondRandomRowCell.RowIndex].Cells[i].Selected || !dataGridView.Rows[thirdRandomRowCell.RowIndex].Cells[i].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "All of the cells in a row should be selected when its row is selected and the selection mode is FullRowSelect.  This functionality is not currently working.");
                }
            }

            // verify that all cells NOT in the first, second and third selected rows are NOT selected
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    if (i != firstRandomRowCell.RowIndex && i != secondRandomRowCell.RowIndex && i != thirdRandomRowCell.RowIndex)
                    {
                        scenRes.IncCounters(!dataGridView.Rows[i].Cells[j].Selected, "Cell " + dataGridView.Rows[i].Cells[j] + " should NOT have been selected.", p.log);
                    }
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionFullRowSelect_FourRowContinuousRandomMultipleSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full row select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // randomly select a row to start the continuous selection from,
            // carful not to fall off the end of the DataGridView
            int randomRowStartIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 5);

            // random col index we will use for the cell we will select the row with
            int randomColIndex = 0;

            // select first random cell
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 4);

            DataGridViewCell firstRandomRowCell = dataGridView.Rows[randomRowStartIndex].Cells[randomColIndex];

            Console.WriteLine("FirstRandomRowCell row # " + firstRandomRowCell.RowIndex);
            firstRandomRowCell.Selected = true;
            if (!dataGridView.Rows[firstRandomRowCell.RowIndex].Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Should be able to select a row when one of its cells is select and the selection mode is FullRowSelect.  This functionality is not currently working.");
            }

            // verify the row is selected
            scenRes.IncCounters(dataGridView.Rows[firstRandomRowCell.RowIndex].Selected, "Unable to select row " + firstRandomRowCell.RowIndex, p.log);

            // verify first random cell is selected
            scenRes.IncCounters(firstRandomRowCell.Selected, "Unable to select first Cell " + firstRandomRowCell, p.log);

            // verify each cell in the first selected row is selected
            for (int j = 0; j < dataGridView.Columns.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[j].Selected, "Unable to select cell " + dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[j], p.log);
                if (!dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[j].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "All of the cells in a row should be selected when its row is selected and the selection mode is FullRowSelect.  This functionality is not currently working.");
                }
            }

            // select second random cell
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 4);

            DataGridViewCell secondRandomRowCell = dataGridView.Rows[randomRowStartIndex + 1].Cells[randomColIndex + 1];

            Console.WriteLine("SecondRandomRowCell row # " + secondRandomRowCell.RowIndex);
            secondRandomRowCell.Selected = true;
            if (!dataGridView.Rows[secondRandomRowCell.RowIndex].Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Should be able to select a row when one of its cells is select and the selection mode is FullRowSelect.  This functionality is not currently working.");
            }

            // verify that second row is selected
            scenRes.IncCounters(dataGridView.Rows[secondRandomRowCell.RowIndex].Selected, "Unable to select row " + secondRandomRowCell.RowIndex, p.log);

            // verify second random cell is selected
            scenRes.IncCounters(secondRandomRowCell.Selected, "Unable to select second Cell " + secondRandomRowCell, p.log);

            // verify first and second cell are still selected
            scenRes.IncCounters(firstRandomRowCell.Selected && secondRandomRowCell.Selected, "Unable to simultaneoulsy select cells: " + firstRandomRowCell + " and " + secondRandomRowCell, p.log);

            // verify each cell in the first two selected rows are selected
            for (int j = 0; j < dataGridView.Columns.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[j].Selected, "Unable to select cell " + dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[j], p.log);
                scenRes.IncCounters(dataGridView.Rows[secondRandomRowCell.RowIndex].Cells[j].Selected, "Unable to select cell " + dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[j], p.log);
                if (!dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[j].Selected || !dataGridView.Rows[secondRandomRowCell.RowIndex].Cells[j].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "All of the cells in a row should be selected when its row is selected and the selection mode is FullRowSelect.  This functionality is not currently working.");
                }
            }

            // select third random cell
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 4);

            DataGridViewCell thirdRandomRowCell = dataGridView.Rows[randomRowStartIndex + 2].Cells[randomColIndex + 2];

            Console.WriteLine("ThirdRandomRowCell row # " + thirdRandomRowCell.RowIndex);
            thirdRandomRowCell.Selected = true;
            if (!dataGridView.Rows[thirdRandomRowCell.RowIndex].Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Should be able to select a row when one of its cells is select and the selection mode is FullRowSelect.  This functionality is not currently working.");
            }

            // verify third Row is selected
            scenRes.IncCounters(dataGridView.Rows[thirdRandomRowCell.RowIndex].Selected, "Unable to select Row " + thirdRandomRowCell.RowIndex, p.log);

            // verify third random cell is selected
            scenRes.IncCounters(thirdRandomRowCell.Selected, "Unable to select third Cell " + thirdRandomRowCell, p.log);

            // verify first, second and third random cells are selected at same time
            scenRes.IncCounters(firstRandomRowCell.Selected && secondRandomRowCell.Selected && thirdRandomRowCell.Selected, "Unable to simultaneoulsy select cells: " + firstRandomRowCell + " and " + secondRandomRowCell + " and " + thirdRandomRowCell, p.log);

            // verify that all cells in the first, second, and third selected rows are selected
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                scenRes.IncCounters(dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[i].Selected, "Unable to select cell " + dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[i], p.log);
                scenRes.IncCounters(dataGridView.Rows[secondRandomRowCell.RowIndex].Cells[i].Selected, "Unable to select cell " + dataGridView.Rows[secondRandomRowCell.RowIndex].Cells[i], p.log);
                scenRes.IncCounters(dataGridView.Rows[thirdRandomRowCell.RowIndex].Cells[i].Selected, "Unable to select cell " + dataGridView.Rows[thirdRandomRowCell.RowIndex].Cells[i], p.log);
                if (!dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[i].Selected || !dataGridView.Rows[secondRandomRowCell.RowIndex].Cells[i].Selected || !dataGridView.Rows[thirdRandomRowCell.RowIndex].Cells[i].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "All of the cells in a row should be selected when its row is selected and the selection mode is FullRowSelect.  This functionality is not currently working.");
                }
            }

            // select fourth random cell
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 4);

            DataGridViewCell fourthRandomRowCell = dataGridView.Rows[randomRowStartIndex + 3].Cells[randomColIndex + 3];

            Console.WriteLine("FourthRandomRowCell row # " + fourthRandomRowCell.RowIndex);
            fourthRandomRowCell.Selected = true;
            if (!dataGridView.Rows[fourthRandomRowCell.RowIndex].Selected)
            {
                p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "Should be able to select a row when one of its cells is select and the selection mode is FullRowSelect.  This functionality is not currently working.");
            }

            // verify fourth Row is selected
            scenRes.IncCounters(dataGridView.Rows[fourthRandomRowCell.RowIndex].Selected, "Unable to select Row " + fourthRandomRowCell.RowIndex, p.log);

            // verify fourth random cell is selected
            scenRes.IncCounters(fourthRandomRowCell.Selected, "Unable to select fourth Cell " + fourthRandomRowCell, p.log);

            // verify first, second, third, and fourth random cells are selected at same time
            scenRes.IncCounters(firstRandomRowCell.Selected && secondRandomRowCell.Selected && thirdRandomRowCell.Selected && fourthRandomRowCell.Selected, "Unable to simultaneoulsy select cells: " + firstRandomRowCell + " and " + secondRandomRowCell + " and " + thirdRandomRowCell + " and " + fourthRandomRowCell, p.log);

            // verify that all cells in the first, second, third, and fourth selected rows are selected
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                scenRes.IncCounters(dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[i].Selected, "Unable to select cell " + dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[i], p.log);
                scenRes.IncCounters(dataGridView.Rows[secondRandomRowCell.RowIndex].Cells[i].Selected, "Unable to select cell " + dataGridView.Rows[secondRandomRowCell.RowIndex].Cells[i], p.log);
                scenRes.IncCounters(dataGridView.Rows[thirdRandomRowCell.RowIndex].Cells[i].Selected, "Unable to select cell " + dataGridView.Rows[thirdRandomRowCell.RowIndex].Cells[i], p.log);
                scenRes.IncCounters(dataGridView.Rows[fourthRandomRowCell.RowIndex].Cells[i].Selected, "Unable to select cell " + dataGridView.Rows[fourthRandomRowCell.RowIndex].Cells[i], p.log);
                if (!dataGridView.Rows[firstRandomRowCell.RowIndex].Cells[i].Selected || !dataGridView.Rows[secondRandomRowCell.RowIndex].Cells[i].Selected || !dataGridView.Rows[thirdRandomRowCell.RowIndex].Cells[i].Selected || !dataGridView.Rows[fourthRandomRowCell.RowIndex].Cells[i].Selected)
                {
                    p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81827, "All of the cells in a row should be selected when its row is selected and the selection mode is FullRowSelect.  This functionality is not currently working.");
                }
            }

            // verify that all cells NOT in the first, second, third, and fourth selected rows are NOT selected
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    if (i != firstRandomRowCell.RowIndex && i != secondRandomRowCell.RowIndex && i != thirdRandomRowCell.RowIndex && i != fourthRandomRowCell.RowIndex)
                    {
                        scenRes.IncCounters(!dataGridView.Rows[i].Cells[j].Selected, "Cell " + dataGridView.Rows[i].Cells[j] + " should NOT have been selected.", p.log);
                    }
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult MultipleSelectionFullRowSelect_TopLeftHeaderCellSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // set static contexts so we are testing full row select
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            try
            {
                // Should not be able to access the Selected property of 
                // a DataGridView's TopLeftHeaderCell
                // select the top left header cell
                dataGridView.TopLeftHeaderCell.Selected = true;
                scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 81894, "No exception when setting Selected on TopLeftHeaderCell");
            }
            catch (System.InvalidOperationException e)
            {
                p.log.WriteLine("Exception: " + e.Message);
                scenRes.IncCounters(true);
            }
            catch (Exception e1)
            {
                scenRes.IncCounters(false, p.log, BugDb.VSWhidbey, 81894, "Unexpected exception when setting Selected on TopLeftHeaderCell: " + e1.Message);
            }
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult AreAllCellsSelected_SelectAllVisibleCells(TParams p)
        {
            p.log.WriteLine(dataGridView.Rows.Count + " rows , " + dataGridView.Columns.Count + " columns.");
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a DataGridView with all visible cells
            ResetTest(DataGridViewUtils.GetVisibleDataGridView(true));
            dataGridView.MultiSelect = true;

            string selectionMethod = "DataGridView.MultiSelect = " + dataGridView.MultiSelect + " and DataGridView.SelectionMode = " + dataGridView.SelectionMode;

            // call the selectAll method
            dataGridView.SelectAll();

            // verify that all cells in the DataGridView are selected
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    scenRes.IncCounters(dataGridView.Rows[i].Cells[j].Selected, "Cell " + dataGridView.Rows[i].Cells[j].Selected + " was NOT selected, when " + selectionMethod, p.log);
                }
            }

            // Call AreAllCellsSelected with true parameter
            bool allSelected = dataGridView.AreAllCellsSelected(true);

            // Verify that the call returned true, as expected
            scenRes.IncCounters(allSelected, "AreAllCellsSelected returned: " + allSelected + " when it should have returned true, when " + selectionMethod, p.log);

            // Call AreAllCellsSelected with false parameter
            allSelected = dataGridView.AreAllCellsSelected(false);

            // Verify that the call returned true, as expected
            scenRes.IncCounters(allSelected, "AreAllCellsSelected returned: " + allSelected + " when it should have returned true, when " + selectionMethod, p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult AreAllCellsSelected_SelectSomeVisibleCells(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetVisibleDataGridView(true));
            dataGridView.MultiSelect = true;

            string selectionMethod = "";
            int rowIndex = 0;

            // select some of the cells by individual cell selection
            selectionMethod = "Individual cell selection used to select some of the cells.";

            // select some of the visible cells
            for (int i = 0; i < dataGridView.Rows.Count / 1000; i++)
            {
                rowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
                for (int j = 0; j < dataGridView.Columns.Count / 10; j++)
                {
                    dataGridView.Rows[rowIndex].Cells[j].Selected = true;
                }
            }

            // Call the method with true parameter
            bool allSelected = dataGridView.AreAllCellsSelected(true);

            // verify that false was returned
            scenRes.IncCounters(!allSelected, "AreAllCellsSelected returned " + allSelected + " when NOT all cells were selected, when " + selectionMethod, p.log);

            // Call the method with false parameter
            allSelected = dataGridView.AreAllCellsSelected(false);

            // verify that false was returned
            scenRes.IncCounters(!allSelected, "AreAllCellsSelected returned " + allSelected + " When NOT all visible cell were selected, when " + selectionMethod, p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult AreAllCellsSelected_SelectAllInvisibleAndVisible(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a typical DataGridView
            ResetTest(DataGridViewUtils.GetMixedInvisibleVisibleDataGridView(true, p));
            dataGridView.MultiSelect = true;

            string selectionMethod = "DataGridView.Multiselect = " + dataGridView.MultiSelect + " and DataGridView.SelectionMode = " + dataGridView.SelectionMode;

            // select all cells in the DataGridView using SelectAll method
            dataGridView.SelectAll();

            // verify all cells in the DataGridView are selected
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    scenRes.IncCounters(dataGridView.Rows[i].Cells[j].Selected, "Unable to select cell " + dataGridView.Rows[i].Cells[j] + " when " + selectionMethod, p.log);
                }
            }

            // call method with true parameter
            bool allSelected = dataGridView.AreAllCellsSelected(true);

            // verify that method returned true
            scenRes.IncCounters(allSelected, "AreAllCellsSelected returned " + allSelected + " when it should have returned true, when " + selectionMethod, p.log);

            // call method with false parameter
            allSelected = dataGridView.AreAllCellsSelected(false);

            // verify that method returned true
            scenRes.IncCounters(allSelected, "AreAllCellsSelected returned " + allSelected + " when it should have returned true, when " + selectionMethod, p.log);
            return scenRes;
        }

        [Scenario(true)]
        private void SelectAllAndClearSelection(DataGridView DataGridView, ScenarioResult scenRes, TParams p, string selectionMode)
        {
            // select the entire DataGridView
            DataGridView.SelectAll();
            if (DataGridView.SelectionMode == DataGridViewSelectionMode.CellSelect)
            {
                // verify that the entire DataGridView was selected
                for (int i = 0; i < DataGridView.Rows.Count; i++)
                {
                    for (int j = 0; j < DataGridView.Columns.Count; j++)
                    {
                        scenRes.IncCounters(DataGridView.Rows[i].Cells[j].Selected, "Unable to select cell " + DataGridView.Rows[i].Cells[j] + " when " + selectionMode, p.log);
                    }
                }
            }
            else if (DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect || DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
            {
                for (int j = 0; j < DataGridView.Rows.Count; j++)
                {
                    scenRes.IncCounters(DataGridView.Rows[j].Selected, "Unable to select row " + j + " when " + selectionMode, p.log);
                }
            }
            else if (DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect || DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect)
            {
                // check that all columns were selected
                for (int j = 0; j < DataGridView.Columns.Count; j++)
                {
                    scenRes.IncCounters(DataGridView.Columns[j].Selected, "Unable to select column " + j + " when " + selectionMode, p.log);
                }
            }

            // clear the selection
            DataGridView.ClearSelection();

            // verify there are no longer any selected cells in the DataGridView
            if (DataGridView.SelectionMode == DataGridViewSelectionMode.CellSelect)
            {
                for (int i = 0; i < DataGridView.Rows.Count; i++)
                {
                    for (int j = 0; j < DataGridView.Columns.Count; j++)
                    {
                        // check that all cells were unselected
                        scenRes.IncCounters(!DataGridView.Rows[i].Cells[j].Selected, "Cell " + DataGridView.Rows[i].Cells[j] + " is still selected, when " + selectionMode, p.log);
                    }
                }
            }
            else if (DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect || DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
            {
                for (int i = 0; i < DataGridView.Rows.Count; i++)
                {
                    // check that all rows were unselected
                    scenRes.IncCounters(!DataGridView.Rows[i].Selected, "Row " + i + " is still selected, when " + selectionMode, p.log);
                }
            }
            else if (DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect || DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect)
            {
                // check that all columns were unselected
                for (int j = 0; j < DataGridView.Columns.Count; j++)
                {
                    scenRes.IncCounters(!DataGridView.Columns[j].Selected, "Column " + j + " is still selected when it should have been unselected, when " + selectionMode, p.log);
                }
            }
        }

        [Scenario(true)]
        private void ClearSelection(DataGridView DataGridView, ScenarioResult scenRes, TParams p, string selectionMode)
        {
            // clear the selection
            DataGridView.ClearSelection();

            // verify there are no longer any selected cells in the DataGridView
            if (DataGridView.SelectionMode == DataGridViewSelectionMode.CellSelect)
            {
                for (int i = 0; i < DataGridView.Rows.Count; i++)
                {
                    for (int j = 0; j < DataGridView.Columns.Count; j++)
                    {
                        // check that all cells were unselected
                        scenRes.IncCounters(!DataGridView.Rows[i].Cells[j].Selected, "Cell " + DataGridView.Rows[i].Cells[j] + " is still selected, when " + selectionMode, p.log);
                    }
                }
            }
            else if (DataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect || DataGridView.SelectionMode == DataGridViewSelectionMode.RowHeaderSelect)
            {
                // check that all rows were unselected
                for (int j = 0; j < DataGridView.Rows.Count; j++)
                {
                    scenRes.IncCounters(!DataGridView.Rows[j].Selected, "Row " + j + " is still selected when it should have been unselected, when " + selectionMode, p.log);
                }
            }
            else if (DataGridView.SelectionMode == DataGridViewSelectionMode.ColumnHeaderSelect || DataGridView.SelectionMode == DataGridViewSelectionMode.FullColumnSelect)
            {
                // check that all columns were unselected
                for (int j = 0; j < DataGridView.Columns.Count; j++)
                {
                    scenRes.IncCounters(!DataGridView.Columns[j].Selected, "Column " + j + " is still selected when it should have been unselected, when " + selectionMode, p.log);
                }
            }
        }

        [Scenario(true)]
        public ScenarioResult SelectAllAndClearSelection_SelectAllWithNoExistingSelection(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a DataGridView that is made of all invisible cells
            ResetTest(DataGridViewUtils.GetTypicalDataGridView(true));

            string selectionMode = "default selectionMode";

            this.SelectAllAndClearSelection(dataGridView, scenRes, p, selectionMode);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SelectAllAndClearSelection_SelectAllWithExistingSelection(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a DataGridView that is made of all invisible cells
            ResetTest(DataGridViewUtils.GetTypicalDataGridView(true));

            string selectionMode = "";
            int randomRowIndex = 0;
            int randomColIndex = 0;
            DataGridViewCell firstRandomColumnCell = null;

            selectionMode = "DataGridView.SelectionMode = " + dataGridView.SelectionMode + " and DataGridView.MultiSelect = " + dataGridView.MultiSelect;

            // choose two cells randomly and select them at the same time
            randomRowIndex = 0;
            randomColIndex = 0;

            // select first random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            firstRandomColumnCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];
            firstRandomColumnCell.Selected = true;

            // select second random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell secondRandomColumnCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            secondRandomColumnCell.Selected = true;
            this.SelectAllAndClearSelection(dataGridView, scenRes, p, selectionMode);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SelectAllAndClearSelection_ClearSelectionWithExistingSelection(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a DataGridView that is made of all invisible cells
            ResetTest(DataGridViewUtils.GetTypicalDataGridView(true));

            string selectionMode = "";
            int randomRowIndex = 0;
            int randomColIndex = 0;
            DataGridViewCell firstRandomColumnCell = null;

            selectionMode = "DataGridView.SelectionMode = " + dataGridView.SelectionMode + " and DataGridView.MultiSelect = " + dataGridView.MultiSelect;

            // choose two cells randomly and select them at the same time
            randomRowIndex = 0;
            randomColIndex = 0;

            // select first random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            firstRandomColumnCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];
            firstRandomColumnCell.Selected = true;

            // select second random cell
            randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);

            DataGridViewCell secondRandomColumnCell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            secondRandomColumnCell.Selected = true;
            this.SelectAllAndClearSelection(dataGridView, scenRes, p, selectionMode);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SelectAllAndClearSelection_SelectAllWithSelectedDataGridView(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a DataGridView that is made of all invisible cells
            ResetTest(DataGridViewUtils.GetTypicalDataGridView(true));

            // allow multiple selection
            dataGridView.MultiSelect = true;

            // Use cell select
            dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;

            // define the selection mode for output purposes
            string selectionMode = "DataGridView.SelectionMode = " + dataGridView.SelectionMode + " and DataGridView.MultiSelect = " + dataGridView.MultiSelect;

            // select the entire DataGridView
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    dataGridView.Rows[i].Cells[j].Selected = true;
                }
            }

            this.SelectAllAndClearSelection(dataGridView, scenRes, p, selectionMode);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SelectAllAndClearSelection_ClearSelectionOnUnselectedDataGridView(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a DataGridView that is made of all invisible cells
            ResetTest(DataGridViewUtils.GetTypicalDataGridView(true));

            string selectionMode = "default selection mode";

            // Call ClearSelection on an unselected DataGridView
            this.ClearSelection(dataGridView, scenRes, p, selectionMode);
            return scenRes;
        }

        [Scenario(true)]
        private void VerifyOnlyCurrentCellSelected(DataGridView DataGridView, ScenarioResult scenRes, TParams p, string selectionMode)
        {
            DataGridViewCell cell = null;

            // choose the next current cell
            int randomRowIndex = 0;
            int randomColIndex = 0;

            cell = DataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            // set the current cell
            DataGridView.CurrentCell = cell;

            // verify that the current cell was set correctly
            scenRes.IncCounters(cell.Equals(DataGridView.CurrentCell), "CurrentCell was supposed to be set to: " + cell + " but instead has a value of: " + DataGridView.CurrentCell + " when " + selectionMode, p.log);

            // verify that the current cell is selected
            scenRes.IncCounters(cell.Equals(DataGridView.CurrentCell), "CurrentCell: " + DataGridView.CurrentCell + " was not selected when " + selectionMode, p.log);

            // verify that the current cell is the only cell that is selected
            for (int k = 0; k < DataGridView.Rows.Count; k++)
            {
                for (int j = 0; j < DataGridView.Columns.Count; j++)
                {
                    // verify non-current cells are not selected
                    if (!DataGridView.Rows[k].Cells[j].Equals(DataGridView.CurrentCell))
                    {
                        scenRes.IncCounters(!DataGridView.Rows[k].Cells[j].Selected, "Cell: " + DataGridView.Rows[k].Cells[j] + " should not be selected, when " + selectionMode, p.log);
                    }
                }
            }
        }

        [Scenario(true)]
        public ScenarioResult CurrentCell_UseOfValidCells(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a DataGridView that is made of all invisible cells
            ResetTest(DataGridViewUtils.GetTypicalDataGridView(true));

            string selectionMode = "";

            selectionMode = "DataGridView.SelectionMode = " + dataGridView.SelectionMode + " and DataGridView.MultiSelect = " + dataGridView.MultiSelect;
            this.VerifyOnlyCurrentCellSelected(dataGridView, scenRes, p, selectionMode);
            return scenRes;
        }
        /*
            // SetSelectedCellCore is protected
            public ScenarioResult SetSelectedCore_ValidCalls (TParams p)
            {
                // scenarioResult we will return as the result of this test
                ScenarioResult scenRes = new ScenarioResult ();

                // set the DataGridView instance to a DataGridView that is made of all invisible cells
                ResetTest(DataGridViewUtils.GetDerivedDataGridView (p));

                // test selecting cell (0,0)
                ((DerivedDataGridView)DataGridView).SetSelectedCellCore (0, 0, true);

                // verify that cell (0,0) is selected
                scenRes.IncCounters (DataGridView.Rows[0].Cells[0].Selected, "Cell (0,0) was not selected, as expected.", p.log);

                // test de-selected cell (0,0) 
                ((DerivedDataGridView)DataGridView).SetSelectedCellCore (0, 0, false);

                // verify that cell (0,0) is de-selected
                scenRes.IncCounters (!DataGridView.Rows[0].Cells[0].Selected, "Cell (0,0) is still selected.", p.log);

                // test selecting cell (1,1)
                ((DerivedDataGridView)DataGridView).SetSelectedCellCore (1, 1, true);

                // verify that cell (1,1) is selected
                scenRes.IncCounters (DataGridView.Rows[1].Cells[1].Selected, "Cell (1,1) was not selected, as expected.", p.log);

                // test selected cell (5,5)
                ((DerivedDataGridView)DataGridView).SetSelectedCellCore (5, 5, true);

                // verify that cell (5,5) is selected
                scenRes.IncCounters (DataGridView.Rows[5].Cells[5].Selected, "Cell (5,5) was not selected, as expected.", p.log);
                return scenRes;
            }
            */

        [Scenario(true)]
        public ScenarioResult SetAndGetCellSelected(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a DataGridView that is made of all invisible cells
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // obtain a random cell
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            DataGridViewCell cell = dataGridView.Rows[randomRowIndex].Cells[randomColIndex];

            // select the cell
            cell.Selected = true;

            // verify that the cell is selected
            scenRes.IncCounters(cell.Selected, "Cell: " + cell + " was not selected.", p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SetAndGetColumnSelected(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a DataGridView that is made of all invisible cells
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;

            // obtain a random column
            int randomColIndex = p.ru.GetRange(0, dataGridView.Columns.Count - 1);
            DataGridViewColumn column = dataGridView.Columns[randomColIndex];

            // select the column
            column.Selected = true;

            // verify that the column is selected
            scenRes.IncCounters(column.Selected, "Column: " + column + " was not selected when DataGridView.MultiSelect = " + dataGridView.MultiSelect + " and DataGridView.DataGridViewSelectionMode = " + dataGridView.SelectionMode, p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SetAndGetRowSelected(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a DataGridView that is made of all invisible cells
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // obtain a random row
            int randomRowIndex = p.ru.GetRange(0, dataGridView.Rows.Count - 1);
            DataGridViewRow row = dataGridView.Rows[randomRowIndex];

            // select the row
            row.Selected = true;

            // verify that the row is selected
            scenRes.IncCounters(row.Selected, "Row: " + row + " was not selected.", p.log);
            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SelectionAndCollections_CellSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // obtain a DataGridView instance
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.CellSelect;

            // Select the entire DataGridView
            dataGridView.SelectAll();

            // verify that each cell in the DataGridView was selected
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    scenRes.IncCounters(dataGridView.Rows[i].Cells[j].Selected, "Cell " + dataGridView.Rows[i].Cells[j] + " was not selected.", p.log);
                    if (!dataGridView.Rows[i].Cells[j].Selected)
                    {
                        p.log.LogKnownBug(WFCTestLib.Log.BugDb.VSWhidbey, 81937, "SelectAll does not select cells at the Cell level when DataGridView.SelectMode = DataGridViewSelectionMode.CellSelect and hence the cells in the DataGridView are not selected, and the expected selected cells are not returned in the Selected Collection.");
                    }
                }
            }

            // obtain the collection of selected cells
            DataGridViewSelectedCellCollection cellCollection = dataGridView.SelectedCells;

            // verify that the collection contains each cell in the DataGridView
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    scenRes.IncCounters(cellCollection.Contains(dataGridView.Rows[i].Cells[j]), "Cell " + dataGridView.Rows[i].Cells[j] + " is not contained within the Cell Collection returned by DataGridView.SelectedCells.", p.log);
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SelectionAndCollections_ColumnSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a DataGridView that is made of all invisible cells
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullColumnSelect;

            // Select the entire DataGridView
            dataGridView.SelectAll();

            // verify that each column in the DataGridView was selected
            for (int j = 0; j < dataGridView.Columns.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Columns[j].Selected, "Column " + dataGridView.Columns[j] + " was not selected.", p.log);
            }

            // obtain the collection of selected columns
            DataGridViewSelectedColumnCollection columnCollection = dataGridView.SelectedColumns;

            // verify that the collection contains each column in the DataGridView
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int j = 0; j < dataGridView.Columns.Count; j++)
                {
                    scenRes.IncCounters(columnCollection.Contains(dataGridView.Columns[j]), "Column " + dataGridView.Columns[j] + " is not contained within the Column Collection returned by DataGridView.SelectedColumns.", p.log);
                }
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult SelectionAndCollections_RowSelect(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a DataGridView that is made of all invisible cells
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));
            dataGridView.MultiSelect = true;
            dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Select the entire DataGridView
            dataGridView.SelectAll();

            // verify that each row in the DataGridView was selected
            for (int j = 0; j < dataGridView.Rows.Count; j++)
            {
                scenRes.IncCounters(dataGridView.Rows[j].Selected, "Row " + dataGridView.Rows[j] + " was not selected.", p.log);
            }

            // obtain the collection of selected rows
            DataGridViewSelectedRowCollection rowCollection = dataGridView.SelectedRows;

            // verify that the collection contains each row in the DataGridView
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                scenRes.IncCounters(rowCollection.Contains(dataGridView.Rows[i]), "Row " + dataGridView.Rows[i] + " is not contained within the Row Collection returned by DataGridView.SelectedRows.", p.log);
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult AddDelete_Row(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a DataGridView that is made of all invisible cells
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // obtain current row count for DataGridView
            int initialRowCount = dataGridView.Rows.Count;

            // Add a new row to the DataGridView
            dataGridView.Rows.Add();

            // obtain new row count for DataGridView
            int newRowCount = dataGridView.Rows.Count;

            // verify that the row count for the DataGridView increased by one
            scenRes.IncCounters(newRowCount == (initialRowCount + 1), "New row was not added to DataGridView correctly.", p.log);

            // will have to remove row (Count-2) if AllowUserToAddRows=true
            if (dataGridView.RowCount == 1 && dataGridView.AllowUserToAddRows)
            {
                initialRowCount = dataGridView.RowCount;
                dataGridView.Rows.AddCopies(0, 1);
                newRowCount = dataGridView.RowCount;
                scenRes.IncCounters(newRowCount == (initialRowCount + 1), "New row was not added to DataGridView correctly.", p.log);
            }

            // Delete a row from the DataGridView
            int delta = 1;

            // cannot modify last row when AllowUserToAddRows = true
            if (dataGridView.AllowUserToAddRows)
                delta = 2;

            DataGridViewRow toRemove = dataGridView.Rows[dataGridView.Rows.Count - delta];

            p.log.WriteLine("last two rows are the same: " + dataGridView.Rows[dataGridView.Rows.Count - 1].Equals(dataGridView.Rows[dataGridView.Rows.Count - 2]));
            p.log.WriteLine("two rows before last are the same: " + dataGridView.Rows[dataGridView.Rows.Count - 3].Equals(dataGridView.Rows[dataGridView.Rows.Count - 2]));
            p.log.WriteLine("prior remove RowCount = " + dataGridView.RowCount);
            p.log.WriteLine("attempted to remove row " + toRemove.Index);
            dataGridView.Rows.Remove(toRemove);
            p.log.WriteLine("after remove RowCount = " + dataGridView.RowCount);

            // verify that the row was removed by verifying the row count
            scenRes.IncCounters(dataGridView.Rows.Count == initialRowCount, "Count was no updated after row removal", p.log);
            if (dataGridView.RowCount == initialRowCount)
            {
                p.log.WriteLine("Rows.Contains(toRemove): " + dataGridView.Rows.Contains(toRemove));
                scenRes.IncCounters(!dataGridView.Rows.Contains(toRemove), p.log, BugDb.VSWhidbey, 116534, "Contains() returned true for recently removed row");
            }

            return scenRes;
        }

        [Scenario(true)]
        public ScenarioResult AddDelete_Column(TParams p)
        {
            // scenarioResult we will return as the result of this test
            ScenarioResult scenRes = new ScenarioResult();

            // set the DataGridView instance to a DataGridView that is made of all invisible cells
            ResetTest(DataGridViewUtils.GetStandardDataGridView(typeof(DataGridViewTextBoxColumn)));

            // obtain current column count for DataGridView
            int initialColumnCount = dataGridView.Columns.Count;

            // Add a new column to the DataGridView
            dataGridView.Columns.Add("ColumnOne", "Column One");

            // obtain new column count for DataGridView
            int newColumnCount = dataGridView.Columns.Count;

            // verify that the column count for the DataGridView increased by one
            scenRes.IncCounters(newColumnCount == (initialColumnCount + 1), "New column was not added to DataGridView correctly.", p.log);

            // Delete a column from the DataGridView
            dataGridView.Columns.Remove(dataGridView.Columns[dataGridView.Columns.Count - 1]);

            // verify that the column was removed by verifying the column count
            scenRes.IncCounters(dataGridView.Columns.Count == initialColumnCount, "Last column in DataGridView was not removed successfully.", p.log);
            return scenRes;
        }

        private int getVisibleRowIndex()
        {
            // according to DataGridViewUtils row 19 is invisible
            // we will exclude this row and return random row other than 19
            int randomIndex = scenarioParams.ru.GetRange(0, dataGridView.Rows.Count - 1);
            while (randomIndex == 19)
                randomIndex = scenarioParams.ru.GetRange(0, dataGridView.Rows.Count - 1);

            return randomIndex;
        }

        private int getVisibleColumnIndex()
        {
            // according to DataGridViewUtils columns 6 & 11 are invisible
            // we will exclude these columns and return random column other than 6&11
            int randomIndex = scenarioParams.ru.GetRange(0, dataGridView.Columns.Count - 1);
            while (randomIndex == 6 || randomIndex == 11)
                randomIndex = scenarioParams.ru.GetRange(0, dataGridView.Columns.Count - 1);

            return randomIndex;
        }
    }
}

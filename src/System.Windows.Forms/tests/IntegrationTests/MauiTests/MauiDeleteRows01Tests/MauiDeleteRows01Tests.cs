// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Forms.IntegrationTests.Common;
using System.Threading;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDeleteRows01Tests : ReflectBase
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDeleteRows01Tests(args));
        }

        public MauiDeleteRows01Tests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        DataGridView myObj = null;
        Control otherObj = null;
        bool userDeletedRow = false;
        bool userDeletingRow = false;

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);

            otherObj = new Control();
            this.Controls.Add(otherObj);

            myObj = new DataGridView();
            this.Controls.Add(myObj);

            myObj.UserDeletedRow += new DataGridViewRowEventHandler(myObj_UserDeletedRow);
            myObj.UserDeletingRow += new DataGridViewRowCancelEventHandler(myObj_UserDeletingRow);

            int numColumns = p.ru.GetRange(1, 10);
            for (int i = 0; i < numColumns; i++) { myObj.Columns.Add(p.ru.GetString(255), p.ru.GetString(255)); }

            int numRows = p.ru.GetRange(1, 100);
            for (int i = 0; i < numRows; i++)
            {
                DataGridViewRow row = new DataGridViewRow();

                for (int j = 0; j < numColumns; j++)
                {
                    DataGridViewCell cell = new DataGridViewTextBoxCell();
                    cell.Value = p.ru.GetString(10, true);
                    row.Cells.Add(cell);
                }

                myObj.Rows.Add(row);
            }
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            myObj.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            myObj.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            myObj.AllowUserToDeleteRows = true;
            myObj.ClearSelection();

            for (int i = 0; i < myObj.Columns.Count; i++)
                myObj.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;

            SafeMethods.Focus(myObj);
            Application.DoEvents();

            return base.BeforeScenario(p, scenario);
        }

        void myObj_UserDeletedRow(object sender, DataGridViewRowEventArgs e) { userDeletedRow = true; }
        void myObj_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) { userDeletingRow = true; }

        //==========================================
        // Scenarios
        //==========================================
        [Scenario(true)]
        public ScenarioResult AllCellsInARowSelected(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            DataGridViewSelectionMode mode = p.ru.GetEnumValue<DataGridViewSelectionMode>();
            while (mode == DataGridViewSelectionMode.FullRowSelect || mode == DataGridViewSelectionMode.RowHeaderSelect)
            { mode = p.ru.GetEnumValue<DataGridViewSelectionMode>(); }

            myObj.SelectionMode = mode;
            p.log.WriteLine("myObj.SelectionMode=" + myObj.SelectionMode.ToString());
            p.log.WriteLine("Can delete=" + myObj.AllowUserToDeleteRows.ToString());

            int numRows = myObj.Rows.Count;
            p.log.WriteLine("myObj.Rows.Count=" + myObj.Rows.Count.ToString());
            p.log.WriteLine("numRows=" + numRows.ToString());
            int selectedRow = p.ru.GetRange(0, numRows - 1);
            if (selectedRow >= 0) { p.log.WriteLine("myObj.Rows[" + selectedRow + "].Cells.Count=" + myObj.Rows[selectedRow].Cells.Count.ToString()); }

            foreach (DataGridViewCell cell in myObj.Rows[selectedRow].Cells) { cell.Selected = true; }
            p.log.WriteLine("myObj.SelectedCells.Count=" + myObj.SelectedCells.Count);

            userDeletedRow = false;
            userDeletingRow = false;
            SendKeys.SendWait("{DELETE}");
            result.IncCounters(!userDeletedRow, "FAIL: UserDeletedRow shouldn't have been fired", p.log);
            result.IncCounters(!userDeletingRow, "FAIL: UserDeletingRow shouldn't have been fired", p.log);
            result.IncCounters(numRows == myObj.Rows.Count, "FAIL: numRows != myObj.Rows.Count", myObj.Rows.Count, numRows, p.log);

            return result;
        }

        [Scenario(true)]
        public ScenarioResult AllCellsSelected(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            DataGridViewSelectionMode mode = p.ru.GetEnumValue<DataGridViewSelectionMode>();
            while (mode == DataGridViewSelectionMode.FullRowSelect || mode == DataGridViewSelectionMode.RowHeaderSelect)
            { mode = p.ru.GetEnumValue<DataGridViewSelectionMode>(); }

            myObj.SelectionMode = mode;
            p.log.WriteLine("myObj.SelectionMode=" + myObj.SelectionMode.ToString());

            int numRows = myObj.Rows.Count;
            p.log.WriteLine("myObj.Rows.Count=" + myObj.Rows.Count.ToString());
            if (numRows > 0) { p.log.WriteLine("myObj.Rows[0].Cells.Count=" + myObj.Rows[0].Cells.Count.ToString()); }

            foreach (DataGridViewRow row in myObj.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells) { cell.Selected = true; }
            }
            p.log.WriteLine("myObj.SelectedCells.Count=" + myObj.SelectedCells.Count);

            userDeletedRow = false;
            userDeletingRow = false;
            SendKeys.SendWait("{DELETE}");
            result.IncCounters(!userDeletedRow, "FAIL: UserDeletedRow shouldn't have been thrown", p.log);
            result.IncCounters(!userDeletingRow, "FAIL: UserDeletingRow shouldn't have been thrown", p.log);
            result.IncCounters(numRows == myObj.Rows.Count, "FAIL: numRows != myObj.Rows.Count", p.log);

            return result;
        }

        [Scenario(true)]
        public ScenarioResult EditOnEnter(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            myObj.AllowUserToDeleteRows = true;
            p.log.WriteLine("myObj.AllowUserToDeleteRows=" + myObj.AllowUserToDeleteRows.ToString());

            myObj.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            p.log.WriteLine("myObj.SelectionMode=" + myObj.SelectionMode.ToString());

            myObj.EditMode = DataGridViewEditMode.EditOnEnter;
            p.log.WriteLine("myObj.EditMode=" + myObj.EditMode.ToString());

            int numRows = myObj.Rows.Count;
            p.log.WriteLine("myObj.Rows.Count=" + numRows.ToString());

            int selectedRow = p.ru.GetRange(0, numRows);
            p.log.WriteLine("selectedRow=" + selectedRow.ToString());

            myObj.Rows[selectedRow].Selected = true;
            p.log.WriteLine("myObj.SelectedRows.Count=" + myObj.SelectedRows.Count);

            userDeletedRow = false;
            userDeletingRow = false;
            SendKeys.SendWait("{DELETE}");
            result.IncCounters(!userDeletedRow, "FAIL: UserDeletedRow shouldn't have been thrown", p.log);
            result.IncCounters(!userDeletingRow, "FAIL: UserDeletingRow shouldn't have been thrown", p.log);
            result.IncCounters(numRows == myObj.Rows.Count, "FAIL: numRows != myObj.Rows.Count", p.log);
            SafeMethods.Focus(otherObj);

            return result;
        }

        [Scenario(true)]
        public ScenarioResult AllowUserToDeleteRows(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            myObj.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            p.log.WriteLine("myObj.SelectionMode=" + myObj.SelectionMode.ToString());

            myObj.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            p.log.WriteLine("myObj.EditMode=" + myObj.EditMode.ToString());

            myObj.AllowUserToDeleteRows = false;
            p.log.WriteLine("myObj.AllowUserToDeleteRows=" + myObj.AllowUserToDeleteRows.ToString());

            int numRows = myObj.Rows.Count;
            p.log.WriteLine("myObj.Rows.Count=" + numRows.ToString());

            int selectedRow = p.ru.GetRange(0, numRows);
            p.log.WriteLine("selectedRow=" + selectedRow.ToString());

            myObj.Rows[selectedRow].Selected = true;
            p.log.WriteLine("myObj.SelectedRows.Count=" + myObj.SelectedRows.Count);

            userDeletedRow = false;
            userDeletingRow = false;
            SendKeys.SendWait("{DELETE}");
            result.IncCounters(!userDeletedRow, "FAIL: UserDeletedRow shouldn't have been thrown", p.log);
            result.IncCounters(!userDeletingRow, "FAIL: UserDeletingRow shouldn't have been thrown", p.log);
            result.IncCounters(numRows == myObj.Rows.Count, "FAIL: numRows != myObj.Rows.Count", p.log);

            return result;
        }
    }
}

// [Scenarios]
//@ AllCellsInARowSelected()
//@ AllCellsSelected()
//@ EditOnEnter()
//@ AllowUserToDeleteRows()

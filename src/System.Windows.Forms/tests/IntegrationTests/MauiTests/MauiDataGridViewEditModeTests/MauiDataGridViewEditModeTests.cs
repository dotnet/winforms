// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using System.Windows.Forms.IntegrationTests.Common;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Threading;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDataGridViewEditModeTests : ReflectBase
    {

        #region Testcase setup
        public MauiDataGridViewEditModeTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDataGridViewEditModeTests(args));
        }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            dgv = new DataGridView();
            int numCols = p.ru.GetRange(5, 10);
            int numRows = p.ru.GetRange(10, 100);
            for (int i = 0; i < numCols; i++)
                dgv.Columns.Add(new DataGridViewTextBoxColumn());
            dgv.Rows.Add(numRows);
            this.Controls.Add(dgv);
        }

        private void EditCell(ScenarioResult sr, TParams p)
        {
            sr.IncCounters(dgv.IsCurrentCellInEditMode, "Current cell is not in edit mode.", p.log);
            p.log.WriteLine("Edit mode before action: " + dgv.IsCurrentCellInEditMode.ToString());
            SafeMethods.SendWait(GetSendKeysSafeString(p));

            string editAction = (p.ru.GetBoolean()) ? "{ENTER}" : "{ESC}";
            p.log.WriteLine("Finishing action: " + editAction);

            SafeMethods.SendWait(editAction);

            p.log.WriteLine("Edit mode after action: " + dgv.IsCurrentCellInEditMode.ToString());
            sr.IncCounters(!dgv.IsCurrentCellInEditMode, "Current cell is still in edit mode after the edit.", p.log);
        }

        private void SelectRandomCell(TParams p)
        {
            DataGridViewCell currentCell = dgv.CurrentCell;
            int rowIndex;
            int colIndex;

            do
            {
                rowIndex = p.ru.GetRange(0, dgv.RowCount - 1);
            } while (rowIndex == currentCell.RowIndex);

            do
            {
                colIndex = p.ru.GetRange(0, dgv.ColumnCount - 1);
            } while (colIndex == currentCell.ColumnIndex);


            dgv.CurrentCell = dgv.Rows[rowIndex].Cells[colIndex];
        }

        private string GetSendKeysSafeString(TParams p)
        {
            GenStrings.IntlStrings intlStr = new GenStrings.IntlStrings();

            // We need to exlcude keys which SendKeys recognizes as "special"
            char[] exclude = new char[] { '%', '^', '+', '{', '}', '~', '(', ')' };
            bool isValid = true;
            string candidate = "";

            do
            {
                isValid = true;
                //candidate = p.ru.GetString(100);
                candidate = System.Text.Encoding.UTF8.GetString(intlStr.GetUniStrRandAnsiBytes(100, true, true, GenStrings.enuCodeType.CODE_UTF8, GenStrings.enuLCIDList.English, true));
                if (candidate.IndexOfAny(exclude) > -1)
                    isValid = false;
            }
            while (!isValid);

            p.log.WriteLine("Sending: " + candidate);

            return candidate;
        }
        private DataGridView dgv;
        #endregion

        #region Scenarios

        [Scenario(true)]
        public ScenarioResult EditOnEnter(TParams p)
        {
            dgv.EditMode = DataGridViewEditMode.EditOnEnter;
            SelectRandomCell(p);

            // With EditOnEnter, you can't actually leave edit mode on a cell
            return new ScenarioResult(dgv.IsCurrentCellInEditMode, "Current cell is not in edit mode.", p.log);
        }

        [Scenario(true)]
        public ScenarioResult EditOnF2(TParams p)
        {
            dgv.EditMode = DataGridViewEditMode.EditOnF2;

            SelectRandomCell(p);
            SafeMethods.SendWait("{F2}");

            ScenarioResult sr = new ScenarioResult();
            EditCell(sr, p);
            return sr;
        }

        [Scenario(true)]
        public ScenarioResult EditOnKeystroke(TParams p)
        {
            dgv.EditMode = DataGridViewEditMode.EditOnKeystroke;
            SelectRandomCell(p);
            string s = GetSendKeysSafeString(p);
            p.log.WriteLine("Sending: " + s);
            SafeMethods.SendWait(s);

            ScenarioResult sr = new ScenarioResult();
            EditCell(sr, p);
            return sr;
        }

        [Scenario(true)]
        public ScenarioResult EditOnKeystrokeOrF2(TParams p)
        {
            dgv.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            SelectRandomCell(p);
            SafeMethods.SendWait(GetSendKeysSafeString(p));

            ScenarioResult sr = new ScenarioResult();
            EditCell(sr, p);
            SelectRandomCell(p);
            SafeMethods.SendWait("{F2}");
            EditCell(sr, p);

            return sr;
        }

        [Scenario(true)]
        public ScenarioResult EditProgrammatically(TParams p)
        {
            dgv.EditMode = DataGridViewEditMode.EditProgrammatically;
            SelectRandomCell(p);

            dgv.BeginEdit(false);

            ScenarioResult sr = new ScenarioResult();
            sr.IncCounters(dgv.IsCurrentCellInEditMode, "Current cell is not in edit mode.", p.log);
            SendKeys.Send("stuff");

            dgv.EndEdit(DataGridViewDataErrorContexts.Commit);
            sr.IncCounters(!dgv.IsCurrentCellInEditMode, "Current cell is still in edit mode after the edit.", p.log);

            return sr;
        }

        #endregion
    }
}


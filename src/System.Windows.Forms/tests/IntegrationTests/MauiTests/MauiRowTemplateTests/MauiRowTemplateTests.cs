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
    public class MauiRowTemplateTests : ReflectBase
    {

        #region Testcase setup
        public MauiRowTemplateTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiRowTemplateTests(args));
        }


        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            base.BeforeScenario(p, scenario);
            if (dgv.RowTemplate == null)
                dgv.RowTemplate = new DataGridViewRow();

            return true;
        }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            dgv = new DataGridView();
            int numcols = p.ru.GetRange(1, 5);
            for (int i = 0; i < numcols; i++)
                dgv.Columns.Add(GetRandomColumn(p));
            this.Controls.Add(dgv);
            dgv.RowTemplate = new DataGridViewRow();
        }

        private DataGridViewColumn GetRandomColumn(TParams p)
        {
            int colType = p.ru.GetRange(0, 5);

            switch (colType)
            {
                case 0:
                    return new DataGridViewTextBoxColumn();
                    //break;
                case 1:
                    return new DataGridViewComboBoxColumn();
                    //break;
                case 2:
                    return new DataGridViewImageColumn();
                    //break;
                case 3:
                    return new DataGridViewLinkColumn();
                    //break;
                case 4:
                    return new DataGridViewButtonColumn();
                    //break;
                case 5:
                    return new DataGridViewCheckBoxColumn();
                    //break;
                default:
                    throw new ArgumentException(colType.ToString() + " is invalid.");
                    //break;
            }
        }

        private bool DoesRowUseRowTemplate(DataGridViewRow row, TParams p)
        {
            bool usesTemplate = true;
            DataGridViewRow template = dgv.RowTemplate;

            if (row.Height != template.Height)
            {
                p.log.WriteLine("Heights are not equal.");
                p.log.WriteLine("row: {0}, template : {1}", row.Height, template.Height);
                usesTemplate = false;
            }

            if (!row.DefaultCellStyle.Equals(template.DefaultCellStyle))
            {
                p.log.WriteLine("DefaultCellStyles are not equal.");
                p.log.WriteLine("row      : {0}", row.DefaultCellStyle);
                p.log.WriteLine("template : {0}", template.DefaultCellStyle);
                usesTemplate = false;
            }


            //Deal with Resizable == NotSet
            DataGridViewTriState effectiveTemplateResizable = template.Resizable;
            if (effectiveTemplateResizable == DataGridViewTriState.NotSet)
                effectiveTemplateResizable = row.DataGridView.AllowUserToResizeRows ? DataGridViewTriState.True : DataGridViewTriState.False;

            //Check state 
            if (row.ReadOnly != template.ReadOnly || row.Selected != template.Selected ||
                row.Frozen != template.Frozen || row.Visible != template.Visible || row.Resizable != effectiveTemplateResizable)
            {
                p.log.WriteLine("Row states are not equal.");
                p.log.WriteLine("(ReadOnly ) row: {0}, template : {1}", row.ReadOnly, template.ReadOnly);
                p.log.WriteLine("(Visible  ) row: {0}, template : {1}", row.Visible, template.Visible);
                p.log.WriteLine("(Frozen   ) row: {0}, template : {1}", row.Frozen, template.Frozen);
                p.log.WriteLine("(Selected ) row: {0}, template : {1}", row.Selected, template.Selected);
                p.log.WriteLine("(Resizable) row: {0}, template : {1}", row.Resizable, template.Resizable);
                usesTemplate = false;
            }


            return usesTemplate;
        }

        private DataGridView dgv;

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("Get RowTemplate's index.  Ensure that it is -1.")]
        [Scenario(true)]
        public ScenarioResult RowTemplateIndex(TParams p)
        {
            // TODO: Rename Scenario1 with a descriptive method name.
            return new ScenarioResult(dgv.RowTemplate.Index == -1, "RowTemplate had the wrong index.", -1, dgv.RowTemplate.Index, p.log);
        }

        //[Scenario("Set the RowTemplate to an existing row.  Expect an exception")]
        [Scenario(true)]
        public ScenarioResult SetToExistingRow(TParams p)
        {
            dgv.Rows.Add(p.ru.GetRange(1, 100));
            bool exceptionThrown = false;
            try
            {
                dgv.RowTemplate = dgv.Rows[p.ru.GetRange(0, dgv.Rows.Count)];
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }
            catch (Exception ex)
            {
                p.log.WriteLine("Exception is type " + ex.GetType().ToString());
                exceptionThrown = true;
            }
            return new ScenarioResult(exceptionThrown, "No exception was thrown when setting the RowTemplate to an existing row.", p.log);

        }

        //[Scenario("Set RowTemplate.Selected to true and add a row.  Ensure that we get an InvalidOperationException, and that the text makes sense.")]
        [Scenario(true)]
        public ScenarioResult SetSelected(TParams p)
        {
            bool exceptionThrown = false;
            try
            {
                dgv.RowTemplate.Selected = true;
                dgv.Rows.Add();
            }
            catch (InvalidOperationException)
            {
                exceptionThrown = true;
            }
            return new ScenarioResult(exceptionThrown, "No exception was thrown when setting RowTemplate.Selected to true and adding a row.", p.log);
        }

        //[Scenario("Create a DataGridView row and change its DefaultCellStyle.  Set the RowTemplate to that row.  Set it to null, and get it again.  Ensure that the row received is a new DataGridViewRow.")]
        [Scenario(true)]
        public ScenarioResult ResetRowTemplate(TParams p)
        {
            DataGridViewRow originalTemplate = dgv.RowTemplate;
            dgv.RowTemplate = null;
            return new ScenarioResult((originalTemplate != dgv.RowTemplate) && dgv.RowTemplate != null, "Nulling the RowTemplate and calling it again did not return a new row.", p.log);
        }

        //[Scenario("Create a DataGridView row and change its DefaultCellStyle.  Set the RowTemplate to that row.  Add a row with no arguments, and ensure that both the added row and the new row used the RowTemplage.  Also ensure that the DataGridView's DefaultCellStyle has not changed.")]
        [Scenario(true)]
        public ScenarioResult RowTemplateCellStyle(TParams p)
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.BackColor = p.ru.GetColor();
            style.Font = p.ru.GetFont();
            style.ForeColor = p.ru.GetColor();
            dgv.RowTemplate.DefaultCellStyle = style;
            return new ScenarioResult(dgv.DefaultCellStyle != style, "Setting the RowTemplate's DefaultCellStyle should not affect the DataGridView's DefaultCellSTyle.", p.log);
        }

        //[Scenario("Create a DataGridView row and change its DefaultCellStyle.  Set the RowTemplate to that row.  Add a row with a row argument and ensure that the added row did not use the RowTemplate, and that the new row did.")]
        [Scenario(true)]
        public ScenarioResult CheckForRowTemplateUsage(TParams p)
        {
            DataGridViewCellStyle style = new DataGridViewCellStyle();
            style.BackColor = p.ru.GetColor();
            style.Font = p.ru.GetFont();
            style.ForeColor = p.ru.GetColor();
            dgv.RowTemplate.DefaultCellStyle = style;
            dgv.AllowUserToAddRows = false;
            dgv.Rows.Add(new DataGridViewRow());
            dgv.AllowUserToAddRows = true;
            ScenarioResult ret = new ScenarioResult();
            ret.IncCounters(!DoesRowUseRowTemplate(dgv.Rows[dgv.Rows.Count - 2], p), "Added row uses the RowTemplate, but it should not.", p.log);
            ret.IncCounters(DoesRowUseRowTemplate(dgv.Rows[dgv.Rows.Count - 1] /* New Row */, p), "New row does not use the RowTemplate.", p.log);
            return ret;
        }

        [Scenario(true)]
        //[Scenario("Create a DataGridView row and change its DefaultCellStyle.  Set the RowTemplate to that row.  Add a row, then change the RowTemplate.DefaultCellStyle.BackColor to red.  Add a row.  Ensure that the change is not made retroactively to other existing rows, and that the new row used the changed RowTemplate.")]
        public ScenarioResult ChangeStyle(TParams p)
        {
            dgv.AllowUserToAddRows = true;

            DataGridViewCellStyle originalStyle = dgv.RowTemplate.DefaultCellStyle;
            dgv.Rows.Add();
            DataGridViewRow unchangedRow = dgv.Rows[dgv.Rows.Count - 1];

            DataGridViewCellStyle newStyle = new DataGridViewCellStyle();
            newStyle.BackColor = p.ru.GetColor();
            newStyle.Font = p.ru.GetFont();
            newStyle.ForeColor = p.ru.GetColor();
            dgv.RowTemplate.DefaultCellStyle = newStyle;
            dgv.AllowUserToAddRows = false;
            dgv.Rows.Add();
            dgv.AllowUserToAddRows = true;

            ScenarioResult ret = new ScenarioResult();
            ret.IncCounters(unchangedRow.DefaultCellStyle != newStyle, "Style changes were applied retroactively to other rows.", p.log);
            //ret.IncCounters(DoesRowUseRowTemplate(dgv.Rows[dgv.Rows.Count - 1], p), "New row does not use the RowTemplate.", p.log);
            return ret;
        }

        //[Scenario("Remove a row that uses the RowTemplate.  Expect no change in the style of the other rows that used the RowTemplate.")]
        [Scenario(true)]
        public ScenarioResult RemoveRow(TParams p)
        {
            DataGridViewCellStyle newStyle = new DataGridViewCellStyle();
            newStyle.BackColor = p.ru.GetColor();
            newStyle.Font = p.ru.GetFont();
            newStyle.ForeColor = p.ru.GetColor();
            dgv.RowTemplate.DefaultCellStyle = newStyle;

            int initialRowIndex = dgv.Rows.Count - 1;
            int numAdded = 10; // there are already parameter tests
            dgv.Rows.Add(numAdded);
            dgv.Rows.Remove(dgv.Rows[initialRowIndex]); // there are already parameter tests
            ScenarioResult ret = new ScenarioResult();
            for (int i = initialRowIndex; i < dgv.Rows.Count - 1; i++)
                ret.IncCounters(DoesRowUseRowTemplate(dgv.Rows[i], p), "Row " + i.ToString() + " no longer uses the RowTemplate.", p.log);
            return ret;
        }

        //[Scenario("Call AddCopies.  Rows should not use the RowTemplate.")]
        [Scenario(true)]
        public ScenarioResult AddCopies(TParams p)
        {
            int rowsAdded = 5; // there are already parameter tests
            int initialRowIndex = dgv.Rows.Count - 1;
            dgv.Rows.AddCopies(0, rowsAdded);
            ScenarioResult ret = new ScenarioResult();
            for (int i = initialRowIndex; i < dgv.Rows.Count - 1; i++)
                ret.IncCounters(!DoesRowUseRowTemplate(dgv.Rows[i], p), "Row uses RowTemplate and should not.", p.log);
            return ret;
        }

        //[Scenario("Call Insert.  Rows should use the RowTemplate.")]
        [Scenario(true)]
        public ScenarioResult Insert(TParams p)
        {
            int rowsAdded = 5; // there are already parameter tests
            int initialRowIndex = dgv.Rows.Count - 1;
            dgv.Rows.Insert(0, rowsAdded);
            ScenarioResult ret = new ScenarioResult();
            for (int i = 0; i < rowsAdded; i++)
                ret.IncCounters(DoesRowUseRowTemplate(dgv.Rows[i], p), "Row doesn't use the RowTemplate as expected.", p.log);
            return ret;
        }

        #endregion
    }
}


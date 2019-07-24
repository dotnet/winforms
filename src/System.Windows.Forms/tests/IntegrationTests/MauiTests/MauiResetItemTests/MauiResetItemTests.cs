// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using System.Windows.Forms.IntegrationTests.Common;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Collections;
using System.Data;
using System.ComponentModel;
using System.Threading;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiResetItemTests : ReflectBase
    {

        DataSet ds;
        DataTable dt;
        ArrayList strs;
        BindingSource dc;
        const int DefaultRowCount = 10;
        ScenarioResult sr = null;
        DataGridView dgv = null;
        int initialize = 0;
        ListChangedEventArgs args = null;
        int eventCount = 0;

        #region Testcase setup
        public MauiResetItemTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiResetItemTests(args));
        }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            dc = new BindingSource();
            int i = p.ru.GetRange(1, 5);
            initialize = i;
            switch (i)
            {
                case 1:
                    InitIList(DefaultRowCount);
                    break;
                case 2:
                    InitIListAsIListSource(DefaultRowCount);
                    break;
                case 3:
                    InitIListSource(DefaultRowCount, p);
                    break;
                case 4:
                    InitIBindingList(DefaultRowCount, p, p.ru.GetBoolean());
                    break;
                case 5:
                    InitIBindingListAsIListSource(DefaultRowCount, p, p.ru.GetBoolean());
                    break;
                default:
                    break;
            }
            sr = new ScenarioResult();
            dc.ListChanged += new System.ComponentModel.ListChangedEventHandler(dc_ListChanged);
            this.Controls.Clear();
            dgv = new DataGridView();
            dgv.DataSource = dc;
            this.Controls.Add(dgv);
            Application.DoEvents();
            return base.BeforeScenario(p, scenario);
        }

        void dc_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e)
        {
            eventCount++;
            args = e;
        }

        // IList
        void InitIList(int rows)
        {
            strs = new ArrayList();
            for (int i = 0; i < rows; i++)
                strs.Add(new Customer(scenarioParams.ru.GetString(20), i));

            scenarioParams.log.WriteLine("Data Source: IList");
            dc.DataSource = strs;
        }

        // IList as IListSource
        void InitIListAsIListSource(int rows)
        {
            strs = new ArrayList();
            for (int i = 0; i < rows; i++)
                strs.Add(new Customer(scenarioParams.ru.GetString(20), i));

            scenarioParams.log.WriteLine("Data Source: IList as an IListSource");
            dc.DataSource = new MyIListSource(strs);
        }

        // IListSource
        void InitIListSource(int rows, TParams scenarioParams)
        {
            dt = new DataTable("tbl");
            for (int i = 0; i < 5; i++)
                dt.Columns.Add("col" + i);

            for (int i = 0; i < rows; i++)
                dt.Rows.Add(scenarioParams.ru.GetString(50), scenarioParams.ru.GetString(50), scenarioParams.ru.GetString(50), scenarioParams.ru.GetString(50), scenarioParams.ru.GetString(50));

            ds = new DataSet();
            ds.Tables.Add(dt);
            scenarioParams.log.WriteLine("Data Source: IListSource");
            // DataTable implements IListSource
            dc.DataSource = dt;
        }

        // IBindingList
        void InitIBindingList(int rows, TParams scenarioParams, bool allowNew)
        {
            dt = new DataTable("tbl");
            for (int i = 0; i < 5; i++)
                dt.Columns.Add("col" + i);

            for (int i = 0; i < rows; i++)
                dt.Rows.Add(scenarioParams.ru.GetString(50), scenarioParams.ru.GetString(50), scenarioParams.ru.GetString(50), scenarioParams.ru.GetString(50), scenarioParams.ru.GetString(50));

            DataView dv = dt.DefaultView;
            dv.AllowNew = allowNew;
            scenarioParams.log.WriteLine("Data Source: IBindingList");
            // DataView implements IBindingList
            dc.DataSource = dv;
        }

        // IBindingList as IListSource
        void InitIBindingListAsIListSource(int rows, TParams scenarioParams, bool allowNew)
        {
            dt = new DataTable("tbl");
            for (int i = 0; i < 5; i++)
                dt.Columns.Add("col" + i);

            for (int i = 0; i < rows; i++)
                dt.Rows.Add(scenarioParams.ru.GetString(50), scenarioParams.ru.GetString(50), scenarioParams.ru.GetString(50), scenarioParams.ru.GetString(50), scenarioParams.ru.GetString(50));

            DataView dv = dt.DefaultView;
            dv.AllowNew = allowNew;
            scenarioParams.log.WriteLine("Data Source: IBindingList as an IListSource w/ Add New False");
            dc.DataSource = new MyIListSource(dv);
        }

        private class MyIListSource : System.ComponentModel.IListSource
        {
            IList ilist;

            public MyIListSource(IList s)
            {
                ilist = s;
            }

            #region IListSource Members

            bool System.ComponentModel.IListSource.ContainsListCollection
            {
                get
                {
                    return true;
                }
            }

            IList System.ComponentModel.IListSource.GetList()
            {
                return ilist;
            }

            #endregion
        }


        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios

        //[Scenario("Dont change value. Call ResetItem.")]
        [Scenario(true)]
        public ScenarioResult Scenario1(TParams p)
        {
            string oldValue;
            eventCount = 0;
            args = null;
            int index = p.ru.GetRange(0, dc.List.Count - 1);
            if (initialize == 1 || initialize == 2)
            {
                oldValue = (strs[index] as Customer).Name;
                dc.ResetItem(index);
                sr.IncCounters(oldValue, dgv.Rows[index].Cells[0].Value.ToString(), "Wrong value after reset", p.log);
            }
            else
            {
                int col = p.ru.GetRange(0, dt.Columns.Count - 1);
                oldValue = (dc[index] as DataRowView)[col].ToString();
                dc.ResetItem(index);
                sr.IncCounters(oldValue, dgv.Rows[index].Cells[col].Value.ToString(), "Wrong value after reset", p.log);
            }
            sr.IncCounters(1, eventCount, "Wrong count for eventCount", p.log);
            sr.IncCounters(ListChangedType.ItemChanged, args.ListChangedType, "Wrong type", p.log);
            return sr;
        }

        //[Scenario("Change value. Call ResetItem. ")]
        [Scenario(true)]
        public ScenarioResult Scenario2(TParams p)
        {
            string oldValue;
            string newValue = p.ru.GetString(50);
            eventCount = 0;
            args = null;
            int index = p.ru.GetRange(0, dc.List.Count - 1);
            if (initialize == 1 || initialize == 2)
            {
                oldValue = (strs[index] as Customer).Name;
                (strs[index] as Customer).Name = newValue;
                dc.ResetItem(index);
                sr.IncCounters(newValue, dgv.Rows[index].Cells[0].Value.ToString(), "Wrong value after reset", p.log);
            }
            else
            {
                int col = p.ru.GetRange(0, dt.Columns.Count - 1);
                oldValue = (dc[index] as DataRowView)[col].ToString();
                (dc[index] as DataRowView)[col] = newValue;
                eventCount = 0;
                dc.ResetItem(index);
                sr.IncCounters(newValue, dgv.Rows[index].Cells[col].Value.ToString(), "Wrong value after reset", p.log);
            }
            sr.IncCounters(1, eventCount, "Wrong count for eventCount", p.log);
            sr.IncCounters(ListChangedType.ItemChanged, args.ListChangedType, "Wrong type", p.log);
            return sr;
        }

        //[Scenario("Edit to same value. ")]
        [Scenario(true)]
        public ScenarioResult Scenario3(TParams p)
        {
            string oldValue;
            string newValue;
            eventCount = 0;
            args = null;
            int index = p.ru.GetRange(0, dc.List.Count - 1);
            if (initialize == 1 || initialize == 2)
            {
                oldValue = (strs[index] as Customer).Name;
                newValue = oldValue;
                (strs[index] as Customer).Name = newValue;
                dc.ResetItem(index);
                sr.IncCounters(newValue, dgv.Rows[index].Cells[0].Value.ToString(), "Wrong value after reset", p.log);
            }
            else
            {
                int col = p.ru.GetRange(0, dt.Columns.Count - 1);
                oldValue = (dc[index] as DataRowView)[col].ToString();
                newValue = oldValue;
                (dc[index] as DataRowView)[col] = newValue;
                eventCount = 0;
                dc.ResetItem(index);
                sr.IncCounters(newValue, dgv.Rows[index].Cells[col].Value.ToString(), "Wrong value after reset", p.log);
            }
            sr.IncCounters(1, eventCount, "Wrong count for eventCount", p.log);
            sr.IncCounters(ListChangedType.ItemChanged, args.ListChangedType, "Wrong type", p.log);
            return sr;
        }

        //[Scenario("Edit from null value to another value. ")]
        [Scenario(true)]
        public ScenarioResult Scenario4(TParams p)
        {
            string oldValue;
            string newValue;
            int index = p.ru.GetRange(0, dc.List.Count - 1);
            if (initialize == 1 || initialize == 2)
            {
                (strs[index] as Customer).Name = null;
                eventCount = 0;
                args = null;
                oldValue = (strs[index] as Customer).Name;
                newValue = p.ru.GetString(50);
                (strs[index] as Customer).Name = newValue;
                dc.ResetItem(index);
                sr.IncCounters(newValue, dgv.Rows[index].Cells[0].Value.ToString(), "Wrong value after reset", p.log);
            }
            else
            {
                int col = p.ru.GetRange(0, dt.Columns.Count - 1);
                (dc[index] as DataRowView)[col] = null;
                oldValue = (dc[index] as DataRowView)[col].ToString();
                newValue = p.ru.GetString(50);
                (dc[index] as DataRowView)[col] = newValue;
                eventCount = 0;
                args = null;
                dc.ResetItem(index);
                sr.IncCounters(newValue, dgv.Rows[index].Cells[col].Value.ToString(), "Wrong value after reset", p.log);
            }
            sr.IncCounters(1, eventCount, "Wrong count for eventCount", p.log);
            sr.IncCounters(ListChangedType.ItemChanged, args.ListChangedType, "Wrong type", p.log);
            return sr;
        }

        //[Scenario("Edit to null value")]
        [Scenario(true)]
        public ScenarioResult Scenario5(TParams p)
        {
            string oldValue;
            string newValue = null;
            eventCount = 0;
            args = null;
            int index = p.ru.GetRange(0, dc.List.Count - 1);
            if (initialize == 1 || initialize == 2)
            {
                oldValue = (strs[index] as Customer).Name;
                (strs[index] as Customer).Name = newValue;
                dc.ResetItem(index);
                sr.IncCounters(null, dgv.Rows[index].Cells[0].Value, "Wrong value after reset", p.log);
            }
            else
            {
                int col = p.ru.GetRange(0, dt.Columns.Count - 1);
                oldValue = (dc[index] as DataRowView)[col].ToString();
                (dc[index] as DataRowView)[col] = newValue;
                eventCount = 0;
                dc.ResetItem(index);
                sr.IncCounters(String.Empty, dgv.Rows[index].Cells[col].Value.ToString(), "Wrong value after reset", p.log);
            }
            sr.IncCounters(1, eventCount, "Wrong count for eventCount", p.log);
            sr.IncCounters(ListChangedType.ItemChanged, args.ListChangedType, "Wrong type", p.log);
            return sr;
        }

        #endregion

        public class Customer
        {
            private string name;
            private int id;

            public Customer(string name, int id)
            {
                this.name = name;
                this.id = id;
            }

            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                }
            }

            public int ID
            {
                get
                {
                    return id;
                }
                set
                {
                    id = value;
                }
            }
        }
    }
}


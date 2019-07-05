// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Globalization;
using System.Data;
using System.ComponentModel;
using ReflectTools;
using ReflectTools.AutoPME;
using WFCTestLib.Util;
using WFCTestLib.Log;
using System.Collections;
using System.Windows.Forms.IntegrationTests.Common;


/*
Abstract:
Contexts: List is an IList that isn't an IBindingList, List is an IBindingList with AllowNew = false, List is an IBindingList with AllowNew = true.
Choose randomly whether the DataSource is the actual list or an IListSource.
Choose randomly whether to call AddNew via code or by clicking the AddNew Vcr button.
Verify that AddingNew/AddedNew events fire as expected
For all scenarios, verify that a new row is added which becomes the current row and that bound controls update.
*/



[assembly: System.Security.AllowPartiallyTrustedCallers]
// Required for security tests
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiAddNewTests : ReflectBase
    {
        public MauiAddNewTests(string[] args)
            : base(args)
        {
            this.BringToForeground();
        }

        //IList notBindingList, bindingListAllowNew, bindingListAllowNewFalse;

        private DataSet ds;

        private DataTable dt;

        private BindingSource dc;

        string events;

        const int DEFAULTROWCOUNT = 10;

        const string POSITIONCHANGED = "PositionChanged:";
        const string CURRENTITEMCHANGED = "CurrentItemChanged:";
        const string CURRENTCHANGED = "CurrentChanged:";
        const string ADDINGNEW = "AddingNew:";
        const string NORMALORDER = ADDINGNEW;
        const string ADDINGTOEMPTY = ADDINGNEW + POSITIONCHANGED + CURRENTCHANGED + CURRENTITEMCHANGED;

        void InitData()
        {
            InitData(DEFAULTROWCOUNT);
        }

        void InitDataAllowNewFalse()
        {
            InitDataAllowNewFalse(DEFAULTROWCOUNT);
        }

        void InitDataAllowNewFalse(int rows)
        {
            dt = new DataTable("tbl");
            for (int i = 0; i < 5; i++)
            {
                dt.Columns.Add("col" + i);
            }

            for (int i = 0; i < rows; i++)
            {
                dt.Rows.Add(MauiAddNewTests.scenarioParams.ru.GetString(50), MauiAddNewTests.scenarioParams.ru.GetString(50), MauiAddNewTests.scenarioParams.ru.GetString(50), MauiAddNewTests.scenarioParams.ru.GetString(50), MauiAddNewTests.scenarioParams.ru.GetString(50));
            }

            DataView dv = dt.DefaultView;
            dv.AllowNew = false;
            if (scenarioParams.ru.GetBoolean())
            {
                scenarioParams.log.WriteLine("Data Source: IBindingList w/ Add New False");
                dc.DataSource = dv;
            }
            else
            {
                scenarioParams.log.WriteLine("Data Source: IBindingList as an IListSource w/ Add New False");
                dc.DataSource = new MyIListSource(dv);
            }
            ResetEvents();
        }

        void InitData(int rows)
        {
            dt = new DataTable("tbl");
            for (int i = 0; i < 5; i++)
            {
                dt.Columns.Add("col" + i);
            }

            for (int i = 0; i < rows; i++)
            {
                dt.Rows.Add(MauiAddNewTests.scenarioParams.ru.GetString(50), MauiAddNewTests.scenarioParams.ru.GetString(50), MauiAddNewTests.scenarioParams.ru.GetString(50), MauiAddNewTests.scenarioParams.ru.GetString(50), MauiAddNewTests.scenarioParams.ru.GetString(50));
            }

            ds = new DataSet();
            ds.Tables.Add(dt);
            if (scenarioParams.ru.GetBoolean())
            {
                scenarioParams.log.WriteLine("Data Source: IBindingList w/ Add New True");
                dc.DataSource = dt;
            }
            else
            {
                scenarioParams.log.WriteLine("Data Source: IBindingList as an IListSource w/ Add New True");
                dc.DataSource = (IListSource)dt;
            }
            ResetEvents();
        }
        void InitDataIList()
        {
            InitDataIList(DEFAULTROWCOUNT);
        }

        void InitDataIList(int rows)
        {
            string[] strs = new string[rows];

            for (int i = 0; i < rows; i++)
            {
                strs[i] = scenarioParams.ru.GetString(50);
            }

            if (scenarioParams.ru.GetBoolean())
            {
                scenarioParams.log.WriteLine("Data Source: IList");
                dc.DataSource = strs;
            }
            else
            {
                scenarioParams.log.WriteLine("Data Source: IList as an IListSource");
                dc.DataSource = new MyIListSource(strs);
            }
            ResetEvents();
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

        void ResetEvents()
        {
            events = "";
        }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            dc = new BindingSource();
            dc.PositionChanged += new EventHandler(dc_PositionChanged);
            dc.CurrentChanged += new EventHandler(dc_CurrentChanged);
            dc.AddingNew += new AddingNewEventHandler(dc_AddingNew);
            dc.CurrentItemChanged += new EventHandler(dc_CurrentItemChanged);
        }

        //Added in case you want to log stuff, handle events, etc. before adding.
        private void AddNewRow(BindingSource dc)
        {
            dc.AddNew();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiAddNewTests(args));
        }

        [Scenario(true)]
        //[Scenario("AddNew from first row.")]
        public ScenarioResult FromFirstRecord(TParams p)
        {
            InitData();
            dc.Position = 0;
            ResetEvents();
            AddNewRow(dc);

            ScenarioResult sr = new ScenarioResult();

            sr.IncCounters(dc.List.Count - 1, dc.Position, "Wrong position", p.log);

            p.log.WriteLine("WORKAROUND: VSCurrent 357429 Won't fix");
            string temp = ADDINGNEW + CURRENTCHANGED + CURRENTITEMCHANGED + POSITIONCHANGED;
            sr.IncCounters(temp, events, "Wrong events", p.log);
            //sr.IncCounters(ADDINGTOEMPTY, events, "Wrong events, VSCurrent Bug# 357429", p.log);
            return sr;
        }

        [Scenario(true)]
       //[Scenario("AddNew from first row, AllowNew=false")]
        public ScenarioResult FromFirstRecord2(TParams p)
        {
            InitDataAllowNewFalse();
            dc.Position = 0;
            ResetEvents();
            try
            {
                AddNewRow(dc);
                return new ScenarioResult(false, "Didn't throw", p.log);
            }
            catch (InvalidOperationException)
            {
                return new ScenarioResult(true, "Threw as expected", p.log);
            }
        }

        [Scenario(true)]
        //[Scenario("AddNew from last row.")]
        public ScenarioResult FromLastRecord(TParams p)
        {
            InitData();
            dc.MoveLast();

            ScenarioResult sr = new ScenarioResult();

            sr.IncCounters(DEFAULTROWCOUNT - 1, dc.Position, "Wrong position 1", p.log);
            ResetEvents();
            AddNewRow(dc);
            sr.IncCounters(DEFAULTROWCOUNT, dc.Position, "Wrong position 2", p.log);

            p.log.WriteLine("WORKAROUND: VSCurrent 357429 Won't fix");
            string temp = ADDINGNEW + CURRENTCHANGED + CURRENTITEMCHANGED + POSITIONCHANGED;
            sr.IncCounters(temp, events, "Wrong events", p.log);
            //sr.IncCounters(ADDINGTOEMPTY, events, "Wrong events. VSCurrent Bug# 357429", p.log);
            return sr;
        }

       [Scenario(true)]
        //[Scenario("AddNew from last row, AllowNew=false")]
        public ScenarioResult FromLastRecord2(TParams p)
        {
            InitDataAllowNewFalse();
            dc.MoveLast();
            try
            {
                AddNewRow(dc);
                return new ScenarioResult(false, "Didn't throw", p.log);
            }
            catch (InvalidOperationException)
            {
                return new ScenarioResult(true, "Threw as expected", p.log);
            }
        }

        [Scenario(true)]
        //[Scenario("AddNew from random middle row.")]
        public ScenarioResult FromMiddleRecord(TParams p)
        {
            InitData();

            int pos = p.ru.GetRange(1, DEFAULTROWCOUNT - 2);

            dc.Position = pos;

            ScenarioResult sr = new ScenarioResult();

            sr.IncCounters(pos, dc.Position, "Wrong position", p.log);
            ResetEvents();
            AddNewRow(dc);
            sr.IncCounters(DEFAULTROWCOUNT, dc.Position, "Wrong position 2", p.log);

            p.log.WriteLine("WORKAROUND: VSCurrent 357429 Won't fix");
            string temp = ADDINGNEW + CURRENTCHANGED + CURRENTITEMCHANGED + POSITIONCHANGED;
            sr.IncCounters(temp, events, "Wrong events", p.log);
            //sr.IncCounters(ADDINGTOEMPTY, events, "Wrong events. VSCurrent Bug# 357429", p.log);
            return sr;
        }

        [Scenario(true)]
        //[Scenario("AddNew from random middle row, AllowNew=false.")]
        public ScenarioResult FromMiddleRecord2(TParams p)
        {
            InitDataAllowNewFalse();
            int pos = p.ru.GetRange(1, DEFAULTROWCOUNT - 2);
            dc.Position = pos;
            try
            {
                AddNewRow(dc);
                return new ScenarioResult(false, "Didn't throw", p.log);
            }
            catch (InvalidOperationException)
            {
                return new ScenarioResult(true, "Threw as expected", p.log);
            }
        }

       [Scenario(true)]
        //[Scenario("AddNew from empty data source")]
        public ScenarioResult WithNoData(TParams p)
        {
            InitData(0);

            ScenarioResult sr = new ScenarioResult();

            sr.IncCounters(-1, dc.Position, "Wrong position", p.log);
            ResetEvents();
            AddNewRow(dc);
            sr.IncCounters(0, dc.Position, "Wrong position 2", p.log);
            sr.IncCounters(ADDINGTOEMPTY, events, "Wrong events", p.log);
            return sr;
        }

        [Scenario(true)]
        //[Scenario("AddNew from empty data source, AllowNew=false")]
        public ScenarioResult WithNoData2(TParams p)
        {
            InitDataAllowNewFalse(0);
            try
            {
                AddNewRow(dc);
                return new ScenarioResult(false, "Didn't throw", p.log);
            }
            catch (InvalidOperationException)
            {
                return new ScenarioResult(true, "Threw as expected", p.log);
            }
        }

      [Scenario(true)]
        //[Scenario("AddNew with DataSource = dataset, DataMember = null")]
        public ScenarioResult InvalidEmptyDataMember(TParams p)
        {
            InitData();
            dc.DataSource = ds;

            ScenarioResult sr = new ScenarioResult();

            sr.IncCounters(0, dc.Position, "Wrong position", p.log);
            ResetEvents();
            try
            {
                AddNewRow(dc);
                sr.IncCounters(false, "Didn't throw expected exception", p.log);
            }
            catch (InvalidOperationException)
            {
                sr.IncCounters(true, "Threw expected exception", p.log);
            }
            return sr;
        }

        #region EVENTS
        private void dc_PositionChanged(object sender, EventArgs e)
        {
            events += POSITIONCHANGED;
        }

        private void dc_CurrentChanged(object sender, EventArgs e)
        {
            events += CURRENTCHANGED;
        }

        private void dc_AddingNew(object sender, AddingNewEventArgs e)
        {
            events += ADDINGNEW;
        }


        void dc_CurrentItemChanged(object sender, EventArgs e)
        {
            events += CURRENTITEMCHANGED;
        }
        #endregion
    }
}




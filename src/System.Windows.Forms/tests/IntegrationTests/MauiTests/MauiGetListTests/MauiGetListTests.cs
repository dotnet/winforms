// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Windows.Forms;
using WFCTestLib;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.ComponentModel;
using System.Collections;
using System.Windows.Forms.IntegrationTests.Common;
using System.Threading;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiGetListTests : ReflectBase
    {
        DataTable dt;
        public MauiGetListTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiGetListTests(args));
        }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            dt = this.getDataTable(p);
            return base.BeforeScenario(p, scenario);
        }

        public DataTable getDataTable(TParams p)
        {
            DataTable book = new DataTable("Book");
            DataRow dr;
            book.Columns.Add("Name", typeof(string));
            book.Columns.Add("Price", typeof(decimal));
            book.Columns.Add("ProductDate", typeof(DateTime));
            int count = p.ru.GetRange(1, 5);
            for (int i = 0; i < count; i++)
            {
                dr = book.NewRow();
                dr[0] = "bName" + i.ToString();
                dr[1] = p.ru.GetRange(1000, 1000 + 10 * i);
                dr[2] = DateTime.Now.AddDays(p.ru.GetRange(0, i));
                book.Rows.Add(dr);
            }
            return book;
        }

        #region 3 scenarios

        //GetList(null).  Verify no exception.
        [Scenario(true)]
        public ScenarioResult GetListIsNull(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            object ob = ListBindingHelper.GetList(null);
            sr.IncCounters(null, ob, "GetList(null) gets Exception", p.log);
            return sr;
        }

        //Pass in an IListSource.  Verify results identical to IListSource.GetList().
        [Scenario(true)]
        public ScenarioResult GetListFromIListSource(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            IListSource ls = dt;
            object ob1 = ListBindingHelper.GetList(ls);
            IList ob2 = ls.GetList();
            sr.IncCounters(ob1, ob2, "These 2 method Doesn't get the same result", p.log);
            return sr;
        }

        //Pass in an object that is not an IListSource.  Verify the dataSource is returned.
        [Scenario(true)]
        public ScenarioResult GetListFromDataTable(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            object ob = ListBindingHelper.GetList(dt);
            DataView dv1 = new DataView(dt);
            DataView dv2 = (DataView)ob;
            sr.IncCounters(typeof(DataView), ob.GetType(), "Doesn't return the Data Source", p.log);
            sr.IncCounters(dv1.Equals(dv2), "Doesn't return the Data Source", p.log);
            return sr;
        }
        #endregion
    }
}


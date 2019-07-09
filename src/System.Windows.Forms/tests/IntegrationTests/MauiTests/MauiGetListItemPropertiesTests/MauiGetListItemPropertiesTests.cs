// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.ComponentModel;
using System.Collections;
using System.Reflection;
using System.Windows.Forms.IntegrationTests.Common;
using System.Threading;

namespace System.Windows.Forms.IntegrationTests.MauiTests
 {
    public class MauiGetListItemPropertiesTests : ReflectBase
    {
        DataTable dt;
        int count = 0;
        public MauiGetListItemPropertiesTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiGetListItemPropertiesTests(args));
        }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            return base.BeforeScenario(p, scenario);
        }

        public DataTable getDataTable(TParams p)
        {
            DataTable book = new DataTable("Book");
            DataRow dr;
            book.Columns.Add("Name", typeof(string));
            book.Columns.Add("Price", typeof(decimal));
            book.Columns.Add("ProductDate", typeof(DateTime));
            count = p.ru.GetRange(1, 5);
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

        public DataTable getDataTable2(TParams p)
        {
            DataTable novel = new DataTable("Novel");
            DataRow dr;
            novel.Columns.Add("Name", typeof(string));
            novel.Columns.Add("Category", typeof(string));
            for (int i = 0; i < count; i++)
            {
                dr = novel.NewRow();
                dr[0] = "bName" + i.ToString();
                dr[1] = p.ru.GetString(5, 10);
                novel.Rows.Add(dr);
            }
            return novel;
        }

        #region 10 scenarios

        // @Pass in an array.  Verify properties are returned for the Array element type.
        [Scenario(true)]
        public ScenarioResult GetListItemPropertiesInArray(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            GetSampleClass var1 = new GetSampleClass();
            GetSampleClass var2 = new GetSampleClass();
            GetSampleClass[] gsc = new GetSampleClass[2] { var1, var2 };
            PropertyDescriptorCollection pdc = ListBindingHelper.GetListItemProperties(gsc);
            Type type = var1.GetType();
            for (int i = 0; i < pdc.Count; i++)
            {
                PropertyInfo pi = type.GetProperty(pdc[i].Name);
                sr.IncCounters(pdc[i].PropertyType, pi.PropertyType, "The expect doesn't match the actual result", p.log);
            }
            return sr;
        }

        // @Pass in an ITypedList.  Verify results identical to ITypedList.GetItemProperties.  Should use reflection.
        [Scenario(true)]
        public ScenarioResult GetListItemPropertiesInITypedList(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            dt = getDataTable(p);
            ITypedList tl = new DataView(dt);
            PropertyDescriptorCollection pdc1 = ListBindingHelper.GetListItemProperties(tl);
            PropertyDescriptorCollection pdc2 = tl.GetItemProperties(null);
            sr.IncCounters(pdc1.Equals(pdc2), "The result doesn't the same", p.log);
            return sr;
        }

        // @Pass in an object with a typed indexer.  Verify properties are returned for the indexer type.
        [Scenario(true)]
        public ScenarioResult GetListItemPropertiesInIndexer(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            IndexTest index = new IndexTest();
            PropertyDescriptorCollection pdc = ListBindingHelper.GetListItemProperties(index[0]);
            Button btn = new Button();
            Type type = btn.GetType();
            for (int i = 0; i < pdc.Count; i++)
            {
                PropertyInfo pi = type.GetProperty(pdc[i].Name);
                sr.IncCounters(pdc[i].PropertyType, pi.PropertyType, "The expect doesn't match the actual result", p.log);
            }
            return sr;
        }

        // @Pass in a list instance.  Verify returned for the first item in the list.
        [Scenario(true)]
        public ScenarioResult GetListItemPropertiesInArrayList(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            ArrayList al = new ArrayList();
            Button btn = new Button();
            ListBox lb = new ListBox();
            DataGridView dgv = new DataGridView();
            al.Add(btn);
            al.Add(lb);
            al.Add(dgv);
            PropertyDescriptorCollection pdc = ListBindingHelper.GetListItemProperties(al);
            Type type = btn.GetType();
            for (int i = 0; i < pdc.Count; i++)
            {
                PropertyInfo pi = type.GetProperty(pdc[i].Name);
                sr.IncCounters(pdc[i].PropertyType, pi.PropertyType, "The expect doesn't match the actual result", p.log);
            }
            return sr;
        }

        // @Pass in an empty IList.  Verify empty collection is returned.
        [Scenario(true)]
        public ScenarioResult GetListItemPropertiesInEmptyIList(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            ArrayList al = new ArrayList();
            PropertyDescriptorCollection pdc = ListBindingHelper.GetListItemProperties(al);
            sr.IncCounters(0, pdc.Count, "The collection should be empty", p.log);
            return sr;
        }

        // @Pass in an object and PropertyDescriptors.  Verify returns the properties according to the item in the array (i.e., DataSource and array of PropertyDescriptors for the DataSource are passed in.  Should return properties for the first table.
        [Scenario(true)]
        public ScenarioResult GetListItemPropertiesInObject(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            dt = this.getDataTable(p);
            DataSet ds = new DataSet();
            ds.Tables.Add(dt);
            DataTable dt2 = this.getDataTable2(p);
            ds.Tables.Add(dt2);
            ds.Relations.Add("relation1", dt.Columns[0], dt2.Columns[0]);
            PropertyDescriptorCollection pdc = ListBindingHelper.GetListItemProperties(ds.Tables[0], null);
            PropertyDescriptor pd = pdc["relation1"];
            PropertyDescriptorCollection pdc2 = ListBindingHelper.GetListItemProperties(ds.Tables[0], new PropertyDescriptor[] { pd });
            for (int i = 0; i < dt2.Columns.Count; i++)
            {
                sr.IncCounters(pdc2[i].Name, dt2.Columns[i].ColumnName, "Column name is incorrect", p.log);
                sr.IncCounters(pdc2[i].PropertyType, dt2.Columns[i].DataType, "Column's DataType doesn't match PropertyType", p.log);
            }
            return sr;
        }

        // @Pass in array in the wrong order (highest parent up the chain should be last in the array).  Verify error.
        [Scenario(true)]
        public ScenarioResult GetListItemPropertiesInOrder(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            GetSampleClass var1 = new GetSampleClass();
            GetSampleClass var2 = new GetSampleClass();
            GetSampleClass[] gsc = new GetSampleClass[2] { var1, var2 };
            PropertyDescriptorCollection tmp = TypeDescriptor.GetProperties(gsc);
            PropertyDescriptor[] pd = new PropertyDescriptor[tmp.Count];
            for (int i = 0; i < tmp.Count; i++)
                pd[tmp.Count - 1 - i] = tmp[i];
            try
            {
                PropertyDescriptorCollection pdc = ListBindingHelper.GetListItemProperties(gsc, pd);
                sr.IncCounters(false, "Pass in array in the wrong order.It should be error", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(true, ex.Message, p.log);
            }
            return sr;
        }

        // @Pass in array with a bad PropertyDescripter.  Verify error.
        [Scenario(true)]
        public ScenarioResult GetListItemPropertiesInBadOrder(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            //Create an array: gsc
            GetSampleClass var1 = new GetSampleClass();
            GetSampleClass var2 = new GetSampleClass();
            GetSampleClass[] gsc = new GetSampleClass[] { var1, var2 };

            //Create a bad PropertyDescriptor: pd
            PropertyDescriptor[] pd;
            PropertyDescriptorCollection tmp = ListBindingHelper.GetListItemProperties(new ProgressBar());
            pd = new PropertyDescriptor[tmp.Count];
            for (int i = 0; i < tmp.Count; i++)
            { pd[i] = tmp[i]; }

            try
            {
                PropertyDescriptorCollection pdc = ListBindingHelper.GetListItemProperties(gsc, pd);
                sr.IncCounters(false, "The parameter don't match.It should be error", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(true, ex.Message, p.log);
            }
            return sr;
        }

        // @Pass in a type.  Verify no issues.
        [Scenario(true)]
        public ScenarioResult GetListItemPropertiesInType(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            Type type = typeof(Int32);
            try
            {
                PropertyDescriptorCollection pdc = ListBindingHelper.GetListItemProperties(type);
                sr.IncCounters(true, "There are no issues happened", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, ex.Message, p.log);
            }
            return sr;
        }

        // @Pass in an array with too many or too few PropertyDescriptors.  Verify error.
        [Scenario(true)]
        public ScenarioResult GetListItemPropertiesInArrayWithPropertyDescriptors(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            GetSampleClass gsc = new GetSampleClass();
            PropertyDescriptorCollection tmp = TypeDescriptor.GetProperties(gsc);
            PropertyDescriptor[] pd = new PropertyDescriptor[tmp.Count - 1];
            for (int i = 0; i < tmp.Count - 1; i++)
                pd[i] = tmp[i];
            PropertyDescriptorCollection pdc = ListBindingHelper.GetListItemProperties(gsc, pd);
            sr.IncCounters(tmp.Count != pdc.Count, "Pass in an array with too many or too few PropertyDescriptors will be error", p.log);
            return sr;
        }
        #endregion
    }

    public class GetSampleClass
    {
        public int Properties1
        {
            get { return 100; }
        }
        public string Properties2
        {
            get { return "Properties2"; }
        }
    }

    public class IndexTest
    {
    Button btn = new Button();
    public Button this[int index]
    {
        get { return btn; }
    }
    }

 }

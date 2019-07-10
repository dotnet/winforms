// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Data;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.IntegrationTests.Common;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Threading;
using System.Security.Authentication.ExtendedProtection;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiBindingSourceTests : ReflectBase
    {

        #region Testcase setup
        public MauiBindingSourceTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiBindingSourceTests(args));
        }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
        }


        BindingSource dc;
        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            dc = new BindingSource();
            return base.BeforeScenario(p, scenario);
        }

        #endregion

        #region Scenarios
        [Scenario(true)]
        public ScenarioResult Verify(object current, int length)
        {
            ScenarioResult result = new ScenarioResult();
            result.IncCounters(length, dc.Count, "Wrong count", scenarioParams.log);
            if (length > 0)
                result.IncCounters(current, dc.Current, "Wrong current", scenarioParams.log);
            return result;
        }

        //[Scenario("Array instance")]
        [Scenario(true)]
        public ScenarioResult ArrayInstance(TParams p)
        {
            int len = p.ru.GetRange(1, 50);
            string[] str = new string[len];
            for (int i = 0; i < len; i++)
                str[i] = p.ru.GetString(50);
            dc.DataSource = str;

            return Verify(str[0], len);
        }

        //[Scenario("IListSource instance")]
        [Scenario(true)]
        public ScenarioResult IListSourceInstance(TParams p)
        {
            ILS ils = new ILS();
            dc.DataSource = ils;
            return Verify(ils.Buttons[0], ils.Buttons.Count);
        }

        //[Scenario("IBindingList instance")]
        [Scenario(true)]
        public ScenarioResult IBindingListInstance(TParams p)
        {
            BindingList<Button> buttons = new ILS().Buttons;
            dc.DataSource = buttons;
            return Verify(buttons[0], buttons.Count);
        }

        //[Scenario("IList instance")]
        [Scenario(true)]
        public ScenarioResult IListInstance(TParams p)
        {
            Microsoft.VisualBasic.Collection col = new Microsoft.VisualBasic.Collection();
            col.Add("foo", "1", null, null);
            col.Add("foo2", "2", null, null);
            dc.DataSource = col;
            return Verify("foo", 2);
        }

        //[Scenario("non-IEnumerable instance")]
        [Scenario(true)]
        public ScenarioResult NonIEnumerable(TParams p)
        {
            Control c = new Control();
            dc.DataSource = c;
            return Verify(c, 1);
        }

        //[Scenario("ICustomTypeDescriptor instance")]
        [Scenario(true)]
        public ScenarioResult ICustomTypeDescriptorInstance(TParams p)
        {
            CTD ctd = new CTD();
            dc.DataSource = ctd;
            return Verify(ctd, 1);
        }

        //[Scenario("typeof(someArray)")]
        [Scenario(true)]
        public ScenarioResult ArrayType(TParams p)
        {
            dc.DataSource = typeof(string[]);
            return Verify(null, 0);
        }

        //[Scenario("typeof(IListSourceImplemention)")]
        [Scenario(true)]
        public ScenarioResult IListSourceType(TParams p)
        {
            dc.DataSource = typeof(ILS);
            ScenarioResult result = new ScenarioResult();
            result.IncCounters(5, dc.Count, "Wrong count", scenarioParams.log);
            result.IncCounters(typeof(Button), dc.Current.GetType(), "Wrong current", scenarioParams.log);
            return result;
        }

        //[Scenario("typeof(ITypedListImplemention)")]
        [Scenario(true)]
        public ScenarioResult ITypedListType(TParams p)
        {
            dc.DataSource = typeof(BindingSource);
            ScenarioResult result = new ScenarioResult();
            result.IncCounters(0, dc.Count, "Wrong count", scenarioParams.log);
            return result;
        }

        //[Scenario("typeof(IListImplemention)")]
        [Scenario(true)]
        public ScenarioResult IListType(TParams p)
        {
            dc.DataSource = typeof(Microsoft.VisualBasic.Collection);
            return Verify(null, 0);
        }

        //[Scenario("typeof(nonIEnumerableImplemention)")]
        [Scenario(true)]
        public ScenarioResult RandomType(TParams p)
        {
            dc.DataSource = typeof(Control);
            return Verify(null, 0);
        }

        //[Scenario("typeof(IEnumerableImplemention)")]
        [Scenario(true)]
        public ScenarioResult IEnumerableType(TParams p)
        {
            dc.DataSource = typeof(Queue);
            ScenarioResult result = new ScenarioResult();
            result.IncCounters(0, dc.Count, "Wrong count", scenarioParams.log);
            return result;
        }

        //[Scenario("typeof(ICustomTypeDescriptorImplementation)")]
        [Scenario(true)]
        public ScenarioResult ICustomTypeDescriptorType(TParams p)
        {
            dc.DataSource = typeof(DataRowView);
            ScenarioResult result = new ScenarioResult();
            result.IncCounters(0, dc.Count, "Wrong count", scenarioParams.log);
            return result;
        }

        #endregion
    }
}
#region ILS
class ILS : IListSource
{
    BindingList<Button> buttons;
    public ILS()
    {
        buttons = new BindingList<Button>();
        for (int i = 0; i < 5; i++)
            buttons.Add(new Button());
    }

    bool IListSource.ContainsListCollection
    {
        get
        {
            return true;
        }
    }

    public BindingList<Button> Buttons
    {
        get
        {
            return buttons;
        }
    }


    System.Collections.IList IListSource.GetList()
    {
        return buttons;
    }
}
#endregion

#region CTD
class CTD : CustomTypeDescriptor
{
    public Button Button
    {
        get
        {
            return button;
        }
    }

    Button button;
    public override PropertyDescriptorCollection GetProperties()
    {
        button = new Button();
        Button[] b = new Button[] { button };
        PropertyDescriptorCollection coll = new BindingContext()[b].GetItemProperties();
        PropertyDescriptor pd1, pd2, pd3;
        pd1 = coll["Text"];
        pd2 = coll["Width"];
        pd3 = coll["Height"];
        PropertyDescriptorCollection pdc = new PropertyDescriptorCollection(new PropertyDescriptor[] { pd1, pd2, pd3 });
        return pdc;
    }
}
#endregion


// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Array instance

//@ IListSource instance

//@ IBindingList instance

//@ IList instance

//@ non-IEnumerable instance

//@ ICustomTypeDescriptor instance

//@ typeof(someArray)

//@ typeof(IListSourceImplemention) (throws)

//@ typeof(ITypedListImplemention) (throws)

//@ typeof(IListImplemention)

//@ typeof(nonIEnumerableImplemention)

//@ typeof(IEnumerableImplemention) (throws?)

//@ typeof(ICustomTypeDescriptorImplementation) (throws)


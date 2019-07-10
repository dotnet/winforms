// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Windows.Forms.IntegrationTests.Common;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Threading;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiNotifyIconTests : ReflectBase
    {
        const int MENUITEMS_COUNT = 5;

        NotifyIcon ti;
        ContextMenu cm;
        MenuItem[] mis;

        public MauiNotifyIconTests(String[] args) : base(args)
        {
            this.BringToForeground();
            mis = new MenuItem[MENUITEMS_COUNT];
            for (int i = 0; i < MENUITEMS_COUNT; i++)
            {
                mis[i] = new MenuItem();
                mis[i].Text = "MenuItem #" + i.ToString();
            }

            cm = new ContextMenu(mis);
            ti = new NotifyIcon();
        }

        public static void Main(String[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiNotifyIconTests(args));
        }

        //==========================================
        // Test Methods
        //==========================================
        [Scenario(true)]
        public ScenarioResult VisibleScenario(TParams p)
        {
            p.log.WriteLine("Verify Visible is true by default and can be get/set");

            // TODO: BUG #40461
#if false
		if (! ti.Visible)
			return new ScenarioResult(false, "Visible should be true as default.");
#endif

            for (int i = 0; i < 2; i++)
            {
                bool fVisible = !ti.Visible;
                ti.Visible = fVisible;
                if (ti.Visible != fVisible)
                    return new ScenarioResult(false, "failed to set Visible to " + fVisible.ToString());
            }

            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult TextScenario(TParams p)
        {
            p.log.WriteLine("Verify Text can be get/set");

            string szText = "This is the acceptance test for the WFC NotifyIcon control.";

            ti.Text = szText;
            if (!ti.Text.Equals(szText))
                return new ScenarioResult(false, "failed to set Text.");

            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult IconScenario(TParams p)
        {
            p.log.WriteLine("Verify Icon can be get/set");

            Icon ic;

            try
            {
                ic = new Icon("VS.ico");
                ti.Icon = ic;
                if (!ti.Icon.Equals(ic))
                    return new ScenarioResult(false, "failed to set Icon.");
            }
            catch (Exception)
            {
                return new ScenarioResult(false, "make sure you have VS.ico in the current path.");
            }

            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult ContextMenuScenario(TParams p)
        {
            p.log.WriteLine("Verify ContextMenu can be get/set");

            try
            {
                ti.ContextMenu = cm;
                for (int i = 0; i < MENUITEMS_COUNT; i++)
                {
                    if (!ti.ContextMenu.MenuItems[i].Text.Equals(cm.MenuItems[i].Text))
                        return new ScenarioResult(false, "failed to set ContextMenu.");
                }
            }
            catch (Exception ex)
            {
                return new ScenarioResult(false, "unexpected exception thrown: " + ex.ToString());
            }

            return ScenarioResult.Pass;
        }
    }
}

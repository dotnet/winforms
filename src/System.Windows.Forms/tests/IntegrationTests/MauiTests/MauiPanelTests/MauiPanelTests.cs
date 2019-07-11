// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms.IntegrationTests.Common;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Threading;
using System.Security.Authentication.ExtendedProtection;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiPanelTests : ReflectBase
    {
        Button btn1, btn2, btn3, btn4;
        Panel p1, p2, p3, p4;

        public MauiPanelTests(String[] args) : base(args)
        {
            this.BringToForeground();
        }

        public static void Main(String[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiPanelTests(args));
        }

        [Scenario(true)]
        public ScenarioResult PanelinFormScenario(TParams p)
        {
            p.log.WriteLine("Verify I can add a panel to a form");

            try
            {
                p1 = new Panel();
                this.Controls.Add(p1);
            }
            catch (Exception)
            {
                return new ScenarioResult(false, "Failed to add panel to form");
            }
            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult ButtoninPanelScenario(TParams p)
        {
            p.log.WriteLine("Verify I can add a button to my panel");

            try
            {
                btn1 = new Button();
                p1.Controls.Add(btn1);
            }
            catch (Exception)
            {
                return new ScenarioResult(false, "Failed to add button to panel");
            }
            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult PanelinPanelScenario(TParams p)
        {
            p.log.WriteLine("Verify I can add a panel to my panel");

            try
            {
                p2 = new Panel();
                p1.Controls.Add(p2);
            }
            catch (Exception)
            {
                return new ScenarioResult(false, "Failed to add panel to panel");
            }
            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult Button2inPanelScenario(TParams p)
        {
            p.log.WriteLine("Verify I can add a button to my panel inside of panel");

            try
            {
                btn2 = new Button();
                p2.Controls.Add(btn2);
            }
            catch (Exception)
            {
                return new ScenarioResult(false, "Failed to add button to panel");
            }
            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult PanelinFormNoHostScenario(TParams p)
        {
            p.log.WriteLine("Verify I can add a panel to a form");

            try
            {
                p3 = new Panel();
            }
            catch (Exception)
            {
                return new ScenarioResult(false, "Failed to create panel");
            }
            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult PanelinPanelNoHostScenario(TParams p)
        {
            p.log.WriteLine("Verify I can add a panel to my panel");

            try
            {
                p4 = new Panel();
                p3.Controls.Add(p4);
            }
            catch (Exception)
            {
                return new ScenarioResult(false, "Failed to add panel to panel");
            }
            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult Button2inPanelNoHostScenario(TParams p)
        {
            p.log.WriteLine("Verify I can add a button to my panel inside of panel");

            try
            {
                btn4 = new Button();
                p4.Controls.Add(btn4);
            }
            catch (Exception)
            {
                return new ScenarioResult(false, "Failed to add button to panel");
            }
            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult ButtoninPanelNoHostScenario(TParams p)
        {
            p.log.WriteLine("Verify I can add a button to my panel");

            try
            {
                btn3 = new Button();
                p3.Controls.Add(btn3);
            }
            catch (Exception)
            {
                return new ScenarioResult(false, "Failed to add button to panel");
            }
            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult RemoveAllScenario(TParams p)
        {
            p.log.WriteLine("Verify I can remove all items");

            try
            {
                this.Controls.Remove(p1);
            }
            catch (Exception)
            {
                return new ScenarioResult(false, "Failed to remove panel");
            }
            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult RemovePanelScenario(TParams p)
        {
            p.log.WriteLine("Verify I can remove panel from panel");

            try
            {
                this.Controls.Add(p1);
                p1.Controls.Remove(p2);
            }
            catch (Exception)
            {
                return new ScenarioResult(false, "Failed to remove panel");
            }
            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult RemoveButtonScenario(TParams p)
        {
            p.log.WriteLine("Verify I can remove panel from panel");

            try
            {
                p1.Controls.Remove(btn1);
            }
            catch (Exception)
            {
                return new ScenarioResult(false, "Failed to remove panel");
            }
            return ScenarioResult.Pass;
        }
    }
}

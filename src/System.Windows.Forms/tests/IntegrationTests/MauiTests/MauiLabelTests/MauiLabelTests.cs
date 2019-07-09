// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.IntegrationTests.Common;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Threading;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiLabelTests : ReflectBase
    {
        Label lbl;
        Size NewSize, OldSize;

        public MauiLabelTests(String[] args) : base(args)
        {
            lbl = new Label();
            lbl.AutoSize = true;
            lbl.Text = "Hello";
            this.Controls.Add(lbl);
        }

        /**
        * Calls static method LaunchTest to start the test
        */
        public static void Main(String[] args)
        {
            Application.Run(new MauiLabelTests(args));
        }


        //==========================================
        // Test Methods
        //==========================================
        [Scenario(true)]
        public ScenarioResult AutoTTScenario(TParams p)
        {
            p.log.WriteLine("Make Sure autosize works on T-T toggel");
            lbl.AutoSize = true;
            lbl.Size = new Size(10, 10);
            lbl.Text = "Hello";
            OldSize = lbl.Size;
            lbl.Text = "Say Hello";
            lbl.AutoSize = true;
            NewSize = lbl.Size;
            return new ScenarioResult(!(NewSize.Equals(OldSize)));
        }

        [Scenario(true)]
        public ScenarioResult AutoTFScenario(TParams p)
        {
            p.log.WriteLine("Make Sure autosize works on T-F toggel");
            lbl.AutoSize = true;
            Size initialSize = new Size(10, 10);
            lbl.Size = initialSize;
            lbl.Text = "Say Hello to me";
            OldSize = lbl.Size;
            lbl.AutoSize = false;
            NewSize = lbl.Size;

            string descr = "";
            if (NewSize.Equals(OldSize))
            {
                descr = "Bug # 43661";
            }

            return new ScenarioResult((!NewSize.Equals(OldSize)) && NewSize.Equals(initialSize), descr, p.log);
        }

        [Scenario(true)]
        public ScenarioResult AutoFFScenario(TParams p)
        {
            p.log.WriteLine("Make Sure autosize works on F-F toggel");
            lbl.AutoSize = false;
            lbl.Size = new Size(10, 10);
            OldSize = lbl.Size;
            lbl.Text = "is there a bug in this code here";
            lbl.AutoSize = false;
            NewSize = lbl.Size;
            return new ScenarioResult(NewSize.Equals(OldSize));
        }

        [Scenario(true)]
        public ScenarioResult AutoFTScenario(TParams p)
        {
            p.log.WriteLine("Make Sure autosize works on F-T toggel");
            lbl.AutoSize = false;
            lbl.Size = new Size(10, 10);
            lbl.Text = "Say Hello";
            OldSize = lbl.Size;
            lbl.AutoSize = true;
            NewSize = lbl.Size;
            return new ScenarioResult(!NewSize.Equals(OldSize));
        }

        [Scenario(true)]
        public ScenarioResult TextAlignEnumScenario(TParams p)
        {
            p.log.WriteLine("Make Sure I can set all Align Enums");
            lbl.AutoSize = true;
            try
            {
                lbl.TextAlign = ContentAlignment.TopLeft;
                lbl.TextAlign = ContentAlignment.TopCenter;
                lbl.TextAlign = ContentAlignment.TopRight;

                lbl.TextAlign = ContentAlignment.MiddleLeft;
                lbl.TextAlign = ContentAlignment.MiddleCenter;
                lbl.TextAlign = ContentAlignment.MiddleRight;

                lbl.TextAlign = ContentAlignment.BottomLeft;
                lbl.TextAlign = ContentAlignment.BottomCenter;
                lbl.TextAlign = ContentAlignment.BottomRight;
            }
            catch (Win32Exception)
            {
                return new ScenarioResult(false, "Failed to set all Alignment enums");
            }
            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult TextAlignValidLiteralScenario(TParams p)
        {
            // all numeric values from ContentAlignment
            int[] contentAlignmentValues = { 1, 2, 4, 16, 32, 64, 256, 512, 1024 };

            p.log.WriteLine("Make Sure I can set all Align Enums via literals");
            lbl.AutoSize = true;
            int len = contentAlignmentValues.Length;
            try
            {
                for (int n = 0; n < len; n++)
                    lbl.TextAlign = (ContentAlignment)contentAlignmentValues[n];
            }
            catch (Win32Exception)
            {
                return new ScenarioResult(false, "Failed to set all Alignment enums");
            }
            return ScenarioResult.Pass;
        }

        [Scenario(true)]
        public ScenarioResult TextAlignInValidLiteralScenario(TParams p)
        {
            p.log.WriteLine("Make Sure I get Exceptions on -1 Align Enums via literals");
            lbl.AutoSize = true;
            try
            {
                int i = -1;
                lbl.TextAlign = (ContentAlignment)i;
            }
            catch (ArgumentException e)
            {
                p.log.WriteLine("exception was caught: " + e.Message);
                return ScenarioResult.Pass;
            }
            catch (Exception e)
            {
                p.log.WriteLine("exception was caught: " + e.Message);
                return new ScenarioResult(false, "FAILED: wrong exception was thrown", p.log);
            }
            return new ScenarioResult(false, "Failed to throw exception on -1 textalignment", p.log);
        }

        [Scenario(true)]
        public ScenarioResult TextAlignInValidLiteral2Scenario(TParams p)
        {
            p.log.WriteLine("Make Sure I get Exceptions on 3 Align Enums via literals");
            lbl.AutoSize = true;
            try
            {
                lbl.TextAlign = (ContentAlignment)3;
            }
            catch (ArgumentException e)
            {
                p.log.WriteLine("exception was caught: " + e.Message);
                return ScenarioResult.Pass;
            }
            catch (Exception e)
            {
                p.log.WriteLine("exception was caught: " + e.Message);
                return new ScenarioResult(false, "FAILED: wrong exception was thrown", p.log);
            }
            return new ScenarioResult(false, "Failed to throw exception on 3 textalignment", p.log);
        }
    }
}
// [Scenarios]
//@ AutoTTScenario()
//@ AutoTFScenario()
//@ AutoFFScenario()
//@ AutoFTScenario()
//@ TextAlignEnumScenario()
//@ TextAlignValidLiteralScenario()
//@ TextAlignInValidLiteralScenario()
//@ TextAlignInValidLiteral2Scenario()

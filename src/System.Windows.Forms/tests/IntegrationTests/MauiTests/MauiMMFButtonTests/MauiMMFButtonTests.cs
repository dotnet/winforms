// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.IntegrationTests.Common;
using ReflectTools;
using WFCTestLib.Util;
using WFCTestLib.Log;
using Maui.Core;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiMMFButtonTests : ReflectBase
    {
        Button button;
        Point pt;
        string backcolor;
        string mouseoverbc;

        #region Testcase setup
        public MauiMMFButtonTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiMMFButtonTests(args));
        }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);

            button = new Button();
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.MouseOverBackColor = Color.Teal;
            this.Controls.Add(button);
            this.button.Left = this.Width / 2 - this.button.Width / 2;
            this.button.Top = this.Height / 2 - this.button.Height / 2;
            pt = new Point((button.Left + button.Right) / 2, (button.Top + button.Bottom) / 2);
            pt = this.PointToScreen(pt);
        }

        private void DragOn()
        {
            Mouse.ClickDrag(pt.X, pt.Y - 50, pt.X, pt.Y);
        }

        private void DragOff()
        {
            Mouse.ClickDrag(pt.X, pt.Y, pt.X, pt.Y - 50);
        }

        #endregion

        #region Scenarios
        [Scenario(true)]
        public ScenarioResult NotFlat(TParams p)
        {
            // MouseOver effects ineffective when FlatStyle <> Flat
            ScenarioResult sr = new ScenarioResult();

            bool found;

            foreach (FlatStyle style in Enum.GetValues(typeof(FlatStyle)))
            {
                Application.DoEvents();
                Mouse.Move(0, 0);
                this.button.FlatStyle = style;
                this.button.Text = style.ToString();
                DragOn();
                Thread.Sleep(500);
                using (Bitmap bmp = Utilities.GetBitmapOfControl(this.button, true))
                {
                    //found = Utilities.ContainsColor(bmp, Color.Teal);

                    Rectangle subRect1 = new Rectangle(0, 0, 75, 23);
                    found = Utilities.ContainsColor(bmp, Color.Teal, 8, subRect1);
                }
                p.log.WriteLine(found.ToString());
                DragOff();
                Thread.Sleep(500);
                sr.IncCounters(found == (style == FlatStyle.Flat),
                    "Value was " + found + " when FlatStyle was " + style.ToString(), p.log);
            }

            this.button.FlatStyle = FlatStyle.Flat;

            return sr;
        }

        [Scenario(true)]
        public ScenarioResult BackColorScenario(TParams p)
        {
            // BackColor unaffected when visual styles are used
            ScenarioResult sr = new ScenarioResult();

            Mouse.Move(0, 0);

            backcolor = this.button.BackColor.ToString();
            mouseoverbc = this.button.FlatAppearance.MouseOverBackColor.ToString();
            sr.IncCounters(backcolor == "Color [Control]", "1) backcolor was " + backcolor, p.log);
            sr.IncCounters(mouseoverbc == "Color [Teal]", "1) mouseoverbc was " + mouseoverbc, p.log);

            pt = new Point((button.Left + button.Right) / 2, (button.Top + button.Bottom) / 2);
            pt = this.PointToScreen(pt);

            DragOn();

            backcolor = this.button.BackColor.ToString();
            mouseoverbc = this.button.FlatAppearance.MouseOverBackColor.ToString();
            sr.IncCounters(backcolor == "Color [Control]", "2) backcolor was " + backcolor, p.log);
            sr.IncCounters(mouseoverbc == "Color [Teal]", "2) mouseoverbc was " + mouseoverbc, p.log);

            DragOff();

            this.button.FlatAppearance.MouseOverBackColor = Color.Teal;
            Application.DoEvents();

            backcolor = this.button.BackColor.ToString();
            mouseoverbc = this.button.FlatAppearance.MouseOverBackColor.ToString();
            sr.IncCounters(backcolor == "Color [Control]", "3) backcolor was " + backcolor, p.log);
            sr.IncCounters(mouseoverbc == "Color [Teal]", "3) mouseoverbc was " + mouseoverbc, p.log);

            DragOn();

            backcolor = this.button.BackColor.ToString();
            mouseoverbc = this.button.FlatAppearance.MouseOverBackColor.ToString();
            sr.IncCounters(backcolor == "Color [Control]", "4) backcolor was " + backcolor, p.log);
            sr.IncCounters(mouseoverbc == "Color [Teal]", "4) mouseoverbc was " + mouseoverbc, p.log);

            return sr;
        }

        [Scenario(true)]
        public ScenarioResult MouseOverBackColor(TParams p)
        {
            // MouseOverBackColor doesn't flip out if same color as default
            DragOff();
            this.button.BackColor = Color.Firebrick;
            this.button.FlatAppearance.MouseOverBackColor = Color.Firebrick;

            bool firebrick;

            using (Bitmap bmp = Utilities.GetBitmapOfControl(this.button, true))
            {
                firebrick = Utilities.ContainsColor(bmp, Color.Firebrick);
            }

            ScenarioResult sr = new ScenarioResult();

            sr.IncCounters(firebrick, "Desired backcolor not found with mouse not over.", p.log);
            DragOn();

            using (Bitmap bmp = Utilities.GetBitmapOfControl(this.button, true))
            {
                firebrick = Utilities.ContainsColor(bmp, Color.Firebrick);
            }

            sr.IncCounters(firebrick, "Desired backcolor not found with mouse over.", p.log);
            return sr;
        }

        [Scenario(true)]
        public ScenarioResult BorderColorScenario(TParams p)
        {
            // BorderColor doesn't flip out if same color as default
            DragOff();
            this.button.BackColor = Color.Gold;
            this.button.FlatAppearance.BorderColor = Color.Gold;
            this.button.FlatAppearance.BorderSize = 5;

            bool gold;

            using (Bitmap bmp = Utilities.GetBitmapOfControl(this.button, true))
            {
                gold = Utilities.ContainsColor(bmp, Color.Gold);
                // bmp.Save("d:\\MMFButtonBorderColor1.bmp");
            }

            ScenarioResult sr = new ScenarioResult();

            sr.IncCounters(gold, "Desired backcolor not found with mouse not over.", p.log);
            DragOn();
            using (Bitmap bmp = Utilities.GetBitmapOfControl(this.button, true))
            {
                gold = Utilities.ContainsColor(bmp, Color.Gold);
                // bmp.Save("d:\\MMFButtonBorderColor2.bmp");
            }

            sr.IncCounters(gold, "Desired backcolor not found with mouse over.", p.log);
            return sr;
        }

        [Scenario(true)]
        public ScenarioResult BorderSizeScenario(TParams p)
        {
            // BorderSize throws InvalidArgumentException or something for negative values and zero
            ScenarioResult sr = new ScenarioResult();

            // Valid value
            this.button.FlatAppearance.BorderSize = 5;
            int size = this.button.FlatAppearance.BorderSize;
            sr.IncCounters(size == 5, "Size should have been 5, but was " + size.ToString(), p.log);

            // Negative
            try
            {
                this.button.FlatAppearance.BorderSize = -5;
                sr.IncCounters(false, "Should have thrown when setting to -5.", p.log);
            }
            catch (System.ArgumentException e)
            {
                e.ToString();
                sr.IncCounters(true);
            }

            // Zero
            try
            {
                this.button.FlatAppearance.BorderSize = 0;
                sr.IncCounters(true);
            }
            catch (System.ArgumentException e)
            {
                e.ToString();
                sr.IncCounters(false, "Should not throw when setting to zero.", p.log);
            }
            return sr;
        }
        #endregion
    }
}


// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using System.Windows.Forms.IntegrationTests.Common;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Drawing;
using Maui.Core;
using System.Threading;
using System.Security.Authentication.ExtendedProtection;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiNumericUpDownAccelerationTests : ReflectBase
    {

        #region Testcase setup
        public MauiNumericUpDownAccelerationTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiNumericUpDownAccelerationTests(args));
        }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            // Create and Initialize component
            InitializeComponent();
        }
        private void InitializeComponent()
        {
            Controls.Clear();
            this.nud = new System.Windows.Forms.NumericUpDown();
            this.nud.Location = new System.Drawing.Point(28, 29);
            this.nud.Size = new System.Drawing.Size(120, 20);
            nud.Maximum = decimal.MaxValue;
            nud.Minimum = decimal.MinValue;
            this.Controls.Add(this.nud);
        }
        private NumericUpDown nud;
        private decimal nudVal1 = 0;
        private decimal nudVal2 = 0;

        private void Reset()
        {
            nudVal1 = 0; nudVal2 = 0;
        }
        private static void Wait(int time)
        {
            for (int i = 0; i < time; i++)
            {
                Application.DoEvents(); System.Threading.Thread.Sleep(100);
            }
        }
        private void Click_UpButton()
        {
            Point pt = new Point(nud.Location.X + nud.Size.Width - 8, nud.Location.Y + 5);
            pt = this.PointToScreen(pt);
            Mouse.MouseEvent(MouseAction.ButtonDown, MouseFlags.LeftButton, pt.X, pt.Y);
            Wait(20);
            nudVal1 = nud.Value;
            Wait(20);
            nudVal2 = nud.Value;
            Mouse.MouseEvent(MouseAction.ButtonUp, MouseFlags.LeftButton, pt.X, pt.Y);
        }
        #endregion

        #region Scenarios
        //@ Apply Acceleration Table with an entry for 0 s 5 i verify that all valid values are multiples of 5
        [Scenario(true)]
        public ScenarioResult ApplyAccelerationTable(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            Reset();
            //nud.AccelerationTable = new NumericUpDownAcceleration[] { new NumericUpDownAcceleration(0, 5) };
            nud.Accelerations.Add(new NumericUpDownAcceleration(0, 5));
            nud.Value = 0;
            Click_UpButton();
            sr.IncCounters((nudVal2 - nudVal1) % 5 == 0, "FAIL: Acceleration was not a multiple of the increment", p.log);
            return sr;
        }

        //@ Set Increment property to 2, verify that AccelerationTable overrides increment.
        [Scenario(true)]
        public ScenarioResult AccelerationTableOverridesIncrement(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(); InitializeComponent();
            Reset();
            //nud.AccelerationTable = new NumericUpDownAcceleration[] { new NumericUpDownAcceleration(0, 5) };
            nud.Accelerations.Add(new NumericUpDownAcceleration(0, 5));
            nud.Value = 0;
            nud.Increment = 2;
            Click_UpButton();
            sr.IncCounters((nudVal2 - nudVal1) % 5 == 0, "FAIL: Acceleration did not override the increment", p.log);
            return sr;
        }

        //@ Apply AccelerationTable with an entry for int.Max s 5 i and set increment=3 verify that all valid values are multiples of 3
        [Scenario(true)]
        public ScenarioResult AccelerationTableAtMax(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(); InitializeComponent();
            Reset();
            //nud.AccelerationTable = new NumericUpDownAcceleration[] { new NumericUpDownAcceleration(50, 5) };
            nud.Accelerations.Add(new NumericUpDownAcceleration(500, 5));
            nud.Value = 0;
            nud.Increment = 3;
            Click_UpButton();
            sr.IncCounters((nudVal2 - nudVal1) % 3 == 0, "FAIL: Acceleration was applied before it's time", p.log);
            return sr;
        }

        //@ Apply AccelerationTable with duplicate entries for 0 s, (higher increment is applied)
        [Scenario(true)]
        public ScenarioResult AccelTableDuplicates(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(); InitializeComponent();
            Reset();
            //nud.AccelerationTable = new NumericUpDownAcceleration[] { new NumericUpDownAcceleration(0, 5), new NumericUpDownAcceleration(0, 6) };
            nud.Accelerations.Add(new NumericUpDownAcceleration(0, 5));
            nud.Accelerations.Add(new NumericUpDownAcceleration(0, 6));
            nud.Value = 0;
            nud.Increment = 3;
            Click_UpButton();
            sr.IncCounters((nudVal2 - nudVal1) % 6 == 0, "FAIL: Acceleration didn't apply larger value", p.log);
            return sr;
        }

        //@ Apply AccelerationTable with 0 s and 4 i set inital value to .5 and verify the decimal is preserved
        [Scenario(true)]
        public ScenarioResult AccelerationTableWithFraction(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            //nud.AccelerationTable = new NumericUpDownAcceleration[] { new NumericUpDownAcceleration(0, 4)};
            nud.Accelerations.Add(new NumericUpDownAcceleration(0, 4));
            nud.Value = 0.5M;
            for (int i = 0; i < 100; ++i)
            {
                nud.UpButton();
                Application.DoEvents();
                //p.log.WriteLine(nud.Value.ToString());
                sr.IncCounters(0.5M == (nud.Value - Math.Floor(nud.Value)), "FAIL: Acceleration didn't preserve the decimal part", p.log);
            }
            return sr;
        }

        //@ Apply AccelerationTable with unsorted time values, and verify that they are sorted
        [Scenario(true)]
        public ScenarioResult AccelerationTableSorting(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            // nud.Value = 0;
            // int numValues = p.ru.GetRange(1, 50);
            // NumericUpDownAcceleration[] accelTable = new NumericUpDownAcceleration[numValues];
            // for (int i = 0; i < accelTable.Length; ++i)
            // {
            //     accelTable[i] = new NumericUpDownAcceleration(p.ru.GetRange(0, int.MaxValue), (decimal)p.ru.GetRange(0, int.MaxValue));
            // }
            // nud.AccelerationTable = accelTable;
            //  accelTable = nud.AccelerationTable;
            // int prevTime = 0;
            // for (int i = 0; i < accelTable.Length; ++i)
            //{
            //    sr.IncCounters(accelTable[i].Seconds >= prevTime, "FAIL: Time was not sorted ascending", p.log);
            //}
            return new ScenarioResult(true);
            //Bug 385672  AccerelationTbale deprecated
        }
        #endregion
    }
}


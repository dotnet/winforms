// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;

using System.Drawing;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms.IntegrationTests.Common;
using System.Threading;

//
//  CheckBox priority 1 tests
//  

//
//  Created by: Hallas - February 2001.
//  Test fix: zhenlwa - January 7, 2007
//

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiCheckBoxTests : ReflectBase
    {

        const int MAX_TEXT_LEN = 256;
        const int MAX_MENU_ITEM = 10;             // max length of string for menu items
        const int MIN_TEXT_LEN = 5;              // minimal Text length when not empty string

        private CheckBox cb;

        bool eventFired = false;              // true if CheckBox.Click was raised
        int eventCount = 0;                  // number of CheckBox.Click events     

        Point pointToClick = new Point(0, 0);

        public MauiCheckBoxTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        /**
        * Calls static method LaunchTest to start the test
        */
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiCheckBoxTests(args));
        }

        protected override void InitTest(TParams p)
        {
            this.Text = "CheckBoxP1";
            base.InitTest(p);

            // Ctreate and Initialize control
            InitControl();

            //zhenlwa: Before the show-up of form, the location of checkbox cannot be calculated correctly.
            //pointToClick = CalculatePointToClick();
        }

        protected void InitControl()
        {
            cb = new CheckBox();
            cb.Location = new Point(10, 10);
            cb.Text = "Tested CheckBox";
            cb.Click += new EventHandler(cbClick);
            this.Controls.Add(cb);
        }

        public void ResetConditions()
        {
            eventFired = false;
            eventCount = 0;
        }

        // Click event 
        private void cbClick(Object sender, EventArgs e)
        {
            eventFired = true;
            eventCount++;
        }

        //==========================================
        // Test Methods
        //==========================================


        //  //////////////////////////////////////////
        //  ///  Autocheck  /////////////////////////
        //  //////////////////////////////////////////

        //
        // it doesn't make any difference if user clicks on text or on check-rectangle within 
        // CheckBox. In any case Checked/CheckState is toggled when CheckOnClick = true.
        // When CheckOnClick = false, mouse clicks should not toggle Checked/CheckState.
        // 
        [Scenario(true)]
        public ScenarioResult AutoCheck(TParams p)
        {
            //zhenlwa: the location of checkbox should be calculated here
            pointToClick = CalculatePointToClick();

            ScenarioResult sr = new ScenarioResult();
            CheckState initCS = cb.CheckState;
            bool initC = cb.Checked;

            //  AutoCheck = false  - no toggling
            p.log.WriteLine(" A. AutoCheck = false");
            cb.AutoCheck = false;
            p.log.WriteLine("   1. ThreeState = false");
            cb.ThreeState = false;
            PerformLeftMouseClick(p, sr);
            IncrementCounters(p, sr, initCS, initC);

            p.log.WriteLine("   2. ThreeState = true");
            cb.ThreeState = true;
            PerformLeftMouseClick(p, sr);
            IncrementCounters(p, sr, initCS, initC);

            // AutoCheck = true - should be toggled
            p.log.WriteLine(" B. AutoCheck = true");
            cb.AutoCheck = true;
            p.log.WriteLine("   1. ThreeState = false");
            cb.ThreeState = false;
            initCS = cb.CheckState;
            initC = cb.Checked;
            PerformLeftMouseClick(p, sr);
            IncrementCounters(p, sr, NextCheckState(initCS), !initC);

            p.log.WriteLine("   2. ThreeState = true");
            cb.ThreeState = true;
            initCS = cb.CheckState;
            bool expC = true;         // expected Checked when clicking ThreeStated checkbox
            if (initCS == CheckState.Indeterminate)
                expC = false;
            else if (initCS == CheckState.Unchecked)
                expC = true;
            PerformLeftMouseClick(p, sr);
            IncrementCounters(p, sr, NextCheckState(initCS), expC);

            return sr;
        }


        //  //////////////////////////////////////////
        //  ///  Checked  /////////////////////////
        //  //////////////////////////////////////////

        //
        //  CheckStates Checked & Indeterminate ~ Checked = true
        //  CheckState Unchecked  ~ Checked = false
        // 

        [Scenario(true)]
        public ScenarioResult CheckedProperty(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            //  from different Checked
            p.log.WriteLine(" A. Checked vs Checked");
            for (int i = 0; i < 2; i++)
            {
                // ThreeState should not affect setting Checked from code
                if (i == 0)
                    cb.ThreeState = false;
                else
                    cb.ThreeState = true;
                p.log.WriteLine(" " + (i + 1) + ". ThreeState = " + cb.ThreeState);

                // testing
                CheckState expCS = cb.CheckState;
                cb.Checked = true;
                p.log.WriteLine("   a. true --> true");
                cb.Checked = true;
                if (expCS == CheckState.Unchecked)
                    expCS = CheckState.Checked;
                IncrementCounters(p, sr, expCS, true);

                p.log.WriteLine("   b. true --> false");
                cb.Checked = false;
                IncrementCounters(p, sr, CheckState.Unchecked, false);

                p.log.WriteLine("   c. false --> false");
                cb.Checked = false;
                IncrementCounters(p, sr, CheckState.Unchecked, false);

                p.log.WriteLine("   d. false --> true");
                cb.Checked = true;
                IncrementCounters(p, sr, CheckState.Checked, true);
            }


            //  from different CheckStates
            p.log.WriteLine(" B. CheckState vs Checked");

            for (int i = 0; i < 2; i++)
            {

                // ThreeState should not affect setting Checked from code
                if (i == 0)
                    cb.ThreeState = false;
                else
                    cb.ThreeState = true;
                p.log.WriteLine(" " + (i + 1) + ". ThreeState = " + cb.ThreeState);

                // testing
                // different initial CheckState
                for (int j = 0; j < 3; j++)
                {
                    cb.CheckState = (CheckState)j;

                    CheckState expCS = cb.CheckState;
                    // setting Checked to true
                    p.log.WriteLine("   " + (j + 1) + ".1. " + ((CheckState)j).ToString() + " --> true");
                    cb.Checked = true;
                    if (expCS == CheckState.Unchecked)        // Unchecked --> Checked, but Indeterminate is not changed
                        expCS = CheckState.Checked;
                    IncrementCounters(p, sr, expCS, true);

                    cb.CheckState = (CheckState)j;
                    // setting Checked to false
                    p.log.WriteLine("   " + (j + 1) + ".1. " + ((CheckState)j).ToString() + " --> true");
                    cb.Checked = false;
                    IncrementCounters(p, sr, CheckState.Unchecked, false);
                }

            }


            return sr;
        }

        //  //////////////////////////////////////////
        //  ///  CheckState  /////////////////////////
        //  //////////////////////////////////////////

        //
        //  CheckStates Checked & Indeterminate ~ Checked = true
        //  CheckState Unchecked  ~ Checked = false
        //  * Setting Checked is tested in CheckedProperty
        // 
        [Scenario(true)]
        public ScenarioResult CheckStateProperty(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // loop through ThreeStates
            for (int i = 0; i < 2; i++)
            {

                // ThreeState should not affect setting Checked from code
                if (i == 0)
                    cb.ThreeState = false;
                else
                    cb.ThreeState = true;
                p.log.WriteLine("    ******* ThreeState = " + cb.ThreeState);

                // loop through initial CheckState
                for (int j = 0; j < 3; j++)
                {

                    // loop through CheckStates to set
                    for (int k = 0; k < 3; k++)
                    {
                        cb.CheckState = (CheckState)j;

                        bool expC = ((CheckState)k == CheckState.Checked) ||
                                    ((CheckState)k == CheckState.Indeterminate);

                        p.log.WriteLine("   " + (j + 1) + "." + (k + 1) + ". " + ((CheckState)j).ToString() + " --> " + ((CheckState)k).ToString());
                        cb.CheckState = (CheckState)k;
                        IncrementCounters(p, sr, (CheckState)k, expC);
                    }

                }

            }            // end of loop for ThreeState


            return sr;
        }


        //  //////////////////////////////////////////
        //  ///  ThreeState  /////////////////////////
        //  //////////////////////////////////////////

        //
        // Verify that changing ThreeState doesn't affect current Checked / CheckState
        // 
        [Scenario(true)]
        public ScenarioResult ThreeState(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // loop through different initial CheckState
            for (int j = 0; j < 3; j++)
            {

                p.log.WriteLine("   " + (j + 1) + ". initially CheckState = " + ((CheckState)j).ToString());
                // loop through ThreeStates to set
                for (int k = 0; k < 2; k++)
                {
                    cb.CheckState = (CheckState)j;
                    bool expC = ((CheckState)j == CheckState.Checked) ||
                                ((CheckState)j == CheckState.Indeterminate);

                    bool value = false;         // initisl ThreeState
                    if (k == 1)
                        value = true;

                    cb.ThreeState = value;        // set initial ThreeState
                    IncrementCounters(p, sr, (CheckState)j, expC);

                    p.log.WriteLine("   " + (j + 1) + "." + (k + 1) + ".1. " + value + " --> " + value);
                    cb.ThreeState = value;
                    IncrementCounters(p, sr, (CheckState)j, expC);

                    p.log.WriteLine("   " + (j + 1) + "." + (k + 1) + ".2. " + value + " --> " + (!value));
                    cb.ThreeState = !value;
                    IncrementCounters(p, sr, (CheckState)j, expC);
                }

            }

            return sr;
        }

        ////////////////////////////////////////////////////////
        //	utils and helpers
        //

        // helper to perform left mouse-click on CheckBox
        // Click is performed in the middle of the CheckBox.ClientArea.
        // Method also checks if Click event was raised as a result of left mouse click.
        // * reaction on right/middle mouse clicks is tested in CheckBoxEvents tests
        //
        // 
        void PerformLeftMouseClick(TParams p, ScenarioResult result)
        {
            ResetConditions();
            Maui.Core.Mouse.Click(pointToClick.X, pointToClick.Y);
            Application.DoEvents();

            result.IncCounters(eventFired, "Failed: no Click event after left mouse click", p.log);
            if (eventFired)
                result.IncCounters(eventCount == 1, "Failed: " + eventCount + " Click events instead of 1", p.log);
        }

        //
        //  Calculate point in the middle CheckBox.ClientArea 
        //         
        protected Point CalculatePointToClick()
        {
            // CheckBox.ClientRectangle in screen coordinates
            Rectangle tempRect = cb.RectangleToScreen(cb.ClientRectangle);

            // calculating point where mouse click is to be performed
            Point resPoint = new Point(tempRect.Left + tempRect.Width / 2, tempRect.Top + tempRect.Height / 2);

            return resPoint;
        }


        //   
        // returnes next CheckState depending on ThreeState - when CheckState is toggled via mouse 
        //      
        protected CheckState NextCheckState(CheckState init)
        {
            int intState = (int)init;

            if (cb.ThreeState)
            {         // can be Checked, Unchecked, Indeterminate
                if (intState == 2)
                    intState = 0;
                else
                    intState++;
            }

            else
            {               // Checked / Unchecked
                if (intState == 0)
                    intState = 1;
                else
                    intState = 0;
            }

            return (CheckState)intState;
        }

        //
        //  Increments Counters:     1) if current CheckState == expected 'cs'
        //                           2) if current Checked == expected 'c'
        // 
        //      
        void IncrementCounters(TParams p, ScenarioResult result, CheckState cs, bool c)
        {
            result.IncCounters(cb.CheckState == cs, "Failed: new CheckState = " +
                            cb.CheckState.ToString() + " instead of " + cs.ToString(), p.log);

            result.IncCounters(cb.Checked == c, "Failed: new Checked = " + cb.Checked + " instead of " + c, p.log);
        }



    }
}


// [Scenarios]
//@ AutoCheck()
//@ CheckedProperty()
//@ CheckStateProperty()
//@ ThreeState()

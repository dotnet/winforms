// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.IntegrationTests.Common;
using ReflectTools;
using ReflectTools.AutoPME;
using WFCTestLib.Util;
using WFCTestLib.Log;
using System.Media;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiXSystemSoundsTests : XObject
    {
        public MauiXSystemSoundsTests(String[] args) : base(args) 
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiXSystemSoundsTests(args));
        }

        protected override Type Class
        {
            get { return typeof(SystemSounds); }
        }

        protected override object CreateObject(TParams p)
        {
            return SystemSounds.Beep;
        }

        [Scenario(true)]
        protected ScenarioResult get_Asterisk(TParams p)
        {
            SystemSounds.Asterisk.Play();
            return new ScenarioResult(true, "get_Asterisk failure");
        }

        [Scenario(true)]
        protected ScenarioResult get_Beep(TParams p)
        {
            SystemSounds.Beep.Play();
            return new ScenarioResult(true, "get_Beep failure");
        }

        [Scenario(true)]
        protected ScenarioResult get_Exclamation(TParams p)
        {
            SystemSounds.Exclamation.Play();
            return new ScenarioResult(true, "get_Exclamation failure");
        }

        [Scenario(true)]
        protected ScenarioResult get_Hand(TParams p)
        {
            SystemSounds.Hand.Play();
            return new ScenarioResult(true, "get_Hand failure");
        }

        [Scenario(true)]
        protected ScenarioResult get_Question(TParams p)
        {
            SystemSounds.Question.Play();
            return new ScenarioResult(true, "get_Question failure");
        }
    }
}



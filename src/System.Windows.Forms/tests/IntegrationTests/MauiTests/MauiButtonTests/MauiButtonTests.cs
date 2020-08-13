// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Forms.IntegrationTests.Common;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiButtonTests : ReflectBase
    {
        private readonly Button _button;
        private bool _wasClicked;

        public MauiButtonTests(string[] args) : base(args)
        {
            this.BringToForeground();
            _button = new Button
            {
                Text = "&Click"
            };
            _button.Click += (x, y) => _wasClicked = true;
            Controls.Add(_button);
            Application.DoEvents();
        }

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiButtonTests(args));
        }

        [Scenario(true)]
        public ScenarioResult Click_Fires_OnClick(TParams p)
        {
            p.log.WriteLine("Pass if click event is fired");
            _button.PerformClick();

            Application.DoEvents();
            Thread.Sleep(300);

            return new ScenarioResult(_wasClicked);
        }

        [Scenario(true)]
        public ScenarioResult Hotkey_Fires_OnClick(TParams p)
        {
            _wasClicked = false;

            p.log.WriteLine("Pass if click event is fired");
            Application.DoEvents();

            p.log.WriteLine("Button is focused: " + _button.Focused);
            p.log.WriteLine("press ALT + C ");

            SendKeys.SendWait("%C");
            Application.DoEvents();
            Thread.Sleep(600);

            return new ScenarioResult(_wasClicked);
        }

        [Scenario(true)]
        public ScenarioResult Hotkey_DoesNotFire_OnClick(TParams p)
        {
            _wasClicked = false;

            p.log.WriteLine("Pass if click event is not fired");
            p.log.WriteLine("Button is focused: " + _button.Focused);
            p.log.WriteLine("press ALT + l ");

            SendKeys.SendWait("%l");
            Application.DoEvents();
            Thread.Sleep(600);

            return new ScenarioResult(!_wasClicked);
        }
    }
}

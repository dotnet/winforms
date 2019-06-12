// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using WFCTestLib.Log;
using ReflectTools;

namespace System.Windows.Forms.Maui.IntegrationTests
{
    public class MauiButtonTest : ReflectBase
    {
        private readonly Button _button;
        private bool _wasClicked = false;

        public MauiButtonTest(string[] args) : base(args)
        {
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
            Application.Run(new MauiButtonTest(args));
        }

#region Test Methods

        public ScenarioResult Click_Fires_OnClick(TParams p)
        {
            p.log.WriteLine("Pass if click event is fired");
            _button.PerformClick();

            Application.DoEvents();
            Thread.Sleep(300);

            return new ScenarioResult(_wasClicked);
        }

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

        #endregion Test Methods
    }
}

// These control what scenarios are executed by Maui, do not remove!!
// [Scenarios]
//@ Click_Fires_OnClick()
//@ Hotkey_Fires_OnClick()
//@ Hotkey_DoesNotFire_OnClick()

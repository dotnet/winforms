// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Forms.IntegrationTests.Common;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiToolStripDropDownTests : ReflectBase
    {
        private readonly SubToolStripDropDown _toolStrip;

        public MauiToolStripDropDownTests(string[] args) : base(args)
        {
            this.BringToForeground();
            _toolStrip = new SubToolStripDropDown();
        }

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiToolStripDropDownTests(args));
        }

        [Scenario(true)]
        public ScenarioResult KeyboardAccelerators_Test(TParams p)
        {
            bool _result = true;

            _toolStrip.Enabled = true; // it needs for correct work of Control.CanProcessMnemonic method
            _toolStrip.Visible = true; //

            _result &= !_toolStrip.ProcessDialogChar('F');

            _toolStrip.DisplayedItems.Add("&First item");
            _toolStrip.DisplayedItems.Add("&Second item");
            _toolStrip.DisplayedItems.Add("Third item");
            _toolStrip.Visible = true; // it needs for correct work of Control.CanProcessMnemonic method

            _result &= _toolStrip.ProcessDialogChar('F');
            _result &= _toolStrip.ProcessDialogChar('S');
            _result &= !_toolStrip.ProcessDialogChar('T');

            return new ScenarioResult(_result);
        }

        private class SubToolStripDropDown : ToolStripDropDown
        {
            public new ToolStripItemCollection DisplayedItems => base.DisplayedItems;

            public new bool ProcessDialogChar(char charCode) => base.ProcessDialogChar(charCode);
        }
    }
}

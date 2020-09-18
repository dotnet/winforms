// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Forms.IntegrationTests.Common;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiPrintPreviewDialogTests : ReflectBase
    {
        private readonly PrintPreviewDialog printPreviewDialog;

        public MauiPrintPreviewDialogTests(string[] args) : base(args)
        {
            this.BringToForeground();
            printPreviewDialog = new PrintPreviewDialog();
        }

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiPrintPreviewDialogTests(args));
        }

        [Scenario(true)]
        public ScenarioResult Hotkey_Ctrl_1(TParams p)
        {
            printPreviewDialog.Show();
            p.log.WriteLine("Press Ctrl+1");
            SendKeys.SendWait("^1");
            return new ScenarioResult(printPreviewDialog.PrintPreviewControl.Rows == 1 && printPreviewDialog.PrintPreviewControl.Columns == 1);
        }

        [Scenario(true)]
        public ScenarioResult Hotkey_Ctrl_2(TParams p)
        {
            printPreviewDialog.Show();
            p.log.WriteLine("Press Ctrl+2");
            SendKeys.SendWait("^2");
            return new ScenarioResult(printPreviewDialog.PrintPreviewControl.Rows == 1 && printPreviewDialog.PrintPreviewControl.Columns == 2);
        }

        [Scenario(true)]
        public ScenarioResult Hotkey_Ctrl_3(TParams p)
        {
            printPreviewDialog.Show();
            p.log.WriteLine("Press Ctrl+3");
            SendKeys.SendWait("^3");
            return new ScenarioResult(printPreviewDialog.PrintPreviewControl.Rows == 1 && printPreviewDialog.PrintPreviewControl.Columns == 3);
        }

        [Scenario(true)]
        public ScenarioResult Hotkey_Ctrl_4(TParams p)
        {
            printPreviewDialog.Show();
            p.log.WriteLine("Press Ctrl+4");
            SendKeys.SendWait("^4");
            return new ScenarioResult(printPreviewDialog.PrintPreviewControl.Rows == 2 && printPreviewDialog.PrintPreviewControl.Columns == 2);
        }

        [Scenario(true)]
        public ScenarioResult Hotkey_Ctrl_5(TParams p)
        {
            printPreviewDialog.Show();
            p.log.WriteLine("Press Ctrl+5");
            SendKeys.SendWait("^5");
            return new ScenarioResult(printPreviewDialog.PrintPreviewControl.Rows == 2 && printPreviewDialog.PrintPreviewControl.Columns == 3);
        }
    }
}

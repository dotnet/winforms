// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using ReflectTools;
using ReflectTools.AutoPME;
using WFCTestLib.Util;
using WFCTestLib.Log;
using System.Diagnostics;
using System.Windows.Forms.IntegrationTests.Common;
using System.Security.Authentication.ExtendedProtection;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiPrintingSecurityChecksTests : ReflectBase
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiPrintingSecurityChecksTests(args));
        }

        public MauiPrintingSecurityChecksTests(String[] args) : base(args)
        {
            this.BringToForeground();
        }

        public System.Windows.Forms.Timer tTimer = new System.Windows.Forms.Timer();
        private PrintDocument GetNonDefaultPrinter(TParams p)
        {
            // changeddate: 06-27-2006, changedby: v-hzhang
            // if only have one printer, it will be the default printer always             
            if (SafeMethods.GetInstalledPrinters().Count <= 1)
            {
                return null;
            }

            PrintDocument doc = new PrintDocument();

            foreach (string printerName in SafeMethods.GetInstalledPrinters())
            {
                SafeMethods.SetPrinterName(doc.PrinterSettings, printerName);
                if (!doc.PrinterSettings.IsDefaultPrinter) break;

            }

            if (doc.PrinterSettings.IsDefaultPrinter && SafeMethods.GetInstalledPrinters().Count > 1)
                return null;


            p.log.WriteLine("Got non-default printer: " + SafeMethods.GetPrinterName(doc.PrinterSettings));

            return doc;
        }

        // AllPrinting lets us do anything, so we'll check it with a non-default printer and no dialog
        [Scenario(true)]
        public ScenarioResult CheckAllPrinting(TParams p)
        {
            PrintDocument doc = GetNonDefaultPrinter(p);
            if (doc == null) return new ScenarioResult(false, "could not get non-default printer");

            ScenarioResult sr = new ScenarioResult();

            //SafeMethods.Print(doc);

            //[v-torend 8/31/2007]
            //When Microsoft Office Docuemnt Writer is set as default printer the save file dialog comes up
            //and no printing happens.
            if (SafeMethods.GetPrinterName(doc.PrinterSettings).Contains("Writer"))
            {
                sr.IncCounters(true);
            }
            else
            {
                SecurityCheck(sr,
                    delegate { doc.Print(); },
                    typeof(PrintDocument).GetMethod("Print"),
                    LibSecurity.AllPrinting);
            }
            return sr;
        }

        // DefaultPrinting allows printing to the default printer settings without showing a dialog
        [Scenario(true)]
        public ScenarioResult CheckDefaultPrinting(TParams p)
        {
            PrintDocument doc = new PrintDocument();
            ScenarioResult sr = new ScenarioResult();

            //When Microsoft Office Docuemnt Writer is set as default printer the save file dialog comes up
            //and no printing happens.
            if (SafeMethods.GetPrinterName(doc.PrinterSettings).Contains("Writer"))
            {
                sr.IncCounters(true);
            }
            else
            {
                SecurityCheck(sr,
                    delegate { doc.Print(); },
                    typeof(PrintDocument).GetMethod("Print"),
                    LibSecurity.DefaultPrinting);
            }

            return sr;
        }

        // SafePrinting requires that you show a dialog before printing
        [Scenario(true)]
        public ScenarioResult CheckSafePrinting(TParams p)
        {
            PrintDocument doc = new PrintDocument();
            PrintDialog dialog = new PrintDialog();
            ScenarioResult sr = new ScenarioResult();
            dialog.PrinterSettings = doc.PrinterSettings;
            p.log.WriteLine("Starting second thread");
            EndFaxAndSendEnterByTimer();
            p.log.WriteLine("calling ShowDialog()");
            dialog.ShowDialog();
            tTimer.Enabled = false;
            //When Microsoft Office Docuemnt Writer is set as default printer the save file dialog comes up
            if (SafeMethods.GetPrinterName(doc.PrinterSettings).Contains("Writer"))
            {
                sr.IncCounters(true);
            }
            else
            {
                SecurityCheck(sr,
                    delegate { doc.Print(); },
                    typeof(PrintDocument).GetMethod("Print"),
                    LibSecurity.SafePrinting);
            }

            return sr;
        }

        public void EndFaxAndSendEnterByTimer()
        {
            tTimer.Interval = 500;
            tTimer.Tick += new System.EventHandler(this.tTimer_Tick);
            tTimer.Enabled = true;
        }

        public void tTimer_Tick(object sender, EventArgs e)
        {
            //1.Kill Process "WFS" related to Fax Setup window by which print dialog is covered.
            //2.Using a timer instead of a thread to send enter key can make sure that print dialog can get enter key even if OS is slow.
            foreach (Process prc in Process.GetProcesses())
            {
                if (prc.ProcessName.ToUpper() == "WFS")
                {
                    prc.Kill();
                    break;
                }
            }

            SendKeys.Send("{Enter}");
        }
    }
}

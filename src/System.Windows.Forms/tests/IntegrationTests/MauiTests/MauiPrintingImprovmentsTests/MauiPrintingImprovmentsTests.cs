// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Reflection;
using ReflectTools;
using ReflectTools.AutoPME;
using WFCTestLib.Util;
using WFCTestLib.Log;
using System.Windows.Forms.IntegrationTests.Common;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiPrintingImprovmentsTests : ReflectBase
    {
        [STAThread]
        static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiPrintingImprovmentsTests(args));
        }

        public MauiPrintingImprovmentsTests(String[] args) : base(args)
        {
            this.BringToForeground();
        }

        // PrinterSettings.IsDirectPrintingSupported - should be true if ExtEscape(CHECKJPEGFORMAT/CHECKPNGFORMAT) succeeds.
        //	NOTE: it would take way too much interop work to actually test if this value is correct.  So we're just gonna
        //		  make sure it doesn't throw.
        [Scenario(true)]
        public ScenarioResult IsDirectPrintingSupported(TParams p)
        {
            PrinterSettings settings = new PrinterSettings();
            ScenarioResult result = new ScenarioResult(true);

            p.log.WriteLine("checking Images...");
            foreach (ImageStyle imageStyle in Enum.GetValues(typeof(ImageStyle)))
            {
                Image image = p.ru.GetImage(imageStyle);
                bool supported = settings.IsDirectPrintingSupported(image);

                p.log.WriteLine(image.RawFormat.ToString() + "(" + supported + ")");
                // NOTE: we can't be sure of this
                //if (imageStyle == ImageStyle.JPG && !supported) { result.IncCounters(false, "JPEG image should be supported", p.log); }
                //else if (supported) { result.IncCounters(false, "JPEG image should be only format supported", p.log); }
            }

            p.log.WriteLine("checking ImageFormats...");
            foreach (PropertyInfo info in typeof(ImageFormat).GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (info.PropertyType != typeof(ImageFormat)) { continue; }

                ImageFormat imageFormat = info.GetGetMethod().Invoke(null, null) as ImageFormat;
                bool supported = settings.IsDirectPrintingSupported(imageFormat);

                p.log.WriteLine(imageFormat.ToString() + "(" + supported + ")");
                // NOTE: we can't be sure of this
                //if (imageFormat == ImageFormat.Jpeg && !supported) { result.IncCounters(false, "JPEG image should be supported", p.log); }
                //else if (supported) { result.IncCounters(false, "JPEG image should be only format supported", p.log); }
            }

            ImageFormat customFormat = new ImageFormat(new Guid());
            bool customSupported = settings.IsDirectPrintingSupported(customFormat);
            p.log.WriteLine(customFormat.ToString() + "(" + customSupported + ")");
            // NOTE: we can't be sure of this
            //result.IncCounters(!customSupported, "JPEG image should be only format supported", p.log);

            return result;
        }

        // PrintEventArgs.PrintAction - check this property for print to printer, print to file and print to preview;  should match method each time.
        [Scenario(true)]
        public ScenarioResult CheckPrintAction(TParams p)
        {
            TestPrintDocument doc = new TestPrintDocument();
            ScenarioResult result = new ScenarioResult();
            tParams = p;

            // check for print to printer
            p.log.WriteLine("PrintAction.PrintToPrinter...");
            printAction = PrintAction.PrintToPrinter;
            beginPaintFailed = false;
            endPaintFailed = false;

            //When Microsoft Office Docuemnt Writer is set as default printer the save file dialog comes up
            //and no printing happens.
            if (SafeMethods.GetPrinterName(doc.PrinterSettings).Contains("Writer"))
            {
                result.IncCounters(true);
            }
            else
            {
                SafeMethods.Print(doc);
            }
            result.IncCounters(!beginPaintFailed, "PrintAction.PrintToPrinter - OnBeginPrint is not correct", p.log);
            result.IncCounters(!endPaintFailed, "PrintAction.PrintToPrinter - OnEndPrint is not correct", p.log);

            // check for print to file
            p.log.WriteLine("PrintAction.PrintToFile...");
            doc.PrinterSettings.PrintToFile = true;
            SafeMethods.SetPrintFileName(doc.PrinterSettings, "test.txt");
            printAction = PrintAction.PrintToFile;
            beginPaintFailed = false;
            endPaintFailed = false;
            SafeMethods.Print(doc);
            result.IncCounters(!beginPaintFailed, "PrintAction.PrintToFile - OnBeginPrint is not correct", p.log);
            result.IncCounters(!endPaintFailed, "PrintAction.PrintToFile - OnEndPrint is not correct", p.log);

            // check for printpreview
            p.log.WriteLine("PrintAction.PrintToPreview...");
            doc.PrintController = new PreviewPrintController();
            printAction = PrintAction.PrintToPreview;
            beginPaintFailed = false;
            endPaintFailed = false;
            SafeMethods.Print(doc);
            result.IncCounters(!beginPaintFailed, "PrintAction.PrintToPreview - OnBeginPrint is not correct", p.log);
            result.IncCounters(!endPaintFailed, "PrintAction.PrintToPreview - OnEndPrint is not correct", p.log);

            // check PrintPreviewDialog
            p.log.WriteLine("PrintPreviewDialog...");
            dlg = new PrintPreviewDialog();
            dlg.Document = doc;
            printAction = PrintAction.PrintToPreview;
            beginPaintFailed = false;
            endPaintFailed = false;
            Thread thread = new Thread(new ThreadStart(delegate { Thread.Sleep(10000); dlg.Invoke((MethodInvoker)delegate { dlg.Close(); }); }));
            thread.Start();
            dlg.ShowDialog();
            result.IncCounters(!beginPaintFailed, "PrintPreviewDialog - OnBeginPrint is not correct", p.log);
            result.IncCounters(!endPaintFailed, "PrintPreviewDialog - OnEndPrint is not correct", p.log);

            return result;
        }

        static PrintAction printAction;
        static bool beginPaintFailed;
        static bool endPaintFailed;
        static PrintPreviewDialog dlg;
        static TParams tParams;

        class TestPrintDocument : PrintDocument
        {
            protected override void OnBeginPrint(PrintEventArgs e)
            {
                tParams.log.WriteLine("  OnBeginPrint(" + e.PrintAction.ToString() + ")");
                beginPaintFailed = (e.PrintAction != MauiPrintingImprovmentsTests.printAction);
                base.OnBeginPrint(e);
            }

            protected override void OnEndPrint(PrintEventArgs e)
            {
                tParams.log.WriteLine("  OnEndPrint(" + e.PrintAction.ToString() + ")");
                endPaintFailed = (e.PrintAction != MauiPrintingImprovmentsTests.printAction);
                base.OnEndPrint(e);
            }

            protected override void OnPrintPage(PrintPageEventArgs e)
            {
                base.OnPrintPage(e);
            }

            protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e)
            {
                base.OnQueryPageSettings(e);
            }
        }
    }
}

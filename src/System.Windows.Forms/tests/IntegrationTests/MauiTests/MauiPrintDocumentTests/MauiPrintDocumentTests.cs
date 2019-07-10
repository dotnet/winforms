// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Runtime.InteropServices;
using ReflectTools;
using ReflectTools.AutoPME;
using WFCTestLib.Util;
using WFCTestLib.Log;
using System.Windows.Forms.IntegrationTests.Common;
using System.Threading;
using System.Security.Authentication.ExtendedProtection;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiPrintDocumentTests : ReflectBase
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiPrintDocumentTests(args));
        }

        public MauiPrintDocumentTests(String[] args) : base(args)
        {
            this.BringToForeground();
        }

        [Scenario(true)]
        public ScenarioResult DoCheck(TParams p)
        {
            TestPrintDocument doc = new TestPrintDocument();
            result = new ScenarioResult();
            param = p;

            p.log.WriteLine("***same TestPrintDocument instances***");
            p.log.WriteLine("[OriginAtMargins = false]");
            doc.OriginAtMargins = false;
            doc.PrintController = new PreviewPrintController();
            SafeMethods.Print(doc);

            p.log.WriteLine();
            p.log.WriteLine("[OriginAtMargins = true]");
            doc.OriginAtMargins = true;
            doc.PrintController = new PreviewPrintController();
            SafeMethods.Print(doc);

            p.log.WriteLine();
            p.log.WriteLine("***different TestPrintDocument instances***");
            p.log.WriteLine("[OriginAtMargins = false]");
            doc = new TestPrintDocument();
            doc.OriginAtMargins = false;
            doc.PrintController = new PreviewPrintController();
            SafeMethods.Print(doc);

            p.log.WriteLine();
            p.log.WriteLine("[OriginAtMargins = true]");
            doc = new TestPrintDocument();
            doc.OriginAtMargins = true;
            doc.PrintController = new PreviewPrintController();
            SafeMethods.Print(doc);

            return result;
        }

        static ScenarioResult result;
        static TParams param;

        class TestPrintDocument : PrintDocument
        {
            protected override void OnBeginPrint(PrintEventArgs e)
            {
                base.OnBeginPrint(e);
            }

            protected override void OnEndPrint(PrintEventArgs e)
            {
                base.OnEndPrint(e);
            }

            protected override void OnPrintPage(PrintPageEventArgs e)
            {
                Graphics normalMeasurementGraphics = e.PageSettings.PrinterSettings.CreateMeasurementGraphics(false);
                Graphics originMeasurementGraphics = e.PageSettings.PrinterSettings.CreateMeasurementGraphics(true);
                PointF visibleClipBoundsOriginShouldBe;
                float tolerance = 0.0001f; // needed for the floating point errors

                param.log.WriteLine("e.Graphics.VisibleClipBounds=" + e.Graphics.VisibleClipBounds.ToString());
                param.log.WriteLine("normalMeasurementGraphics.VisibleClipBounds=" + normalMeasurementGraphics.VisibleClipBounds.ToString());
                param.log.WriteLine("originMeasurementGraphics.VisibleClipBounds=" + originMeasurementGraphics.VisibleClipBounds.ToString());

                if (OriginAtMargins)
                {
                    visibleClipBoundsOriginShouldBe = new PointF(0 - e.PageSettings.Margins.Left + e.PageSettings.HardMarginX, 0 - e.PageSettings.Margins.Top + e.PageSettings.HardMarginY);
                    param.log.WriteLine("visibleClipBoundsOriginShouldBe=" + visibleClipBoundsOriginShouldBe.ToString());

                    result.IncCounters(originMeasurementGraphics.VisibleClipBounds == e.Graphics.VisibleClipBounds, "FAIL: originMeasurementGraphics.VisibleClipBounds != e.Graphics.VisibleClipBounds", param.log);
                    result.IncCounters(normalMeasurementGraphics.VisibleClipBounds != e.Graphics.VisibleClipBounds, "FAIL: normalMeasurementGraphics.VisibleClipBounds == e.Graphics.VisibleClipBounds", param.log);

                    // VisibleClipBounds tends to have floating point errors, while visibleClipBoundsOriginShouldBe doesn't
                    if (originMeasurementGraphics.VisibleClipBounds.Location.X - tolerance < visibleClipBoundsOriginShouldBe.X ||
                        originMeasurementGraphics.VisibleClipBounds.Location.X + tolerance > visibleClipBoundsOriginShouldBe.X ||
                        originMeasurementGraphics.VisibleClipBounds.Location.Y - tolerance < visibleClipBoundsOriginShouldBe.Y ||
                        originMeasurementGraphics.VisibleClipBounds.Location.Y + tolerance > visibleClipBoundsOriginShouldBe.Y)
                    {
                        result.IncCounters(true, "FAIL: originMeasurementGraphics.VisibleClipBounds.Location != visibleClipBoundsOriginShouldBe", param.log);
                    }

                    // VisibleClipBounds tends to have floating point errors, while visibleClipBoundsOriginShouldBe doesn't
                    if (normalMeasurementGraphics.VisibleClipBounds.Location.X - tolerance >= visibleClipBoundsOriginShouldBe.X &&
                        normalMeasurementGraphics.VisibleClipBounds.Location.X + tolerance <= visibleClipBoundsOriginShouldBe.X &&
                        normalMeasurementGraphics.VisibleClipBounds.Location.Y - tolerance >= visibleClipBoundsOriginShouldBe.Y &&
                        normalMeasurementGraphics.VisibleClipBounds.Location.Y + tolerance <= visibleClipBoundsOriginShouldBe.Y)
                    {
                        result.IncCounters(true, "FAIL: normalMeasurementGraphics.VisibleClipBounds.Location == visibleClipBoundsOriginShouldBe", param.log);
                    }
                }
                else
                {
                    visibleClipBoundsOriginShouldBe = new PointF(0, 0);
                    param.log.WriteLine("visibleClipBoundsOriginShouldBe=" + visibleClipBoundsOriginShouldBe.ToString());

                    result.IncCounters(originMeasurementGraphics.VisibleClipBounds != e.Graphics.VisibleClipBounds, "FAIL: originMeasurementGraphics.VisibleClipBounds == e.Graphics.VisibleClipBounds", param.log);
                    result.IncCounters(normalMeasurementGraphics.VisibleClipBounds == e.Graphics.VisibleClipBounds, "FAIL: normalMeasurementGraphics.VisibleClipBounds != e.Graphics.VisibleClipBounds", param.log);

                    // VisibleClipBounds tends to have floating point errors, while visibleClipBoundsOriginShouldBe doesn't
                    if (originMeasurementGraphics.VisibleClipBounds.Location.X - tolerance >= visibleClipBoundsOriginShouldBe.X &&
                        originMeasurementGraphics.VisibleClipBounds.Location.X + tolerance <= visibleClipBoundsOriginShouldBe.X &&
                        originMeasurementGraphics.VisibleClipBounds.Location.Y - tolerance >= visibleClipBoundsOriginShouldBe.Y &&
                        originMeasurementGraphics.VisibleClipBounds.Location.Y + tolerance <= visibleClipBoundsOriginShouldBe.Y)
                    {
                        result.IncCounters(true, "FAIL: originMeasurementGraphics.VisibleClipBounds.Location != visibleClipBoundsOriginShouldBe", param.log);
                    }

                    // VisibleClipBounds tends to have floating point errors, while visibleClipBoundsOriginShouldBe doesn't
                    if (normalMeasurementGraphics.VisibleClipBounds.Location.X - tolerance < visibleClipBoundsOriginShouldBe.X ||
                        normalMeasurementGraphics.VisibleClipBounds.Location.X + tolerance > visibleClipBoundsOriginShouldBe.X ||
                        normalMeasurementGraphics.VisibleClipBounds.Location.Y - tolerance < visibleClipBoundsOriginShouldBe.Y ||
                        normalMeasurementGraphics.VisibleClipBounds.Location.Y + tolerance > visibleClipBoundsOriginShouldBe.Y)
                    {
                        result.IncCounters(true, "FAIL: normalMeasurementGraphics.VisibleClipBounds.Location == visibleClipBoundsOriginShouldBe", param.log);
                    }
                }

                base.OnPrintPage(e);
            }

            protected override void OnQueryPageSettings(QueryPageSettingsEventArgs e)
            {
                base.OnQueryPageSettings(e);
            }

        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using ReflectTools;
using ReflectTools.AutoPME;
using WFCTestLib.Util;
using WFCTestLib.Log;
using System.Windows.Forms.IntegrationTests.Common;

/// <summary>
/// Summary description for Class1.
/// </summary>
/// 
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    class MauiDblBuffSecurityChecksTests : ReflectBase
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDblBuffSecurityChecksTests(args));
        }

        public MauiDblBuffSecurityChecksTests(String[] args) : base(args)
        {
            this.BringToForeground();
        }

        [Scenario(true)]
        public ScenarioResult CheckRenderDirectlyToDC(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            using (Graphics gfx = this.CreateGraphics())
            {
                BufferedGraphics buff = SafeMethods.BufferedGraphicsAllocate(gfx, new Rectangle(0, 0, 100, 100));
                using (Graphics gfx2 = this.CreateGraphics())
                {
                    IntPtr hdc = SafeMethods.GetHdc(gfx2);

                    SecurityCheck(sr,
                        delegate { buff.Render(hdc); },
                        typeof(BufferedGraphics).GetMethod("Render", new Type[] { typeof(IntPtr) }),
                        LibSecurity.UnmanagedCode);

                    SafeMethods.ReleaseHdc(gfx2, hdc);
                }
            }

            return sr;
        }

        [Scenario(true)]
        public ScenarioResult CheckDCAccess(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true);

            using (Graphics gfx = this.CreateGraphics())
            {
                BufferedGraphics buff = SafeMethods.BufferedGraphicsAllocate(gfx, new Rectangle(0, 0, 100, 100));
                IntPtr hdc = IntPtr.Zero;

                bool success = SecurityCheck(sr,
                        delegate { hdc = buff.Graphics.GetHdc(); },
                        null, // LinkDemand
                        LibSecurity.UnmanagedCode);

                if (success) { SafeMethods.ReleaseHdc(buff.Graphics, hdc); }
            }

            return sr;
        }

        [Scenario(true)]
        public ScenarioResult CheckBufferAllocation(TParams p)
        {
            ScenarioResult sr = new ScenarioResult(true);

            SecurityCheck(sr,
                        //delegate { BufferedGraphicsManager.Current.MaximumBuffer = new Size(int.MaxValue - 1, int.MaxValue - 1); },
                        delegate { BufferedGraphicsManager.Current.MaximumBuffer = new Size(int.MaxValue, 1); }, // [v-jush 02/17/2012] to adapt new bundary with multiply of width and height is less than or equals int.MaxValue
                        typeof(BufferedGraphics).GetMethod("set_MaximumBuffer"),
                        LibSecurity.AllWindows);

            return sr;
        }
    }
}

// [Scenarios]
//@ CheckRenderDirectlyToDC()
//@ CheckDCAccess()
//@ CheckBufferAllocation()

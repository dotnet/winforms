// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.IntegrationTests.Common;
using ReflectTools;
using ReflectTools.AutoPME;
using WFCTestLib.Util;
using WFCTestLib.Log;
using System.Threading;
using System.Security.Authentication.ExtendedProtection;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    /// <summary>
    /// NotifyIcon should throw a security exception if you try to instantiate it without AllWindows.
    /// </summary>
    public class MauiNotifyIconSecurityTests : ReflectBase
    {
        public MauiNotifyIconSecurityTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiNotifyIconSecurityTests(args));
        }

        //@ Verify NotifyIcon can't be instantiated without AllWindows permission
        [Scenario(true)]
        public ScenarioResult SecurityTest(TParams p)
        {
            // AddRequiredPermission() is obsolete, so use BeginSecurityCheck() to replace.
            //AddRequiredPermission(LibSecurity.AllWindows);

            BeginSecurityCheck(LibSecurity.AllWindows);
            NotifyIcon icon = new NotifyIcon();

            return new ScenarioResult(Utilities.HavePermission(LibSecurity.AllWindows), "This should fail without AllWindows", p.log);
        }
    }
}

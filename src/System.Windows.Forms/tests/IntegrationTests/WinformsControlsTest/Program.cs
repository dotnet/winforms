// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);

            //Application.Defaults.SetDefaultFont(new Font(new FontFamily("Microsoft Sans Serif"), 8f));
            //Application.Defaults.SetDefaultFont(new Font(new FontFamily("Chiller"), 12f));
            Application.Defaults.SetDefaultFont(new Font(new FontFamily("Calibri"), 11f));

            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException); //UnhandledExceptionMode.ThrowException
            Thread.CurrentThread.CurrentUICulture = Thread.CurrentThread.CurrentCulture;
            try
            {
                Application.Run(new MainForm());
            }
            catch (System.Exception)
            {
                Environment.Exit(-1);
            }

            Environment.Exit(0);
        }
    }
}

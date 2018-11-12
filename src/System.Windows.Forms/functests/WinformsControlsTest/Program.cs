using System;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.ThrowException); //UnhandledExceptionMode.ThrowException
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

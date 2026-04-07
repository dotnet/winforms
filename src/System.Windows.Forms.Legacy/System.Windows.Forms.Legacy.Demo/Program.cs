#if NET
using System.Windows.Forms;
#endif

namespace Demo
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
#if NET
            Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
#endif
            Application.Run(new MainForm());
        }
    }
}

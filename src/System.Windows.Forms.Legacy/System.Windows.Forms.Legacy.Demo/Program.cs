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
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Set High DPI mode to DpiUnaware, as currently there are some scaling issues when setting to other values
            // https://github.com/WiseTechGlobal/Modernization.Content/blob/main/Controls/work-items/Difference%20display%20between%20migrated%20forms%20and%20original%20forms.md
#if NET
            Application.SetHighDpiMode(HighDpiMode.DpiUnaware);
#endif
            //ApplicationConfiguration.Initialize();
            Application.Run(new MainForm());
        }
    }
}

#if NETFRAMEWORK
namespace System.Runtime.Versioning
{
    class SupportedOSPlatformAttribute : Attribute
    {
        public SupportedOSPlatformAttribute(string platformName) { }
    }
}
#endif

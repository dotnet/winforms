using System.Runtime.InteropServices;

namespace System.Windows.Forms.FuncTests
{
    public class ExternalTestHelpers
    {
        [DllImport("User32.dll")]
        static extern int SetForegroundWindow(IntPtr point);

        [DllImport("User32.dll")]
        static extern IntPtr GetForegroundWindow();

        public static int TrySetForegroundWindow(IntPtr point)
        {
            return SetForegroundWindow(point);
        }

        public static IntPtr TryGetForegroundWindow()
        {
            return GetForegroundWindow();
        }
    }
}

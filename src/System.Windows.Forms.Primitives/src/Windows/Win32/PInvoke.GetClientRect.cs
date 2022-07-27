using Windows.Win32.Foundation;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static BOOL GetClientRect(IHandle hWnd, out RECT lpRect)
        {
            BOOL result = GetClientRect((HWND)hWnd.Handle, out lpRect);
            GC.KeepAlive(hWnd);
            return result;
        }
    }
}

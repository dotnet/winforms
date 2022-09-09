// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32)]
        public static extern int SetScrollInfo(IntPtr hWnd, SB nBar, ref SCROLLINFO lpsi, BOOL redraw);

        public static int SetScrollInfo(IHandle hWnd, SB nBar, ref SCROLLINFO lpsi, BOOL redraw)
        {
            int result = SetScrollInfo(hWnd.Handle, nBar, ref lpsi, redraw);
            GC.KeepAlive(hWnd);
            return result;
        }
    }
}

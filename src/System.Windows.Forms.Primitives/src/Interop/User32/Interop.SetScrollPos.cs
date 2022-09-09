// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, SetLastError = true)]
        public static extern int SetScrollPos(IntPtr hWnd, SB nBar, int nPos, BOOL bRedraw);

        public static int SetScrollPos(IHandle hWnd, SB nBar, int nPos, BOOL bRedraw)
        {
            int result = SetScrollPos(hWnd.Handle, nBar, nPos, bRedraw);
            GC.KeepAlive(hWnd);
            return result;
        }
    }
}

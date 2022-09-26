// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32)]
        public static unsafe extern BOOL RedrawWindow(IntPtr hWnd, RECT* lprcUpdate = default, HRGN hrgnUpdate = default, RDW flags = default);

        public unsafe static BOOL RedrawWindow(IHandle hWnd, RECT* lprcUpdate = default, HRGN hrgnUpdate = default, RDW flags = default)
        {
            BOOL result = RedrawWindow(hWnd.Handle, lprcUpdate, hrgnUpdate, flags);
            GC.KeepAlive(hWnd);
            return result;
        }
    }
}

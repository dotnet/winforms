// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true)]
        private static extern BOOL InvalidateRgn(IntPtr hWnd, IntPtr hrgn, BOOL erase);

        public static BOOL InvalidateRgn(IHandle hWnd, HandleRef hrgn, BOOL erase)
        {
            BOOL result = InvalidateRgn(hWnd.Handle, hrgn.Handle, erase);
            GC.KeepAlive(hWnd);
            GC.KeepAlive(hrgn.Wrapper);
            return result;
        }
    }
}

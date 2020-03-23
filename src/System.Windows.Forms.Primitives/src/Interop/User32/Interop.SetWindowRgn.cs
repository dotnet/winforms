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
        private static extern int SetWindowRgn(IntPtr hwnd, IntPtr hrgn, BOOL fRedraw);

        public static int SetWindowRgn(IHandle hwnd, HandleRef hrgn, BOOL fRedraw)
        {
            int result = SetWindowRgn(hwnd.Handle, hrgn.Handle, fRedraw);
            GC.KeepAlive(hwnd);
            GC.KeepAlive(hrgn.Wrapper);
            return result;
        }

        public static int SetWindowRgn(IHandle hwnd, IntPtr hrgn, BOOL fRedraw)
        {
            int result = SetWindowRgn(hwnd.Handle, hrgn, fRedraw);
            GC.KeepAlive(hwnd);
            return result;
        }
    }
}

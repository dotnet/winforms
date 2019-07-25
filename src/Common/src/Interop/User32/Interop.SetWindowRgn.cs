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
        public static extern int SetWindowRgn(IntPtr hwnd, IntPtr hrgn, bool fRedraw);

        public static int SetWindowRgn(HandleRef hwnd, IntPtr hrgn, bool fRedraw)
        {
            int result = SetWindowRgn(hwnd.Handle, hrgn, fRedraw);
            GC.KeepAlive(hwnd.Wrapper);
            return result;
        }

        public static int SetWindowRgn(HandleRef hwnd, HandleRef hrgn, bool fRedraw)
        {
            int result = SetWindowRgn(hwnd.Handle, hrgn.Handle, fRedraw);
            GC.KeepAlive(hwnd.Wrapper);
            GC.KeepAlive(hrgn.Wrapper);
            return result;
        }
    }
}

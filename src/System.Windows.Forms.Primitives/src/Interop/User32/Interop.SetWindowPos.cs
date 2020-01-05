// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public static IntPtr HWND_TOP = (IntPtr)0;
        public static IntPtr HWND_BOTTOM = (IntPtr)1;
        public static IntPtr HWND_TOPMOST = (IntPtr)(-1);
        public static IntPtr HWND_NOTOPMOST = (IntPtr)(-2);
        public static IntPtr HWND_MESSAGE = (IntPtr)(-3);

        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern BOOL SetWindowPos(
            IntPtr hWnd,
            IntPtr hWndInsertAfter,
            int x = 0,
            int y = 0,
            int cx = 0,
            int cy = 0,
            SWP flags = (SWP)0);

        public static BOOL SetWindowPos(
            HandleRef hWnd,
            IntPtr hWndInsertAfter,
            int x = 0,
            int y = 0,
            int cx = 0,
            int cy = 0,
            SWP flags = (SWP)0)
        {
            BOOL result = SetWindowPos(hWnd.Handle, hWndInsertAfter, x, y, cx, cy, flags);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }

        public static BOOL SetWindowPos(
            HandleRef hWnd,
            HandleRef hWndInsertAfter,
            int x = 0,
            int y = 0,
            int cx = 0,
            int cy = 0,
            SWP flags = (SWP)0)
        {
            BOOL result = SetWindowPos(hWnd.Handle, hWndInsertAfter.Handle, x, y, cx, cy, flags);
            GC.KeepAlive(hWnd.Wrapper);
            GC.KeepAlive(hWndInsertAfter.Wrapper);
            return result;
        }
    }
}

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
        public static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);

        public static IntPtr SetParent(IntPtr hWnd, HandleRef hWndParent)
        {
            IntPtr result = SetParent(hWnd, hWndParent.Handle);
            GC.KeepAlive(hWndParent.Handle);
            return result;
        }

        public static IntPtr SetParent(HandleRef hWnd, IntPtr hWndParent)
        {
            IntPtr result = SetParent(hWnd.Handle, hWndParent);
            GC.KeepAlive(hWnd.Handle);
            return result;
        }

        public static IntPtr SetParent(HandleRef hWnd, HandleRef hWndParent)
        {
            IntPtr result = SetParent(hWnd.Handle, hWndParent.Handle);
            GC.KeepAlive(hWnd.Handle);
            GC.KeepAlive(hWndParent.Handle);
            return result;
        }
    }
}

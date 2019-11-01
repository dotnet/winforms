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
        public static extern BOOL IsChild(IntPtr hWndParent, IntPtr hWnd);

        public static BOOL IsChild(IntPtr hWndParent, HandleRef hWnd)
        {
            BOOL result = IsChild(hWndParent, hWnd.Handle);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }

        public static BOOL IsChild(HandleRef hWndParent, IntPtr hWnd)
        {
            BOOL result = IsChild(hWndParent.Handle, hWnd);
            GC.KeepAlive(hWndParent.Wrapper);
            return result;
        }

        public static BOOL IsChild(HandleRef hWndParent, HandleRef hWnd)
        {
            BOOL result = IsChild(hWndParent.Handle, hWnd.Handle);
            GC.KeepAlive(hWndParent.Wrapper);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}

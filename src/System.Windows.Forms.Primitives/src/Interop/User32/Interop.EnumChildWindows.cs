// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public delegate BOOL EnumChildWindowsCallback(IntPtr hwnd, IntPtr lParam);

        [DllImport(Libraries.User32, ExactSpelling = true)]
        public static extern BOOL EnumChildWindows(IntPtr hwndParent, EnumChildWindowsCallback lpEnumFunc, IntPtr lParam);

        public static BOOL EnumChildWindows(IHandle hwndParent, EnumChildWindowsCallback lpEnumFunc, IntPtr lParam)
        {
            BOOL result = EnumChildWindows(hwndParent.Handle, lpEnumFunc, lParam);
            GC.KeepAlive(hwndParent);
            return result;
        }

        public static BOOL EnumChildWindows(HandleRef hwndParent, EnumChildWindowsCallback lpEnumFunc, IntPtr lParam)
        {
            BOOL result = EnumChildWindows(hwndParent.Handle, lpEnumFunc, lParam);
            GC.KeepAlive(hwndParent.Wrapper);
            return result;
        }
    }
}

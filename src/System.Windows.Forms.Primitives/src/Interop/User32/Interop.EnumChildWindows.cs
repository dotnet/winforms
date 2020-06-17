// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public delegate BOOL EnumChildWindowsCallback(IntPtr hWnd);

        private delegate BOOL EnumChildWindowsNativeCallback(IntPtr hWnd, IntPtr lParam);

        private static readonly EnumChildWindowsNativeCallback s_enumChildWindowsNativeCallback = HandleEnumChildWindowsNativeCallback;

        [DllImport(Libraries.User32, ExactSpelling = true)]
        private static extern BOOL EnumChildWindows(IntPtr hwndParent, EnumChildWindowsNativeCallback lpEnumFunc, IntPtr lParam);

        public static BOOL EnumChildWindows(IntPtr hwndParent, EnumChildWindowsCallback lpEnumFunc)
        {
            // We pass a static delegate to the native function and supply the callback as
            // reference data, so that the CLR doesn't need to generate a native code block for
            // each callback delegate instance (for storing the closure pointer).
            var gcHandle = GCHandle.Alloc(lpEnumFunc);
            try
            {
                return EnumChildWindows(hwndParent, s_enumChildWindowsNativeCallback, GCHandle.ToIntPtr(gcHandle));
            }
            finally
            {
                gcHandle.Free();
            }
        }

        public static BOOL EnumChildWindows(IHandle hwndParent, EnumChildWindowsCallback lpEnumFunc)
        {
            BOOL result = EnumChildWindows(hwndParent.Handle, lpEnumFunc);
            GC.KeepAlive(hwndParent);
            return result;
        }

        public static BOOL EnumChildWindows(HandleRef hwndParent, EnumChildWindowsCallback lpEnumFunc)
        {
            BOOL result = EnumChildWindows(hwndParent.Handle, lpEnumFunc);
            GC.KeepAlive(hwndParent.Wrapper);
            return result;
        }

        private static BOOL HandleEnumChildWindowsNativeCallback(IntPtr hWnd, IntPtr lParam)
        {
            return ((EnumChildWindowsCallback)GCHandle.FromIntPtr(lParam).Target!)(hWnd);
        }
    }
}

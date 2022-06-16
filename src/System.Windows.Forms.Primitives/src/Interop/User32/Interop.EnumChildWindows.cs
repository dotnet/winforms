// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public delegate BOOL EnumChildWindowsCallback(IntPtr hWnd);

        [LibraryImport(Libraries.User32)]
        private static unsafe partial BOOL EnumChildWindows(IntPtr hwndParent, delegate* unmanaged<IntPtr, IntPtr, BOOL> lpEnumFunc, IntPtr lParam);

        public static unsafe BOOL EnumChildWindows(IntPtr hwndParent, EnumChildWindowsCallback lpEnumFunc)
        {
            // We pass a function pointer to the native function and supply the callback as
            // reference data, so that the CLR doesn't need to generate a native code block for
            // each callback delegate instance (for storing the closure pointer).
            var gcHandle = GCHandle.Alloc(lpEnumFunc);
            try
            {
                return EnumChildWindows(hwndParent, &HandleEnumChildWindowsNativeCallback, (IntPtr)gcHandle);
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

        [UnmanagedCallersOnly]
        private static BOOL HandleEnumChildWindowsNativeCallback(IntPtr hWnd, IntPtr lParam)
        {
            return ((EnumChildWindowsCallback)((GCHandle)lParam).Target!)(hWnd);
        }
    }
}

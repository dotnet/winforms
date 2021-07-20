// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public delegate BOOL EnumWindowsCallback(IntPtr hWnd);

        [DllImport(Libraries.User32, ExactSpelling = true, SetLastError = true)]
        private static extern unsafe BOOL EnumWindows(delegate* unmanaged<IntPtr, IntPtr, BOOL> lpEnumFunc, IntPtr lParam);

        public static unsafe BOOL EnumWindows(EnumWindowsCallback lpEnumFunc)
        {
            // We pass a function pointer to the native function and supply the callback as
            // reference data, so that the CLR doesn't need to generate a native code block for
            // each callback delegate instance (for storing the closure pointer).
            var gcHandle = GCHandle.Alloc(lpEnumFunc);
            try
            {
                return EnumWindows(&HandleEnumWindowsNativeCallback, (IntPtr)gcHandle);
            }
            finally
            {
                gcHandle.Free();
            }
        }

        [UnmanagedCallersOnly]
        private static BOOL HandleEnumWindowsNativeCallback(IntPtr hWnd, IntPtr lParam)
        {
            return ((EnumWindowsCallback)((GCHandle)lParam).Target!)(hWnd);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public delegate BOOL EnumThreadWindowsCallback(HWND hWnd);

        [DllImport(Libraries.User32)]
        private static unsafe extern BOOL EnumThreadWindows(uint dwThreadId, delegate* unmanaged<HWND, LPARAM, BOOL> lpfn, LPARAM lParam);

        public static unsafe BOOL EnumThreadWindows(uint dwThreadId, EnumThreadWindowsCallback lpfn)
        {
            // We pass a function pointer to the native function and supply the callback as
            // reference data, so that the CLR doesn't need to generate a native code block for
            // each callback delegate instance (for storing the closure pointer).
            var gcHandle = GCHandle.Alloc(lpfn);
            try
            {
                return EnumThreadWindows(dwThreadId, &HandleEnumThreadWindowsNativeCallback, (IntPtr)gcHandle);
            }
            finally
            {
                gcHandle.Free();
            }
        }

        [UnmanagedCallersOnly]
        private static BOOL HandleEnumThreadWindowsNativeCallback(HWND hWnd, LPARAM lParam)
        {
            return ((EnumThreadWindowsCallback)((GCHandle)(nint)lParam).Target!)(hWnd);
        }
    }
}

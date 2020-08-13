// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        public delegate BOOL EnumThreadWindowsCallback(IntPtr hWnd);

        private delegate BOOL EnumThreadWindowsNativeCallback(IntPtr hWnd, IntPtr lParam);

        private static readonly EnumThreadWindowsNativeCallback s_enumThreadWindowsNativeCallback = HandleEnumThreadWindowsNativeCallback;

        [DllImport(Libraries.User32, ExactSpelling = true)]
        private static extern BOOL EnumThreadWindows(uint dwThreadId, EnumThreadWindowsNativeCallback lpfn, IntPtr lParam);

        public static BOOL EnumThreadWindows(uint dwThreadId, EnumThreadWindowsCallback lpfn)
        {
            // We pass a static delegate to the native function and supply the callback as
            // reference data, so that the CLR doesn't need to generate a native code block for
            // each callback delegate instance (for storing the closure pointer).
            var gcHandle = GCHandle.Alloc(lpfn);
            try
            {
                return EnumThreadWindows(dwThreadId, s_enumThreadWindowsNativeCallback, GCHandle.ToIntPtr(gcHandle));
            }
            finally
            {
                gcHandle.Free();
            }
        }

        private static BOOL HandleEnumThreadWindowsNativeCallback(IntPtr hWnd, IntPtr lParam)
        {
            return ((EnumThreadWindowsCallback)GCHandle.FromIntPtr(lParam).Target!)(hWnd);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        // We only ever call this on 32 bit so IntPtr is correct
        [LibraryImport(Libraries.User32, SetLastError = true)]
        private static partial nint GetWindowLongW(IntPtr hWnd, GWL nIndex);

        [LibraryImport(Libraries.User32, SetLastError = true)]
        public static partial nint GetWindowLongPtrW(IntPtr hWnd, GWL nIndex);

        public static nint GetWindowLong(IntPtr hWnd, GWL nIndex)
        {
            if (!Environment.Is64BitProcess)
            {
                return GetWindowLongW(hWnd, nIndex);
            }

            return GetWindowLongPtrW(hWnd, nIndex);
        }

        public static nint GetWindowLong(IHandle hWnd, GWL nIndex)
        {
            nint result = GetWindowLong(hWnd.Handle, nIndex);
            GC.KeepAlive(hWnd);
            return result;
        }

        public static nint GetWindowLong(HandleRef hWnd, GWL nIndex)
        {
            nint result = GetWindowLong(hWnd.Handle, nIndex);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}

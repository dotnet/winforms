// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        // We only ever call this on 32 bit so IntPtr is correct
        [DllImport(Libraries.User32, ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr GetWindowLongW(IntPtr hWnd, GWL nIndex);

        [DllImport(Libraries.User32, ExactSpelling = true, SetLastError = true)]
        public static extern IntPtr GetWindowLongPtrW(IntPtr hWnd, GWL nIndex);

        public static IntPtr GetWindowLong(IntPtr hWnd, GWL nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return GetWindowLongW(hWnd, nIndex);
            }
            return GetWindowLongPtrW(hWnd, nIndex);
        }

        public static IntPtr GetWindowLong(IHandle hWnd, GWL nIndex)
        {
            IntPtr result = GetWindowLong(hWnd.Handle, nIndex);
            GC.KeepAlive(hWnd);
            return result;
        }

        public static IntPtr GetWindowLong(HandleRef hWnd, GWL nIndex)
        {
            IntPtr result = GetWindowLong(hWnd.Handle, nIndex);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}

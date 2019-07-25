// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true, EntryPoint = "GetWindowLongW")]
        private static extern IntPtr GetWindowLong32(IntPtr hWnd, WindowLong nIndex);

        [DllImport(Libraries.User32, ExactSpelling = true, EntryPoint = "GetWindowLongPtrW")]
        private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, WindowLong nIndex);

        /// <summary>
        ///  Retrieves information about the specified window. The function also retrieves the value at a
        ///  specified offset into the extra window memory.
        /// </summary>
        /// <remarks>
        ///  GetWindowLong won't work correctly for 64-bit: we should use GetWindowLongPtr instead. On
        ///  32-bit, GetWindowLongPtr is just #defined as GetWindowLong. GetWindowLong really should
        ///  take/return int instead of IntPtr/HandleRef, but since we're running this only for 32-bit
        ///  it'll be OK.
        /// </remarks>
        public static IntPtr GetWindowLong(IntPtr hWnd, WindowLong nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return GetWindowLong32(hWnd, nIndex);
            }

            return GetWindowLongPtr64(hWnd, nIndex);
        }

        public static IntPtr GetWindowLong(HandleRef hWnd, WindowLong nIndex)
        {
            IntPtr result = GetWindowLong(hWnd.Handle, nIndex);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true, EntryPoint = "SetWindowLongW")]
        private static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, WindowLong nIndex, IntPtr dwNewLong);

        [DllImport(Libraries.User32, ExactSpelling = true, EntryPoint = "SetWindowLongPtrW")]
        private static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, WindowLong nIndex, IntPtr dwNewLong);

        [DllImport(Libraries.User32, ExactSpelling = true, EntryPoint = "SetWindowLongW")]
        private static extern IntPtr SetWindowLongPtr32(HandleRef hWnd, WindowLong nIndex, WndProc wndproc);

        [DllImport(Libraries.User32, ExactSpelling = true, EntryPoint = "SetWindowLongPtrW")]
        private static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, WindowLong nIndex, WndProc wndproc);

        /// <remarks>
        ///  SetWindowLong won't work correctly for 64-bit: we should use SetWindowLongPtr instead. On
        ///  32-bit, SetWindowLongPtr is just #defined as SetWindowLong. SetWindowLong really should
        ///  take/return int instead of IntPtr/HandleRef, but since we're running this only for 32-bit
        ///  it'll be OK.
        /// </remarks>
        public static IntPtr SetWindowLong(IntPtr hWnd, WindowLong nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            }

            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        public static IntPtr SetWindowLong(IntPtr hWnd, WindowLong nIndex, HandleRef dwNewLong)
        {
            IntPtr result = SetWindowLong(hWnd, nIndex, dwNewLong.Handle);
            GC.KeepAlive(dwNewLong.Wrapper);
            return result;
        }

        public static IntPtr SetWindowLong(HandleRef hWnd, WindowLong nIndex, IntPtr dwNewLong)
        {
            IntPtr result = SetWindowLong(hWnd.Handle, nIndex, dwNewLong);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }

        public static IntPtr SetWindowLong(HandleRef hWnd, WindowLong nIndex, HandleRef dwNewLong)
        {
            IntPtr result = SetWindowLong(hWnd.Handle, nIndex, dwNewLong.Handle);
            GC.KeepAlive(hWnd.Wrapper);
            GC.KeepAlive(dwNewLong.Wrapper);
            return result;
        }

        public static IntPtr SetWindowLong(HandleRef hWnd, WindowLong nIndex, WndProc wndproc)
        {
            if (IntPtr.Size == 4)
            {
                return SetWindowLongPtr32(hWnd, nIndex, wndproc);
            }

            return SetWindowLongPtr64(hWnd, nIndex, wndproc);
        }
    }
}

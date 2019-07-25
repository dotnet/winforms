// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [DllImport(Libraries.User32, ExactSpelling = true, EntryPoint = "SetClassLongW")]
        private static extern IntPtr SetClassLongPtr32(HandleRef hwnd, ClassLong nIndex, IntPtr dwNewLong);

        [DllImport(Libraries.User32, ExactSpelling = true, EntryPoint = "SetClassLongPtrW")]
        private static extern IntPtr SetClassLongPtr64(HandleRef hwnd, ClassLong nIndex, IntPtr dwNewLong);

        /// <remarks>
        ///  SetClassLong won't work correctly for 64-bit: we should use SetClassLongPtr instead. On
        ///  32-bit, SetClassLongPtr is just #defined as SetClassLong. SetClassLong really should
        ///  take/return int instead of IntPtr/HandleRef, but since we're running this only for 32-bit
        ///  it'll be OK.
        /// </remarks>
        public static IntPtr SetClassLong(HandleRef hWnd, ClassLong nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetClassLongPtr32(hWnd, nIndex, dwNewLong);
            }

            return SetClassLongPtr64(hWnd, nIndex, dwNewLong);
        }
    }
}

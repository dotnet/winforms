﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        // We only ever call this on 32 bit so IntPtr is correct
        [LibraryImport(Libraries.User32)]
        private static partial IntPtr SetClassLongW(IntPtr hwnd, GCL nIndex, IntPtr dwNewLong);

        [LibraryImport(Libraries.User32)]
        private static partial IntPtr SetClassLongPtrW(IntPtr hwnd, GCL nIndex, IntPtr dwNewLong);

        public static IntPtr SetClassLong(IntPtr hWnd, GCL nIndex, IntPtr dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetClassLongW(hWnd, nIndex, dwNewLong);
            }

            return SetClassLongPtrW(hWnd, nIndex, dwNewLong);
        }
    }
}

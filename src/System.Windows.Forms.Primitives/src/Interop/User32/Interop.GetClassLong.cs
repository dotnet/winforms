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
        // https://msdn.microsoft.com/library/windows/desktop/ms633580.aspx
        [DllImport(Libraries.User32, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr GetClassLongW(IntPtr hWnd, GCL nIndex);

        // https://msdn.microsoft.com/library/windows/desktop/ms633581.aspx
        [DllImport(Libraries.User32, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr GetClassLongPtrW(IntPtr hWnd, GCL nIndex);

        public static IntPtr GetClassLong(IntPtr hWnd, GCL nIndex)
        {
            if (IntPtr.Size == 4)
            {
                return GetClassLongW(hWnd, nIndex);
            }

            return GetClassLongPtrW(hWnd, nIndex);
        }
    }
}

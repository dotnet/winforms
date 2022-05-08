// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [LibraryImport(Libraries.User32)]
        public unsafe static partial BOOL InvalidateRect(IntPtr hWnd, RECT* lpRect, BOOL bErase);

        public unsafe static BOOL InvalidateRect(HandleRef hWnd, RECT* lpRect, BOOL bErase)
        {
            BOOL result = InvalidateRect(hWnd.Handle, lpRect, bErase);
            GC.KeepAlive(hWnd.Wrapper);
            return result;
        }
    }
}

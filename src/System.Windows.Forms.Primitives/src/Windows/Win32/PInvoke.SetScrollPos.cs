// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="SetScrollPos(HWND, SCROLLBAR_CONSTANTS, int, BOOL)"/>
    public static int SetScrollPos<T>(T hWnd, SCROLLBAR_CONSTANTS nBar, int nPos, BOOL bRedraw)
        where T : IHandle<HWND>
    {
        int result = SetScrollPos(hWnd.Handle, nBar, nPos, bRedraw);
        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }
}

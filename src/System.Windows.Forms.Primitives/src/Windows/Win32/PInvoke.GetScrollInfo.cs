// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="GetScrollInfo(HWND, SCROLLBAR_CONSTANTS, ref SCROLLINFO)"/>
    public static BOOL GetScrollInfo<T>(T hwnd, SCROLLBAR_CONSTANTS nBar, ref SCROLLINFO lpsi)
        where T : IHandle<HWND>
    {
        BOOL result = GetScrollInfo(hwnd.Handle, nBar, ref lpsi);
        GC.KeepAlive(hwnd.Wrapper);
        return result;
    }
}

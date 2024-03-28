// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="EnableScrollBar(HWND, uint, ENABLE_SCROLL_BAR_ARROWS)"/>
    public static BOOL EnableScrollBar<T>(T hWnd, SCROLLBAR_CONSTANTS wSBflags, ENABLE_SCROLL_BAR_ARROWS wArrows)
        where T : IHandle<HWND>
    {
        BOOL result = EnableScrollBar(hWnd.Handle, (uint)wSBflags, wArrows);
        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="RedrawWindow(HWND, RECT*, HRGN, REDRAW_WINDOW_FLAGS)"/>
    public static unsafe BOOL RedrawWindow<T>(T hWnd, RECT* lprcUpdate, HRGN hrgnUpdate, REDRAW_WINDOW_FLAGS flags)
        where T : IHandle<HWND>
    {
        BOOL result = RedrawWindow(hWnd.Handle, lprcUpdate, hrgnUpdate, flags);
        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }
}

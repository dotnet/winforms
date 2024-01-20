// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="ValidateRect(HWND, RECT*)"/>
    public static unsafe BOOL ValidateRect<T>(T hWnd, RECT* lpRect)
        where T : IHandle<HWND>
    {
        BOOL result = ValidateRect(hWnd.Handle, lpRect);
        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    /// <inheritdoc cref="GetWindowRect(HWND, out RECT)"/>
    public static BOOL GetWindowRect<T>(T hWnd, out RECT lpRect) where T : IHandle<HWND>
    {
        BOOL result = GetWindowRect(hWnd.Handle, out lpRect);
        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }
}

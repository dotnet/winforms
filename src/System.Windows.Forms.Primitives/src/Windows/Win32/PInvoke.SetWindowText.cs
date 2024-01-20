// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="SetWindowText(HWND, string)"/>
    public static BOOL SetWindowText<T>(T hWnd, string text) where T : IHandle<HWND>
    {
        BOOL result = SetWindowText(hWnd.Handle, text);
        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    /// <inheritdoc cref="GetWindowTextLength(HWND)"/>
    public static int GetWindowTextLength<T>(T hWnd) where T : IHandle<HWND>
    {
        int result = GetWindowTextLength(hWnd.Handle);
        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }
}

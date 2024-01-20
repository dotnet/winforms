// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="SetLayeredWindowAttributes(HWND, COLORREF, byte, LAYERED_WINDOW_ATTRIBUTES_FLAGS)"/>
    public static BOOL SetLayeredWindowAttributes<T>(T hwnd, COLORREF crKey, byte bAlpha, LAYERED_WINDOW_ATTRIBUTES_FLAGS dwFlags)
        where T : IHandle<HWND>
    {
        BOOL result = SetLayeredWindowAttributes(hwnd.Handle, crKey, bAlpha, dwFlags);
        GC.KeepAlive(hwnd.Wrapper);
        return result;
    }
}

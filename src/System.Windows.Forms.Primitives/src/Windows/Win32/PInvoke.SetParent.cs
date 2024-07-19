// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="SetParent(HWND, HWND)"/>
    public static HWND SetParent<TChild, TParent>(TChild hWndChild, TParent hWndNewParent)
        where TChild : IHandle<HWND>
        where TParent : IHandle<HWND>
    {
        HWND result = SetParent(hWndChild.Handle, hWndNewParent.Handle);
        GC.KeepAlive(hWndChild.Wrapper);
        GC.KeepAlive(hWndNewParent.Wrapper);
        return result;
    }
}

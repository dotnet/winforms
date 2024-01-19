// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="SetCapture(HWND)"/>
    public static IntPtr SetCapture<T>(T hWnd) where T : IHandle<HWND>
    {
        IntPtr result = SetCapture(hWnd.Handle);
        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }
}

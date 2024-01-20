// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="DrawMenuBar(HWND)"/>
    public static BOOL DrawMenuBar<T>(T hWnd)
        where T : IHandle<HWND>
    {
        BOOL result = DrawMenuBar(hWnd.Handle);
        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }
}

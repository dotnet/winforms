// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="ClientToScreen(HWND, ref Point)"/>
    public static BOOL ClientToScreen<T>(T hWnd, ref Point lpPoint)
        where T : IHandle<HWND>
    {
        BOOL result = ClientToScreen(hWnd.Handle, ref lpPoint);
        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }
}

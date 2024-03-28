// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="DragAcceptFiles(HWND, BOOL)"/>
    public static void DragAcceptFiles<T>(T hWnd, BOOL fAccept) where T : IHandle<HWND>
    {
        DragAcceptFiles(hWnd.Handle, fAccept);
        GC.KeepAlive(hWnd.Wrapper);
    }
}

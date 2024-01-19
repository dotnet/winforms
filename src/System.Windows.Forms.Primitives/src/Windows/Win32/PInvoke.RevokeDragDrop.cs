// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="RevokeDragDrop(HWND)"/>
    public static HRESULT RevokeDragDrop<T>(T hwnd) where T : IHandle<HWND>
    {
        HRESULT result = RevokeDragDrop(hwnd.Handle);
        GC.KeepAlive(hwnd.Wrapper);
        return result;
    }
}

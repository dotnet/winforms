// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="GetSystemMenu(HWND, BOOL)"/>
    public static HMENU GetSystemMenu<T>(T hwnd, BOOL bRevert) where T : IHandle<HWND>
    {
        HMENU result = GetSystemMenu(hwnd.Handle, bRevert);
        GC.KeepAlive(hwnd.Wrapper);
        return result;
    }
}

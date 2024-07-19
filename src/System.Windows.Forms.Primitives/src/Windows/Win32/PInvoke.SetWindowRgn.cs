// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="SetWindowRgn(HWND, HRGN, BOOL)"/>
    public static int SetWindowRgn<T>(T hwnd, HRGN hrgn, BOOL fRedraw)
        where T : IHandle<HWND>
    {
        int result = SetWindowRgn(hwnd.Handle, hrgn, fRedraw);
        GC.KeepAlive(hwnd.Wrapper);
        return result;
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="GetDpiForWindow(HWND)"/>
    public static uint GetDpiForWindow<T>(T hwnd) where T : IHandle<HWND>
    {
        if (OsVersion.IsWindows10_1607OrGreater())
        {
            uint result = GetDpiForWindow(hwnd.Handle);
            GC.KeepAlive(hwnd.Wrapper);
            return result;
        }

        return 0;
    }
}

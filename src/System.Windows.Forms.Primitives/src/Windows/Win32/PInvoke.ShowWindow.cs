// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    public static BOOL ShowWindow<T>(T hWnd, SHOW_WINDOW_CMD nCmdShow) where T : IHandle<HWND>
    {
        BOOL result = ShowWindow(hWnd.Handle, nCmdShow);
        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }
}

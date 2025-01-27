// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    internal static unsafe LRESULT CallWindowProc<T>(void* lpPrevWndFunc, T hWnd, uint Msg, WPARAM wParam, LPARAM lParam)
        where T : IHandle<HWND>
    {
        LRESULT result = CallWindowProc(
            (delegate* unmanaged[Stdcall]<HWND, uint, WPARAM, LPARAM, LRESULT>)lpPrevWndFunc,
            hWnd.Handle,
            Msg,
            wParam,
            lParam);

        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }
}

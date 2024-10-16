// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    [DllImport(Libraries.User32, SetLastError = true)]
    private static extern nint SetWindowLongW(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex, nint dwNewLong);

    [DllImport(Libraries.User32, SetLastError = true)]
    private static extern nint SetWindowLongPtrW(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex, nint dwNewLong);

    public static nint SetWindowLong<T>(T hWnd, WINDOW_LONG_PTR_INDEX nIndex, nint newValue)
        where T : IHandle<HWND>
    {
        nint result = Environment.Is64BitProcess
            ? SetWindowLongPtrW(hWnd.Handle, nIndex, newValue)
            : SetWindowLongW(hWnd.Handle, nIndex, (int)newValue);
        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }

    public static nint SetWindowLong<THwnd, TNewValue>(THwnd hWnd, WINDOW_LONG_PTR_INDEX nIndex, TNewValue newValue)
        where THwnd : IHandle<HWND>
        where TNewValue : IHandle<HWND>
    {
        nint result = SetWindowLong(hWnd, nIndex, (nint)newValue.Handle);
        GC.KeepAlive(newValue.Wrapper);
        return result;
    }

    public static nint SetWindowLong<T>(T hWnd, WINDOW_LONG_PTR_INDEX nIndex, WNDPROC dwNewLong)
        where T : IHandle<HWND>
    {
        IntPtr pointer = Marshal.GetFunctionPointerForDelegate(dwNewLong);
        IntPtr result = SetWindowLong(hWnd, nIndex, pointer);
        GC.KeepAlive(dwNewLong);
        return result;
    }
}

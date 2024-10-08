// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    [DllImport(Libraries.User32, SetLastError = true)]
    private static extern nint GetWindowLongW(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex);

    [DllImport(Libraries.User32, SetLastError = true)]
    private static extern nint GetWindowLongPtrW(HWND hWnd, WINDOW_LONG_PTR_INDEX nIndex);

    /// <summary>
    ///  Dynamic wrapper for GetWindowLong that works on both 32 and 64 bit.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   <see href="https://learn.microsoft.com/windows/win32/api/winuser/nf-winuser-getwindowlongptrw">
    ///    GetWindowLong documentation.
    ///   </see>
    ///  </para>
    /// </remarks>
    /// <returns></returns>
    public static nint GetWindowLong<T>(T hWnd, WINDOW_LONG_PTR_INDEX nIndex)
        where T : IHandle<HWND>
    {
        nint result = Environment.Is64BitProcess
            ? GetWindowLongPtrW(hWnd.Handle, nIndex)
            : GetWindowLongW(hWnd.Handle, nIndex);
        GC.KeepAlive(hWnd.Wrapper);
        return result;
    }
}

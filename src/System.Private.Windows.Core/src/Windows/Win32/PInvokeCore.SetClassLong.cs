// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    // We only ever call this on 32 bit so IntPtr is correct
    [DllImport(Libraries.User32)]
    private static extern nint SetClassLongW(HWND hwnd, GET_CLASS_LONG_INDEX nIndex, nint dwNewLong);

    [DllImport(Libraries.User32)]
    private static extern nint SetClassLongPtrW(HWND hwnd, GET_CLASS_LONG_INDEX nIndex, nint dwNewLong);

    public static nint SetClassLong(HWND hWnd, GET_CLASS_LONG_INDEX nIndex, nint dwNewLong)
         => Environment.Is64BitProcess
           ? SetClassLongPtrW(hWnd, nIndex, dwNewLong)
           : SetClassLongW(hWnd, nIndex, dwNewLong);
}

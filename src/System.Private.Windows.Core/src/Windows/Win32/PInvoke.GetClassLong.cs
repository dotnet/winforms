// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    // We only ever call this on 32 bit so IntPtr is correct
    // https://msdn.microsoft.com/library/windows/desktop/ms633580.aspx
    [DllImport(Libraries.User32, SetLastError = true)]
    private static extern nint GetClassLongW(HWND hWnd, GET_CLASS_LONG_INDEX nIndex);

    // https://msdn.microsoft.com/library/windows/desktop/ms633581.aspx
    [DllImport(Libraries.User32, SetLastError = true)]
    private static extern nint GetClassLongPtrW(HWND hWnd, GET_CLASS_LONG_INDEX nIndex);

    public static IntPtr GetClassLong(HWND hWnd, GET_CLASS_LONG_INDEX nIndex)
        => Environment.Is64BitProcess
           ? GetClassLongPtrW(hWnd, nIndex)
           : GetClassLongW(hWnd, nIndex);
}

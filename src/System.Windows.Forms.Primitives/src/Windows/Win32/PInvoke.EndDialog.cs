// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="EndDialog(HWND, nint)"/>
    public static BOOL EndDialog<T>(T hDlg, IntPtr nResult)
        where T : IHandle<HWND>
    {
        BOOL result = EndDialog(hDlg.Handle, nResult);
        GC.KeepAlive(hDlg.Wrapper);
        return result;
    }
}

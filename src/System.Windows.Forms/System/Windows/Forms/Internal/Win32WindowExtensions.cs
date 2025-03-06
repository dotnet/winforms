// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal static class Win32WindowExtensions
{
    public static WINDOW_EX_STYLE GetExtendedStyle(this IWin32Window window)
    {
        WINDOW_EX_STYLE style = (WINDOW_EX_STYLE)PInvokeCore.GetWindowLong(
            Control.GetSafeHandle(window),
            WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE);
        return style;
    }
}

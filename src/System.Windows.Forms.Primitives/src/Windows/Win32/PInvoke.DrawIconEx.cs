// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    public static BOOL DrawIconEx<T>(
        HDC hDC,
        int xLeft,
        int yTop,
        T hIcon,
        int cxWidth,
        int cyWidth)
        where T : IHandle<HICON>
    {
        BOOL result = DrawIconEx(hDC, xLeft, yTop, hIcon.Handle, cxWidth, cyWidth, 0, HBRUSH.Null, DI_FLAGS.DI_NORMAL);
        GC.KeepAlive(hIcon.Wrapper);
        return result;
    }
}

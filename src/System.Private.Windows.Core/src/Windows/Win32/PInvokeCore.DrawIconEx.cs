// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    /// <inheritdoc cref="DrawIconEx(HDC, int, int, HICON, int, int, uint, HBRUSH, DI_FLAGS)"/>
    public static BOOL DrawIconEx<T>(
        HDC hDC,
        int xLeft,
        int yTop,
        T hIcon,
        int cxWidth,
        int cyWidth,
        DI_FLAGS diFlags = DI_FLAGS.DI_NORMAL)
        where T : IHandle<HICON>
    {
        // DrawIcon effectively calls DrawIconEx with the following parameters:
        //
        //  DrawIconEx(hdc, x, y, hicon, 0, 0, 0, 0, DI_NORMAL | DI_COMPAT | DI_DEFAULTSIZE);
        //
        // DI_COMPAT is not used.

        BOOL result = DrawIconEx(hDC, xLeft, yTop, hIcon.Handle, cxWidth, cyWidth, 0, HBRUSH.Null, diFlags);
        GC.KeepAlive(hIcon.Wrapper);
        return result;
    }
}

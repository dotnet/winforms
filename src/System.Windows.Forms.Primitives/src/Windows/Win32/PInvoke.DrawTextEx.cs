// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="DrawTextEx(HDC, PWSTR, int, RECT*, DRAW_TEXT_FORMAT, DRAWTEXTPARAMS*)"/>
    public static unsafe int DrawTextEx(
        HDC hdc,
        ReadOnlySpan<char> lpchText,
        RECT* lprc,
        DRAW_TEXT_FORMAT format,
        DRAWTEXTPARAMS* lpdtp)
    {
        lpdtp->cbSize = (uint)sizeof(DRAWTEXTPARAMS);

        fixed (char* c = lpchText)
        {
            return DrawTextEx(hdc, (PWSTR)c, lpchText.Length, lprc, format, lpdtp);
        }
    }
}

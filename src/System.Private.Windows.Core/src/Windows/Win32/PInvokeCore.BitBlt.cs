// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    /// <inheritdoc cref="BitBlt(HDC, int, int, int, int, HDC, int, int, ROP_CODE)"/>
    public static BOOL BitBlt<T>(
        T hdc,
        int x,
        int y,
        int cx,
        int cy,
        HDC hdcSrc,
        int x1,
        int y1,
        ROP_CODE rop) where T : IHandle<HDC>
    {
        BOOL result = BitBlt(
            hdc.Handle,
            x,
            y,
            cx,
            cy,
            hdcSrc,
            x1,
            y1,
            rop);
        GC.KeepAlive(hdc.Wrapper);
        return result;
    }

    /// <inheritdoc cref="BitBlt(HDC, int, int, int, int, HDC, int, int, ROP_CODE)"/>
    public static BOOL BitBlt<T>(
        HDC hdc,
        int x,
        int y,
        int cx,
        int cy,
        T hdcSrc,
        int x1,
        int y1,
        ROP_CODE rop) where T : IHandle<HDC>
    {
        BOOL result = BitBlt(
            hdc,
            x,
            y,
            cx,
            cy,
            hdcSrc.Handle,
            x1,
            y1,
            rop);
        GC.KeepAlive(hdcSrc.Wrapper);
        return result;
    }
}

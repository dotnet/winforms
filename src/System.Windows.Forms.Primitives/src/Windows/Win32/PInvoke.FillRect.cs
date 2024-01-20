// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="FillRect(HDC, in RECT, HBRUSH)"/>
    public static int FillRect<T>(T hDC, ref RECT lprc, HBRUSH hbr)
        where T : IHandle<HDC>
    {
        int result = FillRect(hDC.Handle, in lprc, hbr);
        GC.KeepAlive(hDC.Wrapper);
        return result;
    }
}

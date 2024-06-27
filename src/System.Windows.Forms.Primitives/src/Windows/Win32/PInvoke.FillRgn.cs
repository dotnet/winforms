// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="FillRgn(HDC, HRGN, HBRUSH)"/>
    public static int FillRgn<T>(T hDC, ref HRGN hRgn, HBRUSH hbr)
        where T : IHandle<HDC>
    {
        int result = FillRgn(hDC.Handle, hRgn, hbr);
        GC.KeepAlive(hDC.Wrapper);

        return result;
    }
}

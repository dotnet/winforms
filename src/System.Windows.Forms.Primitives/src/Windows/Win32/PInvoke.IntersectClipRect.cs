// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="IntersectClipRect(HDC, int, int, int, int)"/>
    public static GDI_REGION_TYPE IntersectClipRect<T>(T hdc, int left, int top, int right, int bottom) where T : IHandle<HDC>
    {
        GDI_REGION_TYPE result = IntersectClipRect(hdc.Handle, left, top, right, bottom);
        GC.KeepAlive(hdc.Wrapper);
        return result;
    }
}

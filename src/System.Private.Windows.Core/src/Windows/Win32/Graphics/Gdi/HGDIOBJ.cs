// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Graphics.Gdi;

internal readonly unsafe partial struct HGDIOBJ
{
    public static implicit operator HGDIOBJ(HDC value) => (HGDIOBJ)value.Value;

    public static explicit operator HFONT(HGDIOBJ value) => (HFONT)value.Value;
    public static explicit operator HBRUSH(HGDIOBJ value) => (HBRUSH)value.Value;
    public static explicit operator HBITMAP(HGDIOBJ value) => (HBITMAP)value.Value;
}

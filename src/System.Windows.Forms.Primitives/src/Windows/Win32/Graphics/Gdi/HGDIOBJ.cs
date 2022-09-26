// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32.Graphics.Gdi
{
    internal readonly partial struct HGDIOBJ
    {
        public static implicit operator HGDIOBJ(HFONT value) => (HGDIOBJ)value.Value;
        public static implicit operator HGDIOBJ(HDC value) => (HGDIOBJ)value.Value;
        public static implicit operator HGDIOBJ(HPEN value) => (HGDIOBJ)value.Value;
        public static implicit operator HGDIOBJ(HBRUSH value) => (HGDIOBJ)value.Value;
        public static implicit operator HGDIOBJ(HBITMAP value) => (HGDIOBJ)value.Value;
        public static implicit operator HGDIOBJ(HRGN value) => (HGDIOBJ)value.Value;
        public static implicit operator HGDIOBJ(HPALETTE value) => (HGDIOBJ)value.Value;

        public static explicit operator HFONT(HGDIOBJ value) => (HFONT)value.Value;
        public static explicit operator HBRUSH(HGDIOBJ value) => (HBRUSH)value.Value;
        public static explicit operator HBITMAP(HGDIOBJ value) => (HBITMAP)value.Value;
    }
}

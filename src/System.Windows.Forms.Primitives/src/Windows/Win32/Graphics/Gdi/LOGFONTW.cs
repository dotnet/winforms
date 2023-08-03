// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Interop;
using System.Runtime.CompilerServices;

namespace Windows.Win32.Graphics.Gdi;

internal partial struct LOGFONTW
{
    public const int LF_FACESIZE = 32;

    // LOGFONTW has space for 32 characters, we use this to ensure we cut to 31 to make room for the null.
    // We should never reference lfFaceName directly and use this property instead.
    public ReadOnlySpan<char> FaceName
    {
        [UnscopedRef]
        get => lfFaceName.AsSpan().SliceAtFirstNull();
        set => SpanHelpers.CopyAndTerminate(value, lfFaceName.AsSpan());
    }

    public static LOGFONTW FromFont(Font font)
    {
        font.ToLogFont(out LOGFONT logFont);
        return Unsafe.As<LOGFONT, LOGFONTW>(ref logFont);
    }

    public static LOGFONTW FromFont(Font font, global::System.Drawing.Graphics graphics)
    {
        font.ToLogFont(out LOGFONT logFont, graphics);
        return Unsafe.As<LOGFONT, LOGFONTW>(ref logFont);
    }
}

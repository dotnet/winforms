// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Interop;
using System.Runtime.CompilerServices;

namespace System.Drawing;

internal static class FontExtensions
{
    public static LOGFONTW ToLogicalFont(this Font font)
    {
        font.ToLogFont(out LOGFONT logFont);
        return Unsafe.As<LOGFONT, LOGFONTW>(ref logFont);
    }

    public static LOGFONTW ToLogicalFont(this Font font, Graphics graphics)
    {
        font.ToLogFont(out LOGFONT logFont, graphics);
        return Unsafe.As<LOGFONT, LOGFONTW>(ref logFont);
    }
}

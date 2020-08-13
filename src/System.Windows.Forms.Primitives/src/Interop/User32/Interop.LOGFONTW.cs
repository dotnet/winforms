// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class User32
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct LOGFONTW
        {
            public const int LF_FACESIZE = 32;

            public int lfHeight;
            public int lfWidth;
            public int lfEscapement;
            public int lfOrientation;
            public Gdi32.FW lfWeight;
            public byte lfItalic;
            public byte lfUnderline;
            public byte lfStrikeOut;
            public byte lfCharSet;
            public Gdi32.OUT_PRECIS lfOutPrecision;
            public byte lfClipPrecision;
            public Gdi32.QUALITY lfQuality;
            public byte lfPitchAndFamily;
            private fixed char _lfFaceName[LF_FACESIZE];
            private Span<char> lfFaceName
            {
                get { fixed (char* c = _lfFaceName) { return new Span<char>(c, LF_FACESIZE); } }
            }

            public ReadOnlySpan<char> FaceName
            {
                get => lfFaceName.SliceAtFirstNull();
                set => SpanHelpers.CopyAndTerminate(value, lfFaceName);
            }

            // Font.ToLogFont will copy LOGFONT into a blittable struct,
            // but we need to box it upfront so we can unbox.

            public static LOGFONTW FromFont(Font font)
            {
                object logFont = new LOGFONTW();
                font.ToLogFont(logFont);
                return (LOGFONTW)logFont;
            }

            public static LOGFONTW FromFont(Font font, Graphics graphics)
            {
                object logFont = new LOGFONTW();
                font.ToLogFont(logFont, graphics);
                return (LOGFONTW)logFont;
            }
        }
    }
}

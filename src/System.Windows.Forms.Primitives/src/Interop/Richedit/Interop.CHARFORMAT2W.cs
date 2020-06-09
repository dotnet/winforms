// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal partial class Interop
{
    internal static partial class Richedit
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public unsafe struct CHARFORMAT2W
        {
            private const int LF_FACESIZE = 32;

            public uint cbSize;
            public CFM dwMask;
            public CFE dwEffects;
            public int yHeight;
            public int yOffset;
            public int crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;

            public fixed char _szFaceName[LF_FACESIZE];

            public ushort wWeight;
            public short sSpacing;
            public int crBackColor;
            public int lcid;
            public uint dwCookie;
            public short sStyle;
            public ushort wKerning;
            public byte bUnderlineType;
            public byte bAnimation;
            public byte bRevAuthor;

            // Only available in RichEdit 8.0
            // public byte bUnderlineColor

            private Span<char> szFaceName
            {
                get { fixed (char* c = _szFaceName) { return new Span<char>(c, LF_FACESIZE); } }
            }

            public ReadOnlySpan<char> FaceName
            {
                get => szFaceName.SliceAtFirstNull();
                set => SpanHelpers.CopyAndTerminate(value, szFaceName);
            }
        }
    }
}

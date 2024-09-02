// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.UI.Controls.RichEdit;

internal partial class Interop
{
    internal static partial class Richedit
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode, Pack = RichEditPack)]
        public unsafe struct CHARFORMAT2W
        {
            public uint cbSize;
            public CFM_MASK dwMask;
            public CFE_EFFECTS dwEffects;
            public int yHeight;
            public int yOffset;
            public int crTextColor;
            public byte bCharSet;
            public byte bPitchAndFamily;

            public fixed char _szFaceName[(int)PInvokeCore.LF_FACESIZE];

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
                get { fixed (char* c = _szFaceName) { return new Span<char>(c, (int)PInvokeCore.LF_FACESIZE); } }
            }

            public ReadOnlySpan<char> FaceName
            {
                get => szFaceName.SliceAtFirstNull();
                set => SpanHelpers.CopyAndTerminate(value, szFaceName);
            }
        }
    }
}

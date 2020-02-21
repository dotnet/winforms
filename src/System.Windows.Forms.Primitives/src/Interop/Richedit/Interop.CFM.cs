// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Richedit
    {
        [Flags]
        public enum CFM : uint
        {
            BOLD = 0x00000001,
            ITALIC = 0x00000002,
            UNDERLINE = 0x00000004,
            STRIKEOUT = 0x00000008,
            PROTECTED = 0x00000010,
            LINK = 0x00000020,
            SIZE = 0x80000000,
            COLOR = 0x40000000,
            FACE = 0x20000000,
            OFFSET = 0x10000000,
            CHARSET = 0x08000000,
            SMALLCAPS = 0x00000040,
            ALLCAPS = 0x00000080,
            HIDDEN = 0x00000100,
            OUTLINE = 0x00000200,
            SHADOW = 0x00000400,
            EMBOSS = 0x00000800,
            IMPRINT = 0x00001000,
            DISABLED = 0x00002000,
            REVISED = 0x00004000,
            REVAUTHOR = 0x00008000,
            ANIMATION = 0x00040000,
            STYLE = 0x00080000,
            KERNING = 0x00100000,
            SPACING = 0x00200000,
            WEIGHT = 0x00400000,
            UNDERLINETYPE = 0x00800000,
            COOKIE = 0x01000000,
            LCID = 0x02000000,
            BACKCOLOR = 0x04000000,
            SUBSCRIPT = CFE.SUBSCRIPT | CFE.SUPERSCRIPT,
            SUPERSCRIPT = CFM.SUBSCRIPT,
        }
    }
}

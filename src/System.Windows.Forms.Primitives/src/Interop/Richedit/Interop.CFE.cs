// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal partial class Interop
{
    internal static partial class Richedit
    {
        [Flags]
        public enum CFE : uint
        {
            BOLD = 0x00000001,
            ITALIC = 0x00000002,
            UNDERLINE = 0x00000004,
            STRIKEOUT = 0x00000008,
            PROTECTED = 0x00000010,
            LINK = 0x00000020,
            AUTOCOLOR = 0x40000000,
            SUBSCRIPT = 0x00010000,
            SUPERSCRIPT = 0x00020000,
            SMALLCAPS = CFM.SMALLCAPS,
            ALLCAPS = CFM.ALLCAPS,
            HIDDEN = CFM.HIDDEN,
            OUTLINE = CFM.OUTLINE,
            SHADOW = CFM.SHADOW,
            EMBOSS = CFM.EMBOSS,
            IMPRINT = CFM.IMPRINT,
            DISABLED = CFM.DISABLED,
            REVISED = CFM.REVISED,
            AUTOBACKCOLOR = CFM.BACKCOLOR,
            FONTBOUND = 0x00100000,
            LINKPROTECTED = 0x00800000,
            EXTENDED = 0x02000000,
            MATHNOBUILDUP = 0x08000000,
            MATH = 0x10000000,
            MATHORDINARY = 0x20000000,
        }
    }
}

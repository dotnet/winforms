// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class Gdi32
    {
        public enum QUALITY : byte
        {
            /// <summary>
            ///  Appearance of the font does not matter.
            /// </summary>
            DEFAULT = 0,

            /// <summary>
            ///  Appearance of the font is less important than when PROOF_QUALITY is used.
            ///  For GDI raster fonts, scaling is enabled, which means that more font sizes are available,
            ///  but the quality may be lower. Bold, italic, underline, and strikeout fonts are synthesized if necessary.
            /// </summary>
            DRAFT = 1,

            /// <summary>
            ///  Character quality of the font is more important than exact matching of the logical-font attributes.
            ///  For GDI raster fonts, scaling is disabled and the font closest in size is chosen.
            ///  Although the chosen font size may not be mapped exactly when PROOF_QUALITY is used, the quality of
            ///  the font is high and there is no distortion of appearance. Bold, italic, underline, and strikeout
            ///  fonts are synthesized if necessary.
            /// </summary>
            PROOF = 2,

            /// <summary>
            ///  Font is never antialiased.
            /// </summary>
            NONANTIALIASED = 3,

            /// <summary>
            ///  Font is always antialiased if the font supports it and the size of the font is not
            ///  too small or too large.
            /// </summary>
            ANTIALIASED = 4,

            /// <summary>
            ///  The following situations do not support ClearType antialiasing:
            ///  - Text is rendered on a printer.
            ///  - Display set for 256 colors or less.
            ///  - Text is rendered to a terminal server client.
            ///  - The font is not a TrueType font or an OpenType font with TrueType outlines.
            ///  For example, the following do not support ClearType antialiasing:
            ///  Type 1 fonts, Postscript OpenType fonts without TrueType outlines, bitmap fonts, vector fonts, and device fonts.
            ///  - The font has tuned embedded bitmaps, for any font sizes that contain the embedded bitmaps.
            ///  For example, this occurs commonly in East Asian fonts.
            ///  If set, text is rendered (when possible) using ClearType antialiasing method.
            /// </summary>
            CLEARTYPE = 5,
            CLEARTYPE_NATURAL = 6
        }
    }
}

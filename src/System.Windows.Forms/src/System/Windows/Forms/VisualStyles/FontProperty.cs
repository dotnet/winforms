// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.UxTheme;

namespace System.Windows.Forms.VisualStyles
{
    public enum FontProperty
    {
        /// <summary>
        /// The font that will be used to draw text within the context of this part.
        /// </summary>
        TextFont = TMT.FONT,
        GlyphFont = TMT.GLYPHFONT
    }
}

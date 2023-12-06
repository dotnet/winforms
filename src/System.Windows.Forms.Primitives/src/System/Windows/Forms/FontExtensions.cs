// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

internal static class FontExtensions
{
    /// <summary>
    ///  Creates a copy of the <paramref name="templateFont"/> with the new em-size in the units of the template font.
    /// </summary>
    /// <param name="templateFont">The font instance to clone.</param>
    /// <param name="emSize">The em-size of the new font in the units specified by the unit parameter.</param>
    /// <returns>
    ///  A new <see cref="Font"/> object with the new size, or <see langword="null"/> if <paramref name="templateFont"/>
    ///  is <see langword="null"/>.
    /// </returns>
    [return: NotNullIfNotNull(nameof(templateFont))]
    public static Font? WithSize(this Font? templateFont, float emSize) => templateFont is null
        ? null
        : new(
            templateFont.FontFamily,
            emSize,
            templateFont.Style,
            templateFont.Unit,
            templateFont.GdiCharSet,
            templateFont.GdiVerticalFont);
}

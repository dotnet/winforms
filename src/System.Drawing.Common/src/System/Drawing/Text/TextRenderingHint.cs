// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Text;

/// <summary>
///  Specifies the quality of text rendering.
/// </summary>
public enum TextRenderingHint
{
    /// <summary>
    ///  Glyph with system default rendering hint.
    /// </summary>
    SystemDefault = GdiPlus.TextRenderingHint.TextRenderingHintSystemDefault,

    /// <summary>
    ///  Glyph bitmap with hinting.
    /// </summary>
    SingleBitPerPixelGridFit = GdiPlus.TextRenderingHint.TextRenderingHintSingleBitPerPixelGridFit,

    /// <summary>
    ///  Glyph bitmap without hinting.
    /// </summary>
    SingleBitPerPixel = GdiPlus.TextRenderingHint.TextRenderingHintSingleBitPerPixel,

    /// <summary>
    ///  Anti-aliasing with hinting.
    /// </summary>
    AntiAliasGridFit = GdiPlus.TextRenderingHint.TextRenderingHintAntiAliasGridFit,

    /// <summary>
    ///  Glyph anti-alias bitmap without hinting.
    /// </summary>
    AntiAlias = GdiPlus.TextRenderingHint.TextRenderingHintAntiAlias,

    /// <summary>
    ///  Glyph CT bitmap with hinting.
    /// </summary>
    ClearTypeGridFit = GdiPlus.TextRenderingHint.TextRenderingHintClearTypeGridFit
}

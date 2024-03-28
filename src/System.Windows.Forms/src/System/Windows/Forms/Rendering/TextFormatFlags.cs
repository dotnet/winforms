// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#pragma warning disable format

/// <summary>
///  Specifies the display and layout information for text strings.
/// </summary>
[Flags]
public enum TextFormatFlags
{
    Bottom                              = (int)DRAW_TEXT_FORMAT.DT_BOTTOM,
    EndEllipsis                         = (int)DRAW_TEXT_FORMAT.DT_END_ELLIPSIS,
    ExpandTabs                          = (int)DRAW_TEXT_FORMAT.DT_EXPANDTABS,
    ExternalLeading                     = (int)DRAW_TEXT_FORMAT.DT_EXTERNALLEADING,
    Default                             = default,
    HidePrefix                          = (int)DRAW_TEXT_FORMAT.DT_HIDEPREFIX,
    HorizontalCenter                    = (int)DRAW_TEXT_FORMAT.DT_CENTER,
    Internal                            = (int)DRAW_TEXT_FORMAT.DT_INTERNAL,

    /// <remarks>
    ///  <para>This is the default.</para>
    /// </remarks>
    Left                                = (int)DRAW_TEXT_FORMAT.DT_LEFT,

    [Obsolete("ModifyString mutates strings and should be avoided. It will be blocked in a future release.")]
    ModifyString                        = (int)DRAW_TEXT_FORMAT.DT_MODIFYSTRING,
    NoClipping                          = (int)DRAW_TEXT_FORMAT.DT_NOCLIP,
    NoPrefix                            = (int)DRAW_TEXT_FORMAT.DT_NOPREFIX,
    NoFullWidthCharacterBreak           = (int)DRAW_TEXT_FORMAT.DT_NOFULLWIDTHCHARBREAK,
    PathEllipsis                        = (int)DRAW_TEXT_FORMAT.DT_PATH_ELLIPSIS,
    PrefixOnly                          = (int)DRAW_TEXT_FORMAT.DT_PREFIXONLY,
    Right                               = (int)DRAW_TEXT_FORMAT.DT_RIGHT,
    RightToLeft                         = (int)DRAW_TEXT_FORMAT.DT_RTLREADING,
    SingleLine                          = (int)DRAW_TEXT_FORMAT.DT_SINGLELINE,
    TextBoxControl                      = (int)DRAW_TEXT_FORMAT.DT_EDITCONTROL,

    /// <remarks>
    ///  <para>This is the default.</para>
    /// </remarks>
    Top                                 = (int)DRAW_TEXT_FORMAT.DT_TOP,

    VerticalCenter                      = (int)DRAW_TEXT_FORMAT.DT_VCENTER,
    WordBreak                           = (int)DRAW_TEXT_FORMAT.DT_WORDBREAK,
    WordEllipsis                        = (int)DRAW_TEXT_FORMAT.DT_WORD_ELLIPSIS,

    ///  The following flags are exclusive of TextRenderer (no Windows native flags)
    ///  and apply to methods receiving a Graphics as the IDeviceContext object, and
    ///  specify whether to reapply clipping and coordinate transformations to the hdc
    ///  obtained from the Graphics object, which returns a clean hdc.
    ///
    ///  These get stripped off by TextExtensions before we call DrawText or MeasureText
    ///  (see: GdiUnsupportedFlagMask).

    PreserveGraphicsClipping            = 0x0100_0000,
    PreserveGraphicsTranslateTransform  = 0x0200_0000,

    /// <summary>
    ///  Adds padding related to the drawing binding box, computed according to the font size.
    ///  Match the System.Internal.GDI.TextPaddingOptions.
    /// </summary>
    GlyphOverhangPadding                = 0x0000_0000,
    NoPadding                           = 0x1000_0000,
    LeftAndRightPadding                 = 0x2000_0000
}

#pragma warning restore format

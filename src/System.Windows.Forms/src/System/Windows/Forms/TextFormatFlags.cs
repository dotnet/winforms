// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the display and layout information for text strings.
    /// </summary>
    /// <remarks>
    ///  This is a public enum wrapping the internal <see cref="User32.DT"/>.
    /// </remarks>
    [Flags]
    public enum TextFormatFlags
    {
        Bottom                              = (int)User32.DT.BOTTOM,
        EndEllipsis                         = (int)User32.DT.END_ELLIPSIS,
        ExpandTabs                          = (int)User32.DT.EXPANDTABS,
        ExternalLeading                     = (int)User32.DT.EXTERNALLEADING,
        Default                             = (int)User32.DT.DEFAULT,
        HidePrefix                          = (int)User32.DT.HIDEPREFIX,
        HorizontalCenter                    = (int)User32.DT.CENTER,
        Internal                            = (int)User32.DT.INTERNAL,

        /// <remarks>
        ///  This is the default.
        /// </remarks>
        Left                                = (int)User32.DT.LEFT,

        [Obsolete("ModifyString mutates strings and should be avoided. It will be blocked in a future release.")]
        ModifyString                        = (int)User32.DT.MODIFYSTRING,
        NoClipping                          = (int)User32.DT.NOCLIP,
        NoPrefix                            = (int)User32.DT.NOPREFIX,
        NoFullWidthCharacterBreak           = (int)User32.DT.NOFULLWIDTHCHARBREAK,
        PathEllipsis                        = (int)User32.DT.PATH_ELLIPSIS,
        PrefixOnly                          = (int)User32.DT.PREFIXONLY,
        Right                               = (int)User32.DT.RIGHT,
        RightToLeft                         = (int)User32.DT.RTLREADING,
        SingleLine                          = (int)User32.DT.SINGLELINE,
        TextBoxControl                      = (int)User32.DT.EDITCONTROL,

        /// <remarks>
        ///  This is the default.
        /// </remarks>
        Top                                 = (int)User32.DT.TOP,

        VerticalCenter                      = (int)User32.DT.VCENTER,
        WordBreak                           = (int)User32.DT.WORDBREAK,
        WordEllipsis                        = (int)User32.DT.WORD_ELLIPSIS,

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
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the display and layout information for text strings.
    /// </summary>
    [Flags]
    public enum TextFormatFlags
    {
        Bottom = (int)User32.TextFormatFlags.DT_BOTTOM,
        EndEllipsis = (int)User32.TextFormatFlags.DT_END_ELLIPSIS,
        ExpandTabs = (int)User32.TextFormatFlags.DT_EXPANDTABS,
        ExternalLeading = (int)User32.TextFormatFlags.DT_EXTERNALLEADING,
        Default = (int)User32.TextFormatFlags.DT_DEFAULT,
        HidePrefix = (int)User32.TextFormatFlags.DT_HIDEPREFIX,
        HorizontalCenter = (int)User32.TextFormatFlags.DT_CENTER,
        Internal = (int)User32.TextFormatFlags.DT_INTERNAL,

        // This is the default.
        Left = (int)User32.TextFormatFlags.DT_LEFT,
        ModifyString = (int)User32.TextFormatFlags.DT_MODIFYSTRING,
        NoClipping = (int)User32.TextFormatFlags.DT_NOCLIP,
        NoPrefix = (int)User32.TextFormatFlags.DT_NOPREFIX,
        NoFullWidthCharacterBreak = (int)User32.TextFormatFlags.DT_NOFULLWIDTHCHARBREAK,
        PathEllipsis = (int)User32.TextFormatFlags.DT_PATH_ELLIPSIS,
        PrefixOnly = (int)User32.TextFormatFlags.DT_PREFIXONLY,
        Right = (int)User32.TextFormatFlags.DT_RIGHT,
        RightToLeft = (int)User32.TextFormatFlags.DT_RTLREADING,
        SingleLine = (int)User32.TextFormatFlags.DT_SINGLELINE,
        TextBoxControl = (int)User32.TextFormatFlags.DT_EDITCONTROL,

        // This is the default.
        Top = (int)User32.TextFormatFlags.DT_TOP,

        VerticalCenter = (int)User32.TextFormatFlags.DT_VCENTER,
        WordBreak = (int)User32.TextFormatFlags.DT_WORDBREAK,
        WordEllipsis = (int)User32.TextFormatFlags.DT_WORD_ELLIPSIS,

        /// <summary>
        ///  The following flags are exclusive of TextRenderer (no Windows native flags)
        ///  and apply to methods receiving a Graphics as the IDeviceContext object, and
        ///  specify whether to reapply clipping and coordintate transformations to the hdc
        ///  obtained from the Graphics object, which returns a clean hdc.
        /// </summary>
        PreserveGraphicsClipping = 0x01000000,
        PreserveGraphicsTranslateTransform = 0x02000000,

        /// <summary>
        ///  Adds padding related to the drawing binding box, computed according to the font size.
        ///  Match the System.Internal.GDI.TextPaddingOptions.
        /// </summary>
        // This is the default.
        GlyphOverhangPadding = 0x00000000,
        NoPadding = 0x10000000,
        LeftAndRightPadding = 0x20000000
    }
}

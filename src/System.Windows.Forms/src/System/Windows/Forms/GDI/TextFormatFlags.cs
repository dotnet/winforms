// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Note: This is a public enum wrapping the internal Interop.User32.TextFormatFlags.
    /// </summary>
    [Flags]
    public enum TextFormatFlags
    {
        Bottom = (int)Interop.User32.TextFormatFlags.DT_BOTTOM,
        EndEllipsis = (int)Interop.User32.TextFormatFlags.DT_END_ELLIPSIS,
        ExpandTabs = (int)Interop.User32.TextFormatFlags.DT_EXPANDTABS,
        ExternalLeading = (int)Interop.User32.TextFormatFlags.DT_EXTERNALLEADING,
        Default = (int)Interop.User32.TextFormatFlags.DT_DEFAULT,
        HidePrefix = (int)Interop.User32.TextFormatFlags.DT_HIDEPREFIX,
        HorizontalCenter = (int)Interop.User32.TextFormatFlags.DT_CENTER,
        Internal = (int)Interop.User32.TextFormatFlags.DT_INTERNAL,

        /// <remarks>
        ///  This is the default.
        /// </remarks>
        Left = (int)Interop.User32.TextFormatFlags.DT_LEFT,
        ModifyString = (int)Interop.User32.TextFormatFlags.DT_MODIFYSTRING,
        NoClipping = (int)Interop.User32.TextFormatFlags.DT_NOCLIP,
        NoPrefix = (int)Interop.User32.TextFormatFlags.DT_NOPREFIX,
        NoFullWidthCharacterBreak = (int)Interop.User32.TextFormatFlags.DT_NOFULLWIDTHCHARBREAK,
        PathEllipsis = (int)Interop.User32.TextFormatFlags.DT_PATH_ELLIPSIS,
        PrefixOnly = (int)Interop.User32.TextFormatFlags.DT_PREFIXONLY,
        Right = (int)Interop.User32.TextFormatFlags.DT_RIGHT,
        RightToLeft = (int)Interop.User32.TextFormatFlags.DT_RTLREADING,
        SingleLine = (int)Interop.User32.TextFormatFlags.DT_SINGLELINE,
        TextBoxControl = (int)Interop.User32.TextFormatFlags.DT_EDITCONTROL,

        /// <remarks>
        ///  This is the default.
        /// </remarks>
        Top = (int)Interop.User32.TextFormatFlags.DT_TOP,

        VerticalCenter = (int)Interop.User32.TextFormatFlags.DT_VCENTER,
        WordBreak = (int)Interop.User32.TextFormatFlags.DT_WORDBREAK,
        WordEllipsis = (int)Interop.User32.TextFormatFlags.DT_WORD_ELLIPSIS,

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
        /// <remarks>
        ///  This is the default.
        /// </remarks>
        GlyphOverhangPadding = 0x00000000,
        NoPadding = 0x10000000,
        LeftAndRightPadding = 0x20000000
    }
}

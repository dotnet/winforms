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
        Bottom = (int)User32.DT.BOTTOM,
        EndEllipsis = (int)User32.DT.END_ELLIPSIS,
        ExpandTabs = (int)User32.DT.EXPANDTABS,
        ExternalLeading = (int)User32.DT.EXTERNALLEADING,
        Default = (int)User32.DT.DEFAULT,
        HidePrefix = (int)User32.DT.HIDEPREFIX,
        HorizontalCenter = (int)User32.DT.CENTER,
        Internal = (int)User32.DT.INTERNAL,

        /// <remarks>
        ///  This is the default.
        /// </remarks>
        Left = (int)User32.DT.LEFT,

        [Obsolete("ModifyString mutates strings and should be avoided. It will be blocked in a future release.")]
        ModifyString = (int)User32.DT.MODIFYSTRING,
        NoClipping = (int)User32.DT.NOCLIP,
        NoPrefix = (int)User32.DT.NOPREFIX,
        NoFullWidthCharacterBreak = (int)User32.DT.NOFULLWIDTHCHARBREAK,
        PathEllipsis = (int)User32.DT.PATH_ELLIPSIS,
        PrefixOnly = (int)User32.DT.PREFIXONLY,
        Right = (int)User32.DT.RIGHT,
        RightToLeft = (int)User32.DT.RTLREADING,
        SingleLine = (int)User32.DT.SINGLELINE,
        TextBoxControl = (int)User32.DT.EDITCONTROL,

        /// <remarks>
        ///  This is the default.
        /// </remarks>
        Top = (int)User32.DT.TOP,

        VerticalCenter = (int)User32.DT.VCENTER,
        WordBreak = (int)User32.DT.WORDBREAK,
        WordEllipsis = (int)User32.DT.WORD_ELLIPSIS,

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

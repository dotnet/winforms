// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.Internal;

namespace System.Windows.Forms
{
    /// <summary>
    /// Note: This is a public enum wrapping the internal Interop.User32.TextFormatFlags
    /// defined in the System.Windows.Forms.Internal namespace, we need to do this to
    /// be able to compile the internal one into different assemblies w/o
    /// creating any conflict/dependency on public namespaces.
    /// Additionally, TextFormatFlags has some extra values.
    /// </summary>
    [Flags]
    public enum TextFormatFlags
    {
        Bottom = Interop.User32.TextFormatFlags.DT_BOTTOM,
        EndEllipsis = Interop.User32.TextFormatFlags.DT_END_ELLIPSIS,
        ExpandTabs = Interop.User32.TextFormatFlags.DT_EXPANDTABS,
        ExternalLeading = Interop.User32.TextFormatFlags.DT_EXTERNALLEADING,
        Default = Interop.User32.TextFormatFlags.DT_DEFAULT,
        HidePrefix = Interop.User32.TextFormatFlags.DT_HIDEPREFIX,
        HorizontalCenter = Interop.User32.TextFormatFlags.DT_CENTER,
        Internal = Interop.User32.TextFormatFlags.DT_INTERNAL,
        Left = Interop.User32.TextFormatFlags.DT_LEFT,  // default
        ModifyString = Interop.User32.TextFormatFlags.DT_MODIFYSTRING,
        NoClipping = Interop.User32.TextFormatFlags.DT_NOCLIP,
        NoPrefix = Interop.User32.TextFormatFlags.DT_NOPREFIX,
        NoFullWidthCharacterBreak = Interop.User32.TextFormatFlags.DT_NOFULLWIDTHCHARBREAK,
        PathEllipsis = Interop.User32.TextFormatFlags.DT_PATH_ELLIPSIS,
        PrefixOnly = Interop.User32.TextFormatFlags.DT_PREFIXONLY,
        Right = Interop.User32.TextFormatFlags.DT_RIGHT,
        RightToLeft = Interop.User32.TextFormatFlags.DT_RTLREADING,
        SingleLine = Interop.User32.TextFormatFlags.DT_SINGLELINE,
        // NOTE: TextRenderer does not expose a way to set the tab stops. Observe that ExapandTabs is available.
        // TabStop                     = Interop.User32.TextFormatFlags.TabStop,
        TextBoxControl = Interop.User32.TextFormatFlags.DT_EDITCONTROL,
        Top = Interop.User32.TextFormatFlags.DT_TOP, // default
        VerticalCenter = Interop.User32.TextFormatFlags.DT_VCENTER,

        WordBreak = Interop.User32.TextFormatFlags.DT_WORDBREAK,
        WordEllipsis = Interop.User32.TextFormatFlags.DT_WORD_ELLIPSIS,

        /// <summary>
        /// The following flags are exclusive of TextRenderer (no Windows native flags)
        /// and apply to methods receiving a Graphics as the IDeviceContext object, and
        /// specify whether to reapply clipping and coordintate transformations to the hdc
        /// obtained from the Graphics object, which returns a clean hdc.
        /// </summary>
        PreserveGraphicsClipping = 0x01000000,
        PreserveGraphicsTranslateTransform = 0x02000000,

        /// <summary>
        /// Adds padding related to the drawing binding box, computed according to the font size.
        /// Match the System.Internal.GDI.TextPaddingOptions.
        /// </summary>
        GlyphOverhangPadding = 0x00000000, // default.
        NoPadding = 0x10000000,
        LeftAndRightPadding = 0x20000000
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Windows.Forms.Internal;
    using System.Diagnostics.CodeAnalysis;

    /// <devdoc>
    ///     TextFormatFlags enum defining options for drawing/measuring text in 
    ///     TextRenderer.
    ///     
    ///     Note: This is a public enum wrapping the internal IntTextFormatFlags
    ///     defined in the System.Windows.Forms.Internal namespace, we need to do this to
    ///     be able to compile the internal one into different assemblies w/o
    ///     creating any conflict/dependency on public namespaces.
    ///     Additionally, TextFormatFlags has some extra values.
    /// </devdoc>
    [Flags]
    // PM team has reviewed and decided on naming changes already
    [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum TextFormatFlags
    {
        Bottom                      = IntTextFormatFlags.Bottom,
        // NOTE: This flag is used for measuring text and TextRenderer has methods for doing that
        // so we don't expose it to avoid confusion.
        // CalculateRectangle          = IntTextFormatFlags.CalculateRectangle,
        EndEllipsis                 = IntTextFormatFlags.EndEllipsis,
        ExpandTabs                  = IntTextFormatFlags.ExpandTabs,
        ExternalLeading             = IntTextFormatFlags.ExternalLeading,
        Default                     = IntTextFormatFlags.Default,
        HidePrefix                  = IntTextFormatFlags.HidePrefix,
        HorizontalCenter            = IntTextFormatFlags.HorizontalCenter,
        Internal                    = IntTextFormatFlags.Internal,
        Left                        = IntTextFormatFlags.Left,  // default
        ModifyString                = IntTextFormatFlags.ModifyString,
        NoClipping                  = IntTextFormatFlags.NoClipping,
        NoPrefix                    = IntTextFormatFlags.NoPrefix,
        NoFullWidthCharacterBreak   = IntTextFormatFlags.NoFullWidthCharacterBreak,
        PathEllipsis                = IntTextFormatFlags.PathEllipsis,
        PrefixOnly                  = IntTextFormatFlags.PrefixOnly,
        Right                       = IntTextFormatFlags.Right,
        RightToLeft                 = IntTextFormatFlags.RightToLeft,
        SingleLine                  = IntTextFormatFlags.SingleLine,
        // NOTE: TextRenderer does not expose a way to set the tab stops. Observe that ExapandTabs is available.
        // TabStop                     = IntTextFormatFlags.TabStop,
        TextBoxControl              = IntTextFormatFlags.TextBoxControl,
        Top                         = IntTextFormatFlags.Top, // default
        VerticalCenter              = IntTextFormatFlags.VerticalCenter,
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly")] //Not a compound word
        WordBreak                   = IntTextFormatFlags.WordBreak,
        WordEllipsis                = IntTextFormatFlags.WordEllipsis,

        /// <devdoc>
        ///     The following flags are exclusive of TextRenderer (no Windows native flags)
        ///     and apply to methods receiving a Graphics as the IDeviceContext object, and
        ///     specify whether to reapply clipping and coordintate transformations to the hdc
        ///     obtained from the Graphics object, which returns a clean hdc.
        /// </devdoc>
        PreserveGraphicsClipping           = 0x01000000,
        PreserveGraphicsTranslateTransform = 0x02000000,

        /// <devdoc>
        ///     Adds padding related to the drawing binding box, computed according to the font size.
        ///     Match the System.Internal.GDI.TextPaddingOptions.
        /// </devdoc>
        GlyphOverhangPadding = 0x00000000, // default.
        NoPadding            = 0x10000000,
        LeftAndRightPadding  = 0x20000000
    }
}

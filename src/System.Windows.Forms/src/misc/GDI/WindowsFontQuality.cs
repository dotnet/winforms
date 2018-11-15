// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if WINFORMS_NAMESPACE
namespace System.Windows.Forms.Internal
#elif DRAWING_NAMESPACE
namespace System.Drawing.Internal
#else
namespace System.Experimental.Gdi
#endif
{
    /// <devdoc>
    ///    Device capability indexes - See Win32 GetDeviceCaps().
    /// </devdoc>
#if WINFORMS_PUBLIC_GRAPHICS_LIBRARY
    public
#else
    internal
#endif
    enum WindowsFontQuality
    {
        /// Appearance of the font does not matter.
        Default = IntNativeMethods.DEFAULT_QUALITY,

        /// Appearance of the font is less important than when PROOF_QUALITY is used. 
        /// For GDI raster fonts, scaling is enabled, which means that more font sizes are available, 
        /// but the quality may be lower. Bold, italic, underline, and strikeout fonts are synthesized if necessary.
        Draft = IntNativeMethods.DRAFT_QUALITY,

        /// Character quality of the font is more important than exact matching of the logical-font attributes. 
        /// For GDI raster fonts, scaling is disabled and the font closest in size is chosen. 
        /// Although the chosen font size may not be mapped exactly when PROOF_QUALITY is used, the quality of 
        /// the font is high and there is no distortion of appearance. Bold, italic, underline, and strikeout 
        /// fonts are synthesized if necessary.
        Proof = IntNativeMethods.PROOF_QUALITY,

        /// Font is never antialiased.
        /// WINVER >= 0x0400
        NonAntiAliased = IntNativeMethods.NONANTIALIASED_QUALITY,

        /// Font is always antialiased if the font supports it and the size of the font is not too small or too large. 
        /// See note for Win9x systems.
        /// WINVER >= 0x0400
        AntiAliased = IntNativeMethods.ANTIALIASED_QUALITY,

        /// Windows XP: The following situations do not support ClearType antialiasing: 
        /// - Text is rendered on a printer. 
        /// - Display set for 256 colors or less. 
        /// - Text is rendered to a terminal server client. 
        /// - The font is not a TrueType font or an OpenType font with TrueType outlines. 
        ///   For example, the following do not support ClearType antialiasing: 
        ///     Type 1 fonts, Postscript OpenType fonts without TrueType outlines, bitmap fonts, vector fonts, and device fonts.
        /// - The font has tuned embedded bitmaps, for any font sizes that contain the embedded bitmaps. 
        ///   For example, this occurs commonly in East Asian fonts. 
        /// Windows XP: If set, text is rendered (when possible) using ClearType antialiasing method. 
        /// See Remarks for more information.
        /// WIN32_WINNT >= 0x0500
        ClearType        = IntNativeMethods.CLEARTYPE_QUALITY,
        /// WIN32_WINNT >= 0x0501
        ClearTypeNatural = IntNativeMethods.CLEARTYPE_NATURAL_QUALITY
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.Internal;
using static Interop;

namespace System.Windows.Forms
{
    internal static class TextExtensions
    {
        // The value of the ItalicPaddingFactor comes from several tests using different fonts & drawing
        // flags and some benchmarking with GDI+.
        private const float ItalicPaddingFactor = 1 / 2f;

        // Used to clear TextRenderer specific flags from TextFormatFlags
        internal const int GdiUnsupportedFlagMask = (unchecked((int)0xFF000000));

        [Conditional("DEBUG")]
        private static void ValidateFlags(User32.DT flags)
        {
            Debug.Assert(((uint)flags & GdiUnsupportedFlagMask) == 0,
                "Some custom flags were left over and are not GDI compliant!");
        }

        private static (User32.DT Flags, TextPaddingOptions Padding) SplitTextFormatFlags(TextFormatFlags flags)
        {
            if (((uint)flags & GdiUnsupportedFlagMask) == 0)
            {
                return ((User32.DT)flags, TextPaddingOptions.GlyphOverhangPadding);
            }

            // Clear TextRenderer custom flags.
            User32.DT windowsGraphicsSupportedFlags = (User32.DT)((uint)flags & ~GdiUnsupportedFlagMask);

            TextPaddingOptions padding = flags.HasFlag(TextFormatFlags.LeftAndRightPadding)
                ? TextPaddingOptions.LeftAndRightPadding
                : flags.HasFlag(TextFormatFlags.NoPadding)
                    ? TextPaddingOptions.NoPadding
                    : TextPaddingOptions.GlyphOverhangPadding;

            return (windowsGraphicsSupportedFlags, padding);
        }

        /// <summary>
        ///  Draws the text in the given bounds, using the given Font, foreColor and backColor, and according to the specified
        ///  TextFormatFlags flags.
        ///
        ///  If font is null, the font currently selected in the hdc is used.
        ///
        ///  If foreColor and/or backColor are Color.Empty, the hdc current text and/or background color are used.
        /// </summary>
        public static void DrawText(
            this Gdi32.HDC hdc,
            ReadOnlySpan<char> text,
            FontCache.Scope font,
            Rectangle bounds,
            Color foreColor,
            TextFormatFlags flags,
            Color backColor = default)
        {
            if (text.IsEmpty || foreColor == Color.Transparent)
                return;

            (User32.DT dt, TextPaddingOptions padding) = SplitTextFormatFlags(flags);

            // DrawText requires default text alignment.
            using var alignment = new Gdi32.SetTextAlignmentScope(hdc, default);

            // Color empty means use the one currently selected in the dc.
            using var textColor = foreColor.IsEmpty ? default : new Gdi32.SetTextColorScope(hdc, foreColor);
            using var fontSelection = new Gdi32.SelectObjectScope(hdc, (Gdi32.HFONT)font);

            Gdi32.BKMODE newBackGroundMode = (backColor.IsEmpty || backColor == Color.Transparent) ?
                Gdi32.BKMODE.TRANSPARENT :
                Gdi32.BKMODE.OPAQUE;

            using var backgroundMode = new Gdi32.SetBkModeScope(hdc, newBackGroundMode);
            using var backgroundColor = newBackGroundMode != Gdi32.BKMODE.TRANSPARENT
                ? new Gdi32.SetBackgroundColorScope(hdc, backColor)
                : default;

            User32.DRAWTEXTPARAMS dtparams = GetTextMargins(font, padding);

            bounds = AdjustForVerticalAlignment(hdc, text, bounds, dt, ref dtparams);

            // Adjust unbounded rect to avoid overflow since Rectangle ctr does not do param validation.
            if (bounds.Width == TextRenderer.MaxSize.Width)
            {
                bounds.Width -= bounds.X;
            }

            if (bounds.Height == TextRenderer.MaxSize.Height)
            {
                bounds.Height -= bounds.Y;
            }

            RECT rect = bounds;
            User32.DrawTextExW(hdc, text, ref rect, dt, ref dtparams);
        }

        /// <summary>
        ///  Get the bounding box internal text padding to be used when drawing text.
        /// </summary>
        public static User32.DRAWTEXTPARAMS GetTextMargins(
            this FontCache.Scope font,
            TextPaddingOptions padding = default)
        {
            // DrawText(Ex) adds a small space at the beginning of the text bounding box but not at the end,
            // this is more noticeable when the font has the italic style.  We compensate with this factor.

            int leftMargin = 0;
            int rightMargin = 0;
            float overhangPadding;

            switch (padding)
            {
                case TextPaddingOptions.GlyphOverhangPadding:
                    // [overhang padding][Text][overhang padding][italic padding]
                    overhangPadding = font.Data.Height / 6f;
                    leftMargin = (int)Math.Ceiling(overhangPadding);
                    rightMargin = (int)Math.Ceiling(overhangPadding * (1 + ItalicPaddingFactor));
                    break;

                case TextPaddingOptions.LeftAndRightPadding:
                    // [2 * overhang padding][Text][2 * overhang padding][italic padding]
                    overhangPadding = font.Data.Height / 6f;
                    leftMargin = (int)Math.Ceiling(2 * overhangPadding);
                    rightMargin = (int)Math.Ceiling(overhangPadding * (2 + ItalicPaddingFactor));
                    break;

                case TextPaddingOptions.NoPadding:
                default:
                    break;
            }

            return new User32.DRAWTEXTPARAMS
            {
                iLeftMargin = leftMargin,
                iRightMargin = rightMargin
            };
        }

        /// <summary>
        ///  The GDI DrawText does not do multiline alignment when User32.DT.SINGLELINE is not set. This
        ///  adjustment is to workaround that limitation. We don't want to duplicate SelectObject calls here,
        ///  so put your Font in the dc before calling this.
        ///
        ///  AdjustForVerticalAlignment is only used when the text is multiline and it fits inside the bounds passed in.
        ///  In that case we want the horizontal center of the multiline text to be at the horizontal center of the bounds.
        ///
        ///  If the text is multiline and it does not fit inside the bounds passed in, then return the bounds that were passed in.
        ///  This way we paint the top of the text at the top of the bounds passed in.
        /// </summary>
        public static Rectangle AdjustForVerticalAlignment(
            this Gdi32.HDC hdc,
            ReadOnlySpan<char> text,
            Rectangle bounds,
            User32.DT flags,
            ref User32.DRAWTEXTPARAMS dtparams)
        {
            ValidateFlags(flags);

            // Ok if any Top (Cannot test User32.DT.Top because it is 0), single line text or measuring text.
            bool isTop = (flags & User32.DT.BOTTOM) == 0 && (flags & User32.DT.VCENTER) == 0;
            if (isTop || ((flags & User32.DT.SINGLELINE) != 0) || ((flags & User32.DT.CALCRECT) != 0))
            {
                return bounds;
            }

            RECT rect = new RECT(bounds);

            // Get the text bounds.
            flags |= User32.DT.CALCRECT;
            int textHeight = User32.DrawTextExW(hdc, text, ref rect, flags, ref dtparams);

            // if the text does not fit inside the bounds then return the bounds that were passed in
            if (textHeight > bounds.Height)
            {
                return bounds;
            }

            Rectangle adjustedBounds = bounds;

            if ((flags & User32.DT.VCENTER) != 0)
            {
                // Middle
                adjustedBounds.Y = adjustedBounds.Top + adjustedBounds.Height / 2 - textHeight / 2;
            }
            else
            {
                // Bottom
                adjustedBounds.Y = adjustedBounds.Bottom - textHeight;
            }

            return adjustedBounds;
        }

        /// <summary>
        ///  Returns the Size in logical units of the given text using the given Font, and according to the formatting flags.
        ///  The proposed size is used to create a bounding rectangle as follows:
        ///  - If there are multiple lines of text, DrawText uses the width of the rectangle pointed to by
        ///  the lpRect parameter and extends the base of the rectangle to bound the last line of text.
        ///  - If the largest word is wider than the rectangle, the width is expanded.
        ///  - If the text is less than the width of the rectangle, the width is reduced.
        ///  - If there is only one line of text, DrawText modifies the right side of the rectangle so that
        ///  it bounds the last character in the line.
        ///  If the font is null, the hdc's current font will be used.
        ///
        ///  Note for vertical fonts (if ever supported): DrawTextEx uses GetTextExtentPoint32 for measuring the text and this
        ///  function has the following limitation (from MSDN):
        ///  - This function assumes that the text is horizontal, that is, that the escapement is always 0. This is true for both
        ///  the horizontal and vertical measurements of the text.  The application must convert it explicitly.
        /// </summary>
        public static Size MeasureText(
            this Gdi32.HDC hdc,
            ReadOnlySpan<char> text,
            FontCache.Scope font,
            Size proposedSize,
            TextFormatFlags flags)
        {
            (User32.DT dt, TextPaddingOptions padding) = SplitTextFormatFlags(flags);

            if (text.IsEmpty)
            {
                return Size.Empty;
            }

            // DrawText returns a rectangle useful for aligning, but not guaranteed to encompass all
            // pixels (its not a FitBlackBox, if the text is italicized, it will overhang on the right.)
            // So we need to account for this.

            User32.DRAWTEXTPARAMS dtparams = GetTextMargins(font, padding);

            // If Width / Height are < 0, we need to make them larger or DrawText will return
            // an unbounded measurement when we actually trying to make it very narrow.
            int minWidth = 1 + dtparams.iLeftMargin + dtparams.iRightMargin;

            if (proposedSize.Width <= minWidth)
            {
                proposedSize.Width = minWidth;
            }

            if (proposedSize.Height <= 0)
            {
                proposedSize.Height = 1;
            }

            var rect = new RECT(0, 0, proposedSize.Width, proposedSize.Height);

            using var fontSelection = new Gdi32.SelectObjectScope(hdc, font.Object);

            // If proposedSize.Height >= MaxSize.Height it is assumed bounds needed.  If flags contain SINGLELINE and
            // VCENTER or BOTTOM options, DrawTextEx does not bind the rectangle to the actual text height since
            // it assumes the text is to be vertically aligned; we need to clear the VCENTER and BOTTOM flags to
            // get the actual text bounds.
            if (proposedSize.Height >= TextRenderer.MaxSize.Height && (dt & User32.DT.SINGLELINE) != 0)
            {
                // Clear vertical-alignment flags.
                dt &= ~(User32.DT.BOTTOM | User32.DT.VCENTER);
            }

            if (proposedSize.Width == TextRenderer.MaxSize.Width)
            {
                // PERF: No constraining width means no word break.
                // in this case, we dont care about word wrapping - there should be enough room to fit it all
                dt &= ~(User32.DT.WORDBREAK);
            }

            dt |= User32.DT.CALCRECT;
            User32.DrawTextExW(hdc, text, ref rect, dt, ref dtparams);

            return rect.Size;
        }

        /// <summary>
        ///  Returns the Size of the given text using the specified font if not null, otherwise the font currently
        ///  set in the dc is used.
        ///  This method is used to get the size in points of a line of text; it uses GetTextExtentPoint32 function
        ///  which computes the width and height of the text ignoring TAB\CR\LF characters.
        ///  A text extent is the distance between the beginning of the space and a character that will fit in the space.
        /// </summary>
        public static Size GetTextExtent(this Gdi32.HDC hdc, string? text, Gdi32.HFONT hfont)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Size.Empty;
            }

            Size size = new Size();
            using var selectFont = new Gdi32.SelectObjectScope(hdc, hfont);

            Gdi32.GetTextExtentPoint32W(hdc, text, text.Length, ref size);

            return new Size(size.Width, size.Height);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Internal;

namespace System.Windows.Forms;

internal static class TextExtensions
{
    // The value of the ItalicPaddingFactor comes from several tests using different fonts & drawing
    // flags and some benchmarking with GDI+.
    private const float ItalicPaddingFactor = 1 / 2f;

    // Used to clear TextRenderer specific flags from TextFormatFlags
    internal const int GdiUnsupportedFlagMask = unchecked((int)0xFF000000);

    [Conditional("DEBUG")]
    private static void ValidateFlags(DRAW_TEXT_FORMAT flags)
    {
        Debug.Assert(((uint)flags & GdiUnsupportedFlagMask) == 0,
            "Some custom flags were left over and are not GDI compliant!");
    }

    private static (DRAW_TEXT_FORMAT Flags, TextPaddingOptions Padding) SplitTextFormatFlags(TextFormatFlags flags)
    {
        if (((uint)flags & GdiUnsupportedFlagMask) == 0)
        {
            return ((DRAW_TEXT_FORMAT)flags, TextPaddingOptions.GlyphOverhangPadding);
        }

        // Clear TextRenderer custom flags.
        DRAW_TEXT_FORMAT windowsGraphicsSupportedFlags = (DRAW_TEXT_FORMAT)((uint)flags & ~GdiUnsupportedFlagMask);

        TextPaddingOptions padding = flags.HasFlag(TextFormatFlags.LeftAndRightPadding)
            ? TextPaddingOptions.LeftAndRightPadding
            : flags.HasFlag(TextFormatFlags.NoPadding)
                ? TextPaddingOptions.NoPadding
                : TextPaddingOptions.GlyphOverhangPadding;

        return (windowsGraphicsSupportedFlags, padding);
    }

    /// <summary>
    ///  Draws the given <paramref name="text"/> text in the given <paramref name="hdc"/>.
    /// </summary>
    /// <param name="backColor">If <see cref="Color.Empty"/>, the hdc current background color is used.</param>
    /// <param name="foreColor">If <see cref="Color.Empty"/>, the hdc current foreground color is used.</param>
    public static unsafe void DrawText(
        this HDC hdc,
        ReadOnlySpan<char> text,
        FontCache.Scope font,
        Rectangle bounds,
        Color foreColor,
        TextFormatFlags flags,
        Color backColor = default)
    {
        if (text.IsEmpty || foreColor == Color.Transparent)
        {
            return;
        }

        (DRAW_TEXT_FORMAT dt, TextPaddingOptions padding) = SplitTextFormatFlags(flags);

        // DrawText requires default text alignment.
        using SetTextAlignmentScope alignment = new(hdc, default);

        // Color empty means use the one currently selected in the dc.
        using var textColor = foreColor.IsEmpty ? default : new SetTextColorScope(hdc, foreColor);
        using SelectObjectScope fontSelection = new(hdc, (HFONT)font);

        BACKGROUND_MODE newBackGroundMode = (backColor.IsEmpty || backColor == Color.Transparent)
            ? BACKGROUND_MODE.TRANSPARENT
            : BACKGROUND_MODE.OPAQUE;

        using SetBkModeScope backgroundMode = new(hdc, newBackGroundMode);
        using var backgroundColor = newBackGroundMode != BACKGROUND_MODE.TRANSPARENT
            ? new SetBackgroundColorScope(hdc, backColor)
            : default;

        DRAWTEXTPARAMS dtparams = GetTextMargins(font, padding);

        bounds = AdjustForVerticalAlignment(hdc, text, bounds, dt, &dtparams);

        // Adjust unbounded rect to avoid overflow.
        if (bounds.Width == int.MaxValue)
        {
            bounds.Width -= bounds.X;
        }

        if (bounds.Height == int.MaxValue)
        {
            bounds.Height -= bounds.Y;
        }

        RECT rect = bounds;
        PInvoke.DrawTextEx(hdc, text, &rect, dt, &dtparams);
    }

    /// <summary>
    ///  Get the bounding box internal text padding to be used when drawing text.
    /// </summary>
    public static DRAWTEXTPARAMS GetTextMargins(
        this FontCache.Scope font,
        TextPaddingOptions padding = default)
    {
        // DrawText(Ex) adds a small space at the beginning of the text bounding box but not at the end,
        // this is more noticeable when the font has the italic style. We compensate with this factor.

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

        return new DRAWTEXTPARAMS
        {
            iLeftMargin = leftMargin,
            iRightMargin = rightMargin
        };
    }

    /// <summary>
    ///  Adjusts <paramref name="bounds"/> to allow for vertical alignment.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The GDI DrawText does not do multiline alignment when User32.DT.SINGLELINE is not set. This
    ///   adjustment is to workaround that limitation.
    ///  </para>
    /// </remarks>
    public static unsafe Rectangle AdjustForVerticalAlignment(
        this HDC hdc,
        ReadOnlySpan<char> text,
        Rectangle bounds,
        DRAW_TEXT_FORMAT flags,
        DRAWTEXTPARAMS* dtparams)
    {
        ValidateFlags(flags);

        // No need to do anything if TOP (Cannot test DT_TOP because it is 0), single line text or measuring text.
        bool isTop = !flags.HasFlag(DRAW_TEXT_FORMAT.DT_BOTTOM) && !flags.HasFlag(DRAW_TEXT_FORMAT.DT_VCENTER);
        if (isTop || flags.HasFlag(DRAW_TEXT_FORMAT.DT_SINGLELINE) || flags.HasFlag(DRAW_TEXT_FORMAT.DT_CALCRECT))
        {
            return bounds;
        }

        RECT rect = bounds;

        // Get the text bounds.
        flags |= DRAW_TEXT_FORMAT.DT_CALCRECT;
        int textHeight = PInvoke.DrawTextEx(hdc, text, &rect, flags, dtparams);

        // If the text does not fit inside the bounds then return the bounds that were passed in.
        // This way we paint the top of the text at the top of the bounds passed in.
        if (textHeight > bounds.Height)
        {
            return bounds;
        }

        Rectangle adjustedBounds = bounds;

        if (flags.HasFlag(DRAW_TEXT_FORMAT.DT_VCENTER))
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
    ///  Returns the bounds in logical units of the given <paramref name="text"/>.
    /// </summary>
    /// <param name="proposedSize">
    ///  <para>
    ///   The desired bounds. It will be modified as follows:
    ///  </para>
    ///  <list type="bullet">
    ///   <item><description>The base is extended to fit multiple lines of text.</description></item>
    ///   <item><description>The width is extended to fit the largest word.</description></item>
    ///   <item><description>The width is reduced if the text is smaller than the requested width.</description></item>
    ///   <item><description>The width is extended to fit a single line of text.</description></item>
    ///  </list>
    /// </param>
    public static unsafe Size MeasureText(
        this HDC hdc,
        ReadOnlySpan<char> text,
        FontCache.Scope font,
        Size proposedSize,
        TextFormatFlags flags)
    {
        (DRAW_TEXT_FORMAT dt, TextPaddingOptions padding) = SplitTextFormatFlags(flags);

        if (text.IsEmpty)
        {
            return Size.Empty;
        }

        // DrawText returns a rectangle useful for aligning, but not guaranteed to encompass all
        // pixels (its not a FitBlackBox, if the text is italicized, it will overhang on the right.)
        // So we need to account for this.

        DRAWTEXTPARAMS dtparams = GetTextMargins(font, padding);

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

        RECT rect = new(proposedSize);

        using SelectObjectScope fontSelection = new(hdc, font.Object);

        // If proposedSize.Height == int.MaxValue it is assumed bounds are needed. If flags contain SINGLELINE and
        // VCENTER or BOTTOM options, DrawTextEx does not bind the rectangle to the actual text height since
        // it assumes the text is to be vertically aligned; we need to clear the VCENTER and BOTTOM flags to
        // get the actual text bounds.
        if (proposedSize.Height == int.MaxValue && dt.HasFlag(DRAW_TEXT_FORMAT.DT_SINGLELINE))
        {
            // Clear vertical-alignment flags.
            dt &= ~(DRAW_TEXT_FORMAT.DT_BOTTOM | DRAW_TEXT_FORMAT.DT_VCENTER);
        }

        if (proposedSize.Width == int.MaxValue)
        {
            // If there is no constraining width, there should be no need to calculate word breaks.
            dt &= ~(DRAW_TEXT_FORMAT.DT_WORDBREAK);
        }

        dt |= DRAW_TEXT_FORMAT.DT_CALCRECT;
        PInvoke.DrawTextEx(hdc, text, &rect, dt, &dtparams);

        return rect.Size;
    }

    /// <summary>
    ///  Returns the dimensions the of the given <paramref name="text"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This method is used to get the size in logical units of a line of text; it uses GetTextExtentPoint32 function
    ///   which computes the width and height of the text ignoring TAB\CR\LF characters.
    ///  </para>
    ///  <para>
    ///   A text extent is the distance between the beginning of the space and a character that will fit in the space.
    ///  </para>
    /// </remarks>
    public static unsafe Size GetTextExtent(this HDC hdc, string? text, HFONT hfont)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Size.Empty;
        }

        Size size = default;
        using SelectObjectScope selectFont = new(hdc, hfont);

        fixed (char* pText = text)
        {
            PInvoke.GetTextExtentPoint32W(hdc, pText, text.Length, (SIZE*)(void*)&size);
        }

        return size;
    }
}

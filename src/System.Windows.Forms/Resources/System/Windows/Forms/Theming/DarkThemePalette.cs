// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Theming;

/// <summary>
/// A minimal, immutable palette for Dark Mode theming of DataGridView.
/// </summary>
public class DarkThemePalette
{
    /// <summary>
    ///  Gets the surface (background) color.
    /// </summary>
    public Color Surface { get; }

    /// <summary>
    ///  Gets the on-surface (foreground/text) color.
    /// </summary>
    public Color OnSurface { get; }

    /// <summary>
    ///  Gets the header background color.
    /// </summary>
    public Color HeaderBackgroundColor { get; }

    /// <summary>
    ///  Gets the header foreground color.
    /// </summary>
    public Color HeaderForegroundColor { get; }

    /// <summary>
    ///  Gets the selection background color.
    /// </summary>
    public Color SelectionBackgroundColor { get; }

    /// <summary>
    ///  Gets the selection foreground color.
    /// </summary>
    public Color SelectionForegroundColor { get; }

    /// <summary>
    ///  Gets the grid line color.
    /// </summary>
    public Color Grid { get; }

    /// <summary>
    ///  Gets the sort glyph (arrow) color for column headers.
    /// </summary>
    public Color SortGlyphColor { get; }

    public DarkThemePalette(
        Color surface,
        Color onSurface,
        Color headerBackgroundColor,
        Color headerForegroundColor,
        Color selectionBackgroundColor,
        Color selectionForegroundColor,
        Color grid,
        Color sortGlyphColor)
    {
        Surface = surface;
        OnSurface = onSurface;
        HeaderBackgroundColor = headerBackgroundColor;
        HeaderForegroundColor = headerForegroundColor;
        SelectionBackgroundColor = selectionBackgroundColor;
        SelectionForegroundColor = selectionForegroundColor;
        Grid = grid;
        SortGlyphColor = sortGlyphColor;
    }

    /// <summary>
    /// Creates a conservative default palette that follows SystemColors in Dark Mode
    /// and meets contrast requirements for selection states.
    /// </summary>
    public static DarkThemePalette CreateDefault()
    {
        Color surface = SystemColors.Window;
        Color onSurface = SystemColors.WindowText;
        Color headerBg = SystemColors.ControlDarkDark;
        Color headerFg = SystemColors.ActiveCaptionText;
        Color selectionBg = Color.FromArgb(0x33, 0x66, 0xCC);
        Color selectionFg = Color.White;
        Color grid = ControlPaint.Dark(surface, 0.6f);

        // Use a light gray color for sort glyph that provides good contrast on dark backgrounds
        Color sortGlyph = Color.White;

        return new DarkThemePalette(
            surface, onSurface, headerBg, headerFg, selectionBg, selectionFg, grid, sortGlyph);
    }
}

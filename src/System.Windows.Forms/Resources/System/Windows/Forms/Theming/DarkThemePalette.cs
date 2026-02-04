// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Theming;

/// <summary>
/// A minimal, immutable palette for Dark Mode theming of DataGridView.
/// </summary>
public readonly struct DarkThemePalette
{
    public Color Surface { get; }
    public Color OnSurface { get; }
    public Color HeaderBackgroundColor { get; }
    public Color HeaderForegroundColor { get; }
    public Color SelectionBackgroundColor { get; }
    public Color SelectionForegroundColor { get; }
    public Color Grid { get; }

    public DarkThemePalette(
        Color surface,
        Color onSurface,
        Color headerBackgroundColor,
        Color headerForegroundColor,
        Color selectionBackgroundColor,
        Color selectionForegroundColor,
        Color grid)
    {
        Surface = surface;
        OnSurface = onSurface;
        HeaderBackgroundColor = headerBackgroundColor;
        HeaderForegroundColor = headerForegroundColor;
        SelectionBackgroundColor = selectionBackgroundColor;
        SelectionForegroundColor = selectionForegroundColor;
        Grid = grid;
    }

    /// <summary>
    /// Creates a conservative default palette that follows SystemColors in Dark Mode
    /// and meets contrast requirements for selection states.
    /// </summary>
    public static DarkThemePalette CreateDefault()
    {
        var surface = SystemColors.Window;
        var onSurface = SystemColors.WindowText;
        var headerBg = SystemColors.ControlDarkDark;
        var headerFg = SystemColors.ActiveCaptionText;
        var selectionBg = Color.FromArgb(0x33, 0x66, 0xCC);
        var selectionFg = Color.White;
        var grid = ControlPaint.Dark(surface, 0.6f);

        return new DarkThemePalette(
            surface, onSurface, headerBg, headerFg, selectionBg, selectionFg, grid);
    }
}

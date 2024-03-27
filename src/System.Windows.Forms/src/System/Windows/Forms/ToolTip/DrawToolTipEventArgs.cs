// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  This class contains the information a user needs to paint the ToolTip.
/// </summary>
public class DrawToolTipEventArgs : EventArgs
{
    private readonly Color _backColor;
    private readonly Color _foreColor;

    /// <summary>
    ///  Creates a new DrawToolTipEventArgs with the given parameters.
    /// </summary>
    public DrawToolTipEventArgs(
        Graphics graphics,
        IWin32Window? associatedWindow,
        Control? associatedControl,
        Rectangle bounds,
        string? toolTipText,
        Color backColor,
        Color foreColor,
        Font? font)
    {
        Graphics = graphics.OrThrowIfNull();
        AssociatedWindow = associatedWindow;
        AssociatedControl = associatedControl;
        Bounds = bounds;
        ToolTipText = toolTipText;
        _backColor = backColor;
        _foreColor = foreColor;
        Font = font;
    }

    /// <summary>
    ///  Graphics object with which painting should be done.
    /// </summary>
    public Graphics Graphics { get; }

    /// <summary>
    ///  The window for which the tooltip is being painted.
    /// </summary>
    public IWin32Window? AssociatedWindow { get; }

    /// <summary>
    ///  The control for which the tooltip is being painted.
    /// </summary>
    public Control? AssociatedControl { get; }

    /// <summary>
    ///  The rectangle outlining the area in which the painting should be done.
    /// </summary>
    public Rectangle Bounds { get; }

    /// <summary>
    ///  The text that should be drawn.
    /// </summary>
    public string? ToolTipText { get; }

    /// <summary>
    ///  The font used to draw tooltip text.
    /// </summary>
    public Font? Font { get; }

    /// <summary>
    ///  Draws the background of the ToolTip.
    /// </summary>
    public void DrawBackground()
    {
        using var backBrush = _backColor.GetCachedSolidBrushScope();
        Graphics.FillRectangle(backBrush, Bounds);
    }

    /// <summary>
    ///  Draws the text (overloaded)
    /// </summary>
    public void DrawText()
    {
        // Pass in a set of flags to mimic default behavior
        DrawText(TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter
            | TextFormatFlags.SingleLine | TextFormatFlags.HidePrefix);
    }

    /// <summary>
    ///  Draws the text (overloaded) - takes a TextFormatFlags argument.
    /// </summary>
    public void DrawText(TextFormatFlags flags)
    {
        TextRenderer.DrawText(Graphics, ToolTipText, Font, Bounds, _foreColor, flags);
    }

    /// <summary>
    ///  Draws a border for the ToolTip similar to the default border.
    /// </summary>
    public void DrawBorder()
    {
        ControlPaint.DrawBorder(Graphics, Bounds, Application.SystemColors.WindowFrame, ButtonBorderStyle.Solid);
    }
}

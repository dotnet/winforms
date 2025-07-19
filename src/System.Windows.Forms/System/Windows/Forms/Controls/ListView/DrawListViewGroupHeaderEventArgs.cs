// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  This class contains the information a user needs to paint ListView
///  group headers.
/// </summary>
public class DrawListViewGroupHeaderEventArgs : EventArgs
{
    /// <summary>
    ///  Creates a new DrawListViewGroupHeaderEventArgs with the given parameters.
    /// </summary>
    public DrawListViewGroupHeaderEventArgs(
        Graphics graphics,
        Rectangle bounds,
        ListViewGroup? group,
        int groupId,
        ListViewGroupState state,
        Color foreColor,
        Color backColor,
        Font? font)
    {
        Graphics = graphics.OrThrowIfNull();
        Bounds = bounds;
        Group = group;
        GroupId = groupId;
        State = state;
        ForeColor = foreColor;
        BackColor = backColor;
        Font = font;
    }

    /// <summary>
    ///  Graphics object with which painting should be done.
    /// </summary>
    public Graphics Graphics { get; }

    /// <summary>
    ///  The rectangle outlining the area in which the painting should be done.
    /// </summary>
    public Rectangle Bounds { get; }

    /// <summary>
    ///  The ListViewGroup being drawn.
    /// </summary>
    public ListViewGroup? Group { get; }

    /// <summary>
    ///  The ID of the group being drawn.
    /// </summary>
    public int GroupId { get; }

    /// <summary>
    ///  State information pertaining to the group header.
    /// </summary>
    public ListViewGroupState State { get; }

    /// <summary>
    ///  Color used to draw the group header's text.
    /// </summary>
    public Color ForeColor { get; }

    /// <summary>
    ///  Color used to draw the group header's background.
    /// </summary>
    public Color BackColor { get; }

    /// <summary>
    ///  Font used to render the group header's text.
    /// </summary>
    public Font? Font { get; }

    /// <summary>
    ///  Causes the group header to be drawn by the system instead of owner drawn.
    /// </summary>
    public bool DrawDefault { get; set; }

    /// <summary>
    ///  Gets the header text of the group.
    /// </summary>
    public string? Header => Group?.Header;

    /// <summary>
    ///  Gets the subtitle text of the group.
    /// </summary>
    public string? Subtitle => Group?.Subtitle;

    /// <summary>
    ///  Gets whether the group is collapsed.
    /// </summary>
    public bool IsCollapsed => Group?.CollapsedState == ListViewGroupCollapsedState.Collapsed;

    /// <summary>
    ///  Draws the group header's background.
    /// </summary>
    public void DrawBackground()
    {
        using var backBrush = BackColor.GetCachedSolidBrushScope();
        Graphics.FillRectangle(backBrush, Bounds);
    }

    /// <summary>
    ///  Draws the group header's text.
    /// </summary>
    public void DrawText()
    {
        DrawText(TextFormatFlags.Left);
    }

    /// <summary>
    ///  Draws the group header's text with the specified format flags.
    /// </summary>
    public void DrawText(TextFormatFlags flags)
    {
        if (!string.IsNullOrEmpty(Header))
        {
            Rectangle textBounds = Bounds;
            textBounds.Inflate(-4, 0);
            TextRenderer.DrawText(Graphics, Header, Font, textBounds, ForeColor, flags);
        }
    }
}

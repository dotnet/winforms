// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

internal class TableLayoutPanelResizeGlyph : Glyph
{
    private Rectangle bounds;
    private Cursor hitTestCursor;
    private TableLayoutStyle style;
    private TableLayoutResizeType type;

    /// <summary>
    ///  This constructor caches our necessary state and determine what 'type' it is.
    /// </summary>
    internal TableLayoutPanelResizeGlyph(Rectangle controlBounds, TableLayoutStyle style, Cursor hitTestCursor, Behavior behavior) : base(behavior)
    {
        bounds = controlBounds;
        this.hitTestCursor = hitTestCursor;
        this.style = style;

        if (style is ColumnStyle)
        {
            type = TableLayoutResizeType.Column;
        }
        else
        {
            type = TableLayoutResizeType.Row;
        }
    }

    /// <summary>
    ///  Represents the bounds of the row or column line being rendered by the TableLayoutPanelDesigner.
    /// </summary>
    public override Rectangle Bounds => bounds;

    /// <summary>
    ///  Represents the Style associated with this glyph: Row or Column.
    ///  This is used by the behaviors resize methods to set the values.
    /// </summary>
    public TableLayoutStyle Style => style;

    /// <summary>
    ///  Used as quick check by our behavior when dragging/resizing.
    /// </summary>
    public TableLayoutResizeType Type => type;

    /// <summary>
    ///  Simply returns the proper cursor if the mouse pointer is within our cached bounds.
    /// </summary>
    public override Cursor GetHitTest(Point p)
    {
        if (bounds.Contains(p))
        {
            return hitTestCursor;
        }

        return null;
    }

    /// <summary>
    ///  No painting necessary - this glyph is more of a 'hot spot'
    /// </summary>
    public override void Paint(PaintEventArgs pe)
    {
    }

    /// <summary>
    ///  Internal Enum defining the two different types of glyphs a TableLayoutPanel can have: column or row.
    /// </summary>
    public enum TableLayoutResizeType
    {
        Column,
        Row
    }
}

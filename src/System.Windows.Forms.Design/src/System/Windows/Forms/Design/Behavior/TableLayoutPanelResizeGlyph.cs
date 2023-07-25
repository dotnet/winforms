// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

internal class TableLayoutPanelResizeGlyph : Glyph
{
    private Rectangle bounds;
    private Cursor hitTestCursor;
    private TableLayoutStyle style;
    private TableLayoutResizeType type;

    internal TableLayoutPanelResizeGlyph(Rectangle controlBounds, TableLayoutStyle style, Cursor hitTestCursor, Behavior behavior) : base(behavior)
    {
        this.bounds = controlBounds;
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

    public override Rectangle Bounds
    {
        get
        {
            return bounds;
        }
    }

    public TableLayoutStyle Style
    {
        get
        {
            return style;
        }
    }

    public TableLayoutResizeType Type
    {
        get
        {
            return type;
        }
    }

    public override Cursor GetHitTest(Point p)
    {
        if (bounds.Contains(p))
        {
            return hitTestCursor;
        }

        return null;
    }

    public override void Paint(PaintEventArgs pe)
    {
    }

    public enum TableLayoutResizeType
    {
        Column,
        Row
    }
}

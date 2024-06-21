// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public class ToolStripRenderEventArgs : EventArgs
{
    private Color _backColor;

    /// <summary>
    ///  This class represents all the information to render the toolStrip
    /// </summary>
    public ToolStripRenderEventArgs(Graphics g, ToolStrip toolStrip)
        : this(g, toolStrip, new Rectangle(Point.Empty, toolStrip.OrThrowIfNull().Size), Color.Empty)
    {
    }

    /// <summary>
    ///  This class represents all the information to render the toolStrip
    /// </summary>
    public ToolStripRenderEventArgs(
        Graphics g,
        ToolStrip toolStrip,
        Rectangle affectedBounds,
        Color backColor)
    {
        Graphics = g.OrThrowIfNull();
        ToolStrip = toolStrip.OrThrowIfNull();
        AffectedBounds = affectedBounds;
        _backColor = backColor;
    }

    /// <summary>
    ///  The graphics object to draw with
    /// </summary>
    public Graphics Graphics { get; }

    /// <summary>
    ///  The bounds to draw in
    /// </summary>
    public Rectangle AffectedBounds { get; }

    /// <summary>
    ///  Represents which toolStrip was affected by the click
    /// </summary>
    public ToolStrip ToolStrip { get; }

    /// <summary>
    ///  The back color to draw with.
    /// </summary>
    public Color BackColor
    {
        get
        {
            if (_backColor != Color.Empty)
            {
                return _backColor;
            }

            // get the user specified color
            if (ToolStrip is null)
            {
                _backColor = Application.SystemColors.Control;
                return _backColor;
            }

            _backColor = ToolStrip.RawBackColor;
            if (_backColor != Color.Empty)
            {
                return _backColor;
            }

            if (ToolStrip is ToolStripDropDown)
            {
                _backColor = Application.SystemColors.Menu;
            }
            else if (ToolStrip is MenuStrip)
            {
                _backColor = Application.SystemColors.MenuBar;
            }
            else
            {
                _backColor = Application.SystemColors.Control;
            }

            return _backColor;
        }
    }

    public Rectangle ConnectedArea
    {
        get
        {
            if (ToolStrip is ToolStripDropDown dropDown)
            {
                ToolStripDropDownItem? ownerItem = dropDown.OwnerItem as ToolStripDropDownItem;

                if (ownerItem is MdiControlStrip.SystemMenuItem)
                {
                    // there's no connected rect between a system menu item and a dropdown.
                    return Rectangle.Empty;
                }

                if (ownerItem is not null && ownerItem.ParentInternal is not null && !ownerItem.IsOnDropDown)
                {
                    // translate the item into our coordinate system.
                    Rectangle itemBounds = new(ToolStrip.PointToClient(ownerItem.TranslatePoint(Point.Empty, ToolStripPointType.ToolStripItemCoords, ToolStripPointType.ScreenCoords)), ownerItem.Size);

                    Rectangle overlap = ToolStrip.ClientRectangle;
                    overlap.Inflate(1, 1);
                    if (overlap.IntersectsWith(itemBounds))
                    {
                        switch (ownerItem.DropDownDirection)
                        {
                            case ToolStripDropDownDirection.AboveLeft:
                            case ToolStripDropDownDirection.AboveRight:
                                return Rectangle.Empty;
                            case ToolStripDropDownDirection.BelowRight:
                            case ToolStripDropDownDirection.BelowLeft:
                                overlap.Intersect(itemBounds);
                                if (overlap.Height == 2)
                                {
                                    return new Rectangle(itemBounds.X + 1, 0, itemBounds.Width - 2, 2);
                                }

                                // If its overlapping more than one pixel, this means we've pushed it to obscure
                                // the menu item. In this case pretend it's not connected.
                                return Rectangle.Empty;
                            case ToolStripDropDownDirection.Right:
                            case ToolStripDropDownDirection.Left:
                                return Rectangle.Empty;
                        }
                    }
                }
            }

            return Rectangle.Empty;
        }
    }
}

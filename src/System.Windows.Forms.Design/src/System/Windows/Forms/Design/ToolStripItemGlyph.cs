// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design;

/// <summary>
///  The glyph we put over the items. Basically this sets the hit-testable area of the item itself.
/// </summary>
internal class ToolStripItemGlyph : ControlBodyGlyph
{
    public ToolStripItemGlyph(ToolStripItem item, ToolStripItemDesigner itemDesigner, Rectangle bounds, Behavior.Behavior? b) : base(bounds, Cursors.Default, item, b)
    {
        Item = item;
        ItemDesigner = itemDesigner;
    }

    public ToolStripItem Item { get; }

    public ToolStripItemDesigner ItemDesigner { get; }

    /// <summary>
    ///  Abstract method that forces Glyph implementations to provide hit test logic. Given any point
    ///  - if the Glyph has decided to be involved with that location, the Glyph will need to return a valid Cursor.
    ///  Otherwise, returning null will cause the the BehaviorService to simply ignore it.
    /// </summary>
    public override Cursor? GetHitTest(Point p)
    {
        if (Item.Visible && Bounds.Contains(p))
        {
            return Cursors.Default;
        }

        return null;
    }

    /// <summary>
    ///  Control host don't draw on Invalidation...
    /// </summary>
    public override void Paint(PaintEventArgs pe)
    {
        if (Item is ToolStripControlHost && Item.IsOnDropDown)
        {
            if (Item is ToolStripComboBox && VisualStyles.VisualStyleRenderer.IsSupported)
            {
                // When processing WM_PAINT and the OS has a theme enabled, the native ComboBox
                // sends a WM_PAINT message to its parent when a theme is enabled in the OS forcing
                // a repaint in the AdornerWindow generating an infinite WM_PAINT message processing loop.
                // We guard against this here.
                return;
            }

            Item.Invalidate();
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.Design.Behavior;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  The glyph we put over the items. Basically this sets the hit-testable area of the item itself.
    /// </summary>
    internal class ToolStripItemGlyph : ControlBodyGlyph
    {
        private readonly ToolStripItem _item;
        private Rectangle _bounds;
        private readonly ToolStripItemDesigner _itemDesigner;

        public ToolStripItemGlyph(ToolStripItem item, ToolStripItemDesigner itemDesigner, Rectangle bounds, System.Windows.Forms.Design.Behavior.Behavior b) : base(bounds, Cursors.Default, item, b)
        {
            _item = item;
            _bounds = bounds;
            _itemDesigner = itemDesigner;
        }

        public ToolStripItem Item
        {
            get => _item;
        }

        public override Rectangle Bounds
        {
            get => _bounds;
        }

        public ToolStripItemDesigner ItemDesigner
        {
            get => _itemDesigner;
        }

        /// <summary>
        ///  Abstract method that forces Glyph implementations to provide hit test logic. Given any point - if the Glyph has decided to  be involved with that location, the Glyph will need to return  a valid Cursor. Otherwise, returning null will cause the  the BehaviorService to simply ignore it.
        /// </summary>
        public override Cursor GetHitTest(Point p)
        {
            if (_item.Visible && _bounds.Contains(p))
            {
                return Cursors.Default;
            }
            return null;
        }

        /// <summary>
        ///  Control host dont draw on Invalidation...
        /// </summary>
        public override void Paint(PaintEventArgs pe)
        {
            if (_item is ToolStripControlHost && _item.IsOnDropDown)
            {
                if (_item is System.Windows.Forms.ToolStripComboBox && VisualStyles.VisualStyleRenderer.IsSupported)
                {
                    // When processing WM_PAINT and the OS has a theme enabled, the native ComboBox sends a WM_PAINT  message to its parent when a theme is enabled in the OS forcing a repaint in the AdornerWindow  generating an infinite WM_PAINT message processing loop. We guard against this here.
                    return;
                }
                _item.Invalidate();
            }
        }
    }
}

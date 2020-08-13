// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  The NoResizeHandleGlyph represents the handle for a non-resizeable control in our new seleciton model.  Note that the pen and brush are created once per instance of this class and re-used in our painting logic for perf. reasonse.
    /// </summary>
    internal class NoResizeHandleGlyph : SelectionGlyphBase
    {
        private readonly bool _isPrimary;

        /// <summary>
        ///  NoResizeHandleGlyph's constructor takes additional parameters: 'type' and 'primary selection'.
        ///  Also, we create/cache our pen and brush here to avoid this action with every paint message.
        /// </summary>
        internal NoResizeHandleGlyph(Rectangle controlBounds, SelectionRules selRules, bool primarySelection, Behavior behavior) : base(behavior)
        {
            _isPrimary = primarySelection;
            hitTestCursor = Cursors.Default;
            rules = SelectionRules.None;
            if ((selRules & SelectionRules.Moveable) != 0)
            {
                rules = SelectionRules.Moveable;
                hitTestCursor = Cursors.SizeAll;
            }
            // The handle is always upperleft
            bounds = new Rectangle(controlBounds.X - DesignerUtils.NORESIZEHANDLESIZE, controlBounds.Y - DesignerUtils.NORESIZEHANDLESIZE, DesignerUtils.NORESIZEHANDLESIZE, DesignerUtils.NORESIZEHANDLESIZE);
            hitBounds = bounds;
        }

        /// <summary>
        ///  Very simple paint logic.
        /// </summary>
        public override void Paint(PaintEventArgs pe)
        {
            DesignerUtils.DrawNoResizeHandle(pe.Graphics, bounds, _isPrimary, this);
        }
    }
}

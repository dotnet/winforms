// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  The LockedHandleGlyph represents the handle for a non-resizeable control in our new seleciton model.  Note that the pen and brush are created once per instance of this class and re-used in our painting logic for perf. reasonse.
    /// </summary>
    internal class LockedHandleGlyph : SelectionGlyphBase
    {
        private readonly bool _isPrimary = false;

        /// <summary>
        ///  LockedHandleGlyph's constructor takes additional parameters: 'type' and 'primary selection'. Also, we create/cache our pen & brush here to avoid this action with every paint message.
        /// </summary>
        internal LockedHandleGlyph(Rectangle controlBounds, bool primarySelection) : base(null)
        {
            _isPrimary = primarySelection;
            hitTestCursor = Cursors.Default;
            rules = SelectionRules.None;
            bounds = new Rectangle((controlBounds.X + DesignerUtils.LOCKHANDLEOVERLAP) - DesignerUtils.LOCKHANDLEWIDTH,
                                    (controlBounds.Y + DesignerUtils.LOCKHANDLEOVERLAP) - DesignerUtils.LOCKHANDLEHEIGHT,
                                    DesignerUtils.LOCKHANDLEWIDTH, DesignerUtils.LOCKHANDLEHEIGHT);
            hitBounds = bounds;
        }

        /// <summary>
        ///  Very simple paint logic.
        /// </summary>
        public override void Paint(PaintEventArgs pe)
        {
            DesignerUtils.DrawLockedHandle(pe.Graphics, bounds, _isPrimary, this);
        }

    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  The GrabHandleGlyph represents the 8 handles of our new seleciton model.  Note that the pen and brush are created once per instance of this class and re-used in our painting logic for perf. reasonse.
    /// </summary>
    internal class GrabHandleGlyph : SelectionGlyphBase
    {
        private readonly bool _isPrimary;

        /// <summary>
        ///  GrabHandleGlyph's constructor takes additional parameters: 'type' and 'primary selection'.
        ///  Also, we create/cache our pen and brush here to avoid this action with every paint message.
        /// </summary>
        internal GrabHandleGlyph(Rectangle controlBounds, GrabHandleGlyphType type, Behavior behavior, bool primarySelection) : base(behavior)
        {
            _isPrimary = primarySelection;
            hitTestCursor = Cursors.Default;
            rules = SelectionRules.None;

            // We +/- DesignerUtils.HANDLEOVERLAP because we want each GrabHandle to overlap the control by DesignerUtils.HANDLEOVERLAP pixels
            switch (type)
            {
                case GrabHandleGlyphType.UpperLeft:
                    bounds = new Rectangle((controlBounds.X + DesignerUtils.HANDLEOVERLAP) - DesignerUtils.HANDLESIZE, (controlBounds.Y + DesignerUtils.HANDLEOVERLAP) - DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE);
                    hitTestCursor = Cursors.SizeNWSE;
                    rules = SelectionRules.TopSizeable | SelectionRules.LeftSizeable;
                    break;
                case GrabHandleGlyphType.UpperRight:
                    bounds = new Rectangle(controlBounds.Right - DesignerUtils.HANDLEOVERLAP, (controlBounds.Y + DesignerUtils.HANDLEOVERLAP) - DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE);
                    hitTestCursor = Cursors.SizeNESW;
                    rules = SelectionRules.TopSizeable | SelectionRules.RightSizeable;
                    break;
                case GrabHandleGlyphType.LowerRight:
                    bounds = new Rectangle(controlBounds.Right - DesignerUtils.HANDLEOVERLAP, controlBounds.Bottom - DesignerUtils.HANDLEOVERLAP, DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE);
                    hitTestCursor = Cursors.SizeNWSE;
                    rules = SelectionRules.BottomSizeable | SelectionRules.RightSizeable;
                    break;
                case GrabHandleGlyphType.LowerLeft:
                    bounds = new Rectangle((controlBounds.X + DesignerUtils.HANDLEOVERLAP) - DesignerUtils.HANDLESIZE, controlBounds.Bottom - DesignerUtils.HANDLEOVERLAP, DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE);
                    hitTestCursor = Cursors.SizeNESW;
                    rules = SelectionRules.BottomSizeable | SelectionRules.LeftSizeable;
                    break;
                case GrabHandleGlyphType.MiddleTop:
                    // Only add this one if there's room enough. Room is enough is as follows: 2*HANDLEOVERLAP for UpperLeft and UpperRight handles, 1 HANDLESIZE for the MiddleTop handle, 1 HANDLESIZE for padding
                    if (controlBounds.Width >= (2 * DesignerUtils.HANDLEOVERLAP) + (2 * DesignerUtils.HANDLESIZE))
                    {
                        bounds = new Rectangle(controlBounds.X + (controlBounds.Width / 2) - (DesignerUtils.HANDLESIZE / 2), (controlBounds.Y + DesignerUtils.HANDLEOVERLAP) - DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE);
                        hitTestCursor = Cursors.SizeNS;
                        rules = SelectionRules.TopSizeable;
                    }
                    break;
                case GrabHandleGlyphType.MiddleBottom:
                    // Only add this one if there's room enough. Room is enough is as follows: 2*HANDLEOVERLAP for LowerLeft and LowerRight handles, 1 HANDLESIZE for the MiddleBottom handle, 1 HANDLESIZE for padding
                    if (controlBounds.Width >= (2 * DesignerUtils.HANDLEOVERLAP) + (2 * DesignerUtils.HANDLESIZE))
                    {
                        bounds = new Rectangle(controlBounds.X + (controlBounds.Width / 2) - (DesignerUtils.HANDLESIZE / 2), controlBounds.Bottom - DesignerUtils.HANDLEOVERLAP, DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE);
                        hitTestCursor = Cursors.SizeNS;
                        rules = SelectionRules.BottomSizeable;
                    }
                    break;
                case GrabHandleGlyphType.MiddleLeft:
                    // Only add this one if there's room enough. Room is enough is as follows: 2*HANDLEOVERLAP for UpperLeft and LowerLeft handles, 1 HANDLESIZE for the MiddleLeft handle, 1 HANDLESIZE for padding
                    if (controlBounds.Height >= (2 * DesignerUtils.HANDLEOVERLAP) + (2 * DesignerUtils.HANDLESIZE))
                    {
                        bounds = new Rectangle((controlBounds.X + DesignerUtils.HANDLEOVERLAP) - DesignerUtils.HANDLESIZE, controlBounds.Y + (controlBounds.Height / 2) - (DesignerUtils.HANDLESIZE / 2), DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE);
                        hitTestCursor = Cursors.SizeWE;
                        rules = SelectionRules.LeftSizeable;
                    }
                    break;
                case GrabHandleGlyphType.MiddleRight:
                    // Only add this one if there's room enough. Room is enough is as follows: 2*HANDLEOVERLAP for UpperRight and LowerRight handles, 1 HANDLESIZE for the MiddleRight handle, 1 HANDLESIZE for padding
                    if (controlBounds.Height >= (2 * DesignerUtils.HANDLEOVERLAP) + (2 * DesignerUtils.HANDLESIZE))
                    {
                        bounds = new Rectangle(controlBounds.Right - DesignerUtils.HANDLEOVERLAP, controlBounds.Y + (controlBounds.Height / 2) - (DesignerUtils.HANDLESIZE / 2), DesignerUtils.HANDLESIZE, DesignerUtils.HANDLESIZE);
                        hitTestCursor = Cursors.SizeWE;
                        rules = SelectionRules.RightSizeable;
                    }
                    break;
                default:
                    Debug.Assert(false, "GrabHandleGlyph was called with a bad GrapHandleGlyphType.");
                    break;
            }

            hitBounds = bounds;
        }

        /// <summary>
        ///  Very simple paint logic.
        /// </summary>
        public override void Paint(PaintEventArgs pe)
        {
            DesignerUtils.DrawGrabHandle(pe.Graphics, bounds, _isPrimary, this);
        }
    }
}

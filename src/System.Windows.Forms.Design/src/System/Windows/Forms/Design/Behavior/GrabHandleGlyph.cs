// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

/// <summary>
///  The GrabHandleGlyph represents the 8 handles of our new selection model.
///  Note that the pen and brush are created once per instance of this class and re-used in our painting logic
///  for perf reasons.
/// </summary>
[DebuggerDisplay("{GetType().Name, nq}:: Behavior={Behavior.GetType().Name, nq}, {rules}, {hitTestCursor}")]
internal class GrabHandleGlyph : SelectionGlyphBase
{
    private readonly bool _isPrimary;

    /// <summary>
    ///  GrabHandleGlyph's constructor takes additional parameters: 'type' and 'primary selection'.
    ///  Also, we create/cache our pen and brush here to avoid this action with every paint message.
    /// </summary>
    internal GrabHandleGlyph(Rectangle controlBounds, GrabHandleGlyphType type, Behavior? behavior, bool primarySelection)
        : base(behavior)
    {
        _isPrimary = primarySelection;
        hitTestCursor = Cursors.Default;
        rules = SelectionRules.None;

        // We +/- DesignerUtils.HANDLEOVERLAP because we want each GrabHandle to overlap the control by DesignerUtils.HANDLEOVERLAP pixels
        switch (type)
        {
            case GrabHandleGlyphType.UpperLeft:
                bounds = new Rectangle(controlBounds.X + DesignerUtils.s_handleOverlap - DesignerUtils.s_handleSize, controlBounds.Y + DesignerUtils.s_handleOverlap - DesignerUtils.s_handleSize, DesignerUtils.s_handleSize, DesignerUtils.s_handleSize);
                hitTestCursor = Cursors.SizeNWSE;
                rules = SelectionRules.TopSizeable | SelectionRules.LeftSizeable;
                break;
            case GrabHandleGlyphType.UpperRight:
                bounds = new Rectangle(controlBounds.Right - DesignerUtils.s_handleOverlap, controlBounds.Y + DesignerUtils.s_handleOverlap - DesignerUtils.s_handleSize, DesignerUtils.s_handleSize, DesignerUtils.s_handleSize);
                hitTestCursor = Cursors.SizeNESW;
                rules = SelectionRules.TopSizeable | SelectionRules.RightSizeable;
                break;
            case GrabHandleGlyphType.LowerRight:
                bounds = new Rectangle(controlBounds.Right - DesignerUtils.s_handleOverlap, controlBounds.Bottom - DesignerUtils.s_handleOverlap, DesignerUtils.s_handleSize, DesignerUtils.s_handleSize);
                hitTestCursor = Cursors.SizeNWSE;
                rules = SelectionRules.BottomSizeable | SelectionRules.RightSizeable;
                break;
            case GrabHandleGlyphType.LowerLeft:
                bounds = new Rectangle(controlBounds.X + DesignerUtils.s_handleOverlap - DesignerUtils.s_handleSize, controlBounds.Bottom - DesignerUtils.s_handleOverlap, DesignerUtils.s_handleSize, DesignerUtils.s_handleSize);
                hitTestCursor = Cursors.SizeNESW;
                rules = SelectionRules.BottomSizeable | SelectionRules.LeftSizeable;
                break;
            case GrabHandleGlyphType.MiddleTop:
                // Only add this one if there's room enough. Room is enough is as follows:
                //     2*HANDLEOVERLAP for UpperLeft and UpperRight handles,
                //     1 HANDLESIZE for the MiddleTop handle, 1 HANDLESIZE for padding
                if (controlBounds.Width >= (2 * DesignerUtils.s_handleOverlap) + (2 * DesignerUtils.s_handleSize))
                {
                    bounds = new Rectangle(controlBounds.X + (controlBounds.Width / 2) - (DesignerUtils.s_handleSize / 2), controlBounds.Y + DesignerUtils.s_handleOverlap - DesignerUtils.s_handleSize, DesignerUtils.s_handleSize, DesignerUtils.s_handleSize);
                    hitTestCursor = Cursors.SizeNS;
                    rules = SelectionRules.TopSizeable;
                }

                break;
            case GrabHandleGlyphType.MiddleBottom:
                // Only add this one if there's room enough. Room is enough is as follows:
                //     2*HANDLEOVERLAP for LowerLeft and LowerRight handles,
                //     1 HANDLESIZE for the MiddleBottom handle, 1 HANDLESIZE for padding
                if (controlBounds.Width >= (2 * DesignerUtils.s_handleOverlap) + (2 * DesignerUtils.s_handleSize))
                {
                    bounds = new Rectangle(controlBounds.X + (controlBounds.Width / 2) - (DesignerUtils.s_handleSize / 2), controlBounds.Bottom - DesignerUtils.s_handleOverlap, DesignerUtils.s_handleSize, DesignerUtils.s_handleSize);
                    hitTestCursor = Cursors.SizeNS;
                    rules = SelectionRules.BottomSizeable;
                }

                break;
            case GrabHandleGlyphType.MiddleLeft:
                // Only add this one if there's room enough. Room is enough is as follows:
                //     2*HANDLEOVERLAP for UpperLeft and LowerLeft handles,
                //     1 HANDLESIZE for the MiddleLeft handle, 1 HANDLESIZE for padding
                if (controlBounds.Height >= (2 * DesignerUtils.s_handleOverlap) + (2 * DesignerUtils.s_handleSize))
                {
                    bounds = new Rectangle(controlBounds.X + DesignerUtils.s_handleOverlap - DesignerUtils.s_handleSize, controlBounds.Y + (controlBounds.Height / 2) - (DesignerUtils.s_handleSize / 2), DesignerUtils.s_handleSize, DesignerUtils.s_handleSize);
                    hitTestCursor = Cursors.SizeWE;
                    rules = SelectionRules.LeftSizeable;
                }

                break;
            case GrabHandleGlyphType.MiddleRight:
                // Only add this one if there's room enough. Room is enough is as follows:
                //     2*HANDLEOVERLAP for UpperRight and LowerRight handles,
                //     1 HANDLESIZE for the MiddleRight handle, 1 HANDLESIZE for padding
                if (controlBounds.Height >= (2 * DesignerUtils.s_handleOverlap) + (2 * DesignerUtils.s_handleSize))
                {
                    bounds = new Rectangle(controlBounds.Right - DesignerUtils.s_handleOverlap, controlBounds.Y + (controlBounds.Height / 2) - (DesignerUtils.s_handleSize / 2), DesignerUtils.s_handleSize, DesignerUtils.s_handleSize);
                    hitTestCursor = Cursors.SizeWE;
                    rules = SelectionRules.RightSizeable;
                }

                break;
            default:
                Debug.Assert(false, "GrabHandleGlyph was called with a bad GrabHandleGlyphType.");
                break;
        }

        hitBounds = bounds;
    }

    /// <summary>
    ///  Very simple paint logic.
    /// </summary>
    public override void Paint(PaintEventArgs pe)
    {
        DesignerUtils.DrawGrabHandle(pe.Graphics, bounds, _isPrimary);
    }
}

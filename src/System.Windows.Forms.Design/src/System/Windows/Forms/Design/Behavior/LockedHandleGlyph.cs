// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

/// <summary>
///  The LockedHandleGlyph represents the handle for a non-resizeable control in our new selection model.
///  Note that the pen and brush are created once per instance of this class and re-used
///  in our painting logic for perf. reasons.
/// </summary>
internal class LockedHandleGlyph : SelectionGlyphBase
{
    private readonly bool _isPrimary;

    /// <summary>
    ///  LockedHandleGlyph's constructor takes additional parameters: 'type' and 'primary selection'.
    ///  Also, we create/cache our pen and brush here to avoid this action with every paint message.
    /// </summary>
    internal LockedHandleGlyph(Rectangle controlBounds, bool primarySelection) : base(null)
    {
        _isPrimary = primarySelection;
        hitTestCursor = Cursors.Default;
        rules = SelectionRules.None;
        bounds = new Rectangle((controlBounds.X + DesignerUtils.s_lockHandleOverlap) - DesignerUtils.s_lockHandleWidth,
                                (controlBounds.Y + DesignerUtils.s_lockHandleOverlap) - DesignerUtils.s_lockHandleHeight,
                                DesignerUtils.s_lockHandleWidth, DesignerUtils.s_lockHandleHeight);
        hitBounds = bounds;
    }

    /// <summary>
    ///  Very simple paint logic.
    /// </summary>
    public override void Paint(PaintEventArgs pe)
    {
        DesignerUtils.DrawLockedHandle(pe.Graphics, bounds, _isPrimary);
    }
}

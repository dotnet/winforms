// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

/// <summary>
///  The SelectionBorderGlyph draws one side (depending on type) of a SelectionBorder.
/// </summary>
internal class SelectionBorderGlyph : SelectionGlyphBase
{
    /// <summary>
    ///  This constructor extends from the standard SelectionGlyphBase constructor.
    /// </summary>
    internal SelectionBorderGlyph(Rectangle controlBounds, SelectionRules rules, SelectionBorderGlyphType type, Behavior? behavior)
        : base(behavior)
    {
        InitializeGlyph(controlBounds, rules, type);
    }

    /// <summary>
    ///  Helper function that initializes the Glyph based on bounds, type, and bordersize.
    /// </summary>
    private void InitializeGlyph(Rectangle controlBounds, SelectionRules selRules, SelectionBorderGlyphType type)
    {
        rules = SelectionRules.None;
        hitTestCursor = Cursors.Default;

        // this will return the rect representing the bounds of the glyph
        bounds = DesignerUtils.GetBoundsForSelectionType(controlBounds, type);
        hitBounds = bounds;

        // The hitbounds for the border is actually a bit bigger than the glyph bounds
        switch (type)
        {
            case SelectionBorderGlyphType.Top:
                if ((selRules & SelectionRules.TopSizeable) != 0)
                {
                    hitTestCursor = Cursors.SizeNS;
                    rules = SelectionRules.TopSizeable;
                }

                // We want to apply the SELECTIONBORDERHITAREA to the top and the bottom of the selection border glyph
                hitBounds.Y -= (DesignerUtils.s_selectionBorderHitArea - DesignerUtils.s_selectionBorderSize) / 2;
                hitBounds.Height += DesignerUtils.s_selectionBorderHitArea - DesignerUtils.s_selectionBorderSize;
                break;
            case SelectionBorderGlyphType.Bottom:
                if ((selRules & SelectionRules.BottomSizeable) != 0)
                {
                    hitTestCursor = Cursors.SizeNS;
                    rules = SelectionRules.BottomSizeable;
                }

                // We want to apply the SELECTIONBORDERHITAREA to the top and the bottom of the selection border glyph
                hitBounds.Y -= (DesignerUtils.s_selectionBorderHitArea - DesignerUtils.s_selectionBorderSize) / 2;
                hitBounds.Height += DesignerUtils.s_selectionBorderHitArea - DesignerUtils.s_selectionBorderSize;
                break;
            case SelectionBorderGlyphType.Left:
                if ((selRules & SelectionRules.LeftSizeable) != 0)
                {
                    hitTestCursor = Cursors.SizeWE;
                    rules = SelectionRules.LeftSizeable;
                }

                // We want to apply the SELECTIONBORDERHITAREA to the left and the right of the selection border glyph
                hitBounds.X -= (DesignerUtils.s_selectionBorderHitArea - DesignerUtils.s_selectionBorderSize) / 2;
                hitBounds.Width += DesignerUtils.s_selectionBorderHitArea - DesignerUtils.s_selectionBorderSize;
                break;
            case SelectionBorderGlyphType.Right:
                if ((selRules & SelectionRules.RightSizeable) != 0)
                {
                    hitTestCursor = Cursors.SizeWE;
                    rules = SelectionRules.RightSizeable;
                }

                // We want to apply the SELECTIONBORDERHITAREA to the left and the right of the selection border glyph
                hitBounds.X -= (DesignerUtils.s_selectionBorderHitArea - DesignerUtils.s_selectionBorderSize) / 2;
                hitBounds.Width += DesignerUtils.s_selectionBorderHitArea - DesignerUtils.s_selectionBorderSize;
                break;
        }
    }

    /// <summary>
    ///  Simple painting logic for selection Glyphs.
    /// </summary>
    public override void Paint(PaintEventArgs pe)
    {
        DesignerUtils.DrawSelectionBorder(pe.Graphics, bounds);
    }
}

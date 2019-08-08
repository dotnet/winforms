// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  The NoResizeSelectionBorderGlyph draws one side (depending on type) of a SelectionBorder.
    /// </summary>
    internal class NoResizeSelectionBorderGlyph : SelectionGlyphBase
    {
        /// <summary>
        ///  This constructor extends from the standard SelectionGlyphBase constructor.
        /// </summary>
        internal NoResizeSelectionBorderGlyph(Rectangle controlBounds, SelectionRules rules, SelectionBorderGlyphType type, Behavior behavior) : base(behavior)
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
            if ((selRules & SelectionRules.Moveable) != 0)
            {
                rules = SelectionRules.Moveable;
                hitTestCursor = Cursors.SizeAll;
            }

            //this will return the rect representing the bounds of the glyph
            bounds = DesignerUtils.GetBoundsForNoResizeSelectionType(controlBounds, type);
            hitBounds = bounds;

            // The hitbounds for the border is actually a bit bigger than the glyph bounds

            switch (type)
            {
                case SelectionBorderGlyphType.Top:
                    goto case SelectionBorderGlyphType.Bottom;
                case SelectionBorderGlyphType.Bottom:
                    // We want to apply the SELECTIONBORDERHITAREA to the top and the bottom of the selection border glyph
                    hitBounds.Y -= (DesignerUtils.SELECTIONBORDERHITAREA - DesignerUtils.SELECTIONBORDERSIZE) / 2;
                    hitBounds.Height += DesignerUtils.SELECTIONBORDERHITAREA - DesignerUtils.SELECTIONBORDERSIZE;
                    break;
                case SelectionBorderGlyphType.Left:
                    goto case SelectionBorderGlyphType.Right;
                case SelectionBorderGlyphType.Right:
                    // We want to apply the SELECTIONBORDERHITAREA to the left and the right of the selection border glyph
                    hitBounds.X -= (DesignerUtils.SELECTIONBORDERHITAREA - DesignerUtils.SELECTIONBORDERSIZE) / 2;
                    hitBounds.Width += DesignerUtils.SELECTIONBORDERHITAREA - DesignerUtils.SELECTIONBORDERSIZE;
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
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  This is the base class for all the selection Glyphs: GrabHandle, Hidden, Locked, Selection, and Tray Glyphs.  This class includes all like-operations for the Selection glyphs.
    /// </summary>
    internal abstract class SelectionGlyphBase : Glyph
    {
        protected Rectangle bounds; // defines the bounds of the selection glyph
        protected Rectangle hitBounds; // defines the bounds used for hittest - it could be different than the bounds of the glyph itself
        protected Cursor hitTestCursor; // the cursor returned if hit test is positive
        protected SelectionRules rules; // the selection rules - defining how the control can change

        /// <summary>
        ///  Standard constructor.
        /// </summary>
        internal SelectionGlyphBase(Behavior behavior) : base(behavior)
        {
        }

        /// <summary>
        ///  Read-only property describing the SelecitonRules for these Glyphs.
        /// </summary>
        public SelectionRules SelectionRules
        {
            get => rules;
        }

        /// <summary>
        ///  Simple hit test rule: if the point is contained within the bounds - then it is a positive hit test.
        /// </summary>
        public override Cursor GetHitTest(Point p)
        {
            if (hitBounds.Contains(p))
            {
                return hitTestCursor;
            }
            return null;
        }

        /// <summary>
        ///  Returns the HitTestCursor for this glyph.
        /// </summary>
        public Cursor HitTestCursor
        {
            get => hitTestCursor;
        }

        /// <summary>
        ///  The Bounds of this glyph.
        /// </summary>
        public override Rectangle Bounds
        {
            get => bounds;
        }

        /// <summary>
        ///  There's no paint logic on this base class.
        /// </summary>
        public override void Paint(PaintEventArgs pe)
        {
        }
    }
}

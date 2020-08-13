// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  This is the glyph used to drag container controls around the designer. This glyph (and associated behavior) is created by the ParentControlDesigner.
    /// </summary>
    internal sealed class ContainerSelectorGlyph : Glyph
    {
        private Rectangle _glyphBounds;
        private readonly ContainerSelectorBehavior _relatedBehavior;

        /// <summary>
        ///  ContainerSelectorGlyph constructor.
        /// </summary>
        internal ContainerSelectorGlyph(Rectangle containerBounds, int glyphSize, int glyphOffset, ContainerSelectorBehavior behavior) : base(behavior)
        {
            _relatedBehavior = (ContainerSelectorBehavior)behavior;
            _glyphBounds = new Rectangle(containerBounds.X + glyphOffset, containerBounds.Y - (int)(glyphSize * .5), glyphSize, glyphSize);
        }

        /// <summary>
        ///  The bounds of this Glyph.
        /// </summary>
        public override Rectangle Bounds
        {
            get => _glyphBounds;
        }

        public Behavior RelatedBehavior
        {
            get => _relatedBehavior;
        }

        /// <summary>
        ///  Simple hit test rule: if the point is contained within the bounds - then it is a positive hit test.
        /// </summary>
        public override Cursor GetHitTest(Point p)
        {
            if (_glyphBounds.Contains(p) || _relatedBehavior.OkToMove)
            {
                return Cursors.SizeAll;
            }
            return null;
        }

        private Bitmap _glyph;
        private Bitmap MoveGlyph
        {
            get
            {
                if (_glyph is null)
                {
                    _glyph = new Icon(typeof(ContainerSelectorGlyph), "MoverGlyph").ToBitmap();
                }
                return _glyph;
            }
        }

        /// <summary>
        ///  Very simple paint logic.
        /// </summary>
        public override void Paint(PaintEventArgs pe)
        {
            pe.Graphics.DrawImage(MoveGlyph, _glyphBounds);
        }
    }
}

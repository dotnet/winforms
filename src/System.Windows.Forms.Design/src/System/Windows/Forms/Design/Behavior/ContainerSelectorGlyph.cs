// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  This is the glyph used to drag container controls around the designer. This glyph (and associated behavior) is created by the ParentControlDesigner.
    /// </summary>
    [DebuggerDisplay("{GetType().Name}:: Component: {_component}, Cursor: {_hitTestCursor}")]
    internal sealed class ContainerSelectorGlyph : Glyph
    {
        private Rectangle _glyphBounds;
        private Bitmap? _glyph;
        private readonly ContainerSelectorBehavior? _relatedBehavior;

        /// <summary>
        ///  ContainerSelectorGlyph constructor.
        /// </summary>
        internal ContainerSelectorGlyph(Rectangle containerBounds, int glyphSize, int glyphOffset, ContainerSelectorBehavior? behavior)
            : base(behavior)
        {
            _relatedBehavior = behavior;
            _glyphBounds = new Rectangle(containerBounds.X + glyphOffset, containerBounds.Y - (int)(glyphSize * .5), glyphSize, glyphSize);
        }

        /// <summary>
        ///  The bounds of this Glyph.
        /// </summary>
        public override Rectangle Bounds
        {
            get => _glyphBounds;
        }

        public Behavior? RelatedBehavior
        {
            get => _relatedBehavior;
        }

        /// <summary>
        ///  Simple hit test rule: if the point is contained within the bounds - then it is a positive hit test.
        /// </summary>
        public override Cursor? GetHitTest(Point p)
        {
            if (_glyphBounds.Contains(p) || _relatedBehavior?.OkToMove == true)
            {
                return Cursors.SizeAll;
            }

            return null;
        }

        private Bitmap MoveGlyph
        {
            get
            {
                if (_glyph is null)
                {
                    _glyph = new Bitmap(typeof(ContainerSelectorGlyph), "MoverGlyph");
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

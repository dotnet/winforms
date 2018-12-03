// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///     The ComponentGlyph class simply contains a pointer back
    ///     to it's related Component.  This can be used to trace
    ///     Glyphs (during drag operations or otherwise) back to
    ///     their component.
    /// </summary>
    public class ComponentGlyph : Glyph
    {
        /// <summary>
        ///     Standard constructor.
        /// </summary>
        public ComponentGlyph(IComponent relatedComponent, Behavior behavior) : base(behavior)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public ComponentGlyph(IComponent relatedComponent) : base(null)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Returns the Component this Glyph is related to.
        /// </summary>
        public IComponent RelatedComponent => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Overrides GetHitTest - this implementation does nothing.
        /// </summary>
        public override Cursor GetHitTest(Point p)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Overrides Glyph::Paint - this implementation does nothing.
        /// </summary>
        public override void Paint(PaintEventArgs pe)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}

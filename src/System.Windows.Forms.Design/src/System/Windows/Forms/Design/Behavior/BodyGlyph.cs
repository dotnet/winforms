// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  This Glyph is placed on every control sized to the exact bounds of
    ///  the control.
    /// </summary>
    public class ControlBodyGlyph : ComponentGlyph
    {
        /// <summary>
        ///  Standard Constructor.
        /// </summary>
        public ControlBodyGlyph(Rectangle bounds, Cursor cursor, IComponent relatedComponent, ControlDesigner designer)
            : base(relatedComponent, new ControlDesigner.TransparentBehavior(designer))
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public ControlBodyGlyph(Rectangle bounds, Cursor cursor, IComponent relatedComponent, Behavior behavior) : base(
            relatedComponent, behavior)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  The bounds of this glyph.
        /// </summary>
        public override Rectangle Bounds => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///  Simple hit test rule: if the point is contained within the bounds
        ///  AND the component is Visible (controls on some tab pages may
        ///  not be, for ex) then it is a positive hit test.
        /// </summary>
        public override Cursor GetHitTest(Point p)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///     A Glyph represents a single UI entity managed by an Adorner.  A Glyph
    ///     does not have an HWnd - and is rendered on the BehaviorService's
    ///     AdornerWindow control.  Each Glyph can have a Behavior associated with
    ///     it - the idea here is that a successfully Hit-Tested Glyph has the
    ///     opportunity to 'push' a new/different Behavior onto the BehaviorService's
    ///     BehaviorStack.  Note that all Glyphs really do is paint and hit test.
    /// </summary>
    public abstract class Glyph
    {
        /// <summary>
        ///     Glyph's default constructor takes a Behavior.
        /// </summary>
        protected Glyph(Behavior behavior)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     This read-only property will return the Behavior associated with
        ///     this Glyph.  The Behavior can be null.
        /// </summary>
        public virtual Behavior Behavior => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     This read-only property will return the Bounds associated with
        ///     this Glyph.  The Bounds can be empty.
        /// </summary>
        public virtual Rectangle Bounds => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// <summary>
        ///     Abstract method that forces Glyph implementations to provide
        ///     hit test logic.  Given any point - if the Glyph has decided to
        ///     be involved with that location, the Glyph will need to return
        ///     a valid Cursor.  Otherwise, returning null will cause the
        ///     the BehaviorService to simply ignore it.
        /// </summary>
        public abstract Cursor GetHitTest(Point p);

        /// <summary>
        ///     Abstract method that forces Glyph implementations to provide
        ///     paint logic.  The PaintEventArgs object passed into this method
        ///     contains the Graphics object related to the BehaviorService's
        ///     AdornerWindow.
        /// </summary>
        public abstract void Paint(PaintEventArgs pe);

        /// <summary>
        ///     This method is called by inheriting classes to change the
        ///     Behavior object associated with the Glyph.
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
        protected void SetBehavior(Behavior behavior)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}

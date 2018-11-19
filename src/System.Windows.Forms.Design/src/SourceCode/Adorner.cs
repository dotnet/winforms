// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///     An Adorner manages a collection of UI-related Glyphs.  Each Adorner
    ///     can be enabled/disabled.  Only Enabled Adorners will receive hit test
    ///     and paint messages from the BehaviorService.  An Adorner can be viewed
    ///     as a proxy between UI-related elements (all Glyphs) and the BehaviorService.
    /// </summary>
    public sealed class Adorner
    {
        /// <summary>
        ///     Standard constructor.  Creates a new GlyphCollection and by default is enabled.
        /// </summary>
        public Adorner()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     When an Adorner is added to the BehaviorService's AdornerCollection, the collection
        ///     will set this property so that the Adorner can call back to the BehaviorService.
        /// </summary>
        public BehaviorService BehaviorService
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Determines if the BehaviorService will send HitTest and Paint messages to
        ///     the Adorner. This will invalidate behavior service when changed.
        /// </summary>
        public bool Enabled
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Returns the stronly-typed Glyph collection.
        /// </summary>
        public GlyphCollection Glyphs => throw new NotImplementedException(SR.NotImplementedByDesign);

        /// ///
        /// <summary>
        ///     Forces the BehaviorService to refresh its AdornerWindow.
        /// </summary>
        public void Invalidate()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Forces the BehaviorService to refresh its AdornerWindow within the given Rectangle.
        /// </summary>
        public void Invalidate(Rectangle rectangle)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Forces the BehaviorService to refresh its AdornerWindow within the given Region.
        /// </summary>
        public void Invalidate(Region region)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}

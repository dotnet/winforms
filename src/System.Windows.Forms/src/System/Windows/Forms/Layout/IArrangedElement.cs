// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Layout
{
    internal interface IArrangedElement : IComponent
    {
        /// <summary>
        ///  Bounding rectangle of the element.
        /// </summary>
        Rectangle Bounds { get; }

        /// <summary>
        ///  Sets the bounds for an element. Implementors should call
        ///  CommonProperties.SetSpecifiedBounds.
        ///  See Control.SetBoundsCore.
        /// </summary>
        void SetBounds(Rectangle bounds, BoundsSpecified specified);

        /// <summary>
        ///  Query element for its preferred size.  There is no guarantee
        ///  that layout engine will assign the element the returned size.
        ///  ProposedSize is a hint about constraints.
        /// </summary>
        Size GetPreferredSize(Size proposedSize);

        /// <summary>
        ///  DisplayRectangle is the client area of a container element.
        ///  Could possibly disappear if we change control to keep an
        ///  up-to-date copy of display rectangle in the property store.
        /// </summary>
        Rectangle DisplayRectangle { get; }

        /// <summary>
        ///  True if the element is currently visible (some layouts, like
        ///  flow, need to skip non-visible elements.)
        /// </summary>
        bool ParticipatesInLayout { get; }

        /// <summary>
        ///  Internally, layout engines will get properties from the
        ///  property store on this interface.  In Orcas, this will be
        ///  replaced with a global PropertyManager for DPs.
        /// </summary>
        PropertyStore Properties { get; }

        /// <summary>
        ///  When an extender provided property is changed, we call this
        ///  method to update the layout on the element. In Orcas, we
        ///  will sync the DPs changed event.
        /// </summary>
        void PerformLayout(IArrangedElement affectedElement, string propertyName);

        /// <summary>
        ///  Returns the element's parent container (on a control, this forwands to Parent)
        /// </summary>
        IArrangedElement Container { get; }

        /// <summary>
        ///  Returns the element's childern (on a control, this forwands to Controls)
        /// </summary>
        ArrangedElementCollection Children { get; }
    }
}

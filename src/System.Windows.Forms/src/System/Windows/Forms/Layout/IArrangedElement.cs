// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Layout {

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;

    // 

    internal interface IArrangedElement : IComponent {
        // Bounding rectangle of the element.
        Rectangle Bounds {get; }

        // Sets the bounds for an element.  Implementors should call CommonProperties.SetSpecifiedBounds.
        // See Control.SetBoundsCore.
        void SetBounds(Rectangle bounds, BoundsSpecified specified);

        // Query element for its preferred size.  There is no guarantee
        // that layout engine will assign the element the returned size.
        // ProposedSize is a hint about constraints.
        Size GetPreferredSize(Size proposedSize);

        // DisplayRectangle is the client area of a container element.
        // Could possibly disappear if we change control to keep an
        // up-to-date copy of display rectangle in the property store.
        Rectangle DisplayRectangle { get; }

        // True if the element is currently visible (some layouts, like
        // flow, need to skip non-visible elements.)
        bool ParticipatesInLayout { get; }

        // Internally, layout engines will get properties from the
        // property store on this interface.  In Orcas, this will be
        // replaced with a global PropertyManager for DPs.
        PropertyStore Properties { get; }

        // When an extender provided property is changed, we call this
        // method to update the layout on the element. In Orcas, we
        // will sync the DPs changed event.
        void PerformLayout(IArrangedElement affectedElement, string propertyName);

        // Returns the element's parent container (on a control, this forwands to Parent)
        IArrangedElement Container { get; }

        // Returns the element's childern (on a control, this forwands to Controls)
        ArrangedElementCollection Children { get; }
    }
}

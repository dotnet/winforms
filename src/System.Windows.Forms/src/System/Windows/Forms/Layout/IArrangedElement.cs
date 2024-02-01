// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using static System.Windows.Forms.Layout.DefaultLayout;
using static System.Windows.Forms.Layout.TableLayout;

namespace System.Windows.Forms.Layout;

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
    ///  When an extender provided property is changed, we call this
    ///  method to update the layout on the element. In Orcas, we
    ///  will sync the DPs changed event.
    /// </summary>
    void PerformLayout(IArrangedElement affectedElement, string? propertyName);

    /// <summary>
    ///  Returns the element's parent container (on a control, this forwards to Parent)
    /// </summary>
    IArrangedElement? Container { get; }

    /// <summary>
    ///  Returns the element's children (on a control, this forwards to Controls)
    /// </summary>
    ArrangedElementCollection Children { get; }

    /// <summary>
    ///  Returns the layout state bit vector.
    ///  CAREFUL: this is a copy of the state.
    ///  You need to pass the value back to the property to save your changes.
    /// </summary>
    BitVector32 LayoutState { get; set; }

    Rectangle SpecifiedBounds { get; set; }

    Size PreferredSize { get; set; }

    Padding? Padding { get; set; }

    Padding? Margin { get; set; }

    Size? MinimumSize { get; set; }

    Size? MaximumSize { get; set; }

    Size LayoutBounds { get; set; }

    AnchorInfo? AnchorInfo { get; set; }

    Dictionary<IArrangedElement, Rectangle> CachedBounds { get; }

    LayoutInfo LayoutInfo { get; set; }

    ContainerInfo ContainerInfo { get; }

#if DEBUG
    Dictionary<string, string?>? LastKnownState { get; set; }
#endif
}

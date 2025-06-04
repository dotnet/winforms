// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design;

/// <summary>
///  IOverlayService is a service that supports adding simple overlay windows to a design surface.
///  Overlay windows can be used to paint extra glyphs on top of existing controls.
///  Once an overlay is added, it will be forced on top of the Z-order for the other controls and overlays.
///  If you want the overlay to be transparent, then you must do this work yourself.
///  A typical way to make an overlay control transparent is to use the method setRegion
///  on the control class to define the non-transparent portion of the control.
/// </summary>
internal interface IOverlayService
{
    /// <summary>
    ///  Pushes the given control on top of the overlay list.
    ///  This is a "push" operation, meaning that it forces this control to the top of the existing overlay list.
    /// </summary>
    int PushOverlay(Control control);

    /// <summary>
    ///  Removes the given control from the overlay list.
    ///  Unlike pushOverlay, this can remove a control from the middle of the overlay list.
    /// </summary>
    void RemoveOverlay(Control control);

    /// <summary>
    ///  Inserts the given control from the overlay list. You need to pass the index of the overlay.
    /// </summary>
    void InsertOverlay(Control control, int index);

    /// <summary>
    ///  Invalidates the overlays
    /// </summary>
    void InvalidateOverlays(Rectangle screenRectangle);

    /// <summary>
    ///  Invalidates the overlays
    /// </summary>
    void InvalidateOverlays(Region screenRegion);
}

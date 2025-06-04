// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Design;

internal interface IMouseHandler
{
    /// <summary>
    ///  This is called when the user double clicks on a component.
    ///  The typical behavior is to create an event handler for the component's default event and
    ///  navigate to the handler.
    /// </summary>
    void OnMouseDoubleClick(IComponent component);

    /// <summary>
    ///  This is called when a mouse button is depressed.
    ///  This will perform the default drag action for the selected components,
    ///  which is to move those components around by the mouse.
    /// </summary>
    void OnMouseDown(IComponent component, MouseButtons button, int x, int y);

    /// <summary>
    ///  This is called when the mouse momentarily hovers over the view for the given component.
    /// </summary>
    void OnMouseHover(IComponent component);

    /// <summary>
    ///  This is called for each movement of the mouse.
    /// </summary>
    void OnMouseMove(IComponent component, int x, int y);
    /// <summary>
    ///  This is called when the user releases the mouse from a component.
    ///  This will update the UI to reflect the release of the mouse.
    /// </summary>
    void OnMouseUp(IComponent component, MouseButtons button);

    /// <summary>
    ///  This is called when the cursor for the given component should be updated.
    ///  The mouse is always over the given component's view when this is called.
    /// </summary>
    void OnSetCursor(IComponent component);
}

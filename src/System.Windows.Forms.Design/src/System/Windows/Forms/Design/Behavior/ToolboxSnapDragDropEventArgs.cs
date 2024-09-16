// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design.Behavior;

/// <summary>
///  This class is created by the ToolboxItemSnapLineBehavior when the user clicks, drags, and drops a control
///  from the toolbox. This class adds value to the standard DragEventArgs by holding information about how the user
///  snapped a control when it was dropped. We'll use this information in ParentControlDesigner when this new control
///  is created to properly position and size the new control.
/// </summary>
internal sealed partial class ToolboxSnapDragDropEventArgs : DragEventArgs
{
    /// <summary>
    ///  Constructor that is called when the user drops - here, we'll essentially
    ///  push the original drag event info down to the base class and store off
    ///  our direction and offset.
    /// </summary>
    public ToolboxSnapDragDropEventArgs(SnapDirection snapDirections, Point offset, DragEventArgs origArgs)
        : base(origArgs.Data, origArgs.KeyState, origArgs.X, origArgs.Y, origArgs.AllowedEffect, origArgs.Effect)
    {
        SnapDirections = snapDirections;
        Offset = offset;
    }

    /// <summary>
    ///  This is the last direction that the user was snapped to directly before
    ///  the drop happened...
    /// </summary>
    public SnapDirection SnapDirections { get; }

    /// <summary>
    ///  The offset in pixel between the mouse cursor (at time of drop) and the
    ///  'drag box' that is dancing around and snapping to other components.
    /// </summary>
    public Point Offset { get; }
}

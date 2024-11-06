// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design.Behavior;

internal sealed partial class ToolboxSnapDragDropEventArgs
{
    /// <summary>
    ///  Flag enum used to define the different directions a 'drag box' could be snapped to.
    /// </summary>
    [Flags]
    public enum SnapDirection
    {
        None = 0x00,
        Top = 0x01,
        Bottom = 0x02,
        Right = 0x04,
        Left = 0x08
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

/// <summary>
///  Specifies a set of selection rule identifiers that can be used to indicate attributes for a selected component.
/// </summary>
// We need to combine the SelectionRules.
[Flags]
public enum SelectionRules
{
    /// <summary>
    ///  Indicates no special selection attributes.
    /// </summary>
    None = 0x00000000,

    /// <summary>
    ///  Indicates the given component supports a location property that allows it to be moved on the screen,
    ///  and that the selection service is not currently locked.
    /// </summary>
    Moveable = 0x10000000,

    /// <summary>
    ///  Indicates the given component has some form of visible user interface and
    ///  the selection service is drawing a selection border around this user interface.
    ///  If a selected component has this rule set, you can assume
    ///  that the component implements <see cref="System.ComponentModel.IComponent"/> and
    ///  that it is associated with a corresponding design instance.
    /// </summary>
    Visible = 0x40000000,

    /// <summary>
    ///  Indicates the given component is locked to its container.
    ///  Overrides the moveable and sizeable properties of this enum.
    /// </summary>
    Locked = unchecked((int)0x80000000),

    /// <summary>
    ///  Indicates the given component supports resize from the top.
    ///  This bit will be ignored unless the Sizeable bit is also set.
    /// </summary>
    TopSizeable = 0x00000001,

    /// <summary>
    ///  Indicates the given component supports resize from the bottom.
    ///  This bit will be ignored unless the Sizeable bit is also set.
    /// </summary>
    BottomSizeable = 0x00000002,

    /// <summary>
    ///  Indicates the given component supports resize from the left.
    ///  This bit will be ignored unless the Sizeable bit is also set.
    /// </summary>
    LeftSizeable = 0x00000004,

    /// <summary>
    ///  Indicates the given component supports resize from the right.
    ///  This bit will be ignored unless the Sizeable bit is also set.
    /// </summary>
    RightSizeable = 0x00000008,

    /// <summary>
    ///  Indicates the given component supports sizing in all directions,
    ///  and the selection service is not currently locked.
    /// </summary>
    AllSizeable = TopSizeable | BottomSizeable | LeftSizeable | RightSizeable
}

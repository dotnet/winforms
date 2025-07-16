// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the state of a ListView group header.
/// </summary>
[Flags]
public enum ListViewGroupState
{
    /// <summary>
    ///  The group header is in its normal state.
    /// </summary>
    Normal = 0x00000000,

    /// <summary>
    ///  The group is collapsed.
    /// </summary>
    Collapsed = 0x00000001,

    /// <summary>
    ///  The group header is hot (mouse is over it).
    /// </summary>
    Hot = 0x00000002,

    /// <summary>
    ///  The group header is selected.
    /// </summary>
    Selected = 0x00000004,

    /// <summary>
    ///  The group header has focus.
    /// </summary>
    Focused = 0x00000008,

    /// <summary>
    ///  The group can be collapsed/expanded.
    /// </summary>
    Collapsible = 0x00000010,
}

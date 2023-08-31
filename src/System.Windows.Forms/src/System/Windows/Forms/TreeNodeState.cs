// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Gives state information about a TreeView node. Used with owner draw.
/// </summary>
[Flags]
public enum TreeNodeStates
{
    Checked = (int)NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_CHECKED,
    Default = (int)NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_DEFAULT,
    Focused = (int)NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_FOCUS,
    Grayed = (int)NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_GRAYED,
    Hot = (int)NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_HOT,
    Indeterminate = (int)NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_INDETERMINATE,
    Marked = (int)NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_MARKED,
    Selected = (int)NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_SELECTED,
    ShowKeyboardCues = (int)NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_SHOWKEYBOARDCUES
}

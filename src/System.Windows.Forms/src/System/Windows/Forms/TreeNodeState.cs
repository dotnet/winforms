// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Gives state information about a TreeView node. Used with owner draw.
    /// </summary>
    [Flags]
    public enum TreeNodeStates
    {
        Checked = (int)ComCtl32.CDIS.CHECKED,
        Default = (int)ComCtl32.CDIS.DEFAULT,
        Focused = (int)ComCtl32.CDIS.FOCUS,
        Grayed = (int)ComCtl32.CDIS.GRAYED,
        Hot = (int)ComCtl32.CDIS.HOT,
        Indeterminate = (int)ComCtl32.CDIS.INDETERMINATE,
        Marked = (int)ComCtl32.CDIS.MARKED,
        Selected = (int)ComCtl32.CDIS.SELECTED,
        ShowKeyboardCues = (int)ComCtl32.CDIS.SHOWKEYBOARDCUES
    }
}

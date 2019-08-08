// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Gives state information about a ListView item/sub-item. Used with owner draw.
    /// </summary>
    [Flags]
    public enum ListViewItemStates
    {
        Checked = NativeMethods.CDIS_CHECKED,
        Default = NativeMethods.CDIS_DEFAULT,
        Focused = NativeMethods.CDIS_FOCUS,
        Grayed = NativeMethods.CDIS_GRAYED,
        Hot = NativeMethods.CDIS_HOT,
        Indeterminate = NativeMethods.CDIS_INDETERMINATE,
        Marked = NativeMethods.CDIS_MARKED,
        Selected = NativeMethods.CDIS_SELECTED,
        ShowKeyboardCues = NativeMethods.CDIS_SHOWKEYBOARDCUES
    }
}

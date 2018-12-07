// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    /// <include file='doc\ListViewItemStates.uex' path='docs/doc[@for="ListViewItemStates"]/*' />
    /// <devdoc>
    ///    <para>
    ///
    ///	   Gives state information about a ListView item/sub-item. Used with owner draw. 
    ///
    ///    </para>
    /// </devdoc>

    [Flags]
    public enum ListViewItemStates {
        /// <include file='doc\ListViewItemStates.uex' path='docs/doc[@for="ListViewItemStates.Checked"]/*' />
        Checked = NativeMethods.CDIS_CHECKED,

        /// <include file='doc\ListViewItemStates.uex' path='docs/doc[@for="ListViewItemStates.Default"]/*' />
        Default = NativeMethods.CDIS_DEFAULT,

        /// <include file='doc\ListViewItemStates.uex' path='docs/doc[@for="ListViewItemStates.Focused"]/*' />
        Focused = NativeMethods.CDIS_FOCUS,

	    /// <include file='doc\ListViewItemStates.uex' path='docs/doc[@for="ListViewItemStates.Grayed"]/*' />
        Grayed = NativeMethods.CDIS_GRAYED,

	    /// <include file='doc\ListViewItemStates.uex' path='docs/doc[@for="ListViewItemStates.Hot"]/*' />
        Hot = NativeMethods.CDIS_HOT,

	    /// <include file='doc\ListViewItemStates.uex' path='docs/doc[@for="ListViewItemStates.Indeterminate"]/*' />
        Indeterminate = NativeMethods.CDIS_INDETERMINATE,

	    /// <include file='doc\ListViewItemStates.uex' path='docs/doc[@for="ListViewItemStates.Marked"]/*' />
        Marked = NativeMethods.CDIS_MARKED,

	    /// <include file='doc\ListViewItemStates.uex' path='docs/doc[@for="ListViewItemStates.Selected"]/*' />
        Selected = NativeMethods.CDIS_SELECTED,

	    /// <include file='doc\ListViewItemStates.uex' path='docs/doc[@for="ListViewItemStates.ShowKeyboardCues"]/*' />
        ShowKeyboardCues = NativeMethods.CDIS_SHOWKEYBOARDCUES
    }
}

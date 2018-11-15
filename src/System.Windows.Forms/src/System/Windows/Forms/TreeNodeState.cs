// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    /// <include file='doc\TreeNodeStates.uex' path='docs/doc[@for="TreeNodeStates"]/*' />
    /// <devdoc>
    ///    <para>
    ///
    ///	   Gives state information about a TreeView node. Used with owner draw. 
    ///
    ///    </para>
    /// </devdoc>

    [Flags]
    public enum TreeNodeStates {
        /// <include file='doc\TreeNodeStates.uex' path='docs/doc[@for="TreeNodeStates.Checked"]/*' />
        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        Checked = NativeMethods.CDIS_CHECKED,

        /// <include file='doc\TreeNodeStates.uex' path='docs/doc[@for="TreeNodeStates.Default"]/*' />
        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        Default = NativeMethods.CDIS_DEFAULT,

        /// <include file='doc\TreeNodeStates.uex' path='docs/doc[@for="TreeNodeStates.Focused"]/*' />
        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        Focused = NativeMethods.CDIS_FOCUS,

        /// <include file='doc\TreeNodeStates.uex' path='docs/doc[@for="TreeNodeStates.Grayed"]/*' />
        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        Grayed = NativeMethods.CDIS_GRAYED,

        /// <include file='doc\TreeNodeStates.uex' path='docs/doc[@for="TreeNodeStates.Hot"]/*' />
        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        Hot = NativeMethods.CDIS_HOT,

        /// <include file='doc\TreeNodeStates.uex' path='docs/doc[@for="TreeNodeStates.Indeterminate"]/*' />
        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        Indeterminate = NativeMethods.CDIS_INDETERMINATE,

        /// <include file='doc\TreeNodeStates.uex' path='docs/doc[@for="TreeNodeStates.Marked"]/*' />
        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        Marked = NativeMethods.CDIS_MARKED,

        /// <include file='doc\TreeNodeStates.uex' path='docs/doc[@for="TreeNodeStates.Selected"]/*' />
        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        Selected = NativeMethods.CDIS_SELECTED,

        /// <include file='doc\TreeNodeStates.uex' path='docs/doc[@for="TreeNodeStates.ShowKeyboardCues"]/*' />
        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        ShowKeyboardCues = NativeMethods.CDIS_SHOWKEYBOARDCUES

    }
}

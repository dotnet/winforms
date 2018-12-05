// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    /// <devdoc>
    ///    <para>
    ///
    ///	   Gives state information about a TreeView node. Used with owner draw. 
    ///
    ///    </para>
    /// </devdoc>

    [Flags]
    public enum TreeNodeStates {
        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        Checked = NativeMethods.CDIS_CHECKED,

        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        Default = NativeMethods.CDIS_DEFAULT,

        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        Focused = NativeMethods.CDIS_FOCUS,

        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        Grayed = NativeMethods.CDIS_GRAYED,

        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        Hot = NativeMethods.CDIS_HOT,

        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        Indeterminate = NativeMethods.CDIS_INDETERMINATE,

        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        Marked = NativeMethods.CDIS_MARKED,

        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        Selected = NativeMethods.CDIS_SELECTED,

        /// <devdoc>
        ///    <para>
        /// [To be supplied.] 
        ///    </para>
        /// </devdoc>
        ShowKeyboardCues = NativeMethods.CDIS_SHOWKEYBOARDCUES

    }
}

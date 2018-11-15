// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <include file='doc\DrawItemState.uex' path='docs/doc[@for="DrawItemState"]/*' />
    /// <devdoc>
    ///    <para>
    ///
    ///       Specifies
    ///       the state of an item that is being drawn.
    ///    </para>
    /// </devdoc>
    [Flags]
    public enum DrawItemState {
        /// <include file='doc\DrawItemState.uex' path='docs/doc[@for="DrawItemState.Checked"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The item is checked. Only menu controls use this value.
        ///
        ///    </para>
        /// </devdoc>
        Checked = NativeMethods.ODS_CHECKED,
        /// <include file='doc\DrawItemState.uex' path='docs/doc[@for="DrawItemState.ComboBoxEdit"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The item is the editing portion of a <see cref='System.Windows.Forms.ComboBox'/> .
        ///    </para>
        /// </devdoc>
        ComboBoxEdit = NativeMethods.ODS_COMBOBOXEDIT,
        /// <include file='doc\DrawItemState.uex' path='docs/doc[@for="DrawItemState.Default"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The item is the default item of the control.
        ///    </para>
        /// </devdoc>
        Default     = NativeMethods.ODS_DEFAULT,
        /// <include file='doc\DrawItemState.uex' path='docs/doc[@for="DrawItemState.Disabled"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The item is disabled.
        ///    </para>
        /// </devdoc>
        Disabled     = NativeMethods.ODS_DISABLED,
        /// <include file='doc\DrawItemState.uex' path='docs/doc[@for="DrawItemState.Focus"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The item has focus.
        ///    </para>
        /// </devdoc>
        Focus        = NativeMethods.ODS_FOCUS,
        /// <include file='doc\DrawItemState.uex' path='docs/doc[@for="DrawItemState.Grayed"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The item
        ///       is grayed. Only menu controls use this value.
        ///
        ///    </para>
        /// </devdoc>
        Grayed        = NativeMethods.ODS_GRAYED,
        /// <include file='doc\DrawItemState.uex' path='docs/doc[@for="DrawItemState.HotLight"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The item is being hot-tracked.
        ///    </para>
        /// </devdoc>
        HotLight        = NativeMethods.ODS_HOTLIGHT,
        /// <include file='doc\DrawItemState.uex' path='docs/doc[@for="DrawItemState.Inactive"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The item is inactive.
        ///    </para>
        /// </devdoc>
        Inactive        = NativeMethods.ODS_INACTIVE,
        /// <include file='doc\DrawItemState.uex' path='docs/doc[@for="DrawItemState.NoAccelerator"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The item displays without a keyboard accelarator.
        ///    </para>
        /// </devdoc>
        NoAccelerator        = NativeMethods.ODS_NOACCEL,
        /// <include file='doc\DrawItemState.uex' path='docs/doc[@for="DrawItemState.NoFocusRect"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The item displays without the visual cue that indicates it has the focus.
        ///    </para>
        /// </devdoc>
        NoFocusRect        = NativeMethods.ODS_NOFOCUSRECT,
        /// <include file='doc\DrawItemState.uex' path='docs/doc[@for="DrawItemState.Selected"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The item is selected.
        ///    </para>
        /// </devdoc>
        Selected     = NativeMethods.ODS_SELECTED,
        /// <include file='doc\DrawItemState.uex' path='docs/doc[@for="DrawItemState.None"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The item is in its default visual state.
        ///    </para>
        /// </devdoc>
        None         = 0,

    }
}

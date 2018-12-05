// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;


    /// <devdoc>
    ///    <para>
    ///
    ///       Specifies
    ///       the state of an item that is being drawn.
    ///    </para>
    /// </devdoc>
    [Flags]
    public enum DrawItemState {
        /// <devdoc>
        ///    <para>
        ///       The item is checked. Only menu controls use this value.
        ///
        ///    </para>
        /// </devdoc>
        Checked = NativeMethods.ODS_CHECKED,
        /// <devdoc>
        ///    <para>
        ///       The item is the editing portion of a <see cref='System.Windows.Forms.ComboBox'/> .
        ///    </para>
        /// </devdoc>
        ComboBoxEdit = NativeMethods.ODS_COMBOBOXEDIT,
        /// <devdoc>
        ///    <para>
        ///       The item is the default item of the control.
        ///    </para>
        /// </devdoc>
        Default     = NativeMethods.ODS_DEFAULT,
        /// <devdoc>
        ///    <para>
        ///       The item is disabled.
        ///    </para>
        /// </devdoc>
        Disabled     = NativeMethods.ODS_DISABLED,
        /// <devdoc>
        ///    <para>
        ///       The item has focus.
        ///    </para>
        /// </devdoc>
        Focus        = NativeMethods.ODS_FOCUS,
        /// <devdoc>
        ///    <para>
        ///       The item
        ///       is grayed. Only menu controls use this value.
        ///
        ///    </para>
        /// </devdoc>
        Grayed        = NativeMethods.ODS_GRAYED,
        /// <devdoc>
        ///    <para>
        ///       The item is being hot-tracked.
        ///    </para>
        /// </devdoc>
        HotLight        = NativeMethods.ODS_HOTLIGHT,
        /// <devdoc>
        ///    <para>
        ///       The item is inactive.
        ///    </para>
        /// </devdoc>
        Inactive        = NativeMethods.ODS_INACTIVE,
        /// <devdoc>
        ///    <para>
        ///       The item displays without a keyboard accelarator.
        ///    </para>
        /// </devdoc>
        NoAccelerator        = NativeMethods.ODS_NOACCEL,
        /// <devdoc>
        ///    <para>
        ///       The item displays without the visual cue that indicates it has the focus.
        ///    </para>
        /// </devdoc>
        NoFocusRect        = NativeMethods.ODS_NOFOCUSRECT,
        /// <devdoc>
        ///    <para>
        ///       The item is selected.
        ///    </para>
        /// </devdoc>
        Selected     = NativeMethods.ODS_SELECTED,
        /// <devdoc>
        ///    <para>
        ///       The item is in its default visual state.
        ///    </para>
        /// </devdoc>
        None         = 0,

    }
}

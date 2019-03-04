// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Specifies the state of an item that is being drawn.
    /// </devdoc>
    [Flags]
    public enum DrawItemState
    {
        /// <devdoc>
        /// The item is checked. Only menu controls use this value.
        /// </devdoc>
        Checked = NativeMethods.ODS_CHECKED,

        /// <devdoc>
        /// The item is the editing portion of a <see cref='System.Windows.Forms.ComboBox'/> .
        /// </devdoc>
        ComboBoxEdit = NativeMethods.ODS_COMBOBOXEDIT,

        /// <devdoc>
        /// The item is the default item of the control.
        /// </devdoc>
        Default = NativeMethods.ODS_DEFAULT,

        /// <devdoc>
        /// The item is disabled.
        /// </devdoc>
        Disabled = NativeMethods.ODS_DISABLED,

        /// <devdoc>
        /// The item has focus.
        /// </devdoc>
        Focus = NativeMethods.ODS_FOCUS,
        
        /// <devdoc>
        /// The item is grayed. Only menu controls use this value.
        /// </devdoc>
        Grayed = NativeMethods.ODS_GRAYED,

        /// <devdoc>
        /// The item is being hot-tracked.
        /// </devdoc>
        HotLight = NativeMethods.ODS_HOTLIGHT,

        /// <devdoc>
        /// The item is inactive.
        /// </devdoc>
        Inactive = NativeMethods.ODS_INACTIVE,

        /// <devdoc>
        /// The item displays without a keyboard accelarator.
        /// </devdoc>
        NoAccelerator = NativeMethods.ODS_NOACCEL,

        /// <devdoc>
        /// The item displays without the visual cue that indicates it has the focus.
        /// </devdoc>
        NoFocusRect = NativeMethods.ODS_NOFOCUSRECT,

        /// <devdoc>
        /// The item is selected.
        /// </devdoc>
        Selected = NativeMethods.ODS_SELECTED,

        /// <devdoc>
        /// The item is in its default visual state.
        /// </devdoc>
        None = 0,
    }
}

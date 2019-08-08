// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies the state of an item that is being drawn.
    /// </summary>
    [Flags]
    public enum DrawItemState
    {
        /// <summary>
        ///  The item is checked. Only menu controls use this value.
        /// </summary>
        Checked = NativeMethods.ODS_CHECKED,

        /// <summary>
        ///  The item is the editing portion of a <see cref='ComboBox'/> .
        /// </summary>
        ComboBoxEdit = NativeMethods.ODS_COMBOBOXEDIT,

        /// <summary>
        ///  The item is the default item of the control.
        /// </summary>
        Default = NativeMethods.ODS_DEFAULT,

        /// <summary>
        ///  The item is disabled.
        /// </summary>
        Disabled = NativeMethods.ODS_DISABLED,

        /// <summary>
        ///  The item has focus.
        /// </summary>
        Focus = NativeMethods.ODS_FOCUS,

        /// <summary>
        ///  The item is grayed. Only menu controls use this value.
        /// </summary>
        Grayed = NativeMethods.ODS_GRAYED,

        /// <summary>
        ///  The item is being hot-tracked.
        /// </summary>
        HotLight = NativeMethods.ODS_HOTLIGHT,

        /// <summary>
        ///  The item is inactive.
        /// </summary>
        Inactive = NativeMethods.ODS_INACTIVE,

        /// <summary>
        ///  The item displays without a keyboard accelarator.
        /// </summary>
        NoAccelerator = NativeMethods.ODS_NOACCEL,

        /// <summary>
        ///  The item displays without the visual cue that indicates it has the focus.
        /// </summary>
        NoFocusRect = NativeMethods.ODS_NOFOCUSRECT,

        /// <summary>
        ///  The item is selected.
        /// </summary>
        Selected = NativeMethods.ODS_SELECTED,

        /// <summary>
        ///  The item is in its default visual state.
        /// </summary>
        None = 0,
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

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
        Checked = (int)User32.ODS.CHECKED,

        /// <summary>
        ///  The item is the editing portion of a <see cref='ComboBox'/> .
        /// </summary>
        ComboBoxEdit = (int)User32.ODS.COMBOBOXEDIT,

        /// <summary>
        ///  The item is the default item of the control.
        /// </summary>
        Default = (int)User32.ODS.DEFAULT,

        /// <summary>
        ///  The item is disabled.
        /// </summary>
        Disabled = (int)User32.ODS.DISABLED,

        /// <summary>
        ///  The item has focus.
        /// </summary>
        Focus = (int)User32.ODS.FOCUS,

        /// <summary>
        ///  The item is grayed. Only menu controls use this value.
        /// </summary>
        Grayed = (int)User32.ODS.GRAYED,

        /// <summary>
        ///  The item is being hot-tracked.
        /// </summary>
        HotLight = (int)User32.ODS.HOTLIGHT,

        /// <summary>
        ///  The item is inactive.
        /// </summary>
        Inactive = (int)User32.ODS.INACTIVE,

        /// <summary>
        ///  The item displays without a keyboard accelarator.
        /// </summary>
        NoAccelerator = (int)User32.ODS.NOACCEL,

        /// <summary>
        ///  The item displays without the visual cue that indicates it has the focus.
        /// </summary>
        NoFocusRect = (int)User32.ODS.NOFOCUSRECT,

        /// <summary>
        ///  The item is selected.
        /// </summary>
        Selected = (int)User32.ODS.SELECTED,

        /// <summary>
        ///  The item is in its default visual state.
        /// </summary>
        None = 0,
    }
}

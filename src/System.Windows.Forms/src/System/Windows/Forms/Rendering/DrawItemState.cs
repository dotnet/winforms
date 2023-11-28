// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Specifies the state of an item that is being drawn.
/// </summary>
[Flags]
public enum DrawItemState
{
    /// <summary>
    ///  The item is checked. Only menu controls use this value.
    /// </summary>
    Checked = (int)ODS_FLAGS.ODS_CHECKED,

    /// <summary>
    ///  The item is the editing portion of a <see cref="ComboBox"/> .
    /// </summary>
    ComboBoxEdit = (int)ODS_FLAGS.ODS_COMBOBOXEDIT,

    /// <summary>
    ///  The item is the default item of the control.
    /// </summary>
    Default = (int)ODS_FLAGS.ODS_DEFAULT,

    /// <summary>
    ///  The item is disabled.
    /// </summary>
    Disabled = (int)ODS_FLAGS.ODS_DISABLED,

    /// <summary>
    ///  The item has focus.
    /// </summary>
    Focus = (int)ODS_FLAGS.ODS_FOCUS,

    /// <summary>
    ///  The item is grayed. Only menu controls use this value.
    /// </summary>
    Grayed = (int)ODS_FLAGS.ODS_GRAYED,

    /// <summary>
    ///  The item is being hot-tracked.
    /// </summary>
    HotLight = (int)ODS_FLAGS.ODS_HOTLIGHT,

    /// <summary>
    ///  The item is inactive.
    /// </summary>
    Inactive = (int)ODS_FLAGS.ODS_INACTIVE,

    /// <summary>
    ///  The item displays without a keyboard accelerator.
    /// </summary>
    NoAccelerator = (int)ODS_FLAGS.ODS_NOACCEL,

    /// <summary>
    ///  The item displays without the visual cue that indicates it has the focus.
    /// </summary>
    NoFocusRect = (int)ODS_FLAGS.ODS_NOFOCUSRECT,

    /// <summary>
    ///  The item is selected.
    /// </summary>
    Selected = (int)ODS_FLAGS.ODS_SELECTED,

    /// <summary>
    ///  The item is in its default visual state.
    /// </summary>
    None = 0,
}

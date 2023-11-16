// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.PropertyGridInternal;

internal abstract partial class GridEntry
{
    [Flags]
    public enum Flags
    {
        TextEditable            = 0x00000001,

        /// <summary>
        ///  The <see cref="TypeConverter"/> supports standard values.
        /// </summary>
        StandardValuesSupported = 0x00000002,

        /// <summary>
        ///  The current <see cref="UITypeEditor"/> returned true for <see cref="UITypeEditor.GetPaintValueSupported()"/>.
        /// </summary>
        CustomPaint             = 0x00000004,

        /// <summary>
        ///  The current <see cref="UITypeEditor.GetEditStyle()"/> is <see cref="UITypeEditorEditStyle.Modal"/>.
        /// </summary>
        ModalEditable           = 0x00000010,

        /// <summary>
        ///  The current <see cref="UITypeEditor.GetEditStyle()"/> is <see cref="UITypeEditorEditStyle.DropDown"/>.
        /// </summary>
        DropDownEditable        = 0x00000020,

        /// <summary>
        ///  True if the label should be rendered in bold text. Used by <see cref="CategoryGridEntry"/>.
        /// </summary>
        LabelBold               = 0x00000040,

        /// <summary>
        ///  True when the value cannot be edited via the text box, but has a modal editor (`...` button).
        /// </summary>
        ReadOnlyEditable        = 0x00000080,

        RenderReadOnly          = 0x00000100,

        /// <summary>
        ///  True when the value is attributed with <see cref="ImmutableObjectAttribute"/> or the
        ///  <see cref="TypeConverter.GetCreateInstanceSupported()"/> indicates that it is immutable.
        /// </summary>
        Immutable               = 0x00000200,

        /// <summary>
        ///  Used to distribute read-only behavior to child properties when the root <see cref="GridEntry"/> is
        ///  read-only or one of the objects in the root <see cref="GridEntry"/> (with multiple select) is read-only.
        /// </summary>
        ForceReadOnly           = 0x00000400,

        /// <summary>
        ///  True when <see cref="PasswordPropertyTextAttribute"/> is set.
        /// </summary>
        RenderPassword          = 0x00001000,

        Disposed                = 0x00002000,
        Expand                  = 0x00010000,
        Expandable              = 0x00020000,
        ExpandableFailed        = 0x00080000,

        /// <summary>
        ///  Inverse of <see cref="CustomPaint"/> that is only used when full flags have not been checked.
        /// </summary>
        NoCustomPaint           = 0x00100000,

        /// <summary>
        ///  Set when all the flags have been checked.
        /// </summary>
        Checked                 = unchecked((int)0x80000000)
    }
}

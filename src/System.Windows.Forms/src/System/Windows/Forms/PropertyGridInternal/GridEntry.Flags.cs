// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing.Design;

namespace System.Windows.Forms.PropertyGridInternal
{
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
            CustomPaint             = 0x00000004,
            ImmediatelyEditable     = 0x00000008,

            /// <summary>
            ///  The current <see cref="UITypeEditor.GetEditStyle()"/> is <see cref="UITypeEditorEditStyle.Modal"/>.
            /// </summary>
            ModalEditable           = 0x00000010,

            /// <summary>
            ///  The current <see cref="UITypeEditor.GetEditStyle()"/> is <see cref="UITypeEditorEditStyle.DropDown"/>.
            /// </summary>
            DropDownEditable        = 0x00000020,
            LabelBold               = 0x00000040,
            ReadOnlyEditable        = 0x00000080,
            RenderReadOnly          = 0x00000100,
            Immutable               = 0x00000200,

            /// <summary>
            ///  Used to distribute read-only behavior to child properties when the root <see cref="GridEntry"/> is
            ///  read-only or one of the objects in the root <see cref="GridEntry"/> (with multiple select) is read-only.
            /// </summary>
            ForceReadOnly           = 0x00000400,
            RenderPassword          = 0x00001000,
            Disposed                = 0x00002000,
            Expand                  = 0x00010000,
            Expandable              = 0x00020000,
            ExpandableFailed        = 0x00080000,
            NoCustomPaint           = 0x00100000,
            Categories              = 0x00200000,
            Checked                 = unchecked((int)0x80000000)
        }
    }
}

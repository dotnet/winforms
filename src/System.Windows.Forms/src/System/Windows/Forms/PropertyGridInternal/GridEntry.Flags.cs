// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.PropertyGridInternal
{
    internal abstract partial class GridEntry
    {
        // Type flags
        [Flags]
        public enum Flags
        {
            TextEditable        = 0x00000001,
            Enumerable          = 0x00000002,
            CustomPaint         = 0x00000004,
            ImmediatelyEditable = 0x00000008,
            CustomEditable      = 0x00000010,
            DropdownEditable    = 0x00000020,
            LabelBold           = 0x00000040,
            ReadOnlyEditable    = 0x00000080,
            RenderReadOnly      = 0x00000100,
            Immutable           = 0x00000200,
            ForceReadOnly       = 0x00000400,
            RenderPassword      = 0x00001000,
            Disposed            = 0x00002000,
            Expand              = 0x00010000,
            Expandable          = 0x00020000,
            ExpandableFailed    = 0x00080000,
            NoCustomPaint       = 0x00100000,
            Categories          = 0x00200000,
            Checked             = unchecked((int)0x80000000)
        }
    }
}

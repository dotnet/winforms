// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class MaskedTextBox
    {
        internal class MaskedTextBoxAccessibleObject : TextBoxBaseAccessibleObject
        {
            public MaskedTextBoxAccessibleObject(MaskedTextBox owner) : base(owner)
            {
            }

            public override string? Name
            {
                get => string.IsNullOrEmpty((Owner as MaskedTextBox)?.Mask)
                    ? base.Name
                    // If base.Name is null mask template will be used as a name, which is not descriptive for users.
                    // Instead, we want to show an empty string to signal developers to set an appropriate name.
                    : base.Name ?? string.Empty;
                set => base.Name = value;
            }

            protected override string ValueInternal => Owner.WindowText;
        }
    }
}

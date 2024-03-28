// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class MaskedTextBox
{
    internal sealed class MaskedTextBoxAccessibleObject : TextBoxBaseAccessibleObject
    {
        public MaskedTextBoxAccessibleObject(MaskedTextBox owner) : base(owner)
        {
        }

        public override string? Name
        {
            get => !this.TryGetOwnerAs(out MaskedTextBox? owner) || string.IsNullOrEmpty(owner.Mask)
                ? base.Name
                // If base.Name is null mask template will be used as a name, which is not descriptive for users.
                // Instead, we want to show an empty string to signal developers to set an appropriate name.
                : base.Name ?? string.Empty;
        }

        private protected override bool IsInternal => true;

        protected override string ValueInternal
            => this.TryGetOwnerAs(out MaskedTextBox? owner) ? owner.WindowText : string.Empty;
    }
}

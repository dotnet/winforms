// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class Label
{
    internal class LabelAccessibleObject : ControlAccessibleObject
    {
        public LabelAccessibleObject(Label owner) : base(owner)
        {
        }

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.StaticText);

        public override string? KeyboardShortcut => !this.TryGetOwnerAs(out Label? owner) || !owner.UseMnemonic ? null : base.KeyboardShortcut;

        private protected override bool IsInternal => true;

        public override string? Name
        {
            get
            {
                if (!this.TryGetOwnerAs(out Label? owner))
                {
                    return null;
                }

                if (owner.AccessibleName is { } name)
                {
                    return name;
                }

                return owner.UseMnemonic ? WindowsFormsUtils.TextWithoutMnemonics(TextLabel) : TextLabel;
            }
        }
    }
}

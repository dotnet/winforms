// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public abstract partial class UpDownBase
{
    internal sealed class UpDownBaseAccessibleObject : ControlAccessibleObject
    {
        public UpDownBaseAccessibleObject(UpDownBase owner) : base(owner)
        { }

        public override AccessibleObject? GetChild(int index)
        {
            if (!this.TryGetOwnerAs(out UpDownBase? owner))
            {
                return null;
            }

            return index switch
            {
                // TextBox child
                0 => owner.TextBox.AccessibilityObject.Parent,
                // Up/down buttons
                1 => owner.UpDownButtonsInternal.AccessibilityObject.Parent,
                _ => null,
            };
        }

        private protected override bool IsInternal => true;

        public override int GetChildCount() => 2;

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.SpinButton);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

        public override string? Name => this.TryGetOwnerAs(out Label? owner) && owner.UseMnemonic ? base.Name : TextLabel;
    }
}

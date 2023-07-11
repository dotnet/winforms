// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms;

public partial class RadioButton
{
    public class RadioButtonAccessibleObject : ControlAccessibleObject
    {
        public RadioButtonAccessibleObject(RadioButton owner) : base(owner)
        {
        }

        public override string DefaultAction
            => this.TryGetOwnerAs(out RadioButton? owner) && owner.AccessibleDefaultActionDescription is { } description
                ? description
                : SR.AccessibleActionCheck;

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.RadioButton);

        public override AccessibleStates State
            => this.TryGetOwnerAs(out RadioButton? owner) && owner.Checked
                ? AccessibleStates.Checked | base.State
                : base.State;

        internal override bool IsItemSelected
            => this.TryGetOwnerAs(out RadioButton? owner) && owner.Checked;

        public override void DoDefaultAction()
        {
            if (this.TryGetOwnerAs(out RadioButton? owner) && owner.IsHandleCreated)
            {
                owner.PerformClick();
            }
        }

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                UiaCore.UIA.HasKeyboardFocusPropertyId => this.TryGetOwnerAs(out RadioButton? owner) && owner.Focused,
                UiaCore.UIA.IsKeyboardFocusablePropertyId
                    // This is necessary for compatibility with MSAA proxy:
                    // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                    => true,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsPatternSupported(UiaCore.UIA patternId)
            => patternId switch
            {
                var p when
                    p == UiaCore.UIA.SelectionItemPatternId => true,
                _ => base.IsPatternSupported(patternId)
            };

        public override string? KeyboardShortcut => this.TryGetOwnerAs(out RadioButton? owner)
            ? ButtonBaseAccessibleObject.GetKeyboardShortcut(owner, owner.UseMnemonic, PreviousLabel)
            : null;

        public override string? Name
        {
            get
            {
                if (!this.TryGetOwnerAs(out RadioButton? owner))
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

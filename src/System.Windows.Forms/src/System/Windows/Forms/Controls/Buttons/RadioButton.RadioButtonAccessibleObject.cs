// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class RadioButton
{
    public class RadioButtonAccessibleObject(RadioButton owner) : ControlAccessibleObject(owner)
    {
        public override string DefaultAction =>
            this.TryGetOwnerAs(out RadioButton? owner) && owner.AccessibleDefaultActionDescription is { } description
                ? description
                : SR.AccessibleActionCheck;

        internal override bool CanGetDefaultActionInternal => false;

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.RadioButton);

        public override AccessibleStates State =>
            this.TryGetOwnerAs(out RadioButton? owner) && owner.Checked
                ? AccessibleStates.Checked | base.State
                : base.State;

        internal override bool IsItemSelected => this.TryGetOwnerAs(out RadioButton? owner) && owner.Checked;

        public override void DoDefaultAction()
        {
            if (this.IsOwnerHandleCreated(out RadioButton? owner))
            {
                owner.PerformClick();
            }
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) => propertyID switch
        {
            UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)(this.TryGetOwnerAs(out RadioButton? owner) && owner.Focused),
            UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId
                // This is necessary for compatibility with MSAA proxy:
                // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                => VARIANT.True,
            _ => base.GetPropertyValue(propertyID)
        };

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId) => patternId switch
        {
            var p when p == UIA_PATTERN_ID.UIA_SelectionItemPatternId => true,
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

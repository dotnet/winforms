// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class CheckBox
{
    public class CheckBoxAccessibleObject(Control owner) : ButtonBaseAccessibleObject((owner is CheckBox)
        ? owner
        : throw new ArgumentException(string.Format(SR.ConstructorArgumentInvalidValueType, nameof(owner), typeof(CheckBox))))
    {
        public override string DefaultAction => this.TryGetOwnerAs(out CheckBox? owner)
            ? owner.AccessibleDefaultActionDescription ?? (owner.Checked ? SR.AccessibleActionUncheck : SR.AccessibleActionCheck)
            : string.Empty;

        internal override bool CanGetDefaultActionInternal => false;

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.CheckButton);

        public override AccessibleStates State => !this.TryGetOwnerAs(out CheckBox? owner)
            ? base.State
            : owner.CheckState switch
            {
                CheckState.Checked => AccessibleStates.Checked | base.State,
                CheckState.Indeterminate => AccessibleStates.Indeterminate | base.State,
                _ => base.State
            };

        internal override ToggleState ToggleState => this.TryGetOwnerAs(out CheckBox? owner)
            ? owner.CheckState switch
            {
                CheckState.Checked => ToggleState.ToggleState_On,
                CheckState.Unchecked => ToggleState.ToggleState_Off,
                _ => ToggleState.ToggleState_Indeterminate,
            }
            : ToggleState.ToggleState_Off;

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId) => patternId switch
        {
            var p when p == UIA_PATTERN_ID.UIA_TogglePatternId => true,
            _ => base.IsPatternSupported(patternId)
        };

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) => propertyID switch
        {
            UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)(this.TryGetOwnerAs(out Control? owner) && owner.Focused),
            UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId =>
                // This is necessary for compatibility with MSAA proxy:
                // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                VARIANT.True,
            _ => base.GetPropertyValue(propertyID)
        };

        public override void DoDefaultAction()
        {
            if (!this.IsOwnerHandleCreated(out CheckBox? owner))
            {
                return;
            }

            try
            {
                owner.AccObjDoDefaultAction = true;
                base.DoDefaultAction();
            }
            finally
            {
                owner.AccObjDoDefaultAction = false;
            }
        }

        internal override void Toggle()
        {
            if (this.TryGetOwnerAs(out CheckBox? owner))
            {
                if (owner.ThreeState)
                {
                    owner.CheckState = owner.CheckState switch
                    {
                        CheckState.Unchecked => CheckState.Checked,
                        CheckState.Checked => CheckState.Indeterminate,
                        _ => CheckState.Unchecked
                    };
                }
                else
                {
                    owner.Checked = !owner.Checked;
                }
            }
        }
    }
}

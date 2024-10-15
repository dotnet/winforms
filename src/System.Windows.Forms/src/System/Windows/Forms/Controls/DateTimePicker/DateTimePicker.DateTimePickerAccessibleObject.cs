// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class DateTimePicker
{
    public class DateTimePickerAccessibleObject : ControlAccessibleObject
    {
        public DateTimePickerAccessibleObject(DateTimePicker owner) : base(owner)
        {
        }

        public override string? KeyboardShortcut
        {
            get
            {
                // APP COMPAT. When computing DateTimePickerAccessibleObject::get_KeyboardShortcut the previous label
                // takes precedence over DTP::Text.
                // This code was copied from the Everett sources.
                Label? previousLabel = PreviousLabel;

                if (previousLabel is not null)
                {
                    char previousLabelMnemonic = WindowsFormsUtils.GetMnemonic(previousLabel.Text, convertToUpperCase: false);
                    if (previousLabelMnemonic != '\0')
                    {
                        return $"Alt+{previousLabelMnemonic}";
                    }
                }

                // Win32 DTP does not interpret ampersand in its Text as an escape character for a mnemonic.
                return null;
            }
        }

        // Note: returns empty string instead of null, because the date value replaces null,
        // so name is not empty in this case even if AccessibleName is not set.
        public override string Name => this.GetOwnerAccessibleName(string.Empty);

        internal override bool CanGetNameInternal => false;

        public override string Value
        {
            get
            {
                string? baseValue = base.Value;
                return !string.IsNullOrEmpty(baseValue) ? baseValue : this.GetOwnerText();
            }
        }

        internal override bool CanGetValueInternal => false;

        public override AccessibleStates State
        {
            get
            {
                AccessibleStates state = base.State;

                if (this.TryGetOwnerAs(out DateTimePicker? owner) && owner.ShowCheckBox && owner.Checked)
                {
                    state |= AccessibleStates.Checked;
                }

                return state;
            }
        }

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.ComboBox);

        internal override bool IsIAccessibleExSupported() => true;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_LocalizedControlTypePropertyId when this.GetOwnerAccessibleRole() == AccessibleRole.Default
                    // We define a custom "LocalizedControlType" by default.
                    // If DateTimePicker.AccessibleRole value is customized by a user
                    // then "LocalizedControlType" value will be based on "ControlType"
                    // which depends on DateTimePicker.AccessibleRole.
                    => (VARIANT)s_dateTimePickerLocalizedControlTypeString,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_TogglePatternId when this.TryGetOwnerAs(out DateTimePicker? owner) && owner.ShowCheckBox => true,
                UIA_PATTERN_ID.UIA_ExpandCollapsePatternId => true,
                UIA_PATTERN_ID.UIA_ValuePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };

        public override string DefaultAction
            => ExpandCollapseState switch
            {
                ExpandCollapseState.ExpandCollapseState_Collapsed => SR.AccessibleActionExpand,
                ExpandCollapseState.ExpandCollapseState_Expanded => SR.AccessibleActionCollapse,
                _ => string.Empty
            };

        internal override bool CanGetDefaultActionInternal => false;

        public override void DoDefaultAction()
        {
            switch (ExpandCollapseState)
            {
                case ExpandCollapseState.ExpandCollapseState_Collapsed:
                    Expand();
                    break;
                case ExpandCollapseState.ExpandCollapseState_Expanded:
                    Collapse();
                    break;
            }
        }

        internal override ToggleState ToggleState
            => this.TryGetOwnerAs(out DateTimePicker? owner) && owner.Checked ? ToggleState.ToggleState_On : ToggleState.ToggleState_Off;

        internal override void Toggle()
        {
            if (this.IsOwnerHandleCreated(out DateTimePicker? owner))
            {
                owner.Checked = !owner.Checked;
            }
        }

        internal override void Expand()
        {
            if (this.IsOwnerHandleCreated(out DateTimePicker? owner)
                && ExpandCollapseState == ExpandCollapseState.ExpandCollapseState_Collapsed)
            {
                PInvokeCore.SendMessage(owner, PInvokeCore.WM_SYSKEYDOWN, (WPARAM)(int)Keys.Down);
            }
        }

        internal override void Collapse()
        {
            if (this.IsOwnerHandleCreated(out DateTimePicker? owner)
                && ExpandCollapseState == ExpandCollapseState.ExpandCollapseState_Expanded)
            {
                PInvokeCore.SendMessage(owner, PInvoke.DTM_CLOSEMONTHCAL);
            }
        }

        internal override ExpandCollapseState ExpandCollapseState
            => this.TryGetOwnerAs(out DateTimePicker? owner)
                ? owner._expandCollapseState
                : ExpandCollapseState.ExpandCollapseState_Collapsed;
    }
}

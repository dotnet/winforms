// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

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
        public override string Name => this.GetOwnerAccessibleName("");

        public override string Value
        {
            get
            {
                string? baseValue = base.Value;
                return !string.IsNullOrEmpty(baseValue) ? baseValue : this.GetOwnerText();
            }
        }

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

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                UiaCore.UIA.LocalizedControlTypePropertyId when this.GetOwnerAccessibleRole() == AccessibleRole.Default
                    // We define a custom "LocalizedControlType" by default.
                    // If DateTimePicker.AccessibleRole value is customized by a user
                    // then "LocalizedControlType" value will be based on "ControlType"
                    // which depends on DateTimePicker.AccessibleRole.
                    => s_dateTimePickerLocalizedControlTypeString,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsPatternSupported(UiaCore.UIA patternId)
            => patternId switch
            {
                UiaCore.UIA.TogglePatternId when this.TryGetOwnerAs(out DateTimePicker? owner) && owner.ShowCheckBox => true,
                UiaCore.UIA.ExpandCollapsePatternId => true,
                UiaCore.UIA.ValuePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };

        public override string DefaultAction
            => ExpandCollapseState switch
            {
                UiaCore.ExpandCollapseState.Collapsed => SR.AccessibleActionExpand,
                UiaCore.ExpandCollapseState.Expanded => SR.AccessibleActionCollapse,
                _ => string.Empty
            };

        public override void DoDefaultAction()
        {
            switch (ExpandCollapseState)
            {
                case UiaCore.ExpandCollapseState.Collapsed:
                    Expand();
                    break;
                case UiaCore.ExpandCollapseState.Expanded:
                    Collapse();
                    break;
            }
        }

        internal override UiaCore.ToggleState ToggleState
            => this.TryGetOwnerAs(out DateTimePicker? owner) && owner.Checked ? UiaCore.ToggleState.On : UiaCore.ToggleState.Off;

        internal override void Toggle()
        {
            if (this.IsHandleCreated(out DateTimePicker? owner))
            {
                owner.Checked = !owner.Checked;
            }
        }

        internal override void Expand()
        {
            if (this.IsHandleCreated(out DateTimePicker? owner)
                && ExpandCollapseState == UiaCore.ExpandCollapseState.Collapsed)
            {
                PInvoke.SendMessage(owner, PInvoke.WM_SYSKEYDOWN, (WPARAM)(int)Keys.Down);
            }
        }

        internal override void Collapse()
        {
            if (this.IsHandleCreated(out DateTimePicker? owner)
                && ExpandCollapseState == UiaCore.ExpandCollapseState.Expanded)
            {
                PInvoke.SendMessage(owner, PInvoke.DTM_CLOSEMONTHCAL);
            }
        }

        internal override UiaCore.ExpandCollapseState ExpandCollapseState
            => this.TryGetOwnerAs(out DateTimePicker? owner)
                ? owner._expandCollapseState
                : UiaCore.ExpandCollapseState.Collapsed;
    }
}

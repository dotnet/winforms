// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
    public partial class DateTimePicker
    {
        public class DateTimePickerAccessibleObject : ControlAccessibleObject
        {
            public DateTimePickerAccessibleObject(DateTimePicker owner) : base(owner)
            {
            }

            public override string KeyboardShortcut
            {
                get
                {
                    // APP COMPAT. When computing DateTimePickerAccessibleObject::get_KeyboardShortcut the previous label
                    // takes precedence over DTP::Text.
                    // This code was copied from the Everett sources.
                    Label previousLabel = PreviousLabel;

                    if (previousLabel is not null)
                    {
                        char previousLabelMnemonic = WindowsFormsUtils.GetMnemonic(previousLabel.Text, false /*convertToUpperCase*/);
                        if (previousLabelMnemonic != (char)0)
                        {
                            return "Alt+" + previousLabelMnemonic;
                        }
                    }

                    string baseShortcut = base.KeyboardShortcut;

                    if ((baseShortcut is null || baseShortcut.Length == 0))
                    {
                        char ownerTextMnemonic = WindowsFormsUtils.GetMnemonic(Owner.Text, false /*convertToUpperCase*/);
                        if (ownerTextMnemonic != (char)0)
                        {
                            return "Alt+" + ownerTextMnemonic;
                        }
                    }

                    return baseShortcut;
                }
            }

            public override string Value
            {
                get
                {
                    string baseValue = base.Value;
                    if (baseValue is null || baseValue.Length == 0)
                    {
                        return Owner.Text;
                    }

                    return baseValue;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = base.State;

                    if (((DateTimePicker)Owner).ShowCheckBox &&
                       ((DateTimePicker)Owner).Checked)
                    {
                        state |= AccessibleStates.Checked;
                    }

                    return state;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }

                    return AccessibleRole.ComboBox;
                }
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.IsTogglePatternAvailablePropertyId:
                        return IsPatternSupported(UiaCore.UIA.TogglePatternId);
                    case UiaCore.UIA.LocalizedControlTypePropertyId:
                        // We define a custom "LocalizedControlType" by default.
                        // If DateTimePicker.AccessibleRole value is customized by a user
                        // then "LocalizedControlType" value will be based on "ControlType"
                        // which depends on DateTimePicker.AccessibleRole.
                        return Owner.AccessibleRole == AccessibleRole.Default
                               ? s_dateTimePickerLocalizedControlTypeString
                               : base.GetPropertyValue(propertyID);
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.TogglePatternId && ((DateTimePicker)Owner).ShowCheckBox)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            #region Toggle Pattern

            internal override UiaCore.ToggleState ToggleState
            {
                get
                {
                    return ((DateTimePicker)Owner).Checked ?
                        UiaCore.ToggleState.On :
                        UiaCore.ToggleState.Off;
                }
            }

            internal override void Toggle()
            {
                if (Owner.IsHandleCreated)
                {
                    ((DateTimePicker)Owner).Checked = !((DateTimePicker)Owner).Checked;
                }
            }

            #endregion
        }
    }
}

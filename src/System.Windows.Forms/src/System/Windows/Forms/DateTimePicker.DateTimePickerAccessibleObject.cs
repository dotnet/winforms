// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;
using static Interop.ComCtl32;
using static Interop.User32;

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

            // Note: returns empty string instead of null, because the date value replaces null,
            // so name is not empty in this case even if AccessibleName is not set.
            public override string Name => Owner.AccessibleName ?? string.Empty;

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
                => propertyID switch
                {
                    UiaCore.UIA.LocalizedControlTypePropertyId =>
                        // We define a custom "LocalizedControlType" by default.
                        // If DateTimePicker.AccessibleRole value is customized by a user
                        // then "LocalizedControlType" value will be based on "ControlType"
                        // which depends on DateTimePicker.AccessibleRole.
                        Owner.AccessibleRole == AccessibleRole.Default
                            ? s_dateTimePickerLocalizedControlTypeString
                            : base.GetPropertyValue(propertyID),
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => (patternId == UiaCore.UIA.TogglePatternId && ((DateTimePicker)Owner).ShowCheckBox) ||
                    patternId == UiaCore.UIA.ExpandCollapsePatternId ||
                    patternId == UiaCore.UIA.ValuePatternId ||
                    base.IsPatternSupported(patternId);

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

            #region Expand-Collapse Pattern

            internal override void Expand()
            {
                if (Owner.IsHandleCreated && ExpandCollapseState == UiaCore.ExpandCollapseState.Collapsed)
                {
                    SendMessageW(Owner, WM.SYSKEYDOWN, (nint)Keys.Down);
                }
            }

            internal override void Collapse()
            {
                if (Owner.IsHandleCreated && ExpandCollapseState == UiaCore.ExpandCollapseState.Expanded)
                {
                    SendMessageW(Owner, (WM)DTM.CLOSEMONTHCAL);
                }
            }

            internal override UiaCore.ExpandCollapseState ExpandCollapseState => ((DateTimePicker)Owner)._expandCollapseState;

            #endregion
        }
    }
}

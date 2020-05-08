// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class RadioButton
    {
        public class RadioButtonAccessibleObject : ControlAccessibleObject
        {
            public RadioButtonAccessibleObject(RadioButton owner) : base(owner)
            {
                OwningRadioButton = owner;
            }

            private RadioButton OwningRadioButton { get; set; }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.SelectionItemPatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.NamePropertyId
                        => Name,
                    UiaCore.UIA.AutomationIdPropertyId
                        => OwningRadioButton.IsHandleCreated ? OwningRadioButton.Name : string.Empty,
                    UiaCore.UIA.IsSelectionItemPatternAvailablePropertyId
                        => IsPatternSupported(UiaCore.UIA.SelectionItemPatternId),
                    UiaCore.UIA.ControlTypePropertyId
                        => UiaCore.UIA.RadioButtonControlTypeId,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId
                        // This is necessary for compatibility with MSAA proxy:
                        // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                        => true,
                    _ => base.GetPropertyValue(propertyID)
                };

            public override string DefaultAction
            {
                get
                {
                    string defaultAction = Owner.AccessibleDefaultActionDescription;
                    if (defaultAction != null)
                    {
                        return defaultAction;
                    }

                    return SR.AccessibleActionCheck;
                }
            }

            internal override bool IsItemSelected
                => OwningRadioButton.Checked;

            public override void DoDefaultAction()
            {
                if (OwningRadioButton.IsHandleCreated)
                {
                    OwningRadioButton.PerformClick();
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

                    return AccessibleRole.RadioButton;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    if (OwningRadioButton.Checked)
                    {
                        return AccessibleStates.Checked | base.State;
                    }

                    return base.State;
                }
            }
        }
    }
}

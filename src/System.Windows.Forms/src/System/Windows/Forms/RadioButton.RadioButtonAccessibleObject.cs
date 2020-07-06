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
            private readonly RadioButton _owningRadioButton;

            public RadioButtonAccessibleObject(RadioButton owner) : base(owner)
            {
                _owningRadioButton = owner;
            }

            public override string DefaultAction
                => Owner.AccessibleDefaultActionDescription ?? SR.AccessibleActionCheck;

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    return role != AccessibleRole.Default
                        ? role
                        : AccessibleRole.RadioButton;
                }
            }

            public override AccessibleStates State
                => _owningRadioButton.Checked
                    ? AccessibleStates.Checked | base.State
                    : base.State;

            internal override bool IsItemSelected
                => _owningRadioButton.Checked;

            public override void DoDefaultAction()
            {
                if (_owningRadioButton.IsHandleCreated)
                {
                    _owningRadioButton.PerformClick();
                }
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.NamePropertyId
                        => Name,
                    UiaCore.UIA.AutomationIdPropertyId
                        => Owner.Name,
                    UiaCore.UIA.IsSelectionItemPatternAvailablePropertyId
                        => IsPatternSupported(UiaCore.UIA.SelectionItemPatternId),
                    UiaCore.UIA.ControlTypePropertyId
                        => UiaCore.UIA.RadioButtonControlTypeId,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId
                        // This is necessary for compatibility with MSAA proxy:
                        // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                        => true,
                    UiaCore.UIA.HasKeyboardFocusPropertyId
                        => Owner.Focused,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    var p when
                        p == UiaCore.UIA.SelectionItemPatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };
        }
    }
}

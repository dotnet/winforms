﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms;

public partial class CheckBox
{
    public class CheckBoxAccessibleObject : ButtonBaseAccessibleObject
    {
        public CheckBoxAccessibleObject(Control owner)
            : base((owner is CheckBox owningCheckBox)
                  ? owner
                  : throw new ArgumentException(string.Format(SR.ConstructorArgumentInvalidValueType, nameof(owner), typeof(CheckBox))))
        {
        }

        public override string DefaultAction
            => this.TryGetOwnerAs(out CheckBox? owner)
                ? owner.AccessibleDefaultActionDescription ?? (owner.Checked ? SR.AccessibleActionUncheck : SR.AccessibleActionCheck)
                : string.Empty;

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.CheckButton);

        public override AccessibleStates State
            => !this.TryGetOwnerAs(out CheckBox? owner) ? base.State : owner.CheckState switch
            {
                CheckState.Checked => AccessibleStates.Checked | base.State,
                CheckState.Indeterminate => AccessibleStates.Indeterminate | base.State,
                _ => base.State
            };

        internal override UiaCore.ToggleState ToggleState
            => this.TryGetOwnerAs(out CheckBox? owner)
                ? owner.CheckState switch
                {
                    CheckState.Checked => UiaCore.ToggleState.On,
                    CheckState.Unchecked => UiaCore.ToggleState.Off,
                    _ => UiaCore.ToggleState.Indeterminate,
                }
                : UiaCore.ToggleState.Off;

        internal override bool IsPatternSupported(UiaCore.UIA patternId)
            => patternId switch
            {
                var p when
                    p == UiaCore.UIA.TogglePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                UiaCore.UIA.HasKeyboardFocusPropertyId => this.TryGetOwnerAs(out Control? owner) && owner.Focused,
                UiaCore.UIA.IsKeyboardFocusablePropertyId
                    =>
                    // This is necessary for compatibility with MSAA proxy:
                    // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                    true,
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

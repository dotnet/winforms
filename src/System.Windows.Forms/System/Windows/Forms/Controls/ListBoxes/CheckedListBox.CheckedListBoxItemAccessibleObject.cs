// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class CheckedListBox
{
    internal sealed class CheckedListBoxItemAccessibleObject : ListBoxItemAccessibleObject
    {
        private readonly CheckedListBox _owningCheckedListBox;

        public CheckedListBoxItemAccessibleObject(CheckedListBox owningCheckedListBox, ItemArray.Entry item, CheckedListBoxAccessibleObject owningAccessibleObject) : base(owningCheckedListBox, item, owningAccessibleObject)
        {
            _owningCheckedListBox = owningCheckedListBox;
        }

        public override string DefaultAction
        {
            get
            {
                if (!_owningCheckedListBox.IsHandleCreated)
                {
                    return string.Empty;
                }

                return IsItemChecked ? SR.AccessibleActionUncheck : SR.AccessibleActionCheck;
            }
        }

        internal override bool CanGetDefaultActionInternal => false;

        public override void DoDefaultAction()
        {
            if (!_owningCheckedListBox.IsHandleCreated)
            {
                return;
            }

            _owningCheckedListBox.SetItemChecked(CurrentIndex, !IsItemChecked);
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_CheckBoxControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };

        private bool IsItemChecked => _owningCheckedListBox.GetItemChecked(CurrentIndex);

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_InvokePatternId => true,
                UIA_PATTERN_ID.UIA_TogglePatternId => true,
                UIA_PATTERN_ID.UIA_ValuePatternId => true,
                _ => base.IsPatternSupported(patternId)
            };

        public override AccessibleRole Role => AccessibleRole.CheckButton;

        public override AccessibleStates State
        {
            get
            {
                if (!_owningCheckedListBox.IsHandleCreated)
                {
                    return AccessibleStates.None;
                }

                AccessibleStates state = AccessibleStates.Selectable | AccessibleStates.Focusable;

                if (!Parent.BoundingRectangle.IntersectsWith(Bounds))
                {
                    state |= AccessibleStates.Offscreen;
                }

                // Checked state
                switch (_owningCheckedListBox.GetItemCheckState(CurrentIndex))
                {
                    case CheckState.Checked:
                        state |= AccessibleStates.Checked;
                        break;
                    case CheckState.Indeterminate:
                        state |= AccessibleStates.Indeterminate;
                        break;
                    case CheckState.Unchecked:
                        // No accessible state corresponding to unchecked
                        break;
                }

                // Selected state
                if (_owningCheckedListBox.SelectedIndex == CurrentIndex)
                {
                    state |= AccessibleStates.Selected | AccessibleStates.Focused;
                }

                if (_owningCheckedListBox.Focused && _owningCheckedListBox.SelectedIndex == -1)
                {
                    state |= AccessibleStates.Focused;
                }

                return state;
            }
        }

        internal override void Toggle() => DoDefaultAction();

        internal override ToggleState ToggleState
        {
            get
            {
                ToggleState toggleState = ToggleState.ToggleState_Off;
                switch (_owningCheckedListBox.GetItemCheckState(CurrentIndex))
                {
                    case CheckState.Checked:
                        toggleState = ToggleState.ToggleState_On;
                        break;
                    case CheckState.Indeterminate:
                        toggleState = ToggleState.ToggleState_Indeterminate;
                        break;
                }

                return toggleState;
            }
        }

        public override string Value => IsItemChecked.ToString();

        internal override bool CanGetValueInternal => false;
    }
}

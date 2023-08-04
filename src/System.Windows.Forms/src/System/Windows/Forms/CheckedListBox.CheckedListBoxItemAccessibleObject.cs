﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms;

public partial class CheckedListBox
{
    internal class CheckedListBoxItemAccessibleObject : ListBoxItemAccessibleObject
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

        public override void DoDefaultAction()
        {
            if (!_owningCheckedListBox.IsHandleCreated)
            {
                return;
            }

            _owningCheckedListBox.SetItemChecked(CurrentIndex, !IsItemChecked);
        }

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.CheckBoxControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };

        private bool IsItemChecked => _owningCheckedListBox.GetItemChecked(CurrentIndex);

        internal override bool IsPatternSupported(UiaCore.UIA patternId)
            => patternId switch
            {
                UiaCore.UIA.InvokePatternId => true,
                UiaCore.UIA.TogglePatternId => true,
                UiaCore.UIA.ValuePatternId => true,
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

        internal override UiaCore.ToggleState ToggleState
        {
            get
            {
                UiaCore.ToggleState toggleState= UiaCore.ToggleState.Off;
                switch (_owningCheckedListBox.GetItemCheckState(CurrentIndex))
                {
                    case CheckState.Checked:
                        toggleState = UiaCore.ToggleState.On;
                        break;
                    case CheckState.Indeterminate:
                        toggleState = UiaCore.ToggleState.Indeterminate;
                        break;
                }

                return toggleState;
            }
        }

        public override string Value => IsItemChecked.ToString();
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ToolStripButton
{
    /// <summary>
    ///  An implementation of AccessibleChild for use with ToolStripItems
    /// </summary>
    internal class ToolStripButtonAccessibleObject : ToolStripItemAccessibleObject
    {
        private readonly ToolStripButton _ownerItem;

        public ToolStripButtonAccessibleObject(ToolStripButton ownerItem) : base(ownerItem)
        {
            _ownerItem = ownerItem;
        }

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId) =>
            patternId switch
            {
                UIA_PATTERN_ID.UIA_TogglePatternId => Role == AccessibleRole.CheckButton,
                _ => base.IsPatternSupported(patternId)
            };

        public override AccessibleRole Role
        {
            get
            {
                AccessibleRole role = _ownerItem.AccessibleRole;
                if (role != AccessibleRole.Default)
                {
                    return role;
                }

                if (_ownerItem.CheckOnClick || _ownerItem.Checked)
                {
                    return AccessibleRole.CheckButton;
                }

                return base.Role;
            }
        }

        public override AccessibleStates State
        {
            get
            {
                if (_ownerItem.Enabled && _ownerItem.Checked)
                {
                    return base.State | AccessibleStates.Checked;
                }

                // Disabled ToolStripButton, that is selected, must have focus state so that
                // Narrator can announce it
                if (!_ownerItem.Enabled && _ownerItem.Selected)
                {
                    return base.State | AccessibleStates.Focused;
                }

                return base.State;
            }
        }

        #region Toggle Pattern

        internal override void Toggle()
        {
            if (IsPatternSupported(UIA_PATTERN_ID.UIA_TogglePatternId))
            {
                _ownerItem.Checked = !_ownerItem.Checked;
            }
        }

        internal override ToggleState ToggleState
            => CheckStateToToggleState(_ownerItem.CheckState);

        internal void OnCheckStateChanged(CheckState oldValue, CheckState newValue)
        {
            RaiseAutomationPropertyChangedEvent(
                UIA_PROPERTY_ID.UIA_ToggleToggleStatePropertyId,
                (VARIANT)(int)CheckStateToToggleState(oldValue),
                (VARIANT)(int)CheckStateToToggleState(newValue));
        }

        private static ToggleState CheckStateToToggleState(CheckState checkState)
            => checkState switch
            {
                CheckState.Checked => ToggleState.ToggleState_On,
                CheckState.Unchecked => ToggleState.ToggleState_Off,
                _ => ToggleState.ToggleState_Indeterminate
            };

        #endregion
    }
}

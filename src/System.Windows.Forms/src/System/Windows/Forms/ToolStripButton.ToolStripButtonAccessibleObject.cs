// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

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

        internal override bool IsPatternSupported(UiaCore.UIA patternId) =>
            patternId switch
            {
                UiaCore.UIA.TogglePatternId => Role == AccessibleRole.CheckButton,
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
            if (IsPatternSupported(UiaCore.UIA.TogglePatternId))
            {
                _ownerItem.Checked = !_ownerItem.Checked;
            }
        }

        internal override UiaCore.ToggleState ToggleState
            => CheckStateToToggleState(_ownerItem.CheckState);

        internal void OnCheckStateChanged(CheckState oldValue, CheckState newValue)
        {
            RaiseAutomationPropertyChangedEvent(
                UiaCore.UIA.ToggleToggleStatePropertyId,
                CheckStateToToggleState(oldValue),
                CheckStateToToggleState(newValue));
        }

        private static UiaCore.ToggleState CheckStateToToggleState(CheckState checkState)
            => checkState switch
            {
                CheckState.Checked => UiaCore.ToggleState.On,
                CheckState.Unchecked => UiaCore.ToggleState.Off,
                _ => UiaCore.ToggleState.Indeterminate
            };

        #endregion
    }
}

﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms;

public partial class DataGridViewComboBoxEditingControl
{
    /// <summary>
    ///  Defines the DataGridView ComboBox EditingControl accessible object.
    /// </summary>
    internal class DataGridViewComboBoxEditingControlAccessibleObject : ComboBoxAccessibleObject
    {
        /// <summary>
        ///  The parent is changed when the editing control is attached to another editing cell.
        /// </summary>
        private AccessibleObject? _parentAccessibleObject;

        public DataGridViewComboBoxEditingControlAccessibleObject(DataGridViewComboBoxEditingControl ownerControl)
            : base(ownerControl)
        {
        }

        internal void ClearParent() => _parentAccessibleObject = null;

        public override AccessibleObject? Parent => _parentAccessibleObject;

        internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            switch (direction)
            {
                case UiaCore.NavigateDirection.Parent:
                    if (this.TryGetOwnerAs(out Control? owner) && owner is IDataGridViewEditingControl editingControl
                        && editingControl.EditingControlDataGridView?.EditingControl == owner
                        && owner.ToolStripControlHost is null)
                    {
                        return _parentAccessibleObject;
                    }

                    break;
            }

            return base.FragmentNavigate(direction);
        }

        internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot
            => this.TryGetOwnerAs(out IDataGridViewEditingControl? owner)
                ? owner.EditingControlDataGridView?.AccessibilityObject
                : null;

        internal override bool IsPatternSupported(UiaCore.UIA patternId) => patternId switch
        {
            UiaCore.UIA.ExpandCollapsePatternId when this.TryGetOwnerAs(out DataGridViewComboBoxEditingControl? owner)
                => owner.DropDownStyle != ComboBoxStyle.Simple,
            _ => base.IsPatternSupported(patternId)
        };

        internal override UiaCore.ExpandCollapseState ExpandCollapseState
            => this.TryGetOwnerAs(out DataGridViewComboBoxEditingControl? owner) && owner.DroppedDown
                ? UiaCore.ExpandCollapseState.Expanded
                : UiaCore.ExpandCollapseState.Collapsed;

        /// <summary>
        ///  Sets the parent accessible object for the node which can be added or removed to/from hierarchy nodes.
        /// </summary>
        /// <param name="parent">The parent accessible object.</param>
        internal override void SetParent(AccessibleObject? parent) => _parentAccessibleObject = parent;
    }
}

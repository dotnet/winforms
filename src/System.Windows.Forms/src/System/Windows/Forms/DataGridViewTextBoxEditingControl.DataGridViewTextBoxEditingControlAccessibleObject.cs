// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms;

public partial class DataGridViewTextBoxEditingControl
{
    /// <summary>
    ///  Defines the DataGridView TextBox EditingControl accessible object.
    /// </summary>
    internal class DataGridViewTextBoxEditingControlAccessibleObject : TextBoxBaseAccessibleObject
    {
        /// <summary>
        ///  The parent is changed when the editing control is attached to another editing cell.
        /// </summary>
        private AccessibleObject? _parentAccessibleObject;

        public DataGridViewTextBoxEditingControlAccessibleObject(DataGridViewTextBoxEditingControl ownerControl) : base(ownerControl)
        { }

        internal void ClearParent()
        {
            _parentAccessibleObject = null;
        }

        public override AccessibleObject? Parent => _parentAccessibleObject;

        public override string Name => this.GetOwnerAccessibleName(SR.DataGridView_AccEditingControlAccName);

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

        internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            => propertyID switch
            {
                // If we don't set a default role for the accessible object it will be retrieved from Windows.
                // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                UiaCore.UIA.ControlTypePropertyId when this.GetOwnerAccessibleRole() == AccessibleRole.Default
                    => UiaCore.UIA.EditControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };

        /// <summary>
        ///  Sets the parent accessible object for the node which can be added or removed to/from hierarchy nodes.
        /// </summary>
        /// <param name="parent">The parent accessible object.</param>
        internal override void SetParent(AccessibleObject? parent) => _parentAccessibleObject = parent;
    }
}

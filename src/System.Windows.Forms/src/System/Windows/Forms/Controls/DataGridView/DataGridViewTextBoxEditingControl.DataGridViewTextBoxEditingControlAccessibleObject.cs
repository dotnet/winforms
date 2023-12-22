// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class DataGridViewTextBoxEditingControl
{
    /// <summary>
    ///  Defines the DataGridView TextBox EditingControl accessible object.
    /// </summary>
    internal sealed class DataGridViewTextBoxEditingControlAccessibleObject : TextBoxBaseAccessibleObject
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

        private protected override bool IsInternal => true;

        public override string Name => this.GetOwnerAccessibleName(SR.DataGridView_AccEditingControlAccName);

        internal override bool CanGetNameInternal => false;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            switch (direction)
            {
                case NavigateDirection.NavigateDirection_Parent:
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

        internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot =>
            this.TryGetOwnerAs(out IDataGridViewEditingControl? owner)
                ? owner.EditingControlDataGridView?.AccessibilityObject
                : null;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                // If we don't set a default role for the accessible object it will be retrieved from Windows.
                // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId when this.GetOwnerAccessibleRole() == AccessibleRole.Default
                    => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_EditControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };

        /// <summary>
        ///  Sets the parent accessible object for the node which can be added or removed to/from hierarchy nodes.
        /// </summary>
        /// <param name="parent">The parent accessible object.</param>
        internal override void SetParent(AccessibleObject? parent) => _parentAccessibleObject = parent;
    }
}

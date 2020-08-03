// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    partial class DataGridViewTextBoxEditingControl
    {
        /// <summary>
        ///  Defines the DataGridView TextBox EditingControl accessible object.
        /// </summary>
        internal class DataGridViewTextBoxEditingControlAccessibleObject : Control.ControlAccessibleObject
        {
            /// <summary>
            ///  The parent is changed when the editing control is attached to another editing cell.
            /// </summary>
            private AccessibleObject? _parentAccessibleObject;

            public DataGridViewTextBoxEditingControlAccessibleObject(DataGridViewTextBoxEditingControl ownerControl) : base(ownerControl)
            {
            }

            public override AccessibleObject? Parent => _parentAccessibleObject;

            public override string? Name
            {
                get => Owner.AccessibleName ?? SR.DataGridView_AccEditingControlAccName;
                set => base.Name = value;
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.Parent => (Owner is IDataGridViewEditingControl owner && owner.EditingControlDataGridView.EditingControl == owner)
                                               ? _parentAccessibleObject
                                               : null,
                    _ => base.FragmentNavigate(direction),
                };

            internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot
                => (Owner as IDataGridViewEditingControl)?.EditingControlDataGridView?.AccessibilityObject;

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.EditControlTypeId,
                    UiaCore.UIA.NamePropertyId => Name,
                    UiaCore.UIA.IsValuePatternAvailablePropertyId => true,
                    _ => base.GetPropertyValue(propertyID),
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.ValuePatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };

            /// <summary>
            ///  Sets the parent accessible object for the node which can be added or removed to/from hierachy nodes.
            /// </summary>
            /// <param name="parent">The parent accessible object.</param>
            internal override void SetParent(AccessibleObject? parent) => _parentAccessibleObject = parent;
        }
    }
}

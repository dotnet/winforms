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
            private readonly TextBoxBase _owningDataGridViewTextBoxEditingControl;
            private readonly TextBoxBaseUiaTextProvider _textProvider;

            public DataGridViewTextBoxEditingControlAccessibleObject(DataGridViewTextBoxEditingControl ownerControl) : base(ownerControl)
            {
                _owningDataGridViewTextBoxEditingControl = ownerControl;
                _textProvider = new TextBoxBaseUiaTextProvider(ownerControl);
                UseTextProviders(_textProvider, _textProvider);
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
                    UiaCore.UIA.IsTextPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.TextPatternId),
                    UiaCore.UIA.IsTextPattern2AvailablePropertyId => IsPatternSupported(UiaCore.UIA.TextPattern2Id),
                    UiaCore.UIA.IsValuePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.ValuePatternId),
                    _ => base.GetPropertyValue(propertyID),
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.ValuePatternId => true,
                    UiaCore.UIA.TextPatternId => true,
                    UiaCore.UIA.TextPattern2Id => true,
                    _ => base.IsPatternSupported(patternId)
                };

            internal override bool IsReadOnly => _owningDataGridViewTextBoxEditingControl.ReadOnly;

            /// <summary>
            ///  Sets the parent accessible object for the node which can be added or removed to/from hierachy nodes.
            /// </summary>
            /// <param name="parent">The parent accessible object.</param>
            internal override void SetParent(AccessibleObject? parent) => _parentAccessibleObject = parent;
        }
    }
}

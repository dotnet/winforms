// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

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
            private readonly DataGridViewTextBoxEditingControl ownerControl;

            /// <summary>
            ///  The parent is changed when the editing control is attached to another editing cell.
            /// </summary>
            private AccessibleObject _parentAccessibleObject;

            public DataGridViewTextBoxEditingControlAccessibleObject(DataGridViewTextBoxEditingControl ownerControl) : base(ownerControl)
            {
                this.ownerControl = ownerControl;
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return _parentAccessibleObject;
                }
            }

            public override string Name
            {
                get
                {
                    string name = Owner.AccessibleName;
                    if (name != null)
                    {
                        return name;
                    }
                    else
                    {
                        return SR.DataGridView_AccEditingControlAccName;
                    }
                }
                set => base.Name = value;
            }

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        if (Owner is IDataGridViewEditingControl owner && owner.EditingControlDataGridView.EditingControl == owner)
                        {
                            return _parentAccessibleObject;
                        }

                        return null;
                }

                return base.FragmentNavigate(direction);
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return (Owner as IDataGridViewEditingControl)?.EditingControlDataGridView?.AccessibilityObject;
                }
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.EditControlTypeId;
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.IsValuePatternAvailablePropertyId:
                        return true;
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.ValuePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            /// <summary>
            ///  Sets the parent accessible object for the node which can be added or removed to/from hierachy nodes.
            /// </summary>
            /// <param name="parent">The parent accessible object.</param>
            internal override void SetParent(AccessibleObject parent)
            {
                _parentAccessibleObject = parent;
            }
        }
    }
}

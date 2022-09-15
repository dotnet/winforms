// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class Form
    {
        /// <summary>
        ///  Form control accessible object with UI Automation provider functionality.
        ///  This inherits from the base ControlAccessibleObject
        ///  to have all base functionality.
        /// </summary>
        internal class FormAccessibleObject : ControlAccessibleObject
        {
            private readonly Form _owner;

            internal FormAccessibleObject(Form owner) : base(owner)
            {
                _owner = owner;
            }

            public override Rectangle Bounds => _owner.IsHandleCreated ? _owner.RectangleToScreen(_owner.ClientRectangle) : Rectangle.Empty;

            internal override Rectangle BoundingRectangle
            {
                get
                {
                    if (!_owner.IsHandleCreated)
                    {
                        return Rectangle.Empty;
                    }

                    if (_owner.Parent is null)
                    {
                        // If the Forms is a main window and a root object. 
                        return _owner.Bounds;
                    }

                    // If the Form is placed on another control, eg. Form or Panel.
                    return _owner.Parent.RectangleToScreen(_owner.Bounds);
                }
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    // Unlike other controls, here the default "ControlType" doesn't correspond the value from the mapping
                    // depending on the default Role.
                    // In other cases "ControlType" will reflect changes to Form.AccessibleRole (i.e. if it is set to a custom role).
                    UiaCore.UIA.ControlTypePropertyId when
                        Role == AccessibleRole.Client
                        => UiaCore.UIA.WindowControlTypeId,
                    UiaCore.UIA.IsDialogPropertyId => _owner.Modal,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsIAccessibleExSupported() => true;

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    return role != AccessibleRole.Default
                        ? role
                        : AccessibleRole.Client;
                }
            }
        }
    }
}

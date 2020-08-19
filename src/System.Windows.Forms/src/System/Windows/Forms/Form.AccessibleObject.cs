// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

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

            internal override Rectangle BoundingRectangle => _owner.IsHandleCreated ? _owner.Bounds : Rectangle.Empty;

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                return propertyID == UiaCore.UIA.NamePropertyId
                    ? Name
                    : base.GetPropertyValue(propertyID);
            }

            internal override bool IsIAccessibleExSupported()
            {
                if (_owner != null)
                {
                    return true;
                }

                return base.IsIAccessibleExSupported();
            }

            internal override void SetValue(string newValue)
            {
                Value = newValue;
            }
        }
    }
}

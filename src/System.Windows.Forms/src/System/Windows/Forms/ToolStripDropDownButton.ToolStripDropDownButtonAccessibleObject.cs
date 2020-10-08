// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripDropDownButton
    {
        /// <summary>
        ///  An implementation of Accessibleobject for use with ToolStripDropDownButton
        /// </summary>
        internal class ToolStripDropDownButtonAccessibleObject : ToolStripDropDownItemAccessibleObject
        {
            private readonly ToolStripDropDownButton _owningToolStripDropDownButton;

            public ToolStripDropDownButtonAccessibleObject(ToolStripDropDownButton ownerItem)
                : base(ownerItem)
            {
                _owningToolStripDropDownButton = ownerItem;
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                if (propertyID == UiaCore.UIA.ControlTypePropertyId)
                {
                    return UiaCore.UIA.ButtonControlTypeId;
                }
                else
                {
                    return base.GetPropertyValue(propertyID);
                }
            }
        }
    }
}

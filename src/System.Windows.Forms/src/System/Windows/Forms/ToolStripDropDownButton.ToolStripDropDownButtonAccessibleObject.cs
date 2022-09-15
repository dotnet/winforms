// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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

            internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    // ToolStripDropDownItemAccessibleObject implements a default Role as MenuItem
                    // because of this, ToolStripItemAccessibleObject will return the unexpected result for this.
                    // Return Button as the expected value by default
                    UiaCore.UIA.ControlTypePropertyId when
                        _owningToolStripDropDownButton.AccessibleRole == AccessibleRole.Default
                        => UiaCore.UIA.ButtonControlTypeId,
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}

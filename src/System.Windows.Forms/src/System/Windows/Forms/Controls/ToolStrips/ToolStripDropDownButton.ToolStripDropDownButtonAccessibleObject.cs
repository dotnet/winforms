// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

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

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                // ToolStripDropDownItemAccessibleObject implements a default Role as MenuItem
                // because of this, ToolStripItemAccessibleObject will return the unexpected result for this.
                // Return Button as the expected value by default
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId when
                    _owningToolStripDropDownButton.AccessibleRole == AccessibleRole.Default
                    => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class DataGridViewTextBoxCell
{
    protected class DataGridViewTextBoxCellAccessibleObject : DataGridViewCellAccessibleObject
    {
        public DataGridViewTextBoxCellAccessibleObject(DataGridViewCell? owner) : base(owner)
        {
        }

        internal override bool IsIAccessibleExSupported() => true;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_DataItemControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };
    }
}

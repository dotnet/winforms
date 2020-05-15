// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridViewTextBoxCell
    {
        protected class DataGridViewTextBoxCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            public DataGridViewTextBoxCellAccessibleObject(DataGridViewCell owner) : base(owner)
            {
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                if (propertyID == UiaCore.UIA.ControlTypePropertyId)
                {
                    return UiaCore.UIA.EditControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}

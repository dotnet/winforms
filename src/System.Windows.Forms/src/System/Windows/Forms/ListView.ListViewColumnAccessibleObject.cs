// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop.UiaCore;

namespace System.Windows.Forms
{
    public partial class ListView
    {
        internal class ListViewColumnAccessibleObject : AccessibleObject
        {
            private ColumnHeader _columnHeader;

            public ListViewColumnAccessibleObject(ColumnHeader columnHeader)
            {
                _columnHeader = columnHeader;
            }

            internal override object GetPropertyValue(UIA propertyID)
            {
                switch (propertyID)
                {
                    case UIA.ControlTypePropertyId:
                        return UIA.HeaderItemControlTypeId;
                    case UIA.NamePropertyId:
                        return _columnHeader?.Text ?? string.Empty;
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }
        }
    }
}

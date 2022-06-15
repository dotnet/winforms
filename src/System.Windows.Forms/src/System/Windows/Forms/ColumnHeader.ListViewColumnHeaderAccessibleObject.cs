// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.UiaCore;

namespace System.Windows.Forms
{
    public partial class ColumnHeader
    {
        internal class ListViewColumnHeaderAccessibleObject : AccessibleObject
        {
            private readonly ColumnHeader _owningColumnHeader;

            public ListViewColumnHeaderAccessibleObject(ColumnHeader columnHeader)
            {
                _owningColumnHeader = columnHeader.OrThrowIfNull();
            }

            public override string? Name => _owningColumnHeader.Text;

            internal override int[] RuntimeId => new int[] { RuntimeIDFirstItem, _owningColumnHeader.GetHashCode() };

            internal override object? GetPropertyValue(UIA propertyID)
                => propertyID switch
                {
                    UIA.ControlTypePropertyId => UIA.HeaderItemControlTypeId,
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}

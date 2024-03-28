// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ColumnHeader
{
    internal sealed class ListViewColumnHeaderAccessibleObject : AccessibleObject
    {
        private readonly ColumnHeader _owningColumnHeader;

        public ListViewColumnHeaderAccessibleObject(ColumnHeader columnHeader)
        {
            _owningColumnHeader = columnHeader.OrThrowIfNull();
        }

        public override string? Name => _owningColumnHeader.Text;

        private protected override bool IsInternal => true;

        internal override bool CanGetNameInternal => false;

        internal override int[] RuntimeId => [RuntimeIDFirstItem, _owningColumnHeader.GetHashCode()];

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_HeaderItemControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };
    }
}

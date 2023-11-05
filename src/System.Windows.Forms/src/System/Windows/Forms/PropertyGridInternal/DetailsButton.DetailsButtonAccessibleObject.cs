// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class DetailsButton
{
    internal class DetailsButtonAccessibleObject : ControlAccessibleObject
    {
        private readonly DetailsButton _ownerItem;

        public DetailsButtonAccessibleObject(DetailsButton owner) : base(owner)
        {
            _ownerItem = owner;
        }

        internal override bool IsIAccessibleExSupported()
        {
            Debug.Assert(_ownerItem is not null, "AccessibleObject owner cannot be null");
            return true;
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID == UIA_PROPERTY_ID.UIA_ControlTypePropertyId
                ? (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId
                : base.GetPropertyValue(propertyID);

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId == UIA_PATTERN_ID.UIA_ExpandCollapsePatternId || base.IsPatternSupported(patternId);

        internal override ExpandCollapseState ExpandCollapseState
            => _ownerItem.Expanded
                ? ExpandCollapseState.ExpandCollapseState_Expanded
                : ExpandCollapseState.ExpandCollapseState_Collapsed;

        internal override void Expand()
        {
            if (_ownerItem is not null && !_ownerItem.Expanded)
            {
                DoDefaultAction();
            }
        }

        internal override void Collapse()
        {
            if (_ownerItem is not null && _ownerItem.Expanded)
            {
                DoDefaultAction();
            }
        }
    }
}

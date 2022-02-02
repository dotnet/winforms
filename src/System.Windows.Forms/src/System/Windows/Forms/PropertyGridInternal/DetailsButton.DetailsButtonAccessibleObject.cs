// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class DetailsButton
    {
        internal class DetailsButtonAccessibleObject : Control.ControlAccessibleObject
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

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID == UiaCore.UIA.ControlTypePropertyId
                    ? UiaCore.UIA.ButtonControlTypeId
                    : base.GetPropertyValue(propertyID);

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId == UiaCore.UIA.ExpandCollapsePatternId || base.IsPatternSupported(patternId);

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
                => _ownerItem.Expanded
                    ? UiaCore.ExpandCollapseState.Expanded
                    : UiaCore.ExpandCollapseState.Collapsed;

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
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class DetailsButton
    {
        internal class DetailsButtonAccessibleObject : Control.ControlAccessibleObject
        {
            private readonly DetailsButton ownerItem;

            public DetailsButtonAccessibleObject(DetailsButton owner) : base(owner)
            {
                ownerItem = owner;
            }

            internal override bool IsIAccessibleExSupported()
            {
                Debug.Assert(ownerItem is not null, "AccessibleObject owner cannot be null");
                return true;
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

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.ExpandCollapsePatternId)
                {
                    return true;
                }
                else
                {
                    return base.IsPatternSupported(patternId);
                }
            }

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
            {
                get
                {
                    return ownerItem.Expanded ? UiaCore.ExpandCollapseState.Expanded : UiaCore.ExpandCollapseState.Collapsed;
                }
            }

            internal override void Expand()
            {
                if (ownerItem is not null && !ownerItem.Expanded)
                {
                    DoDefaultAction();
                }
            }

            internal override void Collapse()
            {
                if (ownerItem is not null && ownerItem.Expanded)
                {
                    DoDefaultAction();
                }
            }
        }
    }
}

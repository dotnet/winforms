// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripSplitButton
    {
        internal class ToolStripSplitButtonExAccessibleObject : ToolStripSplitButtonAccessibleObject
        {
            private readonly ToolStripSplitButton _ownerItem;

            public ToolStripSplitButtonExAccessibleObject(ToolStripSplitButton item)
                : base(item)
            {
                _ownerItem = item;
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

            internal override bool IsIAccessibleExSupported()
            {
                if (_ownerItem != null)
                {
                    return true;
                }
                else
                {
                    return base.IsIAccessibleExSupported();
                }
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.ExpandCollapsePatternId && _ownerItem.HasDropDownItems)
                {
                    return true;
                }
                else
                {
                    return base.IsPatternSupported(patternId);
                }
            }

            internal override void Expand()
            {
                DoDefaultAction();
            }

            internal override void Collapse()
            {
                if (_ownerItem != null && _ownerItem.DropDown != null && _ownerItem.DropDown.Visible)
                {
                    _ownerItem.DropDown.Close();
                }
            }

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
            {
                get
                {
                    return _ownerItem.DropDown.Visible ? UiaCore.ExpandCollapseState.Expanded : UiaCore.ExpandCollapseState.Collapsed;
                }
            }

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                        return DropDownItemsCount > 0 ? _ownerItem.DropDown.Items[0].AccessibilityObject : null;
                    case UiaCore.NavigateDirection.LastChild:
                        return DropDownItemsCount > 0 ? _ownerItem.DropDown.Items[_ownerItem.DropDown.Items.Count - 1].AccessibilityObject : null;
                }
                return base.FragmentNavigate(direction);
            }

            private int DropDownItemsCount
            {
                get
                {
                    // Do not expose child items when the drop-down is collapsed to prevent Narrator from announcing
                    // invisible menu items when Narrator is in item's mode (CAPSLOCK + Arrow Left/Right) or
                    // in scan mode (CAPSLOCK + Space)
                    if (ExpandCollapseState == UiaCore.ExpandCollapseState.Collapsed)
                    {
                        return 0;
                    }

                    return _ownerItem.DropDownItems.Count;
                }
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripSplitButton
    {
        internal class ToolStripSplitButtonExAccessibleObject : ToolStripSplitButtonAccessibleObject
        {
            private readonly ToolStripSplitButton _owningToolStripSplitButton;

            public ToolStripSplitButtonExAccessibleObject(ToolStripSplitButton item)
                : base(item)
            {
                _owningToolStripSplitButton = item;
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                // If we don't set a default role for the accessible object
                // it will be retrieved from Windows.
                // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                if (propertyID == UiaCore.UIA.ControlTypePropertyId && _owningToolStripSplitButton.AccessibleRole == AccessibleRole.Default)
                {
                    return UiaCore.UIA.ButtonControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool IsIAccessibleExSupported()
            {
                if (_owningToolStripSplitButton is not null)
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
                if (patternId == UiaCore.UIA.ExpandCollapsePatternId && _owningToolStripSplitButton.HasDropDownItems)
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
                if (_owningToolStripSplitButton is not null && _owningToolStripSplitButton.DropDown is not null && _owningToolStripSplitButton.DropDown.Visible)
                {
                    _owningToolStripSplitButton.DropDown.Close();
                }
            }

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
            {
                get
                {
                    return _owningToolStripSplitButton.DropDown.Visible ? UiaCore.ExpandCollapseState.Expanded : UiaCore.ExpandCollapseState.Collapsed;
                }
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                        return DropDownItemsCount > 0 ? _owningToolStripSplitButton.DropDown.Items[0].AccessibilityObject : null;
                    case UiaCore.NavigateDirection.LastChild:
                        return DropDownItemsCount > 0 ? _owningToolStripSplitButton.DropDown.Items[_owningToolStripSplitButton.DropDown.Items.Count - 1].AccessibilityObject : null;
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

                    return _owningToolStripSplitButton.DropDownItems.Count;
                }
            }
        }
    }
}

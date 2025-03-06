// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

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

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                // If we don't set a default role for the accessible object
                // it will be retrieved from Windows.
                // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId when
                    _owningToolStripSplitButton.AccessibleRole == AccessibleRole.Default
                    => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };

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

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
        {
            if (patternId == UIA_PATTERN_ID.UIA_ExpandCollapsePatternId && _owningToolStripSplitButton.HasDropDownItems)
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

        internal override ExpandCollapseState ExpandCollapseState
        {
            get
            {
                return _owningToolStripSplitButton.DropDown.Visible ? ExpandCollapseState.ExpandCollapseState_Expanded : ExpandCollapseState.ExpandCollapseState_Collapsed;
            }
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_FirstChild => DropDownItemsCount > 0 ? _owningToolStripSplitButton.DropDown.Items[0].AccessibilityObject : null,
                NavigateDirection.NavigateDirection_LastChild => DropDownItemsCount > 0 ? _owningToolStripSplitButton.DropDown.Items[_owningToolStripSplitButton.DropDown.Items.Count - 1].AccessibilityObject : null,
                _ => base.FragmentNavigate(direction),
            };

        private int DropDownItemsCount
        {
            get
            {
                // Do not expose child items when the drop-down is collapsed to prevent Narrator from announcing
                // invisible menu items when Narrator is in item's mode (CAPSLOCK + Arrow Left/Right) or
                // in scan mode (CAPSLOCK + Space)
                if (ExpandCollapseState == ExpandCollapseState.ExpandCollapseState_Collapsed)
                {
                    return 0;
                }

                return _owningToolStripSplitButton.DropDownItems.Count;
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Layout;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public class ToolStripDropDownItemAccessibleObject : ToolStripItem.ToolStripItemAccessibleObject
{
    private readonly ToolStripDropDownItem _owner;

    public ToolStripDropDownItemAccessibleObject(ToolStripDropDownItem item) : base(item)
    {
        _owner = item;
    }

    public override AccessibleRole Role
    {
        get
        {
            AccessibleRole role = Owner.AccessibleRole;
            if (role != AccessibleRole.Default)
            {
                return role;
            }

            return AccessibleRole.MenuItem;
        }
    }

    public override void DoDefaultAction()
    {
        if (Owner is ToolStripDropDownItem item && item.HasDropDownItems)
        {
            item.ShowDropDown();
        }
        else
        {
            base.DoDefaultAction();
        }
    }

    internal override bool IsIAccessibleExSupported()
    {
        return true;
    }

    internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
    {
        if (patternId == UIA_PATTERN_ID.UIA_ExpandCollapsePatternId && _owner.HasDropDownItems)
        {
            return true;
        }
        else
        {
            return base.IsPatternSupported(patternId);
        }
    }

    internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
        propertyID switch
        {
            UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId when
                _owner.Owner is ToolStripDropDown toolStripDropDown
                => (VARIANT)!toolStripDropDown.Visible,
            _ => base.GetPropertyValue(propertyID)
        };

    internal override void Expand()
        => DoDefaultAction();

    internal override void Collapse()
    {
        if (_owner.DropDown.Visible)
        {
            _owner.DropDown.Close();
        }
    }

    internal override ExpandCollapseState ExpandCollapseState
    {
        get
        {
            return _owner.DropDown.Visible ? ExpandCollapseState.ExpandCollapseState_Expanded : ExpandCollapseState.ExpandCollapseState_Collapsed;
        }
    }

    public override AccessibleObject? GetChild(int index)
    {
        if (!_owner.HasDropDownItems)
        {
            return null;
        }

        return _owner.DropDown.AccessibilityObject.GetChild(index);
    }

    public override int GetChildCount()
    {
        if (!_owner.HasDropDownItems)
        {
            return -1;
        }

        // Do not expose child items when the submenu is collapsed to prevent Narrator from announcing
        // invisible menu items when Narrator is in item's mode (CAPSLOCK + Arrow Left/Right) or
        // in scan mode (CAPSLOCK + Space)
        if (ExpandCollapseState == ExpandCollapseState.ExpandCollapseState_Collapsed)
        {
            return 0;
        }

        if (_owner.DropDown.LayoutRequired)
        {
            LayoutTransaction.DoLayout(_owner.DropDown, _owner.DropDown, PropertyNames.Items);
        }

        return _owner.DropDown.AccessibilityObject.GetChildCount();
    }

    internal int GetChildFragmentIndex(ToolStripItem.ToolStripItemAccessibleObject child)
    {
        if (_owner.DropDownItems is null)
        {
            return -1;
        }

        for (int i = 0; i < _owner.DropDownItems.Count; i++)
        {
            if (_owner.DropDownItems[i].Available && child.Owner == _owner.DropDownItems[i])
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    ///  Gets the number of children belonging to an accessible object.
    /// </summary>
    /// <returns>The number of children.</returns>
    internal int GetChildFragmentCount()
    {
        if (_owner.DropDownItems is null)
        {
            return -1;
        }

        int count = 0;
        for (int i = 0; i < _owner.DropDownItems.Count; i++)
        {
            if (_owner.DropDownItems[i].Available)
            {
                count++;
            }
        }

        return count;
    }

    internal AccessibleObject? GetChildFragment(int index, NavigateDirection direction)
    {
        if (_owner.DropDown.AccessibilityObject is ToolStrip.ToolStripAccessibleObject toolStripAccessibleObject)
        {
            return toolStripAccessibleObject.GetChildFragment(index, direction);
        }

        return null;
    }

    internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
    {
        switch (direction)
        {
            case NavigateDirection.NavigateDirection_NextSibling:
            case NavigateDirection.NavigateDirection_PreviousSibling:
                if (_owner.Owner is not ToolStripDropDown dropDown)
                {
                    break;
                }

                int index = dropDown.DisplayedItems.IndexOf(_owner);

                if (index == -1)
                {
                    Debug.Fail("No item matched the index?");
                    return null;
                }

                index += direction == NavigateDirection.NavigateDirection_NextSibling ? 1 : -1;

                if (index >= 0 && index < dropDown.DisplayedItems.Count)
                {
                    ToolStripItem item = dropDown.DisplayedItems[index];
                    if (item is ToolStripControlHost controlHostItem)
                    {
                        return controlHostItem.ControlAccessibilityObject;
                    }

                    return item.AccessibilityObject;
                }

                return null;
            case NavigateDirection.NavigateDirection_FirstChild:
            case NavigateDirection.NavigateDirection_LastChild:
                // Don't add invisible items to the accessibility tree,
                // they might not have been created yet.
                return _owner.DropDown.Visible
                    ? _owner.DropDown.AccessibilityObject
                    : null;
        }

        return base.FragmentNavigate(direction);
    }
}

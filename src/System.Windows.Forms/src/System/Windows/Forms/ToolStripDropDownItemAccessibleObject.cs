// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms
{
    public class ToolStripDropDownItemAccessibleObject : ToolStripItem.ToolStripItemAccessibleObject
    {
        private readonly ToolStripDropDownItem owner;
        public ToolStripDropDownItemAccessibleObject(ToolStripDropDownItem item) : base(item)
        {
            owner = item;
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
            if (owner is not null)
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
            if (patternId == UiaCore.UIA.ExpandCollapsePatternId && owner.HasDropDownItems)
            {
                return true;
            }
            else
            {
                return base.IsPatternSupported(patternId);
            }
        }

        internal override object GetPropertyValue(UiaCore.UIA propertyID)
        {
            if (propertyID == UiaCore.UIA.IsOffscreenPropertyId && owner is not null && owner.Owner is ToolStripDropDown)
            {
                return !((ToolStripDropDown)owner.Owner).Visible;
            }

            return base.GetPropertyValue(propertyID);
        }

        internal override void Expand()
            => DoDefaultAction();

        internal override void Collapse()
        {
            if (owner is not null && owner.DropDown is not null && owner.DropDown.Visible)
            {
                owner.DropDown.Close();
            }
        }

        internal override UiaCore.ExpandCollapseState ExpandCollapseState
        {
            get
            {
                return owner.DropDown.Visible ? UiaCore.ExpandCollapseState.Expanded : UiaCore.ExpandCollapseState.Collapsed;
            }
        }

        public override AccessibleObject GetChild(int index)
        {
            if ((owner is null) || !owner.HasDropDownItems)
            {
                return null;
            }

            return owner.DropDown.AccessibilityObject.GetChild(index);
        }

        public override int GetChildCount()
        {
            if ((owner is null) || !owner.HasDropDownItems)
            {
                return -1;
            }

            // Do not expose child items when the submenu is collapsed to prevent Narrator from announcing
            // invisible menu items when Narrator is in item's mode (CAPSLOCK + Arrow Left/Right) or
            // in scan mode (CAPSLOCK + Space)
            if (ExpandCollapseState == UiaCore.ExpandCollapseState.Collapsed)
            {
                return 0;
            }

            if (owner.DropDown.LayoutRequired)
            {
                LayoutTransaction.DoLayout(owner.DropDown, owner.DropDown, PropertyNames.Items);
            }

            return owner.DropDown.AccessibilityObject.GetChildCount();
        }

        internal int GetChildFragmentIndex(ToolStripItem.ToolStripItemAccessibleObject child)
        {
            if ((owner is null) || (owner.DropDownItems is null))
            {
                return -1;
            }

            for (int i = 0; i < owner.DropDownItems.Count; i++)
            {
                if (owner.DropDownItems[i].Available && child.Owner == owner.DropDownItems[i])
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
            if ((owner is null) || (owner.DropDownItems is null))
            {
                return -1;
            }

            int count = 0;
            for (int i = 0; i < owner.DropDownItems.Count; i++)
            {
                if (owner.DropDownItems[i].Available)
                {
                    count++;
                }
            }

            return count;
        }

        internal AccessibleObject GetChildFragment(int index, UiaCore.NavigateDirection direction)
        {
            if (owner.DropDown.AccessibilityObject is ToolStrip.ToolStripAccessibleObject toolStripAccessibleObject)
            {
                return toolStripAccessibleObject.GetChildFragment(index, direction);
            }

            return null;
        }

        internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            if (owner is null || owner.DropDown is null)
            {
                return null;
            }

            switch (direction)
            {
                case UiaCore.NavigateDirection.NextSibling:
                case UiaCore.NavigateDirection.PreviousSibling:
                    if (!(owner.Owner is ToolStripDropDown dropDown))
                    {
                        break;
                    }

                    int index = dropDown.Items.IndexOf(owner);

                    if (index == -1)
                    {
                        Debug.Fail("No item matched the index?");
                        return null;
                    }

                    index += direction == UiaCore.NavigateDirection.NextSibling ? 1 : -1;

                    if (index >= 0 && index < dropDown.Items.Count)
                    {
                        ToolStripItem item = dropDown.Items[index];
                        if (item is ToolStripControlHost controlHostItem)
                        {
                            return controlHostItem.ControlAccessibilityObject;
                        }

                        return item.AccessibilityObject;
                    }

                    return null;
            }

            return base.FragmentNavigate(direction);
        }
    }
}

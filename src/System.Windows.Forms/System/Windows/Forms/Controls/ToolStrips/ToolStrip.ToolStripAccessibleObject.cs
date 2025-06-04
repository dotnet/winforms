// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ToolStrip
{
    public class ToolStripAccessibleObject : ControlAccessibleObject
    {
        public ToolStripAccessibleObject(ToolStrip owner) : base(owner)
        {
        }

        internal override IRawElementProviderFragment.Interface? ElementProviderFromPoint(double x, double y)
            => this.IsOwnerHandleCreated(out ToolStrip? _) ? HitTest((int)x, (int)y) : null;

        /// <summary>
        ///  Return the child object at the given screen coordinates.
        /// </summary>
        public override AccessibleObject? HitTest(int x, int y)
        {
            if (!this.IsOwnerHandleCreated(out ToolStrip? owner))
            {
                return null;
            }

            Point clientHit = owner.PointToClient(new Point(x, y));
            ToolStripItem? item = owner.GetItemAt(clientHit);
            return ((item is not null) && (item.AccessibilityObject is not null))
                ? item.AccessibilityObject
                : base.HitTest(x, y);
        }

        /// <summary>
        ///  When overridden in a derived class, gets the accessible child corresponding to the specified
        ///  index.
        /// </summary>
        public override AccessibleObject? GetChild(int index)
        {
            if (!this.TryGetOwnerAs(out ToolStrip? owner) || owner.Items is null)
            {
                return null;
            }

            if (index == 0 && owner.Grip.Visible)
            {
                return owner.Grip.AccessibilityObject;
            }
            else if (owner.Grip.Visible && index > 0)
            {
                index--;
            }

            if (index < owner.Items.Count)
            {
                ToolStripItem? item = null;
                int myIndex = 0;

                // First we walk through the head aligned items.
                for (int i = 0; i < owner.Items.Count; ++i)
                {
                    if (owner.Items[i].Available && owner.Items[i].Alignment == ToolStripItemAlignment.Left)
                    {
                        if (myIndex == index)
                        {
                            item = owner.Items[i];
                            break;
                        }

                        myIndex++;
                    }
                }

                // If we didn't find it, then we walk through the tail aligned items.
                if (item is null)
                {
                    for (int i = 0; i < owner.Items.Count; ++i)
                    {
                        if (owner.Items[i].Available && owner.Items[i].Alignment == ToolStripItemAlignment.Right)
                        {
                            if (myIndex == index)
                            {
                                item = owner.Items[i];
                                break;
                            }

                            myIndex++;
                        }
                    }
                }

                if (item is null)
                {
                    Debug.Fail("No item matched the index??");
                    return null;
                }

                if (item.Placement == ToolStripItemPlacement.Overflow)
                {
                    return new ToolStripAccessibleObjectWrapperForItemsOnOverflow(item);
                }

                return item.AccessibilityObject;
            }

            if (owner.CanOverflow && owner.OverflowButton.Visible && index == owner.Items.Count)
            {
                return owner.OverflowButton.AccessibilityObject;
            }

            return null;
        }

        /// <summary>
        ///  When overridden in a derived class, gets the number of children
        ///  belonging to an accessible object.
        /// </summary>
        public override int GetChildCount()
        {
            if (!this.TryGetOwnerAs(out ToolStrip? owner) || owner.Items is null)
            {
                return -1;
            }

            int count = 0;
            for (int i = 0; i < owner.Items.Count; i++)
            {
                if (owner.Items[i].Available)
                {
                    count++;
                }
            }

            if (owner.Grip.Visible)
            {
                count++;
            }

            if (owner.CanOverflow && owner.OverflowButton.Visible)
            {
                count++;
            }

            return count;
        }

        internal AccessibleObject? GetChildFragment(int fragmentIndex, NavigateDirection direction, bool getOverflowItem = false)
        {
            if (!this.TryGetOwnerAs(out ToolStrip? owner) || fragmentIndex < 0)
            {
                return null;
            }

            ToolStripItemCollection items = getOverflowItem
                ? owner.OverflowItems
                : owner.DisplayedItems;

            if (!getOverflowItem
                && owner.CanOverflow
                && owner.OverflowButton.Visible
                && fragmentIndex == items.Count - 1)
            {
                return owner.OverflowButton.AccessibilityObject;
            }

            if (fragmentIndex < items.Count)
            {
                ToolStripItem item = items[fragmentIndex];
                if (item.Available)
                {
                    return GetItemAccessibleObject(item);
                }
            }

            return null;

            AccessibleObject? GetItemAccessibleObject(ToolStripItem item)
            {
                if (item is ToolStripControlHost controlHostItem and not ToolStripScrollButton)
                {
                    if (ShouldItemBeSkipped(controlHostItem.Control))
                    {
                        return GetFollowingChildFragment(fragmentIndex, items, direction);
                    }

                    return controlHostItem.ControlAccessibilityObject;
                }

                return item.AccessibilityObject;
            }

            bool ShouldItemBeSkipped(Control hostedControl)
                => hostedControl is null
                    || !hostedControl.SupportsUiaProviders
                    || (hostedControl is Label label && string.IsNullOrEmpty(label.Text));

            // Returns the next or the previous ToolStrip item, that is considered a valid navigation fragment
            //  (e.g. a control, that supports UIA providers and not a ToolStripControlHost).
            //  This method removes hosted ToolStrip items that are native controls
            //  (their accessible objects are provided by Windows), from the accessibility tree.
            //  It's necessary, because hosted native controls internally add accessible objects
            //  to the accessibility tree, and thus create duplicated. To avoid duplicates,
            //  remove hosted items with native accessibility objects from the tree.
            AccessibleObject? GetFollowingChildFragment(int index, ToolStripItemCollection items, NavigateDirection direction)
            {
                switch (direction)
                {
                    // "direction" is not used for navigation. This method is helper only.
                    // FirstChild, LastChild are used when searching non-native hosted child items of the ToolStrip.
                    // NextSibling, PreviousSibling are used when searching an item siblings.
                    case NavigateDirection.NavigateDirection_FirstChild:
                    case NavigateDirection.NavigateDirection_NextSibling:
                        for (int i = index + 1; i < items.Count; i++)
                        {
                            ToolStripItem item = items[i];
                            if (item is ToolStripControlHost controlHostItem)
                            {
                                if (ShouldItemBeSkipped(controlHostItem.Control))
                                {
                                    continue;
                                }

                                return controlHostItem.ControlAccessibilityObject;
                            }

                            return item.AccessibilityObject;
                        }

                        break;

                    case NavigateDirection.NavigateDirection_LastChild:
                    case NavigateDirection.NavigateDirection_PreviousSibling:
                        for (int i = index - 1; i >= 0; i--)
                        {
                            ToolStripItem item = items[i];
                            if (item is ToolStripControlHost controlHostItem)
                            {
                                if (ShouldItemBeSkipped(controlHostItem.Control))
                                {
                                    continue;
                                }

                                return controlHostItem.ControlAccessibilityObject;
                            }

                            return item.AccessibilityObject;
                        }

                        break;
                }

                return null;
            }
        }

        internal int GetChildOverflowFragmentCount()
        {
            if (!this.TryGetOwnerAs(out ToolStrip? owner) || owner.OverflowItems is null)
            {
                return -1;
            }

            return owner.OverflowItems.Count;
        }

        internal int GetChildFragmentCount()
        {
            if (!this.TryGetOwnerAs(out ToolStrip? owner) || owner.DisplayedItems is null)
            {
                return -1;
            }

            return owner.DisplayedItems.Count;
        }

        internal int GetChildFragmentIndex(ToolStripItem.ToolStripItemAccessibleObject child)
        {
            if (!this.TryGetOwnerAs(out ToolStrip? owner) || owner.Items is null)
            {
                return -1;
            }

            if (child.Owner == owner.Grip)
            {
                return 0;
            }

            ToolStripItemCollection items;
            ToolStripItemPlacement placement = child.Owner.Placement;

            if (owner is ToolStripOverflow overflow)
            {
                // Overflow items in ToolStripOverflow host are in DisplayedItems collection.
                items = overflow.DisplayedItems;
            }
            else
            {
                if (owner.CanOverflow && owner.OverflowButton.Visible && child.Owner == owner.OverflowButton)
                {
                    return GetChildFragmentCount() - 1;
                }

                // Items can be either in DisplayedItems or in OverflowItems (if overflow)
                items = placement == ToolStripItemPlacement.Main || child.Owner is ToolStripScrollButton
                    ? owner.DisplayedItems
                    : owner.OverflowItems;
            }

            for (int index = 0; index < items.Count; index++)
            {
                ToolStripItem item = items[index];
                if (item.Available && child.Owner == item)
                {
                    return index;
                }
            }

            return -1;
        }

        internal int GetChildIndex(ToolStripItem.ToolStripItemAccessibleObject child)
        {
            if (!this.TryGetOwnerAs(out ToolStrip? owner) || owner.Items is null)
            {
                return -1;
            }

            int index = 0;
            if (owner.Grip.Visible)
            {
                if (child.Owner == owner.Grip)
                {
                    return 0;
                }

                index = 1;
            }

            if (owner.CanOverflow && owner.OverflowButton.Visible && child.Owner == owner.OverflowButton)
            {
                return owner.Items.Count + index;
            }

            // First we walk through the head aligned items.
            for (int i = 0; i < owner.Items.Count; ++i)
            {
                if (owner.Items[i].Available && owner.Items[i].Alignment == ToolStripItemAlignment.Left)
                {
                    if (child.Owner == owner.Items[i])
                    {
                        return index;
                    }

                    index++;
                }
            }

            // If we didn't find it, then we walk through the tail aligned items.
            for (int i = 0; i < owner.Items.Count; ++i)
            {
                if (owner.Items[i].Available && owner.Items[i].Alignment == ToolStripItemAlignment.Right)
                {
                    if (child.Owner == owner.Items[i])
                    {
                        return index;
                    }

                    index++;
                }
            }

            return -1;
        }

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.ToolBar);

        internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => this;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!this.IsOwnerHandleCreated(out ToolStrip? _))
            {
                return null;
            }

            switch (direction)
            {
                case NavigateDirection.NavigateDirection_FirstChild:
                    int childCount = GetChildFragmentCount();
                    if (childCount > 0)
                    {
                        return GetChildFragment(0, direction);
                    }

                    break;
                case NavigateDirection.NavigateDirection_LastChild:
                    childCount = GetChildFragmentCount();
                    if (childCount > 0)
                    {
                        return GetChildFragment(childCount - 1, direction);
                    }

                    break;
            }

            return base.FragmentNavigate(direction);
        }
    }
}

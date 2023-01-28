// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStrip
    {
        public class ToolStripAccessibleObject : ControlAccessibleObject
        {
            private readonly ToolStrip _owningToolStrip;

            public ToolStripAccessibleObject(ToolStrip owner) : base(owner)
            {
                _owningToolStrip = owner;
            }

            internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
                => _owningToolStrip.IsHandleCreated ? HitTest((int)x, (int)y) : null;

            /// <summary>
            ///  Return the child object at the given screen coordinates.
            /// </summary>
            public override AccessibleObject? HitTest(int x, int y)
            {
                if (!_owningToolStrip.IsHandleCreated)
                {
                    return null;
                }

                Point clientHit = _owningToolStrip.PointToClient(new Point(x, y));
                ToolStripItem item = _owningToolStrip.GetItemAt(clientHit);
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
                if ((_owningToolStrip is null) || (_owningToolStrip.Items is null))
                {
                    return null;
                }

                if (index == 0 && _owningToolStrip.Grip.Visible)
                {
                    return _owningToolStrip.Grip.AccessibilityObject;
                }
                else if (_owningToolStrip.Grip.Visible && index > 0)
                {
                    index--;
                }

                if (index < _owningToolStrip.Items.Count)
                {
                    ToolStripItem? item = null;
                    int myIndex = 0;

                    // First we walk through the head aligned items.
                    for (int i = 0; i < _owningToolStrip.Items.Count; ++i)
                    {
                        if (_owningToolStrip.Items[i].Available && _owningToolStrip.Items[i].Alignment == ToolStripItemAlignment.Left)
                        {
                            if (myIndex == index)
                            {
                                item = _owningToolStrip.Items[i];
                                break;
                            }

                            myIndex++;
                        }
                    }

                    // If we didn't find it, then we walk through the tail aligned items.
                    if (item is null)
                    {
                        for (int i = 0; i < _owningToolStrip.Items.Count; ++i)
                        {
                            if (_owningToolStrip.Items[i].Available && _owningToolStrip.Items[i].Alignment == ToolStripItemAlignment.Right)
                            {
                                if (myIndex == index)
                                {
                                    item = _owningToolStrip.Items[i];
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

                if (_owningToolStrip.CanOverflow && _owningToolStrip.OverflowButton.Visible && index == _owningToolStrip.Items.Count)
                {
                    return _owningToolStrip.OverflowButton.AccessibilityObject;
                }

                return null;
            }

            /// <summary>
            ///  When overridden in a derived class, gets the number of children
            ///  belonging to an accessible object.
            /// </summary>
            public override int GetChildCount()
            {
                if ((_owningToolStrip is null) || (_owningToolStrip.Items is null))
                {
                    return -1;
                }

                int count = 0;
                for (int i = 0; i < _owningToolStrip.Items.Count; i++)
                {
                    if (_owningToolStrip.Items[i].Available)
                    {
                        count++;
                    }
                }

                if (_owningToolStrip.Grip.Visible)
                {
                    count++;
                }

                if (_owningToolStrip.CanOverflow && _owningToolStrip.OverflowButton.Visible)
                {
                    count++;
                }

                return count;
            }

            internal AccessibleObject? GetChildFragment(int fragmentIndex, UiaCore.NavigateDirection direction, bool getOverflowItem = false)
            {
                if (fragmentIndex < 0)
                {
                    return null;
                }

                ToolStripItemCollection items = getOverflowItem
                    ? _owningToolStrip.OverflowItems
                    : _owningToolStrip.DisplayedItems;

                if (!getOverflowItem
                    && _owningToolStrip.CanOverflow
                    && _owningToolStrip.OverflowButton.Visible
                    && fragmentIndex == items.Count - 1)
                {
                    return _owningToolStrip.OverflowButton.AccessibilityObject;
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

                //  Returns the next or the previous ToolStrip item, that is considered a valid navigation fragment (e.g. a control,
                //  that supports UIA providers and not a ToolStripControlHost). This method removes hosted ToolStrip
                //  items that are native controls (their accessible objects are provided by Windows),
                //  from the accessibility tree. It's necessary, because hosted native controls internally add accessible objects
                //  to the accessibility tree, and thus create duplicated. To avoid duplicates, remove hosted items with native accessibility objects from the tree.
                AccessibleObject? GetFollowingChildFragment(int index, ToolStripItemCollection items, UiaCore.NavigateDirection direction)
                {
                    switch (direction)
                    {
                        // "direction" is not used for navigation. This method is helper only.
                        // FirstChild, LastChild are used when searching non-native hosted child items of the ToolStrip.
                        // NextSibling, PreviousSibling are used when searching an item siblings.
                        case UiaCore.NavigateDirection.FirstChild:
                        case UiaCore.NavigateDirection.NextSibling:
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

                        case UiaCore.NavigateDirection.LastChild:
                        case UiaCore.NavigateDirection.PreviousSibling:
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
                if (_owningToolStrip is null || _owningToolStrip.OverflowItems is null)
                {
                    return -1;
                }

                return _owningToolStrip.OverflowItems.Count;
            }

            internal int GetChildFragmentCount()
            {
                if (_owningToolStrip is null || _owningToolStrip.DisplayedItems is null)
                {
                    return -1;
                }

                return _owningToolStrip.DisplayedItems.Count;
            }

            internal int GetChildFragmentIndex(ToolStripItem.ToolStripItemAccessibleObject child)
            {
                if (_owningToolStrip is null || _owningToolStrip.Items is null)
                {
                    return -1;
                }

                if (child.Owner == _owningToolStrip.Grip)
                {
                    return 0;
                }

                ToolStripItemCollection items;
                ToolStripItemPlacement placement = child.Owner.Placement;

                if (_owningToolStrip is ToolStripOverflow overflow)
                {
                    // Overflow items in ToolStripOverflow host are in DisplayedItems collection.
                    items = overflow.DisplayedItems;
                }
                else
                {
                    if (_owningToolStrip.CanOverflow && _owningToolStrip.OverflowButton.Visible && child.Owner == _owningToolStrip.OverflowButton)
                    {
                        return GetChildFragmentCount() - 1;
                    }

                    // Items can be either in DisplayedItems or in OverflowItems (if overflow)
                    items = placement == ToolStripItemPlacement.Main || child.Owner is ToolStripScrollButton
                        ? _owningToolStrip.DisplayedItems
                        : _owningToolStrip.OverflowItems;
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
                if ((_owningToolStrip is null) || (_owningToolStrip.Items is null))
                {
                    return -1;
                }

                int index = 0;
                if (_owningToolStrip.Grip.Visible)
                {
                    if (child.Owner == _owningToolStrip.Grip)
                    {
                        return 0;
                    }

                    index = 1;
                }

                if (_owningToolStrip.CanOverflow && _owningToolStrip.OverflowButton.Visible && child.Owner == _owningToolStrip.OverflowButton)
                {
                    return _owningToolStrip.Items.Count + index;
                }

                // First we walk through the head aligned items.
                for (int i = 0; i < _owningToolStrip.Items.Count; ++i)
                {
                    if (_owningToolStrip.Items[i].Available && _owningToolStrip.Items[i].Alignment == ToolStripItemAlignment.Left)
                    {
                        if (child.Owner == _owningToolStrip.Items[i])
                        {
                            return index;
                        }

                        index++;
                    }
                }

                // If we didn't find it, then we walk through the tail aligned items.
                for (int i = 0; i < _owningToolStrip.Items.Count; ++i)
                {
                    if (_owningToolStrip.Items[i].Available && _owningToolStrip.Items[i].Alignment == ToolStripItemAlignment.Right)
                    {
                        if (child.Owner == _owningToolStrip.Items[i])
                        {
                            return index;
                        }

                        index++;
                    }
                }

                return -1;
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

                    return AccessibleRole.ToolBar;
                }
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
                => this;

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (!_owningToolStrip.IsHandleCreated)
                {
                    return null;
                }

                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                        int childCount = GetChildFragmentCount();
                        if (childCount > 0)
                        {
                            return GetChildFragment(0, direction);
                        }

                        break;
                    case UiaCore.NavigateDirection.LastChild:
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
}

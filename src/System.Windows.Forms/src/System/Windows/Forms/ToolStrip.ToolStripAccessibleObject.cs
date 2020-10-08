// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStrip
    {
        public class ToolStripAccessibleObject : ControlAccessibleObject
        {
            private readonly ToolStrip owner;

            public ToolStripAccessibleObject(ToolStrip owner) : base(owner)
            {
                this.owner = owner;
            }

            internal override UiaCore.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
                => owner.IsHandleCreated ? HitTest((int)x, (int)y) : null;

            /// <summary>
            ///  Return the child object at the given screen coordinates.
            /// </summary>
            public override AccessibleObject HitTest(int x, int y)
            {
                if (!owner.IsHandleCreated)
                {
                    return null;
                }

                Point clientHit = owner.PointToClient(new Point(x, y));
                ToolStripItem item = owner.GetItemAt(clientHit);
                return ((item != null) && (item.AccessibilityObject != null))
                    ? item.AccessibilityObject
                    : base.HitTest(x, y);
            }

            /// <summary>
            ///  When overridden in a derived class, gets the accessible child corresponding to the specified
            ///  index.
            /// </summary>
            //
            public override AccessibleObject GetChild(int index)
            {
                if ((owner is null) || (owner.Items is null))
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
                    ToolStripItem item = null;
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
                if ((owner is null) || (owner.Items is null))
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

            internal AccessibleObject GetChildFragment(int fragmentIndex, bool getOverflowItem = false)
            {
                ToolStripItemCollection items = getOverflowItem ? owner.OverflowItems : owner.DisplayedItems;
                int childFragmentCount = items.Count;

                if (!getOverflowItem && owner.CanOverflow && owner.OverflowButton.Visible && fragmentIndex == childFragmentCount - 1)
                {
                    return owner.OverflowButton.AccessibilityObject;
                }

                for (int index = 0; index < childFragmentCount; index++)
                {
                    ToolStripItem item = items[index];
                    if (item.Available && item.Alignment == ToolStripItemAlignment.Left && fragmentIndex == index)
                    {
                        if (item is ToolStripControlHost controlHostItem)
                        {
                            return controlHostItem.ControlAccessibilityObject;
                        }

                        return item.AccessibilityObject;
                    }
                }

                for (int index = 0; index < childFragmentCount; index++)
                {
                    ToolStripItem item = owner.Items[index];
                    if (item.Available && item.Alignment == ToolStripItemAlignment.Right && fragmentIndex == index)
                    {
                        if (item is ToolStripControlHost controlHostItem)
                        {
                            return controlHostItem.ControlAccessibilityObject;
                        }

                        return item.AccessibilityObject;
                    }
                }

                return null;
            }

            internal int GetChildOverflowFragmentCount()
            {
                if (owner is null || owner.OverflowItems is null)
                {
                    return -1;
                }

                return owner.OverflowItems.Count;
            }

            internal int GetChildFragmentCount()
            {
                if (owner is null || owner.DisplayedItems is null)
                {
                    return -1;
                }

                return owner.DisplayedItems.Count;
            }

            internal int GetChildFragmentIndex(ToolStripItem.ToolStripItemAccessibleObject child)
            {
                if (owner is null || owner.Items is null)
                {
                    return -1;
                }

                if (child.Owner == owner.Grip)
                {
                    return 0;
                }

                ToolStripItemCollection items;
                ToolStripItemPlacement placement = child.Owner.Placement;

                if (owner is ToolStripOverflow)
                {
                    // Overflow items in ToolStripOverflow host are in DisplayedItems collection.
                    items = owner.DisplayedItems;
                }
                else
                {
                    if (owner.CanOverflow && owner.OverflowButton.Visible && child.Owner == owner.OverflowButton)
                    {
                        return GetChildFragmentCount() - 1;
                    }

                    // Items can be either in DisplayedItems or in OverflowItems (if overflow)
                    items = (placement == ToolStripItemPlacement.Main) ? owner.DisplayedItems : owner.OverflowItems;
                }

                // First we walk through the head aligned items.
                for (int index = 0; index < items.Count; index++)
                {
                    ToolStripItem item = items[index];
                    if (item.Available && item.Alignment == ToolStripItemAlignment.Left && child.Owner == items[index])
                    {
                        return index;
                    }
                }

                // If we didn't find it, then we walk through the tail aligned items.
                for (int index = 0; index < items.Count; index++)
                {
                    ToolStripItem item = items[index];
                    if (item.Available && item.Alignment == ToolStripItemAlignment.Right && child.Owner == items[index])
                    {
                        return index;
                    }
                }

                return -1;
            }

            internal int GetChildIndex(ToolStripItem.ToolStripItemAccessibleObject child)
            {
                if ((owner is null) || (owner.Items is null))
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
            {
                get
                {
                    return this;
                }
            }

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                        int childCount = GetChildFragmentCount();
                        if (childCount > 0)
                        {
                            return GetChildFragment(0);
                        }
                        break;
                    case UiaCore.NavigateDirection.LastChild:
                        childCount = GetChildFragmentCount();
                        if (childCount > 0)
                        {
                            return GetChildFragment(childCount - 1);
                        }
                        break;
                }

                return base.FragmentNavigate(direction);
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.ToolBarControlTypeId;
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}

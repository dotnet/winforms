// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ListViewItem
{
    internal class ListViewItemListAccessibleObject : ListViewItemWithImageAccessibleObject
    {
        public ListViewItemListAccessibleObject(ListViewItem owningItem) : base(owningItem)
        {
        }

        public override Rectangle Bounds
            => !_owningListView.IsHandleCreated
                ? Rectangle.Empty
                : new Rectangle(
                    _owningListView.AccessibilityObject.Bounds.X + _owningItem.Bounds.X,
                    _owningListView.AccessibilityObject.Bounds.Y + _owningItem.Bounds.Y,
                    _owningItem.Bounds.Width,
                    _owningItem.Bounds.Height);

        protected override View View => View.List;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            AccessibleObject _parentInternal = _owningListView.AccessibilityObject;

            switch (direction)
            {
                case NavigateDirection.NavigateDirection_Parent:
                    return _parentInternal;
                case NavigateDirection.NavigateDirection_NextSibling:
                    int childIndex = _parentInternal.GetChildIndex(this);
                    return childIndex == InvalidIndex ? null : _parentInternal.GetChild(childIndex + 1);
                case NavigateDirection.NavigateDirection_PreviousSibling:
                    return _parentInternal.GetChild(_parentInternal.GetChildIndex(this) - 1);
            }

            return base.FragmentNavigate(direction);
        }
    }
}

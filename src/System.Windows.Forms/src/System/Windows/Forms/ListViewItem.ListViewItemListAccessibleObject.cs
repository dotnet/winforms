// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
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

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                AccessibleObject _parentInternal = _owningListView.AccessibilityObject;

                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return _parentInternal;
                    case UiaCore.NavigateDirection.NextSibling:
                        int childIndex = _parentInternal.GetChildIndex(this);
                        return childIndex == InvalidIndex ? null : _parentInternal.GetChild(childIndex + 1);
                    case UiaCore.NavigateDirection.PreviousSibling:
                        return _parentInternal.GetChild(_parentInternal.GetChildIndex(this) - 1);
                }

                return base.FragmentNavigate(direction);
            }
        }
    }
}

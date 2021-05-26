// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Accessibility;
using static System.Windows.Forms.ListView;
using static System.Windows.Forms.ListViewGroup;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ListViewItem
    {
        internal class ListViewItemDetailsAccessibleObject : ListViewItemBaseAccessibleObject
        {
            private readonly Dictionary<int, AccessibleObject> _listViewSubItemAccessibleObjects;

            public ListViewItemDetailsAccessibleObject(ListViewItem owningItem) : base(owningItem)
            {
                _listViewSubItemAccessibleObjects = new Dictionary<int, AccessibleObject>();
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                        return GetChild(0);
                    case UiaCore.NavigateDirection.LastChild:
                        return GetChild(_owningListView.Columns.Count - 1);
                }

                return base.FragmentNavigate(direction);
            }

            // If the ListView does not support ListViewSubItems, the index is greater than the number of columns
            // or the index is negative, then we return null
            public override AccessibleObject? GetChild(int index)
            {
                if (_owningListView.View != View.Details)
                {
                    throw new InvalidOperationException(string.Format(SR.ListViewItemAccessibilityObjectInvalidViewException, nameof(View.Details)));
                }

                return !_owningListView.SupportsListViewSubItems || _owningListView.Columns.Count <= index || index < 0
                    ? null
                    : GetDetailsSubItemOrFake(index);
            }

            public override int GetChildCount()
            {
                if (_owningListView.View != View.Details)
                {
                    throw new InvalidOperationException(string.Format(SR.ListViewItemAccessibilityObjectInvalidViewException, nameof(View.Details)));
                }

                return !_owningListView.IsHandleCreated || !_owningListView.SupportsListViewSubItems
                    ? -1
                    : _owningListView.Columns.Count;
            }

            internal override int GetChildIndex(AccessibleObject? child)
            {
                if (child is null
                    || !_owningListView.SupportsListViewSubItems
                    || child is not ListViewSubItem.ListViewSubItemAccessibleObject subItemAccessibleObject)
                {
                    return -1;
                }

                if (subItemAccessibleObject.OwningSubItem is null)
                {
                    return GetFakeSubItemIndex(subItemAccessibleObject);
                }

                int index = _owningItem.SubItems.IndexOf(subItemAccessibleObject.OwningSubItem);
                return index > _owningListView.Columns.Count - 1 ? -1 : index;
            }

            // This method returns an accessibility object for an existing ListViewSubItem, or creates a fake one
            // if the ListViewSubItem does not exist. This is necessary for the "Details" view,
            // when there is no ListViewSubItem, but the cell for it is displayed and the user can interact with it.
            internal AccessibleObject? GetDetailsSubItemOrFake(int subItemIndex)
            {
                if (subItemIndex < _owningItem.SubItems.Count)
                {
                    _listViewSubItemAccessibleObjects.Remove(subItemIndex);

                    return _owningItem.SubItems[subItemIndex].AccessibilityObject;
                }

                if (_listViewSubItemAccessibleObjects.ContainsKey(subItemIndex))
                {
                    return _listViewSubItemAccessibleObjects[subItemIndex];
                }

                ListViewSubItem.ListViewSubItemAccessibleObject fakeAccessibleObject = new(owningSubItem: null, _owningItem);
                _listViewSubItemAccessibleObjects.Add(subItemIndex, fakeAccessibleObject);
                return fakeAccessibleObject;
            }

            // This method is required to get the index of the fake accessibility object. Since the fake accessibility object
            // has no ListViewSubItem from which we could get an index, we have to get its index from the dictionary
            private int GetFakeSubItemIndex(ListViewSubItem.ListViewSubItemAccessibleObject fakeAccessibleObject)
            {
                foreach (KeyValuePair<int, AccessibleObject> keyValuePair in _listViewSubItemAccessibleObjects)
                {
                    if (keyValuePair.Value == fakeAccessibleObject)
                    {
                        return keyValuePair.Key;
                    }
                }

                return -1;
            }

            internal override Rectangle GetSubItemBounds(int subItemIndex)
                => _owningListView.IsHandleCreated
                    ? _owningListView.GetSubItemRect(_owningItem.Index, subItemIndex)
                    : Rectangle.Empty;
        }
    }
}

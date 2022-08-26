// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ListViewItem
    {
        internal class ListViewItemDetailsAccessibleObject : ListViewItemBaseAccessibleObject
        {
            private const int ImageAccessibleObjectIndex = 0;
            private readonly Dictionary<int, AccessibleObject> _listViewSubItemAccessibleObjects;

            public ListViewItemDetailsAccessibleObject(ListViewItem owningItem) : base(owningItem)
            {
                _listViewSubItemAccessibleObjects = new Dictionary<int, AccessibleObject>();
            }

            internal override int FirstSubItemIndex => HasImage ? 1 : 0;

            private bool HasImage => _owningItem.ImageIndex != ImageList.Indexer.DefaultIndex;

            private int LastChildIndex => HasImage ? _owningListView.Columns.Count : _owningListView.Columns.Count - 1;

            /// <summary>
            ///  Converts the provided index of the <see cref="AccessibleObject"/>'s child to an index of a <see cref="ListViewSubItem"/>.
            /// </summary>
            /// <param name="accessibleChildIndex">The index of the child <see cref="AccessibleObject"/>.</param>
            /// <returns>The index of an owning <see cref="ListViewSubItem"/>'s object.</returns>
            private int AccessibleChildToSubItemIndex(int accessibleChildIndex) => HasImage ? accessibleChildIndex - 1 : accessibleChildIndex;

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                        return GetChild(0);
                    case UiaCore.NavigateDirection.LastChild:
                        return GetChild(LastChildIndex);
                }

                return base.FragmentNavigate(direction);
            }

            public override AccessibleObject? GetChild(int accessibleChildIndex)
            {
                if (_owningListView.View != View.Details)
                {
                    throw new InvalidOperationException(string.Format(SR.ListViewItemAccessibilityObjectInvalidViewException, nameof(View.Details)));
                }

                // If the ListView does not support ListViewSubItems, the accessibleChildIndex is greater than the number of columns
                // or the accessibleChildIndex is negative, then we return null
                return !_owningListView.SupportsListViewSubItems || accessibleChildIndex > LastChildIndex || accessibleChildIndex < 0
                    ? null
                    : GetDetailsSubItemOrFakeInternal(accessibleChildIndex);
            }

            public override int GetChildCount()
            {
                if (_owningListView.View != View.Details)
                {
                    throw new InvalidOperationException(string.Format(SR.ListViewItemAccessibilityObjectInvalidViewException, nameof(View.Details)));
                }

                return !_owningListView.IsHandleCreated || !_owningListView.SupportsListViewSubItems
                    ? -1
                    : LastChildIndex + 1;
            }

            internal override int GetChildIndex(AccessibleObject? child)
            {
                if (child is null || !_owningListView.SupportsListViewSubItems)
                {
                    return InvalidIndex;
                }

                if (child is ListViewItemImageAccessibleObject)
                {
                    return ImageAccessibleObjectIndex;
                }

                if (child is not ListViewSubItem.ListViewSubItemAccessibleObject subItemAccessibleObject)
                {
                    return InvalidIndex;
                }

                if (subItemAccessibleObject.OwningSubItem is null)
                {
                    return GetFakeSubItemIndex(subItemAccessibleObject);
                }

                int subItemIndex = _owningItem.SubItems.IndexOf(subItemAccessibleObject.OwningSubItem);
                int accessibleChildIndex = SubItemToAccessibleChildIndex(subItemIndex);
                return accessibleChildIndex > LastChildIndex ? InvalidIndex : accessibleChildIndex;
            }

            // This method returns an accessibility object for an existing ListViewSubItem, or creates a fake one
            // if the ListViewSubItem does not exist. This is necessary for the "Details" view,
            // when there is no ListViewSubItem, but the cell for it is displayed and the user can interact with it.
            internal AccessibleObject? GetDetailsSubItemOrFake(int subItemIndex)
            {
                int accessibleChildIndex = SubItemToAccessibleChildIndex(subItemIndex);
                return GetDetailsSubItemOrFakeInternal(accessibleChildIndex);
            }

            private AccessibleObject? GetDetailsSubItemOrFakeInternal(int accessibleChildIndex)
            {
                if (accessibleChildIndex == ImageAccessibleObjectIndex && HasImage)
                {
                    if (_listViewSubItemAccessibleObjects.ContainsKey(accessibleChildIndex))
                    {
                        return _listViewSubItemAccessibleObjects[accessibleChildIndex];
                    }

                    ListViewItemImageAccessibleObject imageAccessibleObject = new(_owningItem);
                    _listViewSubItemAccessibleObjects.Add(accessibleChildIndex, imageAccessibleObject);
                    return imageAccessibleObject;
                }

                int subItemIndex = AccessibleChildToSubItemIndex(accessibleChildIndex);
                if (subItemIndex < _owningItem.SubItems.Count)
                {
                    _listViewSubItemAccessibleObjects.Remove(accessibleChildIndex);

                    return _owningItem.SubItems[subItemIndex].AccessibilityObject;
                }

                if (_listViewSubItemAccessibleObjects.ContainsKey(accessibleChildIndex))
                {
                    return _listViewSubItemAccessibleObjects[accessibleChildIndex];
                }

                ListViewSubItem.ListViewSubItemAccessibleObject fakeAccessibleObject = new(owningSubItem: null, _owningItem);
                _listViewSubItemAccessibleObjects.Add(accessibleChildIndex, fakeAccessibleObject);
                return fakeAccessibleObject;
            }

            // This method is required to get the accessibleChildIndex of the fake accessibility object. Since the fake accessibility object
            // has no ListViewSubItem from which we could get an accessibleChildIndex, we have to get its accessibleChildIndex from the dictionary
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

            internal override Rectangle GetSubItemBounds(int accessibleChildIndex)
                => _owningListView.IsHandleCreated
                    ? _owningListView.GetSubItemRect(_owningItem.Index, AccessibleChildToSubItemIndex(accessibleChildIndex))
                    : Rectangle.Empty;

            /// <devdoc>
            /// .Caller should ensure that the current OS is Windows 8 or greater.
            /// </devdoc>
            internal override void ReleaseChildUiaProviders()
            {
                base.ReleaseChildUiaProviders();

                foreach (AccessibleObject accessibleObject in _listViewSubItemAccessibleObjects.Values)
                {
                    UiaCore.UiaDisconnectProvider(accessibleObject);
                }

                _listViewSubItemAccessibleObjects.Clear();
            }

            /// <summary>
            ///  Converts the provided index of a <see cref="ListViewSubItem"/> to an index of the <see cref="AccessibleObject"/>'s child.
            /// </summary>
            /// <param name="subItemIndex">The index of an owning <see cref="ListViewSubItem"/>'s object.</param>
            /// <returns>The index of the child <see cref="AccessibleObject"/>.</returns>
            private int SubItemToAccessibleChildIndex(int subItemIndex) => HasImage ? subItemIndex + 1 : subItemIndex;
        }
    }
}

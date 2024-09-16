// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ListViewItem
{
    internal sealed class ListViewItemDetailsAccessibleObject : ListViewItemBaseAccessibleObject
    {
        private const int ImageAccessibleObjectIndex = 0;
        private readonly Dictionary<int, AccessibleObject> _listViewSubItemAccessibleObjects;

        public ListViewItemDetailsAccessibleObject(ListViewItem owningItem) : base(owningItem)
        {
            _listViewSubItemAccessibleObjects = [];
        }

        internal override int FirstSubItemIndex => HasImage ? 1 : 0;

        private int LastChildIndex => HasImage ? _owningListView.Columns.Count : _owningListView.Columns.Count - 1;

        protected override View View => View.Details;

        /// <summary>
        ///  Converts the provided index of the <see cref="AccessibleObject"/>'s child to an index of a <see cref="ListViewSubItem"/>.
        /// </summary>
        /// <param name="accessibleChildIndex">The index of the child <see cref="AccessibleObject"/>.</param>
        /// <returns>The index of an owning <see cref="ListViewSubItem"/>'s object.</returns>
        private int AccessibleChildToSubItemIndex(int accessibleChildIndex) => HasImage
            ? _owningListView.Columns[accessibleChildIndex - 1]._correspondingListViewSubItemIndex
            : _owningListView.Columns[accessibleChildIndex]._correspondingListViewSubItemIndex;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
            => direction switch
            {
                NavigateDirection.NavigateDirection_FirstChild => GetChild(0),
                NavigateDirection.NavigateDirection_LastChild => GetChild(LastChildIndex),
                _ => base.FragmentNavigate(direction),
            };

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

        internal AccessibleObject? GetChild(int subItemIndex, Point point)
        {
            if (!HasImage || subItemIndex > 0)
            {
                return GetDetailsSubItemOrFake(subItemIndex);
            }

            return GetDetailsSubItemOrFakeInternal(ImageAccessibleObjectIndex) is ListViewItemImageAccessibleObject imageAccessibleObject &&
                   imageAccessibleObject.GetImageRectangle().Contains(point)
                ? imageAccessibleObject
                : GetDetailsSubItemOrFake(0);
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
            int accessibleChildIndex = InvalidIndex;
            for (int i = 0; i < _owningListView.Columns.Count; i++)
            {
                if (_owningListView.Columns[i]._correspondingListViewSubItemIndex == subItemIndex)
                {
                    accessibleChildIndex = i + (HasImage ? 1 : 0);
                    break;
                }
            }

            return accessibleChildIndex > LastChildIndex ? InvalidIndex : accessibleChildIndex;
        }

        // This method returns an accessibility object for an existing ListViewSubItem, or creates a fake one
        // if the ListViewSubItem does not exist. This is necessary for the "Details" view,
        // when there is no ListViewSubItem, but the cell for it is displayed and the user can interact with it.
        internal AccessibleObject? GetDetailsSubItemOrFake(int subItemIndex)
        {
            int accessibleChildIndex = HasImage ? subItemIndex + 1 : subItemIndex;
            return GetDetailsSubItemOrFakeInternal(accessibleChildIndex);
        }

        private AccessibleObject? GetDetailsSubItemOrFakeInternal(int accessibleChildIndex)
        {
            if (accessibleChildIndex == ImageAccessibleObjectIndex && HasImage)
            {
                if (_listViewSubItemAccessibleObjects.TryGetValue(accessibleChildIndex, out AccessibleObject? childAO))
                {
                    return childAO;
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

            if (_listViewSubItemAccessibleObjects.TryGetValue(accessibleChildIndex, out AccessibleObject? childAO2))
            {
                return childAO2;
            }

            ListViewSubItem.ListViewSubItemAccessibleObject fakeAccessibleObject = new(owningSubItem: null, _owningItem);
            _listViewSubItemAccessibleObjects.Add(accessibleChildIndex, fakeAccessibleObject);
            return fakeAccessibleObject;
        }

        // This method is required to get the accessibleChildIndex of the fake accessibility object.
        // Since the fake accessibility object
        // has no ListViewSubItem from which we could get an accessibleChildIndex,
        // we have to get its accessibleChildIndex from the dictionary
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
                ? _owningListView.GetSubItemRect(_owningItem.Index, HasImage ? accessibleChildIndex - 1 : accessibleChildIndex)
                : Rectangle.Empty;

        /// <devdoc>
        /// .Caller should ensure that the current OS is Windows 8 or greater.
        /// </devdoc>
        internal override void ReleaseChildUiaProviders()
        {
            base.ReleaseChildUiaProviders();

            foreach (AccessibleObject accessibleObject in _listViewSubItemAccessibleObjects.Values)
            {
                PInvoke.UiaDisconnectProvider(accessibleObject, skipOSCheck: true);
            }

            _listViewSubItemAccessibleObjects.Clear();
        }
    }
}

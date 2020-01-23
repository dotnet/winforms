// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;

namespace System.Windows.Forms
{
    internal class ListViewGroupItemCollection : ListView.ListViewItemCollection.IInnerList
    {
        private readonly ListViewGroup _group;
        private ArrayList _items;

        public ListViewGroupItemCollection(ListViewGroup group)
        {
            _group = group;
        }

        public int Count => Items.Count;

        private ArrayList Items => _items ?? (_items = new ArrayList());

        public bool OwnerIsVirtualListView => _group.ListView != null && _group.ListView.VirtualMode;

        public bool OwnerIsDesignMode => _group.ListView?.Site != null && _group.ListView.Site.DesignMode;

        public ListViewItem this[int index]
        {
            get => (ListViewItem)Items[index];
            set
            {
                if (value != Items[index])
                {
                    MoveToGroup((ListViewItem)Items[index], null);
                    Items[index] = value;
                    MoveToGroup((ListViewItem)Items[index], _group);
                }
            }
        }

        public ListViewItem Add(ListViewItem value)
        {
            CheckListViewItem(value);

            MoveToGroup(value, _group);
            Items.Add(value);
            return value;
        }

        public void AddRange(ListViewItem[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                CheckListViewItem(items[i]);
            }

            Items.AddRange(items);

            for (int i = 0; i < items.Length; i++)
            {
                MoveToGroup(items[i], _group);
            }
        }

        private void CheckListViewItem(ListViewItem item)
        {
            if (item.ListView != null && item.ListView != _group.ListView)
            {
                throw new ArgumentException(string.Format(SR.OnlyOneControl, item.Text), nameof(item));
            }
        }

        public void Clear()
        {
            for (int i = 0; i < Count; i++)
            {
                MoveToGroup(this[i], null);
            }
            Items.Clear();
        }

        public bool Contains(ListViewItem item) => Items.Contains(item);

        public void CopyTo(Array dest, int index) => Items.CopyTo(dest, index);

        public IEnumerator GetEnumerator() => Items.GetEnumerator();

        public int IndexOf(ListViewItem item) => Items.IndexOf(item);

        public ListViewItem Insert(int index, ListViewItem item)
        {
            CheckListViewItem(item);

            MoveToGroup(item, _group);
            Items.Insert(index, item);
            return item;
        }

        private void MoveToGroup(ListViewItem item, ListViewGroup newGroup)
        {
            ListViewGroup oldGroup = item.Group;
            if (oldGroup != newGroup)
            {
                item.group = newGroup;
                oldGroup?.Items.Remove(item);
                UpdateNativeListViewItem(item);
            }
        }

        public void Remove(ListViewItem item)
        {
            Items.Remove(item);

            if (item.group == _group)
            {
                item.group = null;
                UpdateNativeListViewItem(item);
            }
        }

        public void RemoveAt(int index) => Remove(this[index]);

        private void UpdateNativeListViewItem(ListViewItem item)
        {
            if (item.ListView != null && item.ListView.IsHandleCreated && !item.ListView.InsertingItemsNatively)
            {
                item.UpdateStateToListView(item.Index);
            }
        }
    }
}

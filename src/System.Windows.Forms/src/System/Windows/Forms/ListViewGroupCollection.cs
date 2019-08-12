// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows.Forms
{
    /// <summary>
    ///  A collection of listview groups.
    /// </summary>
    [ListBindable(false)]
    public class ListViewGroupCollection : IList
    {
        private readonly ListView _listView;

        private ArrayList _list;

        internal ListViewGroupCollection(ListView listView)
        {
            _listView = listView;
        }

        public int Count => List.Count;

        object ICollection.SyncRoot => this;

        bool ICollection.IsSynchronized => true;

        bool IList.IsFixedSize => false;

        bool IList.IsReadOnly => false;

        private ArrayList List => _list ?? (_list = new ArrayList());

        public ListViewGroup this[int index]
        {
            get => (ListViewGroup)List[index];
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (List.Contains(value))
                {
                    return;
                }

                CheckListViewItems(value);
                value.ListView = _listView;
                List[index] = value;
            }
        }

        public ListViewGroup this[string key]
        {
            get
            {
                if (_list == null)
                {
                    return null;
                }

                for (int i = 0; i < _list.Count; i++)
                {
                    if (string.Equals(key, this[i].Name, StringComparison.CurrentCulture))
                    {
                        return this[i];
                    }
                }

                return null;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (_list == null)
                {
                    // nothing to do
                    return;
                }

                int index = -1;
                for (int i = 0; i < _list.Count; i++)
                {
                    if (string.Equals(key, this[i].Name, StringComparison.CurrentCulture))
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    _list[index] = value;
                }
            }
        }

        object IList.this[int index]
        {
            get => this[index];
            set
            {
                if (value is ListViewGroup group)
                {
                    this[index] = group;
                }
            }
        }

        public int Add(ListViewGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            if (Contains(group))
            {
                return -1;
            }

            CheckListViewItems(group);
            group.ListView = _listView;
            int index = List.Add(group);
            if (_listView.IsHandleCreated)
            {
                _listView.InsertGroupInListView(List.Count, group);
                MoveGroupItems(group);
            }

            return index;
        }

        public ListViewGroup Add(string key, string headerText)
        {
            ListViewGroup group = new ListViewGroup(key, headerText);
            Add(group);
            return group;
        }

        int IList.Add(object value)
        {
            if (!(value is ListViewGroup group))
            {
                throw new ArgumentException(SR.ListViewGroupCollectionBadListViewGroup, nameof(value));
            }

            return Add(group);
        }

        public void AddRange(ListViewGroup[] groups)
        {
            if (groups == null)
            {
                throw new ArgumentNullException(nameof(groups));
            }

            for (int i = 0; i < groups.Length; i++)
            {
                Add(groups[i]);
            }
        }

        public void AddRange(ListViewGroupCollection groups)
        {
            if (groups == null)
            {
                throw new ArgumentNullException(nameof(groups));
            }

            for (int i = 0; i < groups.Count; i++)
            {
                Add(groups[i]);
            }
        }

        private void CheckListViewItems(ListViewGroup group)
        {
            for (int i = 0; i < group.Items.Count; i++)
            {
                ListViewItem item = group.Items[i];
                if (item.ListView != null && item.ListView != _listView)
                {
                    throw new ArgumentException(string.Format(SR.OnlyOneControl, item.Text));
                }
            }
        }

        public void Clear()
        {
            if (_listView.IsHandleCreated)
            {
                for (int i = 0; i < Count; i++)
                {
                    _listView.RemoveGroupFromListView(this[i]);
                }
            }

            // Dissociate groups from the ListView
            for (int i = 0; i < Count; i++)
            {
                this[i].ListView = null;
            }

            List.Clear();

            // we have to tell the listView that there are no more groups
            // so the list view knows to remove items from the default group
            _listView.UpdateGroupView();
        }

        public bool Contains(ListViewGroup value) => List.Contains(value);

        bool IList.Contains(object value)
        {
            if (!(value is ListViewGroup group))
            {
                return false;
            }

            return Contains(group);
        }

        public void CopyTo(Array array, int index) => List.CopyTo(array, index);

        public IEnumerator GetEnumerator() => List.GetEnumerator();

        public int IndexOf(ListViewGroup value) => List.IndexOf(value);

        int IList.IndexOf(object value)
        {
            if (!(value is ListViewGroup group))
            {
                return -1;
            }

            return IndexOf((ListViewGroup)value);
        }

        public void Insert(int index, ListViewGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            if (Contains(group))
            {
                return;
            }

            CheckListViewItems(group);
            group.ListView = _listView;
            List.Insert(index, group);
            if (_listView.IsHandleCreated)
            {
                _listView.InsertGroupInListView(index, group);
                MoveGroupItems(group);
            }
        }

        void IList.Insert(int index, object value)
        {
            if (value is ListViewGroup group)
            {
                Insert(index, group);
            }
        }

        private void MoveGroupItems(ListViewGroup group)
        {
            Debug.Assert(_listView.IsHandleCreated, "MoveGroupItems pre-condition: listView handle must be created");

            foreach (ListViewItem item in group.Items)
            {
                if (item.ListView == _listView)
                {
                    item.UpdateStateToListView(item.Index);
                }
            }
        }

        public void Remove(ListViewGroup group)
        {
            group.ListView = null;
            List.Remove(group);

            if (_listView.IsHandleCreated)
            {
                _listView.RemoveGroupFromListView(group);
            }
        }

        void IList.Remove(object value)
        {
            if (value is ListViewGroup group)
            {
                Remove(group);
            }
        }

        public void RemoveAt(int index) => Remove(this[index]);
    }
}

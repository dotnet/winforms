// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Globalization;

using static System.Windows.Forms.ItemArray;

namespace System.Windows.Forms;

public partial class ListBox
{
    /// <summary>
    ///  A collection that stores objects.
    /// </summary>
    [ListBindable(false)]
    public class ObjectCollection : IList
    {
        private readonly ListBox _owner;
        private ItemArray _items = null!;

        public ObjectCollection(ListBox owner)
        {
            _owner = owner.OrThrowIfNull();
        }

        /// <summary>
        ///  Initializes a new instance of ListBox.ObjectCollection based on another ListBox.ObjectCollection.
        /// </summary>
        public ObjectCollection(ListBox owner, ObjectCollection value)
            : this(owner)
        {
            ArgumentNullException.ThrowIfNull(value);

            AddRange(value);
        }

        /// <summary>
        ///  Initializes a new instance of ListBox.ObjectCollection containing any array of objects.
        /// </summary>
        public ObjectCollection(ListBox owner, object[] value)
            : this(owner)
        {
            ArgumentNullException.ThrowIfNull(value);

            AddRange(value);
        }

        /// <summary>
        ///  Retrieves the number of items.
        /// </summary>
        public int Count => InnerArray.Count;

        /// <summary>
        ///  Internal access to the actual data store.
        /// </summary>
        internal ItemArray InnerArray
        {
            get
            {
                _items ??= new ItemArray(_owner);

                return _items;
            }
        }

        object ICollection.SyncRoot => this;

        bool ICollection.IsSynchronized => false;

        bool IList.IsFixedSize => false;

        public bool IsReadOnly => false;

        /// <summary>
        ///  Adds an item to the List box. For an unsorted List box, the item is
        ///  added to the end of the existing list of items. For a sorted List box,
        ///  the item is inserted into the list according to its sorted position.
        ///  The item's toString() method is called to obtain the string that is
        ///  displayed in the List box.
        ///  A SystemException occurs if there is insufficient space available to
        ///  store the new item.
        /// </summary>
        public int Add(object item)
        {
            _owner.CheckNoDataSource();
            int index = AddInternal(item);
            _owner.UpdateHorizontalExtent();
            return index;
        }

        private int AddInternal(object item)
        {
            ArgumentNullException.ThrowIfNull(item);

            int index = -1;
            if (!_owner._sorted)
            {
                InnerArray.Add(item);
            }
            else
            {
                Entry entry = GetEntry(item);
                if (Count > 0)
                {
                    index = InnerArray.BinarySearch(entry);
                    if (index < 0)
                    {
                        // getting the index of the first element that is larger than the search value
                        // this index will be used for insert
                        index = ~index;
                    }
                }
                else
                {
                    index = 0;
                }

                Debug.Assert(index >= 0 && index <= Count, "Wrong index for insert");
                InnerArray.InsertEntry(index, entry);
            }

            bool successful = false;

            try
            {
                if (_owner._sorted)
                {
                    if (_owner.IsHandleCreated)
                    {
                        _owner.NativeInsert(index, item);
                        _owner.UpdateMaxItemWidth(item, false);
                        // Sorting may throw the LB contents and the selectedItem array out of synch.
                        _owner._selectedItems?.Dirty();
                    }
                }
                else
                {
                    index = Count - 1;
                    if (_owner.IsHandleCreated)
                    {
                        _owner.NativeAdd(item);
                        _owner.UpdateMaxItemWidth(item, false);
                    }
                }

                successful = true;
            }
            finally
            {
                if (!successful)
                {
                    InnerArray.Remove(item);
                }
            }

            return index;
        }

        int IList.Add(object? item) => Add(item!);

        public void AddRange(ObjectCollection value)
        {
            ArgumentNullException.ThrowIfNull(value);

            _owner.CheckNoDataSource();
            AddRangeInternal(value);
        }

        public void AddRange(params object[] items)
        {
            ArgumentNullException.ThrowIfNull(items);

            _owner.CheckNoDataSource();
            AddRangeInternal(items);
        }

        internal void AddRangeInternal(ICollection items)
        {
            Debug.Assert(items is not null);

            _owner.BeginUpdate();
            try
            {
                foreach (object item in items)
                {
                    // adding items one-by-one for performance
                    // not using sort because after the array is sorted index of each newly added item will need to be found
                    // AddInternal is based on BinarySearch and finds index without any additional cost
                    AddInternal(item);
                }
            }
            finally
            {
                _owner.UpdateHorizontalExtent();
                _owner.EndUpdate();
            }
        }

        /// <summary>
        ///  Retrieves the item with the specified index.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual object this[int index]
        {
            get
            {
                ArgumentOutOfRangeException.ThrowIfNegative(index);
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, InnerArray.Count);

                return InnerArray.GetItem(index);
            }
            set
            {
                _owner.CheckNoDataSource();
                SetItemInternal(index, value);
            }
        }

        object? IList.this[int index]
        {
            get => this[index];
            set => this[index] = value!;
        }

        /// <summary>
        ///  Removes all items from the ListBox.
        /// </summary>
        public virtual void Clear()
        {
            _owner.CheckNoDataSource();
            ClearInternal();
        }

        /// <summary>
        ///  Removes all items from the ListBox. Bypasses the data source check.
        /// </summary>
        internal void ClearInternal()
        {
            // update the width.. to reset Scrollbars..
            // Clear the selection state.
            int cnt = _owner.Items.Count;
            for (int i = 0; i < cnt; i++)
            {
                _owner.UpdateMaxItemWidth(InnerArray.GetItem(i), true);
            }

            if (_owner.IsHandleCreated)
            {
                _owner.NativeClear();
            }

            InnerArray.Clear();
            _owner._maxWidth = -1;
            _owner.UpdateHorizontalExtent();
            _owner.ClearListItemAccessibleObjects();
        }

        public bool Contains(object value)
        {
            return IndexOf(value) != -1;
        }

        bool IList.Contains(object? value) => Contains(value!);

        /// <summary>
        ///  Copies the ListBox Items collection to a destination array.
        /// </summary>
        public void CopyTo(object[] destination, int arrayIndex)
        {
            ArgumentNullException.ThrowIfNull(destination);

            int count = InnerArray.Count;
            for (int i = 0; i < count; i++)
            {
                destination[i + arrayIndex] = InnerArray.GetItem(i);
            }
        }

        void ICollection.CopyTo(Array destination, int index)
        {
            ArgumentNullException.ThrowIfNull(destination);

            int count = InnerArray.Count;
            for (int i = 0; i < count; i++)
            {
                destination.SetValue(InnerArray.GetItem(i), i + index);
            }
        }

        /// <summary>
        ///  Returns an enumerator for the ListBox Items collection.
        /// </summary>
        public IEnumerator GetEnumerator() => InnerArray.GetEnumerator(0);

        public int IndexOf(object value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return InnerArray.IndexOf(value);
        }

        int IList.IndexOf(object? value) => IndexOf(value!);

        internal int IndexOfIdentifier(object value)
        {
            ArgumentNullException.ThrowIfNull(value);

            return InnerArray.IndexOf(value);
        }

        /// <summary>
        ///  Adds an item to the List box. For an unsorted List box, the item is
        ///  added to the end of the existing list of items. For a sorted List box,
        ///  the item is inserted into the list according to its sorted position.
        ///  The item's toString() method is called to obtain the string that is
        ///  displayed in the List box.
        ///  A SystemException occurs if there is insufficient space available to
        ///  store the new item.
        /// </summary>
        public void Insert(int index, object item)
        {
            _owner.CheckNoDataSource();

            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, InnerArray.Count);
            ArgumentNullException.ThrowIfNull(item);

            // If the List box is sorted, then nust treat this like an add
            // because we are going to twiddle the index anyway.
            if (_owner._sorted)
            {
                Add(item);
            }
            else
            {
                InnerArray.Insert(index, item);
                if (_owner.IsHandleCreated)
                {
                    bool successful = false;

                    try
                    {
                        _owner.NativeInsert(index, item);
                        _owner.UpdateMaxItemWidth(item, false);
                        successful = true;
                    }
                    finally
                    {
                        if (!successful)
                        {
                            InnerArray.RemoveAt(index);
                        }
                    }
                }
            }

            _owner.UpdateHorizontalExtent();
        }

        void IList.Insert(int index, object? item) => Insert(index, item!);

        /// <summary>
        ///  Removes the given item from the ListBox, provided that it is
        ///  actually in the list.
        /// </summary>
        public void Remove(object value)
        {
            _owner.CheckNoDataSource();

            int index = InnerArray.IndexOf(value);
            if (index != -1)
            {
                RemoveAt(index);
            }
        }

        void IList.Remove(object? value) => Remove(value!);

        /// <summary>
        ///  Removes an item from the ListBox at the given index.
        /// </summary>
        public void RemoveAt(int index)
        {
            _owner.CheckNoDataSource();

            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, InnerArray.Count);

            _owner.UpdateMaxItemWidth(InnerArray.GetItem(index), true);

            // Remove AccessibleObject before removing item from InnerArray because AccessibleObject relies on
            // item's presence in InnerArray
            _owner.RemoveListItemAccessibleObjectAt(index);

            // Update InnerArray before calling NativeRemoveAt to ensure that when
            // SelectedIndexChanged is raised (by NativeRemoveAt), InnerArray's state matches wrapped LB state.
            InnerArray.RemoveAt(index);

            if (_owner.IsHandleCreated)
            {
                _owner.NativeRemoveAt(index);
            }

            _owner.UpdateHorizontalExtent();
        }

        internal void SetItemInternal(int index, object value)
        {
            ArgumentNullException.ThrowIfNull(value);
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, InnerArray.Count);

            _owner.UpdateMaxItemWidth(InnerArray.GetItem(index), true);
            InnerArray.SetItem(index, value);

            // If the native control has been created, and the display text of the new list item object
            // is different to the current text in the native list item, recreate the native list item...
            if (_owner.IsHandleCreated)
            {
                bool selected = (_owner.SelectedIndex == index);
                if (string.Compare(_owner.GetItemText(value), _owner.NativeGetItemText(index), true, CultureInfo.CurrentCulture) != 0)
                {
                    _owner.NativeRemoveAt(index);
                    _owner.SelectedItems.SetSelected(index, false);
                    _owner.NativeInsert(index, value);
                    _owner.UpdateMaxItemWidth(value, false);
                    if (selected)
                    {
                        _owner.SelectedIndex = index;
                    }
                }
                else
                {
                    // FOR COMPATIBILITY REASONS
                    if (selected)
                    {
                        _owner.OnSelectedIndexChanged(EventArgs.Empty); // will fire selectedvaluechanged
                    }
                }
            }

            _owner.UpdateHorizontalExtent();
        }
    }
}

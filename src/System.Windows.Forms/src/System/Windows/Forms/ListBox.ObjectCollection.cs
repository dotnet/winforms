// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace System.Windows.Forms
{
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
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            /// <summary>
            ///  Initializes a new instance of ListBox.ObjectCollection based on another ListBox.ObjectCollection.
            /// </summary>
            public ObjectCollection(ListBox owner, ObjectCollection value)
                : this(owner)
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                AddRange(value);
            }

            /// <summary>
            ///  Initializes a new instance of ListBox.ObjectCollection containing any array of objects.
            /// </summary>
            public ObjectCollection(ListBox owner, object[] value)
                : this(owner)
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                AddRange(value);
            }

            /// <summary>
            ///  Retrieves the number of items.
            /// </summary>
            public int Count => InnerArray.GetCount(0);

            /// <summary>
            ///  Internal access to the actual data store.
            /// </summary>
            internal ItemArray InnerArray
            {
                get
                {
                    if (_items is null)
                    {
                        _items = new ItemArray(_owner);
                    }
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
                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                int index = -1;
                if (!_owner.sorted)
                {
                    InnerArray.Add(item);
                }
                else
                {
                    if (Count > 0)
                    {
                        index = InnerArray.BinarySearch(item);
                        if (index < 0)
                        {
                            // getting the index of the first element that is larger than the search value
                            //this index will be used for insert
                            index = ~index;
                        }
                    }
                    else
                    {
                        index = 0;
                    }

                    Debug.Assert(index >= 0 && index <= Count, "Wrong index for insert");
                    InnerArray.Insert(index, item);
                }
                bool successful = false;

                try
                {
                    if (_owner.sorted)
                    {
                        if (_owner.IsHandleCreated)
                        {
                            _owner.NativeInsert(index, item);
                            _owner.UpdateMaxItemWidth(item, false);
                            if (_owner.selectedItems != null)
                            {
                                // Sorting may throw the LB contents and the selectedItem array out of synch.
                                _owner.selectedItems.Dirty();
                            }
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
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _owner.CheckNoDataSource();
                AddRangeInternal(value);
            }

            public void AddRange(object[] items)
            {
                if (items is null)
                {
                    throw new ArgumentNullException(nameof(items));
                }

                _owner.CheckNoDataSource();
                AddRangeInternal(items);
            }

            internal void AddRangeInternal(ICollection items)
            {
                Debug.Assert(items != null);

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
                    if (index < 0 || index >= InnerArray.GetCount(0))
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    return InnerArray.GetItem(index, 0);
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
            ///  Removes all items from the ListBox.  Bypasses the data source check.
            /// </summary>
            internal void ClearInternal()
            {
                //update the width.. to reset Scrollbars..
                // Clear the selection state.
                //
                int cnt = _owner.Items.Count;
                for (int i = 0; i < cnt; i++)
                {
                    _owner.UpdateMaxItemWidth(InnerArray.GetItem(i, 0), true);
                }

                if (_owner.IsHandleCreated)
                {
                    _owner.NativeClear();
                }
                InnerArray.Clear();
                _owner.maxWidth = -1;
                _owner.UpdateHorizontalExtent();
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
                if (destination is null)
                {
                    throw new ArgumentNullException(nameof(destination));
                }

                int count = InnerArray.GetCount(0);
                for (int i = 0; i < count; i++)
                {
                    destination[i + arrayIndex] = InnerArray.GetItem(i, 0);
                }
            }

            void ICollection.CopyTo(Array destination, int index)
            {
                if (destination is null)
                {
                    throw new ArgumentNullException(nameof(destination));
                }

                int count = InnerArray.GetCount(0);
                for (int i = 0; i < count; i++)
                {
                    destination.SetValue(InnerArray.GetItem(i, 0), i + index);
                }
            }

            /// <summary>
            ///  Returns an enumerator for the ListBox Items collection.
            /// </summary>
            public IEnumerator GetEnumerator() => InnerArray.GetEnumerator(0);

            public int IndexOf(object value)
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                return InnerArray.IndexOf(value, 0);
            }

            int IList.IndexOf(object? value) => IndexOf(value!);

            internal int IndexOfIdentifier(object value)
            {
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                return InnerArray.IndexOfIdentifier(value, 0);
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

                if (index < 0 || index > InnerArray.GetCount(0))
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                if (item is null)
                {
                    throw new ArgumentNullException(nameof(item));
                }

                // If the List box is sorted, then nust treat this like an add
                // because we are going to twiddle the index anyway.
                //
                if (_owner.sorted)
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

                int index = InnerArray.IndexOf(value, 0);
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

                if (index < 0 || index >= InnerArray.GetCount(0))
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                _owner.UpdateMaxItemWidth(InnerArray.GetItem(index, 0), true);

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
                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (index < 0 || index >= InnerArray.GetCount(0))
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                _owner.UpdateMaxItemWidth(InnerArray.GetItem(index, 0), true);
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
}

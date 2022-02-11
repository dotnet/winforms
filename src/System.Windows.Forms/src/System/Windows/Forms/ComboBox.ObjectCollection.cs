// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using static System.Windows.Forms.ComboBox.ObjectCollection;

namespace System.Windows.Forms
{
    public partial class ComboBox
    {
        [ListBindable(false)]
        public partial class ObjectCollection : IList, IComparer<Entry>
        {
            private readonly ComboBox _owner;
            private ComboBoxAccessibleObject _ownerComboBoxAccessibleObject;
            private List<Entry> _innerList;

            public ObjectCollection(ComboBox owner)
            {
                _owner = owner;
            }

            private ComboBoxAccessibleObject OwnerComboBoxAccessibleObject
            {
                get
                {
                    if (!_owner.IsAccessibilityObjectCreated)
                    {
                        return null;
                    }

                    if (_ownerComboBoxAccessibleObject is not null)
                    {
                        return _ownerComboBoxAccessibleObject;
                    }

                    if (_owner.AccessibilityObject is ComboBoxAccessibleObject accessibleObject)
                    {
                        _ownerComboBoxAccessibleObject = accessibleObject;
                    }

                    return _ownerComboBoxAccessibleObject;
                }
            }

            internal List<Entry> InnerList
            {
                get
                {
                    if (_innerList is null)
                    {
                        _innerList = new List<Entry>();
                    }

                    return _innerList;
                }
            }

            /// <summary>
            ///  Retrieves the number of items.
            /// </summary>
            public int Count => InnerList.Count;

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return false;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return false;
                }
            }

            /// <summary>
            ///  Adds an item to the combo box. For an unsorted combo box, the item is
            ///  added to the end of the existing list of items. For a sorted combo box,
            ///  the item is inserted into the list according to its sorted position.
            ///  The item's toString() method is called to obtain the string that is
            ///  displayed in the combo box.
            ///  A SystemException occurs if there is insufficient space available to
            ///  store the new item.
            /// </summary>
            public int Add(object item)
            {
                _owner.CheckNoDataSource();
                int index = AddInternal(item);
                if (_owner.UpdateNeeded() && _owner.AutoCompleteSource == AutoCompleteSource.ListItems)
                {
                    _owner.SetAutoComplete(false, false);
                }

                return index;
            }

            private int AddInternal(object item)
            {
                ArgumentNullException.ThrowIfNull(item);

                int index = -1;
                if (!_owner._sorted)
                {
                    InnerList.Add(new Entry(item));
                }
                else
                {
                    Entry entry = item is Entry entryItem ? entryItem : new Entry(item);
                    index = InnerList.BinarySearch(index: 0, Count, entry, this);
                    if (index < 0)
                    {
                        index = ~index; // getting the index of the first element that is larger than the search value
                    }

                    Debug.Assert(index >= 0 && index <= Count, "Wrong index for insert");
                    InnerList.Insert(index, entry);
                }

                bool successful = false;

                try
                {
                    if (_owner._sorted)
                    {
                        if (_owner.IsHandleCreated)
                        {
                            _owner.NativeInsert(index, item);
                        }
                    }
                    else
                    {
                        index = Count - 1;
                        if (_owner.IsHandleCreated)
                        {
                            _owner.NativeAdd(item);
                        }
                    }

                    successful = true;
                }
                finally
                {
                    if (!successful)
                    {
                        OwnerComboBoxAccessibleObject?.ItemAccessibleObjects.Remove(InnerList[index]);
                        Remove(item);
                    }
                }

                return index;
            }

            int IList.Add(object item)
            {
                return Add(item);
            }

            public void AddRange(object[] items)
            {
                _owner.CheckNoDataSource();
                _owner.BeginUpdate();
                try
                {
                    AddRangeInternal(items);
                }
                finally
                {
                    _owner.EndUpdate();
                }
            }

            internal void AddRangeInternal(IList items)
            {
                ArgumentNullException.ThrowIfNull(items);

                foreach (object item in items)
                {
                    // adding items one-by-one for performance (especially for sorted combobox)
                    // we can not rely on ArrayList.Sort since its worst case complexity is n*n
                    // AddInternal is based on BinarySearch and ensures n*log(n) complexity
                    AddInternal(item);
                }

                if (_owner.AutoCompleteSource == AutoCompleteSource.ListItems)
                {
                    _owner.SetAutoComplete(false, false);
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
                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    return InnerList[index].Item;
                }
                set
                {
                    _owner.CheckNoDataSource();
                    SetItemInternal(index, value);
                }
            }

            /// <summary>
            ///  Removes all items from the ComboBox.
            /// </summary>
            public void Clear()
            {
                _owner.CheckNoDataSource();
                ClearInternal();
            }

            internal void ClearInternal()
            {
                if (_owner.IsHandleCreated)
                {
                    _owner.NativeClear();
                }

                InnerList.Clear();

                OwnerComboBoxAccessibleObject?.ItemAccessibleObjects.Clear();

                _owner._selectedIndex = -1;
                if (_owner.AutoCompleteSource == AutoCompleteSource.ListItems)
                {
                    _owner.SetAutoComplete(false, true /*recreateHandle*/);
                }
            }

            public bool Contains(object value)
            {
                return IndexOf(value) != -1;
            }

            /// <summary>
            ///  Copies the ComboBox Items collection to a destination array.
            /// </summary>
            public void CopyTo(object[] destination, int arrayIndex)
            {
                ArgumentNullException.ThrowIfNull(destination);

                int count = InnerList.Count;

                if (arrayIndex < 0 || count + arrayIndex > destination.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(arrayIndex), arrayIndex, string.Format(SR.InvalidArgument, nameof(arrayIndex), arrayIndex));
                }

                for (int i = 0; i < count; i++)
                {
                    destination[i + arrayIndex] = InnerList[i].Item;
                }
            }

            void ICollection.CopyTo(Array destination, int index)
            {
                ArgumentNullException.ThrowIfNull(destination);

                int count = InnerList.Count;

                if (index < 0 || count + index > destination.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                for (int i = 0; i < count; i++)
                {
                    destination.SetValue(InnerList[i], i + index);
                }
            }

            /// <summary>
            ///  Returns an enumerator for the ComboBox Items collection.
            /// </summary>
            public IEnumerator GetEnumerator() => new EntryEnumerator(InnerList);

            /// <summary>
            ///  Adds an item to the combo box. For an unsorted combo box, the item is
            ///  added to the end of the existing list of items. For a sorted combo box,
            ///  the item is inserted into the list according to its sorted position.
            ///  The item's toString() method is called to obtain the string that is
            ///  displayed in the combo box.
            ///  A SystemException occurs if there is insufficient space available to
            ///  store the new item.
            /// </summary>
            public void Insert(int index, object item)
            {
                _owner.CheckNoDataSource();

                ArgumentNullException.ThrowIfNull(item);

                if (index < 0 || index > Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                // If the combo box is sorted, then nust treat this like an add
                // because we are going to twiddle the index anyway.
                //
                if (_owner._sorted)
                {
                    Add(item);
                }
                else
                {
                    InnerList.Insert(index, new Entry(item));
                    if (_owner.IsHandleCreated)
                    {
                        bool successful = false;

                        try
                        {
                            _owner.NativeInsert(index, item);
                            successful = true;
                        }
                        finally
                        {
                            if (successful)
                            {
                                if (_owner.AutoCompleteSource == AutoCompleteSource.ListItems)
                                {
                                    _owner.SetAutoComplete(false, false);
                                }
                            }
                            else
                            {
                                OwnerComboBoxAccessibleObject?.ItemAccessibleObjects.Remove(InnerList[index]);
                                InnerList.RemoveAt(index);
                            }
                        }
                    }
                }
            }

            /// <summary>
            ///  Removes an item from the ComboBox at the given index.
            /// </summary>
            public void RemoveAt(int index)
            {
                _owner.CheckNoDataSource();

                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                // Must get this before InnerList.RemoveAt
                string text = _owner.Text;

                OwnerComboBoxAccessibleObject?.ItemAccessibleObjects.Remove(InnerList[index]);
                InnerList.RemoveAt(index);

                if (_owner.IsHandleCreated)
                {
                    int selectedIndex = _owner.SelectedIndex;
                    _owner.NativeRemoveAt(index);

                    if (index == selectedIndex)
                    {
                        if (InnerList.Count == 0)
                        {
                            // Text is not cleared when the last item is removed. This is native behavior that must be compensated
                            _owner.UpdateText();
                        }
                        else if (!string.IsNullOrEmpty(text))
                        {
                            _owner.OnTextChanged(EventArgs.Empty);
                        }

                        _owner.OnSelectedItemChanged(EventArgs.Empty);
                        _owner.OnSelectedIndexChanged(EventArgs.Empty);
                    }
                }
                else
                {
                    if (index < _owner._selectedIndex)
                    {
                        // Don't raise events: the selected item merely changed position in the collection
                        _owner._selectedIndex--;
                    }
                    else if (index == _owner._selectedIndex)
                    {
                        // Raise events
                        _owner.SelectedIndex = -1;
                    }
                }

                if (_owner.AutoCompleteSource == AutoCompleteSource.ListItems)
                {
                    _owner.SetAutoComplete(false, false);
                }
            }

            /// <summary>
            ///  Removes the given item from the ComboBox, provided that it is
            ///  actually in the list.
            /// </summary>
            public void Remove(object value)
            {
                int index = IndexOf(value);

                if (index != -1)
                {
                    RemoveAt(index);
                }
            }

            internal void SetItemInternal(int index, object value)
            {
                if (index < 0 || index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                InnerList[index].Item = value.OrThrowIfNull();

                // If the native control has been created, and the display text of the new list item object
                // is different to the current text in the native list item, recreate the native list item...
                if (_owner.IsHandleCreated)
                {
                    bool selected = (index == _owner.SelectedIndex);

                    if (string.Compare(_owner.GetItemText(value), _owner.NativeGetItemText(index), true, CultureInfo.CurrentCulture) != 0)
                    {
                        _owner.NativeRemoveAt(index);
                        _owner.NativeInsert(index, value);
                        if (selected)
                        {
                            _owner.SelectedIndex = index;
                            _owner.UpdateText();
                        }

                        if (_owner.AutoCompleteSource == AutoCompleteSource.ListItems)
                        {
                            _owner.SetAutoComplete(false, false);
                        }
                    }
                    else
                    {
                        // NEW - FOR COMPATIBILITY REASONS
                        // Minimum compatibility fix
                        if (selected)
                        {
                            _owner.OnSelectedItemChanged(EventArgs.Empty);   //we do this because set_SelectedIndex does this. (for consistency)
                            _owner.OnSelectedIndexChanged(EventArgs.Empty);
                        }
                    }
                }
            }

            public int IndexOf(object value)
            {
                int virtualIndex = -1;

                foreach (Entry entry in InnerList)
                {
                    virtualIndex++;
                    if ((value is Entry itemEntry && entry == itemEntry) || entry.Item.Equals(value))
                    {
                        return virtualIndex;
                    }
                }

                return -1;
            }

            int IComparer<Entry>.Compare(Entry entry1, Entry entry2)
            {
                string itemName1 = _owner.GetItemText(entry1.Item);
                string itemName2 = _owner.GetItemText(entry2.Item);

                CompareInfo compInfo = Application.CurrentCulture.CompareInfo;
                return compInfo.Compare(itemName1, itemName2, CompareOptions.StringSort);
            }
        } // end ObjectCollection
    }
}

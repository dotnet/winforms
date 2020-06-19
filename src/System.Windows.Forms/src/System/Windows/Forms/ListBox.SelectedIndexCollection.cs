// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
    public partial class ListBox
    {
        public class SelectedIndexCollection : IList
        {
            private readonly ListBox _owner;

            public SelectedIndexCollection(ListBox owner)
            {
                _owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            /// <summary>
            ///  Number of current selected items.
            /// </summary>
            [Browsable(false)]
            public int Count
            {
                get
                {
                    return _owner.SelectedItems.Count;
                }
            }

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
                    return true;
                }
            }

            bool IList.IsFixedSize
            {
                get
                {
                    return true;
                }
            }

            public bool IsReadOnly
            {
                get
                {
                    return true;
                }
            }

            public bool Contains(int selectedIndex)
            {
                return IndexOf(selectedIndex) != -1;
            }

            bool IList.Contains(object selectedIndex)
            {
                if (selectedIndex is int)
                {
                    return Contains((int)selectedIndex);
                }
                else
                {
                    return false;
                }
            }

            public int IndexOf(int selectedIndex)
            {
                // Just what does this do?  The selectedIndex parameter above is the index into the
                // main object collection.  We look at the state of that item, and if the state indicates
                // that it is selected, we get back the virtualized index into this collection.  Indexes on
                // this collection match those on the SelectedObjectCollection.
                if (selectedIndex >= 0 &&
                    selectedIndex < InnerArray.GetCount(0) &&
                    InnerArray.GetState(selectedIndex, SelectedObjectCollection.SelectedObjectMask))
                {
                    return InnerArray.IndexOf(InnerArray.GetItem(selectedIndex, 0), SelectedObjectCollection.SelectedObjectMask);
                }

                return -1;
            }

            int IList.IndexOf(object selectedIndex)
            {
                if (selectedIndex is int)
                {
                    return IndexOf((int)selectedIndex);
                }
                else
                {
                    return -1;
                }
            }

            int IList.Add(object value)
            {
                throw new NotSupportedException(SR.ListBoxSelectedIndexCollectionIsReadOnly);
            }

            void IList.Clear()
            {
                throw new NotSupportedException(SR.ListBoxSelectedIndexCollectionIsReadOnly);
            }

            void IList.Insert(int index, object value)
            {
                throw new NotSupportedException(SR.ListBoxSelectedIndexCollectionIsReadOnly);
            }

            void IList.Remove(object value)
            {
                throw new NotSupportedException(SR.ListBoxSelectedIndexCollectionIsReadOnly);
            }

            void IList.RemoveAt(int index)
            {
                throw new NotSupportedException(SR.ListBoxSelectedIndexCollectionIsReadOnly);
            }

            /// <summary>
            ///  Retrieves the specified selected item.
            /// </summary>
            public int this[int index]
            {
                get
                {
                    object identifier = InnerArray.GetEntryObject(index, SelectedObjectCollection.SelectedObjectMask);
                    return InnerArray.IndexOfIdentifier(identifier, 0);
                }
            }

            object IList.this[int index]
            {
                get
                {
                    return this[index];
                }
                set
                {
                    throw new NotSupportedException(SR.ListBoxSelectedIndexCollectionIsReadOnly);
                }
            }

            /// <summary>
            ///  This is the item array that stores our data.  We share this backing store
            ///  with the main object collection.
            /// </summary>
            private ItemArray InnerArray
            {
                get
                {
                    _owner.SelectedItems.EnsureUpToDate();
                    return ((ObjectCollection)_owner.Items).InnerArray;
                }
            }

            public void CopyTo(Array destination, int index)
            {
                int cnt = Count;
                for (int i = 0; i < cnt; i++)
                {
                    destination.SetValue(this[i], i + index);
                }
            }

            public void Clear()
            {
                if (_owner != null)
                {
                    _owner.ClearSelected();
                }
            }

            public void Add(int index)
            {
                if (_owner != null)
                {
                    ObjectCollection items = _owner.Items;
                    if (items != null)
                    {
                        if (index != -1 && !Contains(index))
                        {
                            _owner.SetSelected(index, true);
                        }
                    }
                }
            }

            public void Remove(int index)
            {
                if (_owner != null)
                {
                    ObjectCollection items = _owner.Items;
                    if (items != null)
                    {
                        if (index != -1 && Contains(index))
                        {
                            _owner.SetSelected(index, false);
                        }
                    }
                }
            }

            public IEnumerator GetEnumerator()
            {
                return new SelectedIndexEnumerator(this);
            }

            /// <summary>
            ///  EntryEnumerator is an enumerator that will enumerate over
            ///  a given state mask.
            /// </summary>
            private class SelectedIndexEnumerator : IEnumerator
            {
                private readonly SelectedIndexCollection items;
                private int current;

                /// <summary>
                ///  Creates a new enumerator that will enumerate over the given state.
                /// </summary>
                public SelectedIndexEnumerator(SelectedIndexCollection items)
                {
                    this.items = items;
                    current = -1;
                }

                /// <summary>
                ///  Moves to the next element, or returns false if at the end.
                /// </summary>
                bool IEnumerator.MoveNext()
                {
                    if (current < items.Count - 1)
                    {
                        current++;
                        return true;
                    }
                    else
                    {
                        current = items.Count;
                        return false;
                    }
                }

                /// <summary>
                ///  Resets the enumeration back to the beginning.
                /// </summary>
                void IEnumerator.Reset()
                {
                    current = -1;
                }

                /// <summary>
                ///  Retrieves the current value in the enumerator.
                /// </summary>
                object IEnumerator.Current
                {
                    get
                    {
                        if (current == -1 || current == items.Count)
                        {
                            throw new InvalidOperationException(SR.ListEnumCurrentOutOfRange);
                        }

                        return items[current];
                    }
                }
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows.Forms
{
    public partial class DataGridViewComboBoxCell : DataGridViewCell
    {
        /// <summary>
        ///  A collection that stores objects.
        /// </summary>
        [ListBindable(false)]
        public class ObjectCollection : IList
        {
            private readonly DataGridViewComboBoxCell owner;
            private List<object> items;
            private IComparer<object> comparer;

            public ObjectCollection(DataGridViewComboBoxCell owner)
            {
                Debug.Assert(owner is not null);
                this.owner = owner;
            }

            private IComparer<object> Comparer
            {
                get
                {
                    comparer ??= new ItemComparer(owner);

                    return comparer;
                }
            }

            /// <summary>
            ///  Retrieves the number of items.
            /// </summary>
            public int Count
            {
                get
                {
                    return InnerArray.Count;
                }
            }

            /// <summary>
            ///  Internal access to the actual data store.
            /// </summary>
            internal List<object> InnerArray
            {
                get
                {
                    items ??= new List<object>();

                    return items;
                }
            }

            object ICollection.SyncRoot => ((ICollection)InnerArray).SyncRoot;

            bool ICollection.IsSynchronized => ((ICollection)InnerArray).IsSynchronized;

            bool IList.IsFixedSize => ((IList)InnerArray).IsFixedSize;

            public bool IsReadOnly => ((IList)InnerArray).IsReadOnly;

            /// <summary>
            ///  Adds an item to the collection. For an unsorted combo box, the item is
            ///  added to the end of the existing list of items. For a sorted combo box,
            ///  the item is inserted into the list according to its sorted position.
            ///  The item's ToString() method is called to obtain the string that is
            ///  displayed in the combo box.
            /// </summary>
            public int Add(object item)
            {
                //this.owner.CheckNoSharedCell();
                owner.CheckNoDataSource();

                ArgumentNullException.ThrowIfNull(item);

                int index = ((IList)InnerArray).Add(item);

                bool success = false;
                if (owner.Sorted)
                {
                    try
                    {
                        InnerArray.Sort(Comparer);
                        index = InnerArray.IndexOf(item);
                        success = true;
                    }
                    finally
                    {
                        if (!success)
                        {
                            InnerArray.Remove(item);
                        }
                    }
                }

                owner.OnItemsCollectionChanged();
                return index;
            }

            int IList.Add(object item)
            {
                return Add(item);
            }

            public void AddRange(params object[] items)
            {
                //this.owner.CheckNoSharedCell();
                owner.CheckNoDataSource();
                AddRangeInternal(items);
                owner.OnItemsCollectionChanged();
            }

            public void AddRange(ObjectCollection value)
            {
                //this.owner.CheckNoSharedCell();
                owner.CheckNoDataSource();
                AddRangeInternal((ICollection<object>)value);
                owner.OnItemsCollectionChanged();
            }

            /// <summary>
            ///  Add range that bypasses the data source check.
            /// </summary>
            internal void AddRangeInternal(ICollection<object> items)
            {
                ArgumentNullException.ThrowIfNull(items);

                foreach (object item in items)
                {
                    if (item is null)
                    {
                        throw new InvalidOperationException(SR.InvalidNullItemInCollection);
                    }
                }

                // Add everything to the collection first, then sort
                InnerArray.AddRange(items);
                if (owner.Sorted)
                {
                    InnerArray.Sort(Comparer);
                }
            }

            internal void SortInternal()
            {
                InnerArray.Sort(Comparer);
            }

            /// <summary>
            ///  Retrieves the item with the specified index.
            /// </summary>
            public virtual object this[int index]
            {
                get
                {
                    if (index < 0 || index >= InnerArray.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    return InnerArray[index];
                }
                set
                {
                    //this.owner.CheckNoSharedCell();
                    owner.CheckNoDataSource();

                    if (index < 0 || index >= InnerArray.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    InnerArray[index] = value.OrThrowIfNull();
                    owner.OnItemsCollectionChanged();
                }
            }

            /// <summary>
            ///  Removes all items from the collection.
            /// </summary>
            public void Clear()
            {
                if (InnerArray.Count > 0)
                {
                    //this.owner.CheckNoSharedCell();
                    owner.CheckNoDataSource();
                    InnerArray.Clear();
                    owner.OnItemsCollectionChanged();
                }
            }

            internal void ClearInternal()
            {
                InnerArray.Clear();
            }

            public bool Contains(object value)
            {
                return IndexOf(value) != -1;
            }

            /// <summary>
            ///  Copies the DataGridViewComboBoxCell Items collection to a destination array.
            /// </summary>
            public void CopyTo(object[] destination, int arrayIndex)
            {
                ((ICollection)InnerArray).CopyTo(destination, arrayIndex);
            }

            void ICollection.CopyTo(Array destination, int index)
            {
                ((ICollection)InnerArray).CopyTo(destination, index);
            }

            /// <summary>
            ///  Returns an enumerator for the DataGridViewComboBoxCell Items collection.
            /// </summary>
            public IEnumerator GetEnumerator()
            {
                return InnerArray.GetEnumerator();
            }

            public int IndexOf(object value)
            {
                ArgumentNullException.ThrowIfNull(value);

                return InnerArray.IndexOf(value);
            }

            /// <summary>
            ///  Adds an item to the collection. For an unsorted combo box, the item is
            ///  added to the end of the existing list of items. For a sorted combo box,
            ///  the item is inserted into the list according to its sorted position.
            ///  The item's toString() method is called to obtain the string that is
            ///  displayed in the combo box.
            /// </summary>
            public void Insert(int index, object item)
            {
                //this.owner.CheckNoSharedCell();
                owner.CheckNoDataSource();

                ArgumentNullException.ThrowIfNull(item);

                if (index < 0 || index > InnerArray.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), nameof(index)));
                }

                // If the combo box is sorted, then just treat this like an add
                // because we are going to twiddle the index anyway.
                if (owner.Sorted)
                {
                    Add(item);
                }
                else
                {
                    InnerArray.Insert(index, item);
                    owner.OnItemsCollectionChanged();
                }
            }

            /// <summary>
            ///  Removes the given item from the collection, provided that it is
            ///  actually in the list.
            /// </summary>
            public void Remove(object value)
            {
                int index = InnerArray.IndexOf(value);

                if (index != -1)
                {
                    RemoveAt(index);
                }
            }

            /// <summary>
            ///  Removes an item from the collection at the given index.
            /// </summary>
            public void RemoveAt(int index)
            {
                //this.owner.CheckNoSharedCell();
                owner.CheckNoDataSource();

                if (index < 0 || index >= InnerArray.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                InnerArray.RemoveAt(index);
                owner.OnItemsCollectionChanged();
            }
        } // end ObjectCollection
    }
}

﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a collection of selected <see cref="DataGridViewCell"/> objects in the <see cref="DataGridView"/>
    ///  control.
    /// </summary>
    [ListBindable(false)]
    public class DataGridViewSelectedRowCollection : BaseCollection, IList
    {
        private readonly List<DataGridViewRow> _items = new();

        int IList.Add(object value)
        {
            throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);
        }

        void IList.Clear()
        {
            throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);
        }

        bool IList.Contains(object value)
        {
            return value switch
            {
                DataGridViewRow dataGridViewRow => Contains(dataGridViewRow),
                _ => false,
            };
        }

        int IList.IndexOf(object value)
        {
            return value switch
            {
                DataGridViewRow dataGridViewRow => _items.IndexOf(dataGridViewRow),
                _ => -1,
            };
        }

        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);
        }

        bool IList.IsFixedSize
        {
            get { return true; }
        }

        bool IList.IsReadOnly
        {
            get { return true; }
        }

        object IList.this[int index]
        {
            get { return _items[index]; }
            set { throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection); }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_items).CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return _items.Count; }
        }

        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get { return this; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        internal DataGridViewSelectedRowCollection()
        {
        }

        protected override ArrayList List
        {
            get
            {
                return ArrayList.Adapter(_items);
            }
        }

        public DataGridViewRow this[int index]
        {
            get
            {
                return _items[index];
            }
        }

        /// <summary>
        ///  Adds a <see cref="DataGridViewCell"/> to this collection.
        /// </summary>
        internal int Add(DataGridViewRow dataGridViewRow)
        {
            _items.Add(dataGridViewRow);

            return _items.Count - 1;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Clear()
        {
            throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);
        }

        /// <summary>
        ///  Checks to see if a DataGridViewCell is contained in this collection.
        /// </summary>
        public bool Contains(DataGridViewRow dataGridViewRow)
        {
            return _items.IndexOf(dataGridViewRow) != -1;
        }

        public void CopyTo(DataGridViewRow[] array, int index)
        {
            _items.CopyTo(array, index);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Insert(int index, DataGridViewRow dataGridViewRow)
        {
            throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);
        }
    }
}

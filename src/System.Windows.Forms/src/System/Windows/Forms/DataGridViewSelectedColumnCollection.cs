﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
    [ListBindable(false)]
    public class DataGridViewSelectedColumnCollection : BaseCollection, IList
    {
        private readonly List<DataGridViewColumn> _items = new();

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
                DataGridViewColumn dataGridViewColumn => Contains(dataGridViewColumn),
                _ => false,
            };
        }

        int IList.IndexOf(object value)
        {
            return value switch
            {
                DataGridViewColumn dataGridViewColumn => _items.IndexOf(dataGridViewColumn),
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

        internal DataGridViewSelectedColumnCollection()
        {
        }

        protected override ArrayList List
        {
            get
            {
                return ArrayList.Adapter(_items);
            }
        }

        public DataGridViewColumn this[int index]
        {
            get
            {
                return _items[index];
            }
        }

        /// <summary>
        ///  Adds a <see cref="DataGridViewCell"/> to this collection.
        /// </summary>
        internal int Add(DataGridViewColumn dataGridViewColumn)
        {
            _items.Add(dataGridViewColumn);
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
        public bool Contains(DataGridViewColumn dataGridViewColumn)
        {
            return _items.IndexOf(dataGridViewColumn) != -1;
        }

        public void CopyTo(DataGridViewColumn[] array, int index)
        {
            _items.CopyTo(array, index);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Insert(int index, DataGridViewColumn dataGridViewColumn)
        {
            throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);
        }
    }
}

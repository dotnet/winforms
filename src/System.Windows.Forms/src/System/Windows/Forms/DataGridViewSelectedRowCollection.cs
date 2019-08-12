// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a collection of selected <see cref='DataGridViewCell'/> objects in the <see cref='DataGridView'/>
    ///  control.
    /// </summary>
    [ListBindable(false)]
    public class DataGridViewSelectedRowCollection : BaseCollection, IList
    {
        readonly ArrayList items = new ArrayList();

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
            return items.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return items.IndexOf(value);
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
            get { return items[index]; }
            set { throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection); }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            items.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return items.Count; }
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
            return items.GetEnumerator();
        }

        internal DataGridViewSelectedRowCollection()
        {
        }

        protected override ArrayList List
        {
            get
            {
                return items;
            }
        }

        public DataGridViewRow this[int index]
        {
            get
            {
                return (DataGridViewRow)items[index];
            }
        }

        /// <summary>
        ///  Adds a <see cref='DataGridViewCell'/> to this collection.
        /// </summary>
        internal int Add(DataGridViewRow dataGridViewRow)
        {
            return items.Add(dataGridViewRow);
        }

        /* Unused at this point
        internal void AddRange(DataGridViewRow[] dataGridViewRows)
        {
            Debug.Assert(dataGridViewRows != null);
            foreach(DataGridViewRow dataGridViewRow in dataGridViewRows)
            {
                this.items.Add(dataGridViewRow);
            }
        }

        internal void AddRowCollection(DataGridViewRowCollection dataGridViewRows)
        {
            Debug.Assert(dataGridViewRows != null);
            foreach(DataGridViewRow dataGridViewRow in dataGridViewRows)
            {
                this.items.Add(dataGridViewRow);
            }
        }
        */

        [
            EditorBrowsable(EditorBrowsableState.Never)
        ]
        public void Clear()
        {
            throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);
        }

        /// <summary>
        ///  Checks to see if a DataGridViewCell is contained in this collection.
        /// </summary>
        public bool Contains(DataGridViewRow dataGridViewRow)
        {
            return items.IndexOf(dataGridViewRow) != -1;
        }

        public void CopyTo(DataGridViewRow[] array, int index)
        {
            items.CopyTo(array, index);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Insert(int index, DataGridViewRow dataGridViewRow)
        {
            throw new NotSupportedException(SR.DataGridView_ReadOnlyCollection);
        }
    }
}

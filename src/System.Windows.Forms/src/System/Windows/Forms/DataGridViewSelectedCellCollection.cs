// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System.Diagnostics;
    using System;
    using System.Collections;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Globalization;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// <para>Represents a collection of selected <see cref='System.Windows.Forms.DataGridViewCell'/> objects in the <see cref='System.Windows.Forms.DataGridView'/> 
    /// control.</para>
    /// </summary>
    [
        ListBindable(false),
        SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface") // Consider adding an IList<DataGridViewSelectedCellCollection> implementation
    ]
    public class DataGridViewSelectedCellCollection : BaseCollection, IList
    {
        ArrayList items = new ArrayList();

        int IList.Add(object value)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        void IList.Clear()
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
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
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        void IList.Remove(object value)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
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
            set { throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection)); }
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

        internal DataGridViewSelectedCellCollection()
        {
        }

        protected override ArrayList List
        {
            get
            {
                return items;
            }
        }

        public DataGridViewCell this[int index]
        {
            get
            {
                return (DataGridViewCell)items[index];
            }
        }

        /// <summary>
        /// <para>Adds a <see cref='System.Windows.Forms.DataGridViewCell'/> to this collection.</para>
        /// </summary>
        internal int Add(DataGridViewCell dataGridViewCell)
        {
            Debug.Assert(!Contains(dataGridViewCell));
            return items.Add(dataGridViewCell);
        }

        /* Not used for now
        internal void AddRange(DataGridViewCell[] dataGridViewCells)
        {
            Debug.Assert(dataGridViewCells != null);
            foreach(DataGridViewCell dataGridViewCell in dataGridViewCells) 
            {
                Debug.Assert(!Contains(dataGridViewCell));
                this.items.Add(dataGridViewCell);
            }
        }

        internal void AddCellCollection(DataGridViewSelectedCellCollection dataGridViewCells)
        {
            Debug.Assert(dataGridViewCells != null);
            foreach(DataGridViewCell dataGridViewCell in dataGridViewCells) 
            {
                Debug.Assert(!Contains(dataGridViewCell));
                this.items.Add(dataGridViewCell);
            }
        }
        */

        /// <summary>
        /// <para>Adds all the <see cref='System.Windows.Forms.DataGridViewCell'/> objects from the provided linked list to this collection.</para>
        /// </summary>
        internal void AddCellLinkedList(DataGridViewCellLinkedList dataGridViewCells)
        {
            Debug.Assert(dataGridViewCells != null);
            foreach (DataGridViewCell dataGridViewCell in dataGridViewCells)
            {
                Debug.Assert(!Contains(dataGridViewCell));
                items.Add(dataGridViewCell);
            }
        }

        [
            EditorBrowsable(EditorBrowsableState.Never)
        ]
        public void Clear()
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        /// <summary>
        ///      Checks to see if a DataGridViewCell is contained in this collection.
        /// </summary>
        public bool Contains(DataGridViewCell dataGridViewCell)
        {
            return items.IndexOf(dataGridViewCell) != -1;
        }

        public void CopyTo(DataGridViewCell[] array, int index)
        {
            items.CopyTo(array, index);
        }

        [
            EditorBrowsable(EditorBrowsableState.Never),
            SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")
        ]
        public void Insert(int index, DataGridViewCell dataGridViewCell)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }
    }
}

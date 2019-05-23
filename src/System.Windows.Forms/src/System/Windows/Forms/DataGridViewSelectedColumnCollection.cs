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

    [
        ListBindable(false),
        SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface") // Consider adding an IList<DataGridViewSelectedColumnCollection> implementation
    ]
    public class DataGridViewSelectedColumnCollection : BaseCollection, IList
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
            return this.items.Contains(value);
        }

        int IList.IndexOf(object value)
        {
            return this.items.IndexOf(value);
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
            get { return this.items[index]; }
            set { throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection)); }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            this.items.CopyTo(array, index);
        }

        int ICollection.Count
        {
            get { return this.items.Count; }
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
            return this.items.GetEnumerator();
        }

        internal DataGridViewSelectedColumnCollection()
        {
        }

        protected override ArrayList List
        {
            get
            {
                return this.items;
            }
        }

        public DataGridViewColumn this[int index]
        {
            get
            {
                return (DataGridViewColumn)this.items[index];
            }
        }

        /// <summary>
        /// <para>Adds a <see cref='System.Windows.Forms.DataGridViewCell'/> to this collection.</para>
        /// </summary>
        internal int Add(DataGridViewColumn dataGridViewColumn)
        {
            return this.items.Add(dataGridViewColumn);
        }

        /* Unused at this point
        internal void AddRange(DataGridViewColumn[] dataGridViewColumns)
        {
            Debug.Assert(dataGridViewColumns != null);
            foreach(DataGridViewColumn dataGridViewColumn in dataGridViewColumns) 
            {
                this.items.Add(dataGridViewColumn);
            }
        }
        */

        /* Unused at this point
        internal void AddColumnCollection(DataGridViewColumnCollection dataGridViewColumns)
        {
            Debug.Assert(dataGridViewColumns != null);
            foreach(DataGridViewColumn dataGridViewColumn in dataGridViewColumns) 
            {
                this.items.Add(dataGridViewColumn);
            }
        }
        */

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
        public bool Contains(DataGridViewColumn dataGridViewColumn)
        {
            return this.items.IndexOf(dataGridViewColumn) != -1;
        }

        public void CopyTo(DataGridViewColumn[] array, int index)
        {
            this.items.CopyTo(array, index);
        }

        [
            EditorBrowsable(EditorBrowsableState.Never),
            SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")
        ]
        public void Insert(int index, DataGridViewColumn dataGridViewColumn)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }
    }
}

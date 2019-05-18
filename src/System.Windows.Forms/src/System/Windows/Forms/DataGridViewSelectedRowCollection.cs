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
    /// </devdoc>
    [
        ListBindable(false),
        SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface") // Consider adding an IList<DataGridViewSelectedRowCollection> implementation
    ]
    public class DataGridViewSelectedRowCollection : BaseCollection, IList
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

        internal DataGridViewSelectedRowCollection()
        {
        }

        protected override ArrayList List
        {
            get
            {
                return this.items;
            }
        }
        
        public DataGridViewRow this[int index]
        {
            get
            {
                return (DataGridViewRow) this.items[index];
            }
        }

        /// <summary>
        /// <para>Adds a <see cref='System.Windows.Forms.DataGridViewCell'/> to this collection.</para>
        /// </devdoc>
        internal int Add(DataGridViewRow dataGridViewRow)
        {
            return this.items.Add(dataGridViewRow);
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
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        /// <summary>
        ///      Checks to see if a DataGridViewCell is contained in this collection.
        /// </devdoc>
        public bool Contains(DataGridViewRow dataGridViewRow)
        {
            return this.items.IndexOf(dataGridViewRow) != -1;
        }

        public void CopyTo(DataGridViewRow[] array, int index)
        {
            this.items.CopyTo(array, index);
        }

        [
            EditorBrowsable(EditorBrowsableState.Never),
            SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")
        ]
        public void Insert(int index, DataGridViewRow dataGridViewRow)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }
    }
}

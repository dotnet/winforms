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

    /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection"]/*' />
    /// <devdoc>
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

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.IList.Add"]/*' />
        /// <internalonly/>
        int IList.Add(object value)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.IList.Clear"]/*' />
        /// <internalonly/>
        void IList.Clear()
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.IList.Contains"]/*' />
        /// <internalonly/>
        bool IList.Contains(object value)
        {
            return this.items.Contains(value);
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.IList.IndexOf"]/*' />
        /// <internalonly/>
        int IList.IndexOf(object value)
        {
            return this.items.IndexOf(value);
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.IList.Insert"]/*' />
        /// <internalonly/>
        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.IList.Remove"]/*' />
        /// <internalonly/>
        void IList.Remove(object value)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.IList.RemoveAt"]/*' />
        /// <internalonly/>
        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.IList.IsFixedSize"]/*' />
        /// <internalonly/>
        bool IList.IsFixedSize
        {
            get { return true; }
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.IList.IsReadOnly"]/*' />
        /// <internalonly/>
        bool IList.IsReadOnly
        {
            get { return true; }
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.IList.this"]/*' />
        /// <internalonly/>
        object IList.this[int index]
        {
            get { return this.items[index]; }
            set { throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection)); }
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.ICollection.CopyTo"]/*' />
        /// <internalonly/>
        void ICollection.CopyTo(Array array, int index)
        {
            this.items.CopyTo(array, index);
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.ICollection.Count"]/*' />
        /// <internalonly/>
        int ICollection.Count
        {
            get { return this.items.Count; }
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.ICollection.IsSynchronized"]/*' />
        /// <internalonly/>
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.ICollection.SyncRoot"]/*' />
        /// <internalonly/>
        object ICollection.SyncRoot
        {
            get { return this; }
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.IEnumerable.GetEnumerator"]/*' />
        /// <internalonly/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        internal DataGridViewSelectedRowCollection()
        {
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.List"]/*' />
        protected override ArrayList List
        {
            get
            {
                return this.items;
            }
        }
        
        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.this"]/*' />
        public DataGridViewRow this[int index]
        {
            get
            {
                return (DataGridViewRow) this.items[index];
            }
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.Add"]/*' />
        /// <devdoc>
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

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.Clear"]/*' />
        [
            EditorBrowsable(EditorBrowsableState.Never)
        ]
        public void Clear()
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.Contains"]/*' />
        /// <devdoc>
        ///      Checks to see if a DataGridViewCell is contained in this collection.
        /// </devdoc>
        public bool Contains(DataGridViewRow dataGridViewRow)
        {
            return this.items.IndexOf(dataGridViewRow) != -1;
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.CopyTo"]/*' />
        public void CopyTo(DataGridViewRow[] array, int index)
        {
            this.items.CopyTo(array, index);
        }

        /// <include file='doc\DataGridViewSelectedRowCollection.uex' path='docs/doc[@for="DataGridViewSelectedRowCollection.Insert"]/*' />
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

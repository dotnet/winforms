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

    /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection"]/*' />
    [
        ListBindable(false),
        SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface") // Consider adding an IList<DataGridViewSelectedColumnCollection> implementation
    ]
    public class DataGridViewSelectedColumnCollection : BaseCollection, IList
    {
        ArrayList items = new ArrayList();

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.IList.Add"]/*' />
        /// <internalonly/>
        int IList.Add(object value)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.IList.Clear"]/*' />
        /// <internalonly/>
        void IList.Clear()
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.IList.Contains"]/*' />
        /// <internalonly/>
        bool IList.Contains(object value)
        {
            return this.items.Contains(value);
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.IList.IndexOf"]/*' />
        /// <internalonly/>
        int IList.IndexOf(object value)
        {
            return this.items.IndexOf(value);
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.IList.Insert"]/*' />
        /// <internalonly/>
        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.IList.Remove"]/*' />
        /// <internalonly/>
        void IList.Remove(object value)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.IList.RemoveAt"]/*' />
        /// <internalonly/>
        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.IList.IsFixedSize"]/*' />
        /// <internalonly/>
        bool IList.IsFixedSize
        {
            get { return true; }
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.IList.IsReadOnly"]/*' />
        /// <internalonly/>
        bool IList.IsReadOnly
        {
            get { return true; }
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.IList.this"]/*' />
        /// <internalonly/>
        object IList.this[int index]
        {
            get { return this.items[index]; }
            set { throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection)); }
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.ICollection.CopyTo"]/*' />
        /// <internalonly/>
        void ICollection.CopyTo(Array array, int index)
        {
            this.items.CopyTo(array, index);
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.ICollection.Count"]/*' />
        /// <internalonly/>
        int ICollection.Count
        {
            get { return this.items.Count; }
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.ICollection.IsSynchronized"]/*' />
        /// <internalonly/>
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.ICollection.SyncRoot"]/*' />
        /// <internalonly/>
        object ICollection.SyncRoot
        {
            get { return this; }
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.IEnumerable.GetEnumerator"]/*' />
        /// <internalonly/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        internal DataGridViewSelectedColumnCollection()
        {
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.List"]/*' />
        protected override ArrayList List
        {
            get
            {
                return this.items;
            }
        }
        
        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.this"]/*' />
        public DataGridViewColumn this[int index]
        {
            get
            {
                return (DataGridViewColumn) this.items[index];
            }
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.Add"]/*' />
        /// <devdoc>
        /// <para>Adds a <see cref='System.Windows.Forms.DataGridViewCell'/> to this collection.</para>
        /// </devdoc>
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

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.Clear"]/*' />
        [
            EditorBrowsable(EditorBrowsableState.Never)
        ]
        public void Clear()
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.Contains"]/*' />
        /// <devdoc>
        ///      Checks to see if a DataGridViewCell is contained in this collection.
        /// </devdoc>
        public bool Contains(DataGridViewColumn dataGridViewColumn)
        {
            return this.items.IndexOf(dataGridViewColumn) != -1;
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.CopyTo"]/*' />
        public void CopyTo(DataGridViewColumn[] array, int index)
        {
            this.items.CopyTo(array, index);
        }

        /// <include file='doc\DataGridViewSelectedColumnCollection.uex' path='docs/doc[@for="DataGridViewSelectedColumnCollection.Insert"]/*' />
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

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


    /// <devdoc>
    /// <para>Represents a collection of selected <see cref='System.Windows.Forms.DataGridViewCell'/> objects in the <see cref='System.Windows.Forms.DataGridView'/> 
    /// control.</para>
    /// </devdoc>
    [
        ListBindable(false),
        SuppressMessage("Microsoft.Design", "CA1010:CollectionsShouldImplementGenericInterface") // Consider adding an IList<DataGridViewSelectedCellCollection> implementation
    ]
    public class DataGridViewSelectedCellCollection : BaseCollection, IList
    {
        ArrayList items = new ArrayList();


        /// <internalonly/>
        int IList.Add(object value)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }


        /// <internalonly/>
        void IList.Clear()
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }


        /// <internalonly/>
        bool IList.Contains(object value)
        {
            return this.items.Contains(value);
        }


        /// <internalonly/>
        int IList.IndexOf(object value)
        {
            return this.items.IndexOf(value);
        }


        /// <internalonly/>
        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }


        /// <internalonly/>
        void IList.Remove(object value)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }


        /// <internalonly/>
        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }


        /// <internalonly/>
        bool IList.IsFixedSize
        {
            get { return true; }
        }


        /// <internalonly/>
        bool IList.IsReadOnly
        {
            get { return true; }
        }


        /// <internalonly/>
        object IList.this[int index]
        {
            get { return this.items[index]; }
            set { throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection)); }
        }


        /// <internalonly/>
        void ICollection.CopyTo(Array array, int index)
        {
            this.items.CopyTo(array, index);
        }


        /// <internalonly/>
        int ICollection.Count
        {
            get { return this.items.Count; }
        }


        /// <internalonly/>
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }


        /// <internalonly/>
        object ICollection.SyncRoot
        {
            get { return this; }
        }


        /// <internalonly/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.items.GetEnumerator();
        }

        internal DataGridViewSelectedCellCollection()
        {
        }


        protected override ArrayList List
        {
            get
            {
                return this.items;
            }
        }
        

        public DataGridViewCell this[int index]
        {
            get
            {
                return (DataGridViewCell) this.items[index];
            }
        }


        /// <devdoc>
        /// <para>Adds a <see cref='System.Windows.Forms.DataGridViewCell'/> to this collection.</para>
        /// </devdoc>
        internal int Add(DataGridViewCell dataGridViewCell)
        {
            Debug.Assert(!Contains(dataGridViewCell));
            return this.items.Add(dataGridViewCell);
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


        /// <devdoc>
        /// <para>Adds all the <see cref='System.Windows.Forms.DataGridViewCell'/> objects from the provided linked list to this collection.</para>
        /// </devdoc>
        internal void AddCellLinkedList(DataGridViewCellLinkedList dataGridViewCells)
        {
            Debug.Assert(dataGridViewCells != null);
            foreach (DataGridViewCell dataGridViewCell in dataGridViewCells)
            {
                Debug.Assert(!Contains(dataGridViewCell));
                this.items.Add(dataGridViewCell);
            }
        }


        [
            EditorBrowsable(EditorBrowsableState.Never)
        ]
        public void Clear()
        {
            throw new NotSupportedException(string.Format(SR.DataGridView_ReadOnlyCollection));
        }


        /// <devdoc>
        ///      Checks to see if a DataGridViewCell is contained in this collection.
        /// </devdoc>
        public bool Contains(DataGridViewCell dataGridViewCell)
        {
            return this.items.IndexOf(dataGridViewCell) != -1;
        }


        public void CopyTo(DataGridViewCell[] array, int index)
        {
            this.items.CopyTo(array, index);
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

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Collections;
    using System.Security.Permissions;
    using System.ComponentModel;

    /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection"]/*' />
    /// <devdoc>
    ///    <para>Represents a collection of strings.</para>
    /// </devdoc>
    public class AutoCompleteStringCollection : IList {

        CollectionChangeEventHandler onCollectionChanged;
        private ArrayList data = new ArrayList();
        
        public AutoCompleteStringCollection()
        {
            
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.this"]/*' />
        /// <devdoc>
        /// <para>Represents the entry at the specified index of the <see cref='System.Collections.Specialized.AutoCompleteStringCollection'/>.</para>
        /// </devdoc>
        public string this[int index] {
            get {
                return ((string)data[index]);
            }
            set {
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, data[index])); 
                data[index] = value;
                OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
            }
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.Count"]/*' />
        /// <devdoc>
        ///    <para>Gets the number of strings in the 
        ///    <see cref='System.Collections.Specialized.AutoCompleteStringCollection'/> .</para>
        /// </devdoc>
        public int Count {
            get {
                return data.Count;
            }
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.IList.IsReadOnly"]/*' />
        bool IList.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.IList.IsFixedSize"]/*' />
        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }


        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.CollectionChanged"]/*' />
        public event CollectionChangeEventHandler CollectionChanged
        {
            add
            {
                this.onCollectionChanged += value;
            }
            remove
            {
                this.onCollectionChanged -= value;
            }
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.OnCollectionChanged"]/*' />
        protected void OnCollectionChanged(CollectionChangeEventArgs e)
        {
            if (this.onCollectionChanged != null)
            {
                this.onCollectionChanged(this, e);
            }
        }


        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.Add"]/*' />
        /// <devdoc>
        ///    <para>Adds a string with the specified value to the 
        ///    <see cref='System.Collections.Specialized.AutoCompleteStringCollection'/> .</para>
        /// </devdoc>
        public int Add(string value) {
            int index =  data.Add(value);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
            return index;
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.AddRange"]/*' />
        /// <devdoc>
        /// <para>Copies the elements of a string array to the end of the <see cref='System.Collections.Specialized.AutoCompleteStringCollection'/>.</para>
        /// </devdoc>
        public void AddRange(string[] value) {
            if (value == null) {
                throw new ArgumentNullException(nameof(value));
            }
            data.AddRange(value);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.Clear"]/*' />
        /// <devdoc>
        ///    <para>Removes all the strings from the 
        ///    <see cref='System.Collections.Specialized.AutoCompleteStringCollection'/> .</para>
        /// </devdoc>
        public void Clear() {
            data.Clear();
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.Contains"]/*' />
        /// <devdoc>
        ///    <para>Gets a value indicating whether the 
        ///    <see cref='System.Collections.Specialized.AutoCompleteStringCollection'/> contains a string with the specified 
        ///       value.</para>
        /// </devdoc>
        public bool Contains(string value) {
            return data.Contains(value);
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.CopyTo"]/*' />
        /// <devdoc>
        /// <para>Copies the <see cref='System.Collections.Specialized.AutoCompleteStringCollection'/> values to a one-dimensional <see cref='System.Array'/> instance at the 
        ///    specified index.</para>
        /// </devdoc>
        public void CopyTo(string[] array, int index) {
            data.CopyTo(array, index);
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.IndexOf"]/*' />
        /// <devdoc>
        ///    <para>Returns the index of the first occurrence of a string in 
        ///       the <see cref='System.Collections.Specialized.AutoCompleteStringCollection'/> .</para>
        /// </devdoc>
        public int IndexOf(string value) {
            return data.IndexOf(value);
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.Insert"]/*' />
        /// <devdoc>
        /// <para>Inserts a string into the <see cref='System.Collections.Specialized.AutoCompleteStringCollection'/> at the specified 
        ///    index.</para>
        /// </devdoc>
        public void Insert(int index, string value) {
            data.Insert(index, value);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, value));
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.IsReadOnly"]/*' />
        /// <devdoc>
        /// <para>Gets a value indicating whether the <see cref='System.Collections.Specialized.AutoCompleteStringCollection'/> is read-only.</para>
        /// </devdoc>
        public bool IsReadOnly {
            get {
                return false;
            }
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.IsSynchronized"]/*' />
        /// <devdoc>
        ///    <para>Gets a value indicating whether access to the 
        ///    <see cref='System.Collections.Specialized.AutoCompleteStringCollection'/> 
        ///    is synchronized (thread-safe).</para>
        /// </devdoc>
        public bool IsSynchronized {
            get {
                return false;
            }
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.Remove"]/*' />
        /// <devdoc>
        ///    <para> Removes a specific string from the 
        ///    <see cref='System.Collections.Specialized.AutoCompleteStringCollection'/> .</para>
        /// </devdoc>
        public void Remove(string value) {
            data.Remove(value);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.RemoveAt"]/*' />
        /// <devdoc>
        /// <para>Removes the string at the specified index of the <see cref='System.Collections.Specialized.AutoCompleteStringCollection'/>.</para>
        /// </devdoc>
        public void RemoveAt(int index) {
            string value = (string)data[index];
            data.RemoveAt(index);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, value));
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.SyncRoot"]/*' />
        /// <devdoc>
        /// <para>Gets an object that can be used to synchronize access to the <see cref='System.Collections.Specialized.AutoCompleteStringCollection'/>.</para>
        /// </devdoc>
        public object SyncRoot {
            [HostProtection(Synchronization=true)]                
            [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
            get {
                return this;
            }
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.IList.this"]/*' />
        object IList.this[int index] {
            get {
                return this[index];
            }
            set {
                this[index] = (string)value;
            }
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.IList.Add"]/*' />
        int IList.Add(object value) {
            return Add((string)value);
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.IList.Contains"]/*' />
        bool IList.Contains(object value) {
            return Contains((string) value);
        }


        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.IList.IndexOf"]/*' />
        int IList.IndexOf(object value) {
            return IndexOf((string)value);
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.IList.Insert"]/*' />
        void IList.Insert(int index, object value) {
            Insert(index, (string)value);
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.IList.Remove"]/*' />
        void IList.Remove(object value) {
            Remove((string)value);
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.ICollection.CopyTo"]/*' />
        void ICollection.CopyTo(Array array, int index) {
            data.CopyTo(array, index);
        }

        /// <include file='doc\AutoCompleteStringCollection.uex' path='docs/doc[@for="AutoCompleteStringCollection.IEnumerable.GetEnumerator"]/*' />
        public IEnumerator GetEnumerator() {
            return data.GetEnumerator();
        }
 
    }
}

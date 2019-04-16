// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection"]/*' />
    /// <devdoc>
    ///    <para>
    ///         A collection of listview groups.
    ///    </para>
    /// </devdoc>
    [ListBindable(false)]
    public class ListViewGroupCollection : IList {

        private ListView listView;

        private ArrayList list;

        internal ListViewGroupCollection(ListView listView) {
            this.listView = listView;
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.Count"]/*' />
        public int Count
        {
            get
            {
                return this.List.Count;
            }
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.ICollection.SyncRoot"]/*' />
        /// <internalonly/>
        object ICollection.SyncRoot {
            get {
                return this;
            }
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.ICollection.IsSynchronized"]/*' />
        /// <internalonly/>
        bool ICollection.IsSynchronized {
            get {
                return true;
            }
        }
        
        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.IList.IsFixedSize"]/*' />
        /// <internalonly/>
        bool IList.IsFixedSize {
            get {
                return false;
            }
        }
        
        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.IList.IsReadOnly"]/*' />
        /// <internalonly/>
        bool IList.IsReadOnly {
            get {
                return false;
            }
        }

        private ArrayList List
        {
            get
            {
                if (list == null) {
                    list = new ArrayList();
                }
                return list;
            }
        }
                
        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.this"]/*' />
        public ListViewGroup this[int index] {
            get
            {
                return (ListViewGroup)this.List[index];
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (this.List.Contains(value)) {
                    return;
                }
                this.List[index] = value;
            }
        }
                
        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.this2"]/*' />
        public ListViewGroup this[string key] {
            get {

                if (list == null) {
                    return null;
                }

                for (int i = 0; i < list.Count; i ++) {
                    if (string.Compare(key, this[i].Name, false /*case insensitive*/, System.Globalization.CultureInfo.CurrentCulture) == 0) {
                        return this[i];
                    }
                }

                return null;
            }
            set {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                int index = -1;

                if (this.list == null) {
                    return; // nothing to do
                }

                for (int i = 0; i < this.list.Count; i ++) {
                    if (string.Compare(key, this[i].Name, false /*case insensitive*/, System.Globalization.CultureInfo.CurrentCulture) ==0) {
                        index = i;
                        break;
                    }
                }

                if (index != -1) {
                    this.list[index] = value;
                }
            }
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.IList.this"]/*' />
        /// <internalonly/>
        object IList.this[int index] {
            get
            {
                return this[index];
            }
            set
            {
                if (value is ListViewGroup) {
                    this[index] = (ListViewGroup)value;
                }
            }
        }
        
        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.Add"]/*' />
        public int Add(ListViewGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            if (this.Contains(group)) {
                return -1;
            }

            // Will throw InvalidOperationException if group contains items which are parented by another listView.
            CheckListViewItems(group);
            group.ListViewInternal = this.listView;
            int index = this.List.Add(group);
            if (listView.IsHandleCreated) {
                listView.InsertGroupInListView(this.List.Count, group);
                MoveGroupItems(group);
            }            
            return index;
        }        
        
        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.Add1"]/*' />
        public ListViewGroup Add(string key, string headerText)
        {
            ListViewGroup group = new ListViewGroup(key, headerText);
            this.Add(group);
            return group;
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.IList.Add"]/*' />
        /// <internalonly/>
        [SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
        [
            SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // "value" is the name of the param passed in.
                                                                                                        // So we don't have to localize it.
        ]
        int IList.Add(object value) {
            if (value is ListViewGroup) {
                return Add((ListViewGroup)value);
            }
            throw new ArgumentException(SR.ListViewGroupCollectionBadListViewGroup, nameof(value));
        }
                
        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.AddRange"]/*' />
        public void AddRange(ListViewGroup[] groups)
        {
            if (groups == null)
            {
                throw new ArgumentNullException(nameof(groups));
            }

            for(int i=0; i < groups.Length; i++) {
                Add(groups[i]);
            }
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.AddRange2"]/*' />
        public void AddRange(ListViewGroupCollection groups)
        {
            if (groups == null)
            {
                throw new ArgumentNullException(nameof(groups));
            }

            for(int i=0; i < groups.Count; i++) {
                Add(groups[i]);
            }
        }

        private void CheckListViewItems(ListViewGroup group) {
            for (int i = 0; i < group.Items.Count; i ++) {
                ListViewItem item = group.Items[i];
                if (item.ListView != null && item.ListView != this.listView) {
                    throw new ArgumentException(string.Format(SR.OnlyOneControl, item.Text));
                }
            }
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.Clear"]/*' />
        public void Clear() {
            if (listView.IsHandleCreated) {
                for(int i=0; i < Count; i++) {
                    listView.RemoveGroupFromListView(this[i]);
                }
            }
            // Dissociate groups from the ListView
            //
            for(int i=0; i < Count; i++) {
                this[i].ListViewInternal = null;
            }
            this.List.Clear();

            // we have to tell the listView that there are no more groups
            // so the list view knows to remove items from the default group
            this.listView.UpdateGroupView();
        }
        
        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.Contains"]/*' />
        public bool Contains(ListViewGroup value) {
            return this.List.Contains(value);
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.IList.Contains"]/*' />
        /// <internalonly/>
        bool IList.Contains(object value)
        {
            if (value is ListViewGroup) {
                return Contains((ListViewGroup)value);
            }
            return false;
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.CopyTo"]/*' />
        public void CopyTo(Array array, int index) {
            this.List.CopyTo(array, index);
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.GetEnumerator"]/*' />
        public IEnumerator GetEnumerator()
        {
            return this.List.GetEnumerator();
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.IndexOf"]/*' />
        public int IndexOf(ListViewGroup value) {
            return this.List.IndexOf(value);
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.IList.IndexOf"]/*' />
        /// <internalonly/>
        int IList.IndexOf(object value) {
            if (value is ListViewGroup) {
                return IndexOf((ListViewGroup)value);
            }
            return -1;
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.Insert"]/*' />
        public void Insert(int index, ListViewGroup group) {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            if (Contains(group)) {
                return;
            }
            group.ListViewInternal = this.listView;
            this.List.Insert(index, group);
            if (listView.IsHandleCreated) {
                listView.InsertGroupInListView(index, group);
                MoveGroupItems(group);
            }            
        }       

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.IList.Insert"]/*' />
        /// <internalonly/>
        void IList.Insert(int index, object value) {
            if (value is ListViewGroup) {
                Insert(index, (ListViewGroup)value);
            }            
        }

        private void MoveGroupItems(ListViewGroup group) {
            Debug.Assert(listView.IsHandleCreated, "MoveGroupItems pre-condition: listView handle must be created");

            foreach(ListViewItem item in group.Items) {
                if (item.ListView == this.listView) {
                    item.UpdateStateToListView(item.Index);                    
                }
            }
        }
       
        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.Remove"]/*' />
        public void Remove(ListViewGroup group) {
            group.ListViewInternal = null;            
            this.List.Remove(group);

            if (listView.IsHandleCreated) {
                listView.RemoveGroupFromListView(group);
            }
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.IList.Remove"]/*' />
        /// <internalonly/>
        void IList.Remove(object value)
        {
            if (value is ListViewGroup) {
                Remove((ListViewGroup)value);
            }
        }

        /// <include file='doc\ListViewGroup.uex' path='docs/doc[@for="ListViewGroup.ListViewGroupCollection.RemoveAt"]/*' />
        public void RemoveAt(int index) {
            Remove(this[index]);
        }        
    }        
}


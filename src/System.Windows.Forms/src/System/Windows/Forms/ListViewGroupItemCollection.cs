// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System;
using System.Security.Permissions;
using System.Drawing;
using System.Windows.Forms;    
using System.ComponentModel.Design;
using System.Collections;
using Microsoft.Win32;
using System.Globalization;

namespace System.Windows.Forms
{
    // A collection of items in a ListViewGroup.
    //
    internal class ListViewGroupItemCollection : ListView.ListViewItemCollection.IInnerList {
        private ListViewGroup group;
        private ArrayList items;

        public ListViewGroupItemCollection(ListViewGroup group) {
            this.group = group;
        }  

        public int Count
        {
            get
            {
                return Items.Count;
            }
        }

        private ArrayList Items
        {
            get
            {
                if (items == null) {
                    items = new ArrayList();
                }
                return items;
            }
        }

        public bool OwnerIsVirtualListView {
            get {
                if (this.group.ListView != null) {
                    return group.ListView.VirtualMode;
                } else {
                    return false;
                }
            }
        }

        public bool OwnerIsDesignMode {
            get {
                if (this.group.ListView != null) {
                    ISite s = group.ListView.Site;
                    return(s == null) ? false : s.DesignMode;
                } else {
                    return false;
                }
            }
        }

        public ListViewItem this[int index] {
            get {
                return (ListViewItem)Items[index];
            }
            set {
                if (value != Items[index]) {
                    MoveToGroup((ListViewItem)Items[index], null);                
                    Items[index] = value;
                    MoveToGroup((ListViewItem)Items[index], this.group);
                }
            }
        }
        
        public ListViewItem Add(ListViewItem value) {          
            CheckListViewItem(value);

            MoveToGroup(value, this.group);
            Items.Add(value);
            return value;
        }
        
        public void AddRange(ListViewItem[] items) {
            for (int i = 0; i < items.Length; i ++) {
                CheckListViewItem(items[i]);
            }

            Items.AddRange(items);
        
            for(int i=0; i < items.Length; i++) {
                MoveToGroup(items[i], this.group);
            }
        }

        /// <devdoc>
        ///     throws an ArgumentException if the listViewItem is in another listView already.
        /// </devdoc>
        private void CheckListViewItem(ListViewItem item) {
            if (item.ListView != null && item.ListView != this.group.ListView) {
                // Microsoft: maybe we should throw an InvalidOperationException when we add an item from another listView
                // into this group's collection.
                // But in a similar situation, ListViewCollection throws an ArgumentException. This is the v1.* behavior
                throw new ArgumentException(string.Format(SR.OnlyOneControl, item.Text), "item");
            }
        }
        
        public void Clear() {
            for(int i=0; i < this.Count; i++) {
                MoveToGroup(this[i], null);
            }
            Items.Clear();
        }               

        public bool Contains(ListViewItem item) {
            return Items.Contains(item);
        }

        public void CopyTo(Array dest, int index) {
            Items.CopyTo(dest, index);
        }

        public IEnumerator GetEnumerator() {
            return Items.GetEnumerator();
        }

        public int IndexOf(ListViewItem item)
        {
            return Items.IndexOf(item);
        }
        
        public ListViewItem Insert(int index, ListViewItem item) {            
            CheckListViewItem(item);

            MoveToGroup(item, this.group);                        
            Items.Insert(index, item);
            return item;
        }

        private void MoveToGroup(ListViewItem item, ListViewGroup newGroup) {

            ListViewGroup oldGroup = item.Group;
            if (oldGroup != newGroup) {
                item.group = newGroup;
                if (oldGroup != null) {
                    oldGroup.Items.Remove(item);
                }
                UpdateNativeListViewItem(item);
            }
        }

        public void Remove(ListViewItem item)
        {
            Items.Remove(item);

            if (item.group == this.group) {                                
                item.group = null;
                UpdateNativeListViewItem(item);
            }
        }
        
        public void RemoveAt(int index) {            
            Remove(this[index]);
        }                

        private void UpdateNativeListViewItem(ListViewItem item) {
            if (item.ListView != null && item.ListView.IsHandleCreated && !item.ListView.InsertingItemsNatively) {
                item.UpdateStateToListView(item.Index);
            }
        }
    }
}


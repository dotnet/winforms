﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.InteropServices;

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Drawing.Design;
    using System.Globalization;

    [
    Editor("System.Windows.Forms.Design.TreeNodeCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))
    ]
    public class TreeNodeCollection : IList {
        private TreeNode owner;

        /// A caching mechanism for key accessor
        /// We use an index here rather than control so that we don't have lifetime
        /// issues by holding on to extra references.
        private int lastAccessedIndex = -1;
		
        //this index is used to optimize performance of AddRange
        //items are added from last to first after this index 
        //(to work around TV_INSertItem comctl32 perf issue with consecutive adds in the end of the list) 
        private int fixedIndex = -1;
        

        internal TreeNodeCollection(TreeNode owner) {
            this.owner = owner;
        }
        
        internal int FixedIndex
        {
            get {
                return fixedIndex;
            }
            set {
                fixedIndex = value;
            }
        }


        public virtual TreeNode this[int index] {
            get {
                if (index < 0 || index >= owner.childCount) {
                    throw new ArgumentOutOfRangeException(nameof(index));
                }
                return owner.children[index];
            }
            set {
                if (index < 0 || index >= owner.childCount)
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                value.parent = owner;
                value.index = index;
                owner.children[index] = value;
                value.Realize(false);
            }
        }
        
        object IList.this[int index] {
            get {
                return this[index];
            }
            set {
                if (value is TreeNode) {
                    this[index] = (TreeNode)value;
                }
                else { 
                    throw new ArgumentException(SR.TreeNodeCollectionBadTreeNode, "value");
                }
            }
        }

        /// <devdoc>
        ///     <para>Retrieves the child control with the specified key.</para>
        /// </devdoc>
        public virtual TreeNode this[string key] {
            get {
                // We do not support null and empty string as valid keys.
                if (string.IsNullOrEmpty(key)){
                    return null;
                }

                // Search for the key in our collection
                int index = IndexOfKey(key);
                if (IsValidIndex(index)) {
                    return this[index];
                }
                else {
                    return null;
                }

            }
        }
        // Make this property available to Intellisense. (Removed the EditorBrowsable attribute.)
        [Browsable(false)]
        public int Count {
            get {
                return owner.childCount;
            }
        }

        object ICollection.SyncRoot {
            get {
                return this;
            }
        }

        bool ICollection.IsSynchronized {
            get {
                return false;
            }
        }
        
        bool IList.IsFixedSize {
            get {
                return false;
            }
        }
        
        public bool IsReadOnly {
            get {  
                return false;
            }
        }

        /// <devdoc>
        ///     Creates a new child node under this node.  Child node is positioned after siblings.
        /// </devdoc>
        public virtual TreeNode Add(string text) {
            TreeNode tn = new TreeNode(text);
            Add(tn);
            return tn;
        }

        // <-- NEW ADD OVERLOADS IN WHIDBEY

        /// <devdoc>
        ///     Creates a new child node under this node.  Child node is positioned after siblings.
        /// </devdoc>
        public virtual TreeNode Add(string key, string text) {
            TreeNode tn = new TreeNode(text);
            tn.Name = key;
            Add(tn);
            return tn;
        }

        /// <devdoc>
        ///     Creates a new child node under this node.  Child node is positioned after siblings.
        /// </devdoc>
        public virtual TreeNode Add(string key, string text, int imageIndex) {
            TreeNode tn = new TreeNode(text);
            tn.Name = key;
            tn.ImageIndex = imageIndex;
            Add(tn);
            return tn;
        }

        /// <devdoc>
        ///     Creates a new child node under this node.  Child node is positioned after siblings.
        /// </devdoc>
        public virtual TreeNode Add(string key, string text, string imageKey) {
            TreeNode tn = new TreeNode(text);
            tn.Name = key;
            tn.ImageKey = imageKey;
            Add(tn);
            return tn;
        }

        /// <devdoc>
        ///     Creates a new child node under this node.  Child node is positioned after siblings.
        /// </devdoc>
        public virtual TreeNode Add(string key, string text, int imageIndex, int selectedImageIndex) {
            TreeNode tn = new TreeNode(text, imageIndex, selectedImageIndex);
            tn.Name = key;
            Add(tn);
            return tn;
        }

        /// <devdoc>
        ///     Creates a new child node under this node.  Child node is positioned after siblings.
        /// </devdoc>
        public virtual TreeNode Add(string key, string text, string imageKey, string selectedImageKey) {
            TreeNode tn = new TreeNode(text);
            tn.Name = key;
            tn.ImageKey = imageKey;
            tn.SelectedImageKey = selectedImageKey;
            Add(tn);
            return tn;
        }

        // END - NEW ADD OVERLOADS IN WHIDBEY -->
        
        public virtual void AddRange(TreeNode[] nodes) {
            if (nodes == null) {
                throw new ArgumentNullException(nameof(nodes));
            }
            if (nodes.Length == 0)
                return;
            TreeView tv = owner.TreeView;
            if (tv != null && nodes.Length > TreeNode.MAX_TREENODES_OPS) {
                tv.BeginUpdate();
            }
            owner.Nodes.FixedIndex = owner.childCount;
            owner.EnsureCapacity(nodes.Length);
            for (int i = nodes.Length-1 ; i >= 0; i--) {
                AddInternal(nodes[i],i);
            }
            owner.Nodes.FixedIndex = -1;
            if (tv != null && nodes.Length > TreeNode.MAX_TREENODES_OPS) {
                tv.EndUpdate();
            }
        }

        public TreeNode[] Find (string key, bool searchAllChildren) {
             ArrayList foundNodes =  FindInternal(key, searchAllChildren, this, new ArrayList());

             // 
             TreeNode[] stronglyTypedFoundNodes = new TreeNode[foundNodes.Count];
             foundNodes.CopyTo(stronglyTypedFoundNodes, 0);

             return stronglyTypedFoundNodes;
        }

        private ArrayList FindInternal(string key, bool searchAllChildren, TreeNodeCollection treeNodeCollectionToLookIn, ArrayList foundTreeNodes) {
          if ((treeNodeCollectionToLookIn == null) || (foundTreeNodes == null)) {
                return null; 
            }

            // Perform breadth first search - as it's likely people will want tree nodes belonging
            // to the same parent close to each other.
            
            for (int i = 0; i < treeNodeCollectionToLookIn.Count; i++) {
                  if (treeNodeCollectionToLookIn[i] == null){
                      continue;
                  }
                  
                  if (WindowsFormsUtils.SafeCompareStrings(treeNodeCollectionToLookIn[i].Name, key, /* ignoreCase = */ true)) {
                       foundTreeNodes.Add(treeNodeCollectionToLookIn[i]);
                  }
            }

            // Optional recurive search for controls in child collections.
            
            if (searchAllChildren){
                for (int i = 0; i < treeNodeCollectionToLookIn.Count; i++) {    
                  if (treeNodeCollectionToLookIn[i] == null){
                      continue;
                  }
                    if ((treeNodeCollectionToLookIn[i].Nodes != null) && treeNodeCollectionToLookIn[i].Nodes.Count > 0){
                        // if it has a valid child collecion, append those results to our collection
                        foundTreeNodes = FindInternal(key, searchAllChildren, treeNodeCollectionToLookIn[i].Nodes, foundTreeNodes);
                    }
                 }
            }
            return foundTreeNodes;
        }

		/// <devdoc>
		///     Adds a new child node to this node.  Child node is positioned after siblings.
		/// </devdoc>
		public virtual int Add(TreeNode node) {
			return AddInternal(node, 0);
		}

       
        private int AddInternal(TreeNode node, int delta) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }
            if (node.handle != IntPtr.Zero)
                throw new ArgumentException(string.Format(SR.OnlyOneControl, node.Text), "node");

            // Check for ParentingCycle
            owner.CheckParentingCycle(node);

            // If the TreeView is sorted, index is ignored
            TreeView tv = owner.TreeView;
            if (tv != null && tv.Sorted) {
                return owner.AddSorted(node);                
            }
            node.parent = owner;
            int fixedIndex = owner.Nodes.FixedIndex;
            if (fixedIndex != -1) {
                node.index = fixedIndex + delta;
            }
            else {
                //if fixedIndex != -1 capacity was ensured by AddRange 
                Debug.Assert(delta == 0,"delta should be 0");
                owner.EnsureCapacity(1);
                node.index = owner.childCount;
            }
            owner.children[node.index] = node;
            owner.childCount++;
            node.Realize(false);

            if (tv != null && node == tv.selectedNode)
                tv.SelectedNode = node; // communicate this to the handle
            
            if (tv != null && tv.TreeViewNodeSorter != null) {
                tv.Sort();
            }
                
            return node.index;
        }

        int IList.Add(object node) {
            if (node == null) {
                throw new ArgumentNullException(nameof(node));
            }
            else if (node is TreeNode) {
                return Add((TreeNode)node);
            }            
            else
            {
                return Add(node.ToString()).index;
            }
        }

        public bool Contains(TreeNode node) {
            return IndexOf(node) != -1;
        }

        /// <devdoc>
        ///     <para>Returns true if the collection contains an item with the specified key, false otherwise.</para>
        /// </devdoc>
        public virtual bool ContainsKey(string key) {
           return IsValidIndex(IndexOfKey(key)); 
        }


        bool IList.Contains(object node) {
            if (node is TreeNode) {
                return Contains((TreeNode)node);
            }
            else {  
                return false;
            }
        }

        public int IndexOf(TreeNode node) {
            for(int index=0; index < Count; ++index) {
                if (this[index] == node) {
                    return index;
                } 
            }
            return -1;
        }

        int IList.IndexOf(object node) {
            if (node is TreeNode) {
                return IndexOf((TreeNode)node);
            }
            else {  
                return -1;
            }
        }


        /// <devdoc>
        ///     <para>The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.</para>
        /// </devdoc>
        public virtual int  IndexOfKey(string key) {
            // Step 0 - Arg validation
            if (string.IsNullOrEmpty(key)){
                return -1; // we dont support empty or null keys.
            }

            // step 1 - check the last cached item
            if (IsValidIndex(lastAccessedIndex))
            {
                if (WindowsFormsUtils.SafeCompareStrings(this[lastAccessedIndex].Name, key, /* ignoreCase = */ true)) {
                    return lastAccessedIndex;
                }
            }

            // step 2 - search for the item
            for (int i = 0; i < this.Count; i ++) {
                if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, /* ignoreCase = */ true)) {
                    lastAccessedIndex = i;
                    return i;
                }
            }

            // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
            lastAccessedIndex = -1;
            return -1;
        }


        /// <devdoc>
        ///     Inserts a new child node on this node.  Child node is positioned as specified by index.
        /// </devdoc>
        public virtual void Insert(int index, TreeNode node) {
            if (node.handle != IntPtr.Zero)
                throw new ArgumentException(string.Format(SR.OnlyOneControl, node.Text), "node");

            // Check for ParentingCycle
            owner.CheckParentingCycle(node);

            // If the TreeView is sorted, index is ignored
            TreeView tv = owner.TreeView;
            if (tv != null && tv.Sorted) {
                owner.AddSorted(node);
                return;
            }

            if (index < 0) index = 0;
            if (index > owner.childCount) index = owner.childCount;
            owner.InsertNodeAt(index, node);
        }
        
        void IList.Insert(int index, object node) {
            if (node is TreeNode) {
                Insert(index, (TreeNode)node);
            }
            else {  
                throw new ArgumentException(SR.TreeNodeCollectionBadTreeNode, "node");
            }
        }

        // <-- NEW INSERT OVERLOADS IN WHIDBEY

        /// <devdoc>
        ///     Inserts a new child node on this node.  Child node is positioned as specified by index.
        /// </devdoc>
        public virtual TreeNode Insert(int index, string text) {
            TreeNode tn = new TreeNode(text);
            Insert(index, tn);
            return tn;
        }

        /// <devdoc>
        ///     Inserts a new child node on this node.  Child node is positioned as specified by index.
        /// </devdoc>
        public virtual TreeNode Insert(int index, string key, string text) {
            TreeNode tn = new TreeNode(text);
            tn.Name = key;
            Insert(index, tn);
            return tn;
        }

        /// <devdoc>
        ///     Inserts a new child node on this node.  Child node is positioned as specified by index.
        /// </devdoc>
        public virtual TreeNode Insert(int index, string key, string text, int imageIndex) {
            TreeNode tn = new TreeNode(text);
            tn.Name = key;
            tn.ImageIndex = imageIndex;
            Insert(index, tn);
            return tn;
        }

        /// <devdoc>
        ///     Inserts a new child node on this node.  Child node is positioned as specified by index.
        /// </devdoc>
        public virtual TreeNode Insert(int index, string key, string text, string imageKey) {
            TreeNode tn = new TreeNode(text);
            tn.Name = key;
            tn.ImageKey = imageKey;
            Insert(index, tn);
            return tn;
        }

        /// <devdoc>
        ///     Inserts a new child node on this node.  Child node is positioned as specified by index.
        /// </devdoc>
        public virtual TreeNode Insert(int index, string key, string text, int imageIndex, int selectedImageIndex) {
            TreeNode tn = new TreeNode(text, imageIndex, selectedImageIndex);
            tn.Name = key;
            Insert(index, tn);
            return tn;
        }

        /// <devdoc>
        ///     Inserts a new child node on this node.  Child node is positioned as specified by index.
        /// </devdoc>
        public virtual TreeNode Insert(int index, string key, string text, string imageKey, string selectedImageKey) {
            TreeNode tn = new TreeNode(text);
            tn.Name = key;
            tn.ImageKey = imageKey;
            tn.SelectedImageKey = selectedImageKey;
            Insert(index, tn);
            return tn;
        }

        // END - NEW INSERT OVERLOADS IN WHIDBEY -->

        /// <devdoc>
        ///     <para>Determines if the index is valid for the collection.</para>
        /// </devdoc>
        private bool IsValidIndex(int index) {
            return ((index >= 0) && (index < this.Count));
        }

        /// <devdoc>
        ///     Remove all nodes from the tree view.
        /// </devdoc>
        public virtual void Clear() {
            owner.Clear();
        }

        public void CopyTo(Array dest, int index) {
            if (owner.childCount > 0) {
                System.Array.Copy(owner.children, 0, dest, index, owner.childCount);
            }
        }
        
        public void Remove(TreeNode node) {
            node.Remove();
        }
        
        void IList.Remove(object node) {
            if (node is TreeNode ) {
                Remove((TreeNode)node);
            }
        }

        public virtual void RemoveAt(int index) {
            this[index].Remove();
        }
       
        /// <devdoc>
        ///     <para>Removes the child control with the specified key.</para>
        /// </devdoc>
        public virtual void RemoveByKey(string key) {
            int index = IndexOfKey(key);
            if (IsValidIndex(index)) {
                RemoveAt(index); 
             }
        }


        public IEnumerator GetEnumerator() {
            return new WindowsFormsUtils.ArraySubsetEnumerator(owner.children, owner.childCount);
        }
    }
}

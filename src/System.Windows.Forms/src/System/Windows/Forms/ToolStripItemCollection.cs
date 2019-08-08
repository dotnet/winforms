// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Summary description for ToolStripItemCollection.
    /// </summary>
    [
    Editor("System.Windows.Forms.Design.ToolStripCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
    ListBindable(false),
    ]
    public class ToolStripItemCollection : ArrangedElementCollection, IList
    {
        private readonly ToolStrip owner;
        private readonly bool itemsCollection;
        private readonly bool isReadOnly = false;
        ///  A caching mechanism for key accessor
        ///  We use an index here rather than control so that we don't have lifetime
        ///  issues by holding on to extra references.
        ///  Note this is not Thread Safe - but WinForms has to be run in a STA anyways.
        private int lastAccessedIndex = -1;

        internal ToolStripItemCollection(ToolStrip owner, bool itemsCollection) : this(owner, itemsCollection, /*isReadOnly=*/false)
        {
        }

        internal ToolStripItemCollection(ToolStrip owner, bool itemsCollection, bool isReadOnly)
        {
            this.owner = owner;
            this.itemsCollection = itemsCollection;
            this.isReadOnly = isReadOnly;
        }

        public ToolStripItemCollection(ToolStrip owner, ToolStripItem[] value)
        {
            this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            AddRange(value);
        }

        /// <summary>
        /// </summary>
        public new virtual ToolStripItem this[int index]
        {
            get
            {
                return (ToolStripItem)(InnerList[index]);
            }
        }

        /// <summary>
        ///  Retrieves the child control with the specified key.
        /// </summary>
        public virtual ToolStripItem this[string key]
        {
            get
            {
                // We do not support null and empty string as valid keys.
                if ((key == null) || (key.Length == 0))
                {
                    return null;
                }

                // Search for the key in our collection
                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                {
                    return (ToolStripItem)InnerList[index];
                }
                else
                {
                    return null;
                }

            }
        }

        public ToolStripItem Add(string text)
        {
            return Add(text, null, null);
        }
        public ToolStripItem Add(Image image)
        {
            return Add(null, image, null);
        }
        public ToolStripItem Add(string text, Image image)
        {
            return Add(text, image, null);
        }
        public ToolStripItem Add(string text, Image image, EventHandler onClick)
        {
            ToolStripItem item = owner.CreateDefaultItem(text, image, onClick);
            Add(item);
            return item;
        }

        public int Add(ToolStripItem value)
        {
            CheckCanAddOrInsertItem(value);

            SetOwner(value);
            int retVal = InnerList.Add(value);
            if (itemsCollection && owner != null)
            {
                owner.OnItemAddedInternal(value);
                owner.OnItemAdded(new ToolStripItemEventArgs(value));
            }
            return retVal;

        }

        public void AddRange(ToolStripItem[] toolStripItems)
        {
            if (toolStripItems == null)
            {
                throw new ArgumentNullException(nameof(toolStripItems));
            }
            if (IsReadOnly)
            {
                throw new NotSupportedException(SR.ToolStripItemCollectionIsReadOnly);
            }

            // ToolStripDropDown will look for PropertyNames.Items to determine if it needs
            // to resize itself.
            using (new LayoutTransaction(owner, owner, PropertyNames.Items))
            {
                for (int i = 0; i < toolStripItems.Length; i++)
                {
                    Add(toolStripItems[i]);
                }
            }
        }

        public void AddRange(ToolStripItemCollection toolStripItems)
        {
            if (toolStripItems == null)
            {
                throw new ArgumentNullException(nameof(toolStripItems));
            }
            if (IsReadOnly)
            {
                throw new NotSupportedException(SR.ToolStripItemCollectionIsReadOnly);
            }

            // ToolStripDropDown will look for PropertyNames.Items to determine if it needs
            // to resize itself.
            using (new LayoutTransaction(owner, owner, PropertyNames.Items))
            {
                int currentCount = toolStripItems.Count;
                for (int i = 0; i < currentCount; i++)
                {
                    Add(toolStripItems[i]);
                }
            }

        }

        public bool Contains(ToolStripItem value)
        {
            return InnerList.Contains(value);
        }

        public virtual void Clear()
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException(SR.ToolStripItemCollectionIsReadOnly);
            }
            if (Count == 0)
            {
                return;
            }
            ToolStripOverflow overflow = null;

            if (owner != null && !owner.IsDisposingItems)
            {
                owner.SuspendLayout();
                overflow = owner.GetOverflow();
                if (overflow != null)
                {
                    overflow.SuspendLayout();
                }
            }
            try
            {
                while (Count != 0)
                {
                    RemoveAt(Count - 1);
                }
            }
            finally
            {
                if (overflow != null)
                {
                    overflow.ResumeLayout(false);
                }
                if (owner != null && !owner.IsDisposingItems)
                {
                    owner.ResumeLayout();
                }
            }
        }

        /// <summary>
        ///  Returns true if the collection contains an item with the specified key, false otherwise.
        /// </summary>
        public virtual bool ContainsKey(string key)
        {
            return IsValidIndex(IndexOfKey(key));
        }

        private void CheckCanAddOrInsertItem(ToolStripItem value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (IsReadOnly)
            {
                throw new NotSupportedException(SR.ToolStripItemCollectionIsReadOnly);
            }

            if (owner is ToolStripDropDown dropDown)
            {
                // If we're on a dropdown, we can only add non-control host items
                // as we dont want anything on a dropdown to get keyboard messages in the Internet.

                if (dropDown.OwnerItem == value)
                {
                    throw new NotSupportedException(SR.ToolStripItemCircularReference);
                }
            }

        }

        /// <summary>
        ///  Searches for Items by their Name property, builds up an array
        ///  of all the controls that match.
            /// </summary>
        public ToolStripItem[] Find(string key, bool searchAllChildren)
        {
            if ((key == null) || (key.Length == 0))
            {
                throw new ArgumentNullException(nameof(key), SR.FindKeyMayNotBeEmptyOrNull);
            }

            ArrayList foundItems = FindInternal(key, searchAllChildren, this, new ArrayList());

            // Make this a stongly typed collection.
            ToolStripItem[] stronglyTypedFoundItems = new ToolStripItem[foundItems.Count];
            foundItems.CopyTo(stronglyTypedFoundItems, 0);

            return stronglyTypedFoundItems;
        }

        /// <summary>
        ///  Searches for Items by their Name property, builds up an array list
        ///  of all the items that match.
            /// </summary>
        private ArrayList FindInternal(string key, bool searchAllChildren, ToolStripItemCollection itemsToLookIn, ArrayList foundItems)
        {
            if ((itemsToLookIn == null) || (foundItems == null))
            {
                return null;  //
            }

            try
            {
                for (int i = 0; i < itemsToLookIn.Count; i++)
                {
                    if (itemsToLookIn[i] == null)
                    {
                        continue;
                    }

                    if (WindowsFormsUtils.SafeCompareStrings(itemsToLookIn[i].Name, key, /* ignoreCase = */ true))
                    {
                        foundItems.Add(itemsToLookIn[i]);
                    }
                }

                // Optional recurive search for controls in child collections.

                if (searchAllChildren)
                {
                    for (int j = 0; j < itemsToLookIn.Count; j++)
                    {
                        if (!(itemsToLookIn[j] is ToolStripDropDownItem item))
                        {
                            continue;
                        }
                        if (item.HasDropDownItems)
                        {
                            // if it has a valid child collecion, append those results to our collection
                            foundItems = FindInternal(key, searchAllChildren, item.DropDownItems, foundItems);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Make sure we deal with non-critical failures gracefully.
                if (ClientUtils.IsCriticalException(e))
                {
                    throw;
                }
            }

            return foundItems;
        }

        public override bool IsReadOnly { get { return isReadOnly; } }

        void IList.Clear() { Clear(); }
        bool IList.IsFixedSize { get { return InnerList.IsFixedSize; } }
        bool IList.Contains(object value) { return InnerList.Contains(value); }
        void IList.RemoveAt(int index) { RemoveAt(index); }
        void IList.Remove(object value) { Remove(value as ToolStripItem); }
        int IList.Add(object value) { return Add(value as ToolStripItem); }
        int IList.IndexOf(object value) { return IndexOf(value as ToolStripItem); }
        void IList.Insert(int index, object value) { Insert(index, value as ToolStripItem); }

        object IList.this[int index]
        {
            get { return InnerList[index]; }
            set { throw new NotSupportedException(SR.ToolStripCollectionMustInsertAndRemove); /* InnerList[index] = value; */ }
        }
        public void Insert(int index, ToolStripItem value)
        {
            CheckCanAddOrInsertItem(value);
            SetOwner(value);
            InnerList.Insert(index, value);
            if (itemsCollection && owner != null)
            {
                if (owner.IsHandleCreated)
                {
                    LayoutTransaction.DoLayout(owner, value, PropertyNames.Parent);
                }
                else
                {
                    // next time we fetch the preferred size, recalc it.
                    CommonProperties.xClearPreferredSizeCache(owner);
                }
                owner.OnItemAddedInternal(value);
                owner.OnItemAdded(new ToolStripItemEventArgs(value));
            }

        }

        public int IndexOf(ToolStripItem value)
        {
            return InnerList.IndexOf(value);
        }
        /// <summary>
        ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
        /// </summary>
        public virtual int IndexOfKey(string key)
        {
            // Step 0 - Arg validation
            if ((key == null) || (key.Length == 0))
            {
                return -1; // we dont support empty or null keys.
            }

            // step 1 - check the last cached item
            if (IsValidIndex(lastAccessedIndex))
            {
                if (WindowsFormsUtils.SafeCompareStrings(this[lastAccessedIndex].Name, key, /* ignoreCase = */ true))
                {
                    return lastAccessedIndex;
                }
            }

            // step 2 - search for the item
            for (int i = 0; i < Count; i++)
            {
                if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, /* ignoreCase = */ true))
                {
                    lastAccessedIndex = i;
                    return i;
                }
            }

            // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
            lastAccessedIndex = -1;
            return -1;
        }

        /// <summary>
        ///  Determines if the index is valid for the collection.
        /// </summary>
        private bool IsValidIndex(int index)
        {
            return ((index >= 0) && (index < Count));
        }

        /// <summary>
        ///  Do proper cleanup of ownership, etc.
        /// </summary>
        private void OnAfterRemove(ToolStripItem item)
        {
            if (itemsCollection)
            {
                ToolStrip parent = null;
                if (item != null)
                {
                    parent = item.ParentInternal;
                    item.SetOwner(null);
                }

                if (owner != null)
                {
                    owner.OnItemRemovedInternal(item);

                    if (!owner.IsDisposingItems)
                    {
                        ToolStripItemEventArgs e = new ToolStripItemEventArgs(item);
                        owner.OnItemRemoved(e);

                        // dont fire the ItemRemoved event for Overflow
                        // it would fire constantly.... instead clear any state if the item
                        // is really being removed from the master collection.
                        if (parent != null && parent != owner)
                        {
                            parent.OnItemVisibleChanged(e, /*performLayout*/false);
                        }
                    }
                }
            }
        }

        public void Remove(ToolStripItem value)
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException(SR.ToolStripItemCollectionIsReadOnly);
            }
            InnerList.Remove(value);
            OnAfterRemove(value);
        }

        public void RemoveAt(int index)
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException(SR.ToolStripItemCollectionIsReadOnly);
            }
            ToolStripItem item = null;
            if (index < Count && index >= 0)
            {
                item = (ToolStripItem)(InnerList[index]);
            }
            InnerList.RemoveAt(index);
            OnAfterRemove(item);
        }

        /// <summary>
        ///  Removes the child item with the specified key.
        /// </summary>
        public virtual void RemoveByKey(string key)
        {
            if (IsReadOnly)
            {
                throw new NotSupportedException(SR.ToolStripItemCollectionIsReadOnly);
            }
            int index = IndexOfKey(key);
            if (IsValidIndex(index))
            {
                RemoveAt(index);
            }
        }

        public void CopyTo(ToolStripItem[] array, int index)
        {
            InnerList.CopyTo(array, index);
        }

        //
        internal void MoveItem(ToolStripItem value)
        {
            if (value.ParentInternal != null)
            {
                int indexOfItem = value.ParentInternal.Items.IndexOf(value);
                if (indexOfItem >= 0)
                {
                    value.ParentInternal.Items.RemoveAt(indexOfItem);
                }
            }
            Add(value);

        }

        internal void MoveItem(int index, ToolStripItem value)
        {
            // if moving to the end - call add instead.
            if (index == Count)
            {
                MoveItem(value);
                return;
            }

            if (value.ParentInternal != null)
            {
                int indexOfItem = value.ParentInternal.Items.IndexOf(value);

                if (indexOfItem >= 0)
                {
                    value.ParentInternal.Items.RemoveAt(indexOfItem);

                    if ((value.ParentInternal == owner) && (index > indexOfItem))
                    {
                        index--;
                    }
                }
            }
            Insert(index, value);

        }

        private void SetOwner(ToolStripItem item)
        {
            if (itemsCollection)
            {

                if (item != null)
                {
                    if (item.Owner != null)
                    {
                        item.Owner.Items.Remove(item);
                    }

                    item.SetOwner(owner);
                    if (item.Renderer != null)
                    {
                        item.Renderer.InitializeItem(item);
                    }
                }
            }
        }

    }

}


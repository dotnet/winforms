// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

public partial class CheckedListBox
{
    public class CheckedIndexCollection : IList
    {
        private readonly CheckedListBox _owner;

        internal CheckedIndexCollection(CheckedListBox owner)
        {
            _owner = owner.OrThrowIfNull();
        }

        /// <summary>
        ///  Number of current checked items.
        /// </summary>
        public int Count
        {
            get
            {
                return _owner.CheckedItems.Count;
            }
        }

        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                return true;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        ///  Retrieves the specified checked item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int this[int index]
        {
            get
            {
                object identifier = InnerArray.GetEntryObject(index, CheckedItemCollection.s_anyMask);
                return InnerArray.IndexOf(identifier, stateMask: 0);
            }
        }

        object? IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                throw new NotSupportedException(SR.CheckedListBoxCheckedIndexCollectionIsReadOnly);
            }
        }

        int IList.Add(object? value)
        {
            throw new NotSupportedException(SR.CheckedListBoxCheckedIndexCollectionIsReadOnly);
        }

        void IList.Clear()
        {
            throw new NotSupportedException(SR.CheckedListBoxCheckedIndexCollectionIsReadOnly);
        }

        void IList.Insert(int index, object? value)
        {
            throw new NotSupportedException(SR.CheckedListBoxCheckedIndexCollectionIsReadOnly);
        }

        void IList.Remove(object? value)
        {
            throw new NotSupportedException(SR.CheckedListBoxCheckedIndexCollectionIsReadOnly);
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException(SR.CheckedListBoxCheckedIndexCollectionIsReadOnly);
        }

        public bool Contains(int index)
        {
            return IndexOf(index) != -1;
        }

        bool IList.Contains(object? index)
        {
            if (index is int indexAsInt)
            {
                return Contains(indexAsInt);
            }
            else
            {
                return false;
            }
        }

        public void CopyTo(Array dest, int index)
        {
            int cnt = _owner.CheckedItems.Count;
            for (int i = 0; i < cnt; i++)
            {
                dest.SetValue(this[i], i + index);
            }
        }

        /// <summary>
        ///  This is the item array that stores our data. We share this backing store
        ///  with the main object collection.
        /// </summary>
        private ItemArray InnerArray
        {
            get
            {
                return _owner.Items.InnerArray;
            }
        }

        public IEnumerator GetEnumerator()
        {
            int[] indices = new int[Count];
            CopyTo(indices, 0);
            return indices.GetEnumerator();
        }

        public int IndexOf(int index)
        {
            if (index >= 0 && index < _owner.Items.Count)
            {
                object value = InnerArray.GetEntryObject(index, 0);
                return _owner.CheckedItems.IndexOfIdentifier(value);
            }

            return -1;
        }

        int IList.IndexOf(object? index)
        {
            if (index is int indexAsInt)
            {
                return IndexOf(indexAsInt);
            }
            else
            {
                return -1;
            }
        }
    }
}

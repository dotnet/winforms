// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

public partial class CheckedListBox
{
    public class CheckedItemCollection : IList
    {
        internal static int s_checkedItemMask = ItemArray.CreateMask();
        internal static int s_indeterminateItemMask = ItemArray.CreateMask();
        internal static int s_anyMask = s_checkedItemMask | s_indeterminateItemMask;

        private readonly CheckedListBox _owner;

        internal CheckedItemCollection(CheckedListBox owner)
        {
            _owner = owner;
        }

        /// <summary>
        ///  Number of current checked items.
        /// </summary>
        public int Count
        {
            get
            {
                return InnerArray.GetCount(s_anyMask);
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

        /// <summary>
        ///  Retrieves the specified checked item.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object? this[int index]
        {
            get
            {
                return InnerArray.GetItem(index, s_anyMask);
            }
            set
            {
                throw new NotSupportedException(SR.CheckedListBoxCheckedItemCollectionIsReadOnly);
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

        public bool Contains(object? item)
        {
            return IndexOf(item) != -1;
        }

        public int IndexOf(object? item)
        {
            return InnerArray.IndexOf(item, s_anyMask);
        }

        internal int IndexOfIdentifier(object item)
        {
            return InnerArray.IndexOf(item, s_anyMask);
        }

        int IList.Add(object? value)
        {
            throw new NotSupportedException(SR.CheckedListBoxCheckedItemCollectionIsReadOnly);
        }

        void IList.Clear()
        {
            throw new NotSupportedException(SR.CheckedListBoxCheckedItemCollectionIsReadOnly);
        }

        void IList.Insert(int index, object? value)
        {
            throw new NotSupportedException(SR.CheckedListBoxCheckedItemCollectionIsReadOnly);
        }

        void IList.Remove(object? value)
        {
            throw new NotSupportedException(SR.CheckedListBoxCheckedItemCollectionIsReadOnly);
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException(SR.CheckedListBoxCheckedItemCollectionIsReadOnly);
        }

        public void CopyTo(Array dest, int index)
        {
            int cnt = InnerArray.GetCount(s_anyMask);
            for (int i = 0; i < cnt; i++)
            {
                dest.SetValue(InnerArray.GetItem(i, s_anyMask), i + index);
            }
        }

        /// <summary>
        ///  This method returns if the actual item index is checked. The index is the index to the MAIN
        ///  collection, not this one.
        /// </summary>
        internal CheckState GetCheckedState(int index)
        {
            bool isChecked = InnerArray.GetState(index, s_checkedItemMask);
            bool isIndeterminate = InnerArray.GetState(index, s_indeterminateItemMask);
            Debug.Assert(!isChecked || !isIndeterminate, "Can't be both checked and indeterminate. Somebody broke our state.");
            if (isIndeterminate)
            {
                return CheckState.Indeterminate;
            }
            else if (isChecked)
            {
                return CheckState.Checked;
            }

            return CheckState.Unchecked;
        }

        public IEnumerator GetEnumerator()
        {
            return InnerArray.GetEnumerator(s_anyMask, true);
        }

        /// <summary>
        ///  Same thing for GetChecked.
        /// </summary>
        internal void SetCheckedState(int index, CheckState value)
        {
            bool isChecked;
            bool isIndeterminate;

            switch (value)
            {
                case CheckState.Checked:
                    isChecked = true;
                    isIndeterminate = false;
                    break;

                case CheckState.Indeterminate:
                    isChecked = false;
                    isIndeterminate = true;
                    break;

                default:
                    isChecked = false;
                    isIndeterminate = false;
                    break;
            }

            bool wasChecked = InnerArray.GetState(index, s_checkedItemMask);
            bool wasIndeterminate = InnerArray.GetState(index, s_indeterminateItemMask);

            InnerArray.SetState(index, s_checkedItemMask, isChecked);
            InnerArray.SetState(index, s_indeterminateItemMask, isIndeterminate);

            if (wasChecked != isChecked || wasIndeterminate != isIndeterminate)
            {
                // Raise a notify event that this item has changed.
                _owner.AccessibilityNotifyClients(AccessibleEvents.StateChange, index);
            }
        }
    }
}

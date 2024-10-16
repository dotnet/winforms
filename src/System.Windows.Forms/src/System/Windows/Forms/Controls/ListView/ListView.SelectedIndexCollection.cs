// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

public partial class ListView
{
    [ListBindable(false)]
    public class SelectedIndexCollection : IList
    {
        private readonly ListView _owner;

        /* C#r: protected */
        public SelectedIndexCollection(ListView owner)
        {
            _owner = owner;
        }

        /// <summary>
        ///  Number of currently selected items.
        /// </summary>
        [Browsable(false)]
        public int Count
        {
            get
            {
                if (_owner.IsHandleCreated)
                {
                    return (int)PInvokeCore.SendMessage(_owner, PInvoke.LVM_GETSELECTEDCOUNT);
                }
                else
                {
                    if (_owner._savedSelectedItems is not null)
                    {
                        return _owner._savedSelectedItems.Count;
                    }

                    return 0;
                }
            }
        }

        private int[] IndicesArray
        {
            get
            {
                int count = Count;
                int[] indices = new int[count];

                if (_owner.IsHandleCreated)
                {
                    int displayIndex = -1;
                    for (int i = 0; i < count; i++)
                    {
                        int fidx = (int)PInvokeCore.SendMessage(
                            _owner,
                            PInvoke.LVM_GETNEXTITEM,
                            (WPARAM)displayIndex,
                            (LPARAM)PInvoke.LVNI_SELECTED);
                        if (fidx > -1)
                        {
                            indices[i] = fidx;
                            displayIndex = fidx;
                        }
                        else
                        {
                            throw new InvalidOperationException(SR.SelectedNotEqualActual);
                        }
                    }
                }
                else
                {
                    Debug.Assert(_owner._savedSelectedItems is not null || count == 0, "if the count of selectedItems is greater than 0 then the selectedItems should have been saved by now");
                    if (_owner._savedSelectedItems is not null)
                    {
                        for (int i = 0; i < count; i++)
                        {
                            indices[i] = _owner._savedSelectedItems[i].Index;
                        }
                    }
                }

                return indices;
            }
        }

        /// <summary>
        ///  Selected item in the list.
        /// </summary>
        public int this[int index]
        {
            get
            {
                ArgumentOutOfRangeException.ThrowIfNegative(index);
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, Count);

                if (_owner.IsHandleCreated)
                {
                    // Count through the selected items in the ListView, until we reach the 'index'th selected item.
                    int fidx = -1;
                    for (int count = 0; count <= index; count++)
                    {
                        fidx = (int)PInvokeCore.SendMessage(
                            _owner,
                            PInvoke.LVM_GETNEXTITEM,
                            (WPARAM)fidx,
                            (LPARAM)PInvoke.LVNI_SELECTED);
                        Debug.Assert(fidx != -1, "Invalid index returned from LVM_GETNEXTITEM");
                    }

                    return fidx;
                }
                else
                {
                    Debug.Assert(_owner._savedSelectedItems is not null, "Null selected items collection");
                    return _owner._savedSelectedItems[index].Index;
                }
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
                throw new NotSupportedException();
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
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Contains(int selectedIndex)
        {
            if (selectedIndex < 0 || selectedIndex >= _owner.Items.Count)
            {
                return false;
            }

            return _owner.Items[selectedIndex].Selected;
        }

        bool IList.Contains(object? selectedIndex)
        {
            if (selectedIndex is int selectedIndexAsInt)
            {
                return Contains(selectedIndexAsInt);
            }
            else
            {
                return false;
            }
        }

        public int IndexOf(int selectedIndex)
        {
            int[] indices = IndicesArray;
            for (int index = 0; index < indices.Length; ++index)
            {
                if (indices[index] == selectedIndex)
                {
                    return index;
                }
            }

            return -1;
        }

        int IList.IndexOf(object? selectedIndex)
        {
            if (selectedIndex is int selectedIndexAsInt)
            {
                return IndexOf(selectedIndexAsInt);
            }
            else
            {
                return -1;
            }
        }

        int IList.Add(object? value)
        {
            if (value is int valueAsInt)
            {
                return Add(valueAsInt);
            }
            else
            {
                throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(value), value), nameof(value));
            }
        }

        void IList.Clear()
        {
            Clear();
        }

        void IList.Insert(int index, object? value)
        {
            throw new NotSupportedException();
        }

        void IList.Remove(object? value)
        {
            if (value is int valueAsInt)
            {
                Remove(valueAsInt);
            }
            else
            {
                throw new ArgumentException(string.Format(SR.InvalidArgument, nameof(value), value), nameof(value));
            }
        }

        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public int Add(int itemIndex)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(itemIndex);
            if (_owner.VirtualMode)
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(itemIndex, _owner.VirtualListSize);

                if (_owner.IsHandleCreated)
                {
                    _owner.SetItemState(itemIndex, LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED, LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED);
                    return Count;
                }
                else
                {
                    return -1;
                }
            }
            else
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(itemIndex, _owner.Items.Count);

                _owner.Items[itemIndex].Selected = true;
                return Count;
            }
        }

        public void Clear()
        {
            if (!_owner.VirtualMode)
            {
                _owner._savedSelectedItems = null;
            }

            if (_owner.IsHandleCreated)
            {
                _owner.SetItemState(-1, 0, LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED);
            }
        }

        public void CopyTo(Array dest, int index)
        {
            if (Count > 0)
            {
                Array.Copy(IndicesArray, 0, dest, index, Count);
            }
        }

        public IEnumerator GetEnumerator()
        {
            int[] indices = IndicesArray;
            if (indices is not null)
            {
                return indices.GetEnumerator();
            }
            else
            {
                return Array.Empty<int>().GetEnumerator();
            }
        }

        public void Remove(int itemIndex)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(itemIndex);
            if (_owner.VirtualMode)
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(itemIndex, _owner.VirtualListSize);

                if (_owner.IsHandleCreated)
                {
                    _owner.SetItemState(itemIndex, 0, LIST_VIEW_ITEM_STATE_FLAGS.LVIS_SELECTED);
                }
            }
            else
            {
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(itemIndex, _owner.Items.Count);

                _owner.Items[itemIndex].Selected = false;
            }
        }
    }
}

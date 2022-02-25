// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    public partial class ListView
    {
        [ListBindable(false)]
        public class SelectedIndexCollection : IList
        {
            private readonly ListView owner;

            /* C#r: protected */
            public SelectedIndexCollection(ListView owner)
            {
                this.owner = owner;
            }

            /// <summary>
            ///  Number of currently selected items.
            /// </summary>
            [Browsable(false)]
            public int Count
            {
                get
                {
                    if (owner.IsHandleCreated)
                    {
                        return (int)User32.SendMessageW(owner, (User32.WM)LVM.GETSELECTEDCOUNT);
                    }
                    else
                    {
                        if (owner._savedSelectedItems is not null)
                        {
                            return owner._savedSelectedItems.Count;
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

                    if (owner.IsHandleCreated)
                    {
                        int displayIndex = -1;
                        for (int i = 0; i < count; i++)
                        {
                            int fidx = (int)User32.SendMessageW(owner, (User32.WM)LVM.GETNEXTITEM, displayIndex, (nint)LVNI.SELECTED);
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
                        Debug.Assert(owner._savedSelectedItems is not null || count == 0, "if the count of selectedItems is greater than 0 then the selectedItems should have been saved by now");
                        for (int i = 0; i < count; i++)
                        {
                            indices[i] = owner._savedSelectedItems[i].Index;
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
                    if (index < 0 || index >= Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    if (owner.IsHandleCreated)
                    {
                        // Count through the selected items in the ListView, until we reach the 'index'th selected item.
                        int fidx = -1;
                        for (int count = 0; count <= index; count++)
                        {
                            fidx = (int)User32.SendMessageW(owner, (User32.WM)LVM.GETNEXTITEM, fidx, (nint)LVNI.SELECTED);
                            Debug.Assert(fidx != -1, "Invalid index returned from LVM_GETNEXTITEM");
                        }

                        return fidx;
                    }
                    else
                    {
                        Debug.Assert(owner._savedSelectedItems is not null, "Null selected items collection");
                        return owner._savedSelectedItems[index].Index;
                    }
                }
            }

            object IList.this[int index]
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
                if (selectedIndex < 0 || selectedIndex >= owner.Items.Count)
                {
                    return false;
                }

                return owner.Items[selectedIndex].Selected;
            }

            bool IList.Contains(object selectedIndex)
            {
                if (selectedIndex is int)
                {
                    return Contains((int)selectedIndex);
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

            int IList.IndexOf(object selectedIndex)
            {
                if (selectedIndex is int)
                {
                    return IndexOf((int)selectedIndex);
                }
                else
                {
                    return -1;
                }
            }

            int IList.Add(object value)
            {
                if (value is int)
                {
                    return Add((int)value);
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

            void IList.Insert(int index, object value)
            {
                throw new NotSupportedException();
            }

            void IList.Remove(object value)
            {
                if (value is int)
                {
                    Remove((int)value);
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
                if (owner.VirtualMode)
                {
                    if (itemIndex < 0 || itemIndex >= owner.VirtualListSize)
                    {
                        throw new ArgumentOutOfRangeException(nameof(itemIndex), itemIndex, string.Format(SR.InvalidArgument, nameof(itemIndex), itemIndex));
                    }

                    if (owner.IsHandleCreated)
                    {
                        owner.SetItemState(itemIndex, LVIS.SELECTED, LVIS.SELECTED);
                        return Count;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    if (itemIndex < 0 || itemIndex >= owner.Items.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(itemIndex), itemIndex, string.Format(SR.InvalidArgument, nameof(itemIndex), itemIndex));
                    }

                    owner.Items[itemIndex].Selected = true;
                    return Count;
                }
            }

            public void Clear()
            {
                if (!owner.VirtualMode)
                {
                    owner._savedSelectedItems = null;
                }

                if (owner.IsHandleCreated)
                {
                    owner.SetItemState(-1, 0, LVIS.SELECTED);
                }
            }

            public void CopyTo(Array dest, int index)
            {
                if (Count > 0)
                {
                    System.Array.Copy(IndicesArray, 0, dest, index, Count);
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
                if (owner.VirtualMode)
                {
                    if (itemIndex < 0 || itemIndex >= owner.VirtualListSize)
                    {
                        throw new ArgumentOutOfRangeException(nameof(itemIndex), itemIndex, string.Format(SR.InvalidArgument, nameof(itemIndex), itemIndex));
                    }

                    if (owner.IsHandleCreated)
                    {
                        owner.SetItemState(itemIndex, 0, LVIS.SELECTED);
                    }
                }
                else
                {
                    if (itemIndex < 0 || itemIndex >= owner.Items.Count)
                    {
                        throw new ArgumentOutOfRangeException(nameof(itemIndex), itemIndex, string.Format(SR.InvalidArgument, nameof(itemIndex), itemIndex));
                    }

                    owner.Items[itemIndex].Selected = false;
                }
            }
        }
    }
}

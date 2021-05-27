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
        public class ColumnHeaderCollection : IList
        {
            private readonly ListView owner;

            public ColumnHeaderCollection(ListView owner)
            {
                this.owner = owner ?? throw new ArgumentNullException(nameof(owner));
            }

            /// <summary>
            ///  Given a Zero based index, returns the ColumnHeader object
            ///  for the column at that index
            /// </summary>
            public virtual ColumnHeader this[int index]
            {
                get
                {
                    if (owner.columnHeaders is null || index < 0 || index >= owner.columnHeaders.Length)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }

                    return owner.columnHeaders[index];
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

            /// <summary>
            ///  Retrieves the child control with the specified key.
            /// </summary>
            public virtual ColumnHeader this[string key]
            {
                get
                {
                    // We do not support null and empty string as valid keys.
                    if (string.IsNullOrEmpty(key))
                    {
                        return null;
                    }

                    // Search for the key in our collection
                    int index = IndexOfKey(key);
                    if (IsValidIndex(index))
                    {
                        return this[index];
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            /// <summary>
            ///  The number of columns the ListView currently has in Details view.
            /// </summary>
            [Browsable(false)]
            public int Count
            {
                get
                {
                    return owner.columnHeaders is null ? 0 : owner.columnHeaders.Length;
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
                    return true;
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

            /// <summary>
            ///  Removes the child control with the specified key.
            /// </summary>
            public virtual void RemoveByKey(string key)
            {
                int index = IndexOfKey(key);
                if (IsValidIndex(index))
                {
                    RemoveAt(index);
                }
            }

            ///  A caching mechanism for key accessor
            ///  We use an index here rather than control so that we don't have lifetime
            ///  issues by holding on to extra references.
            ///  Note this is not Thread Safe - but WinForms has to be run in a STA anyways.
            private int lastAccessedIndex = -1;

            /// <summary>
            ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
            /// </summary>
            public virtual int IndexOfKey(string key)
            {
                // Step 0 - Arg validation
                if (string.IsNullOrEmpty(key))
                {
                    return -1; // we don't support empty or null keys.
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
                return (index >= 0) && (index < Count);
            }

            /// <summary>
            ///  Adds a column to the end of the Column list
            /// </summary>
            public virtual ColumnHeader Add(string text, int width, HorizontalAlignment textAlign)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Text = text,
                    Width = width,
                    TextAlign = textAlign
                };
                return owner.InsertColumn(Count, columnHeader);
            }

            public virtual int Add(ColumnHeader value)
            {
                int index = Count;
                owner.InsertColumn(index, value);
                return index;
            }

            public virtual ColumnHeader Add(string text)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Text = text
                };
                return owner.InsertColumn(Count, columnHeader);
            }

            // <-- NEW ADD OVERLOADS IN WHIDBEY

            public virtual ColumnHeader Add(string text, int width)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Text = text,
                    Width = width
                };
                return owner.InsertColumn(Count, columnHeader);
            }

            public virtual ColumnHeader Add(string key, string text)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Name = key,
                    Text = text
                };
                return owner.InsertColumn(Count, columnHeader);
            }

            public virtual ColumnHeader Add(string key, string text, int width)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Name = key,
                    Text = text,
                    Width = width
                };
                return owner.InsertColumn(Count, columnHeader);
            }

            public virtual ColumnHeader Add(string key, string text, int width, HorizontalAlignment textAlign, string imageKey)
            {
                ColumnHeader columnHeader = new ColumnHeader(imageKey)
                {
                    Name = key,
                    Text = text,
                    Width = width,
                    TextAlign = textAlign
                };
                return owner.InsertColumn(Count, columnHeader);
            }

            public virtual ColumnHeader Add(string key, string text, int width, HorizontalAlignment textAlign, int imageIndex)
            {
                ColumnHeader columnHeader = new ColumnHeader(imageIndex)
                {
                    Name = key,
                    Text = text,
                    Width = width,
                    TextAlign = textAlign
                };
                return owner.InsertColumn(Count, columnHeader);
            }

            // END - NEW ADD OVERLOADS IN WHIDBEY  -->

            public virtual void AddRange(ColumnHeader[] values)
            {
                if (values is null)
                {
                    throw new ArgumentNullException(nameof(values));
                }

                Hashtable usedIndices = new Hashtable();
                int[] indices = new int[values.Length];

                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i] is null)
                    {
                        throw new ArgumentNullException(nameof(values));
                    }

                    if (values[i].DisplayIndex == -1)
                    {
                        values[i].DisplayIndexInternal = i;
                    }

                    if (!usedIndices.ContainsKey(values[i].DisplayIndex) && values[i].DisplayIndex >= 0 && values[i].DisplayIndex < values.Length)
                    {
                        usedIndices.Add(values[i].DisplayIndex, i);
                    }

                    indices[i] = values[i].DisplayIndex;
                    Add(values[i]);
                }

                if (usedIndices.Count == values.Length)
                {
                    owner.SetDisplayIndices(indices);
                }
            }

            int IList.Add(object value)
            {
                if (value is ColumnHeader)
                {
                    return Add((ColumnHeader)value);
                }
                else
                {
                    throw new ArgumentException(SR.ColumnHeaderCollectionInvalidArgument, nameof(value));
                }
            }

            /// <summary>
            ///  Removes all columns from the list view.
            /// </summary>
            public virtual void Clear()
            {
                // Delete the columns
                if (owner.columnHeaders is not null)
                {
                    if (owner.View == View.Tile)
                    {
                        // in Tile view our ListView uses the column header collection to update the Tile Information
                        for (int colIdx = owner.columnHeaders.Length - 1; colIdx >= 0; colIdx--)
                        {
                            int w = owner.columnHeaders[colIdx].Width; // Update width before detaching from ListView
                            owner.columnHeaders[colIdx].OwnerListview = null;
                        }

                        owner.columnHeaders = null;
                        if (owner.IsHandleCreated)
                        {
                            owner.RecreateHandleInternal();
                        }
                    }
                    else
                    {
                        for (int colIdx = owner.columnHeaders.Length - 1; colIdx >= 0; colIdx--)
                        {
                            int w = owner.columnHeaders[colIdx].Width; // Update width before detaching from ListView
                            if (owner.IsHandleCreated)
                            {
                                User32.SendMessageW(owner, (User32.WM)LVM.DELETECOLUMN, (IntPtr)colIdx);
                            }

                            owner.columnHeaders[colIdx].OwnerListview = null;
                        }

                        owner.columnHeaders = null;
                    }
                }
            }

            public bool Contains(ColumnHeader value)
            {
                return IndexOf(value) != -1;
            }

            bool IList.Contains(object value)
            {
                if (value is ColumnHeader)
                {
                    return Contains((ColumnHeader)value);
                }

                return false;
            }

            /// <summary>
            ///  Returns true if the collection contains an item with the specified key, false otherwise.
            /// </summary>
            public virtual bool ContainsKey(string key)
            {
                return IsValidIndex(IndexOfKey(key));
            }

            void ICollection.CopyTo(Array dest, int index)
            {
                if (Count > 0)
                {
                    System.Array.Copy(owner.columnHeaders, 0, dest, index, Count);
                }
            }

            public int IndexOf(ColumnHeader value)
            {
                for (int index = 0; index < Count; ++index)
                {
                    if (this[index] == value)
                    {
                        return index;
                    }
                }

                return -1;
            }

            int IList.IndexOf(object value)
            {
                if (value is ColumnHeader)
                {
                    return IndexOf((ColumnHeader)value);
                }

                return -1;
            }

            public void Insert(int index, ColumnHeader value)
            {
                if (index < 0 || index > Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                owner.InsertColumn(index, value);
            }

            void IList.Insert(int index, object value)
            {
                if (value is ColumnHeader)
                {
                    Insert(index, (ColumnHeader)value);
                }
            }

            public void Insert(int index, string text, int width, HorizontalAlignment textAlign)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Text = text,
                    Width = width,
                    TextAlign = textAlign
                };
                Insert(index, columnHeader);
            }

            // <-- NEW INSERT OVERLOADS IN WHIDBEY

            public void Insert(int index, string text)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Text = text
                };
                Insert(index, columnHeader);
            }

            public void Insert(int index, string text, int width)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Text = text,
                    Width = width
                };
                Insert(index, columnHeader);
            }

            public void Insert(int index, string key, string text)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Name = key,
                    Text = text
                };
                Insert(index, columnHeader);
            }

            public void Insert(int index, string key, string text, int width)
            {
                ColumnHeader columnHeader = new ColumnHeader
                {
                    Name = key,
                    Text = text,
                    Width = width
                };
                Insert(index, columnHeader);
            }

            public void Insert(int index, string key, string text, int width, HorizontalAlignment textAlign, string imageKey)
            {
                ColumnHeader columnHeader = new ColumnHeader(imageKey)
                {
                    Name = key,
                    Text = text,
                    Width = width,
                    TextAlign = textAlign
                };
                Insert(index, columnHeader);
            }

            public void Insert(int index, string key, string text, int width, HorizontalAlignment textAlign, int imageIndex)
            {
                ColumnHeader columnHeader = new ColumnHeader(imageIndex)
                {
                    Name = key,
                    Text = text,
                    Width = width,
                    TextAlign = textAlign
                };
                Insert(index, columnHeader);
            }

            // END - NEW INSERT OVERLOADS IN WHIDBEY -->

            /// <summary>
            ///  removes a column from the ListView
            /// </summary>
            public virtual void RemoveAt(int index)
            {
                if (owner.columnHeaders is null || index < 0 || index >= owner.columnHeaders.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                int w = owner.columnHeaders[index].Width; // Update width before detaching from ListView

                // in Tile view our ListView uses the column header collection to update the Tile Information
                if (owner.IsHandleCreated && owner.View != View.Tile)
                {
                    int retval = unchecked((int)(long)User32.SendMessageW(owner, (User32.WM)LVM.DELETECOLUMN, (IntPtr)index));
                    if (0 == retval)
                    {
                        throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                    }
                }

                // we need to update the display indices
                int[] indices = new int[Count - 1];

                ColumnHeader removeHdr = this[index];
                for (int i = 0; i < Count; i++)
                {
                    ColumnHeader hdr = this[i];
                    if (i != index)
                    {
                        if (hdr.DisplayIndex >= removeHdr.DisplayIndex)
                        {
                            hdr.DisplayIndexInternal--;
                        }

                        indices[i > index ? i - 1 : i] = hdr.DisplayIndexInternal;
                    }
                }

                removeHdr.DisplayIndexInternal = -1;

                owner.columnHeaders[index].OwnerListview = null;
                int columnCount = owner.columnHeaders.Length;
                Debug.Assert(columnCount >= 1, "Column mismatch");
                if (columnCount == 1)
                {
                    owner.columnHeaders = null;
                }
                else
                {
                    ColumnHeader[] newHeaders = new ColumnHeader[--columnCount];
                    if (index > 0)
                    {
                        System.Array.Copy(owner.columnHeaders, 0, newHeaders, 0, index);
                    }

                    if (index < columnCount)
                    {
                        System.Array.Copy(owner.columnHeaders, index + 1, newHeaders, index, columnCount - index);
                    }

                    owner.columnHeaders = newHeaders;
                }

                // in Tile view our ListView uses the column header collection to update the Tile Information
                if (owner.IsHandleCreated && owner.View == View.Tile)
                {
                    owner.RecreateHandleInternal();
                }

                owner.SetDisplayIndices(indices);
            }

            public virtual void Remove(ColumnHeader column)
            {
                int index = IndexOf(column);
                if (index != -1)
                {
                    RemoveAt(index);
                }
            }

            void IList.Remove(object value)
            {
                if (value is ColumnHeader)
                {
                    Remove((ColumnHeader)value);
                }
            }

            public IEnumerator GetEnumerator()
            {
                if (owner.columnHeaders is not null)
                {
                    return owner.columnHeaders.GetEnumerator();
                }
                else
                {
                    return Array.Empty<ColumnHeader>().GetEnumerator();
                }
            }
        }
    }
}

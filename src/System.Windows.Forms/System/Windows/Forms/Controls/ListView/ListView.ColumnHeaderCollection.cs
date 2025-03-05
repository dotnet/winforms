// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

public partial class ListView
{
    [ListBindable(false)]
    public class ColumnHeaderCollection : IList
    {
        private readonly ListView _owner;

        public ColumnHeaderCollection(ListView owner)
        {
            _owner = owner.OrThrowIfNull();
        }

        /// <summary>
        ///  Given a Zero based index, returns the ColumnHeader object
        ///  for the column at that index
        /// </summary>
        public virtual ColumnHeader this[int index] => _owner.GetColumnHeader(index);

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

        /// <summary>
        ///  Retrieves the child control with the specified key.
        /// </summary>
        public virtual ColumnHeader? this[string? key]
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
                return _owner._columnHeaders is null ? 0 : _owner._columnHeaders.Length;
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
        public virtual void RemoveByKey(string? key)
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
        private int _lastAccessedIndex = -1;

        /// <summary>
        ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
        /// </summary>
        public virtual int IndexOfKey(string? key)
        {
            // Step 0 - Arg validation
            if (string.IsNullOrEmpty(key))
            {
                return -1; // we don't support empty or null keys.
            }

            // step 1 - check the last cached item
            if (IsValidIndex(_lastAccessedIndex))
            {
                if (WindowsFormsUtils.SafeCompareStrings(this[_lastAccessedIndex].Name, key, /* ignoreCase = */ true))
                {
                    return _lastAccessedIndex;
                }
            }

            // step 2 - search for the item
            for (int i = 0; i < Count; i++)
            {
                if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, /* ignoreCase = */ true))
                {
                    _lastAccessedIndex = i;
                    return i;
                }
            }

            // step 3 - we didn't find it. Invalidate the last accessed index and return -1.
            _lastAccessedIndex = -1;
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
        public virtual ColumnHeader Add(string? text, int width, HorizontalAlignment textAlign)
        {
            ColumnHeader columnHeader = new ColumnHeader
            {
                Text = text,
                Width = width,
                TextAlign = textAlign
            };
            return _owner.InsertColumn(Count, columnHeader);
        }

        public virtual int Add(ColumnHeader value)
        {
            int index = Count;
            _owner.InsertColumn(index, value);
            return index;
        }

        public virtual ColumnHeader Add(string? text)
        {
            ColumnHeader columnHeader = new ColumnHeader
            {
                Text = text
            };
            return _owner.InsertColumn(Count, columnHeader);
        }

        // <-- NEW ADD OVERLOADS IN WHIDBEY

        public virtual ColumnHeader Add(string? text, int width)
        {
            ColumnHeader columnHeader = new ColumnHeader
            {
                Text = text,
                Width = width
            };
            return _owner.InsertColumn(Count, columnHeader);
        }

        public virtual ColumnHeader Add(string? key, string? text)
        {
            ColumnHeader columnHeader = new ColumnHeader
            {
                Name = key,
                Text = text
            };
            return _owner.InsertColumn(Count, columnHeader);
        }

        public virtual ColumnHeader Add(string? key, string? text, int width)
        {
            ColumnHeader columnHeader = new ColumnHeader
            {
                Name = key,
                Text = text,
                Width = width
            };
            return _owner.InsertColumn(Count, columnHeader);
        }

        public virtual ColumnHeader Add(string? key, string? text, int width, HorizontalAlignment textAlign, string imageKey)
        {
            ColumnHeader columnHeader = new(imageKey)
            {
                Name = key,
                Text = text,
                Width = width,
                TextAlign = textAlign
            };
            return _owner.InsertColumn(Count, columnHeader);
        }

        public virtual ColumnHeader Add(string? key, string? text, int width, HorizontalAlignment textAlign, int imageIndex)
        {
            ColumnHeader columnHeader = new(imageIndex)
            {
                Name = key,
                Text = text,
                Width = width,
                TextAlign = textAlign
            };
            return _owner.InsertColumn(Count, columnHeader);
        }

        // END - NEW ADD OVERLOADS IN WHIDBEY  -->

        public virtual void AddRange(params ColumnHeader[] values)
        {
            ArgumentNullException.ThrowIfNull(values);

            HashSet<int> usedIndices = [];
            int[] indices = new int[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                ColumnHeader? header = values[i];
                ArgumentNullException.ThrowIfNull(header, nameof(values));

                if (header.DisplayIndex == -1)
                {
                    header.DisplayIndexInternal = i;
                }

                if (!usedIndices.Contains(header.DisplayIndex) && header.DisplayIndex >= 0 && header.DisplayIndex < values.Length)
                {
                    usedIndices.Add(header.DisplayIndex);
                }

                indices[i] = header.DisplayIndex;
                Add(header);
            }

            if (usedIndices.Count == values.Length)
            {
                _owner.SetDisplayIndices(indices);
            }
        }

        int IList.Add(object? value)
        {
            if (value is ColumnHeader columnHeader)
            {
                return Add(columnHeader);
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
            if (_owner._columnHeaders is not null)
            {
                if (_owner.View == View.Tile)
                {
                    // in Tile view our ListView uses the column header collection to update the Tile Information
                    for (int colIdx = _owner._columnHeaders.Length - 1; colIdx >= 0; colIdx--)
                    {
                        _ = _owner._columnHeaders[colIdx].Width; // Update width before detaching from ListView
                        _owner._columnHeaders[colIdx].OwnerListview = null;
                        _owner._columnHeaders[colIdx].ReleaseUiaProvider();
                    }

                    _owner._columnHeaders = null;
                    if (_owner.IsHandleCreated)
                    {
                        _owner.RecreateHandleInternal();
                    }
                }
                else
                {
                    for (int colIdx = _owner._columnHeaders.Length - 1; colIdx >= 0; colIdx--)
                    {
                        _ = _owner._columnHeaders[colIdx].Width; // Update width before detaching from ListView
                        if (_owner.IsHandleCreated)
                        {
                            PInvokeCore.SendMessage(_owner, PInvoke.LVM_DELETECOLUMN, (WPARAM)colIdx);
                        }

                        _owner._columnHeaders[colIdx].OwnerListview = null;
                        _owner._columnHeaders[colIdx].ReleaseUiaProvider();
                    }

                    _owner._columnHeaders = null;
                }
            }
        }

        public bool Contains(ColumnHeader? value)
        {
            return IndexOf(value) != -1;
        }

        bool IList.Contains(object? value)
        {
            if (value is ColumnHeader columnHeader)
            {
                return Contains(columnHeader);
            }

            return false;
        }

        /// <summary>
        ///  Returns true if the collection contains an item with the specified key, false otherwise.
        /// </summary>
        public virtual bool ContainsKey(string? key)
        {
            return IsValidIndex(IndexOfKey(key));
        }

        void ICollection.CopyTo(Array dest, int index)
        {
            if (Count > 0)
            {
                Array.Copy(_owner._columnHeaders!, 0, dest, index, Count);
            }
        }

        public int IndexOf(ColumnHeader? value)
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

        int IList.IndexOf(object? value)
        {
            if (value is ColumnHeader columnHeader)
            {
                return IndexOf(columnHeader);
            }

            return -1;
        }

        public void Insert(int index, ColumnHeader value)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(index);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(index, Count);

            _owner.InsertColumn(index, value);
        }

        void IList.Insert(int index, object? value)
        {
            if (value is ColumnHeader columnHeader)
            {
                Insert(index, columnHeader);
            }
        }

        public void Insert(int index, string? text, int width, HorizontalAlignment textAlign)
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

        public void Insert(int index, string? text)
        {
            ColumnHeader columnHeader = new ColumnHeader
            {
                Text = text
            };
            Insert(index, columnHeader);
        }

        public void Insert(int index, string? text, int width)
        {
            ColumnHeader columnHeader = new ColumnHeader
            {
                Text = text,
                Width = width
            };
            Insert(index, columnHeader);
        }

        public void Insert(int index, string? key, string? text)
        {
            ColumnHeader columnHeader = new ColumnHeader
            {
                Name = key,
                Text = text
            };
            Insert(index, columnHeader);
        }

        public void Insert(int index, string? key, string? text, int width)
        {
            ColumnHeader columnHeader = new ColumnHeader
            {
                Name = key,
                Text = text,
                Width = width
            };
            Insert(index, columnHeader);
        }

        public void Insert(int index, string? key, string? text, int width, HorizontalAlignment textAlign, string imageKey)
        {
            ColumnHeader columnHeader = new(imageKey)
            {
                Name = key,
                Text = text,
                Width = width,
                TextAlign = textAlign
            };
            Insert(index, columnHeader);
        }

        public void Insert(int index, string? key, string? text, int width, HorizontalAlignment textAlign, int imageIndex)
        {
            ColumnHeader columnHeader = new(imageIndex)
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
            ColumnHeader columnHeader = _owner.GetColumnHeader(index);

            _ = columnHeader.Width; // Update width before detaching from ListView

            // in Tile view our ListView uses the column header collection to update the Tile Information
            if (_owner.IsHandleCreated && _owner.View != View.Tile)
            {
                int retval = (int)PInvokeCore.SendMessage(_owner, PInvoke.LVM_DELETECOLUMN, (WPARAM)index);
                if (retval == 0)
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
            removeHdr.ReleaseUiaProvider();

            columnHeader.OwnerListview = null;
            int columnCount = _owner._columnHeaders.Length;
            Debug.Assert(columnCount >= 1, "Column mismatch");
            if (columnCount == 1)
            {
                _owner._columnHeaders = null;
            }
            else
            {
                ColumnHeader[] newHeaders = new ColumnHeader[--columnCount];
                if (index > 0)
                {
                    Array.Copy(_owner._columnHeaders, 0, newHeaders, 0, index);
                }

                if (index < columnCount)
                {
                    Array.Copy(_owner._columnHeaders, index + 1, newHeaders, index, columnCount - index);
                }

                _owner._columnHeaders = newHeaders;
            }

            // in Tile view our ListView uses the column header collection to update the Tile Information
            if (_owner.IsHandleCreated && _owner.View == View.Tile)
            {
                _owner.RecreateHandleInternal();
            }

            _owner.SetDisplayIndices(indices);
        }

        public virtual void Remove(ColumnHeader column)
        {
            int index = IndexOf(column);
            if (index != -1)
            {
                RemoveAt(index);
            }
        }

        void IList.Remove(object? value)
        {
            if (value is ColumnHeader columnHeader)
            {
                Remove(columnHeader);
            }
        }

        public IEnumerator GetEnumerator()
        {
            if (_owner._columnHeaders is not null)
            {
                return _owner._columnHeaders.GetEnumerator();
            }
            else
            {
                return Array.Empty<ColumnHeader>().GetEnumerator();
            }
        }
    }
}

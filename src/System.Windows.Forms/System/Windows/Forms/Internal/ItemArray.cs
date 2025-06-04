// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Globalization;
using static System.Windows.Forms.ItemArray;

namespace System.Windows.Forms;

/// <summary>
///  This is similar to ArrayList except that it also
///  maintains a bit-flag based state element for each item
///  in the array.
///
///  The methods to enumerate, count and get data support
///  virtualized indexes. Indexes are virtualized according
///  to the state mask passed in. This allows ItemArray
///  to be the backing store for one read-write "master"
///  collection and several read-only collections based
///  on masks. ItemArray supports up to 31 masks.
/// </summary>
internal partial class ItemArray : IComparer<Entry>
{
    private static int s_lastMask = 1;

    private readonly ListControl _listControl;
    private readonly List<Entry> _entries;

    public ItemArray(ListControl listControl)
    {
        _listControl = listControl;
        _entries = [];
    }

    internal IReadOnlyList<Entry?> Entries => _entries;

    /// <summary>
    ///  The version of this array. This number changes with each
    ///  change to the item list.
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    ///  Adds the given item to the array. The state is initially
    ///  zero.
    /// </summary>
    public object? Add(object item)
    {
        Entry entry = new(item);
        _entries.Add(entry);
        Version++;
        return entry;
    }

    /// <summary>
    ///  Clears this array.
    /// </summary>
    public void Clear()
    {
        _entries.Clear();
        Version++;
    }

    /// <summary>
    ///  Allocates a new bitmask for use.
    /// </summary>
    public static int CreateMask()
    {
        int mask = s_lastMask;
        s_lastMask <<= 1;
        Debug.Assert(s_lastMask > mask, "We have overflowed our state mask.");
        return mask;
    }

    /// <summary>
    ///  Turns a virtual index into an actual index.
    /// </summary>
    public int GetActualIndex(int virtualIndex, int stateMask)
    {
        if (stateMask == 0)
        {
            return virtualIndex;
        }

        // More complex; we must compute this index.
        int calcIndex = -1;

        for (int i = 0; i < Count; i++)
        {
            if ((_entries[i].State & stateMask) != 0)
            {
                calcIndex++;
                if (calcIndex == virtualIndex)
                {
                    return i;
                }
            }
        }

        return -1;
    }

    /// <summary>
    ///  Gets the main count
    /// </summary>
    public int Count => _entries.Count;

    /// <summary>
    ///  Gets the count of items matching the given mask.
    /// </summary>
    public int GetCount(int stateMask = 0)
    {
        // If mask is zero, then just give the main count
        if (stateMask == 0)
        {
            return Count;
        }

        // more complex:  must provide a count of items
        // based on a mask.
        int filteredCount = 0;

        foreach (Entry entry in _entries)
        {
            if ((entry.State & stateMask) != 0)
            {
                filteredCount++;
            }
        }

        return filteredCount;
    }

    /// <summary>
    ///  Retrieves an enumerator that will enumerate based on
    ///  the given mask.
    /// </summary>
    public IEnumerator GetEnumerator(int stateMask = 0)
    {
        return GetEnumerator(stateMask, anyBit: false);
    }

    /// <summary>
    ///  Retrieves an enumerator that will enumerate based on
    ///  the given mask.
    /// </summary>
    public IEnumerator GetEnumerator(int stateMask, bool anyBit)
    {
        return new EntryEnumerator(this, stateMask, anyBit);
    }

    /// <summary>
    ///  Gets the item at the given index. The index is
    ///  virtualized against the given mask value.
    /// </summary>
    public object GetItem(int virtualIndex, int stateMask = 0)
    {
        int actualIndex = GetActualIndex(virtualIndex, stateMask);
        if (actualIndex == -1)
        {
            throw new IndexOutOfRangeException();
        }

        return _entries[actualIndex].Item;
    }

    /// <summary>
    ///  Gets the item at the given index. The index is
    ///  virtualized against the given mask value.
    /// </summary>
    internal Entry GetEntryObject(int virtualIndex, int stateMask = 0)
    {
        int actualIndex = GetActualIndex(virtualIndex, stateMask);
        if (actualIndex == -1)
        {
            throw new IndexOutOfRangeException();
        }

        return _entries[actualIndex];
    }

    /// <summary>
    ///  Returns true if the requested state mask is set.
    ///  The index is the actual index to the array.
    /// </summary>
    public bool GetState(int index, int stateMask)
    {
        return (_entries[index].State & stateMask) == stateMask;
    }

    /// <summary>
    ///  Returns the virtual index of the item based on the
    ///  state mask.
    /// </summary>
    public int IndexOf(object? item, int stateMask = 0)
    {
        int virtualIndex = -1;

        foreach (Entry entry in _entries)
        {
            if (stateMask == 0 || (entry.State & stateMask) != 0)
            {
                virtualIndex++;
                if ((item is Entry itemEntry && entry == itemEntry) || entry.Item.Equals(item))
                {
                    return virtualIndex;
                }
            }
        }

        return -1;
    }

    /// <summary>
    ///  Inserts item at the given index. The index
    ///  is not virtualized.
    /// </summary>
    public void Insert(int index, object item)
    {
        InsertEntry(index, new Entry(item));
    }

    public void InsertEntry(int index, Entry item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _entries.Insert(index, item);
        Version++;
    }

    /// <summary>
    ///  Removes the given item from the array. If
    ///  the item is not in the array, this does nothing.
    /// </summary>
    public void Remove(object item)
    {
        int index = IndexOf(item);
        if (index != -1)
        {
            RemoveAt(index);
        }
    }

    /// <summary>
    ///  Removes the item at the given index.
    /// </summary>
    public void RemoveAt(int index)
    {
        _entries.RemoveAt(index);
        Version++;
    }

    /// <summary>
    ///  Sets the item at the given index to a new value.
    /// </summary>
    public void SetItem(int index, object item)
    {
        _entries[index].Item = item;
    }

    /// <summary>
    ///  Sets the state data for the given index.
    /// </summary>
    public void SetState(int index, int stateMask, bool value)
    {
        if (value)
        {
            _entries[index].State |= stateMask;
        }
        else
        {
            _entries[index].State &= ~stateMask;
        }

        Version++;
    }

    public static Entry GetEntry(object element)
    {
        return element is Entry entryElement ? entryElement : new Entry(element);
    }

    /// <summary>
    ///  Find element in sorted array. If element is not found returns a binary complement of index for inserting
    /// </summary>
    public int BinarySearch(Entry element)
    {
        return _entries.BinarySearch(index: 0, Count, element, this);
    }

    /// <summary>
    ///  Sorts our array.
    /// </summary>
    public void Sort()
    {
        _entries.Sort(this);
    }

    int IComparer<Entry>.Compare(Entry? entry1, Entry? entry2)
    {
        if (IComparerHelpers.CompareReturnIfNull(entry1, entry2, out int? returnValue))
        {
            return (int)returnValue;
        }

        string? itemName1 = _listControl.GetItemText(entry1.Item);
        string? itemName2 = _listControl.GetItemText(entry2.Item);

        CompareInfo compInfo = Application.CurrentCulture.CompareInfo;
        return compInfo.Compare(itemName1, itemName2, CompareOptions.StringSort);
    }
}

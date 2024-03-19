// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;

namespace System.Windows.Forms;

/// <summary>
///  A read-only collection of GridItem objects
/// </summary>
public class GridItemCollection : ICollection
{
    public static GridItemCollection Empty = new(entries: null);

    private protected IReadOnlyList<GridItem> _entries;

    internal GridItemCollection(IReadOnlyList<GridItem>? entries)
    {
        _entries = entries ?? Array.Empty<GridItem>();
    }

    /// <summary>
    ///  Retrieves the number of member attributes.
    /// </summary>
    public int Count => _entries.Count;

    object ICollection.SyncRoot => this;

    bool ICollection.IsSynchronized => false;

    /// <summary>
    ///  Retrieves the member attribute with the specified index.
    /// </summary>
    public GridItem this[int index] => _entries[index];

    public GridItem? this[string label]
    {
        get
        {
            foreach (GridItem item in _entries)
            {
                if (item.Label == label)
                {
                    return item;
                }
            }

            return null;
        }
    }

    void ICollection.CopyTo(Array dest, int index)
    {
        if (_entries.Count > 0)
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                ((IList)dest)[index + i] = _entries[i];
            }
        }
    }

    /// <summary>
    ///  Creates and retrieves a new enumerator for this collection.
    /// </summary>
    public IEnumerator GetEnumerator() => _entries.GetEnumerator();
}

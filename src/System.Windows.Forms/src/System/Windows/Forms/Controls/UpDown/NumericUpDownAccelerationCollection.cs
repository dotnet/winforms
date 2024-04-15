// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
///  Represents a SORTED collection of NumericUpDownAcceleration objects in the NumericUpDown Control.
///  The elements in the collection are sorted by the NumericUpDownAcceleration.Seconds property.
/// </summary>
[ListBindable(false)]
public class NumericUpDownAccelerationCollection : MarshalByRefObject, ICollection<NumericUpDownAcceleration>, IEnumerable<NumericUpDownAcceleration>
{
    private readonly List<NumericUpDownAcceleration> _items;

    /// <summary>
    ///  Adds an item (NumericUpDownAcceleration object) to the ICollection.
    ///  The item is added preserving the collection sorted.
    /// </summary>
    public void Add(NumericUpDownAcceleration acceleration)
    {
        ArgumentNullException.ThrowIfNull(acceleration);

        // Keep the array sorted, insert in the right spot.
        int index = 0;

        while (index < _items.Count)
        {
            if (acceleration.Seconds < _items[index].Seconds)
            {
                break;
            }

            index++;
        }

        _items.Insert(index, acceleration);
    }

    /// <summary>
    ///  Removes all items from the ICollection.
    /// </summary>
    public void Clear()
    {
        _items.Clear();
    }

    /// <summary>
    ///  Determines whether the IList contains a specific value.
    /// </summary>
    public bool Contains(NumericUpDownAcceleration acceleration)
    {
        return _items.Contains(acceleration);
    }

    /// <summary>
    ///  Copies the elements of the ICollection to an Array, starting at a particular Array index.
    /// </summary>
    public void CopyTo(NumericUpDownAcceleration[] array, int index)
    {
        _items.CopyTo(array, index);
    }

    /// <summary>
    ///  Gets the number of elements contained in the ICollection.
    /// </summary>
    public int Count
    {
        get { return _items.Count; }
    }

    /// <summary>
    ///  Gets a value indicating whether the ICollection is read-only.
    ///  This collection property returns false always.
    /// </summary>
    public bool IsReadOnly
    {
        get { return false; }
    }

    /// <summary>
    ///  Removes the specified item from the ICollection.
    /// </summary>
    public bool Remove(NumericUpDownAcceleration acceleration)
    {
        return _items.Remove(acceleration);
    }

    /// <summary>
    ///  Returns an enumerator that can iterate through the collection.
    /// </summary>
    IEnumerator<NumericUpDownAcceleration> IEnumerable<NumericUpDownAcceleration>.GetEnumerator()
    {
        return _items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)_items).GetEnumerator();
    }

    ///  NumericUpDownAccelerationCollection methods.

    /// <summary>
    ///  Class constructor.
    /// </summary>
    public NumericUpDownAccelerationCollection()
    {
        _items = [];
    }

    /// <summary>
    ///  Adds the elements of specified array to the collection, keeping the collection sorted.
    /// </summary>
    public void AddRange(params NumericUpDownAcceleration[] accelerations)
    {
        ArgumentNullException.ThrowIfNull(accelerations);

        // Accept the range only if ALL elements in the array are not null.
        foreach (NumericUpDownAcceleration acceleration in accelerations)
        {
            ArgumentNullException.ThrowIfNull(acceleration);
        }

        // The expected array size is typically small (5 items?), so we don't need to try to be smarter about the
        // way we add the elements to the collection, just call Add.
        foreach (NumericUpDownAcceleration acceleration in accelerations)
        {
            Add(acceleration);
        }
    }

    /// <summary>
    ///  Gets (ReadOnly) the element at the specified index. In C#, this property is the indexer for
    ///  the IList class.
    /// </summary>
    public NumericUpDownAcceleration this[int index]
    {
        get { return _items[index]; }
    }
}

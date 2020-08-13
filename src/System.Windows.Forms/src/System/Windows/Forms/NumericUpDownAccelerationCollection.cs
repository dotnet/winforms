// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a SORTED collection of NumericUpDownAcceleration objects in the NumericUpDown Control.
    ///  The elements in the collection are sorted by the NumericUpDownAcceleration.Seconds property.
    /// </summary>
    [ListBindable(false)]
    public class NumericUpDownAccelerationCollection : MarshalByRefObject, ICollection<NumericUpDownAcceleration>, IEnumerable<NumericUpDownAcceleration>
    {
        readonly List<NumericUpDownAcceleration> items;

        /// <summary>
        ///  Adds an item (NumericUpDownAcceleration object) to the ICollection.
        ///  The item is added preserving the collection sorted.
        /// </summary>
        public void Add(NumericUpDownAcceleration acceleration)
        {
            if (acceleration is null)
            {
                throw new ArgumentNullException(nameof(acceleration));
            }

            // Keep the array sorted, insert in the right spot.
            int index = 0;

            while (index < items.Count)
            {
                if (acceleration.Seconds < items[index].Seconds)
                {
                    break;
                }
                index++;
            }
            items.Insert(index, acceleration);
        }

        /// <summary>
        ///  Removes all items from the ICollection.
        /// </summary>
        public void Clear()
        {
            items.Clear();
        }

        /// <summary>
        ///  Determines whether the IList contains a specific value.
        /// </summary>
        public bool Contains(NumericUpDownAcceleration acceleration)
        {
            return items.Contains(acceleration);
        }

        /// <summary>
        ///  Copies the elements of the ICollection to an Array, starting at a particular Array index.
        /// </summary>
        public void CopyTo(NumericUpDownAcceleration[] array, int index)
        {
            items.CopyTo(array, index);
        }

        /// <summary>
        ///  Gets the number of elements contained in the ICollection.
        /// </summary>
        public int Count
        {
            get { return items.Count; }
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
            return items.Remove(acceleration);
        }

        /// <summary>
        ///  Returns an enumerator that can iterate through the collection.
        /// </summary>
        IEnumerator<NumericUpDownAcceleration> IEnumerable<NumericUpDownAcceleration>.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)items).GetEnumerator();
        }

        ///  NumericUpDownAccelerationCollection methods.

        /// <summary>
        ///  Class constructor.
        /// </summary>
        public NumericUpDownAccelerationCollection()
        {
            items = new List<NumericUpDownAcceleration>();
        }

        /// <summary>
        ///  Adds the elements of specified array to the collection, keeping the collection sorted.
        /// </summary>
        public void AddRange(params NumericUpDownAcceleration[] accelerations)
        {
            if (accelerations is null)
            {
                throw new ArgumentNullException(nameof(accelerations));
            }

            // Accept the range only if ALL elements in the array are not null.
            foreach (NumericUpDownAcceleration acceleration in accelerations)
            {
                if (acceleration is null)
                {
                    throw new ArgumentNullException(SR.NumericUpDownAccelerationCollectionAtLeastOneEntryIsNull);
                }
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
            get { return items[index]; }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///  A collection that stores <see cref='Adorner' /> objects.
    /// </summary>
    /// <seealso cref='BehaviorServiceAdornerCollection' />
    public sealed class BehaviorServiceAdornerCollection : CollectionBase
    {
        /// <summary>
        ///  Initializes a new instance of <see cref='BehaviorServiceAdornerCollection' />.
        /// </summary>
        public BehaviorServiceAdornerCollection(BehaviorService behaviorService)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Initializes a new instance of <see cref='BehaviorServiceAdornerCollection'/> based on another
        ///  <see cref='BehaviorServiceAdornerCollection'/>.
        /// </summary>
        public BehaviorServiceAdornerCollection(BehaviorServiceAdornerCollection value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Initializes a new instance of <see cref='BehaviorServiceAdornerCollection' />
        ///  containing any array of <see cref='Adorner' /> objects.
        /// </summary>
        /// <param name='value'>
        ///  A array of <see cref='Adorner' /> objects with which to intialize the collection.
        /// </param>
        public BehaviorServiceAdornerCollection(Adorner[] value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Represents the entry at the specified index of the <see cref='Adorner' />.
        /// </summary>
        /// <param name='index'>
        ///  The zero-based index of the entry to locate in the collection.
        /// </param>
        /// <value>
        ///  The entry at the specified index of the collection.
        /// </value>
        /// <exception cref='ArgumentOutOfRangeException'>
        ///  <paramref name='index' /> is outside the valid range of indexes for the collection.
        /// </exception>
        public Adorner this[int index]
        {
            get => throw new NotImplementedException(SR.NotImplementedByDesign);
            set => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Adds a <see cref='Adorner' /> with the specified value to the
        ///  <see cref='BehaviorServiceAdornerCollection' /> .
        /// </summary>
        /// <param name='value'>The <see cref='Adorner' /> to add.</param>
        /// <returns>
        ///  The index at which the new element was inserted.
        /// </returns>
        /// <seealso cref='BehaviorServiceAdornerCollection.AddRange' />
        public int Add(Adorner value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Copies the elements of an array to the end of the
        ///  <see cref='BehaviorServiceAdornerCollection' />.
        /// </summary>
        /// <param name='value'>
        ///  An array of type <see cref='Adorner' /> containing the objects to add to the
        ///  collection.
        /// </param>
        /// <returns>
        ///  None.
        /// </returns>
        /// <seealso cref='Add' />
        public void AddRange(Adorner[] value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Adds the contents of another
        ///  <see cref='BehaviorServiceAdornerCollection' /> to the end of the
        ///  collection.
        /// </summary>
        /// <param name='value'>
        ///  A <see cref='BehaviorServiceAdornerCollection' /> containing the objects to
        ///  add to the collection.
        /// </param>
        /// <returns>
        ///  None.
        /// </returns>
        /// <seealso cref='Add' />
        public void AddRange(BehaviorServiceAdornerCollection value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Gets a value indicating whether the
        ///  <see cref='BehaviorServiceAdornerCollection' /> contains the specified
        ///  <see cref='Adorner' />.
        /// </summary>
        /// <param name='value'>The <see cref='Adorner' /> to locate.</param>
        /// <returns>
        ///  <see langword='true' /> if the <see cref='Adorner' /> is contained in the
        ///  collection;
        ///  otherwise, <see langword='false' />.
        /// </returns>
        /// <seealso cref='IndexOf' />
        public bool Contains(Adorner value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Copies the <see cref='BehaviorServiceAdornerCollection' /> values to a
        ///  one-dimensional <see cref='Array' /> instance at the
        ///  specified index.
        /// </summary>
        /// <param name='array'>
        ///  The one-dimensional <see cref='Array' /> that is the destination of the values copied from
        ///  <see cref='BehaviorServiceAdornerCollection' /> .
        /// </param>
        /// <param name='index'>The index in <paramref name='array' /> where copying begins.</param>
        /// <returns>
        ///  None.
        /// </returns>
        /// <exception cref='ArgumentException'>
        ///  <paramref name='array' /> is multidimensional.
        ///  -or-
        ///  The number of elements in the
        ///  <see cref='BehaviorServiceAdornerCollection' /> is greater than the
        ///  available space between <paramref name='index' /> and the end of <paramref name='array' />.
        /// </exception>
        /// <exception cref='ArgumentNullException'><paramref name='array' /> is <see langword='null' />. </exception>
        /// <exception cref='ArgumentOutOfRangeException'>
        ///  <paramref name='arrayIndex' /> is less than
        ///  <paramref name='array' />'s lowbound.
        /// </exception>
        /// <seealso cref='Array' />
        public void CopyTo(Adorner[] array, int index)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Returns the index of a <see cref='Adorner' /> in
        ///  the <see cref='BehaviorServiceAdornerCollection' /> .
        /// </summary>
        /// <param name='value'>The <see cref='Adorner' /> to locate.</param>
        /// <returns>
        ///  The index of the <see cref='Adorner' /> of <paramref name='value' /> in
        ///  the
        ///  <see cref='BehaviorServiceAdornerCollection' />, if found; otherwise, -1.
        /// </returns>
        /// <seealso cref='Contains' />
        public int IndexOf(Adorner value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Inserts a <see cref='Adorner' /> into the
        ///  <see cref='BehaviorServiceAdornerCollection' /> at the specified index.
        /// </summary>
        /// <param name='index'>The zero-based index where <paramref name='value' /> should be inserted.</param>
        /// <param name=' value'>The <see cref='Adorner' /> to insert.</param>
        /// <returns>
        ///  None.
        /// </returns>
        /// <seealso cref='Add' />
        public void Insert(int index, Adorner value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Returns an enumerator that can iterate through
        ///  the <see cref='BehaviorServiceAdornerCollection' /> .
        /// </summary>
        /// <returns>
        ///  None.
        /// </returns>
        /// <seealso cref='IEnumerator' />
        public new BehaviorServiceAdornerCollectionEnumerator GetEnumerator()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///  Removes a specific <see cref='Adorner' /> from the
        ///  <see cref='BehaviorServiceAdornerCollection' /> .
        /// </summary>
        /// <param name='value'>
        ///  The <see cref='Adorner' /> to remove from the
        ///  <see cref='BehaviorServiceAdornerCollection' /> .
        /// </param>
        /// <returns>
        ///  None.
        /// </returns>
        /// <exception cref='ArgumentException'><paramref name='value' /> is not found in the Collection. </exception>
        public void Remove(Adorner value)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }

    public class BehaviorServiceAdornerCollectionEnumerator : object, IEnumerator
    {
        public BehaviorServiceAdornerCollectionEnumerator(BehaviorServiceAdornerCollection mappings)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public Adorner Current => throw new NotImplementedException(SR.NotImplementedByDesign);

        object IEnumerator.Current => throw new NotImplementedException();

        public bool MoveNext()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public void Reset()
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}

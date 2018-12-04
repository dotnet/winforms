// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///     A strongly-typed collection that stores Behavior.Glyph objects.
    /// </summary>
    public class GlyphCollection : CollectionBase
    {
        /// <summary>
        ///     Initializes a new instance of Behavior.GlyphCollection.
        /// </summary>
        public GlyphCollection()
        {
        }

        /// <summary>
        ///     Initializes a new instance of Behavior.GlyphCollection based on another Behavior.GlyphCollection.
        /// </summary>
        public GlyphCollection(GlyphCollection value)
        {
            AddRange(value);
        }

        /// <summary>
        ///     Initializes a new instance of Behavior.GlyphCollection containing any array of Behavior.Glyph objects.
        /// </summary>
        public GlyphCollection(Glyph[] value)
        {
            AddRange(value);
        }

        /// <summary>
        ///     Represents the entry at the specified index of the Behavior.Glyph.
        /// </summary>
        public Glyph this[int index]
        {
            get => (Glyph)List[index];
            set => List[index] = value;
        }

        /// <summary>
        ///     Adds a Behavior.Glyph with the specified value to the
        ///     Behavior.GlyphCollection .
        /// </summary>
        public int Add(Glyph value)
        {
            return List.Add(value);
        }

        /// <summary>
        ///     Copies the elements of an array to the end of the Behavior.GlyphCollection.
        /// </summary>
        public void AddRange(Glyph[] value)
        {
            for (int i = 0; i < value.Length; i = i + 1) Add(value[i]);
        }

        /// <summary>
        ///     Adds the contents of another Behavior.GlyphCollection to the end of the collection.
        /// </summary>
        public void AddRange(GlyphCollection value)
        {
            for (int i = 0; i < value.Count; i = i + 1) Add(value[i]);
        }

        /// <summary>
        ///     Gets a value indicating whether the
        ///     Behavior.GlyphCollection contains the specified Behavior.Glyph.
        /// </summary>
        public bool Contains(Glyph value)
        {
            return List.Contains(value);
        }

        /// <summary>
        ///     Copies the Behavior.GlyphCollection values to a one-dimensional
        ///     <see cref='System.Array instance at the 
        /// specified index.
        /// 
        /// </summary>
        public void CopyTo(Glyph[] array, int index)
        {
            List.CopyTo(array, index);
        }

        /// <summary>
        ///     Returns the index of a Behavior.Glyph in
        ///     the Behavior.GlyphCollection .
        /// </summary>
        public int IndexOf(Glyph value)
        {
            return List.IndexOf(value);
        }

        /// <summary>
        ///     Inserts a Behavior.Glyph into the Behavior.GlyphCollection at the specified index.
        /// </summary>
        public void Insert(int index, Glyph value)
        {
            List.Insert(index, value);
        }

        /// <summary>
        ///     Removes a specific Behavior.Glyph from the
        ///     Behavior.GlyphCollection .
        /// </summary>
        public void Remove(Glyph value)
        {
            List.Remove(value);
        }
    }
}

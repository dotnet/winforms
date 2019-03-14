﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// A read-only collection of GridItem objects
    /// </devdoc>
    public class GridItemCollection : ICollection
    {
        public static GridItemCollection Empty = new GridItemCollection(Array.Empty<GridItem>());

        private protected GridItem[] _entries;

        internal GridItemCollection(GridItem[] entries)
        {
            _entries = entries ?? Array.Empty<GridItem>();
        }

        /// <devdoc>
        /// Retrieves the number of member attributes.
        /// </devdoc>
        public int Count => _entries.Length;

        object ICollection.SyncRoot => this;

        bool ICollection.IsSynchronized => false;

        /// <devdoc>
        /// Retrieves the member attribute with the specified index.
        /// </devdoc>
        public GridItem this[int index] => _entries[index];

        public GridItem this[string label]
        {
            get
            {
                foreach (GridItem g in _entries)
                {
                    if (g.Label == label)
                    {
                        return g;
                    }
                }

                return null;
            }
        }

        void ICollection.CopyTo(Array dest, int index)
        {
            if (_entries.Length > 0)
            {
                Array.Copy(_entries, 0, dest, index, _entries.Length);
            }
        }

        /// <devdoc>
        /// Creates and retrieves a new enumerator for this collection.
        /// </devdoc>
        public IEnumerator GetEnumerator() => _entries.GetEnumerator();
    }
}

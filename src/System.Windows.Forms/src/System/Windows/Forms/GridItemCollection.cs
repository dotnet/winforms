// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Collections;

    /// <devdoc>
    ///  A read-only collection of GridItem objects
    /// </devdoc>
    public class GridItemCollection : ICollection {
    
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public static GridItemCollection Empty = new GridItemCollection(new GridItem[0]);

        internal GridItem[] entries;
        
        internal GridItemCollection(GridItem[] entries) {
            if (entries == null) {
               this.entries = new GridItem[0];
            }
            else {
               this.entries = entries;
            }
        }
        
        /// <devdoc>
        ///     Retrieves the number of member attributes.
        /// </devdoc>
        public int Count {
            get {
                return entries.Length;
            }
        }

        /// <internalonly/>
        object ICollection.SyncRoot {
            get {
                return this;
            }
        }

        /// <internalonly/>
        bool ICollection.IsSynchronized {
            get {
                return false;
            }
        }

        /// <devdoc>
        ///     Retrieves the member attribute with the specified index.
        /// </devdoc>
        public GridItem this[int index] {
            get {
                return entries[index];
            }
        }
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public GridItem this[string label]{
            get {
                foreach(GridItem g in entries) {
                    if (g.Label == label) {
                        return g;
                    }
                }
                return null;
            }
        }

        /// <internalonly/>
        void ICollection.CopyTo(Array dest, int index) {
            if (entries.Length > 0) {
                System.Array.Copy(entries, 0, dest, index, entries.Length);
            }
        }
        /// <devdoc>
        ///      Creates and retrieves a new enumerator for this collection.
        /// </devdoc>
        public IEnumerator GetEnumerator() {
            return entries.GetEnumerator();
        }

    }
    
}

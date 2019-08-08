// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides the base functionality for creating collections.
    /// </summary>
    public class BaseCollection : MarshalByRefObject, ICollection
    {
        /// <summary>
        ///  Gets the total number of elements in a collection.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual int Count => List.Count;

        public void CopyTo(Array ar, int index) => List.CopyTo(ar, index);

        /// <summary>
        ///  Gets an IEnumerator for the collection.
        /// </summary>
        public IEnumerator GetEnumerator() => List.GetEnumerator();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsReadOnly => false;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsSynchronized => false;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public object SyncRoot => this;

        protected virtual ArrayList List => null;
    }
}

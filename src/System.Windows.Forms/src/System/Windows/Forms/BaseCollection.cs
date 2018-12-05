// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Runtime.InteropServices;

    using System;
    using System.ComponentModel;
    using System.Collections;
    using ArrayList = System.Collections.ArrayList;

    /// <devdoc>
    ///    <para>Provides the base functionality for creating collections.</para>
    /// </devdoc>
    public class BaseCollection : MarshalByRefObject, ICollection {

        //==================================================
        // the ICollection methods
        //==================================================
        /// <devdoc>
        ///    <para>Gets the total number of elements in a collection.</para>
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public virtual int Count {
            get {
                return List.Count;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public void CopyTo(Array ar, int index) {
            List.CopyTo(ar, index);
        }

        /// <devdoc>
        ///    <para>Gets an IEnumerator for the collection.</para>
        /// </devdoc>
        public IEnumerator GetEnumerator() {
            return List.GetEnumerator();
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public bool IsReadOnly {
            get {
                return false;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsSynchronized {
            get {
                // so the user will know that it has to lock this object
                return false;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public object SyncRoot {
            get {
                return this;
            }
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        protected virtual ArrayList List {
            get {
                return null;
            }
        }
    }
}

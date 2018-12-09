// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Runtime.InteropServices;

    using System;
    using System.ComponentModel;
    using System.Collections;
    using ArrayList = System.Collections.ArrayList;

    /// <include file='doc\BaseCollection.uex' path='docs/doc[@for="BaseCollection"]/*' />
    /// <devdoc>
    ///    <para>Provides the base functionality for creating collections.</para>
    /// </devdoc>
    public class BaseCollection : MarshalByRefObject, ICollection {

        //==================================================
        // the ICollection methods
        //==================================================
        /// <include file='doc\BaseCollection.uex' path='docs/doc[@for="BaseCollection.Count"]/*' />
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

        /// <include file='doc\BaseCollection.uex' path='docs/doc[@for="BaseCollection.CopyTo"]/*' />
        public void CopyTo(Array ar, int index) {
            List.CopyTo(ar, index);
        }

        /// <include file='doc\BaseCollection.uex' path='docs/doc[@for="BaseCollection.GetEnumerator"]/*' />
        /// <devdoc>
        ///    <para>Gets an IEnumerator for the collection.</para>
        /// </devdoc>
        public IEnumerator GetEnumerator() {
            return List.GetEnumerator();
        }

        /// <include file='doc\BaseCollection.uex' path='docs/doc[@for="BaseCollection.IsReadOnly"]/*' />
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public bool IsReadOnly {
            get {
                return false;
            }
        }

        /// <include file='doc\BaseCollection.uex' path='docs/doc[@for="BaseCollection.IsSynchronized"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsSynchronized {
            get {
                // so the user will know that it has to lock this object
                return false;
            }
        }

        /// <include file='doc\BaseCollection.uex' path='docs/doc[@for="BaseCollection.SyncRoot"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public object SyncRoot {
            get {
                return this;
            }
        }

        /// <include file='doc\BaseCollection.uex' path='docs/doc[@for="BaseCollection.List"]/*' />
        protected virtual ArrayList List {
            get {
                return null;
            }
        }
    }
}

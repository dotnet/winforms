// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Globalization;
    
    /// <include file='doc\FormCollection.uex' path='docs/doc[@for="FormCollection"]/*' />
    /// <devdoc>
    ///    <para>
    ///       This is a read only collection of Forms exposed as a static property of the 
    ///       Application class. This is used to store all the currently loaded forms in an app.
    ///    </para>
    /// </devdoc>
    public class FormCollection : ReadOnlyCollectionBase {

        internal static object CollectionSyncRoot = new object();
        
        /// <include file='doc\FormCollection.uex' path='docs/doc[@for="FormCollection.this"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a form specified by name, if present, else returns null. If there are multiple
        ///       forms with matching names, the first form found is returned.
        ///    </para>
        /// </devdoc>
        public virtual Form this[string name] {
            get {
                if (name != null) {
                    lock (CollectionSyncRoot) {
                        foreach(Form form in InnerList) {
                            if (string.Equals(form.Name, name, StringComparison.OrdinalIgnoreCase)) {
                                return form;
                            }
                        }
                    }
                }
                return null;
            }
        }
        
        /// <include file='doc\FormCollection.uex' path='docs/doc[@for="FormCollection.this1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a form specified by index.
        ///    </para>
        /// </devdoc>
        public virtual Form this[int index] {
            get {
                Form f = null;

                lock (CollectionSyncRoot) {
                    f = (Form) InnerList[index];
                }
                return f;
            }
        }
        
        /// <devdoc>
        ///    Used internally to add a Form to the FormCollection
        /// </devdoc>
        internal void Add(Form form) {
            lock (CollectionSyncRoot) {
                InnerList.Add(form);
            }
        }

        /// <devdoc>
        ///    Used internally to check if a Form is in the FormCollection
        /// </devdoc>
        internal bool Contains(Form form)
        {
            bool inCollection = false;
            lock (CollectionSyncRoot)
            {
                inCollection = InnerList.Contains(form);
            }
            return inCollection;
        }

        /// <devdoc>
        ///    Used internally to add a Form to the FormCollection
        /// </devdoc>
        internal void Remove(Form form) {
            lock (CollectionSyncRoot) {
                InnerList.Remove(form);
            }
        }
    }
}


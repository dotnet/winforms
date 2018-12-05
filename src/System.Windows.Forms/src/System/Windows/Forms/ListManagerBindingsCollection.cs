// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using Microsoft.Win32;
    using System.Diagnostics;    
    using System.ComponentModel;
    using System.Collections;
    
    /// <include file='doc\ListManagerBindingsCollection.uex' path='docs/doc[@for="ListManagerBindingsCollection"]/*' />
    /// <devdoc>
    /// BindingsCollection is a collection of bindings for a Control.  It has Add/Remove capabilities,
    /// as well as an All array property, enumeration, etc.
    /// </devdoc>
    [DefaultEvent(nameof(CollectionChanged))]
    internal class ListManagerBindingsCollection : BindingsCollection {

        private BindingManagerBase bindingManagerBase;
        
        /// <include file='doc\ListManagerBindingsCollection.uex' path='docs/doc[@for="ListManagerBindingsCollection.ListManagerBindingsCollection"]/*' />
        /// <devdoc>
        /// ColumnsCollection constructor.  Used only by DataSource.
        /// </devdoc>
        internal ListManagerBindingsCollection(BindingManagerBase bindingManagerBase) : base() {
            Debug.Assert(bindingManagerBase != null, "How could a listmanagerbindingscollection not have a bindingManagerBase associated with it!");
            this.bindingManagerBase = bindingManagerBase;
        }

        protected override void AddCore(Binding dataBinding) {
            if (dataBinding == null)
                throw new ArgumentNullException(nameof(dataBinding));
            if (dataBinding.BindingManagerBase == bindingManagerBase)
                throw new ArgumentException(SR.BindingsCollectionAdd1, "dataBinding");
            if (dataBinding.BindingManagerBase != null)
                throw new ArgumentException(SR.BindingsCollectionAdd2, "dataBinding");

            // important to set prop first for error checking.
            dataBinding.SetListManager(bindingManagerBase);

            base.AddCore(dataBinding);
        }

        protected override void ClearCore() {
            int numLinks = Count;
            for (int i = 0; i < numLinks; i++) {
                Binding dataBinding = this[i];
                dataBinding.SetListManager(null);
            }
            base.ClearCore();
        }

        protected override void RemoveCore(Binding dataBinding) {
            if (dataBinding.BindingManagerBase != bindingManagerBase)
                throw new ArgumentException(SR.BindingsCollectionForeign);
            dataBinding.SetListManager(null);
            base.RemoveCore(dataBinding);
        }
    }
}

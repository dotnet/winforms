// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using Microsoft.Win32;
    using System.Diagnostics;    
    using System.ComponentModel;
    using System.Collections;
    
    /// <include file='doc\BindingsCollection.uex' path='docs/doc[@for="BindingsCollection"]/*' />
    /// <devdoc>
    ///    <para>Represents a collection of data bindings on a control.</para>
    /// </devdoc>
    [DefaultEvent(nameof(CollectionChanged))]
    public class BindingsCollection : System.Windows.Forms.BaseCollection {

        private ArrayList list;
        private CollectionChangeEventHandler onCollectionChanging;
        private CollectionChangeEventHandler onCollectionChanged;

        // internalonly
        internal BindingsCollection() {
        }

        /// <include file='doc\BindingsCollection.uex' path='docs/doc[@for="BindingsCollection.Count"]/*' />
        public override int Count {
            get {
                if (list == null) {
                    return 0;
                }
                return base.Count;
            }
        }

        /// <include file='doc\BindingsCollection.uex' path='docs/doc[@for="BindingsCollection.List"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Gets the bindings in the collection as an object.
        ///    </para>
        /// </devdoc>
        protected override ArrayList List {
            get {
                if (list == null)
                    list = new ArrayList();
                return list;
            }
        }
        
        /// <include file='doc\BindingsCollection.uex' path='docs/doc[@for="BindingsCollection.this"]/*' />
        /// <devdoc>
        /// <para>Gets the <see cref='System.Windows.Forms.Binding'/> at the specified index.</para>
        /// </devdoc>
        public Binding this[int index] {
            get {
                return (Binding) List[index];
            }
        }

        /// <include file='doc\BindingsCollection.uex' path='docs/doc[@for="BindingsCollection.Add"]/*' />
        // internalonly
        internal protected void Add(Binding binding) {
            CollectionChangeEventArgs ccevent = new CollectionChangeEventArgs(CollectionChangeAction.Add, binding);
            OnCollectionChanging(ccevent);
            AddCore(binding);
            OnCollectionChanged(ccevent);
        }

        // internalonly
        /// <include file='doc\BindingsCollection.uex' path='docs/doc[@for="BindingsCollection.AddCore"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Adds a <see cref='System.Windows.Forms.Binding'/>
        ///       to the collection.
        ///    </para>
        /// </devdoc>
        protected virtual void AddCore(Binding dataBinding) {
            if (dataBinding == null)
                throw new ArgumentNullException(nameof(dataBinding));

            List.Add(dataBinding);
        }

        /// <include file='doc\BindingsCollection.uex' path='docs/doc[@for="BindingsCollection.CollectionChanging"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs when the collection is about to change.
        ///    </para>
        /// </devdoc>
        [SRDescription(nameof(SR.collectionChangingEventDescr))]
        public event CollectionChangeEventHandler CollectionChanging {
            add {
                onCollectionChanging += value;
            }
            remove {
                onCollectionChanging -= value;
            }
        }

        /// <include file='doc\BindingsCollection.uex' path='docs/doc[@for="BindingsCollection.CollectionChanged"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs when the collection is changed.
        ///    </para>
        /// </devdoc>
        [SRDescription(nameof(SR.collectionChangedEventDescr))]
        public event CollectionChangeEventHandler CollectionChanged {
            add {
                onCollectionChanged += value;
            }
            remove {
                onCollectionChanged -= value;
            }
        }

        // internalonly
        /// <include file='doc\BindingsCollection.uex' path='docs/doc[@for="BindingsCollection.Clear"]/*' />
        internal protected void Clear() {
            CollectionChangeEventArgs ccevent = new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null);
            OnCollectionChanging(ccevent);
            ClearCore();
            OnCollectionChanged(ccevent);
        }

        // internalonly
        /// <include file='doc\BindingsCollection.uex' path='docs/doc[@for="BindingsCollection.ClearCore"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Clears the collection of any members.
        ///    </para>
        /// </devdoc>
        protected virtual void ClearCore() {
            List.Clear();
        }

        /// <include file='doc\BindingsCollection.uex' path='docs/doc[@for="BindingsCollection.OnCollectionChanging"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.BindingsCollection.CollectionChanging'/> event.
        ///    </para>
        /// </devdoc>
        protected virtual void OnCollectionChanging(CollectionChangeEventArgs e) {
            if (onCollectionChanging != null) {
                onCollectionChanging(this, e);
            }
        }

        /// <include file='doc\BindingsCollection.uex' path='docs/doc[@for="BindingsCollection.OnCollectionChanged"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.BindingsCollection.CollectionChanged'/> event.
        ///    </para>
        /// </devdoc>
        protected virtual void OnCollectionChanged(CollectionChangeEventArgs ccevent) {
            if (onCollectionChanged != null) {
                onCollectionChanged(this, ccevent);
            }
        }

        /// <include file='doc\BindingsCollection.uex' path='docs/doc[@for="BindingsCollection.Remove"]/*' />
        // internalonly
        internal protected void Remove(Binding binding) {
            CollectionChangeEventArgs ccevent = new CollectionChangeEventArgs(CollectionChangeAction.Remove, binding);
            OnCollectionChanging(ccevent);
            RemoveCore(binding);
            OnCollectionChanged(ccevent);
        }


        /// <include file='doc\BindingsCollection.uex' path='docs/doc[@for="BindingsCollection.RemoveAt"]/*' />
        // internalonly
        internal protected void RemoveAt(int index) {
            Remove(this[index]);
        }

        // internalonly
        /// <include file='doc\BindingsCollection.uex' path='docs/doc[@for="BindingsCollection.RemoveCore"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Removes the specified <see cref='System.Windows.Forms.Binding'/> from the collection.
        ///    </para>
        /// </devdoc>
        protected virtual void RemoveCore(Binding dataBinding) {
            List.Remove(dataBinding);
        }


        /// <include file='doc\BindingsCollection.uex' path='docs/doc[@for="BindingsCollection.ShouldSerializeMyAll"]/*' />
        // internalonly
        internal protected bool ShouldSerializeMyAll() {
            return Count > 0;
        }
    }
}

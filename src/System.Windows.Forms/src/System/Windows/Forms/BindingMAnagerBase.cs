// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Collections;
    using System.Diagnostics.CodeAnalysis;


    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors")] // Shipped in Everett
    public abstract class BindingManagerBase
    {
        private BindingsCollection bindings;
        private bool pullingData = false;


        protected EventHandler onCurrentChangedHandler;


        protected EventHandler onPositionChangedHandler;

        // Hook BindingComplete events on all owned Binding objects, and propagate those events through our own BindingComplete event
        private BindingCompleteEventHandler onBindingCompleteHandler = null;

        // same deal about the new currentItemChanged event
        internal EventHandler onCurrentItemChangedHandler;

        // Event handler for the DataError event
        internal BindingManagerDataErrorEventHandler onDataErrorHandler;


        public BindingsCollection Bindings {
            get {
                if (bindings == null) {
                    bindings = new ListManagerBindingsCollection(this);

                    // Hook collection change events on collection, so we can hook or unhook the BindingComplete events on individual bindings
                    bindings.CollectionChanging += new CollectionChangeEventHandler(OnBindingsCollectionChanging);
                    bindings.CollectionChanged += new CollectionChangeEventHandler(OnBindingsCollectionChanged);
                }

                return bindings;
            }
        }


        internal protected void OnBindingComplete(BindingCompleteEventArgs args) {
            if (onBindingCompleteHandler != null) {
                onBindingCompleteHandler(this, args);
            }
        }


        internal protected abstract void OnCurrentChanged(EventArgs e);


        internal protected abstract void OnCurrentItemChanged(EventArgs e);


        internal protected void OnDataError(Exception e) {
            if (onDataErrorHandler != null) {
                onDataErrorHandler(this, new BindingManagerDataErrorEventArgs(e));
            }
        }


        public abstract object Current {
            get;
        }

        internal abstract void SetDataSource(object dataSource);


        public BindingManagerBase() { }

        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // If the constructor does not call SetDataSource
                                                                                                    // it would be a breaking change.
        ]
        internal BindingManagerBase(object dataSource) {
            this.SetDataSource(dataSource);
        }

        internal abstract Type BindType{
            get;
        }

        internal abstract PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors);


        public virtual PropertyDescriptorCollection GetItemProperties() {
            return GetItemProperties(null);
        }


        protected internal virtual PropertyDescriptorCollection GetItemProperties(ArrayList dataSources, ArrayList listAccessors) {
            IList list = null;
            if (this is CurrencyManager) {
                list = ((CurrencyManager)this).List;
            }
            if (list is ITypedList) {
                PropertyDescriptor[] properties = new PropertyDescriptor[listAccessors.Count];
                listAccessors.CopyTo(properties, 0);
                return ((ITypedList)list).GetItemProperties(properties);
            }
            return this.GetItemProperties(this.BindType, 0, dataSources, listAccessors);
        }

        // listType is the type of the top list in the list.list.list.list reference
        // offset is how far we are in the listAccessors
        // listAccessors is the list of accessors (duh)
        //

        protected virtual PropertyDescriptorCollection GetItemProperties(Type listType, int offset, ArrayList dataSources, ArrayList listAccessors) {
            if (listAccessors.Count < offset)
                return null;

            if (listAccessors.Count == offset) {
                if (typeof(IList).IsAssignableFrom(listType)) {
                    System.Reflection.PropertyInfo[] itemProps = listType.GetProperties();
                    // PropertyDescriptorCollection itemProps = TypeDescriptor.GetProperties(listType);
                    for (int i = 0; i < itemProps.Length; i ++) {
                        if ("Item".Equals(itemProps[i].Name) && itemProps[i].PropertyType != typeof(object))
                            return TypeDescriptor.GetProperties(itemProps[i].PropertyType, new Attribute[] {new BrowsableAttribute(true)});
                    }
                    // return the properties on the type of the first element in the list
                    IList list = dataSources[offset - 1] as IList;
                    if (list != null && list.Count > 0)
                        return TypeDescriptor.GetProperties(list[0]);
                } else {
                    return TypeDescriptor.GetProperties(listType);
                }
                return null;
            }

            System.Reflection.PropertyInfo[] props = listType.GetProperties();
            // PropertyDescriptorCollection props = TypeDescriptor.GetProperties(listType);
            if (typeof(IList).IsAssignableFrom(listType)) {
                PropertyDescriptorCollection itemProps = null;
                for (int i = 0; i < props.Length; i++) {
                    if ("Item".Equals(props[i].Name) && props[i].PropertyType != typeof(object)) {
                        // get all the properties that are not marked as Browsable(false)
                        //
                        itemProps = TypeDescriptor.GetProperties(props[i].PropertyType, new Attribute[] {new BrowsableAttribute(true)});
                    }
                }

                if (itemProps == null) {
                    // use the properties on the type of the first element in the list
                    // if offset == 0, then this means that the first dataSource did not have a strongly typed Item property.
                    // the dataSources are added only for relatedCurrencyManagers, so in this particular case
                    // we need to use the dataSource in the currencyManager.
                    IList list;
                    if (offset == 0)
                        list = this.DataSource as IList;
                    else
                        list = dataSources[offset - 1] as IList;
                    if (list != null && list.Count > 0) {
                        itemProps = TypeDescriptor.GetProperties(list[0]);
                    }
                }

                if (itemProps != null) {
                    for (int j=0; j<itemProps.Count; j++) {
                        if (itemProps[j].Equals(listAccessors[offset]))
                            return this.GetItemProperties(itemProps[j].PropertyType, offset + 1, dataSources, listAccessors);
                    }
                }

            } else {
                for (int i = 0; i < props.Length; i++) {
                    if (props[i].Name.Equals(((PropertyDescriptor)listAccessors[offset]).Name))
                        return this.GetItemProperties(props[i].PropertyType, offset + 1, dataSources, listAccessors);
                }
            }
            return null;
        }


        public event BindingCompleteEventHandler BindingComplete {
            add {
                onBindingCompleteHandler += value;
            }
            remove {
                onBindingCompleteHandler -= value;
            }
        }


        public event EventHandler CurrentChanged {
            add {
                onCurrentChangedHandler += value;
            }
            remove {
                onCurrentChangedHandler -= value;
            }
        }


        public event EventHandler CurrentItemChanged {
            add {
                onCurrentItemChangedHandler += value;
            }
            remove {
                onCurrentItemChangedHandler -= value;
            }
        }


        public event BindingManagerDataErrorEventHandler DataError {
            add {
                onDataErrorHandler += value;
            }
            remove {
                onDataErrorHandler -= value;
            }
        }

        internal abstract string GetListName();

        public abstract void CancelCurrentEdit();

        public abstract void EndCurrentEdit();


        public abstract void AddNew();

        public abstract void RemoveAt(int index);


        public abstract int Position{get; set;}


        public event EventHandler PositionChanged {
            add {
                this.onPositionChangedHandler += value;
            }
            remove {
                this.onPositionChangedHandler -= value;
            }
        }

        protected abstract void UpdateIsBinding();


        protected internal abstract string GetListName(ArrayList listAccessors);


        public abstract void SuspendBinding();


        public abstract void ResumeBinding();


        protected void PullData() {
            bool success;
            PullData(out success);
        }


        internal void PullData(out bool success) {
            success = true;
            pullingData = true;

            try {
                UpdateIsBinding();

                int numLinks = Bindings.Count;
                for (int i = 0; i < numLinks; i++) {
                    if (Bindings[i].PullData()) {
                        success = false;
                    }
                }
            }
            finally {
                pullingData = false;
            }
        }


        protected void PushData() {
            bool success;
            PushData(out success);
        }


        internal void PushData(out bool success) {
            success = true;

            if (pullingData)
                return;

            UpdateIsBinding();

            int numLinks = Bindings.Count;
            for (int i = 0; i < numLinks; i++) {
                if (Bindings[i].PushData()) {
                    success = false;
                }
            }
        }

        internal abstract object DataSource {
            get;
        }

        internal abstract bool IsBinding {
            get;
        }


        /// <devdoc>
        /// <devdoc>
        public bool IsBindingSuspended {
            get {
                return !IsBinding;
            }
        }


        public abstract int Count {
            get;
        }

        // BindingComplete events on individual Bindings are propagated up through the BindingComplete event on
        // the owning BindingManagerBase. To do this, we have to track changes to the bindings collection, adding
        // or removing handlers on items in the collection as appropriate.
        //
        // For the Add and Remove cases, we hook the collection 'changed' event, and add or remove handler for
        // specific binding.
        //
        // For the Refresh case, we hook both the 'changing' and 'changed' events, removing handlers for all
        // items that were in the collection before the change, then adding handlers for whatever items are
        // in the collection after the change.
        //
        private void OnBindingsCollectionChanged(object sender, CollectionChangeEventArgs e) {
            Binding b = e.Element as Binding;

            switch (e.Action) {
                case CollectionChangeAction.Add:
                    b.BindingComplete += new BindingCompleteEventHandler(Binding_BindingComplete);
                    break;
                case CollectionChangeAction.Remove:
                    b.BindingComplete -= new BindingCompleteEventHandler(Binding_BindingComplete);
                    break;
                case CollectionChangeAction.Refresh:
                    foreach (Binding bi in bindings) {
                        bi.BindingComplete += new BindingCompleteEventHandler(Binding_BindingComplete);
                    }
                    break;
            }
        }

        private void OnBindingsCollectionChanging(object sender, CollectionChangeEventArgs e) {
            if (e.Action == CollectionChangeAction.Refresh) {
                foreach (Binding bi in bindings) {
                    bi.BindingComplete -= new BindingCompleteEventHandler(Binding_BindingComplete);
                }
            }
        }

        internal void Binding_BindingComplete(object sender, BindingCompleteEventArgs args) {
            this.OnBindingComplete(args);
        }

    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using Microsoft.Win32;
    using System.Diagnostics;    
    using System.ComponentModel;
    using System.Collections;
    using System.Globalization;
    
    /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection"]/*' />
    /// <devdoc>
    ///    <para> 
    ///       Represents the collection of data bindings for a control.</para>
    /// </devdoc>
    [DefaultEvent(nameof(CollectionChanged)),
     Editor("System.Drawing.Design.UITypeEditor, " + AssemblyRef.SystemDrawing, typeof(System.Drawing.Design.UITypeEditor)),
     TypeConverterAttribute("System.Windows.Forms.Design.ControlBindingsConverter, " + AssemblyRef.SystemDesign),
     ]
    public class ControlBindingsCollection : BindingsCollection {

        internal IBindableComponent control;

        private DataSourceUpdateMode defaultDataSourceUpdateMode = DataSourceUpdateMode.OnValidation;

        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.ControlBindingsCollection"]/*' />
        public ControlBindingsCollection(IBindableComponent control) : base() {
            Debug.Assert(control != null, "How could a controlbindingscollection not have a control associated with it!");
            this.control = control;
        }

        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.BindableComponent"]/*' />
        public IBindableComponent BindableComponent {
            get {
                return this.control;
            }
        }
                
        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.Control"]/*' />
        public Control Control {
            get {
                return this.control as Control;
            }
        }

        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.this"]/*' />
        public Binding this[string propertyName] {
            get {
                foreach (Binding binding in this) {
                    if (String.Equals(binding.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return binding;
                    }
                }
                return null;
            }
        }
                
        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.Add"]/*' />
        /// <devdoc>
        /// Adds the binding to the collection.  An ArgumentNullException is thrown if this binding
        /// is null.  An exception is thrown if a binding to the same target and Property as an existing binding or
        /// if the binding's column isn't a valid column given this DataSource.Table's schema.
        /// Fires the CollectionChangedEvent.
        /// </devdoc>
        public new void Add(Binding binding) {
            base.Add(binding);
        }

        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.Add1"]/*' />
        /// <devdoc>
        /// Creates the binding and adds it to the collection.  An InvalidBindingException is thrown
        /// if this binding can't be constructed.  An exception is thrown if a binding to the same target and Property as an existing binding or
        /// if the binding's column isn't a valid column given this DataSource.Table's schema.
        /// Fires the CollectionChangedEvent.
        /// </devdoc>
        public Binding Add(string propertyName, object dataSource, string dataMember) {
            return Add(propertyName, dataSource, dataMember, false, this.DefaultDataSourceUpdateMode, null, String.Empty, null);
        }

        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.Add6"]/*' />
        public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled) {
            return Add(propertyName, dataSource, dataMember, formattingEnabled, this.DefaultDataSourceUpdateMode, null, String.Empty, null);
        }

        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.Add2"]/*' />
        public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode updateMode) {
            return Add(propertyName, dataSource, dataMember, formattingEnabled, updateMode, null, String.Empty, null);
        }

        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.Add3"]/*' />
        public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode updateMode, object nullValue) {
            return Add(propertyName, dataSource, dataMember, formattingEnabled, updateMode, nullValue, String.Empty, null);
        }

        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.Add5"]/*' />
        public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode updateMode, object nullValue, string formatString) {
            return Add(propertyName, dataSource, dataMember, formattingEnabled, updateMode, nullValue, formatString, null);
        }

        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.Add4"]/*' />
        public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode updateMode, object nullValue, string formatString, IFormatProvider formatInfo) {
            if (dataSource == null)
                throw new ArgumentNullException(nameof(dataSource));
            Binding binding = new Binding(propertyName, dataSource, dataMember, formattingEnabled, updateMode, nullValue, formatString, formatInfo);
            Add(binding);
            return binding;
        }

        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.AddCore"]/*' />
        /// <devdoc>
        /// Creates the binding and adds it to the collection.  An InvalidBindingException is thrown
        /// if this binding can't be constructed.  An exception is thrown if a binding to the same target and Property as an existing binding or
        /// if the binding's column isn't a valid column given this DataSource.Table's schema.
        /// Fires the CollectionChangedEvent.
        /// </devdoc>
        protected override void AddCore(Binding dataBinding) {
            if (dataBinding == null)
                throw new ArgumentNullException(nameof(dataBinding));
            if (dataBinding.BindableComponent == control)
                throw new ArgumentException(SR.BindingsCollectionAdd1);
            if (dataBinding.BindableComponent != null)
                throw new ArgumentException(SR.BindingsCollectionAdd2);

            // important to set prop first for error checking.
            dataBinding.SetBindableComponent(control);

            base.AddCore(dataBinding);
        }

        // internalonly
        internal void CheckDuplicates(Binding binding) {
            if (binding.PropertyName.Length == 0) {
                return;
            }
            for (int i = 0; i < Count; i++) {
                if (binding != this[i] && this[i].PropertyName.Length > 0 &&
                    (String.Compare(binding.PropertyName, this[i].PropertyName, false, CultureInfo.InvariantCulture) == 0)) {
                    throw new ArgumentException(SR.BindingsCollectionDup, "binding");
                }
            }
        }

        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.Clear"]/*' />
        /// <devdoc>
        /// Clears the collection of any bindings.
        /// Fires the CollectionChangedEvent.
        /// </devdoc>
        public new void Clear() {
            base.Clear();
        }

        // internalonly
        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.ClearCore"]/*' />
        protected override void ClearCore() {
            int numLinks = Count;
            for (int i = 0; i < numLinks; i++) {
                Binding dataBinding = this[i];
                dataBinding.SetBindableComponent(null);
            }
            base.ClearCore();
        }

        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.DefaultDataSourceUpdateMode"]/*' />
        /// <devdoc>
        /// </devdoc>
        public DataSourceUpdateMode DefaultDataSourceUpdateMode {
            get {
                return defaultDataSourceUpdateMode;
            }

            set {
                defaultDataSourceUpdateMode = value;
            }
        }

        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.Remove"]/*' />
        /// <devdoc>
        /// Removes the given binding from the collection.
        /// An ArgumentNullException is thrown if this binding is null.  An ArgumentException is thrown
        /// if this binding doesn't belong to this collection.
        /// The CollectionChanged event is fired if it succeeds.
        /// </devdoc>
        public new void Remove(Binding binding) {
            base.Remove(binding);
        }

        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.RemoveAt"]/*' />
        /// <devdoc>
        /// Removes the given binding from the collection.
        /// It throws an IndexOutOfRangeException if this doesn't have
        /// a valid binding.
        /// The CollectionChanged event is fired if it succeeds.
        /// </devdoc>
        public new void RemoveAt(int index) {
            base.RemoveAt(index);
        }

        /// <include file='doc\ControlBindingsCollection.uex' path='docs/doc[@for="ControlBindingsCollection.RemoveCore"]/*' />
        protected override void RemoveCore(Binding dataBinding) {
            if (dataBinding.BindableComponent != control)
                throw new ArgumentException(SR.BindingsCollectionForeign);
            dataBinding.SetBindableComponent(null);
            base.RemoveCore(dataBinding);
        }
    }
}

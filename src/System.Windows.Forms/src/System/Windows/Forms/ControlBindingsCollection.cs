// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents the collection of data bindings for a control.
    /// </summary>
    [DefaultEvent(nameof(CollectionChanged))]
    [Editor("System.Drawing.Design.UITypeEditor, " + AssemblyRef.SystemDrawing, typeof(UITypeEditor))]
    [TypeConverter("System.Windows.Forms.Design.ControlBindingsConverter, " + AssemblyRef.SystemDesign)]
    public class ControlBindingsCollection : BindingsCollection
    {
        private readonly IBindableComponent _control;

        public ControlBindingsCollection(IBindableComponent control)
        {
            _control = control;
        }

        public IBindableComponent BindableComponent => _control;

        public Control Control => _control as Control;

        public Binding this[string propertyName]
        {
            get
            {
                foreach (Binding binding in this)
                {
                    if (string.Equals(binding.PropertyName, propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        return binding;
                    }
                }

                return null;
            }
        }

        /// <summary>
        ///  Adds the binding to the collection. An ArgumentNullException is thrown if this
        ///  binding is null. An exception is thrown if a binding to the same target and
        ///  Property as an existing binding or if the binding's column isn't a valid column
        ///  given this DataSource.Table's schema.
        ///  Fires the CollectionChangedEvent.
        /// </summary>
        public new void Add(Binding binding) => base.Add(binding);

        /// <summary>
        ///  Creates the binding and adds it to the collection. An InvalidBindingException is
        ///  thrown if this binding can't be constructed. An exception is thrown if a binding
        ///  to the same target and Property as an existing binding or if the binding's column
        ///  isn't a valid column given this DataSource.Table's schema.
        ///  Fires the CollectionChangedEvent.
        /// </summary>
        public Binding Add(string propertyName, object dataSource, string dataMember)
        {
            return Add(propertyName, dataSource, dataMember, false, DefaultDataSourceUpdateMode, null, string.Empty, null);
        }

        public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled)
        {
            return Add(propertyName, dataSource, dataMember, formattingEnabled, DefaultDataSourceUpdateMode, null, string.Empty, null);
        }

        public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode updateMode)
        {
            return Add(propertyName, dataSource, dataMember, formattingEnabled, updateMode, null, string.Empty, null);
        }

        public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode updateMode, object nullValue)
        {
            return Add(propertyName, dataSource, dataMember, formattingEnabled, updateMode, nullValue, string.Empty, null);
        }

        public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode updateMode, object nullValue, string formatString)
        {
            return Add(propertyName, dataSource, dataMember, formattingEnabled, updateMode, nullValue, formatString, null);
        }

        public Binding Add(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode updateMode, object nullValue, string formatString, IFormatProvider formatInfo)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException(nameof(dataSource));
            }

            var binding = new Binding(propertyName, dataSource, dataMember, formattingEnabled, updateMode, nullValue, formatString, formatInfo);
            Add(binding);
            return binding;
        }

        /// <summary>
        ///  Creates the binding and adds it to the collection. An InvalidBindingException is
        ///  thrown if this binding can't be constructed. An exception is thrown if a binding to
        ///  the same target and Property as an existing binding or if the binding's column isn't
        ///  a valid column given this DataSource.Table's schema.
        ///  Fires the CollectionChangedEvent.
        /// </summary>
        protected override void AddCore(Binding dataBinding)
        {
            if (dataBinding == null)
            {
                throw new ArgumentNullException(nameof(dataBinding));
            }
            if (dataBinding.BindableComponent == _control)
            {
                throw new ArgumentException(SR.BindingsCollectionAdd1, nameof(dataBinding));
            }
            if (dataBinding.BindableComponent != null)
            {
                throw new ArgumentException(SR.BindingsCollectionAdd2, nameof(dataBinding));
            }

            // important to set prop first for error checking.
            dataBinding.SetBindableComponent(_control);

            base.AddCore(dataBinding);
        }

        internal void CheckDuplicates(Binding binding)
        {
            Debug.Assert(!string.IsNullOrEmpty(binding.PropertyName), "The caller should check for this.");

            for (int i = 0; i < Count; i++)
            {
                Binding current = this[i];
                if (binding != current && !string.IsNullOrEmpty(current.PropertyName) &&
                    string.Equals(binding.PropertyName, current.PropertyName, StringComparison.InvariantCulture))
                {
                    throw new ArgumentException(SR.BindingsCollectionDup, nameof(binding));
                }
            }
        }

        /// <summary>
        ///  Clears the collection of any bindings.
        ///  Fires the CollectionChangedEvent.
        /// </summary>
        public new void Clear() => base.Clear();

        protected override void ClearCore()
        {
            int numLinks = Count;
            for (int i = 0; i < numLinks; i++)
            {
                Binding dataBinding = this[i];
                dataBinding.SetBindableComponent(null);
            }

            base.ClearCore();
        }

        public DataSourceUpdateMode DefaultDataSourceUpdateMode { get; set; } = DataSourceUpdateMode.OnValidation;

        /// <summary>
        ///  Removes the given binding from the collection.
        ///  An ArgumentNullException is thrown if this binding is null. An ArgumentException is
        ///  thrown if this binding doesn't belong to this collection.
        ///  The CollectionChanged event is fired if it succeeds.
        /// </summary>
        public new void Remove(Binding binding) => base.Remove(binding);

        /// <summary>
        ///  Removes the given binding from the collection.
        ///  It throws an IndexOutOfRangeException if this doesn't have a valid binding.
        ///  The CollectionChanged event is fired if it succeeds.
        /// </summary>
        public new void RemoveAt(int index) => base.RemoveAt(index);

        protected override void RemoveCore(Binding dataBinding)
        {
            if (dataBinding == null)
            {
                throw new ArgumentNullException(nameof(dataBinding));
            }
            if (dataBinding.BindableComponent != _control)
            {
                throw new ArgumentException(SR.BindingsCollectionForeign, nameof(dataBinding));
            }

            dataBinding.SetBindableComponent(null);
            base.RemoveCore(dataBinding);
        }
    }
}

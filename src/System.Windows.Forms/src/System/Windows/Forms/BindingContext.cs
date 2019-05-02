﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Manages the collection of System.Windows.Forms.BindingManagerBase
    /// objects for a Win Form.
    /// </devdoc>
    [DefaultEvent(nameof(CollectionChanged))]
    public class BindingContext : ICollection
    {
        private Hashtable _listManagers;

        /// <devdoc>
        /// Initializes a new instance of the System.Windows.Forms.BindingContext class.
        /// </devdoc>
        public BindingContext()
        {
            _listManagers = new Hashtable();
        }

        /// <devdoc>
        /// Gets the total number of System.Windows.Forms.BindingManagerBases objects.
        /// </devdoc>
        int ICollection.Count
        {
            get
            {
                ScrubWeakRefs();
                return _listManagers.Count;
            }
        }

        /// <devdoc>
        /// Copies the elements of the collection into a specified array, starting
        /// at the collection index.
        /// </devdoc>
        void ICollection.CopyTo(Array ar, int index)
        {
            ScrubWeakRefs();
            _listManagers.CopyTo(ar, index);
        }

        /// <devdoc>
        /// Gets an enumerator for the collection.
        /// </devdoc>
        IEnumerator IEnumerable.GetEnumerator()
        {
            ScrubWeakRefs();
            return _listManagers.GetEnumerator();
        }

        /// <devdoc>
        /// Gets a value indicating whether the collection is read-only.
        /// </devdoc>
        public bool IsReadOnly => false;

        /// <devdoc>
        /// Gets a value indicating whether the collection is synchronized.
        /// </devdoc>
        bool ICollection.IsSynchronized => false;

        /// <devdoc>
        /// Gets an object to use for synchronization (thread safety).
        /// </devdoc>
        object ICollection.SyncRoot => null;

        /// <devdoc>
        /// Gets the System.Windows.Forms.BindingManagerBase associated with the specified
        /// data source.
        /// </devdoc>
        public BindingManagerBase this[object dataSource] => this[dataSource, string.Empty];

        /// <devdoc>
        /// Gets the System.Windows.Forms.BindingManagerBase associated with the specified
        /// data source and data member.
        /// </devdoc>
        public BindingManagerBase this[object dataSource, string dataMember]
        {
            get => EnsureListManager(dataSource, dataMember);
        }

        /// <devdoc>
        /// Adds the listManager to the collection. An ArgumentNullException is thrown if this
        /// listManager is null. An exception is thrown if a listManager to the same target
        /// and Property as an existing listManager or if the listManager's column isn't a
        /// valid column given this DataSource.Table's schema.
        /// Fires the CollectionChangedEvent.
        /// </devdoc>
        /// <remarks>
        /// This method is obsolete and unused.
        /// </remarks>
        internal protected void Add(object dataSource, BindingManagerBase listManager)
        {
            AddCore(dataSource, listManager);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataSource));
        }

        /// <remarks>
        /// This method is obsolete and unused.
        /// </remarks>
        protected virtual void AddCore(object dataSource, BindingManagerBase listManager)
        {
            if (dataSource == null)
            {
                throw new ArgumentNullException(nameof(dataSource));
            }
            if (listManager == null)
            {
                throw new ArgumentNullException(nameof(listManager));
            }

            _listManagers[GetKey(dataSource, string.Empty)] = new WeakReference(listManager, false);
        }

        /// <devdoc>
        /// Occurs when the collection has changed.
        /// </devdoc>
        /// <remarks>
        /// This method is obsolete and unused.
        /// </remarks>
        [SRDescription(nameof(SR.collectionChangedEventDescr)), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public event CollectionChangeEventHandler CollectionChanged
        {
            [SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")]
            add
            {
                throw new NotImplementedException();
            }
            [SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters")]
            remove
            {
            }
        }

        /// <devdoc>
        /// Clears the collection of any bindings.
        /// Fires the CollectionChangedEvent.
        /// </devdoc>
        /// <remarks>
        /// This method is obsolete and unused.
        /// </remarks>
        internal protected void Clear()
        {
            ClearCore();
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        /// <devdoc>
        /// Clears the collection.
        /// </devdoc>
        /// <remarks>
        /// This method is obsolete and unused.
        /// </remarks>
        protected virtual void ClearCore() => _listManagers.Clear();

        /// <devdoc>
        /// Gets a value indicating whether the System.Windows.Forms.BindingContext contains
        /// the specified data source.
        /// </devdoc>
        public bool Contains(object dataSource) => Contains(dataSource, string.Empty);

        /// <devdoc>
        /// Gets a value indicating whether the System.Windows.Forms.BindingContext
        /// contains the specified data source and data member.
        /// </devdoc>
        public bool Contains(object dataSource, string dataMember)
        {
            return _listManagers.ContainsKey(GetKey(dataSource, dataMember));
        }

        internal HashKey GetKey(object dataSource, string dataMember)
        {
            return new HashKey(dataSource, dataMember);
        }

        internal class HashKey
        {
            private WeakReference _wRef;
            private int _dataSourceHashCode;
            private string _dataMember;

            internal HashKey(object dataSource, string dataMember)
            {
                if (dataSource == null)
                {
                    throw new ArgumentNullException(nameof(dataSource));
                }
                if (dataMember == null)
                {
                    dataMember = string.Empty;
                }

                // The dataMember should be case insensitive, so convert the
                // dataMember to lower case
                _wRef = new WeakReference(dataSource, false);
                _dataSourceHashCode = dataSource.GetHashCode();
                _dataMember = dataMember.ToLower(CultureInfo.InvariantCulture);
            }

            public override int GetHashCode() => _dataSourceHashCode * _dataMember.GetHashCode();

            public override bool Equals(object target)
            {
                if (!(target is HashKey keyTarget))
                {
                    return false;
                }
                
                return _wRef.Target == keyTarget._wRef.Target && _dataMember == keyTarget._dataMember;
            }
        }

        /// <devdoc>
        /// This method is called whenever the collection changes. Overriders of this method
        /// should call the base implementation of this method.
        /// </devdoc>
        protected virtual void OnCollectionChanged(CollectionChangeEventArgs ccevent)
        {
        }

        /// <devdoc>
        /// Removes the given listManager from the collection.
        /// An ArgumentNullException is thrown if this listManager is null. An ArgumentException
        /// is thrown if this listManager doesn't belong to this collection.
        /// The CollectionChanged event is fired if it succeeds.
        /// </devdoc>
        /// <remarks>
        /// This method is obsolete and unused.
        /// </remarks>
        internal protected void Remove(object dataSource)
        {
            RemoveCore(dataSource);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, dataSource));
        }

        /// <remarks>
        /// This method is obsolete and unused.
        /// </remarks>
        protected virtual void RemoveCore(object dataSource)
        {
            _listManagers.Remove(GetKey(dataSource, string.Empty));
        }

        /// <devdoc>
        /// Create a suitable binding manager for the specified dataSource/dataMember combination.
        /// - If one has already been created and cached by this BindingContext, return that
        ///   instead.
        /// - If the data source is an ICurrencyManagerProvider, just delegate to the data
        ///   source.
        /// </devdoc>
        internal BindingManagerBase EnsureListManager(object dataSource, string dataMember)
        {
            BindingManagerBase bindingManagerBase = null;

            if (dataMember == null)
            {
                dataMember = string.Empty;
            }

            // Check whether data source wants to provide its own binding managers
            // (but fall through to old logic if it fails to provide us with one)
            if (dataSource is ICurrencyManagerProvider currencyManagerProvider)
            {
                bindingManagerBase = currencyManagerProvider.GetRelatedCurrencyManager(dataMember);
                if (bindingManagerBase != null)
                {
                    return bindingManagerBase;
                }
            }

            // Check for previously created binding manager
            HashKey key = GetKey(dataSource, dataMember);
            WeakReference wRef = _listManagers[key] as WeakReference;
            if (wRef != null)
            {
                bindingManagerBase = (BindingManagerBase)wRef.Target;
            }
            if (bindingManagerBase != null)
            {
                return bindingManagerBase;
            }

            if (dataMember.Length == 0)
            {
                // No data member specified, so create binding manager directly on the data source
                if (dataSource is IList || dataSource is IListSource)
                {
                    // IListSource so we can bind the dataGrid to a table and a dataSet
                    bindingManagerBase = new CurrencyManager(dataSource);
                }
                else
                {
                    // Otherwise assume simple property binding
                    bindingManagerBase = new PropertyManager(dataSource);
                }
            }
            else
            {
                // Data member specified, so get data source's binding manager, and hook a 'related' binding manager to it
                int lastDot = dataMember.LastIndexOf('.');
                string dataPath = (lastDot == -1) ? string.Empty : dataMember.Substring(0, lastDot);
                string dataField = dataMember.Substring(lastDot + 1);

                BindingManagerBase formerManager = EnsureListManager(dataSource, dataPath);

                PropertyDescriptor prop = formerManager.GetItemProperties().Find(dataField, true);
                if (prop == null)
                {
                    throw new ArgumentException(string.Format(SR.RelatedListManagerChild, dataField));
                }

                if (typeof(IList).IsAssignableFrom(prop.PropertyType))
                {
                    bindingManagerBase = new RelatedCurrencyManager(formerManager, dataField);
                }
                else
                {
                    bindingManagerBase = new RelatedPropertyManager(formerManager, dataField);
                }
            }

            // if wRef == null, then it is the first time we want this bindingManagerBase: so add it
            // if wRef != null, then the bindingManagerBase was GC'ed at some point: keep the old wRef and change its target
            if (wRef == null)
            {
                _listManagers.Add(key, new WeakReference(bindingManagerBase, false));
            }
            else
            {
                wRef.Target = bindingManagerBase;
            }

            ScrubWeakRefs();
            // Return the final binding manager
            return bindingManagerBase;
        }

        private static void CheckPropertyBindingCycles(BindingContext newBindingContext, Binding propBinding)
        {
            if (newBindingContext == null || propBinding == null)
            {
                return;
            }

            if (newBindingContext.Contains(propBinding.BindableComponent, string.Empty))
            {
                // this way we do not add a bindingManagerBase to the
                // bindingContext if there isn't one already
                BindingManagerBase bindingManagerBase = newBindingContext.EnsureListManager(propBinding.BindableComponent, string.Empty);
                for (int i = 0; i < bindingManagerBase.Bindings.Count; i++)
                {
                    Binding binding = bindingManagerBase.Bindings[i];
                    if (binding.DataSource == propBinding.BindableComponent)
                    {
                        if (propBinding.BindToObject.BindingMemberInfo.BindingMember.Equals(binding.PropertyName))
                        {
                            throw new ArgumentException(string.Format(SR.DataBindingCycle, binding.PropertyName), nameof(propBinding));
                        }
                    }
                    else if (propBinding.BindToObject.BindingManagerBase is PropertyManager)
                    {
                        CheckPropertyBindingCycles(newBindingContext, binding);
                    }
                }
            }
        }

        private void ScrubWeakRefs()
        {
            ArrayList cleanupList = null;
            foreach (DictionaryEntry de in _listManagers)
            {
                WeakReference wRef = (WeakReference)de.Value;
                if (wRef.Target == null)
                {
                    if (cleanupList == null)
                    {
                        cleanupList = new ArrayList();
                    }
                    cleanupList.Add(de.Key);
                }
            }

            if (cleanupList != null)
            {
                foreach (object o in cleanupList)
                {
                    _listManagers.Remove(o);
                }
            }
        }

        /// <devdoc>
        /// Associates a Binding with a different BindingContext. Intended for use by components
        /// that support IBindableComponent, to update their Bindings when the value of
        /// IBindableComponent.BindingContext is changed.
        /// </devdoc>
        public static void UpdateBinding(BindingContext newBindingContext, Binding binding)
        {
            if (binding == null)
            {
                throw new ArgumentNullException(nameof(binding));
            }

            BindingManagerBase oldManager = binding.BindingManagerBase;
            if (oldManager != null)
            {
                oldManager.Bindings.Remove(binding);
            }

            if (newBindingContext != null)
            {
                // we need to first check for cycles before adding this binding to the collection
                // of bindings.
                if (binding.BindToObject.BindingManagerBase is PropertyManager)
                {
                    CheckPropertyBindingCycles(newBindingContext, binding);
                }

                BindToObject bindTo = binding.BindToObject;
                BindingManagerBase newManager = newBindingContext.EnsureListManager(bindTo.DataSource, bindTo.BindingMemberInfo.BindingPath);
                newManager.Bindings.Add(binding);
            }
        }
    }
}

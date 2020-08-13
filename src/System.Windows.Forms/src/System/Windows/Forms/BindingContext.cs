// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Manages the collection of System.Windows.Forms.BindingManagerBase
    ///  objects for a Win Form.
    /// </summary>
    [DefaultEvent(nameof(CollectionChanged))]
    public class BindingContext : ICollection
    {
        private readonly Hashtable _listManagers;

        /// <summary>
        ///  Initializes a new instance of the System.Windows.Forms.BindingContext class.
        /// </summary>
        public BindingContext()
        {
            _listManagers = new Hashtable();
        }

        /// <summary>
        ///  Gets the total number of System.Windows.Forms.BindingManagerBases objects.
        /// </summary>
        int ICollection.Count
        {
            get
            {
                ScrubWeakRefs();
                return _listManagers.Count;
            }
        }

        /// <summary>
        ///  Copies the elements of the collection into a specified array, starting
        ///  at the collection index.
        /// </summary>
        void ICollection.CopyTo(Array ar, int index)
        {
            ScrubWeakRefs();
            _listManagers.CopyTo(ar, index);
        }

        /// <summary>
        ///  Gets an enumerator for the collection.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            ScrubWeakRefs();
            return _listManagers.GetEnumerator();
        }

        /// <summary>
        ///  Gets a value indicating whether the collection is read-only.
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        ///  Gets a value indicating whether the collection is synchronized.
        /// </summary>
        bool ICollection.IsSynchronized => false;

        /// <summary>
        ///  Gets an object to use for synchronization (thread safety).
        /// </summary>
        object ICollection.SyncRoot => this;

        /// <summary>
        ///  Gets the System.Windows.Forms.BindingManagerBase associated with the specified
        ///  data source.
        /// </summary>
        public BindingManagerBase this[object dataSource] => this[dataSource, string.Empty];

        /// <summary>
        ///  Gets the System.Windows.Forms.BindingManagerBase associated with the specified
        ///  data source and data member.
        /// </summary>
        public BindingManagerBase this[object dataSource, string dataMember]
        {
            get => EnsureListManager(dataSource, dataMember);
        }

        /// <summary>
        ///  Adds the listManager to the collection. An ArgumentNullException is thrown if this
        ///  listManager is null. An exception is thrown if a listManager to the same target
        ///  and Property as an existing listManager or if the listManager's column isn't a
        ///  valid column given this DataSource.Table's schema.
        ///  Fires the CollectionChangedEvent.
        /// </summary>
        /// <remarks>
        ///  This method is obsolete and unused.
        /// </remarks>
        protected internal void Add(object dataSource, BindingManagerBase listManager)
        {
            AddCore(dataSource, listManager);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Add, dataSource));
        }

        /// <remarks>
        ///  This method is obsolete and unused.
        /// </remarks>
        protected virtual void AddCore(object dataSource, BindingManagerBase listManager)
        {
            if (dataSource is null)
            {
                throw new ArgumentNullException(nameof(dataSource));
            }
            if (listManager is null)
            {
                throw new ArgumentNullException(nameof(listManager));
            }

            _listManagers[GetKey(dataSource, string.Empty)] = new WeakReference(listManager, false);
        }

        /// <summary>
        ///  Occurs when the collection has changed.
        /// </summary>
        /// <remarks>
        ///  This method is obsolete and unused.
        /// </remarks>
        [SRDescription(nameof(SR.collectionChangedEventDescr))]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public event CollectionChangeEventHandler CollectionChanged
        {
            add
            {
                throw new NotImplementedException();
            }
            remove
            {
            }
        }

        /// <summary>
        ///  Clears the collection of any bindings.
        ///  Fires the CollectionChangedEvent.
        /// </summary>
        /// <remarks>
        ///  This method is obsolete and unused.
        /// </remarks>
        protected internal void Clear()
        {
            ClearCore();
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Refresh, null));
        }

        /// <summary>
        ///  Clears the collection.
        /// </summary>
        /// <remarks>
        ///  This method is obsolete and unused.
        /// </remarks>
        protected virtual void ClearCore() => _listManagers.Clear();

        /// <summary>
        ///  Gets a value indicating whether the System.Windows.Forms.BindingContext contains
        ///  the specified data source.
        /// </summary>
        public bool Contains(object dataSource) => Contains(dataSource, string.Empty);

        /// <summary>
        ///  Gets a value indicating whether the System.Windows.Forms.BindingContext
        ///  contains the specified data source and data member.
        /// </summary>
        public bool Contains(object dataSource, string dataMember)
        {
            return _listManagers.ContainsKey(GetKey(dataSource, dataMember));
        }

        private HashKey GetKey(object dataSource, string dataMember)
        {
            return new HashKey(dataSource, dataMember);
        }

        private class HashKey
        {
            private readonly WeakReference _wRef;
            private readonly int _dataSourceHashCode;
            private readonly string _dataMember;

            internal HashKey(object dataSource, string dataMember)
            {
                if (dataSource is null)
                {
                    throw new ArgumentNullException(nameof(dataSource));
                }
                if (dataMember is null)
                {
                    dataMember = string.Empty;
                }

                // The dataMember should be case insensitive, so convert the
                // dataMember to lower case
                _wRef = new WeakReference(dataSource, false);
                _dataSourceHashCode = dataSource.GetHashCode();
                _dataMember = dataMember.ToLower(CultureInfo.InvariantCulture);
            }

            public override int GetHashCode() => HashCode.Combine(_dataSourceHashCode, _dataMember);

            public override bool Equals(object target)
            {
                if (!(target is HashKey keyTarget))
                {
                    return false;
                }

                return _wRef.Target == keyTarget._wRef.Target && _dataMember == keyTarget._dataMember;
            }
        }

        /// <summary>
        ///  This method is called whenever the collection changes. Overriders of this method
        ///  should call the base implementation of this method.
        /// </summary>
        protected virtual void OnCollectionChanged(CollectionChangeEventArgs ccevent)
        {
        }

        /// <summary>
        ///  Removes the given listManager from the collection.
        ///  An ArgumentNullException is thrown if this listManager is null. An ArgumentException
        ///  is thrown if this listManager doesn't belong to this collection.
        ///  The CollectionChanged event is fired if it succeeds.
        /// </summary>
        /// <remarks>
        ///  This method is obsolete and unused.
        /// </remarks>
        protected internal void Remove(object dataSource)
        {
            RemoveCore(dataSource);
            OnCollectionChanged(new CollectionChangeEventArgs(CollectionChangeAction.Remove, dataSource));
        }

        /// <remarks>
        ///  This method is obsolete and unused.
        /// </remarks>
        protected virtual void RemoveCore(object dataSource)
        {
            _listManagers.Remove(GetKey(dataSource, string.Empty));
        }

        /// <summary>
        ///  Create a suitable binding manager for the specified dataSource/dataMember combination.
        ///  - If one has already been created and cached by this BindingContext, return that
        ///  instead.
        ///  - If the data source is an ICurrencyManagerProvider, just delegate to the data
        ///  source.
        /// </summary>
        private BindingManagerBase EnsureListManager(object dataSource, string dataMember)
        {
            BindingManagerBase bindingManagerBase = null;

            if (dataMember is null)
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
                if (prop is null)
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

            // if wRef is null, then it is the first time we want this bindingManagerBase: so add it
            // if wRef != null, then the bindingManagerBase was GC'ed at some point: keep the old wRef and change its target
            if (wRef is null)
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
            Debug.Assert(newBindingContext != null, "Always called with a non-null BindingContext");
            Debug.Assert(propBinding != null, "Always called with a non-null Binding.");

            if (propBinding.BindableComponent != null && newBindingContext.Contains(propBinding.BindableComponent, string.Empty))
            {
                // this way we do not add a bindingManagerBase to the
                // bindingContext if there isn't one already
                BindingManagerBase bindingManagerBase = newBindingContext.EnsureListManager(propBinding.BindableComponent, string.Empty);
                for (int i = 0; i < bindingManagerBase.Bindings.Count; i++)
                {
                    Binding binding = bindingManagerBase.Bindings[i];
                    if (binding.DataSource == propBinding.BindableComponent)
                    {
                        if (propBinding.BindingMemberInfo.BindingMember.Equals(binding.PropertyName))
                        {
                            throw new ArgumentException(string.Format(SR.DataBindingCycle, binding.PropertyName), nameof(propBinding));
                        }
                    }
                    else if (propBinding.BindingManagerBase is PropertyManager)
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
                if (wRef.Target is null)
                {
                    if (cleanupList is null)
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

        /// <summary>
        ///  Associates a Binding with a different BindingContext. Intended for use by components
        ///  that support IBindableComponent, to update their Bindings when the value of
        ///  IBindableComponent.BindingContext is changed.
        /// </summary>
        public static void UpdateBinding(BindingContext newBindingContext, Binding binding)
        {
            if (binding is null)
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
                if (binding.BindingManagerBase is PropertyManager)
                {
                    CheckPropertyBindingCycles(newBindingContext, binding);
                }

                BindingManagerBase newManager = newBindingContext.EnsureListManager(binding.DataSource, binding.BindingMemberInfo.BindingPath);
                newManager.Bindings.Add(binding);
            }
        }
    }
}

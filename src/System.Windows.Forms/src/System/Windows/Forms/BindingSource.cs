// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope="member", Target="System.Windows.Forms.BindingSource.System.ComponentModel.ISupportInitialize.EndInit():System.Void")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope="member", Target="System.Windows.Forms.BindingSource.System.ComponentModel.ICancelAddNew.CancelNew(System.Int32):System.Void")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope="member", Target="System.Windows.Forms.BindingSource.System.ComponentModel.ICancelAddNew.EndNew(System.Int32):System.Void")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope="member", Target="System.Windows.Forms.BindingSource.System.ComponentModel.ISupportInitialize.BeginInit():System.Void")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope="member", Target="System.Windows.Forms.BindingSource.System.ComponentModel.ISupportInitializeNotification.get_IsInitialized():System.Boolean")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope="member", Target="System.Windows.Forms.BindingSource.System.ComponentModel.IBindingList.AddIndex(System.ComponentModel.PropertyDescriptor):System.Void")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope="member", Target="System.Windows.Forms.BindingSource.System.ComponentModel.ISupportInitializeNotification.add_Initialized(System.EventHandler):System.Void")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope="member", Target="System.Windows.Forms.BindingSource.System.ComponentModel.ISupportInitializeNotification.remove_Initialized(System.EventHandler):System.Void")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope="member", Target="System.Windows.Forms.BindingSource.System.ComponentModel.IBindingList.RemoveIndex(System.ComponentModel.PropertyDescriptor):System.Void")]

namespace System.Windows.Forms {
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Reflection;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;

    [
    DefaultProperty(nameof(DataSource)),
    DefaultEvent(nameof(CurrentChanged)),
    ComplexBindingProperties(nameof(DataSource), nameof(DataMember)),
    Designer("System.Windows.Forms.Design.BindingSourceDesigner, " + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionBindingSource)),
    System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1035:ICollectionImplementationsHaveStronglyTypedMembers"), // ICollection.CopyTo: Its just a wrapper class, it doesn't have a specific member type
    System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), // ICollection: We don't want class name to have to end in 'Collection'
    System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1039:ListsAreStronglyTyped"), // IList.Add: Its just a wrapper class, it doesn't have a specific member type
    ]
    /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource"]/* />
    public class BindingSource : Component,
                                 IBindingListView,
                                 ITypedList,
                                 ICancelAddNew,
                                 ISupportInitializeNotification,
                                 ICurrencyManagerProvider {

        // Public events
        private static readonly object EVENT_ADDINGNEW          = new object();
        private static readonly object EVENT_BINDINGCOMPLETE    = new object();
        private static readonly object EVENT_CURRENTCHANGED     = new object();
        private static readonly object EVENT_CURRENTITEMCHANGED = new object();
        private static readonly object EVENT_DATAERROR          = new object();
        private static readonly object EVENT_DATAMEMBERCHANGED  = new object();
        private static readonly object EVENT_DATASOURCECHANGED  = new object();
        private static readonly object EVENT_LISTCHANGED        = new object();
        private static readonly object EVENT_POSITIONCHANGED    = new object();
        private static readonly object EVENT_INITIALIZED        = new object();

        // Public property values
        private object dataSource = null;
        private string dataMember = String.Empty;
        private string sort = null;
        private string filter = null;
        private CurrencyManager currencyManager;
        private bool raiseListChangedEvents = true;
        private bool parentsCurrentItemChanging = false;
        private bool disposedOrFinalized = false;

        // Description of the current bound list
        private IList _innerList; // ...DON'T access this directly. ALWAYS use the List property.
        private bool isBindingList;
        private bool listRaisesItemChangedEvents;
        private bool listExtractedFromEnumerable;

        // Description of items in the current bound list
        private Type itemType;
        private ConstructorInfo itemConstructor;
        private PropertyDescriptorCollection itemShape;

        // Cached list of 'related' binding sources returned to callers of ICurrencyManagerProvider.GetRelatedCurrencyManager()
        Dictionary<string, BindingSource> relatedBindingSources;

        // Support for user-overriding of the AllowNew property
        private bool allowNewIsSet       = false;
        private bool allowNewSetValue    = true;

        // Support for property change event hooking on list items
        private object currentItemHookedForItemChange = null;
        private object lastCurrentItem = null;
        private EventHandler listItemPropertyChangedHandler;

        // State data
        private int  addNewPos = -1;
        private bool initializing = false;
        private bool needToSetList = false;
        private bool recursionDetectionFlag = false;
        private bool innerListChanging = false;
        private bool endingEdit = false;


        ///////////////////////////////////////////////////////////////////////////////
        //
        // Constructors
        //
        ///////////////////////////////////////////////////////////////////////////////

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.BindingSource"]/* />
        public BindingSource() : this(null, String.Empty) {
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.BindingSource1"]/* />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")] // Indirect call to OnListChanged virtual. Unavoidable but quite acceptible.
        public BindingSource(object dataSource, string dataMember) : base() {
            // Set data source and data member
            this.dataSource = dataSource;
            this.dataMember = dataMember;

            // Set inner list to something, so that the currency manager doesn't have an issue.
            this._innerList = new ArrayList();

            // Set up the currency manager
            this.currencyManager = new CurrencyManager(this);
            WireCurrencyManager(this.currencyManager);

            // Create event handlers
            this.listItemPropertyChangedHandler = new EventHandler(ListItem_PropertyChanged);

            // Now set up the inner list properly (which requires the currency manager to be set up beforehand)
            ResetList();

            // Wire up the data source
            WireDataSource();
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.BindingSource1"]/* />
        public BindingSource(System.ComponentModel.IContainer container) : this() {
            if (container == null) {
                throw new ArgumentNullException(nameof(container));
            }

            container.Add(this);
        }

        ///////////////////////////////////////////////////////////////////////////////
        //
        // Properties
        //
        ///////////////////////////////////////////////////////////////////////////////

        private bool AllowNewInternal(bool checkconstructor) {
            if (disposedOrFinalized) {
                return false;
            }
            if (this.allowNewIsSet) {
                return this.allowNewSetValue;
            }
            else if (this.listExtractedFromEnumerable) {
                return false;
            }
            else if (isBindingList) {
                return ((IBindingList)List).AllowNew;
            }
            else {
                return IsListWriteable(checkconstructor);
            }
        }

        private bool IsListWriteable(bool checkconstructor) {
            return !List.IsReadOnly && !List.IsFixedSize && (!checkconstructor || this.itemConstructor != null);
        }

        [Browsable(false)]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.CurrencyManager"]/* />
        public virtual CurrencyManager CurrencyManager {
            get {
                return (this as ICurrencyManagerProvider).GetRelatedCurrencyManager(null);
            }
        }
        
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.GetRelatedCurrencyManager"]/* />
        public virtual CurrencyManager GetRelatedCurrencyManager(string dataMember) {
            // Make sure inner list has been set up! We do this here so that
            // the list is set up as early as possible after initialization.
            EnsureInnerList();

            // If no data member specified, just return the main currency manager
            if (String.IsNullOrEmpty(dataMember)) {
                return this.currencyManager;
            }

            // Today, this particular implementation of ICurrencyManagerProvider doesn't support the use of 'dot notation'
            // to specify chains of related data members. We don't have any scenarios for this which involve binding sources.
            // Return 'null' to force the binding context to fall back on its default related currency manager behavior.
            if (dataMember.IndexOf(".") != -1) {
                return null;
            }

            // Get related binding source for specified data member (created on first call, then cached on subsequent calls)
            BindingSource bs = GetRelatedBindingSource(dataMember);

            // Return related binding source's currency manager
            return (bs as ICurrencyManagerProvider).CurrencyManager;
        }

        private BindingSource GetRelatedBindingSource(string dataMember) {
            // Auto-create the binding source cache on first use
            if (this.relatedBindingSources == null) {
                this.relatedBindingSources = new Dictionary<string, BindingSource>();
            }

            // Look for an existing binding source that uses this data member, and return that
            foreach (string key in this.relatedBindingSources.Keys) {
                if (String.Equals(key, dataMember, StringComparison.OrdinalIgnoreCase)) {
                    return this.relatedBindingSources[key];
                }
            }

            // Otherwise create the related binding source, cache it, and return it
            BindingSource bs = new BindingSource(this, dataMember);
            this.relatedBindingSources[dataMember] = bs;
            return bs;
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.Current"]/*' />
        [Browsable(false)]
        public object Current {
            get {
                return currencyManager.Count > 0 ? currencyManager.Current : null;
            }
        }

        [
        SRCategory(nameof(SR.CatData)),
        DefaultValue(""),
        RefreshProperties(RefreshProperties.Repaint),
        Editor("System.Windows.Forms.Design.DataMemberListEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor)),
        SRDescription(nameof(SR.BindingSourceDataMemberDescr))
        ]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.DataMember"]/* />
        public string DataMember {
            get {
                return dataMember;
            }

            set {
                if (value == null) {
                    value = String.Empty;
                }

                if (!dataMember.Equals(value)) {
                    dataMember = value;
                    ResetList();
                    OnDataMemberChanged(EventArgs.Empty);
                }
            }
        }

        [
        SRCategory(nameof(SR.CatData)),
        DefaultValue(null),
        RefreshProperties(RefreshProperties.Repaint),
        AttributeProvider(typeof(IListSource)),
        SRDescription(nameof(SR.BindingSourceDataSourceDescr))
        ]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.DataSource"]/* />
        public object DataSource {
            get {
                return dataSource;
            }

            set {
                if (dataSource != value) {
                    // Throw InvalidOperationException if the new data source introduces a cycle.
                    ThrowIfBindingSourceRecursionDetected(value);

                    // Unhook events on old data source
                    UnwireDataSource();

                    // Accept the new data source
                    dataSource = value;

                    // Blow away the data member property, if its no longer valid under the new data source
                    ClearInvalidDataMember();

                    /* CUT:
                    // Automatically assign a value to the data member property, when its appropriate to do so
                    AutoSetDataMember();
                    */

                    // Get the inner list for our new DataSource/DataMember combination
                    ResetList();

                    // Hook events on new data source
                    WireDataSource();

                    // Emit our DataSourceChanged event
                    OnDataSourceChanged(EventArgs.Empty);
                }
            }
        }

        private string InnerListFilter {
            get {
                IBindingListView iblv = List as IBindingListView;

                if (iblv != null && iblv.SupportsFiltering) {
                    return iblv.Filter;
                }
                else {
                    return String.Empty;
                }
            }

            set {
                if (this.initializing || DesignMode) {
                    return;
                }

                if (String.Equals(value, this.InnerListFilter, StringComparison.Ordinal)) {
                    return;
                }

                IBindingListView iblv = List as IBindingListView;

                if (iblv != null && iblv.SupportsFiltering) {
                    iblv.Filter = value;
                }
            }
        }

        private string InnerListSort {
            get {
                ListSortDescriptionCollection sortsColln = null;

                IBindingListView iblv = List as IBindingListView;
                IBindingList ibl = List as IBindingList;

                if (iblv != null && iblv.SupportsAdvancedSorting) {
                    sortsColln = iblv.SortDescriptions;
                }
                else if (ibl != null && ibl.SupportsSorting && ibl.IsSorted) {
                    ListSortDescription[] sortsArray = new ListSortDescription[1];
                    sortsArray[0] = new ListSortDescription(ibl.SortProperty, ibl.SortDirection);
                    sortsColln = new ListSortDescriptionCollection(sortsArray);
                }

                return BuildSortString(sortsColln);
            }

            set {
                if (this.initializing || DesignMode) {
                    return;
                }

                if (String.Compare(value, this.InnerListSort, false, CultureInfo.InvariantCulture) == 0) {
                    return;
                }

                ListSortDescriptionCollection sortsColln = ParseSortString(value);

                IBindingListView iblv = List as IBindingListView;
                IBindingList ibl = List as IBindingList;

                if (iblv != null && iblv.SupportsAdvancedSorting) {
                    if (sortsColln.Count == 0) {
                        iblv.RemoveSort();
                        return;
                    }
                    else {
                        iblv.ApplySort(sortsColln);
                        return;
                    }
                }

                if (ibl != null && ibl.SupportsSorting) {
                    if (sortsColln.Count == 0) {
                        ibl.RemoveSort();
                        return;
                    }
                    else if (sortsColln.Count == 1) {
                        ibl.ApplySort(sortsColln[0].PropertyDescriptor, sortsColln[0].SortDirection);
                        return;
                    }
                }
            }
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.IsBindingSuspended"]/*' />
        [Browsable(false)]
        public bool IsBindingSuspended {
            get {
                return currencyManager.IsBindingSuspended;
            }
        }

        [Browsable(false)]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.List"]/* />
        public IList List {
            get {
                // Make sure inner list has been set up!
                EnsureInnerList();

                return this._innerList;
            }
        }

        [
        DefaultValue(-1),
        Browsable(false)
        ]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.Position"]/* />
        public int Position {
            get {
                return currencyManager.Position;
            }

            set {
                if (currencyManager.Position != value)
                {
                    // Setting the position on the currency manager causes EndCurrentEdit
                    // even if the position inside the currency manager does not change.
                    // This is the RTM/Everett behavior and we have to live w/ it.
                    currencyManager.Position = value;
                }
            }
        }

        [
        DefaultValue(true),
        Browsable(false)
        ]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.RaiseListChangedEvents"]/* />
        public bool RaiseListChangedEvents {
            get {
                return this.raiseListChangedEvents;
            }

            set {
                if (this.raiseListChangedEvents != value) {
                    this.raiseListChangedEvents = value;
                }
            }
        }

        [
        SRCategory(nameof(SR.CatData)),
        DefaultValue(null),
        SRDescription(nameof(SR.BindingSourceSortDescr))
        ]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.Sort"]/*' />
        public string Sort {
            get {
                return this.sort;
            }
            set {
                this.sort = value;
                InnerListSort = value;
            }
        }

        #region Events
        ///////////////////////////////////////////////////////////////////////////////
        //
        // Events
        //
        ///////////////////////////////////////////////////////////////////////////////

        [
        SRCategory(nameof(SR.CatData)),
        SRDescription(nameof(SR.BindingSourceAddingNewEventHandlerDescr))
        ]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.AddingNew"]/*' />
        public event AddingNewEventHandler AddingNew {
            add {
                Events.AddHandler(EVENT_ADDINGNEW, value);
            }
            remove {
                Events.RemoveHandler(EVENT_ADDINGNEW, value);
            }
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.BindingComplete"]/*' />
        [
        SRCategory(nameof(SR.CatData)),
        SRDescription(nameof(SR.BindingSourceBindingCompleteEventHandlerDescr))
        ]
        public event BindingCompleteEventHandler BindingComplete {
            add {
                Events.AddHandler(EVENT_BINDINGCOMPLETE, value);
            }
            remove {
                Events.RemoveHandler(EVENT_BINDINGCOMPLETE, value);
            }
        }

        [
        SRCategory(nameof(SR.CatData)),
        SRDescription(nameof(SR.BindingSourceDataErrorEventHandlerDescr))
        ]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.DataError"]/*' />
        public event BindingManagerDataErrorEventHandler DataError {
            add {
                Events.AddHandler(EVENT_DATAERROR, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DATAERROR, value);
            }
        }

        [
        SRCategory(nameof(SR.CatData)),
        SRDescription(nameof(SR.BindingSourceDataSourceChangedEventHandlerDescr))
        ]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.DataSourceChanged"]/*' />
        public event EventHandler DataSourceChanged {
            add {
                Events.AddHandler(EVENT_DATASOURCECHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DATASOURCECHANGED, value);
            }
        }

        [
        SRCategory(nameof(SR.CatData)),
        SRDescription(nameof(SR.BindingSourceDataMemberChangedEventHandlerDescr))
        ]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.DataMemberChanged"]/*' />
        public event EventHandler DataMemberChanged {
            add {
                Events.AddHandler(EVENT_DATAMEMBERCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DATAMEMBERCHANGED, value);
            }
        }

        [
        SRCategory(nameof(SR.CatData)),
        SRDescription(nameof(SR.BindingSourceCurrentChangedEventHandlerDescr))
        ]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.CurrentChanged"]/*' />
        public event EventHandler CurrentChanged {
            add {
                Events.AddHandler(EVENT_CURRENTCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_CURRENTCHANGED, value);
            }
        }

        [
        SRCategory(nameof(SR.CatData)),
        SRDescription(nameof(SR.BindingSourceCurrentItemChangedEventHandlerDescr))
        ]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.CurrentItemChanged"]/*' />
        public event EventHandler CurrentItemChanged {
            add {
                Events.AddHandler(EVENT_CURRENTITEMCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_CURRENTITEMCHANGED, value);
            }
        }

        [
        SRCategory(nameof(SR.CatData)),
        SRDescription(nameof(SR.BindingSourceListChangedEventHandlerDescr))
        ]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.ListChanged"]/*' />
        public event ListChangedEventHandler ListChanged {
            add {
                Events.AddHandler(EVENT_LISTCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_LISTCHANGED, value);
            }
        }

        [
        SRCategory(nameof(SR.CatData)),
        SRDescription(nameof(SR.BindingSourcePositionChangedEventHandlerDescr))
        ]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.PositionChanged"]/*' />
        public event EventHandler PositionChanged {
            add {
                Events.AddHandler(EVENT_POSITIONCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_POSITIONCHANGED, value);
            }
        }
        #endregion Events

        #region Methods
        ///////////////////////////////////////////////////////////////////////////////
        //
        // Methods
        //
        ///////////////////////////////////////////////////////////////////////////////

        /* CUT:

        //
        // AutoSetDataMember()
        //
        // Used when data source changes. If data member is not set, and the data source
        // is a list of lists, arbitrarily point the data member at one of these lists.
        //
        private void AutoSetDataMember() {
            // Data member already assigned!
            if (!String.IsNullOrEmpty(this.dataMember)) {
                return;
            }

            // Get the list of lists
            IListSource listSource = this.dataSource as IListSource;

            // Not a list of lists!
            if (listSource == null || !listSource.ContainsListCollection) {
                return;
            }

            // Get properties of data source
            PropertyDescriptorCollection props = ListBindingHelper.GetListItemProperties(listSource);

            // Walk properties of data source, looking for one that returns IList (but ignoring ones that return Array)
            for (int i = 0; i < props.Count; ++i) {
                PropertyDescriptor prop = props[i];

                if (typeof(IList).IsAssignableFrom(prop.PropertyType) && !typeof(Array).IsAssignableFrom(prop.PropertyType)) {
                    // Bingo - got one!
                    this.dataMember = prop.Name;
                    OnDataMemberChanged(EventArgs.Empty);
                    break;
                }
            }
        }

        */

        private static string BuildSortString(ListSortDescriptionCollection sortsColln) {
                if (sortsColln == null) {
                    return String.Empty;
                }

                StringBuilder sb = new StringBuilder(sortsColln.Count);

                for (int i = 0; i < sortsColln.Count; ++i) {
                    sb.Append(sortsColln[i].PropertyDescriptor.Name +
                              ((sortsColln[i].SortDirection == ListSortDirection.Ascending) ? " ASC" : " DESC") +
                              ((i < sortsColln.Count - 1) ? "," : String.Empty));
                }

                return sb.ToString();
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.CancelEdit"]/*' />
        public void CancelEdit() {
            currencyManager.CancelCurrentEdit();
        }

        // Walks the BindingSource::DataSource chain until
        // 1. there is a break in the chain ( BindingSource::DataSource is not a BindingSource ), or
        // 2. detects a cycle in the chain.
        // If a cycle is detected we throw the BindingSourceRecursionDetected exception
        private void ThrowIfBindingSourceRecursionDetected(object newDataSource) {
            BindingSource bindingSource = newDataSource as BindingSource;

            while (bindingSource != null) {
                if (bindingSource == this) {
                    throw new InvalidOperationException(SR.BindingSourceRecursionDetected);
                }
                bindingSource = bindingSource.DataSource as BindingSource;
            }
        }

        private void ClearInvalidDataMember() {
            if (!IsDataMemberValid()) {
                this.dataMember = "";
                OnDataMemberChanged(EventArgs.Empty);
            }
        }

        // Creates an instance of BindingList<T> where T is only known at run time, not compile time
        private static IList CreateBindingList(Type type) {
            Type genericType = typeof(BindingList<>);
            Type bindingType = genericType.MakeGenericType(new Type[] { type });

            return (IList) SecurityUtils.SecureCreateInstance(bindingType);
        }

        // Create an object of the given type. Throw an exception if this fails.
        private static object CreateInstanceOfType(Type type) {
            object instancedObject = null;
            Exception instanceException = null;

            try {
                instancedObject = SecurityUtils.SecureCreateInstance(type);
            }
            catch (TargetInvocationException ex) {
                instanceException = ex; // Default ctor threw an exception
            }
            catch (MethodAccessException ex) {
                instanceException = ex; // Default ctor was not public
            }
            catch (MissingMethodException ex) {
                instanceException = ex; // No default ctor defined
            }

            if (instanceException != null) {
                throw new NotSupportedException(SR.BindingSourceInstanceError, instanceException);
            }

            return instancedObject;
        }

        private void CurrencyManager_PositionChanged(object sender, EventArgs e) {
            Debug.Assert(sender == this.currencyManager, "only receive notifications from the currency manager");
            OnPositionChanged(e);
        }

        private void CurrencyManager_CurrentChanged(object sender, EventArgs e) {
            OnCurrentChanged(EventArgs.Empty);
        }

        private void CurrencyManager_CurrentItemChanged(object sender, EventArgs e) {
            OnCurrentItemChanged(EventArgs.Empty);
        }

        private void CurrencyManager_BindingComplete(object sender, BindingCompleteEventArgs e) {
            OnBindingComplete(e);
        }

        private void CurrencyManager_DataError(object sender, BindingManagerDataErrorEventArgs e) {
            OnDataError(e);
        }

        /// <devdoc>
        ///     Unhook BindingSource from its data source, since the data source could be some
        ///     global object who's lifetime exceeds the lifetime of the parent form. Otherwise
        ///     the BindingSource (and any components bound through it) will end up in limbo,
        ///     still processing list change events, etc. And when unhooking from the data source,
        ///     take care not to trigger any events that could confuse compoents bound to us.
        /// </devdoc>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                UnwireDataSource();
                UnwireInnerList();
                UnhookItemChangedEventsForOldCurrent();
                UnwireCurrencyManager(this.currencyManager);
                this.dataSource = null;
                this.sort = null;
                this.dataMember = null;
                this._innerList = null;
                this.isBindingList = false;
                this.needToSetList = true;
                this.raiseListChangedEvents = false;
            }
            disposedOrFinalized = true;
            base.Dispose(disposing);
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.EndEdit"]/*' />
        public void EndEdit() {
            if (endingEdit) {
                return;
            }
            try {
                endingEdit = true;
                currencyManager.EndCurrentEdit();
            }
            finally {
                endingEdit = false;
            }
        }

        //
        // EnsureInnerList()
        //
        // Ensures that the inner list has been set up. Handles the case of ResetList() being called during
        // initialization, which sets a flag to defer ResetList() work until after initialization is complete.
        //
        private void EnsureInnerList() {
            if (!this.initializing && needToSetList) {
                needToSetList = false;
                ResetList();
            }
        }

        //
        // Find()
        //
        // Overload of IBindingList.Find that takes a string instead of a property descriptor (for convenience).
        //
        public int Find(String propertyName, object key) {
            PropertyDescriptor pd = (itemShape == null) ? null : itemShape.Find(propertyName, true);

            if (pd == null) {
                throw new System.ArgumentException(string.Format(SR.DataSourceDataMemberPropNotFound, propertyName));
            }

            return (this as IBindingList).Find(pd, key);
        }

        //
        // GetListFromType()
        //
        // Given a type, create a list based on that type. If the type represents a list type,
        // we create an instance of that type (or throw if we cannot instance that type).
        // Otherwise we assume the type represents the item type, in which case we create
        // a typed BindingList of that item type.
        //

        private static IList GetListFromType(Type type) {
            IList list = null;

            if (typeof(ITypedList).IsAssignableFrom(type) && typeof(IList).IsAssignableFrom(type)) {
                list = CreateInstanceOfType(type) as IList;
            }
            else if (typeof(IListSource).IsAssignableFrom(type)) {
                list = (CreateInstanceOfType(type) as IListSource).GetList();
            }
            else {
                list = CreateBindingList(ListBindingHelper.GetListItemType(type));
            }

            return list;
        }

        //
        // GetListFromEnumerable()
        //
        // Creates a list based on an enumerable object. We rip through the enumerable,
        // extract all its items, and stuff these items into a typed BindingList, using
        // the type of the first item to determine the type of the list.
        //
        private static IList GetListFromEnumerable(IEnumerable enumerable) {
            IList list = null;

            foreach (object item in enumerable) {
                if (list == null) {
                    list = CreateBindingList(item.GetType());
                }

                list.Add(item);
            }

            return list;
        }

        //
        // IsDataMemberValid()
        //
        // Used when we change data sources or when the properties of the current data source change.
        // Decides whether this would be a good time to blow away the data member field, since it
        // might not refer to a valid data source property any more.
        //
        private bool IsDataMemberValid() {
            // Don't mess with things during initialization because the data
            // member property can get set before the data source property.
            if (this.initializing) {
                return true;
            }

            // If data member has not been specified, leave the data member property alone
            if (String.IsNullOrEmpty(this.dataMember)) {
                return true;
            }

            // See if data member corresponds to a valid property on the specified data source
            PropertyDescriptorCollection dsProps = ListBindingHelper.GetListItemProperties(this.dataSource);
            PropertyDescriptor dmProp = dsProps[this.dataMember];
            if (dmProp != null) {
                return true;
            }

            return false;
        }

        private void InnerList_ListChanged(object sender, ListChangedEventArgs e) {
            // Set recursive flag
            // Basically, we can have computed columns that cause our parent
            // to change when our list changes.  This can cause recursion because we update
            // when our parent updates which then causes our parent to update which
            // then causes us to update which then causes our parent to update which
            // then causes us to update which then causes our parent to update...
            if (!this.innerListChanging) {
                try {
                    this.innerListChanging = true;
                    OnListChanged(e);
                }
                finally {
                    this.innerListChanging = false;
                }
            }
        }

        private void ListItem_PropertyChanged(object sender, EventArgs e) {
            int index;

            // Performance: If the item that changed is the current item, we can avoid a potentially expensive call to IndexOf()
            if (sender == currentItemHookedForItemChange) {
                index = this.Position;
                Debug.Assert(index >= 0, "BindingSource.ListItem_PropertyChanged - no current item.");
                Debug.Assert(index == ((IList) this).IndexOf(sender), "BindingSource.ListItem_PropertyChanged - unexpected current item.");
            }
            else {
                index = ((IList) this).IndexOf(sender);
            }
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.MoveFirst"]/*' />
        public void MoveFirst() {
            Position = 0;
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.MoveLast"]/*' />
        public void MoveLast() {
            Position = Count - 1;
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.MoveNext"]/*' />
        public void MoveNext() {
            ++Position;
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.MovePrevious"]/*' />
        public void MovePrevious() {
            --Position;
        }

        // This method is used to fire ListChanged events when the inner list
        // is not an IBindingList (and therefore cannot fire them itself).
        //
        private void OnSimpleListChanged(ListChangedType listChangedType, int newIndex) {
            if (!isBindingList) {
                OnListChanged(new ListChangedEventArgs(listChangedType, newIndex));
            }
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.OnAddingNew"]/*' />
        protected virtual void OnAddingNew(AddingNewEventArgs e) {
            AddingNewEventHandler eh = (AddingNewEventHandler) Events[EVENT_ADDINGNEW];
            if (eh != null)
                eh(this, e);
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.OnBindingComplete"]/*' />
        protected virtual void OnBindingComplete(BindingCompleteEventArgs e) {
            BindingCompleteEventHandler eh = (BindingCompleteEventHandler) Events[EVENT_BINDINGCOMPLETE];
            if (eh != null)
                eh(this, e);
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.OnCurrentChanged"]/*' />
        protected virtual void OnCurrentChanged(EventArgs e) {
            // Unhook change events for old current item (recorded by currentItemHookedForItemChange)
            UnhookItemChangedEventsForOldCurrent();

            // Hook change events for new current item (as indicated now by this.Current)
            HookItemChangedEventsForNewCurrent();

            EventHandler eh = (EventHandler) Events[EVENT_CURRENTCHANGED];
            if (eh != null)
                eh(this, e);
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.OnCurrentItemChanged"]/*' />
        protected virtual void OnCurrentItemChanged(EventArgs e) {
            EventHandler eh = (EventHandler) Events[EVENT_CURRENTITEMCHANGED];
            if (eh != null)
                eh(this, e);
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.OnDataError"]/*' />
        protected virtual void OnDataError(BindingManagerDataErrorEventArgs e) {
            BindingManagerDataErrorEventHandler eh = Events[EVENT_DATAERROR] as BindingManagerDataErrorEventHandler;
            if (eh != null)
                eh(this, e);
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.OnDataMemberContextChanged"]/*' />
        protected virtual void OnDataMemberChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_DATAMEMBERCHANGED] as EventHandler;
            if (eh != null)
                eh(this, e);
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.OnDataSourceContextChanged"]/*' />
        protected virtual void OnDataSourceChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_DATASOURCECHANGED] as EventHandler;
            if (eh != null)
                eh(this, e);
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.OnListChanged"]/*' />
        protected virtual void OnListChanged(ListChangedEventArgs e) {
            // Sometimes we are required to suppress ListChanged events
            if (!this.raiseListChangedEvents || this.initializing) {
                return;
            }

            ListChangedEventHandler eh = (ListChangedEventHandler)Events[EVENT_LISTCHANGED];
            if (eh != null)
                eh(this, e);
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.OnPositionChanged"]/*' />
        protected virtual void OnPositionChanged(EventArgs e) {
            EventHandler eh = (EventHandler) Events[EVENT_POSITIONCHANGED];
            if (eh != null)
                eh(this,e);
        }

        //
        // ParentCurrencyManager_CurrentItemChanged()
        //
        // When the data member is set, and the data source signals a change of current item,
        // we need to query its new current item for the list specified by the data member.
        // Or if there is no longer a current item on the data source, we use an empty list.
        // In either case, we only have to change lists, not metadata, since we can assume
        // that the new list has the same item properties as the old list.
        //
        private void ParentCurrencyManager_CurrentItemChanged(object sender, EventArgs e) {
            if (this.initializing)
                return;

            // Commit pending changes in prior list
            if (parentsCurrentItemChanging) {
                return;
            }
            try {
                parentsCurrentItemChanging = true;
                // Do what RelatedCurrencyManager does when the parent changes:
                // 1. PullData from the controls into the back end.
                // 2. Don't EndEdit the transaction.
                bool success;
                this.currencyManager.PullData(out success);
            }
            finally {
                parentsCurrentItemChanging = false;
            }

            CurrencyManager cm = (CurrencyManager)sender;

            // track if the current list changed
            bool currentItemChanged = true;

            if (!String.IsNullOrEmpty(this.dataMember)) {
                object currentValue = null;
                IList currentList = null;

                if (cm.Count > 0) {
                    // If parent list has a current item, get the sub-list from the relevant property on that item
                    PropertyDescriptorCollection dsProps = cm.GetItemProperties();
                    PropertyDescriptor dmProp = dsProps[this.dataMember];
                    if (dmProp != null) {
                        currentValue = ListBindingHelper.GetList(dmProp.GetValue(cm.Current));
                        currentList = currentValue as IList;
                    }
                }

                if (currentList != null) {
                    // Yippeeee, the current item gave us a list to bind to!
                    // [NOTE: Specify applySortAndFilter=TRUE to apply our sort/filter settings to new list]
                    SetList(currentList, false, true);
                }
                else if (currentValue != null) {
                    // Ok, we didn't get a list, but we did get something, so wrap it in a list
                    // [NOTE: Specify applySortAndFilter=FALSE to stop BindingList<T> from throwing]
                    SetList(WrapObjectInBindingList(currentValue), false, false);
                }
                else {
                    // Nothing to bind to (no current item, or item's property returned null).
                    // Create an empty list, using the previously determined item type.
                    // [NOTE: Specify applySortAndFilter=FALSE to stop BindingList<T> from throwing]
                    SetList(CreateBindingList(this.itemType), false, false);
                }

                // After a change of child lists caused by a change in the current parent item, we
                // should reset the list position (a la RelatedCurrencyManager). But we have to do
                // this explicitly, because a CurrencyManager normally tries to preserve its position
                // after a list reset event.

                // Only reset the position if the list really changed or if the list
                // position is incorrect
                currentItemChanged = ((null == lastCurrentItem) || (cm.Count == 0) || (lastCurrentItem != cm.Current) || (this.Position >= this.Count));

                // Save last current item
                lastCurrentItem = cm.Count > 0 ? cm.Current : null;

                if (currentItemChanged) {
                    this.Position = (this.Count > 0 ? 0 : -1);
                }
            }

            OnCurrentItemChanged(EventArgs.Empty);
        }

        //
        // ParentCurrencyManager_MetaDataChanged()
        //
        // When the data source signals a change of metadata, we need to re-query for the list specified
        // by the data member field. If the data member is no longer valid under the data source's new
        // metadata, we have no choice but to clear the data member field and just bind directly to the
        // data source itself.
        //
        private void ParentCurrencyManager_MetaDataChanged(object sender, EventArgs e) {
            ClearInvalidDataMember();
            ResetList();
        }

        // << Some of this code is taken from System.Data.DataTable::ParseSortString method >>
        private ListSortDescriptionCollection ParseSortString(string sortString) {
            if (String.IsNullOrEmpty(sortString)) {
                return new ListSortDescriptionCollection();
            }

            ArrayList sorts = new ArrayList();
            PropertyDescriptorCollection props = this.currencyManager.GetItemProperties();

            string[] split = sortString.Split(new char[] {','});
            for (int i = 0; i < split.Length; i++) {
                string current = split[i].Trim();

                // Handle ASC and DESC
                int length = current.Length;
                bool ascending = true;
                if (length >= 5 && String.Compare(current, length - 4, " ASC", 0, 4, true, CultureInfo.InvariantCulture) == 0) {
                    current = current.Substring(0, length - 4).Trim();
                }
                else if (length >= 6 && String.Compare(current, length - 5, " DESC", 0, 5, true, CultureInfo.InvariantCulture) == 0) {
                    ascending = false;
                    current = current.Substring(0, length - 5).Trim();
                }

                // Handle brackets
                if (current.StartsWith("[")) {
                    if (current.EndsWith("]")) {
                        current = current.Substring(1, current.Length - 2);
                    }
                    else {
                        throw new ArgumentException(SR.BindingSourceBadSortString);
                    }
                }

                // Find the property
                PropertyDescriptor prop = props.Find(current, true);
                if (prop == null) {
                    throw new ArgumentException(SR.BindingSourceSortStringPropertyNotInIBindingList);
                }

                // Add the sort description
                sorts.Add(new ListSortDescription(prop, ascending ? ListSortDirection.Ascending : ListSortDirection.Descending));
            }

            ListSortDescription[] result = new ListSortDescription[sorts.Count];
            sorts.CopyTo(result);
            return new ListSortDescriptionCollection(result);
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.RemoveCurrent"]/*' />
        public void RemoveCurrent() {
            if (!(this as IBindingList).AllowRemove) {
                throw new InvalidOperationException(SR.BindingSourceRemoveCurrentNotAllowed);
            }

            if (Position < 0 || Position >= Count) {
                throw new InvalidOperationException(SR.BindingSourceRemoveCurrentNoCurrentItem);
            }

            RemoveAt(Position);
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.ResetAllowNew"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual void ResetAllowNew() {
            this.allowNewIsSet = false;
            this.allowNewSetValue = true;
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.ResetBindings"]/* />
        public void ResetBindings(bool metadataChanged) {
            if (metadataChanged) {
                OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, null));
            }

            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.ResetCurrentItem"]/* />
        public void ResetCurrentItem() {
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, this.Position));
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.ResetItem"]/* />
        public void ResetItem(int itemIndex) {
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, itemIndex));
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.ResumeBinding"]/*' />
        public void ResumeBinding() {
            currencyManager.ResumeBinding();
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.SuspendBinding"]/*' />
        public void SuspendBinding() {
            currencyManager.SuspendBinding();
        }

        //
        // ResetList()
        //
        // Binds the BindingSource to the list specified by its DataSource and DataMember properties.
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")] // List is cast to IEnumerable twice. Acceptible trade-off versus code clarity (this method contains *critical* logic).
        private void ResetList() {

            //
            // Don't bind during initialization, since the data source may not have been initialized yet.
            // Instead, set a flag that causes binding to occur on first post-init attempt to access list.
            //
            if (this.initializing) {
                needToSetList = true;
                return;
            }
            else {
                needToSetList = false;
            }

            //
            // Find the list identified by the current DataSource and DataMember properties.
            //
            // If the DataSource only specifies a Type, we actually create an
            // instance from that Type and obtain the list from that instance.
            //
            // Note: The method below will throw an exception if a data member is specified
            // but does not correspond to a valid property on the data source.
            //
            object dataSourceInstance = (this.dataSource is Type) ? GetListFromType(this.dataSource as Type) : this.dataSource;
            object list = ListBindingHelper.GetList(dataSourceInstance, this.dataMember);
            this.listExtractedFromEnumerable = false;

            //
            // Convert the candidate list into an IList, if necessary...
            //
            IList bindingList = null;
            if (list is IList) {
                // If its already an IList then we're done!
                bindingList = list as IList;
            }
            else {
                if (list is IListSource) {
                    bindingList = (list as IListSource).GetList();
                }
                else if (list is IEnumerable) {
                    // If its an enumerable list, extract its contents and put them in a new list
                    bindingList = GetListFromEnumerable(list as IEnumerable);

                    // GetListFromEnumerable returns null if there are no elements
                    // Don't consider it a list of enumerables in this case
                    if (bindingList != null)
                        this.listExtractedFromEnumerable = true;
                }
                // If it's not an IList, IListSource or IEnumerable
                if (bindingList == null) {
                    if (list != null) {
                        // If its some random non-list object, just wrap it in a list
                        bindingList = WrapObjectInBindingList(list);
                    }
                    else {
                        // Can't get any list from the data source (eg. data member specifies related sub-list but the
                        // data source's list is empty, so there is no current item to get the sub-list from). In this
                        // case we simply determine what the list's item type would be, and create empty list with that
                        // same item type. If the item type cannot be determined, we end up with an item type of 'Object'.
                        Type type = ListBindingHelper.GetListItemType(this.dataSource, this.dataMember);
                        bindingList = GetListFromType(type);
                        if (bindingList == null) {
                            bindingList = CreateBindingList (type);
                        }
                    }
                }
            }

            //
            // Bind to this list now
            //
            SetList(bindingList, true, true);
        }

        //
        // SetList()
        //
        // Binds the BindingSource to the specified list, rewiring internal event handlers,
        // firing any appropriate external events, and updating all relevant field members.
        //

        private void SetList(IList list, bool metaDataChanged, bool applySortAndFilter) {
            if (list == null) {
                // The list argument should never be null! We will handle null gracefully
                // at run-time, but we will complain bitterly about this in debug builds!
                Debug.Fail("BindingSource.SetList() was called with illegal <null> list argument!");
                list = CreateBindingList(this.itemType);
            }

            // Unwire stuff from the old list
            UnwireInnerList();
            UnhookItemChangedEventsForOldCurrent();

            // Bind to the new list
            IList listInternal = ListBindingHelper.GetList(list) as IList;
            if (listInternal == null) {
                listInternal = list;
            }

            this._innerList = listInternal;

            // Remember whether the new list implements IBindingList
            this.isBindingList = (listInternal is IBindingList);

            //
            // Determine whether the new list converts PropertyChanged events on its items into ListChanged events.
            // If it does, then the BindingSource won't need to hook the PropertyChanged events itself. If the list
            // implements IRaiseItemChangedEvents, we can ask it directly. Otherwise we will assume that any list
            // which impements IBindingList automatically supports this capability.
            //
            if (listInternal is IRaiseItemChangedEvents) {
                this.listRaisesItemChangedEvents = (listInternal as IRaiseItemChangedEvents).RaisesItemChangedEvents;
            }
            else {
                this.listRaisesItemChangedEvents = this.isBindingList;
            }

            // If list schema may have changed, update list item info now
            if (metaDataChanged) {
                this.itemType = ListBindingHelper.GetListItemType(List);
                this.itemShape = ListBindingHelper.GetListItemProperties(List);
                this.itemConstructor = this.itemType.GetConstructor(BindingFlags.Public |
                                                                    BindingFlags.Instance |
                                                                    BindingFlags.CreateInstance,
                                                                    null, new Type[0], null);
            }

            // Wire stuff up to the new list
            WireInnerList();
            HookItemChangedEventsForNewCurrent();

            // Fire list reset and/or metadata changed events
            ResetBindings(metaDataChanged);

            //
            // Apply any custom Sort and Filter values to the new list.
            //
            // NOTE: The list will throw a NotSupportedException here if it rejects the new sort
            // and filter settings (either because it doesn't support sorting and filtering, or
            // because the sort or filter values were invalid).
            //
            if (applySortAndFilter) {
                if (this.Sort != null) {
                    this.InnerListSort = this.Sort;
                }
                if (this.Filter != null) {
                    this.InnerListFilter = this.Filter;
                }
            }
        }

        private static IList WrapObjectInBindingList(object obj) {
            IList list = CreateBindingList(obj.GetType());
            list.Add(obj);
            return list;
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.ShouldSerializeAllowNew"]/*' />
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeAllowNew() {
            return this.allowNewIsSet;
        }

        ///////////////////////////////////////////////////////////////////////////////
        //
        // ItemChanged event support
        //
        ///////////////////////////////////////////////////////////////////////////////

        // Hooks property changed events for the NEW current item, if nececssary
        private void HookItemChangedEventsForNewCurrent() {
            Debug.Assert(this.currentItemHookedForItemChange == null, "BindingSource trying to hook new current item before unhooking old current item!");

            if (!this.listRaisesItemChangedEvents) {
                if (this.Position >= 0 && this.Position <= this.Count - 1) {
                    this.currentItemHookedForItemChange = this.Current;
                    WirePropertyChangedEvents(this.currentItemHookedForItemChange);
                }
                else {
                    this.currentItemHookedForItemChange = null;
                }
            }
        }

        // Unhooks property changed events for the OLD current item, if necessary
        private void UnhookItemChangedEventsForOldCurrent() {
            if (!this.listRaisesItemChangedEvents) {
                UnwirePropertyChangedEvents(this.currentItemHookedForItemChange);
                this.currentItemHookedForItemChange = null;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////
        //
        // Event wiring and unwiring methods
        //
        ///////////////////////////////////////////////////////////////////////////////

        private void WireCurrencyManager(CurrencyManager cm) {
            if (cm != null) {
                cm.PositionChanged    += new EventHandler(CurrencyManager_PositionChanged);
                cm.CurrentChanged     += new EventHandler(CurrencyManager_CurrentChanged);
                cm.CurrentItemChanged += new EventHandler(CurrencyManager_CurrentItemChanged);
                cm.BindingComplete    += new BindingCompleteEventHandler(CurrencyManager_BindingComplete);
                cm.DataError          += new BindingManagerDataErrorEventHandler(CurrencyManager_DataError);
            }
        }

        private void UnwireCurrencyManager(CurrencyManager cm) {
            if (cm != null) {
                cm.PositionChanged    -= new EventHandler(CurrencyManager_PositionChanged);
                cm.CurrentChanged     -= new EventHandler(CurrencyManager_CurrentChanged);
                cm.CurrentItemChanged -= new EventHandler(CurrencyManager_CurrentItemChanged);
                cm.BindingComplete    -= new BindingCompleteEventHandler(CurrencyManager_BindingComplete);
                cm.DataError          -= new BindingManagerDataErrorEventHandler(CurrencyManager_DataError);
            }
        }

        private void WireDataSource() {
            if (dataSource is ICurrencyManagerProvider) {
                CurrencyManager cm  = (dataSource as ICurrencyManagerProvider).CurrencyManager;
                cm.CurrentItemChanged += new EventHandler(ParentCurrencyManager_CurrentItemChanged);
                cm.MetaDataChanged    += new EventHandler(ParentCurrencyManager_MetaDataChanged);
            }
        }

        private void UnwireDataSource() {
            if (dataSource is ICurrencyManagerProvider) {
                CurrencyManager cm  = (dataSource as ICurrencyManagerProvider).CurrencyManager;
                cm.CurrentItemChanged -= new EventHandler(ParentCurrencyManager_CurrentItemChanged);
                cm.MetaDataChanged    -= new EventHandler(ParentCurrencyManager_MetaDataChanged);
            }
        }

        private void WireInnerList() {
            if (_innerList is IBindingList) {
                IBindingList list = _innerList as IBindingList;
                list.ListChanged += new ListChangedEventHandler(InnerList_ListChanged);
            }
        }

        private void UnwireInnerList() {
            if (_innerList is IBindingList) {
                IBindingList list = _innerList as IBindingList;
                list.ListChanged -= new ListChangedEventHandler(InnerList_ListChanged);
            }
        }

        private void WirePropertyChangedEvents(object item) {
            if (item != null && this.itemShape != null) {
                for (int j = 0; j < this.itemShape.Count; j++) {
                    this.itemShape[j].AddValueChanged(item, listItemPropertyChangedHandler);
                }
            }
        }

        private void UnwirePropertyChangedEvents(object item) {
            if (item != null && this.itemShape != null) {
                for (int j = 0; j < this.itemShape.Count; j++) {
                    this.itemShape[j].RemoveValueChanged(item, listItemPropertyChangedHandler);
                }
            }
        }
        #endregion Methods

        #region ISupportInitialize/ISupportInitializeNotification
        ///////////////////////////////////////////////////////////////////////////////
        //
        // ISupportInitialize and ISupportInitializeNotification interfaces
        //
        ///////////////////////////////////////////////////////////////////////////////

        // Begin bulk member initialization - deferring calculation of inner list until EndInit is reached
        //
        void ISupportInitialize.BeginInit() {
            initializing = true;
        }

        // End bulk member initialization - updating the inner list and notifying any dependents of our completion
        //
        private void EndInitCore() {
            initializing = false;
            EnsureInnerList();
            OnInitialized();
        }

        // Check to see if DataSource has completed its initialization, before ending our initialization.
        // If DataSource is still initializing, hook its Initialized event and wait for it to signal completion.
        // If DataSource is already initialized, just go ahead and complete our initialization now.
        //
        void ISupportInitialize.EndInit() {
            ISupportInitializeNotification dsInit = (this.DataSource as ISupportInitializeNotification);

            if (dsInit != null && !dsInit.IsInitialized) {
                dsInit.Initialized += new EventHandler(DataSource_Initialized);
            }
            else {
                EndInitCore();
            }
        }

        // Respond to late completion of the DataSource's initialization, by completing our own initialization.
        // This situation can arise if the call to the DataSource's EndInit() method comes after the call to the
        // BindingSource's EndInit() method (since code-generated ordering of these calls is non-deterministic).
        //
        private void DataSource_Initialized(object sender, EventArgs e) {
            ISupportInitializeNotification dsInit = (this.DataSource as ISupportInitializeNotification);

            Debug.Assert(dsInit != null, "BindingSource: ISupportInitializeNotification.Initialized event received, but current DataSource does not support ISupportInitializeNotification!");
            Debug.Assert(dsInit.IsInitialized, "BindingSource: DataSource sent ISupportInitializeNotification.Initialized event but before it had finished initializing.");

            if (dsInit != null) {
                dsInit.Initialized -= new EventHandler(DataSource_Initialized);
            }

            EndInitCore();
        }

        // Report to any dependents whether we are still in bulk member initialization
        //
        bool ISupportInitializeNotification.IsInitialized {
            get {
                return !initializing;
            }
        }

        // Event used to signal to our dependents that we have completed bulk member initialization and updated our inner list
        //
        event EventHandler ISupportInitializeNotification.Initialized {
            add {
                Events.AddHandler(EVENT_INITIALIZED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_INITIALIZED, value);
            }
        }

        // Method used to raise the Initialized event above
        //
        private void OnInitialized() {
            EventHandler eh = (EventHandler) Events[EVENT_INITIALIZED];
            if (eh != null)
                eh(this, EventArgs.Empty);
        }
        #endregion ISupportInitialize/ISupportInitializeNotification

        #region IEnumerable
        ///////////////////////////////////////////////////////////////////////////////
        //
        // IEnumerable interface
        //
        ///////////////////////////////////////////////////////////////////////////////

        public virtual IEnumerator GetEnumerator() {
            return List.GetEnumerator();
        }
        #endregion

        #region ICollection
        ///////////////////////////////////////////////////////////////////////////////
        //
        // ICollection interface
        //
        ///////////////////////////////////////////////////////////////////////////////

        public virtual void CopyTo(Array arr, int index) {
            List.CopyTo(arr, index);
        }

        [Browsable(false)]
        public virtual int Count {
            get {
                try 
                {
                    if (disposedOrFinalized)
                    {
                        return 0;
                    }
                    if (recursionDetectionFlag) 
                    {
                        throw new InvalidOperationException(SR.BindingSourceRecursionDetected);
                    }
                    recursionDetectionFlag = true;

                    return List.Count;
                }
                finally 
                {
                    recursionDetectionFlag = false;
                }
            }
        }

        [Browsable(false)]
        public virtual bool IsSynchronized
        {
            get {
                return List.IsSynchronized;
            }
        }

        [Browsable(false)]
        public virtual object SyncRoot {
            get {
                return List.SyncRoot;
            }
        }

        #endregion ICollection

        #region IList
        ///////////////////////////////////////////////////////////////////////////////
        //
        // IList interface
        //
        ///////////////////////////////////////////////////////////////////////////////

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.Add"]/* />
        public virtual int Add(object value) {
            int ret = -1;

            // Special case: If no data source has been assigned, the inner list will just
            // be an empty un-typed binding list. 
            //
            if (dataSource == null && List.Count == 0) {
                SetList(CreateBindingList((value == null) ? typeof(object) : value.GetType()), true, true);
            }

            // Throw if user tries to add items to list that don't match the current item type
            if (value != null && !itemType.IsAssignableFrom(value.GetType())) {
                throw new InvalidOperationException(SR.BindingSourceItemTypeMismatchOnAdd);
            }

            if (value == null && itemType.IsValueType) {
                throw new InvalidOperationException(SR.BindingSourceItemTypeIsValueType);
            }

            ret = List.Add(value);
            OnSimpleListChanged(ListChangedType.ItemAdded, ret);
            return ret;
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.Clear"]/* />
        public virtual void Clear() {
            UnhookItemChangedEventsForOldCurrent();
            List.Clear();
            OnSimpleListChanged(ListChangedType.Reset, -1);
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.Contains"]/* />
        public virtual bool Contains(object value) {
            return List.Contains(value);
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.IndexOf"]/* />
        public virtual int IndexOf(object value) {
            return List.IndexOf(value);
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.Insert"]/* />
        public virtual void Insert(int index, object value) {
            List.Insert(index, value);
            OnSimpleListChanged(ListChangedType.ItemAdded, index);
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.Remove"]/* />
        public virtual void Remove(object value) {
            int index = ((IList) this).IndexOf(value);
            List.Remove(value);
            if (index != -1) {
                OnSimpleListChanged(ListChangedType.ItemDeleted, index);
            }
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.RemoveAt"]/* />
        public virtual void RemoveAt(int index) {
            object value = ((IList) this)[index];
            List.RemoveAt(index);
            OnSimpleListChanged(ListChangedType.ItemDeleted, index);
        }

        [Browsable(false)]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.Item"]/* />
        public virtual object this[int index] {
            get {
                return List[index];
            }

            set {
                List[index] = value;

                if (!isBindingList) {
                    OnSimpleListChanged(ListChangedType.ItemChanged, index);
                }
            }
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.IsFixedSize"]/* />
        [Browsable(false)]
        public virtual bool IsFixedSize {
            get {
                return List.IsFixedSize;
            }
        }

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.IsReadOnly"]/* />
        [Browsable(false)]
        public virtual bool IsReadOnly {
            get {
                return List.IsReadOnly;
            }
        }
        #endregion

        #region ITypedList
        ///////////////////////////////////////////////////////////////////////////////
        //
        // ITypedList interface
        //
        ///////////////////////////////////////////////////////////////////////////////

        public virtual string GetListName(PropertyDescriptor[] listAccessors) {
            return ListBindingHelper.GetListName(List, listAccessors);
        }

        public virtual PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[] listAccessors) {
            object ds = ListBindingHelper.GetList(this.dataSource);

            if (ds is ITypedList && !String.IsNullOrEmpty(this.dataMember)) {
                return ListBindingHelper.GetListItemProperties(ds, this.dataMember, listAccessors);
            }
            else {
                return ListBindingHelper.GetListItemProperties(List, listAccessors);
            }
        }
        #endregion ITypedList

        #region IBindingList
        ///////////////////////////////////////////////////////////////////////////////
        //
        // IBindingList interface
        //
        ///////////////////////////////////////////////////////////////////////////////

        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.AddNew"]/*' />
        public virtual object AddNew() {
            // Throw if adding new items has been disabled
            if (!AllowNewInternal(false)) {
                throw new InvalidOperationException(SR.BindingSourceBindingListWrapperAddToReadOnlyList);
            }

            if (!AllowNewInternal(true)) {
                throw new InvalidOperationException(string.Format(
                    SR.BindingSourceBindingListWrapperNeedToSetAllowNew,
                    itemType == null ? "(null)" : itemType.FullName
                    ));
            }

            // Remember this since EndEdit() below will clear it
            int saveAddNew = this.addNewPos;

            // Commit any uncomitted list changes now
            EndEdit();

            // We just committed a new item; mimic DataView and fire an ItemAdded event for it here
            if (saveAddNew != -1) {
                OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, saveAddNew));
            }

            // Raise the AddingNew event in case listeners want to supply the new item for us
            AddingNewEventArgs addingNew = new AddingNewEventArgs();
            int oldCount = List.Count;
            OnAddingNew(addingNew);
            object addNewItem = addingNew.NewObject;

            //
            // If no item came back from AddingNew event, we must create the new item ourselves...
            //
            if (addNewItem == null) {
                // If the inner list is an IBindingList, let it create and add the new item for us.
                // Then make the new item the current item (...assuming, as CurrencyManager does,
                // that the new item was added at the *bottom* of the list).
                if (isBindingList) {
                    addNewItem = (List as IBindingList).AddNew();
                    this.Position = this.Count - 1;
                    return addNewItem;
                }

                // Throw if we don't know how to create items of the current item type
                if (this.itemConstructor == null) {
                    throw new InvalidOperationException(string.Format(
                        SR.BindingSourceBindingListWrapperNeedAParameterlessConstructor,
                        itemType == null ? "(null)" : itemType.FullName
                        ));
                }

                // Create new item using default ctor for current item type
                addNewItem = this.itemConstructor.Invoke(null);
            }

            if (List.Count > oldCount) {
                // If event handler has already added item to list, then simply record the item's position
                this.addNewPos = this.Position;
            }
            else {
                // If event handler has not yet added item to list, then add it
                // ourselves, make it the current item, and record its position.
                this.addNewPos = this.Add(addNewItem);
                this.Position = this.addNewPos;
            }

            return addNewItem;
        }

        [Browsable(false)]
        public virtual bool AllowEdit {
            get {
                if (isBindingList) {
                    return ((IBindingList) List).AllowEdit;
                }
                else {
                    return !List.IsReadOnly;
                }
            }
        }

        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.BindingSourceAllowNewDescr)),
        ]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.AllowNew"]/*' />
        public virtual bool AllowNew {
            get {
                //we check to ensure we have a valid default constructor (if we get that far).
                return AllowNewInternal(true);
            }

            set {
                // If value was previously set and isn't changing now, do nothing
                if (this.allowNewIsSet && value == this.allowNewSetValue) {
                    return;
                }

                // Don't let user set value to true if inner list can never support adding of items
                // do NOT check for a default constructor because someone will set AllowNew=True
                // when they have overridden OnAddingNew (which we cannot detect).
                if (value == true && !isBindingList && !IsListWriteable(false)) {
                    throw new InvalidOperationException(SR.NoAllowNewOnReadOnlyList);
                }

                // Record new value, which will now override inner list's value
                this.allowNewIsSet = true;
                this.allowNewSetValue = value;

                // Mimic the DataView class and fire a list reset event now
                OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
            }
        }

        [Browsable(false)]
        public virtual bool AllowRemove {
            get {
                if (isBindingList) {
                    return ((IBindingList) List).AllowRemove;
                }
                else {
                    return !List.IsReadOnly && !List.IsFixedSize;
                }
            }
        }

        [Browsable(false)]
        public virtual bool SupportsChangeNotification {
            get {
                return true;
            }
        }

        [Browsable(false)]
        public virtual bool SupportsSearching {
            get {
                if (isBindingList) {
                    return ((IBindingList) List).SupportsSearching;
                }
                else {
                    return false;
                }
            }
        }

        [Browsable(false)]
        public virtual bool SupportsSorting {
            get {
                if (isBindingList) {
                    return ((IBindingList) List).SupportsSorting;
                }
                else {
                    return false;
                }
            }
        }

        [Browsable(false)]
        public virtual bool IsSorted {
            get {
                if (isBindingList) {
                    return ((IBindingList) List).IsSorted;
                }
                else {
                    return false;
                }
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual PropertyDescriptor SortProperty {
            get {
                if (isBindingList) {
                    return ((IBindingList) List).SortProperty;
                }
                else {
                    return null;
                }
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ListSortDirection SortDirection {
            get {
                if (isBindingList) {
                    return ((IBindingList) List).SortDirection;
                }
                else {
                    return ListSortDirection.Ascending;
                }
            }
        }

        void IBindingList.AddIndex(PropertyDescriptor property) {
            if (isBindingList) {
                ((IBindingList) List).AddIndex(property);
            }
            else {
                throw new NotSupportedException(SR.OperationRequiresIBindingList);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ApplySort(PropertyDescriptor property, ListSortDirection sort) {
            if (isBindingList) {
                ((IBindingList) List).ApplySort(property, sort);
            }
            else {
                throw new NotSupportedException(SR.OperationRequiresIBindingList);
            }
        }

        public virtual int Find(PropertyDescriptor prop, object key) {
            if (isBindingList) {
                return ((IBindingList) List).Find(prop, key);
            }
            else {
                throw new NotSupportedException(SR.OperationRequiresIBindingList);
            }
        }

        void IBindingList.RemoveIndex(PropertyDescriptor prop) {
            if (isBindingList) {
                ((IBindingList) List).RemoveIndex(prop);
            }
            else {
                throw new NotSupportedException(SR.OperationRequiresIBindingList);
            }
        }

        public virtual void RemoveSort() {
            this.sort = null;

            if (isBindingList) {
                ((IBindingList) List).RemoveSort();
            }
        }
        #endregion IBindingList

        #region IBindingListView
        ///////////////////////////////////////////////////////////////////////////////
        //
        // IBindingListView interface
        //
        ///////////////////////////////////////////////////////////////////////////////

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ApplySort(ListSortDescriptionCollection sorts) {
            IBindingListView iblw = List as IBindingListView;
            if (iblw != null) {
                iblw.ApplySort(sorts);
            }
            else {
                throw new NotSupportedException(SR.OperationRequiresIBindingListView);
            }
        }

        
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ListSortDescriptionCollection SortDescriptions {
            get {
                IBindingListView iblw = List as IBindingListView;
                if (iblw != null) {
                    return iblw.SortDescriptions;
                } else {
                    return null;
                }
            }
        }

        [
        SRCategory(nameof(SR.CatData)),
        DefaultValue(null),
        SRDescription(nameof(SR.BindingSourceFilterDescr))
        ]
        /// <include file='doc\BindingSource.uex' path='docs/doc[@for="BindingSource.Filter"]/* />
        public virtual string Filter
        {
            get
            {
                return this.filter;
            }
            set
            {
                this.filter = value;
                InnerListFilter = value;
            }
        }

        public virtual void RemoveFilter()
        {
            this.filter = null;

            IBindingListView iblw = List as IBindingListView;
            if (iblw != null) {
                iblw.RemoveFilter();
            }
        }

        [Browsable(false)]
        public virtual bool SupportsAdvancedSorting {
            get {
                IBindingListView iblw = List as IBindingListView;
                if (iblw != null) {
                    return iblw.SupportsAdvancedSorting;
                } else {
                    return false;
                }
            }
        }

        [Browsable(false)]
        public virtual bool SupportsFiltering {
            get {
                IBindingListView iblw = List as IBindingListView;
                if (iblw != null) {
                    return iblw.SupportsFiltering;
                } else {
                    return false;
                }
            }
        }
        #endregion IBindingListView

        #region ICancelAddNew
        ///////////////////////////////////////////////////////////////////////////////
        //
        // ICancelAddNew interface
        //
        ///////////////////////////////////////////////////////////////////////////////

        void ICancelAddNew.CancelNew(int position) {
            if (this.addNewPos >= 0 && this.addNewPos == position) {
                RemoveAt(this.addNewPos); // ...will fire OnListChanged if necessary
                this.addNewPos = -1;
            }
            else {
                ICancelAddNew icancel = List as ICancelAddNew;
                if (icancel != null) {
                    icancel.CancelNew(position);
                }
            }
        }

        void ICancelAddNew.EndNew(int position) {
            if (this.addNewPos >= 0 && this.addNewPos == position) {
                this.addNewPos = -1;
            }
            else {
                ICancelAddNew icancel = List as ICancelAddNew;
                if (icancel != null) {
                    icancel.EndNew(position);
                }
            }
        }
        #endregion ICancelAddNew

        ///////////////////////////////////////////////////////////////////////////////
        //
        // End of BindingSource class
        //
        ///////////////////////////////////////////////////////////////////////////////

    }
}

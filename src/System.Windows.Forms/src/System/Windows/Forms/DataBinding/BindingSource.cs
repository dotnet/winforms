// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Reflection;
using System.Text;

namespace System.Windows.Forms;

[DefaultProperty(nameof(DataSource))]
[DefaultEvent(nameof(CurrentChanged))]
[ComplexBindingProperties(nameof(DataSource), nameof(DataMember))]
[Designer($"System.Windows.Forms.Design.BindingSourceDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionBindingSource))]
public partial class BindingSource : Component,
                             IBindingListView,
                             ITypedList,
                             ICancelAddNew,
                             ISupportInitializeNotification,
                             ICurrencyManagerProvider
{
    // Public events
    private static readonly object s_eventAddingNew = new();
    private static readonly object s_eventBindingComplete = new();
    private static readonly object s_eventCurrentChanged = new();
    private static readonly object s_eventCurrentItemChanged = new();
    private static readonly object s_eventDataError = new();
    private static readonly object s_eventDataMemberChanged = new();
    private static readonly object s_eventDataSourceChanged = new();
    private static readonly object s_eventListChanged = new();
    private static readonly object s_eventPositionChanged = new();
    private static readonly object s_eventInitialized = new();

    // flags enum field to hold private bool fields
    private BindingSourceStates _state;

    // Public property values
    private object? _dataSource;
    private string _dataMember = string.Empty;
    private string? _sort;
    private string? _filter;
    private readonly CurrencyManager _currencyManager;

    // Description of the current bound list
    private IList _innerList; // ...DON'T access this directly. ALWAYS use the List property.

    // Description of items in the current bound list
    private Type? _itemType;
    private ConstructorInfo? _itemConstructor;
    private PropertyDescriptorCollection? _itemShape;

    // Cached list of 'related' binding sources returned to callers of ICurrencyManagerProvider.GetRelatedCurrencyManager()
    private Dictionary<string, BindingSource>? _relatedBindingSources;

    // Support for property change event hooking on list items
    private object? _currentItemHookedForItemChange;
    private object? _lastCurrentItem;
    private readonly EventHandler _listItemPropertyChangedHandler;

    // State data
    private int _addNewPos = -1;

    public BindingSource()
        : this(dataSource: null, dataMember: string.Empty)
    {
    }

    public BindingSource(object? dataSource, string dataMember)
        : base()
    {
        // default values for state
        _state.ChangeFlags(BindingSourceStates.AllowNewSetValue, true);

        // Set data source and data member
        _dataSource = dataSource;
        _dataMember = dataMember;

        // Set inner list to something, so that the currency manager doesn't have an issue.
        _innerList = new List<object>();

        // Set up the currency manager
        _currencyManager = new CurrencyManager(this);
        WireCurrencyManager(_currencyManager);

        // Create event handlers
        _listItemPropertyChangedHandler = ListItem_PropertyChanged;

        // Now set up the inner list properly (which requires the currency manager to be set up beforehand)
        ResetList();

        // Wire up the data source
        WireDataSource();
    }

    public BindingSource(IContainer container)
        : this()
    {
        ArgumentNullException.ThrowIfNull(container);

        container.Add(this);
    }

    private bool AllowNewInternal(bool checkConstructor)
    {
        if (_state.HasFlag(BindingSourceStates.DisposedOrFinalized))
        {
            return false;
        }

        if (_state.HasFlag(BindingSourceStates.AllowNewIsSet))
        {
            return _state.HasFlag(BindingSourceStates.AllowNewSetValue);
        }
        else if (_state.HasFlag(BindingSourceStates.ListExtractedFromEnumerable))
        {
            return false;
        }
        else if (_state.HasFlag(BindingSourceStates.IsBindingList))
        {
            return ((IBindingList)List).AllowNew;
        }
        else
        {
            return IsListWriteable(checkConstructor);
        }
    }

    private bool IsListWriteable(bool checkConstructor)
    {
        return !List.IsReadOnly && !List.IsFixedSize && (!checkConstructor || _itemConstructor is not null);
    }

    [Browsable(false)]
    public virtual CurrencyManager? CurrencyManager
    {
        get => ((ICurrencyManagerProvider)this).GetRelatedCurrencyManager(dataMember: null);
    }

    public virtual CurrencyManager? GetRelatedCurrencyManager(string? dataMember)
    {
        // Make sure inner list has been set up! We do this here so that
        // the list is set up as early as possible after initialization.
        EnsureInnerList();

        // If no data member specified, just return the main currency manager
        if (string.IsNullOrEmpty(dataMember))
        {
            return _currencyManager;
        }

        // Today, this particular implementation of ICurrencyManagerProvider doesn't support the use of 'dot notation'
        // to specify chains of related data members. We don't have any scenarios for this which involve binding sources.
        // Return 'null' to force the binding context to fall back on its default related currency manager behavior.
        if (dataMember.Contains('.'))
        {
            return null;
        }

        // Get related binding source for specified data member (created on first call, then cached on subsequent calls)
        BindingSource bs = GetRelatedBindingSource(dataMember);

        // Return related binding source's currency manager
        return (bs as ICurrencyManagerProvider).CurrencyManager;
    }

    private BindingSource GetRelatedBindingSource(string dataMember)
    {
        // Auto-create the binding source cache on first use
        _relatedBindingSources ??= [];

        // Look for an existing binding source that uses this data member, and return that
        foreach (string key in _relatedBindingSources.Keys)
        {
            if (string.Equals(key, dataMember, StringComparison.OrdinalIgnoreCase))
            {
                return _relatedBindingSources[key];
            }
        }

        // Otherwise create the related binding source, cache it, and return it
        BindingSource bs = new(this, dataMember);
        _relatedBindingSources[dataMember] = bs;
        return bs;
    }

    [Browsable(false)]
    public object? Current => _currencyManager.Count > 0 ? _currencyManager.Current : null;

    [SRCategory(nameof(SR.CatData))]
    [DefaultValue("")]
    [RefreshProperties(RefreshProperties.Repaint)]
    [Editor($"System.Windows.Forms.Design.DataMemberListEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [SRDescription(nameof(SR.BindingSourceDataMemberDescr))]
    [AllowNull]
    public string DataMember
    {
        get => _dataMember;
        set
        {
            value ??= string.Empty;

            if (!_dataMember.Equals(value))
            {
                _dataMember = value;
                ResetList();
                OnDataMemberChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatData))]
    [DefaultValue(null)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [AttributeProvider(typeof(IListSource))]
    [SRDescription(nameof(SR.BindingSourceDataSourceDescr))]
    public object? DataSource
    {
        get => _dataSource;
        set
        {
            if (_dataSource != value)
            {
                // Throw InvalidOperationException if the new data source introduces a cycle.
                ThrowIfBindingSourceRecursionDetected(value);

                // Unhook events on old data source
                UnwireDataSource();

                // Accept the new data source
                _dataSource = value;

                // Blow away the data member property, if its no longer valid under the new data source
                ClearInvalidDataMember();

                // Get the inner list for our new DataSource/DataMember combination
                ResetList();

                // Hook events on new data source
                WireDataSource();

                // Emit our DataSourceChanged event
                OnDataSourceChanged(EventArgs.Empty);
            }
        }
    }

    private string? InnerListFilter
    {
        get
        {
            if (List is IBindingListView iblv && iblv.SupportsFiltering)
            {
                return iblv.Filter;
            }
            else
            {
                return string.Empty;
            }
        }
        set
        {
            if (_state.HasFlag(BindingSourceStates.Initializing) || DesignMode)
            {
                return;
            }

            if (string.Equals(value, InnerListFilter, StringComparison.Ordinal))
            {
                return;
            }

            if (List is IBindingListView iblv && iblv.SupportsFiltering)
            {
                iblv.Filter = value;
            }
        }
    }

    private string? InnerListSort
    {
        get
        {
            ListSortDescriptionCollection? sortsColln = null;

            if (List is IBindingListView iblv && iblv.SupportsAdvancedSorting)
            {
                sortsColln = iblv.SortDescriptions;
            }
            else if (List is IBindingList ibl && ibl.SupportsSorting && ibl.IsSorted)
            {
                ListSortDescription[] sortsArray = [new ListSortDescription(ibl.SortProperty, ibl.SortDirection)];
                sortsColln = new ListSortDescriptionCollection(sortsArray);
            }

            return BuildSortString(sortsColln);
        }
        set
        {
            if (_state.HasFlag(BindingSourceStates.Initializing) || DesignMode)
            {
                return;
            }

            if (string.Equals(value, InnerListSort, StringComparison.InvariantCulture))
            {
                return;
            }

            ListSortDescriptionCollection sortsColln = ParseSortString(value);

            if (List is IBindingListView iblv && iblv.SupportsAdvancedSorting)
            {
                if (sortsColln.Count == 0)
                {
                    iblv.RemoveSort();
                    return;
                }
                else
                {
                    iblv.ApplySort(sortsColln);
                    return;
                }
            }

            if (List is IBindingList ibl && ibl.SupportsSorting)
            {
                if (sortsColln.Count == 0)
                {
                    ibl.RemoveSort();
                    return;
                }
                else if (sortsColln.Count == 1)
                {
                    ListSortDescription listSortDescription = sortsColln[0]!;
                    ibl.ApplySort(listSortDescription.PropertyDescriptor!, listSortDescription.SortDirection);
                    return;
                }
            }
        }
    }

    [Browsable(false)]
    public bool IsBindingSuspended => _currencyManager.IsBindingSuspended;

    [Browsable(false)]
    public IList List
    {
        get
        {
            EnsureInnerList();
            return _innerList;
        }
    }

    [DefaultValue(-1)]
    [Browsable(false)]
    public int Position
    {
        get => _currencyManager.Position;
        set
        {
            if (_currencyManager.Position != value)
            {
                // Setting the position on the currency manager causes EndCurrentEdit
                // even if the position inside the currency manager does not change.
                // This is the RTM/Everett behavior and we have to live w/ it.
                _currencyManager.Position = value;
            }
        }
    }

    [DefaultValue(true)]
    [Browsable(false)]
    public bool RaiseListChangedEvents { get; set; } = true;

    [SRCategory(nameof(SR.CatData))]
    [DefaultValue(null)]
    [SRDescription(nameof(SR.BindingSourceSortDescr))]
    public string? Sort
    {
        get => _sort;
        set
        {
            _sort = value;
            InnerListSort = value;
        }
    }

    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.BindingSourceAddingNewEventHandlerDescr))]
    public event AddingNewEventHandler? AddingNew
    {
        add => Events.AddHandler(s_eventAddingNew, value);
        remove => Events.RemoveHandler(s_eventAddingNew, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.BindingSourceBindingCompleteEventHandlerDescr))]
    public event BindingCompleteEventHandler? BindingComplete
    {
        add => Events.AddHandler(s_eventBindingComplete, value);
        remove => Events.RemoveHandler(s_eventBindingComplete, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.BindingSourceDataErrorEventHandlerDescr))]
    public event BindingManagerDataErrorEventHandler? DataError
    {
        add => Events.AddHandler(s_eventDataError, value);
        remove => Events.RemoveHandler(s_eventDataError, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.BindingSourceDataSourceChangedEventHandlerDescr))]
    public event EventHandler? DataSourceChanged
    {
        add => Events.AddHandler(s_eventDataSourceChanged, value);
        remove => Events.RemoveHandler(s_eventDataSourceChanged, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.BindingSourceDataMemberChangedEventHandlerDescr))]
    public event EventHandler? DataMemberChanged
    {
        add => Events.AddHandler(s_eventDataMemberChanged, value);
        remove => Events.RemoveHandler(s_eventDataMemberChanged, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.BindingSourceCurrentChangedEventHandlerDescr))]
    public event EventHandler? CurrentChanged
    {
        add => Events.AddHandler(s_eventCurrentChanged, value);
        remove => Events.RemoveHandler(s_eventCurrentChanged, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.BindingSourceCurrentItemChangedEventHandlerDescr))]
    public event EventHandler? CurrentItemChanged
    {
        add => Events.AddHandler(s_eventCurrentItemChanged, value);
        remove => Events.RemoveHandler(s_eventCurrentItemChanged, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.BindingSourceListChangedEventHandlerDescr))]
    public event ListChangedEventHandler? ListChanged
    {
        add => Events.AddHandler(s_eventListChanged, value);
        remove => Events.RemoveHandler(s_eventListChanged, value);
    }

    [SRCategory(nameof(SR.CatData))]
    [SRDescription(nameof(SR.BindingSourcePositionChangedEventHandlerDescr))]
    public event EventHandler? PositionChanged
    {
        add => Events.AddHandler(s_eventPositionChanged, value);
        remove => Events.RemoveHandler(s_eventPositionChanged, value);
    }

    private static string BuildSortString(ListSortDescriptionCollection? sortsColln)
    {
        if (sortsColln is null || sortsColln.Count == 0)
        {
            return string.Empty;
        }

        StringBuilder sb = new();

        for (int i = 0; i < sortsColln.Count; ++i)
        {
            ListSortDescription listSortDescription = sortsColln[i]!;
            sb.Append(listSortDescription.PropertyDescriptor!.Name);
            sb.Append(listSortDescription.SortDirection == ListSortDirection.Ascending ? " ASC" : " DESC");
            sb.Append(',');
        }

        return sb.ToString(0, sb.Length - 1);
    }

    public void CancelEdit() => _currencyManager.CancelCurrentEdit();

    // Walks the BindingSource::DataSource chain until
    // 1. there is a break in the chain ( BindingSource::DataSource is not a BindingSource ), or
    // 2. detects a cycle in the chain.
    // If a cycle is detected we throw the BindingSourceRecursionDetected exception
    private void ThrowIfBindingSourceRecursionDetected(object? newDataSource)
    {
        BindingSource? bindingSource = newDataSource as BindingSource;

        while (bindingSource is not null)
        {
            if (bindingSource == this)
            {
                throw new InvalidOperationException(SR.BindingSourceRecursionDetected);
            }

            bindingSource = bindingSource.DataSource as BindingSource;
        }
    }

    private void ClearInvalidDataMember()
    {
        if (!IsDataMemberValid())
        {
            _dataMember = string.Empty;
            OnDataMemberChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  Creates an instance of <see cref="BindingList{T}"/> where T is only known at run time,
    ///  not compile time
    /// </summary>
    private static IList CreateBindingList(Type type)
    {
        Type genericType = typeof(BindingList<>);
        Type bindingType = genericType.MakeGenericType([type]);

        return (IList)Activator.CreateInstance(bindingType)!;
    }

    /// <summary>
    ///  Create an object of the given type. Throw an exception if this fails.
    /// </summary>
    private static object? CreateInstanceOfType(Type type)
    {
        object? instancedObject = null;
        Exception? instanceException = null;

        try
        {
            instancedObject = Activator.CreateInstance(type);
        }
        catch (TargetInvocationException ex)
        {
            instanceException = ex; // Default ctor threw an exception
        }
        catch (MethodAccessException ex)
        {
            instanceException = ex; // Default ctor was not public
        }
        catch (MissingMethodException ex)
        {
            instanceException = ex; // No default ctor defined
        }

        if (instanceException is not null)
        {
            throw new NotSupportedException(SR.BindingSourceInstanceError, instanceException);
        }

        return instancedObject;
    }

    private void CurrencyManager_PositionChanged(object? sender, EventArgs e)
    {
        Debug.Assert(sender == _currencyManager, "only receive notifications from the currency manager");
        OnPositionChanged(e);
    }

    private void CurrencyManager_CurrentChanged(object? sender, EventArgs e)
    {
        OnCurrentChanged(EventArgs.Empty);
    }

    private void CurrencyManager_CurrentItemChanged(object? sender, EventArgs e)
    {
        OnCurrentItemChanged(EventArgs.Empty);
    }

    private void CurrencyManager_BindingComplete(object? sender, BindingCompleteEventArgs e)
    {
        OnBindingComplete(e);
    }

    private void CurrencyManager_DataError(object? sender, BindingManagerDataErrorEventArgs e)
    {
        OnDataError(e);
    }

    /// <summary>
    ///  Unhook BindingSource from its data source, since the data source could be some
    ///  global object who's lifetime exceeds the lifetime of the parent form. Otherwise
    ///  the BindingSource (and any components bound through it) will end up in limbo,
    ///  still processing list change events, etc. And when unhooking from the data source,
    ///  take care not to trigger any events that could confuse components bound to us.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            UnwireDataSource();
            UnwireInnerList();
            UnhookItemChangedEventsForOldCurrent();
            UnwireCurrencyManager(_currencyManager);
            _dataSource = null;
            _sort = null;
            _dataMember = null!;
            _innerList = null!;
            _state.ChangeFlags(BindingSourceStates.IsBindingList, false);
            _state.ChangeFlags(BindingSourceStates.NeedToSetList, true);
            RaiseListChangedEvents = false;
        }

        _state.ChangeFlags(BindingSourceStates.DisposedOrFinalized, true);
        base.Dispose(disposing);
    }

    public void EndEdit()
    {
        if (_state.HasFlag(BindingSourceStates.EndingEdit))
        {
            return;
        }

        try
        {
            _state.ChangeFlags(BindingSourceStates.EndingEdit, true);
            _currencyManager.EndCurrentEdit();
        }
        finally
        {
            _state.ChangeFlags(BindingSourceStates.EndingEdit, false);
        }
    }

    /// <summary>
    ///  Ensures that the inner list has been set up. Handles the case of ResetList() being
    ///  called during initialization, which sets a flag to defer ResetList() work until
    ///  after initialization is complete.
    /// </summary>
    private void EnsureInnerList()
    {
        if (!_state.HasFlag(BindingSourceStates.Initializing) && _state.HasFlag(BindingSourceStates.NeedToSetList))
        {
            _state.ChangeFlags(BindingSourceStates.NeedToSetList, false);
            ResetList();
        }
    }

    /// <summary>
    ///  Overload of IBindingList.Find that takes a string instead of a property
    ///  descriptor (for convenience).
    /// </summary>
    public int Find(string propertyName, object key)
    {
        PropertyDescriptor? pd = (_itemShape?.Find(propertyName, true))
            ?? throw new ArgumentException(string.Format(SR.DataSourceDataMemberPropNotFound, propertyName));

        return ((IBindingList)this).Find(pd, key);
    }

    /// <summary>
    ///  Given a type, create a list based on that type. If the type represents a list type,
    ///  we create an instance of that type (or throw if we cannot instance that type).
    ///  Otherwise we assume the type represents the item type, in which case we create
    ///  a typed BindingList of that item type.
    /// </summary>
    private static IList GetListFromType(Type type)
    {
        if (typeof(ITypedList).IsAssignableFrom(type) && typeof(IList).IsAssignableFrom(type))
        {
            return (IList)CreateInstanceOfType(type)!;
        }
        else if (typeof(IListSource).IsAssignableFrom(type))
        {
            return ((IListSource)CreateInstanceOfType(type)!).GetList();
        }
        else
        {
            return CreateBindingList(ListBindingHelper.GetListItemType(type));
        }
    }

    /// <summary>
    ///  Creates a list based on an enumerable object. We rip through the enumerable,
    ///  extract all its items, and stuff these items into a typed BindingList, using
    ///  the type of the first item to determine the type of the list.
    /// </summary>
    private static IList? GetListFromEnumerable(IEnumerable enumerable)
    {
        IList? list = null;

        foreach (object item in enumerable)
        {
            list ??= CreateBindingList(item.GetType());

            list.Add(item);
        }

        return list;
    }

    /// <summary>
    ///  Used when we change data sources or when the properties of the current data source change.
    ///  Decides whether this would be a good time to blow away the data member field, since it
    ///  might not refer to a valid data source property any more.
    /// </summary>
    private bool IsDataMemberValid()
    {
        // Don't mess with things during initialization because the data
        // member property can get set before the data source property.
        if (_state.HasFlag(BindingSourceStates.Initializing))
        {
            return true;
        }

        // If data member has not been specified, leave the data member property alone
        if (string.IsNullOrEmpty(_dataMember))
        {
            return true;
        }

        // See if data member corresponds to a valid property on the specified data source
        PropertyDescriptorCollection dsProps = ListBindingHelper.GetListItemProperties(_dataSource);
        PropertyDescriptor? dmProp = dsProps[_dataMember];
        if (dmProp is not null)
        {
            return true;
        }

        return false;
    }

    private void InnerList_ListChanged(object? sender, ListChangedEventArgs e)
    {
        // Set recursive flag
        // Basically, we can have computed columns that cause our parent
        // to change when our list changes. This can cause recursion because we update
        // when our parent updates which then causes our parent to update which
        // then causes us to update which then causes our parent to update which
        // then causes us to update which then causes our parent to update...
        if (!_state.HasFlag(BindingSourceStates.InnerListChanging))
        {
            try
            {
                _state.ChangeFlags(BindingSourceStates.InnerListChanging, true);
                OnListChanged(e);
            }
            finally
            {
                _state.ChangeFlags(BindingSourceStates.InnerListChanging, false);
            }
        }
    }

    private void ListItem_PropertyChanged(object? sender, EventArgs e)
    {
        int index;

        // Performance: If the item that changed is the current item, we can avoid a potentially expensive call to IndexOf()
        if (sender == _currentItemHookedForItemChange)
        {
            index = Position;
            Debug.Assert(index >= 0, "BindingSource.ListItem_PropertyChanged - no current item.");
            Debug.Assert(index == ((IList)this).IndexOf(sender), "BindingSource.ListItem_PropertyChanged - unexpected current item.");
        }
        else
        {
            index = ((IList)this).IndexOf(sender);
        }

        OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
    }

    public void MoveFirst() => Position = 0;

    public void MoveLast() => Position = Count - 1;

    public void MoveNext() => Position++;

    public void MovePrevious() => Position--;

    /// <remarks>
    ///  <para>
    ///   This method is used to fire ListChanged events when the inner list is not an <see cref="IBindingList"/>
    ///   (and therefore cannot fire them itself).
    ///  </para>
    /// </remarks>
    private void OnSimpleListChanged(ListChangedType listChangedType, int newIndex)
    {
        if (!_state.HasFlag(BindingSourceStates.IsBindingList))
        {
            OnListChanged(new ListChangedEventArgs(listChangedType, newIndex));
        }
    }

    protected virtual void OnAddingNew(AddingNewEventArgs e)
    {
        AddingNewEventHandler? eh = (AddingNewEventHandler?)Events[s_eventAddingNew];
        eh?.Invoke(this, e);
    }

    protected virtual void OnBindingComplete(BindingCompleteEventArgs e)
    {
        BindingCompleteEventHandler? eh = (BindingCompleteEventHandler?)Events[s_eventBindingComplete];
        eh?.Invoke(this, e);
    }

    protected virtual void OnCurrentChanged(EventArgs e)
    {
        // Unhook change events for old current item (recorded by
        // currentItemHookedForItemChange)
        UnhookItemChangedEventsForOldCurrent();

        // Hook change events for new current item (as indicated now by Current)
        HookItemChangedEventsForNewCurrent();

        EventHandler? eh = (EventHandler?)Events[s_eventCurrentChanged];
        eh?.Invoke(this, e);
    }

    protected virtual void OnCurrentItemChanged(EventArgs e)
    {
        EventHandler? eh = (EventHandler?)Events[s_eventCurrentItemChanged];
        eh?.Invoke(this, e);
    }

    protected virtual void OnDataError(BindingManagerDataErrorEventArgs e)
    {
        BindingManagerDataErrorEventHandler? eh = Events[s_eventDataError] as BindingManagerDataErrorEventHandler;
        eh?.Invoke(this, e);
    }

    protected virtual void OnDataMemberChanged(EventArgs e)
    {
        EventHandler? eh = Events[s_eventDataMemberChanged] as EventHandler;
        eh?.Invoke(this, e);
    }

    protected virtual void OnDataSourceChanged(EventArgs e)
    {
        EventHandler? eh = Events[s_eventDataSourceChanged] as EventHandler;
        eh?.Invoke(this, e);
    }

    protected virtual void OnListChanged(ListChangedEventArgs e)
    {
        // Sometimes we are required to suppress ListChanged events
        if (!RaiseListChangedEvents || _state.HasFlag(BindingSourceStates.Initializing))
        {
            return;
        }

        ListChangedEventHandler? eh = (ListChangedEventHandler?)Events[s_eventListChanged];
        eh?.Invoke(this, e);
    }

    protected virtual void OnPositionChanged(EventArgs e)
    {
        EventHandler? eh = (EventHandler?)Events[s_eventPositionChanged];
        eh?.Invoke(this, e);
    }

    /// <summary>
    ///  When the data member is set, and the data source signals a change of current item,
    ///  we need to query its new current item for the list specified by the data member.
    ///  Or if there is no longer a current item on the data source, we use an empty list.
    ///  In either case, we only have to change lists, not metadata, since we can assume
    ///  that the new list has the same item properties as the old list.
    /// </summary>
    private void ParentCurrencyManager_CurrentItemChanged(object? sender, EventArgs e)
    {
        if (_state.HasFlag(BindingSourceStates.Initializing))
        {
            return;
        }

        // Commit pending changes in prior list
        if (_state.HasFlag(BindingSourceStates.ParentsCurrentItemChanging))
        {
            return;
        }

        try
        {
            _state.ChangeFlags(BindingSourceStates.ParentsCurrentItemChanging, true);
            // Do what RelatedCurrencyManager does when the parent changes:
            // 1. PullData from the controls into the back end.
            // 2. Don't EndEdit the transaction.
            _currencyManager.PullData(out bool success);
        }
        finally
        {
            _state.ChangeFlags(BindingSourceStates.ParentsCurrentItemChanging, false);
        }

        CurrencyManager cm = (CurrencyManager)sender!;

        // track if the current list changed
        bool currentItemChanged;

        if (!string.IsNullOrEmpty(_dataMember))
        {
            object? currentValue = null;
            IList? currentList = null;

            if (cm.Count > 0)
            {
                // If parent list has a current item, get the sub-list from the relevant property on that item
                PropertyDescriptorCollection dsProps = cm.GetItemProperties();
                PropertyDescriptor? dmProp = dsProps[_dataMember];
                if (dmProp is not null)
                {
                    currentValue = ListBindingHelper.GetList(dmProp.GetValue(cm.Current));
                    currentList = currentValue as IList;
                }
            }

            if (currentList is not null)
            {
                // Yippeeee, the current item gave us a list to bind to!
                // [NOTE: Specify applySortAndFilter=TRUE to apply our sort/filter settings to new list]
                SetList(currentList, false, true);
            }
            else if (currentValue is not null)
            {
                // Ok, we didn't get a list, but we did get something, so wrap it in a list
                // [NOTE: Specify applySortAndFilter=FALSE to stop BindingList<T> from throwing]
                SetList(WrapObjectInBindingList(currentValue), false, false);
            }
            else
            {
                // Nothing to bind to (no current item, or item's property returned null).
                // Create an empty list, using the previously determined item type.
                // [NOTE: Specify applySortAndFilter=FALSE to stop BindingList<T> from throwing]
                SetList(CreateBindingList(_itemType!), false, false);
            }

            // After a change of child lists caused by a change in the current parent item, we
            // should reset the list position (a la RelatedCurrencyManager). But we have to do
            // this explicitly, because a CurrencyManager normally tries to preserve its position
            // after a list reset event.

            // Only reset the position if the list really changed or if the list
            // position is incorrect
            currentItemChanged = ((_lastCurrentItem is null) || (cm.Count == 0) || (_lastCurrentItem != cm.Current) || (Position >= Count));

            // Save last current item
            _lastCurrentItem = cm.Count > 0 ? cm.Current : null;

            if (currentItemChanged)
            {
                Position = (Count > 0 ? 0 : -1);
            }
        }

        OnCurrentItemChanged(EventArgs.Empty);
    }

    /// <summary>
    ///  When the data source signals a change of metadata, we need to re-query for the
    ///  list specified by the data member field. If the data member is no longer valid
    ///  under the data source's new metadata, we have no choice but to clear the data
    ///  member field and just bind directly to the data source itself.
    /// </summary>
    private void ParentCurrencyManager_MetaDataChanged(object? sender, EventArgs e)
    {
        ClearInvalidDataMember();
        ResetList();
    }

    private ListSortDescriptionCollection ParseSortString(string? sortString)
    {
        if (string.IsNullOrEmpty(sortString))
        {
            return new ListSortDescriptionCollection();
        }

        PropertyDescriptorCollection props = _currencyManager.GetItemProperties();

        string[] split = sortString.Split(',', StringSplitOptions.TrimEntries);
        ListSortDescription[] sorts = new ListSortDescription[split.Length];
        for (int i = 0; i < split.Length; i++)
        {
            string current = split[i];

            // Handle ASC and DESC
            int length = current.Length;
            bool ascending = true;
            if (current.EndsWith(" ASC", StringComparison.InvariantCulture))
            {
                current = current[..(length - 4)].Trim();
            }
            else if (current.EndsWith(" DESC", StringComparison.InvariantCulture))
            {
                ascending = false;
                current = current[..(length - 5)].Trim();
            }

            // Handle brackets
            if (current.StartsWith('['))
            {
                current = current.EndsWith(']')
                    ? current[1..^1]
                    : throw new ArgumentException(SR.BindingSourceBadSortString);
            }

            // Find the property
            PropertyDescriptor? prop = props.Find(current, true)
                ?? throw new ArgumentException(SR.BindingSourceSortStringPropertyNotInIBindingList);

            // Add the sort description
            sorts[i] = new ListSortDescription(prop, ascending ? ListSortDirection.Ascending : ListSortDirection.Descending);
        }

        return new ListSortDescriptionCollection(sorts);
    }

    public void RemoveCurrent()
    {
        if (!((IBindingList)this).AllowRemove)
        {
            throw new InvalidOperationException(SR.BindingSourceRemoveCurrentNotAllowed);
        }

        if (Position < 0 || Position >= Count)
        {
            throw new InvalidOperationException(SR.BindingSourceRemoveCurrentNoCurrentItem);
        }

        RemoveAt(Position);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public virtual void ResetAllowNew()
    {
        _state.ChangeFlags(BindingSourceStates.AllowNewIsSet, false);
        _state.ChangeFlags(BindingSourceStates.AllowNewSetValue, true);
    }

    public void ResetBindings(bool metadataChanged)
    {
        if (metadataChanged)
        {
            OnListChanged(new ListChangedEventArgs(ListChangedType.PropertyDescriptorChanged, null));
        }

        OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
    }

    public void ResetCurrentItem()
    {
        OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, Position));
    }

    public void ResetItem(int itemIndex)
    {
        OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, itemIndex));
    }

    public void ResumeBinding() => _currencyManager.ResumeBinding();
    public void SuspendBinding() => _currencyManager.SuspendBinding();

    /// <summary>
    ///  Binds the BindingSource to the list specified by its DataSource and DataMember
    ///  properties.
    /// </summary>
    private void ResetList()
    {
        // Don't bind during initialization, since the data source may not have been initialized yet.
        // Instead, set a flag that causes binding to occur on first post-init attempt to access list.
        if (_state.HasFlag(BindingSourceStates.Initializing))
        {
            _state.ChangeFlags(BindingSourceStates.NeedToSetList, true);
            return;
        }
        else
        {
            _state.ChangeFlags(BindingSourceStates.NeedToSetList, false);
        }

        // Find the list identified by the current DataSource and DataMember properties.
        //
        // If the DataSource only specifies a Type, we actually create an
        // instance from that Type and obtain the list from that instance.
        //
        // Note: The method below will throw an exception if a data member is specified
        // but does not correspond to a valid property on the data source.
        object? dataSourceInstance = _dataSource is Type dataSourceType ? GetListFromType(dataSourceType) : _dataSource;
        object? list = ListBindingHelper.GetList(dataSourceInstance, _dataMember);
        _state.ChangeFlags(BindingSourceStates.ListExtractedFromEnumerable, false);

        // Convert the candidate list into an IList, if necessary...
        IList? bindingList = null;
        if (list is IList iList)
        {
            // If its already an IList then we're done!
            bindingList = iList;
        }
        else
        {
            if (list is IListSource iListSource)
            {
                bindingList = iListSource.GetList();
            }
            else if (list is IEnumerable iEnumerable)
            {
                // If its an enumerable list, extract its contents and put them in a new list
                bindingList = GetListFromEnumerable(iEnumerable);

                // GetListFromEnumerable returns null if there are no elements
                // Don't consider it a list of enumerables in this case
                if (bindingList is not null)
                {
                    _state.ChangeFlags(BindingSourceStates.ListExtractedFromEnumerable, true);
                }
            }

            // If it's not an IList, IListSource or IEnumerable
            if (bindingList is null)
            {
                if (list is not null)
                {
                    // If its some random non-list object, just wrap it in a list
                    bindingList = WrapObjectInBindingList(list);
                }
                else
                {
                    // Can't get any list from the data source (eg. data member specifies related sub-list but the
                    // data source's list is empty, so there is no current item to get the sub-list from). In this
                    // case we simply determine what the list's item type would be, and create empty list with that
                    // same item type. If the item type cannot be determined, we end up with an item type of 'Object'.
                    Type type = ListBindingHelper.GetListItemType(_dataSource, _dataMember);
                    bindingList = GetListFromType(type);
                    bindingList ??= CreateBindingList(type);
                }
            }
        }

        // Bind to this list now
        SetList(bindingList, metaDataChanged: true, applySortAndFilter: true);
    }

    /// <summary>
    ///  Binds the BindingSource to the specified list, rewiring internal event handlers,
    ///  firing any appropriate external events, and updating all relevant field members.
    /// </summary>
    private void SetList(IList list, bool metaDataChanged, bool applySortAndFilter)
    {
        if (list is null)
        {
            // The list argument should never be null! We will handle null gracefully
            // at run-time, but we will complain bitterly about this in debug builds!
            Debug.Fail("BindingSource.SetList() was called with illegal <null> list argument!");
            list = CreateBindingList(_itemType);
        }

        // Unwire stuff from the old list
        UnwireInnerList();
        UnhookItemChangedEventsForOldCurrent();

        // Bind to the new list
        if (ListBindingHelper.GetList(list) is not IList listInternal)
        {
            listInternal = list;
        }

        _innerList = listInternal;

        // Remember whether the new list implements IBindingList
        _state.ChangeFlags(BindingSourceStates.IsBindingList, (listInternal is IBindingList));

        // Determine whether the new list converts PropertyChanged events on its items into ListChanged events.
        // If it does, then the BindingSource won't need to hook the PropertyChanged events itself. If the list
        // implements IRaiseItemChangedEvents, we can ask it directly. Otherwise we will assume that any list
        // which implements IBindingList automatically supports this capability.
        if (listInternal is IRaiseItemChangedEvents raiseItemChangedEvents)
        {
            _state.ChangeFlags(BindingSourceStates.ListRaisesItemChangedEvents, raiseItemChangedEvents.RaisesItemChangedEvents);
        }
        else
        {
            _state.ChangeFlags(BindingSourceStates.ListRaisesItemChangedEvents, _state.HasFlag(BindingSourceStates.IsBindingList));
        }

        // If list schema may have changed, update list item info now
        if (metaDataChanged)
        {
            _itemType = ListBindingHelper.GetListItemType(List);
            _itemShape = ListBindingHelper.GetListItemProperties(List);
            _itemConstructor = _itemType.GetConstructor(
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.CreateInstance,
                binder: null,
                [],
                modifiers: null);
        }

        // Wire stuff up to the new list
        WireInnerList();
        HookItemChangedEventsForNewCurrent();

        // Fire list reset and/or metadata changed events
        ResetBindings(metaDataChanged);

        // Apply any custom Sort and Filter values to the new list.
        //
        // NOTE: The list will throw a NotSupportedException here if it rejects the new sort
        // and filter settings (either because it doesn't support sorting and filtering, or
        // because the sort or filter values were invalid).
        if (applySortAndFilter)
        {
            if (Sort is not null)
            {
                InnerListSort = Sort;
            }

            if (Filter is not null)
            {
                InnerListFilter = Filter;
            }
        }
    }

    private static IList WrapObjectInBindingList(object obj)
    {
        IList list = CreateBindingList(obj.GetType());
        list.Add(obj);
        return list;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool ShouldSerializeAllowNew() => _state.HasFlag(BindingSourceStates.AllowNewIsSet);

    /// <summary>
    ///  Hooks property changed events for the NEW current item, if necessary
    /// </summary>
    private void HookItemChangedEventsForNewCurrent()
    {
        Debug.Assert(_currentItemHookedForItemChange is null, "BindingSource trying to hook new current item before unhooking old current item!");

        if (!_state.HasFlag(BindingSourceStates.ListRaisesItemChangedEvents))
        {
            if (Position >= 0 && Position <= Count - 1)
            {
                _currentItemHookedForItemChange = Current;
                WirePropertyChangedEvents(_currentItemHookedForItemChange);
            }
            else
            {
                _currentItemHookedForItemChange = null;
            }
        }
    }

    /// <summary>
    ///  Unhooks property changed events for the OLD current item, if necessary
    /// </summary>
    private void UnhookItemChangedEventsForOldCurrent()
    {
        if (!_state.HasFlag(BindingSourceStates.ListRaisesItemChangedEvents))
        {
            UnwirePropertyChangedEvents(_currentItemHookedForItemChange);
            _currentItemHookedForItemChange = null;
        }
    }

    private void WireCurrencyManager(CurrencyManager cm)
    {
        Debug.Assert(cm is not null);

        cm.PositionChanged += CurrencyManager_PositionChanged;
        cm.CurrentChanged += CurrencyManager_CurrentChanged;
        cm.CurrentItemChanged += CurrencyManager_CurrentItemChanged;
        cm.BindingComplete += CurrencyManager_BindingComplete;
        cm.DataError += CurrencyManager_DataError;
    }

    private void UnwireCurrencyManager(CurrencyManager cm)
    {
        Debug.Assert(cm is not null);

        cm.PositionChanged -= CurrencyManager_PositionChanged;
        cm.CurrentChanged -= CurrencyManager_CurrentChanged;
        cm.CurrentItemChanged -= CurrencyManager_CurrentItemChanged;
        cm.BindingComplete -= CurrencyManager_BindingComplete;
        cm.DataError -= CurrencyManager_DataError;
    }

    private void WireDataSource()
    {
        if (_dataSource is ICurrencyManagerProvider provider)
        {
            CurrencyManager? cm = provider.CurrencyManager;
            if (cm is not null)
            {
                cm.CurrentItemChanged += ParentCurrencyManager_CurrentItemChanged;
                cm.MetaDataChanged += ParentCurrencyManager_MetaDataChanged;
            }
        }
    }

    private void UnwireDataSource()
    {
        if (_dataSource is ICurrencyManagerProvider provider)
        {
            CurrencyManager? cm = provider.CurrencyManager;
            if (cm is not null)
            {
                cm.CurrentItemChanged -= ParentCurrencyManager_CurrentItemChanged;
                cm.MetaDataChanged -= ParentCurrencyManager_MetaDataChanged;
            }
        }
    }

    private void WireInnerList()
    {
        if (_innerList is IBindingList list)
        {
            list.ListChanged += InnerList_ListChanged;
        }
    }

    private void UnwireInnerList()
    {
        if (_innerList is IBindingList list)
        {
            list.ListChanged -= InnerList_ListChanged;
        }
    }

    private void WirePropertyChangedEvents(object? item)
    {
        if (item is not null && _itemShape is not null)
        {
            for (int j = 0; j < _itemShape.Count; j++)
            {
                _itemShape[j].AddValueChanged(item, _listItemPropertyChangedHandler);
            }
        }
    }

    private void UnwirePropertyChangedEvents(object? item)
    {
        if (item is not null && _itemShape is not null)
        {
            for (int j = 0; j < _itemShape.Count; j++)
            {
                _itemShape[j].RemoveValueChanged(item, _listItemPropertyChangedHandler);
            }
        }
    }

    /// <summary>
    ///  Begin bulk member initialization - deferring calculation of inner list until
    ///  EndInit is reached
    /// </summary>
    void ISupportInitialize.BeginInit() => _state.ChangeFlags(BindingSourceStates.Initializing, true);

    /// <summary>
    ///  End bulk member initialization - updating the inner list and notifying any
    ///  dependents of our completion
    /// </summary>
    private void EndInitCore()
    {
        _state.ChangeFlags(BindingSourceStates.Initializing, false);
        EnsureInnerList();
        OnInitialized();
    }

    /// <summary>
    ///  Check to see if DataSource has completed its initialization, before ending our
    ///  initialization.
    ///  If DataSource is still initializing, hook its Initialized event and wait for it
    ///  to signal completion.
    ///  If DataSource is already initialized, just go ahead and complete our
    ///  initialization now.
    /// </summary>
    void ISupportInitialize.EndInit()
    {
        if (DataSource is ISupportInitializeNotification dsInit && !dsInit.IsInitialized)
        {
            dsInit.Initialized += DataSource_Initialized;
        }
        else
        {
            EndInitCore();
        }
    }

    /// <summary>
    ///  Respond to late completion of the DataSource's initialization, by completing our
    ///  own initialization. This situation can arise if the call to the DataSource's
    ///  EndInit() method comes after the call to the BindingSource's EndInit() method
    ///  (since code-generated ordering of these calls is non-deterministic).
    /// </summary>
    private void DataSource_Initialized(object? sender, EventArgs e)
    {
        if (DataSource is ISupportInitializeNotification dsInit)
        {
            dsInit.Initialized -= DataSource_Initialized;
        }

        EndInitCore();
    }

    /// <summary>
    ///  Report to any dependents whether we are still in bulk member initialization
    /// </summary>
    bool ISupportInitializeNotification.IsInitialized => !_state.HasFlag(BindingSourceStates.Initializing);

    /// <summary>
    ///  Event used to signal to our dependents that we have completed bulk member
    ///  initialization and updated our inner list
    /// </summary>
    event EventHandler ISupportInitializeNotification.Initialized
    {
        add => Events.AddHandler(s_eventInitialized, value);
        remove => Events.RemoveHandler(s_eventInitialized, value);
    }

    private void OnInitialized()
    {
        EventHandler? eh = (EventHandler?)Events[s_eventInitialized];
        eh?.Invoke(this, EventArgs.Empty);
    }

    public virtual IEnumerator GetEnumerator() => List.GetEnumerator();

    public virtual void CopyTo(Array arr, int index) => List.CopyTo(arr, index);

    [Browsable(false)]
    public virtual int Count
    {
        get
        {
            try
            {
                if (_state.HasFlag(BindingSourceStates.DisposedOrFinalized))
                {
                    return 0;
                }

                if (_state.HasFlag(BindingSourceStates.RecursionDetectionFlag))
                {
                    throw new InvalidOperationException(SR.BindingSourceRecursionDetected);
                }

                _state.ChangeFlags(BindingSourceStates.RecursionDetectionFlag, true);

                return List.Count;
            }
            finally
            {
                _state.ChangeFlags(BindingSourceStates.RecursionDetectionFlag, false);
            }
        }
    }

    [Browsable(false)]
    public virtual bool IsSynchronized => List.IsSynchronized;

    [Browsable(false)]
    public virtual object SyncRoot => List.SyncRoot;

    public virtual int Add(object? value)
    {
        int position;

        // Special case: If no data source has been assigned, the inner list will just
        // be an empty un-typed binding list.
        if (_dataSource is null && List.Count == 0)
        {
            SetList(CreateBindingList((value is null) ? typeof(object) : value.GetType()), true, true);
        }

        // Throw if user tries to add items to list that don't match the current item type.
        // Either List or SetList has set _itemType, so we don't need to check if it is null.
        if (value is not null && !_itemType!.IsAssignableFrom(value.GetType()))
        {
            throw new InvalidOperationException(SR.BindingSourceItemTypeMismatchOnAdd);
        }

        if (value is null && _itemType!.IsValueType)
        {
            throw new InvalidOperationException(SR.BindingSourceItemTypeIsValueType);
        }

        position = List.Add(value);
        OnSimpleListChanged(ListChangedType.ItemAdded, position);
        return position;
    }

    public virtual void Clear()
    {
        UnhookItemChangedEventsForOldCurrent();
        List.Clear();
        OnSimpleListChanged(ListChangedType.Reset, -1);
    }

    public virtual bool Contains(object? value) => List.Contains(value);

    public virtual int IndexOf(object? value) => List.IndexOf(value);

    public virtual void Insert(int index, object? value)
    {
        List.Insert(index, value);
        OnSimpleListChanged(ListChangedType.ItemAdded, index);
    }

    public virtual void Remove(object? value)
    {
        int index = ((IList)this).IndexOf(value);
        List.Remove(value);
        if (index != -1)
        {
            OnSimpleListChanged(ListChangedType.ItemDeleted, index);
        }
    }

    public virtual void RemoveAt(int index)
    {
        // Virtual, need to call for compat.
        _ = this[index];
        List.RemoveAt(index);
        OnSimpleListChanged(ListChangedType.ItemDeleted, index);
    }

    [Browsable(false)]
    public virtual object? this[int index]
    {
        get => List[index];
        set
        {
            List[index] = value;

            if (!_state.HasFlag(BindingSourceStates.IsBindingList))
            {
                OnSimpleListChanged(ListChangedType.ItemChanged, index);
            }
        }
    }

    [Browsable(false)]
    public virtual bool IsFixedSize => List.IsFixedSize;

    [Browsable(false)]
    public virtual bool IsReadOnly => List.IsReadOnly;

    public virtual string GetListName(PropertyDescriptor[]? listAccessors) =>
        ListBindingHelper.GetListName(List, listAccessors);

    public virtual PropertyDescriptorCollection GetItemProperties(PropertyDescriptor[]? listAccessors)
    {
        object? ds = ListBindingHelper.GetList(_dataSource);

        if (ds is ITypedList && !string.IsNullOrEmpty(_dataMember))
        {
            return ListBindingHelper.GetListItemProperties(ds, _dataMember, listAccessors);
        }
        else
        {
            return ListBindingHelper.GetListItemProperties(List, listAccessors);
        }
    }

    public virtual object? AddNew()
    {
        // Throw if adding new items has been disabled
        if (!AllowNewInternal(checkConstructor: false))
        {
            throw new InvalidOperationException(SR.BindingSourceBindingListWrapperAddToReadOnlyList);
        }

        if (!AllowNewInternal(checkConstructor: true))
        {
            throw new InvalidOperationException(string.Format(
                SR.BindingSourceBindingListWrapperNeedToSetAllowNew,
                _itemType is null ? "(null)" : _itemType.FullName));
        }

        // Remember this since EndEdit() below will clear it
        int saveAddNew = _addNewPos;

        // Commit any uncommitted list changes now
        EndEdit();

        // We just committed a new item; mimic DataView and fire an ItemAdded event for it here
        if (saveAddNew != -1)
        {
            OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, saveAddNew));
        }

        // Raise the AddingNew event in case listeners want to supply the new item for us
        AddingNewEventArgs addingNew = new();
        int oldCount = List.Count;
        OnAddingNew(addingNew);
        object? addNewItem = addingNew.NewObject;

        // If no item came back from AddingNew event, we must create the new item ourselves...
        if (addNewItem is null)
        {
            // If the inner list is an IBindingList, let it create and add the new item for us.
            // Then make the new item the current item (...assuming, as CurrencyManager does,
            // that the new item was added at the *bottom* of the list).
            if (_state.HasFlag(BindingSourceStates.IsBindingList))
            {
                addNewItem = ((IBindingList)List).AddNew();
                Position = Count - 1;
                return addNewItem;
            }

            // Throw if we don't know how to create items of the current item type
            if (_itemConstructor is null)
            {
                throw new InvalidOperationException(string.Format(
                    SR.BindingSourceBindingListWrapperNeedAParameterlessConstructor,
                    _itemType is null ? "(null)" : _itemType.FullName));
            }

            // Create new item using default ctor for current item type
            addNewItem = _itemConstructor.Invoke(null);
        }

        if (List.Count > oldCount)
        {
            // If event handler has already added item to list, then simply record the item's position
            _addNewPos = Position;
        }
        else
        {
            // If event handler has not yet added item to list, then add it
            // ourselves, make it the current item, and record its position.
            _addNewPos = Add(addNewItem);
            Position = _addNewPos;
        }

        return addNewItem;
    }

    [Browsable(false)]
    public virtual bool AllowEdit
    {
        get => _state.HasFlag(BindingSourceStates.IsBindingList) ? ((IBindingList)List).AllowEdit : !List.IsReadOnly;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.BindingSourceAllowNewDescr)),]
    public virtual bool AllowNew
    {
        get => AllowNewInternal(true);
        set
        {
            // If value was previously set and isn't changing now, do nothing
            if (_state.HasFlag(BindingSourceStates.AllowNewIsSet) && value == _state.HasFlag(BindingSourceStates.AllowNewSetValue))
            {
                return;
            }

            // Don't let user set value to true if inner list can never support adding of items
            // do NOT check for a default constructor because someone will set AllowNew=True
            // when they have overridden OnAddingNew (which we cannot detect).
            if (value && !_state.HasFlag(BindingSourceStates.IsBindingList) && !IsListWriteable(checkConstructor: false))
            {
                throw new InvalidOperationException(SR.NoAllowNewOnReadOnlyList);
            }

            // Record new value, which will now override inner list's value
            _state.ChangeFlags(BindingSourceStates.AllowNewIsSet, true);
            _state.ChangeFlags(BindingSourceStates.AllowNewSetValue, value);

            // Mimic the DataView class and fire a list reset event now
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
    }

    [Browsable(false)]
    public virtual bool AllowRemove
    {
        get => _state.HasFlag(BindingSourceStates.IsBindingList) ? ((IBindingList)List).AllowRemove : !List.IsReadOnly && !List.IsFixedSize;
    }

    [Browsable(false)]
    public virtual bool SupportsChangeNotification => true;

    [Browsable(false)]
    public virtual bool SupportsSearching
    {
        get => _state.HasFlag(BindingSourceStates.IsBindingList) && ((IBindingList)List).SupportsSearching;
    }

    [Browsable(false)]
    public virtual bool SupportsSorting
    {
        get => _state.HasFlag(BindingSourceStates.IsBindingList) && ((IBindingList)List).SupportsSorting;
    }

    [Browsable(false)]
    public virtual bool IsSorted
    {
        get => _state.HasFlag(BindingSourceStates.IsBindingList) && ((IBindingList)List).IsSorted;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual PropertyDescriptor? SortProperty
    {
        get => _state.HasFlag(BindingSourceStates.IsBindingList) ? ((IBindingList)List).SortProperty : null;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ListSortDirection SortDirection
    {
        get => _state.HasFlag(BindingSourceStates.IsBindingList) ? ((IBindingList)List).SortDirection : ListSortDirection.Ascending;
    }

    void IBindingList.AddIndex(PropertyDescriptor property)
    {
        if (!_state.HasFlag(BindingSourceStates.IsBindingList))
        {
            throw new NotSupportedException(SR.OperationRequiresIBindingList);
        }

        ((IBindingList)List).AddIndex(property);
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void ApplySort(PropertyDescriptor property, ListSortDirection sort)
    {
        if (!_state.HasFlag(BindingSourceStates.IsBindingList))
        {
            throw new NotSupportedException(SR.OperationRequiresIBindingList);
        }

        ((IBindingList)List).ApplySort(property, sort);
    }

    public virtual int Find(PropertyDescriptor prop, object key)
    {
        if (!_state.HasFlag(BindingSourceStates.IsBindingList))
        {
            throw new NotSupportedException(SR.OperationRequiresIBindingList);
        }

        return ((IBindingList)List).Find(prop, key);
    }

    void IBindingList.RemoveIndex(PropertyDescriptor prop)
    {
        if (!_state.HasFlag(BindingSourceStates.IsBindingList))
        {
            throw new NotSupportedException(SR.OperationRequiresIBindingList);
        }

        ((IBindingList)List).RemoveIndex(prop);
    }

    public virtual void RemoveSort()
    {
        _sort = null;

        if (_state.HasFlag(BindingSourceStates.IsBindingList))
        {
            ((IBindingList)List).RemoveSort();
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual void ApplySort(ListSortDescriptionCollection sorts)
    {
        if (List is not IBindingListView iblv)
        {
            throw new NotSupportedException(SR.OperationRequiresIBindingListView);
        }

        iblv.ApplySort(sorts);
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual ListSortDescriptionCollection SortDescriptions
    {
        get => List is IBindingListView iblv
            ? iblv.SortDescriptions
            : new ListSortDescriptionCollection();
    }

    [SRCategory(nameof(SR.CatData))]
    [DefaultValue(null)]
    [SRDescription(nameof(SR.BindingSourceFilterDescr))]
    public virtual string? Filter
    {
        get => _filter;

        [RequiresUnreferencedCode(TrimmingConstants.BindingListViewFilterMessage)]
        set
        {
            _filter = value;
            InnerListFilter = value;
        }
    }

    public virtual void RemoveFilter()
    {
        _filter = null;

        if (List is IBindingListView iblv)
        {
            iblv.RemoveFilter();
        }
    }

    [Browsable(false)]
    public virtual bool SupportsAdvancedSorting
    {
        get => List is IBindingListView iblv && iblv.SupportsAdvancedSorting;
    }

    [Browsable(false)]
    public virtual bool SupportsFiltering
    {
        get => List is IBindingListView iblv && iblv.SupportsFiltering;
    }

    void ICancelAddNew.CancelNew(int position)
    {
        if (_addNewPos >= 0 && _addNewPos == position)
        {
            RemoveAt(_addNewPos); // ...will fire OnListChanged if necessary
            _addNewPos = -1;
        }
        else
        {
            if (List is ICancelAddNew iCancel)
            {
                iCancel.CancelNew(position);
            }
        }
    }

    void ICancelAddNew.EndNew(int position)
    {
        if (_addNewPos >= 0 && _addNewPos == position)
        {
            _addNewPos = -1;
        }
        else
        {
            if (List is ICancelAddNew iCancel)
            {
                iCancel.EndNew(position);
            }
        }
    }
}

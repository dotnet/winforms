// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;

namespace System.Windows.Forms;

[LookupBindingProperties(nameof(DataSource), nameof(DisplayMember), nameof(ValueMember), nameof(SelectedValue))]
public abstract class ListControl : Control
{
    private static readonly object s_dataSourceChangedEvent = new();
    private static readonly object s_displayMemberChangedEvent = new();
    private static readonly object s_valueMemberChangedEvent = new();
    private static readonly object s_selectedValueChangedEvent = new();
    private static readonly object s_formatInfoChangedEvent = new();
    private static readonly object s_formatStringChangedEvent = new();
    private static readonly object s_formattingEnabledChangedEvent = new();
    private static readonly object s_formatEvent = new();

    private object? _dataSource;
    private CurrencyManager? _dataManager;
    private BindingMemberInfo _displayMember;
    private BindingMemberInfo _valueMember;

    private string _formatString = string.Empty;
    private IFormatProvider? _formatInfo;
    private bool _formattingEnabled;
    private TypeConverter? _displayMemberConverter;
    private static TypeConverter? s_stringTypeConverter;

    private bool _isDataSourceInitialized;
    private bool _isDataSourceInitEventHooked;
    private bool _inSetDataConnection;

    /// <summary>
    ///  The ListSource to consume as this ListBox's source of data.
    ///  When set, a user can not modify the Items collection.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DefaultValue(null)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [AttributeProvider(typeof(IListSource))]
    [SRDescription(nameof(SR.ListControlDataSourceDescr))]
    public object? DataSource
    {
        get => _dataSource;
        set
        {
            if (value is not null and not (IList or IListSource))
            {
                throw new ArgumentException(SR.BadDataSourceForComplexBinding, nameof(value));
            }

            if (_dataSource == value)
            {
                return;
            }

            // When we change the dataSource to null, we should reset
            // the displayMember to "".
            try
            {
                SetDataConnection(value, _displayMember, force: false);
            }
            catch
            {
                // There are several possibilities why setting the data source throws an exception:
                // 1. the app throws an exception in the events that fire when we change the data source: DataSourceChanged,
                // 2. we get an exception when we set the data source and populate the list controls
                //    (say,something went wrong while formatting the data)
                // 3. the DisplayMember does not fit w/ the new data source (this could happen if the user resets the
                //    data source but did not reset the DisplayMember)
                // in all cases ListControl should reset the DisplayMember to String.Empty
                // the ListControl should also eat the exception - this is the RTM behavior and doing anything
                // else is a breaking change
                DisplayMember = string.Empty;
            }

            if (value is null)
            {
                DisplayMember = string.Empty;
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ListControlOnDataSourceChangedDescr))]
    public event EventHandler? DataSourceChanged
    {
        add => Events.AddHandler(s_dataSourceChangedEvent, value);
        remove => Events.RemoveHandler(s_dataSourceChangedEvent, value);
    }

    protected CurrencyManager? DataManager => _dataManager;

    /// <summary>
    ///  If the ListBox contains objects that support properties, this indicates
    ///  which property of the object to show. If "", the object shows it's ToString().
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DefaultValue("")]
    [TypeConverter($"System.Windows.Forms.Design.DataMemberFieldConverter, {AssemblyRef.SystemDesign}")]
    [Editor($"System.Windows.Forms.Design.DataMemberFieldEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [SRDescription(nameof(SR.ListControlDisplayMemberDescr))]
    [AllowNull]
    public string DisplayMember
    {
        get => _displayMember.BindingMember;
        set
        {
            BindingMemberInfo oldDisplayMember = _displayMember;
            try
            {
                SetDataConnection(_dataSource, new BindingMemberInfo(value), force: false);
            }
            catch
            {
                _displayMember = oldDisplayMember;
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ListControlOnDisplayMemberChangedDescr))]
    public event EventHandler? DisplayMemberChanged
    {
        add => Events.AddHandler(s_displayMemberChangedEvent, value);
        remove => Events.RemoveHandler(s_displayMemberChangedEvent, value);
    }

    /// <summary>
    ///  Cached type converter of the property associated with the display member
    /// </summary>
    private TypeConverter? DisplayMemberConverter
    {
        get
        {
            if (!Binding.IsSupported)
            {
                throw new NotSupportedException(SR.BindingNotSupported);
            }

            if (_displayMemberConverter is null)
            {
                PropertyDescriptorCollection? props = DataManager?.GetItemProperties();
                if (props is not null)
                {
                    PropertyDescriptor? displayMemberProperty = props.Find(_displayMember.BindingField, true);
                    if (displayMemberProperty is not null)
                    {
                        _displayMemberConverter = displayMemberProperty.Converter;
                    }
                }
            }

            return _displayMemberConverter;
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ListControlFormatDescr))]
    public event ListControlConvertEventHandler? Format
    {
        add
        {
            Events.AddHandler(s_formatEvent, value);
            RefreshItems();
        }
        remove
        {
            Events.RemoveHandler(s_formatEvent, value);
            RefreshItems();
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DefaultValue(null)]
    public IFormatProvider? FormatInfo
    {
        get => _formatInfo;
        set
        {
            if (value != _formatInfo)
            {
                _formatInfo = value;
                RefreshItems();
                OnFormatInfoChanged(EventArgs.Empty);
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ListControlFormatInfoChangedDescr))]
    public event EventHandler? FormatInfoChanged
    {
        add => Events.AddHandler(s_formatInfoChangedEvent, value);
        remove => Events.RemoveHandler(s_formatInfoChangedEvent, value);
    }

    [DefaultValue("")]
    [SRDescription(nameof(SR.ListControlFormatStringDescr))]
    [Editor($"System.Windows.Forms.Design.FormatStringEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [MergableProperty(false)]
    public string FormatString
    {
        get => _formatString;
        set
        {
            value ??= string.Empty;

            if (!value.Equals(_formatString))
            {
                _formatString = value;
                RefreshItems();
                OnFormatStringChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ListControlFormatStringChangedDescr))]
    public event EventHandler? FormatStringChanged
    {
        add => Events.AddHandler(s_formatStringChangedEvent, value);
        remove => Events.RemoveHandler(s_formatStringChangedEvent, value);
    }

    [DefaultValue(false)]
    [SRDescription(nameof(SR.ListControlFormattingEnabledDescr))]
    public bool FormattingEnabled
    {
        get => _formattingEnabled;
        set
        {
            if (value != _formattingEnabled)
            {
                _formattingEnabled = value;
                RefreshItems();
                OnFormattingEnabledChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ListControlFormattingEnabledChangedDescr))]
    public event EventHandler? FormattingEnabledChanged
    {
        add => Events.AddHandler(s_formattingEnabledChangedEvent, value);
        remove => Events.RemoveHandler(s_formattingEnabledChangedEvent, value);
    }

    private static bool BindingMemberInfoInDataManager(CurrencyManager dataManager, BindingMemberInfo bindingMemberInfo)
    {
        Debug.Assert(dataManager is not null);

        PropertyDescriptorCollection props = dataManager.GetItemProperties();

        for (int i = 0; i < props.Count; i++)
        {
            if (typeof(IList).IsAssignableFrom(props[i].PropertyType))
            {
                continue;
            }

            if (props[i].Name.Equals(bindingMemberInfo.BindingField))
            {
                return true;
            }
        }

        for (int i = 0; i < props.Count; i++)
        {
            if (typeof(IList).IsAssignableFrom(props[i].PropertyType))
            {
                continue;
            }

            if (string.Equals(props[i].Name, bindingMemberInfo.BindingField, StringComparison.CurrentCultureIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    [SRCategory(nameof(SR.CatData))]
    [DefaultValue("")]
    [Editor($"System.Windows.Forms.Design.DataMemberFieldEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [SRDescription(nameof(SR.ListControlValueMemberDescr))]
    [AllowNull]
    public string ValueMember
    {
        get => _valueMember.BindingMember;
        set
        {
            value ??= string.Empty;

            BindingMemberInfo newValueMember = new(value);
            if (!newValueMember.Equals(_valueMember))
            {
                // If the displayMember is set to the EmptyString, then recreate the dataConnection
                if (DisplayMember.Length == 0)
                {
                    SetDataConnection(DataSource, newValueMember, force: false);
                }

                // See if the valueMember is a member of
                // the properties in the dataManager
                if (DataManager is not null && !string.IsNullOrEmpty(value))
                {
                    if (!BindingMemberInfoInDataManager(DataManager, newValueMember))
                    {
                        throw new ArgumentException(SR.ListControlWrongValueMember, nameof(value));
                    }
                }

                _valueMember = newValueMember;
                OnValueMemberChanged(EventArgs.Empty);
                OnSelectedValueChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ListControlOnValueMemberChangedDescr))]
    public event EventHandler? ValueMemberChanged
    {
        add => Events.AddHandler(s_valueMemberChangedEvent, value);
        remove => Events.RemoveHandler(s_valueMemberChangedEvent, value);
    }

    /// <summary>
    ///  Indicates whether list currently allows selection of list items.
    /// </summary>
    protected virtual bool AllowSelection => true;

    public abstract int SelectedIndex { get; set; }

    [SRCategory(nameof(SR.CatData))]
    [DefaultValue(null)]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ListControlSelectedValueDescr))]
    [Bindable(true)]
    [DisallowNull]
    public object? SelectedValue
    {
        get
        {
            if (SelectedIndex != -1 && _dataManager is not null)
            {
                object? currentItem = _dataManager[SelectedIndex];
                return FilterItemOnProperty(currentItem, _valueMember.BindingField);
            }

            return null;
        }
        set
        {
            if (_dataManager is not null)
            {
                string propertyName = _valueMember.BindingField;
                // We can't set the SelectedValue property when the listManager does not
                // have a ValueMember set.
                if (string.IsNullOrEmpty(propertyName))
                {
                    throw new InvalidOperationException(SR.ListControlEmptyValueMemberInSettingSelectedValue);
                }

                PropertyDescriptorCollection props = _dataManager.GetItemProperties();
                PropertyDescriptor? property = props.Find(propertyName, true);
                int index = _dataManager.Find(property, value);
                SelectedIndex = index;
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ListControlOnSelectedValueChangedDescr))]
    public event EventHandler? SelectedValueChanged
    {
        add => Events.AddHandler(s_selectedValueChangedEvent, value);
        remove => Events.RemoveHandler(s_selectedValueChangedEvent, value);
    }

    private void DataManager_PositionChanged(object? sender, EventArgs e)
    {
        if (_dataManager is not null)
        {
            if (AllowSelection)
            {
                SelectedIndex = _dataManager.Position;
            }
        }
    }

    private void DataManager_ItemChanged(object? sender, ItemChangedEventArgs e)
    {
        // Note this is being called internally with a null event.
        if (_dataManager is not null)
        {
            if (e.Index == -1)
            {
                SetItemsCore(_dataManager.List);
                if (AllowSelection)
                {
                    SelectedIndex = _dataManager.Position;
                }
            }
            else
            {
                SetItemCore(e.Index, _dataManager[e.Index]!);
            }
        }
    }

    protected object? FilterItemOnProperty(object? item)
    {
        return FilterItemOnProperty(item, _displayMember.BindingField);
    }

    protected object? FilterItemOnProperty(object? item, string? field)
    {
        if (item is not null && !string.IsNullOrEmpty(field))
        {
            if (!Binding.IsSupported)
            {
                throw new NotSupportedException(SR.BindingNotSupported);
            }

            try
            {
                // if we have a dataSource, then use that to display the string
                PropertyDescriptor? descriptor;
                if (DataManager is not null)
                {
                    descriptor = DataManager.GetItemProperties().Find(field, true);
                }
                else
                {
                    descriptor = TypeDescriptor.GetProperties(item).Find(field, true);
                }

                if (descriptor is not null)
                {
                    item = descriptor.GetValue(item);
                }
            }
            catch
            {
            }
        }

        return item;
    }

    /// <remarks>
    ///  <para>
    ///   We use this to prevent getting the selected item when mouse is hovering over the dropdown.
    ///  </para>
    /// </remarks>
    private protected bool BindingFieldEmpty => _displayMember.BindingField.Length == 0;

    private protected int FindStringInternal(string? str, IList? items, int startIndex, bool exact, bool ignoreCase)
    {
        if (str is null)
        {
            return -1;
        }

        if (items is null || items.Count == 0)
        {
            return -1;
        }

        ArgumentOutOfRangeException.ThrowIfLessThan(startIndex, -1);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(startIndex, items.Count);

        // Start from the start index and wrap around until we find the string
        // in question. Use a separate counter to ensure that we aren't cycling through the list infinitely.
        int numberOfTimesThroughLoop = 0;

        // this API is really Find NEXT String...
        for (int index = (startIndex + 1) % items.Count; numberOfTimesThroughLoop < items.Count; index = (index + 1) % items.Count)
        {
            numberOfTimesThroughLoop++;

            bool found;
            if (exact)
            {
                found = string.Compare(str, GetItemText(items[index]), ignoreCase, CultureInfo.CurrentCulture) == 0;
            }
            else
            {
                found = string.Compare(str, 0, GetItemText(items[index]), 0, str.Length, ignoreCase, CultureInfo.CurrentCulture) == 0;
            }

            if (found)
            {
                return index;
            }
        }

        return -1;
    }

    public string? GetItemText(object? item)
    {
        if (!_formattingEnabled)
        {
            if (item is null)
            {
                return string.Empty;
            }

            item = FilterItemOnProperty(item, _displayMember.BindingField);
            if (item is null)
            {
                return string.Empty;
            }

            return Convert.ToString(item, CultureInfo.CurrentCulture);
        }

        object? filteredItem = FilterItemOnProperty(item, _displayMember.BindingField);

        // First try the OnFormat event
        ListControlConvertEventArgs e = new(filteredItem, typeof(string), item);
        OnFormat(e);
        if (e.Value != item && e.Value is string stringValue)
        {
            return stringValue;
        }

        // Try Formatter.FormatObject
        if (!UseComponentModelRegisteredTypes)
        {
            s_stringTypeConverter ??= TypeDescriptor.GetConverter(typeof(string));
        }
        else
        {
            // Call the trim safe API
            s_stringTypeConverter ??= TypeDescriptor.GetConverterFromRegisteredType(typeof(string));
        }

        try
        {
            return (string?)Formatter.FormatObject(
                filteredItem,
                typeof(string),
                DisplayMemberConverter,
                s_stringTypeConverter,
                _formatString,
                _formatInfo,
                formattedNullValue: null,
                DBNull.Value);
        }
        catch (Exception exception) when (!exception.IsCriticalException())
        {
            // if we did not do any work then return the old ItemText
            return Convert.ToString(item, CultureInfo.CurrentCulture);
        }
    }

    /// <summary>
    ///  Handling special input keys, such as PageUp, PageDown, Home, End, etc...
    /// </summary>
    protected override bool IsInputKey(Keys keyData)
    {
        if ((keyData & Keys.Alt) == Keys.Alt)
        {
            return false;
        }

        return (keyData & Keys.KeyCode) switch
        {
            Keys.PageUp or Keys.PageDown or Keys.Home or Keys.End => true,
            _ => base.IsInputKey(keyData),
        };
    }

    protected override void OnBindingContextChanged(EventArgs e)
    {
        SetDataConnection(_dataSource, _displayMember, force: true);
        base.OnBindingContextChanged(e);
    }

    protected virtual void OnDataSourceChanged(EventArgs e)
    {
        EventHandler? eh = Events[s_dataSourceChangedEvent] as EventHandler;
        eh?.Invoke(this, e);
    }

    protected virtual void OnDisplayMemberChanged(EventArgs e)
    {
        EventHandler? eh = Events[s_displayMemberChangedEvent] as EventHandler;
        eh?.Invoke(this, e);
    }

    protected virtual void OnFormat(ListControlConvertEventArgs e)
    {
        ListControlConvertEventHandler? eh = Events[s_formatEvent] as ListControlConvertEventHandler;
        eh?.Invoke(this, e);
    }

    protected virtual void OnFormatInfoChanged(EventArgs e)
    {
        EventHandler? eh = Events[s_formatInfoChangedEvent] as EventHandler;
        eh?.Invoke(this, e);
    }

    protected virtual void OnFormatStringChanged(EventArgs e)
    {
        EventHandler? eh = Events[s_formatStringChangedEvent] as EventHandler;
        eh?.Invoke(this, e);
    }

    protected virtual void OnFormattingEnabledChanged(EventArgs e)
    {
        EventHandler? eh = Events[s_formattingEnabledChangedEvent] as EventHandler;
        eh?.Invoke(this, e);
    }

    /// <summary>
    ///  Actually goes and fires the selectedIndexChanged event. Inheriting controls
    ///  should use this to know when the event is fired [this is preferable to
    ///  adding an event handler on yourself for this event]. They should,
    ///  however, remember to call base.OnSelectedIndexChanged(e); to ensure the event is
    ///  still fired to external listeners
    /// </summary>
    protected virtual void OnSelectedIndexChanged(EventArgs e)
    {
        OnSelectedValueChanged(e);
    }

    protected virtual void OnValueMemberChanged(EventArgs e)
    {
        EventHandler? eh = Events[s_valueMemberChangedEvent] as EventHandler;
        eh?.Invoke(this, e);
    }

    protected virtual void OnSelectedValueChanged(EventArgs e)
    {
        EventHandler? eh = Events[s_selectedValueChangedEvent] as EventHandler;
        eh?.Invoke(this, e);
    }

    protected abstract void RefreshItem(int index);

    protected virtual void RefreshItems()
    {
    }

    private void DataSourceDisposed(object? sender, EventArgs e)
    {
        SetDataConnection(null, new BindingMemberInfo(string.Empty), true);
    }

    private void DataSourceInitialized(object? sender, EventArgs e)
    {
        SetDataConnection(_dataSource, _displayMember, true);
    }

    private void SetDataConnection(object? newDataSource, BindingMemberInfo newDisplayMember, bool force)
    {
        bool dataSourceChanged = _dataSource != newDataSource;
        bool displayMemberChanged = !_displayMember.Equals(newDisplayMember);

        if (_inSetDataConnection)
        {
            return;
        }

        try
        {
            if (force || dataSourceChanged || displayMemberChanged)
            {
                _inSetDataConnection = true;
                IList? currentList = DataManager?.List;
                bool currentManagerIsNull = DataManager is null;

                UnwireDataSource();

                _dataSource = newDataSource;
                _displayMember = newDisplayMember;

                WireDataSource();

                // Provided the data source has been fully initialized, start listening to change events on its
                // currency manager and refresh our list. If the data source has not yet been initialized, we will
                // skip this step for now, and try again later (once the data source has fired its Initialized event).
                if (_isDataSourceInitialized)
                {
                    CurrencyManager? newDataManager = null;
                    if (newDataSource is not null && BindingContext is not null && newDataSource != Convert.DBNull)
                    {
                        if (!Binding.IsSupported)
                        {
                            throw new NotSupportedException(SR.BindingNotSupported);
                        }

                        newDataManager = (CurrencyManager)BindingContext[newDataSource, newDisplayMember.BindingPath];
                    }

                    if (_dataManager != newDataManager)
                    {
                        if (_dataManager is not null)
                        {
                            _dataManager.ItemChanged -= DataManager_ItemChanged;
                            _dataManager.PositionChanged -= DataManager_PositionChanged;
                        }

                        _dataManager = newDataManager;

                        if (_dataManager is not null)
                        {
                            _dataManager.ItemChanged += DataManager_ItemChanged;
                            _dataManager.PositionChanged += DataManager_PositionChanged;
                        }
                    }

                    // See if the BindingField in the newDisplayMember is valid
                    // The same thing if dataSource Changed
                    // "" is a good value for displayMember
                    if (_dataManager is not null && (displayMemberChanged || dataSourceChanged) && !string.IsNullOrEmpty(_displayMember.BindingMember))
                    {
                        if (!BindingMemberInfoInDataManager(_dataManager, _displayMember))
                        {
                            throw new ArgumentException(SR.ListControlWrongDisplayMember, nameof(newDisplayMember));
                        }
                    }

                    if (_dataManager is not null && (dataSourceChanged || displayMemberChanged || force))
                    {
                        // If we force a new data manager, then change the items in the list control
                        // only if the list changed or if we go from a null dataManager to a full fledged one
                        // or if the DisplayMember changed
                        if (displayMemberChanged || (force && (currentList != _dataManager.List || currentManagerIsNull)))
                        {
                            DataManager_ItemChanged(_dataManager, new ItemChangedEventArgs(-1));
                        }
                    }
                }

                _displayMemberConverter = null;
            }

            if (dataSourceChanged)
            {
                OnDataSourceChanged(EventArgs.Empty);
            }

            if (displayMemberChanged)
            {
                OnDisplayMemberChanged(EventArgs.Empty);
            }
        }
        finally
        {
            _inSetDataConnection = false;
        }
    }

    private void UnwireDataSource()
    {
        // If the source is a component, then unhook the Disposed event
        if (_dataSource is IComponent componentDataSource)
        {
            componentDataSource.Disposed -= DataSourceDisposed;
        }

        if (_dataSource is ISupportInitializeNotification dsInit && _isDataSourceInitEventHooked)
        {
            // If we previously hooked the data source's ISupportInitializeNotification
            // Initialized event, then unhook it now (we don't always hook this event,
            // only if we needed to because the data source was previously uninitialized)
            dsInit.Initialized -= DataSourceInitialized;
            _isDataSourceInitEventHooked = false;
        }
    }

    private void WireDataSource()
    {
        // If the source is a component, then hook the Disposed event,
        // so we know when the component is deleted from the form
        if (_dataSource is IComponent componentDataSource)
        {
            componentDataSource.Disposed += DataSourceDisposed;
        }

        if (_dataSource is ISupportInitializeNotification dsInit && !dsInit.IsInitialized)
        {
            // If the source provides initialization notification, and is not yet
            // fully initialized, then hook the Initialized event, so that we can
            // delay connecting to it until it *is* initialized.
            dsInit.Initialized += DataSourceInitialized;
            _isDataSourceInitEventHooked = true;
            _isDataSourceInitialized = false;
        }
        else
        {
            // Otherwise either the data source says it *is* initialized, or it
            // does not support the capability to report whether its initialized,
            // in which case we have to just assume it that is initialized.
            _isDataSourceInitialized = true;
        }
    }

    protected abstract void SetItemsCore(IList items);

    protected virtual void SetItemCore(int index, object value)
    {
    }
}

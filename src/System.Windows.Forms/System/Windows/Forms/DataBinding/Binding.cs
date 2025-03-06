// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms;

/// <summary>
///  Represents a simple binding of a value in a list and the property of a control.
/// </summary>
[TypeConverter(typeof(ListBindingConverter))]
public partial class Binding
{
    // Feature switch, when set to false, binding is not supported in trimmed applications.
    [FeatureSwitchDefinition("System.Windows.Forms.Binding.IsSupported")]
#pragma warning disable IDE0075 // Simplify conditional expression - the simpler expression is hard to read
    internal static bool IsSupported { get; } =
        AppContext.TryGetSwitch("System.Windows.Forms.Binding.IsSupported", out bool isSupported)
            ? isSupported
            : true;
#pragma warning restore IDE0075

    private BindingManagerBase? _bindingManagerBase;

    private readonly BindToObject _bindToObject;

    private PropertyDescriptor? _propInfo;
    private PropertyDescriptor? _propIsNullInfo;
    private EventDescriptor? _validateInfo;
    private TypeConverter? _propInfoConverter;

    private BindingStates _state;

    // formatting stuff
    private string _formatString = string.Empty;
    private IFormatProvider? _formatInfo;
    private object? _nullValue;
    private object? _dsNullValue = Formatter.GetDefaultDataSourceNullValue(null);
    private ConvertEventHandler? _onParse;
    private ConvertEventHandler? _onFormat;

    // binding stuff
    private ControlUpdateMode _controlUpdateMode = ControlUpdateMode.OnPropertyChanged;
    private BindingCompleteEventHandler? _onComplete;

    /// <summary>
    ///  Initializes a new instance of the <see cref="Binding"/> class
    ///  that binds a property on the owning control to a property on a data source.
    /// </summary>
    public Binding(string propertyName, object? dataSource, string? dataMember)
        : this(
              propertyName,
              dataSource,
              dataMember,
              formattingEnabled: false,
              0,
              nullValue: null,
              formatString: string.Empty,
              formatInfo: null)
    {
    }

    public Binding(
        string propertyName,
        object? dataSource,
        string? dataMember,
        bool formattingEnabled)
        : this(
              propertyName,
              dataSource,
              dataMember,
              formattingEnabled,
              0,
              nullValue: null,
              formatString: string.Empty,
              formatInfo: null)
    {
    }

    public Binding(
        string propertyName,
        object? dataSource,
        string? dataMember,
        bool formattingEnabled,
        DataSourceUpdateMode dataSourceUpdateMode)
        : this(
              propertyName,
              dataSource,
              dataMember,
              formattingEnabled,
              dataSourceUpdateMode,
              nullValue: null,
              formatString: string.Empty,
              formatInfo: null)
    {
    }

    public Binding(
        string propertyName,
        object? dataSource,
        string? dataMember,
        bool formattingEnabled,
        DataSourceUpdateMode dataSourceUpdateMode,
        object? nullValue)
        : this(
              propertyName,
              dataSource,
              dataMember,
              formattingEnabled,
              dataSourceUpdateMode,
              nullValue,
              formatString: string.Empty,
              formatInfo: null)
    {
    }

    public Binding(
        string propertyName,
        object? dataSource,
        string? dataMember,
        bool formattingEnabled,
        DataSourceUpdateMode dataSourceUpdateMode,
        object? nullValue,
        string formatString)
        : this(
              propertyName,
              dataSource,
              dataMember,
              formattingEnabled,
              dataSourceUpdateMode,
              nullValue,
              formatString,
              formatInfo: null)
    {
    }

    public Binding(
        string propertyName,
        object? dataSource,
        string? dataMember,
        bool formattingEnabled,
        DataSourceUpdateMode dataSourceUpdateMode,
        object? nullValue,
        string formatString,
        IFormatProvider? formatInfo)
    {
        DataSource = dataSource;
        BindingMemberInfo = new BindingMemberInfo(dataMember);
        _bindToObject = new BindToObject(this);

        PropertyName = propertyName;
        _state.ChangeFlags(BindingStates.FormattingEnabled, formattingEnabled);
        _formatString = formatString;
        _nullValue = nullValue;
        _formatInfo = formatInfo;
        DataSourceUpdateMode = dataSourceUpdateMode;

        CheckBinding();
    }

    public object? DataSource { get; }

    public BindingMemberInfo BindingMemberInfo { get; }

    /// <summary>
    ///  Gets the control to which the binding belongs.
    /// </summary>
    [DefaultValue(null)]
    public IBindableComponent? BindableComponent { get; private set; }

    /// <summary>
    ///  Gets the control to which the binding belongs.
    /// </summary>
    [DefaultValue(null)]
    public Control? Control => BindableComponent as Control;

    /// <summary>
    ///  Is the bindable component in a 'created' (ready-to-use) state? For controls,
    ///  this depends on whether the window handle has been created yet. For everything
    ///  else, we'll assume they are always in a created state.
    /// </summary>
    internal static bool IsComponentCreated(IBindableComponent? component)
    {
        return component is not Control control || control.Created;
    }

    /// <summary>
    ///  Instance-specific property equivalent to the static method above
    /// </summary>
    internal bool ComponentCreated => IsComponentCreated(BindableComponent);

    private void FormLoaded(object? sender, EventArgs e)
    {
        Debug.Assert(sender == BindableComponent, "which other control can send us the Load event?");
        // update the binding
        CheckBinding();
    }

    internal void SetBindableComponent(IBindableComponent? value)
    {
        if (BindableComponent != value)
        {
            IBindableComponent? oldTarget = BindableComponent;
            BindTarget(false);
            BindableComponent = value;
            BindTarget(true);
            try
            {
                CheckBinding();
            }
            catch
            {
                BindTarget(false);
                BindableComponent = oldTarget;
                BindTarget(true);
                throw;
            }

            // We are essentially doing to the listManager what we were doing to the
            // BindToObject: bind only when the control is created and it has a BindingContext
            BindingContext.UpdateBinding((BindableComponent is not null && IsComponentCreated(BindableComponent) ? BindableComponent.BindingContext : null), this);
            if (value is Form form)
            {
                form.Load += FormLoaded;
            }
        }
    }

    /// <summary>
    ///  Gets a value indicating whether the binding is active.
    /// </summary>
    public bool IsBinding { get; private set; }

    /// <summary>
    ///  Gets the <see cref="Forms.BindingManagerBase"/> of this binding that
    ///  allows enumeration of a set of bindings.
    /// </summary>
    public BindingManagerBase? BindingManagerBase
    {
        get => _bindingManagerBase;
        internal set
        {
            if (_bindingManagerBase != value)
            {
                if (_bindingManagerBase is CurrencyManager oldCurrencyManager)
                {
                    oldCurrencyManager.MetaDataChanged -= binding_MetaDataChanged;
                }

                _bindingManagerBase = value;

                if (value is CurrencyManager newCurrencyManager)
                {
                    newCurrencyManager.MetaDataChanged += binding_MetaDataChanged;
                }

                _bindToObject.SetBindingManagerBase(value);
                CheckBinding();
            }
        }
    }

    /// <summary>
    ///  Gets or sets the property on the control to bind to.
    /// </summary>
    [DefaultValue("")]
    public string PropertyName { get; } = string.Empty;

    public event BindingCompleteEventHandler? BindingComplete
    {
        add => _onComplete += value;
        remove => _onComplete -= value;
    }

    public event ConvertEventHandler? Parse
    {
        add => _onParse += value;
        remove => _onParse -= value;
    }

    public event ConvertEventHandler? Format
    {
        add => _onFormat += value;
        remove => _onFormat -= value;
    }

    [DefaultValue(false)]
    public bool FormattingEnabled
    {
        // A note about FormattingEnabled: This flag was introduced in Whidbey, to enable new
        // formatting features. However, it is also used to trigger other new Whidbey binding
        // behavior not related to formatting (such as error handling). This preserves Everett
        // legacy behavior for old bindings (where FormattingEnabled = false).
        get => _state.HasFlag(BindingStates.FormattingEnabled);
        set
        {
            if (_state.HasFlag(BindingStates.FormattingEnabled) != value)
            {
                _state.ChangeFlags(BindingStates.FormattingEnabled, value);
                if (IsBinding)
                {
                    PushData();
                }
            }
        }
    }

    [DefaultValue(null)]
    public IFormatProvider? FormatInfo
    {
        get => _formatInfo;
        set
        {
            if (_formatInfo != value)
            {
                _formatInfo = value;
                if (IsBinding)
                {
                    PushData();
                }
            }
        }
    }

    public string FormatString
    {
        get => _formatString;
        set
        {
            value ??= string.Empty;

            if (!value.Equals(_formatString))
            {
                _formatString = value;
                if (IsBinding)
                {
                    PushData();
                }
            }
        }
    }

    public object? NullValue
    {
        get => _nullValue;
        set
        {
            // Try to compare logical values, not object references...
            if (!Equals(_nullValue, value))
            {
                _nullValue = value;

                // If data member is currently DBNull, force update of bound
                // control property so that it displays the new NullValue
                if (IsBinding && Formatter.IsNullData(_bindToObject.GetValue(), _dsNullValue))
                {
                    PushData();
                }
            }
        }
    }

    public object? DataSourceNullValue
    {
        get => _dsNullValue;
        set
        {
            // Try to compare logical values, not object references...
            if (!Equals(_dsNullValue, value))
            {
                // Save old Value
                object? oldValue = _dsNullValue;

                // Set value
                _dsNullValue = value;

                _state.ChangeFlags(BindingStates.DataSourceNullValueSet, true);

                // If control's property is capable of displaying a special value for DBNull,
                // and the DBNull status of data source's property has changed, force the
                // control property to refresh itself from the data source property.
                if (IsBinding)
                {
                    object? dsValue = _bindToObject.GetValue();

                    // Check previous DataSourceNullValue for null
                    if (Formatter.IsNullData(dsValue, oldValue))
                    {
                        // Update DataSource Value to new DataSourceNullValue
                        WriteValue();
                    }

                    // Check current DataSourceNullValue
                    if (Formatter.IsNullData(dsValue, value))
                    {
                        // Update Control because the DataSource is now null
                        ReadValue();
                    }
                }
            }
        }
    }

    [DefaultValue(ControlUpdateMode.OnPropertyChanged)]
    public ControlUpdateMode ControlUpdateMode
    {
        get => _controlUpdateMode;
        set
        {
            if (_controlUpdateMode != value)
            {
                _controlUpdateMode = value;

                // Refresh the control from the data source, to reflect the new update mode
                if (IsBinding)
                {
                    PushData();
                }
            }
        }
    }

    [DefaultValue(DataSourceUpdateMode.OnValidation)]
    public DataSourceUpdateMode DataSourceUpdateMode { get; set; } = DataSourceUpdateMode.OnValidation;

    private void BindTarget(bool bind)
    {
        if (BindableComponent is null)
        {
            return;
        }

        if (bind)
        {
            if (IsBinding)
            {
                if (_propInfo is not null)
                {
                    EventHandler handler = new(Target_PropertyChanged);
                    _propInfo.AddValueChanged(BindableComponent, handler);
                }

                if (_validateInfo is not null)
                {
                    CancelEventHandler handler = new(Target_Validate);
                    _validateInfo.AddEventHandler(BindableComponent, handler);
                }
            }
        }
        else
        {
            if (_propInfo is not null)
            {
                EventHandler handler = new(Target_PropertyChanged);
                _propInfo.RemoveValueChanged(BindableComponent, handler);
            }

            if (_validateInfo is not null)
            {
                CancelEventHandler handler = new(Target_Validate);
                _validateInfo.RemoveEventHandler(BindableComponent, handler);
            }
        }
    }

    private void binding_MetaDataChanged(object? sender, EventArgs e)
    {
        Debug.Assert(sender == _bindingManagerBase, "we should only receive notification from our binding manager base");
        CheckBinding();
    }

    private void CheckBinding()
    {
        _bindToObject.CheckBinding();

        if (BindableComponent is not null && !string.IsNullOrEmpty(PropertyName))
        {
            BindableComponent.DataBindings.CheckDuplicates(this);

            Type controlClass = BindableComponent.GetType();

            // Check Properties
            string propertyNameIsNull = PropertyName + "IsNull";
            PropertyDescriptor? tempPropInfo = null;
            PropertyDescriptor? tempPropIsNullInfo = null;
            PropertyDescriptorCollection propInfos;

            // If the control is being inherited, then get the properties for
            // the control's type rather than for the control itself. Getting
            // properties for the control will merge the control's properties with
            // those of its designer. Normally we want that, but for
            // inherited controls we don't because an inherited control should
            // "act" like a runtime control.
            InheritanceAttribute? attr = (InheritanceAttribute?)TypeDescriptor.GetAttributes(BindableComponent)[typeof(InheritanceAttribute)];
            if (attr is not null && attr.InheritanceLevel != InheritanceLevel.NotInherited)
            {
                propInfos = TypeDescriptor.GetProperties(controlClass);
            }
            else
            {
                propInfos = TypeDescriptor.GetProperties(BindableComponent);
            }

            for (int i = 0; i < propInfos.Count; i++)
            {
                if (tempPropInfo is null && string.Equals(propInfos[i].Name, PropertyName, StringComparison.OrdinalIgnoreCase))
                {
                    tempPropInfo = propInfos[i];
                    if (tempPropIsNullInfo is not null)
                    {
                        break;
                    }
                }

                if (tempPropIsNullInfo is null && string.Equals(propInfos[i].Name, propertyNameIsNull, StringComparison.OrdinalIgnoreCase))
                {
                    tempPropIsNullInfo = propInfos[i];
                    if (tempPropInfo is not null)
                    {
                        break;
                    }
                }
            }

            if (tempPropInfo is null)
            {
                throw new ArgumentException(string.Format(SR.ListBindingBindProperty, PropertyName), nameof(PropertyName));
            }

            if (tempPropInfo.IsReadOnly && _controlUpdateMode != ControlUpdateMode.Never)
            {
                throw new ArgumentException(string.Format(SR.ListBindingBindPropertyReadOnly, PropertyName), nameof(PropertyName));
            }

            _propInfo = tempPropInfo;
            _propInfoConverter = _propInfo.Converter;

            if (tempPropIsNullInfo is not null && tempPropIsNullInfo.PropertyType == typeof(bool) && !tempPropIsNullInfo.IsReadOnly)
            {
                _propIsNullInfo = tempPropIsNullInfo;
            }

            // Check events
            EventDescriptor? tempValidateInfo = null;
            string validateName = "Validating";
            EventDescriptorCollection eventInfos = TypeDescriptor.GetEvents(BindableComponent);
            for (int i = 0; i < eventInfos.Count; i++)
            {
                if (tempValidateInfo is null && string.Equals(eventInfos[i]!.Name, validateName, StringComparison.OrdinalIgnoreCase))
                {
                    tempValidateInfo = eventInfos[i];
                    break;
                }
            }

            _validateInfo = tempValidateInfo;
        }
        else
        {
            _propInfo = null;
            _validateInfo = null;
        }

        // go see if we become bound now.
        UpdateIsBinding();
    }

    internal bool ControlAtDesignTime()
    {
        if (BindableComponent is not IComponent comp)
        {
            return false;
        }

        return comp.Site?.DesignMode ?? false;
    }

    private object? GetDataSourceNullValue(Type? type) =>
        _state.HasFlag(BindingStates.DataSourceNullValueSet) ? _dsNullValue : Formatter.GetDefaultDataSourceNullValue(type);

    private object? GetPropValue()
    {
        if (_propIsNullInfo is not null && (bool?)_propIsNullInfo.GetValue(BindableComponent) == true)
        {
            return DataSourceNullValue;
        }

        return _propInfo?.GetValue(BindableComponent) ?? DataSourceNullValue;
    }

    private BindingCompleteEventArgs CreateBindingCompleteEventArgs(BindingCompleteContext context, Exception? ex)
    {
        bool cancel = false;
        string errorText;
        BindingCompleteState state = BindingCompleteState.Success;

        if (ex is not null)
        {
            // If an exception was provided, report that
            errorText = ex.Message;
            state = BindingCompleteState.Exception;
            cancel = true;
        }
        else
        {
            // If data error info on data source for this binding, report that
            errorText = _bindToObject.DataErrorText;

            // We should not cancel with an IDataErrorInfo error - we didn't in Everett
            if (!string.IsNullOrEmpty(errorText))
            {
                state = BindingCompleteState.DataError;
            }
        }

        return new BindingCompleteEventArgs(this, state, context, errorText, ex, cancel);
    }

    protected virtual void OnBindingComplete(BindingCompleteEventArgs e)
    {
        if (!_state.HasFlag(BindingStates.InOnBindingComplete))
        {
            try
            {
                _state.ChangeFlags(BindingStates.InOnBindingComplete, true);
                _onComplete?.Invoke(this, e);
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
                // BindingComplete event is intended primarily as an "FYI" event with support for cancellation.
                // User code should not be throwing exceptions from this event as a way to signal new error conditions (they should use
                // things like the Format or Parse events for that). Exceptions thrown here can mess up currency manager behavior big time.
                // For now, eat any non-critical exceptions and instead just cancel the current push/pull operation.
                if (e is not null)
                {
                    e.Cancel = true;
                }
            }
            finally
            {
                _state.ChangeFlags(BindingStates.InOnBindingComplete, false);
            }
        }
    }

    protected virtual void OnParse(ConvertEventArgs cevent)
    {
        _onParse?.Invoke(this, cevent);

        if (!_state.HasFlag(BindingStates.FormattingEnabled) && cevent is not null)
        {
            if (!(cevent.Value is DBNull) && cevent.Value is not null && cevent.DesiredType is not null && !cevent.DesiredType.IsInstanceOfType(cevent.Value) && (cevent.Value is IConvertible))
            {
                cevent.Value = Convert.ChangeType(cevent.Value, cevent.DesiredType, CultureInfo.CurrentCulture);
            }
        }
    }

    protected virtual void OnFormat(ConvertEventArgs cevent)
    {
        _onFormat?.Invoke(this, cevent);

        if (!_state.HasFlag(BindingStates.FormattingEnabled) && cevent is not null)
        {
            if (!(cevent.Value is DBNull) && cevent.DesiredType is not null && !cevent.DesiredType.IsInstanceOfType(cevent.Value) && (cevent.Value is IConvertible))
            {
                cevent.Value = Convert.ChangeType(cevent.Value, cevent.DesiredType, CultureInfo.CurrentCulture);
            }
        }
    }

    private object? ParseObject(object? value)
    {
        Type? type = _bindToObject.BindToType;
        Debug.Assert(type is not null);
        if (_state.HasFlag(BindingStates.FormattingEnabled))
        {
            // Fire the Parse event so that user code gets a chance to supply the parsed value for us
            ConvertEventArgs e = new(value, type);
            OnParse(e);

            object? newValue = e.Value;
            if (!Equals(value, newValue))
            {
                // If event handler replaced formatted value with parsed value, use that
                return newValue;
            }
            else
            {
                // Otherwise parse the formatted value ourselves
                TypeConverter? fieldInfoConverter = null;
                if (_bindToObject.FieldInfo is not null)
                {
                    fieldInfoConverter = _bindToObject.FieldInfo.Converter;
                }

                return Formatter.ParseObject(
                    value,
                    type,
                    (value is null ? _propInfo!.PropertyType : value.GetType()),
                    fieldInfoConverter,
                    _propInfoConverter,
                    _formatInfo,
                    _nullValue,
                    GetDataSourceNullValue(type));
            }
        }
        else
        {
            ConvertEventArgs e = new(value, type);

            // First try: use the OnParse event
            OnParse(e);
            if (e.Value is not null && (e.Value.GetType().IsSubclassOf(type) || e.Value.GetType() == type || e.Value is DBNull))
            {
                return e.Value;
            }

            // Second try: use the TypeConverter
            TypeConverter typeConverter = TypeDescriptor.GetConverter(value is not null ? value.GetType() : typeof(object));
            if (typeConverter is not null && typeConverter.CanConvertTo(type))
            {
                return typeConverter.ConvertTo(value, type);
            }

            // Last try: use Convert.ToType
            if (value is IConvertible)
            {
                object ret = Convert.ChangeType(value, type, CultureInfo.CurrentCulture);
                if (ret is not null && (ret.GetType().IsSubclassOf(type) || ret.GetType() == type))
                {
                    return ret;
                }
            }

            return null;
        }
    }

    private object? FormatObject(object? value)
    {
        // We will not format the object when the control is in design time.
        // This is because if we bind a boolean property on a control to a
        // row that is full of DBNulls then we cause problems in the shell.
        if (ControlAtDesignTime())
        {
            return value;
        }

        Type type = _propInfo!.PropertyType;
        if (_state.HasFlag(BindingStates.FormattingEnabled))
        {
            // Fire the Format event so that user code gets a chance to supply the formatted value for us
            ConvertEventArgs e = new(value, type);
            OnFormat(e);

            if (e.Value != value)
            {
                // If event handler replaced parsed value with formatted value, use that
                return e.Value;
            }
            else
            {
                // Otherwise format the parsed value ourselves
                TypeConverter? fieldInfoConverter = null;
                if (_bindToObject.FieldInfo is not null)
                {
                    fieldInfoConverter = _bindToObject.FieldInfo.Converter;
                }

                return Formatter.FormatObject(value, type, fieldInfoConverter, _propInfoConverter, _formatString, _formatInfo, _nullValue, _dsNullValue);
            }
        }
        else
        {
            // first try: use the Format event
            ConvertEventArgs e = new(value, type);
            OnFormat(e);
            object? ret = e.Value;

            // Fire the Format event even if the control property is of type Object.
            if (type == typeof(object))
            {
                return value;
            }

            // stop now if we have a value of a compatible type
            if (ret is not null && (ret.GetType().IsSubclassOf(type) || ret.GetType() == type))
            {
                return ret;
            }

            // second try: use type converter for the desiredType
            TypeConverter typeConverter = TypeDescriptor.GetConverter(value is not null ? value.GetType() : typeof(object));
            if (typeConverter is not null && typeConverter.CanConvertTo(type))
            {
                return typeConverter.ConvertTo(value, type);
            }

            // last try: use Convert.ChangeType
            if (value is IConvertible)
            {
                ret = Convert.ChangeType(value, type, CultureInfo.CurrentCulture);
                if (ret is not null && (ret.GetType().IsSubclassOf(type) || ret.GetType() == type))
                {
                    return ret;
                }
            }

            throw new FormatException(SR.ListBindingFormatFailed);
        }
    }

    /// <summary>
    ///  Pulls data from control property into data source. Returns bool indicating whether caller
    ///  should cancel the higher level operation. Raises a BindingComplete event regardless of
    ///  success or failure.
    ///
    ///  When the user leaves the control, it will raise a Validating event, calling the Binding.Target_Validate
    ///  method, which in turn calls PullData. PullData is also called by the binding manager when pulling data
    ///  from all bounds properties in one go.
    /// </summary>
    internal bool PullData() => PullData(reformat: true, force: false);

    internal bool PullData(bool reformat) => PullData(reformat, force: false);

    internal bool PullData(bool reformat, bool force)
    {
        // Don't update the control if the control update mode is never.
        if (ControlUpdateMode == ControlUpdateMode.Never)
        {
            reformat = false;
        }

        bool parseFailed = false;
        object? parsedValue = null;
        Exception? lastException = null;

        // Check whether binding has been suspended or is simply not possible right now
        if (!IsBinding)
        {
            return false;
        }

        // If caller is not FORCING us to pull, determine whether we want to pull right now...
        if (!force)
        {
            // If control property supports change events, only pull if the value has been changed since
            // the last update (ie. its dirty). For properties that do NOT support change events, we cannot
            // track the dirty state, so we just have to pull all the time.
            if (_propInfo!.SupportsChangeEvents && !_state.HasFlag(BindingStates.Modified))
            {
                return false;
            }

            // Don't pull if the update mode is 'Never' (ie. read-only binding)
            if (DataSourceUpdateMode == DataSourceUpdateMode.Never)
            {
                return false;
            }
        }

        // Re-entrancy check between push and pull (new for Whidbey - requires FormattingEnabled)
        if (_state.HasFlag(BindingStates.InPushOrPull) && _state.HasFlag(BindingStates.FormattingEnabled))
        {
            return false;
        }

        _state.ChangeFlags(BindingStates.InPushOrPull, true);

        // Get the value from the bound control property
        object? value = GetPropValue();

        // Attempt to parse the property value into a format suitable for the data source
        try
        {
            parsedValue = ParseObject(value);
        }
        catch (Exception ex)
        {
            // Eat parsing exceptions.
            lastException = ex;
        }

        try
        {
            // If parse failed, reset control property value back to original data source value.
            // An exception always indicates a parsing failure.
            if (lastException is not null || (!FormattingEnabled && parsedValue is null))
            {
                parseFailed = true;
                parsedValue = _bindToObject.GetValue();
            }

            // Format the parsed value to be re-displayed in the control
            if (reformat)
            {
                if (FormattingEnabled && parseFailed)
                {
                    // If parsing fails, do NOT push the original data source value back
                    // into the control.
                }
                else
                {
                    object? formattedObject = FormatObject(parsedValue);
                    if (force || !FormattingEnabled || !Equals(formattedObject, value))
                    {
                        SetPropValue(formattedObject);
                    }
                }
            }

            // Put the value into the data model
            if (!parseFailed)
            {
                _bindToObject.SetValue(parsedValue);
            }
        }
        catch (Exception ex) when (FormattingEnabled)
        {
            // Throw the exception unless this binding has formatting enabled
            lastException = ex;
        }
        finally
        {
            _state.ChangeFlags(BindingStates.InPushOrPull, false);
        }

        if (FormattingEnabled)
        {
            // Raise the BindingComplete event, giving listeners a chance to process any
            // errors that occurred and decide whether the operation should be cancelled.
            BindingCompleteEventArgs args = CreateBindingCompleteEventArgs(BindingCompleteContext.DataSourceUpdate, lastException);
            OnBindingComplete(args);

            // If the operation completed successfully (and was not cancelled), we can clear the dirty flag
            // on this binding because we know the value in the control was valid and has been accepted by
            // the data source. But if the operation failed (or was cancelled), we must leave the dirty flag
            // alone, so that the control's value will continue to be re-validated and re-pulled later.
            if (args.BindingCompleteState == BindingCompleteState.Success && !args.Cancel)
            {
                _state.ChangeFlags(BindingStates.Modified, false);
            }

            return args.Cancel;
        }
        else
        {
            // Do not emit BindingComplete events, or allow the operation to be cancelled.
            // If we get this far, treat the operation as successful and clear the dirty flag.
            _state.ChangeFlags(BindingStates.Modified, false);
            return false;
        }
    }

    /// <summary>
    ///  Pushes data from data source into control property. Returns bool indicating whether caller
    ///  should cancel the higher level operation. Raises a BindingComplete event regardless of
    ///  success or failure.
    /// </summary>
    internal bool PushData() => PushData(force: false);

    internal bool PushData(bool force)
    {
        object? dataSourceValue;
        Exception? lastException = null;

        // Don't push if update mode is 'Never' (unless caller is FORCING us to push)
        if (!force && ControlUpdateMode == ControlUpdateMode.Never)
        {
            return false;
        }

        // Re-entrancy check between push and pull
        if (_state.HasFlag(BindingStates.InPushOrPull) && _state.HasFlag(BindingStates.FormattingEnabled))
        {
            return false;
        }

        _state.ChangeFlags(BindingStates.InPushOrPull, true);

        try
        {
            if (IsBinding)
            {
                dataSourceValue = _bindToObject.GetValue();
                object? controlValue = FormatObject(dataSourceValue);
                SetPropValue(controlValue);
                _state.ChangeFlags(BindingStates.Modified, false);
            }
            else
            {
                SetPropValue(null);
            }
        }
        catch (Exception ex) when (FormattingEnabled)
        {
            // Re-throw the exception unless this binding has formatting enabled
            lastException = ex;
        }
        finally
        {
            _state.ChangeFlags(BindingStates.InPushOrPull, false);
        }

        if (FormattingEnabled)
        {
            // Raise the BindingComplete event, giving listeners a chance to process any errors that occurred, and decide
            // whether the operation should be cancelled. But don't emit the event if we didn't actually update the control.
            BindingCompleteEventArgs args = CreateBindingCompleteEventArgs(BindingCompleteContext.ControlUpdate, lastException);
            OnBindingComplete(args);

            return args.Cancel;
        }
        else
        {
            // Do not emit BindingComplete events, or allow the operation to be cancelled.
            return false;
        }
    }

    /// <summary>
    ///  Reads current value from data source, and sends this to the control.
    /// </summary>
    public void ReadValue() => PushData(force: true);

    /// <summary>
    ///  Takes current value from control, and writes this out to the data source.
    /// </summary>
    public void WriteValue() => PullData(reformat: true, force: true);

    private void SetPropValue(object? value)
    {
        // we will not pull the data from the back end into the control
        // when the control is in design time. this is because if we bind a boolean property on a control
        // to a row that is full of DBNulls then we cause problems in the shell.
        if (ControlAtDesignTime())
        {
            return;
        }

        _state.ChangeFlags(BindingStates.InSetPropValue, true);

        try
        {
            bool isNull = value is null || Formatter.IsNullData(value, DataSourceNullValue);
            if (isNull)
            {
                if (_propIsNullInfo is not null)
                {
                    _propIsNullInfo.SetValue(BindableComponent, true);
                }
                else if (_propInfo is not null)
                {
                    if (_propInfo.PropertyType == typeof(object))
                    {
                        _propInfo.SetValue(BindableComponent, DataSourceNullValue);
                    }
                    else
                    {
                        _propInfo.SetValue(BindableComponent, null);
                    }
                }
            }
            else
            {
                _propInfo!.SetValue(BindableComponent, value);
            }
        }
        finally
        {
            _state.ChangeFlags(BindingStates.InSetPropValue, false);
        }
    }

    private bool ShouldSerializeFormatString() => !string.IsNullOrEmpty(_formatString);

    private bool ShouldSerializeNullValue() => _nullValue is not null;

    private bool ShouldSerializeDataSourceNullValue() =>
       _state.HasFlag(BindingStates.DataSourceNullValueSet) && _dsNullValue != Formatter.GetDefaultDataSourceNullValue(null);

    private void Target_PropertyChanged(object? sender, EventArgs e)
    {
        if (_state.HasFlag(BindingStates.InSetPropValue))
        {
            return;
        }

        if (IsBinding)
        {
            _state.ChangeFlags(BindingStates.Modified, true);

            // If required, update data source every time control property changes.
            // NOTE: We need modified=true to be set both before pulling data
            // (so that pull will work) and afterwards (so that validation will
            // still occur later on).
            if (DataSourceUpdateMode == DataSourceUpdateMode.OnPropertyChanged)
            {
                PullData(reformat: false);
                _state.ChangeFlags(BindingStates.Modified, true);
            }
        }
    }

    /// <summary>
    ///  Event handler for the Control.Validating event on the control that we are bound to.
    ///
    ///  If value in control has changed, we want to send that value back up to the data source
    ///  when the control undergoes validation (eg. on loss of focus). If an error occurs, we
    ///  will set e.Cancel=true to make validation fail and force focus to remain on the control.
    ///
    ///  NOTE: If no error occurs, we MUST leave e.Cancel alone, to respect any value put in there
    ///  by event handlers high up the event chain.
    /// </summary>
    private void Target_Validate(object? sender, CancelEventArgs e)
    {
        try
        {
            if (PullData(true))
            {
                e.Cancel = true;
            }
        }
        catch
        {
            e.Cancel = true;
        }
    }

    [MemberNotNullWhen(true, nameof(_bindingManagerBase))]
    internal bool IsBindable =>
        BindableComponent is not null
        && !string.IsNullOrEmpty(PropertyName)
        && DataSource is not null
        && _bindingManagerBase is not null;

    internal void UpdateIsBinding()
    {
        bool newBound = IsBindable && ComponentCreated && _bindingManagerBase.IsBinding;
        if (IsBinding != newBound)
        {
            IsBinding = newBound;
            BindTarget(newBound);
            if (IsBinding)
            {
                if (_controlUpdateMode == ControlUpdateMode.Never)
                {
                    PullData(reformat: false, force: true);
                }
                else
                {
                    PushData();
                }
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a simple binding of a value in a list and the property of a control.
    /// </summary>
    [TypeConverter(typeof(ListBindingConverter))]
    public class Binding
    {
        private BindingManagerBase _bindingManagerBase;

        private readonly BindToObject _bindToObject;

        private PropertyDescriptor _propInfo;
        private PropertyDescriptor _propIsNullInfo;
        private EventDescriptor _validateInfo;
        private TypeConverter _propInfoConverter;

        private bool _formattingEnabled;
        private bool _modified;

        // Recursion guards
        private bool _inSetPropValue;
        private bool _inPushOrPull;
        private bool _inOnBindingComplete;

        // formatting stuff
        private string _formatString = string.Empty;
        private IFormatProvider _formatInfo;
        private object _nullValue;
        private object _dsNullValue = Formatter.GetDefaultDataSourceNullValue(null);
        private bool _dsNullValueSet;
        private ConvertEventHandler _onParse;
        private ConvertEventHandler _onFormat;

        // binding stuff
        private ControlUpdateMode _controlUpdateMode = ControlUpdateMode.OnPropertyChanged;
        private BindingCompleteEventHandler _onComplete;

        /// <summary>
        ///  Initializes a new instance of the <see cref='Binding'/> class
        ///  that binds a property on the owning control to a property on a data source.
        /// </summary>
        public Binding(string propertyName, object dataSource, string dataMember) : this(propertyName, dataSource, dataMember, false, 0, null, string.Empty, null)
        {
        }

        public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled) : this(propertyName, dataSource, dataMember, formattingEnabled, 0, null, string.Empty, null)
        {
        }

        public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode) : this(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode, null, string.Empty, null)
        {
        }

        public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue) : this(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode, nullValue, string.Empty, null)
        {
        }

        public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue, string formatString) : this(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode, nullValue, formatString, null)
        {
        }

        public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue, string formatString, IFormatProvider formatInfo)
        {
            DataSource = dataSource;
            BindingMemberInfo = new BindingMemberInfo(dataMember);
            _bindToObject = new BindToObject(this);

            PropertyName = propertyName;
            _formattingEnabled = formattingEnabled;
            _formatString = formatString;
            _nullValue = nullValue;
            _formatInfo = formatInfo;
            DataSourceUpdateMode = dataSourceUpdateMode;

            CheckBinding();
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='Binding'/> class.
        /// </summary>
        private Binding()
        {
        }

        public object DataSource { get; }

        public BindingMemberInfo BindingMemberInfo { get; }

        /// <summary>
        ///  Gets the control to which the binding belongs.
        /// </summary>
        [DefaultValue(null)]
        public IBindableComponent BindableComponent { get; private set; }

        /// <summary>
        ///  Gets the control to which the binding belongs.
        /// </summary>
        [DefaultValue(null)]
        public Control Control => BindableComponent as Control;

        /// <summary>
        ///  Is the binadable component in a 'created' (ready-to-use) state? For controls,
        ///  this depends on whether the window handle has been created yet. For everything
        ///  else, we'll assume they are always in a created state.
        /// </summary>
        internal static bool IsComponentCreated(IBindableComponent component)
        {
            return !(component is Control control) || control.Created;
        }

        /// <summary>
        ///  Instance-specific property equivalent to the static method above
        /// </summary>
        internal bool ComponentCreated => IsComponentCreated(BindableComponent);

        private void FormLoaded(object sender, EventArgs e)
        {
            Debug.Assert(sender == BindableComponent, "which other control can send us the Load event?");
            // update the binding
            CheckBinding();
        }

        internal void SetBindableComponent(IBindableComponent value)
        {
            if (BindableComponent != value)
            {
                IBindableComponent oldTarget = BindableComponent;
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
                BindingContext.UpdateBinding((BindableComponent != null && IsComponentCreated(BindableComponent) ? BindableComponent.BindingContext : null), this);
                if (value is Form form)
                {
                    form.Load += new EventHandler(FormLoaded);
                }
            }
        }

        /// <summary>
        ///  Gets a value indicating whether the binding is active.
        /// </summary>
        public bool IsBinding { get; private set; }

        /// <summary>
        ///  Gets the <see cref='Forms.BindingManagerBase'/> of this binding that
        ///  allows enumeration of a set of bindings.
        /// </summary>
        public BindingManagerBase BindingManagerBase
        {
            get => _bindingManagerBase;
            internal set
            {
                if (_bindingManagerBase != value)
                {
                    if (_bindingManagerBase is CurrencyManager oldCurrencyManager)
                    {
                        oldCurrencyManager.MetaDataChanged -= new EventHandler(binding_MetaDataChanged);
                    }

                    _bindingManagerBase = value;

                    if (value is CurrencyManager newCurrencyManager)
                    {
                        newCurrencyManager.MetaDataChanged += new EventHandler(binding_MetaDataChanged);
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

        public event BindingCompleteEventHandler BindingComplete
        {
            add => _onComplete += value;
            remove => _onComplete -= value;
        }

        public event ConvertEventHandler Parse
        {
            add => _onParse += value;
            remove => _onParse -= value;
        }

        public event ConvertEventHandler Format
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
            get => _formattingEnabled;
            set
            {
                if (_formattingEnabled != value)
                {
                    _formattingEnabled = value;
                    if (IsBinding)
                    {
                        PushData();
                    }
                }
            }
        }

        [DefaultValue(null)]
        public IFormatProvider FormatInfo
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
                if (value is null)
                {
                    value = string.Empty;
                }

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

        public object NullValue
        {
            get => _nullValue;
            set
            {
                // Try to compare logical values, not object references...
                if (!object.Equals(_nullValue, value))
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

        public object DataSourceNullValue
        {
            get => _dsNullValue;
            set
            {
                // Try to compare logical values, not object references...
                if (!object.Equals(_dsNullValue, value))
                {
                    // Save old Value
                    object oldValue = _dsNullValue;

                    // Set value
                    _dsNullValue = value;

                    _dsNullValueSet = true;

                    // If control's property is capable of displaying a special value for DBNull,
                    // and the DBNull status of data source's property has changed, force the
                    // control property to refresh itself from the data source property.
                    if (IsBinding)
                    {
                        object dsValue = _bindToObject.GetValue();

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
            if (bind)
            {
                if (IsBinding)
                {
                    if (_propInfo != null && BindableComponent != null)
                    {
                        EventHandler handler = new EventHandler(Target_PropertyChanged);
                        _propInfo.AddValueChanged(BindableComponent, handler);
                    }
                    if (_validateInfo != null)
                    {
                        CancelEventHandler handler = new CancelEventHandler(Target_Validate);
                        _validateInfo.AddEventHandler(BindableComponent, handler);
                    }
                }
            }
            else
            {
                if (_propInfo != null && BindableComponent != null)
                {
                    EventHandler handler = new EventHandler(Target_PropertyChanged);
                    _propInfo.RemoveValueChanged(BindableComponent, handler);
                }
                if (_validateInfo != null)
                {
                    CancelEventHandler handler = new CancelEventHandler(Target_Validate);
                    _validateInfo.RemoveEventHandler(BindableComponent, handler);
                }
            }
        }

        private void binding_MetaDataChanged(object sender, EventArgs e)
        {
            Debug.Assert(sender == _bindingManagerBase, "we should only receive notification from our binding manager base");
            CheckBinding();
        }

        private void CheckBinding()
        {
            _bindToObject.CheckBinding();

            if (BindableComponent != null && !string.IsNullOrEmpty(PropertyName))
            {
                BindableComponent.DataBindings.CheckDuplicates(this);

                Type controlClass = BindableComponent.GetType();

                // Check Properties
                string propertyNameIsNull = PropertyName + "IsNull";
                Type propType = null;
                PropertyDescriptor tempPropInfo = null;
                PropertyDescriptor tempPropIsNullInfo = null;
                PropertyDescriptorCollection propInfos;

                // If the control is being inherited, then get the properties for
                // the control's type rather than for the control itself.  Getting
                // properties for the control will merge the control's properties with
                // those of its designer.  Normally we want that, but for
                // inherited controls we don't because an inherited control should
                // "act" like a runtime control.
                InheritanceAttribute attr = (InheritanceAttribute)TypeDescriptor.GetAttributes(BindableComponent)[typeof(InheritanceAttribute)];
                if (attr != null && attr.InheritanceLevel != InheritanceLevel.NotInherited)
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
                        if (tempPropIsNullInfo != null)
                        {
                            break;
                        }
                    }
                    if (tempPropIsNullInfo is null && string.Equals(propInfos[i].Name, propertyNameIsNull, StringComparison.OrdinalIgnoreCase))
                    {
                        tempPropIsNullInfo = propInfos[i];
                        if (tempPropInfo != null)
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
                propType = _propInfo.PropertyType;
                _propInfoConverter = _propInfo.Converter;

                if (tempPropIsNullInfo != null && tempPropIsNullInfo.PropertyType == typeof(bool) && !tempPropIsNullInfo.IsReadOnly)
                {
                    _propIsNullInfo = tempPropIsNullInfo;
                }

                // Check events
                EventDescriptor tempValidateInfo = null;
                string validateName = "Validating";
                EventDescriptorCollection eventInfos = TypeDescriptor.GetEvents(BindableComponent);
                for (int i = 0; i < eventInfos.Count; i++)
                {
                    if (tempValidateInfo is null && string.Equals(eventInfos[i].Name, validateName, StringComparison.OrdinalIgnoreCase))
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
            if (!(BindableComponent is IComponent comp))
            {
                return false;
            }

            return comp.Site?.DesignMode ?? false;
        }

        private object GetDataSourceNullValue(Type type)
        {
            return _dsNullValueSet ? _dsNullValue : Formatter.GetDefaultDataSourceNullValue(type);
        }

        private object GetPropValue()
        {
            if (_propIsNullInfo != null && (bool)_propIsNullInfo.GetValue(BindableComponent))
            {
                return DataSourceNullValue;
            }

            return _propInfo.GetValue(BindableComponent) ?? DataSourceNullValue;
        }

        private BindingCompleteEventArgs CreateBindingCompleteEventArgs(BindingCompleteContext context, Exception ex)
        {
            bool cancel = false;
            string errorText = string.Empty;
            BindingCompleteState state = BindingCompleteState.Success;

            if (ex != null)
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
            if (!_inOnBindingComplete)
            {
                try
                {
                    _inOnBindingComplete = true;
                    _onComplete?.Invoke(this, e);
                }
                catch (Exception ex) when (!ClientUtils.IsCriticalException(ex))
                {
                    // BindingComplete event is intended primarily as an "FYI" event with support for cancellation.
                    // User code should not be throwing exceptions from this event as a way to signal new error conditions (they should use
                    // things like the Format or Parse events for that). Exceptions thrown here can mess up currency manager behavior big time.
                    // For now, eat any non-critical exceptions and instead just cancel the current push/pull operation.
                    if (e != null)
                    {
                        e.Cancel = true;
                    }
                }
                finally
                {
                    _inOnBindingComplete = false;
                }
            }
        }

        protected virtual void OnParse(ConvertEventArgs cevent)
        {
            _onParse?.Invoke(this, cevent);

            if (!_formattingEnabled && cevent != null)
            {
                if (!(cevent.Value is DBNull) && cevent.Value != null && cevent.DesiredType != null && !cevent.DesiredType.IsInstanceOfType(cevent.Value) && (cevent.Value is IConvertible))
                {
                    cevent.Value = Convert.ChangeType(cevent.Value, cevent.DesiredType, CultureInfo.CurrentCulture);
                }
            }
        }

        protected virtual void OnFormat(ConvertEventArgs cevent)
        {
            _onFormat?.Invoke(this, cevent);

            if (!_formattingEnabled && cevent != null)
            {
                if (!(cevent.Value is DBNull) && cevent.DesiredType != null && !cevent.DesiredType.IsInstanceOfType(cevent.Value) && (cevent.Value is IConvertible))
                {
                    cevent.Value = Convert.ChangeType(cevent.Value, cevent.DesiredType, CultureInfo.CurrentCulture);
                }
            }
        }

        private object ParseObject(object value)
        {
            Type type = _bindToObject.BindToType;
            if (_formattingEnabled)
            {
                // Fire the Parse event so that user code gets a chance to supply the parsed value for us
                var e = new ConvertEventArgs(value, type);
                OnParse(e);

                object newValue = e.Value;
                if (!object.Equals(value, newValue))
                {
                    // If event handler replaced formatted value with parsed value, use that
                    return newValue;
                }
                else
                {
                    // Otherwise parse the formatted value ourselves
                    TypeConverter fieldInfoConverter = null;
                    if (_bindToObject.FieldInfo != null)
                    {
                        fieldInfoConverter = _bindToObject.FieldInfo.Converter;
                    }

                    return Formatter.ParseObject(value, type, (value is null ? _propInfo.PropertyType : value.GetType()), fieldInfoConverter, _propInfoConverter, _formatInfo, _nullValue, GetDataSourceNullValue(type));
                }
            }
            else
            {
                var e = new ConvertEventArgs(value, type);
                // first try: use the OnParse event
                OnParse(e);
                if (e.Value != null && (e.Value.GetType().IsSubclassOf(type) || e.Value.GetType() == type || e.Value is DBNull))
                {
                    return e.Value;
                }

                // second try: use the TypeConverter
                TypeConverter typeConverter = TypeDescriptor.GetConverter(value != null ? value.GetType() : typeof(object));
                if (typeConverter != null && typeConverter.CanConvertTo(type))
                {
                    return typeConverter.ConvertTo(value, type);
                }
                // last try: use Convert.ToType
                if (value is IConvertible)
                {
                    object ret = Convert.ChangeType(value, type, CultureInfo.CurrentCulture);
                    if (ret != null && (ret.GetType().IsSubclassOf(type) || ret.GetType() == type))
                    {
                        return ret;
                    }
                }

                return null;
            }
        }

        private object FormatObject(object value)
        {
            // We will not format the object when the control is in design time.
            // This is because if we bind a boolean property on a control to a
            // row that is full of DBNulls then we cause problems in the shell.
            if (ControlAtDesignTime())
            {
                return value;
            }

            Type type = _propInfo.PropertyType;
            if (_formattingEnabled)
            {
                // Fire the Format event so that user code gets a chance to supply the formatted value for us
                var e = new ConvertEventArgs(value, type);
                OnFormat(e);

                if (e.Value != value)
                {
                    // If event handler replaced parsed value with formatted value, use that
                    return e.Value;
                }
                else
                {
                    // Otherwise format the parsed value ourselves
                    TypeConverter fieldInfoConverter = null;
                    if (_bindToObject.FieldInfo != null)
                    {
                        fieldInfoConverter = _bindToObject.FieldInfo.Converter;
                    }

                    return Formatter.FormatObject(value, type, fieldInfoConverter, _propInfoConverter, _formatString, _formatInfo, _nullValue, _dsNullValue);
                }
            }
            else
            {
                // first try: use the Format event
                var e = new ConvertEventArgs(value, type);
                OnFormat(e);
                object ret = e.Value;

                // Fire the Format event even if the control property is of type Object.
                if (type == typeof(object))
                {
                    return value;
                }

                // stop now if we have a value of a compatible type
                if (ret != null && (ret.GetType().IsSubclassOf(type) || ret.GetType() == type))
                {
                    return ret;
                }

                // second try: use type converter for the desiredType
                TypeConverter typeConverter = TypeDescriptor.GetConverter(value != null ? value.GetType() : typeof(object));
                if (typeConverter != null && typeConverter.CanConvertTo(type))
                {
                    return typeConverter.ConvertTo(value, type);
                }

                // last try: use Convert.ChangeType
                if (value is IConvertible)
                {
                    ret = Convert.ChangeType(value, type, CultureInfo.CurrentCulture);
                    if (ret != null && (ret.GetType().IsSubclassOf(type) || ret.GetType() == type))
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
            object parsedValue = null;
            Exception lastException = null;

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
                if (_propInfo.SupportsChangeEvents && !_modified)
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
            if (_inPushOrPull && _formattingEnabled)
            {
                return false;
            }

            _inPushOrPull = true;

            // Get the value from the bound control property
            object value = GetPropValue();

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
                if (lastException != null || (!FormattingEnabled && parsedValue is null))
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
                        object formattedObject = FormatObject(parsedValue);
                        if (force || !FormattingEnabled || !object.Equals(formattedObject, value))
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
                _inPushOrPull = false;
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
                if (args.BindingCompleteState == BindingCompleteState.Success && args.Cancel == false)
                {
                    _modified = false;
                }

                return args.Cancel;
            }
            else
            {
                // Do not emit BindingComplete events, or allow the operation to be cancelled.
                // If we get this far, treat the operation as successful and clear the dirty flag.
                _modified = false;
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
            object dataSourceValue = null;
            Exception lastException = null;

            // Don't push if update mode is 'Never' (unless caller is FORCING us to push)
            if (!force && ControlUpdateMode == ControlUpdateMode.Never)
            {
                return false;
            }

            // Re-entrancy check between push and pull
            if (_inPushOrPull && _formattingEnabled)
            {
                return false;
            }

            _inPushOrPull = true;

            try
            {
                if (IsBinding)
                {
                    dataSourceValue = _bindToObject.GetValue();
                    object controlValue = FormatObject(dataSourceValue);
                    SetPropValue(controlValue);
                    _modified = false;
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
                _inPushOrPull = false;
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

        private void SetPropValue(object value)
        {
            // we will not pull the data from the back end into the control
            // when the control is in design time. this is because if we bind a boolean property on a control
            // to a row that is full of DBNulls then we cause problems in the shell.
            if (ControlAtDesignTime())
            {
                return;
            }

            _inSetPropValue = true;

            try
            {
                bool isNull = value is null || Formatter.IsNullData(value, DataSourceNullValue);
                if (isNull)
                {
                    if (_propIsNullInfo != null)
                    {
                        _propIsNullInfo.SetValue(BindableComponent, true);
                    }
                    else if (_propInfo != null)
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
                    _propInfo.SetValue(BindableComponent, value);
                }
            }
            finally
            {
                _inSetPropValue = false;
            }
        }

        private bool ShouldSerializeFormatString() => !string.IsNullOrEmpty(_formatString);

        private bool ShouldSerializeNullValue() => _nullValue != null;

        private bool ShouldSerializeDataSourceNullValue()
        {
            return _dsNullValueSet && _dsNullValue != Formatter.GetDefaultDataSourceNullValue(null);
        }

        private void Target_PropertyChanged(object sender, EventArgs e)
        {
            if (_inSetPropValue)
            {
                return;
            }

            if (IsBinding)
            {
                _modified = true;

                // If required, update data source every time control property changes.
                // NOTE: We need modified=true to be set both before pulling data
                // (so that pull will work) and afterwards (so that validation will
                // still occur later on).
                if (DataSourceUpdateMode == DataSourceUpdateMode.OnPropertyChanged)
                {
                    PullData(reformat: false);
                    _modified = true;
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
        private void Target_Validate(object sender, CancelEventArgs e)
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

        internal bool IsBindable
        {
            get
            {
                return (BindableComponent != null && !string.IsNullOrEmpty(PropertyName) &&
                                DataSource != null && _bindingManagerBase != null);
            }
        }

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

        private class BindToObject
        {
            private BindingManagerBase _bindingManager;
            private readonly Binding _owner;
            private bool _dataSourceInitialized;
            private bool _waitingOnDataSource;

            private void PropValueChanged(object sender, EventArgs e)
            {
                _bindingManager?.OnCurrentChanged(EventArgs.Empty);
            }

            private bool IsDataSourceInitialized
            {
                get
                {
                    Debug.Assert(_owner.DataSource != null, "how can we determine if DataSource is initialized or not if we have no data source?");
                    if (_dataSourceInitialized)
                    {
                        return true;
                    }

                    if (!(_owner.DataSource is ISupportInitializeNotification ds) || ds.IsInitialized)
                    {
                        _dataSourceInitialized = true;
                        return true;
                    }

                    // We have an ISupportInitializeNotification which was not initialized yet.
                    // We already hooked up the Initialized event and the data source is not initialized yet.
                    if (_waitingOnDataSource)
                    {
                        return false;
                    }

                    // Hook up the Initialized event.
                    ds.Initialized += new EventHandler(DataSource_Initialized);
                    _waitingOnDataSource = true;
                    return false;
                }
            }

            internal BindToObject(Binding owner)
            {
                Debug.Assert(owner != null);
                _owner = owner;
                CheckBinding();
            }

            private void DataSource_Initialized(object sender, EventArgs e)
            {
                Debug.Assert(sender == _owner.DataSource, "data source should not change");
                Debug.Assert(_owner.DataSource is ISupportInitializeNotification, "data source should not change on the BindToObject");
                Debug.Assert(_waitingOnDataSource);

                // Unhook the Initialized event.
                if (_owner.DataSource is ISupportInitializeNotification ds)
                {
                    ds.Initialized -= new EventHandler(DataSource_Initialized);
                }

                // The wait is over: DataSource is initialized.
                _waitingOnDataSource = false;
                _dataSourceInitialized = true;

                // Rebind.
                CheckBinding();
            }

            internal void SetBindingManagerBase(BindingManagerBase lManager)
            {
                if (_bindingManager == lManager)
                {
                    return;
                }

                // remove notification from the backEnd
                if (_bindingManager != null && FieldInfo != null && _bindingManager.IsBinding && !(_bindingManager is CurrencyManager))
                {
                    FieldInfo.RemoveValueChanged(_bindingManager.Current, new EventHandler(PropValueChanged));
                    FieldInfo = null;
                }

                _bindingManager = lManager;
                CheckBinding();
            }

            internal string DataErrorText { get; private set; } = string.Empty;

            /// <summary>
            ///  Returns any data error info on the data source for the bound data field
            ///  in the current row
            /// </summary>
            private string GetErrorText(object value)
            {
                string text = string.Empty;

                if (value is IDataErrorInfo errorInfo)
                {
                    // Get the row error if there is no DataMember
                    if (FieldInfo is null)
                    {
                        text = errorInfo.Error;
                    }
                    // Get the column error if there is a DataMember.
                    // The DataTable uses its own Locale to lookup column names <sigh>.
                    // So passing the DataMember from the BindingField could cause problems.
                    // Pass the name from the PropertyDescriptor that the DataTable gave us.
                    // (If there is no fieldInfo, data binding would have failed already )
                    else
                    {
                        text = errorInfo[FieldInfo.Name];
                    }
                }

                return text ?? string.Empty;
            }

            internal object GetValue()
            {
                object obj = _bindingManager.Current;

                // Update IDataErrorInfo text: it's ok to get this now because we're going to need
                // this as part of the BindingCompleteEventArgs anyway.
                DataErrorText = GetErrorText(obj);

                if (FieldInfo != null)
                {
                    obj = FieldInfo.GetValue(obj);
                }

                return obj;
            }

            internal Type BindToType
            {
                get
                {
                    if (_owner.BindingMemberInfo.BindingField.Length == 0)
                    {
                        // if we are bound to a list w/o any properties, then
                        // take the type from the BindingManager
                        Type type = _bindingManager.BindType;
                        if (typeof(Array).IsAssignableFrom(type))
                        {
                            type = type.GetElementType();
                        }
                        return type;
                    }

                    return FieldInfo?.PropertyType;
                }
            }

            internal void SetValue(object value)
            {
                object obj = null;

                if (FieldInfo != null)
                {
                    obj = _bindingManager.Current;
                    if (obj is IEditableObject editableObject)
                    {
                        editableObject.BeginEdit();
                    }
                    if (!FieldInfo.IsReadOnly)
                    {
                        FieldInfo.SetValue(obj, value);
                    }
                }
                else
                {
                    if (_bindingManager is CurrencyManager cm)
                    {
                        cm[cm.Position] = value;
                        obj = value;
                    }
                }

                // Update IDataErrorInfo text.
                DataErrorText = GetErrorText(obj);
            }

            internal PropertyDescriptor FieldInfo { get; private set; }

            internal void CheckBinding()
            {
                // At design time, don't check anything.
                if (_owner.BindableComponent != null && _owner.ControlAtDesignTime())
                {
                    return;
                }

                // Remove propertyChangedNotification when this binding is deleted
                if (_bindingManager != null &&
                    FieldInfo != null &&
                    _bindingManager.IsBinding &&
                    !(_bindingManager is CurrencyManager))
                {
                    FieldInfo.RemoveValueChanged(_bindingManager.Current, new EventHandler(PropValueChanged));
                }

                if (_bindingManager != null &&
                    _owner.BindableComponent != null &&
                    _owner.ComponentCreated &&
                    IsDataSourceInitialized)
                {
                    string dataField = _owner.BindingMemberInfo.BindingField;

                    FieldInfo = _bindingManager.GetItemProperties().Find(dataField, true);
                    if (_bindingManager.DataSource != null && FieldInfo is null && dataField.Length > 0)
                    {
                        throw new ArgumentException(string.Format(SR.ListBindingBindField, dataField), "dataMember");
                    }

                    // Do not add propertyChange notification if the fieldInfo is null
                    //
                    // We add an event handler to the dataSource in the BindingManagerBase because
                    // if the binding is of the form (Control, ControlProperty, DataSource, Property1.Property2.Property3)
                    // then we want to get notification from Current.Property1.Property2 and not from DataSource
                    // when we get the backEnd notification we push the new value into the Control's property
                    if (FieldInfo != null && _bindingManager.IsBinding &&
                        !(_bindingManager is CurrencyManager))
                    {
                        FieldInfo.AddValueChanged(_bindingManager.Current, new EventHandler(PropValueChanged));
                    }
                }
                else
                {
                    FieldInfo = null;
                }
            }
        }
    }
}

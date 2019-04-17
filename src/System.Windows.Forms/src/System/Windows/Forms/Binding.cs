// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    /// Represents a simple binding of a value in a list and the property of a control.
    /// </summary>
    [TypeConverterAttribute(typeof(ListBindingConverter))]
    public class Binding
    {
        // the two collection owners that this binding belongs to.
        private IBindableComponent control;
        private BindingManagerBase bindingManagerBase;

        private BindToObject bindToObject = null;

        private string propertyName = string.Empty;

        private PropertyDescriptor propInfo;
        private PropertyDescriptor propIsNullInfo;
        private EventDescriptor validateInfo;
        private TypeConverter propInfoConverter;

        private bool formattingEnabled = false;

        private bool bound = false;
        private bool modified = false;

        // Recursion guards
        private bool inSetPropValue = false;
        private bool inPushOrPull = false;
        private bool inOnBindingComplete = false;

        // formatting stuff
        private string formatString = string.Empty;
        private IFormatProvider formatInfo = null;
        private object nullValue = null;
        private object dsNullValue = Formatter.GetDefaultDataSourceNullValue(null);
        private bool dsNullValueSet;
        private ConvertEventHandler onParse = null;
        private ConvertEventHandler onFormat = null;

        // binding stuff
        private ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged;
        private BindingCompleteEventHandler onComplete = null;

        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.Binding'/> class
        /// that binds a property on the owning control to a property on a data source.
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

        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters", Justification = "'formatString' is an appropriate name, since its a string passed to the Format method")]
        public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue, string formatString) : this(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode, nullValue, formatString, null)
        {
        }

        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "By design (no-one should be subclassing this class)")]
        [SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters", Justification = "'formatString' is an appropriate name, since its a string passed to the Format method")]
        public Binding(string propertyName, object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue, string formatString, IFormatProvider formatInfo)
        {
            bindToObject = new BindToObject(this, dataSource, dataMember);

            this.propertyName = propertyName;
            this.formattingEnabled = formattingEnabled;
            this.formatString = formatString;
            this.nullValue = nullValue;
            this.formatInfo = formatInfo;
            this.formattingEnabled = formattingEnabled;
            DataSourceUpdateMode = dataSourceUpdateMode;

            CheckBinding();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.Binding'/> class.
        /// </summary>
        private Binding()
        {
        }

        internal BindToObject BindToObject => bindToObject;

        public object DataSource => bindToObject.DataSource;

        public BindingMemberInfo BindingMemberInfo => bindToObject.BindingMemberInfo;

        /// <summary>
        /// Gets the control to which the binding belongs.
        /// </summary>
        [DefaultValue(null)]
        public IBindableComponent BindableComponent => control;

        /// <summary>
        /// Gets the control to which the binding belongs.
        /// </summary>
        [DefaultValue(null)]
        public Control Control => control as Control;

        /// <summary>
        /// Is the binadable component in a 'created' (ready-to-use) state? For controls,
        /// this depends on whether the window handle has been created yet. For everything
        /// else, we'll assume they are always in a created state.
        /// </summary>
        internal static bool IsComponentCreated(IBindableComponent component)
        {
            if (component is Control ctrl)
            {
                return ctrl.Created;
            }
            
            return true;
        }

        // <summary>
        /// Instance-specific property equivalent to the static method above
        // </summary>
        internal bool ComponentCreated => IsComponentCreated(this.control);

        private void FormLoaded(object sender, EventArgs e)
        {
            Debug.Assert(sender == control, "which other control can send us the Load event?");
            // update the binding
            CheckBinding();
        }

        internal void SetBindableComponent(IBindableComponent value)
        {
            if (control != value)
            {
                IBindableComponent oldTarget = control;
                BindTarget(false);
                control = value;
                BindTarget(true);
                try
                {
                    CheckBinding();
                }
                catch
                {
                    BindTarget(false);
                    control = oldTarget;
                    BindTarget(true);
                    throw;
                }

                // We are essentially doing to the listManager what we were doing to the
                // BindToObject: bind only when the control is created and it has a BindingContext
                BindingContext.UpdateBinding((control != null && IsComponentCreated(control) ? control.BindingContext : null), this);
                Form form = value as Form;
                if (form != null)
                {
                    form.Load += new EventHandler(FormLoaded);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the binding is active.
        /// </summary>
        public bool IsBinding => bound;

        /// <summary>
        /// Gets the <see cref='System.Windows.Forms.BindingManagerBase'/> of this binding that
        /// allows enumeration of a set of bindings.
        /// </summary>
        public BindingManagerBase BindingManagerBase => bindingManagerBase;

        internal void SetListManager(BindingManagerBase newBindingManagerBase)
        {
            if (bindingManagerBase is CurrencyManager oldCurrencyManagEr)
            {
                oldCurrencyManagEr.MetaDataChanged -= new EventHandler(binding_MetaDataChanged);
            }

            bindingManagerBase = newBindingManagerBase;

            if (newBindingManagerBase is CurrencyManager newCurrencyManager)
            {
                newCurrencyManager.MetaDataChanged += new EventHandler(binding_MetaDataChanged);
            }

            BindToObject.SetBindingManagerBase(newBindingManagerBase);
            CheckBinding();
        }

        /// <summary>
        /// Gets or sets the property on the control to bind to.
        /// </summary>
        [DefaultValue("")]
        public string PropertyName => propertyName;

        public event BindingCompleteEventHandler BindingComplete
        {
            add
            {
                onComplete += value;
            }
            remove
            {
                onComplete -= value;
            }
        }

        public event ConvertEventHandler Parse
        {
            add
            {
                onParse += value;
            }
            remove
            {
                onParse -= value;
            }
        }

        public event ConvertEventHandler Format
        {
            add
            {
                onFormat += value;
            }
            remove
            {
                onFormat -= value;
            }
        }

        [DefaultValue(false)]
        public bool FormattingEnabled
        {
            // A note about FormattingEnabled: This flag was introduced in Whidbey, to enable new
            // formatting features. However, it is also used to trigger other new Whidbey binding
            // behavior not related to formatting (such as error handling). This preserves Everett
            // legacy behavior for old bindings (where FormattingEnabled = false).
            get => formattingEnabled;
            set
            {
                if (formattingEnabled != value)
                {
                    formattingEnabled = value;
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
            get => formatInfo;
            set
            {
                if (formatInfo != value)
                {
                    formatInfo = value;
                    if (IsBinding)
                    {
                        PushData();
                    }
                }
            }
        }

        public string FormatString
        {
            get => formatString;
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (!value.Equals(formatString))
                {
                    formatString = value;
                    if (IsBinding)
                    {
                        PushData();
                    }
                }
            }
        }

        public object NullValue
        {
            get => nullValue;
            set
            {
                // Try to compare logical values, not object references...
                if (!object.Equals(nullValue, value))
                {
                    nullValue = value;

                    // If data member is currently DBNull, force update of bound
                    // control property so that it displays the new NullValue
                    if (IsBinding && Formatter.IsNullData(bindToObject.GetValue(), dsNullValue))
                    {
                        PushData();
                    }
                }
            }
        }

        public object DataSourceNullValue
        {
            get => dsNullValue;
            set
            {
                // Try to compare logical values, not object references...
                if (!Object.Equals(dsNullValue, value))
                {
                    // Save old Value
                    object oldValue = dsNullValue;

                    // Set value
                    dsNullValue = value;

                    dsNullValueSet = true;

                    // If control's property is capable of displaying a special value for DBNull,
                    // and the DBNull status of data source's property has changed, force the
                    // control property to refresh itself from the data source property.
                    if (IsBinding)
                    {
                        object dsValue = bindToObject.GetValue();

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
            get => controlUpdateMode;
            set
            {
                if (controlUpdateMode != value)
                {
                    controlUpdateMode = value;

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
                    if (propInfo != null && control != null)
                    {
                        EventHandler handler = new EventHandler(this.Target_PropertyChanged);
                        propInfo.AddValueChanged(control, handler);
                    }
                    if (validateInfo != null)
                    {
                        CancelEventHandler handler = new CancelEventHandler(this.Target_Validate);
                        validateInfo.AddEventHandler(control, handler);
                    }
                }
            }
            else
            {
                if (propInfo != null && control != null)
                {
                    EventHandler handler = new EventHandler(this.Target_PropertyChanged);
                    propInfo.RemoveValueChanged(control, handler);
                }
                if (validateInfo != null)
                {
                    CancelEventHandler handler = new CancelEventHandler(this.Target_Validate);
                    validateInfo.RemoveEventHandler(control, handler);
                }
            }
        }

        private void binding_MetaDataChanged(object sender, EventArgs e)
        {
            Debug.Assert(sender == this.bindingManagerBase, "we should only receive notification from our binding manager base");
            CheckBinding();
        }

        private void CheckBinding()
        {
            bindToObject.CheckBinding();

            if (control != null && !string.IsNullOrEmpty(propertyName))
            {
                control.DataBindings.CheckDuplicates(this);

                Type controlClass = control.GetType();

                // Check Properties
                string propertyNameIsNull = propertyName + "IsNull";
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
                InheritanceAttribute attr = (InheritanceAttribute)TypeDescriptor.GetAttributes(control)[typeof(InheritanceAttribute)];
                if (attr != null && attr.InheritanceLevel != InheritanceLevel.NotInherited)
                {
                    propInfos = TypeDescriptor.GetProperties(controlClass);
                }
                else
                {
                    propInfos = TypeDescriptor.GetProperties(control);
                }

                for (int i = 0; i < propInfos.Count; i++)
                {
                    if (tempPropInfo == null && string.Equals(propInfos[i].Name, propertyName, StringComparison.OrdinalIgnoreCase))
                    {
                        tempPropInfo = propInfos[i];
                        if (tempPropIsNullInfo != null)
                        {
                            break;
                        }
                    }
                    if (tempPropIsNullInfo == null && string.Equals(propInfos[i].Name, propertyNameIsNull, StringComparison.OrdinalIgnoreCase))
                    {
                        tempPropIsNullInfo = propInfos[i];
                        if (tempPropInfo != null)
                        {
                            break;
                        }
                    }
                }

                if (tempPropInfo == null)
                {
                    throw new ArgumentException(string.Format(SR.ListBindingBindProperty, propertyName), "PropertyName");
                }
                if (tempPropInfo.IsReadOnly && this.controlUpdateMode != ControlUpdateMode.Never)
                {
                    throw new ArgumentException(string.Format(SR.ListBindingBindPropertyReadOnly, propertyName), "PropertyName");
                }

                propInfo = tempPropInfo;
                propType = propInfo.PropertyType;
                propInfoConverter = propInfo.Converter;

                if (tempPropIsNullInfo != null && tempPropIsNullInfo.PropertyType == typeof(bool) && !tempPropIsNullInfo.IsReadOnly)
                {
                    propIsNullInfo = tempPropIsNullInfo;
                }

                // Check events
                EventDescriptor tempValidateInfo = null;
                string validateName = "Validating";
                EventDescriptorCollection eventInfos = TypeDescriptor.GetEvents(control);
                for (int i = 0; i < eventInfos.Count; i++)
                {
                    if (tempValidateInfo == null && string.Equals(eventInfos[i].Name, validateName, StringComparison.OrdinalIgnoreCase))
                    {
                        tempValidateInfo = eventInfos[i];
                        break;
                    }
                }
                validateInfo = tempValidateInfo;
            }
            else
            {
                propInfo = null;
                validateInfo = null;
            }

            // go see if we become bound now.
            UpdateIsBinding();
        }

        internal bool ControlAtDesignTime()
        {
            if (!(control is IComponent comp))
            {
                return false;
            }

            ISite site = comp.Site;
            return site != null && site.DesignMode;
        }

        private object GetDataSourceNullValue(Type type)
        {
            return dsNullValueSet ? dsNullValue : Formatter.GetDefaultDataSourceNullValue(type);
        }

        [SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes", Justification = "Perfectly acceptible when dealing with PropertyDescriptors")]
        private object GetPropValue()
        {
            bool isNull = false;
            if (propIsNullInfo != null)
            {
                isNull = (bool)propIsNullInfo.GetValue(control);
            }
            object value;
            if (isNull)
            {
                return DataSourceNullValue;
            }
             
            return propInfo.GetValue(control) ?? DataSourceNullValue;
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
                errorText = bindToObject.DataErrorText;

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
            // This recursion guard will only be in effect if FormattingEnabled because this method
            // is only called if formatting is enabled.
            if (!inOnBindingComplete)
            {
                try
                {
                    inOnBindingComplete = true;
                    onComplete?.Invoke(this, e);
                }
                catch (Exception ex) when (!ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    // BindingComplete event is intended primarily as an "FYI" event with support for cancellation.
                    // User code should not be throwing exceptions from this event as a way to signal new error conditions (they should use
                    // things like the Format or Parse events for that). Exceptions thrown here can mess up currency manager behavior big time.
                    // For now, eat any non-critical exceptions and instead just cancel the current push/pull operation.
                    e.Cancel = true;
                }
                finally
                {
                    inOnBindingComplete = false;
                }
            }
        }

        protected virtual void OnParse(ConvertEventArgs cevent)
        {
            onParse?.Invoke(this, cevent);

            if (!formattingEnabled)
            {
                if (!(cevent.Value is DBNull) && cevent.Value != null && cevent.DesiredType != null && !cevent.DesiredType.IsInstanceOfType(cevent.Value) && (cevent.Value is IConvertible))
                {
                    cevent.Value = Convert.ChangeType(cevent.Value, cevent.DesiredType, CultureInfo.CurrentCulture);
                }
            }
        }

        protected virtual void OnFormat(ConvertEventArgs cevent)
        {
            onFormat?.Invoke(this, cevent);

            if (!formattingEnabled)
            {
                if (!(cevent.Value is DBNull) && cevent.DesiredType != null && !cevent.DesiredType.IsInstanceOfType(cevent.Value) && (cevent.Value is IConvertible))
                {
                    cevent.Value = Convert.ChangeType(cevent.Value, cevent.DesiredType, CultureInfo.CurrentCulture);
                }
            }
        }

        private object ParseObject(object value)
        {
            Type type = bindToObject.BindToType;
            if (formattingEnabled)
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
                    if (bindToObject.FieldInfo != null)
                    {
                        fieldInfoConverter = bindToObject.FieldInfo.Converter;
                    }

                    return Formatter.ParseObject(value, type, (value == null ? propInfo.PropertyType : value.GetType()), fieldInfoConverter, propInfoConverter, formatInfo, nullValue, GetDataSourceNullValue(type));
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

            Type type = propInfo.PropertyType;
            if (formattingEnabled)
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
                    if (bindToObject.FieldInfo != null)
                    {
                        fieldInfoConverter = bindToObject.FieldInfo.Converter;
                    }

                    return Formatter.FormatObject(value, type, fieldInfoConverter, propInfoConverter, formatString, formatInfo, nullValue, dsNullValue);
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
        /// Pulls data from control property into data source. Returns bool indicating whether caller
        /// should cancel the higher level operation. Raises a BindingComplete event regardless of
        /// success or failure.
        ///
        /// When the user leaves the control, it will raise a Validating event, calling the Binding.Target_Validate
        /// method, which in turn calls PullData. PullData is also called by the binding manager when pulling data
        /// from all bounds properties in one go.
        /// </summary>
        internal bool PullData() =>  PullData(reformat: true, force: false);

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
                if (propInfo.SupportsChangeEvents && !modified)
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
            if (inPushOrPull && formattingEnabled)
            {
                return false;
            }

            inPushOrPull = true;

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
                if (lastException != null || (!FormattingEnabled && parsedValue == null))
                {
                    parseFailed = true;
                    parsedValue = this.bindToObject.GetValue();
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
                        if (force || !FormattingEnabled || !Object.Equals(formattedObject, value))
                        {
                            SetPropValue(formattedObject);
                        }
                    }
                }

                // Put the value into the data model
                if (!parseFailed)
                {
                    bindToObject.SetValue(parsedValue);
                }
            }
            catch (Exception ex) when (FormattingEnabled)
            {
                // Throw the exception unless this binding has formatting enabled
                lastException = ex;
            }
            finally
            {
                inPushOrPull = false;
            }

            if (FormattingEnabled)
            {
                // Raise the BindingComplete event, giving listeners a chance to process any
                // errors that occured and decide whether the operation should be cancelled.
                BindingCompleteEventArgs args = CreateBindingCompleteEventArgs(BindingCompleteContext.DataSourceUpdate, lastException);
                OnBindingComplete(args);

                // If the operation completed successfully (and was not cancelled), we can clear the dirty flag
                // on this binding because we know the value in the control was valid and has been accepted by
                // the data source. But if the operation failed (or was cancelled), we must leave the dirty flag
                // alone, so that the control's value will continue to be re-validated and re-pulled later.
                if (args.BindingCompleteState == BindingCompleteState.Success && args.Cancel == false)
                {
                    modified = false;
                }

                return args.Cancel;
            }
            else
            {
                // Do not emit BindingComplete events, or allow the operation to be cancelled.
                // If we get this far, treat the operation as successful and clear the dirty flag.
                modified = false;
                return false;
            }
        }

        /// <summary>
        /// Pushes data from data source into control property. Returns bool indicating whether caller
        /// should cancel the higher level operation. Raises a BindingComplete event regardless of
        /// success or failure.
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
            if (inPushOrPull && formattingEnabled)
            {
                return false;
            }

            inPushOrPull = true;

            try
            {
                if (IsBinding)
                {
                    dataSourceValue = bindToObject.GetValue();
                    object controlValue = FormatObject(dataSourceValue);
                    SetPropValue(controlValue);
                    modified = false;
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
                inPushOrPull = false;
            }

            if (FormattingEnabled)
            {
                // Raise the BindingComplete event, giving listeners a chance to process any errors that occured, and decide
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
        /// Reads current value from data source, and sends this to the control.
        /// </summary>
        public void ReadValue() => PushData(force: true);

        /// <summary>
        /// Takes current value from control, and writes this out to the data source.
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

            inSetPropValue = true;

            try
            {
                bool isNull = value == null || Formatter.IsNullData(value, DataSourceNullValue);
                if (isNull)
                {
                    if (propIsNullInfo != null)
                    {
                        propIsNullInfo.SetValue(control, true);
                    }
                    else
                    {
                        if (propInfo.PropertyType == typeof(object))
                        {
                            propInfo.SetValue(control, DataSourceNullValue);
                        }
                        else
                        {
                            propInfo.SetValue(control, null);
                        }
                    }
                }
                else
                {
                    propInfo.SetValue(control, value);
                }
            }
            finally
            {
                inSetPropValue = false;
            }
        }

        private bool ShouldSerializeFormatString() => !string.IsNullOrEmpty(formatString);

        private bool ShouldSerializeNullValue() => nullValue != null;

        private bool ShouldSerializeDataSourceNullValue()
        {
            return dsNullValueSet && dsNullValue != Formatter.GetDefaultDataSourceNullValue(null);
        }

        private void Target_PropertyChanged(object sender, EventArgs e)
        {
            if (inSetPropValue)
            {
                return;
            }

            if (IsBinding)
            {
                modified = true;

                // If required, update data source every time control property changes.
                // NOTE: We need modified=true to be set both before pulling data
                // (so that pull will work) and afterwards (so that validation will
                // still occur later on).
                if (DataSourceUpdateMode == DataSourceUpdateMode.OnPropertyChanged)
                {
                    PullData(reformat: false);
                    modified = true;
                }
            }
        }

        /// <summary>
        /// Event handler for the Control.Validating event on the control that we are bound to.
        ///
        /// If value in control has changed, we want to send that value back up to the data source
        /// when the control undergoes validation (eg. on loss of focus). If an error occurs, we
        /// will set e.Cancel=true to make validation fail and force focus to remain on the control.
        ///
        /// NOTE: If no error occurs, we MUST leave e.Cancel alone, to respect any value put in there
        /// by event handlers high up the event chain.
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
                return (control != null && !string.IsNullOrEmpty(propertyName) &&
                                bindToObject.DataSource != null && bindingManagerBase != null);
            }
        }

        internal void UpdateIsBinding()
        {
            bool newBound = IsBindable && ComponentCreated && bindingManagerBase.IsBinding;
            if (bound != newBound)
            {
                bound = newBound;
                BindTarget(newBound);
                if (bound)
                {
                    if (controlUpdateMode == ControlUpdateMode.Never)
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
}

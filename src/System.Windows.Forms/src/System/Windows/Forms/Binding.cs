// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using Microsoft.Win32;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Collections;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Security;

    /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Represents a simple binding of a value in a list
    ///       and the property of a control.
    ///    </para>
    /// </devdoc>
    [TypeConverterAttribute(typeof(ListBindingConverter))]
    public class Binding {

        // the two collection owners that this binding belongs to.
        private IBindableComponent control;
        private BindingManagerBase bindingManagerBase;
        
        private BindToObject bindToObject = null;
        
        private string propertyName = "";

        private PropertyDescriptor propInfo;
        private PropertyDescriptor propIsNullInfo;
        private EventDescriptor validateInfo;
        private TypeConverter propInfoConverter;

        private bool formattingEnabled = false;

        private bool bound = false;
        private bool modified = false;

        //Recursion guards
        private bool inSetPropValue = false;
        private bool inPushOrPull = false;
        private bool inOnBindingComplete = false;

        // formatting stuff
        private string formatString = String.Empty;
        private IFormatProvider formatInfo = null;
        private object nullValue = null;
        private object dsNullValue = Formatter.GetDefaultDataSourceNullValue(null);
        private bool dsNullValueSet;
        private ConvertEventHandler onParse = null;
        private ConvertEventHandler onFormat = null;

        // binding stuff
        private ControlUpdateMode controlUpdateMode = ControlUpdateMode.OnPropertyChanged;
        private DataSourceUpdateMode dataSourceUpdateMode = DataSourceUpdateMode.OnValidation;
        private BindingCompleteEventHandler onComplete = null;

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.Binding"]/*' />
        /// <devdoc>
        ///     Initializes a new instance of the <see cref='System.Windows.Forms.Binding'/> class
        ///     that binds a property on the owning control to a property on a data source.
        /// </devdoc>
        public Binding(string propertyName, Object dataSource, string dataMember) : this(propertyName, dataSource, dataMember, false, 0, null, String.Empty, null) {
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.Binding6"]/*' />
        public Binding(string propertyName, Object dataSource, string dataMember, bool formattingEnabled) : this(propertyName, dataSource, dataMember, formattingEnabled, 0, null, String.Empty, null) {
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.Binding2"]/*' />
        public Binding(string propertyName, Object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode) : this(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode, null, String.Empty, null) {
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.Binding3"]/*' />
        public Binding(string propertyName, Object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue) : this(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode, nullValue, String.Empty, null) {
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.Binding5"]/*' />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")] // 'formatString' is an appropriate name, since its a string passed to the Format method
        public Binding(string propertyName, Object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue, string formatString) : this(propertyName, dataSource, dataMember, formattingEnabled, dataSourceUpdateMode, nullValue, formatString, null) {
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.Binding4"]/*' />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")] // By design (no-one should be subclassing this class)
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")] // 'formatString' is an appropriate name, since its a string passed to the Format method
        public Binding(string propertyName, Object dataSource, string dataMember, bool formattingEnabled, DataSourceUpdateMode dataSourceUpdateMode, object nullValue, string formatString, IFormatProvider formatInfo) {
            this.bindToObject = new BindToObject(this, dataSource, dataMember);

            this.propertyName = propertyName;
            this.formattingEnabled = formattingEnabled;
            this.formatString = formatString;
            this.nullValue = nullValue;
            this.formatInfo = formatInfo;
            this.formattingEnabled = formattingEnabled;
            this.dataSourceUpdateMode = dataSourceUpdateMode;

            CheckBinding();
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.Binding1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.Binding'/> class.
        ///    </para>
        /// </devdoc>
        private Binding() {
        }

        internal BindToObject BindToObject {
            get {
                return this.bindToObject;
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.DataSource"]/*' />
        public object DataSource {
            get {
                return this.bindToObject.DataSource;
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.BindingMemberInfo"]/*' />
        public BindingMemberInfo BindingMemberInfo {
            get {
                return this.bindToObject.BindingMemberInfo;
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.BindableComponent"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the control to which the binding belongs.
        ///    </para>
        /// </devdoc>
        [
        DefaultValue(null)
        ]
        public IBindableComponent BindableComponent {
            [
                SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)
            ]
            get {
                return this.control;
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.Control"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the control to which the binding belongs.
        ///    </para>
        /// </devdoc>
        [
        DefaultValue(null)
        ]
        public Control Control {
            [
                SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)
            ]
            get {
                return this.control as Control;
            }
        }

        // Is the binadable component in a 'created' (ready-to-use) state? For controls,
        // this depends on whether the window handle has been created yet. For everything
        // else, we'll assume they are always in a created state.
        internal static bool IsComponentCreated(IBindableComponent component) {
            Control ctrl = (component as Control);

            if (ctrl != null) {
                return ctrl.Created;
            }
            else {
                return true;
            }
        }

        // Instance-specific property equivalent to the static method above
        internal bool ComponentCreated {
            get {
                return IsComponentCreated(this.control);
            }
        }

        private void FormLoaded(object sender, EventArgs e) {
            Debug.Assert(sender == control, "which other control can send us the Load event?");
            // update the binding
            CheckBinding();
        }

        internal void SetBindableComponent(IBindableComponent value) {
            if (this.control != value) {
                IBindableComponent oldTarget = control;
                BindTarget(false);
                this.control = value;
                BindTarget(true);
                try {
                    CheckBinding();
                }
                catch {
                    BindTarget(false);
                    control = oldTarget;
                    BindTarget(true);
                    throw;
                }

                // We are essentially doing to the listManager what we were doing to the
                // BindToObject: bind only when the control is created and it has a BindingContext
                BindingContext.UpdateBinding((control != null && IsComponentCreated(control) ? control.BindingContext: null), this);
                Form form = value as Form;
                if (form != null) {
                    form.Load += new EventHandler(FormLoaded);
                }
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.IsBinding"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the binding is active.
        ///    </para>
        /// </devdoc>
        public bool IsBinding {
            get {
                return bound;
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.BindingManagerBase"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the <see cref='System.Windows.Forms.BindingManagerBase'/>
        ///       of this binding that allows enumeration of a set of
        ///       bindings.
        ///    </para>
        /// </devdoc>
        public BindingManagerBase BindingManagerBase{
            get {
                return bindingManagerBase;
            }
        }

        internal void SetListManager(BindingManagerBase bindingManagerBase) {
            if (this.bindingManagerBase is CurrencyManager) {
                ((CurrencyManager)this.bindingManagerBase).MetaDataChanged -= new EventHandler(binding_MetaDataChanged);
            }

            this.bindingManagerBase = bindingManagerBase;

            if (this.bindingManagerBase is CurrencyManager ) {
                ((CurrencyManager)this.bindingManagerBase).MetaDataChanged += new EventHandler(binding_MetaDataChanged);
            }

            this.BindToObject.SetBindingManagerBase(bindingManagerBase);
            CheckBinding();
        }
        
        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.PropertyName"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the property on the control to bind to.
        ///    </para>
        /// </devdoc>
        [DefaultValue("")]
        public string PropertyName {
            get {
                return propertyName;
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.BindingComplete"]/*' />
        public event BindingCompleteEventHandler BindingComplete {
            add {
                onComplete += value;
            }
            remove {
                onComplete -= value;
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.Parse"]/*' />
        public event ConvertEventHandler Parse {
            add {
                onParse += value;
            }
            remove {
                onParse -= value;
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.Format"]/*' />
        public event ConvertEventHandler Format {
            add {
                onFormat += value;
            }
            remove {
                onFormat -= value;
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.FormattingEnabled"]/*' />
        [DefaultValue(false)]
        public bool FormattingEnabled {

            // A note about FormattingEnabled: This flag was introduced in Whidbey, to enable new
            // formatting features. However, it is also used to trigger other new Whidbey binding
            // behavior not related to formatting (such as error handling). This preserves Everett
            // legacy behavior for old bindings (where FormattingEnabled = false).

            get {
                return formattingEnabled;
            }
            set {
                if (formattingEnabled != value) {
                    formattingEnabled = value;
                    if (IsBinding) {
                        PushData();
                    }
                }
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.FormatInfo"]/*' />
        [DefaultValue(null)]
        public IFormatProvider FormatInfo {
            get {
                return this.formatInfo;
            }
            set {
                if (formatInfo != value) {
                    this.formatInfo = value;
                    if (IsBinding) {
                        PushData();
                    }
                }
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.FormatString"]/*' />
        public string FormatString {
            get {
                return this.formatString;
            }
            set {
                if (value == null)
                    value = String.Empty;
                if (!value.Equals(formatString)) {
                    this.formatString = value;
                    if (IsBinding) {
                        PushData();
                    }
                }
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.NullValue"]/*' />
        public object NullValue {
            get {
                return this.nullValue;
            }
            set {
                // Try to compare logical values, not object references...
                if (!Object.Equals(nullValue, value)) {
                    this.nullValue = value;

                    // If data member is currently DBNull, force update of bound
                    // control property so that it displays the new NullValue
                    //
                    if (IsBinding && Formatter.IsNullData(bindToObject.GetValue(), this.dsNullValue)) {
                        PushData();
                    }
                }
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.DataSourceNullValue"]/*' />
        public object DataSourceNullValue {
            get {
                return this.dsNullValue;
            }
            set {
                // Try to compare logical values, not object references...
                if (!Object.Equals(this.dsNullValue, value)) {

                    // Save old Value
                    object oldValue = this.dsNullValue;

                    // Set value
                    this.dsNullValue = value;

                    this.dsNullValueSet = true;

                    // If control's property is capable of displaying a special value for DBNull,
                    // and the DBNull status of data source's property has changed, force the
                    // control property to refresh itself from the data source property.
                    //
                    if (IsBinding) {
                        object dsValue = bindToObject.GetValue();

                        // Check previous DataSourceNullValue for null
                        if (Formatter.IsNullData(dsValue, oldValue)) {
                            // Update DataSource Value to new DataSourceNullValue
                            WriteValue();
                        }

                        // Check current DataSourceNullValue
                        if (Formatter.IsNullData(dsValue, value)) {
                            // Update Control because the DataSource is now null
                            ReadValue();
                        }
                    }
                }
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.ControlUpdateMode"]/*' />
        [DefaultValue(ControlUpdateMode.OnPropertyChanged)]
        public ControlUpdateMode ControlUpdateMode {
            get {
                return this.controlUpdateMode;
            }
            set {
                if (this.controlUpdateMode != value) {
                    this.controlUpdateMode = value;

                    // Refresh the control from the data source, to reflect the new update mode
                    if (IsBinding) {
                        PushData();
                    }
                }
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.DataSourceUpdateMode"]/*' />
        [DefaultValue(DataSourceUpdateMode.OnValidation)]
        public DataSourceUpdateMode DataSourceUpdateMode {
            get {
                return this.dataSourceUpdateMode;
            }
            set {
                if (this.dataSourceUpdateMode != value) {
                    this.dataSourceUpdateMode = value;
                }
            }
        }

        private void BindTarget(bool bind) {
            if (bind) {
                if (IsBinding) {
                    if (propInfo != null && control != null) {
                        EventHandler handler = new EventHandler(this.Target_PropertyChanged);
                        propInfo.AddValueChanged(control, handler);
                    }
                    if (validateInfo != null) {
                        CancelEventHandler handler = new CancelEventHandler(this.Target_Validate);
                        validateInfo.AddEventHandler(control, handler);
                    }
                }
            }
            else {
                if (propInfo != null && control != null) {
                    EventHandler handler = new EventHandler(this.Target_PropertyChanged);
                    propInfo.RemoveValueChanged(control, handler);
                }
                if (validateInfo != null) {
                    CancelEventHandler handler = new CancelEventHandler(this.Target_Validate);
                    validateInfo.RemoveEventHandler(control, handler);
                }               
            }
        }

        private void binding_MetaDataChanged(object sender, EventArgs e) {
            Debug.Assert(sender == this.bindingManagerBase, "we should only receive notification from our binding manager base");
            CheckBinding();
        }

        private void CheckBinding() {
            this.bindToObject.CheckBinding();

            if (control != null && propertyName.Length > 0) {
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
                //
                InheritanceAttribute attr = (InheritanceAttribute)TypeDescriptor.GetAttributes(control)[typeof(InheritanceAttribute)];
                if (attr != null && attr.InheritanceLevel != InheritanceLevel.NotInherited) {
                    propInfos = TypeDescriptor.GetProperties(controlClass);
                }
                else {
                    propInfos = TypeDescriptor.GetProperties(control);
                }
                
                for (int i = 0; i < propInfos.Count; i++) {
                    if(tempPropInfo==null && String.Equals (propInfos[i].Name, propertyName, StringComparison.OrdinalIgnoreCase)) {
                        tempPropInfo = propInfos[i];
                        if (tempPropIsNullInfo != null)
                            break;
                    }
                    if(tempPropIsNullInfo == null && String.Equals (propInfos[i].Name, propertyNameIsNull, StringComparison.OrdinalIgnoreCase)) {
                        tempPropIsNullInfo = propInfos[i];
                        if (tempPropInfo != null)
                            break;
                    }
                }

                if (tempPropInfo == null) {
                    throw new ArgumentException(string.Format(SR.ListBindingBindProperty, propertyName), "PropertyName");
                }
                if (tempPropInfo.IsReadOnly && this.controlUpdateMode != ControlUpdateMode.Never) {
                    throw new ArgumentException(string.Format(SR.ListBindingBindPropertyReadOnly, propertyName), "PropertyName");
                }

                propInfo = tempPropInfo;
                propType = propInfo.PropertyType;
                propInfoConverter = propInfo.Converter;

                if (tempPropIsNullInfo != null && tempPropIsNullInfo.PropertyType == typeof(bool) && !tempPropIsNullInfo.IsReadOnly)
                    propIsNullInfo = tempPropIsNullInfo;

                // Check events
                EventDescriptor tempValidateInfo = null;
                string validateName = "Validating";
                EventDescriptorCollection eventInfos = TypeDescriptor.GetEvents(control);
                for (int i = 0; i < eventInfos.Count; i++) {
                    if(tempValidateInfo==null && String.Equals (eventInfos[i].Name, validateName, StringComparison.OrdinalIgnoreCase)) {
                        tempValidateInfo = eventInfos[i];
                        break;
                    }
                }
                validateInfo = tempValidateInfo;
            }
            else {
                propInfo = null;
                validateInfo = null;
            }

            // go see if we become bound now.
            UpdateIsBinding();
        }

        internal bool ControlAtDesignTime() {
            IComponent comp = (this.control as IComponent);
            if (comp == null)
                return false;

            ISite site = comp.Site;
            if (site == null)
                return false;

            return site.DesignMode;
        }

        private object GetDataSourceNullValue(Type type) {
            return this.dsNullValueSet ? this.dsNullValue : Formatter.GetDefaultDataSourceNullValue(type);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes")] // Perfectly acceptible when dealing with PropertyDescriptors
        private Object GetPropValue() {
            bool isNull = false;
            if (propIsNullInfo != null) {
                isNull = (bool) propIsNullInfo.GetValue(control);
            }
            Object value;
            if (isNull) {
                value = DataSourceNullValue;
            }
            else {
                value =  propInfo.GetValue(control);
                // 


                if (value == null) {
                    value = DataSourceNullValue;
                }
            }
            return value;
        }

        private BindingCompleteEventArgs CreateBindingCompleteEventArgs(BindingCompleteContext context, Exception ex) {
            bool cancel = false;
            string errorText = String.Empty;
            BindingCompleteState state = BindingCompleteState.Success;

            if (ex != null) {
                // If an exception was provided, report that
                errorText = ex.Message;
                state = BindingCompleteState.Exception;
                cancel = true;
            }
            else {
                // If data error info on data source for this binding, report that
                errorText = this.BindToObject.DataErrorText;

                // We should not cancel with an IDataErrorInfo error - we didn't in Everett
                if (!String.IsNullOrEmpty(errorText)) {
                    state = BindingCompleteState.DataError;
                }
            }

            return new BindingCompleteEventArgs(this, state, context, errorText, ex, cancel);
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.OnBindingComplete"]/*' />
        protected virtual void OnBindingComplete(BindingCompleteEventArgs e) {
            // This recursion guard will only be in effect if FormattingEnabled because this method
            // is only called if formatting is enabled.
            if (!inOnBindingComplete) {
                try {
                    inOnBindingComplete = true;
                    if (onComplete != null) {
                        onComplete(this, e);
                    }
                }
                catch (Exception ex) {
                    if (ClientUtils.IsSecurityOrCriticalException(ex)) {
                        throw;
                    }

                    // BindingComplete event is intended primarily as an "FYI" event with support for cancellation.
                    // User code should not be throwing exceptions from this event as a way to signal new error conditions (they should use
                    // things like the Format or Parse events for that). Exceptions thrown here can mess up currency manager behavior big time.
                    // For now, eat any non-critical exceptions and instead just cancel the current push/pull operation.
                    e.Cancel = true;
                }
                finally {
                    inOnBindingComplete = false;
                }
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.OnParse"]/*' />
        protected virtual void OnParse(ConvertEventArgs cevent) {
            if (onParse != null) {
                onParse(this, cevent);
            }

            if (!formattingEnabled) {
                if (!(cevent.Value is System.DBNull) && cevent.Value != null && cevent.DesiredType != null && !cevent.DesiredType.IsInstanceOfType(cevent.Value) && (cevent.Value is IConvertible)) {
                    cevent.Value = Convert.ChangeType(cevent.Value, cevent.DesiredType, CultureInfo.CurrentCulture);
                }
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.OnFormat"]/*' />
        protected virtual void OnFormat(ConvertEventArgs cevent) {
            if (onFormat!= null) {
                onFormat(this, cevent);
            }
            if (!formattingEnabled) {
                if (!(cevent.Value is System.DBNull) && cevent.DesiredType != null && !cevent.DesiredType.IsInstanceOfType(cevent.Value) && (cevent.Value is IConvertible)) {
                    cevent.Value = Convert.ChangeType(cevent.Value, cevent.DesiredType, CultureInfo.CurrentCulture);
                }
            }
        }

        private object ParseObject(object value) {
            Type type = this.bindToObject.BindToType;

            if (formattingEnabled) {
                // -------------------------------
                // Behavior for Whidbey and beyond
                // -------------------------------

                // Fire the Parse event so that user code gets a chance to supply the parsed value for us
                ConvertEventArgs e = new ConvertEventArgs(value, type);
                OnParse(e);

                object newValue = e.Value;

                if (!object.Equals(value, newValue)) {
                    // If event handler replaced formatted value with parsed value, use that
                    return newValue;
                }
                else {
                    // Otherwise parse the formatted value ourselves
                    TypeConverter fieldInfoConverter = null;
                    if (bindToObject.FieldInfo != null) {
                        fieldInfoConverter = bindToObject.FieldInfo.Converter;
                    }
                    return Formatter.ParseObject(value, type, (value == null ? propInfo.PropertyType : value.GetType()), fieldInfoConverter, propInfoConverter, formatInfo, nullValue, GetDataSourceNullValue(type));
                }

            } else {
                // ----------------------------
                // Behavior for RTM and Everett  [DO NOT MODIFY!]
                // ----------------------------

                ConvertEventArgs e = new ConvertEventArgs(value, type);
                // first try: use the OnParse event
                OnParse(e);
                // 
                if (e.Value != null && (e.Value.GetType().IsSubclassOf(type) || e.Value.GetType() == type || e.Value is System.DBNull))
                    return e.Value;
                // second try: use the TypeConverter
                TypeConverter typeConverter = TypeDescriptor.GetConverter(value != null ? value.GetType() : typeof(Object));
                if (typeConverter != null && typeConverter.CanConvertTo(type)) {
                    return typeConverter.ConvertTo(value, type);
                }
                // last try: use Convert.ToType
                if (value is IConvertible) {
                    object ret = Convert.ChangeType(value, type, CultureInfo.CurrentCulture);
                    if (ret != null && (ret.GetType().IsSubclassOf(type) || ret.GetType() == type))
                        return ret;
                }
                // time to fail: (RTM/Everett just returns null, whereas Whidbey throws an exception)
                return null;
            }
        }

        private object FormatObject(object value) {
            // We will not format the object when the control is in design time.
            // This is because if we bind a boolean property on a control to a
            // row that is full of DBNulls then we cause problems in the shell.
            if (ControlAtDesignTime())
                return value;

            Type type = propInfo.PropertyType;

            if (formattingEnabled) {
                // -------------------------------
                // Behavior for Whidbey and beyond
                // -------------------------------

                // Fire the Format event so that user code gets a chance to supply the formatted value for us
                ConvertEventArgs e = new ConvertEventArgs(value, type);
                OnFormat(e);

                if (e.Value != value) {
                    // If event handler replaced parsed value with formatted value, use that
                    return e.Value;
                }
                else {
                    // Otherwise format the parsed value ourselves
                    TypeConverter fieldInfoConverter = null;
                    if (bindToObject.FieldInfo != null) {
                        fieldInfoConverter = bindToObject.FieldInfo.Converter;
                    }
                    return Formatter.FormatObject(value, type, fieldInfoConverter, propInfoConverter, formatString, formatInfo, nullValue, dsNullValue);
                }

            } else {
                // ----------------------------
                // Behavior for RTM and Everett  [DO NOT MODIFY!]
                // ----------------------------

                // first try: use the Format event
                ConvertEventArgs e = new ConvertEventArgs(value, type);
                OnFormat(e);
                object ret = e.Value;

                // Approved breaking-change behavior between RTM and Everett: Fire the Format event even if the control property is of type
                // Object (RTM used to skip the event for properties of this type).

                if (type == typeof(object))
                    return value;

                // stop now if we have a value of a compatible type
                if (ret != null && (ret.GetType().IsSubclassOf(type) || ret.GetType() == type))
                    return ret;
                // second try: use type converter for the desiredType
                TypeConverter typeConverter = TypeDescriptor.GetConverter(value != null ? value.GetType() : typeof(Object));
                if (typeConverter != null && typeConverter.CanConvertTo(type)) {
                    ret = typeConverter.ConvertTo(value, type);
                    return ret;
                }
                // last try: use Convert.ChangeType
                if (value is IConvertible) {
                    ret = Convert.ChangeType(value, type, CultureInfo.CurrentCulture);
                    if (ret != null && (ret.GetType().IsSubclassOf(type) || ret.GetType() == type))
                        return ret;
                }

                // time to fail:
                throw new FormatException(SR.ListBindingFormatFailed);
            }
        }

        //
        // PullData()
        //
        // Pulls data from control property into data source. Returns bool indicating whether caller
        // should cancel the higher level operation. Raises a BindingComplete event regardless of
        // success or failure.
        //
        // When the user leaves the control, it will raise a Validating event, calling the Binding.Target_Validate
        // method, which in turn calls PullData. PullData is also called by the binding manager when pulling data
        // from all bounds properties in one go.
        //

        internal bool PullData() {
            return PullData(true, false);
        }

        internal bool PullData(bool reformat) {
            return PullData(reformat, false);
        }

        internal bool PullData(bool reformat, bool force) {
            //Don't update the control if the control update mode is never.
            if (ControlUpdateMode == ControlUpdateMode.Never) {
                reformat = false;
            }
            bool parseFailed = false;
            object parsedValue = null;
            Exception lastException = null;

            // Check whether binding has been suspended or is simply not possible right now
            if (!IsBinding) {
                return false;
            }

            // If caller is not FORCING us to pull, determine whether we want to pull right now...
            if (!force) {
                // If control property supports change events, only pull if the value has been changed since
                // the last update (ie. its dirty). For properties that do NOT support change events, we cannot
                // track the dirty state, so we just have to pull all the time.
                if (propInfo.SupportsChangeEvents && !modified) {
                    return false;
                }

                // Don't pull if the update mode is 'Never' (ie. read-only binding)
                if (DataSourceUpdateMode == DataSourceUpdateMode.Never) {
                    return false;
                }
            }

            // Re-entrancy check between push and pull (new for Whidbey - requires FormattingEnabled)
            if (inPushOrPull && formattingEnabled) {
                return false;
            }

            inPushOrPull = true;

            // Get the value from the bound control property
            Object value = GetPropValue();

            // Attempt to parse the property value into a format suitable for the data source
            try {
                parsedValue = ParseObject(value);
            }
            catch (Exception ex) {
                lastException = ex;

                // ...pre-Whidbey behavior was to eat parsing exceptions. This behavior is preserved.
            }

            try {
                // If parse failed, reset control property value back to original data source value.
                // An exception always indicates a parsing failure. A parsed value of null only indicates
                // a parsing failure when following pre-Whidbey behavior (ie. FormattingEnabled=False) since
                // in Whidbey we now support writing null back to the data source (eg. for business objects).
                if (lastException != null || (!FormattingEnabled && parsedValue == null)) {
                    parseFailed = true;
                    parsedValue = this.bindToObject.GetValue();
                }

                // Format the parsed value to be re-displayed in the control
                if (reformat) {
                    if (FormattingEnabled && parseFailed) {
                        // New behavior for Whidbey (ie. requires FormattingEnabled=true). If parsing
                        // fails, do NOT push the original data source value back into the control.
                        // This blows away the invalid value before the user gets a chance to see
                        // what needs correcting, which was the Everett behavior.
                    }
                    else {
                        object formattedObject = FormatObject(parsedValue);
 
                        if (force || !FormattingEnabled || !Object.Equals(formattedObject, value)) {
                            SetPropValue(formattedObject);
                        }
                    }
                }

                // Put the value into the data model
                if (!parseFailed) {
                    this.bindToObject.SetValue(parsedValue);
                }
            }
            catch (Exception ex) {
                lastException = ex;

                // This try/catch is new for Whidbey. To preserve Everett behavior, re-throw the
                // exception unless this binding has formatting enabled (new Whidbey feature).
                if (!FormattingEnabled) {
                    throw;
                }
            }
            finally {
                inPushOrPull = false;
            }

            if (FormattingEnabled) {
                // Whidbey...

                // Raise the BindingComplete event, giving listeners a chance to process any
                // errors that occured and decide whether the operation should be cancelled.
                BindingCompleteEventArgs args = CreateBindingCompleteEventArgs(BindingCompleteContext.DataSourceUpdate, lastException);
                OnBindingComplete(args);

                // If the operation completed successfully (and was not cancelled), we can clear the dirty flag
                // on this binding because we know the value in the control was valid and has been accepted by
                // the data source. But if the operation failed (or was cancelled), we must leave the dirty flag
                // alone, so that the control's value will continue to be re-validated and re-pulled later.
                if (args.BindingCompleteState == BindingCompleteState.Success && args.Cancel == false)
                    modified = false;

                return args.Cancel;
            }
            else {
                // Everett...

                // Do not emit BindingComplete events, or allow the operation to be cancelled.
                // If we get this far, treat the operation as successful and clear the dirty flag.
                modified = false;
                return false;
            }
        }

        //
        // PushData()
        //
        // Pushes data from data source into control property. Returns bool indicating whether caller
        // should cancel the higher level operation. Raises a BindingComplete event regardless of
        // success or failure.
        //

        internal bool PushData() {
            return PushData(false);
        }

        internal bool PushData(bool force) {
            Object dataSourceValue = null;
            Exception lastException = null;

            // Don't push if update mode is 'Never' (unless caller is FORCING us to push)
            if (!force && ControlUpdateMode == ControlUpdateMode.Never) {
                return false;
            }

            // Re-entrancy check between push and pull (new for Whidbey - requires FormattingEnabled)
            if (inPushOrPull && formattingEnabled) {
                return false;
            }

            inPushOrPull = true;

            try {
                if (IsBinding) {
                    dataSourceValue = bindToObject.GetValue();
                    object controlValue = FormatObject(dataSourceValue);
                    SetPropValue(controlValue);
                    modified = false;
                }
                else {
                    SetPropValue(null);
                }
            }
            catch (Exception ex) {
                lastException = ex;

                // This try/catch is new for Whidbey. To preserve Everett behavior, re-throw the
                // exception unless this binding has formatting enabled (new Whidbey feature).
                if (!FormattingEnabled) {
                    throw;
                }
            }
            finally {
                inPushOrPull = false;
            }

            if (FormattingEnabled) {
                // Whidbey...

                // Raise the BindingComplete event, giving listeners a chance to process any errors that occured, and decide
                // whether the operation should be cancelled. But don't emit the event if we didn't actually update the control.
                BindingCompleteEventArgs args = CreateBindingCompleteEventArgs(BindingCompleteContext.ControlUpdate, lastException);
                OnBindingComplete(args);

                return args.Cancel;
            }
            else {
                // Everett...

                // Do not emit BindingComplete events, or allow the operation to be cancelled.
                return false;
            }
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.ReadValue"]/*' />
        /// <devdoc>
        ///     Reads current value from data source, and sends this to the control.
        /// </devdoc>
        public void ReadValue() {
            PushData(/*force:*/ true);
        }

        /// <include file='doc\ListBinding.uex' path='docs/doc[@for="Binding.WriteValue"]/*' />
        /// <devdoc>
        ///     Takes current value from control, and writes this out to the data source.
        /// </devdoc>
        public void WriteValue() {
            // PullData has a guard for ControlUpdateMode == Never.

            PullData(/*reformat:*/ true, /*force:*/ true);
        }

        private void SetPropValue(Object value) {
            // we will not pull the data from the back end into the control
            // when the control is in design time. this is because if we bind a boolean property on a control
            // to a row that is full of DBNulls then we cause problems in the shell.
            if (ControlAtDesignTime())
                return;

            inSetPropValue = true;

            try {
                bool isNull = value == null || Formatter.IsNullData(value, DataSourceNullValue);
                if (isNull) {
                    if (propIsNullInfo != null) {
                        propIsNullInfo.SetValue(control, true);
                    }
                    else {
                        if (propInfo.PropertyType == typeof(object)) {
                            propInfo.SetValue(control, DataSourceNullValue);
                        }
                        else {
                            propInfo.SetValue(control, null);
                        }

                    }
                }
                else {
                    propInfo.SetValue(control, value);
                }
            }
            finally {
                inSetPropValue = false;
            }
        }

        private bool ShouldSerializeFormatString() {
            return formatString != null && formatString.Length > 0;

        }

        private bool ShouldSerializeNullValue() {
            return nullValue != null;
        }

        private bool ShouldSerializeDataSourceNullValue() {
            return this.dsNullValueSet && this.dsNullValue != Formatter.GetDefaultDataSourceNullValue(null);
        }

        private void Target_PropertyChanged(Object sender, EventArgs e) {
            if (inSetPropValue)
                return;

            if (IsBinding) {
                //dataSource.BeginEdit();

                modified = true;

                // If required, update data source every time control property changes.
                //      NOTE: We need modified=true to be set both before pulling data
                //      (so that pull will work) and afterwards (so that validation will
                //      still occur later on).
                if (DataSourceUpdateMode == DataSourceUpdateMode.OnPropertyChanged) {
                    PullData(false); // false = don't reformat property (bad user experience!!)
                    modified = true;
                }
            }
        }

        // Event handler for the Control.Validating event on the control that we are bound to.
        //
        // If value in control has changed, we want to send that value back up to the data source
        // when the control undergoes validation (eg. on loss of focus). If an error occurs, we
        // will set e.Cancel=true to make validation fail and force focus to remain on the control.
        //
        // NOTE: If no error occurs, we MUST leave e.Cancel alone, to respect any value put in there
        // by event handlers high up the event chain.
        //
        private void Target_Validate(Object sender, CancelEventArgs e) {
            try {
                if (PullData(true)) {
                    e.Cancel = true;
                }
            }
            catch {
                e.Cancel = true;
            }
        }

        internal bool IsBindable {
            get {
                return (control != null && propertyName.Length > 0 &&
                                bindToObject.DataSource != null && bindingManagerBase != null);
            }
        }
        
        internal void UpdateIsBinding() {
            bool newBound =  IsBindable && ComponentCreated && bindingManagerBase.IsBinding;
            if (bound != newBound) {
                bound = newBound;
                BindTarget(newBound);
                if (bound) {
                    if (controlUpdateMode == ControlUpdateMode.Never) {
                        PullData(false, true); //Don't reformat, force pull
                    } 
                    else {
                        PushData();
                    }
                }
            }
        }
    }
}

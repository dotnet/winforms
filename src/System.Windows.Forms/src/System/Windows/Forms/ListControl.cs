// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;

    using System.Diagnostics;

    using System;
    using System.Security.Permissions;
    using System.Globalization;
    using System.Windows.Forms;
    
    using System.Drawing.Design;
    using System.ComponentModel;
    using System.Windows.Forms.ComponentModel;

    using System.Collections;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Text;

    /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl"]/*' />
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    LookupBindingProperties("DataSource", "DisplayMember", "ValueMember", "SelectedValue")
    ]
    public abstract class ListControl : Control {

        private static readonly object EVENT_DATASOURCECHANGED          = new object();
        private static readonly object EVENT_DISPLAYMEMBERCHANGED       = new object();
        private static readonly object EVENT_VALUEMEMBERCHANGED         = new object();
        private static readonly object EVENT_SELECTEDVALUECHANGED       = new object();
        private static readonly object EVENT_FORMATINFOCHANGED          = new object();
        private static readonly object EVENT_FORMATSTRINGCHANGED        = new object();
        private static readonly object EVENT_FORMATTINGENABLEDCHANGED   = new object();

        private object dataSource;
        private CurrencyManager dataManager;
        private BindingMemberInfo displayMember;        
        private BindingMemberInfo valueMember;

        // Formatting stuff
        private string formatString = String.Empty;
        private IFormatProvider formatInfo = null;
        private bool formattingEnabled = false;
        private static readonly object EVENT_FORMAT   = new object();
        private TypeConverter displayMemberConverter = null;
        private static TypeConverter stringTypeConverter = null;
        
        private bool isDataSourceInitialized;
        private bool isDataSourceInitEventHooked;
        private bool inSetDataConnection = false;

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.DataSource"]/*' />
        /// <devdoc>
        ///     The ListSource to consume as this ListBox's source of data.
        ///     When set, a user can not modify the Items collection.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatData)),
        DefaultValue(null),
        RefreshProperties(RefreshProperties.Repaint),
        AttributeProvider(typeof(IListSource)),
        SRDescription(nameof(SR.ListControlDataSourceDescr))
        ]
        public object DataSource {
            get {
                return dataSource;
            }
            set {
                if (value != null && !(value is IList || value is IListSource))
                    throw new ArgumentException(SR.BadDataSourceForComplexBinding);
                if (dataSource == value)
                    return;
                // When we change the dataSource to null, we should reset
                // the displayMember to "".
                try {
                    SetDataConnection(value, displayMember, false);
                } catch {
                    // There are several possibilities why setting the data source throws an exception:
                    // 1. the app throws an exception in the events that fire when we change the data source: DataSourceChanged, 
                    // 2. we get an exception when we set the data source and populate the list controls (say,something went wrong while formatting the data)
                    // 3. the DisplayMember does not fit w/ the new data source (this could happen if the user resets the data source but did not reset the DisplayMember)
                    // in all cases ListControl should reset the DisplayMember to String.Empty
                    // the ListControl should also eat the exception - this is the RTM behavior and doing anything else is a breaking change
                    DisplayMember = "";
                }
                if (value == null)
                    DisplayMember = "";
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.DataSourceChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ListControlOnDataSourceChangedDescr))]
        public event EventHandler DataSourceChanged {
            add {
                Events.AddHandler(EVENT_DATASOURCECHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DATASOURCECHANGED, value);
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.DataManager"]/*' />
        protected CurrencyManager DataManager {
            get {
                return this.dataManager;
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.DisplayMember"]/*' />
        /// <devdoc>
        ///     If the ListBox contains objects that support properties, this indicates
        ///     which property of the object to show.  If "", the object shows it's ToString().
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatData)),
        DefaultValue(""),
        TypeConverterAttribute("System.Windows.Forms.Design.DataMemberFieldConverter, " + AssemblyRef.SystemDesign),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor)),
        SRDescription(nameof(SR.ListControlDisplayMemberDescr))
        ]
        public string DisplayMember {
            get {
                return displayMember.BindingMember;
            }
            set {
                BindingMemberInfo oldDisplayMember = displayMember;
                try {
                    SetDataConnection(dataSource, new BindingMemberInfo(value), false);
                } catch {
                    displayMember = oldDisplayMember;
                }
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.DisplayMemberChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ListControlOnDisplayMemberChangedDescr))]
        public event EventHandler DisplayMemberChanged {
            add {
                Events.AddHandler(EVENT_DISPLAYMEMBERCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DISPLAYMEMBERCHANGED, value);
            }
        }

        // Cached type converter of the property associated with the display member
        private TypeConverter DisplayMemberConverter {
            get {
                if (this.displayMemberConverter == null && 
                    this.DataManager != null &&
                    this.displayMember != null) {
                    PropertyDescriptorCollection props = this.DataManager.GetItemProperties();
                    if (props != null) {
                        PropertyDescriptor displayMemberProperty = props.Find(this.displayMember.BindingField, true);
                        if (displayMemberProperty != null)
                        {
                            this.displayMemberConverter = displayMemberProperty.Converter;
                        }
                    }
                }
                return this.displayMemberConverter;
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.Format"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ListControlFormatDescr))]
        public event ListControlConvertEventHandler Format
        {
            add {
                Events.AddHandler(EVENT_FORMAT, value);
                RefreshItems();
            }
            remove {
                Events.RemoveHandler(EVENT_FORMAT, value);
                RefreshItems();
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.FormatInfo"]/*' />
        [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Advanced),
            DefaultValue(null)
        ]
        public IFormatProvider FormatInfo {
            get {
                return this.formatInfo;
            }
            set {
                if (value != formatInfo) {
                    formatInfo = value;
                    RefreshItems();
                    OnFormatInfoChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.FormatInfoChanged"]/*' />
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.ListControlFormatInfoChangedDescr))
        ]
        public event EventHandler FormatInfoChanged {
            add {
                Events.AddHandler(EVENT_FORMATINFOCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_FORMATINFOCHANGED, value);
            }
        }

        [
            DefaultValue(""), 
            SRDescription(nameof(SR.ListControlFormatStringDescr)),
            EditorAttribute("System.Windows.Forms.Design.FormatStringEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor)),
            MergableProperty(false)
        ]
        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.FormatString"]/*' />
        public string FormatString {
            get {
                return formatString;
            }
            set {
                if (value == null)
                    value = String.Empty;
                if (!value.Equals(formatString)) {
                    formatString = value;
                    RefreshItems();
                    OnFormatStringChanged(EventArgs.Empty);
                }
            }
        }
        
        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.FormatStringChanged"]/*' />
        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.ListControlFormatStringChangedDescr))
        ]
        public event EventHandler FormatStringChanged {
            add {
                Events.AddHandler(EVENT_FORMATSTRINGCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_FORMATSTRINGCHANGED, value);
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.FormattingEnabled"]/*' />
        [
            DefaultValue(false), 
            SRDescription(nameof(SR.ListControlFormattingEnabledDescr))
        ]
        public bool FormattingEnabled {
            get {
                return formattingEnabled;
            }
            set {
                if (value != formattingEnabled) {
                    formattingEnabled = value;
                    RefreshItems();
                    OnFormattingEnabledChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.FormattingEnabledChanged"]/*' />
        [
            SRCategory(nameof(SR.CatPropertyChanged)),
            SRDescription(nameof(SR.ListControlFormattingEnabledChangedDescr))
        ]
        public event EventHandler FormattingEnabledChanged {
            add {
                Events.AddHandler(EVENT_FORMATTINGENABLEDCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_FORMATTINGENABLEDCHANGED, value);
            }
        }

        private bool BindingMemberInfoInDataManager(BindingMemberInfo bindingMemberInfo) {
            if (dataManager == null)
                return false;
            PropertyDescriptorCollection props = dataManager.GetItemProperties();
            int propsCount = props.Count;

            for (int i = 0; i < propsCount; i++) {
                if (typeof(IList).IsAssignableFrom(props[i].PropertyType))
                    continue;
                if (props[i].Name.Equals(bindingMemberInfo.BindingField)) {
                    return true;
                }
            }

            for (int i = 0; i < propsCount; i++) {
                if (typeof(IList).IsAssignableFrom(props[i].PropertyType))
                    continue;
                if (String.Compare(props[i].Name, bindingMemberInfo.BindingField, true, CultureInfo.CurrentCulture) == 0) {
                    return true;
                }
            }

            return false;
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.ValueMember"]/*' />
        [
        SRCategory(nameof(SR.CatData)),
        DefaultValue(""),
        Editor("System.Windows.Forms.Design.DataMemberFieldEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor)),
        SRDescription(nameof(SR.ListControlValueMemberDescr))
        ]
        public string ValueMember {
            get {
                return valueMember.BindingMember;
            }
            set {
                if (value == null)
                    value = "";
                BindingMemberInfo newValueMember = new BindingMemberInfo(value);
                BindingMemberInfo oldValueMember = valueMember;
                if (!newValueMember.Equals(valueMember)) {
                    // If the displayMember is set to the EmptyString, then recreate the dataConnection
                    //
                    if (DisplayMember.Length == 0)
                        SetDataConnection(DataSource, newValueMember, false);
                    // See if the valueMember is a member of 
                    // the properties in the dataManager
                    if (this.dataManager != null && value != null && value.Length != 0)
                        if (!BindingMemberInfoInDataManager(newValueMember)) {
                            throw new ArgumentException(SR.ListControlWrongValueMember, "value");
                        }

                    valueMember = newValueMember;
                    OnValueMemberChanged(EventArgs.Empty);
                    OnSelectedValueChanged(EventArgs.Empty);
                }
            }
        }        

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.ValueMemberChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ListControlOnValueMemberChangedDescr))]
        public event EventHandler ValueMemberChanged {
            add {
                Events.AddHandler(EVENT_VALUEMEMBERCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_VALUEMEMBERCHANGED, value);
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.AllowSelection"]/*' />
        /// <devdoc>
        ///    Indicates whether list currently allows selection of list items.
        /// </devdoc>
        protected virtual bool AllowSelection {
            get {
                return true;
            }
        }
        
        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.SelectedIndex"]/*' />
        public abstract int SelectedIndex {
            get;
            set;
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.SelectedValue"]/*' />
        [
        SRCategory(nameof(SR.CatData)),
        DefaultValue(null),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ListControlSelectedValueDescr)),
        Bindable(true)
        ]        
        public object SelectedValue {
            get {
                if (SelectedIndex != -1 && dataManager != null ) {
                    object currentItem = dataManager[SelectedIndex];
                    object filteredItem = FilterItemOnProperty(currentItem, valueMember.BindingField);
                    return filteredItem;
                }
                return null;
            }
            set {
                if (dataManager != null) {
                    string propertyName = valueMember.BindingField;
                    // we can't set the SelectedValue property when the listManager does not
                    // have a ValueMember set.
                    if (string.IsNullOrEmpty(propertyName))
                        throw new InvalidOperationException(SR.ListControlEmptyValueMemberInSettingSelectedValue);
                    PropertyDescriptorCollection props = dataManager.GetItemProperties();
                    PropertyDescriptor property = props.Find(propertyName, true);
                    int index = dataManager.Find(property, value, true);
                    this.SelectedIndex = index;
                }
            }
        }
        
        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.SelectedValueChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ListControlOnSelectedValueChangedDescr))]
        public event EventHandler SelectedValueChanged {
            add {
                Events.AddHandler(EVENT_SELECTEDVALUECHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_SELECTEDVALUECHANGED, value);
            }
        }

        private void DataManager_PositionChanged(object sender, EventArgs e) {
            if (this.dataManager != null) {
                if (AllowSelection) {
                    this.SelectedIndex = dataManager.Position;
                }
            }
        }

        private void DataManager_ItemChanged(object sender, ItemChangedEventArgs e) {
            // Note this is being called internally with a null event.
            if (dataManager != null) {
                if (e.Index == -1) {
                    SetItemsCore(dataManager.List);
                    if (AllowSelection) {
                        this.SelectedIndex = this.dataManager.Position;
                    }
                }
                else {
                    SetItemCore(e.Index, dataManager[e.Index]);
                }
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.FilterItemOnProperty"]/*' />
        /// <internalonly/>
        protected object FilterItemOnProperty(object item) {
            return FilterItemOnProperty(item, displayMember.BindingField);
        }
        
        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.FilterItemOnProperty1"]/*' />
        /// <internalonly/>
        protected object FilterItemOnProperty(object item, string field) {
            if (item != null && field.Length > 0) {
                try {
                    // if we have a dataSource, then use that to display the string
                    PropertyDescriptor descriptor;
                    if (this.dataManager != null)
                        descriptor = this.dataManager.GetItemProperties().Find(field, true);
                    else
                        descriptor = TypeDescriptor.GetProperties(item).Find(field, true);
                    if (descriptor != null) {
                        item = descriptor.GetValue(item);
                    }
                }
                catch {
                }
            }
            return item;
        }

        //We use  this to prevent getting the selected item when mouse is hovering over the dropdown.
        //
        internal bool BindingFieldEmpty { 
            get {
                return (displayMember.BindingField.Length > 0 ? false : true);
            }

        }

        internal int FindStringInternal(string str, IList items, int startIndex, bool exact) {
            return FindStringInternal(str, items, startIndex, exact, true);
        }

        internal int FindStringInternal(string str, IList items, int startIndex, bool exact, bool ignorecase) {
        
            // Sanity check parameters
            //
            if (str == null || items == null) {
                return -1;
            }
            // The last item in the list is still a valid place to start looking!
            if (startIndex < -1 || startIndex >= items.Count) {
                return -1;
            }
        
            bool found = false;
            int length = str.Length;

            // Start from the start index and wrap around until we find the string 
            // in question.  Use a separate counter to ensure that we arent cycling through the list infinitely.
            int numberOfTimesThroughLoop = 0;

            // this API is really Find NEXT String... 
            for (int index = (startIndex+1) % items.Count; numberOfTimesThroughLoop < items.Count; index = (index+1) % items.Count) {
                numberOfTimesThroughLoop++;
                if (exact) {
                    found = String.Compare(str, GetItemText(items[index]), ignorecase, CultureInfo.CurrentCulture) == 0;
                }
                else {
                    found = String.Compare(str, 0, GetItemText(items[index]), 0, length, ignorecase, CultureInfo.CurrentCulture) == 0;
                }
                
                if (found) {
                    return index;
                }
              
            }
            
            return -1;
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.GetItemText"]/*' />
        public string GetItemText(object item)
        {

            // !formattingEnabled == RTM behaviour
            if (!formattingEnabled) {

                if (item == null) {
                    return String.Empty;
                }

                item = FilterItemOnProperty(item, displayMember.BindingField);
                return (item != null) ? Convert.ToString(item, CultureInfo.CurrentCulture) : "";
            }

            //
            // Whidbey formatting features
            //

            object filteredItem = FilterItemOnProperty(item, displayMember.BindingField);

            // first try: the OnFormat event
            ListControlConvertEventArgs e = new ListControlConvertEventArgs(filteredItem, typeof(String), item);
            OnFormat(e);

            // Microsoft: we need a better check. Should add the Handled property on the ListControlConvertEventArgs?
            if (e.Value != item && e.Value is String) {
                return (string) e.Value;
            }

            // try Formatter::FormatObject
            if (stringTypeConverter == null)
            { 
                stringTypeConverter = TypeDescriptor.GetConverter(typeof(String));
            }  
            try
            {
                return (string) Formatter.FormatObject(filteredItem, typeof(String), this.DisplayMemberConverter, stringTypeConverter, formatString, formatInfo, null, System.DBNull.Value);
            }
            catch (Exception exception)
            {
                if (ClientUtils.IsSecurityOrCriticalException(exception))
                {
                    throw;
                }
                // if we did not do any work then return the old ItemText
                return (filteredItem != null) ? Convert.ToString(item, CultureInfo.CurrentCulture) : "";
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.IsInputKey"]/*' />
        /// <devdoc>
        ///      Handling special input keys, such as pgup, pgdown, home, end, etc...
        /// </devdoc>
        protected override bool IsInputKey(Keys keyData) {
            if ((keyData & Keys.Alt) == Keys.Alt) return false;
            switch (keyData & Keys.KeyCode) {
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                    return true;
            }
            return base.IsInputKey(keyData);
        }
  
        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.OnBindingContextChanged"]/*' />
        protected override void OnBindingContextChanged(EventArgs e) {
            SetDataConnection(dataSource, displayMember, true);
            
            base.OnBindingContextChanged(e);
        }


        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.OnDataSourceChanged"]/*' />
        protected virtual void OnDataSourceChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_DATASOURCECHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.OnDisplayMemberChanged"]/*' />
        protected virtual void OnDisplayMemberChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_DISPLAYMEMBERCHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.OnFormat"]/*' />
        protected virtual void OnFormat(ListControlConvertEventArgs e) {
            ListControlConvertEventHandler eh = Events[EVENT_FORMAT] as ListControlConvertEventHandler;
            if (eh != null)
                eh(this, e);
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.OnFormatInfoChanged"]/*' />
        protected virtual void OnFormatInfoChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_FORMATINFOCHANGED] as EventHandler;
            if (eh != null)
                eh(this,e);
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.OnFormatStringChanged"]/*' />
        protected virtual void OnFormatStringChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_FORMATSTRINGCHANGED] as EventHandler;
            if (eh != null)
                eh(this,e);
        }
        
        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.OnFormattingEnabledChanged"]/*' />
        protected virtual void OnFormattingEnabledChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_FORMATTINGENABLEDCHANGED] as EventHandler;
            if (eh != null)
                eh(this,e);
        }
        
        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.OnSelectedIndexChanged"]/*' />
        /// <devdoc>
        ///     Actually goes and fires the selectedIndexChanged event.  Inheriting controls
        ///     should use this to know when the event is fired [this is preferable to
        ///     adding an event handler on yourself for this event].  They should,
        ///     however, remember to call base.OnSelectedIndexChanged(e); to ensure the event is
        ///     still fired to external listeners
        /// </devdoc>
        protected virtual void OnSelectedIndexChanged(EventArgs e) {
            OnSelectedValueChanged(EventArgs.Empty);
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.OnValueMemberChanged"]/*' />
        protected virtual void OnValueMemberChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_VALUEMEMBERCHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.OnSelectedValueChanged"]/*' />
        protected virtual void OnSelectedValueChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_SELECTEDVALUECHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.RefreshItem"]/*' />
        protected abstract void RefreshItem(int index);
        
        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.RefreshItems"]/*' />
        protected virtual void RefreshItems() {
        }
        
        private void DataSourceDisposed(object sender, EventArgs e) {
            Debug.Assert(sender == this.dataSource, "how can we get dispose notification for anything other than our dataSource?");
            SetDataConnection(null, new BindingMemberInfo(""), true);
        }

        private void DataSourceInitialized(object sender, EventArgs e) {
            ISupportInitializeNotification dsInit = (this.dataSource as ISupportInitializeNotification);
            Debug.Assert(dsInit != null, "ListControl: ISupportInitializeNotification.Initialized event received, but current DataSource does not support ISupportInitializeNotification!");
            Debug.Assert(dsInit.IsInitialized, "ListControl: DataSource sent ISupportInitializeNotification.Initialized event but before it had finished initializing.");
            SetDataConnection(this.dataSource, this.displayMember, true);
        }

        private void SetDataConnection(object newDataSource, BindingMemberInfo newDisplayMember, bool force) {
            bool dataSourceChanged = dataSource != newDataSource;
            bool displayMemberChanged = !displayMember.Equals(newDisplayMember);

            // make sure something interesting is happening...
            //
            //force = force && (dataSource != null || newDataSource != null);
            if (inSetDataConnection) {
                return;
            }
            try {
                if (force || dataSourceChanged || displayMemberChanged) {
                    inSetDataConnection = true;
                    IList currentList = this.DataManager != null ? this.DataManager.List : null;
                    bool currentManagerIsNull = this.DataManager == null;

                    UnwireDataSource();

                    dataSource = newDataSource;
                    displayMember = newDisplayMember;

                    WireDataSource();

                    // Provided the data source has been fully initialized, start listening to change events on its
                    // currency manager and refresh our list. If the data source has not yet been initialized, we will
                    // skip this step for now, and try again later (once the data source has fired its Initialized event).
                    //
                    if (isDataSourceInitialized) {

                        CurrencyManager newDataManager = null;
                        if (newDataSource != null && BindingContext != null && !(newDataSource == Convert.DBNull)) {
                            newDataManager = (CurrencyManager)BindingContext[newDataSource, newDisplayMember.BindingPath];
                        }

                        if (dataManager != newDataManager) {
                            if (dataManager != null) {
                                dataManager.ItemChanged -= new ItemChangedEventHandler(DataManager_ItemChanged);
                                dataManager.PositionChanged -= new EventHandler(DataManager_PositionChanged);
                            }

                            dataManager = newDataManager;

                            if (dataManager != null) {
                                dataManager.ItemChanged += new ItemChangedEventHandler(DataManager_ItemChanged);
                                dataManager.PositionChanged += new EventHandler(DataManager_PositionChanged);
                            }
                        }

                        // See if the BindingField in the newDisplayMember is valid
                        // The same thing if dataSource Changed
                        // "" is a good value for displayMember
                        if (dataManager != null && (displayMemberChanged || dataSourceChanged) && displayMember.BindingMember != null && displayMember.BindingMember.Length != 0) {

                            if (!BindingMemberInfoInDataManager(displayMember))
                                throw new ArgumentException(SR.ListControlWrongDisplayMember, "newDisplayMember");
                        }

                        if (dataManager != null && (dataSourceChanged || displayMemberChanged || force)) {
                            // if we force a new data manager, then change the items in the list control
                            // only if the list changed or if we go from a null dataManager to a full fledged one
                            // or if the DisplayMember changed
                            if (displayMemberChanged || (force && (currentList != this.dataManager.List || currentManagerIsNull))) {
                                DataManager_ItemChanged(dataManager, new ItemChangedEventArgs(-1));
                            }
                        }
                    }
                    this.displayMemberConverter = null;
                }

                if (dataSourceChanged) {
                    OnDataSourceChanged(EventArgs.Empty);
                }

                if (displayMemberChanged) {
                    OnDisplayMemberChanged(EventArgs.Empty);
                }
            }
            finally {
                inSetDataConnection = false;
            }
        }

        private void UnwireDataSource() {
                // If the source is a component, then unhook the Disposed event
                if (this.dataSource is IComponent) {
                    ((IComponent) this.dataSource).Disposed -= new EventHandler(DataSourceDisposed);
                }

                ISupportInitializeNotification dsInit = (this.dataSource as ISupportInitializeNotification);

                if (dsInit != null && isDataSourceInitEventHooked) {
                    // If we previously hooked the data source's ISupportInitializeNotification
                    // Initialized event, then unhook it now (we don't always hook this event,
                    // only if we needed to because the data source was previously uninitialized)
                    dsInit.Initialized -= new EventHandler(DataSourceInitialized);
                    isDataSourceInitEventHooked = false;
                }
        }

        private void WireDataSource() {
                // If the source is a component, then hook the Disposed event,
                // so we know when the component is deleted from the form
                if (this.dataSource is IComponent) {
                    ((IComponent) this.dataSource).Disposed += new EventHandler(DataSourceDisposed);
                }

                ISupportInitializeNotification dsInit = (this.dataSource as ISupportInitializeNotification);

                if (dsInit != null && !dsInit.IsInitialized) {
                    // If the source provides initialization notification, and is not yet
                    // fully initialized, then hook the Initialized event, so that we can
                    // delay connecting to it until it *is* initialized.
                    dsInit.Initialized += new EventHandler(DataSourceInitialized);
                    isDataSourceInitEventHooked = true;
                    isDataSourceInitialized = false;
                }
                else {
                    // Otherwise either the data source says it *is* initialized, or it
                    // does not support the capability to report whether its initialized,
                    // in which case we have to just assume it that is initialized.
                    isDataSourceInitialized = true;
                }
        }

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.SetItemsCore"]/*' />
        protected abstract void SetItemsCore(IList items);

        /// <include file='doc\ListControl.uex' path='docs/doc[@for="ListControl.SetItemCore"]/*' />
        protected virtual void SetItemCore(int index, object value) {}
    }
}

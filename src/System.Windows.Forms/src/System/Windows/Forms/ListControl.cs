// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing.Design;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [LookupBindingProperties(nameof(ListControl.DataSource), nameof(ListControl.DisplayMember), nameof(ListControl.ValueMember), nameof(ListControl.SelectedValue))]
    public abstract class ListControl : Control
    {
        private static readonly object EVENT_DATASOURCECHANGED = new object();
        private static readonly object EVENT_DISPLAYMEMBERCHANGED = new object();
        private static readonly object EVENT_VALUEMEMBERCHANGED = new object();
        private static readonly object EVENT_SELECTEDVALUECHANGED = new object();
        private static readonly object EVENT_FORMATINFOCHANGED = new object();
        private static readonly object EVENT_FORMATSTRINGCHANGED = new object();
        private static readonly object EVENT_FORMATTINGENABLEDCHANGED = new object();

        private object dataSource;
        private CurrencyManager dataManager;
        private BindingMemberInfo displayMember;
        private BindingMemberInfo valueMember;

        // Formatting stuff
        private string formatString = string.Empty;
        private IFormatProvider formatInfo = null;
        private bool formattingEnabled = false;
        private static readonly object EVENT_FORMAT = new object();
        private TypeConverter displayMemberConverter = null;
        private static TypeConverter stringTypeConverter = null;

        private bool isDataSourceInitialized;
        private bool isDataSourceInitEventHooked;
        private bool inSetDataConnection = false;

        /// <devdoc>
        /// The ListSource to consume as this ListBox's source of data.
        /// When set, a user can not modify the Items collection.
        /// </devdoc>
        [SRCategory(nameof(SR.CatData))]
        [DefaultValue(null)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [AttributeProvider(typeof(IListSource))]
        [SRDescription(nameof(SR.ListControlDataSourceDescr))]
        public object DataSource
        {
            get => dataSource;
            set
            {
                if (value != null && !(value is IList || value is IListSource))
                {
                    throw new ArgumentException(SR.BadDataSourceForComplexBinding);
                }

                if (dataSource == value)
                {
                    return;
                }

                // When we change the dataSource to null, we should reset
                // the displayMember to "".
                try
                {
                    SetDataConnection(value, displayMember, force: false);
                }
                catch
                {
                    // There are several possibilities why setting the data source throws an exception:
                    // 1. the app throws an exception in the events that fire when we change the data source: DataSourceChanged, 
                    // 2. we get an exception when we set the data source and populate the list controls (say,something went wrong while formatting the data)
                    // 3. the DisplayMember does not fit w/ the new data source (this could happen if the user resets the data source but did not reset the DisplayMember)
                    // in all cases ListControl should reset the DisplayMember to String.Empty
                    // the ListControl should also eat the exception - this is the RTM behavior and doing anything else is a breaking change
                    DisplayMember = string.Empty;
                }
                if (value == null)
                {
                    DisplayMember = string.Empty;
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ListControlOnDataSourceChangedDescr))]
        public event EventHandler DataSourceChanged
        {
            add
            {
                Events.AddHandler(EVENT_DATASOURCECHANGED, value);
            }
            remove
            {
                Events.RemoveHandler(EVENT_DATASOURCECHANGED, value);
            }
        }

        protected CurrencyManager DataManager => dataManager;

        /// <devdoc>
        /// If the ListBox contains objects that support properties, this indicates
        /// which property of the object to show. If "", the object shows it's ToString().
        /// </devdoc>
        [SRCategory(nameof(SR.CatData))]
        [DefaultValue("")]
        [TypeConverterAttribute("System.Windows.Forms.Design.DataMemberFieldConverter, " + AssemblyRef.SystemDesign)]
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [SRDescription(nameof(SR.ListControlDisplayMemberDescr))]
        public string DisplayMember
        {
            get => displayMember.BindingMember;
            set
            {
                BindingMemberInfo oldDisplayMember = displayMember;
                try
                {
                    SetDataConnection(dataSource, new BindingMemberInfo(value), force: false);
                }
                catch
                {
                    displayMember = oldDisplayMember;
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ListControlOnDisplayMemberChangedDescr))]
        public event EventHandler DisplayMemberChanged
        {
            add
            {
                Events.AddHandler(EVENT_DISPLAYMEMBERCHANGED, value);
            }
            remove
            {
                Events.RemoveHandler(EVENT_DISPLAYMEMBERCHANGED, value);
            }
        }

        /// <summary>
        /// Cached type converter of the property associated with the display member
        /// </summary>
        private TypeConverter DisplayMemberConverter
        {
            get
            {
                if (displayMemberConverter == null &&
                    DataManager != null &&
                    displayMember != null)
                {
                    PropertyDescriptorCollection props = DataManager.GetItemProperties();
                    if (props != null)
                    {
                        PropertyDescriptor displayMemberProperty = props.Find(displayMember.BindingField, true);
                        if (displayMemberProperty != null)
                        {
                            displayMemberConverter = displayMemberProperty.Converter;
                        }
                    }
                }

                return displayMemberConverter;
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ListControlFormatDescr))]
        public event ListControlConvertEventHandler Format
        {
            add
            {
                Events.AddHandler(EVENT_FORMAT, value);
                RefreshItems();
            }
            remove
            {
                Events.RemoveHandler(EVENT_FORMAT, value);
                RefreshItems();
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DefaultValue(null)]
        public IFormatProvider FormatInfo
        {
            get => formatInfo;
            set
            {
                if (value != formatInfo)
                {
                    formatInfo = value;
                    RefreshItems();
                    OnFormatInfoChanged(EventArgs.Empty);
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ListControlFormatInfoChangedDescr))]
        public event EventHandler FormatInfoChanged
        {
            add
            {
                Events.AddHandler(EVENT_FORMATINFOCHANGED, value);
            }
            remove
            {
                Events.RemoveHandler(EVENT_FORMATINFOCHANGED, value);
            }
        }

        [DefaultValue("")]
        [SRDescription(nameof(SR.ListControlFormatStringDescr))]
        [EditorAttribute("System.Windows.Forms.Design.FormatStringEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [MergableProperty(false)]
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
                    RefreshItems();
                    OnFormatStringChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ListControlFormatStringChangedDescr))]
        public event EventHandler FormatStringChanged
        {
            add
            {
                Events.AddHandler(EVENT_FORMATSTRINGCHANGED, value);
            }
            remove
            {
                Events.RemoveHandler(EVENT_FORMATSTRINGCHANGED, value);
            }
        }

        [DefaultValue(false)]
        [SRDescription(nameof(SR.ListControlFormattingEnabledDescr))]
        public bool FormattingEnabled
        {
            get => formattingEnabled;
            set
            {
                if (value != formattingEnabled)
                {
                    formattingEnabled = value;
                    RefreshItems();
                    OnFormattingEnabledChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ListControlFormattingEnabledChangedDescr))]
        public event EventHandler FormattingEnabledChanged
        {
            add
            {
                Events.AddHandler(EVENT_FORMATTINGENABLEDCHANGED, value);
            }
            remove
            {
                Events.RemoveHandler(EVENT_FORMATTINGENABLEDCHANGED, value);
            }
        }

        private bool BindingMemberInfoInDataManager(BindingMemberInfo bindingMemberInfo)
        {
            if (dataManager == null)
            {
                return false;
            }

            PropertyDescriptorCollection props = dataManager.GetItemProperties();
            int propsCount = props.Count;

            for (int i = 0; i < propsCount; i++)
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

            for (int i = 0; i < propsCount; i++)
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
        [Editor("System.Windows.Forms.Design.DataMemberFieldEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [SRDescription(nameof(SR.ListControlValueMemberDescr))]
        public string ValueMember
        {
            get => valueMember.BindingMember;
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                BindingMemberInfo newValueMember = new BindingMemberInfo(value);
                BindingMemberInfo oldValueMember = valueMember;
                if (!newValueMember.Equals(valueMember))
                {
                    // If the displayMember is set to the EmptyString, then recreate the dataConnection
                    if (DisplayMember.Length == 0)
                    {
                        SetDataConnection(DataSource, newValueMember, force: false);
                    }
    
                    // See if the valueMember is a member of 
                    // the properties in the dataManager
                    if (DataManager != null && !string.IsNullOrEmpty(value))
                    {
                        if (!BindingMemberInfoInDataManager(newValueMember))
                        {
                            throw new ArgumentException(SR.ListControlWrongValueMember, nameof(value));
                        }
                    }

                    valueMember = newValueMember;
                    OnValueMemberChanged(EventArgs.Empty);
                    OnSelectedValueChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ListControlOnValueMemberChangedDescr))]
        public event EventHandler ValueMemberChanged
        {
            add
            {
                Events.AddHandler(EVENT_VALUEMEMBERCHANGED, value);
            }
            remove
            {
                Events.RemoveHandler(EVENT_VALUEMEMBERCHANGED, value);
            }
        }

        /// <devdoc>
        /// Indicates whether list currently allows selection of list items.
        /// </devdoc>
        protected virtual bool AllowSelection => true;

        public abstract int SelectedIndex { get; set; }

        [SRCategory(nameof(SR.CatData))]
        [DefaultValue(null)]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ListControlSelectedValueDescr))]
        [Bindable(true)]
        public object SelectedValue
        {
            get
            {
                if (SelectedIndex != -1 && dataManager != null)
                {
                    object currentItem = dataManager[SelectedIndex];
                    return FilterItemOnProperty(currentItem, valueMember.BindingField);
                }

                return null;
            }
            set
            {
                if (dataManager != null)
                {
                    string propertyName = valueMember.BindingField;
                    // We can't set the SelectedValue property when the listManager does not
                    // have a ValueMember set.
                    if (string.IsNullOrEmpty(propertyName))
                    {
                        throw new InvalidOperationException(SR.ListControlEmptyValueMemberInSettingSelectedValue);
                    }

                    PropertyDescriptorCollection props = dataManager.GetItemProperties();
                    PropertyDescriptor property = props.Find(propertyName, true);
                    int index = dataManager.Find(property, value, true);
                    SelectedIndex = index;
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ListControlOnSelectedValueChangedDescr))]
        public event EventHandler SelectedValueChanged
        {
            add
            {
                Events.AddHandler(EVENT_SELECTEDVALUECHANGED, value);
            }
            remove
            {
                Events.RemoveHandler(EVENT_SELECTEDVALUECHANGED, value);
            }
        }

        private void DataManager_PositionChanged(object sender, EventArgs e)
        {
            if (DataManager != null)
            {
                if (AllowSelection)
                {
                    SelectedIndex = dataManager.Position;
                }
            }
        }

        private void DataManager_ItemChanged(object sender, ItemChangedEventArgs e)
        {
            // Note this is being called internally with a null event.
            if (dataManager != null)
            {
                if (e.Index == -1)
                {
                    SetItemsCore(dataManager.List);
                    if (AllowSelection)
                    {
                        SelectedIndex = DataManager.Position;
                    }
                }
                else
                {
                    SetItemCore(e.Index, dataManager[e.Index]);
                }
            }
        }

        protected object FilterItemOnProperty(object item)
        {
            return FilterItemOnProperty(item, displayMember.BindingField);
        }

        protected object FilterItemOnProperty(object item, string field)
        {
            if (item != null && field.Length > 0)
            {
                try
                {
                    // if we have a dataSource, then use that to display the string
                    PropertyDescriptor descriptor;
                    if (DataManager != null)
                    {
                        descriptor = DataManager.GetItemProperties().Find(field, true);
                    }
                    else
                    {
                        descriptor = TypeDescriptor.GetProperties(item).Find(field, true);
                    }
                    if (descriptor != null)
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
        /// We use this to prevent getting the selected item when mouse is hovering
        /// over the dropdown.
        /// </remarks>
        private protected bool BindingFieldEmpty => displayMember.BindingField.Length == 0;

        private protected int FindStringInternal(string str, IList items, int startIndex, bool exact, bool ignoreCase)
        {
            if (str == null || items == null)
            {
                return -1;
            }

            // The last item in the list is still a valid place to start looking.
            if (startIndex < -1 || startIndex >= items.Count)
            {
                return -1;
            }

            bool found = false;
            int length = str.Length;

            // Start from the start index and wrap around until we find the string 
            // in question. Use a separate counter to ensure that we arent cycling through the list infinitely.
            int numberOfTimesThroughLoop = 0;

            // this API is really Find NEXT String... 
            for (int index = (startIndex + 1) % items.Count; numberOfTimesThroughLoop < items.Count; index = (index + 1) % items.Count)
            {
                numberOfTimesThroughLoop++;
                if (exact)
                {
                    found = string.Compare(str, GetItemText(items[index]), ignoreCase, CultureInfo.CurrentCulture) == 0;
                }
                else
                {
                    found = string.Compare(str, 0, GetItemText(items[index]), 0, length, ignoreCase, CultureInfo.CurrentCulture) == 0;
                }

                if (found)
                {
                    return index;
                }
            }

            return -1;
        }

        public string GetItemText(object item)
        {
            if (!formattingEnabled)
            {
                if (item == null)
                {
                    return string.Empty;
                }

                item = FilterItemOnProperty(item, displayMember.BindingField);
                if (item == null)
                {
                    return string.Empty;
                }

                return Convert.ToString(item, CultureInfo.CurrentCulture);
            }

            object filteredItem = FilterItemOnProperty(item, displayMember.BindingField);

            // First try the OnFormat event
            var e = new ListControlConvertEventArgs(filteredItem, typeof(string), item);
            OnFormat(e);
            if (e.Value != item && e.Value is string stringValue)
            {
                return stringValue;
            }

            // Try Formatter.FormatObject
            if (stringTypeConverter == null)
            {
                stringTypeConverter = TypeDescriptor.GetConverter(typeof(string));
            }
            try
            {
                return (string)Formatter.FormatObject(filteredItem, typeof(string), DisplayMemberConverter, stringTypeConverter, formatString, formatInfo, null, DBNull.Value);
            }
            catch (Exception exception) when (!ClientUtils.IsSecurityOrCriticalException(exception))
            {
                if (filteredItem == null)
                {
                    return string.Empty;
                }

                // if we did not do any work then return the old ItemText
                return Convert.ToString(item, CultureInfo.CurrentCulture);
            }
        }

        /// <devdoc>
        /// Handling special input keys, such as PageUp, PageDown, Home, End, etc...
        /// </devdoc>
        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.Alt) == Keys.Alt)
            {
                return false;
            }

            switch (keyData & Keys.KeyCode)
            {
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                    return true;
            }

            return base.IsInputKey(keyData);
        }

        protected override void OnBindingContextChanged(EventArgs e)
        {
            SetDataConnection(dataSource, displayMember, force: true);
            base.OnBindingContextChanged(e);
        }

        protected virtual void OnDataSourceChanged(EventArgs e)
        {
            EventHandler eh = Events[EVENT_DATASOURCECHANGED] as EventHandler;
            eh?.Invoke(this, e);
        }

        protected virtual void OnDisplayMemberChanged(EventArgs e)
        {
            EventHandler eh = Events[EVENT_DISPLAYMEMBERCHANGED] as EventHandler;
            eh?.Invoke(this, e);
        }

        protected virtual void OnFormat(ListControlConvertEventArgs e)
        {
            ListControlConvertEventHandler eh = Events[EVENT_FORMAT] as ListControlConvertEventHandler;
            eh?.Invoke(this, e);
        }

        protected virtual void OnFormatInfoChanged(EventArgs e)
        {
            EventHandler eh = Events[EVENT_FORMATINFOCHANGED] as EventHandler;
            eh?.Invoke(this, e);
        }

        protected virtual void OnFormatStringChanged(EventArgs e)
        {
            EventHandler eh = Events[EVENT_FORMATSTRINGCHANGED] as EventHandler;
            eh?.Invoke(this, e);
        }

        protected virtual void OnFormattingEnabledChanged(EventArgs e)
        {
            EventHandler eh = Events[EVENT_FORMATTINGENABLEDCHANGED] as EventHandler;
            eh?.Invoke(this, e);
        }

        /// <devdoc>
        /// Actually goes and fires the selectedIndexChanged event. Inheriting controls
        /// should use this to know when the event is fired [this is preferable to
        /// adding an event handler on yourself for this event]. They should,
        /// however, remember to call base.OnSelectedIndexChanged(e); to ensure the event is
        /// still fired to external listeners
        /// </devdoc>
        protected virtual void OnSelectedIndexChanged(EventArgs e)
        {
            OnSelectedValueChanged(EventArgs.Empty);
        }

        protected virtual void OnValueMemberChanged(EventArgs e)
        {
            EventHandler eh = Events[EVENT_VALUEMEMBERCHANGED] as EventHandler;
            eh?.Invoke(this, e);
        }

        protected virtual void OnSelectedValueChanged(EventArgs e)
        {
            EventHandler eh = Events[EVENT_SELECTEDVALUECHANGED] as EventHandler;
            eh?.Invoke(this, e);
        }

        protected abstract void RefreshItem(int index);

        protected virtual void RefreshItems()
        {
        }

        private void DataSourceDisposed(object sender, EventArgs e)
        {
            Debug.Assert(sender == dataSource, "how can we get dispose notification for anything other than our dataSource?");
            SetDataConnection(null, new BindingMemberInfo(string.Empty), true);
        }

        private void DataSourceInitialized(object sender, EventArgs e)
        {
            ISupportInitializeNotification dsInit = (dataSource as ISupportInitializeNotification);
            Debug.Assert(dsInit != null, "ListControl: ISupportInitializeNotification.Initialized event received, but current DataSource does not support ISupportInitializeNotification!");
            Debug.Assert(dsInit.IsInitialized, "ListControl: DataSource sent ISupportInitializeNotification.Initialized event but before it had finished initializing.");
            SetDataConnection(dataSource, displayMember, true);
        }

        private void SetDataConnection(object newDataSource, BindingMemberInfo newDisplayMember, bool force)
        {
            bool dataSourceChanged = dataSource != newDataSource;
            bool displayMemberChanged = !displayMember.Equals(newDisplayMember);

            if (inSetDataConnection)
            {
                return;
            }
            try
            {
                if (force || dataSourceChanged || displayMemberChanged)
                {
                    inSetDataConnection = true;
                    IList currentList = DataManager?.List;
                    bool currentManagerIsNull = DataManager == null;

                    UnwireDataSource();

                    dataSource = newDataSource;
                    displayMember = newDisplayMember;

                    WireDataSource();

                    // Provided the data source has been fully initialized, start listening to change events on its
                    // currency manager and refresh our list. If the data source has not yet been initialized, we will
                    // skip this step for now, and try again later (once the data source has fired its Initialized event).
                    if (isDataSourceInitialized)
                    {
                        CurrencyManager newDataManager = null;
                        if (newDataSource != null && BindingContext != null && newDataSource != Convert.DBNull)
                        {
                            newDataManager = (CurrencyManager)BindingContext[newDataSource, newDisplayMember.BindingPath];
                        }

                        if (dataManager != newDataManager)
                        {
                            if (dataManager != null)
                            {
                                dataManager.ItemChanged -= new ItemChangedEventHandler(DataManager_ItemChanged);
                                dataManager.PositionChanged -= new EventHandler(DataManager_PositionChanged);
                            }

                            dataManager = newDataManager;

                            if (dataManager != null)
                            {
                                dataManager.ItemChanged += new ItemChangedEventHandler(DataManager_ItemChanged);
                                dataManager.PositionChanged += new EventHandler(DataManager_PositionChanged);
                            }
                        }

                        // See if the BindingField in the newDisplayMember is valid
                        // The same thing if dataSource Changed
                        // "" is a good value for displayMember
                        if (dataManager != null && (displayMemberChanged || dataSourceChanged) && !string.IsNullOrEmpty(displayMember.BindingMember))
                        {
                            if (!BindingMemberInfoInDataManager(displayMember))
                            {
                                throw new ArgumentException(SR.ListControlWrongDisplayMember, nameof(newDisplayMember));
                            }
                        }

                        if (dataManager != null && (dataSourceChanged || displayMemberChanged || force))
                        {
                            // If we force a new data manager, then change the items in the list control
                            // only if the list changed or if we go from a null dataManager to a full fledged one
                            // or if the DisplayMember changed
                            if (displayMemberChanged || (force && (currentList != DataManager.List || currentManagerIsNull)))
                            {
                                DataManager_ItemChanged(dataManager, new ItemChangedEventArgs(-1));
                            }
                        }
                    }

                    displayMemberConverter = null;
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
                inSetDataConnection = false;
            }
        }

        private void UnwireDataSource()
        {
            // If the source is a component, then unhook the Disposed event
            if (dataSource is IComponent componentDataSource)
            {
                componentDataSource.Disposed -= new EventHandler(DataSourceDisposed);
            }

            if (dataSource is ISupportInitializeNotification dsInit && isDataSourceInitEventHooked)
            {
                // If we previously hooked the data source's ISupportInitializeNotification
                // Initialized event, then unhook it now (we don't always hook this event,
                // only if we needed to because the data source was previously uninitialized)
                dsInit.Initialized -= new EventHandler(DataSourceInitialized);
                isDataSourceInitEventHooked = false;
            }
        }

        private void WireDataSource()
        {
            // If the source is a component, then hook the Disposed event,
            // so we know when the component is deleted from the form
            if (dataSource is IComponent componentDataSource)
            {
                componentDataSource.Disposed += new EventHandler(DataSourceDisposed);
            }

            if (dataSource is ISupportInitializeNotification dsInit && !dsInit.IsInitialized)
            {
                // If the source provides initialization notification, and is not yet
                // fully initialized, then hook the Initialized event, so that we can
                // delay connecting to it until it *is* initialized.
                dsInit.Initialized += new EventHandler(DataSourceInitialized);
                isDataSourceInitEventHooked = true;
                isDataSourceInitialized = false;
            }
            else
            {
                // Otherwise either the data source says it *is* initialized, or it
                // does not support the capability to report whether its initialized,
                // in which case we have to just assume it that is initialized.
                isDataSourceInitialized = true;
            }
        }

        protected abstract void SetItemsCore(IList items);

        protected virtual void SetItemCore(int index, object value)
        {
        }
    }
}

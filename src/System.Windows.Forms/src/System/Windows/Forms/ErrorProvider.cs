// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Threading;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;
    using System.Windows.Forms.Internal;    
    using System.Security.Permissions;
    using System.Runtime.Versioning;

    /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider"]/*' />
    /// <devdoc>
    ///     ErrorProvider presents a simple user interface for indicating to the
    ///     user that a control on a form has an error associated with it.  If a
    ///     error description string is specified for the control, then an icon
    ///     will appear next to the control, and when the mouse hovers over the
    ///     icon, a tooltip will appear showing the error description string.
    /// </devdoc>
    [
    ProvideProperty("IconPadding", typeof(Control)),
    ProvideProperty("IconAlignment", typeof(Control)),
    ProvideProperty("Error", typeof(Control)),
    ToolboxItemFilter("System.Windows.Forms"),
    ComplexBindingProperties(nameof(DataSource), nameof(DataMember)),
    SRDescription(nameof(SR.DescriptionErrorProvider))
    ]
    public class ErrorProvider : Component, IExtenderProvider, ISupportInitialize {

        //
        // FIELDS
        //

        Hashtable items = new Hashtable();
        Hashtable windows = new Hashtable();
        Icon icon = DefaultIcon;
        IconRegion region;
        int itemIdCounter;
        int blinkRate;
        ErrorBlinkStyle blinkStyle;
        bool showIcon = true;                       // used for blinking
        private bool inSetErrorManager = false;
        private bool setErrorManagerOnEndInit = false;
        private bool initializing = false;
        [ThreadStatic]
        static Icon defaultIcon = null;
        const int defaultBlinkRate = 250;
        const ErrorBlinkStyle defaultBlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
        const ErrorIconAlignment defaultIconAlignment = ErrorIconAlignment.MiddleRight;

        // data binding
        private ContainerControl parentControl;
        private object dataSource = null;
        private string dataMember = null;
        private BindingManagerBase errorManager;
        private EventHandler currentChanged;

        // listen to the OnPropertyChanged event in the ContainerControl
        private EventHandler propChangedEvent;

        private EventHandler onRightToLeftChanged;
        private bool rightToLeft = false;
        
        private object userData;

        //
        // CONSTRUCTOR
        //

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ErrorProvider"]/*' />
        /// <devdoc>
        ///     Default constructor.
        /// </devdoc>
        public ErrorProvider() {
            icon = DefaultIcon;
            blinkRate = defaultBlinkRate;
            blinkStyle = defaultBlinkStyle;
            currentChanged = new EventHandler(ErrorManager_CurrentChanged);
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ErrorProvider1"]/*' />
        public ErrorProvider(ContainerControl parentControl) : this() {
            this.parentControl = parentControl;
            propChangedEvent = new EventHandler(ParentControl_BindingContextChanged);
            parentControl.BindingContextChanged += propChangedEvent;
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ErrorProvider2"]/*' />
        public ErrorProvider(IContainer container) : this() {
            if (container == null) {
                throw new ArgumentNullException(nameof(container));
            }

            container.Add(this);
        }

        //
        // PROPERTIES
        //

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.Site"]/*' />
        public override ISite Site {
            set {
                base.Site = value;
                if (value == null)
                    return;

                IDesignerHost host = value.GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (host != null) {
                    IComponent baseComp = host.RootComponent;

                    if (baseComp is ContainerControl) {
                        this.ContainerControl = (ContainerControl) baseComp;
                    }
                }
            }
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.BlinkStyle"]/*' />
        /// <devdoc>
        ///     Returns or sets when the error icon flashes.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(defaultBlinkStyle),
        SRDescription(nameof(SR.ErrorProviderBlinkStyleDescr))
        ]
        public ErrorBlinkStyle BlinkStyle {
            get {
                if (blinkRate == 0) {
                    return ErrorBlinkStyle.NeverBlink;
                }
                return blinkStyle;
            }
            set {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ErrorBlinkStyle.BlinkIfDifferentError, (int)ErrorBlinkStyle.NeverBlink)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ErrorBlinkStyle));
                }

                // If the blinkRate == 0, then set blinkStyle = neverBlink
                //
                if (this.blinkRate == 0) {
                    value = ErrorBlinkStyle.NeverBlink;
                }

                if (this.blinkStyle == value) {
                    return;
                }

                if (value == ErrorBlinkStyle.AlwaysBlink) {
                    // we need to startBlinking on all the controlItems
                    // in our items hashTable.
                    this.showIcon = true;
                    this.blinkStyle = ErrorBlinkStyle.AlwaysBlink;
                    foreach (ErrorWindow w in windows.Values)
                    {
                        w.StartBlinking();
                    }
                }
                else if (blinkStyle == ErrorBlinkStyle.AlwaysBlink) {
                    // we need to stop blinking...
                    this.blinkStyle = value;
                    foreach (ErrorWindow w in windows.Values) {
                        w.StopBlinking();
                    }
                }
                else {
                    this.blinkStyle = value;
                }
            }
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ContainerControl"]/*' />
        /// <devdoc>
        ///    Indicates what container control (usually the form) should be inspected for bindings.
        ///    A binding will reveal what control to place errors on for a
        ///    error in the current row in the DataSource/DataMember pair.
        /// </devdoc>
        [
        DefaultValue(null),
        SRCategory(nameof(SR.CatData)),
        SRDescription(nameof(SR.ErrorProviderContainerControlDescr))
        ]
        public ContainerControl ContainerControl {
            [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
            [UIPermission(SecurityAction.InheritanceDemand, Window=UIPermissionWindow.AllWindows)]
            get {
                return parentControl;
            }
            set {
                if (parentControl != value) {
                    if (parentControl != null)
                        parentControl.BindingContextChanged -= propChangedEvent;

                    parentControl = value;

                    if (parentControl != null)
                        parentControl.BindingContextChanged += propChangedEvent;

                    Set_ErrorManager(this.DataSource, this.DataMember, true);
                }
            }
        }

        /// <include file='doc\DateTimePicker.uex' path='docs/doc[@for="DateTimePicker.RightToLeft"]/*' />
        /// <devdoc>
        ///     This is used for international applications where the language
        ///     is written from RightToLeft. When this property is true,
        //      text will be from right to left.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(false),
        SRDescription(nameof(SR.ControlRightToLeftDescr))
        ]
        public virtual bool RightToLeft {
            get {

                return rightToLeft;
            }

            set {
                if (value != rightToLeft) {
                    rightToLeft = value;
                    OnRightToLeftChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.RightToLeftChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnRightToLeftChangedDescr))]
        public event EventHandler RightToLeftChanged {
            add {
                onRightToLeftChanged += value;
            }
            remove {
                onRightToLeftChanged -= value;
            }
        }        

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.Tag"]/*' />
        /// <devdoc>
        ///    User defined data associated with the control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatData)),
        Localizable(false),
        Bindable(true),
        SRDescription(nameof(SR.ControlTagDescr)),
        DefaultValue(null),
        TypeConverter(typeof(StringConverter)),
        ]
        public object Tag {
            get {
                return userData;
            }
            set {
                userData = value;
            }
        }

        private void Set_ErrorManager(object newDataSource, string newDataMember, bool force) {
            if (inSetErrorManager)
                return;
            inSetErrorManager = true;
            try
            {
                bool dataSourceChanged = this.DataSource != newDataSource;
                bool dataMemberChanged = this.DataMember != newDataMember;

                //if nothing changed, then do not do any work
                //
                if (!dataSourceChanged && !dataMemberChanged && !force)
                {
                    return;
                }

                // set the dataSource and the dataMember
                //
                this.dataSource = newDataSource;
                this.dataMember = newDataMember;

                if (initializing) {
                    setErrorManagerOnEndInit = true;
                }
                else {
                    // unwire the errorManager:
                    //
                    UnwireEvents(errorManager);

                    // get the new errorManager
                    //
                    if (parentControl != null && this.dataSource != null && parentControl.BindingContext != null) {
                        errorManager = parentControl.BindingContext[this.dataSource, this.dataMember];
                    }
                    else {
                        errorManager = null;
                    }

                    // wire the events
                    //
                    WireEvents(errorManager);

                    // see if there are errors at the current
                    // item in the list, w/o waiting for the position to change
                    if (errorManager != null)
                        UpdateBinding();
                }
            } finally {
                inSetErrorManager = false;
            }
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.DataSource"]/*' />
        /// <devdoc>
        ///    Indicates the source of data to bind errors against.
        /// </devdoc>
        [
        DefaultValue(null),
        SRCategory(nameof(SR.CatData)),
        AttributeProvider(typeof(IListSource)),
        SRDescription(nameof(SR.ErrorProviderDataSourceDescr))
        ]
        public object DataSource {
            get {
                return dataSource;
            }
            set {
                if (parentControl != null && value != null && !String.IsNullOrEmpty(this.dataMember)) {
                    // Let's check if the datamember exists in the new data source
                    try {
                        errorManager = parentControl.BindingContext[value, this.dataMember];
                    }
                    catch (ArgumentException) {
                        // The data member doesn't exist in the data source, so set it to null
                        this.dataMember = "";
                    }
                }

                Set_ErrorManager(value, this.DataMember, false);
            }
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ShouldSerializeDataSource"]/*' />
        private bool ShouldSerializeDataSource() {
            return (dataSource != null);
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.DataMember"]/*' />
        /// <devdoc>
        ///    Indicates the sub-list of data from the DataSource to bind errors against.
        /// </devdoc>
        [
        DefaultValue(null),
        SRCategory(nameof(SR.CatData)),
        Editor("System.Windows.Forms.Design.DataMemberListEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor)),
        SRDescription(nameof(SR.ErrorProviderDataMemberDescr))
        ]
        public string DataMember {
            get {
                return dataMember;
            }
            set {
                if (value == null) value = "";
                Set_ErrorManager(this.DataSource, value, false);
            }
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ShouldSerializeDataMember"]/*' />
        private bool ShouldSerializeDataMember() {
            return (dataMember != null && dataMember.Length != 0);
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.BindToDataAndErrors"]/*' />
        public void BindToDataAndErrors(object newDataSource, string newDataMember) {
            Set_ErrorManager(newDataSource, newDataMember, false);
        }

        private void WireEvents(BindingManagerBase listManager) {
            if (listManager != null) {
                listManager.CurrentChanged += currentChanged;
                listManager.BindingComplete += new BindingCompleteEventHandler(this.ErrorManager_BindingComplete);

                CurrencyManager currManager = listManager as CurrencyManager;

                if (currManager != null) {
                    currManager.ItemChanged += new ItemChangedEventHandler(this.ErrorManager_ItemChanged);
                    currManager.Bindings.CollectionChanged += new CollectionChangeEventHandler(this.ErrorManager_BindingsChanged);
                }
            }
        }

        private void UnwireEvents(BindingManagerBase listManager) {
            if (listManager != null) {
                listManager.CurrentChanged -= currentChanged;
                listManager.BindingComplete -= new BindingCompleteEventHandler(this.ErrorManager_BindingComplete);

                CurrencyManager currManager = listManager as CurrencyManager;

                if (currManager != null) {
                    currManager.ItemChanged -= new ItemChangedEventHandler(this.ErrorManager_ItemChanged);
                    currManager.Bindings.CollectionChanged -= new CollectionChangeEventHandler(this.ErrorManager_BindingsChanged);
                }
            }
        }

        private void ErrorManager_BindingComplete(object sender, BindingCompleteEventArgs e) {
            Binding binding = e.Binding;

            if (binding != null && binding.Control != null) {
                SetError(binding.Control, (e.ErrorText == null ? String.Empty : e.ErrorText));
            }
        }

        private void ErrorManager_BindingsChanged(object sender, CollectionChangeEventArgs e) {
            ErrorManager_CurrentChanged(errorManager, e);
        }

        private void ParentControl_BindingContextChanged(object sender, EventArgs e) {
            Set_ErrorManager(this.DataSource, this.DataMember, true);
        }

        // Work around... we should figure out if errors changed automatically.
        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.UpdateBinding"]/*' />
        public void UpdateBinding() {
            ErrorManager_CurrentChanged(errorManager, EventArgs.Empty);
        }

        private void ErrorManager_ItemChanged(object sender, ItemChangedEventArgs e) {
            BindingsCollection errBindings = errorManager.Bindings;
            int bindingsCount = errBindings.Count;

            // If the list became empty then reset the errors
            if (e.Index == -1 && errorManager.Count == 0) {
                for (int j = 0; j < bindingsCount; j++) {
                    if (errBindings[j].Control != null) {
                        // ...ignore everything but bindings to Controls
                        SetError(errBindings[j].Control, "");
                    }
                }
            }
            else {
                ErrorManager_CurrentChanged(sender, e);
            }
        }

        private void ErrorManager_CurrentChanged(object sender, EventArgs e) {
            Debug.Assert(sender == errorManager, "who else can send us messages?");

            // flush the old list
            //
            // items.Clear();

            if (errorManager.Count == 0) {
                return;
            }

            object value = errorManager.Current;
            if ( !(value is IDataErrorInfo)) {
                return;
            }

            BindingsCollection errBindings = errorManager.Bindings;
            int bindingsCount = errBindings.Count;

            // we need to delete the blinkPhases from each controlItem (suppose
            // that the error that we get is the same error. then we want to
            // show the error and not blink )
            //
            foreach (ControlItem ctl in items.Values) {
                ctl.BlinkPhase = 0;
            }

            // We can only show one error per control, so we will build up a string...
            //
            Hashtable controlError = new Hashtable(bindingsCount);

            for (int j = 0; j < bindingsCount; j++) {

                // Ignore everything but bindings to Controls
                if (errBindings[j].Control == null) {
                    continue;
                }

                BindToObject dataBinding = errBindings[j].BindToObject;
                string error = ((IDataErrorInfo) value)[dataBinding.BindingMemberInfo.BindingField];

                if (error == null) {
                    error = "";
                }

                string outputError = "";

                if (controlError.Contains(errBindings[j].Control))
                    outputError = (string) controlError[errBindings[j].Control];

                // Utilize the error string without including the field name.
                if (String.IsNullOrEmpty(outputError)) {
                    outputError = error;
                } else {
                    outputError = string.Concat(outputError, "\r\n", error);
                }

                controlError[errBindings[j].Control] = outputError;
            }

            IEnumerator enumerator = controlError.GetEnumerator();
            while (enumerator.MoveNext()) {
                DictionaryEntry entry = (DictionaryEntry) enumerator.Current;
                SetError((Control) entry.Key, (string) entry.Value);
            }
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.BlinkRate"]/*' />
        /// <devdoc>
        ///     Returns or set the rate in milliseconds at which the error icon flashes.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(defaultBlinkRate),
        SRDescription(nameof(SR.ErrorProviderBlinkRateDescr)),
        RefreshProperties(RefreshProperties.Repaint)
        ]
        public int BlinkRate {
            get {
                return blinkRate;
            }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(BlinkRate), value, SR.BlinkRateMustBeZeroOrMore);
                }
                blinkRate = value;
                // If we set the blinkRate = 0 then set BlinkStyle = NeverBlink
                if (blinkRate == 0)
                    BlinkStyle = ErrorBlinkStyle.NeverBlink;
            }
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.DefaultIcon"]/*' />
        /// <devdoc>
        ///     Demand load and cache the default icon.
        /// </devdoc>
        /// <internalonly/>
        static Icon DefaultIcon {
            get {
                if (defaultIcon == null) {
                    lock (typeof(ErrorProvider)) {
                        if (defaultIcon == null) {
                            defaultIcon = new Icon(typeof(ErrorProvider), "Error.ico");
                        }
                    }
                }
                return defaultIcon;
            }
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.Icon"]/*' />
        /// <devdoc>
        ///     Returns or sets the Icon that displayed next to a control when an error
        ///     description string has been set for the control.  For best results, an
        ///     icon containing a 16 by 16 icon should be used.
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ErrorProviderIconDescr))
        ]
        public Icon Icon {
            get {
                return icon;
            }
            set {
                if (value == null)
                    throw new ArgumentNullException(nameof(value));
                icon = value;
                DisposeRegion();
                ErrorWindow[] array = new ErrorWindow[windows.Values.Count];
                windows.Values.CopyTo(array, 0);
                for (int i = 0; i < array.Length; i++)
                    array[i].Update(false /*timerCaused*/);
            }
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.Region"]/*' />
        /// <devdoc>
        ///     Create the icon region on demand.
        /// </devdoc>
        /// <internalonly/>
        internal IconRegion Region {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                if (region == null)
                    region = new IconRegion(Icon);
                return region;
            }
        }

        //
        // METHODS
        //

        // Begin bulk member initialization - deferring binding to data source until EndInit is reached
        void ISupportInitialize.BeginInit() {
            initializing = true;
        }

        // End bulk member initialization by binding to data source
        private void EndInitCore() {
            initializing = false;

            if (setErrorManagerOnEndInit) {
                setErrorManagerOnEndInit = false;
                Set_ErrorManager(this.DataSource, this.DataMember, true);
            }
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

            Debug.Assert(dsInit != null, "ErrorProvider: ISupportInitializeNotification.Initialized event received, but current DataSource does not support ISupportInitializeNotification!");
            Debug.Assert(dsInit.IsInitialized, "ErrorProvider: DataSource sent ISupportInitializeNotification.Initialized event but before it had finished initializing.");

            if (dsInit != null) {
                dsInit.Initialized -= new EventHandler(DataSource_Initialized);
            }

            EndInitCore();
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.Clear"]/*' />
        /// <devdoc>
        ///     Clears all errors being tracked by this error provider, ie. undoes all previous calls to SetError.
        /// </devdoc>
        public void Clear() {
            ErrorWindow[] w = new ErrorWindow[windows.Values.Count];
            windows.Values.CopyTo(w, 0);
            for (int i = 0; i < w.Length; i++) {
                w[i].Dispose();
            }
            windows.Clear();
            foreach (ControlItem item in items.Values) {
                if (item != null) {
                    item.Dispose();
                }
            }
            items.Clear();
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.CanExtend"]/*' />
        /// <devdoc>
        ///     Returns whether a control can be extended.
        /// </devdoc>
        public bool CanExtend(object extendee) {
            return extendee is Control && !(extendee is Form) && !(extendee is ToolBar);
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.Dispose"]/*' />
        /// <devdoc>
        ///     Release any resources that this component is using.  After calling Dispose,
        ///     the component should no longer be used.
        /// </devdoc>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                Clear();
                DisposeRegion();
                UnwireEvents(errorManager);
            }
            base.Dispose(disposing);
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.DisposeRegion"]/*' />
        /// <devdoc>
        ///     Helper to dispose the cached icon region.
        /// </devdoc>
        /// <internalonly/>
        void DisposeRegion() {
            if (region != null) {
                region.Dispose();
                region = null;
            }
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.EnsureControlItem"]/*' />
        /// <devdoc>
        ///     Helper to make sure we have allocated a control item for this control.
        /// </devdoc>
        /// <internalonly/>
        private ControlItem EnsureControlItem(Control control) {
            if (control == null)
                throw new ArgumentNullException(nameof(control));
            ControlItem item = (ControlItem)items[control];
            if (item == null) {
                item = new ControlItem(this, control, (IntPtr)(++itemIdCounter));
                items[control] = item;
            }
            return item;
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.EnsureErrorWindow"]/*' />
        /// <devdoc>
        ///     Helper to make sure we have allocated an error window for this control.
        /// </devdoc>
        /// <internalonly/>
        internal ErrorWindow EnsureErrorWindow(Control parent) {
            ErrorWindow window = (ErrorWindow)windows[parent];
            if (window == null) {
                window = new ErrorWindow(this, parent);
                windows[parent] = window;
            }
            return window;
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.GetError"]/*' />
        /// <devdoc>
        ///     Returns the current error description string for the specified control.
        /// </devdoc>
        [
        DefaultValue(""),
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ErrorProviderErrorDescr))
        ]
        public string GetError(Control control) {
            return EnsureControlItem(control).Error;
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.GetIconAlignment"]/*' />
        /// <devdoc>
        ///     Returns where the error icon should be placed relative to the control.
        /// </devdoc>
        [
        DefaultValue(defaultIconAlignment),
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ErrorProviderIconAlignmentDescr))
        ]
        public ErrorIconAlignment GetIconAlignment(Control control) {
            return EnsureControlItem(control).IconAlignment;
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.GetIconPadding"]/*' />
        /// <devdoc>
        ///     Returns the amount of extra space to leave next to the error icon.
        /// </devdoc>
        [
        DefaultValue(0),
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ErrorProviderIconPaddingDescr))
        ]
        public int GetIconPadding(Control control) {
            return EnsureControlItem(control).IconPadding;
        }

        private void ResetIcon() {
            Icon = DefaultIcon;
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnRightToLeftChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnRightToLeftChanged(EventArgs e) {

            foreach (ErrorWindow w in windows.Values)
            {
                w.Update(false);
            }
        
            if (onRightToLeftChanged != null) {
                 onRightToLeftChanged(this, e);
            }
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.SetError"]/*' />
        /// <devdoc>
        ///     Sets the error description string for the specified control.
        /// </devdoc>
        public void SetError(Control control, string value) {
            EnsureControlItem(control).Error = value;
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.SetIconAlignment"]/*' />
        /// <devdoc>
        ///     Sets where the error icon should be placed relative to the control.
        /// </devdoc>
        public void SetIconAlignment(Control control, ErrorIconAlignment value) {
            EnsureControlItem(control).IconAlignment = value;
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.SetIconPadding"]/*' />
        /// <devdoc>
        ///     Sets the amount of extra space to leave next to the error icon.
        /// </devdoc>
        public void SetIconPadding(Control control, int padding) {
            EnsureControlItem(control).IconPadding = padding;
        }

        private bool ShouldSerializeIcon() {
            return Icon != DefaultIcon;
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ErrorWindow"]/*' />
        /// <devdoc>
        ///     There is one ErrorWindow for each control parent.  It is parented to the
        ///     control parent.  The window's region is made up of the regions from icons
        ///     of all child icons.  The window's size is the enclosing rectangle for all
        ///     the regions.  A tooltip window is created as a child of this window.  The
        ///     rectangle associated with each error icon being displayed is added as a
        ///     tool to the tooltip window.
        /// </devdoc>
        /// <internalonly/>
        internal class ErrorWindow : NativeWindow {

            //
            // FIELDS
            //

            ArrayList items = new ArrayList();
            Control parent;
            ErrorProvider provider;
            Rectangle windowBounds = Rectangle.Empty;
            System.Windows.Forms.Timer timer;
            NativeWindow tipWindow;
            
            DeviceContext mirrordc= null;
            Size mirrordcExtent = Size.Empty;
            Point mirrordcOrigin = Point.Empty;
            DeviceContextMapMode mirrordcMode = DeviceContextMapMode.Text;

            //
            // CONSTRUCTORS
            //

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ErrorWindow.ErrorWindow"]/*' />
            /// <devdoc>
            ///     Construct an error window for this provider and control parent.
            /// </devdoc>
            public ErrorWindow(ErrorProvider provider, Control parent) {
                this.provider = provider;
                this.parent = parent;
            }

            //
            // METHODS
            //

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ErrorWindow.Add"]/*' />
            /// <devdoc>
            ///     This is called when a control would like to show an error icon.
            /// </devdoc>
            public void Add(ControlItem item) {
                items.Add(item);
                if (!EnsureCreated())
                {
                    return;
                }

                NativeMethods.TOOLINFO_T toolInfo = new NativeMethods.TOOLINFO_T();
                toolInfo.cbSize = Marshal.SizeOf(toolInfo);
                toolInfo.hwnd = Handle;
                toolInfo.uId = item.Id;
                toolInfo.lpszText = item.Error;
                toolInfo.uFlags = NativeMethods.TTF_SUBCLASS;
                UnsafeNativeMethods.SendMessage(new HandleRef(tipWindow, tipWindow.Handle), NativeMethods.TTM_ADDTOOL, 0, toolInfo);

                Update(false /*timerCaused*/);
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ErrorWindow.Dispose"]/*' />
            /// <devdoc>
            ///     Called to get rid of any resources the Object may have.
            /// </devdoc>
            public void Dispose() {
                EnsureDestroyed();
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ErrorWindow.EnsureCreated"]/*' />
            /// <devdoc>
            ///     Make sure the error window is created, and the tooltip window is created.
            /// </devdoc>
            bool EnsureCreated() {
                if (Handle == IntPtr.Zero) {
                    if (!parent.IsHandleCreated)
                    {
                        return false;
                    }
                    CreateParams cparams = new CreateParams();
                    cparams.Caption = String.Empty;
                    cparams.Style = NativeMethods.WS_VISIBLE | NativeMethods.WS_CHILD;
                    cparams.ClassStyle = NativeMethods.CS_DBLCLKS;
                    cparams.X = 0;
                    cparams.Y = 0;
                    cparams.Width = 0;
                    cparams.Height = 0;
                    cparams.Parent = parent.Handle;

                    CreateHandle(cparams);

                    NativeMethods.INITCOMMONCONTROLSEX icc = new NativeMethods.INITCOMMONCONTROLSEX();
                    icc.dwICC = NativeMethods.ICC_TAB_CLASSES;
                    icc.dwSize = Marshal.SizeOf(icc);
                    SafeNativeMethods.InitCommonControlsEx(icc);
                    cparams = new CreateParams();
                    cparams.Parent = Handle;
                    cparams.ClassName = NativeMethods.TOOLTIPS_CLASS;
                    cparams.Style = NativeMethods.TTS_ALWAYSTIP;
                    tipWindow = new NativeWindow();
                    tipWindow.CreateHandle(cparams);

                    UnsafeNativeMethods.SendMessage(new HandleRef(tipWindow, tipWindow.Handle), NativeMethods.TTM_SETMAXTIPWIDTH, 0, SystemInformation.MaxWindowTrackSize.Width);
                    SafeNativeMethods.SetWindowPos(new HandleRef(tipWindow, tipWindow.Handle), NativeMethods.HWND_TOP, 0, 0, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOACTIVATE);
                    UnsafeNativeMethods.SendMessage(new HandleRef(tipWindow, tipWindow.Handle), NativeMethods.TTM_SETDELAYTIME, NativeMethods.TTDT_INITIAL, 0);
                }
                return true;
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ErrorWindow.EnsureDestroyed"]/*' />
            /// <devdoc>
            ///     Destroy the timer, toolwindow, and the error window itself.
            /// </devdoc>
            void EnsureDestroyed() {
                if (timer != null) {
                    timer.Dispose();
                    timer = null;
                }
                if (tipWindow != null) {
                    tipWindow.DestroyHandle();
                    tipWindow = null;
                }

                // Hide the window and invalidate the parent to ensure
                // that we leave no visual artifacts... given that we
                // have a bizare region window, this is needed.
                //
                SafeNativeMethods.SetWindowPos(new HandleRef(this, Handle),
                                               NativeMethods.HWND_TOP,
                                               windowBounds.X,
                                               windowBounds.Y,
                                               windowBounds.Width,
                                               windowBounds.Height,
                                               NativeMethods.SWP_HIDEWINDOW
                                               | NativeMethods.SWP_NOSIZE
                                               | NativeMethods.SWP_NOMOVE);
                if (parent != null) {
                    parent.Invalidate(true);
                }
                DestroyHandle();

                Debug.Assert(mirrordc == null, "Why is mirrordc non-null?");
                if (mirrordc != null) {
                    mirrordc.Dispose();
                }
            }

            /// <devdoc>            
            ///
            /// Since we added mirroring to certain controls, we need to make sure the
            /// error icons show up in the correct place. We cannot mirror the errorwindow
            /// in EnsureCreated (although that would have been really easy), since we use
            /// GDI+ for some of this code, and as we all know, GDI+ does not handle mirroring
            /// at all.
            ///
            /// To work around that we create our own mirrored dc when we need to.
            ///
            /// </devdoc>
            void CreateMirrorDC(IntPtr hdc, int originOffset) {

                Debug.Assert(mirrordc == null, "Why is mirrordc non-null? Did you not call RestoreMirrorDC?");

                mirrordc = DeviceContext.FromHdc(hdc);
                if (parent.IsMirrored && mirrordc != null) {
                    mirrordc.SaveHdc();
                    mirrordcExtent = mirrordc.ViewportExtent;
                    mirrordcOrigin = mirrordc.ViewportOrigin;

                    mirrordcMode = mirrordc.SetMapMode(DeviceContextMapMode.Anisotropic);
                    mirrordc.ViewportExtent = new Size(-(mirrordcExtent.Width), mirrordcExtent.Height);
                    mirrordc.ViewportOrigin = new Point(mirrordcOrigin.X + originOffset, mirrordcOrigin.Y);
                }
            }

            void RestoreMirrorDC() {
                                    
                if (parent.IsMirrored && mirrordc != null) {
                    mirrordc.ViewportExtent = mirrordcExtent;
                    mirrordc.ViewportOrigin = mirrordcOrigin;
                    mirrordc.SetMapMode(mirrordcMode);
                    mirrordc.RestoreHdc();
                    mirrordc.Dispose();
                }

                mirrordc= null;
                mirrordcExtent = Size.Empty;
                mirrordcOrigin = Point.Empty;
                mirrordcMode = DeviceContextMapMode.Text;
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ErrorWindow.OnPaint"]/*' />
            /// <devdoc>
            ///     This is called when the error window needs to paint.  We paint each icon at its
            ///     correct location.
            /// </devdoc>
            void OnPaint(ref Message m) {
                NativeMethods.PAINTSTRUCT ps = new NativeMethods.PAINTSTRUCT();
                IntPtr hdc = UnsafeNativeMethods.BeginPaint(new HandleRef(this, Handle), ref ps);
                try {
                    CreateMirrorDC(hdc, windowBounds.Width - 1);

                    try {
                        for (int i = 0; i < items.Count; i++) {
                            ControlItem item = (ControlItem)items[i];
                            Rectangle bounds = item.GetIconBounds(provider.Region.Size);
                            SafeNativeMethods.DrawIconEx(new HandleRef(this, mirrordc.Hdc), bounds.X - windowBounds.X, bounds.Y - windowBounds.Y, new HandleRef(provider.Region, provider.Region.IconHandle), bounds.Width, bounds.Height, 0, NativeMethods.NullHandleRef, NativeMethods.DI_NORMAL);
                        }
                    }
                    finally {
                        RestoreMirrorDC();
                    }
                } 
                finally {
                    UnsafeNativeMethods.EndPaint(new HandleRef(this, Handle), ref ps);
                }
            }

            protected override void OnThreadException(Exception e) {
                Application.OnThreadException(e);
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ErrorWindow.OnTimer"]/*' />
            /// <devdoc>
            ///     This is called when an error icon is flashing, and the view needs to be updatd.
            /// </devdoc>
            void OnTimer(Object sender, EventArgs e) {
                int blinkPhase = 0;
                for (int i = 0; i < items.Count; i++) {
                    blinkPhase += ((ControlItem)items[i]).BlinkPhase;
                }
                if (blinkPhase == 0 && provider.BlinkStyle != ErrorBlinkStyle.AlwaysBlink) {
                    Debug.Assert(timer != null);
                    timer.Stop();
                }
                Update(true /*timerCaused*/);
            }

            private void OnToolTipVisibilityChanging(System.IntPtr id, bool toolTipShown) {
                for (int i = 0; i < items.Count; i++) {
                    if (((ControlItem)items[i]).Id == id) {
                        ((ControlItem)items[i]).ToolTipShown = toolTipShown;
                    }
                }
#if DEBUG
                int shownTooltips = 0;
                for (int j = 0; j < items.Count; j++) {
                    if (((ControlItem)items[j]).ToolTipShown) {
                        shownTooltips++;
                    }
                }
                Debug.Assert(shownTooltips <= 1);
#endif
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ErrorWindow.Remove"]/*' />
            /// <devdoc>
            ///     This is called when a control no longer needs to display an error icon.
            /// </devdoc>
            public void Remove(ControlItem item) {
                items.Remove(item);

                if (tipWindow != null) {
                    NativeMethods.TOOLINFO_T toolInfo = new NativeMethods.TOOLINFO_T();
                    toolInfo.cbSize = Marshal.SizeOf(toolInfo);
                    toolInfo.hwnd = Handle;
                    toolInfo.uId = item.Id;
                    UnsafeNativeMethods.SendMessage(new HandleRef(tipWindow, tipWindow.Handle), NativeMethods.TTM_DELTOOL, 0, toolInfo);
                }

                if (items.Count == 0) {
                    EnsureDestroyed();
                }
                else {
                    Update(false /*timerCaused*/);
                }
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ErrorWindow.StartBlinking"]/*' />
            /// <devdoc>
            ///     Start the blinking process.  The timer will fire until there are no more
            ///     icons that need to blink.
            /// </devdoc>
            internal void StartBlinking() {
                if (timer == null) {
                    timer = new System.Windows.Forms.Timer();
                    timer.Tick += new EventHandler(OnTimer);
                }
                timer.Interval = provider.BlinkRate;
                timer.Start();
                Update(false /*timerCaused*/);
            }

            internal void StopBlinking() {
                if (timer != null) {
                    timer.Stop();
                }
                Update(false /*timerCaused*/);
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ErrorWindow.Update"]/*' />
            /// <devdoc>
            ///     Move and size the error window, compute and set the window region,
            ///     set the tooltip rectangles and descriptions.  This basically brings
            ///     the error window up to date with the internal data structures.
            /// </devdoc>
            public void Update(bool timerCaused) {
                IconRegion iconRegion = provider.Region;
                Size size = iconRegion.Size;
                windowBounds = Rectangle.Empty;
                for (int i = 0; i < items.Count; i++) {
                    ControlItem item = (ControlItem)items[i];
                    Rectangle iconBounds = item.GetIconBounds(size);
                    if (windowBounds.IsEmpty)
                        windowBounds = iconBounds;
                    else
                        windowBounds = Rectangle.Union(windowBounds, iconBounds);
                }

                Region windowRegion =  new Region(new Rectangle(0, 0, 0, 0));
                IntPtr windowRegionHandle = IntPtr.Zero;
                try {
                    for (int i = 0; i < items.Count; i++) {
                        ControlItem item = (ControlItem)items[i];
                        Rectangle iconBounds = item.GetIconBounds(size);
                        iconBounds.X -= windowBounds.X;
                        iconBounds.Y -= windowBounds.Y;

                        bool showIcon = true;
                        if (!item.ToolTipShown) {
                            switch (provider.BlinkStyle) {
                                case ErrorBlinkStyle.NeverBlink:
                                    // always show icon
                                    break;

                                case ErrorBlinkStyle.BlinkIfDifferentError:
                                    showIcon = (item.BlinkPhase == 0) || (item.BlinkPhase > 0 && (item.BlinkPhase & 1) == (i & 1));
                                    break;

                                case ErrorBlinkStyle.AlwaysBlink:
                                    showIcon = ((i & 1) == 0) == provider.showIcon;
                                    break;
                            }
                        }

                        if (showIcon)
                        {
                            iconRegion.Region.Translate(iconBounds.X, iconBounds.Y);
                            windowRegion.Union(iconRegion.Region);
                            iconRegion.Region.Translate(-iconBounds.X, -iconBounds.Y);
                        }

                        if (tipWindow != null) {
                            NativeMethods.TOOLINFO_T toolInfo = new NativeMethods.TOOLINFO_T();
                            toolInfo.cbSize = Marshal.SizeOf(toolInfo);
                            toolInfo.hwnd = Handle;
                            toolInfo.uId = item.Id;
                            toolInfo.lpszText = item.Error;
                            toolInfo.rect = NativeMethods.RECT.FromXYWH(iconBounds.X, iconBounds.Y, iconBounds.Width, iconBounds.Height);
                            toolInfo.uFlags = NativeMethods.TTF_SUBCLASS;
                            if (provider.RightToLeft) {
                                toolInfo.uFlags |= NativeMethods.TTF_RTLREADING;
                            }
                            UnsafeNativeMethods.SendMessage(new HandleRef(tipWindow, tipWindow.Handle), NativeMethods.TTM_SETTOOLINFO, 0, toolInfo);
                        }

                        if (timerCaused && item.BlinkPhase > 0) {
                            item.BlinkPhase--;
                        }
                    }
                    if (timerCaused) {
                        provider.showIcon = !provider.showIcon;
                    }


                    DeviceContext dc = null;
                    dc = DeviceContext.FromHwnd(this.Handle);
                    try {
                        CreateMirrorDC(dc.Hdc, windowBounds.Width);
                        
                        Graphics graphics = Graphics.FromHdcInternal(mirrordc.Hdc);
                        try {
                            windowRegionHandle = windowRegion.GetHrgn(graphics);
                            System.Internal.HandleCollector.Add(windowRegionHandle, NativeMethods.CommonHandles.GDI);
                        }
                        finally {
                            graphics.Dispose();
                            RestoreMirrorDC();
                        }
                        
                        if (UnsafeNativeMethods.SetWindowRgn(new HandleRef(this, Handle), new HandleRef(windowRegion, windowRegionHandle), true) != 0) {
                            //The HWnd owns the region.
                            windowRegionHandle = IntPtr.Zero;
                        }
                    }
                    
                    finally {
                        if (dc != null) {
                            dc.Dispose();
                        }
                    }

                }
                finally {
                    windowRegion.Dispose();
                    if (windowRegionHandle != IntPtr.Zero) {
                        SafeNativeMethods.DeleteObject(new HandleRef(null, windowRegionHandle));
                    }
                }

                SafeNativeMethods.SetWindowPos(new HandleRef(this, Handle), NativeMethods.HWND_TOP, windowBounds.X, windowBounds.Y,
                                     windowBounds.Width, windowBounds.Height, NativeMethods.SWP_NOACTIVATE);
                SafeNativeMethods.InvalidateRect(new HandleRef(this, Handle), null, false);
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ErrorWindow.WndProc"]/*' />
            /// <devdoc>
            ///     Called when the error window gets a windows message.
            /// </devdoc>
            protected override void WndProc(ref Message m) {
                switch (m.Msg) {
                    case NativeMethods.WM_NOTIFY:
                        NativeMethods.NMHDR nmhdr = (NativeMethods.NMHDR)m.GetLParam(typeof(NativeMethods.NMHDR));
                        if (nmhdr.code == NativeMethods.TTN_SHOW || nmhdr.code == NativeMethods.TTN_POP)
                        {
                            OnToolTipVisibilityChanging(nmhdr.idFrom, nmhdr.code == NativeMethods.TTN_SHOW);
                        }
                        break;
                    case NativeMethods.WM_ERASEBKGND:
                        break;
                    case NativeMethods.WM_PAINT:
                        OnPaint(ref m);
                        break;
                    default:
                        base.WndProc(ref m);
                        break;
                }
            }
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ControlItem"]/*' />
        /// <devdoc>
        ///     There is one ControlItem for each control that the ErrorProvider is
        ///     is tracking state for.  It contains the values of all the extender
        ///     properties.
        /// </devdoc>
        internal class ControlItem {

            //
            // FIELDS
            //

            string error;
            Control control;
            ErrorWindow window;
            ErrorProvider provider;
            int blinkPhase;
            IntPtr id;
            int iconPadding;
            bool toolTipShown;
            ErrorIconAlignment iconAlignment;
            const int startingBlinkPhase = 10;          // cause we want to blink 5 times

            //
            // CONSTRUCTORS
            //

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ControlItem.ControlItem"]/*' />
            /// <devdoc>
            ///     Construct the item with its associated control, provider, and
            ///     a unique ID.  The ID is used for the tooltip ID.
            /// </devdoc>
            public ControlItem(ErrorProvider provider, Control control, IntPtr id) {
                this.toolTipShown = false;
                this.iconAlignment = defaultIconAlignment;
                this.error = String.Empty;
                this.id = id;
                this.control = control;
                this.provider = provider;
                this.control.HandleCreated += new EventHandler(OnCreateHandle);
                this.control.HandleDestroyed += new EventHandler(OnDestroyHandle);
                this.control.LocationChanged += new EventHandler(OnBoundsChanged);
                this.control.SizeChanged += new EventHandler(OnBoundsChanged);
                this.control.VisibleChanged += new EventHandler(OnParentVisibleChanged);
                this.control.ParentChanged += new EventHandler(OnParentVisibleChanged);
            }

            public void Dispose() {
                if (control != null) {
                    control.HandleCreated -= new EventHandler(OnCreateHandle);
                    control.HandleDestroyed -= new EventHandler(OnDestroyHandle);
                    control.LocationChanged -= new EventHandler(OnBoundsChanged);
                    control.SizeChanged -= new EventHandler(OnBoundsChanged);
                    control.VisibleChanged -= new EventHandler(OnParentVisibleChanged);
                    control.ParentChanged -= new EventHandler(OnParentVisibleChanged);
                }
                error = string.Empty;
            }

            //
            // PROPERTIES
            //

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ControlItem.Id"]/*' />
            /// <devdoc>
            ///     Returns the unique ID for this control.  The ID used as the tooltip ID.
            /// </devdoc>
            public IntPtr Id {
                get {
                    return id;
                }
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ControlItem.BlinkPhase"]/*' />
            /// <devdoc>
            ///     Returns or set the phase of blinking that this control is currently
            ///     in.   If zero, the control is not blinking.  If odd, then the control
            ///     is blinking, but invisible.  If even, the control is blinking and
            ///     currently visible.  Each time the blink timer fires, this value is
            ///     reduced by one (until zero), thus causing the error icon to appear
            ///     or disappear.
            /// </devdoc>
            public int BlinkPhase {
                get {
                    return blinkPhase;
                }
                set {
                    blinkPhase = value;
                }
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ControlItem.IconPadding"]/*' />
            /// <devdoc>
            ///     Returns or sets the icon padding for the control.
            /// </devdoc>
            public int IconPadding {
                get {
                    return iconPadding;
                }
                set {
                    if (iconPadding != value) {
                        iconPadding = value;
                        UpdateWindow();
                    }
                }
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ControlItem.Error"]/*' />
            /// <devdoc>
            ///     Returns or sets the error description string for the control.
            /// </devdoc>
            public string Error {
                get {
                    return error;
                }
                set {
                    if (value == null) {
                        value = "";
                    }

                    // if the error is the same and the blinkStyle is not AlwaysBlink, then
                    // we should not add the error and not start blinking.
                    if (error.Equals(value) && provider.BlinkStyle != ErrorBlinkStyle.AlwaysBlink) {
                        return;
                    }

                    bool adding = error.Length == 0;
                    error = value;
                    if (value.Length == 0) {
                        RemoveFromWindow();
                    }
                    else {
                        if (adding) {
                            AddToWindow();
                        }
                        else {
                            if (provider.BlinkStyle != ErrorBlinkStyle.NeverBlink) {
                                StartBlinking();
                            }
                            else {
                                UpdateWindow();
                            }
                        }
                    }
                }
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ControlItem.IconAlignment"]/*' />
            /// <devdoc>
            ///     Returns or sets the location of the error icon for the control.
            /// </devdoc>
            public ErrorIconAlignment IconAlignment {
                get {
                    return iconAlignment;
                }
                set {
                    if (iconAlignment != value) {
                        //valid values are 0x0 to 0x5
                        if (!ClientUtils.IsEnumValid(value, (int)value, (int)ErrorIconAlignment.TopLeft, (int)ErrorIconAlignment.BottomRight))
                        {
                            throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ErrorIconAlignment));
                        }
                        iconAlignment = value;
                        UpdateWindow();
                    }
                }
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ControlItem.ToolTipShown"]/*' />
            /// <devdoc>
            ///     Returns true if the tooltip for this control item is currently shown.
            /// </devdoc>
            public bool ToolTipShown
            {
                get {
                    return this.toolTipShown;
                }
                set {
                    this.toolTipShown = value;
                }
            }

            internal ErrorIconAlignment RTLTranslateIconAlignment(ErrorIconAlignment align) {
                if (provider.RightToLeft) {
                    switch (align) {
                        case ErrorIconAlignment.TopLeft:
                            return ErrorIconAlignment.TopRight;
                        case ErrorIconAlignment.MiddleLeft:
                            return ErrorIconAlignment.MiddleRight;
                        case ErrorIconAlignment.BottomLeft:
                            return ErrorIconAlignment.BottomRight;
                        case ErrorIconAlignment.TopRight:
                            return ErrorIconAlignment.TopLeft;
                        case ErrorIconAlignment.MiddleRight:
                            return ErrorIconAlignment.MiddleLeft;
                        case ErrorIconAlignment.BottomRight:
                            return ErrorIconAlignment.BottomLeft;
                        default:
                            Debug.Fail("Unknown ErrorIconAlignment value");
                            return align;
                    }
                }
                else {
                    return align;
                }
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ControlItem.GetIconBounds"]/*' />
            /// <devdoc>
            ///     Returns the location of the icon in the same coordinate system as
            ///     the control being extended.  The size passed in is the size of
            ///     the icon.
            /// </devdoc>
            internal Rectangle GetIconBounds(Size size) {
                int x = 0;
                int y = 0;

                switch (RTLTranslateIconAlignment(IconAlignment)) {
                    case ErrorIconAlignment.TopLeft:
                    case ErrorIconAlignment.MiddleLeft:
                    case ErrorIconAlignment.BottomLeft:
                        x = control.Left - size.Width - iconPadding;
                        break;
                    case ErrorIconAlignment.TopRight:
                    case ErrorIconAlignment.MiddleRight:
                    case ErrorIconAlignment.BottomRight:
                        x = control.Right + iconPadding;
                        break;
                }

                switch (IconAlignment) {
                    case ErrorIconAlignment.TopLeft:
                    case ErrorIconAlignment.TopRight:
                        y = control.Top;
                        break;
                    case ErrorIconAlignment.MiddleLeft:
                    case ErrorIconAlignment.MiddleRight:
                        y = control.Top + (control.Height - size.Height) / 2;
                        break;
                    case ErrorIconAlignment.BottomLeft:
                    case ErrorIconAlignment.BottomRight:
                        y = control.Bottom - size.Height;
                        break;
                }

                return new Rectangle(x, y, size.Width, size.Height);
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ControlItem.UpdateWindow"]/*' />
            /// <devdoc>
            ///     If this control's error icon has been added to the error
            ///     window, then update the window state because some property
            ///     has changed.
            /// </devdoc>
            void UpdateWindow() {
                if (window != null) {
                    window.Update(false /*timerCaused*/);
                }
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ControlItem.StartBlinking"]/*' />
            /// <devdoc>
            ///     If this control's error icon has been added to the error
            ///     window, then start blinking the error window.  The blink
            ///     count
            /// </devdoc>
            void StartBlinking() {
                if (window != null) {
                    BlinkPhase = startingBlinkPhase;
                    window.StartBlinking();
                }
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ControlItem.AddToWindow"]/*' />
            /// <devdoc>
            ///     Add this control's error icon to the error window.
            /// </devdoc>
            void AddToWindow() {
                // if we are recreating the control, then add the control.
                if (window == null &&
                    (control.Created || control.RecreatingHandle) &&
                    control.Visible && control.ParentInternal != null &&
                    error.Length > 0) {
                    window = provider.EnsureErrorWindow(control.ParentInternal);
                    window.Add(this);
                    // Make sure that we blink if the style is set to AlwaysBlink or BlinkIfDifferrentError
                    if (provider.BlinkStyle != ErrorBlinkStyle.NeverBlink)
                    {
                        StartBlinking();
                    }
                }
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ControlItem.RemoveFromWindow"]/*' />
            /// <devdoc>
            ///     Remove this control's error icon from the error window.
            /// </devdoc>
            void RemoveFromWindow() {
                if (window != null) {
                    window.Remove(this);
                    window = null;
                }
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ControlItem.OnBoundsChanged"]/*' />
            /// <devdoc>
            ///     This is called when a property on the control is changed.
            /// </devdoc>
            void OnBoundsChanged(Object sender, EventArgs e) {
                UpdateWindow();
            }

            void OnParentVisibleChanged(Object sender, EventArgs e) {
                this.BlinkPhase = 0;
                RemoveFromWindow();
                AddToWindow();
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ControlItem.OnCreateHandle"]/*' />
            /// <devdoc>
            ///     This is called when the control's handle is created.
            /// </devdoc>
            void OnCreateHandle(Object sender, EventArgs e) {
                AddToWindow();
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.ControlItem.OnDestroyHandle"]/*' />
            /// <devdoc>
            ///     This is called when the control's handle is destroyed.
            /// </devdoc>
            void OnDestroyHandle(Object sender, EventArgs e) {
                RemoveFromWindow();
            }
        }

        /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.IconRegion"]/*' />
        /// <devdoc>
        ///     This represents the HRGN of icon.  The region is calculate from the icon's mask.
        /// </devdoc>
        internal class IconRegion {

            //
            // FIELDS
            //

            Region region;
            Icon icon;

            //
            // CONSTRUCTORS
            //

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.IconRegion.IconRegion"]/*' />
            /// <devdoc>
            ///     Constructor that takes an Icon and extracts its 16x16 version.
            /// </devdoc>
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            public IconRegion(Icon icon) {
                this.icon = new Icon(icon, 16, 16);
            }

            //
            // PROPERTIES
            //

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.IconRegion.IconHandle"]/*' />
            /// <devdoc>
            ///     Returns the handle of the icon.
            /// </devdoc>
            public IntPtr IconHandle {
                get {
                    return icon.Handle;
                }
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.IconRegion.Region"]/*' />
            /// <devdoc>
            ///     Returns the handle of the region.
            /// </devdoc>
            public Region Region {
                [ResourceExposure(ResourceScope.Process)]
                [ResourceConsumption(ResourceScope.Process | ResourceScope.Machine, ResourceScope.Process | ResourceScope.Machine)]
                get {
                    if (region == null) {
                        region = new Region(new Rectangle(0,0,0,0));

                        IntPtr mask = IntPtr.Zero;
                        try {
                            Size size = icon.Size;
                            Bitmap bitmap = icon.ToBitmap();
                            bitmap.MakeTransparent();
                            mask = ControlPaint.CreateHBitmapTransparencyMask(bitmap);
                            bitmap.Dispose();

                            // It is been observed that users can use non standard size icons (not a 16 bit multiples for width and height) 
                            // and GetBitmapBits method allocate bytes in multiple of 16 bits for each row. Following calculation is to get right width in bytes.
                            int bitmapBitsAllocationSize = 16;

                            //if width is not multiple of 16, we need to allocate BitmapBitsAllocationSize for remaining bits.
                            int widthInBytes = 2 * ((size.Width +15) / bitmapBitsAllocationSize); // its in bytes.
                            byte[] bits = new byte[widthInBytes * size.Height];
                            SafeNativeMethods.GetBitmapBits(new HandleRef(null, mask), bits.Length, bits);

                            for (int y = 0; y < size.Height; y++) {
                                for (int x = 0; x < size.Width; x++) {

                                    // see if bit is set in mask.  bits in byte are reversed. 0 is black (set).                                    
                                    if ((bits[y * widthInBytes + x / 8] & (1 << (7 - (x % 8)))) == 0) {
                                        region.Union(new Rectangle(x, y, 1, 1));
                                    }                                   
                                }
                            }
                            region.Intersect(new Rectangle(0, 0, size.Width, size.Height));
                        }
                        finally {
                            if (mask != IntPtr.Zero)
                                SafeNativeMethods.DeleteObject(new HandleRef(null, mask));
                        }
                    }

                    return region;
                }
            }

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.IconRegion.Size"]/*' />
            /// <devdoc>
            ///     Return the size of the icon.
            /// </devdoc>
            public Size Size {
                get {
                    return icon.Size;
                }
            }

            //
            // METHODS
            //

            /// <include file='doc\ErrorProvider.uex' path='docs/doc[@for="ErrorProvider.IconRegion.Dispose"]/*' />
            /// <devdoc>
            ///     Release any resources held by this Object.
            /// </devdoc>
            public void Dispose() {
                if (region != null) {
                    region.Dispose();
                    region = null;
                }
                icon.Dispose();
            }

        }
    }
}



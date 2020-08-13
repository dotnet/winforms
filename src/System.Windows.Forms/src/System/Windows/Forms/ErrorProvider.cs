// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  ErrorProvider presents a simple user interface for indicating to the
    ///  user that a control on a form has an error associated with it. If a
    ///  error description string is specified for the control, then an icon
    ///  will appear next to the control, and when the mouse hovers over the
    ///  icon, a tooltip will appear showing the error description string.
    /// </summary>
    [ProvideProperty("IconPadding", typeof(Control))]
    [ProvideProperty("IconAlignment", typeof(Control))]
    [ProvideProperty("Error", typeof(Control))]
    [ToolboxItemFilter("System.Windows.Forms")]
    [ComplexBindingProperties(nameof(DataSource), nameof(DataMember))]
    [SRDescription(nameof(SR.DescriptionErrorProvider))]
    public partial class ErrorProvider : Component, IExtenderProvider, ISupportInitialize
    {
        private readonly Hashtable _items = new Hashtable();
        private readonly Hashtable _windows = new Hashtable();
        private Icon _icon = DefaultIcon;
        private IconRegion _region;
        private int _itemIdCounter;
        private int _blinkRate;
        private ErrorBlinkStyle _blinkStyle;
        private bool _showIcon = true; // used for blinking
        private bool _inSetErrorManager;
        private bool _setErrorManagerOnEndInit;
        private bool _initializing;

        [ThreadStatic]
        private static Icon t_defaultIcon;

        private const int DefaultBlinkRate = 250;
        private const ErrorBlinkStyle DefaultBlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
        private const ErrorIconAlignment DefaultIconAlignment = ErrorIconAlignment.MiddleRight;

        // data binding
        private ContainerControl _parentControl;
        private object _dataSource;
        private string _dataMember;
        private BindingManagerBase _errorManager;
        private readonly EventHandler _currentChanged;

        // listen to the OnPropertyChanged event in the ContainerControl
        private readonly EventHandler _propChangedEvent;

        private EventHandler _onRightToLeftChanged;

        private bool _rightToLeft;

        /// <summary>
        ///  Default constructor.
        /// </summary>
        public ErrorProvider()
        {
            _icon = DefaultIcon;
            _blinkRate = DefaultBlinkRate;
            _blinkStyle = DefaultBlinkStyle;
            _currentChanged = new EventHandler(ErrorManager_CurrentChanged);
        }

        public ErrorProvider(ContainerControl parentControl) : this()
        {
            if (parentControl is null)
            {
                throw new ArgumentNullException(nameof(parentControl));
            }

            _parentControl = parentControl;
            _propChangedEvent = new EventHandler(ParentControl_BindingContextChanged);
            parentControl.BindingContextChanged += _propChangedEvent;
        }

        public ErrorProvider(IContainer container) : this()
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

            container.Add(this);
        }

        public override ISite Site
        {
            set
            {
                base.Site = value;
                if (value is null)
                {
                    return;
                }

                if (value.GetService(typeof(IDesignerHost)) is IDesignerHost host)
                {
                    if (host.RootComponent is ContainerControl rootContainer)
                    {
                        ContainerControl = rootContainer;
                    }
                }
            }
        }

        /// <summary>
        ///  Returns or sets when the error icon flashes.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(DefaultBlinkStyle)]
        [SRDescription(nameof(SR.ErrorProviderBlinkStyleDescr))]
        public ErrorBlinkStyle BlinkStyle
        {
            get => _blinkRate == 0 ? ErrorBlinkStyle.NeverBlink : _blinkStyle;
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ErrorBlinkStyle.BlinkIfDifferentError, (int)ErrorBlinkStyle.NeverBlink))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ErrorBlinkStyle));
                }

                // If the blinkRate == 0, then set blinkStyle = neverBlink
                if (_blinkRate == 0)
                {
                    value = ErrorBlinkStyle.NeverBlink;
                }

                if (_blinkStyle == value)
                {
                    return;
                }

                if (value == ErrorBlinkStyle.AlwaysBlink)
                {
                    // Need to start blinking on all the controlItems in our items hashTable.
                    _showIcon = true;
                    _blinkStyle = ErrorBlinkStyle.AlwaysBlink;
                    foreach (ErrorWindow w in _windows.Values)
                    {
                        w.StartBlinking();
                    }
                }
                else if (_blinkStyle == ErrorBlinkStyle.AlwaysBlink)
                {
                    // Need to stop blinking.
                    _blinkStyle = value;
                    foreach (ErrorWindow w in _windows.Values)
                    {
                        w.StopBlinking();
                    }
                }
                else
                {
                    _blinkStyle = value;
                }
            }
        }

        /// <summary>
        ///  Indicates what container control (usually the form) should be inspected for bindings.
        ///  A binding will reveal what control to place errors on for a error in the current row
        ///  in the DataSource/DataMember pair.
        /// </summary>
        [DefaultValue(null)]
        [SRCategory(nameof(SR.CatData))]
        [SRDescription(nameof(SR.ErrorProviderContainerControlDescr))]
        public ContainerControl ContainerControl
        {
            get => _parentControl;
            set
            {
                if (_parentControl == value)
                {
                    return;
                }

                if (_parentControl != null)
                {
                    _parentControl.BindingContextChanged -= _propChangedEvent;
                }

                _parentControl = value;

                if (_parentControl != null)
                {
                    _parentControl.BindingContextChanged += _propChangedEvent;
                }

                SetErrorManager(DataSource, DataMember, force: true);
            }
        }

        /// <summary>
        ///  This is used for international applications where the language is written from RightToLeft.
        ///  When this property is true, text will be from right to left.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [Localizable(true)]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ControlRightToLeftDescr))]
        public virtual bool RightToLeft
        {
            get => _rightToLeft;
            set
            {
                if (value == _rightToLeft)
                {
                    return;
                }

                _rightToLeft = value;
                OnRightToLeftChanged(EventArgs.Empty);
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ControlOnRightToLeftChangedDescr))]
        public event EventHandler RightToLeftChanged
        {
            add => _onRightToLeftChanged += value;
            remove => _onRightToLeftChanged -= value;
        }

        /// <summary>
        ///  User defined data associated with the control.
        /// </summary>
        [SRCategory(nameof(SR.CatData))]
        [Localizable(false)]
        [Bindable(true)]
        [SRDescription(nameof(SR.ControlTagDescr))]
        [DefaultValue(null)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag { get; set; }

        private void SetErrorManager(object newDataSource, string newDataMember, bool force)
        {
            if (_inSetErrorManager)
            {
                return;
            }

            _inSetErrorManager = true;
            try
            {
                bool dataSourceChanged = DataSource != newDataSource;
                bool dataMemberChanged = DataMember != newDataMember;

                // If nothing changed, then do not do any work
                if (!dataSourceChanged && !dataMemberChanged && !force)
                {
                    return;
                }

                // Set the dataSource and the dataMember
                _dataSource = newDataSource;
                _dataMember = newDataMember;

                if (_initializing)
                {
                    _setErrorManagerOnEndInit = true;
                }
                else
                {
                    // Unwire the errorManager:
                    UnwireEvents(_errorManager);

                    // Get the new errorManager
                    if (_parentControl != null && _dataSource != null && _parentControl.BindingContext != null)
                    {
                        _errorManager = _parentControl.BindingContext[_dataSource, _dataMember];
                    }
                    else
                    {
                        _errorManager = null;
                    }

                    // Wire the events
                    WireEvents(_errorManager);

                    // See if there are errors at the current item in the list, without waiting for
                    // the position to change
                    if (_errorManager != null)
                    {
                        UpdateBinding();
                    }
                }
            }
            finally
            {
                _inSetErrorManager = false;
            }
        }

        /// <summary>
        ///  Indicates the source of data to bind errors against.
        /// </summary>
        [DefaultValue(null)]
        [SRCategory(nameof(SR.CatData))]
        [AttributeProvider(typeof(IListSource))]
        [SRDescription(nameof(SR.ErrorProviderDataSourceDescr))]
        public object DataSource
        {
            get => _dataSource;
            set
            {
                if (_parentControl != null && _parentControl.BindingContext != null && value != null && !string.IsNullOrEmpty(_dataMember))
                {
                    // Let's check if the datamember exists in the new data source
                    try
                    {
                        _errorManager = _parentControl.BindingContext[value, _dataMember];
                    }
                    catch (ArgumentException)
                    {
                        // The data member doesn't exist in the data source, so set it to null
                        _dataMember = string.Empty;
                    }
                }

                SetErrorManager(value, DataMember, force: false);
            }
        }

        private bool ShouldSerializeDataSource() => _dataSource != null;

        /// <summary>
        ///  Indicates the sub-list of data from the DataSource to bind errors against.
        /// </summary>
        [DefaultValue(null)]
        [SRCategory(nameof(SR.CatData))]
        [Editor("System.Windows.Forms.Design.DataMemberListEditor, " + AssemblyRef.SystemDesign, typeof(Drawing.Design.UITypeEditor))]
        [SRDescription(nameof(SR.ErrorProviderDataMemberDescr))]
        public string DataMember
        {
            get => _dataMember;
            set
            {
                if (value is null)
                {
                    value = string.Empty;
                }

                SetErrorManager(DataSource, value, false);
            }
        }

        private bool ShouldSerializeDataMember() => !string.IsNullOrEmpty(_dataMember);

        public void BindToDataAndErrors(object newDataSource, string newDataMember)
        {
            SetErrorManager(newDataSource, newDataMember, false);
        }

        private void WireEvents(BindingManagerBase listManager)
        {
            if (listManager is null)
            {
                return;
            }

            listManager.CurrentChanged += _currentChanged;
            listManager.BindingComplete += new BindingCompleteEventHandler(ErrorManager_BindingComplete);

            if (listManager is CurrencyManager currManager)
            {
                currManager.ItemChanged += new ItemChangedEventHandler(ErrorManager_ItemChanged);
                currManager.Bindings.CollectionChanged += new CollectionChangeEventHandler(ErrorManager_BindingsChanged);
            }
        }

        private void UnwireEvents(BindingManagerBase listManager)
        {
            if (listManager is null)
            {
                return;
            }

            listManager.CurrentChanged -= _currentChanged;
            listManager.BindingComplete -= new BindingCompleteEventHandler(ErrorManager_BindingComplete);

            if (listManager is CurrencyManager currManager)
            {
                currManager.ItemChanged -= new ItemChangedEventHandler(ErrorManager_ItemChanged);
                currManager.Bindings.CollectionChanged -= new CollectionChangeEventHandler(ErrorManager_BindingsChanged);
            }
        }

        private void ErrorManager_BindingComplete(object sender, BindingCompleteEventArgs e)
        {
            Binding binding = e.Binding;
            if (binding != null && binding.Control != null)
            {
                SetError(binding.Control, (e.ErrorText ?? string.Empty));
            }
        }

        private void ErrorManager_BindingsChanged(object sender, CollectionChangeEventArgs e)
        {
            ErrorManager_CurrentChanged(_errorManager, e);
        }

        private void ParentControl_BindingContextChanged(object sender, EventArgs e)
        {
            SetErrorManager(DataSource, DataMember, true);
        }

        public void UpdateBinding()
        {
            ErrorManager_CurrentChanged(_errorManager, EventArgs.Empty);
        }

        private void ErrorManager_ItemChanged(object sender, ItemChangedEventArgs e)
        {
            BindingsCollection errBindings = _errorManager.Bindings;
            int bindingsCount = errBindings.Count;

            // If the list became empty then reset the errors
            if (e.Index == -1 && _errorManager.Count == 0)
            {
                for (int j = 0; j < bindingsCount; j++)
                {
                    if (errBindings[j].Control != null)
                    {
                        // Ignore everything but bindings to Controls
                        SetError(errBindings[j].Control, "");
                    }
                }
            }
            else
            {
                ErrorManager_CurrentChanged(sender, e);
            }
        }

        private void ErrorManager_CurrentChanged(object sender, EventArgs e)
        {
            Debug.Assert(sender == _errorManager, "who else can send us messages?");

            // Flush the old list
            if (_errorManager.Count == 0)
            {
                return;
            }

            object value = _errorManager.Current;
            if (!(value is IDataErrorInfo))
            {
                return;
            }

            BindingsCollection errBindings = _errorManager.Bindings;
            int bindingsCount = errBindings.Count;

            // We need to delete the blinkPhases from each controlItem (suppose that the error that
            // we get is the same error. then we want to show the error and not blink )
            foreach (ControlItem ctl in _items.Values)
            {
                ctl.BlinkPhase = 0;
            }

            // We can only show one error per control, so we will build up a string.
            Hashtable controlError = new Hashtable(bindingsCount);

            for (int j = 0; j < bindingsCount; j++)
            {
                // Ignore everything but bindings to Controls
                if (errBindings[j].Control is null)
                {
                    continue;
                }

                Binding dataBinding = errBindings[j];
                string error = ((IDataErrorInfo)value)[dataBinding.BindingMemberInfo.BindingField];

                if (error is null)
                {
                    error = string.Empty;
                }

                string outputError = string.Empty;
                if (controlError.Contains(dataBinding.Control))
                {
                    outputError = (string)controlError[dataBinding.Control];
                }

                // Utilize the error string without including the field name.
                if (string.IsNullOrEmpty(outputError))
                {
                    outputError = error;
                }
                else
                {
                    outputError = string.Concat(outputError, "\r\n", error);
                }

                controlError[dataBinding.Control] = outputError;
            }

            IEnumerator enumerator = controlError.GetEnumerator();
            while (enumerator.MoveNext())
            {
                DictionaryEntry entry = (DictionaryEntry)enumerator.Current;
                SetError((Control)entry.Key, (string)entry.Value);
            }
        }

        /// <summary>
        ///  Returns or set the rate in milliseconds at which the error icon flashes.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(DefaultBlinkRate)]
        [SRDescription(nameof(SR.ErrorProviderBlinkRateDescr))]
        [RefreshProperties(RefreshProperties.Repaint)]
        public int BlinkRate
        {
            get => _blinkRate;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, SR.BlinkRateMustBeZeroOrMore);
                }

                _blinkRate = value;
                // If we set the blinkRate = 0 then set BlinkStyle = NeverBlink
                if (_blinkRate == 0)
                {
                    BlinkStyle = ErrorBlinkStyle.NeverBlink;
                }
            }
        }

        /// <summary>
        ///  Demand load and cache the default icon.
        /// </summary>
        private static Icon DefaultIcon
        {
            get
            {
                if (t_defaultIcon is null)
                {
                    lock (typeof(ErrorProvider))
                    {
                        t_defaultIcon ??= new Icon(typeof(ErrorProvider), "Error");
                    }
                }

                return t_defaultIcon;
            }
        }

        /// <summary>
        ///  Returns or sets the Icon that displayed next to a control when an error
        ///  description string has been set for the control. For best results, an
        ///  icon containing a 16 by 16 icon should be used.
        /// </summary>
        [Localizable(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ErrorProviderIconDescr))]
        public Icon Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value ?? throw new ArgumentNullException(nameof(value));
                DisposeRegion();
                ErrorWindow[] array = new ErrorWindow[_windows.Values.Count];
                _windows.Values.CopyTo(array, 0);
                for (int i = 0; i < array.Length; i++)
                {
                    array[i].Update(timerCaused: false);
                }
            }
        }

        /// <summary>
        ///  Create the icon region on demand.
        /// </summary>
        internal IconRegion Region => _region ??= new IconRegion(Icon);

        /// <summary>
        ///  Begin bulk member initialization - deferring binding to data source until EndInit is reached
        /// </summary>
        void ISupportInitialize.BeginInit()
        {
            _initializing = true;
        }

        /// <summary>
        ///  End bulk member initialization by binding to data source
        /// </summary>
        private void EndInitCore()
        {
            _initializing = false;

            if (_setErrorManagerOnEndInit)
            {
                _setErrorManagerOnEndInit = false;
                SetErrorManager(DataSource, DataMember, true);
            }
        }

        /// <summary>
        ///  Check to see if DataSource has completed its initialization, before ending our initialization.
        ///  If DataSource is still initializing, hook its Initialized event and wait for it to signal completion.
        ///  If DataSource is already initialized, just go ahead and complete our initialization now.
        /// </summary>
        void ISupportInitialize.EndInit()
        {
            if (DataSource is ISupportInitializeNotification dsInit && !dsInit.IsInitialized)
            {
                dsInit.Initialized += new EventHandler(DataSource_Initialized);
            }
            else
            {
                EndInitCore();
            }
        }

        /// <summary>
        ///  Respond to late completion of the DataSource's initialization, by completing our own initialization.
        ///  This situation can arise if the call to the DataSource's EndInit() method comes after the call to the
        ///  BindingSource's EndInit() method (since code-generated ordering of these calls is non-deterministic).
        /// </summary>
        private void DataSource_Initialized(object sender, EventArgs e)
        {
            ISupportInitializeNotification dsInit = (DataSource as ISupportInitializeNotification);

            Debug.Assert(dsInit != null, "ErrorProvider: ISupportInitializeNotification.Initialized event received, but current DataSource does not support ISupportInitializeNotification!");
            Debug.Assert(dsInit.IsInitialized, "ErrorProvider: DataSource sent ISupportInitializeNotification.Initialized event but before it had finished initializing.");

            if (dsInit != null)
            {
                dsInit.Initialized -= new EventHandler(DataSource_Initialized);
            }

            EndInitCore();
        }

        /// <summary>
        ///  Clears all errors being tracked by this error provider, ie. undoes all previous calls to SetError.
        /// </summary>
        public void Clear()
        {
            ErrorWindow[] w = new ErrorWindow[_windows.Values.Count];
            _windows.Values.CopyTo(w, 0);
            for (int i = 0; i < w.Length; i++)
            {
                w[i].Dispose();
            }
            _windows.Clear();
            foreach (ControlItem item in _items.Values)
            {
                item?.Dispose();
            }

            _items.Clear();
        }

        /// <summary>
        ///  Returns whether a control can be extended.
        /// </summary>
        public bool CanExtend(object extendee)
        {
            return extendee is Control && !(extendee is Form);
        }

        /// <summary>
        ///  Release any resources that this component is using. After calling Dispose,
        ///  the component should no longer be used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Clear();
                DisposeRegion();
                UnwireEvents(_errorManager);
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///  Helper to dispose the cached icon region.
        /// </summary>
        void DisposeRegion()
        {
            if (_region != null)
            {
                _region.Dispose();
                _region = null;
            }
        }

        /// <summary>
        ///  Helper to make sure we have allocated a control item for this control.
        /// </summary>
        private ControlItem EnsureControlItem(Control control)
        {
            if (control is null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            ControlItem item = (ControlItem)_items[control];
            if (item is null)
            {
                item = new ControlItem(this, control, (IntPtr)(++_itemIdCounter));
                _items[control] = item;
            }
            return item;
        }

        /// <summary>
        ///  Helper to make sure we have allocated an error window for this control.
        /// </summary>
        internal ErrorWindow EnsureErrorWindow(Control parent)
        {
            ErrorWindow window = (ErrorWindow)_windows[parent];
            if (window is null)
            {
                window = new ErrorWindow(this, parent);
                _windows[parent] = window;
            }

            return window;
        }

        /// <summary>
        ///  Returns the current error description string for the specified control.
        /// </summary>
        [DefaultValue("")]
        [Localizable(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ErrorProviderErrorDescr))]
        public string GetError(Control control) => EnsureControlItem(control).Error;

        /// <summary>
        ///  Returns where the error icon should be placed relative to the control.
        /// </summary>
        [DefaultValue(DefaultIconAlignment)]
        [Localizable(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ErrorProviderIconAlignmentDescr))]
        public ErrorIconAlignment GetIconAlignment(Control control) => EnsureControlItem(control).IconAlignment;

        /// <summary>
        ///  Returns the amount of extra space to leave next to the error icon.
        /// </summary>
        [DefaultValue(0)]
        [Localizable(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.ErrorProviderIconPaddingDescr))]
        public int GetIconPadding(Control control) => EnsureControlItem(control).IconPadding;

        private void ResetIcon() => Icon = DefaultIcon;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnRightToLeftChanged(EventArgs e)
        {
            foreach (ErrorWindow w in _windows.Values)
            {
                w.Update(false);
            }

            _onRightToLeftChanged?.Invoke(this, e);
        }

        /// <summary>
        ///  Sets the error description string for the specified control.
        /// </summary>
        public void SetError(Control control, string value)
        {
            EnsureControlItem(control).Error = value;
        }

        /// <summary>
        ///  Sets where the error icon should be placed relative to the control.
        /// </summary>
        public void SetIconAlignment(Control control, ErrorIconAlignment value)
        {
            EnsureControlItem(control).IconAlignment = value;
        }

        /// <summary>
        ///  Sets the amount of extra space to leave next to the error icon.
        /// </summary>
        public void SetIconPadding(Control control, int padding)
        {
            EnsureControlItem(control).IconPadding = padding;
        }

        private bool ShouldSerializeIcon() => Icon != DefaultIcon;

        /// <summary>
        ///  There is one ErrorWindow for each control parent. It is parented to the
        ///  control parent. The window's region is made up of the regions from icons
        ///  of all child icons. The window's size is the enclosing rectangle for all
        ///  the regions. A tooltip window is created as a child of this window. The
        ///  rectangle associated with each error icon being displayed is added as a
        ///  tool to the tooltip window.
        /// </summary>
        internal class ErrorWindow : NativeWindow
        {
            private static readonly int s_accessibilityProperty = PropertyStore.CreateKey();

            private readonly List<ControlItem> _items = new List<ControlItem>();
            private readonly Control _parent;
            private readonly ErrorProvider _provider;
            private Rectangle _windowBounds;
            private Timer _timer;
            private NativeWindow _tipWindow;

            /// <summary>
            ///  Construct an error window for this provider and control parent.
            /// </summary>
            public ErrorWindow(ErrorProvider provider, Control parent)
            {
                _provider = provider;
                _parent = parent;
                Properties = new PropertyStore();
            }

            /// <summary>
            ///  The Accessibility Object for this ErrorProvider
            /// </summary>
            internal AccessibleObject AccessibilityObject
            {
                get
                {
                    AccessibleObject accessibleObject = (AccessibleObject)Properties.GetObject(s_accessibilityProperty);

                    if (accessibleObject is null)
                    {
                        accessibleObject = CreateAccessibilityInstance();
                        Properties.SetObject(s_accessibilityProperty, accessibleObject);
                    }

                    return accessibleObject;
                }
            }

            /// <summary>
            ///  This is called when a control would like to show an error icon.
            /// </summary>
            public void Add(ControlItem item)
            {
                _items.Add(item);
                if (!EnsureCreated())
                {
                    return;
                }

                var toolInfo = new ComCtl32.ToolInfoWrapper<ErrorWindow>(this, item.Id, ComCtl32.TTF.SUBCLASS, item.Error);
                toolInfo.SendMessage(_tipWindow, (User32.WM)ComCtl32.TTM.ADDTOOLW);

                Update(timerCaused: false);
            }

            internal List<ControlItem> ControlItems => _items;

            /// <summary>
            ///  Constructs the new instance of the accessibility object for this ErrorProvider. Subclasses
            ///  should not call base.CreateAccessibilityObject.
            /// </summary>
            private AccessibleObject CreateAccessibilityInstance()
            {
                return new ErrorWindowAccessibleObject(this);
            }

            /// <summary>
            ///  Called to get rid of any resources the Object may have.
            /// </summary>
            public void Dispose() => EnsureDestroyed();

            /// <summary>
            ///  Make sure the error window is created, and the tooltip window is created.
            /// </summary>
            bool EnsureCreated()
            {
                if (Handle == IntPtr.Zero)
                {
                    if (!_parent.IsHandleCreated)
                    {
                        return false;
                    }
                    CreateParams cparams = new CreateParams
                    {
                        Caption = string.Empty,
                        Style = (int)(User32.WS.VISIBLE | User32.WS.CHILD),
                        ClassStyle = (int)User32.CS.DBLCLKS,
                        X = 0,
                        Y = 0,
                        Width = 0,
                        Height = 0,
                        Parent = _parent.Handle
                    };

                    CreateHandle(cparams);

                    var icc = new ComCtl32.INITCOMMONCONTROLSEX
                    {
                        dwICC = ComCtl32.ICC.TAB_CLASSES
                    };
                    ComCtl32.InitCommonControlsEx(ref icc);

                    cparams = new CreateParams
                    {
                        Parent = Handle,
                        ClassName = ComCtl32.WindowClasses.TOOLTIPS_CLASS,
                        Style = (int)ComCtl32.TTS.ALWAYSTIP
                    };
                    _tipWindow = new NativeWindow();
                    _tipWindow.CreateHandle(cparams);

                    User32.SendMessageW(_tipWindow, (User32.WM)ComCtl32.TTM.SETMAXTIPWIDTH, IntPtr.Zero, (IntPtr)SystemInformation.MaxWindowTrackSize.Width);
                    User32.SetWindowPos(
                        new HandleRef(_tipWindow, _tipWindow.Handle),
                        User32.HWND_TOP,
                        flags: User32.SWP.NOSIZE | User32.SWP.NOMOVE | User32.SWP.NOACTIVATE);
                    User32.SendMessageW(_tipWindow, (User32.WM)ComCtl32.TTM.SETDELAYTIME, (IntPtr)ComCtl32.TTDT.INITIAL, (IntPtr)0);
                }

                return true;
            }

            /// <summary>
            ///  Destroy the timer, toolwindow, and the error window itself.
            /// </summary>
            private void EnsureDestroyed()
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
                if (_tipWindow != null)
                {
                    _tipWindow.DestroyHandle();
                    _tipWindow = null;
                }

                // Hide the window and invalidate the parent to ensure
                // that we leave no visual artifacts. given that we
                // have a bizare region window, this is needed.
                User32.SetWindowPos(
                    new HandleRef(this, Handle),
                    User32.HWND_TOP,
                    _windowBounds.X,
                    _windowBounds.Y,
                    _windowBounds.Width,
                    _windowBounds.Height,
                    User32.SWP.HIDEWINDOW | User32.SWP.NOSIZE | User32.SWP.NOMOVE);
                _parent?.Invalidate(true);
                DestroyHandle();
            }

            private unsafe void MirrorDcIfNeeded(Gdi32.HDC hdc)
            {
                if (_parent.IsMirrored)
                {
                    // Mirror the DC
                    Gdi32.SetMapMode(hdc, Gdi32.MM.ANISOTROPIC);
                    Gdi32.GetViewportExtEx(hdc, out Size originalExtents);
                    Gdi32.SetViewportExtEx(hdc, -originalExtents.Width, originalExtents.Height, null);
                    Gdi32.GetViewportOrgEx(hdc, out Point originalOrigin);
                    Gdi32.SetViewportOrgEx(hdc, originalOrigin.X + _windowBounds.Width - 1, originalOrigin.Y, null);
                }
            }

            /// <summary>
            ///  This is called when the error window needs to paint. We paint each icon at its correct location.
            /// </summary>
            private unsafe void OnPaint()
            {
                using var hdc = new User32.BeginPaintScope(Handle);
                using var save = new Gdi32.SaveDcScope(hdc);

                MirrorDcIfNeeded(hdc);

                for (int i = 0; i < _items.Count; i++)
                {
                    ControlItem item = _items[i];
                    Rectangle bounds = item.GetIconBounds(_provider.Region.Size);
                    User32.DrawIconEx(
                        hdc,
                        bounds.X - _windowBounds.X,
                        bounds.Y - _windowBounds.Y,
                        _provider.Region,
                        bounds.Width, bounds.Height);
                }
            }

            protected override void OnThreadException(Exception e)
            {
                Application.OnThreadException(e);
            }

            /// <summary>
            ///  This is called when an error icon is flashing, and the view needs to be updatd.
            /// </summary>
            private void OnTimer(object sender, EventArgs e)
            {
                int blinkPhase = 0;
                for (int i = 0; i < _items.Count; i++)
                {
                    blinkPhase += _items[i].BlinkPhase;
                }
                if (blinkPhase == 0 && _provider.BlinkStyle != ErrorBlinkStyle.AlwaysBlink)
                {
                    Debug.Assert(_timer != null);
                    _timer.Stop();
                }
                Update(timerCaused: true);
            }

            private void OnToolTipVisibilityChanging(IntPtr id, bool toolTipShown)
            {
                for (int i = 0; i < _items.Count; i++)
                {
                    if (_items[i].Id == id)
                    {
                        _items[i].ToolTipShown = toolTipShown;
                    }
                }
#if DEBUG
                int shownTooltips = 0;
                for (int j = 0; j < _items.Count; j++)
                {
                    if (_items[j].ToolTipShown)
                    {
                        shownTooltips++;
                    }
                }
                Debug.Assert(shownTooltips <= 1);
#endif
            }

            /// <summary>
            ///  Retrieves our internal property storage object. If you have a property
            ///  whose value is not always set, you should store it in here to save
            ///  space.
            /// </summary>
            internal PropertyStore Properties { get; }

            /// <summary>
            ///  This is called when a control no longer needs to display an error icon.
            /// </summary>
            public void Remove(ControlItem item)
            {
                _items.Remove(item);

                if (_tipWindow != null)
                {
                    var info = new ComCtl32.ToolInfoWrapper<ErrorWindow>(this, item.Id);
                    info.SendMessage(_tipWindow, (User32.WM)ComCtl32.TTM.DELTOOLW);
                }

                if (_items.Count == 0)
                {
                    EnsureDestroyed();
                }
                else
                {
                    Update(timerCaused: false);
                }
            }

            /// <summary>
            ///  Start the blinking process. The timer will fire until there are no more
            ///  icons that need to blink.
            /// </summary>
            public void StartBlinking()
            {
                if (_timer is null)
                {
                    _timer = new Timer();
                    _timer.Tick += new EventHandler(OnTimer);
                }

                _timer.Interval = _provider.BlinkRate;
                _timer.Start();
                Update(timerCaused: false);
            }

            public void StopBlinking()
            {
                _timer?.Stop();
                Update(timerCaused: false);
            }

            /// <summary>
            ///  Move and size the error window, compute and set the window region, set the tooltip
            ///  rectangles and descriptions. This basically brings the error window up to date with
            ///  the internal data structures.
            /// </summary>
            public unsafe void Update(bool timerCaused)
            {
                IconRegion iconRegion = _provider.Region;
                Size size = iconRegion.Size;
                _windowBounds = Rectangle.Empty;
                for (int i = 0; i < _items.Count; i++)
                {
                    ControlItem item = _items[i];
                    Rectangle iconBounds = item.GetIconBounds(size);
                    if (_windowBounds.IsEmpty)
                    {
                        _windowBounds = iconBounds;
                    }
                    else
                    {
                        _windowBounds = Rectangle.Union(_windowBounds, iconBounds);
                    }
                }

                using var windowRegion = new Region(new Rectangle(0, 0, 0, 0));

                for (int i = 0; i < _items.Count; i++)
                {
                    ControlItem item = _items[i];
                    Rectangle iconBounds = item.GetIconBounds(size);
                    iconBounds.X -= _windowBounds.X;
                    iconBounds.Y -= _windowBounds.Y;

                    bool showIcon = true;
                    if (!item.ToolTipShown)
                    {
                        switch (_provider.BlinkStyle)
                        {
                            case ErrorBlinkStyle.NeverBlink:
                                // always show icon
                                break;
                            case ErrorBlinkStyle.BlinkIfDifferentError:
                                showIcon = (item.BlinkPhase == 0) || (item.BlinkPhase > 0 && (item.BlinkPhase & 1) == (i & 1));
                                break;
                            case ErrorBlinkStyle.AlwaysBlink:
                                showIcon = ((i & 1) == 0) == _provider._showIcon;
                                break;
                        }
                    }

                    if (showIcon)
                    {
                        iconRegion.Region.Translate(iconBounds.X, iconBounds.Y);
                        windowRegion.Union(iconRegion.Region);
                        iconRegion.Region.Translate(-iconBounds.X, -iconBounds.Y);
                    }

                    if (_tipWindow != null)
                    {
                        ComCtl32.TTF flags = ComCtl32.TTF.SUBCLASS;
                        if (_provider.RightToLeft)
                        {
                            flags |= ComCtl32.TTF.RTLREADING;
                        }

                        var toolInfo = new ComCtl32.ToolInfoWrapper<ErrorWindow>(this, item.Id, flags, item.Error, iconBounds);
                        toolInfo.SendMessage(_tipWindow, (User32.WM)ComCtl32.TTM.SETTOOLINFOW);
                    }

                    if (timerCaused && item.BlinkPhase > 0)
                    {
                        item.BlinkPhase--;
                    }
                }

                if (timerCaused)
                {
                    _provider._showIcon = !_provider._showIcon;
                }

                using var hdc = new User32.GetDcScope(Handle);
                using var save = new Gdi32.SaveDcScope(hdc);
                MirrorDcIfNeeded(hdc);

                using Graphics g = hdc.CreateGraphics();
                using var windowRegionHandle = new Gdi32.RegionScope(windowRegion, g);
                if (User32.SetWindowRgn(this, windowRegionHandle, BOOL.TRUE) != 0)
                {
                    // The HWnd owns the region.
                    windowRegionHandle.RelinquishOwnership();
                }

                User32.SetWindowPos(
                    new HandleRef(this, Handle),
                    User32.HWND_TOP,
                    _windowBounds.X,
                    _windowBounds.Y,
                    _windowBounds.Width,
                    _windowBounds.Height,
                    User32.SWP.NOACTIVATE);
                User32.InvalidateRect(new HandleRef(this, Handle), null, BOOL.FALSE);
            }

            /// <summary>
            ///  Handles the WM_GETOBJECT message. Used for accessibility.
            /// </summary>
            private void WmGetObject(ref Message m)
            {
                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "In WmGetObject, this = " + GetType().FullName + ", lParam = " + m.LParam.ToString());

                if (m.Msg == (int)User32.WM.GETOBJECT && m.LParam == (IntPtr)NativeMethods.UiaRootObjectId)
                {
                    // If the requested object identifier is UiaRootObjectId,
                    // we should return an UI Automation provider using the UiaReturnRawElementProvider function.
                    InternalAccessibleObject intAccessibleObject = new InternalAccessibleObject(AccessibilityObject);
                    m.Result = UiaCore.UiaReturnRawElementProvider(
                        new HandleRef(this, Handle),
                        m.WParam,
                        m.LParam,
                        intAccessibleObject);

                    return;
                }

                // some accessible object requested that we don't care about, so do default message processing
                DefWndProc(ref m);
            }

            /// <summary>
            ///  Called when the error window gets a windows message.
            /// </summary>
            protected unsafe override void WndProc(ref Message m)
            {
                switch ((User32.WM)m.Msg)
                {
                    case User32.WM.GETOBJECT:
                        WmGetObject(ref m);
                        break;
                    case User32.WM.NOTIFY:
                        User32.NMHDR* nmhdr = (User32.NMHDR*)m.LParam;
                        if (nmhdr->code == (int)ComCtl32.TTN.SHOW || nmhdr->code == (int)ComCtl32.TTN.POP)
                        {
                            OnToolTipVisibilityChanging(nmhdr->idFrom, nmhdr->code == (int)ComCtl32.TTN.SHOW);
                        }
                        break;
                    case User32.WM.ERASEBKGND:
                        break;
                    case User32.WM.PAINT:
                        OnPaint();
                        break;
                    default:
                        base.WndProc(ref m);
                        break;
                }
            }
        }

        /// <summary>
        ///  There is one ControlItem for each control that the ErrorProvider is tracking state for.
        ///  It contains the values of all the extender properties.
        /// </summary>
        internal class ControlItem
        {
            private static readonly int s_accessibilityProperty = PropertyStore.CreateKey();

            private string _error;
            private readonly Control _control;
            private ErrorWindow _window;
            private readonly ErrorProvider _provider;
            private int _iconPadding;
            private ErrorIconAlignment _iconAlignment;
            private const int _startingBlinkPhase = 10; // We want to blink 5 times

            /// <summary>
            ///  Construct the item with its associated control, provider, and a unique ID. The ID is
            ///  used for the tooltip ID.
            /// </summary>
            public ControlItem(ErrorProvider provider, Control control, IntPtr id)
            {
                _iconAlignment = DefaultIconAlignment;
                _error = string.Empty;
                Id = id;
                _control = control;
                _provider = provider;
                _control.HandleCreated += new EventHandler(OnCreateHandle);
                _control.HandleDestroyed += new EventHandler(OnDestroyHandle);
                _control.LocationChanged += new EventHandler(OnBoundsChanged);
                _control.SizeChanged += new EventHandler(OnBoundsChanged);
                _control.VisibleChanged += new EventHandler(OnParentVisibleChanged);
                _control.ParentChanged += new EventHandler(OnParentVisibleChanged);
                Properties = new PropertyStore();
            }

            /// <summary>
            ///  The Accessibility Object for this ErrorProvider
            /// </summary>
            internal AccessibleObject AccessibilityObject
            {
                get
                {
                    AccessibleObject accessibleObject = (AccessibleObject)Properties.GetObject(s_accessibilityProperty);

                    if (accessibleObject is null)
                    {
                        accessibleObject = CreateAccessibilityInstance();
                        Properties.SetObject(s_accessibilityProperty, accessibleObject);
                    }

                    return accessibleObject;
                }
            }

            /// <summary>
            ///  Constructs the new instance of the accessibility object for this ErrorProvider. Subclasses
            ///  should not call base.CreateAccessibilityObject.
            /// </summary>
            private AccessibleObject CreateAccessibilityInstance()
            {
                return new ControlItemAccessibleObject(this, _window, _control.ParentInternal, _provider);
            }

            public void Dispose()
            {
                if (_control != null)
                {
                    _control.HandleCreated -= new EventHandler(OnCreateHandle);
                    _control.HandleDestroyed -= new EventHandler(OnDestroyHandle);
                    _control.LocationChanged -= new EventHandler(OnBoundsChanged);
                    _control.SizeChanged -= new EventHandler(OnBoundsChanged);
                    _control.VisibleChanged -= new EventHandler(OnParentVisibleChanged);
                    _control.ParentChanged -= new EventHandler(OnParentVisibleChanged);
                }

                _error = string.Empty;
            }

            /// <summary>
            ///  Returns the unique ID for this control. The ID used as the tooltip ID.
            /// </summary>
            public IntPtr Id { get; }

            /// <summary>
            ///  Returns or set the phase of blinking that this control is currently
            ///  in.  If zero, the control is not blinking. If odd, then the control
            ///  is blinking, but invisible. If even, the control is blinking and
            ///  currently visible. Each time the blink timer fires, this value is
            ///  reduced by one (until zero), thus causing the error icon to appear
            ///  or disappear.
            /// </summary>
            public int BlinkPhase { get; set; }

            /// <summary>
            ///  Returns or sets the icon padding for the control.
            /// </summary>
            public int IconPadding
            {
                get => _iconPadding;
                set
                {
                    if (_iconPadding == value)
                    {
                        return;
                    }

                    _iconPadding = value;
                    UpdateWindow();
                }
            }

            /// <summary>
            ///  Returns or sets the error description string for the control.
            /// </summary>
            public string Error
            {
                get => _error;
                set
                {
                    if (value is null)
                    {
                        value = string.Empty;
                    }

                    // If the error is the same and the blinkStyle is not AlwaysBlink, then
                    // we should not add the error and not start blinking.
                    if (_error.Equals(value) && _provider.BlinkStyle != ErrorBlinkStyle.AlwaysBlink)
                    {
                        return;
                    }

                    bool adding = _error.Length == 0;
                    _error = value;
                    if (value.Length == 0)
                    {
                        RemoveFromWindow();
                    }
                    else
                    {
                        if (adding)
                        {
                            AddToWindow();
                        }
                        else
                        {
                            if (_provider.BlinkStyle != ErrorBlinkStyle.NeverBlink)
                            {
                                StartBlinking();
                            }
                            else
                            {
                                UpdateWindow();
                            }
                        }
                    }
                }
            }

            /// <summary>
            ///  Returns or sets the location of the error icon for the control.
            /// </summary>
            public ErrorIconAlignment IconAlignment
            {
                get => _iconAlignment;
                set
                {
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)ErrorIconAlignment.TopLeft, (int)ErrorIconAlignment.BottomRight))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ErrorIconAlignment));
                    }

                    if (_iconAlignment == value)
                    {
                        return;
                    }

                    _iconAlignment = value;
                    UpdateWindow();
                }
            }

            /// <summary>
            ///  Returns true if the tooltip for this control item is currently shown.
            /// </summary>
            public bool ToolTipShown { get; set; }

            internal ErrorIconAlignment RTLTranslateIconAlignment(ErrorIconAlignment align)
            {
                if (_provider.RightToLeft)
                {
                    switch (align)
                    {
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
                else
                {
                    return align;
                }
            }

            /// <summary>
            ///  Returns the location of the icon in the same coordinate system as the control being
            ///  extended. The size passed in is the size of the icon.
            /// </summary>
            internal Rectangle GetIconBounds(Size size)
            {
                int x = 0;
                int y = 0;

                switch (RTLTranslateIconAlignment(IconAlignment))
                {
                    case ErrorIconAlignment.TopLeft:
                    case ErrorIconAlignment.MiddleLeft:
                    case ErrorIconAlignment.BottomLeft:
                        x = _control.Left - size.Width - _iconPadding;
                        break;
                    case ErrorIconAlignment.TopRight:
                    case ErrorIconAlignment.MiddleRight:
                    case ErrorIconAlignment.BottomRight:
                        x = _control.Right + _iconPadding;
                        break;
                }

                switch (IconAlignment)
                {
                    case ErrorIconAlignment.TopLeft:
                    case ErrorIconAlignment.TopRight:
                        y = _control.Top;
                        break;
                    case ErrorIconAlignment.MiddleLeft:
                    case ErrorIconAlignment.MiddleRight:
                        y = _control.Top + (_control.Height - size.Height) / 2;
                        break;
                    case ErrorIconAlignment.BottomLeft:
                    case ErrorIconAlignment.BottomRight:
                        y = _control.Bottom - size.Height;
                        break;
                }

                return new Rectangle(x, y, size.Width, size.Height);
            }

            /// <summary>
            ///  If this control's error icon has been added to the error window, then update the
            ///  window state because some property has changed.
            /// </summary>
            private void UpdateWindow() => _window?.Update(timerCaused: false);

            /// <summary>
            ///  If this control's error icon has been added to the error window, then start blinking
            ///  the error window.
            /// </summary>
            private void StartBlinking()
            {
                if (_window != null)
                {
                    BlinkPhase = _startingBlinkPhase;
                    _window.StartBlinking();
                }
            }

            /// <summary>
            ///  Add this control's error icon to the error window.
            /// </summary>
            private void AddToWindow()
            {
                // if we are recreating the control, then add the control.
                if (_window is null &&
                    (_control.Created || _control.RecreatingHandle) &&
                    _control.Visible && _control.ParentInternal != null &&
                    _error.Length > 0)
                {
                    _window = _provider.EnsureErrorWindow(_control.ParentInternal);
                    _window.Add(this);
                    // Make sure that we blink if the style is set to AlwaysBlink or BlinkIfDifferrentError
                    if (_provider.BlinkStyle != ErrorBlinkStyle.NeverBlink)
                    {
                        StartBlinking();
                    }
                }
            }

            /// <summary>
            ///  Remove this control's error icon from the error window.
            /// </summary>
            private void RemoveFromWindow()
            {
                if (_window != null)
                {
                    _window.Remove(this);
                    _window = null;
                }
            }

            /// <summary>
            ///  This is called when a property on the control is changed.
            /// </summary>
            private void OnBoundsChanged(object sender, EventArgs e) => UpdateWindow();

            void OnParentVisibleChanged(object sender, EventArgs e)
            {
                BlinkPhase = 0;
                RemoveFromWindow();
                AddToWindow();
            }

            /// <summary>
            ///  Retrieves our internal property storage object. If you have a property
            ///  whose value is not always set, you should store it in here to save
            ///  space.
            /// </summary>
            private PropertyStore Properties { get; }

            /// <summary>
            ///  This is called when the control's handle is created.
            /// </summary>
            private void OnCreateHandle(object sender, EventArgs e) => AddToWindow();

            /// <summary>
            ///  This is called when the control's handle is destroyed.
            /// </summary>
            private void OnDestroyHandle(object sender, EventArgs e) => RemoveFromWindow();
        }

        /// <summary>
        ///  This represents the HRGN of icon. The region is calculate from the icon's mask.
        /// </summary>
        internal class IconRegion : IHandle
        {
            private Region region;
            private readonly Icon icon;

            /// <summary>
            ///  Constructor that takes an Icon and extracts its 16x16 version.
            /// </summary>
            public IconRegion(Icon icon)
            {
                this.icon = new Icon(icon, 16, 16);
            }

            /// <summary>
            ///  Returns the handle of the icon.
            /// </summary>
            public IntPtr Handle => icon.Handle;

            /// <summary>
            ///  Returns the handle of the region.
            /// </summary>
            public Region Region
            {
                get
                {
                    if (region is null)
                    {
                        region = new Region(new Rectangle(0, 0, 0, 0));

                        IntPtr mask = IntPtr.Zero;
                        try
                        {
                            Size size = icon.Size;
                            Bitmap bitmap = icon.ToBitmap();
                            mask = ControlPaint.CreateHBitmapTransparencyMask(bitmap);
                            bitmap.Dispose();

                            // It is been observed that users can use non standard size icons (not a 16 bit multiples for width and height)
                            // and GetBitmapBits method allocate bytes in multiple of 16 bits for each row. Following calculation is to get right width in bytes.
                            int bitmapBitsAllocationSize = 16;

                            // If width is not multiple of 16, we need to allocate BitmapBitsAllocationSize for remaining bits.
                            int widthInBytes = 2 * ((size.Width + 15) / bitmapBitsAllocationSize); // its in bytes.
                            byte[] bits = new byte[widthInBytes * size.Height];
                            Gdi32.GetBitmapBits(mask, bits.Length, bits);

                            for (int y = 0; y < size.Height; y++)
                            {
                                for (int x = 0; x < size.Width; x++)
                                {
                                    // see if bit is set in mask. bits in byte are reversed. 0 is black (set).
                                    if ((bits[y * widthInBytes + x / 8] & (1 << (7 - (x % 8)))) == 0)
                                    {
                                        region.Union(new Rectangle(x, y, 1, 1));
                                    }
                                }
                            }
                            region.Intersect(new Rectangle(0, 0, size.Width, size.Height));
                        }
                        finally
                        {
                            if (mask != IntPtr.Zero)
                            {
                                Gdi32.DeleteObject(mask);
                            }
                        }
                    }

                    return region;
                }
            }

            /// <summary>
            ///  Return the size of the icon.
            /// </summary>
            public Size Size => icon.Size;

            /// <summary>
            ///  Release any resources held by this Object.
            /// </summary>
            public void Dispose()
            {
                if (region != null)
                {
                    region.Dispose();
                    region = null;
                }
                icon.Dispose();
            }
        }
    }
}

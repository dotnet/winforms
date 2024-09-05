// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;

namespace System.Windows.Forms;

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
    private readonly Dictionary<Control, ControlItem> _items = [];
    private readonly Dictionary<Control, ErrorWindow> _windows = [];
    private Icon _icon = DefaultIcon;
    private IconRegion? _region;
    private int _itemIdCounter;
    private int _blinkRate;
    private ErrorBlinkStyle _blinkStyle;
    private ErrorProviderStates _state = ErrorProviderStates.ShowIcon;
    private bool ShowIcon
    {
        get => _state.HasFlag(ErrorProviderStates.ShowIcon);
        set => _state.ChangeFlags(ErrorProviderStates.ShowIcon, value);
    }

    [ThreadStatic]
    private static Icon? t_defaultIcon;

    private const int DefaultBlinkRate = 250;
    private const ErrorBlinkStyle DefaultBlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
    private const ErrorIconAlignment DefaultIconAlignment = ErrorIconAlignment.MiddleRight;

    // data binding
    private ContainerControl? _parentControl;
    private object? _dataSource;
    private string? _dataMember;
    private BindingManagerBase? _errorManager;
    private readonly EventHandler _currentChanged;

    // listen to the OnPropertyChanged event in the ContainerControl
    private readonly EventHandler? _propChangedEvent;

    private EventHandler? _onRightToLeftChanged;

    private int _errorCount;

    /// <summary>
    ///  Default constructor.
    /// </summary>
    public ErrorProvider()
    {
        _blinkRate = DefaultBlinkRate;
        _blinkStyle = DefaultBlinkStyle;
        _currentChanged = ErrorManager_CurrentChanged;
    }

    public ErrorProvider(ContainerControl parentControl)
        : this()
    {
        ArgumentNullException.ThrowIfNull(parentControl);

        _parentControl = parentControl;
        _propChangedEvent = ParentControl_BindingContextChanged;
        parentControl.BindingContextChanged += _propChangedEvent;
    }

    public ErrorProvider(IContainer container)
        : this()
    {
        ArgumentNullException.ThrowIfNull(container);

        container.Add(this);
    }

    public override ISite? Site
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
            SourceGenerated.EnumValidator.Validate(value);

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
                // Need to start blinking on all the controlItems in our items dictionary.
                _state.ChangeFlags(ErrorProviderStates.ShowIcon, true);
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
    public ContainerControl? ContainerControl
    {
        get => _parentControl;
        set
        {
            if (_parentControl == value)
            {
                return;
            }

            if (_parentControl is not null)
            {
                _parentControl.BindingContextChanged -= _propChangedEvent;
            }

            _parentControl = value;

            if (_parentControl is not null)
            {
                _parentControl.BindingContextChanged += _propChangedEvent;
            }

            SetErrorManager(DataSource, DataMember, force: true);
        }
    }

    /// <summary>
    ///  Gets a value that indicates if this <see cref="ErrorProvider"/> has any errors for any of the associated controls.
    /// </summary>
    public bool HasErrors => _errorCount > 0;

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
        get => _state.HasFlag(ErrorProviderStates.RightToLeft);
        set
        {
            if (value == _state.HasFlag(ErrorProviderStates.RightToLeft))
            {
                return;
            }

            _state.ChangeFlags(ErrorProviderStates.RightToLeft, value);
            OnRightToLeftChanged(EventArgs.Empty);
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnRightToLeftChangedDescr))]
    public event EventHandler? RightToLeftChanged
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
    public object? Tag { get; set; }

    private void SetErrorManager(object? newDataSource, string? newDataMember, bool force)
    {
        if (_state.HasFlag(ErrorProviderStates.InSetErrorManager))
        {
            return;
        }

        _state.ChangeFlags(ErrorProviderStates.InSetErrorManager, true);
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

            if (_state.HasFlag(ErrorProviderStates.Initializing))
            {
                _state.ChangeFlags(ErrorProviderStates.SetErrorManagerOnEndInit, true);
            }
            else
            {
                // Unwire the errorManager:
                UnwireEvents(_errorManager);

                // Get the new errorManager
                if (_parentControl is not null && _dataSource is not null && _parentControl.BindingContext is not null)
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
                if (_errorManager is not null)
                {
                    UpdateBinding();
                }
            }
        }
        finally
        {
            _state.ChangeFlags(ErrorProviderStates.InSetErrorManager, false);
        }
    }

    /// <summary>
    ///  Indicates the source of data to bind errors against.
    /// </summary>
    [DefaultValue(null)]
    [SRCategory(nameof(SR.CatData))]
    [AttributeProvider(typeof(IListSource))]
    [SRDescription(nameof(SR.ErrorProviderDataSourceDescr))]
    public object? DataSource
    {
        get => _dataSource;
        set
        {
            if (_parentControl is not null && _parentControl.BindingContext is not null && value is not null && !string.IsNullOrEmpty(_dataMember))
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

    private bool ShouldSerializeDataSource() => _dataSource is not null;

    /// <summary>
    ///  Indicates the sub-list of data from the DataSource to bind errors against.
    /// </summary>
    [DefaultValue(null)]
    [SRCategory(nameof(SR.CatData))]
    [Editor($"System.Windows.Forms.Design.DataMemberListEditor, {AssemblyRef.SystemDesign}", typeof(Drawing.Design.UITypeEditor))]
    [SRDescription(nameof(SR.ErrorProviderDataMemberDescr))]
    public string? DataMember
    {
        get => _dataMember;
        set
        {
            value ??= string.Empty;

            SetErrorManager(DataSource, value, false);
        }
    }

    private bool ShouldSerializeDataMember() => !string.IsNullOrEmpty(_dataMember);

    public void BindToDataAndErrors(object? newDataSource, string? newDataMember)
    {
        SetErrorManager(newDataSource, newDataMember, false);
    }

    private void WireEvents(BindingManagerBase? listManager)
    {
        if (listManager is null)
        {
            return;
        }

        listManager.CurrentChanged += _currentChanged;
        listManager.BindingComplete += ErrorManager_BindingComplete;

        if (listManager is CurrencyManager currManager)
        {
            currManager.ItemChanged += ErrorManager_ItemChanged;
            currManager.Bindings.CollectionChanged += ErrorManager_BindingsChanged;
        }
    }

    private void UnwireEvents(BindingManagerBase? listManager)
    {
        if (listManager is null)
        {
            return;
        }

        listManager.CurrentChanged -= _currentChanged;
        listManager.BindingComplete -= ErrorManager_BindingComplete;

        if (listManager is CurrencyManager currManager)
        {
            currManager.ItemChanged -= ErrorManager_ItemChanged;
            currManager.Bindings.CollectionChanged -= ErrorManager_BindingsChanged;
        }
    }

    private void ErrorManager_BindingComplete(object? sender, BindingCompleteEventArgs e)
    {
        Binding? binding = e.Binding;
        if (binding is not null && binding.Control is not null)
        {
            SetError(binding.Control, (e.ErrorText ?? string.Empty));
        }
    }

    private void ErrorManager_BindingsChanged(object? sender, CollectionChangeEventArgs e)
    {
        ErrorManager_CurrentChanged(_errorManager, e);
    }

    private void ParentControl_BindingContextChanged(object? sender, EventArgs e)
    {
        SetErrorManager(DataSource, DataMember, true);
    }

    public void UpdateBinding()
    {
        ErrorManager_CurrentChanged(_errorManager, EventArgs.Empty);
    }

    private void ErrorManager_ItemChanged(object? sender, ItemChangedEventArgs e)
    {
        if (_errorManager is null)
        {
            return;
        }

        BindingsCollection errBindings = _errorManager.Bindings;
        int bindingsCount = errBindings.Count;

        // If the list became empty then reset the errors
        if (e.Index == -1 && _errorManager.Count == 0)
        {
            for (int j = 0; j < bindingsCount; j++)
            {
                if (errBindings[j].Control is Control control)
                {
                    // Ignore everything but bindings to Controls
                    SetError(control, string.Empty);
                }
            }
        }
        else
        {
            ErrorManager_CurrentChanged(sender, e);
        }
    }

    private void ErrorManager_CurrentChanged(object? sender, EventArgs e)
    {
        if (_errorManager is null)
        {
            return;
        }

        Debug.Assert(sender == _errorManager, "who else can send us messages?");

        // Flush the old list
        if (_errorManager.Count == 0)
        {
            return;
        }

        object? value = _errorManager.Current;
        if (value is not IDataErrorInfo dataErrorInfo)
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
        Dictionary<Control, string> controlError = new(bindingsCount);

        for (int j = 0; j < bindingsCount; j++)
        {
            // Ignore everything but bindings to Controls
            if (errBindings[j].Control is not Control control)
            {
                continue;
            }

            Binding dataBinding = errBindings[j];
            string error = dataErrorInfo[dataBinding.BindingMemberInfo.BindingField];

            error ??= string.Empty;

            if (!controlError.TryGetValue(control, out string? outputError))
            {
                outputError = string.Empty;
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

            controlError[control] = outputError;
        }

        foreach (KeyValuePair<Control, string> entry in controlError)
        {
            SetError(entry.Key, entry.Value);
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
            lock (typeof(ErrorProvider))
            {
                if (t_defaultIcon is null)
                {
                    // Error provider uses small Icon.
                    int width = PInvokeCore.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CXSMICON);
                    int height = PInvokeCore.GetSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYSMICON);
                    using Icon defaultIcon = new(typeof(ErrorProvider), "Error");
                    t_defaultIcon = new Icon(defaultIcon, width, height);
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
            _icon = value.OrThrowIfNull();
            DisposeRegion();
            ErrorWindow[] array = [.. _windows.Values];
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
        _state.ChangeFlags(ErrorProviderStates.Initializing, true);
    }

    /// <summary>
    ///  End bulk member initialization by binding to data source
    /// </summary>
    private void EndInitCore()
    {
        _state.ChangeFlags(ErrorProviderStates.Initializing, false);

        if (_state.HasFlag(ErrorProviderStates.SetErrorManagerOnEndInit))
        {
            _state.ChangeFlags(ErrorProviderStates.SetErrorManagerOnEndInit, false);
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
            dsInit.Initialized += DataSource_Initialized;
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
    private void DataSource_Initialized(object? sender, EventArgs e)
    {
        ISupportInitializeNotification? dsInit = DataSource as ISupportInitializeNotification;

        Debug.Assert(dsInit is not null, "ErrorProvider: ISupportInitializeNotification.Initialized event received, but current DataSource does not support ISupportInitializeNotification!");
        Debug.Assert(dsInit.IsInitialized, "ErrorProvider: DataSource sent ISupportInitializeNotification.Initialized event but before it had finished initializing.");

        if (dsInit is not null)
        {
            dsInit.Initialized -= DataSource_Initialized;
        }

        EndInitCore();
    }

    /// <summary>
    ///  Clears all errors being tracked by this error provider, ie. undoes all previous calls to SetError.
    /// </summary>
    public void Clear()
    {
        ErrorWindow[] w = [.. _windows.Values];
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
        _errorCount = 0;
    }

    /// <summary>
    ///  Returns whether a control can be extended.
    /// </summary>
    public bool CanExtend(object? extendee)
    {
        return extendee is Control and not Form;
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
    private void DisposeRegion()
    {
        if (_region is not null)
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
        ArgumentNullException.ThrowIfNull(control);

        if (!_items.TryGetValue(control, out ControlItem? item))
        {
            item = new ControlItem(this, control, ++_itemIdCounter);
            _items[control] = item;
        }

        return item;
    }

    /// <summary>
    ///  Helper to make sure we have allocated an error window for this control.
    /// </summary>
    internal ErrorWindow EnsureErrorWindow(Control parent)
    {
        if (!_windows.TryGetValue(parent, out ErrorWindow? window))
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
    public void SetError(Control control, string? value)
    {
        ControlItem controlItem = EnsureControlItem(control);
        bool errorChanged = controlItem.Error != value
            && (string.IsNullOrEmpty(controlItem.Error) != string.IsNullOrEmpty(value));
        EnsureControlItem(control).Error = value;
        if (errorChanged)
        {
            _errorCount += string.IsNullOrEmpty(value) ? -1 : 1;
        }

        Debug.Assert(_errorCount >= 0, "Error count should not be less than zero");
        if (PInvoke.UiaClientsAreListening())
        {
            control.AccessibilityObject.RaiseAutomationNotification(
                Automation.AutomationNotificationKind.ActionAborted,
                Automation.AutomationNotificationProcessing.All,
                value);
        }
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
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Windows.Win32.System.Power;
using Windows.Win32.System.Threading;

namespace System.Windows.Forms;

#if NET11_0_OR_GREATER
/// <summary>
///  Manages common kiosk-mode behavior for a WinForms form.
/// </summary>
/// <remarks>
///  <para>
///   <see cref="KioskModeManager"/> is a component that can be dropped on a
///   form or on a user control at design time. Set <see cref="ContainerControl"/>
///   to the owning container and the component resolves the containing
///   <see cref="Form"/> when fullscreen behavior is needed.
///  </para>
///  <para>
///   The component can make the resolved form fullscreen, optionally cover the
///   taskbar, keep the form topmost while fullscreen, suppress display and
///   system sleep, hide the mouse pointer after inactivity, and notify the
///   application when user or system activity wakes the kiosk experience.
///  </para>
/// </remarks>
/// <example>
///  <code>
///   public partial class MainForm : Form
///   {
///       private readonly KioskModeManager _kioskModeManager;
///
///       public MainForm()
///       {
///           InitializeComponent();
///
///           _kioskModeManager = new KioskModeManager
///           {
///               ContainerControl = this,
///               HideTaskbar = true,
///               TopMostInFullScreen = true,
///               MousePointerAutoHideDelay = 3000,
///               SuppressPowerSaving = true
///           };
///
///           _kioskModeManager.Wakeup += (sender, e) =>
///           {
///               if (e.Source == KioskModeWakeupSource.Mouse)
///               {
///                   // Refresh the kiosk surface after mouse activity.
///               }
///           };
///
///           if (!_kioskModeManager.FullScreen)
///           {
///              _kioskModeManager.ToggleFullScreen();
///           }
///       }
///   }
///  </code>
/// </example>
[DefaultProperty(nameof(ToggleFullScreenKey))]
[ToolboxItemFilter("System.Windows.Forms")]
[SRDescription(nameof(SR.DescriptionKioskModeManager))]
public class KioskModeManager : Component, ISupportInitialize
{
    private const uint PowerRequestContextVersion = 0;
    private const POWER_REQUEST_CONTEXT_FLAGS PowerRequestContextSimpleString = (POWER_REQUEST_CONTEXT_FLAGS)0x00000001;
    private const uint NotifyForThisSession = 0;
    private const nint PbtApmResumeAutomatic = 0x0012;
    private const nint WtsConsoleConnect = 0x0001;
    private const nint WtsRemoteConnect = 0x0003;
    private const nint WtsSessionLogon = 0x0005;
    private const nint WtsSessionUnlock = 0x0008;
    private const string PowerRequestReason = "WinForms KioskModeManager: Preventing screen saver and sleep";

    private static readonly object s_containerControlChangedEvent = new();
    private static readonly object s_fullScreenChangedEvent = new();
    private static readonly object s_wakeupEvent = new();

    private ContainerControl? _containerControl;
    private bool _containerControlExplicitlySet;
    private Form? _targetForm;
    private bool _isFullScreen;
    private bool _pendingFullScreen;
    private bool _initializing;
    private bool _isCursorHidden;
    private readonly List<Control> _parentChain = [];
    private KioskModeMessageFilter? _messageFilter;
    private KioskModeFormObserver? _formObserver;
    private Timer? _mousePointerAutoHideTimer;

    private FormBorderStyle _savedBorderStyle;
    private FormWindowState _savedWindowState;
    private Rectangle _savedBounds;
    private bool _savedTopMost;

    private bool _hideTaskbar;
    private bool _topMostInFullScreen;
    private Keys _toggleFullScreenKey = Keys.F11;
    private bool _escapeExitsFullScreen = true;
    private bool _suppressPowerSaving;
    private int _mousePointerAutoHideDelay;
    private ICommand? _wakeUpCommand;

    private HANDLE _powerRequestHandle;

    /// <summary>
    ///  Initializes a new instance of the <see cref="KioskModeManager"/> class.
    /// </summary>
    public KioskModeManager()
    {
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="KioskModeManager"/> class
    ///  and adds it to the specified container.
    /// </summary>
    /// <param name="container">The container that owns the component.</param>
    /// <exception cref="ArgumentNullException">
    ///  <paramref name="container"/> is <see langword="null"/>.
    /// </exception>
    public KioskModeManager(IContainer container)
    {
        ArgumentNullException.ThrowIfNull(container);
        container.Add(this);
    }

    /// <summary>
    ///  Gets or sets the <see cref="ISite"/> associated with the component.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   When the component is sited at design time and <see cref="ContainerControl"/>
    ///   has not been set explicitly, the component resolves the root component of the
    ///   designer host (typically the owning <see cref="Form"/> or <see cref="UserControl"/>)
    ///   and assigns it as the <see cref="ContainerControl"/>. An explicitly assigned
    ///   <see cref="ContainerControl"/> is never overwritten.
    ///  </para>
    /// </remarks>
    public override ISite? Site
    {
        get => base.Site;
        set
        {
            base.Site = value;

            if (value is not null
                && !_containerControlExplicitlySet
                && value.GetService(typeof(IDesignerHost)) is IDesignerHost host
                && host.RootComponent is ContainerControl root)
            {
                SetContainerControl(root, isExplicitAssignment: false);
            }
        }
    }

    /// <summary>
    ///  Gets or sets the container whose containing form is controlled by this
    ///  component.
    /// </summary>
    /// <value>
    ///  A <see cref="ContainerControl"/> that is either a <see cref="Form"/> or
    ///  a child container that can resolve a parent form; otherwise,
    ///  <see langword="null"/>.
    /// </value>
    /// <remarks>
    ///  <para>
    ///   This property intentionally uses <see cref="ContainerControl"/> rather
    ///   than <see cref="Form"/>. A <see cref="KioskModeManager"/> can be placed
    ///   on a <see cref="UserControl"/> at design time, and that user control
    ///   can later be hosted by a form. Restricting the property to
    ///   <see cref="Form"/> would make that design-time scenario inconsistent.
    ///  </para>
    /// </remarks>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.KioskModeManagerContainerControlDescr))]
    [DefaultValue(null)]
    public ContainerControl? ContainerControl
    {
        get => _containerControl;
        set => SetContainerControl(value, isExplicitAssignment: true);
    }

    /// <summary>
    ///  Occurs when the value of <see cref="ContainerControl"/> changes.
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.KioskModeManagerContainerControlChangedDescr))]
    public event EventHandler? ContainerControlChanged
    {
        add => Events.AddHandler(s_containerControlChangedEvent, value);
        remove => Events.RemoveHandler(s_containerControlChangedEvent, value);
    }

    /// <summary>
    ///  Raises the <see cref="ContainerControlChanged"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnContainerControlChanged(EventArgs e)
    {
        if (Events[s_containerControlChangedEvent] is EventHandler handler)
        {
            handler(this, e);
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether fullscreen mode covers the
    ///  Windows taskbar.
    /// </summary>
    /// <value>
    ///  <see langword="true"/> to size the form to the complete screen bounds;
    ///  <see langword="false"/> to use normal maximized form behavior.
    ///  The default is <see langword="false"/>.
    /// </value>
    /// <remarks>
    ///  <para>
    ///   When this property is <see langword="false"/>, the form is maximized
    ///   and Windows keeps the taskbar available according to the user's shell
    ///   settings. When this property is <see langword="true"/>, the form is
    ///   sized to the screen bounds so the kiosk surface covers the taskbar.
    ///  </para>
    /// </remarks>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.KioskModeManagerHideTaskbarDescr))]
    [DefaultValue(false)]
    public bool HideTaskbar
    {
        get => _hideTaskbar;
        set
        {
            if (_hideTaskbar == value)
            {
                return;
            }

            _hideTaskbar = value;

            if (_isFullScreen && _targetForm is not null)
            {
                ApplyFullScreen(_targetForm);
            }
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the form is topmost while
    ///  fullscreen.
    /// </summary>
    /// <value>
    ///  <see langword="true"/> to set <see cref="Form.TopMost"/> while
    ///  fullscreen; otherwise, <see langword="false"/>. The default is
    ///  <see langword="false"/>.
    /// </value>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.KioskModeManagerTopMostInFullScreenDescr))]
    [DefaultValue(false)]
    public bool TopMostInFullScreen
    {
        get => _topMostInFullScreen;
        set
        {
            if (_topMostInFullScreen == value)
            {
                return;
            }

            _topMostInFullScreen = value;

            if (_isFullScreen && _targetForm is not null)
            {
                _targetForm.TopMost = value;
            }
        }
    }

    /// <summary>
    ///  Gets or sets the key that toggles fullscreen mode.
    /// </summary>
    /// <value>
    ///  A <see cref="Keys"/> value that is handled without modifiers. The
    ///  default is <see cref="Keys.F11"/>.
    /// </value>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.KioskModeManagerToggleFullScreenKeyDescr))]
    [DefaultValue(Keys.F11)]
    public Keys ToggleFullScreenKey
    {
        get => _toggleFullScreenKey;
        set
        {
            if (_toggleFullScreenKey == value)
            {
                return;
            }

            _toggleFullScreenKey = value;
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether pressing Escape exits
    ///  fullscreen mode.
    /// </summary>
    /// <value>
    ///  <see langword="true"/> to exit fullscreen mode when Escape is pressed
    ///  without modifiers; otherwise, <see langword="false"/>. The default is
    ///  <see langword="true"/>.
    /// </value>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.KioskModeManagerEscapeExitsFullScreenDescr))]
    [DefaultValue(true)]
    public bool EscapeExitsFullScreen
    {
        get => _escapeExitsFullScreen;
        set
        {
            if (_escapeExitsFullScreen == value)
            {
                return;
            }

            _escapeExitsFullScreen = value;
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the component requests that
    ///  Windows keep the display and system awake.
    /// </summary>
    /// <value>
    ///  <see langword="true"/> to request that Windows suppress display sleep
    ///  and system sleep; otherwise, <see langword="false"/>. The default is
    ///  <see langword="false"/>.
    /// </value>
    /// <remarks>
    ///  <para>
    ///   This property uses the Windows power request APIs. It prevents ordinary
    ///   display and system idle sleep while enabled, but it does not override
    ///   every system policy and does not configure wake timers, Wake-on-LAN, or
    ///   voice activation.
    ///  </para>
    /// </remarks>
    /// <exception cref="Win32Exception">
    ///  Windows could not create or activate the power request.
    /// </exception>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.KioskModeManagerSuppressPowerSavingDescr))]
    [DefaultValue(false)]
    public bool SuppressPowerSaving
    {
        get => _suppressPowerSaving;
        set
        {
            if (_suppressPowerSaving == value)
            {
                return;
            }

            bool previousValue = _suppressPowerSaving;
            _suppressPowerSaving = value;

            try
            {
                UpdatePowerRequest();
            }
            catch
            {
                _suppressPowerSaving = previousValue;
                throw;
            }
        }
    }

    /// <summary>
    ///  Gets or sets the amount of time, in milliseconds, before the mouse
    ///  pointer is hidden while fullscreen.
    /// </summary>
    /// <value>
    ///  The inactivity delay in milliseconds. A value of 0 disables automatic
    ///  pointer hiding. The default is 0.
    /// </value>
    /// <remarks>
    ///  <para>
    ///   The timer is active only while the component is in fullscreen mode.
    ///   When the user moves the mouse after the component hid the pointer, the
    ///   pointer is shown immediately and the timer starts again.
    ///  </para>
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    ///  <paramref name="value"/> is less than 0.
    /// </exception>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.KioskModeManagerMousePointerAutoHideDelayDescr))]
    [DefaultValue(0)]
    public int MousePointerAutoHideDelay
    {
        get => _mousePointerAutoHideDelay;
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            if (_mousePointerAutoHideDelay == value)
            {
                return;
            }

            _mousePointerAutoHideDelay = value;

            if (value == 0)
            {
                StopMousePointerAutoHideTimer();
                ShowMousePointerIfHidden();
            }
            else if (_isFullScreen)
            {
                RestartMousePointerAutoHideTimer();
            }
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the resolved form is currently
    ///  fullscreen.
    /// </summary>
    /// <value>
    ///  <see langword="true"/> if the component has placed the form in
    ///  fullscreen mode; otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    ///  <para>
    ///   Setting this property to <see langword="true"/> enters fullscreen mode and
    ///   setting it to <see langword="false"/> restores the previously saved form
    ///   state. The property is bindable so it can participate in two-way data
    ///   binding.
    ///  </para>
    /// </remarks>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.KioskModeManagerFullScreenDescr))]
    [Bindable(true)]
    [DefaultValue(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool FullScreen
    {
        get => _pendingFullScreen || _isFullScreen;
        set
        {
            if (!value)
            {
                _pendingFullScreen = false;
                ExitFullScreen();
            }
            else if (_initializing || DesignMode || !EnterFullScreen())
            {
                _pendingFullScreen = true;
            }
            else
            {
                _pendingFullScreen = false;
            }
        }
    }

    /// <summary>
    ///  Occurs when the fullscreen state changes.
    /// </summary>
    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.KioskModeManagerFullScreenChangedDescr))]
    public event EventHandler? FullScreenChanged
    {
        add => Events.AddHandler(s_fullScreenChangedEvent, value);
        remove => Events.RemoveHandler(s_fullScreenChangedEvent, value);
    }

    /// <summary>
    ///  Raises the <see cref="FullScreenChanged"/> event.
    /// </summary>
    /// <param name="e">An <see cref="EventArgs"/> that contains the event data.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnFullScreenChanged(EventArgs e)
    {
        if (Events[s_fullScreenChangedEvent] is EventHandler handler)
        {
            handler(this, e);
        }
    }

    /// <summary>
    ///  Occurs when keyboard, mouse, power resume, or session activity wakes
    ///  the kiosk experience.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The event reports activity that the component can observe on the UI
    ///   thread or through Windows power/session notifications. It does not mean
    ///   that the component caused the computer to wake from sleep, and it does
    ///   not configure voice wake, network wake, or wake timers.
    ///  </para>
    /// </remarks>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.KioskModeManagerWakeupDescr))]
    public event KioskModeWakeupEventHandler? Wakeup
    {
        add => Events.AddHandler(s_wakeupEvent, value);
        remove => Events.RemoveHandler(s_wakeupEvent, value);
    }

    /// <summary>
    ///  Raises the <see cref="Wakeup"/> event.
    /// </summary>
    /// <param name="e">A <see cref="KioskModeWakeupEventArgs"/> that contains the event data.</param>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnWakeup(KioskModeWakeupEventArgs e)
    {
        if (Events[s_wakeupEvent] is KioskModeWakeupEventHandler handler)
        {
            handler(this, e);
        }

        if (_wakeUpCommand is not null)
        {
            // The wakeup source is passed as its string name so the bound command stays
            // independent of the WinForms-specific enum type.
            string parameter = e.Source.ToString();
            if (_wakeUpCommand.CanExecute(parameter))
            {
                _wakeUpCommand.Execute(parameter);
            }
        }
    }

    /// <summary>
    ///  Gets or sets the command that is executed whenever the <see cref="Wakeup"/>
    ///  event is raised.
    /// </summary>
    /// <value>
    ///  An <see cref="ICommand"/> to execute on every wakeup, or <see langword="null"/>
    ///  to execute no command. The default is <see langword="null"/>.
    /// </value>
    /// <remarks>
    ///  <para>
    ///   The command is executed with the wakeup source name (the result of
    ///   <see cref="KioskModeWakeupSource"/>.<see cref="object.ToString"/>) as the command
    ///   parameter, keeping the command independent of the WinForms-specific enum type.
    ///  </para>
    /// </remarks>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.KioskModeManagerWakeUpCommandDescr))]
    [Bindable(true)]
    [DefaultValue(null)]
    public ICommand? WakeUpCommand
    {
        get => _wakeUpCommand;
        set => _wakeUpCommand = value;
    }

    /// <summary>
    ///  Places the resolved form in fullscreen mode.
    /// </summary>
    /// <remarks>
    ///  <para>
///   The component saves the form's border style, window state, bounds, and
///   topmost state before applying fullscreen mode. Call
///   <see cref="ToggleFullScreen"/> again to restore the saved state.
    ///  </para>
    /// </remarks>
    private bool EnterFullScreen()
    {
        if (_isFullScreen || _initializing || DesignMode)
        {
            return _isFullScreen;
        }

        Form? targetForm = ResolveTargetForm();
        if (targetForm is null)
        {
            return false;
        }

        _savedBorderStyle = targetForm.FormBorderStyle;
        _savedWindowState = targetForm.WindowState;
        _savedBounds = targetForm.WindowState == FormWindowState.Normal
            ? targetForm.Bounds
            : targetForm.RestoreBounds;
        _savedTopMost = targetForm.TopMost;

        ApplyFullScreen(targetForm);
        SetFullScreenState(true);
        _pendingFullScreen = false;
        RestartMousePointerAutoHideTimer();
        return true;
    }

    /// <summary>
    ///  Restores the form state that was saved when fullscreen mode was
    ///  entered.
    /// </summary>
    private void ExitFullScreen()
    {
        if (!_isFullScreen || _targetForm is null)
        {
            return;
        }

        StopMousePointerAutoHideTimer();
        ShowMousePointerIfHidden();

        _targetForm.FormBorderStyle = _savedBorderStyle;
        _targetForm.TopMost = _savedTopMost;
        _targetForm.WindowState = FormWindowState.Normal;
        _targetForm.Bounds = _savedBounds;
        _targetForm.WindowState = _savedWindowState;

        SetFullScreenState(false);
    }

    /// <summary>
    ///  Updates the fullscreen state and raises <see cref="FullScreenChanged"/>
    ///  when the value changes.
    /// </summary>
    /// <param name="value">The new fullscreen state.</param>
    private void SetFullScreenState(bool value)
    {
        if (_isFullScreen == value)
        {
            return;
        }

        _isFullScreen = value;
        OnFullScreenChanged(EventArgs.Empty);
    }

    /// <summary>
    ///  Toggles the resolved form between fullscreen and restored mode.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Use <see cref="FullScreen"/> to determine the current state before
    ///   calling this method when the application needs an explicit target
    ///   state.
    ///  </para>
    /// </remarks>
    public void ToggleFullScreen()
        => FullScreen = !FullScreen;

    void ISupportInitialize.BeginInit()
    {
        _initializing = true;
    }

    void ISupportInitialize.EndInit()
    {
        _initializing = false;

        AttachToForm();
        UpdatePowerRequest();

        if (_pendingFullScreen)
        {
            FullScreen = true;
        }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _pendingFullScreen = false;
            ExitFullScreen();
            DetachFromForm();
            _mousePointerAutoHideTimer?.Dispose();
            _mousePointerAutoHideTimer = null;
            ReleasePowerRequest();
        }

        base.Dispose(disposing);
    }

    private void SetContainerControl(ContainerControl? value, bool isExplicitAssignment)
    {
        if (_containerControl == value)
        {
            if (isExplicitAssignment)
            {
                _containerControlExplicitlySet = true;
            }

            return;
        }

        if (_isFullScreen)
        {
            _pendingFullScreen = false;
            ExitFullScreen();
        }

        if (value is null)
        {
            _pendingFullScreen = false;
        }

        DetachFromForm();
        _containerControl = value;
        if (isExplicitAssignment)
        {
            _containerControlExplicitlySet = true;
        }

        AttachToForm();
        OnContainerControlChanged(EventArgs.Empty);
    }

    private void AttachToForm()
    {
        if (_initializing || DesignMode || _containerControl is null)
        {
            return;
        }

        UpdateParentChangedSubscriptions();
        EnsureMessageMonitoring();
        ResolveTargetForm();
        if (_pendingFullScreen && _targetForm is not null)
        {
            EnterFullScreen();
        }
    }

    private void DetachFromForm()
    {
        ClearParentChangedSubscriptions();

        if (_messageFilter is not null)
        {
            Application.RemoveMessageFilter(_messageFilter);
            _messageFilter = null;
        }

        _formObserver?.Detach();
        StopMousePointerAutoHideTimer();
        ShowMousePointerIfHidden();
        _targetForm = null;
    }

    private void OnContainerControlParentChanged(object? sender, EventArgs e)
    {
        UpdateParentChangedSubscriptions();

        Form? newTarget = _containerControl as Form ?? _containerControl?.FindForm();
        if (ReferenceEquals(_targetForm, newTarget))
        {
            return;
        }

        bool restoreFullScreen = _pendingFullScreen || _isFullScreen;

        if (_isFullScreen)
        {
            ExitFullScreen();
        }

        ResolveTargetForm();

        if (restoreFullScreen)
        {
            FullScreen = true;
        }
    }

    private void UpdateParentChangedSubscriptions()
    {
        ClearParentChangedSubscriptions();

        for (Control? current = _containerControl; current is not null; current = current.Parent)
        {
            current.ParentChanged += OnContainerControlParentChanged;
            _parentChain.Add(current);
        }
    }

    private void ClearParentChangedSubscriptions()
    {
        foreach (Control control in _parentChain)
        {
            control.ParentChanged -= OnContainerControlParentChanged;
        }

        _parentChain.Clear();
    }

    private Form? ResolveTargetForm()
    {
        // ContainerControl intentionally supports both Form and UserControl
        // design-time placement. FindForm handles the UserControl case.
        Form? targetForm = _containerControl as Form ?? _containerControl?.FindForm();
        if (!ReferenceEquals(_targetForm, targetForm))
        {
            _targetForm = targetForm;
            UpdateFormObserver();
        }

        return _targetForm;
    }

    private void EnsureMessageMonitoring()
    {
        // IMessageFilter observes input for child controls without changing
        // Form.KeyPreview or installing global keyboard/mouse hooks.
        if (_messageFilter is null)
        {
            _messageFilter = new KioskModeMessageFilter(this);
            Application.AddMessageFilter(_messageFilter);
        }
    }

    private void ApplyFullScreen(Form form)
    {
        Rectangle screenReferenceBounds = form.WindowState == FormWindowState.Normal
            ? form.Bounds
            : form.RestoreBounds;
        Screen screen = Screen.FromRectangle(screenReferenceBounds);
        Rectangle fullScreenBounds = _hideTaskbar
            ? screen.Bounds
            : screen.WorkingArea;

        // Apply fullscreen from a normal state so bounds and border changes do
        // not operate on Windows' maximized window rectangle.
        form.WindowState = FormWindowState.Normal;
        form.FormBorderStyle = FormBorderStyle.None;
        form.TopMost = _topMostInFullScreen;
        form.Bounds = fullScreenBounds;
    }

    private void UpdateFormObserver()
    {
        if (DesignMode)
        {
            return;
        }

        _formObserver ??= new KioskModeFormObserver(this);
        _formObserver.Attach(_targetForm);
    }

    private void UpdatePowerRequest()
    {
        if (_initializing || DesignMode)
        {
            return;
        }

        if (_suppressPowerSaving)
        {
            CreatePowerRequest();
        }
        else
        {
            ReleasePowerRequest();
        }
    }

    private void ProcessPowerBroadcast(WPARAM powerEvent)
    {
        if ((nint)powerEvent.Value == PbtApmResumeAutomatic)
        {
            RaiseWakeup(KioskModeWakeupSource.PowerResume);
        }
    }

    private void ProcessSessionChange(WPARAM sessionEvent)
    {
        nint value = (nint)sessionEvent.Value;
        if (value is WtsConsoleConnect or WtsRemoteConnect or WtsSessionLogon or WtsSessionUnlock)
        {
            RaiseWakeup(KioskModeWakeupSource.Session);
        }
    }

    private void RestartMousePointerAutoHideTimer()
    {
        // Use a WinForms timer so cursor changes happen on the UI thread that
        // receives the input messages restarting this timer.
        if (!_isFullScreen || _mousePointerAutoHideDelay == 0)
        {
            return;
        }

        _mousePointerAutoHideTimer ??= new Timer();
        _mousePointerAutoHideTimer.Stop();
        _mousePointerAutoHideTimer.Interval = _mousePointerAutoHideDelay;
        _mousePointerAutoHideTimer.Tick -= OnMousePointerAutoHideTimerTick;
        _mousePointerAutoHideTimer.Tick += OnMousePointerAutoHideTimerTick;
        _mousePointerAutoHideTimer.Start();
    }

    private void StopMousePointerAutoHideTimer()
    {
        _mousePointerAutoHideTimer?.Stop();
    }

    private void OnMousePointerAutoHideTimerTick(object? sender, EventArgs e)
    {
        StopMousePointerAutoHideTimer();

        if (!_isCursorHidden)
        {
            Cursor.Hide();
            _isCursorHidden = true;
        }
    }

    private void ProcessKeyboardActivity(Keys keyData, Keys modifiers, bool isRepeat)
    {
        // Any keyboard input wakes the kiosk experience, even when it does not
        // match one of the configured fullscreen control keys.
        RaiseWakeup(KioskModeWakeupSource.Keyboard);

        if (isRepeat)
        {
            return;
        }

        Keys keyCode = keyData & Keys.KeyCode;

        if (keyCode == _toggleFullScreenKey && modifiers == Keys.None)
        {
            ToggleFullScreen();
        }
        else if (keyCode == Keys.Escape && modifiers == Keys.None
            && _escapeExitsFullScreen && _isFullScreen)
        {
            ExitFullScreen();
        }
    }

    private void ProcessMouseActivity()
    {
        ShowMousePointerIfHidden();
        RestartMousePointerAutoHideTimer();
        RaiseWakeup(KioskModeWakeupSource.Mouse);
    }

    private void ShowMousePointerIfHidden()
    {
        // Cursor.Hide and Cursor.Show are counter-based. Only balance hides
        // performed by this component.
        if (!_isCursorHidden)
        {
            return;
        }

        Cursor.Show();
        _isCursorHidden = false;
    }

    private void RaiseWakeup(KioskModeWakeupSource source)
        => OnWakeup(new KioskModeWakeupEventArgs(source));

    private unsafe void CreatePowerRequest()
    {
        // PowerClearRequest clears the active request, but only CloseHandle
        // releases the native HANDLE returned by PowerCreateRequest.
        if (!_powerRequestHandle.IsNull)
        {
            return;
        }

        REASON_CONTEXT context = new()
        {
            Version = PowerRequestContextVersion,
            Flags = PowerRequestContextSimpleString,
        };

        nint reasonString = Marshal.StringToHGlobalUni(PowerRequestReason);

        try
        {
            context.Reason.SimpleReasonString = (PWSTR)(char*)reasonString;
            _powerRequestHandle = PInvoke.PowerCreateRequest(in context);
        }
        finally
        {
            Marshal.FreeHGlobal(reasonString);
        }

        if (_powerRequestHandle.IsNull)
        {
            throw new Win32Exception();
        }

        try
        {
            if (!PInvoke.PowerSetRequest(_powerRequestHandle, POWER_REQUEST_TYPE.PowerRequestDisplayRequired)
                || !PInvoke.PowerSetRequest(_powerRequestHandle, POWER_REQUEST_TYPE.PowerRequestSystemRequired))
            {
                throw new Win32Exception();
            }
        }
        catch
        {
            ReleasePowerRequest();
            throw;
        }
    }

    private void ReleasePowerRequest()
    {
        if (_powerRequestHandle.IsNull)
        {
            return;
        }

        PInvoke.PowerClearRequest(_powerRequestHandle, POWER_REQUEST_TYPE.PowerRequestDisplayRequired);
        PInvoke.PowerClearRequest(_powerRequestHandle, POWER_REQUEST_TYPE.PowerRequestSystemRequired);
        PInvoke.CloseHandle(_powerRequestHandle);
        _powerRequestHandle = default;
    }

    private bool IsMessageFromMonitoredControl(HWND hwnd)
    {
        if (_targetForm is not null
            && !_targetForm.IsDisposed
            && (hwnd == _targetForm.HWND || PInvoke.IsChild(_targetForm, hwnd)))
        {
            return true;
        }

        return _containerControl is not null
            && !_containerControl.IsDisposed
            && _containerControl.IsHandleCreated
            && (hwnd == _containerControl.HWND || PInvoke.IsChild(_containerControl, hwnd));
    }

    private bool ProcessMessage(Message message)
    {
        if (_containerControl is null || DesignMode)
        {
            return false;
        }

        if (_targetForm is null || _targetForm.IsDisposed)
        {
            ResolveTargetForm();
            if (_pendingFullScreen && _targetForm is not null)
            {
                EnterFullScreen();
            }
        }

        if (!IsMessageFromMonitoredControl(message.HWND))
        {
            return false;
        }

        if (message.MsgInternal == PInvokeCore.WM_KEYDOWN
            || message.MsgInternal == PInvokeCore.WM_SYSKEYDOWN)
        {
            bool isRepeat = ((nuint)(nint)message.LParamInternal & (1u << 30)) != 0;
            ProcessKeyboardActivity(
                (Keys)(nint)message.WParamInternal,
                Control.ModifierKeys & Keys.Modifiers,
                isRepeat);
        }
        else if (message.MsgInternal >= PInvokeCore.WM_MOUSEFIRST
            && message.MsgInternal <= PInvokeCore.WM_MOUSELAST)
        {
            ProcessMouseActivity();
        }

        return false;
    }

    /// <summary>
    ///  Observes keyboard and mouse messages that belong to the target form.
    /// </summary>
    private sealed class KioskModeMessageFilter : IMessageFilter
    {
        private readonly KioskModeManager _owner;

        public KioskModeMessageFilter(KioskModeManager owner)
        {
            _owner = owner;
        }

        public bool PreFilterMessage(ref Message m)
            => _owner.ProcessMessage(m);
    }

    /// <summary>
    ///  Observes power and session messages that belong to the target form.
    /// </summary>
    private sealed class KioskModeFormObserver : NativeWindow
    {
        private readonly KioskModeManager _owner;
        private Form? _form;
        private bool _sessionNotificationsRegistered;

        public KioskModeFormObserver(KioskModeManager owner)
        {
            _owner = owner;
        }

        public void Attach(Form? form)
        {
            if (ReferenceEquals(_form, form))
            {
                return;
            }

            Detach();

            if (form is null || form.IsDisposed)
            {
                return;
            }

            _form = form;
            form.HandleCreated += OnFormHandleCreated;
            form.HandleDestroyed += OnFormHandleDestroyed;

            if (form.IsHandleCreated)
            {
                OnFormHandleCreated(form, EventArgs.Empty);
            }
        }

        public void Detach()
        {
            if (_form is not null)
            {
                _form.HandleCreated -= OnFormHandleCreated;
                _form.HandleDestroyed -= OnFormHandleDestroyed;
                _form = null;
            }

            UnregisterSessionNotifications();
            if (!HWND.IsNull)
            {
                ReleaseHandle();
            }
        }

        private void OnFormHandleCreated(object? sender, EventArgs e)
        {
            if (_form is null || _form.IsDisposed || !_form.IsHandleCreated)
            {
                return;
            }

            AssignHandle(_form.HWND);
            RegisterSessionNotifications();
        }

        private void OnFormHandleDestroyed(object? sender, EventArgs e)
        {
            UnregisterSessionNotifications();
            if (!HWND.IsNull)
            {
                ReleaseHandle();
            }
        }

        private void RegisterSessionNotifications()
        {
            if (_sessionNotificationsRegistered || HWND.IsNull)
            {
                return;
            }

            _sessionNotificationsRegistered = PInvoke.WTSRegisterSessionNotification(HWND, NotifyForThisSession);
        }

        private void UnregisterSessionNotifications()
        {
            if (!_sessionNotificationsRegistered)
            {
                return;
            }

            PInvoke.WTSUnRegisterSessionNotification(HWND);
            _sessionNotificationsRegistered = false;
        }

        protected override void WndProc(ref Message m)
        {
            if (m.MsgInternal == PInvokeCore.WM_POWERBROADCAST)
            {
                _owner.ProcessPowerBroadcast(m.WParamInternal);
            }
            else if (m.MsgInternal == PInvokeCore.WM_WTSSESSION_CHANGE)
            {
                _owner.ProcessSessionChange(m.WParamInternal);
            }

            base.WndProc(ref m);
        }
    }
}
#endif

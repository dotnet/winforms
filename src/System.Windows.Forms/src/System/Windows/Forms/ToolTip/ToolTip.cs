// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Provides a small pop-up window containing a line of text that describes the purpose of a
///  tool or control (usually represented as a graphical object) in a program.
/// </summary>
[ProvideProperty(nameof(ToolTip), typeof(Control))]
[DefaultEvent(nameof(Popup))]
[ToolboxItemFilter("System.Windows.Forms")]
[SRDescription(nameof(SR.DescriptionToolTip))]
public partial class ToolTip : Component, IExtenderProvider, IHandle<HWND>
{
    // The actual delay values are based on the user-set double click time.
    // These values are initialized using the default double-click time value.
    internal const int DefaultDelay = 500;

    // These values are copied from the ComCtl32's tooltip.
    private const int ReshowRatio = 5;
    private const int AutoPopRatio = 10;

    private const int InfiniteDelay = short.MaxValue;

    private const int BalloonOffsetX = 10;

    private const int LocationIndexTop = 0;
    private const int LocationIndexRight = 1;
    private const int LocationIndexBottom = 2;
    private const int LocationIndexLeft = 3;
    private const int LocationTotal = 4;
    private readonly Dictionary<Control, TipInfo> _tools = [];
    private readonly int[] _delayTimes = new int[4];
    private bool _auto = true;
    private bool _showAlways;
    private ToolTipNativeWindow _window;
    private Control? _topLevelControl;
    private bool _active = true;
    private Color _backColor = SystemColors.Info;
    private Color _foreColor = SystemColors.InfoText;
    private bool _isBalloon;
    private bool _isDisposing;
    private string _toolTipTitle = string.Empty;
    private ToolTipIcon _toolTipIcon = ToolTipIcon.None;
    private ToolTipTimer? _timer;
    private readonly Dictionary<HWND, Control> _owners = [];
    private bool _stripAmpersands;
    private bool _useAnimation = true;
    private bool _useFading = true;
    private int _originalPopupDelay;

    /// <summary>
    ///  Setting TTM_TRACKPOSITION will cause redundant POP and Draw Messages.
    ///  Hence we guard against this by having this private flag.
    /// </summary>
    private bool _trackPosition;

    private PopupEventHandler? _onPopup;
    private DrawToolTipEventHandler? _onDraw;

    /// <summary>
    ///  Adding a tool twice breaks the ToolTip, so we need to track which
    ///  tools are created to prevent this.
    /// </summary>
    private readonly HashSet<Control> _created = [];

    private bool _cancelled;

    /// <summary>
    ///  Initializes a new instance of the <see cref="ToolTip"/> class, given the container.
    /// </summary>
    public ToolTip(IContainer cont)
        : this()
    {
        ArgumentNullException.ThrowIfNull(cont);

        cont.Add(this);

        IsPersistent = OsVersion.IsWindows11_OrGreater();
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="ToolTip"/> class in its default state.
    /// </summary>
    public ToolTip()
    {
        _window = new ToolTipNativeWindow(this);
        _auto = true;
        _delayTimes[(int)PInvoke.TTDT_AUTOMATIC] = DefaultDelay;

        IsPersistent = OsVersion.IsWindows11_OrGreater();

        AdjustBaseFromAuto();
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the <see cref="ToolTip"/> control is currently active.
    /// </summary>
    [SRDescription(nameof(SR.ToolTipActiveDescr))]
    [DefaultValue(true)]
    public bool Active
    {
        get => _active;
        set
        {
            if (_active != value)
            {
                _active = value;

                // Don't activate the tooltip if we're in the designer.
                if (!DesignMode && GetHandleCreated())
                {
                    PInvoke.SendMessage(this, PInvoke.TTM_ACTIVATE, (WPARAM)(BOOL)value);
                }
            }
        }
    }

    internal void HideToolTip(IKeyboardToolTip currentTool)
    {
        IWin32Window? ownerWindow = currentTool.GetOwnerWindow();
        if (ownerWindow is not null)
        {
            Hide(ownerWindow);
        }
    }

    /// <summary>
    ///  Gets or sets the time (in milliseconds) that passes before the <see cref="ToolTip"/> appears.
    /// </summary>
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.ToolTipAutomaticDelayDescr))]
    [DefaultValue(DefaultDelay)]
    public int AutomaticDelay
    {
        get => _delayTimes[(int)PInvoke.TTDT_AUTOMATIC];
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            SetDelayTime((int)PInvoke.TTDT_AUTOMATIC, value);
        }
    }

    internal string? GetCaptionForTool(Control tool)
    {
        return _tools.TryGetValue(tool, out TipInfo? tipInfo) ? tipInfo.Caption : null;
    }

    /// <summary>
    ///  Gets or sets the initial delay for the <see cref="ToolTip"/> control.
    /// </summary>
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.ToolTipAutoPopDelayDescr))]
    public int AutoPopDelay
    {
        get => _delayTimes[(int)PInvoke.TTDT_AUTOPOP];
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            SetDelayTime(PInvoke.TTDT_AUTOPOP, value);
        }
    }

    /// <summary>
    ///  Gets or sets the BackColor for the <see cref="ToolTip"/> control.
    /// </summary>
    [SRDescription(nameof(SR.ToolTipBackColorDescr))]
    [DefaultValue(typeof(Color), "Info")]
    public Color BackColor
    {
        get => _backColor;
        set
        {
            _backColor = value;
            if (GetHandleCreated())
            {
                PInvoke.SendMessage(this, PInvoke.TTM_SETTIPBKCOLOR, (WPARAM)_backColor);
            }
        }
    }

    /// <summary>
    ///  The CreateParams to create the window.
    /// </summary>
    protected virtual CreateParams CreateParams
    {
        get
        {
            CreateParams cp = new();
            if (TopLevelControl is not null && !TopLevelControl.IsDisposed)
            {
                cp.Parent = TopLevelControl.Handle;
            }

            cp.ClassName = PInvoke.TOOLTIPS_CLASS;
            if (_showAlways)
            {
                cp.Style = (int)PInvoke.TTS_ALWAYSTIP;
            }

            if (_isBalloon)
            {
                cp.Style |= (int)PInvoke.TTS_BALLOON;
            }

            if (!_stripAmpersands)
            {
                cp.Style |= (int)PInvoke.TTS_NOPREFIX;
            }

            if (!_useAnimation)
            {
                cp.Style |= (int)PInvoke.TTS_NOANIMATE;
            }

            if (!_useFading)
            {
                cp.Style |= (int)PInvoke.TTS_NOFADE;
            }

            cp.ExStyle = 0;
            cp.Caption = null;

            return cp;
        }
    }

    /// <summary>
    ///  Gets or sets the ForeColor for the <see cref="ToolTip"/> control.
    /// </summary>
    [SRDescription(nameof(SR.ToolTipForeColorDescr))]
    [DefaultValue(typeof(Color), "InfoText")]
    public Color ForeColor
    {
        get => _foreColor;
        set
        {
            if (value.IsEmpty)
            {
                throw new ArgumentException(string.Format(SR.ToolTipEmptyColor, nameof(ForeColor)), nameof(value));
            }

            _foreColor = value;
            if (GetHandleCreated())
            {
                PInvoke.SendMessage(this, PInvoke.TTM_SETTIPTEXTCOLOR, (WPARAM)_foreColor);
            }
        }
    }

    HWND IHandle<HWND>.Handle => HWND;

    internal HWND HWND => (HWND)Handle;

    internal IntPtr Handle
    {
        get
        {
            if (!GetHandleCreated())
            {
                CreateHandle();
            }

            return _window.Handle;
        }
    }

    /// <summary>
    ///  Shows if the keyboard tooltip is currently active.
    /// </summary>
    internal bool IsActivatedByKeyboard { get; set; }

    /// <summary>
    ///  Gets or sets the IsBalloon for the <see cref="ToolTip"/> control.
    /// </summary>
    [SRDescription(nameof(SR.ToolTipIsBalloonDescr))]
    [DefaultValue(false)]
    public bool IsBalloon
    {
        get => _isBalloon;
        set
        {
            if (_isBalloon != value)
            {
                _isBalloon = value;
                if (GetHandleCreated())
                {
                    RecreateHandle();
                }
            }
        }
    }

    private bool IsWindowActive(IWin32Window window)
    {
        // We want to enter in the if block only if ShowParams does not return SW_SHOWNOACTIVATE.
        // for ToolStripDropDown ShowParams returns SW_SHOWNOACTIVATE, in which case we don't
        // want to check IsWindowActive and hence return true.
        if (window is Control control &&
            (control.ShowParams & (SHOW_WINDOW_CMD)0xF) != SHOW_WINDOW_CMD.SW_SHOWNOACTIVATE)
        {
            HWND hwnd = PInvoke.GetActiveWindow();
            HWND rootHwnd = PInvoke.GetAncestor(control, GET_ANCESTOR_FLAGS.GA_ROOT);
            if (hwnd != rootHwnd)
            {
                if (_tools.TryGetValue(control, out TipInfo? tt) && (tt.TipType & TipInfo.Type.SemiAbsolute) != 0)
                {
                    _tools.Remove(control);
                    DestroyRegion(control);
                }

                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///  Gets or sets the initial delay for the <see cref="ToolTip"/> control.
    /// </summary>
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.ToolTipInitialDelayDescr))]
    public int InitialDelay
    {
        get => _delayTimes[(int)PInvoke.TTDT_INITIAL];
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            SetDelayTime(PInvoke.TTDT_INITIAL, value);
        }
    }

    /// <summary>
    ///  Indicates whether the ToolTip will be drawn by the system or the user.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.ToolTipOwnerDrawDescr))]
    public bool OwnerDraw { get; set; }

    /// <summary>
    ///  Gets or sets the length of time (in milliseconds) that it takes subsequent ToolTip
    ///  instances to appear as the mouse pointer moves from one ToolTip region to another.
    /// </summary>
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.ToolTipReshowDelayDescr))]
    public int ReshowDelay
    {
        get => _delayTimes[(int)PInvoke.TTDT_RESHOW];
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            SetDelayTime(PInvoke.TTDT_RESHOW, value);
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the <see cref="ToolTip"/> appears even when its
    ///  parent control is not active.
    /// </summary>
    [DefaultValue(false)]
    [SRDescription(nameof(SR.ToolTipShowAlwaysDescr))]
    public bool ShowAlways
    {
        get => _showAlways;
        set
        {
            if (_showAlways != value)
            {
                _showAlways = value;
                if (GetHandleCreated())
                {
                    RecreateHandle();
                }
            }
        }
    }

    /// <summary>
    ///  When set to true, any ampersands in the Text property are not displayed.
    /// </summary>
    [SRDescription(nameof(SR.ToolTipStripAmpersandsDescr))]
    [Browsable(true)]
    [DefaultValue(false)]
    public bool StripAmpersands
    {
        get => _stripAmpersands;
        set
        {
            if (_stripAmpersands != value)
            {
                _stripAmpersands = value;
                if (GetHandleCreated())
                {
                    RecreateHandle();
                }
            }
        }
    }

    [SRCategory(nameof(SR.CatData))]
    [Localizable(false)]
    [Bindable(true)]
    [SRDescription(nameof(SR.ControlTagDescr))]
    [DefaultValue(null)]
    [TypeConverter(typeof(StringConverter))]
    public object? Tag { get; set; }

    /// <summary>
    ///  Gets or sets an Icon on the ToolTip.
    /// </summary>
    [DefaultValue(ToolTipIcon.None)]
    [SRDescription(nameof(SR.ToolTipToolTipIconDescr))]
    public ToolTipIcon ToolTipIcon
    {
        get => _toolTipIcon;
        set
        {
            if (_toolTipIcon != value)
            {
                SourceGenerated.EnumValidator.Validate(value);

                _toolTipIcon = value;
                if (_toolTipIcon > 0 && GetHandleCreated())
                {
                    // If the title is null/empty, the icon won't display.
                    string title = !string.IsNullOrEmpty(_toolTipTitle) ? _toolTipTitle : " ";
                    PInvoke.SendMessage(this, PInvoke.TTM_SETTITLEW, (uint)_toolTipIcon, title);

                    // Tooltip need to be updated to reflect the changes in the icon because
                    // this operation directly affects the size of the tooltip.
                    PInvoke.SendMessage(this, PInvoke.TTM_UPDATE);
                }
            }
        }
    }

    /// <summary>
    ///  Gets or sets the title of the ToolTip.
    /// </summary>
    [DefaultValue("")]
    [SRDescription(nameof(SR.ToolTipTitleDescr))]
    [AllowNull]
    public string ToolTipTitle
    {
        get => _toolTipTitle;
        set
        {
            value ??= string.Empty;

            if (_toolTipTitle != value)
            {
                _toolTipTitle = value;
                if (GetHandleCreated())
                {
                    PInvoke.SendMessage(this, PInvoke.TTM_SETTITLEW, (uint)_toolTipIcon, _toolTipTitle);

                    // Tooltip need to be updated to reflect the changes in the title text because
                    // this operation directly affects the size of the tooltip.
                    PInvoke.SendMessage(this, PInvoke.TTM_UPDATE);
                }
            }
        }
    }

    private Control? TopLevelControl
    {
        get
        {
            if (_topLevelControl is not null)
            {
                return _topLevelControl;
            }

            Control? currentTopLevel = null;
            Control[] controls = [.. _tools.Keys];
            for (int i = 0; i < controls.Length; i++)
            {
                var control = controls[i];
                currentTopLevel = control.TopLevelControlInternal;
                if (currentTopLevel is not null)
                {
                    break;
                }

                if (control.IsActiveX)
                {
                    currentTopLevel = control;
                    break;
                }

                // In the designer, baseVar can be null since the Parent is not a TopLevel control
                if (currentTopLevel is null)
                {
                    if (control is not null && control.ParentInternal is not null)
                    {
                        while (control.ParentInternal is not null)
                        {
                            control = control.ParentInternal;
                        }

                        currentTopLevel = control;
                        if (currentTopLevel is not null)
                        {
                            break;
                        }
                    }
                }
            }

            _topLevelControl = currentTopLevel;
            if (currentTopLevel is not null)
            {
                currentTopLevel.HandleCreated += TopLevelCreated;
                currentTopLevel.HandleDestroyed += TopLevelDestroyed;
                if (currentTopLevel.IsHandleCreated)
                {
                    TopLevelCreated(currentTopLevel, EventArgs.Empty);
                }

                currentTopLevel.ParentChanged += OnTopLevelPropertyChanged;
            }

            return currentTopLevel;
        }
    }

    /// <summary>
    ///  When set to true, animations are used when tooltip is shown or hidden.
    /// </summary>
    [SRDescription(nameof(SR.ToolTipUseAnimationDescr))]
    [Browsable(true)]
    [DefaultValue(true)]
    public bool UseAnimation
    {
        get => _useAnimation;
        set
        {
            if (_useAnimation != value)
            {
                _useAnimation = value;
                if (GetHandleCreated())
                {
                    RecreateHandle();
                }
            }
        }
    }

    /// <summary>
    ///  When set to true, a fade effect is used when tooltips are shown or hidden.
    /// </summary>
    [SRDescription(nameof(SR.ToolTipUseFadingDescr))]
    [Browsable(true)]
    [DefaultValue(true)]
    public bool UseFading
    {
        get => _useFading;
        set
        {
            if (_useFading != value)
            {
                _useFading = value;
                if (GetHandleCreated())
                {
                    RecreateHandle();
                }
            }
        }
    }

    /// <summary>
    ///  Fires in OwnerDraw mode when the tooltip needs to be drawn.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ToolTipDrawEventDescr))]
    public event DrawToolTipEventHandler? Draw
    {
        add => _onDraw += value;
        remove => _onDraw -= value;
    }

    /// <summary>
    ///  Fires when the tooltip is just about to be shown.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ToolTipPopupEventDescr))]
    public event PopupEventHandler? Popup
    {
        add => _onPopup += value;
        remove => _onPopup -= value;
    }

    /// <summary>
    ///  Adjusts the other delay values based on the Automatic value.
    /// </summary>
    private void AdjustBaseFromAuto()
    {
        int delay = _delayTimes[(int)PInvoke.TTDT_AUTOMATIC];
        _delayTimes[(int)PInvoke.TTDT_RESHOW] = delay / ReshowRatio;
        _delayTimes[(int)PInvoke.TTDT_AUTOPOP] = delay * AutoPopRatio;
        _delayTimes[(int)PInvoke.TTDT_INITIAL] = delay;
    }

    /// <summary>
    ///  Screen reader announces ToolTip text for an element.
    /// </summary>
    private void AnnounceText(Control? tool, string text)
    {
        if (tool is null || !tool.IsAccessibilityObjectCreated)
        {
            return;
        }

        tool.AccessibilityObject.InternalRaiseAutomationNotification(
            Automation.AutomationNotificationKind.ActionCompleted,
            Automation.AutomationNotificationProcessing.All,
            $"{ToolTipTitle} {text}");
    }

    private void HandleCreated(object? sender, EventArgs eventargs)
    {
        // Reset the top level control when the owner's handle is recreated.
        ClearTopLevelControlEvents();
        _topLevelControl = null;

        Control control = (Control)sender!;
        CreateRegion(control);
        SetToolTipToControl(control);

        KeyboardToolTipStateMachine.Instance.Hook(control, this);
    }

    private void HandleDestroyed(object? sender, EventArgs eventargs)
    {
        Control control = (Control)sender!;
        DestroyRegion(control);

        KeyboardToolTipStateMachine.Instance.Unhook(control, this);
    }

    /// <summary>
    ///  Raises the Draw event.
    /// </summary>
    private void OnDraw(DrawToolTipEventArgs e) => _onDraw?.Invoke(this, e);

    /// <summary>
    ///  Raises the Popup event.
    /// </summary>
    private void OnPopup(PopupEventArgs e) => _onPopup?.Invoke(this, e);

    private void TopLevelCreated(object? sender, EventArgs eventargs)
    {
        CreateHandle();
        CreateAllRegions();
    }

    private void TopLevelDestroyed(object? sender, EventArgs eventargs)
    {
        DestroyAllRegions();
        DestroyHandle();
    }

    /// <summary>
    ///  Returns true if the tooltip can offer an extender property to the specified target component.
    /// </summary>
    public bool CanExtend(object target) => target is Control;

    private void ClearTopLevelControlEvents()
    {
        if (_topLevelControl is null)
        {
            return;
        }

        _topLevelControl.ParentChanged -= OnTopLevelPropertyChanged;
        _topLevelControl.HandleCreated -= TopLevelCreated;
        _topLevelControl.HandleDestroyed -= TopLevelDestroyed;
    }

    /// <summary>
    ///  Creates the handle for the control.
    /// </summary>
    private unsafe void CreateHandle()
    {
        if (GetHandleCreated())
        {
            return;
        }

        using (ThemingScope scope = new(Application.UseVisualStyles))
        {
            PInvoke.InitCommonControlsEx(new INITCOMMONCONTROLSEX
            {
                dwSize = (uint)sizeof(INITCOMMONCONTROLSEX),
                dwICC = INITCOMMONCONTROLSEX_ICC.ICC_TAB_CLASSES
            });

            // Avoid reentrant call to CreateHandle.
            CreateParams cp = CreateParams;
            if (GetHandleCreated())
            {
                return;
            }

            _window.CreateHandle(cp);

            if (SystemInformation.HighContrast)
            {
                PInvoke.SetWindowTheme(HWND, string.Empty, string.Empty);
            }
        }

        // If in OwnerDraw mode, we don't want the default border.
        if (OwnerDraw)
        {
            int style = unchecked((int)((long)PInvoke.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE)));
            style &= ~(int)WINDOW_STYLE.WS_BORDER;
            PInvoke.SetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE, style);
        }

        // Setting the max width has the added benefit of enabling multiline tool tips.
        PInvoke.SendMessage(this, PInvoke.TTM_SETMAXTIPWIDTH, 0, SystemInformation.MaxWindowTrackSize.Width);

        if (_auto)
        {
            // AutomaticDelay property should overwrite other delay timer values, _auto field indicates that
            // AutomaticDelay had been set more recently than other delays.
            if (_delayTimes[(int)PInvoke.TTDT_AUTOMATIC] != DefaultDelay)
            {
                SetDelayTime(PInvoke.TTDT_AUTOMATIC, _delayTimes[(int)PInvoke.TTDT_AUTOMATIC]);
            }
            else
            {
                _delayTimes[(int)PInvoke.TTDT_AUTOPOP] = GetDelayTime(PInvoke.TTDT_AUTOPOP);
                _delayTimes[(int)PInvoke.TTDT_INITIAL] = GetDelayTime(PInvoke.TTDT_INITIAL);
                _delayTimes[(int)PInvoke.TTDT_RESHOW] = GetDelayTime(PInvoke.TTDT_RESHOW);
            }
        }
        else
        {
            int delayTime = _delayTimes[(int)PInvoke.TTDT_AUTOPOP];
            if (delayTime is >= 1 and not (DefaultDelay * AutoPopRatio))
            {
                SetDelayTime(PInvoke.TTDT_AUTOPOP, delayTime);
            }

            delayTime = _delayTimes[(int)PInvoke.TTDT_INITIAL];
            if (delayTime >= 1)
            {
                SetDelayTime(PInvoke.TTDT_INITIAL, delayTime);
            }

            delayTime = _delayTimes[(int)PInvoke.TTDT_RESHOW];
            if (delayTime >= 1)
            {
                SetDelayTime(PInvoke.TTDT_RESHOW, delayTime);
            }
        }

        // Set active status.
        PInvoke.SendMessage(this, PInvoke.TTM_ACTIVATE, (WPARAM)(BOOL)_active);

        if (BackColor != SystemColors.Info)
        {
            PInvoke.SendMessage(this, PInvoke.TTM_SETTIPBKCOLOR, (WPARAM)BackColor);
        }

        if (ForeColor != SystemColors.InfoText)
        {
            PInvoke.SendMessage(this, PInvoke.TTM_SETTIPTEXTCOLOR, (WPARAM)ForeColor);
        }

        if (_toolTipIcon > 0 || !string.IsNullOrEmpty(_toolTipTitle))
        {
            // If the title is null/empty, the icon won't display.
            string title = !string.IsNullOrEmpty(_toolTipTitle) ? _toolTipTitle : " ";
            PInvoke.SendMessage(this, PInvoke.TTM_SETTITLEW, (WPARAM)(int)_toolTipIcon, title);
        }
    }

    private void CreateAllRegions()
    {
        Control[] controls = [.. _tools.Keys];
        foreach (Control control in controls)
        {
            CreateRegion(control);
        }
    }

    private void DestroyAllRegions()
    {
        Control[] controls = [.. _tools.Keys];
        foreach (Control control in controls)
        {
            // DataGridView manages its own tool tip.
            if (control is DataGridView)
            {
                return;
            }

            DestroyRegion(control);
        }
    }

    private void SetToolInfo(Control control, string? caption)
    {
        // Don't add a tool for the TabControl itself. It's not needed because:
        // 1. TabControl already relays all mouse events to its tooltip:
        // https://docs.microsoft.com/windows/win32/controls/tab-controls#default-tab-control-message-processing
        // 2. Hit-testing against TabControl detects only TabPages which are separate tools added by TabControl.
        // This prevents a bug when a TabPage tool is placed before the TabControl tool after reordering caused by a tool deletion.
        // Also prevents double handling of mouse messages which is caused by subclassing altogether with the message relaying from the TabControl internals.
        if (control is TabControl)
        {
            return;
        }

        IntPtr result = GetTOOLINFO(control, caption).SendMessage(this, PInvoke.TTM_ADDTOOLW);

        if ((control is TreeView tv && tv.ShowNodeToolTips)
            || (control is ListView lv && lv.ShowItemToolTips))
        {
            return;
        }

        if (result == IntPtr.Zero)
        {
            throw new InvalidOperationException(SR.ToolTipAddFailed);
        }
    }

    private void CreateRegion(Control control)
    {
        string? caption = GetToolTip(control);
        bool handlesCreated = control.IsHandleCreated
                              && TopLevelControl is not null
                              && TopLevelControl.IsHandleCreated;

        if (!_created.Contains(control)
            && !string.IsNullOrEmpty(caption)
            && handlesCreated && !DesignMode)
        {
            // Call the SendMessage through a function.
            SetToolInfo(control, caption);
            _created.Add(control);
        }

        if (control.IsHandleCreated && _topLevelControl is null)
        {
            // Remove first to purge any duplicates.
            control.MouseMove -= MouseMove;
            control.MouseMove += MouseMove;
        }
    }

    private void MouseMove(object? sender, MouseEventArgs me)
    {
        if (sender is not Control control)
        {
            return;
        }

        if (!_created.Contains(control) && control.IsHandleCreated && TopLevelControl is not null)
        {
            CreateRegion(control);
        }

        if (_created.Contains(control))
        {
            control.MouseMove -= MouseMove;
        }
    }

    /// <summary>
    ///  Destroys the handle for this control.
    ///  Required by Label to destroy the handle for the toolTip added for AutoEllipses.
    /// </summary>
    internal void DestroyHandle()
    {
        if (GetHandleCreated())
        {
            _window.DestroyHandle();
        }
    }

    private void DestroyRegion(Control control)
    {
        // When the toplevelControl is a form and is Modal, the Handle of the tooltip is released
        // before we come here. In such a case the tool won't get deleted from the tooltip.
        // So we don't check "Handle" in the handlesCreate but check it only for non-modal dialogs later.
        bool handlesCreated = control.IsHandleCreated
                            && _topLevelControl is not null
                            && _topLevelControl.IsHandleCreated
                            && !_isDisposing;
        if (_topLevelControl is not Form topForm || (topForm is not null && !topForm.Modal))
        {
            handlesCreated = handlesCreated && GetHandleCreated();
        }

        if (_created.Contains(control) && handlesCreated && !DesignMode)
        {
            new ToolInfoWrapper<Control>(control).SendMessage(this, PInvoke.TTM_DELTOOLW);
            _created.Remove(control);
            control.RemoveToolTip(this);
        }
    }

    /// <summary>
    ///  Disposes of the <see cref="ToolTip"/> component.
    /// </summary>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _isDisposing = true;
            try
            {
                ClearTopLevelControlEvents();
                StopTimer();

                // Always destroy the handle.
                DestroyHandle();
                RemoveAll();

                _window = null!;

                // Unhook the DeactivateEvent. Find the Form for associated Control and hook
                // up to the Deactivated event to Hide the Shown tooltip
                if (TopLevelControl is Form baseFrom)
                {
                    baseFrom.Deactivate -= BaseFormDeactivate;
                }
            }
            finally
            {
                _isDisposing = false;
            }
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///  Returns the delay time based on the NativeMethods.TTDT_* values.
    /// </summary>
    internal int GetDelayTime(uint type)
    {
        if (!GetHandleCreated())
        {
            return _delayTimes[(int)type];
        }

        return (int)PInvoke.SendMessage(this, PInvoke.TTM_GETDELAYTIME, (WPARAM)type);
    }

    internal bool GetHandleCreated() => _window is not null && _window.Handle != IntPtr.Zero;

    /// <summary>
    ///  Returns a detailed TOOLINFO_TOOLTIP structure that represents the specified region.
    /// </summary>
    private unsafe ToolInfoWrapper<Control> GetTOOLINFO(Control control, string? caption)
    {
        TOOLTIP_FLAGS flags = TOOLTIP_FLAGS.TTF_TRANSPARENT | TOOLTIP_FLAGS.TTF_SUBCLASS;

        // RightToLeft reading order.
        if (TopLevelControl?.RightToLeft == RightToLeft.Yes && !control.IsMirrored)
        {
            // Indicates that the ToolTip text will be displayed in the opposite direction
            // to the text in the parent window.
            flags |= TOOLTIP_FLAGS.TTF_RTLREADING;
        }

        return control.GetToolInfoWrapper(flags, caption, this);
    }

    private ToolInfoWrapper<HandleRef<HWND>> GetWinTOOLINFO(IWin32Window window)
    {
        TOOLTIP_FLAGS flags = TOOLTIP_FLAGS.TTF_TRANSPARENT | TOOLTIP_FLAGS.TTF_SUBCLASS;

        // RightToLeft reading order
        if (TopLevelControl?.RightToLeft == RightToLeft.Yes)
        {
            // Indicates that the ToolTip text will be displayed in the opposite direction
            // to the text in the parent window.
            if (!window.GetExtendedStyle().HasFlag(WINDOW_EX_STYLE.WS_EX_LAYOUTRTL))
            {
                flags |= TOOLTIP_FLAGS.TTF_RTLREADING;
            }
        }

        return new(Control.GetSafeHandle(window), flags);
    }

    /// <summary>
    ///  Retrieves the <see cref="ToolTip"/> text associated with the specified control.
    /// </summary>
    [DefaultValue("")]
    [Localizable(true)]
    [SRDescription(nameof(SR.ToolTipToolTipDescr))]
    [Editor($"System.ComponentModel.Design.MultilineStringEditor, {AssemblyRef.SystemDesign}", typeof(Drawing.Design.UITypeEditor))]
    public string? GetToolTip(Control? control)
    {
        if (control is null)
        {
            return string.Empty;
        }

        return _tools.TryGetValue(control, out TipInfo? tipInfo)
            ? tipInfo.Caption
            : string.Empty;
    }

    /// <summary>
    ///  Returns the HWND of the window that is at the specified point. This handles special
    ///  cases where one Control owns multiple HWNDs (i.e. ComboBox).
    /// </summary>
    private HWND GetWindowFromPoint(Point screenCoords, ref bool success)
    {
        Control? current = TopLevelControl;

        // Special case ActiveX Controls.
        if (current is not null && current.IsActiveX)
        {
            // Find the matching HWnd matching the ScreenCoord and find if the Control has a Tooltip.
            HWND hwndControl = PInvoke.WindowFromPoint(screenCoords);
            if (!hwndControl.IsNull)
            {
                Control? currentControl = Control.FromHandle(hwndControl);
                if (currentControl is not null &&
                    _tools.ContainsKey(currentControl))
                {
                    return hwndControl;
                }
            }

            return HWND.Null;
        }

        HWND baseHwnd = current?.HWND ?? HWND.Null;
        HWND hwnd = HWND.Null;
        bool finalMatch = false;
        while (!finalMatch)
        {
            Point pt = screenCoords;
            if (current is not null)
            {
                pt = current.PointToClient(screenCoords);
            }

            HWND found = PInvoke.ChildWindowFromPointEx(baseHwnd, pt, CWP_FLAGS.CWP_SKIPINVISIBLE);
            if (found == baseHwnd)
            {
                hwnd = found;
                finalMatch = true;
            }
            else if (found.IsNull)
            {
                finalMatch = true;
            }
            else
            {
                current = Control.FromHandle(found);
                if (current is null)
                {
                    current = Control.FromChildHandle(found);
                    if (current is not null)
                    {
                        hwnd = current.HWND;
                    }

                    finalMatch = true;
                }
                else
                {
                    baseHwnd = current.HWND;
                }
            }
        }

        if (!hwnd.IsNull)
        {
            Control? control = Control.FromHandle(hwnd);
            if (control is not null)
            {
                current = control;
                while (current is not null && current.Visible)
                {
                    current = current.ParentInternal;
                }

                if (current is not null)
                {
                    hwnd = HWND.Null;
                }

                success = true;
            }
        }

        return hwnd;
    }

    private void OnTopLevelPropertyChanged(object? s, EventArgs e)
    {
        ClearTopLevelControlEvents();
        _topLevelControl = null;

        // We must re-acquire this control.  If the existing top level control's handle
        // was never created, but the new parent has a handle, if we don't re-get
        // the top level control here we won't ever create the tooltip handle.
        _topLevelControl = TopLevelControl;
    }

    private void RecreateHandle()
    {
        if (DesignMode)
        {
            return;
        }

        if (GetHandleCreated())
        {
            DestroyHandle();
        }

        _created.Clear();
        CreateHandle();
        CreateAllRegions();
    }

    /// <summary>
    ///  Removes all of the tooltips currently associated with the <see cref="ToolTip"/> control.
    /// </summary>
    public void RemoveAll()
    {
        Control[] controls = [.. _tools.Keys];
        foreach (Control control in controls)
        {
            if (control.IsHandleCreated)
            {
                DestroyRegion(control);
            }

            control.HandleCreated -= HandleCreated;
            control.HandleDestroyed -= HandleDestroyed;

            KeyboardToolTipStateMachine.Instance.Unhook(control, toolTip: this);
        }

        _created.Clear();
        _tools.Clear();

        ClearTopLevelControlEvents();
        _topLevelControl = null;

        KeyboardToolTipStateMachine.Instance.ResetStateMachine(this);
    }

    /// <summary>
    ///  Sets the delay time of TTDT_* values.
    /// </summary>
    private void SetDelayTime(uint type, int time)
    {
        _auto = type == PInvoke.TTDT_AUTOMATIC;

        _delayTimes[(int)type] = time;

        if (GetHandleCreated() && time >= 0)
        {
            PInvoke.SendMessage(this, PInvoke.TTM_SETDELAYTIME, (WPARAM)type, (LPARAM)time);
            if (type == PInvoke.TTDT_AUTOPOP && time != InfiniteDelay)
            {
                IsPersistent = false;
            }

            // Update everyone else if automatic is set. we need to do this
            // to preserve value in case of handle recreation.
            if (_auto)
            {
                _delayTimes[(int)PInvoke.TTDT_AUTOPOP] = GetDelayTime(PInvoke.TTDT_AUTOPOP);
                _delayTimes[(int)PInvoke.TTDT_INITIAL] = GetDelayTime(PInvoke.TTDT_INITIAL);
                _delayTimes[(int)PInvoke.TTDT_RESHOW] = GetDelayTime(PInvoke.TTDT_RESHOW);
            }
        }
        else if (_auto)
        {
            AdjustBaseFromAuto();
        }
    }

    /// <summary>
    ///  Associates <see cref="ToolTip"/> text with the specified control.
    /// </summary>
    public void SetToolTip(Control control, string? caption)
    {
        TipInfo info = new(caption, TipInfo.Type.Auto);
        SetToolTipInternal(control, info);
    }

    /// <summary>
    ///  Associates <see cref="ToolTip"/> text with the specified information
    /// </summary>
    private void SetToolTipInternal(Control control, TipInfo info)
    {
        ArgumentNullException.ThrowIfNull(control);

        bool exists = _tools.ContainsKey(control);
        bool empty = info is null || string.IsNullOrEmpty(info.Caption);
        if (exists && empty)
        {
            _tools.Remove(control);
        }
        else if (!empty)
        {
            _tools[control] = info!;
        }

        if (!empty && !exists)
        {
            control.HandleCreated += HandleCreated;
            control.HandleDestroyed += HandleDestroyed;

            if (control.IsHandleCreated)
            {
                HandleCreated(control, EventArgs.Empty);
            }
        }
        else
        {
            bool handlesCreated = control.IsHandleCreated
                                  && TopLevelControl is not null
                                  && TopLevelControl.IsHandleCreated;

            if (exists && !empty && handlesCreated && !DesignMode)
            {
                ToolInfoWrapper<Control> toolInfo = GetTOOLINFO(control, info!.Caption);
                toolInfo.SendMessage(this, PInvoke.TTM_SETTOOLINFOW);
                SetToolTipToControl(control);
            }
            else if (empty && exists && !DesignMode)
            {
                control.HandleCreated -= HandleCreated;
                control.HandleDestroyed -= HandleDestroyed;

                if (control.IsHandleCreated)
                {
                    HandleDestroyed(control, EventArgs.Empty);
                }

                _created.Remove(control);
            }
        }
    }

    private void SetToolTipToControl(Control associatedControl)
    {
        if (GetHandleCreated())
        {
            associatedControl.SetToolTip(this);
        }
    }

    /// <summary>
    ///  Persistent tooltip is not dismissed automatically by a timer. It is dismissed by mouse
    ///  movements outside this tooltip or by WM_KEYUP for CONTROL or ESCAPE key.
    ///  Windows 11 considers tooltip persistent if AuoPopDelay had never been set or
    ///  was set to infinity.
    /// </summary>
    internal bool IsPersistent { get; set; }

    /// <summary>
    ///  Returns true if the AutomaticDelay property should be persisted.
    /// </summary>
    private bool ShouldSerializeAutomaticDelay() => _auto && AutomaticDelay != DefaultDelay;

    /// <summary>
    ///  Returns true if the AutoPopDelay property should be persisted.
    /// </summary>
    private bool ShouldSerializeAutoPopDelay() => !_auto;

    /// <summary>
    ///  Returns true if the InitialDelay property should be persisted.
    /// </summary>
    private bool ShouldSerializeInitialDelay() => !_auto;

    /// <summary>
    ///  Returns true if the ReshowDelay property should be persisted.
    /// </summary>
    private bool ShouldSerializeReshowDelay() => !_auto;

    /// <summary>
    ///  Shows a tooltip for specified text, window, and hotspot
    /// </summary>
    private void ShowTooltip(string? text, IWin32Window window, int duration)
    {
        ArgumentNullException.ThrowIfNull(window);

        if (window is Control associatedControl)
        {
            PInvoke.GetWindowRect(associatedControl, out var rect);

            _ = Cursor.Current;
            Point cursorLocation = Cursor.Position;
            Point p = cursorLocation;

            Screen screen = Screen.FromPoint(cursorLocation);

            // Place the tool tip on the associated control if its not already there.
            if (cursorLocation.X < rect.left || cursorLocation.X > rect.right ||
                 cursorLocation.Y < rect.top || cursorLocation.Y > rect.bottom)
            {
                // Calculate the dimensions of the visible rectangle which
                // is used to estimate the upper x,y of the tooltip placement.
                RECT visibleRect = new RECT
                {
                    left = (rect.left < screen.WorkingArea.Left) ? screen.WorkingArea.Left : rect.left,
                    top = (rect.top < screen.WorkingArea.Top) ? screen.WorkingArea.Top : rect.top,
                    right = (rect.right > screen.WorkingArea.Right) ? screen.WorkingArea.Right : rect.right,
                    bottom = (rect.bottom > screen.WorkingArea.Bottom) ? screen.WorkingArea.Bottom : rect.bottom
                };

                p.X = visibleRect.left + visibleRect.Width / 2;
                p.Y = visibleRect.top + visibleRect.Height / 2;
                associatedControl.PointToClient(p);
                SetTrackPosition(p.X, p.Y);
                SetTool(window, text, TipInfo.Type.SemiAbsolute, p);

                if (duration > 0)
                {
                    StartTimer(_window, duration);
                }
            }
            else
            {
                if (!_tools.TryGetValue(associatedControl, out TipInfo? tipInfo))
                {
                    tipInfo = new TipInfo(text, TipInfo.Type.SemiAbsolute);
                }
                else
                {
                    tipInfo.TipType |= TipInfo.Type.SemiAbsolute;
                    tipInfo.Caption = text;
                }

                tipInfo.Position = p;
                if (duration > 0)
                {
                    if (_originalPopupDelay == 0)
                    {
                        _originalPopupDelay = AutoPopDelay;
                    }

                    AutoPopDelay = duration;
                }

                SetToolTipInternal(associatedControl, tipInfo);
            }
        }
    }

    /// <summary>
    ///  Associates <see cref="ToolTip"/> with the specified control and displays it.
    /// </summary>
    public void Show(string? text, IWin32Window window)
    {
        // Check if the foreground window is the TopLevelWindow
        if (IsWindowActive(window))
        {
            ShowTooltip(text, window, duration: 0);
        }
    }

    /// <summary>
    ///  Associates <see cref="ToolTip"/> with the specified control and displays it for the
    ///  specified duration.
    /// </summary>
    public void Show(string? text, IWin32Window window, int duration)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentOutOfRangeException.ThrowIfNegative(duration);

        if (IsWindowActive(window))
        {
            ShowTooltip(text, window, duration);
        }
    }

    /// <summary>
    ///  Associates <see cref="ToolTip"/> with the specified control and displays it.
    /// </summary>
    public void Show(string? text, IWin32Window window, Point point)
    {
        ArgumentNullException.ThrowIfNull(window);

        if (IsWindowActive(window))
        {
            // Set the ToolTips.
            PInvoke.GetWindowRect(Control.GetSafeHandle(window), out var r);
            int pointX = r.left + point.X;
            int pointY = r.top + point.Y;

            SetTrackPosition(pointX, pointY);
            SetTool(window, text, TipInfo.Type.Absolute, new Point(pointX, pointY));
        }
    }

    /// <summary>
    ///  Associates <see cref="ToolTip"/> with the specified control and displays it.
    /// </summary>
    public void Show(string? text, IWin32Window window, Point point, int duration)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentOutOfRangeException.ThrowIfNegative(duration);

        if (IsWindowActive(window))
        {
            // Set the ToolTips.
            PInvoke.GetWindowRect(Control.GetSafeHandle(window), out var r);
            int pointX = r.left + point.X;
            int pointY = r.top + point.Y;
            SetTrackPosition(pointX, pointY);
            SetTool(window, text, TipInfo.Type.Absolute, new Point(pointX, pointY));
            StartTimer(window, duration);
        }
    }

    /// <summary>
    ///  Associates <see cref="ToolTip"/> with the specified control and displays it.
    /// </summary>
    public void Show(string? text, IWin32Window window, int x, int y)
    {
        ArgumentNullException.ThrowIfNull(window);

        if (IsWindowActive(window))
        {
            PInvoke.GetWindowRect(Control.GetSafeHandle(window), out var r);
            int pointX = r.left + x;
            int pointY = r.top + y;
            SetTrackPosition(pointX, pointY);
            SetTool(window, text, TipInfo.Type.Absolute, new Point(pointX, pointY));
        }
    }

    /// <summary>
    ///  Associates <see cref="ToolTip"/> with the specified control and displays it.
    /// </summary>
    public void Show(string? text, IWin32Window window, int x, int y, int duration)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentOutOfRangeException.ThrowIfNegative(duration);

        if (IsWindowActive(window))
        {
            PInvoke.GetWindowRect(Control.GetSafeHandle(window), out var r);
            int pointX = r.left + x;
            int pointY = r.top + y;
            SetTrackPosition(pointX, pointY);
            SetTool(window, text, TipInfo.Type.Absolute, new Point(pointX, pointY));
            StartTimer(window, duration);
        }
    }

    internal void ShowKeyboardToolTip(string? text, IKeyboardToolTip tool, int duration)
    {
        ArgumentNullException.ThrowIfNull(tool);
        ArgumentOutOfRangeException.ThrowIfNegative(duration);

        Rectangle toolRectangle = tool.GetNativeScreenRectangle();

        // At first, place the tooltip at the middle of the tool (default location).
        int pointX = (toolRectangle.Left + toolRectangle.Right) / 2;
        int pointY = (toolRectangle.Top + toolRectangle.Bottom) / 2;
        var ownerWindow = tool.GetOwnerWindow();
        if (ownerWindow is null)
        {
            return;
        }

        SetTool(ownerWindow, text, TipInfo.Type.Absolute, new Point(pointX, pointY));

        // Then look for a better ToolTip location.
        if (TryGetBubbleSize(ownerWindow, out Size bubbleSize))
        {
            Point optimalPoint = GetOptimalToolTipPosition(tool, toolRectangle, bubbleSize.Width, bubbleSize.Height);

            // The optimal point should be used as a tracking position.
            pointX = optimalPoint.X;
            pointY = optimalPoint.Y;

            // Update TipInfo for the tool with optimal position.
            if ((tool is Control toolAsControl && _tools.TryGetValue(toolAsControl, out TipInfo? tipInfo)) ||
                (ownerWindow is Control ownerWindowAsControl && _tools.TryGetValue(ownerWindowAsControl, out tipInfo)))
            {
                tipInfo.Position = new Point(pointX, pointY);
            }

            // Ensure that the tooltip bubble is moved to the optimal position even when a mouse tooltip is being replaced with a keyboard tooltip.
            Reposition(optimalPoint, bubbleSize);
        }

        SetTrackPosition(pointX, pointY);
        IsActivatedByKeyboard = true;

        if (!IsPersistent)
        {
            StartTimer(ownerWindow, duration);
        }
    }

    private bool TryGetBubbleSize(IWin32Window ownerWindow, out Size bubbleSize)
    {
        // Get bubble size to use it for optimal position calculation. Requesting the bubble
        // size will AV if there isn't a current tool window.

        HWND hwnd = GetCurrentToolHwnd();
        LRESULT result = default;

        if (!hwnd.IsNull)
        {
            ToolInfoWrapper<HandleRef<HWND>> info = new(Control.GetSafeHandle(ownerWindow));
            result = info.SendMessage(this, PInvoke.TTM_GETBUBBLESIZE);
        }

        if (result == 0)
        {
            bubbleSize = Size.Empty;
            return false;
        }

        bubbleSize = new Size(result.LOWORD, result.HIWORD);

        return true;
    }

    private Point GetOptimalToolTipPosition(IKeyboardToolTip tool, Rectangle toolRectangle, int width, int height)
    {
        // Possible tooltip locations are tied to the tool rectangle bounds.
        int centeredX = toolRectangle.Left + toolRectangle.Width / 2 - width / 2; // tooltip will be aligned with tool vertically
        int centeredY = toolRectangle.Top + toolRectangle.Height / 2 - height / 2; // tooltip will be aligned with tool horizontally

        Rectangle[] possibleLocations =
        [
            new Rectangle(centeredX, toolRectangle.Top - height, width, height),
            new Rectangle(toolRectangle.Right, centeredY, width, height),
            new Rectangle(centeredX, toolRectangle.Bottom, width, height),
            new Rectangle(toolRectangle.Left - width, centeredY, width, height),
        ];

        // Neighboring tools should not be overlapped (ideally) by tooltip.
        IList<Rectangle> neighboringToolsRectangles = tool.GetNeighboringToolsRectangles();

        // Weights are used to determine which one of the possible location overlaps least area of the neighboring tools.
        long[] locationWeights = new long[LocationTotal];

        // Check if the possible locations intersect with the neighboring tools.
        for (int i = 0; i < possibleLocations.Length; i++)
        {
            foreach (Rectangle neighboringToolRectangle in neighboringToolsRectangles)
            {
                Rectangle intersection = Rectangle.Intersect(possibleLocations[i], neighboringToolRectangle);
                checked
                {
                    locationWeights[i] += Math.Abs((long)intersection.Width * intersection.Height); // Intersection is a weight
                }
            }
        }

        // Calculate clipped area of possible locations i.e. area which is located outside the screen area.
        Rectangle screenBounds = SystemInformation.VirtualScreen;
        long[] locationClippedAreas = new long[LocationTotal];
        for (int i = 0; i < possibleLocations.Length; i++)
        {
            Rectangle locationAreaWithinScreen = Rectangle.Intersect(screenBounds, possibleLocations[i]);
            checked
            {
                locationClippedAreas[i] = (Math.Abs((long)possibleLocations[i].Width) - Math.Abs((long)locationAreaWithinScreen.Width))
                    * (Math.Abs((long)possibleLocations[i].Height) - Math.Abs((long)locationAreaWithinScreen.Height));
            }
        }

        // Calculate area of possible locations within top level control rectangle
        long[] locationWithinTopControlAreas = new long[LocationTotal];
        Rectangle topContainerBounds = ((IKeyboardToolTip?)TopLevelControl)?.GetNativeScreenRectangle() ?? Rectangle.Empty;
        if (!topContainerBounds.IsEmpty)
        {
            for (int i = 0; i < possibleLocations.Length; i++)
            {
                Rectangle locationWithinTopControlRectangle = Rectangle.Intersect(topContainerBounds, possibleLocations[i]);
                checked
                {
                    locationWithinTopControlAreas[i] = Math.Abs((long)locationWithinTopControlRectangle.Height * locationWithinTopControlRectangle.Width);
                }
            }
        }

        // Pick optimal location.
        long leastWeight = locationWeights[0];
        long leastClippedArea = locationClippedAreas[0];
        long biggestAreaWithinTopControl = locationWithinTopControlAreas[0];
        int locationIndex = 0;
        Rectangle optimalLocation = possibleLocations[0];
        bool rtlEnabled = tool.HasRtlModeEnabled();

        for (int i = 1; i < possibleLocations.Length; i++)
        {
            if (IsCompetingLocationBetter(leastClippedArea, leastWeight, biggestAreaWithinTopControl, locationIndex,
                locationClippedAreas[i], locationWeights[i], locationWithinTopControlAreas[i], i,
                rtlEnabled))
            {
                leastWeight = locationWeights[i];
                leastClippedArea = locationClippedAreas[i];
                biggestAreaWithinTopControl = locationWithinTopControlAreas[i];
                locationIndex = i;
                optimalLocation = possibleLocations[i];
            }
        }

        return new Point(optimalLocation.Left, optimalLocation.Top);
    }

    private static bool IsCompetingLocationBetter(
        long originalLocationClippedArea,
        long originalLocationWeight,
        long originalLocationAreaWithinTopControl,
        int originalIndex,
        long competingLocationClippedArea,
        long competingLocationWeight,
        long competingLocationAreaWithinTopControl,
        int competingIndex,
        bool rtlEnabled)
    {
        if (competingLocationClippedArea < originalLocationClippedArea)
        {
            // Prefer location with less clipped area.
            return true;
        }
        else if (competingLocationWeight < originalLocationWeight)
        {
            // Otherwise prefer location with less weight.
            return true;
        }
        else if (competingLocationWeight == originalLocationWeight && competingLocationClippedArea == originalLocationClippedArea)
        {
            // Prefer locations located within top level control.
            if (competingLocationAreaWithinTopControl > originalLocationAreaWithinTopControl)
            {
                return true;
            }
            else if (competingLocationAreaWithinTopControl == originalLocationAreaWithinTopControl)
            {
                switch (originalIndex)
                {
                    case LocationIndexTop:
                        // Top location is the least preferred location.
                        return true;
                    case LocationIndexBottom:
                        // Right and Left locations are preferred instead of Bottom location.
                        if (competingIndex is LocationIndexLeft or LocationIndexRight)
                        {
                            return true;
                        }

                        break;
                    case LocationIndexRight:
                        // When RTL is enabled Left location is preferred.
                        if (rtlEnabled && competingIndex == LocationIndexLeft)
                        {
                            return true;
                        }

                        break;
                    case LocationIndexLeft:
                        // When RTL is disabled Right location is preferred.
                        if (!rtlEnabled && competingIndex == LocationIndexRight)
                        {
                            return true;
                        }

                        break;
                    default:
                        throw new NotSupportedException("Unsupported location index value");
                }
            }
        }

        return false;
    }

    /// <summary>
    ///  Private Function to encapsulate TTM_TRACKPOSITION so that this doesnt fire an extra POP event
    /// </summary>
    private void SetTrackPosition(int pointX, int pointY)
    {
        try
        {
            _trackPosition = true;
            PInvoke.SendMessage(this, PInvoke.TTM_TRACKPOSITION, 0, PARAM.FromLowHigh(pointX, pointY));
        }
        finally
        {
            _trackPosition = false;
        }
    }

    /// <summary>
    ///  Hides <see cref="ToolTip"/> with the specified control.
    /// </summary>
    public void Hide(IWin32Window win)
    {
        ArgumentNullException.ThrowIfNull(win);

        if (_window is null)
        {
            return;
        }

        if (GetHandleCreated())
        {
            ToolInfoWrapper<HandleRef<HWND>> info = new(Control.GetSafeHandle(win));
            info.SendMessage(this, PInvoke.TTM_TRACKACTIVATE);
            info.SendMessage(this, PInvoke.TTM_DELTOOLW);
        }

        StopTimer();

        // Check if the passed in IWin32Window is a Control.
        if (win is not Control tool)
        {
            _owners.Remove((HWND)win.Handle);
        }
        else
        {
            if (_tools.ContainsKey(tool))
            {
                SetToolInfo(tool, GetToolTip(tool));
            }
            else
            {
                _owners.Remove((HWND)win.Handle);
            }

            // Find the Form for associated Control and hook up to the Deactivated event
            // to hide the shown tooltip.
            Form? baseFrom = tool.FindForm();
            if (baseFrom is not null)
            {
                baseFrom.Deactivate -= BaseFormDeactivate;
            }
        }

        // Clear off the toplevel control.
        ClearTopLevelControlEvents();
        _topLevelControl = null;
        IsActivatedByKeyboard = false;
    }

    private void BaseFormDeactivate(object? sender, EventArgs e)
    {
        HideAllToolTips();
        KeyboardToolTipStateMachine.Instance.NotifyAboutFormDeactivation(this);
    }

    private void HideAllToolTips()
    {
        Control[] controls = [.. _owners.Values];
        for (int i = 0; i < controls.Length; i++)
        {
            Hide(controls[i]);
        }
    }

    private void SetTool(IWin32Window window, string? text, TipInfo.Type type, Point position)
    {
        Control? tool = window as Control;
        if (tool is not null && _tools.ContainsKey(tool))
        {
            ToolInfoWrapper<Control> toolInfo = new(tool);
            if (toolInfo.SendMessage(this, PInvoke.TTM_GETTOOLINFOW) != IntPtr.Zero)
            {
                TOOLTIP_FLAGS flags = TOOLTIP_FLAGS.TTF_TRACK;
                if (type is TipInfo.Type.Absolute or TipInfo.Type.SemiAbsolute)
                {
                    flags |= TOOLTIP_FLAGS.TTF_ABSOLUTE;
                }

                toolInfo.Info.uFlags |= flags;
                toolInfo.Text = text;
            }

            if (!_tools.TryGetValue(tool, out TipInfo? tipInfo))
            {
                tipInfo = new TipInfo(text, type);
            }
            else
            {
                tipInfo.TipType |= type;
                tipInfo.Caption = text;
            }

            tipInfo.Position = position;
            _tools[tool] = tipInfo;

            IntPtr result = toolInfo.SendMessage(this, PInvoke.TTM_SETTOOLINFOW);
            result = toolInfo.SendMessage(this, PInvoke.TTM_TRACKACTIVATE, true);
        }
        else
        {
            Hide(window);

            // Need to do this BEFORE we call GetWinTOOLINFO, since it relies on the tools array to be populated
            // in order to find the top level parent.
            if (window is Control windowAsControl)
            {
                if (!_tools.TryGetValue(windowAsControl, out TipInfo? tipInfo))
                {
                    tipInfo = new TipInfo(text, type);
                }
                else
                {
                    tipInfo.TipType |= type;
                    tipInfo.Caption = text;
                }

                tipInfo.Position = position;
                _tools[windowAsControl] = tipInfo;

                _owners[windowAsControl.HWND] = windowAsControl;
            }

            var toolInfo = GetWinTOOLINFO(window);
            toolInfo.Info.uFlags |= TOOLTIP_FLAGS.TTF_TRACK;

            if (type is TipInfo.Type.Absolute or TipInfo.Type.SemiAbsolute)
            {
                toolInfo.Info.uFlags |= TOOLTIP_FLAGS.TTF_ABSOLUTE;
            }

            toolInfo.Text = text;
            IntPtr result = toolInfo.SendMessage(this, PInvoke.TTM_ADDTOOLW);
            result = toolInfo.SendMessage(this, PInvoke.TTM_TRACKACTIVATE, true);
        }

        if (tool is not null)
        {
            // Lets find the Form for associated Control
            // and hook up to the Deactivated event to Hide the Shown tooltip.
            Form? baseFrom = tool.FindForm();
            if (baseFrom is not null)
            {
                baseFrom.Deactivate += BaseFormDeactivate;
            }
        }
    }

    /// <summary>
    ///  Starts the timer hiding Positioned ToolTips.
    /// </summary>
    private void StartTimer(IWin32Window owner, int interval)
    {
        if (_timer is null)
        {
            _timer = new ToolTipTimer(owner);
            // Add the timer handler
            _timer.Tick += TimerHandler;
        }

        _timer.Interval = interval;
        _timer.Start();
    }

    /// <summary>
    ///  Stops the timer for hiding Positioned ToolTips.
    /// </summary>
    protected void StopTimer()
    {
        // Hold a local ref to timer so that a posted message doesn't null this out during disposal.
        ToolTipTimer? timerRef = _timer;

        if (timerRef is not null)
        {
            timerRef.Stop();
            timerRef.Dispose();
            _timer = null;
        }
    }

    /// <summary>
    ///  Generates updown events when the timer calls this function.
    /// </summary>
    private void TimerHandler(object? source, EventArgs args)
    {
        Hide(((ToolTipTimer)source!).Host);
    }

    /// <summary>
    ///  Finalizes garbage collection.
    /// </summary>
    ~ToolTip() => DestroyHandle();

    /// <summary>
    ///  Returns a string representation for this control.
    /// </summary>
    public override string ToString()
    {
        string s = base.ToString();
        return $"{s} InitialDelay: {InitialDelay}, ShowAlways: {ShowAlways}";
    }

    private void Reposition(Point tipPosition, Size tipSize)
    {
        Point moveToLocation = tipPosition;
        Screen screen = Screen.FromPoint(moveToLocation);

        // Re-adjust the X position of the tool tip if it bleeds off the screen working area
        if (moveToLocation.X + tipSize.Width > screen.WorkingArea.Right)
        {
            moveToLocation.X = screen.WorkingArea.Right - tipSize.Width;
        }

        // re-adjust the Y position of the tool tip if it bleeds off the screen working area.
        if (moveToLocation.Y + tipSize.Height > screen.WorkingArea.Bottom)
        {
            moveToLocation.Y = screen.WorkingArea.Bottom - tipSize.Height;
        }

        PInvoke.SetWindowPos(
            this,
            HWND.HWND_TOPMOST,
            moveToLocation.X,
            moveToLocation.Y,
            tipSize.Width,
            tipSize.Height,
            SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER);
    }

    private HWND GetCurrentToolHwnd()
    {
        ToolInfoWrapper<Control> toolInfo = default;
        if (toolInfo.SendMessage(this, PInvoke.TTM_GETCURRENTTOOLW) != 0)
        {
            return toolInfo.Info.hwnd;
        }

        return default;
    }

    private Control? GetCurrentToolWindow()
    {
        HWND hwnd = GetCurrentToolHwnd();
        return _owners.TryGetValue(hwnd, out Control? control) ? control : Control.FromHandle(hwnd);
    }

    /// <summary>
    ///  Handles the WM_MOVE message.
    /// </summary>
    private void WmMove()
    {
        IWin32Window? window = GetCurrentToolWindow();
        if (window is not Control windowAsControl || !_tools.TryGetValue(windowAsControl, out TipInfo? tipInfo))
        {
            return;
        }

        // Reposition the tooltip when its about to be shown since the tooltip can go out of screen
        // working area bounds Reposition would check the bounds for us.
        PInvoke.GetWindowRect(this, out var rectangle);
        if (tipInfo.Position != Point.Empty)
        {
            Reposition(tipInfo.Position, rectangle.Size);
        }
    }

    /// <summary>
    ///  Handles the WM_MOUSEACTIVATE message.
    /// </summary>
    private void WmMouseActivate(ref Message message)
    {
        IWin32Window? window = GetCurrentToolWindow();
        if (window is null)
        {
            return;
        }

        PInvoke.GetWindowRect(Control.GetSafeHandle(window), out var r);
        Point cursorLocation = Cursor.Position;

        // Do not activate the mouse if its within the bounds of the
        // the associated tool.
        if (cursorLocation.X >= r.left && cursorLocation.X <= r.right &&
            cursorLocation.Y >= r.top && cursorLocation.Y <= r.bottom)
        {
            message.ResultInternal = (LRESULT)(nint)PInvoke.MA_NOACTIVATE;
        }
    }

    /// <summary>
    ///  Handles the WM_WINDOWFROMPOINT message.
    /// </summary>
    private unsafe void WmWindowFromPoint(ref Message message)
    {
        var lpPoint = (Point*)(nint)message.LParamInternal;
        bool result = false;
        message.ResultInternal = (LRESULT)(nint)GetWindowFromPoint(*lpPoint, ref result);
    }

    /// <summary>
    ///  Handles the TTN_SHOW message.
    /// </summary>
    private void WmShow()
    {
        IWin32Window? window = GetCurrentToolWindow();
        if (window is null)
        {
            return;
        }

        // Get the bounds.
        PInvoke.GetWindowRect(this, out var rect);

        Control? toolControl = window as Control;

        Size currentTooltipSize = rect.Size;
        PopupEventArgs e = new(window, toolControl, IsBalloon, currentTooltipSize);
        OnPopup(e);

        if (toolControl is DataGridView dataGridView && dataGridView.CancelToolTipPopup(this))
        {
            // The dataGridView cancelled the tooltip.
            e.Cancel = true;
        }

        if (!e.Cancel)
        {
            // Use GetWindowText instead of GetCaptionForTool to retrieve the actual caption.
            // GetCaptionForTool doesn't work correctly when the text for a tool is retrieved
            // by TTN_NEEDTEXT notification (e.g. TabPages of TabControl).
            AnnounceText(toolControl, PInvoke.GetWindowText(this));
        }

        // We need to re-get the rectangle of the tooltip here because
        // any of the tooltip attributes/properties could have been updated
        // during the popup event; in which case the size of the tooltip is
        // affected. e.ToolTipSize is respected over rect.Size
        PInvoke.GetWindowRect(this, out rect);
        currentTooltipSize = (e.ToolTipSize == currentTooltipSize) ? rect.Size : e.ToolTipSize;

        if (IsBalloon)
        {
            // Get the text display rectangle
            PInvoke.SendMessage(this, PInvoke.TTM_ADJUSTRECT, (WPARAM)(BOOL)true, ref rect);
            if (rect.Height > currentTooltipSize.Height)
            {
                currentTooltipSize.Height = rect.Height;
            }
        }

        // Set the max possible size of the tooltip to the size we received.
        // This prevents the operating system from drawing incorrect rectangles
        // when determining the correct display rectangle
        // Set the MaxWidth only if user has changed the width.
        if (currentTooltipSize != rect.Size)
        {
            Screen screen = Screen.FromPoint(Cursor.Position);
            int maxwidth = (IsBalloon)
                ? Math.Min(currentTooltipSize.Width - 2 * BalloonOffsetX, screen.WorkingArea.Width)
                : Math.Min(currentTooltipSize.Width, screen.WorkingArea.Width);
            PInvoke.SendMessage(this, PInvoke.TTM_SETMAXTIPWIDTH, 0, maxwidth);
        }

        if (e.Cancel)
        {
            _cancelled = true;
            PInvoke.SetWindowPos(
                this,
                HWND.HWND_TOPMOST,
                0, 0, 0, 0,
                SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER);
        }
        else
        {
            _cancelled = false;

            // Only width/height changes are respected, so set top,left to what we got earlier
            PInvoke.SetWindowPos(
                this,
                HWND.HWND_TOPMOST,
                rect.left,
                rect.top,
                currentTooltipSize.Width,
                currentTooltipSize.Height,
                SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOOWNERZORDER);
        }
    }

    /// <summary>
    ///  Handles the WM_WINDOWPOSCHANGED message.
    ///  We need to Hide the window since the native tooltip actually calls SetWindowPos in its TTN_SHOW even if we cancel showing the
    ///  tooltip. Hence we need to listen to the WindowPosChanged message can hide the window ourselves.
    /// </summary>
    private bool WmWindowPosChanged()
    {
        if (_cancelled)
        {
            PInvoke.ShowWindow(this, SHOW_WINDOW_CMD.SW_HIDE);
            return true;
        }

        return false;
    }

    /// <summary>
    ///  Handles the WM_WINDOWPOSCHANGING message.
    /// </summary>
    private unsafe void WmWindowPosChanging(ref Message message)
    {
        if (_cancelled || _isDisposing)
        {
            return;
        }

        WINDOWPOS* wp = (WINDOWPOS*)(nint)message.LParamInternal;

        Cursor? currentCursor = Cursor.Current;
        Point cursorPos = Cursor.Position;

        if (GetCurrentToolWindow() is not { } window)
        {
            message.ResultInternal = (LRESULT)0;
            return;
        }

        TipInfo? tipInfo = null;

        if (window is Control windowAsControl && !_tools.TryGetValue(windowAsControl, out tipInfo))
        {
            return;
        }

        // Treeview handles its own ToolTips.
        if (window is TreeView treeView && treeView.ShowNodeToolTips)
        {
            return;
        }

        if (IsBalloon)
        {
            wp->cx += 2 * BalloonOffsetX;
            return;
        }

        if ((tipInfo!.TipType & TipInfo.Type.Auto) != 0 && _window is not null)
        {
            _window.DefWndProc(ref message);
            return;
        }

        if (((tipInfo.TipType & TipInfo.Type.SemiAbsolute) != 0) && tipInfo.Position == Point.Empty)
        {
            Screen screen = Screen.FromPoint(cursorPos);
            if (currentCursor is not null)
            {
                wp->x = cursorPos.X;
                wp->y = cursorPos.Y;
                if (wp->y + wp->cy + currentCursor.Size.Height - currentCursor.HotSpot.Y > screen.WorkingArea.Bottom)
                {
                    wp->y = cursorPos.Y - wp->cy;
                }
                else
                {
                    wp->y = cursorPos.Y + currentCursor.Size.Height - currentCursor.HotSpot.Y;
                }
            }

            if (wp->x + wp->cx > screen.WorkingArea.Right)
            {
                wp->x = screen.WorkingArea.Right - wp->cx;
            }
        }
        else if ((tipInfo.TipType & TipInfo.Type.SemiAbsolute) != 0 && tipInfo.Position != Point.Empty)
        {
            Screen screen = Screen.FromPoint(tipInfo.Position);
            wp->x = tipInfo.Position.X;
            if (wp->x + wp->cx > screen.WorkingArea.Right)
            {
                wp->x = screen.WorkingArea.Right - wp->cx;
            }

            wp->y = tipInfo.Position.Y;
            if (wp->y + wp->cy > screen.WorkingArea.Bottom)
            {
                wp->y = screen.WorkingArea.Bottom - wp->cy;
            }
        }

        message.ResultInternal = (LRESULT)0;
    }

    /// <summary>
    ///  Called just before the tooltip is hidden.
    /// </summary>
    private void WmPop()
    {
        if (GetCurrentToolWindow() is not IWin32Window window)
        {
            return;
        }

        if (window is not Control control || !_tools.TryGetValue(control, out TipInfo? tipInfo))
        {
            return;
        }

        // Must reset the maxwidth to the screen size.
        if ((tipInfo.TipType & TipInfo.Type.Auto) != 0 || (tipInfo.TipType & TipInfo.Type.SemiAbsolute) != 0)
        {
            Screen screen = Screen.FromPoint(Cursor.Position);
            PInvoke.SendMessage(this, PInvoke.TTM_SETMAXTIPWIDTH, 0, screen.WorkingArea.Width);
        }

        // For non-auto tips (those shown through the show(.) methods, we need to
        // disassociate them from the tip control.
        if ((tipInfo.TipType & TipInfo.Type.Auto) == 0)
        {
            _tools.Remove(control);
            _owners.Remove((HWND)window.Handle);

            control.HandleCreated -= HandleCreated;
            control.HandleDestroyed -= HandleDestroyed;
            _created.Remove(control);

            if (_originalPopupDelay != 0)
            {
                AutoPopDelay = _originalPopupDelay;
                _originalPopupDelay = 0;
            }
        }
        else
        {
            // Clear all other flags except for the Auto flag to ensure automatic tips can still show
            tipInfo.TipType = TipInfo.Type.Auto;
            tipInfo.Position = Point.Empty;
            _tools[control] = tipInfo;
        }
    }

    private unsafe void WndProc(ref Message message)
    {
        switch (message.Msg)
        {
            case (int)(MessageId.WM_REFLECT_NOTIFY):
                var nmhdr = (NMHDR*)(nint)message.LParamInternal;
                if (nmhdr->code == PInvoke.TTN_SHOW && !_trackPosition)
                {
                    WmShow();
                }
                else if (nmhdr->code == PInvoke.TTN_POP)
                {
                    WmPop();
                    _window?.DefWndProc(ref message);
                }

                break;

            case (int)PInvoke.WM_WINDOWPOSCHANGING:
                WmWindowPosChanging(ref message);
                break;

            case (int)PInvoke.WM_WINDOWPOSCHANGED:
                if (!WmWindowPosChanged() && _window is not null)
                {
                    _window.DefWndProc(ref message);
                }

                break;

            case (int)PInvoke.WM_MOUSEACTIVATE:
                WmMouseActivate(ref message);
                break;

            case (int)PInvoke.WM_MOVE:
                WmMove();
                break;

            case (int)PInvoke.TTM_WINDOWFROMPOINT:
                WmWindowFromPoint(ref message);
                break;

            case (int)PInvoke.WM_PRINTCLIENT:
                goto case (int)PInvoke.WM_PAINT;

            case (int)PInvoke.WM_PAINT:
                if (OwnerDraw && !_isBalloon && !_trackPosition)
                {
                    using BeginPaintScope paintScope = new(HWND);
                    Rectangle bounds = paintScope.PaintRectangle;
                    if (bounds == Rectangle.Empty)
                    {
                        return;
                    }

                    using Graphics graphics = paintScope.HDC.CreateGraphics();

                    IWin32Window? window = GetCurrentToolWindow();
                    if (window is not null)
                    {
                        Font font;
                        try
                        {
                            font = Font.FromHfont(PInvoke.SendMessage(this, PInvoke.WM_GETFONT));
                        }
                        catch (ArgumentException)
                        {
                            // If the current default tooltip font is a non-TrueType font, then
                            // Font.FromHfont throws this exception, so fall back to the default control font.
                            font = Control.DefaultFont;
                        }

                        Control? control = window as Control ?? Control.FromHandle(window.Handle);
                        OnDraw(
                            new DrawToolTipEventArgs(
                                graphics,
                                window,
                                control,
                                bounds,
                                GetToolTip(control),
                                BackColor,
                                ForeColor,
                                font));

                        break;
                    }
                }

                // If not OwnerDraw, fall through.
                goto default;
            default:
                _window?.DefWndProc(ref message);
                break;
        }
    }
}

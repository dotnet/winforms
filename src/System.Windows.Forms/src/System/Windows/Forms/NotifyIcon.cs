// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Specifies a component that creates an icon in the Windows System Tray. This class cannot be inherited.
/// </summary>
[DefaultProperty(nameof(Text))]
[DefaultEvent(nameof(MouseDoubleClick))]
[Designer($"System.Windows.Forms.Design.NotifyIconDesigner, {AssemblyRef.SystemDesign}")]
[ToolboxItemFilter("System.Windows.Forms")]
[SRDescription(nameof(SR.DescriptionNotifyIcon))]
public sealed partial class NotifyIcon : Component
{
    internal const int MaxTextSize = 127;
    private static readonly object s_mouseDownEvent = new();
    private static readonly object s_mouseMoveEvent = new();
    private static readonly object s_mouseUpEvent = new();
    private static readonly object s_clickEvent = new();
    private static readonly object s_doubleClickEvent = new();
    private static readonly object s_mouseClickEvent = new();
    private static readonly object s_mouseDoubleClickEvent = new();
    private static readonly object s_balloonTipShownEvent = new();
    private static readonly object s_balloonTipClickedEvent = new();
    private static readonly object s_balloonTipClosedEvent = new();

    private const int WM_TRAYMOUSEMESSAGE = (int)PInvokeCore.WM_USER + 1024;
    private static uint WM_TASKBARCREATED { get; } = PInvoke.RegisterWindowMessage("TaskbarCreated");

    private readonly Lock _lock = new();

    private Icon? _icon;
    private string _text = string.Empty;
    private readonly uint _id;
    private bool _added;
    private NotifyIconNativeWindow _window;
    private ContextMenuStrip? _contextMenuStrip;
    private ToolTipIcon _balloonTipIcon;
    private string _balloonTipText = string.Empty;
    private string _balloonTipTitle = string.Empty;
    private static uint s_nextId;
    private object? _userData;
    private bool _doubleClick; // checks if doubleclick is fired

    // Visible defaults to false, but the NotifyIconDesigner makes it seem like the default is
    // true. We do this because while visible is the more common case, if it was a true default,
    // there would be no way to create a hidden NotifyIcon without being visible for a moment.
    private bool _visible;

    /// <summary>
    ///  Initializes a new instance of the <see cref="NotifyIcon"/> class.
    /// </summary>
    public NotifyIcon()
    {
        _id = ++s_nextId;
        _window = new NotifyIconNativeWindow(this);
        UpdateIcon(_visible);
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="NotifyIcon"/> class.
    /// </summary>
    public NotifyIcon(IContainer container) : this()
    {
        ArgumentNullException.ThrowIfNull(container);

        container.Add(this);
    }

    /// <summary>
    ///  Gets or sets the BalloonTip text displayed when
    ///  the mouse hovers over a system tray icon.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [DefaultValue("")]
    [SRDescription(nameof(SR.NotifyIconBalloonTipTextDescr))]
    [Editor($"System.ComponentModel.Design.MultilineStringEditor, {AssemblyRef.SystemDesign}", typeof(Drawing.Design.UITypeEditor))]
    public string BalloonTipText
    {
        get
        {
            return _balloonTipText;
        }
        set
        {
            if (value != _balloonTipText)
            {
                _balloonTipText = value;
            }
        }
    }

    /// <summary>
    ///  Gets or sets the BalloonTip icon displayed when
    ///  the mouse hovers over a system tray icon.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(ToolTipIcon.None)]
    [SRDescription(nameof(SR.NotifyIconBalloonTipIconDescr))]
    public ToolTipIcon BalloonTipIcon
    {
        get
        {
            return _balloonTipIcon;
        }
        set
        {
            // valid values are 0x0 to 0x3
            SourceGenerated.EnumValidator.Validate(value);
            if (value != _balloonTipIcon)
            {
                _balloonTipIcon = value;
            }
        }
    }

    /// <summary>
    ///  Gets or sets the BalloonTip title displayed when
    ///  the mouse hovers over a system tray icon.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [DefaultValue("")]
    [SRDescription(nameof(SR.NotifyIconBalloonTipTitleDescr))]
    public string BalloonTipTitle
    {
        get
        {
            return _balloonTipTitle;
        }
        set
        {
            if (value != _balloonTipTitle)
            {
                _balloonTipTitle = value;
            }
        }
    }

    /// <summary>
    ///  [This event is raised on the NIN_BALLOONUSERCLICK message.]
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.NotifyIconOnBalloonTipClickedDescr))]
    public event EventHandler? BalloonTipClicked
    {
        add => Events.AddHandler(s_balloonTipClickedEvent, value);

        remove => Events.RemoveHandler(s_balloonTipClickedEvent, value);
    }

    /// <summary>
    ///  [This event is raised on the NIN_BALLOONTIMEOUT message.]
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.NotifyIconOnBalloonTipClosedDescr))]
    public event EventHandler? BalloonTipClosed
    {
        add => Events.AddHandler(s_balloonTipClosedEvent, value);

        remove => Events.RemoveHandler(s_balloonTipClosedEvent, value);
    }

    /// <summary>
    ///  [This event is raised on the NIN_BALLOONSHOW or NIN_BALLOONHIDE message.]
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.NotifyIconOnBalloonTipShownDescr))]
    public event EventHandler? BalloonTipShown
    {
        add => Events.AddHandler(s_balloonTipShownEvent, value);
        remove => Events.RemoveHandler(s_balloonTipShownEvent, value);
    }

    [DefaultValue(null)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.NotifyIconMenuDescr))]
    public ContextMenuStrip? ContextMenuStrip
    {
        get
        {
            return _contextMenuStrip;
        }

        set
        {
            _contextMenuStrip = value;
        }
    }

    /// <summary>
    ///  Gets or sets the current
    ///  icon.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [DefaultValue(null)]
    [SRDescription(nameof(SR.NotifyIconIconDescr))]
    public Icon? Icon
    {
        get
        {
            return _icon;
        }
        set
        {
            if (_icon != value)
            {
                _icon = value;
                UpdateIcon(_visible);
            }
        }
    }

    /// <summary>
    ///  Gets or sets the ToolTip text displayed when
    ///  the mouse hovers over a system tray icon.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [DefaultValue("")]
    [AllowNull]
    [SRDescription(nameof(SR.NotifyIconTextDescr))]
    [Editor($"System.ComponentModel.Design.MultilineStringEditor, {AssemblyRef.SystemDesign}", typeof(Drawing.Design.UITypeEditor))]
    public string Text
    {
        get
        {
            return _text;
        }
        set
        {
            value ??= string.Empty;

            if (!value.Equals(_text))
            {
                if (value.Length > MaxTextSize)
                {
                    throw new ArgumentOutOfRangeException(nameof(Text), value, SR.TrayIcon_TextTooLong);
                }

                _text = value;
                if (_added)
                {
                    UpdateIcon(true);
                }
            }
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the icon is visible in the Windows System Tray.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.NotifyIconVisDescr))]
    public bool Visible
    {
        get
        {
            return _visible;
        }
        set
        {
            if (_visible != value)
            {
                UpdateIcon(value);
                _visible = value;
            }
        }
    }

    [SRCategory(nameof(SR.CatData))]
    [Localizable(false)]
    [Bindable(true)]
    [SRDescription(nameof(SR.ControlTagDescr))]
    [DefaultValue(null)]
    [TypeConverter(typeof(StringConverter))]
    public object? Tag
    {
        get
        {
            return _userData;
        }
        set
        {
            _userData = value;
        }
    }

    /// <summary>
    ///  Occurs when the user clicks the icon in the system tray.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ControlOnClickDescr))]
    public event EventHandler? Click
    {
        add => Events.AddHandler(s_clickEvent, value);
        remove => Events.RemoveHandler(s_clickEvent, value);
    }

    /// <summary>
    ///  Occurs when the user double-clicks the icon in the system tray.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ControlOnDoubleClickDescr))]
    public event EventHandler? DoubleClick
    {
        add => Events.AddHandler(s_doubleClickEvent, value);
        remove => Events.RemoveHandler(s_doubleClickEvent, value);
    }

    /// <summary>
    ///  Occurs when the user clicks the icon in the system tray.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.NotifyIconMouseClickDescr))]
    public event MouseEventHandler? MouseClick
    {
        add => Events.AddHandler(s_mouseClickEvent, value);
        remove => Events.RemoveHandler(s_mouseClickEvent, value);
    }

    /// <summary>
    ///  Occurs when the user mouse double clicks the icon in the system tray.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.NotifyIconMouseDoubleClickDescr))]
    public event MouseEventHandler? MouseDoubleClick
    {
        add => Events.AddHandler(s_mouseDoubleClickEvent, value);
        remove => Events.RemoveHandler(s_mouseDoubleClickEvent, value);
    }

    /// <summary>
    ///  Occurs when the
    ///  user presses a mouse button while the pointer is over the icon in the system tray.
    /// </summary>
    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.ControlOnMouseDownDescr))]
    public event MouseEventHandler? MouseDown
    {
        add => Events.AddHandler(s_mouseDownEvent, value);
        remove => Events.RemoveHandler(s_mouseDownEvent, value);
    }

    /// <summary>
    ///  Occurs
    ///  when the user moves the mouse pointer over the icon in the system tray.
    /// </summary>
    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.ControlOnMouseMoveDescr))]
    public event MouseEventHandler? MouseMove
    {
        add => Events.AddHandler(s_mouseMoveEvent, value);
        remove => Events.RemoveHandler(s_mouseMoveEvent, value);
    }

    /// <summary>
    ///  Occurs when the
    ///  user releases the mouse button while the pointer
    ///  is over the icon in the system tray.
    /// </summary>
    [SRCategory(nameof(SR.CatMouse))]
    [SRDescription(nameof(SR.ControlOnMouseUpDescr))]
    public event MouseEventHandler? MouseUp
    {
        add => Events.AddHandler(s_mouseUpEvent, value);
        remove => Events.RemoveHandler(s_mouseUpEvent, value);
    }

    /// <summary>
    ///  Releases the unmanaged resources used by the <see cref="NotifyIcon" />
    ///  and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">
    ///  <see langword="true" /> to release both managed and unmanaged resources;
    ///  <see langword="false" /> to release only unmanaged resources.
    /// </param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_window is not null)
            {
                _icon = null;
                Text = string.Empty;
                UpdateIcon(showIconInTray: false);
                _window.DestroyHandle();
                _window = null!;
                _contextMenuStrip = null;
            }
        }
        else
        {
            // This same post is done in ControlNativeWindow's finalize method, so if you change
            // it, change it there too.
            if (_window is not null && _window.Handle != 0)
            {
                PInvokeCore.PostMessage(_window, PInvokeCore.WM_CLOSE);
                _window.ReleaseHandle();
            }
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///  This method raised the BalloonTipClicked event.
    /// </summary>
    private void OnBalloonTipClicked()
    {
        ((EventHandler?)Events[s_balloonTipClickedEvent])?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///  This method raised the BalloonTipClosed event.
    /// </summary>
    private void OnBalloonTipClosed()
    {
        ((EventHandler?)Events[s_balloonTipClosedEvent])?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///  This method raised the BalloonTipShown event.
    /// </summary>
    private void OnBalloonTipShown()
    {
        ((EventHandler?)Events[s_balloonTipShownEvent])?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    ///  This method actually raises the Click event. Inheriting classes should
    ///  override this if they wish to be notified of a Click event. (This is far
    ///  preferable to actually adding an event handler.) They should not,
    ///  however, forget to call base.onClick(e); before exiting, to ensure that
    ///  other recipients do actually get the event.
    /// </summary>
    private void OnClick(EventArgs e)
    {
        ((EventHandler?)Events[s_clickEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onDoubleClick to send this event to any registered event listeners.
    /// </summary>
    private void OnDoubleClick(EventArgs e)
    {
        ((EventHandler?)Events[s_doubleClickEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.OnMouseClick to send this event to any registered event listeners.
    /// </summary>
    private void OnMouseClick(MouseEventArgs mea)
    {
        ((MouseEventHandler?)Events[s_mouseClickEvent])?.Invoke(this, mea);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.OnMouseDoubleClick to send this event to any registered event listeners.
    /// </summary>
    private void OnMouseDoubleClick(MouseEventArgs mea)
    {
        ((MouseEventHandler?)Events[s_mouseDoubleClickEvent])?.Invoke(this, mea);
    }

    /// <summary>
    ///  Raises the <see cref="MouseDown"/> event.
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onMouseDown to send this event to any registered event listeners.
    /// </summary>
    private void OnMouseDown(MouseEventArgs e)
    {
        ((MouseEventHandler?)Events[s_mouseDownEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onMouseMove to send this event to any registered event listeners.
    /// </summary>
    private void OnMouseMove(MouseEventArgs e)
    {
        ((MouseEventHandler?)Events[s_mouseMoveEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Inheriting classes should override this method to handle this event.
    ///  Call base.onMouseUp to send this event to any registered event listeners.
    /// </summary>
    private void OnMouseUp(MouseEventArgs e)
    {
        ((MouseEventHandler?)Events[s_mouseUpEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Displays a balloon tooltip in the taskbar.
    ///
    ///  The system enforces minimum and maximum timeout values. Timeout
    ///  values that are too large are set to the maximum value and values
    ///  that are too small default to the minimum value. The operating system's
    ///  default minimum and maximum timeout values are 10 seconds and 30 seconds,
    ///  respectively.
    ///
    ///  No more than one balloon ToolTip at at time is displayed for the taskbar.
    ///  If an application attempts to display a ToolTip when one is already being displayed,
    ///  the ToolTip will not appear until the existing balloon ToolTip has been visible for at
    ///  least the system minimum timeout value. For example, a balloon ToolTip with timeout
    ///  set to 30 seconds has been visible for seven seconds when another application attempts
    ///  to display a balloon ToolTip. If the system minimum timeout is ten seconds, the first
    ///  ToolTip displays for an additional three seconds before being replaced by the second ToolTip.
    /// </summary>
    public void ShowBalloonTip(int timeout)
    {
        ShowBalloonTip(timeout, _balloonTipTitle, _balloonTipText, _balloonTipIcon);
    }

    /// <summary>
    ///  Displays a balloon tooltip in the taskbar with the specified title,
    ///  text, and icon for a duration of the specified timeout value.
    ///
    ///  The system enforces minimum and maximum timeout values. Timeout
    ///  values that are too large are set to the maximum value and values
    ///  that are too small default to the minimum value. The operating system's
    ///  default minimum and maximum timeout values are 10 seconds and 30 seconds,
    ///  respectively.
    ///
    ///  No more than one balloon ToolTip at at time is displayed for the taskbar.
    ///  If an application attempts to display a ToolTip when one is already being displayed,
    ///  the ToolTip will not appear until the existing balloon ToolTip has been visible for at
    ///  least the system minimum timeout value. For example, a balloon ToolTip with timeout
    ///  set to 30 seconds has been visible for seven seconds when another application attempts
    ///  to display a balloon ToolTip. If the system minimum timeout is ten seconds, the first
    ///  ToolTip displays for an additional three seconds before being replaced by the second ToolTip.
    /// </summary>
    public unsafe void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(timeout);

        if (string.IsNullOrEmpty(tipText))
        {
            throw new ArgumentException(SR.NotifyIconEmptyOrNullTipText);
        }

        // valid values are 0x0 to 0x3
        SourceGenerated.EnumValidator.Validate(tipIcon, nameof(tipIcon));

        if (_added)
        {
            // Bail if in design mode...
            if (DesignMode)
            {
                return;
            }

            NOTIFYICONDATAW data = new()
            {
                cbSize = (uint)sizeof(NOTIFYICONDATAW),
                uFlags = NOTIFY_ICON_DATA_FLAGS.NIF_INFO,
                uID = _id,
                uTimeoutOrVersion = (uint)timeout
            };

            if (_window.Handle == IntPtr.Zero)
            {
                _window.CreateHandle(new CreateParams());
            }

            data.hWnd = _window.Handle;
            data.InfoTitle = tipTitle;
            data.Info = tipText;
            switch (tipIcon)
            {
                case ToolTipIcon.Info:
                    data.dwInfoFlags = NOTIFY_ICON_INFOTIP_FLAGS.NIIF_INFO;
                    break;
                case ToolTipIcon.Warning:
                    data.dwInfoFlags = NOTIFY_ICON_INFOTIP_FLAGS.NIIF_WARNING;
                    break;
                case ToolTipIcon.Error:
                    data.dwInfoFlags = NOTIFY_ICON_INFOTIP_FLAGS.NIIF_ERROR;
                    break;
                case ToolTipIcon.None:
                    data.dwInfoFlags = NOTIFY_ICON_INFOTIP_FLAGS.NIIF_NONE;
                    break;
            }

            PInvoke.Shell_NotifyIconW(NOTIFY_ICON_MESSAGE.NIM_MODIFY, ref data);
        }
    }

    /// <summary>
    ///  Shows the context menu for the tray icon.
    /// </summary>
    private void ShowContextMenu()
    {
        if (_contextMenuStrip is not null)
        {
            PInvoke.GetCursorPos(out Point pt);

            // Summary: the current window must be made the foreground window
            // before calling TrackPopupMenuEx, and a task switch must be
            // forced after the call.
            PInvoke.SetForegroundWindow(_window);

            // this will set the context menu strip to be toplevel
            // and will allow us to overlap the system tray
            _contextMenuStrip.ShowInTaskbar(pt.X, pt.Y);
        }
    }

    /// <summary>
    ///  Updates the icon in the system tray.
    /// </summary>
    private unsafe void UpdateIcon(bool showIconInTray)
    {
        lock (_lock)
        {
            // Bail if in design mode...
            if (DesignMode)
            {
                return;
            }

            _window.LockReference(showIconInTray);

            NOTIFYICONDATAW data = new()
            {
                cbSize = (uint)sizeof(NOTIFYICONDATAW),
                uCallbackMessage = WM_TRAYMOUSEMESSAGE,
                uFlags = NOTIFY_ICON_DATA_FLAGS.NIF_MESSAGE,
                uID = _id
            };

            if (showIconInTray)
            {
                if (_window.Handle == IntPtr.Zero)
                {
                    _window.CreateHandle(new CreateParams());
                }
            }

            data.hWnd = _window.Handle;
            if (_icon is not null)
            {
                data.uFlags |= NOTIFY_ICON_DATA_FLAGS.NIF_ICON;
                data.hIcon = _icon.Handle;
            }

            data.uFlags |= NOTIFY_ICON_DATA_FLAGS.NIF_TIP;
            data.Tip = _text;

            if (showIconInTray && _icon is not null)
            {
                if (!_added)
                {
                    PInvoke.Shell_NotifyIconW(NOTIFY_ICON_MESSAGE.NIM_ADD, ref data);
                    _added = true;
                }
                else
                {
                    PInvoke.Shell_NotifyIconW(NOTIFY_ICON_MESSAGE.NIM_MODIFY, ref data);
                }
            }
            else if (_added)
            {
                PInvoke.Shell_NotifyIconW(NOTIFY_ICON_MESSAGE.NIM_DELETE, ref data);
                _added = false;
            }
        }
    }

    /// <summary>
    ///  Handles the mouse-down event
    /// </summary>
    private void WmMouseDown(MouseButtons button, int clicks)
    {
        if (clicks == 2)
        {
            OnDoubleClick(new MouseEventArgs(button, 2, 0, 0, 0));
            OnMouseDoubleClick(new MouseEventArgs(button, 2, 0, 0, 0));
            _doubleClick = true;
        }

        OnMouseDown(new MouseEventArgs(button, clicks, 0, 0, 0));
    }

    /// <summary>
    ///  Handles the mouse-move event
    /// </summary>
    private void WmMouseMove()
    {
        OnMouseMove(new MouseEventArgs(Control.MouseButtons, 0, 0, 0, 0));
    }

    /// <summary>
    ///  Handles the mouse-up event
    /// </summary>
    private void WmMouseUp(MouseButtons button)
    {
        OnMouseUp(new MouseEventArgs(button, 0, 0, 0, 0));
        // subhag
        if (!_doubleClick)
        {
            OnClick(new MouseEventArgs(button, 0, 0, 0, 0));
            OnMouseClick(new MouseEventArgs(button, 0, 0, 0, 0));
        }

        _doubleClick = false;
    }

    private void WmTaskbarCreated()
    {
        _added = false;
        UpdateIcon(_visible);
    }

    private void WndProc(ref Message msg)
    {
        switch (msg.MsgInternal)
        {
            case WM_TRAYMOUSEMESSAGE:
                switch ((uint)(nint)msg.LParamInternal)
                {
                    case PInvokeCore.WM_LBUTTONDBLCLK:
                        WmMouseDown(MouseButtons.Left, 2);
                        break;
                    case PInvokeCore.WM_LBUTTONDOWN:
                        WmMouseDown(MouseButtons.Left, 1);
                        break;
                    case PInvokeCore.WM_LBUTTONUP:
                        WmMouseUp(MouseButtons.Left);
                        break;
                    case PInvokeCore.WM_MBUTTONDBLCLK:
                        WmMouseDown(MouseButtons.Middle, 2);
                        break;
                    case PInvokeCore.WM_MBUTTONDOWN:
                        WmMouseDown(MouseButtons.Middle, 1);
                        break;
                    case PInvokeCore.WM_MBUTTONUP:
                        WmMouseUp(MouseButtons.Middle);
                        break;
                    case PInvokeCore.WM_MOUSEMOVE:
                        WmMouseMove();
                        break;
                    case PInvokeCore.WM_RBUTTONDBLCLK:
                        WmMouseDown(MouseButtons.Right, 2);
                        break;
                    case PInvokeCore.WM_RBUTTONDOWN:
                        WmMouseDown(MouseButtons.Right, 1);
                        break;
                    case PInvokeCore.WM_RBUTTONUP:
                        if (_contextMenuStrip is not null)
                        {
                            ShowContextMenu();
                        }

                        WmMouseUp(MouseButtons.Right);
                        break;
                    case PInvoke.NIN_BALLOONSHOW:
                        OnBalloonTipShown();
                        break;
                    case PInvoke.NIN_BALLOONHIDE:
                        OnBalloonTipClosed();
                        break;
                    case PInvoke.NIN_BALLOONTIMEOUT:
                        OnBalloonTipClosed();
                        break;
                    case PInvoke.NIN_BALLOONUSERCLICK:
                        OnBalloonTipClicked();
                        break;
                }

                break;
            case PInvokeCore.WM_COMMAND:
                if (msg.LParamInternal == 0)
                {
                    if (Command.DispatchID((int)msg.WParamInternal & 0xFFFF))
                    {
                        return;
                    }
                }
                else
                {
                    _window.DefWndProc(ref msg);
                }

                break;

            case PInvokeCore.WM_DESTROY:
                // Remove the icon from the taskbar
                UpdateIcon(false);
                break;

            case PInvokeCore.WM_INITMENUPOPUP:
            default:
                if (msg.Msg == (int)WM_TASKBARCREATED)
                {
                    WmTaskbarCreated();
                }

                _window.DefWndProc(ref msg);
                break;
        }
    }
}

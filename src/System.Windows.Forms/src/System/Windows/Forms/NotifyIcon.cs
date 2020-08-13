// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.Shell32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies a component that creates
    ///  an icon in the Windows System Tray. This class cannot be inherited.
    /// </summary>
    [DefaultProperty(nameof(Text))]
    [DefaultEvent(nameof(MouseDoubleClick))]
    [Designer("System.Windows.Forms.Design.NotifyIconDesigner, " + AssemblyRef.SystemDesign)]
    [ToolboxItemFilter("System.Windows.Forms")]
    [SRDescription(nameof(SR.DescriptionNotifyIcon))]
    public sealed class NotifyIcon : Component
    {
        private static readonly object EVENT_MOUSEDOWN = new object();
        private static readonly object EVENT_MOUSEMOVE = new object();
        private static readonly object EVENT_MOUSEUP = new object();
        private static readonly object EVENT_CLICK = new object();
        private static readonly object EVENT_DOUBLECLICK = new object();
        private static readonly object EVENT_MOUSECLICK = new object();
        private static readonly object EVENT_MOUSEDOUBLECLICK = new object();
        private static readonly object EVENT_BALLOONTIPSHOWN = new object();
        private static readonly object EVENT_BALLOONTIPCLICKED = new object();
        private static readonly object EVENT_BALLOONTIPCLOSED = new object();

        private const int WM_TRAYMOUSEMESSAGE = (int)User32.WM.USER + 1024;
        private static readonly User32.WM WM_TASKBARCREATED = User32.RegisterWindowMessageW("TaskbarCreated");

        private readonly object syncObj = new object();

        private Icon icon;
        private string text = string.Empty;
        private readonly uint id;
        private bool added;
        private NotifyIconNativeWindow window;
        private ContextMenuStrip contextMenuStrip;
        private ToolTipIcon balloonTipIcon;
        private string balloonTipText = string.Empty;
        private string balloonTipTitle = string.Empty;
        private static uint s_nextId;
        private object userData;
        private bool doubleClick; // checks if doubleclick is fired

        // Visible defaults to false, but the NotifyIconDesigner makes it seem like the default is
        // true.  We do this because while visible is the more common case, if it was a true default,
        // there would be no way to create a hidden NotifyIcon without being visible for a moment.
        private bool visible;

        /// <summary>
        ///  Initializes a new instance of the <see cref='NotifyIcon'/> class.
        /// </summary>
        public NotifyIcon()
        {
            id = ++s_nextId;
            window = new NotifyIconNativeWindow(this);
            UpdateIcon(visible);
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='NotifyIcon'/> class.
        /// </summary>
        public NotifyIcon(IContainer container) : this()
        {
            if (container is null)
            {
                throw new ArgumentNullException(nameof(container));
            }

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
        [Editor("System.ComponentModel.Design.MultilineStringEditor, " + AssemblyRef.SystemDesign, typeof(Drawing.Design.UITypeEditor))]
        public string BalloonTipText
        {
            get
            {
                return balloonTipText;
            }
            set
            {
                if (value != balloonTipText)
                {
                    balloonTipText = value;
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
                return balloonTipIcon;
            }
            set
            {
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolTipIcon.None, (int)ToolTipIcon.Error))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolTipIcon));
                }
                if (value != balloonTipIcon)
                {
                    balloonTipIcon = value;
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
                return balloonTipTitle;
            }
            set
            {
                if (value != balloonTipTitle)
                {
                    balloonTipTitle = value;
                }
            }
        }

        /// <summary>
        ///  [This event is raised on the NIN_BALLOONUSERCLICK message.]
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.NotifyIconOnBalloonTipClickedDescr))]
        public event EventHandler BalloonTipClicked
        {
            add => Events.AddHandler(EVENT_BALLOONTIPCLICKED, value);

            remove => Events.RemoveHandler(EVENT_BALLOONTIPCLICKED, value);
        }

        /// <summary>
        ///  [This event is raised on the NIN_BALLOONTIMEOUT message.]
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.NotifyIconOnBalloonTipClosedDescr))]
        public event EventHandler BalloonTipClosed
        {
            add => Events.AddHandler(EVENT_BALLOONTIPCLOSED, value);

            remove => Events.RemoveHandler(EVENT_BALLOONTIPCLOSED, value);
        }

        /// <summary>
        ///  [This event is raised on the NIN_BALLOONSHOW or NIN_BALLOONHIDE message.]
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.NotifyIconOnBalloonTipShownDescr))]
        public event EventHandler BalloonTipShown
        {
            add => Events.AddHandler(EVENT_BALLOONTIPSHOWN, value);
            remove => Events.RemoveHandler(EVENT_BALLOONTIPSHOWN, value);
        }

        [DefaultValue(null)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.NotifyIconMenuDescr))]
        public ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return contextMenuStrip;
            }

            set
            {
                contextMenuStrip = value;
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
        public Icon Icon
        {
            get
            {
                return icon;
            }
            set
            {
                if (icon != value)
                {
                    icon = value;
                    UpdateIcon(visible);
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
        [SRDescription(nameof(SR.NotifyIconTextDescr))]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, " + AssemblyRef.SystemDesign, typeof(Drawing.Design.UITypeEditor))]
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                if (value is null)
                {
                    value = string.Empty;
                }

                if (value != null && !value.Equals(text))
                {
                    if (value != null && value.Length > 63)
                    {
                        throw new ArgumentOutOfRangeException(nameof(Text), value, SR.TrayIcon_TextTooLong);
                    }
                    text = value;
                    if (added)
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
                return visible;
            }
            set
            {
                if (visible != value)
                {
                    UpdateIcon(value);
                    visible = value;
                }
            }
        }

        [SRCategory(nameof(SR.CatData))]
        [Localizable(false)]
        [Bindable(true)]
        [SRDescription(nameof(SR.ControlTagDescr))]
        [DefaultValue(null)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag
        {
            get
            {
                return userData;
            }
            set
            {
                userData = value;
            }
        }

        /// <summary>
        ///  Occurs when the user clicks the icon in the system tray.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ControlOnClickDescr))]
        public event EventHandler Click
        {
            add => Events.AddHandler(EVENT_CLICK, value);
            remove => Events.RemoveHandler(EVENT_CLICK, value);
        }

        /// <summary>
        ///  Occurs when the user double-clicks the icon in the system tray.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ControlOnDoubleClickDescr))]
        public event EventHandler DoubleClick
        {
            add => Events.AddHandler(EVENT_DOUBLECLICK, value);
            remove => Events.RemoveHandler(EVENT_DOUBLECLICK, value);
        }

        /// <summary>
        ///  Occurs when the user clicks the icon in the system tray.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.NotifyIconMouseClickDescr))]
        public event MouseEventHandler MouseClick
        {
            add => Events.AddHandler(EVENT_MOUSECLICK, value);
            remove => Events.RemoveHandler(EVENT_MOUSECLICK, value);
        }

        /// <summary>
        ///  Occurs when the user mouse double clicks the icon in the system tray.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.NotifyIconMouseDoubleClickDescr))]
        public event MouseEventHandler MouseDoubleClick
        {
            add => Events.AddHandler(EVENT_MOUSEDOUBLECLICK, value);
            remove => Events.RemoveHandler(EVENT_MOUSEDOUBLECLICK, value);
        }

        /// <summary>
        ///  Occurs when the
        ///  user presses a mouse button while the pointer is over the icon in the system tray.
        /// </summary>
        [SRCategory(nameof(SR.CatMouse))]
        [SRDescription(nameof(SR.ControlOnMouseDownDescr))]
        public event MouseEventHandler MouseDown
        {
            add => Events.AddHandler(EVENT_MOUSEDOWN, value);
            remove => Events.RemoveHandler(EVENT_MOUSEDOWN, value);
        }

        /// <summary>
        ///  Occurs
        ///  when the user moves the mouse pointer over the icon in the system tray.
        /// </summary>
        [SRCategory(nameof(SR.CatMouse))]
        [SRDescription(nameof(SR.ControlOnMouseMoveDescr))]
        public event MouseEventHandler MouseMove
        {
            add => Events.AddHandler(EVENT_MOUSEMOVE, value);
            remove => Events.RemoveHandler(EVENT_MOUSEMOVE, value);
        }

        /// <summary>
        ///  Occurs when the
        ///  user releases the mouse button while the pointer
        ///  is over the icon in the system tray.
        /// </summary>
        [SRCategory(nameof(SR.CatMouse))]
        [SRDescription(nameof(SR.ControlOnMouseUpDescr))]
        public event MouseEventHandler MouseUp
        {
            add => Events.AddHandler(EVENT_MOUSEUP, value);
            remove => Events.RemoveHandler(EVENT_MOUSEUP, value);
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
                if (window != null)
                {
                    icon = null;
                    Text = string.Empty;
                    UpdateIcon(false);
                    window.DestroyHandle();
                    window = null;
                    contextMenuStrip = null;
                }
            }
            else
            {
                // This same post is done in ControlNativeWindow's finalize method, so if you change
                // it, change it there too.
                //
                if (window != null && window.Handle != IntPtr.Zero)
                {
                    User32.PostMessageW(window, User32.WM.CLOSE);
                    window.ReleaseHandle();
                }
            }
            base.Dispose(disposing);
        }

        /// <summary>
        ///  This method raised the BalloonTipClicked event.
        /// </summary>
        private void OnBalloonTipClicked()
        {
            ((EventHandler)Events[EVENT_BALLOONTIPCLICKED])?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///  This method raised the BalloonTipClosed event.
        /// </summary>
        private void OnBalloonTipClosed()
        {
            ((EventHandler)Events[EVENT_BALLOONTIPCLOSED])?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        ///  This method raised the BalloonTipShown event.
        /// </summary>
        private void OnBalloonTipShown()
        {
            ((EventHandler)Events[EVENT_BALLOONTIPSHOWN])?.Invoke(this, EventArgs.Empty);
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
            ((EventHandler)Events[EVENT_CLICK])?.Invoke(this, e);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onDoubleClick to send this event to any registered event listeners.
        /// </summary>
        private void OnDoubleClick(EventArgs e)
        {
            ((EventHandler)Events[EVENT_DOUBLECLICK])?.Invoke(this, e);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.OnMouseClick to send this event to any registered event listeners.
        /// </summary>
        private void OnMouseClick(MouseEventArgs mea)
        {
            ((MouseEventHandler)Events[EVENT_MOUSECLICK])?.Invoke(this, mea);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.OnMouseDoubleClick to send this event to any registered event listeners.
        /// </summary>
        private void OnMouseDoubleClick(MouseEventArgs mea)
        {
            ((MouseEventHandler)Events[EVENT_MOUSEDOUBLECLICK])?.Invoke(this, mea);
        }

        /// <summary>
        ///  Raises the <see cref='MouseDown'/> event.
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onMouseDown to send this event to any registered event listeners.
        /// </summary>
        private void OnMouseDown(MouseEventArgs e)
        {
            ((MouseEventHandler)Events[EVENT_MOUSEDOWN])?.Invoke(this, e);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onMouseMove to send this event to any registered event listeners.
        /// </summary>
        private void OnMouseMove(MouseEventArgs e)
        {
            ((MouseEventHandler)Events[EVENT_MOUSEMOVE])?.Invoke(this, e);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onMouseUp to send this event to any registered event listeners.
        /// </summary>
        private void OnMouseUp(MouseEventArgs e)
        {
            ((MouseEventHandler)Events[EVENT_MOUSEUP])?.Invoke(this, e);
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
            ShowBalloonTip(timeout, balloonTipTitle, balloonTipText, balloonTipIcon);
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
            if (timeout < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(timeout), timeout, string.Format(SR.InvalidArgument, nameof(timeout), timeout));
            }

            if (string.IsNullOrEmpty(tipText))
            {
                throw new ArgumentException(SR.NotifyIconEmptyOrNullTipText);
            }

            //valid values are 0x0 to 0x3
            if (!ClientUtils.IsEnumValid(tipIcon, (int)tipIcon, (int)ToolTipIcon.None, (int)ToolTipIcon.Error))
            {
                throw new InvalidEnumArgumentException(nameof(tipIcon), (int)tipIcon, typeof(ToolTipIcon));
            }

            if (added)
            {
                // Bail if in design mode...
                if (DesignMode)
                {
                    return;
                }

                var data = new NOTIFYICONDATAW
                {
                    cbSize = (uint)sizeof(NOTIFYICONDATAW),
                    uFlags = NIF.INFO,
                    uID = id,
                    uTimeoutOrVersion = (uint)timeout
                };
                if (window.Handle == IntPtr.Zero)
                {
                    window.CreateHandle(new CreateParams());
                }
                data.hWnd = window.Handle;
                data.InfoTitle = tipTitle;
                data.Info = tipText;
                switch (tipIcon)
                {
                    case ToolTipIcon.Info:
                        data.dwInfoFlags = NIIF.INFO;
                        break;
                    case ToolTipIcon.Warning:
                        data.dwInfoFlags = NIIF.WARNING;
                        break;
                    case ToolTipIcon.Error:
                        data.dwInfoFlags = NIIF.ERROR;
                        break;
                    case ToolTipIcon.None:
                        data.dwInfoFlags = NIIF.NONE;
                        break;
                }
                Shell32.Shell_NotifyIconW(NIM.MODIFY, ref data);
            }
        }

        /// <summary>
        ///  Shows the context menu for the tray icon.
        /// </summary>
        private void ShowContextMenu()
        {
            if (contextMenuStrip != null)
            {
                User32.GetCursorPos(out Point pt);

                // Summary: the current window must be made the foreground window
                // before calling TrackPopupMenuEx, and a task switch must be
                // forced after the call.
                User32.SetForegroundWindow(window);

                // this will set the context menu strip to be toplevel
                // and will allow us to overlap the system tray
                contextMenuStrip.ShowInTaskbar(pt.X, pt.Y);
            }
        }

        /// <summary>
        ///  Updates the icon in the system tray.
        /// </summary>
        private unsafe void UpdateIcon(bool showIconInTray)
        {
            lock (syncObj)
            {
                // Bail if in design mode...
                //
                if (DesignMode)
                {
                    return;
                }

                window.LockReference(showIconInTray);

                var data = new NOTIFYICONDATAW
                {
                    cbSize = (uint)sizeof(NOTIFYICONDATAW),
                    uCallbackMessage = WM_TRAYMOUSEMESSAGE,
                    uFlags = NIF.MESSAGE,
                    uID = id
                };
                if (showIconInTray)
                {
                    if (window.Handle == IntPtr.Zero)
                    {
                        window.CreateHandle(new CreateParams());
                    }
                }
                data.hWnd = window.Handle;
                if (icon != null)
                {
                    data.uFlags |= NIF.ICON;
                    data.hIcon = icon.Handle;
                }
                data.uFlags |= NIF.TIP;
                data.Tip = text;

                if (showIconInTray && icon != null)
                {
                    if (!added)
                    {
                        Shell_NotifyIconW(NIM.ADD, ref data);
                        added = true;
                    }
                    else
                    {
                        Shell_NotifyIconW(NIM.MODIFY, ref data);
                    }
                }
                else if (added)
                {
                    Shell_NotifyIconW(NIM.DELETE, ref data);
                    added = false;
                }
            }
        }

        /// <summary>
        ///  Handles the mouse-down event
        /// </summary>
        private void WmMouseDown(ref Message m, MouseButtons button, int clicks)
        {
            if (clicks == 2)
            {
                OnDoubleClick(new MouseEventArgs(button, 2, 0, 0, 0));
                OnMouseDoubleClick(new MouseEventArgs(button, 2, 0, 0, 0));
                doubleClick = true;
            }
            OnMouseDown(new MouseEventArgs(button, clicks, 0, 0, 0));
        }

        /// <summary>
        ///  Handles the mouse-move event
        /// </summary>
        private void WmMouseMove(ref Message m)
        {
            OnMouseMove(new MouseEventArgs(Control.MouseButtons, 0, 0, 0, 0));
        }

        /// <summary>
        ///  Handles the mouse-up event
        /// </summary>
        private void WmMouseUp(ref Message m, MouseButtons button)
        {
            OnMouseUp(new MouseEventArgs(button, 0, 0, 0, 0));
            //subhag
            if (!doubleClick)
            {
                OnClick(new MouseEventArgs(button, 0, 0, 0, 0));
                OnMouseClick(new MouseEventArgs(button, 0, 0, 0, 0));
            }
            doubleClick = false;
        }

        private void WmTaskbarCreated(ref Message m)
        {
            added = false;
            UpdateIcon(visible);
        }

        private void WndProc(ref Message msg)
        {
            switch ((User32.WM)msg.Msg)
            {
                case (User32.WM)WM_TRAYMOUSEMESSAGE:
                    switch ((int)msg.LParam)
                    {
                        case (int)User32.WM.LBUTTONDBLCLK:
                            WmMouseDown(ref msg, MouseButtons.Left, 2);
                            break;
                        case (int)User32.WM.LBUTTONDOWN:
                            WmMouseDown(ref msg, MouseButtons.Left, 1);
                            break;
                        case (int)User32.WM.LBUTTONUP:
                            WmMouseUp(ref msg, MouseButtons.Left);
                            break;
                        case (int)User32.WM.MBUTTONDBLCLK:
                            WmMouseDown(ref msg, MouseButtons.Middle, 2);
                            break;
                        case (int)User32.WM.MBUTTONDOWN:
                            WmMouseDown(ref msg, MouseButtons.Middle, 1);
                            break;
                        case (int)User32.WM.MBUTTONUP:
                            WmMouseUp(ref msg, MouseButtons.Middle);
                            break;
                        case (int)User32.WM.MOUSEMOVE:
                            WmMouseMove(ref msg);
                            break;
                        case (int)User32.WM.RBUTTONDBLCLK:
                            WmMouseDown(ref msg, MouseButtons.Right, 2);
                            break;
                        case (int)User32.WM.RBUTTONDOWN:
                            WmMouseDown(ref msg, MouseButtons.Right, 1);
                            break;
                        case (int)User32.WM.RBUTTONUP:
                            if (contextMenuStrip != null)
                            {
                                ShowContextMenu();
                            }
                            WmMouseUp(ref msg, MouseButtons.Right);
                            break;
                        case (int)NIN.BALLOONSHOW:
                            OnBalloonTipShown();
                            break;
                        case (int)NIN.BALLOONHIDE:
                            OnBalloonTipClosed();
                            break;
                        case (int)NIN.BALLOONTIMEOUT:
                            OnBalloonTipClosed();
                            break;
                        case (int)NIN.BALLOONUSERCLICK:
                            OnBalloonTipClicked();
                            break;
                    }
                    break;
                case User32.WM.COMMAND:
                    if (IntPtr.Zero == msg.LParam)
                    {
                        if (Command.DispatchID((int)msg.WParam & 0xFFFF))
                        {
                            return;
                        }
                    }
                    else
                    {
                        window.DefWndProc(ref msg);
                    }
                    break;

                case User32.WM.DESTROY:
                    // Remove the icon from the taskbar
                    UpdateIcon(false);
                    break;

                case User32.WM.INITMENUPOPUP:
                default:
                    if (msg.Msg == (int)WM_TASKBARCREATED)
                    {
                        WmTaskbarCreated(ref msg);
                    }

                    window.DefWndProc(ref msg);
                    break;
            }
        }

        /// <summary>
        ///  Defines a placeholder window that the NotifyIcon is attached to.
        /// </summary>
        private class NotifyIconNativeWindow : NativeWindow
        {
            internal NotifyIcon reference;
            private GCHandle rootRef;   // We will root the control when we do not want to be elligible for garbage collection.

            /// <summary>
            ///  Create a new NotifyIcon, and bind the window to the NotifyIcon component.
            /// </summary>
            internal NotifyIconNativeWindow(NotifyIcon component)
            {
                reference = component;
            }

            ~NotifyIconNativeWindow()
            {
                // This same post is done in Control's Dispose method, so if you change
                // it, change it there too.
                //
                if (Handle != IntPtr.Zero)
                {
                    User32.PostMessageW(this, User32.WM.CLOSE);
                }

                // This releases the handle from our window proc, re-routing it back to
                // the system.
            }

            public void LockReference(bool locked)
            {
                if (locked)
                {
                    if (!rootRef.IsAllocated)
                    {
                        rootRef = GCHandle.Alloc(reference, GCHandleType.Normal);
                    }
                }
                else
                {
                    if (rootRef.IsAllocated)
                    {
                        rootRef.Free();
                    }
                }
            }

            protected override void OnThreadException(Exception e)
            {
                Application.OnThreadException(e);
            }

            /// <summary>
            ///  Pass messages on to the NotifyIcon object's wndproc handler.
            /// </summary>
            protected override void WndProc(ref Message m)
            {
                Debug.Assert(reference != null, "NotifyIcon was garbage collected while it was still visible.  How did we let that happen?");
                reference.WndProc(ref m);
            }
        }
    }
}

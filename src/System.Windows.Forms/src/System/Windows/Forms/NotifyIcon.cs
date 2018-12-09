// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Runtime.Remoting;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;    
    using System.Windows.Forms.Design;
    using Microsoft.Win32;
    using System.Drawing;
    using System.Globalization;

    /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies a component that creates
    ///       an icon in the Windows System Tray. This class cannot be inherited.
    ///    </para>
    /// </devdoc>
    [
    DefaultProperty(nameof(Text)),
    DefaultEvent(nameof(MouseDoubleClick)),
    Designer("System.Windows.Forms.Design.NotifyIconDesigner, " + AssemblyRef.SystemDesign),
    ToolboxItemFilter("System.Windows.Forms"),
    SRDescription(nameof(SR.DescriptionNotifyIcon))
    ]
    public sealed class NotifyIcon : Component {
        private static readonly object EVENT_MOUSEDOWN  = new object();
        private static readonly object EVENT_MOUSEMOVE = new object();
        private static readonly object EVENT_MOUSEUP = new object();
        private static readonly object EVENT_CLICK = new object();
        private static readonly object EVENT_DOUBLECLICK = new object();
        private static readonly object EVENT_MOUSECLICK = new object();
        private static readonly object EVENT_MOUSEDOUBLECLICK = new object();
        private static readonly object EVENT_BALLOONTIPSHOWN = new object();
        private static readonly object EVENT_BALLOONTIPCLICKED = new object();
        private static readonly object EVENT_BALLOONTIPCLOSED = new object();

        private const int WM_TRAYMOUSEMESSAGE = NativeMethods.WM_USER + 1024;
        private static int WM_TASKBARCREATED = SafeNativeMethods.RegisterWindowMessage("TaskbarCreated");

        private object syncObj = new object();

        private Icon icon = null;
        private string text = "";
        private int id = 0;
        private bool added = false;
        private NotifyIconNativeWindow window = null;
        private ContextMenu contextMenu = null;
        private ContextMenuStrip contextMenuStrip = null;
        private ToolTipIcon balloonTipIcon;
        private string balloonTipText = "";
        private string balloonTipTitle = "";
        private static int nextId = 0;
        private object userData;
        private bool doubleClick = false; // checks if doubleclick is fired

        // Visible defaults to false, but the NotifyIconDesigner makes it seem like the default is 
        // true.  We do this because while visible is the more common case, if it was a true default,
        // there would be no way to create a hidden NotifyIcon without being visible for a moment.
        private bool visible = false;

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.NotifyIcon"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.NotifyIcon'/> class.
        ///    </para>
        /// </devdoc>
        public NotifyIcon() {
            id = ++nextId;
            window = new NotifyIconNativeWindow(this);
            UpdateIcon(visible);
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.NotifyIcon1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.NotifyIcon'/> class.
        ///    </para>
        /// </devdoc>
        public NotifyIcon(IContainer container) : this() {
            if (container == null) {
                throw new ArgumentNullException(nameof(container));
            }

            container.Add(this);
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.BalloonTipText"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the BalloonTip text displayed when
        ///       the mouse hovers over a system tray icon.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(""),
        SRDescription(nameof(SR.NotifyIconBalloonTipTextDescr)),
        Editor("System.ComponentModel.Design.MultilineStringEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string BalloonTipText {
            get { 
                return balloonTipText; 
            }
            set {
                if (value != balloonTipText) {
                    balloonTipText = value;
                }
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.BalloonTipIcon"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the BalloonTip icon displayed when
        ///       the mouse hovers over a system tray icon.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(ToolTipIcon.None),
        SRDescription(nameof(SR.NotifyIconBalloonTipIconDescr))
        ]
        public ToolTipIcon BalloonTipIcon {
            get { 
                return balloonTipIcon; 
            }
            set {
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolTipIcon.None, (int)ToolTipIcon.Error)){
                   throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolTipIcon));
                }
                if (value != balloonTipIcon) {
                    balloonTipIcon = value;
                }
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.BalloonTipTitle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the BalloonTip title displayed when
        ///       the mouse hovers over a system tray icon.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(""),
        SRDescription(nameof(SR.NotifyIconBalloonTipTitleDescr))
        ]
        public string BalloonTipTitle {
            get { 
                return balloonTipTitle; 
            }
            set {
                if (value != balloonTipTitle) {
                    balloonTipTitle = value;
                }
            }
        }

        /// <include file='doc\NotifyIcon.uex' path='docs/doc[@for="NotifyIcon.BalloonTipClicked"]/*' />
        /// <devdoc>
        ///    <para>[This event is raised on the NIN_BALLOONUSERCLICK message.]</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.NotifyIconOnBalloonTipClickedDescr))]
        public event EventHandler BalloonTipClicked {
            add {
                Events.AddHandler(EVENT_BALLOONTIPCLICKED, value);
            }

            remove {
                Events.RemoveHandler(EVENT_BALLOONTIPCLICKED, value);
            }
        }

        /// <include file='doc\NotifyIcon.uex' path='docs/doc[@for="NotifyIcon.BalloonTipClosed"]/*' />
        /// <devdoc>
        ///    <para>[This event is raised on the NIN_BALLOONTIMEOUT message.]</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.NotifyIconOnBalloonTipClosedDescr))]
        public event EventHandler BalloonTipClosed {
            add {
                Events.AddHandler(EVENT_BALLOONTIPCLOSED, value);
            }

            remove {
                Events.RemoveHandler(EVENT_BALLOONTIPCLOSED, value);
            }
        }

        /// <include file='doc\NotifyIcon.uex' path='docs/doc[@for="NotifyIcon.BalloonTipShown"]/*' />
        /// <devdoc>
        ///    <para>[This event is raised on the NIN_BALLOONSHOW or NIN_BALLOONHIDE message.]</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.NotifyIconOnBalloonTipShownDescr))]
        public event EventHandler BalloonTipShown {
            add {
                Events.AddHandler(EVENT_BALLOONTIPSHOWN, value);
            }
            remove {
                Events.RemoveHandler(EVENT_BALLOONTIPSHOWN, value);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.ContextMenu"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets context menu
        ///       for the tray icon.
        ///    </para>
        /// </devdoc>
        [
        Browsable(false),
        DefaultValue(null),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.NotifyIconMenuDescr))
        ]
        public ContextMenu ContextMenu {
            get {
                return contextMenu;
            }

            set {
                this.contextMenu = value;
            }
        }

        [
        DefaultValue(null),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.NotifyIconMenuDescr))
        ]
        public ContextMenuStrip ContextMenuStrip {
            get {
                return contextMenuStrip;
            }
    
            set {
                this.contextMenuStrip = value;
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.Icon"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the current
        ///       icon.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(null),
        SRDescription(nameof(SR.NotifyIconIconDescr))
        ]
        public Icon Icon {
            get {
                return icon;
            }
            set {
                if (icon != value) {
                    this.icon = value;
                    UpdateIcon(visible);
                }
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.Text"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the ToolTip text displayed when
        ///       the mouse hovers over a system tray icon.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(""),
        SRDescription(nameof(SR.NotifyIconTextDescr)),
        Editor("System.ComponentModel.Design.MultilineStringEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string Text {
            get {
                return text;
            }
            set {
                if (value == null) value = "";
                if (value != null && !value.Equals(this.text)) {
                    if (value != null && value.Length > 63) {
                        throw new ArgumentOutOfRangeException(nameof(Text), value, string.Format(SR.TrayIcon_TextTooLong));
                    }
                    this.text = value;
                    if (added) {
                        UpdateIcon(true);
                    }
                }
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.Visible"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the icon is visible in the Windows System Tray.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        DefaultValue(false),
        SRDescription(nameof(SR.NotifyIconVisDescr))
        ]
        public bool Visible {
            get {
                return visible;
            }
            set {
                if (visible != value) {
                    UpdateIcon(value);
                    visible = value;
                }
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="TrayIcon.Tag"]/*' />
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

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.Click"]/*' />
        /// <devdoc>
        ///     Occurs when the user clicks the icon in the system tray.
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ControlOnClickDescr))]
        public event EventHandler Click {
            add {
                Events.AddHandler(EVENT_CLICK, value);
            }
            remove {
                Events.RemoveHandler(EVENT_CLICK, value);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.DoubleClick"]/*' />
        /// <devdoc>
        ///     Occurs when the user double-clicks the icon in the system tray.
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ControlOnDoubleClickDescr))]
        public event EventHandler DoubleClick {
            add {
                Events.AddHandler(EVENT_DOUBLECLICK, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DOUBLECLICK, value);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.MouseClick"]/*' />
        /// <devdoc>
        ///     Occurs when the user clicks the icon in the system tray.
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.NotifyIconMouseClickDescr))]
        public event MouseEventHandler MouseClick {
            add {
                Events.AddHandler(EVENT_MOUSECLICK, value);
            }
            remove {
                Events.RemoveHandler(EVENT_MOUSECLICK, value);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.MouseDoubleClick"]/*' />
        /// <devdoc>
        ///     Occurs when the user mouse double clicks the icon in the system tray.
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.NotifyIconMouseDoubleClickDescr))]
        public event MouseEventHandler MouseDoubleClick {
            add {
                Events.AddHandler(EVENT_MOUSEDOUBLECLICK, value);
            }
            remove {
                Events.RemoveHandler(EVENT_MOUSEDOUBLECLICK, value);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.MouseDown"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs when the
        ///       user presses a mouse button while the pointer is over the icon in the system tray.
        ///    </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseDownDescr))]
        public event MouseEventHandler MouseDown {
            add {
                Events.AddHandler(EVENT_MOUSEDOWN, value);
            }
            remove {
                Events.RemoveHandler(EVENT_MOUSEDOWN, value);
            }
        }        

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.MouseMove"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs
        ///       when the user moves the mouse pointer over the icon in the system tray.
        ///    </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseMoveDescr))]
        public event MouseEventHandler MouseMove {
            add {
                Events.AddHandler(EVENT_MOUSEMOVE, value);
            }
            remove {
                Events.RemoveHandler(EVENT_MOUSEMOVE, value);
            }
        }        

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.MouseUp"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs when the
        ///       user releases the mouse button while the pointer
        ///       is over the icon in the system tray.
        ///    </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseUpDescr))]
        public event MouseEventHandler MouseUp {
            add {
                Events.AddHandler(EVENT_MOUSEUP, value);
            }
            remove {
                Events.RemoveHandler(EVENT_MOUSEUP, value);
            }
        }        

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.Dispose"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Disposes of the resources (other than memory) used by the
        ///    <see cref='System.Windows.Forms.NotifyIcon'/>.
        ///    </para>
        /// </devdoc>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (window != null) {
                    this.icon = null;
                    this.Text = String.Empty;
                    UpdateIcon(false);
                    window.DestroyHandle();
                    window = null;
                    contextMenu = null;
                    contextMenuStrip = null;
                }
            }
            else {
                // This same post is done in ControlNativeWindow's finalize method, so if you change
                // it, change it there too.
                //
                if (window != null && window.Handle != IntPtr.Zero) {
                    UnsafeNativeMethods.PostMessage(new HandleRef(window, window.Handle), NativeMethods.WM_CLOSE, 0, 0);
                    window.ReleaseHandle();
                }
            }
            base.Dispose(disposing);
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.BalloonTipClicked"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This method raised the BalloonTipClicked event. 
        ///    </para>
        /// </devdoc>
        private void OnBalloonTipClicked() {
            EventHandler handler = (EventHandler)Events[EVENT_BALLOONTIPCLICKED];
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnBalloonTipClosed"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This method raised the BalloonTipClosed event. 
        ///    </para>
        /// </devdoc>
        private void OnBalloonTipClosed() {
            EventHandler handler = (EventHandler)Events[EVENT_BALLOONTIPCLOSED];
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnBalloonTipShown"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This method raised the BalloonTipShown event. 
        ///    </para>
        /// </devdoc>
        private void OnBalloonTipShown() {
            EventHandler handler = (EventHandler)Events[EVENT_BALLOONTIPSHOWN];
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnClick"]/*' />
        /// <devdoc>
        ///    <para>
        ///       This method actually raises the Click event. Inheriting classes should
        ///       override this if they wish to be notified of a Click event. (This is far
        ///       preferable to actually adding an event handler.) They should not,
        ///       however, forget to call base.onClick(e); before exiting, to ensure that
        ///       other recipients do actually get the event.
        ///    </para>
        /// </devdoc>
        private void OnClick(EventArgs e) {
            EventHandler handler = (EventHandler) Events[ EVENT_CLICK ];
            if (handler != null)
                handler( this, e );
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnDoubleClick"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.onDoubleClick to send this event to any registered event listeners.
        /// </devdoc>
        private void OnDoubleClick(EventArgs e) {
            EventHandler handler = (EventHandler) Events[ EVENT_DOUBLECLICK ];
            if (handler != null)
                handler( this, e );
        }


        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnMouseClick"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.OnMouseClick to send this event to any registered event listeners.
        /// </devdoc>
        private void OnMouseClick(MouseEventArgs mea) {
            MouseEventHandler handler = (MouseEventHandler) Events[ EVENT_MOUSECLICK ];
            if (handler != null)
                handler( this, mea );
        }
        
        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnMouseDoubleClick"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.OnMouseDoubleClick to send this event to any registered event listeners.
        /// </devdoc>
        private void OnMouseDoubleClick(MouseEventArgs mea) {
            MouseEventHandler handler = (MouseEventHandler) Events[ EVENT_MOUSEDOUBLECLICK ];
            if (handler != null)
                handler( this, mea );
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnMouseDown"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.NotifyIcon.MouseDown'/> event.
        ///       Inheriting classes should override this method to handle this event.
        ///       Call base.onMouseDown to send this event to any registered event listeners.
        ///       
        ///    </para>
        /// </devdoc>
        private void OnMouseDown(MouseEventArgs e) {
            MouseEventHandler handler = (MouseEventHandler)Events[EVENT_MOUSEDOWN];
            if (handler != null)
                handler(this, e);
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnMouseMove"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Inheriting classes should override this method to handle this event.
        ///       Call base.onMouseMove to send this event to any registered event listeners.
        ///       
        ///    </para>
        /// </devdoc>
        private void OnMouseMove(MouseEventArgs e) {
            MouseEventHandler handler = (MouseEventHandler)Events[EVENT_MOUSEMOVE];
            if (handler != null)
                handler(this, e);
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.OnMouseUp"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Inheriting classes should override this method to handle this event.
        ///       Call base.onMouseUp to send this event to any registered event listeners.
        ///    </para>
        /// </devdoc>
        private void OnMouseUp(MouseEventArgs e) {
            MouseEventHandler handler = (MouseEventHandler)Events[EVENT_MOUSEUP];
            if (handler != null)
                handler(this, e);
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.ShowBalloonTip"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a balloon tooltip in the taskbar.
        /// 
        ///       The system enforces minimum and maximum timeout values. Timeout 
        ///       values that are too large are set to the maximum value and values 
        ///       that are too small default to the minimum value. The operating system's 
        ///       default minimum and maximum timeout values are 10 seconds and 30 seconds, 
        ///       respectively.
        ///       
        ///       No more than one balloon ToolTip at at time is displayed for the taskbar. 
        ///       If an application attempts to display a ToolTip when one is already being displayed, 
        ///       the ToolTip will not appear until the existing balloon ToolTip has been visible for at 
        ///       least the system minimum timeout value. For example, a balloon ToolTip with timeout 
        ///       set to 30 seconds has been visible for seven seconds when another application attempts 
        ///       to display a balloon ToolTip. If the system minimum timeout is ten seconds, the first 
        ///       ToolTip displays for an additional three seconds before being replaced by the second ToolTip.
        ///    </para>
        /// </devdoc>
        public void ShowBalloonTip(int timeout) {
            ShowBalloonTip(timeout, this.balloonTipTitle, this.balloonTipText, this.balloonTipIcon);
        }


        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.ShowBalloonTip"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a balloon tooltip in the taskbar with the specified title,
        ///       text, and icon for a duration of the specified timeout value.
        /// 
        ///       The system enforces minimum and maximum timeout values. Timeout 
        ///       values that are too large are set to the maximum value and values 
        ///       that are too small default to the minimum value. The operating system's 
        ///       default minimum and maximum timeout values are 10 seconds and 30 seconds, 
        ///       respectively.
        ///       
        ///       No more than one balloon ToolTip at at time is displayed for the taskbar. 
        ///       If an application attempts to display a ToolTip when one is already being displayed, 
        ///       the ToolTip will not appear until the existing balloon ToolTip has been visible for at 
        ///       least the system minimum timeout value. For example, a balloon ToolTip with timeout 
        ///       set to 30 seconds has been visible for seven seconds when another application attempts 
        ///       to display a balloon ToolTip. If the system minimum timeout is ten seconds, the first 
        ///       ToolTip displays for an additional three seconds before being replaced by the second ToolTip.
        ///    </para>
        /// </devdoc>
        public void ShowBalloonTip(int timeout, string tipTitle, string tipText, ToolTipIcon tipIcon) {

            if (timeout < 0) {
               throw new ArgumentOutOfRangeException(nameof(timeout), string.Format(SR.InvalidArgument, "timeout", (timeout).ToString(CultureInfo.CurrentCulture)));
            }

            if (string.IsNullOrEmpty(tipText))
            {
                throw new ArgumentException(SR.NotifyIconEmptyOrNullTipText);
            }

            //valid values are 0x0 to 0x3
            if (!ClientUtils.IsEnumValid(tipIcon, (int)tipIcon, (int)ToolTipIcon.None, (int)ToolTipIcon.Error)){
                throw new InvalidEnumArgumentException(nameof(tipIcon), (int)tipIcon, typeof(ToolTipIcon));
            }
            
            
           if (added ) {
                // Bail if in design mode...
                if (DesignMode) {
                   return;
                }
                IntSecurity.UnrestrictedWindows.Demand();

                NativeMethods.NOTIFYICONDATA data = new NativeMethods.NOTIFYICONDATA();
                if (window.Handle == IntPtr.Zero) {
                   window.CreateHandle(new CreateParams());
                }
                data.hWnd = window.Handle;
                data.uID = id;
                data.uFlags = NativeMethods.NIF_INFO;
                data.uTimeoutOrVersion = timeout;
                data.szInfoTitle = tipTitle;
                data.szInfo = tipText;
                switch (tipIcon) {
                   case ToolTipIcon.Info: data.dwInfoFlags = NativeMethods.NIIF_INFO; break;
                   case ToolTipIcon.Warning: data.dwInfoFlags = NativeMethods.NIIF_WARNING; break;
                   case ToolTipIcon.Error: data.dwInfoFlags = NativeMethods.NIIF_ERROR; break;
                   case ToolTipIcon.None: data.dwInfoFlags = NativeMethods.NIIF_NONE; break;
                }
                UnsafeNativeMethods.Shell_NotifyIcon(NativeMethods.NIM_MODIFY, data);
           }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.ShowContextMenu"]/*' />
        /// <devdoc>
        ///     Shows the context menu for the tray icon.
        /// </devdoc>
        /// <internalonly/>
        private void ShowContextMenu() {

            if (contextMenu != null || contextMenuStrip != null) {
                NativeMethods.POINT pt = new NativeMethods.POINT();
                UnsafeNativeMethods.GetCursorPos(pt);
                                
                // Summary: the current window must be made the foreground window
                // before calling TrackPopupMenuEx, and a task switch must be
                // forced after the call.
                UnsafeNativeMethods.SetForegroundWindow(new HandleRef(window, window.Handle));

                if (contextMenu != null) {
                    contextMenu.OnPopup( EventArgs.Empty );

                    SafeNativeMethods.TrackPopupMenuEx(new HandleRef(contextMenu, contextMenu.Handle),
                                             NativeMethods.TPM_VERTICAL | NativeMethods.TPM_RIGHTALIGN,
                                             pt.x,
                                             pt.y,
                                             new HandleRef(window, window.Handle),
                                             null);

                    // Force task switch (see above)
                    UnsafeNativeMethods.PostMessage(new HandleRef(window, window.Handle), NativeMethods.WM_NULL, IntPtr.Zero, IntPtr.Zero);
                }
                else if (contextMenuStrip != null) {
                    // this will set the context menu strip to be toplevel
                    // and will allow us to overlap the system tray
                    contextMenuStrip.ShowInTaskbar(pt.x, pt.y);
                }
            }
        }
    
        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.UpdateIcon"]/*' />
        /// <devdoc>
        ///     Updates the icon in the system tray.
        /// </devdoc>
        /// <internalonly/>
        private void UpdateIcon(bool showIconInTray) {
            lock(syncObj) {

                // Bail if in design mode...
                //
                if (DesignMode) {
                    return;
                }

                IntSecurity.UnrestrictedWindows.Demand();

                window.LockReference(showIconInTray);

                NativeMethods.NOTIFYICONDATA data = new NativeMethods.NOTIFYICONDATA();
                data.uCallbackMessage = WM_TRAYMOUSEMESSAGE;
                data.uFlags = NativeMethods.NIF_MESSAGE;
                if (showIconInTray) {
                    if (window.Handle == IntPtr.Zero) {
                        window.CreateHandle(new CreateParams());
                    }
                }
                data.hWnd = window.Handle;
                data.uID = id;
                data.hIcon = IntPtr.Zero;
                data.szTip = null;
                if (icon != null) {
                    data.uFlags |= NativeMethods.NIF_ICON;
                    data.hIcon = icon.Handle;
                }
                data.uFlags |= NativeMethods.NIF_TIP;
                data.szTip = text;

                if (showIconInTray && icon != null) {
                    if (!added) {
                        UnsafeNativeMethods.Shell_NotifyIcon(NativeMethods.NIM_ADD, data);
                        added = true;
                    }
                    else {
                        UnsafeNativeMethods.Shell_NotifyIcon(NativeMethods.NIM_MODIFY, data);
                    }
                }
                else if (added) {
                    UnsafeNativeMethods.Shell_NotifyIcon(NativeMethods.NIM_DELETE, data);
                    added = false;
                }
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.WmMouseDown"]/*' />
        /// <devdoc>
        ///     Handles the mouse-down event
        /// </devdoc>
        /// <internalonly/>
        private void WmMouseDown(ref Message m, MouseButtons button, int clicks) {
            if (clicks == 2) {
                OnDoubleClick(new MouseEventArgs(button, 2, 0, 0, 0));
                OnMouseDoubleClick(new MouseEventArgs(button, 2, 0, 0, 0));
                doubleClick = true;
            }
            OnMouseDown(new MouseEventArgs(button, clicks, 0, 0, 0));
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.WmMouseMove"]/*' />
        /// <devdoc>
        ///     Handles the mouse-move event
        /// </devdoc>
        /// <internalonly/>
        private void WmMouseMove(ref Message m) {
            OnMouseMove(new MouseEventArgs(Control.MouseButtons, 0, 0, 0, 0));
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.WmMouseUp"]/*' />
        /// <devdoc>
        ///     Handles the mouse-up event
        /// </devdoc>
        /// <internalonly/>
        private void WmMouseUp(ref Message m, MouseButtons button) {
            OnMouseUp(new MouseEventArgs(button, 0, 0, 0, 0));
            //subhag
            if(!doubleClick) {
               OnClick(new MouseEventArgs(button, 0, 0, 0, 0));
               OnMouseClick(new MouseEventArgs(button, 0, 0, 0, 0));
            }
            doubleClick = false;
        }

        private void WmTaskbarCreated(ref Message m) {
            added = false;
            UpdateIcon(visible);
        }

        private void WndProc(ref Message msg) {

            switch (msg.Msg) {
                case WM_TRAYMOUSEMESSAGE:
                    switch ((int)msg.LParam) {
                        case NativeMethods.WM_LBUTTONDBLCLK:
                            WmMouseDown(ref msg, MouseButtons.Left, 2);
                            break;
                        case NativeMethods.WM_LBUTTONDOWN:
                            WmMouseDown(ref msg, MouseButtons.Left, 1);
                            break;
                        case NativeMethods.WM_LBUTTONUP:
                            WmMouseUp(ref msg, MouseButtons.Left);
                            break;
                        case NativeMethods.WM_MBUTTONDBLCLK:
                            WmMouseDown(ref msg, MouseButtons.Middle, 2);
                            break;
                        case NativeMethods.WM_MBUTTONDOWN:
                            WmMouseDown(ref msg, MouseButtons.Middle, 1);
                            break;
                        case NativeMethods.WM_MBUTTONUP:
                            WmMouseUp(ref msg, MouseButtons.Middle);
                            break;
                        case NativeMethods.WM_MOUSEMOVE:
                            WmMouseMove(ref msg);
                            break;
                        case NativeMethods.WM_RBUTTONDBLCLK:
                            WmMouseDown(ref msg, MouseButtons.Right, 2);
                            break;
                        case NativeMethods.WM_RBUTTONDOWN:
                            WmMouseDown(ref msg, MouseButtons.Right, 1);
                            break;
                        case NativeMethods.WM_RBUTTONUP:
                            if (contextMenu != null || contextMenuStrip != null) {
                                ShowContextMenu();
                            }
                            WmMouseUp(ref msg, MouseButtons.Right);
                            break;
                        case NativeMethods.NIN_BALLOONSHOW:
                            OnBalloonTipShown();
                            break;
                        case NativeMethods.NIN_BALLOONHIDE:
                            OnBalloonTipClosed();
                            break;
                        case NativeMethods.NIN_BALLOONTIMEOUT:
                            OnBalloonTipClosed();
                            break;
                        case NativeMethods.NIN_BALLOONUSERCLICK:
                            OnBalloonTipClicked();
                            break;
                    }
                    break;
                case NativeMethods.WM_COMMAND:
                    if (IntPtr.Zero == msg.LParam) {
                        if (Command.DispatchID((int)msg.WParam & 0xFFFF)) return;
                    }
                    else {
                        window.DefWndProc(ref msg);
                    }
                    break;
                case NativeMethods.WM_DRAWITEM:
                    // If the wparam is zero, then the message was sent by a menu.
                    // See WM_DRAWITEM in MSDN.
                    if (msg.WParam == IntPtr.Zero) {
                        WmDrawItemMenuItem(ref msg);
                    }
                    break;
                case NativeMethods.WM_MEASUREITEM:
                    // If the wparam is zero, then the message was sent by a menu.
                    if (msg.WParam == IntPtr.Zero) {
                        WmMeasureMenuItem(ref msg);
                    }
                    break;
                    
                case NativeMethods.WM_INITMENUPOPUP:
                    WmInitMenuPopup(ref msg);
                    break;

                case NativeMethods.WM_DESTROY:
                    // Remove the icon from the taskbar
                    UpdateIcon(false);
                    break;

                default:
                    if (msg.Msg == WM_TASKBARCREATED) {
                        WmTaskbarCreated(ref msg);
                    }
                    window.DefWndProc(ref msg);
                    break;
            }
        }

        private void WmInitMenuPopup(ref Message m) {
            if (contextMenu != null) {
                if (contextMenu.ProcessInitMenuPopup(m.WParam)) {
                    return;
                }
            }

            window.DefWndProc(ref m);
        }

        private void WmMeasureMenuItem(ref Message m) {
            // Obtain the menu item object
            NativeMethods.MEASUREITEMSTRUCT mis = (NativeMethods.MEASUREITEMSTRUCT)m.GetLParam(typeof(NativeMethods.MEASUREITEMSTRUCT));

            Debug.Assert(m.LParam != IntPtr.Zero, "m.lparam is null");

            // A pointer to the correct MenuItem is stored in the measure item
            // information sent with the message.
            // (See MenuItem.CreateMenuItemInfo)
            MenuItem menuItem = MenuItem.GetMenuItemFromItemData(mis.itemData);
            Debug.Assert(menuItem != null, "UniqueID is not associated with a menu item");

            // Delegate this message to the menu item
            if (menuItem != null) {
                menuItem.WmMeasureItem(ref m);
            }
        }

        private void WmDrawItemMenuItem(ref Message m) {
            // Obtain the menu item object
            NativeMethods.DRAWITEMSTRUCT dis = (NativeMethods.DRAWITEMSTRUCT)m.GetLParam(typeof(NativeMethods.DRAWITEMSTRUCT));

            // A pointer to the correct MenuItem is stored in the draw item
            // information sent with the message.
            // (See MenuItem.CreateMenuItemInfo)
            MenuItem menuItem = MenuItem.GetMenuItemFromItemData(dis.itemData);

            // Delegate this message to the menu item
            if (menuItem != null) {
                menuItem.WmDrawItem(ref m);
            }
        }

        /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.NotifyIconNativeWindow"]/*' />
        /// <devdoc>
        ///     Defines a placeholder window that the NotifyIcon is attached to.
        /// </devdoc>
        /// <internalonly/>
        private class NotifyIconNativeWindow : NativeWindow {
            internal NotifyIcon reference;
            private  GCHandle   rootRef;   // We will root the control when we do not want to be elligible for garbage collection.

            /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.NotifyIconNativeWindow.NotifyIconNativeWindow"]/*' />
            /// <devdoc>
            ///     Create a new NotifyIcon, and bind the window to the NotifyIcon component.
            /// </devdoc>
            /// <internalonly/>
            internal NotifyIconNativeWindow(NotifyIcon component) {
                reference = component;
            }

            ~NotifyIconNativeWindow() {
                // This same post is done in Control's Dispose method, so if you change
                // it, change it there too.
                //
                if (Handle != IntPtr.Zero) {
                    UnsafeNativeMethods.PostMessage(new HandleRef(this, Handle), NativeMethods.WM_CLOSE, 0, 0);
                }
                
                // This releases the handle from our window proc, re-routing it back to
                // the system.
            }

            public void LockReference(bool locked) {
                if (locked) {
                    if (!rootRef.IsAllocated) {
                        rootRef = GCHandle.Alloc(reference, GCHandleType.Normal);
                    }
                }
                else {
                    if (rootRef.IsAllocated) {
                        rootRef.Free();
                    }
                }
            }

            protected override void OnThreadException(Exception e) {
                Application.OnThreadException(e);
            }

            /// <include file='doc\TrayIcon.uex' path='docs/doc[@for="NotifyIcon.NotifyIconNativeWindow.WndProc"]/*' />
            /// <devdoc>
            ///     Pass messages on to the NotifyIcon object's wndproc handler.
            /// </devdoc>
            /// <internalonly/>
            protected override void WndProc(ref Message m) {
                Debug.Assert(reference != null, "NotifyIcon was garbage collected while it was still visible.  How did we let that happen?");
                reference.WndProc(ref m);
            }
        }
    }
}

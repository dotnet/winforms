// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public static partial class ToolStripManager
{
    ///  <remarks>
    ///   <para>
    ///    This installs a message filter when a dropdown becomes active. The filter:
    ///   </para>
    ///   <list type="bullet">
    ///    <item>
    ///     <description>
    ///      Eats WM_MOUSEMOVEs so that the window underneath doesn't get highlight processing/tooltips.
    ///     </description>
    ///    </item>
    ///    <item><description>Dismisses the menu if clicked outside the dropdown.</description></item>
    ///    <item><description>Dismisses all dropdowns if the active window changes.</description></item>
    ///    <item><description>Redirects keyboard messages to the active dropdown.</description></item>
    ///   </list>
    ///   <para>
    ///    There should be one Message Filter per thread and it should be uninstalled once the last dropdown has gone away.
    ///    This is not part of <see cref="ToolStripManager"/> because it is DropDown specific and
    ///    we don't want to publicly expose this message filter.
    ///   </para>
    ///  </remarks>
    internal partial class ModalMenuFilter : IMessageModifyAndFilter
    {
        // The window that was active when we showed the dropdown
        private HandleRef<HWND> _activeHwnd;
        // The window that was last known to be active
        private HandleRef<HWND> _lastActiveWindow;
        private List<ToolStrip>? _inputFilterQueue;
        private bool _inMenuMode;
        private bool _caretHidden;
        private bool _showUnderlines;
        private bool _menuKeyToggle;
        private bool _suspendMenuMode;
        private HostedWindowsFormsMessageHook? _messageHook;
        private Timer? _ensureMessageProcessingTimer;
        private const int MessageProcessingInterval = 500;

        private ToolStrip? _toplevelToolStrip;

        private readonly WeakReference<IKeyboardToolTip?> _lastFocusedTool = new(null);

        [ThreadStatic]
        private static ModalMenuFilter? t_instance;

        internal static ModalMenuFilter Instance => t_instance ??= new ModalMenuFilter();

        private ModalMenuFilter()
        {
        }

        /// <summary>
        ///  The HWnd that was active when we popped the first dropdown.
        /// </summary>
        internal static HandleRef<HWND> ActiveHwnd => Instance.ActiveHwndInternal;

        // returns whether or not we should show focus cues for mnemonics.
        public bool ShowUnderlines
        {
            get => _showUnderlines;
            set
            {
                if (_showUnderlines != value)
                {
                    _showUnderlines = value;
                    NotifyMenuModeChange(invalidateText: true, activationChange: false);
                }
            }
        }

        private HandleRef<HWND> ActiveHwndInternal
        {
            get => _activeHwnd;
            set
            {
                if (_activeHwnd.Handle != value.Handle)
                {
                    Control? control = null;

                    // Unsubscribe from handle recreate.
                    if (_activeHwnd.Handle != IntPtr.Zero)
                    {
                        control = Control.FromHandle(_activeHwnd.Handle);
                        if (control is not null)
                        {
                            control.HandleCreated -= OnActiveHwndHandleCreated;
                        }
                    }

                    _activeHwnd = value;

                    // make sure we watch out for handle recreates.
                    control = Control.FromHandle(_activeHwnd.Handle);
                    if (control is not null)
                    {
                        control.HandleCreated += OnActiveHwndHandleCreated;
                    }
                }
            }
        }

        /// <summary>
        ///  Returns whether or not someone has called EnterMenuMode.
        /// </summary>
        internal static bool InMenuMode => Instance._inMenuMode;

        internal static bool MenuKeyToggle
        {
            get => Instance._menuKeyToggle;
            set
            {
                if (Instance._menuKeyToggle != value)
                {
                    Instance._menuKeyToggle = value;
                }
            }
        }

        /// <summary>
        ///  Used in scenarios where windows forms does not own the message pump,
        ///  but needs access to the message queue.
        /// </summary>
        private HostedWindowsFormsMessageHook MessageHook
            => _messageHook ??= new HostedWindowsFormsMessageHook();

        // ToolStrip analog to WM_ENTERMENULOOP
        private void EnterMenuModeCore()
        {
            Debug.Assert(!InMenuMode, "How did we get here if we're already in menu mode?");

            if (!InMenuMode)
            {
                HWND hwndActive = PInvoke.GetActiveWindow();
                if (!hwndActive.IsNull)
                {
                    ActiveHwndInternal = new(Control.FromHandle(hwndActive), hwndActive);
                }

                Application.ThreadContext.FromCurrent().AddMessageFilter(this);
                Application.ThreadContext.FromCurrent().TrackInput(true);

                if (!Application.ThreadContext.FromCurrent().GetMessageLoop(true))
                {
                    // Message filter isn't going to help as we don't own the message pump
                    // switch over to a MessageHook
                    MessageHook.HookMessages = true;
                }

                _inMenuMode = true;

                NotifyLastLastFocusedToolAboutFocusLoss();

                // Fire timer messages to force our filter to get evaluated.
                ProcessMessages(true);
            }
        }

        internal void NotifyLastLastFocusedToolAboutFocusLoss()
        {
            IKeyboardToolTip? lastFocusedTool = KeyboardToolTipStateMachine.Instance.LastFocusedTool;
            if (lastFocusedTool is not null)
            {
                _lastFocusedTool.SetTarget(lastFocusedTool);
                KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(lastFocusedTool);
            }
        }

        internal static void ExitMenuMode() => Instance.ExitMenuModeCore();

        private void ExitMenuModeCore()
        {
            // Ensure we've cleaned up the timer.
            ProcessMessages(process: false);

            if (!InMenuMode)
            {
                return;
            }

            try
            {
                if (_messageHook is not null)
                {
                    // Message filter isn't going to help as we don't own the message pump
                    // Switch over to a MessageHook
                    _messageHook.HookMessages = false;
                }

                Application.ThreadContext.FromCurrent().RemoveMessageFilter(this);
                Application.ThreadContext.FromCurrent().TrackInput(false);

                if (!ActiveHwnd.Handle.IsNull)
                {
                    // Unsubscribe from handle creates
                    Control? control = Control.FromHandle(ActiveHwnd.Handle);
                    if (control is not null)
                    {
                        control.HandleCreated -= OnActiveHwndHandleCreated;
                    }

                    ActiveHwndInternal = default;
                }

                _inputFilterQueue?.Clear();
                if (_caretHidden)
                {
                    _caretHidden = false;
                    PInvoke.ShowCaret(HWND.Null);
                }

                if (_lastFocusedTool.TryGetTarget(out IKeyboardToolTip? tool) && tool is not null)
                {
                    KeyboardToolTipStateMachine.Instance.NotifyAboutGotFocus(tool);
                }
            }
            finally
            {
                _inMenuMode = false;

                // Skip the setter here so we only iterate through the toolstrips once.
                bool textStyleChanged = _showUnderlines;
                _showUnderlines = false;
                NotifyMenuModeChange(invalidateText: textStyleChanged, activationChange: true);
            }
        }

        internal static ToolStrip? GetActiveToolStrip() => Instance.GetActiveToolStripInternal();

        internal ToolStrip? GetActiveToolStripInternal()
        {
            if (_inputFilterQueue is not null && _inputFilterQueue.Count > 0)
            {
                return _inputFilterQueue[^1];
            }

            return null;
        }

        /// <summary>
        ///  Returns the ToolStrip that is at the root.
        /// </summary>
        private ToolStrip? GetCurrentTopLevelToolStrip()
        {
            if (_toplevelToolStrip is null)
            {
                ToolStrip? activeToolStrip = GetActiveToolStripInternal();
                if (activeToolStrip is not null)
                {
                    _toplevelToolStrip = activeToolStrip.GetToplevelOwnerToolStrip();
                }
            }

            return _toplevelToolStrip;
        }

        private void OnActiveHwndHandleCreated(object? sender, EventArgs e)
        {
            ActiveHwndInternal = new(sender as Control);
        }

        internal static void ProcessMenuKeyDown(ref Message m)
        {
            Keys keyData = (Keys)(nint)m.WParamInternal;

            if (Control.FromHandle(m.HWnd) is ToolStrip toolStrip && !toolStrip.IsDropDown)
            {
                return;
            }

            // Handle the case where the ALT key has been pressed down while a dropdown was open. We need to clear
            // off the MenuKeyToggle so the next ALT will activate the menu.
            if (IsMenuKey(keyData))
            {
                if (!InMenuMode && MenuKeyToggle)
                {
                    MenuKeyToggle = false;
                }
                else if (!MenuKeyToggle)
                {
                    Instance.ShowUnderlines = true;
                }
            }
        }

        internal static void CloseActiveDropDown(ToolStripDropDown activeToolStripDropDown, ToolStripDropDownCloseReason reason)
        {
            activeToolStripDropDown.SetCloseReason(reason);
            activeToolStripDropDown.Visible = false;

            // There's no more dropdowns left in the chain
            if (GetActiveToolStrip() is null)
            {
                ExitMenuMode();

                // Make sure we roll selection off  the toplevel toolstrip.
                activeToolStripDropDown.OwnerItem?.Unselect();
            }
        }

        /// <summary>
        ///  Fire a timer event to ensure we have a message in the queue every 500ms
        /// </summary>
        private void ProcessMessages(bool process)
        {
            if (process)
            {
                _ensureMessageProcessingTimer ??= new Timer();
                _ensureMessageProcessingTimer.Interval = MessageProcessingInterval;
                _ensureMessageProcessingTimer.Enabled = true;
            }
            else if (_ensureMessageProcessingTimer is not null)
            {
                _ensureMessageProcessingTimer.Enabled = false;
                _ensureMessageProcessingTimer.Dispose();
                _ensureMessageProcessingTimer = null;
            }
        }

        private void ProcessMouseButtonPressed(HWND hwndMouseMessageIsFrom, Point location)
        {
            int countDropDowns = _inputFilterQueue?.Count ?? 0;
            for (int i = 0; i < countDropDowns; i++)
            {
                ToolStrip? activeToolStrip = GetActiveToolStripInternal();

                if (activeToolStrip is not null)
                {
                    Point translatedLocation = location;
                    PInvokeCore.MapWindowPoints(hwndMouseMessageIsFrom, activeToolStrip, ref translatedLocation);
                    if (!activeToolStrip.ClientRectangle.Contains(translatedLocation))
                    {
                        if (activeToolStrip is ToolStripDropDown activeToolStripDropDown)
                        {
                            if (!(activeToolStripDropDown.OwnerToolStrip is not null
                                && activeToolStripDropDown.OwnerToolStrip.HWND == hwndMouseMessageIsFrom
                                && activeToolStripDropDown.OwnerDropDownItem is not null
                                && activeToolStripDropDown.OwnerDropDownItem.DropDownButtonArea.Contains(location)))
                            {
                                // The owner item should handle closing the dropdown
                                // this allows code such as if (DropDown.Visible) { Hide, Show } etc.
                                CloseActiveDropDown(activeToolStripDropDown, ToolStripDropDownCloseReason.AppClicked);
                            }
                        }
                        else
                        {
                            // Make sure we clear the selection.
                            activeToolStrip.NotifySelectionChange(item: null);

                            // We're a toplevel toolstrip and we've clicked somewhere else. Exit menu mode.
                            ExitMenuModeCore();
                        }
                    }
                    else
                    {
                        // We've found a dropdown that intersects with the mouse message
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        private bool ProcessActivationChange()
        {
            int countDropDowns = _inputFilterQueue?.Count ?? 0;
            for (int i = 0; i < countDropDowns; i++)
            {
                if (GetActiveToolStripInternal() is ToolStripDropDown activeDropDown && activeDropDown.AutoClose)
                {
                    activeDropDown.Visible = false;
                }
            }

            ExitMenuModeCore();
            return true;
        }

        internal static void SetActiveToolStrip(ToolStrip toolStrip, bool menuKeyPressed)
        {
            if (!InMenuMode && menuKeyPressed)
            {
                Instance.ShowUnderlines = true;
            }

            Instance.SetActiveToolStripCore(toolStrip);
        }

        internal static void SetActiveToolStrip(ToolStrip toolStrip)
            => Instance.SetActiveToolStripCore(toolStrip);

        private void SetActiveToolStripCore(ToolStrip toolStrip)
        {
            if (toolStrip is null)
            {
                return;
            }

            if (toolStrip.IsDropDown)
            {
                // For something that never closes, don't use menu mode.
                ToolStripDropDown dropDown = (ToolStripDropDown)toolStrip;

                if (!dropDown.AutoClose)
                {
                    // Store off the current active hwnd
                    HWND hwndActive = PInvoke.GetActiveWindow();
                    if (!hwndActive.IsNull)
                    {
                        ActiveHwndInternal = new(Control.FromHandle(hwndActive), hwndActive);
                    }

                    // Don't actually enter menu mode.
                    return;
                }
            }

            toolStrip.KeyboardActive = true;

            if (_inputFilterQueue is null)
            {
                // Use list because we want to be able to remove at any point.
                _inputFilterQueue = [];
            }
            else
            {
                ToolStrip? currentActiveToolStrip = GetActiveToolStripInternal();

                // ToolStrip dropdowns push/pull their activation based on visibility.
                // we have to account for the toolstrips that aren't dropdowns
                if (currentActiveToolStrip is not null)
                {
                    if (!currentActiveToolStrip.IsDropDown)
                    {
                        _inputFilterQueue.Remove(currentActiveToolStrip);
                    }
                    else if (toolStrip.IsDropDown
                        && (ToolStripDropDown.GetFirstDropDown(toolStrip)
                            != ToolStripDropDown.GetFirstDropDown(currentActiveToolStrip)))
                    {
                        _inputFilterQueue.Remove(currentActiveToolStrip);

                        ToolStripDropDown currentActiveToolStripDropDown = (ToolStripDropDown)currentActiveToolStrip;
                        currentActiveToolStripDropDown.DismissAll();
                    }
                }
            }

            // Reset the toplevel toolstrip
            _toplevelToolStrip = null;

            if (!_inputFilterQueue.Contains(toolStrip))
            {
                _inputFilterQueue.Add(toolStrip);
            }

            if (!InMenuMode && _inputFilterQueue.Count > 0)
            {
                EnterMenuModeCore();
            }

            // Hide the caret if we're showing a toolstrip dropdown
            if (!_caretHidden && toolStrip.IsDropDown && InMenuMode)
            {
                _caretHidden = true;
                PInvoke.HideCaret(HWND.Null);
            }
        }

        internal static void SuspendMenuMode()
        {
            Instance._suspendMenuMode = true;
        }

        internal static void ResumeMenuMode()
        {
            Instance._suspendMenuMode = false;
        }

        internal static void RemoveActiveToolStrip(ToolStrip toolStrip)
        {
            Instance.RemoveActiveToolStripCore(toolStrip);
        }

        private void RemoveActiveToolStripCore(ToolStrip toolStrip)
        {
            // Precautionary - remove the active toplevel toolstrip.
            _toplevelToolStrip = null;
            _inputFilterQueue?.Remove(toolStrip);
        }

        private static bool IsChildOrSameWindow<T>(in T hwndParent, in T hwndChild) where T : IHandle<HWND>
            => hwndParent.Handle == hwndChild.Handle || PInvoke.IsChild(hwndParent, hwndChild);

        private static bool IsKeyOrMouseMessage(Message m)
        {
            if (m.IsMouseMessage())
            {
                return true;
            }
            else if (m.Msg is >= ((int)PInvokeCore.WM_NCLBUTTONDOWN) and <= ((int)PInvokeCore.WM_NCMBUTTONDBLCLK))
            {
                return true;
            }
            else if (m.IsKeyMessage())
            {
                return true;
            }

            return false;
        }

        public bool PreFilterMessage(ref Message m)
        {
            if (_suspendMenuMode)
            {
                return false;
            }

            ToolStrip? activeToolStrip = GetActiveToolStrip();
            if (activeToolStrip is null)
            {
                return false;
            }

            if (activeToolStrip.IsDisposed)
            {
                RemoveActiveToolStripCore(activeToolStrip);
                return false;
            }

            HandleRef<HWND> activeToolStripHandle = new(activeToolStrip);
            var activeWindowHandle = Control.GetHandleRef(PInvoke.GetActiveWindow());

            if (activeWindowHandle != _lastActiveWindow)
            {
                // If another window has gotten activation - we should dismiss.
                if (activeWindowHandle.Handle.IsNull)
                {
                    // We don't know what it was cause it's on another thread or doesn't exist.
                    ProcessActivationChange();
                }
                else if (Control.FromChildHandle(activeWindowHandle.Handle) is not ToolStripDropDown
                    && !IsChildOrSameWindow(activeWindowHandle, activeToolStripHandle)
                    && !IsChildOrSameWindow(activeWindowHandle, ActiveHwnd))
                {
                    // Not a dropdown, and not a child of the active toolstrip or the active window.
                    ProcessActivationChange();
                }
            }

            // Store this off so we don't have to do activation processing next time
            _lastActiveWindow = activeWindowHandle;

            // Performance: skip over things like WM_PAINT.
            if (!IsKeyOrMouseMessage(m))
            {
                return false;
            }

            DPI_AWARENESS_CONTEXT context = GetDpiAwarenessContextForWindow(m.HWND);

            using (ScaleHelper.EnterDpiAwarenessScope(context))
            {
                switch (m.MsgInternal)
                {
                    case PInvokeCore.WM_MOUSEMOVE:
                    case PInvokeCore.WM_NCMOUSEMOVE:
                        // Mouse move messages should be eaten if they aren't for a dropdown.
                        // this prevents things like ToolTips and mouse over highlights from
                        // being processed.
                        Control? control = Control.FromChildHandle(m.HWnd);
                        if (control is null || control.TopLevelControlInternal is not ToolStripDropDown)
                        {
                            // Double check it's not a child control of the active toolstrip.
                            if (!IsChildOrSameWindow<IHandle<HWND>>(activeToolStripHandle, m))
                            {
                                // It is NOT a child of the current active toolstrip.

                                ToolStrip? toplevelToolStrip = GetCurrentTopLevelToolStrip();
                                if (toplevelToolStrip is not null && IsChildOrSameWindow<IHandle<HWND>>(toplevelToolStrip, m))
                                {
                                    // Don't eat mouse message.
                                    // The mouse message is from an HWND that is part of the toplevel toolstrip - let the mouse move through so
                                    // when you have something like the file menu open and mouse over the edit menu
                                    // the file menu will dismiss.

                                    return false;
                                }
                                else if (!IsChildOrSameWindow<IHandle<HWND>>(ActiveHwnd, m))
                                {
                                    // Don't eat mouse message.
                                    // the mouse message is from another toplevel HWND.
                                    return false;
                                }

                                // Eat mouse message
                                // the HWND is
                                //      not part of the active toolstrip
                                //      not the toplevel toolstrip (e.g. MenuStrip).
                                //      not parented to the toplevel toolstrip (e.g a combo box on a menu strip).
                                return true;
                            }
                        }

                        break;
                    case PInvokeCore.WM_LBUTTONDOWN:
                    case PInvokeCore.WM_RBUTTONDOWN:
                    case PInvokeCore.WM_MBUTTONDOWN:
                        // When a mouse button is pressed, we should determine if it is within the client coordinates
                        // of the active dropdown. If not, we should dismiss it.
                        ProcessMouseButtonPressed(m.HWND, PARAM.ToPoint(m.LParamInternal));
                        break;
                    case PInvokeCore.WM_NCLBUTTONDOWN:
                    case PInvokeCore.WM_NCRBUTTONDOWN:
                    case PInvokeCore.WM_NCMBUTTONDOWN:
                        // When a mouse button is pressed, we should determine if it is within the client coordinates
                        // of the active dropdown. If not, we should dismiss it.
                        ProcessMouseButtonPressed(default, PARAM.ToPoint(m.LParamInternal));
                        break;

                    case PInvokeCore.WM_KEYDOWN:
                    case PInvokeCore.WM_KEYUP:
                    case PInvokeCore.WM_CHAR:
                    case PInvokeCore.WM_DEADCHAR:
                    case PInvokeCore.WM_SYSKEYDOWN:
                    case PInvokeCore.WM_SYSKEYUP:
                    case PInvokeCore.WM_SYSCHAR:
                    case PInvokeCore.WM_SYSDEADCHAR:

                        if (!activeToolStrip.ContainsFocus)
                        {
                            // Route all keyboard messages to the active dropdown.
                            m.HWnd = activeToolStrip.Handle;
                        }

                        break;
                }
            }

            return false;
        }

        internal static DPI_AWARENESS_CONTEXT GetDpiAwarenessContextForWindow(HWND hwnd)
        {
            DPI_AWARENESS_CONTEXT dpiAwarenessContext = DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT;

            if (OsVersion.IsWindows10_1607OrGreater())
            {
                // Works only >= Windows 10/1607
                DPI_AWARENESS_CONTEXT awarenessContext = PInvoke.GetWindowDpiAwarenessContext(hwnd);
                DPI_AWARENESS awareness = PInvoke.GetAwarenessFromDpiAwarenessContext(awarenessContext);
                dpiAwarenessContext = ConvertToDpiAwarenessContext(awareness);
            }

            return dpiAwarenessContext;
        }

        private static DPI_AWARENESS_CONTEXT ConvertToDpiAwarenessContext(DPI_AWARENESS dpiAwareness) => dpiAwareness switch
        {
            DPI_AWARENESS.DPI_AWARENESS_UNAWARE => DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNAWARE,
            DPI_AWARENESS.DPI_AWARENESS_SYSTEM_AWARE => DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE,
            DPI_AWARENESS.DPI_AWARENESS_PER_MONITOR_AWARE => DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2,
            _ => DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_SYSTEM_AWARE,
        };
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public static partial class ToolStripManager
    {
        ///  <remarks>
        ///  - this installs a message filter when a dropdown becomes active.
        ///  - the message filter
        ///  a. eats WM_MOUSEMOVEs so that the window that's underneath
        ///  doesnt get highlight processing/tooltips
        ///  b. detects mouse clicks. if the click is outside the dropdown, it
        ///  dismisses it.
        ///  c. detects when the active window has changed. If the active window
        ///  is unexpected, it dismisses all dropdowns.
        ///  d. detects keyboard messages, and redirects them to the active dropdown.
        ///
        ///  - There should be 1 Message Filter per thread and it should be uninstalled once
        ///  the last dropdown has gone away
        ///  This is not part of ToolStripManager because it's DropDown specific and
        ///  we don't want to publicly expose this message filter.
        ///  </remarks>
        internal partial class ModalMenuFilter : IMessageModifyAndFilter
        {
            // The window that was active when we showed the dropdown
            private HandleRef _activeHwnd = NativeMethods.NullHandleRef;
            // The window that was last known to be active
            private HandleRef _lastActiveWindow = NativeMethods.NullHandleRef;
            private List<ToolStrip> _inputFilterQueue;
            private bool _inMenuMode;
            private bool _caretHidden;
            private bool _showUnderlines;
            private bool _menuKeyToggle;
            private bool _suspendMenuMode;
            private HostedWindowsFormsMessageHook _messageHook;
            private Timer _ensureMessageProcessingTimer;
            private const int MessageProcessingInterval = 500;

            private ToolStrip _toplevelToolStrip;

            private readonly WeakReference<IKeyboardToolTip> _lastFocusedTool = new WeakReference<IKeyboardToolTip>(null);

#if DEBUG
            private bool _justEnteredMenuMode;
#endif

            [ThreadStatic]
            private static ModalMenuFilter t_instance;

            internal static ModalMenuFilter Instance => t_instance ??= new ModalMenuFilter();

            private ModalMenuFilter()
            {
            }

            /// <summary>
            ///  The HWnd that was active when we popped the first dropdown.
            /// </summary>
            internal static HandleRef ActiveHwnd => Instance.ActiveHwndInternal;

            // returns whether or not we should show focus cues for mnemonics.
            public bool ShowUnderlines
            {
                get => _showUnderlines;
                set
                {
                    if (_showUnderlines != value)
                    {
                        _showUnderlines = value;
                        ToolStripManager.NotifyMenuModeChange(invalidateText: true, activationChange: false);
                    }
                }
            }

            private HandleRef ActiveHwndInternal
            {
                get => _activeHwnd;
                set
                {
                    if (_activeHwnd.Handle != value.Handle)
                    {
                        Control control = null;

                        // Unsubscribe from handle recreate.
                        if (_activeHwnd.Handle != IntPtr.Zero)
                        {
                            control = Control.FromHandle(_activeHwnd.Handle);
                            if (control != null)
                            {
                                control.HandleCreated -= new EventHandler(OnActiveHwndHandleCreated);
                            }
                        }

                        _activeHwnd = value;

                        // make sure we watch out for handle recreates.
                        control = Control.FromHandle(_activeHwnd.Handle);
                        if (control != null)
                        {
                            control.HandleCreated += new EventHandler(OnActiveHwndHandleCreated);
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
                    Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "___________Entering MenuMode....");
#if DEBUG
                    _justEnteredMenuMode = true;
#endif
                    IntPtr hwndActive = User32.GetActiveWindow();
                    if (hwndActive != IntPtr.Zero)
                    {
                        ActiveHwndInternal = new HandleRef(this, hwndActive);
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
                IKeyboardToolTip lastFocusedTool = KeyboardToolTipStateMachine.Instance.LastFocusedTool;
                if (lastFocusedTool != null)
                {
                    _lastFocusedTool.SetTarget(lastFocusedTool);
                    KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(lastFocusedTool);
                }
            }

            internal static void ExitMenuMode() => Instance.ExitMenuModeCore();

            private void ExitMenuModeCore()
            {
                // ensure we've cleaned up the timer.
                ProcessMessages(false);

                if (InMenuMode)
                {
                    try
                    {
                        Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "___________Exiting MenuMode....");

                        if (_messageHook != null)
                        {
                            // message filter isn't going to help as we dont own the message pump
                            // switch over to a MessageHook
                            _messageHook.HookMessages = false;
                        }

                        Application.ThreadContext.FromCurrent().RemoveMessageFilter(this);
                        Application.ThreadContext.FromCurrent().TrackInput(false);

#if DEBUG
                        _justEnteredMenuMode = false;
#endif
                        if (ActiveHwnd.Handle != IntPtr.Zero)
                        {
                            // Unsubscribe from handle creates
                            Control control = Control.FromHandle(ActiveHwnd.Handle);
                            if (control != null)
                            {
                                control.HandleCreated -= new EventHandler(OnActiveHwndHandleCreated);
                            }
                            ActiveHwndInternal = NativeMethods.NullHandleRef;
                        }

                        _inputFilterQueue?.Clear();
                        if (_caretHidden)
                        {
                            _caretHidden = false;
                            User32.ShowCaret(IntPtr.Zero);
                        }

                        if (_lastFocusedTool.TryGetTarget(out IKeyboardToolTip tool) && tool != null)
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
                        ToolStripManager.NotifyMenuModeChange(/*textStyleChanged*/textStyleChanged, /*activationChanged*/true);
                    }
                }
            }

            internal static ToolStrip GetActiveToolStrip() => Instance.GetActiveToolStripInternal();

            internal ToolStrip GetActiveToolStripInternal()
            {
                if (_inputFilterQueue != null && _inputFilterQueue.Count > 0)
                {
                    return _inputFilterQueue[_inputFilterQueue.Count - 1];
                }

                return null;
            }

            /// <summary>
            ///  Returns the ToolStrip that is at the root.
            /// </summary>
            private ToolStrip GetCurrentTopLevelToolStrip()
            {
                if (_toplevelToolStrip is null)
                {
                    ToolStrip activeToolStrip = GetActiveToolStripInternal();
                    if (activeToolStrip != null)
                    {
                        _toplevelToolStrip = activeToolStrip.GetToplevelOwnerToolStrip();
                    }
                }

                return _toplevelToolStrip;
            }

            private void OnActiveHwndHandleCreated(object sender, EventArgs e)
            {
                Control topLevel = sender as Control;
                ActiveHwndInternal = new HandleRef(this, topLevel.Handle);
            }

            internal static void ProcessMenuKeyDown(ref Message m)
            {
                Keys keyData = (Keys)(int)m.WParam;

                if (Control.FromHandle(m.HWnd) is ToolStrip toolStrip && !toolStrip.IsDropDown)
                {
                    return;
                }

                // Handle the case where the ALT key has been pressed down while a dropdown
                // was open. We need to clear off the MenuKeyToggle so the next ALT will activate
                // the menu.
                if (ToolStripManager.IsMenuKey(keyData))
                {
                    if (!InMenuMode && MenuKeyToggle)
                    {
                        MenuKeyToggle = false;
                    }
                    else if (!MenuKeyToggle)
                    {
                        ModalMenuFilter.Instance.ShowUnderlines = true;
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
                    Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ModalMenuFilter.CloseActiveDropDown] Calling exit because there are no more dropdowns left to activate.");
                    ExitMenuMode();

                    // Make sure we roll selection off  the toplevel toolstrip.
                    if (activeToolStripDropDown.OwnerItem != null)
                    {
                        activeToolStripDropDown.OwnerItem.Unselect();
                    }
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
                else if (_ensureMessageProcessingTimer != null)
                {
                    _ensureMessageProcessingTimer.Enabled = false;
                    _ensureMessageProcessingTimer.Dispose();
                    _ensureMessageProcessingTimer = null;
                }
            }

            private void ProcessMouseButtonPressed(IntPtr hwndMouseMessageIsFrom, int x, int y)
            {
                Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ModalMenuFilter.ProcessMouseButtonPressed] Found a mouse down.");

                int countDropDowns = _inputFilterQueue.Count;
                for (int i = 0; i < countDropDowns; i++)
                {
                    ToolStrip activeToolStrip = GetActiveToolStripInternal();

                    if (activeToolStrip != null)
                    {
                        var pt = new Point(x, y);
                        User32.MapWindowPoints(new HandleRef(activeToolStrip, hwndMouseMessageIsFrom), new HandleRef(activeToolStrip, activeToolStrip.Handle), ref pt, 1);
                        if (!activeToolStrip.ClientRectangle.Contains(pt.X, pt.Y))
                        {
                            if (activeToolStrip is ToolStripDropDown activeToolStripDropDown)
                            {
                                if (!(activeToolStripDropDown.OwnerToolStrip != null
                                    && activeToolStripDropDown.OwnerToolStrip.Handle == hwndMouseMessageIsFrom
                                    && activeToolStripDropDown.OwnerDropDownItem != null
                                     && activeToolStripDropDown.OwnerDropDownItem.DropDownButtonArea.Contains(x, y)))
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
                                // We're a toplevel toolstrip and we've clicked somewhere else.
                                // Exit menu mode
                                Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ModalMenuFilter.ProcessMouseButtonPressed] Calling exit because we're a toplevel toolstrip and we've clicked somewhere else.");
                                ExitMenuModeCore();
                            }
                        }
                        else
                        {
                            // we've found a dropdown that intersects with the mouse message
                            break;
                        }
                    }
                    else
                    {
                        Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ModalMenuFilter.ProcessMouseButtonPressed] active toolstrip is null.");
                        break;
                    }
                }
            }

            private bool ProcessActivationChange()
            {
                int countDropDowns = _inputFilterQueue.Count;
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
                    // For something that never closes, dont use menu mode.
                    ToolStripDropDown dropDown = toolStrip as ToolStripDropDown;

                    if (dropDown.AutoClose == false)
                    {
                        // Store off the current active hwnd
                        IntPtr hwndActive = User32.GetActiveWindow();
                        if (hwndActive != IntPtr.Zero)
                        {
                            ActiveHwndInternal = new HandleRef(this, hwndActive);
                        }

                        // Dont actually enter menu mode..
                        return;
                    }
                }
                toolStrip.KeyboardActive = true;

                if (_inputFilterQueue is null)
                {
                    // Use list because we want to be able to remove at any point
                    _inputFilterQueue = new List<ToolStrip>();
                }
                else
                {
                    ToolStrip currentActiveToolStrip = GetActiveToolStripInternal();

                    // ToolStrip dropdowns push/pull their activation based on visibility.
                    // we have to account for the toolstrips that aren't dropdowns
                    if (currentActiveToolStrip != null)
                    {
                        if (!currentActiveToolStrip.IsDropDown)
                        {
                            _inputFilterQueue.Remove(currentActiveToolStrip);
                        }
                        else if ((toolStrip.IsDropDown)
                                  && (ToolStripDropDown.GetFirstDropDown(toolStrip)
                                  != ToolStripDropDown.GetFirstDropDown(currentActiveToolStrip)))
                        {
                            Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ModalMenuFilter.SetActiveToolStripCore] Detected a new dropdown not in this chain opened, Dismissing everything in the old chain. ");
                            _inputFilterQueue.Remove(currentActiveToolStrip);

                            ToolStripDropDown currentActiveToolStripDropDown = currentActiveToolStrip as ToolStripDropDown;
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
                    Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ModalMenuFilter.SetActiveToolStripCore] Setting " + WindowsFormsUtils.GetControlInformation(toolStrip.Handle) + " active.");
                    EnterMenuModeCore();
                }

                // Hide the caret if we're showing a toolstrip dropdown
                if (!_caretHidden && toolStrip.IsDropDown && InMenuMode)
                {
                    _caretHidden = true;
                    User32.HideCaret(IntPtr.Zero);
                }
            }

            internal static void SuspendMenuMode()
            {
                Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ModalMenuFilter] SuspendMenuMode");

                Instance._suspendMenuMode = true;
            }

            internal static void ResumeMenuMode()
            {
                Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ModalMenuFilter] ResumeMenuMode");
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

            private static bool IsChildOrSameWindow(HandleRef hwndParent, HandleRef hwndChild)
                => hwndParent.Handle == hwndChild.Handle || User32.IsChild(hwndParent, hwndChild).IsTrue();

            private static bool IsKeyOrMouseMessage(Message m)
            {
                if (m.IsMouseMessage())
                {
                    return true;
                }
                else if (m.Msg >= (int)User32.WM.NCLBUTTONDOWN && m.Msg <= (int)User32.WM.NCMBUTTONDBLCLK)
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
#if DEBUG
                Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose && _justEnteredMenuMode, "[ModalMenuFilter.PreFilterMessage] MenuMode MessageFilter installed and working.");
                _justEnteredMenuMode = false;
#endif

                if (_suspendMenuMode)
                {
                    return false;
                }
                ToolStrip activeToolStrip = GetActiveToolStrip();
                if (activeToolStrip is null)
                {
                    return false;
                }
                if (activeToolStrip.IsDisposed)
                {
                    RemoveActiveToolStripCore(activeToolStrip);
                    return false;
                }
                HandleRef hwndActiveToolStrip = new HandleRef(activeToolStrip, activeToolStrip.Handle);
                HandleRef hwndCurrentActiveWindow = new HandleRef(null, User32.GetActiveWindow());

                // if the active window has changed...
                if (hwndCurrentActiveWindow.Handle != _lastActiveWindow.Handle)
                {
                    // if another window has gotten activation - we should dismiss.
                    if (hwndCurrentActiveWindow.Handle == IntPtr.Zero)
                    {
                        // we dont know what it was cause it's on another thread or doesnt exist
                        Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ModalMenuFilter.PreFilterMessage] Dismissing because: " + WindowsFormsUtils.GetControlInformation(hwndCurrentActiveWindow.Handle) + " has gotten activation. ");
                        ProcessActivationChange();
                    }
                    else if (!(Control.FromChildHandle(hwndCurrentActiveWindow.Handle) is ToolStripDropDown)   // its NOT a dropdown
                        && !IsChildOrSameWindow(hwndCurrentActiveWindow, hwndActiveToolStrip)    // and NOT a child of the active toolstrip
                        && !IsChildOrSameWindow(hwndCurrentActiveWindow, ActiveHwnd))
                    {          // and NOT a child of the active hwnd
                        Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ModalMenuFilter.PreFilterMessage] Calling ProcessActivationChange because: " + WindowsFormsUtils.GetControlInformation(hwndCurrentActiveWindow.Handle) + " has gotten activation. ");
                        ProcessActivationChange();
                    }
                }

                // Store this off so we dont have to do activation processing next time
                _lastActiveWindow = hwndCurrentActiveWindow;

                // Performance: skip over things like WM_PAINT.
                if (!IsKeyOrMouseMessage(m))
                {
                    return false;
                }

                IntPtr context = GetDpiAwarenessContextForWindow(m.HWnd);

                using (DpiHelper.EnterDpiAwarenessScope(context))
                {
                    switch ((User32.WM)m.Msg)
                    {
                        case User32.WM.MOUSEMOVE:
                        case User32.WM.NCMOUSEMOVE:
                            // Mouse move messages should be eaten if they aren't for a dropdown.
                            // this prevents things like ToolTips and mouse over highlights from
                            // being processed.
                            Control control = Control.FromChildHandle(m.HWnd);
                            if (control is null || !(control.TopLevelControlInternal is ToolStripDropDown))
                            {
                                // Double check it's not a child control of the active toolstrip.
                                if (!IsChildOrSameWindow(hwndActiveToolStrip, new HandleRef(null, m.HWnd)))
                                {
                                    // It is NOT a child of the current active toolstrip.

                                    ToolStrip toplevelToolStrip = GetCurrentTopLevelToolStrip();
                                    if (toplevelToolStrip != null
                                        && (IsChildOrSameWindow(new HandleRef(toplevelToolStrip, toplevelToolStrip.Handle),
                                                               new HandleRef(null, m.HWnd))))
                                    {
                                        // Don't eat mouse message.
                                        // The mouse message is from an HWND that is part of the toplevel toolstrip - let the mouse move through so
                                        // when you have something like the file menu open and mouse over the edit menu
                                        // the file menu will dismiss.

                                        return false;
                                    }
                                    else if (!IsChildOrSameWindow(ActiveHwnd, new HandleRef(null, m.HWnd)))
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
                        case User32.WM.LBUTTONDOWN:
                        case User32.WM.RBUTTONDOWN:
                        case User32.WM.MBUTTONDOWN:
                            // When a mouse button is pressed, we should determine if it is within the client coordinates
                            // of the active dropdown. If not, we should dismiss it.
                            ProcessMouseButtonPressed(m.HWnd,
                                x: PARAM.SignedLOWORD(m.LParam),
                                y: PARAM.SignedHIWORD(m.LParam));

                            break;
                        case User32.WM.NCLBUTTONDOWN:
                        case User32.WM.NCRBUTTONDOWN:
                        case User32.WM.NCMBUTTONDOWN:
                            // When a mouse button is pressed, we should determine if it is within the client coordinates
                            // of the active dropdown. If not, we should dismiss it.
                            ProcessMouseButtonPressed(/*nc messages are in screen coords*/IntPtr.Zero,
                                x: PARAM.SignedLOWORD(m.LParam),
                                y: PARAM.SignedHIWORD(m.LParam));
                            break;

                        case User32.WM.KEYDOWN:
                        case User32.WM.KEYUP:
                        case User32.WM.CHAR:
                        case User32.WM.DEADCHAR:
                        case User32.WM.SYSKEYDOWN:
                        case User32.WM.SYSKEYUP:
                        case User32.WM.SYSCHAR:
                        case User32.WM.SYSDEADCHAR:

                            if (!activeToolStrip.ContainsFocus)
                            {
                                Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ModalMenuFilter.PreFilterMessage] MODIFYING Keyboard message " + m.ToString());

                                // Route all keyboard messages to the active dropdown.
                                m.HWnd = activeToolStrip.Handle;
                            }
                            else
                            {
                                Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ModalMenuFilter.PreFilterMessage] got Keyboard message " + m.ToString());
                            }
                            break;
                    }
                }

                return false;
            }

            internal static IntPtr GetDpiAwarenessContextForWindow(IntPtr hWnd)
            {
                IntPtr dpiAwarenessContext = User32.UNSPECIFIED_DPI_AWARENESS_CONTEXT;

                if (OsVersion.IsWindows10_1607OrGreater)
                {
                    // Works only >= Windows 10/1607
                    IntPtr awarenessContext = User32.GetWindowDpiAwarenessContext(hWnd);
                    User32.DPI_AWARENESS awareness = User32.GetAwarenessFromDpiAwarenessContext(awarenessContext);
                    dpiAwarenessContext = ConvertToDpiAwarenessContext(awareness);
                }

                return dpiAwarenessContext;
            }

            private static IntPtr ConvertToDpiAwarenessContext(User32.DPI_AWARENESS dpiAwareness)
            {
                switch (dpiAwareness)
                {
                    case User32.DPI_AWARENESS.UNAWARE:
                        return User32.DPI_AWARENESS_CONTEXT.UNAWARE;
                    case User32.DPI_AWARENESS.SYSTEM_AWARE:
                        return User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE;
                    case User32.DPI_AWARENESS.PER_MONITOR_AWARE:
                        return User32.DPI_AWARENESS_CONTEXT.PER_MONITOR_AWARE_V2;
                    default:
                        return User32.DPI_AWARENESS_CONTEXT.SYSTEM_AWARE;
                }
            }
        }
    }
}

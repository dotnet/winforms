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
        ///  b. detects mouse clicks.  if the click is outside the dropdown, it
        ///  dismisses it.
        ///  c. detects when the active window has changed.  If the active window
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
            private HandleRef _activeHwnd = NativeMethods.NullHandleRef; // the window that was active when we showed the dropdown
            private HandleRef _lastActiveWindow = NativeMethods.NullHandleRef;         // the window that was last known to be active
            private List<ToolStrip> _inputFilterQueue;
            private bool _inMenuMode = false;
            private bool _caretHidden = false;
            private bool _showUnderlines = false;
            private bool menuKeyToggle = false;
            private bool _suspendMenuMode = false;
            private HostedWindowsFormsMessageHook messageHook;
            private Timer _ensureMessageProcessingTimer = null;
            private const int MESSAGE_PROCESSING_INTERVAL = 500;

            private ToolStrip _toplevelToolStrip = null;

            private readonly WeakReference<IKeyboardToolTip> lastFocusedTool = new WeakReference<IKeyboardToolTip>(null);

#if DEBUG
            bool _justEnteredMenuMode = false;
#endif
            [ThreadStatic]
            private static ModalMenuFilter _instance;

            internal static ModalMenuFilter Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new ModalMenuFilter();
                    }
                    return _instance;
                }
            }

            private ModalMenuFilter()
            {
            }

            ///  this is the HWnd that was active when we popped the first dropdown.
            internal static HandleRef ActiveHwnd
            {
                get { return Instance.ActiveHwndInternal; }
            }

            // returns whether or not we should show focus cues for mnemonics.
            public bool ShowUnderlines
            {
                get
                {
                    return _showUnderlines;
                }
                set
                {
                    if (_showUnderlines != value)
                    {
                        _showUnderlines = value;
                        ToolStripManager.NotifyMenuModeChange(/*textStyleChanged*/true, /*activationChanged*/false);
                    }
                }
            }

            private HandleRef ActiveHwndInternal
            {
                get
                {
                    return _activeHwnd;
                }
                set
                {
                    if (_activeHwnd.Handle != value.Handle)
                    {
                        Control control = null;

                        // unsubscribe from handle recreate.
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

            // returns whether or not someone has called EnterMenuMode.
            internal static bool InMenuMode
            {
                get { return Instance._inMenuMode; }
            }

            internal static bool MenuKeyToggle
            {
                get
                {
                    return Instance.menuKeyToggle;
                }
                set
                {
                    if (Instance.menuKeyToggle != value)
                    {
                        Instance.menuKeyToggle = value;
                    }
                }
            }

            ///  This is used in scenarios where windows forms
            ///  does not own the message pump, but needs access
            ///  to the message queue.
            private HostedWindowsFormsMessageHook MessageHook
            {
                get
                {
                    if (messageHook == null)
                    {
                        messageHook = new HostedWindowsFormsMessageHook();
                    }
                    return messageHook;
                }
            }

            // ToolStrip analog to WM_ENTERMENULOOP
            private void EnterMenuModeCore()
            {
                Debug.Assert(!InMenuMode, "How did we get here if we're already in menu mode?");

                if (!InMenuMode)
                {
                    Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "___________Entering MenuMode....");
#if DEBUG
                    _justEnteredMenuMode = true;
#endif
                    IntPtr hwndActive = User32.GetActiveWindow();
                    if (hwndActive != IntPtr.Zero)
                    {
                        ActiveHwndInternal = new HandleRef(this, hwndActive);
                    }

                    // PERF,

                    Application.ThreadContext.FromCurrent().AddMessageFilter(this);
                    Application.ThreadContext.FromCurrent().TrackInput(true);

                    if (!Application.ThreadContext.FromCurrent().GetMessageLoop(true))
                    {
                        // message filter isn't going to help as we don't own the message pump
                        // switch over to a MessageHook
                        MessageHook.HookMessages = true;
                    }
                    _inMenuMode = true;

                    NotifyLastLastFocusedToolAboutFocusLoss();

                    // fire timer messages to force our filter to get evaluated.
                    ProcessMessages(true);
                }
            }

            internal void NotifyLastLastFocusedToolAboutFocusLoss()
            {
                IKeyboardToolTip lastFocusedTool = KeyboardToolTipStateMachine.Instance.LastFocusedTool;
                if (lastFocusedTool != null)
                {
                    this.lastFocusedTool.SetTarget(lastFocusedTool);
                    KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(lastFocusedTool);
                }
            }

            internal static void ExitMenuMode()
            {
                Instance.ExitMenuModeCore();
            }

            // ToolStrip analog to WM_EXITMENULOOP
            private void ExitMenuModeCore()
            {
                // ensure we've cleaned up the timer.
                ProcessMessages(false);

                if (InMenuMode)
                {
                    try
                    {
                        Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "___________Exiting MenuMode....");

                        if (messageHook != null)
                        {
                            // message filter isn't going to help as we dont own the message pump
                            // switch over to a MessageHook
                            messageHook.HookMessages = false;
                        }
                        // PERF,

                        Application.ThreadContext.FromCurrent().RemoveMessageFilter(this);
                        Application.ThreadContext.FromCurrent().TrackInput(false);

#if DEBUG
                        _justEnteredMenuMode = false;
#endif
                        if (ActiveHwnd.Handle != IntPtr.Zero)
                        {
                            // unsubscribe from handle creates
                            Control control = Control.FromHandle(ActiveHwnd.Handle);
                            if (control != null)
                            {
                                control.HandleCreated -= new EventHandler(OnActiveHwndHandleCreated);
                            }
                            ActiveHwndInternal = NativeMethods.NullHandleRef;
                        }
                        if (_inputFilterQueue != null)
                        {
                            _inputFilterQueue.Clear();
                        }
                        if (_caretHidden)
                        {
                            _caretHidden = false;
                            User32.ShowCaret(IntPtr.Zero);
                        }

                        if (lastFocusedTool.TryGetTarget(out IKeyboardToolTip tool) && tool != null)
                        {
                            KeyboardToolTipStateMachine.Instance.NotifyAboutGotFocus(tool);
                        }
                    }
                    finally
                    {
                        _inMenuMode = false;

                        // skip the setter here so we only iterate through the toolstrips once.
                        bool textStyleChanged = _showUnderlines;
                        _showUnderlines = false;
                        ToolStripManager.NotifyMenuModeChange(/*textStyleChanged*/textStyleChanged, /*activationChanged*/true);
                    }
                }
            }

            internal static ToolStrip GetActiveToolStrip()
            {
                return Instance.GetActiveToolStripInternal();
            }

            internal ToolStrip GetActiveToolStripInternal()
            {
                if (_inputFilterQueue != null && _inputFilterQueue.Count > 0)
                {
                    return _inputFilterQueue[_inputFilterQueue.Count - 1];
                }
                return null;
            }

            // return the toolstrip that is at the root.
            private ToolStrip GetCurrentToplevelToolStrip()
            {
                if (_toplevelToolStrip == null)
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

                // handle the case where the ALT key has been pressed down while a dropdown
                // was open.  We need to clear off the MenuKeyToggle so the next ALT will activate
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

                // there's no more dropdowns left in the chain
                if (GetActiveToolStrip() == null)
                {
                    Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.CloseActiveDropDown] Calling exit because there are no more dropdowns left to activate.");
                    ExitMenuMode();

                    // make sure we roll selection off  the toplevel toolstrip.
                    if (activeToolStripDropDown.OwnerItem != null)
                    {
                        activeToolStripDropDown.OwnerItem.Unselect();
                    }
                }
            }

            // fire a timer event to ensure we have a message in the queue every 500ms
            private void ProcessMessages(bool process)
            {
                if (process)
                {
                    if (_ensureMessageProcessingTimer == null)
                    {
                        _ensureMessageProcessingTimer = new Timer();
                    }
                    _ensureMessageProcessingTimer.Interval = MESSAGE_PROCESSING_INTERVAL;
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
                Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.ProcessMouseButtonPressed] Found a mouse down.");

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
                                    // the owner item should handle closing the dropdown
                                    // this allows code such as if (DropDown.Visible) { Hide, Show } etc.
                                    CloseActiveDropDown(activeToolStripDropDown, ToolStripDropDownCloseReason.AppClicked);
                                }
                            }
                            else
                            {
                                // make sure we clear the selection.
                                activeToolStrip.NotifySelectionChange(/*selectedItem=*/null);
                                // we're a toplevel toolstrip and we've clicked somewhere else.
                                // Exit menu mode
                                Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.ProcessMouseButtonPressed] Calling exit because we're a toplevel toolstrip and we've clicked somewhere else.");
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
                        Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.ProcessMouseButtonPressed] active toolstrip is null.");
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
            {
                Instance.SetActiveToolStripCore(toolStrip);
            }

            private void SetActiveToolStripCore(ToolStrip toolStrip)
            {
                if (toolStrip == null)
                {
                    return;
                }
                if (toolStrip.IsDropDown)
                {
                    // for something that never closes, dont use menu mode.
                    ToolStripDropDown dropDown = toolStrip as ToolStripDropDown;

                    if (dropDown.AutoClose == false)
                    {
                        // store off the current active hwnd
                        IntPtr hwndActive = User32.GetActiveWindow();
                        if (hwndActive != IntPtr.Zero)
                        {
                            ActiveHwndInternal = new HandleRef(this, hwndActive);
                        }
                        // dont actually enter menu mode...
                        return;
                    }
                }
                toolStrip.KeyboardActive = true;

                if (_inputFilterQueue == null)
                {
                    // use list because we want to be able to remove at any point
                    _inputFilterQueue = new List<ToolStrip>();
                }
                else
                {
                    ToolStrip currentActiveToolStrip = GetActiveToolStripInternal();

                    // toolstrip dropdowns push/pull their activation based on visibility.
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
                            Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.SetActiveToolStripCore] Detected a new dropdown not in this chain opened, Dismissing everything in the old chain. ");
                            _inputFilterQueue.Remove(currentActiveToolStrip);

                            ToolStripDropDown currentActiveToolStripDropDown = currentActiveToolStrip as ToolStripDropDown;
                            currentActiveToolStripDropDown.DismissAll();
                        }
                    }
                }

                // reset the toplevel toolstrip
                _toplevelToolStrip = null;

                if (!_inputFilterQueue.Contains(toolStrip))
                {
                    _inputFilterQueue.Add(toolStrip);
                }

                if (!InMenuMode && _inputFilterQueue.Count > 0)
                {
                    Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.SetActiveToolStripCore] Setting " + WindowsFormsUtils.GetControlInformation(toolStrip.Handle) + " active.");
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
                Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter] SuspendMenuMode");

                Instance._suspendMenuMode = true;
            }

            internal static void ResumeMenuMode()
            {
                Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter] ResumeMenuMode");
                Instance._suspendMenuMode = false;
            }
            internal static void RemoveActiveToolStrip(ToolStrip toolStrip)
            {
                Instance.RemoveActiveToolStripCore(toolStrip);
            }

            private void RemoveActiveToolStripCore(ToolStrip toolStrip)
            {
                // precautionary - remove the active toplevel toolstrip.
                _toplevelToolStrip = null;

                if (_inputFilterQueue != null)
                {
                    _inputFilterQueue.Remove(toolStrip);
                }
            }

            private static bool IsChildOrSameWindow(HandleRef hwndParent, HandleRef hwndChild)
            {
                return hwndParent.Handle == hwndChild.Handle || User32.IsChild(hwndParent, hwndChild).IsTrue();
            }

            private static bool IsKeyOrMouseMessage(Message m)
            {
                bool filterMessage = false;

                if (m.IsMouseMessage())
                {
                    filterMessage = true;
                }
                else if (m.Msg >= (int)User32.WM.NCLBUTTONDOWN && m.Msg <= (int)User32.WM.NCMBUTTONDBLCLK)
                {
                    filterMessage = true;
                }
                else if (m.IsKeyMessage())
                {
                    filterMessage = true;
                }
                return filterMessage;
            }

            public bool PreFilterMessage(ref Message m)
            {
#if DEBUG
                Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose && _justEnteredMenuMode, "[ModalMenuFilter.PreFilterMessage] MenuMode MessageFilter installed and working.");
                _justEnteredMenuMode = false;
#endif

                if (_suspendMenuMode)
                {
                    return false;
                }
                ToolStrip activeToolStrip = GetActiveToolStrip();
                if (activeToolStrip == null)
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
                        Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.PreFilterMessage] Dismissing because: " + WindowsFormsUtils.GetControlInformation(hwndCurrentActiveWindow.Handle) + " has gotten activation. ");
                        ProcessActivationChange();
                    }
                    else if (!(Control.FromChildHandle(hwndCurrentActiveWindow.Handle) is ToolStripDropDown)   // its NOT a dropdown
                        && !IsChildOrSameWindow(hwndCurrentActiveWindow, hwndActiveToolStrip)    // and NOT a child of the active toolstrip
                        && !IsChildOrSameWindow(hwndCurrentActiveWindow, ActiveHwnd))
                    {          // and NOT a child of the active hwnd
                        Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.PreFilterMessage] Calling ProcessActivationChange because: " + WindowsFormsUtils.GetControlInformation(hwndCurrentActiveWindow.Handle) + " has gotten activation. ");
                        ProcessActivationChange();
                    }
                }

                // store this off so we dont have to do activation processing next time
                _lastActiveWindow = hwndCurrentActiveWindow;

                // PERF: skip over things like PAINT...
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
                            if (control == null || !(control.TopLevelControlInternal is ToolStripDropDown))
                            {
                                // double check it's not a child control of the active toolstrip.
                                if (!IsChildOrSameWindow(hwndActiveToolStrip, new HandleRef(null, m.HWnd)))
                                {
                                    // it is NOT a child of the current active toolstrip.

                                    ToolStrip toplevelToolStrip = GetCurrentToplevelToolStrip();
                                    if (toplevelToolStrip != null
                                        && (IsChildOrSameWindow(new HandleRef(toplevelToolStrip, toplevelToolStrip.Handle),
                                                               new HandleRef(null, m.HWnd))))
                                    {
                                        // DON'T EAT mouse message.
                                        // The mouse message is from an HWND that is part of the toplevel toolstrip - let the mouse move through so
                                        // when you have something like the file menu open and mouse over the edit menu
                                        // the file menu will dismiss.

                                        return false;
                                    }
                                    else if (!IsChildOrSameWindow(ActiveHwnd, new HandleRef(null, m.HWnd)))
                                    {
                                        // DON'T EAT mouse message.
                                        // the mouse message is from another toplevel HWND.
                                        return false;
                                    }
                                    // EAT mouse message
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
                            //
                            // When a mouse button is pressed, we should determine if it is within the client coordinates
                            // of the active dropdown.  If not, we should dismiss it.
                            //
                            ProcessMouseButtonPressed(m.HWnd,
                                /*x=*/PARAM.SignedLOWORD(m.LParam),
                                /*y=*/PARAM.SignedHIWORD(m.LParam));

                            break;
                        case User32.WM.NCLBUTTONDOWN:
                        case User32.WM.NCRBUTTONDOWN:
                        case User32.WM.NCMBUTTONDOWN:
                            //
                            // When a mouse button is pressed, we should determine if it is within the client coordinates
                            // of the active dropdown.  If not, we should dismiss it.
                            //
                            ProcessMouseButtonPressed(/*nc messages are in screen coords*/IntPtr.Zero,
                                /*x=*/PARAM.SignedLOWORD(m.LParam),
                                /*y=*/PARAM.SignedHIWORD(m.LParam));
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
                                Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.PreFilterMessage] MODIFYING Keyboard message " + m.ToString());

                                // route all keyboard messages to the active dropdown.
                                m.HWnd = activeToolStrip.Handle;
                            }
                            else
                            {
                                Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.PreFilterMessage] got Keyboard message " + m.ToString());
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

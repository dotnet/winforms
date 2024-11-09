// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.ExceptionServices;
using System.Windows.Forms.Primitives;
using Microsoft.Office;

namespace System.Windows.Forms;

public sealed partial class Application
{
    /// <summary>
    ///  This class is the embodiment of TLS for windows forms. We do not expose this to end users because
    ///  TLS is really just an unfortunate artifact of using Win 32. We want the world to be free
    ///  threaded.
    /// </summary>
    internal abstract unsafe partial class ThreadContext : MarshalByRefObject, IHandle<HANDLE>
    {
        private bool _oleInitialized;
        private bool _externalOleInit;
        private bool _inThreadException;
        private bool _filterSnapshotValid;

        private static readonly Dictionary<uint, ThreadContext> s_contextHash = [];

        private static readonly Lock s_lock = new();
        private readonly Lock _marshallingControlLock = new();

        private static int s_totalMessageLoopCount;
        private static msoloop s_baseLoopReason;

        [ThreadStatic]
        private static ThreadContext? t_currentThreadContext;

        internal ThreadExceptionEventHandler? _threadExceptionHandler;
        internal EventHandler? _idleHandler;
        internal EventHandler? _enterModalHandler;
        internal EventHandler? _leaveModalHandler;

        // Parking window list
        private readonly List<ParkingWindow> _parkingWindows = [];
        private Control? _marshallingControl;
        private List<IMessageFilter>? _messageFilters;
        private List<IMessageFilter>? _messageFilterSnapshot;
        private int _inProcessFilters;
        private HANDLE _handle;
        private readonly uint _id;
        protected int _messageLoopCount;
        private int _modalCount;

        // Used for correct restoration of focus after modality
        private WeakReference<Control>? _activatingControlRef;

        private ThreadWindows? _threadWindows;
        private int _disposed;

        // Debug helper variable
#if DEBUG
        private int _debugModalCounter;
#endif

        // A private field on Application that stores the callback delegate
        private MessageLoopCallback? _messageLoopCallback;

        protected Form? CurrentForm { get; private set; }
        protected bool PostedQuit { get; private set; }

        /// <summary>
        ///  Creates a new thread context object.
        /// </summary>
        protected ThreadContext()
        {
            HANDLE target;

            PInvoke.DuplicateHandle(
                PInvoke.GetCurrentProcess(),
                PInvoke.GetCurrentThread(),
                PInvoke.GetCurrentProcess(),
                &target,
                0,
                false,
                DUPLICATE_HANDLE_OPTIONS.DUPLICATE_SAME_ACCESS);

            _handle = target;

            _id = PInvokeCore.GetCurrentThreadId();
            _messageLoopCount = 0;
            t_currentThreadContext = this;

            lock (s_lock)
            {
                s_contextHash[_id] = this;
            }
        }

        public ApplicationContext? ApplicationContext { get; private set; }

        public virtual void EnsureReadyForIdle() { }

        internal bool CustomThreadExceptionHandlerAttached => _threadExceptionHandler is not null;

        /// <summary>
        ///  Retrieves the actual parking form. This will demand create the parking window if it needs to.
        /// </summary>
        internal ParkingWindow GetParkingWindow(DPI_AWARENESS_CONTEXT context)
        {
            lock (_parkingWindows)
            {
                ParkingWindow? parkingWindow = GetParkingWindowForContext(context);
                if (parkingWindow is null)
                {
#if DEBUG
                    if (CoreSwitches.PerfTrack.Enabled)
                    {
                        Debug.WriteLine("Creating parking form!");
                        Debug.WriteLine(CoreSwitches.PerfTrack.Enabled, Environment.StackTrace);
                    }
#endif

                    using (ScaleHelper.EnterDpiAwarenessScope(context))
                    {
                        parkingWindow = new ParkingWindow();
                        s_parkingWindowCreated = true;
                    }

                    _parkingWindows.Add(parkingWindow);
                }

                return parkingWindow;
            }
        }

        /// <summary>
        ///  Returns existing parking window that matches the given dpi awareness context, if one exists.
        /// </summary>
        private ParkingWindow? GetParkingWindowForContext(DPI_AWARENESS_CONTEXT context)
        {
            if (_parkingWindows.Count == 0)
            {
                return null;
            }

            // Legacy OS/target framework scenario where ControlDpiContext is set to DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNSPECIFIED
            // because of 'ThreadContextDpiAwareness' API unavailability or this feature is not enabled.
            if (context.IsEquivalent(DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT))
            {
                Debug.Assert(_parkingWindows.Count == 1, "parkingWindows count can not be > 1 for legacy OS/target framework versions");

                return _parkingWindows[0];
            }

            // Supported OS scenario.
            foreach (ParkingWindow window in _parkingWindows)
            {
                if (context.IsEquivalent(window.DpiAwarenessContext))
                {
                    return window;
                }
            }

            // Parking window is not yet created for the requested DpiAwarenessContext
            return null;
        }

        internal Control? ActivatingControl
        {
            get => _activatingControlRef?.TryGetTarget(out Control? target) ?? false ? target : null;
            set => _activatingControlRef = value is null ? null : new(value);
        }

        /// <summary>
        ///  Retrieves the thread's marshalling control.
        /// </summary>
        internal Control MarshallingControl
        {
            get
            {
                if (_marshallingControl is { } control)
                {
                    return control;
                }

                lock (_marshallingControlLock)
                {
                    if (_marshallingControl is null)
                    {
#if DEBUG
                        if (CoreSwitches.PerfTrack.Enabled)
                        {
                            Debug.WriteLine("Creating marshalling control!");
                            Debug.WriteLine(CoreSwitches.PerfTrack.Enabled, Environment.StackTrace);
                        }
#endif

                        _marshallingControl = new ContextMarshallingControl();
                    }

                    return _marshallingControl;
                }
            }
        }

        /// <summary>
        ///  Allows you to setup a message filter for the application's message pump. This
        ///  installs the filter on the current thread.
        /// </summary>
        internal void AddMessageFilter(IMessageFilter? filter)
        {
            _messageFilters ??= [];
            _messageFilterSnapshot ??= [];

            if (filter is not null)
            {
                _filterSnapshotValid = false;
                if (_messageFilters.Count > 0 && filter is IMessageModifyAndFilter)
                {
                    // Insert the IMessageModifyAndFilter filters first
                    _messageFilters.Insert(0, filter);
                }
                else
                {
                    _messageFilters.Add(filter);
                }
            }
        }

        // Called immediately before we begin pumping messages for a modal message loop.
        internal unsafe void BeginModalMessageLoop(ApplicationContext? context)
        {
#if DEBUG
            _debugModalCounter++;
#endif
            BeginModalMessageLoop();

            // This will initialize the ThreadWindows with proper flags.
            DisableWindowsForModalLoop(onlyWinForms: false, context);

            _modalCount++;

            if (_enterModalHandler is not null && _modalCount == 1)
            {
                _enterModalHandler(Thread.CurrentThread, EventArgs.Empty);
            }
        }

        protected virtual void BeginModalMessageLoop() { }

        // Disables windows in preparation of going modal. If parameter is true, we disable all
        // windows, if false, only windows forms windows (i.e., windows controlled by this MsoComponent).
        // See also IMsoComponent.OnEnterState.
        internal void DisableWindowsForModalLoop(bool onlyWinForms, ApplicationContext? context)
        {
            ThreadWindows? old = _threadWindows;
            _threadWindows = new ThreadWindows(onlyWinForms);
            _threadWindows.Enable(false);
            _threadWindows._previousThreadWindows = old;

            if (context is ModalApplicationContext modalContext)
            {
                modalContext.DisableThreadWindows(true, onlyWinForms);
            }
        }

        protected virtual void Dispose(bool disposing) { }

        private void DisposeInternal(bool disposing)
        {
            // Want to ensure both paths are guarded against double disposal.
            if (Interlocked.Exchange(ref _disposed, 1) == 1)
            {
                return;
            }

            Dispose(disposing);

            try
            {
                // We can only clean up if we're being called on our own thread.
                if (PInvokeCore.GetCurrentThreadId() != _id)
                {
                    Debug.Assert(!disposing, "Shouldn't be getting dispose from another thread.");
                    return;
                }

                DisposeThreadWindows();

                try
                {
                    RaiseThreadExit();
                }
                finally
                {
                    if (_oleInitialized && !_externalOleInit)
                    {
                        _oleInitialized = false;
                        PInvoke.OleUninitialize();
                    }
                }
            }
            finally
            {
                // We can always clean up this handle though.
                if (!_handle.IsNull)
                {
                    PInvoke.CloseHandle(_handle);
                    _handle = HANDLE.Null;
                }

                try
                {
                    if (s_totalMessageLoopCount == 0)
                    {
                        RaiseExit();
                    }
                }
                finally
                {
                    lock (s_lock)
                    {
                        s_contextHash.Remove(_id);
                    }

                    if (t_currentThreadContext == this)
                    {
                        t_currentThreadContext = null;
                    }
                }
            }
        }

        /// <summary>
        ///  Disposes this thread context object. Note that this will marshal to the owning thread.
        /// </summary>
        public void Dispose(bool postQuit, bool disposing = true)
        {
            // Unravel our message loop. This will marshal us over to the right thread, making the dispose() method async.
            if (_messageLoopCount > 0 && postQuit)
            {
                PostQuit();
                return;
            }

            DisposeInternal(disposing);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        ///  Disposes of this thread's parking form.
        /// </summary>
        private void DisposeParkingWindow()
        {
            if (_parkingWindows.Count != 0)
            {
                // We take two paths here. If we are on the same thread as
                // the parking window, we can destroy its handle. If not,
                // we just null it and let it GC. When it finalizes it
                // will disconnect its handle and post a WM_CLOSE.
                //
                // It is important that we just call DestroyHandle here
                // and do not call Dispose. Otherwise we would destroy
                // controls that are living on the parking window.
                uint hwndThread = PInvoke.GetWindowThreadProcessId(_parkingWindows[0], out _);
                uint currentThread = PInvokeCore.GetCurrentThreadId();

                for (int i = 0; i < _parkingWindows.Count; i++)
                {
                    if (hwndThread == currentThread)
                    {
                        _parkingWindows[i].Destroy();
                    }
                }

                _parkingWindows.Clear();
            }
        }

        /// <summary>
        ///  Gets rid of all windows in this thread context. Nulls out window objects that we hang on to.
        /// </summary>
        internal void DisposeThreadWindows()
        {
            // We dispose the main window first, so it can perform any cleanup that it may need to do.
            try
            {
                ApplicationContext?.Dispose();
                ApplicationContext = null;

                // Then, we rudely destroy all of the windows on the thread
                ThreadWindows tw = new(onlyWinForms: true);
                tw.Dispose();

                // And dispose the parking form, if it isn't already
                DisposeParkingWindow();
            }
            catch
            {
            }
        }

        // Enables windows in preparation of stopping modal. If parameter is true, we enable all windows,
        // if false, only windows forms windows (i.e., windows controlled by this MsoComponent).
        // See also IMsoComponent.OnEnterState.
        internal void EnableWindowsForModalLoop(bool onlyWinForms, ApplicationContext? context)
        {
            if (_threadWindows is not null)
            {
                _threadWindows.Enable(true);
                _threadWindows = _threadWindows._previousThreadWindows;
            }

            if (context is ModalApplicationContext modalContext)
            {
                modalContext.DisableThreadWindows(false, onlyWinForms);
            }
        }

        // Called immediately after we end pumping messages for a modal message loop.
        internal unsafe void EndModalMessageLoop(ApplicationContext? context)
        {
#if DEBUG
            _debugModalCounter--;
            Debug.Assert(_debugModalCounter >= 0, "Mis-matched calls to Application.BeginModalMessageLoop() and Application.EndModalMessageLoop()");
#endif
            // This will re-enable the windows.
            EnableWindowsForModalLoop(onlyWinForms: false, context);
            EndModalMessageLoop();

            _modalCount--;

            if (_leaveModalHandler is not null && _modalCount == 0)
            {
                _leaveModalHandler(Thread.CurrentThread, EventArgs.Empty);
            }
        }

        protected virtual void EndModalMessageLoop() { }

        /// <summary>
        ///  Exits the program by disposing of all thread contexts and message loops.
        /// </summary>
        internal static void ExitApplication() => ExitCommon(disposing: true);

        private static void ExitCommon(bool disposing)
        {
            lock (s_lock)
            {
                if (s_contextHash is not null)
                {
                    ThreadContext[] contexts = new ThreadContext[s_contextHash.Values.Count];
                    s_contextHash.Values.CopyTo(contexts, 0);
                    for (int i = 0; i < contexts.Length; ++i)
                    {
                        if (contexts[i].ApplicationContext is ApplicationContext context)
                        {
                            context.ExitThread();
                        }
                        else
                        {
                            contexts[i].Dispose(disposing);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Our finalization. This shouldn't be called as we should always be disposed.
        /// </summary>
        ~ThreadContext()
        {
            // Don't call OleUninitialize as the finalizer is called on the wrong thread.
            // We can always clean up this handle, though.
            if (!_handle.IsNull)
            {
                PInvoke.CloseHandle(_handle);
                _handle = HANDLE.Null;
            }
        }

        // When a Form receives a WM_ACTIVATE message, it calls this method so we can do the
        // appropriate MsoComponentManager activation magic
        internal virtual void FormActivated(bool activate)
        {
        }

        /// <summary>
        ///  Sets this component as the tracking component - trumping any active component for message filtering.
        /// </summary>
        internal virtual void TrackInput(bool track)
        {
        }

        /// <summary>
        ///  Retrieves a ThreadContext object for the current thread
        /// </summary>
        internal static ThreadContext FromCurrent() => t_currentThreadContext ?? Create();

        private static ThreadContext Create()
        {
            ThreadContext context = LocalAppContextSwitches.EnableMsoComponentManager
                ? new ComponentThreadContext()
                : new LightThreadContext();

            return context;
        }

        /// <summary>
        ///  Retrieves a ThreadContext object for the given thread ID
        /// </summary>
        internal static ThreadContext? FromId(uint id)
        {
            if (!s_contextHash.TryGetValue(id, out ThreadContext? context) && id == PInvokeCore.GetCurrentThreadId())
            {
                context = Create();
            }

            return context;
        }

        /// <summary>
        ///  Determines if it is OK to allow an application to quit and shutdown
        ///  the runtime. We only allow this if we own the base message pump.
        /// </summary>
        internal static bool GetAllowQuit()
            => s_totalMessageLoopCount > 0 && s_baseLoopReason == msoloop.Main;

        /// <summary>
        ///  Retrieves the handle to this thread.
        /// </summary>
        public HANDLE Handle => _handle;

        HANDLE IHandle<HANDLE>.Handle => Handle;

        /// <summary>
        ///  Retrieves the ID of this thread.
        /// </summary>
        internal uint GetId() => _id;

        /// <summary>
        ///  Determines if a message loop exists on this thread.
        /// </summary>
        internal bool GetMessageLoop() => GetMessageLoop(mustBeActive: false);

        /// <summary>
        ///  Determines if a message loop exists on this thread.
        /// </summary>
        internal unsafe bool GetMessageLoop(bool mustBeActive)
        {
            bool? loopExists = GetMessageLoopInternal(mustBeActive, _messageLoopCount);
            if (loopExists.HasValue)
            {
                return loopExists.Value;
            }

            // Finally, check if a message loop has been registered
            MessageLoopCallback? callback = _messageLoopCallback;
            if (callback is not null)
            {
                return callback();
            }

            // Otherwise, we do not have a loop running.
            return false;
        }

        protected virtual bool? GetMessageLoopInternal(bool mustBeActive, int loopCount) => null;

        internal unsafe ApartmentState OleRequired()
        {
            if (!_oleInitialized)
            {
                HRESULT hr = PInvoke.OleInitialize(pvReserved: (void*)null);

                _oleInitialized = true;
                if (hr == HRESULT.RPC_E_CHANGED_MODE)
                {
                    // This could happen if the thread was already initialized for MTA
                    // and then we call OleInitialize which tries to initialize it for STA
                    // This currently happens while profiling.
                    _externalOleInit = true;
                }
            }

            return _externalOleInit ? ApartmentState.MTA : ApartmentState.STA;
        }

        private void OnAppThreadExit(object? sender, EventArgs e) => Dispose(postQuit: true);

        /// <summary>
        ///  Called when an un-trapped exception occurs in a thread. This allows the programmer to trap these, and, if
        ///  left un-trapped, throws a standard error dialog.
        /// </summary>
        internal void OnThreadException(Exception ex)
        {
            if (_inThreadException)
            {
                return;
            }

            _inThreadException = true;
            try
            {
                if (_threadExceptionHandler is not null)
                {
                    _threadExceptionHandler(Thread.CurrentThread, new ThreadExceptionEventArgs(ex));
                }
                else
                {
                    if (LocalAppContextSwitches.DoNotCatchUnhandledExceptions)
                    {
                        ExceptionDispatchInfo.Capture(ex).Throw();
                    }

                    if (SystemInformation.UserInteractive)
                    {
                        ThreadExceptionDialog dialog = new(ex);
                        DialogResult result = DialogResult.OK;

                        try
                        {
                            result = dialog.ShowDialog();
                        }
                        finally
                        {
                            dialog.Dispose();
                        }

                        switch (result)
                        {
                            case DialogResult.Abort:
                                Exit();
                                Environment.Exit(0);
                                break;
                            case DialogResult.Yes:
                                if (ex is WarningException warning)
                                {
                                    Help.ShowHelp(null, warning.HelpUrl, warning.HelpTopic);
                                }

                                break;
                        }
                    }
                    else
                    {
                        // Ignore unhandled thread exceptions. The user can
                        // override if they really care.
                    }
                }
            }
            finally
            {
                _inThreadException = false;
            }
        }

        internal void PostQuit()
        {
            // Per KB 183116: https://web.archive.org/web/20070510025823/http://support.microsoft.com/kb/183116
            //
            // WM_QUIT may be consumed by another message pump under very specific circumstances.
            // When that occurs, we rely on the STATE_POSTEDQUIT to be caught in the next
            // idle, at which point we can tear down.
            //
            // We can't follow the KB article exactly, because we don't have an HWND to PostMessage to.
            PInvoke.PostThreadMessage(_id, PInvokeCore.WM_QUIT, default, default);
            PostedQuit = true;
        }

        /// <summary>
        ///  Allows the hosting environment to register a callback
        /// </summary>
        internal void RegisterMessageLoop(MessageLoopCallback? callback) => _messageLoopCallback = callback;

        /// <summary>
        ///  Removes a message filter previously installed with addMessageFilter.
        /// </summary>
        internal void RemoveMessageFilter(IMessageFilter f)
        {
            if (_messageFilters is not null)
            {
                _filterSnapshotValid = false;
                _messageFilters.Remove(f);
            }
        }

        /// <summary>
        ///  Starts a message loop for the given reason.
        /// </summary>
        internal void RunMessageLoop(msoloop reason, ApplicationContext? context)
        {
            // Ensure that we attempt to apply theming before doing anything that might create a window.
            using ThemingScope scope = new(UseVisualStyles);
            RunMessageLoopInner(reason, context);
        }

        private void RunMessageLoopInner(msoloop reason, ApplicationContext? context)
        {
            if (reason == msoloop.ModalForm && !SystemInformation.UserInteractive)
            {
                throw new InvalidOperationException(SR.CantShowModalOnNonInteractive);
            }

            // If we've entered because of a Main message loop being pushed
            // (different than a modal message loop or DoEVents loop)
            // then clear the QUIT flag to allow normal processing.
            // this flag gets set during loop teardown for another form.
            if (reason == msoloop.Main)
            {
                PostedQuit = false;
            }

            if (s_totalMessageLoopCount++ == 0)
            {
                s_baseLoopReason = reason;
            }

            _messageLoopCount++;

            if (reason == msoloop.Main)
            {
                // If someone has tried to push another main message loop on this thread, ignore it.
                if (_messageLoopCount != 1)
                {
                    throw new InvalidOperationException(SR.CantNestMessageLoops);
                }

                ApplicationContext = context;
                ApplicationContext!.ThreadExit += OnAppThreadExit;

                if (ApplicationContext.MainForm is not null)
                {
                    ApplicationContext.MainForm.Visible = true;
                }
            }

            Form? oldForm = CurrentForm;
            if (context is not null)
            {
                CurrentForm = context.MainForm;
            }

            bool fullModal = false;
            HWND hwndOwner = default;

            if (reason is msoloop.ModalForm or msoloop.ModalAlert)
            {
                fullModal = true;

                // We're about to disable all windows in the thread so our modal dialog can be the top dog. Because this can interact
                // with external MSO things, and also because the modal dialog could have already had its handle created,
                // Check to see if the handle exists and if the window is currently enabled. We remember this so we can set the
                // window back to enabled after disabling everyone else. This is just a precaution against someone doing the
                // wrong thing and disabling our dialog.

                bool modalEnabled = CurrentForm is not null && CurrentForm.Enabled;

                BeginModalMessageLoop(context);

                // If the owner window of the dialog is still enabled, disable it now.
                // This can happen if the owner window is from a different thread or
                // process.
                if (CurrentForm is not null)
                {
                    hwndOwner = (HWND)PInvokeCore.GetWindowLong(CurrentForm, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT);
                    if (!hwndOwner.IsNull)
                    {
                        if (PInvoke.IsWindowEnabled(hwndOwner))
                        {
                            PInvoke.EnableWindow(hwndOwner, false);
                        }
                        else
                        {
                            // Reset hwndOwner so we are not tempted to fiddle with it
                            hwndOwner = default;
                        }
                    }
                }

                // The second half of the modalEnabled flag above. Here, if we were previously
                // enabled, make sure that's still the case.
                if (CurrentForm is not null && CurrentForm.IsHandleCreated && PInvoke.IsWindowEnabled(CurrentForm) != modalEnabled)
                {
                    PInvoke.EnableWindow(CurrentForm, modalEnabled);
                }
            }

            try
            {
                bool result;

                // Register marshaller for background tasks. At this point,
                // need to be able to successfully get the handle to the
                // parking window. Only do it when we're entering the first
                // message loop for this thread.
                if (_messageLoopCount == 1)
                {
                    WindowsFormsSynchronizationContext.InstallIfNeeded();
                }

                // Need to do this in a try/finally. Also good to do after we installed the synch context.
                if (fullModal && CurrentForm is not null)
                {
                    CurrentForm.Visible = true;
                }

                result = RunMessageLoop(reason, fullModal);
            }
            finally
            {
                if (fullModal)
                {
                    EndModalMessageLoop(context);

                    // Again, if the hwndOwner was valid and disabled above, re-enable it.
                    if (!hwndOwner.IsNull)
                    {
                        PInvoke.EnableWindow(hwndOwner, true);
                    }
                }

                CurrentForm = oldForm;
                s_totalMessageLoopCount--;
                _messageLoopCount--;

                if (_messageLoopCount == 0)
                {
                    // Last message loop shutting down, restore the sync context that was in place before we started
                    // the first message loop.
                    WindowsFormsSynchronizationContext.Uninstall(turnOffAutoInstall: false);
                }

                if (reason == msoloop.Main)
                {
                    Dispose(postQuit: true, disposing: true);
                }
                else if (_messageLoopCount == 0)
                {
                    EndOuterMessageLoop();
                }
            }
        }

        protected abstract bool RunMessageLoop(msoloop reason, bool fullModal);

        protected virtual void EndOuterMessageLoop() { }

        internal bool ProcessFilters(ref MSG msg, out bool modified)
        {
            bool filtered = false;

            modified = false;

            // Account for the case where someone removes a message filter as a result of PreFilterMessage.
            // The message filter will be removed from the _next_ message.

            // If message filter is added or removed inside the user-provided PreFilterMessage function,
            // and user code pumps messages, we might re-enter ProcessFilter on the same stack, we
            // should not update the snapshot until the next message.
            if (_messageFilters is not null && !_filterSnapshotValid && _inProcessFilters == 0)
            {
                if (_messageFilterSnapshot is not null)
                {
                    _messageFilterSnapshot.Clear();
                    if (_messageFilters.Count > 0)
                    {
                        _messageFilterSnapshot.AddRange(_messageFilters);
                    }
                }

                _filterSnapshotValid = true;
            }

            _inProcessFilters++;
            try
            {
                if (_messageFilterSnapshot is not null && _messageFilterSnapshot.Count != 0)
                {
                    IMessageFilter filter;
                    int count = _messageFilterSnapshot.Count;

                    Message message = Message.Create(msg.hwnd, msg.message, msg.wParam, msg.lParam);

                    for (int i = 0; i < count; i++)
                    {
                        filter = _messageFilterSnapshot[i];
                        bool filterMessage = filter.PreFilterMessage(ref message);

                        // Make sure that we update the msg struct with the new result after the call to
                        // PreFilterMessage.
                        if (filter is IMessageModifyAndFilter)
                        {
                            msg.hwnd = (HWND)message.HWnd;
                            msg.message = (uint)message.MsgInternal;
                            msg.wParam = message.WParamInternal;
                            msg.lParam = message.LParamInternal;
                            modified = true;
                        }

                        if (filterMessage)
                        {
                            filtered = true;
                            break;
                        }
                    }
                }
            }
            finally
            {
                _inProcessFilters--;
            }

            return filtered;
        }

        /// <summary>
        ///  Message filtering routine that is called before dispatching a message.
        ///  If this returns true, the message is already processed. If it returns
        ///  false, the message should be allowed to continue through the dispatch
        ///  mechanism.
        /// </summary>
        internal bool PreTranslateMessage(ref MSG msg)
        {
            if (ProcessFilters(ref msg, out _))
            {
                return true;
            }

            if (!msg.IsKeyMessage())
            {
                return false;
            }

            if (msg.message == PInvokeCore.WM_CHAR)
            {
                // 1 = extended keyboard, 46 = scan code
                int breakLParamMask = 0x1460000;
                if ((int)(uint)msg.wParam == 3 && ((int)msg.lParam & breakLParamMask) == breakLParamMask)
                {
                    // wParam is the key character, which for ctrl-brk is the same as ctrl-C.
                    // So we need to go to the lparam to distinguish the two cases.
                    // You might also be able to do this with WM_KEYDOWN (again with wParam=3)

                    if (Debugger.IsAttached)
                    {
                        Debugger.Break();
                    }
                }
            }

            Control? target = Control.FromChildHandle(msg.hwnd);
            bool retValue = false;

            Message m = Message.Create(msg.hwnd, msg.message, msg.wParam, msg.lParam);

            if (target is not null)
            {
                if (NativeWindow.WndProcShouldBeDebuggable)
                {
                    // We don't want to do a catch in the debuggable case.
                    if (Control.PreProcessControlMessageInternal(target, ref m) == PreProcessControlState.MessageProcessed)
                    {
                        retValue = true;
                    }
                }
                else
                {
                    try
                    {
                        if (Control.PreProcessControlMessageInternal(target, ref m) == PreProcessControlState.MessageProcessed)
                        {
                            retValue = true;
                        }
                    }
                    catch (Exception e)
                    {
                        OnThreadException(e);
                    }
                }
            }
            else
            {
                // See if this is a dialog message -- this is for handling any native dialogs that are launched from
                // WinForms code. This can happen with ActiveX controls that launch dialogs specifically

                // First, get the first top-level window in the hierarchy.
                HWND hwndRoot = PInvoke.GetAncestor(msg.hwnd, GET_ANCESTOR_FLAGS.GA_ROOT);

                // If we got a valid HWND, then call IsDialogMessage on it. If that returns true, it's been processed
                // so we should return true to prevent Translate/Dispatch from being called.
                if (!hwndRoot.IsNull && PInvoke.IsDialogMessage(hwndRoot, in msg))
                {
                    return true;
                }
            }

            msg.wParam = m.WParamInternal;
            msg.lParam = m.LParamInternal;

            if (retValue)
            {
                return true;
            }

            return false;
        }
    }
}

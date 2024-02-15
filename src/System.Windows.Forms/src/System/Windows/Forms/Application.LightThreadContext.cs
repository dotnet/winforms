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
    ///  This class is the embodiment of TLS for windows forms.  We do not expose this to end users because
    ///  TLS is really just an unfortunate artifact of using Win 32.  We want the world to be free
    ///  threaded.
    /// </summary>
    internal unsafe sealed class LightThreadContext :
        MarshalByRefObject,
        IHandle<HANDLE>
    {
        private bool _oleInitialized;
        private bool _externalOleInit;
        private bool _inThreadException;
        private bool _postedQuit;
        private bool _filterSnapshotValid;

        private static readonly Dictionary<uint, LightThreadContext> s_contextHash = [];

        // When this gets to zero, we'll invoke a full garbage
        // collect and check for root/window leaks.
        private static readonly object s_tcInternalSyncObject = new();

        private static int s_totalMessageLoopCount;
        private static msoloop s_baseLoopReason;

        [ThreadStatic]
        private static LightThreadContext? t_currentThreadContext;

        internal ThreadExceptionEventHandler? _threadExceptionHandler;
        internal EventHandler? _idleHandler;
        internal EventHandler? _enterModalHandler;
        internal EventHandler? _leaveModalHandler;

        // Parking window list
        private readonly List<ParkingWindow> _parkingWindows = [];
        private Control? _marshalingControl;
        private List<IMessageFilter>? _messageFilters;
        private List<IMessageFilter>? _messageFilterSnapshot;
        private int _inProcessFilters;
        private HANDLE _handle;
        private readonly uint _id;
        private int _messageLoopCount;
        private int _modalCount;

        // Used for correct restoration of focus after modality
        private WeakReference<Control>? _activatingControlRef;

        private Form? _currentForm;
        private ThreadWindows? _threadWindows;
        private int _disposeCount;   // To make sure that we don't allow
                                     // reentrancy in Dispose()

        // Debug helper variable
#if DEBUG
        private int _debugModalCounter;
#endif
        // We need to set this flag if we have started the ModalMessageLoop so that we don't create the ThreadWindows
        // when the ComponentManager calls on us (as IMSOComponent) during the OnEnterState.
        private bool _ourModalLoop;

        // A private field on Application that stores the callback delegate
        private MessageLoopCallback? _messageLoopCallback;

        /// <summary>
        ///  Creates a new thread context object.
        /// </summary>
        public unsafe LightThreadContext()
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

            _id = PInvoke.GetCurrentThreadId();
            _messageLoopCount = 0;
            t_currentThreadContext = this;

            lock (s_tcInternalSyncObject)
            {
                s_contextHash[_id] = this;
            }
        }

        public ApplicationContext? ApplicationContext { get; private set; }

        internal bool CustomThreadExceptionHandlerAttached => _threadExceptionHandler is not null;

        /// <summary>
        ///  Retrieves the actual parking form.  This will demand create the parking window
        ///  if it needs to.
        /// </summary>
        internal ParkingWindow GetParkingWindow(DPI_AWARENESS_CONTEXT context)
        {
            // Locking 'this' here is ok since this is an internal class.
            lock (this)
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
        ///  Returns parking window that matches dpi awareness context. return null if not found.
        /// </summary>
        /// <returns>return matching parking window from list. returns null if not found</returns>
        internal ParkingWindow? GetParkingWindowForContext(DPI_AWARENESS_CONTEXT context)
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
            foreach (ParkingWindow p in _parkingWindows)
            {
                if (context.IsEquivalent(p.DpiAwarenessContext))
                {
                    return p;
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
        ///  Retrieves the actual parking form.  This will demand create the MarshalingControl window
        ///  if it needs to.
        /// </summary>
        internal Control MarshalingControl
        {
            get
            {
                lock (this)
                {
                    if (_marshalingControl is null)
                    {
#if DEBUG
                        if (CoreSwitches.PerfTrack.Enabled)
                        {
                            Debug.WriteLine("Creating marshalling control!");
                            Debug.WriteLine(CoreSwitches.PerfTrack.Enabled, Environment.StackTrace);
                        }
#endif

                        _marshalingControl = new MarshalingControl();
                    }

                    return _marshalingControl;
                }
            }
        }

        /// <summary>
        ///  Allows you to setup a message filter for the application's message pump.  This
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
                    // insert the IMessageModifyAndFilter filters first
                    _messageFilters.Insert(0, filter);
                }
                else
                {
                    _messageFilters.Add(filter);
                }
            }
        }

        // Called immediately before we begin pumping messages for a modal message loop.
        internal void BeginModalMessageLoop(ApplicationContext? context)
        {
#if DEBUG
            _debugModalCounter++;
#endif
            // Set the ourModalLoop flag so that the "IMSOComponent.OnEnterState" is a NOOP since we started the ModalMessageLoop.
            bool wasOurLoop = _ourModalLoop;
            _ourModalLoop = true;
            try
            {
                OnEnterState(msocstate.Modal, BOOL.TRUE);
            }
            finally
            {
                _ourModalLoop = wasOurLoop;
            }

            // This will initialize the ThreadWindows with proper flags.
            DisableWindowsForModalLoop(onlyWinForms: false, context);

            _modalCount++;

            if (_enterModalHandler is not null && _modalCount == 1)
            {
                _enterModalHandler(Thread.CurrentThread, EventArgs.Empty);
            }
        }

        // Disables windows in preparation of going modal.  If parameter is true, we disable all
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

        /// <summary>
        ///  Disposes this thread context object.  Note that this will marshal to the owning thread.
        /// </summary>
        internal void Dispose(bool postQuit)
        {
            // Need to avoid multiple threads coming in here or we'll leak the thread handle.
            lock (this)
            {
                try
                {
                    // Make sure that we are not reentrant
                    if (_disposeCount++ != 0)
                    {
                        return;
                    }

                    // Unravel our message loop. This will marshal us over to the right thread, making the dispose() method async.
                    if (_messageLoopCount > 0 && postQuit)
                    {
                        PostQuit();
                    }
                    else
                    {
                        bool ourThread = PInvoke.GetCurrentThreadId() == _id;

                        try
                        {
                            // We can only clean up if we're being called on our own thread.
                            if (!ourThread)
                            {
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
                                PInvoke.CloseHandle(this);
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
                                lock (s_tcInternalSyncObject)
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

                    GC.SuppressFinalize(this);
                }
                finally
                {
                    _disposeCount--;
                }
            }
        }

        /// <summary>
        ///  Disposes of this thread's parking form.
        /// </summary>
        private void DisposeParkingWindow()
        {
            if (_parkingWindows.Count != 0)
            {
                // We take two paths here.  If we are on the same thread as
                // the parking window, we can destroy its handle.  If not,
                // we just null it and let it GC.  When it finalizes it
                // will disconnect its handle and post a WM_CLOSE.
                //
                // It is important that we just call DestroyHandle here
                // and do not call Dispose.  Otherwise we would destroy
                // controls that are living on the parking window.
                uint hwndThread = PInvoke.GetWindowThreadProcessId(_parkingWindows[0], out _);
                uint currentThread = PInvoke.GetCurrentThreadId();

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
        ///  Gets rid of all windows in this thread context.  Nulls out
        ///  window objects that we hang on to.
        /// </summary>
        internal void DisposeThreadWindows()
        {
            // We dispose the main window first, so it can perform any
            // cleanup that it may need to do.
            try
            {
                if (ApplicationContext is not null)
                {
                    ApplicationContext.Dispose();
                    ApplicationContext = null;
                }

                // Then, we rudely destroy all of the windows on the thread
                ThreadWindows tw = new(true);
                tw.Dispose();

                // And dispose the parking form, if it isn't already
                DisposeParkingWindow();
            }
            catch
            {
            }
        }

        // Enables windows in preparation of stopping modal.  If parameter is true, we enable all windows,
        // if false, only windows forms windows (i.e., windows controlled by this MsoComponent).
        // See also IMsoComponent.OnEnterState.
        internal void EnableWindowsForModalLoop(bool onlyWinForms, ApplicationContext? context)
        {
            if (_threadWindows is not null)
            {
                _threadWindows.Enable(true);
                Debug.Assert(_threadWindows is not null, "OnEnterState recursed, but it's not supposed to be reentrant");
                _threadWindows = _threadWindows._previousThreadWindows;
            }

            if (context is ModalApplicationContext modalContext)
            {
                modalContext.DisableThreadWindows(false, onlyWinForms);
            }
        }

        // Called immediately after we end pumping messages for a modal message loop.
        internal void EndModalMessageLoop(ApplicationContext? context)
        {
#if DEBUG
            _debugModalCounter--;
            Debug.Assert(_debugModalCounter >= 0, "Mis-matched calls to Application.BeginModalMessageLoop() and Application.EndModalMessageLoop()");
#endif
            // This will re-enable the windows...
            EnableWindowsForModalLoop(false, context); // onlyWinForms = false

            bool wasOurLoop = _ourModalLoop;
            _ourModalLoop = true;
            try
            {
                // If We started the ModalMessageLoop .. this will call us back on the IMSOComponent.OnStateEnter and not do anything
                OnEnterState(msocstate.Modal, false);
            }
            finally
            {
                // Reset the flag since we are exiting out of a ModalMessageLoop.
                _ourModalLoop = wasOurLoop;
            }

            _modalCount--;

            if (_leaveModalHandler is not null && _modalCount == 0)
            {
                _leaveModalHandler(Thread.CurrentThread, EventArgs.Empty);
            }
        }

        /// <summary>
        ///  Exits the program by disposing of all thread contexts and message loops.
        /// </summary>
        internal static void ExitApplication() => ExitCommon(disposing: true);

        private static void ExitCommon(bool disposing)
        {
            lock (s_tcInternalSyncObject)
            {
                if (s_contextHash is not null)
                {
                    LightThreadContext[] ctxs = new LightThreadContext[s_contextHash.Values.Count];
                    s_contextHash.Values.CopyTo(ctxs, 0);
                    for (int i = 0; i < ctxs.Length; ++i)
                    {
                        if (ctxs[i].ApplicationContext is ApplicationContext context)
                        {
                            context.ExitThread();
                        }
                        else
                        {
                            ctxs[i].Dispose(disposing);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Our finalization. This shouldn't be called as we should always be disposed.
        /// </summary>
        ~LightThreadContext()
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
        internal void FormActivated(bool activate)
        {
        }

        /// <summary>
        ///  Sets this component as the tracking component - trumping any active component
        ///  for message filtering.
        /// </summary>
        internal void TrackInput(bool track)
        {
        }

        /// <summary>
        ///  Retrieves a ThreadContext object for the current thread
        /// </summary>
        internal static LightThreadContext FromCurrent() => t_currentThreadContext ?? new LightThreadContext();

        /// <summary>
        ///  Retrieves a ThreadContext object for the given thread ID
        /// </summary>
        internal static LightThreadContext? FromId(uint id)
        {
            if (!s_contextHash.TryGetValue(id, out LightThreadContext? context) && id == PInvoke.GetCurrentThreadId())
            {
                context = new LightThreadContext();
            }

            return context;
        }

        /// <summary>
        ///  Determines if it is OK to allow an application to quit and shutdown
        ///  the runtime.  We only allow this if we own the base message pump.
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
        internal bool GetMessageLoop(bool mustBeActive)
        {
            // If we are already running a loop, we're fine.
            // If we are running in external manager we may need to make sure first the loop is active
            if (_messageLoopCount > 0)
            {
                return true;
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

        /// <summary>
        ///  A method of determining whether we are handling messages that does not demand register
        ///  the component manager.
        /// </summary>
        internal bool IsValidComponentId() => true;

        internal ApartmentState OleRequired()
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

            return _externalOleInit? ApartmentState.MTA : ApartmentState.STA;
        }

        private void OnAppThreadExit(object? sender, EventArgs e)
            => Dispose(postQuit: true);

        /// <summary>
        ///  Called when an untrapped exception occurs in a thread. This allows the programmer to trap these, and, if
        ///  left untrapped, throws a standard error dialog.
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
            PInvoke.PostThreadMessage(_id, PInvoke.WM_QUIT, default, default);
            _postedQuit = true;
        }

        /// <summary>
        ///  Allows the hosting environment to register a callback
        /// </summary>
        internal void RegisterMessageLoop(MessageLoopCallback? callback)
        {
            _messageLoopCallback = callback;
        }

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
                _postedQuit = false;
            }

            if (s_totalMessageLoopCount++ == 0)
            {
                s_baseLoopReason = reason;
            }

            _messageLoopCount++;

            if (reason == msoloop.Main)
            {
                // If someone has tried to push another main message loop on this thread,
                // ignore it.
                if (_messageLoopCount != 1)
                {
                    throw new InvalidOperationException(SR.CantNestMessageLoops);
                }

                ApplicationContext = context;

                ApplicationContext!.ThreadExit += new EventHandler(OnAppThreadExit);

                if (ApplicationContext.MainForm is not null)
                {
                    ApplicationContext.MainForm.Visible = true;
                }
            }

            Form? oldForm = _currentForm;
            if (context is not null)
            {
                _currentForm = context.MainForm;
            }

            bool fullModal = false;
            HWND hwndOwner = default;

            if (reason is msoloop.ModalForm or msoloop.ModalAlert)
            {
                fullModal = true;

                // We're about to disable all windows in the thread so our modal dialog can be the top dog.  Because this can interact
                // with external MSO things, and also because the modal dialog could have already had its handle created,
                // Check to see if the handle exists and if the window is currently enabled. We remember this so we can set the
                // window back to enabled after disabling everyone else.  This is just a precaution against someone doing the
                // wrong thing and disabling our dialog.

                bool modalEnabled = _currentForm is not null && _currentForm.Enabled;

                BeginModalMessageLoop(context);

                // If the owner window of the dialog is still enabled, disable it now.
                // This can happen if the owner window is from a different thread or
                // process.
                if (_currentForm is not null)
                {
                    hwndOwner = (HWND)PInvoke.GetWindowLong(_currentForm, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT);
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

                // The second half of the modalEnabled flag above.  Here, if we were previously
                // enabled, make sure that's still the case.
                if (_currentForm is not null && _currentForm.IsHandleCreated && PInvoke.IsWindowEnabled(_currentForm) != modalEnabled)
                {
                    PInvoke.EnableWindow(_currentForm, modalEnabled);
                }
            }

            try
            {
                bool result;

                // Register marshaller for background tasks.  At this point,
                // need to be able to successfully get the handle to the
                // parking window.  Only do it when we're entering the first
                // message loop for this thread.
                if (_messageLoopCount == 1)
                {
                    WindowsFormsSynchronizationContext.InstallIfNeeded();
                }

                // Need to do this in a try/finally.  Also good to do after we installed the synch context.
                if (fullModal && _currentForm is not null)
                {
                    _currentForm.Visible = true;
                }

                result = FPushMessageLoop(reason);
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

                _currentForm = oldForm;
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
                    Dispose(true);
                }
            }
        }

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
                    IMessageFilter f;
                    int count = _messageFilterSnapshot.Count;

                    Message m = Message.Create(msg.hwnd, msg.message, msg.wParam, msg.lParam);

                    for (int i = 0; i < count; i++)
                    {
                        f = _messageFilterSnapshot[i];
                        bool filterMessage = f.PreFilterMessage(ref m);

                        // Make sure that we update the msg struct with the new result after the call to
                        // PreFilterMessage.
                        if (f is IMessageModifyAndFilter)
                        {
                            msg.hwnd = (HWND)m.HWnd;
                            msg.message = (uint)m.MsgInternal;
                            msg.wParam = m.WParamInternal;
                            msg.lParam = m.LParamInternal;
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
        ///  If this returns true, the message is already processed.  If it returns
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

            if (msg.message == PInvoke.WM_CHAR)
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
            bool result = false;

            Message m = Message.Create(msg.hwnd, msg.message, msg.wParam, msg.lParam);

            if (target is not null)
            {
                if (NativeWindow.WndProcShouldBeDebuggable)
                {
                    // We don't want to do a catch in the debuggable case.
                    if (Control.PreProcessControlMessageInternal(target, ref m) == PreProcessControlState.MessageProcessed)
                    {
                        result = true;
                    }
                }
                else
                {
                    try
                    {
                        if (Control.PreProcessControlMessageInternal(target, ref m) == PreProcessControlState.MessageProcessed)
                        {
                            result = true;
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
                // WinForms code.  This can happen with ActiveX controls that launch dialogs specifically

                // First, get the first top-level window in the hierarchy.
                HWND hwndRoot = PInvoke.GetAncestor(msg.hwnd, GET_ANCESTOR_FLAGS.GA_ROOT);

                // If we got a valid HWND, then call IsDialogMessage on it.  If that returns true, it's been processed
                // so we should return true to prevent Translate/Dispatch from being called.
                if (!hwndRoot.IsNull && PInvoke.IsDialogMessage(hwndRoot, in msg))
                {
                    return true;
                }
            }

            msg.wParam = m.WParamInternal;
            msg.lParam = m.LParamInternal;

            return result;
        }

        public void OnEnterState(msocstate uStateID, BOOL fEnter)
        {
            // Return if our (WINFORMS) Modal Loop is still running.
            if (_ourModalLoop)
            {
                return;
            }

            if (uStateID == msocstate.Modal)
            {
                // We should only be messing with windows we own.  See the "ctrl-shift-N" test above.
                if (fEnter)
                {
                    DisableWindowsForModalLoop(onlyWinForms: true, null);
                }
                else
                {
                    EnableWindowsForModalLoop(onlyWinForms: true, null);
                }
            }
        }

        public BOOL FContinueMessageLoop(
            msoloop uReason,
            MSG* pMsgPeeked)
        {
            bool continueLoop = true;

            // If we get a null message, and we have previously posted the WM_QUIT message,
            // then someone ate the message.
            if (pMsgPeeked is null && _postedQuit)
            {
                continueLoop = false;
            }
            else
            {
                switch (uReason)
                {
                    case msoloop.FocusWait:

                        // For focus wait, check to see if we are now the active application.
                        PInvoke.GetWindowThreadProcessId(PInvoke.GetActiveWindow(), out uint pid);
                        if (pid == PInvoke.GetCurrentProcessId())
                        {
                            continueLoop = false;
                        }

                        break;

                    case msoloop.ModalAlert:
                    case msoloop.ModalForm:

                        // For modal forms, check to see if the current active form has been
                        // dismissed.  If there is no active form, then it is an error that
                        // we got into here, so we terminate the loop.

                        if (_currentForm is null || _currentForm.CheckCloseDialog(false))
                        {
                            continueLoop = false;
                        }

                        break;

                    case msoloop.DoEvents:
                    case msoloop.DoEventsModal:
                        // For DoEvents, just see if there are more messages on the queue.
                        MSG temp = default;
                        if (!PInvoke.PeekMessage(&temp, HWND.Null, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_NOREMOVE))
                        {
                            continueLoop = false;
                        }

                        break;
                }
            }

            return continueLoop;
        }

        private BOOL FPushMessageLoop(msoloop uReason)
        {
            BOOL continueLoop = true;
            MSG msg = default;

            while (true)
            {
                if (PInvoke.PeekMessage(&msg, HWND.Null, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_NOREMOVE))
                {
                    if (!FContinueMessageLoop(uReason, &msg))
                    {
                        return true;
                    }

                    // If the component wants us to process the message, do it.
                    PInvoke.GetMessage(&msg, HWND.Null, 0, 0);

                    if (msg.message == PInvoke.WM_QUIT)
                    {
                        ThreadContext.FromCurrent().DisposeThreadWindows();

                        if (uReason != msoloop.Main)
                        {
                            PInvoke.PostQuitMessage((int)msg.wParam);
                        }

                        return true;
                    }

                    // Now translate and dispatch the message.
                    if (!PreTranslateMessage(ref msg))
                    {
                        PInvoke.TranslateMessage(msg);
                        PInvoke.DispatchMessage(&msg);
                    }
                }
                else
                {
                    // If this is a DoEvents loop, then get out. There's nothing left for us to do.
                    if (uReason is msoloop.DoEvents or msoloop.DoEventsModal)
                    {
                        break;
                    }

                    // Nothing is on the message queue. Perform idle processing and then do a WaitMessage.
                    _idleHandler?.Invoke(Thread.CurrentThread, EventArgs.Empty);

                    // Give the component one more chance to terminate the message loop.
                    if (!FContinueMessageLoop(uReason, pMsgPeeked: null))
                    {
                        return true;
                    }

                    // We should call GetMessage here, but we cannot because the component manager requires
                    // that we notify the active component before we pull the message off the queue. This is
                    // a bit of a problem, because WaitMessage waits for a NEW message to appear on the
                    // queue. If a message appeared between processing and now WaitMessage would wait for
                    // the next message. We minimize this here by calling PeekMessage.
                    if (!PInvoke.PeekMessage(&msg, HWND.Null, 0, 0, PEEK_MESSAGE_REMOVE_TYPE.PM_NOREMOVE))
                    {
                        PInvoke.WaitMessage();
                    }
                }
            }

            return !continueLoop;
        }
    }
}

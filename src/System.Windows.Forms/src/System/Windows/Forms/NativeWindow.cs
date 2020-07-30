// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides a low-level encapsulation of a window handle
    ///  and a window procedure. The class automatically manages window class creation and registration.
    /// </summary>
    public partial class NativeWindow : MarshalByRefObject, IWin32Window, IHandle
    {
#if DEBUG
        private static readonly BooleanSwitch AlwaysUseNormalWndProc = new BooleanSwitch("AlwaysUseNormalWndProc", "Skips checking for the debugger when choosing the debuggable WndProc handler");
#endif

        private static readonly TraceSwitch WndProcChoice = new TraceSwitch("WndProcChoice", "Info about choice of WndProc");

        private const int InitializedFlags = 0x01;
        private const int UseDebuggableWndProc = 0x04;

        // Do we have any active HWNDs?
        [ThreadStatic]
        private static bool t_anyHandleCreated;
        private static bool s_anyHandleCreatedInApp;

        [ThreadStatic]
        private static byte t_wndProcFlags = 0;
        [ThreadStatic]
        private static byte t_userSetProcFlags = 0;
        private static byte s_userSetProcFlagsForApp;

        // Need to Store Table of Ids and Handles
        private static short s_globalID = 1;
        private static readonly Dictionary<IntPtr, GCHandle> s_windowHandles = new Dictionary<IntPtr, GCHandle>();
        private static readonly Dictionary<short, IntPtr> s_windowIds = new Dictionary<short, IntPtr>();
        private static readonly object s_internalSyncObject = new object();
        private static readonly object s_createWindowSyncObject = new object();

        // Our window procedure delegate
        private User32.WNDPROC _windowProc;

        // The native handle for our delegate
        private IntPtr _windowProcHandle;

        // The native handle for Windows' default window procedure
        private static IntPtr s_defaultWindowProc;

        private IntPtr _priorWindowProcHandle;
        private bool _suppressedGC;
        private bool _ownHandle;
        private NativeWindow _nextWindow;
        private readonly WeakReference _weakThisPtr;

        static NativeWindow()
        {
            AppDomain.CurrentDomain.ProcessExit += OnShutdown;
        }

        public NativeWindow()
        {
            _weakThisPtr = new WeakReference(this);
        }

        /// <summary>
        ///  Cache window DpiContext awareness information that helps to create handle with right context at the later time.
        /// </summary>
        internal IntPtr DpiAwarenessContext { get; } = DpiHelper.IsScalingRequirementMet
            ? User32.GetThreadDpiAwarenessContext()
            : User32.UNSPECIFIED_DPI_AWARENESS_CONTEXT;

        /// <summary>
        ///  Override's the base object's finalize method.
        /// </summary>
        ~NativeWindow()
        {
            ForceExitMessageLoop();
        }

        /// <summary>
        ///  This was factored into another function so the finalizer in control that releases the window
        ///  can perform the exact same code without further changes.  If you make changes to the finalizer,
        ///  change this method -- try not to change NativeWindow's finalizer.
        /// </summary>
        internal void ForceExitMessageLoop()
        {
            IntPtr handle;
            bool ownedHandle;

            lock (this)
            {
                handle = Handle;
                ownedHandle = _ownHandle;
            }

            if (handle != IntPtr.Zero)
            {
                // Now, before we set handle to zero and finish the finalizer, let's send
                // a WM_NULL to the window.  Why?  Because if the main ui thread is INSIDE
                // the wndproc for this control during our unsubclass, then we could AV
                // when control finally reaches us.
                if (User32.IsWindow(handle).IsTrue())
                {
                    uint id = User32.GetWindowThreadProcessId(handle, out uint lpdwProcessId);
                    Application.ThreadContext ctx = Application.ThreadContext.FromId(id);
                    IntPtr threadHandle = (ctx is null ? IntPtr.Zero : ctx.GetHandle());

                    if (threadHandle != IntPtr.Zero)
                    {
                        Kernel32.GetExitCodeThread(threadHandle, out uint exitCode);
                        if (!AppDomain.CurrentDomain.IsFinalizingForUnload() && (NTSTATUS)exitCode == NTSTATUS.STATUS_PENDING)
                        {
                            User32.SendMessageTimeoutW(
                                handle,
                                User32.RegisteredMessage.WM_UIUNSUBCLASS,
                                IntPtr.Zero,
                                IntPtr.Zero,
                                User32.SMTO.ABORTIFHUNG,
                                100,
                                out _);
                        }
                    }
                }

                if (Handle != IntPtr.Zero)
                {
                    // If the dest thread is gone, it should be safe to unsubclass here.
                    ReleaseHandle(true);
                }
            }

            if (handle != IntPtr.Zero && ownedHandle)
            {
                // If we owned the handle, post a WM_CLOSE to get rid of it.
                User32.PostMessageW(handle, User32.WM.CLOSE);
            }
        }

        /// <summary>
        ///  Indicates whether a window handle was created &amp; is being tracked.
        /// </summary>
        internal static bool AnyHandleCreated => t_anyHandleCreated;

        /// <summary>
        ///  Gets the handle for this window.
        /// </summary>
        public IntPtr Handle { get; private set; }

        /// <summary>
        ///  This returns the prior NativeWindow created with the same native handle, if any.
        /// </summary>
        internal NativeWindow PreviousWindow { get; private set; }

        /// <summary>
        ///  Address of the Windows default WNDPROC (DefWindowProcW).
        /// </summary>
        internal static IntPtr DefaultWindowProc
        {
            get
            {
                if (s_defaultWindowProc == IntPtr.Zero)
                {
                    // Cache the default windows procedure address
                    s_defaultWindowProc = Kernel32.GetProcAddress(
                        Kernel32.GetModuleHandleW(Libraries.User32),
                        "DefWindowProcW");

                    if (s_defaultWindowProc == IntPtr.Zero)
                    {
                        throw new Win32Exception();
                    }
                }

                return s_defaultWindowProc;
            }
        }

        private static int WndProcFlags
        {
            get
            {
                // Upcast for easy bit masking...
                int intWndProcFlags = t_wndProcFlags;

                // Check to see if a debugger is installed. If there is, then use DebuggableCallback instead;
                // this callback has no try/catch around it so exceptions go to the debugger.

                if (intWndProcFlags == 0)
                {
                    Debug.WriteLineIf(WndProcChoice.TraceVerbose, "Init wndProcFlags");
                    Debug.Indent();

                    if (t_userSetProcFlags != 0)
                    {
                        intWndProcFlags = t_userSetProcFlags;
                    }
                    else if (s_userSetProcFlagsForApp != 0)
                    {
                        intWndProcFlags = s_userSetProcFlagsForApp;
                    }
                    else if (!Application.CustomThreadExceptionHandlerAttached)
                    {
                        if (Debugger.IsAttached)
                        {
                            Debug.WriteLineIf(WndProcChoice.TraceVerbose, "Debugger is attached, using debuggable WndProc");
                            intWndProcFlags |= UseDebuggableWndProc;
                        }
                        else
                        {
                            // Reading Framework registry key in Netcore/5.0 doesn't make sense. This path seems to be used to override the
                            // default behaviour after applications deployed (otherwise, Developer/user can set this flag
                            // via Application.SetUnhandledExceptionModeInternal(..).
                            // Disabling this feature from .NET core 3.0 release. Would need to redesign if there are customer requests on this.

                            Debug.WriteLineIf(WndProcChoice.TraceVerbose, "Debugger check from registry is not supported in this release of .Net version");
                        }
                    }

#if DEBUG
                    if (AlwaysUseNormalWndProc.Enabled)
                    {
                        Debug.WriteLineIf(WndProcChoice.TraceVerbose, "Stripping debuggablewndproc due to AlwaysUseNormalWndProc switch");
                        intWndProcFlags &= ~UseDebuggableWndProc;
                    }
#endif
                    intWndProcFlags |= InitializedFlags;
                    Debug.WriteLineIf(WndProcChoice.TraceVerbose, "Final 0x" + intWndProcFlags.ToString("X", CultureInfo.InvariantCulture));
                    t_wndProcFlags = (byte)intWndProcFlags;
                    Debug.Unindent();
                }

                return intWndProcFlags;
            }
        }

        internal static bool WndProcShouldBeDebuggable
            => (WndProcFlags & UseDebuggableWndProc) != 0;

        /// <summary>
        ///  Inserts an entry into this hashtable.
        /// </summary>
        private static void AddWindowToTable(IntPtr handle, NativeWindow window)
        {
            Debug.Assert(handle != IntPtr.Zero, "Should never insert a zero handle into the hash");

            lock (s_internalSyncObject)
            {
                t_anyHandleCreated = true;
                s_anyHandleCreatedInApp = true;

                GCHandle root = GCHandle.Alloc(window, GCHandleType.Weak);

                if (s_windowHandles.TryGetValue(handle, out GCHandle oldRoot))
                {
                    // This handle exists with another NativeWindow, replace it and
                    // hook up the previous and next window pointers so we can get
                    // back to the right window.

                    if (oldRoot.IsAllocated)
                    {
                        if (oldRoot.Target is NativeWindow target)
                        {
                            window.PreviousWindow = target;
                            Debug.Assert(window.PreviousWindow._nextWindow is null, "Last window in chain should have null next ptr");
                            window.PreviousWindow._nextWindow = window;
                        }
                        oldRoot.Free();
                    }
                }

                s_windowHandles[handle] = root;
            }
        }

        /// <summary>
        ///  Creates and applies a unique identifier to the given window <paramref name="handle"/>.
        /// </summary>
        /// <returns>
        ///  The identifier given to the window.
        /// </returns>
        internal static short CreateWindowId(IHandle handle)
        {
            short id = s_globalID++;
            s_windowIds[id] = handle.Handle;

            // Set the Window ID
            User32.SetWindowLong(
                handle,
                User32.GWL.ID,
                (IntPtr)id);

            return id;
        }

        /// <summary>
        ///  Assigns a handle to this <see cref="NativeWindow"/> instance.
        /// </summary>
        public void AssignHandle(IntPtr handle)
            => AssignHandle(handle, assignUniqueID: true);

        internal void AssignHandle(IntPtr handle, bool assignUniqueID)
        {
            lock (this)
            {
                CheckReleased();
                Debug.Assert(handle != IntPtr.Zero, "handle is 0");

                Handle = handle;

                _priorWindowProcHandle = User32.GetWindowLong(this, User32.GWL.WNDPROC);
                Debug.Assert(_priorWindowProcHandle != IntPtr.Zero);

                Debug.WriteLineIf(
                    WndProcChoice.TraceVerbose,
                    WndProcShouldBeDebuggable ? "Using debuggable wndproc" : "Using normal wndproc");

                _windowProc = new User32.WNDPROC(Callback);

                AddWindowToTable(handle, this);

                // Set the NativeWindow window procedure delegate and get back the native pointer for it.
                User32.SetWindowLong(this, User32.GWL.WNDPROC, _windowProc);
                _windowProcHandle = User32.GetWindowLong(this, User32.GWL.WNDPROC);

                // This shouldn't be possible.
                Debug.Assert(_priorWindowProcHandle != _windowProcHandle, "Uh oh! Subclassed ourselves!!!");

                if (assignUniqueID
                    && ((User32.WS)PARAM.ToUInt(User32.GetWindowLong(this, User32.GWL.STYLE))).HasFlag(User32.WS.CHILD)
                    && User32.GetWindowLong(this, User32.GWL.ID) == IntPtr.Zero)
                {
                    User32.SetWindowLong(this, User32.GWL.ID, handle);
                }

                if (_suppressedGC)
                {
                    GC.ReRegisterForFinalize(this);
                    _suppressedGC = false;
                }

                OnHandleChange();
            }
        }

        /// <summary>
        ///  Window message callback method. Control arrives here when a window
        ///  message is sent to this Window. This method packages the window message
        ///  in a Message object and invokes the wndProc() method. A WM_NCDESTROY
        ///  message automatically causes the releaseHandle() method to be called.
        /// </summary>
        private IntPtr Callback(IntPtr hWnd, User32.WM msg, IntPtr wparam, IntPtr lparam)
        {
            // Note: if you change this code be sure to change the
            // corresponding code in DebuggableCallback below!

            Message m = Message.Create(hWnd, msg, wparam, lparam);

            try
            {
                if (_weakThisPtr.IsAlive && _weakThisPtr.Target != null)
                {
                    WndProc(ref m);
                }
                else
                {
                    DefWndProc(ref m);
                }
            }
            catch (Exception e)
            {
                if (WndProcShouldBeDebuggable)
                {
                    throw;
                }
                OnThreadException(e);
            }
            finally
            {
                if (msg == User32.WM.NCDESTROY)
                {
                    ReleaseHandle(handleValid: false);
                }

                if (msg == User32.RegisteredMessage.WM_UIUNSUBCLASS)
                {
                    ReleaseHandle(handleValid: true);
                }
            }

            return m.Result;
        }

        /// <summary>
        ///  Raises an exception if the window handle is not zero.
        /// </summary>
        private void CheckReleased()
        {
            if (Handle != IntPtr.Zero)
            {
                throw new InvalidOperationException(SR.HandleAlreadyExists);
            }
        }

        /// <summary>
        ///  Creates a window handle for this window.
        /// </summary>
        public virtual void CreateHandle(CreateParams cp)
        {
            lock (this)
            {
                CheckReleased();
                WindowClass windowClass = WindowClass.Create(cp.ClassName, (User32.CS)cp.ClassStyle);
                lock (s_createWindowSyncObject)
                {
                    // The CLR will sometimes pump messages while we're waiting on the lock.
                    // If a message comes through (say a WM_ACTIVATE for the parent) which
                    // causes the handle to be created, we can try to create the handle twice
                    // for NativeWindow. Check the handle again to avoid this.
                    if (Handle != IntPtr.Zero)
                    {
                        return;
                    }

                    IntPtr createResult = IntPtr.Zero;
                    int lastWin32Error = 0;

                    NativeWindow prevTargetWindow = windowClass._targetWindow;
                    try
                    {
                        windowClass._targetWindow = this;

                        // Parking window dpi awareness context need to match with dpi awareness context of control being
                        // parented to this parking window. Otherwise, reparenting of control will fail.
                        using (DpiHelper.EnterDpiAwarenessScope(DpiAwarenessContext))
                        {
                            IntPtr modHandle = Kernel32.GetModuleHandleW(null);

                            // Older versions of Windows AV rather than returning E_OUTOFMEMORY.
                            // Catch this and then we re-throw an out of memory error.
                            try
                            {
                                // CreateWindowEx throws if WindowText is greater than the max
                                // length of a 16 bit int (32767).
                                // If it exceeds the max, we should take the substring....
                                if (cp.Caption != null && cp.Caption.Length > short.MaxValue)
                                {
                                    cp.Caption = cp.Caption.Substring(0, short.MaxValue);
                                }

                                createResult = User32.CreateWindowExW(
                                    (User32.WS_EX)cp.ExStyle,
                                    windowClass._windowClassName,
                                    cp.Caption,
                                    (User32.WS)cp.Style,
                                    cp.X,
                                    cp.Y,
                                    cp.Width,
                                    cp.Height,
                                    cp.Parent,
                                    IntPtr.Zero,
                                    modHandle,
                                    cp.Param);

                                lastWin32Error = Marshal.GetLastWin32Error();
                            }
                            catch (NullReferenceException e)
                            {
                                throw new OutOfMemoryException(SR.ErrorCreatingHandle, e);
                            }
                        }
                    }
                    finally
                    {
                        windowClass._targetWindow = prevTargetWindow;
                    }

                    Debug.WriteLineIf(CoreSwitches.PerfTrack.Enabled, "Handle created of type '" + cp.ClassName + "' with caption '" + cp.Caption + "' from NativeWindow of type '" + GetType().FullName + "'");

                    if (createResult == IntPtr.Zero)
                    {
                        throw new Win32Exception(lastWin32Error, SR.ErrorCreatingHandle);
                    }
                    _ownHandle = true;
                }
            }
        }

        /// <summary>
        ///  Invokes the default window procedure associated with this Window. It is
        ///  an error to call this method when the Handle property is zero.
        /// </summary>
        public void DefWndProc(ref Message m)
        {
            if (PreviousWindow is null)
            {
                if (_priorWindowProcHandle == IntPtr.Zero)
                {
                    Debug.Fail($"Can't find a default window procedure for message {m} on class {GetType().Name}");

                    // At this point, there isn't much we can do.  There's a small chance the following
                    // line will allow the rest of the program to run, but don't get your hopes up.
                    m.Result = User32.DefWindowProcW(m.HWnd, (User32.WM)m.Msg, m.WParam, m.LParam);
                    return;
                }
                m.Result = User32.CallWindowProcW(_priorWindowProcHandle, m.HWnd, (User32.WM)m.Msg, m.WParam, m.LParam);
            }
            else
            {
                m.Result = PreviousWindow.Callback(m.HWnd, (User32.WM)m.Msg, m.WParam, m.LParam);
            }
        }

        /// <summary>
        ///  Destroys the handle associated with this window.
        /// </summary>
        public virtual void DestroyHandle()
        {
            lock (this)
            {
                if (Handle != IntPtr.Zero)
                {
                    if (User32.DestroyWindow(this).IsFalse())
                    {
                        UnSubclass();

                        // Now post a close and let it do whatever it needs to do on its own.
                        User32.PostMessageW(this, User32.WM.CLOSE);
                    }

                    Handle = IntPtr.Zero;
                    _ownHandle = false;
                }

                // Now that we have disposed, there is no need to finalize us any more.
                GC.SuppressFinalize(this);
                _suppressedGC = true;
            }
        }

        /// <summary>
        ///  Retrieves the window associated with the specified <paramref name="handle"/>.
        /// </summary>
        public static NativeWindow FromHandle(IntPtr handle)
            => handle != IntPtr.Zero ? GetWindowFromTable(handle) : null;

        /// <summary>
        ///  Returns the native window for the given handle, or null if
        ///  the handle is not in our hash table.
        /// </summary>
        private static NativeWindow GetWindowFromTable(IntPtr handle)
        {
            if (s_windowHandles.TryGetValue(handle, out GCHandle value) && value.IsAllocated)
            {
                return (NativeWindow)value.Target;
            }
            return null;
        }

        /// <summary>
        ///  Returns the handle from the given <paramref name="id"/> if found, otherwise returns
        ///  <see cref="IntPtr.Zero"/>.
        /// </summary>
        internal IntPtr GetHandleFromWindowId(short id)
        {
            if (!s_windowIds.TryGetValue(id, out IntPtr handle))
            {
                handle = IntPtr.Zero;
            }

            return handle;
        }

        /// <summary>
        ///  Specifies a notification method that is called when the handle for a
        ///  window is changed.
        /// </summary>
        protected virtual void OnHandleChange()
        {
        }

        /// <summary>
        ///  On class load, we connect an event to Application to let us know when
        ///  the process or domain terminates.  When this happens, we attempt to
        ///  clear our window class cache.  We cannot destroy windows (because we don't
        ///  have access to their thread), and we cannot unregister window classes
        ///  (because the classes are in use by the windows we can't destroy).  Instead,
        ///  we move the class and window procs to DefWndProc
        /// </summary>
#pragma warning disable SYSLIB0004 // Type or member is obsolete
        [PrePrepareMethod]
#pragma warning restore SYSLIB0004 // Type or member is obsolete
        private static void OnShutdown(object sender, EventArgs e)
        {
            // If we still have windows allocated, we must sling them to userDefWindowProc
            // or else they will AV if they get a message after the managed code has been
            // removed.  In debug builds, we assert and give the "ToString" of the native
            // window. In retail we just detatch the window proc and let it go.  Note that
            // we cannot call DestroyWindow because this API will fail if called from
            // an incorrect thread.

            if (s_windowHandles.Count > 0)
            {
                Debug.Assert(DefaultWindowProc != IntPtr.Zero, "We have active windows but no user window proc?");

                lock (s_internalSyncObject)
                {
                    foreach (KeyValuePair<IntPtr, GCHandle> entry in s_windowHandles)
                    {
                        IntPtr handle = entry.Key;
                        if (handle != IntPtr.Zero && handle != new IntPtr(-1))
                        {
                            User32.SetWindowLong(handle, User32.GWL.WNDPROC, DefaultWindowProc);
                            User32.SetClassLong(handle, User32.GCL.WNDPROC, DefaultWindowProc);
                            User32.PostMessageW(handle, User32.WM.CLOSE);

                            // Fish out the Window object, if it is valid, and NULL the handle pointer.  This
                            // way the rest of WinForms won't think the handle is still valid here.
                            if (entry.Value.IsAllocated)
                            {
                                NativeWindow w = (NativeWindow)entry.Value.Target;
                                if (w != null)
                                {
                                    w.Handle = IntPtr.Zero;
                                }
                            }
                        }
                    }

                    s_windowHandles.Clear();
                }
            }
        }

        /// <summary>
        ///  When overridden in a derived class, manages an unhandled thread exception.
        /// </summary>
        protected virtual void OnThreadException(Exception e)
        {
        }

        /// <summary>
        ///  Releases the handle associated with this window.
        /// </summary>
        public virtual void ReleaseHandle()
        {
            ReleaseHandle(true);
        }

        /// <summary>
        ///  Releases the handle associated with this window.
        /// </summary>
        /// <remarks>
        ///  If <paramref name="handleValid"/> is true, this will unsubclass the window as
        ///  well. <paramref name="handleValid"/> should be false if we are releasing in
        ///  response to a WM_DESTROY. Unsubclassing during this message can cause problems
        ///  with Windows theme manager and it's not needed anyway.
        /// </remarks>
        private void ReleaseHandle(bool handleValid)
        {
            if (Handle == IntPtr.Zero)
                return;

            lock (this)
            {
                if (Handle == IntPtr.Zero)
                {
                    return;
                }

                if (handleValid)
                {
                    UnSubclass();
                }

                RemoveWindowFromDictionary(Handle, this);

                if (_ownHandle)
                {
                    _ownHandle = false;
                }

                Handle = IntPtr.Zero;

                if (_weakThisPtr.IsAlive && _weakThisPtr.Target != null)
                {
                    // We're not already finalizing.
                    OnHandleChange();

                    // Now that we have disposed, there is no need to finalize any more.
                    GC.SuppressFinalize(this);
                    _suppressedGC = true;
                }
            }
        }

        private static void RemoveWindowFromDictionary(IntPtr handle, NativeWindow window)
        {
            Debug.Assert(handle != IntPtr.Zero, "Incorrect handle");

            lock (s_internalSyncObject)
            {
                if (!s_windowHandles.TryGetValue(handle, out GCHandle root))
                {
                    return;
                }

                if (window.PreviousWindow != null)
                {
                    // Connect the prior window directly to the next window (if any)
                    window.PreviousWindow._nextWindow = window._nextWindow;
                }

                if (window._nextWindow != null)
                {
                    // Connect the next window to the prior window
                    window._nextWindow._priorWindowProcHandle = window._priorWindowProcHandle;
                    window._nextWindow.PreviousWindow = window.PreviousWindow;
                }

                if (window._nextWindow is null)
                {
                    // We're the last NativeWindow for this HWND, remove the key or reassign
                    // the value to the prior NativeWindow if it exists.

                    if (root.IsAllocated)
                    {
                        root.Free();
                    }

                    if (window.PreviousWindow != null)
                    {
                        s_windowHandles[handle] = GCHandle.Alloc(window.PreviousWindow, GCHandleType.Weak);
                    }
                    else
                    {
                        s_windowHandles.Remove(handle);
                    }
                }

                // Set our current window's links to null
                window._nextWindow = null;
                window.PreviousWindow = null;
            }
        }

        /// <summary>
        ///  Removes the given Window from the lookup table.
        /// </summary>
        internal static void RemoveWindowFromIDTable(short id) => s_windowIds.Remove(id);

        /// <summary>
        ///  This method can be used to modify the exception handling behavior of
        ///  NativeWindow.  By default, NativeWindow will detect if an application
        ///  is running under a debugger, or is running on a machine with a debugger
        ///  installed.  In this case, an unhandled exception in the NativeWindow's
        ///  WndProc method will remain unhandled so the debugger can trap it.  If
        ///  there is no debugger installed NativeWindow will trap the exception
        ///  and route it to the Application class's unhandled exception filter.
        ///
        ///  You can control this behavior via a config file, or directly through
        ///  code using this method.  Setting the unhandled exception mode does
        ///  not change the behavior of any NativeWindow objects that are currently
        ///  connected to window handles; it only affects new handle connections.
        ///
        ///  When threadScope is false, the application exception mode is set. The
        ///  application exception mode is used for all threads that have the Automatic mode.
        ///  Setting the application exception mode does not affect the setting of the current thread.
        ///
        ///  When threadScope is true, the thread exception mode is set. The thread
        ///  exception mode overrides the application exception mode if it's not Automatic.
        /// </summary>
        internal static void SetUnhandledExceptionModeInternal(UnhandledExceptionMode mode, bool threadScope)
        {
            if (!threadScope && s_anyHandleCreatedInApp)
            {
                throw new InvalidOperationException(SR.ApplicationCannotChangeApplicationExceptionMode);
            }
            if (threadScope && t_anyHandleCreated)
            {
                throw new InvalidOperationException(SR.ApplicationCannotChangeThreadExceptionMode);
            }

            switch (mode)
            {
                case UnhandledExceptionMode.Automatic:
                    if (threadScope)
                    {
                        t_userSetProcFlags = 0;
                    }
                    else
                    {
                        s_userSetProcFlagsForApp = 0;
                    }
                    break;
                case UnhandledExceptionMode.ThrowException:
                    if (threadScope)
                    {
                        t_userSetProcFlags = UseDebuggableWndProc | InitializedFlags;
                    }
                    else
                    {
                        s_userSetProcFlagsForApp = UseDebuggableWndProc | InitializedFlags;
                    }
                    break;
                case UnhandledExceptionMode.CatchException:
                    if (threadScope)
                    {
                        t_userSetProcFlags = InitializedFlags;
                    }
                    else
                    {
                        s_userSetProcFlagsForApp = InitializedFlags;
                    }
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(mode), (int)mode, typeof(UnhandledExceptionMode));
            }
        }

        /// <summary>
        ///  Unsubclassing is a tricky business.  We need to account for some border cases:
        ///
        ///   1) User has done multiple subclasses but has un-subclassed out of order.
        ///   2) User has done multiple subclasses but now our defWindowProc points to
        ///       a NativeWindow that has GC'd.
        ///   3) User releasing this handle but this NativeWindow is not the current
        ///       window proc.
        /// </summary>
        private void UnSubclass()
        {
            bool finalizing = (!_weakThisPtr.IsAlive || _weakThisPtr.Target is null);

            // Don't touch if the current window proc is not ours.

            IntPtr currentWindowProc = User32.GetWindowLong(this, User32.GWL.WNDPROC);
            if (_windowProcHandle == currentWindowProc)
            {
                // The current window proc is ours

                if (PreviousWindow is null)
                {
                    // This is the first NativeWindow registered for this HWND, just put back the prior handle we stashed away.
                    User32.SetWindowLong(this, User32.GWL.WNDPROC, _priorWindowProcHandle);
                }
                else
                {
                    if (finalizing)
                    {
                        // Here, we are finalizing and defWindowProc is pointing to a managed object.  We must assume
                        // that the object defWindowProc is pointing to is also finalizing.  Why?  Because we're
                        // holding a ref to it, and it is holding a ref to us.  The only way this cycle will
                        // finalize is if no one else is hanging onto it.  So, we re-assign the window proc to
                        // userDefWindowProc.
                        User32.SetWindowLong(this, User32.GWL.WNDPROC, DefaultWindowProc);
                    }
                    else
                    {
                        // Here we are not finalizing so we use the windowProc for our previous window.  This may
                        // DIFFER from the value we are currently storing in defWindowProc because someone may
                        // have re-subclassed.
                        User32.SetWindowLong(this, User32.GWL.WNDPROC, PreviousWindow._windowProc);
                    }
                }
            }
            else
            {
                // The current window proc isn't one we registered.

                // Cutting the subclass chain anyway, even if we're not the last one in the chain. If the whole chain
                // is all managed NativeWindow classes it doesnt matter, if the chain is not, then someone didn't clean
                // up properly, too bad for them...

                // We will cut off the chain if we cannot unsubclass.
                // If we find previouswindow pointing to us, then we can let RemoveWindowFromTable reassign the
                // defwndproc pointers properly when this guy gets removed (thereby unsubclassing ourselves)

                if (_nextWindow is null || _nextWindow._priorWindowProcHandle != _windowProcHandle)
                {
                    // we didn't find it... let's unhook anyway and cut the chain... this prevents crashes
                    User32.SetWindowLong(this, User32.GWL.WNDPROC, DefaultWindowProc);
                }
            }
        }

        /// <summary>
        ///  Invokes the default window procedure associated with this window.
        /// </summary>
        protected virtual void WndProc(ref Message m)
        {
            DefWndProc(ref m);
        }
    }
}

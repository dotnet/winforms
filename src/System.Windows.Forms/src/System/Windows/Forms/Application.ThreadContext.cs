// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Threading;
using static Interop;

namespace System.Windows.Forms
{
    public sealed partial class Application
    {
        /// <summary>
        ///  This class is the embodiment of TLS for windows forms.  We do not expose this to end users because
        ///  TLS is really just an unfortunate artifact of using Win 32.  We want the world to be free
        ///  threaded.
        /// </summary>
        internal sealed class ThreadContext : MarshalByRefObject, UnsafeNativeMethods.IMsoComponent
        {
            private const int STATE_OLEINITIALIZED = 0x00000001;
            private const int STATE_EXTERNALOLEINIT = 0x00000002;
            private const int STATE_INTHREADEXCEPTION = 0x00000004;
            private const int STATE_POSTEDQUIT = 0x00000008;
            private const int STATE_FILTERSNAPSHOTVALID = 0x00000010;
            private const int STATE_TRACKINGCOMPONENT = 0x00000020;
            private const int INVALID_ID = unchecked((int)0xFFFFFFFF);

            private static readonly Hashtable contextHash = new Hashtable();

            // When this gets to zero, we'll invoke a full garbage
            // collect and check for root/window leaks.
            //
            private static readonly object tcInternalSyncObject = new object();

            private static int totalMessageLoopCount;
            private static int baseLoopReason;

            [ThreadStatic]
            private static ThreadContext currentThreadContext;

            internal ThreadExceptionEventHandler threadExceptionHandler;
            internal EventHandler idleHandler;
            internal EventHandler enterModalHandler;
            internal EventHandler leaveModalHandler;
            private ApplicationContext applicationContext;

            // Parking window list
            private readonly List<ParkingWindow> parkingWindows = new List<ParkingWindow>();
            private Control marshalingControl;
            private CultureInfo culture;
            private List<IMessageFilter> messageFilters;
            private List<IMessageFilter> messageFilterSnapshot;
            private int inProcessFilters = 0;
            private IntPtr handle;
            private readonly int id;
            private int messageLoopCount;
            private int threadState;
            private int modalCount;

            // used for correct restoration of focus after modality
            private WeakReference activatingControlRef;

            // IMsoComponentManager stuff
            //
            private UnsafeNativeMethods.IMsoComponentManager componentManager;
            private bool externalComponentManager;
            private bool fetchingComponentManager;

            // IMsoComponent stuff
            private int componentID = INVALID_ID;
            private Form currentForm;
            private ThreadWindows threadWindows;
            private NativeMethods.MSG tempMsg = new NativeMethods.MSG();
            private int disposeCount;   // To make sure that we don't allow
                                        // reentrancy in Dispose()

            // Debug helper variable
#if DEBUG
            private int debugModalCounter;
#endif
            // We need to set this flag if we have started the ModalMessageLoop so that we dont create the ThreadWindows
            // when the ComponentManager calls on us (as IMSOComponent) during the OnEnterState.
            private bool ourModalLoop;

            // A private field on Application that stores the callback delegate
            private MessageLoopCallback messageLoopCallback = null;

            /// <summary>
            ///  Creates a new thread context object.
            /// </summary>
            public ThreadContext()
            {
                IntPtr address = IntPtr.Zero;

                UnsafeNativeMethods.DuplicateHandle(new HandleRef(null, SafeNativeMethods.GetCurrentProcess()), new HandleRef(null, SafeNativeMethods.GetCurrentThread()),
                                                    new HandleRef(null, SafeNativeMethods.GetCurrentProcess()), ref address, 0, false,
                                                    NativeMethods.DUPLICATE_SAME_ACCESS);

                handle = address;

                id = SafeNativeMethods.GetCurrentThreadId();
                messageLoopCount = 0;
                currentThreadContext = this;
                contextHash[id] = this;
            }

            public ApplicationContext ApplicationContext
            {
                get
                {
                    return applicationContext;
                }
            }

            /// <summary>
            ///  Retrieves the component manager for this process.  If there is no component manager
            ///  currently installed, we install our own.
            /// </summary>
            internal UnsafeNativeMethods.IMsoComponentManager ComponentManager
            {
                get
                {

                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "Application.ComponentManager.Get:");

                    if (componentManager == null)
                    {

                        // The CLR is a good COM citizen and will pump messages when things are waiting.
                        // This is nice; it keeps the world responsive.  But, it is also very hard for
                        // us because most of the code below causes waits, and the likelihood that
                        // a message will come in and need a component manager is very high.  Recursing
                        // here is very very bad, and will almost certainly lead to application failure
                        // later on as we come out of the recursion.  So, we guard it here and return
                        // null.  EVERYONE who accesses the component manager must handle a NULL return!
                        //
                        if (fetchingComponentManager)
                        {
                            return null;
                        }

                        fetchingComponentManager = true;
                        try
                        {
                            UnsafeNativeMethods.IMsoComponentManager msocm = null;
                            Application.OleRequired();

                            // Attempt to obtain the Host Application MSOComponentManager
                            //
                            IntPtr msgFilterPtr = (IntPtr)0;

                            if (NativeMethods.Succeeded(UnsafeNativeMethods.CoRegisterMessageFilter(NativeMethods.NullHandleRef, ref msgFilterPtr)) && msgFilterPtr != (IntPtr)0)
                            {
                                IntPtr dummy = (IntPtr)0;
                                UnsafeNativeMethods.CoRegisterMessageFilter(new HandleRef(null, msgFilterPtr), ref dummy);

                                object msgFilterObj = Marshal.GetObjectForIUnknown(msgFilterPtr);
                                Marshal.Release(msgFilterPtr);

                                if (msgFilterObj is UnsafeNativeMethods.IOleServiceProvider sp)
                                {
                                    try
                                    {
                                        IntPtr retval = IntPtr.Zero;

                                        // Using typeof() of COM object spins up COM at JIT time.
                                        // Guid compModGuid = typeof(UnsafeNativeMethods.SMsoComponentManager).GUID;
                                        //
                                        Guid compModGuid = new Guid("000C060B-0000-0000-C000-000000000046");
                                        Guid iid = new Guid("{000C0601-0000-0000-C000-000000000046}");
                                        int hr = sp.QueryService(
                                                       ref compModGuid,
                                                       ref iid,
                                                       out retval);

                                        if (NativeMethods.Succeeded(hr) && retval != IntPtr.Zero)
                                        {

                                            // Now query for hte message filter.

                                            IntPtr pmsocm;

                                            try
                                            {
                                                Guid IID_IMsoComponentManager = typeof(UnsafeNativeMethods.IMsoComponentManager).GUID;
                                                hr = Marshal.QueryInterface(retval, ref IID_IMsoComponentManager, out pmsocm);
                                            }
                                            finally
                                            {
                                                Marshal.Release(retval);
                                            }

                                            if (NativeMethods.Succeeded(hr) && pmsocm != IntPtr.Zero)
                                            {

                                                // Ok, we have a native component manager.  Hand this over to
                                                // our broker object to get a proxy we can use
                                                try
                                                {
                                                    msocm = ComponentManagerBroker.GetComponentManager(pmsocm);
                                                }
                                                finally
                                                {
                                                    Marshal.Release(pmsocm);
                                                }
                                            }

                                            if (msocm != null)
                                            {

                                                // If the resulting service is the same pUnk as the
                                                // message filter (a common implementation technique),
                                                // then we want to null msgFilterObj at this point so
                                                // we don't call RelaseComObject on it below.  That would
                                                // also release the RCW for the component manager pointer.
                                                if (msgFilterPtr == retval)
                                                {
                                                    msgFilterObj = null;
                                                }

                                                externalComponentManager = true;
                                                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "Using MSO Component manager");

                                                // Now attach app domain unload events so we can
                                                // detect when we need to revoke our component
                                                //
                                                AppDomain.CurrentDomain.DomainUnload += new EventHandler(OnDomainUnload);
                                                AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnDomainUnload);
                                            }
                                        }
                                    }
                                    catch
                                    {
                                    }
                                }

                                if (msgFilterObj != null && Marshal.IsComObject(msgFilterObj))
                                {
                                    Marshal.ReleaseComObject(msgFilterObj);
                                }
                            }

                            // Otherwise, we implement component manager ourselves
                            //
                            if (msocm == null)
                            {
                                msocm = new ComponentManager();
                                externalComponentManager = false;

                                // We must also store this back into the message filter for others
                                // to use.
                                //
                                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "Using our own component manager");
                            }

                            if (msocm != null && componentID == INVALID_ID)
                            {
                                // Finally, if we got a compnent manager, register ourselves with it.
                                //
                                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "Registering MSO component with the component manager");
                                NativeMethods.MSOCRINFOSTRUCT info = new NativeMethods.MSOCRINFOSTRUCT
                                {
                                    cbSize = Marshal.SizeOf<NativeMethods.MSOCRINFOSTRUCT>(),
                                    uIdleTimeInterval = 0,
                                    grfcrf = NativeMethods.MSOCM.msocrfPreTranslateAll | NativeMethods.MSOCM.msocrfNeedIdleTime,
                                    grfcadvf = NativeMethods.MSOCM.msocadvfModal
                                };

                                bool result = msocm.FRegisterComponent(this, info, out IntPtr localComponentID);
                                componentID = unchecked((int)(long)localComponentID);
                                Debug.Assert(componentID != INVALID_ID, "Our ID sentinel was returned as a valid ID");

                                if (result && !(msocm is ComponentManager))
                                {
                                    messageLoopCount++;
                                }

                                Debug.Assert(result, "Failed to register WindowsForms with the ComponentManager -- DoEvents and modal dialogs will be broken. size: " + info.cbSize);
                                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager.FRegisterComponent returned " + result.ToString());
                                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager.FRegisterComponent assigned a componentID == [0x" + Convert.ToString(componentID, 16) + "]");
                                componentManager = msocm;
                            }
                        }
                        finally
                        {
                            fetchingComponentManager = false;
                        }
                    }

                    return componentManager;
                }
            }

            internal bool CustomThreadExceptionHandlerAttached
            {
                get
                {
                    return threadExceptionHandler != null;
                }
            }

            /// <summary>
            ///  Retrieves the actual parking form.  This will demand create the parking window
            ///  if it needs to.
            /// </summary>
            internal ParkingWindow GetParkingWindow(DpiAwarenessContext context)
            {

                // Locking 'this' here is ok since this is an internal class.
                lock (this)
                {
                    ParkingWindow parkingWindow = GetParkingWindowForContext(context);
                    if (parkingWindow == null)
                    {
#if DEBUG
                        if (CoreSwitches.PerfTrack.Enabled)
                        {
                            Debug.WriteLine("Creating parking form!");
                            Debug.WriteLine(CoreSwitches.PerfTrack.Enabled, Environment.StackTrace);
                        }
#endif

                        using (DpiHelper.EnterDpiAwarenessScope(context))
                        {
                            parkingWindow = new ParkingWindow();
                        }

                        parkingWindows.Add(parkingWindow);
                    }
                    return parkingWindow;
                }
            }

            /// <summary>
            /// Returns parking window that matches dpi awareness context. return null if not found.
            /// </summary>
            /// <returns>return matching parking window from list. returns null if not found</returns>
            internal ParkingWindow GetParkingWindowForContext(DpiAwarenessContext context)
            {

                if (parkingWindows.Count == 0)
                {
                    return null;
                }

                // Legacy OS/target framework scenario where ControlDpiContext is set to DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNSPECIFIED
                // because of 'ThreadContextDpiAwareness' API unavailability or this feature is not enabled.

                if (!DpiHelper.IsScalingRequirementMet || CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(context, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED))
                {

                    Debug.Assert(parkingWindows.Count == 1, "parkingWindows count can not be > 1 for legacy OS/target framework versions");
                    return parkingWindows[0];
                }

                // Supported OS scenario.
                foreach (ParkingWindow p in parkingWindows)
                {
                    if (CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(p.DpiAwarenessContext, context))
                    {
                        return p;
                    }
                }

                // parking window is not yet created for the requested DpiAwarenessContext
                return null;
            }

            internal Control ActivatingControl
            {
                get
                {
                    if ((activatingControlRef != null) && (activatingControlRef.IsAlive))
                    {
                        return activatingControlRef.Target as Control;
                    }
                    return null;
                }
                set
                {
                    if (value != null)
                    {
                        activatingControlRef = new WeakReference(value);
                    }
                    else
                    {
                        activatingControlRef = null;
                    }
                }
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
                        if (marshalingControl == null)
                        {
#if DEBUG
                            if (CoreSwitches.PerfTrack.Enabled)
                            {
                                Debug.WriteLine("Creating marshalling control!");
                                Debug.WriteLine(CoreSwitches.PerfTrack.Enabled, Environment.StackTrace);
                            }
#endif

                            marshalingControl = new MarshalingControl();
                        }
                        return marshalingControl;
                    }
                }
            }

            /// <summary>
            ///  Allows you to setup a message filter for the application's message pump.  This
            ///  installs the filter on the current thread.
            /// </summary>
            internal void AddMessageFilter(IMessageFilter f)
            {
                if (messageFilters == null)
                {
                    messageFilters = new List<IMessageFilter>();
                }
                if (messageFilterSnapshot == null)
                {
                    messageFilterSnapshot = new List<IMessageFilter>();
                }
                if (f != null)
                {
                    SetState(STATE_FILTERSNAPSHOTVALID, false);
                    if (messageFilters.Count > 0 && f is IMessageModifyAndFilter)
                    {
                        // insert the IMessageModifyAndFilter filters first
                        messageFilters.Insert(0, f);
                    }
                    else
                    {
                        messageFilters.Add(f);
                    }
                }
            }

            // Called immediately before we begin pumping messages for a modal message loop.
            internal void BeginModalMessageLoop(ApplicationContext context)
            {
#if DEBUG
                debugModalCounter++;
#endif
                // Set the ourModalLoop flag so that the "IMSOComponent.OnEnterState" is a NOOP since we started the ModalMessageLoop.
                bool wasOurLoop = ourModalLoop;
                ourModalLoop = true;
                try
                {
                    UnsafeNativeMethods.IMsoComponentManager cm = ComponentManager;
                    if (cm != null)
                    {
                        cm.OnComponentEnterState((IntPtr)componentID, NativeMethods.MSOCM.msocstateModal, NativeMethods.MSOCM.msoccontextAll, 0, 0, 0);
                    }
                }
                finally
                {
                    ourModalLoop = wasOurLoop;
                }
                // This will initialize the ThreadWindows with proper flags.
                DisableWindowsForModalLoop(false, context); // onlyWinForms = false

                modalCount++;

                if (enterModalHandler != null && modalCount == 1)
                {
                    enterModalHandler(Thread.CurrentThread, EventArgs.Empty);
                }

            }

            // Disables windows in preparation of going modal.  If parameter is true, we disable all
            // windows, if false, only windows forms windows (i.e., windows controlled by this MsoComponent).
            // See also IMsoComponent.OnEnterState.
            internal void DisableWindowsForModalLoop(bool onlyWinForms, ApplicationContext context)
            {
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : Entering modal state");
                ThreadWindows old = threadWindows;
                threadWindows = new ThreadWindows(onlyWinForms);
                threadWindows.Enable(false);
                threadWindows.previousThreadWindows = old;

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

                // need to avoid multiple threads coming in here or we'll leak the thread
                // handle.
                //
                lock (this)
                {
                    try
                    {
                        if (disposeCount++ == 0)
                        {  // make sure that we are not reentrant
                            // Unravel our message loop.  this will marshal us over to
                            // the right thread, making the dispose() method async.
                            if (messageLoopCount > 0 && postQuit)
                            {
                                PostQuit();
                            }
                            else
                            {
                                bool ourThread = SafeNativeMethods.GetCurrentThreadId() == id;

                                try
                                {
                                    // We can only clean up if we're being called on our
                                    // own thread.
                                    //
                                    if (ourThread)
                                    {

                                        // If we had a component manager, detach from it.
                                        //
                                        if (componentManager != null)
                                        {
                                            RevokeComponent();
                                        }

                                        // DisposeAssociatedComponents();
                                        DisposeThreadWindows();

                                        try
                                        {
                                            Application.RaiseThreadExit();
                                        }
                                        finally
                                        {
                                            if (GetState(STATE_OLEINITIALIZED) && !GetState(STATE_EXTERNALOLEINIT))
                                            {
                                                SetState(STATE_OLEINITIALIZED, false);
                                                UnsafeNativeMethods.OleUninitialize();
                                            }
                                        }
                                    }
                                }
                                finally
                                {
                                    // We can always clean up this handle, though
                                    //
                                    if (handle != IntPtr.Zero)
                                    {
                                        UnsafeNativeMethods.CloseHandle(new HandleRef(this, handle));
                                        handle = IntPtr.Zero;
                                    }

                                    try
                                    {
                                        if (totalMessageLoopCount == 0)
                                        {
                                            Application.RaiseExit();
                                        }
                                    }
                                    finally
                                    {
                                        lock (tcInternalSyncObject)
                                        {
                                            contextHash.Remove((object)id);
                                        }
                                        if (currentThreadContext == this)
                                        {
                                            currentThreadContext = null;
                                        }
                                    }
                                }
                            }

                            GC.SuppressFinalize(this);
                        }
                    }
                    finally
                    {
                        disposeCount--;
                    }
                }
            }

            /// <summary>
            ///  Disposes of this thread's parking form.
            /// </summary>
            private void DisposeParkingWindow()
            {
                if (parkingWindows.Count != 0)
                {

                    // We take two paths here.  If we are on the same thread as
                    // the parking window, we can destroy its handle.  If not,
                    // we just null it and let it GC.  When it finalizes it
                    // will disconnect its handle and post a WM_CLOSE.
                    //
                    // It is important that we just call DestroyHandle here
                    // and do not call Dispose.  Otherwise we would destroy
                    // controls that are living on the parking window.
                    //
                    int hwndThread = SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(parkingWindows[0], parkingWindows[0].Handle), out int pid);
                    int currentThread = SafeNativeMethods.GetCurrentThreadId();

                    for (int i = 0; i < parkingWindows.Count; i++)
                    {
                        if (hwndThread == currentThread)
                        {
                            parkingWindows[i].Destroy();
                        }
                        else
                        {
                            parkingWindows[i] = null;
                        }
                    }
                    parkingWindows.Clear();
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
                //
                try
                {
                    if (applicationContext != null)
                    {
                        applicationContext.Dispose();
                        applicationContext = null;
                    }

                    // Then, we rudely destroy all of the windows on the thread
                    //
                    ThreadWindows tw = new ThreadWindows(true);
                    tw.Dispose();

                    // And dispose the parking form, if it isn't already
                    //
                    DisposeParkingWindow();
                }
                catch
                {
                }
            }

            // Enables windows in preparation of stopping modal.  If parameter is true, we enable all windows,
            // if false, only windows forms windows (i.e., windows controlled by this MsoComponent).
            // See also IMsoComponent.OnEnterState.
            internal void EnableWindowsForModalLoop(bool onlyWinForms, ApplicationContext context)
            {
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : Leaving modal state");
                if (threadWindows != null)
                {
                    threadWindows.Enable(true);
                    Debug.Assert(threadWindows != null, "OnEnterState recursed, but it's not supposed to be reentrant");
                    threadWindows = threadWindows.previousThreadWindows;
                }

                if (context is ModalApplicationContext modalContext)
                {
                    modalContext.DisableThreadWindows(false, onlyWinForms);
                }
            }

            // Called immediately after we end pumping messages for a modal message loop.
            internal void EndModalMessageLoop(ApplicationContext context)
            {
#if DEBUG
                debugModalCounter--;
                Debug.Assert(debugModalCounter >= 0, "Mis-matched calls to Application.BeginModalMessageLoop() and Application.EndModalMessageLoop()");
#endif
                // This will re-enable the windows...
                EnableWindowsForModalLoop(false, context); // onlyWinForms = false

                bool wasOurLoop = ourModalLoop;
                ourModalLoop = true;
                try
                {

                    // If We started the ModalMessageLoop .. this will call us back on the IMSOComponent.OnStateEnter and not do anything ...
                    UnsafeNativeMethods.IMsoComponentManager cm = ComponentManager;
                    if (cm != null)
                    {
                        cm.FOnComponentExitState((IntPtr)componentID, NativeMethods.MSOCM.msocstateModal, NativeMethods.MSOCM.msoccontextAll, 0, 0);
                    }
                }
                finally
                {
                    // Reset the flag since we are exiting out of a ModalMesaageLoop..
                    ourModalLoop = wasOurLoop;
                }

                modalCount--;

                if (leaveModalHandler != null && modalCount == 0)
                {
                    leaveModalHandler(Thread.CurrentThread, EventArgs.Empty);
                }
            }

            /// <summary>
            ///  Exits the program by disposing of all thread contexts and message loops.
            /// </summary>
            internal static void ExitApplication()
            {
                ExitCommon(true /*disposing*/);
            }

            private static void ExitCommon(bool disposing)
            {
                lock (tcInternalSyncObject)
                {
                    if (contextHash != null)
                    {
                        ThreadContext[] ctxs = new ThreadContext[contextHash.Values.Count];
                        contextHash.Values.CopyTo(ctxs, 0);
                        for (int i = 0; i < ctxs.Length; ++i)
                        {
                            if (ctxs[i].ApplicationContext != null)
                            {
                                ctxs[i].ApplicationContext.ExitThread();
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
            ///  Exits the program by disposing of all thread contexts and message loops.
            /// </summary>
            internal static void ExitDomain()
            {
                ExitCommon(false /*disposing*/);
            }

            /// <summary>
            ///  Our finalization.  Minimal stuff... this shouldn't be called... We should always be disposed.
            /// </summary>
            ~ThreadContext()
            {

                // We used to call OleUninitialize() here if we were
                // still STATE_OLEINITIALIZED, but that's never the correct thing to do.
                // At this point we're on the wrong thread and we should never have been
                // called here in the first place.

                // We can always clean up this handle, though
                //
                if (handle != IntPtr.Zero)
                {
                    UnsafeNativeMethods.CloseHandle(new HandleRef(this, handle));
                    handle = IntPtr.Zero;
                }
            }

            // When a Form receives a WM_ACTIVATE message, it calls this method so we can do the
            // appropriate MsoComponentManager activation magic
            internal void FormActivated(bool activate)
            {
                if (activate)
                {
                    UnsafeNativeMethods.IMsoComponentManager cm = ComponentManager;
                    if (cm != null && !(cm is ComponentManager))
                    {
                        cm.FOnComponentActivate((IntPtr)componentID);
                    }
                }
            }

            // Sets this component as the tracking component - trumping any active component
            // for message filtering.
            internal void TrackInput(bool track)
            {

                // protect against double setting, as this causes asserts in the VS component manager.
                if (track != GetState(STATE_TRACKINGCOMPONENT))
                {
                    UnsafeNativeMethods.IMsoComponentManager cm = ComponentManager;
                    if (cm != null && !(cm is ComponentManager))
                    {
                        cm.FSetTrackingComponent((IntPtr)componentID, track);
                        SetState(STATE_TRACKINGCOMPONENT, track);
                    }
                }
            }
            /// <summary>
            ///  Retrieves a ThreadContext object for the current thread
            /// </summary>
            internal static ThreadContext FromCurrent()
            {
                ThreadContext context = currentThreadContext;

                if (context == null)
                {
                    context = new ThreadContext();
                }

                return context;
            }

            /// <summary>
            ///  Retrieves a ThreadContext object for the given thread ID
            /// </summary>
            internal static ThreadContext FromId(int id)
            {
                ThreadContext context = (ThreadContext)contextHash[(object)id];
                if (context == null && id == SafeNativeMethods.GetCurrentThreadId())
                {
                    context = new ThreadContext();
                }

                return context;
            }

            /// <summary>
            ///  Determines if it is OK to allow an application to quit and shutdown
            ///  the runtime.  We only allow this if we own the base message pump.
            /// </summary>
            internal bool GetAllowQuit()
            {
                return totalMessageLoopCount > 0 && baseLoopReason == NativeMethods.MSOCM.msoloopMain;
            }

            /// <summary>
            ///  Retrieves the handle to this thread.
            /// </summary>
            internal IntPtr GetHandle()
            {
                return handle;
            }

            /// <summary>
            ///  Retrieves the ID of this thread.
            /// </summary>
            internal int GetId()
            {
                return id;
            }

            /// <summary>
            ///  Retrieves the culture for this thread.
            /// </summary>
            internal CultureInfo GetCulture()
            {
                if (culture == null || culture.LCID != Kernel32.GetThreadLocale())
                {
                    culture = new CultureInfo((int)Kernel32.GetThreadLocale());
                }

                return culture;
            }

            /// <summary>
            ///  Determines if a message loop exists on this thread.
            /// </summary>
            internal bool GetMessageLoop()
            {
                return GetMessageLoop(false);
            }

            /// <summary>
            ///  Determines if a message loop exists on this thread.
            /// </summary>
            internal bool GetMessageLoop(bool mustBeActive)
            {

                // If we are already running a loop, we're fine.
                // If we are running in external manager we may need to make sure first the loop is active
                //
                if (messageLoopCount > (mustBeActive && externalComponentManager ? 1 : 0))
                {
                    return true;
                }

                // Also, access the ComponentManager property to demand create it, and we're also
                // fine if it is an external manager, because it has already pushed a loop.
                //
                if (ComponentManager != null && externalComponentManager)
                {
                    if (mustBeActive == false)
                    {
                        return true;
                    }

                    UnsafeNativeMethods.IMsoComponent[] activeComponents = new UnsafeNativeMethods.IMsoComponent[1];
                    if (ComponentManager.FGetActiveComponent(NativeMethods.MSOCM.msogacActive, activeComponents, null, 0) &&
                        activeComponents[0] == this)
                    {
                        return true;
                    }
                }

                // Finally, check if a message loop has been registered
                MessageLoopCallback callback = messageLoopCallback;
                if (callback != null)
                {
                    return callback();
                }

                // Otherwise, we do not have a loop running.
                //
                return false;
            }

            private bool GetState(int bit)
            {
                return (threadState & bit) != 0;
            }

            /// <summary>
            /// A method of determining whether we are handling messages that does not demand register
            /// the componentmanager
            /// </summary>
            /// <returns></returns>
            internal bool IsValidComponentId()
            {
                return (componentID != INVALID_ID);
            }

            internal ApartmentState OleRequired()
            {
                Thread current = Thread.CurrentThread;
                if (!GetState(STATE_OLEINITIALIZED))
                {

                    int ret = UnsafeNativeMethods.OleInitialize();

#if false
                    if (!(ret == NativeMethods.S_OK || ret == NativeMethods.S_FALSE || ret == NativeMethods.RPC_E_CHANGED_MODE)) {
                        Debug.Assert(ret == NativeMethods.S_OK || ret == NativeMethods.S_FALSE || ret == NativeMethods.RPC_E_CHANGED_MODE,
                                     "OLE Failed to Initialize!. RetCode: 0x" + Convert.ToString(ret, 16) +
                                     " LastError: " + Marshal.GetLastWin32Error().ToString());
                    }
#endif

                    SetState(STATE_OLEINITIALIZED, true);
                    if (ret == NativeMethods.RPC_E_CHANGED_MODE)
                    {
                        // This could happen if the thread was already initialized for MTA
                        // and then we call OleInitialize which tries to initialized it for STA
                        // This currently happens while profiling...
                        SetState(STATE_EXTERNALOLEINIT, true);
                    }

                }

                if (GetState(STATE_EXTERNALOLEINIT))
                {
                    return System.Threading.ApartmentState.MTA;
                }
                else
                {
                    return System.Threading.ApartmentState.STA;
                }
            }

            private void OnAppThreadExit(object sender, EventArgs e)
            {
                Dispose(true);
            }

            /// <summary>
            ///  Revokes our component if needed.
            /// </summary>
            [PrePrepareMethod]
            private void OnDomainUnload(object sender, EventArgs e)
            {
                RevokeComponent();
                ExitDomain();
            }

            /// <summary>
            ///  Called when an untrapped exception occurs in a thread.  This allows the
            ///  programmer to trap these, and, if left untrapped, throws a standard error
            ///  dialog.
            /// </summary>
            internal void OnThreadException(Exception t)
            {
                if (GetState(STATE_INTHREADEXCEPTION))
                {
                    return;
                }

                SetState(STATE_INTHREADEXCEPTION, true);
                try
                {
                    if (threadExceptionHandler != null)
                    {
                        threadExceptionHandler(Thread.CurrentThread, new ThreadExceptionEventArgs(t));
                    }
                    else
                    {
                        if (SystemInformation.UserInteractive)
                        {
                            ThreadExceptionDialog td = new ThreadExceptionDialog(t);
                            DialogResult result = DialogResult.OK;

                            try
                            {
                                result = td.ShowDialog();
                            }
                            finally
                            {
                                td.Dispose();
                            }
                            switch (result)
                            {
                                case DialogResult.Abort:

                                    Application.ExitInternal();

                                    Environment.Exit(0);
                                    break;
                                case DialogResult.Yes:
                                    if (t is WarningException w)
                                    {
                                        Help.ShowHelp(null, w.HelpUrl, w.HelpTopic);
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            // Ignore unhandled thread exceptions. The user can
                            // override if they really care.
                            //
                        }

                    }
                }
                finally
                {
                    SetState(STATE_INTHREADEXCEPTION, false);
                }
            }

            internal void PostQuit()
            {
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : Attempting to terminate message loop");

                // Per http://support.microsoft.com/support/kb/articles/Q183/1/16.ASP
                //
                // WM_QUIT may be consumed by another message pump under very specific circumstances.
                // When that occurs, we rely on the STATE_POSTEDQUIT to be caught in the next
                // idle, at which point we can tear down.
                //
                // We can't follow the KB article exactly, becasue we don't have an HWND to PostMessage
                // to.
                //
                UnsafeNativeMethods.PostThreadMessage(id, Interop.WindowMessages.WM_QUIT, IntPtr.Zero, IntPtr.Zero);
                SetState(STATE_POSTEDQUIT, true);
            }

            // Allows the hosting environment to register a callback
            internal void RegisterMessageLoop(MessageLoopCallback callback)
            {
                messageLoopCallback = callback;
            }

            /// <summary>
            ///  Removes a message filter previously installed with addMessageFilter.
            /// </summary>
            internal void RemoveMessageFilter(IMessageFilter f)
            {
                if (messageFilters != null)
                {
                    SetState(STATE_FILTERSNAPSHOTVALID, false);
                    messageFilters.Remove(f);
                }
            }

            /// <summary>
            ///  Starts a message loop for the given reason.
            /// </summary>
            internal void RunMessageLoop(int reason, ApplicationContext context)
            {
                // Ensure that we attempt to apply theming before doing anything
                // that might create a window.

                IntPtr userCookie = IntPtr.Zero;
                if (s_useVisualStyles)
                {
                    userCookie = UnsafeNativeMethods.ThemingScope.Activate();
                }

                try
                {
                    RunMessageLoopInner(reason, context);
                }
                finally
                {
                    UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
                }
            }

            private void RunMessageLoopInner(int reason, ApplicationContext context)
            {

                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ThreadContext.PushMessageLoop {");
                Debug.Indent();

                if (reason == NativeMethods.MSOCM.msoloopModalForm && !SystemInformation.UserInteractive)
                {
                    throw new InvalidOperationException(SR.CantShowModalOnNonInteractive);
                }

                // if we've entered because of a Main message loop being pushed
                // (different than a modal message loop or DoEVents loop)
                // then clear the QUIT flag to allow normal processing.
                // this flag gets set during loop teardown for another form.
                if (reason == NativeMethods.MSOCM.msoloopMain)
                {
                    SetState(STATE_POSTEDQUIT, false);
                }

                if (totalMessageLoopCount++ == 0)
                {
                    baseLoopReason = reason;
                }

                messageLoopCount++;

                if (reason == NativeMethods.MSOCM.msoloopMain)
                {
                    // If someone has tried to push another main message loop on this thread, ignore
                    // it.
                    if (messageLoopCount != 1)
                    {
                        throw new InvalidOperationException(SR.CantNestMessageLoops);
                    }

                    applicationContext = context;

                    applicationContext.ThreadExit += new EventHandler(OnAppThreadExit);

                    if (applicationContext.MainForm != null)
                    {
                        applicationContext.MainForm.Visible = true;
                    }

                    DpiHelper.InitializeDpiHelperForWinforms();
                }

                Form oldForm = currentForm;
                if (context != null)
                {
                    currentForm = context.MainForm;
                }

                bool fullModal = false;
                bool localModal = false;
                HandleRef hwndOwner = new HandleRef(null, IntPtr.Zero);

                if (reason == NativeMethods.MSOCM.msoloopDoEventsModal)
                {
                    localModal = true;
                }

                if (reason == NativeMethods.MSOCM.msoloopModalForm || reason == NativeMethods.MSOCM.msoloopModalAlert)
                {
                    fullModal = true;

                    // We're about to disable all windows in the thread so our modal dialog can be the top dog.  Because this can interact
                    // with external MSO things, and also because the modal dialog could have already had its handle created,
                    // Check to see if the handle exists and if the window is currently enabled. We remember this so we can set the
                    // window back to enabled after disabling everyone else.  This is just a precaution against someone doing the
                    // wrong thing and disabling our dialog.
                    //
                    bool modalEnabled = currentForm != null && currentForm.Enabled;

                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "[0x" + Convert.ToString(componentID, 16) + "] Notifying component manager that we are entering a modal loop");
                    BeginModalMessageLoop(context);

                    // If the owner window of the dialog is still enabled, disable it now.
                    // This can happen if the owner window is from a different thread or
                    // process.
                    hwndOwner = new HandleRef(null, UnsafeNativeMethods.GetWindowLong(new HandleRef(currentForm, currentForm.Handle), NativeMethods.GWL_HWNDPARENT));
                    if (hwndOwner.Handle != IntPtr.Zero)
                    {
                        if (SafeNativeMethods.IsWindowEnabled(hwndOwner))
                        {
                            SafeNativeMethods.EnableWindow(hwndOwner, false);
                        }
                        else
                        {
                            // reset hwndOwner so we are not tempted to
                            // fiddle with it
                            hwndOwner = new HandleRef(null, IntPtr.Zero);
                        }
                    }

                    // The second half of the the modalEnabled flag above.  Here, if we were previously
                    // enabled, make sure that's still the case.
                    //
                    if (currentForm != null &&
                        currentForm.IsHandleCreated &&
                        SafeNativeMethods.IsWindowEnabled(new HandleRef(currentForm, currentForm.Handle)) != modalEnabled)
                    {
                        SafeNativeMethods.EnableWindow(new HandleRef(currentForm, currentForm.Handle), modalEnabled);
                    }
                }

                try
                {
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "[0x" + Convert.ToString(componentID, 16) + "] Calling ComponentManager.FPushMessageLoop...");
                    bool result;

                    // Register marshaller for background tasks.  At this point,
                    // need to be able to successfully get the handle to the
                    // parking window.  Only do it when we're entering the first
                    // message loop for this thread.
                    if (messageLoopCount == 1)
                    {
                        WindowsFormsSynchronizationContext.InstallIfNeeded();
                    }

                    //need to do this in a try/finally.  Also good to do after we installed the synch context.
                    if (fullModal && currentForm != null)
                    {
                        currentForm.Visible = true;
                    }

                    if ((!fullModal && !localModal) || ComponentManager is ComponentManager)
                    {
                        result = ComponentManager.FPushMessageLoop((IntPtr)componentID, reason, 0);
                    }
                    else if (reason == NativeMethods.MSOCM.msoloopDoEvents ||
                             reason == NativeMethods.MSOCM.msoloopDoEventsModal)
                    {
                        result = LocalModalMessageLoop(null);
                    }
                    else
                    {
                        result = LocalModalMessageLoop(currentForm);
                    }

                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "[0x" + Convert.ToString(componentID, 16) + "] ComponentManager.FPushMessageLoop returned " + result.ToString());
                }
                finally
                {

                    if (fullModal)
                    {
                        Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "[0x" + Convert.ToString(componentID, 16) + "] Notifying component manager that we are exiting a modal loop");
                        EndModalMessageLoop(context);

                        // Again, if the hwndOwner was valid and disabled above, re-enable it.
                        if (hwndOwner.Handle != IntPtr.Zero)
                        {
                            SafeNativeMethods.EnableWindow(hwndOwner, true);
                        }
                    }

                    currentForm = oldForm;
                    totalMessageLoopCount--;
                    messageLoopCount--;

                    if (messageLoopCount == 0)
                    {
                        // If last message loop shutting down, install the
                        // previous op sync context in place before we started the first
                        // message loop.
                        WindowsFormsSynchronizationContext.Uninstall(false);
                    }

                    if (reason == NativeMethods.MSOCM.msoloopMain)
                    {
                        Dispose(true);
                    }
                    else if (messageLoopCount == 0 && componentManager != null)
                    {
                        // If we had a component manager, detach from it.
                        //
                        RevokeComponent();
                    }
                }

                Debug.Unindent();
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "}");
            }

            private bool LocalModalMessageLoop(Form form)
            {
                try
                {
                    // Execute the message loop until the active component tells us to stop.
                    //
                    NativeMethods.MSG msg = new NativeMethods.MSG();
                    bool unicodeWindow = false;
                    bool continueLoop = true;

                    while (continueLoop)
                    {

                        bool peeked = UnsafeNativeMethods.PeekMessage(ref msg, NativeMethods.NullHandleRef, 0, 0, NativeMethods.PM_NOREMOVE);

                        if (peeked)
                        {

                            // If the component wants us to process the message, do it.
                            // The component manager hosts windows from many places.  We must be sensitive
                            // to ansi / Unicode windows here.
                            //
                            if (msg.hwnd != IntPtr.Zero && SafeNativeMethods.IsWindowUnicode(new HandleRef(null, msg.hwnd)))
                            {
                                unicodeWindow = true;
                                if (!UnsafeNativeMethods.GetMessageW(ref msg, NativeMethods.NullHandleRef, 0, 0))
                                {
                                    continue;
                                }

                            }
                            else
                            {
                                unicodeWindow = false;
                                if (!UnsafeNativeMethods.GetMessageA(ref msg, NativeMethods.NullHandleRef, 0, 0))
                                {
                                    continue;
                                }
                            }

                            if (!PreTranslateMessage(ref msg))
                            {
                                UnsafeNativeMethods.TranslateMessage(ref msg);
                                if (unicodeWindow)
                                {
                                    UnsafeNativeMethods.DispatchMessageW(ref msg);
                                }
                                else
                                {
                                    UnsafeNativeMethods.DispatchMessageA(ref msg);
                                }
                            }

                            if (form != null)
                            {
                                continueLoop = !form.CheckCloseDialog(false);
                            }
                        }
                        else if (form == null)
                        {
                            break;
                        }
                        else if (!UnsafeNativeMethods.PeekMessage(ref msg, NativeMethods.NullHandleRef, 0, 0, NativeMethods.PM_NOREMOVE))
                        {
                            UnsafeNativeMethods.WaitMessage();
                        }
                    }
                    return continueLoop;
                }
                catch
                {
                    return false;
                }
            }

            internal bool ProcessFilters(ref NativeMethods.MSG msg, out bool modified)
            {
                bool filtered = false;

                modified = false;

                // Account for the case where someone removes a message filter as a result of PreFilterMessage.
                // The message filter will be removed from _the next_ message.
                //
                // If a message filter is added or removed inside the user-provided PreFilterMessage function,
                // and user code pumps messages, we might re-enter ProcessFilter on the same stack, and we
                // should not update the snapshot until the next message.
                if (messageFilters != null && !GetState(STATE_FILTERSNAPSHOTVALID) && inProcessFilters == 0)
                {
                    messageFilterSnapshot.Clear();
                    if (messageFilters.Count > 0)
                    {
                        messageFilterSnapshot.AddRange(messageFilters);
                    }
                    SetState(STATE_FILTERSNAPSHOTVALID, true);
                }

                inProcessFilters++;
                try
                {
                    if (messageFilterSnapshot != null && messageFilterSnapshot.Count != 0)
                    {
                        IMessageFilter f;
                        int count = messageFilterSnapshot.Count;

                        Message m = Message.Create(msg.hwnd, msg.message, msg.wParam, msg.lParam);

                        for (int i = 0; i < count; i++)
                        {
                            f = messageFilterSnapshot[i];
                            bool filterMessage = f.PreFilterMessage(ref m);
                            // make sure that we update the msg struct with the new result after the call to
                            // PreFilterMessage.
                            if (f is IMessageModifyAndFilter)
                            {
                                msg.hwnd = m.HWnd;
                                msg.message = m.Msg;
                                msg.wParam = m.WParam;
                                msg.lParam = m.LParam;
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
                    inProcessFilters--;
                }

                return filtered;
            }

            /// <summary>
            ///  Message filtering routine that is called before dispatching a message.
            ///  If this returns true, the message is already processed.  If it returns
            ///  false, the message should be allowed to continue through the dispatch
            ///  mechanism.
            /// </summary>
            internal bool PreTranslateMessage(ref NativeMethods.MSG msg)
            {
                if (ProcessFilters(ref msg, out bool modified))
                {
                    return true;
                }

                if (msg.message >= Interop.WindowMessages.WM_KEYFIRST
                        && msg.message <= Interop.WindowMessages.WM_KEYLAST)
                {
                    if (msg.message == Interop.WindowMessages.WM_CHAR)
                    {
                        int breakLParamMask = 0x1460000; // 1 = extended keyboard, 46 = scan code
                        if (unchecked((int)(long)msg.wParam) == 3 && (unchecked((int)(long)msg.lParam) & breakLParamMask) == breakLParamMask)
                        { // ctrl-brk
                            // wParam is the key character, which for ctrl-brk is the same as ctrl-C.
                            // So we need to go to the lparam to distinguish the two cases.
                            // You might also be able to do this with WM_KEYDOWN (again with wParam=3)

                            if (Debugger.IsAttached)
                            {
                                Debugger.Break();
                            }
                        }
                    }
                    Control target = Control.FromChildHandle(msg.hwnd);
                    bool retValue = false;

                    Message m = Message.Create(msg.hwnd, msg.message, msg.wParam, msg.lParam);

                    if (target != null)
                    {
                        if (NativeWindow.WndProcShouldBeDebuggable)
                        {
                            // we don't want to do a catch in the debuggable case.
                            //
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
                        // winforms code.  This can happen with ActiveX controls that launch dialogs specificially
                        //

                        // first, get the first top-level window in the hierarchy.
                        //
                        IntPtr hwndRoot = UnsafeNativeMethods.GetAncestor(new HandleRef(null, msg.hwnd), NativeMethods.GA_ROOT);

                        // if we got a valid HWND, then call IsDialogMessage on it.  If that returns true, it's been processed
                        // so we should return true to prevent Translate/Dispatch from being called.
                        //
                        if (hwndRoot != IntPtr.Zero && UnsafeNativeMethods.IsDialogMessage(new HandleRef(null, hwndRoot), ref msg))
                        {
                            return true;
                        }
                    }

                    msg.wParam = m.WParam;
                    msg.lParam = m.LParam;

                    if (retValue)
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            ///  Revokes our component from the active component manager.  Does
            ///  nothing if there is no active component manager or we are
            ///  already invoked.
            /// </summary>
            private void RevokeComponent()
            {
                if (componentManager != null && componentID != INVALID_ID)
                {
                    int id = componentID;
                    UnsafeNativeMethods.IMsoComponentManager msocm = componentManager;

                    try
                    {
                        msocm.FRevokeComponent((IntPtr)id);
                        if (Marshal.IsComObject(msocm))
                        {
                            Marshal.ReleaseComObject(msocm);
                        }
                    }
                    finally
                    {
                        componentManager = null;
                        componentID = INVALID_ID;
                    }
                }
            }

            /// <summary>
            ///  Sets the culture for this thread.
            /// </summary>
            internal void SetCulture(CultureInfo culture)
            {
                if (culture != null && culture.LCID != (int)Kernel32.GetThreadLocale())
                {
                    SafeNativeMethods.SetThreadLocale(culture.LCID);
                }
            }

            private void SetState(int bit, bool value)
            {
                if (value)
                {
                    threadState |= bit;
                }
                else
                {
                    threadState &= (~bit);
                }
            }

            ///////////////////////////////////////////////////////////////////////////////////////////////////////

            /****************************************************************************************
             *
             *                                  IMsoComponent
             *
             ****************************************************************************************/

            // Things to test in VS when you change this code:
            // - You can bring up dialogs multiple times (ie, the editor for TextBox.Lines)
            // - Double-click DataFormWizard, cancel wizard
            // - When a dialog is open and you switch to another application, when you switch
            //   back to VS the dialog gets the focus
            // - If one modal dialog launches another, they are all modal (Try web forms Table\Rows\Cell)
            // - When a dialog is up, VS is completely disabled, including moving and resizing VS.
            // - After doing all this, you can ctrl-shift-N start a new project and VS is enabled.

            /// <summary>
            ///  Standard FDebugMessage method.
            ///  Since IMsoComponentManager is a reference counted interface,
            ///  MsoDWGetChkMemCounter should be used when processing the
            ///  msodmWriteBe message.
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponent.FDebugMessage(IntPtr hInst, int msg, IntPtr wparam, IntPtr lparam)
            {
                return false;
            }

            /// <summary>
            ///  Give component a chance to process the message pMsg before it is
            ///  translated and dispatched. Component can do TranslateAccelerator
            ///  do IsDialogMessage, modify pMsg, or take some other action.
            ///  Return TRUE if the message is consumed, FALSE otherwise.
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponent.FPreTranslateMessage(ref NativeMethods.MSG msg)
            {
                return PreTranslateMessage(ref msg);
            }

            /// <summary>
            ///  Notify component when app enters or exits (as indicated by fEnter)
            ///  the state identified by uStateID (a value from olecstate enumeration).
            ///  Component should take action depending on value of uStateID
            ///  (see olecstate comments, above).
            ///
            ///  Note: If n calls are made with TRUE fEnter, component should consider
            ///  the state to be in effect until n calls are made with FALSE fEnter.
            ///
            ///  Note: Components should be aware that it is possible for this method to
            ///  be called with FALSE fEnter more    times than it was called with TRUE
            ///  fEnter (so, for example, if component is maintaining a state counter
            ///  (incremented when this method is called with TRUE fEnter, decremented
            ///  when called with FALSE fEnter), the counter should not be decremented
            ///  for FALSE fEnter if it is already at zero.)
            /// </summary>
            void UnsafeNativeMethods.IMsoComponent.OnEnterState(int uStateID, bool fEnter)
            {

                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : OnEnterState(" + uStateID + ", " + fEnter + ")");

                // Return if our (WINFORMS) Modal Loop is still running.
                if (ourModalLoop)
                {
                    return;
                }
                if (uStateID == NativeMethods.MSOCM.msocstateModal)
                {
                    // We should only be messing with windows we own.  See the "ctrl-shift-N" test above.
                    if (fEnter)
                    {
                        DisableWindowsForModalLoop(true, null); // WinFormsOnly = true
                    }
                    else
                    {
                        EnableWindowsForModalLoop(true, null); // WinFormsOnly = true
                    }
                }
            }

            /// <summary>
            ///  Notify component when the host application gains or loses activation.
            ///  If fActive is TRUE, the host app is being activated and dwOtherThreadID
            ///  is the ID of the thread owning the window being deactivated.
            ///  If fActive is FALSE, the host app is being deactivated and
            ///  dwOtherThreadID is the ID of the thread owning the window being
            ///  activated.
            ///  Note: this method is not called when both the window being activated
            ///  and the one being deactivated belong to the host app.
            /// </summary>
            void UnsafeNativeMethods.IMsoComponent.OnAppActivate(bool fActive, int dwOtherThreadID)
            {
            }

            /// <summary>
            ///  Notify the active component that it has lost its active status because
            ///  the host or another component has become active.
            /// </summary>
            void UnsafeNativeMethods.IMsoComponent.OnLoseActivation()
            {
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : Our component is losing activation.");
            }

            /// <summary>
            ///  Notify component when a new object is being activated.
            ///  If pic is non-NULL, then it is the component that is being activated.
            ///  In this case, fSameComponent is TRUE if pic is the same component as
            ///  the callee of this method, and pcrinfo is the reg info of pic.
            ///  If pic is NULL and fHostIsActivating is TRUE, then the host is the
            ///  object being activated, and pchostinfo is its host info.
            ///  If pic is NULL and fHostIsActivating is FALSE, then there is no current
            ///  active object.
            ///
            ///  If pic is being activated and pcrinfo->grf has the
            ///  olecrfExclusiveBorderSpace bit set, component should hide its border
            ///  space tools (toolbars, status bars, etc.);
            ///  component should also do this if host is activating and
            ///  pchostinfo->grfchostf has the olechostfExclusiveBorderSpace bit set.
            ///  In either of these cases, component should unhide its border space
            ///  tools the next time it is activated.
            ///
            ///  if pic is being activated and pcrinfo->grf has the
            ///  olecrfExclusiveActivation bit is set, then pic is being activated in
            ///  "ExclusiveActive" mode.
            ///  Component should retrieve the top frame window that is hosting pic
            ///  (via pic->HwndGetWindow(olecWindowFrameToplevel, 0)).
            ///  If this window is different from component's own top frame window,
            ///  component should disable its windows and do other things it would do
            ///  when receiving OnEnterState(olecstateModal, TRUE) notification.
            ///  Otherwise, if component is top-level,
            ///  it should refuse to have its window activated by appropriately
            ///  processing WM_MOUSEACTIVATE (but see WM_MOUSEACTIVATE NOTE, above).
            ///  Component should remain in one of these states until the
            ///  ExclusiveActive mode ends, indicated by a future call to
            ///  OnActivationChange with ExclusiveActivation bit not set or with NULL
            ///  pcrinfo.
            /// </summary>
            void UnsafeNativeMethods.IMsoComponent.OnActivationChange(UnsafeNativeMethods.IMsoComponent component, bool fSameComponent,
                                                  int pcrinfo,
                                                  bool fHostIsActivating,
                                                  int pchostinfo,
                                                  int dwReserved)
            {
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : OnActivationChange");
            }

            /// <summary>
            ///  Give component a chance to do idle time tasks.  grfidlef is a group of
            ///  bit flags taken from the enumeration of oleidlef values (above),
            ///  indicating the type of idle tasks to perform.
            ///  Component may periodically call IOleComponentManager::FContinueIdle;
            ///  if this method returns FALSE, component should terminate its idle
            ///  time processing and return.
            ///  Return TRUE if more time is needed to perform the idle time tasks,
            ///  FALSE otherwise.
            ///  Note: If a component reaches a point where it has no idle tasks
            ///  and does not need FDoIdle calls, it should remove its idle task
            ///  registration via IOleComponentManager::FUpdateComponentRegistration.
            ///  Note: If this method is called on while component is performing a
            ///  tracking operation, component should only perform idle time tasks that
            ///  it deems are appropriate to perform during tracking.
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponent.FDoIdle(int grfidlef)
            {
                idleHandler?.Invoke(Thread.CurrentThread, EventArgs.Empty);
                return false;
            }

            /// <summary>
            ///  Called during each iteration of a message loop that the component
            ///  pushed. uReason and pvLoopData are the reason and the component private
            ///  data that were passed to IOleComponentManager::FPushMessageLoop.
            ///  This method is called after peeking the next message in the queue
            ///  (via PeekMessage) but before the message is removed from the queue.
            ///  The peeked message is passed in the pMsgPeeked param (NULL if no
            ///  message is in the queue).  This method may be additionally called when
            ///  the next message has already been removed from the queue, in which case
            ///  pMsgPeeked is passed as NULL.
            ///  Return TRUE if the message loop should continue, FALSE otherwise.
            ///  If FALSE is returned, the component manager terminates the loop without
            ///  removing pMsgPeeked from the queue.
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponent.FContinueMessageLoop(int reason, int pvLoopData, NativeMethods.MSG[] msgPeeked)
            {

                bool continueLoop = true;

                // If we get a null message, and we have previously posted the WM_QUIT message,
                // then someone ate the message...
                //
                if (msgPeeked == null && GetState(STATE_POSTEDQUIT))
                {
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : Abnormal loop termination, no WM_QUIT received");
                    continueLoop = false;
                }
                else
                {
                    switch (reason)
                    {
                        case NativeMethods.MSOCM.msoloopFocusWait:

                            // For focus wait, check to see if we are now the active application.
                            //
                            int pid;
                            SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(null, UnsafeNativeMethods.GetActiveWindow()), out pid);
                            if (pid == SafeNativeMethods.GetCurrentProcessId())
                            {
                                continueLoop = false;
                            }
                            break;

                        case NativeMethods.MSOCM.msoloopModalAlert:
                        case NativeMethods.MSOCM.msoloopModalForm:

                            // For modal forms, check to see if the current active form has been
                            // dismissed.  If there is no active form, then it is an error that
                            // we got into here, so we terminate the loop.
                            //
                            if (currentForm == null || currentForm.CheckCloseDialog(false))
                            {
                                continueLoop = false;
                            }
                            break;

                        case NativeMethods.MSOCM.msoloopDoEvents:
                        case NativeMethods.MSOCM.msoloopDoEventsModal:
                            // For DoEvents, just see if there are more messages on the queue.
                            //
                            if (!UnsafeNativeMethods.PeekMessage(ref tempMsg, NativeMethods.NullHandleRef, 0, 0, NativeMethods.PM_NOREMOVE))
                            {
                                continueLoop = false;
                            }

                            break;
                    }
                }

                return continueLoop;
            }

            /// <summary>
            ///  Called when component manager wishes to know if the component is in a
            ///  state in which it can terminate.  If fPromptUser is FALSE, component
            ///  should simply return TRUE if it can terminate, FALSE otherwise.
            ///  If fPromptUser is TRUE, component should return TRUE if it can
            ///  terminate without prompting the user; otherwise it should prompt the
            ///  user, either 1.) asking user if it can terminate and returning TRUE
            ///  or FALSE appropriately, or 2.) giving an indication as to why it
            ///  cannot terminate and returning FALSE.
            /// </summary>
            bool UnsafeNativeMethods.IMsoComponent.FQueryTerminate(bool fPromptUser)
            {
                return true;
            }

            /// <summary>
            ///  Called when component manager wishes to terminate the component's
            ///  registration.  Component should revoke its registration with component
            ///  manager, release references to component manager and perform any
            ///  necessary cleanup.
            /// </summary>
            void UnsafeNativeMethods.IMsoComponent.Terminate()
            {
                if (messageLoopCount > 0 && !(ComponentManager is ComponentManager))
                {
                    messageLoopCount--;
                }

                Dispose(false);
            }

            /// <summary>
            ///  Called to retrieve a window associated with the component, as specified
            ///  by dwWhich, a olecWindowXXX value (see olecWindow, above).
            ///  dwReserved is reserved for future use and should be zero.
            ///  Component should return the desired window or NULL if no such window
            ///  exists.
            /// </summary>
            IntPtr UnsafeNativeMethods.IMsoComponent.HwndGetWindow(int dwWhich, int dwReserved)
            {
                return IntPtr.Zero;
            }
        }
    }
}

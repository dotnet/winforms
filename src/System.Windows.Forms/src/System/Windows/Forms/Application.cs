// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;
using Directory = System.IO.Directory;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides <see langword='static '/>
    ///  methods and properties
    ///  to manage an application, such as methods to run and quit an application,
    ///  to process Windows messages, and properties to get information about an application. This
    ///  class cannot be inherited.
    /// </summary>
    public sealed partial class Application
    {
        /// <summary>
        ///  Hash table for our event list
        /// </summary>
        static EventHandlerList eventHandlers;
        static string startupPath;
        static string executablePath;
        static object appFileVersion;
        static Type mainType;
        static string companyName;
        static string productName;
        static string productVersion;
        static string safeTopLevelCaptionSuffix;
        private static bool s_useVisualStyles = false;
        static bool comCtlSupportsVisualStylesInitialized = false;
        static bool comCtlSupportsVisualStyles = false;
        private static FormCollection s_forms = null;
        private static readonly object internalSyncObject = new object();
        static bool useWaitCursor = false;

        private static bool useEverettThreadAffinity = false;
        private static bool checkedThreadAffinity = false;
        private const string everettThreadAffinityValue = "EnableSystemEventsThreadAffinityCompatibility";

        /// <summary>
        ///  in case Application.exit gets called recursively
        /// </summary>
        private static bool exiting;

        /// <summary>
        ///  Events the user can hook into
        /// </summary>
        private static readonly object EVENT_APPLICATIONEXIT = new object();
        private static readonly object EVENT_THREADEXIT = new object();

        // Constant string used in Application.Restart()
        private const string IEEXEC = "ieexec.exe";

        // Defines a new callback delegate type
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public delegate bool MessageLoopCallback();

        /// <summary>
        ///  This class is static, there is no need to ever create it.
        /// </summary>
        private Application()
        {
        }

        /// <summary>
        ///  Determines if the caller should be allowed to quit the application.  This will return false,
        ///  for example, if being called from a windows forms control being hosted within a web browser.  The
        ///  windows forms control should not attempt to quit the application.
        /// </summary>
        public static bool AllowQuit
        {
            get
            {
                return ThreadContext.FromCurrent().GetAllowQuit();
            }
        }

        /// <summary>
        ///  Returns True if it is OK to continue idle processing. Typically called in an Application.Idle event handler.
        /// </summary>
        internal static bool CanContinueIdle
        {
            get
            {
                return ThreadContext.FromCurrent().ComponentManager.FContinueIdle();
            }
        }

        ///  Typically, you shouldn't need to use this directly - use RenderWithVisualStyles instead.
        internal static bool ComCtlSupportsVisualStyles
        {
            get
            {
                if (!comCtlSupportsVisualStylesInitialized)
                {
                    comCtlSupportsVisualStyles = InitializeComCtlSupportsVisualStyles();
                    comCtlSupportsVisualStylesInitialized = true;
                }
                return comCtlSupportsVisualStyles;
            }
        }

        private static bool InitializeComCtlSupportsVisualStyles()
        {
            if (s_useVisualStyles)
            {
                // At this point, we may not have loaded ComCtl6 yet, but it will eventually be loaded,
                // so we return true here. This works because UseVisualStyles, once set, cannot be
                // turned off.
                return true;
            }

            // To see if we are comctl6, we look for a function that is exposed only from comctl6
            // we do not call DllGetVersion or any direct p/invoke, because the binding will be
            // cached.
            // GetModuleHandle  returns a handle to a mapped module without incrementing its
            // reference count.
            IntPtr hModule = Kernel32.GetModuleHandleW(Libraries.Comctl32);
            if (hModule != IntPtr.Zero)
            {
                return Kernel32.GetProcAddress(hModule, "ImageList_WriteEx") != IntPtr.Zero;
            }

            // Load comctl since GetModuleHandle failed to find it
            hModule = Kernel32.LoadLibraryFromSystemPathIfAvailable(Libraries.Comctl32);
            if (hModule == IntPtr.Zero)
            {
                return false;
            }

            return Kernel32.GetProcAddress(hModule, "ImageList_WriteEx") != IntPtr.Zero;
        }

        /// <summary>
        ///  Gets the registry
        ///  key for the application data that is shared among all users.
        /// </summary>
        public static RegistryKey CommonAppDataRegistry
        {
            get
            {
                return Registry.LocalMachine.CreateSubKey(CommonAppDataRegistryKeyName);
            }
        }

        internal static string CommonAppDataRegistryKeyName
        {
            get
            {
                string template = @"Software\{0}\{1}\{2}";
                return string.Format(CultureInfo.CurrentCulture, template,
                                                                      CompanyName,
                                                                      ProductName,
                                                                      ProductVersion);
            }
        }

        internal static bool UseEverettThreadAffinity
        {
            get
            {
                if (!checkedThreadAffinity)
                {
                    checkedThreadAffinity = true;
                    try
                    {
                        //We need access to be able to read from the registry here.  We're not creating a
                        //registry key, nor are we returning information from the registry to the user.
                        RegistryKey key = Registry.LocalMachine.OpenSubKey(CommonAppDataRegistryKeyName);
                        if (key != null)
                        {
                            object value = key.GetValue(everettThreadAffinityValue);
                            key.Close();

                            if (value != null && (int)value != 0)
                            {
                                useEverettThreadAffinity = true;
                            }
                        }
                    }
                    catch (Security.SecurityException)
                    {
                        // Can't read the key: use default value (false)
                    }
                    catch (InvalidCastException)
                    {
                        // Key is of wrong type: use default value (false)
                    }
                }
                return useEverettThreadAffinity;
            }
        }

        /// <summary>
        ///  Gets the path for the application data that is shared among all users.
        /// </summary>
        /// <remarks>
        ///  Don't obsolete these. GetDataPath isn't on SystemInformation, and it provides
        // the Windows logo required adornments to the directory (Company\Product\Version)
        /// </remarks>
        public static string CommonAppDataPath
        {
            get
            {
                return GetDataPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            }
        }

        /// <summary>
        ///  Gets the company name associated with the application.
        /// </summary>
        public static string CompanyName
        {
            get
            {
                lock (internalSyncObject)
                {
                    if (companyName == null)
                    {

                        // custom attribute
                        //
                        Assembly entryAssembly = Assembly.GetEntryAssembly();
                        if (entryAssembly != null)
                        {
                            object[] attrs = entryAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                            if (attrs != null && attrs.Length > 0)
                            {
                                companyName = ((AssemblyCompanyAttribute)attrs[0]).Company;
                            }
                        }

                        // win32 version
                        //
                        if (companyName == null || companyName.Length == 0)
                        {
                            companyName = GetAppFileVersionInfo().CompanyName;
                            if (companyName != null)
                            {
                                companyName = companyName.Trim();
                            }
                        }

                        // fake it with a namespace
                        // won't work with MC++ see GetAppMainType.
                        if (companyName == null || companyName.Length == 0)
                        {
                            Type t = GetAppMainType();

                            if (t != null)
                            {
                                string ns = t.Namespace;

                                if (!string.IsNullOrEmpty(ns))
                                {
                                    int firstDot = ns.IndexOf('.');
                                    if (firstDot != -1)
                                    {
                                        companyName = ns.Substring(0, firstDot);
                                    }
                                    else
                                    {
                                        companyName = ns;
                                    }
                                }
                                else
                                {
                                    // last ditch... no namespace, use product name...
                                    //
                                    companyName = ProductName;
                                }
                            }
                        }
                    }
                }
                return companyName;
            }
        }

        /// <summary>
        ///  Gets
        ///  or sets the locale information for the current thread.
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get
            {
                return Thread.CurrentThread.CurrentCulture;
            }
            set
            {
                Thread.CurrentThread.CurrentCulture = value;
            }
        }

        /// <summary>
        ///  Gets or
        ///  sets the current input language for the current thread.
        /// </summary>
        public static InputLanguage CurrentInputLanguage
        {
            get
            {
                return InputLanguage.CurrentInputLanguage;
            }
            set
            {
                InputLanguage.CurrentInputLanguage = value;
            }
        }

        internal static bool CustomThreadExceptionHandlerAttached
        {
            get
            {
                return ThreadContext.FromCurrent().CustomThreadExceptionHandlerAttached;
            }
        }

        /// <summary>
        ///  Gets the
        ///  path for the executable file that started the application.
        /// </summary>
        public static string ExecutablePath
        {
            get
            {
                if (executablePath == null)
                {
                    Assembly asm = Assembly.GetEntryAssembly();
                    if (asm == null)
                    {
                        StringBuilder sb = UnsafeNativeMethods.GetModuleFileNameLongPath(NativeMethods.NullHandleRef);
                        executablePath = Path.GetFullPath(sb.ToString());
                    }
                    else
                    {
                        string cb = asm.CodeBase;
                        Uri codeBase = new Uri(cb);
                        if (codeBase.IsFile)
                        {
                            executablePath = codeBase.LocalPath + Uri.UnescapeDataString(codeBase.Fragment);
                            ;
                        }
                        else
                        {
                            executablePath = codeBase.ToString();
                        }
                    }
                }

                return executablePath;
            }
        }

        /// <summary>
        ///  Gets the current HighDpi mode for the process.
        /// </summary>
        public static HighDpiMode HighDpiMode
        {
            get
            {
                return DpiHelper.GetWinformsApplicationDpiAwareness();
            }
        }

        /// <summary>
        ///  Sets the HighDpi mode for process.
        /// </summary>
        /// <param name="highDpiMode">The HighDpi mode to set.</param>
        /// <returns></returns>
        public static bool SetHighDpiMode(HighDpiMode highDpiMode)
        {
            if (DpiHelper.FirstParkingWindowCreated)
            {
                return false;
            }

            return DpiHelper.SetWinformsApplicationDpiAwareness(highDpiMode);
        }

        /// <summary>
        ///  Gets the path for the application data specific to a local, non-roaming user.
        /// </summary>
        /// <remarks>
        ///  Don't obsolete these. GetDataPath isn't on SystemInformation, and it provides
        ///  the Windows logo required adornments to the directory (Company\Product\Version)
        /// </remarks>
        public static string LocalUserAppDataPath
        {
            get
            {
                return GetDataPath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            }
        }

        /// <summary>
        ///  Determines if a message loop exists on this thread.
        /// </summary>
        public static bool MessageLoop
        {
            get
            {
                return ThreadContext.FromCurrent().GetMessageLoop();
            }
        }

        /// <summary>
        ///  Gets the forms collection associated with this application.
        /// </summary>
        public static FormCollection OpenForms => s_forms ?? (s_forms = new FormCollection());

        /// <summary>
        ///  Gets
        ///  the product name associated with this application.
        /// </summary>
        public static string ProductName
        {
            get
            {
                lock (internalSyncObject)
                {
                    if (productName == null)
                    {
                        // custom attribute
                        //
                        Assembly entryAssembly = Assembly.GetEntryAssembly();
                        if (entryAssembly != null)
                        {
                            object[] attrs = entryAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                            if (attrs != null && attrs.Length > 0)
                            {
                                productName = ((AssemblyProductAttribute)attrs[0]).Product;
                            }
                        }

                        // win32 version info
                        //
                        if (productName == null || productName.Length == 0)
                        {
                            productName = GetAppFileVersionInfo().ProductName;
                            if (productName != null)
                            {
                                productName = productName.Trim();
                            }
                        }

                        // fake it with namespace
                        // won't work with MC++ see GetAppMainType.
                        if (productName == null || productName.Length == 0)
                        {
                            Type t = GetAppMainType();

                            if (t != null)
                            {
                                string ns = t.Namespace;

                                if (!string.IsNullOrEmpty(ns))
                                {
                                    int lastDot = ns.LastIndexOf('.');
                                    if (lastDot != -1 && lastDot < ns.Length - 1)
                                    {
                                        productName = ns.Substring(lastDot + 1);
                                    }
                                    else
                                    {
                                        productName = ns;
                                    }
                                }
                                else
                                {
                                    // last ditch... use the main type
                                    //
                                    productName = t.Name;
                                }
                            }
                        }
                    }
                }

                return productName;
            }
        }

        /// <summary>
        ///  Gets
        ///  the product version associated with this application.
        /// </summary>
        public static string ProductVersion
        {
            get
            {
                lock (internalSyncObject)
                {
                    if (productVersion == null)
                    {

                        // custom attribute
                        //
                        Assembly entryAssembly = Assembly.GetEntryAssembly();
                        if (entryAssembly != null)
                        {
                            object[] attrs = entryAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                            if (attrs != null && attrs.Length > 0)
                            {
                                productVersion = ((AssemblyInformationalVersionAttribute)attrs[0]).InformationalVersion;
                            }
                        }

                        // win32 version info
                        //
                        if (productVersion == null || productVersion.Length == 0)
                        {
                            productVersion = GetAppFileVersionInfo().ProductVersion;
                            if (productVersion != null)
                            {
                                productVersion = productVersion.Trim();
                            }
                        }

                        // fake it
                        //
                        if (productVersion == null || productVersion.Length == 0)
                        {
                            productVersion = "1.0.0.0";
                        }
                    }
                }
                return productVersion;
            }
        }

        // Allows the hosting environment to register a callback
        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public static void RegisterMessageLoop(MessageLoopCallback callback)
        {
            ThreadContext.FromCurrent().RegisterMessageLoop(callback);
        }

        /// <summary>
        ///  Magic property that answers a simple question - are my controls currently going to render with
        //     visual styles? If you are doing visual styles rendering, use this to be consistent with the rest
        //     of the controls in your app.
        /// </summary>
        public static bool RenderWithVisualStyles
        {
            get
            {
                return (ComCtlSupportsVisualStyles && VisualStyles.VisualStyleRenderer.IsSupported);
            }
        }

        /// <summary>
        ///  Gets or sets the format string to apply to top level window captions
        ///  when they are displayed with a warning banner.
        /// </summary>
        public static string SafeTopLevelCaptionFormat
        {
            get
            {
                if (safeTopLevelCaptionSuffix == null)
                {
                    safeTopLevelCaptionSuffix = SR.SafeTopLevelCaptionFormat; // 0 - original, 1 - zone, 2 - site
                }
                return safeTopLevelCaptionSuffix;
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                safeTopLevelCaptionSuffix = value;
            }
        }

        /// <summary>
        ///  Gets the
        ///  path for the executable file that started the application.
        /// </summary>
        public static string StartupPath
        {
            get
            {
                if (startupPath == null)
                {
                    // StringBuilder sb = UnsafeNativeMethods.GetModuleFileNameLongPath(NativeMethods.NullHandleRef);
                    // startupPath = Path.GetDirectoryName(sb.ToString());
                    startupPath = AppContext.BaseDirectory;
                }
                return startupPath;
            }
        }

        // Allows the hosting environment to unregister a callback
        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public static void UnregisterMessageLoop()
        {
            ThreadContext.FromCurrent().RegisterMessageLoop(null);
        }

        /// <summary>
        ///  Gets
        ///  or sets whether the wait cursor is used for all open forms of the application.
        /// </summary>
        public static bool UseWaitCursor
        {
            get
            {
                return useWaitCursor;
            }
            set
            {
                lock (FormCollection.CollectionSyncRoot)
                {
                    useWaitCursor = value;
                    // Set the WaitCursor of all forms.
                    foreach (Form f in OpenForms)
                    {
                        f.UseWaitCursor = useWaitCursor;
                    }
                }
            }
        }

        /// <summary>
        ///  Gets the path for the application data specific to the roaming user.
        /// </summary>
        /// <remarks>
        ///  Don't obsolete these. GetDataPath isn't on SystemInformation, and it provides
        ///  the Windows logo required adornments to the directory (Company\Product\Version)
        /// </remarks>
        public static string UserAppDataPath
        {
            get
            {
                return GetDataPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            }
        }

        /// <summary>
        ///  Gets the registry key of
        ///  the application data specific to the roaming user.
        /// </summary>
        public static RegistryKey UserAppDataRegistry
        {
            get
            {
                string template = @"Software\{0}\{1}\{2}";
                return Registry.CurrentUser.CreateSubKey(string.Format(CultureInfo.CurrentCulture, template, CompanyName, ProductName, ProductVersion));
            }
        }

        public static bool UseVisualStyles => s_useVisualStyles;

        /// <remarks>
        ///  Don't never ever change this name, since the window class and partner teams
        ///  dependent on this. Changing this will introduce breaking changes.
        ///  If there is some reason need to change this, notify any partner teams affected.
        /// </remarks>
        internal static string WindowsFormsVersion => "WindowsForms10";

        internal static string WindowMessagesVersion => "WindowsForms12";

        /// <summary>
        ///  Use this property to determine how visual styles will be applied to this application.
        ///  This property is meaningful only if visual styles are supported on the current
        ///  platform (VisualStyleInformation.SupportedByOS is true).
        ///
        ///  This property can be set only to one of the S.W.F.VisualStyles.VisualStyleState enum values.
        /// </summary>
        public static VisualStyleState VisualStyleState
        {
            get
            {
                if (!VisualStyleInformation.IsSupportedByOS)
                {
                    return VisualStyleState.NoneEnabled;
                }

                VisualStyleState vState = (VisualStyleState)SafeNativeMethods.GetThemeAppProperties();
                return vState;
            }

            set
            {
                if (VisualStyleInformation.IsSupportedByOS)
                {
                    SafeNativeMethods.SetThemeAppProperties((int)value);

                    //248887 we need to send a WM_THEMECHANGED to the top level windows of this application.
                    //We do it this way to ensure that we get all top level windows -- whether we created them or not.
                    SafeNativeMethods.EnumThreadWindowsCallback callback = new SafeNativeMethods.EnumThreadWindowsCallback(Application.SendThemeChanged);
                    SafeNativeMethods.EnumWindows(callback, IntPtr.Zero);

                    GC.KeepAlive(callback);
                }
            }
        }

        /// <summary>
        ///  This helper broadcasts out a WM_THEMECHANGED to appropriate top level windows of this app.
        /// </summary>
        private unsafe static bool SendThemeChanged(IntPtr handle, IntPtr extraParameter)
        {
            int thisPID = SafeNativeMethods.GetCurrentProcessId();
            SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(null, handle), out int processId);
            if (processId == thisPID && SafeNativeMethods.IsWindowVisible(new HandleRef(null, handle)))
            {

                SendThemeChangedRecursive(handle, IntPtr.Zero);
                User32.RedrawWindow(
                    handle,
                    null,
                    IntPtr.Zero,
                    User32.RedrawWindowOptions.RDW_INVALIDATE | User32.RedrawWindowOptions.RDW_FRAME | User32.RedrawWindowOptions.RDW_ERASE | User32.RedrawWindowOptions.RDW_ALLCHILDREN);
            }
            return true;
        }

        /// <summary>
        ///  This helper broadcasts out a WM_THEMECHANGED this window and all children.
        ///  it is assumed at this point that the handle belongs to the current process and has a visible top level window.
        /// </summary>
        private static bool SendThemeChangedRecursive(IntPtr handle, IntPtr lparam)
        {
            //first send to all children...
            UnsafeNativeMethods.EnumChildWindows(new HandleRef(null, handle),
                new NativeMethods.EnumChildrenCallback(Application.SendThemeChangedRecursive),
                NativeMethods.NullHandleRef);

            //then do myself.
            UnsafeNativeMethods.SendMessage(new HandleRef(null, handle), WindowMessages.WM_THEMECHANGED, 0, 0);

            return true;
        }

        /// <summary>
        ///  Occurs when the application is about to shut down.
        /// </summary>
        public static event EventHandler ApplicationExit
        {
            add => AddEventHandler(EVENT_APPLICATIONEXIT, value);
            remove => RemoveEventHandler(EVENT_APPLICATIONEXIT, value);
        }

        private static void AddEventHandler(object key, Delegate value)
        {
            lock (internalSyncObject)
            {
                if (null == eventHandlers)
                {
                    eventHandlers = new EventHandlerList();
                }
                eventHandlers.AddHandler(key, value);
            }
        }
        private static void RemoveEventHandler(object key, Delegate value)
        {
            lock (internalSyncObject)
            {
                if (null == eventHandlers)
                {
                    return;
                }
                eventHandlers.RemoveHandler(key, value);
            }
        }

        /// <summary>
        ///  Adds a message filter to monitor Windows messages as they are routed to their
        ///  destinations.
        /// </summary>
        public static void AddMessageFilter(IMessageFilter value)
        {
            ThreadContext.FromCurrent().AddMessageFilter(value);
        }

        /// <summary>
        ///  Processes all message filters for given message
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static bool FilterMessage(ref Message message)
        {
            // Create copy of MSG structure
            NativeMethods.MSG msg = new NativeMethods.MSG
            {
                hwnd = message.HWnd,
                message = message.Msg,
                wParam = message.WParam,
                lParam = message.LParam
            };

            bool processed = ThreadContext.FromCurrent().ProcessFilters(ref msg, out bool modified);
            if (modified)
            {
                message.HWnd = msg.hwnd;
                message.Msg = msg.message;
                message.WParam = msg.wParam;
                message.LParam = msg.lParam;
            }

            return processed;
        }

        /// <summary>
        ///  Occurs when the application has finished processing and is about to enter the
        ///  idle state.
        /// </summary>
        public static event EventHandler Idle
        {
            add
            {
                ThreadContext current = ThreadContext.FromCurrent();
                lock (current)
                {
                    current.idleHandler += value;
                    // This just ensures that the component manager is hooked up.  We
                    // need it for idle time processing.
                    //
                    object o = current.ComponentManager;
                }
            }
            remove
            {
                ThreadContext current = ThreadContext.FromCurrent();
                lock (current)
                {
                    current.idleHandler -= value;
                }
            }
        }

        /// <summary>
        ///  Occurs when the application is about to enter a modal state
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static event EventHandler EnterThreadModal
        {
            add
            {
                ThreadContext current = ThreadContext.FromCurrent();
                lock (current)
                {
                    current.enterModalHandler += value;
                }
            }
            remove
            {
                ThreadContext current = ThreadContext.FromCurrent();
                lock (current)
                {
                    current.enterModalHandler -= value;
                }
            }
        }

        /// <summary>
        ///  Occurs when the application is about to leave a modal state
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static event EventHandler LeaveThreadModal
        {
            add
            {
                ThreadContext current = ThreadContext.FromCurrent();
                lock (current)
                {
                    current.leaveModalHandler += value;
                }
            }
            remove
            {
                ThreadContext current = ThreadContext.FromCurrent();
                lock (current)
                {
                    current.leaveModalHandler -= value;
                }
            }
        }

        /// <summary>
        ///  Occurs when an untrapped thread exception is thrown.
        /// </summary>
        public static event ThreadExceptionEventHandler ThreadException
        {
            add
            {
                ThreadContext current = ThreadContext.FromCurrent();
                lock (current)
                {
                    current.threadExceptionHandler = value;
                }
            }
            remove
            {
                ThreadContext current = ThreadContext.FromCurrent();
                lock (current)
                {
                    current.threadExceptionHandler -= value;
                }
            }
        }

        /// <summary>
        ///  Occurs when a thread is about to shut down.  When
        ///  the main thread for an application is about to be shut down,
        ///  this event will be raised first, followed by an ApplicationExit
        ///  event.
        /// </summary>
        public static event EventHandler ThreadExit
        {
            add => AddEventHandler(EVENT_THREADEXIT, value);
            remove => RemoveEventHandler(EVENT_THREADEXIT, value);
        }

        /// <summary>
        ///  Called immediately before we begin pumping messages for a modal message loop.
        ///  Does not actually start a message pump; that's the caller's responsibility.
        /// </summary>
        internal static void BeginModalMessageLoop()
        {
            ThreadContext.FromCurrent().BeginModalMessageLoop(null);
        }

        /// <summary>
        ///  Processes
        ///  all Windows messages currently in the message queue.
        /// </summary>
        public static void DoEvents()
        {
            ThreadContext.FromCurrent().RunMessageLoop(NativeMethods.MSOCM.msoloopDoEvents, null);
        }

        internal static void DoEventsModal()
        {
            ThreadContext.FromCurrent().RunMessageLoop(NativeMethods.MSOCM.msoloopDoEventsModal, null);
        }

        /// <summary>
        ///  Enables visual styles for all subsequent Application.Run() and CreateHandle() calls.
        ///  Uses the default theming manifest file shipped with the redist.
        /// </summary>
        public static void EnableVisualStyles()
        {
            // Pull manifest from our resources
            string assemblyLoc = typeof(Application).Assembly.Location;
            if (assemblyLoc != null)
            {
                // CSC embeds DLL manifests as resource ID 2
                s_useVisualStyles = UnsafeNativeMethods.ThemingScope.CreateActivationContext(assemblyLoc, nativeResourceManifestID: 2);
                Debug.Assert(s_useVisualStyles, "Enable Visual Styles failed");
            }
        }

        /// <summary>
        ///  Called immediately after we stop pumping messages for a modal message loop.
        ///  Does not actually end the message pump itself.
        /// </summary>
        internal static void EndModalMessageLoop()
        {
            ThreadContext.FromCurrent().EndModalMessageLoop(null);
        }

        /// <summary>
        ///  Overload of Exit that does not care about e.Cancel.
        /// </summary>
        public static void Exit()
        {
            Exit(null);
        }

        /// <summary>
        ///  Informs all message pumps that they are to terminate and
        ///  then closes all application windows after the messages have been processed.
        ///  e.Cancel indicates whether any of the open forms cancelled the exit call.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void Exit(CancelEventArgs e)
        {
            bool cancelExit = ExitInternal();
            if (e != null)
            {
                e.Cancel = cancelExit;
            }
        }

        /// <summary>
        ///  Private version of Exit which does not do any security checks.
        /// </summary>
        private static bool ExitInternal()
        {
            bool cancelExit = false;
            lock (internalSyncObject)
            {
                if (exiting)
                {
                    return false;
                }
                exiting = true;

                try
                {
                    // Raise the FormClosing and FormClosed events for each open form
                    if (s_forms != null)
                    {
                        foreach (Form f in OpenForms)
                        {
                            if (f.RaiseFormClosingOnAppExit())
                            {
                                cancelExit = true;
                                break; // quit the loop as soon as one form refuses to close
                            }
                        }
                    }
                    if (!cancelExit)
                    {
                        if (s_forms != null)
                        {
                            while (OpenForms.Count > 0)
                            {
                                OpenForms[0].RaiseFormClosedOnAppExit(); // OnFormClosed removes the form from the FormCollection
                            }
                        }
                        ThreadContext.ExitApplication();
                    }
                }
                finally
                {
                    exiting = false;
                }
            }
            return cancelExit;
        }

        /// <summary>
        ///  Exits the message loop on the
        ///  current thread and closes all windows on the thread.
        /// </summary>
        public static void ExitThread()
        {
            ThreadContext context = ThreadContext.FromCurrent();
            if (context.ApplicationContext != null)
            {
                context.ApplicationContext.ExitThread();
            }
            else
            {
                context.Dispose(true);
            }
        }

        // When a Form receives a WM_ACTIVATE message, it calls this method so we can do the
        // appropriate MsoComponentManager activation magic
        internal static void FormActivated(bool modal, bool activated)
        {
            if (modal)
            {
                return;
            }

            ThreadContext.FromCurrent().FormActivated(activated);
        }

        /// <summary>
        ///  Retrieves the FileVersionInfo associated with the main module for
        ///  the application.
        /// </summary>
        private static FileVersionInfo GetAppFileVersionInfo()
        {
            lock (internalSyncObject)
            {
                if (appFileVersion == null)
                {
                    Type t = GetAppMainType();
                    if (t != null)
                    {
                        appFileVersion = FileVersionInfo.GetVersionInfo(t.Module.FullyQualifiedName);
                    }
                    else
                    {
                        appFileVersion = FileVersionInfo.GetVersionInfo(ExecutablePath);
                    }
                }
            }

            return (FileVersionInfo)appFileVersion;
        }

        /// <summary>
        ///  Retrieves the Type that contains the "Main" method.
        /// </summary>
        private static Type GetAppMainType()
        {
            lock (internalSyncObject)
            {
                if (mainType == null)
                {
                    Assembly exe = Assembly.GetEntryAssembly();

                    // Get Main type...This doesn't work in MC++ because Main is a global function and not
                    // a class static method (it doesn't belong to a Type).
                    if (exe != null)
                    {
                        mainType = exe.EntryPoint.ReflectedType;
                    }
                }
            }

            return mainType;
        }

        /// <summary>
        ///  Locates a thread context given a window handle.
        /// </summary>
        private static ThreadContext GetContextForHandle(HandleRef handle)
        {
            int id = SafeNativeMethods.GetWindowThreadProcessId(handle, out int pid);
            ThreadContext cxt = ThreadContext.FromId(id);
            Debug.Assert(cxt != null, "No thread context for handle.  This is expected if you saw a previous assert about the handle being invalid.");
            return cxt;
        }

        /// <summary>
        ///  Returns a string that is the combination of the
        ///  basePath + CompanyName + ProducName + ProductVersion. This
        ///  will also create the directory if it doesn't exist.
        /// </summary>
        private static string GetDataPath(string basePath)
        {
            string template = @"{0}\{1}\{2}\{3}";

            string company = CompanyName;
            string product = ProductName;
            string version = ProductVersion;

            string path = string.Format(CultureInfo.CurrentCulture, template, new object[] { basePath, company, product, version });
            lock (internalSyncObject)
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            return path;
        }

        /// <summary>
        ///  Called by the last thread context before it shuts down.
        /// </summary>
        private static void RaiseExit()
        {
            if (eventHandlers != null)
            {
                Delegate exit = eventHandlers[EVENT_APPLICATIONEXIT];
                if (exit != null)
                {
                    ((EventHandler)exit)(null, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///  Called by the each thread context before it shuts down.
        /// </summary>
        private static void RaiseThreadExit()
        {
            if (eventHandlers != null)
            {
                Delegate exit = eventHandlers[EVENT_THREADEXIT];
                if (exit != null)
                {
                    ((EventHandler)exit)(null, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///  "Parks" the given HWND to a temporary HWND.  This allows WS_CHILD windows to
        ///  be parked.
        /// </summary>
        internal static void ParkHandle(HandleRef handle, DpiAwarenessContext dpiAwarenessContext = DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED)
        {
            Debug.Assert(UnsafeNativeMethods.IsWindow(handle), "Handle being parked is not a valid window handle");
            Debug.Assert(((int)UnsafeNativeMethods.GetWindowLong(handle, NativeMethods.GWL_STYLE) & NativeMethods.WS_CHILD) != 0, "Only WS_CHILD windows should be parked.");

            ThreadContext cxt = GetContextForHandle(handle);
            if (cxt != null)
            {
                cxt.GetParkingWindow(dpiAwarenessContext).ParkHandle(handle);
            }
        }

        /// <summary>
        ///  Park control handle on a parkingwindow that has matching DpiAwareness.
        /// </summary>
        /// <param name="cp"> create params for control handle</param>
        /// <param name="dpiContext"> dpi awareness</param>
        internal static void ParkHandle(CreateParams cp, DpiAwarenessContext dpiAwarenessContext = DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED)
        {
            ThreadContext cxt = ThreadContext.FromCurrent();
            if (cxt != null)
            {
                cp.Parent = cxt.GetParkingWindow(dpiAwarenessContext).Handle;
            }
        }

        /// <summary>
        ///  Initializes OLE on the current thread.
        /// </summary>
        public static ApartmentState OleRequired()
        {
            return ThreadContext.FromCurrent().OleRequired();
        }

        /// <summary>
        ///  Raises the <see cref='ThreadException'/> event.
        /// </summary>
        public static void OnThreadException(Exception t)
        {
            ThreadContext.FromCurrent().OnThreadException(t);
        }

        /// <summary>
        ///  "Unparks" the given HWND to a temporary HWND.  This allows WS_CHILD windows to
        ///  be parked.
        /// </summary>
        internal static void UnparkHandle(HandleRef handle, DpiAwarenessContext context)
        {
            ThreadContext cxt = GetContextForHandle(handle);
            if (cxt != null)
            {
                cxt.GetParkingWindow(context).UnparkHandle(handle);
            }
        }

        /// <summary>
        ///  Raises the Idle event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void RaiseIdle(EventArgs e)
        {
            ThreadContext current = ThreadContext.FromCurrent();
            current.idleHandler?.Invoke(Thread.CurrentThread, e);
        }

        /// <summary>
        ///  Removes a message
        ///  filter from the application's message pump.
        /// </summary>
        public static void RemoveMessageFilter(IMessageFilter value)
        {
            ThreadContext.FromCurrent().RemoveMessageFilter(value);
        }

        /// <summary>
        ///  Restarts the application.
        /// </summary>
        public static void Restart()
        {
            if (Assembly.GetEntryAssembly() == null)
            {
                throw new NotSupportedException(SR.RestartNotSupported);
            }

            bool hrefExeCase = false;

            Process process = Process.GetCurrentProcess();
            Debug.Assert(process != null);
            if (string.Equals(process.MainModule.ModuleName, IEEXEC, StringComparison.OrdinalIgnoreCase))
            {
                string clrPath = Path.GetDirectoryName(typeof(object).Module.FullyQualifiedName);

                if (string.Equals(clrPath + "\\" + IEEXEC, process.MainModule.FileName, StringComparison.OrdinalIgnoreCase))
                {
                    // HRef exe case
                    hrefExeCase = true;
                    ExitInternal();
                    if (AppDomain.CurrentDomain.GetData("APP_LAUNCH_URL") is string launchUrl)
                    {
                        Process.Start(process.MainModule.FileName, launchUrl);
                    }
                }
            }

            if (!hrefExeCase)
            {
                // Regular app case
                string[] arguments = Environment.GetCommandLineArgs();
                Debug.Assert(arguments != null && arguments.Length > 0);
                StringBuilder sb = new StringBuilder((arguments.Length - 1) * 16);
                for (int argumentIndex = 1; argumentIndex < arguments.Length - 1; argumentIndex++)
                {
                    sb.Append('"');
                    sb.Append(arguments[argumentIndex]);
                    sb.Append("\" ");
                }
                if (arguments.Length > 1)
                {
                    sb.Append('"');
                    sb.Append(arguments[arguments.Length - 1]);
                    sb.Append('"');
                }
                ProcessStartInfo currentStartInfo = Process.GetCurrentProcess().StartInfo;
                currentStartInfo.FileName = Application.ExecutablePath;
                if (sb.Length > 0)
                {
                    currentStartInfo.Arguments = sb.ToString();
                }
                ExitInternal();
                Process.Start(currentStartInfo);
            }
        }

        /// <summary>
        ///  Begins running a
        ///  standard
        ///  application message loop on the current thread,
        ///  without a form.
        /// </summary>
        public static void Run()
        {
            ThreadContext.FromCurrent().RunMessageLoop(NativeMethods.MSOCM.msoloopMain, new ApplicationContext());
        }

        /// <summary>
        ///  Begins running a standard application message loop on the current
        ///  thread, and makes the specified form visible.
        /// </summary>
        public static void Run(Form mainForm)
        {
            ThreadContext.FromCurrent().RunMessageLoop(NativeMethods.MSOCM.msoloopMain, new ApplicationContext(mainForm));
        }

        /// <summary>
        ///  Begins running a
        ///  standard
        ///  application message loop on the current thread,
        ///  without a form.
        /// </summary>
        public static void Run(ApplicationContext context)
        {
            ThreadContext.FromCurrent().RunMessageLoop(NativeMethods.MSOCM.msoloopMain, context);
        }

        /// <summary>
        ///  Runs a modal dialog.  This starts a special type of message loop that runs until
        ///  the dialog has a valid DialogResult.  This is called internally by a form
        ///  when an application calls System.Windows.Forms.Form.ShowDialog().
        /// </summary>
        internal static void RunDialog(Form form)
        {
            ThreadContext.FromCurrent().RunMessageLoop(NativeMethods.MSOCM.msoloopModalForm, new ModalApplicationContext(form));
        }

        /// <summary>
        ///  Sets the static UseCompatibleTextRenderingDefault field on Control to the value passed in.
        ///  This switch determines the default text rendering engine to use by some controls that support
        ///  switching rendering engine.
        /// </summary>
        public static void SetCompatibleTextRenderingDefault(bool defaultValue)
        {
            if (NativeWindow.AnyHandleCreated)
            {
                throw new InvalidOperationException(SR.Win32WindowAlreadyCreated);
            }
            Control.UseCompatibleTextRenderingDefault = defaultValue;
        }

        /// <summary>
        ///  Sets the suspend/hibernate state of the machine.
        ///  Returns true if the call succeeded, else false.
        /// </summary>
        public static bool SetSuspendState(PowerState state, bool force, bool disableWakeEvent)
        {
            return UnsafeNativeMethods.SetSuspendState(state == PowerState.Hibernate, force, disableWakeEvent);
        }

        /// <summary>
        ///  Overload version of SetUnhandledExceptionMode that sets the UnhandledExceptionMode
        ///  mode at the current thread level.
        /// </summary>
        public static void SetUnhandledExceptionMode(UnhandledExceptionMode mode)
        {
            SetUnhandledExceptionMode(mode, true /*threadScope*/);
        }

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
        ///  The parameter threadScope defines the scope of the setting: either
        ///  the current thread or the application.
        ///  When a thread exception mode isn't UnhandledExceptionMode.Automatic, it takes
        ///  precedence over the application exception mode.
        /// </summary>
        public static void SetUnhandledExceptionMode(UnhandledExceptionMode mode, bool threadScope)
        {
            NativeWindow.SetUnhandledExceptionModeInternal(mode, threadScope);
        }
    }
}

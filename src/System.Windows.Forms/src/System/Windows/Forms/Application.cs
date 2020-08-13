// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

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
using static Interop;
using Directory = System.IO.Directory;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides <see langword='static'/> methods and properties to manage an application, such as methods to run and quit an application,
    ///  to process Windows messages, and properties to get information about an application.
    ///  This class cannot be inherited.
    /// </summary>
    public sealed partial class Application
    {
        /// <summary>
        ///  Hash table for our event list
        /// </summary>
        private static EventHandlerList s_eventHandlers;
        private static string s_startupPath;
        private static string s_executablePath;
        private static object s_appFileVersion;
        private static Type s_mainType;
        private static string s_companyName;
        private static string s_productName;
        private static string s_productVersion;
        private static string s_safeTopLevelCaptionSuffix;
        private static bool s_comCtlSupportsVisualStylesInitialized;
        private static bool s_comCtlSupportsVisualStyles;
        private static FormCollection s_forms;
        private static readonly object s_internalSyncObject = new object();
        private static bool s_useWaitCursor;

        private static bool s_useEverettThreadAffinity;
        private static bool s_checkedThreadAffinity;
        private const string EverettThreadAffinityValue = "EnableSystemEventsThreadAffinityCompatibility";

        /// <summary>
        ///  Events the user can hook into
        /// </summary>
        private static readonly object s_eventApplicationExit = new object();
        private static readonly object s_eventThreadExit = new object();

        // Constant string used in Application.Restart()
        private const string IEEXEC = "ieexec.exe";

        // Defines a new callback delegate type
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public delegate bool MessageLoopCallback();

        // Used to avoid recursive exit
        private static bool s_exiting;

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
            => ThreadContext.FromCurrent().GetAllowQuit();

        /// <summary>
        ///  Returns True if it is OK to continue idle processing. Typically called in an Application.Idle event handler.
        /// </summary>
        internal static bool CanContinueIdle
            => ThreadContext.FromCurrent().ComponentManager.FContinueIdle().IsTrue();

        /// <summary>
        ///  Typically, you shouldn't need to use this directly - use RenderWithVisualStyles instead.
        /// </summary>
        internal static bool ComCtlSupportsVisualStyles
        {
            get
            {
                if (!s_comCtlSupportsVisualStylesInitialized)
                {
                    s_comCtlSupportsVisualStyles = InitializeComCtlSupportsVisualStyles();
                    s_comCtlSupportsVisualStylesInitialized = true;
                }
                return s_comCtlSupportsVisualStyles;
            }
        }

        private static bool InitializeComCtlSupportsVisualStyles()
        {
            if (UseVisualStyles)
            {
                // At this point, we may not have loaded ComCtl6 yet, but it will eventually be loaded,
                // so we return true here. This works because UseVisualStyles, once set, cannot be
                // turned off.
                return true;
            }

            // To see if we are comctl6, we look for a function that is exposed only from comctl6
            // we do not call DllGetVersion or any direct p/invoke, because the binding will be
            // cached.
            //
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
        ///  Gets the registry key for the application data that is shared among all users.
        /// </summary>
        public static RegistryKey CommonAppDataRegistry
            => Registry.LocalMachine.CreateSubKey(CommonAppDataRegistryKeyName);

        internal static string CommonAppDataRegistryKeyName
            => $"Software\\{CompanyName}\\{ProductName}\\{ProductVersion}";

        internal static bool UseEverettThreadAffinity
        {
            get
            {
                if (!s_checkedThreadAffinity)
                {
                    s_checkedThreadAffinity = true;
                    try
                    {
                        // We need access to be able to read from the registry here.  We're not creating a
                        // registry key, nor are we returning information from the registry to the user.
                        RegistryKey key = Registry.LocalMachine.OpenSubKey(CommonAppDataRegistryKeyName);
                        if (key != null)
                        {
                            object value = key.GetValue(EverettThreadAffinityValue);
                            key.Close();

                            if (value != null && (int)value != 0)
                            {
                                s_useEverettThreadAffinity = true;
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
                return s_useEverettThreadAffinity;
            }
        }

        /// <summary>
        ///  Gets the path for the application data that is shared among all users.
        /// </summary>
        /// <remarks>
        ///  Don't obsolete these. GetDataPath isn't on SystemInformation, and it provides
        ///  the Windows logo required adornments to the directory (Company\Product\Version)
        /// </remarks>
        public static string CommonAppDataPath
            => GetDataPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));

        /// <summary>
        ///  Gets the company name associated with the application.
        /// </summary>
        public static string CompanyName
        {
            get
            {
                lock (s_internalSyncObject)
                {
                    if (s_companyName is null)
                    {
                        // Custom attribute
                        Assembly entryAssembly = Assembly.GetEntryAssembly();
                        if (entryAssembly != null)
                        {
                            object[] attrs = entryAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                            if (attrs != null && attrs.Length > 0)
                            {
                                s_companyName = ((AssemblyCompanyAttribute)attrs[0]).Company;
                            }
                        }

                        // Win32 version
                        if (s_companyName is null || s_companyName.Length == 0)
                        {
                            s_companyName = GetAppFileVersionInfo().CompanyName;
                            if (s_companyName != null)
                            {
                                s_companyName = s_companyName.Trim();
                            }
                        }

                        // fake it with a namespace
                        // won't work with MC++ see GetAppMainType.
                        if (s_companyName is null || s_companyName.Length == 0)
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
                                        s_companyName = ns.Substring(0, firstDot);
                                    }
                                    else
                                    {
                                        s_companyName = ns;
                                    }
                                }
                                else
                                {
                                    // last ditch... no namespace, use product name...
                                    s_companyName = ProductName;
                                }
                            }
                        }
                    }
                }

                return s_companyName;
            }
        }

        /// <summary>
        ///  Gets or sets the locale information for the current thread.
        /// </summary>
        public static CultureInfo CurrentCulture
        {
            get => Thread.CurrentThread.CurrentCulture;
            set => Thread.CurrentThread.CurrentCulture = value;
        }

        /// <summary>
        ///  Gets or sets the current input language for the current thread.
        /// </summary>
        public static InputLanguage CurrentInputLanguage
        {
            get => InputLanguage.CurrentInputLanguage;
            set => InputLanguage.CurrentInputLanguage = value;
        }

        internal static bool CustomThreadExceptionHandlerAttached
            => ThreadContext.FromCurrent().CustomThreadExceptionHandlerAttached;

        /// <summary>
        ///  Gets the path for the executable file that started the application.
        /// </summary>
        public static string ExecutablePath
        {
            get
            {
                if (s_executablePath is null)
                {
                    StringBuilder sb = UnsafeNativeMethods.GetModuleFileNameLongPath(NativeMethods.NullHandleRef);
                    s_executablePath = Path.GetFullPath(sb.ToString());
                }

                return s_executablePath;
            }
        }

        /// <summary>
        ///  Gets the current <see cref="HighDpiMode"/> mode for the process.
        /// </summary>
        /// <value>One of the enumeration values that indicates the high DPI mode.</value>
        public static HighDpiMode HighDpiMode
            => DpiHelper.GetWinformsApplicationDpiAwareness();

        /// <summary>
        ///  Sets the <see cref="HighDpiMode"/> mode for process.
        /// </summary>
        /// <param name="highDpiMode">One of the enumeration values that specifies the high DPI mode to set.</param>
        /// <returns><see langword="true" /> if the high DPI mode was set; otherwise, <see langword="false" />.</returns>
        public static bool SetHighDpiMode(HighDpiMode highDpiMode)
            => !DpiHelper.FirstParkingWindowCreated && DpiHelper.SetWinformsApplicationDpiAwareness(highDpiMode);

        /// <summary>
        ///  Gets the path for the application data specific to a local, non-roaming user.
        /// </summary>
        /// <remarks>
        ///  Don't obsolete these. GetDataPath isn't on SystemInformation, and it provides
        ///  the Windows logo required adornments to the directory (Company\Product\Version)
        /// </remarks>
        public static string LocalUserAppDataPath
            => GetDataPath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));

        /// <summary>
        ///  Determines if a message loop exists on this thread.
        /// </summary>
        public static bool MessageLoop
            => ThreadContext.FromCurrent().GetMessageLoop();

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
                lock (s_internalSyncObject)
                {
                    if (s_productName is null)
                    {
                        // Custom attribute
                        Assembly entryAssembly = Assembly.GetEntryAssembly();
                        if (entryAssembly != null)
                        {
                            object[] attrs = entryAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                            if (attrs != null && attrs.Length > 0)
                            {
                                s_productName = ((AssemblyProductAttribute)attrs[0]).Product;
                            }
                        }

                        // Win32 version info
                        if (s_productName is null || s_productName.Length == 0)
                        {
                            s_productName = GetAppFileVersionInfo().ProductName;
                            if (s_productName != null)
                            {
                                s_productName = s_productName.Trim();
                            }
                        }

                        // fake it with namespace
                        // won't work with MC++ see GetAppMainType.
                        if (s_productName is null || s_productName.Length == 0)
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
                                        s_productName = ns.Substring(lastDot + 1);
                                    }
                                    else
                                    {
                                        s_productName = ns;
                                    }
                                }
                                else
                                {
                                    // last ditch... use the main type
                                    s_productName = t.Name;
                                }
                            }
                        }
                    }
                }

                return s_productName;
            }
        }

        /// <summary>
        ///  Gets the product version associated with this application.
        /// </summary>
        public static string ProductVersion
        {
            get
            {
                lock (s_internalSyncObject)
                {
                    if (s_productVersion is null)
                    {
                        // Custom attribute
                        Assembly entryAssembly = Assembly.GetEntryAssembly();
                        if (entryAssembly != null)
                        {
                            object[] attrs = entryAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                            if (attrs != null && attrs.Length > 0)
                            {
                                s_productVersion = ((AssemblyInformationalVersionAttribute)attrs[0]).InformationalVersion;
                            }
                        }

                        // Win32 version info
                        if (s_productVersion is null || s_productVersion.Length == 0)
                        {
                            s_productVersion = GetAppFileVersionInfo().ProductVersion;
                            if (s_productVersion != null)
                            {
                                s_productVersion = s_productVersion.Trim();
                            }
                        }

                        // fake it
                        if (s_productVersion is null || s_productVersion.Length == 0)
                        {
                            s_productVersion = "1.0.0.0";
                        }
                    }
                }

                return s_productVersion;
            }
        }

        // Allows the hosting environment to register a callback
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void RegisterMessageLoop(MessageLoopCallback callback)
            => ThreadContext.FromCurrent().RegisterMessageLoop(callback);

        /// <summary>
        ///  Magic property that answers a simple question - are my controls currently going to render with
        ///  visual styles? If you are doing visual styles rendering, use this to be consistent with the rest
        ///  of the controls in your app.
        /// </summary>
        public static bool RenderWithVisualStyles
            => ComCtlSupportsVisualStyles && VisualStyleRenderer.IsSupported;

        /// <summary>
        ///  Gets or sets the format string to apply to top level window captions
        ///  when they are displayed with a warning banner.
        /// </summary>
        public static string SafeTopLevelCaptionFormat
        {
            get
            {
                if (s_safeTopLevelCaptionSuffix is null)
                {
                    s_safeTopLevelCaptionSuffix = SR.SafeTopLevelCaptionFormat; // 0 - original, 1 - zone, 2 - site
                }
                return s_safeTopLevelCaptionSuffix;
            }
            set
            {
                if (value is null)
                {
                    value = string.Empty;
                }

                s_safeTopLevelCaptionSuffix = value;
            }
        }

        /// <summary>
        ///  Gets the path for the executable file that started the application.
        /// </summary>
        public static string StartupPath
        {
            get
            {
                if (s_startupPath is null)
                {
                    // StringBuilder sb = UnsafeNativeMethods.GetModuleFileNameLongPath(NativeMethods.NullHandleRef);
                    // startupPath = Path.GetDirectoryName(sb.ToString());
                    s_startupPath = AppContext.BaseDirectory;
                }
                return s_startupPath;
            }
        }

        // Allows the hosting environment to unregister a callback
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void UnregisterMessageLoop()
            => ThreadContext.FromCurrent().RegisterMessageLoop(null);

        /// <summary>
        ///  Gets or sets whether the wait cursor is used for all open forms of the application.
        /// </summary>
        public static bool UseWaitCursor
        {
            get => s_useWaitCursor;
            set
            {
                lock (FormCollection.CollectionSyncRoot)
                {
                    s_useWaitCursor = value;

                    // Set the WaitCursor of all forms.
                    foreach (Form f in OpenForms)
                    {
                        f.UseWaitCursor = s_useWaitCursor;
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
            => GetDataPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));

        /// <summary>
        ///  Gets the registry key of
        ///  the application data specific to the roaming user.
        /// </summary>
        public static RegistryKey UserAppDataRegistry
            => Registry.CurrentUser.CreateSubKey($"Software\\{CompanyName}\\{ProductName}\\{ProductVersion}");

        /// <summary>
        ///  Gets a value that indicates whether visual styles are enabled for the application.
        /// </summary>
        /// <value><see langword="true" /> if visual styles are enabled; otherwise, <see langword="false" />.</value>
        /// <remarks>
        ///  The visual styles can be enabled by calling <see cref="EnableVisualStyles"/>.
        ///  The visual styles will not be enabled if the OS does not support them, or theming is disabled at the OS level.
        /// </remarks>
        public static bool UseVisualStyles { get; private set; }

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

                VisualStyleState vState = (VisualStyleState)UxTheme.GetThemeAppProperties();
                return vState;
            }
            set
            {
                if (VisualStyleInformation.IsSupportedByOS)
                {
                    UxTheme.SetThemeAppProperties((UxTheme.STAP)value);

                    // 248887 we need to send a WM_THEMECHANGED to the top level windows of this application.
                    // We do it this way to ensure that we get all top level windows -- whether we created them or not.
                    User32.EnumWindows(SendThemeChanged);
                }
            }
        }

        /// <summary>
        ///  This helper broadcasts out a WM_THEMECHANGED to appropriate top level windows of this app.
        /// </summary>
        private unsafe static BOOL SendThemeChanged(IntPtr handle)
        {
            uint thisPID = Kernel32.GetCurrentProcessId();
            User32.GetWindowThreadProcessId(handle, out uint processId);
            if (processId == thisPID && User32.IsWindowVisible(handle).IsTrue())
            {
                SendThemeChangedRecursive(handle);
                User32.RedrawWindow(
                    handle,
                    null,
                    IntPtr.Zero,
                    User32.RDW.INVALIDATE | User32.RDW.FRAME | User32.RDW.ERASE | User32.RDW.ALLCHILDREN);
            }
            return BOOL.TRUE;
        }

        /// <summary>
        ///  This helper broadcasts out a WM_THEMECHANGED this window and all children.
        ///  It is assumed at this point that the handle belongs to the current process
        ///  and has a visible top level window.
        /// </summary>
        private static BOOL SendThemeChangedRecursive(IntPtr handle)
        {
            // First send to all children...
            User32.EnumChildWindows(handle, Application.SendThemeChangedRecursive);

            // Then do myself.
            User32.SendMessageW(handle, User32.WM.THEMECHANGED);

            return BOOL.TRUE;
        }

        /// <summary>
        ///  Occurs when the application is about to shut down.
        /// </summary>
        public static event EventHandler ApplicationExit
        {
            add => AddEventHandler(s_eventApplicationExit, value);
            remove => RemoveEventHandler(s_eventApplicationExit, value);
        }

        private static void AddEventHandler(object key, Delegate value)
        {
            lock (s_internalSyncObject)
            {
                if (null == s_eventHandlers)
                {
                    s_eventHandlers = new EventHandlerList();
                }
                s_eventHandlers.AddHandler(key, value);
            }
        }

        private static void RemoveEventHandler(object key, Delegate value)
        {
            lock (s_internalSyncObject)
            {
                if (null == s_eventHandlers)
                {
                    return;
                }
                s_eventHandlers.RemoveHandler(key, value);
            }
        }

        /// <summary>
        ///  Adds a message filter to monitor Windows messages as they are routed to their
        ///  destinations.
        /// </summary>
        public static void AddMessageFilter(IMessageFilter value)
            => ThreadContext.FromCurrent().AddMessageFilter(value);

        /// <summary>
        ///  Processes all message filters for given message
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static bool FilterMessage(ref Message message)
        {
            // Create copy of MSG structure
            User32.MSG msg = message;
            bool processed = ThreadContext.FromCurrent().ProcessFilters(ref msg, out bool modified);
            if (modified)
            {
                message.HWnd = msg.hwnd;
                message.Msg = (int)msg.message;
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
                    current._idleHandler += value;

                    // This just ensures that the component manager is hooked up.  We
                    // need it for idle time processing.
                    object o = current.ComponentManager;
                }
            }
            remove
            {
                ThreadContext current = ThreadContext.FromCurrent();
                lock (current)
                {
                    current._idleHandler -= value;
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
                    current._enterModalHandler += value;
                }
            }
            remove
            {
                ThreadContext current = ThreadContext.FromCurrent();
                lock (current)
                {
                    current._enterModalHandler -= value;
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
                    current._leaveModalHandler += value;
                }
            }
            remove
            {
                ThreadContext current = ThreadContext.FromCurrent();
                lock (current)
                {
                    current._leaveModalHandler -= value;
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
                    current._threadExceptionHandler = value;
                }
            }
            remove
            {
                ThreadContext current = ThreadContext.FromCurrent();
                lock (current)
                {
                    current._threadExceptionHandler -= value;
                }
            }
        }

        /// <summary>
        ///  Occurs when a thread is about to shut down.  When the main thread for an
        ///  application is about to be shut down, this event will be raised first,
        ///  followed by an <see cref="ApplicationExit"/> event.
        /// </summary>
        public static event EventHandler ThreadExit
        {
            add => AddEventHandler(s_eventThreadExit, value);
            remove => RemoveEventHandler(s_eventThreadExit, value);
        }

        /// <summary>
        ///  Called immediately before we begin pumping messages for a modal message loop.
        ///  Does not actually start a message pump; that's the caller's responsibility.
        /// </summary>
        internal static void BeginModalMessageLoop()
            => ThreadContext.FromCurrent().BeginModalMessageLoop(null);

        /// <summary>
        ///  Processes all Windows messages currently in the message queue.
        /// </summary>
        public static void DoEvents()
            => ThreadContext.FromCurrent().RunMessageLoop(Mso.msoloop.DoEvents, null);

        internal static void DoEventsModal()
            => ThreadContext.FromCurrent().RunMessageLoop(Mso.msoloop.DoEventsModal, null);

        /// <summary>
        ///  Enables visual styles for all subsequent <see cref="Run()"/> and <see cref="Control.CreateHandle"/> calls.
        ///  Uses the default theming manifest file shipped with the redist.
        /// </summary>
        public static void EnableVisualStyles()
        {
            // Pull manifest from our resources
            string assemblyLoc = typeof(Application).Assembly.Location;
            if (assemblyLoc != null)
            {
                // CSC embeds DLL manifests as resource ID 2
                UseVisualStyles = ThemingScope.CreateActivationContext(assemblyLoc, nativeResourceManifestID: 2);
                Debug.Assert(UseVisualStyles, "Enable Visual Styles failed");

                s_comCtlSupportsVisualStylesInitialized = false;
            }
        }

        /// <summary>
        ///  Called immediately after we stop pumping messages for a modal message loop.
        ///  Does not actually end the message pump itself.
        /// </summary>
        internal static void EndModalMessageLoop()
            => ThreadContext.FromCurrent().EndModalMessageLoop(null);

        /// <summary>
        ///  Overload of <see cref="Exit(CancelEventArgs)"/> that does not care about e.Cancel.
        /// </summary>
        public static void Exit() => Exit(null);

        /// <summary>
        ///  Informs all message pumps that they are to terminate and then closes all
        ///  application windows after the messages have been processed. e.Cancel indicates
        ///  whether any of the open forms cancelled the exit call.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static void Exit(CancelEventArgs e)
        {
            lock (s_internalSyncObject)
            {
                if (s_exiting)
                {
                    // Recursive call to Exit
                    if (e != null)
                    {
                        e.Cancel = false;
                    }
                    return;
                }
                s_exiting = true;

                try
                {
                    // Raise the FormClosing and FormClosed events for each open form
                    if (s_forms != null)
                    {
                        foreach (Form f in s_forms)
                        {
                            if (f.RaiseFormClosingOnAppExit())
                            {
                                // A form refused to close
                                if (e != null)
                                {
                                    e.Cancel = true;
                                }
                                return;
                            }
                        }

                        while (s_forms.Count > 0)
                        {
                            // OnFormClosed removes the form from the FormCollection
                            s_forms[0].RaiseFormClosedOnAppExit();
                        }
                    }

                    ThreadContext.ExitApplication();
                    if (e != null)
                    {
                        e.Cancel = false;
                    }
                }
                finally
                {
                    s_exiting = false;
                }
            }
        }

        /// <summary>
        ///  Exits the message loop on the current thread and closes all windows on the thread.
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
            lock (s_internalSyncObject)
            {
                if (s_appFileVersion is null)
                {
                    Type t = GetAppMainType();
                    if (t != null)
                    {
                        s_appFileVersion = FileVersionInfo.GetVersionInfo(t.Module.FullyQualifiedName);
                    }
                    else
                    {
                        s_appFileVersion = FileVersionInfo.GetVersionInfo(ExecutablePath);
                    }
                }
            }

            return (FileVersionInfo)s_appFileVersion;
        }

        /// <summary>
        ///  Retrieves the Type that contains the "Main" method.
        /// </summary>
        private static Type GetAppMainType()
        {
            lock (s_internalSyncObject)
            {
                if (s_mainType is null)
                {
                    Assembly exe = Assembly.GetEntryAssembly();

                    // Get Main type...This doesn't work in MC++ because Main is a global function and not
                    // a class static method (it doesn't belong to a Type).
                    if (exe != null)
                    {
                        s_mainType = exe.EntryPoint.ReflectedType;
                    }
                }
            }

            return s_mainType;
        }

        /// <summary>
        ///  Locates a thread context given a window handle.
        /// </summary>
        private static ThreadContext GetContextForHandle(HandleRef handle)
        {
            ThreadContext cxt = ThreadContext.FromId(User32.GetWindowThreadProcessId(handle.Handle, out _));
            Debug.Assert(
                cxt != null,
                "No thread context for handle.  This is expected if you saw a previous assert about the handle being invalid.");

            GC.KeepAlive(handle.Wrapper);
            return cxt;
        }

        /// <summary>
        ///  Returns a string that is the combination of the basePath + CompanyName + ProducName + ProductVersion. This
        ///  will also create the directory if it doesn't exist.
        /// </summary>
        private static string GetDataPath(string basePath)
        {
            string path = Path.Join(basePath, CompanyName, ProductName, ProductVersion);

            lock (s_internalSyncObject)
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
            if (s_eventHandlers != null)
            {
                Delegate exit = s_eventHandlers[s_eventApplicationExit];
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
            if (s_eventHandlers != null)
            {
                Delegate exit = s_eventHandlers[s_eventThreadExit];
                if (exit != null)
                {
                    ((EventHandler)exit)(null, EventArgs.Empty);
                }
            }
        }

        internal static void ParkHandle(HandleRef handle) => ParkHandle(handle, User32.UNSPECIFIED_DPI_AWARENESS_CONTEXT);

        /// <summary>
        ///  "Parks" the given HWND to a temporary HWND.  This allows WS_CHILD windows to
        ///  be parked.
        /// </summary>
        internal static void ParkHandle(HandleRef handle, IntPtr dpiAwarenessContext)
        {
            Debug.Assert(User32.IsWindow(handle).IsTrue(), "Handle being parked is not a valid window handle");
            Debug.Assert(((int)User32.GetWindowLong(handle, User32.GWL.STYLE) & (int)User32.WS.CHILD) != 0, "Only WS_CHILD windows should be parked.");

            ThreadContext cxt = GetContextForHandle(handle);
            if (cxt != null)
            {
                cxt.GetParkingWindow(dpiAwarenessContext).ParkHandle(handle);
            }
        }

        internal static void ParkHandle(CreateParams cp) => ParkHandle(cp, User32.UNSPECIFIED_DPI_AWARENESS_CONTEXT);

        /// <summary>
        ///  Park control handle on a parkingwindow that has matching DpiAwareness.
        /// </summary>
        /// <param name="cp"> create params for control handle</param>
        /// <param name="dpiAwarenessContext"> dpi awareness</param>
        internal static void ParkHandle(CreateParams cp, IntPtr dpiAwarenessContext)
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
            => ThreadContext.FromCurrent().OleRequired();

        /// <summary>
        ///  Raises the <see cref='ThreadException'/> event.
        /// </summary>
        public static void OnThreadException(Exception t)
            => ThreadContext.FromCurrent().OnThreadException(t);

        /// <summary>
        ///  "Unparks" the given HWND to a temporary HWND.  This allows WS_CHILD windows to
        ///  be parked.
        /// </summary>
        internal static void UnparkHandle(HandleRef handle, IntPtr context)
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
            => ThreadContext.FromCurrent()._idleHandler?.Invoke(Thread.CurrentThread, e);

        /// <summary>
        ///  Removes a message filter from the application's message pump.
        /// </summary>
        public static void RemoveMessageFilter(IMessageFilter value)
            => ThreadContext.FromCurrent().RemoveMessageFilter(value);

        /// <summary>
        ///  Restarts the application.
        /// </summary>
        public static void Restart()
        {
            if (Assembly.GetEntryAssembly() is null)
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
                    Exit();
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
                ProcessStartInfo currentStartInfo = new ProcessStartInfo();
                currentStartInfo.FileName = ExecutablePath;
                if (sb.Length > 0)
                {
                    currentStartInfo.Arguments = sb.ToString();
                }
                Exit();
                Process.Start(currentStartInfo);
            }
        }

        /// <summary>
        ///  Begins running a standard application message loop on the current thread,
        ///  without a form.
        /// </summary>
        public static void Run()
            => ThreadContext.FromCurrent().RunMessageLoop(Interop.Mso.msoloop.Main, new ApplicationContext());

        /// <summary>
        ///  Begins running a standard application message loop on the current
        ///  thread, and makes the specified form visible.
        /// </summary>
        public static void Run(Form mainForm)
            => ThreadContext.FromCurrent().RunMessageLoop(Interop.Mso.msoloop.Main, new ApplicationContext(mainForm));

        /// <summary>
        ///  Begins running a standard application message loop on the current thread,
        ///  without a form.
        /// </summary>
        public static void Run(ApplicationContext context)
            => ThreadContext.FromCurrent().RunMessageLoop(Interop.Mso.msoloop.Main, context);

        /// <summary>
        ///  Runs a modal dialog.  This starts a special type of message loop that runs until
        ///  the dialog has a valid DialogResult.  This is called internally by a form
        ///  when an application calls System.Windows.Forms.Form.ShowDialog().
        /// </summary>
        internal static void RunDialog(Form form)
            => ThreadContext.FromCurrent().RunMessageLoop(Interop.Mso.msoloop.ModalForm, new ModalApplicationContext(form));

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
            => Powrprof.SetSuspendState((state == PowerState.Hibernate).ToBOOLEAN(), force.ToBOOLEAN(), disableWakeEvent.ToBOOLEAN()).IsTrue();

        /// <summary>
        ///  Overload version of SetUnhandledExceptionMode that sets the UnhandledExceptionMode
        ///  mode at the current thread level.
        /// </summary>
        public static void SetUnhandledExceptionMode(UnhandledExceptionMode mode)
            => SetUnhandledExceptionMode(mode, true /*threadScope*/);

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
            => NativeWindow.SetUnhandledExceptionModeInternal(mode, threadScope);
    }
}

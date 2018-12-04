// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Text;
    using System.Threading;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Runtime.ConstrainedExecution;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.IO;
    using Microsoft.Win32;
    using System.Security;
    using System.Security.Permissions;
    using System.Collections;
    using System.Globalization;
    using System.Runtime.Versioning;

    using System.Reflection;
    using System.ComponentModel;
    using System.Windows.Forms.Layout;
    using System.Windows.Forms.VisualStyles;
    using Directory = System.IO.Directory;

#if CLICKONCE
    using System.Deployment.Internal.Isolation;
#endif
    using System.Collections.Generic;

    /// <include file='doc\Application.uex' path='docs/doc[@for="Application"]/*' />
    /// <devdoc>
    /// <para>Provides <see langword='static '/>
    /// methods and properties
    /// to manage an application, such as methods to run and quit an application,
    /// to process Windows messages, and properties to get information about an application. This
    /// class cannot be inherited.</para>
    /// </devdoc>
    public sealed class Application {
        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.eventHandlers"]/*' />
        /// <devdoc>
        ///     Hash table for our event list
        /// </devdoc>
        static EventHandlerList eventHandlers;
        static string startupPath;
        static string executablePath;
        static object appFileVersion;
        static Type mainType;
        static string companyName;
        static string productName;
        static string productVersion;
        static string safeTopLevelCaptionSuffix;
        static bool useVisualStyles = false;
        static bool comCtlSupportsVisualStylesInitialized = false;
        static bool comCtlSupportsVisualStyles = false;
        static FormCollection forms = null;
        private static object internalSyncObject = new object();
        static bool useWaitCursor = false;

        private static bool useEverettThreadAffinity = false;
        private static bool checkedThreadAffinity = false;
        private const string everettThreadAffinityValue = "EnableSystemEventsThreadAffinityCompatibility";

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.exiting"]/*' />
        /// <devdoc>
        ///     in case Application.exit gets called recursively
        /// </devdoc>
        private static bool exiting;

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.EVENT_APPLICATIONEXIT"]/*' />
        /// <devdoc>
        ///     Events the user can hook into
        /// </devdoc>
        private static readonly object EVENT_APPLICATIONEXIT = new object();
        private static readonly object EVENT_THREADEXIT      = new object();

    
        // Constant string used in Application.Restart()
        private const string IEEXEC = "ieexec.exe";

        // Constant string used for accessing ClickOnce app's data directory
        private const string CLICKONCE_APPS_DATADIRECTORY = "DataDirectory";

        // Defines a new callback delegate type
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public delegate bool MessageLoopCallback();

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.Application"]/*' />
        /// <devdoc>
        ///     This class is static, there is no need to ever create it.
        /// </devdoc>
        private Application() {
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.AllowQuit"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Determines if the caller should be allowed to quit the application.  This will return false,
        ///      for example, if being called from a windows forms control being hosted within a web browser.  The
        ///      windows forms control should not attempt to quit the application.
        ///
        ///    </para>
        /// </devdoc>
        public static bool AllowQuit {
            get {
                return ThreadContext.FromCurrent().GetAllowQuit();
            }
        }

        /// <devdoc>
        ///    <para>Returns True if it is OK to continue idle processing. Typically called in an Application.Idle event handler.</para>
        /// </devdoc>
        internal static bool CanContinueIdle
        {
            get
            {
                return ThreadContext.FromCurrent().ComponentManager.FContinueIdle();
            }
        }

        /// Typically, you shouldn't need to use this directly - use RenderWithVisualStyles instead.
        internal static bool ComCtlSupportsVisualStyles {
            get {
                if (!comCtlSupportsVisualStylesInitialized) {
                    comCtlSupportsVisualStyles = InitializeComCtlSupportsVisualStyles();
                    comCtlSupportsVisualStylesInitialized = true;
                }
                return comCtlSupportsVisualStyles;
            }
        }

        private static bool InitializeComCtlSupportsVisualStyles() {
            if (useVisualStyles && OSFeature.Feature.IsPresent(OSFeature.Themes)) {
                //NOTE: At this point, we may not have loaded ComCtl6 yet, but it will eventually
                //      be loaded, so we return true here. This works because UseVisualStyles, once
                //      set, cannot be turned off. If that changes (unlikely), this may not work.
                return true;
            }

            //to see if we are comctl6, we look for a function that is exposed only from comctl6
            // we do not call DllGetVersion or any direct p/invoke, because the binding will be
            // cached.
            // The GetModuleHandle function returns a handle to a mapped module without incrementing its reference count.

            IntPtr hModule = UnsafeNativeMethods.GetModuleHandle(ExternDll.Comctl32);
            if (hModule != IntPtr.Zero) {
                try {
                    IntPtr pFunc = UnsafeNativeMethods.GetProcAddress(new HandleRef(null, hModule), "ImageList_WriteEx");
                    return (pFunc != IntPtr.Zero);
                } catch {
                }
            } else {
                // Load comctl since GetModuleHandle failed to find it
                hModule = UnsafeNativeMethods.LoadLibraryFromSystemPathIfAvailable(ExternDll.Comctl32);
                if (hModule != IntPtr.Zero) {
                    IntPtr pFunc = UnsafeNativeMethods.GetProcAddress(new HandleRef(null, hModule), "ImageList_WriteEx");
                    return (pFunc != IntPtr.Zero);
                }
            }
            return false;
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.CommonAppDataRegistry"]/*' />
        /// <devdoc>
        ///    <para>Gets the registry
        ///       key for the application data that is shared among all users.</para>
        /// </devdoc>
        public static RegistryKey CommonAppDataRegistry {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                return Registry.LocalMachine.CreateSubKey(CommonAppDataRegistryKeyName);
            }
        }

        internal static string CommonAppDataRegistryKeyName {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                string template = @"Software\{0}\{1}\{2}";
                return string.Format(CultureInfo.CurrentCulture, template,
                                                                      CompanyName,
                                                                      ProductName,
                                                                      ProductVersion);
            }
        }

        internal static bool UseEverettThreadAffinity {
            get {
                if (!checkedThreadAffinity) {
                    checkedThreadAffinity = true;
                    try {
                        //We need access to be able to read from the registry here.  We're not creating a 
                        //registry key, nor are we returning information from the registry to the user.
                        new RegistryPermission(PermissionState.Unrestricted).Assert();
                        RegistryKey key = Registry.LocalMachine.OpenSubKey(CommonAppDataRegistryKeyName);
                        if (key != null) {
                            object value = key.GetValue(everettThreadAffinityValue);
                            key.Close();

                            if (value != null && (int)value != 0) {
                                useEverettThreadAffinity = true;
                            }
                        }
                    }
                    catch (SecurityException) {
                        // Can't read the key: use default value (false)
                    }
                    catch (InvalidCastException) {
                        // Key is of wrong type: use default value (false)
                    }
                }
                return useEverettThreadAffinity;
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.CommonAppDataPath"]/*' />
        /// <devdoc>
        ///    <para>Gets the path for the application data that is shared among all users.</para>
        /// </devdoc>
        public static string CommonAppDataPath {
            // NOTE   : Don't obsolete these. GetDataPath isn't on SystemInformation, and it
            //        : provides the Win2K logo required adornments to the directory (Company\Product\Version)
            //
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.AppDomain | ResourceScope.Machine)]
            get {
#if CLICKONCE
                try {
                    if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed) {
                        string data = AppDomain.CurrentDomain.GetData(CLICKONCE_APPS_DATADIRECTORY) as string;
                        if (data != null) {
                            return data;
                        }
                    }
                }
                catch (Exception ex) {
                    if (ClientUtils.IsSecurityOrCriticalException(ex)) {
                        throw;
                    }
                }
#endif
                return GetDataPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.CompanyName"]/*' />
        /// <devdoc>
        ///    <para>Gets the company name associated with the application.</para>
        /// </devdoc>
        public static string CompanyName {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                lock(internalSyncObject) {
                    if (companyName == null) {

                        // custom attribute
                        //
                        Assembly entryAssembly = Assembly.GetEntryAssembly();
                        if (entryAssembly != null) {
                            object[] attrs = entryAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                            if (attrs != null && attrs.Length > 0) {
                                companyName = ((AssemblyCompanyAttribute)attrs[0]).Company;
                            }
                        }

                        // win32 version
                        //
                        if (companyName == null || companyName.Length == 0) {
                            companyName = GetAppFileVersionInfo().CompanyName;
                            if (companyName != null) {
                                companyName = companyName.Trim();
                            }
                        }

                        // fake it with a namespace
                        // won't work with MC++ see GetAppMainType.
                        if (companyName == null || companyName.Length == 0) {
                            Type t = GetAppMainType();

                            if (t != null) {
                                string ns = t.Namespace;

                                if (!string.IsNullOrEmpty(ns)){
                                    int firstDot = ns.IndexOf(".");
                                    if( firstDot != -1 ){
                                        companyName = ns.Substring(0, firstDot);
                                    }
                                    else{
                                        companyName = ns;
                                    }
                                }
                                else {
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

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.CurrentCulture"]/*' />
        /// <devdoc>
        ///    <para>Gets
        ///       or sets the locale information for the current thread.</para>
        /// </devdoc>
        public static CultureInfo CurrentCulture {
            get {
                return Thread.CurrentThread.CurrentCulture;
            }
            set {
                Thread.CurrentThread.CurrentCulture = value;
            }
        }


        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.CurrentInputLanguage"]/*' />
        /// <devdoc>
        ///    <para>Gets or
        ///       sets the current input language for the current thread.</para>
        /// </devdoc>
        public static InputLanguage CurrentInputLanguage {
            get {
                return InputLanguage.CurrentInputLanguage;
            }
            set {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "AffectThreadBehavior Demanded");
                IntSecurity.AffectThreadBehavior.Demand();
                InputLanguage.CurrentInputLanguage = value;
            }
        }

        internal static bool CustomThreadExceptionHandlerAttached {
            get {
                return ThreadContext.FromCurrent().CustomThreadExceptionHandlerAttached;
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ExecutablePath"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the
        ///       path for the executable file that started the application.
        ///    </para>
        /// </devdoc>
        /// 


        [SuppressMessage("Microsoft.Security", "CA2103:ReviewImperativeSecurity")]
        public static string ExecutablePath {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                if (executablePath == null) {
                    Assembly asm = Assembly.GetEntryAssembly();
                    if (asm == null) {
                        StringBuilder sb = UnsafeNativeMethods.GetModuleFileNameLongPath(NativeMethods.NullHandleRef);
                        executablePath = IntSecurity.UnsafeGetFullPath(sb.ToString());
                    }
                    else {
                        String cb = asm.CodeBase;
                        Uri codeBase = new Uri(cb);
                        if (codeBase.IsFile) {
                            executablePath = codeBase.LocalPath + Uri.UnescapeDataString(codeBase.Fragment); ;
                        }
                        else {
                            executablePath = codeBase.ToString();
                        }
                    }
                }
                Uri exeUri = new Uri(executablePath);
                if (exeUri.Scheme == "file") {
                    Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileIO(" + executablePath + ") Demanded");
                    new FileIOPermission(FileIOPermissionAccess.PathDiscovery, executablePath).Demand();
                }
                return executablePath;
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.LocalUserAppDataPath"]/*' />
        /// <devdoc>
        ///    <para>Gets the path for the application data specific to a local, non-roaming user.</para>
        /// </devdoc>
        public static string LocalUserAppDataPath {
            // NOTE   : Don't obsolete these. GetDataPath isn't on SystemInformation, and it
            //        : provides the Win2K logo required adornments to the directory (Company\Product\Version)
            //
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine | ResourceScope.AppDomain)]
            get {
#if CLICKONCE
                try {
                    if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed) {
                        string data = AppDomain.CurrentDomain.GetData(CLICKONCE_APPS_DATADIRECTORY) as string;
                        if (data != null) {
                            return data;
                        }
                    }
                }
                catch (Exception ex) {
                    if (ClientUtils.IsSecurityOrCriticalException(ex)) {
                        throw;
                    }
                }
#endif
                return GetDataPath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.MessageLoop"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Determines if a message loop exists on this thread.
        ///    </para>
        /// </devdoc>
        public static bool MessageLoop {
            get {
                return ThreadContext.FromCurrent().GetMessageLoop();
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.Forms"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the forms collection associated with this application.
        ///    </para>
        /// </devdoc>
        public static FormCollection OpenForms {
            [UIPermission(SecurityAction.Demand, Window=UIPermissionWindow.AllWindows)]
            get {
                return OpenFormsInternal;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Internal version of OpenForms without the security demand.
        ///    </para>
        /// </devdoc>
        internal static FormCollection OpenFormsInternal {
            get {
                if (forms == null) {
                    forms = new FormCollection();
                }

                return forms;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Thread safe addition of form to the OpenForms collection.
        ///    </para>
        /// </devdoc>
        internal static void OpenFormsInternalAdd(Form form)
        {
            OpenFormsInternal.Add(form);            
        }

        /// <devdoc>
        ///    <para>
        ///       Thread safe removal of form from the OpenForms collection.
        ///    </para>
        /// </devdoc>
        internal static void OpenFormsInternalRemove(Form form)
        {
            OpenFormsInternal.Remove(form);
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ProductName"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       the product name associated with this application.
        ///    </para>
        /// </devdoc>
        public static string ProductName {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                lock(internalSyncObject) {
                    if (productName == null) {

                        // custom attribute
                        //
                        Assembly entryAssembly = Assembly.GetEntryAssembly();
                        if (entryAssembly != null) {
                            object[] attrs = entryAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                            if (attrs != null && attrs.Length > 0) {
                                productName = ((AssemblyProductAttribute)attrs[0]).Product;
                            }
                        }

                        // win32 version info
                        //
                        if (productName == null || productName.Length == 0) {
                            productName = GetAppFileVersionInfo().ProductName;
                            if (productName != null) {
                                productName = productName.Trim();
                            }
                        }

                        // fake it with namespace
                        // won't work with MC++ see GetAppMainType.
                        if (productName == null || productName.Length == 0) {
                            Type t = GetAppMainType();

                            if (t != null) {
                                string ns = t.Namespace;

                                if (!string.IsNullOrEmpty(ns)) {
                                    int lastDot = ns.LastIndexOf(".");
                                    if (lastDot != -1 && lastDot < ns.Length - 1) {
                                        productName = ns.Substring(lastDot+1);
                                    }
                                    else {
                                        productName = ns;
                                    }
                                }
                                else{
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

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ProductVersion"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       the product version associated with this application.
        ///    </para>
        /// </devdoc>
        public static string ProductVersion {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                lock(internalSyncObject) {
                    if (productVersion == null) {

                        // custom attribute
                        //
                        Assembly entryAssembly = Assembly.GetEntryAssembly();
                        if (entryAssembly != null) {
                            object[] attrs = entryAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                            if (attrs != null && attrs.Length > 0) {
                                productVersion = ((AssemblyInformationalVersionAttribute)attrs[0]).InformationalVersion;
                            }
                        }

                        // win32 version info
                        //
                        if (productVersion == null || productVersion.Length == 0) {
                            productVersion = GetAppFileVersionInfo().ProductVersion;
                            if (productVersion != null) {
                                productVersion = productVersion.Trim();
                            }
                        }

                        // fake it
                        //
                        if (productVersion == null || productVersion.Length == 0) {
                            productVersion = "1.0.0.0";
                        }
                    }
                }
                return productVersion;
            }
        }

        // Allows the hosting environment to register a callback 
        [
            EditorBrowsable(EditorBrowsableState.Advanced),
            SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)
        ]
        public static void RegisterMessageLoop(MessageLoopCallback callback) {
            ThreadContext.FromCurrent().RegisterMessageLoop(callback);
        }
        
        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.RenderWithVisualStyles"]/*' />
        /// <devdoc>
        ///    Magic property that answers a simple question - are my controls currently going to render with
        //     visual styles? If you are doing visual styles rendering, use this to be consistent with the rest
        //     of the controls in your app.
        /// </devdoc>
        public static bool RenderWithVisualStyles {
            get {
                return (ComCtlSupportsVisualStyles && VisualStyles.VisualStyleRenderer.IsSupported);
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.SafeTopLevelCaptionFormat"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the format string to apply to top level window captions
        ///       when they are displayed with a warning banner.</para>
        /// </devdoc>
        public static string SafeTopLevelCaptionFormat {
            get {
                if (safeTopLevelCaptionSuffix == null) {
                    safeTopLevelCaptionSuffix = SR.SafeTopLevelCaptionFormat; // 0 - original, 1 - zone, 2 - site
                }
                return safeTopLevelCaptionSuffix;
            }
            set {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "WindowAdornmentModification Demanded");
                IntSecurity.WindowAdornmentModification.Demand();
                if (value == null) value = string.Empty;
                safeTopLevelCaptionSuffix = value;
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.StartupPath"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the
        ///       path for the executable file that started the application.
        ///    </para>
        /// </devdoc>

        /// 


        [SuppressMessage("Microsoft.Security", "CA2103:ReviewImperativeSecurity")]
        public static string StartupPath {
            get {
                if (startupPath == null) {
                    // StringBuilder sb = UnsafeNativeMethods.GetModuleFileNameLongPath(NativeMethods.NullHandleRef);
                    // startupPath = Path.GetDirectoryName(sb.ToString());
                    startupPath = AppContext.BaseDirectory;
                }
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "FileIO(" + startupPath + ") Demanded");
                new FileIOPermission(FileIOPermissionAccess.PathDiscovery, startupPath).Demand();
                return startupPath;
            }
        }

        // Allows the hosting environment to unregister a callback 
        [
            EditorBrowsable(EditorBrowsableState.Advanced),
            SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)
        ]
        public static void UnregisterMessageLoop() {
            ThreadContext.FromCurrent().RegisterMessageLoop(null);
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.UseWaitCursor"]/*' />
        /// <devdoc>
        ///    <para>Gets
        ///       or sets whether the wait cursor is used for all open forms of the application.</para>
        /// </devdoc>
        public static bool UseWaitCursor {
            get {
                return useWaitCursor;
            }
            set {
                lock (FormCollection.CollectionSyncRoot) 
                {
                    useWaitCursor = value;
                    // Set the WaitCursor of all forms.
                    foreach (Form f in OpenFormsInternal)
                    {
                        f.UseWaitCursor = useWaitCursor;
                    }
                }
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.UserAppDataPath"]/*' />
        /// <devdoc>
        ///    <para>Gets the path for the application data specific to the roaming user.</para>
        /// </devdoc>
        public static string UserAppDataPath {
            // NOTE   : Don't obsolete these. GetDataPath isn't on SystemInformation, and it
            //        : provides the Win2K logo required adornments to the directory (Company\Product\Version)
            //
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine | ResourceScope.AppDomain)]
            get {
#if CLICKONCE
                try {
                    if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed) {
                        string data = AppDomain.CurrentDomain.GetData(CLICKONCE_APPS_DATADIRECTORY) as string;
                        if (data != null) {
                            return data;
                        }
                    }
                }
                catch (Exception ex) {
                    if (ClientUtils.IsSecurityOrCriticalException(ex)) {
                        throw;
                    }
                }
#endif
                return GetDataPath(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.UserAppDataRegistry"]/*' />
        /// <devdoc>
        ///    <para>Gets the registry key of
        ///       the application data specific to the roaming user.</para>
        /// </devdoc>
        public static RegistryKey UserAppDataRegistry {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine | ResourceScope.AppDomain)]
            get {
                string template = @"Software\{0}\{1}\{2}";
                return Registry.CurrentUser.CreateSubKey(string.Format(CultureInfo.CurrentCulture, template, CompanyName, ProductName, ProductVersion));
            }
        }

        internal static bool UseVisualStyles {
            get {
                return useVisualStyles;
            }
        }

        internal static string WindowsFormsVersion {
            get {
                // Notice   : Don't never ever change this name, since window class of Winforms control is dependent on this.
                //            And lots of partner team are related to window class of Winforms control. Changing this will introduce breaking.
                //            If there is some reason need to change this, should take the accountability to notify partner team.
                return "WindowsForms10";
            }
        }

        internal static string WindowMessagesVersion {
            get {
                return "WindowsForms12";
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.VisualStyleState"]/*' />
        /// <devdoc>
        ///    <para>
        ///     Use this property to determine how visual styles will be applied to this application.
        ///     This property is meaningful only if visual styles are supported on the current
        ///     platform (VisualStyleInformation.SupportedByOS is true).
        ///
        ///     This property can be set only to one of the S.W.F.VisualStyles.VisualStyleState enum values.
        ///    </para>
        /// </devdoc>
        public static VisualStyleState VisualStyleState {
            get {
                if (!VisualStyleInformation.IsSupportedByOS) {
                    return VisualStyleState.NoneEnabled;
                }

                VisualStyleState vState = (VisualStyleState) SafeNativeMethods.GetThemeAppProperties();
                return vState;
            }

            set {
                if (VisualStyleInformation.IsSupportedByOS)
				{
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)VisualStyleState.NoneEnabled, (int)VisualStyleState.ClientAndNonClientAreasEnabled)
                        && LocalAppContextSwitches.EnableVisualStyleValidation) {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(VisualStyleState));
                    }

                    SafeNativeMethods.SetThemeAppProperties((int)value);

                    //248887 we need to send a WM_THEMECHANGED to the top level windows of this application.
                    //We do it this way to ensure that we get all top level windows -- whether we created them or not.
                    SafeNativeMethods.EnumThreadWindowsCallback callback = new SafeNativeMethods.EnumThreadWindowsCallback(Application.SendThemeChanged);
                    SafeNativeMethods.EnumWindows(callback, IntPtr.Zero);

                    GC.KeepAlive(callback);

                }
            }
        }

        /// <devdoc>
        /// This helper broadcasts out a WM_THEMECHANGED to appropriate top level windows of this app.
        /// </devdoc>
        private static bool SendThemeChanged(IntPtr handle, IntPtr extraParameter) {
            int processId;
            int thisPID = SafeNativeMethods.GetCurrentProcessId();
            SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(null, handle), out processId);
            if (processId == thisPID && SafeNativeMethods.IsWindowVisible(new HandleRef(null, handle))) {

                SendThemeChangedRecursive(handle, IntPtr.Zero);
                SafeNativeMethods.RedrawWindow(new HandleRef(null, handle),
                                               null, NativeMethods.NullHandleRef,
                                               NativeMethods.RDW_INVALIDATE |
                                               NativeMethods.RDW_FRAME |
                                               NativeMethods.RDW_ERASE |
                                               NativeMethods.RDW_ALLCHILDREN);
            }
            return true;
        }

        /// <devdoc>
        /// This helper broadcasts out a WM_THEMECHANGED this window and all children.
        /// it is assumed at this point that the handle belongs to the current process and has a visible top level window.
        /// </devdoc>
        private static bool SendThemeChangedRecursive(IntPtr handle, IntPtr lparam) {
            //first send to all children...
            UnsafeNativeMethods.EnumChildWindows(new HandleRef(null, handle),
                new NativeMethods.EnumChildrenCallback(Application.SendThemeChangedRecursive),
                NativeMethods.NullHandleRef);

            //then do myself.
            UnsafeNativeMethods.SendMessage(new HandleRef(null, handle), NativeMethods.WM_THEMECHANGED, 0, 0);

            return true;
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ApplicationExit"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the application is about to shut down.</para>
        /// </devdoc>
        public static event EventHandler ApplicationExit {
            add {
                AddEventHandler(EVENT_APPLICATIONEXIT, value);
            }
            remove {
                RemoveEventHandler(EVENT_APPLICATIONEXIT, value);
            }
        }

        private static void AddEventHandler(object key, Delegate value) {
            lock(internalSyncObject) {
                if (null == eventHandlers) {
                    eventHandlers = new EventHandlerList();
                }
                eventHandlers.AddHandler(key, value);
            }
        }
        private static void RemoveEventHandler(object key, Delegate value) {
            lock(internalSyncObject) {
                if (null == eventHandlers) {
                    return;
                }
                eventHandlers.RemoveHandler(key, value);
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.AddMessageFilter"]/*' />
        /// <devdoc>
        ///    <para>Adds a message filter to monitor Windows messages as they are routed to their
        ///       destinations.</para>
        /// </devdoc>
        public static void AddMessageFilter(IMessageFilter value) {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ManipulateWndProcAndHandles Demanded");
            IntSecurity.UnmanagedCode.Demand();
            ThreadContext.FromCurrent().AddMessageFilter(value);
        }

        /// <devdoc>
        ///  Processes all message filters for given message
        /// </devdoc>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
         EditorBrowsable(EditorBrowsableState.Advanced),         
         SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")  // using ref is OK.
        ]
        public static bool FilterMessage(ref Message message) {            

            bool modified;

            // Create copy of MSG structure
            NativeMethods.MSG msg = new NativeMethods.MSG();           
            msg.hwnd = message.HWnd;
            msg.message = message.Msg;
            msg.wParam = message.WParam;
            msg.lParam = message.LParam;

            bool processed = ThreadContext.FromCurrent().ProcessFilters(ref msg, out modified);
            if (modified) {
                message.HWnd = msg.hwnd;
                message.Msg = msg.message;
                message.WParam = msg.wParam;
                message.LParam = msg.lParam;
            }
            
            return processed;
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.Idle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs when the application has finished processing and is about to enter the
        ///       idle state.
        ///    </para>
        /// </devdoc>
        public static event EventHandler Idle {
            add {
                ThreadContext current = ThreadContext.FromCurrent();
                lock(current) {                    
                    current.idleHandler += value;
                    // This just ensures that the component manager is hooked up.  We
                    // need it for idle time processing.
                    //
                    object o = current.ComponentManager;
                }
            }
            remove {
                ThreadContext current = ThreadContext.FromCurrent();
                lock(current) {                    
                    current.idleHandler -= value;
                }
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.Idle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs when the application is about to enter a modal state
        ///    </para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static event EventHandler EnterThreadModal {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            add {
                ThreadContext current = ThreadContext.FromCurrent();
                lock(current) {                    
                    current.enterModalHandler += value;
                }
            }        
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            remove {
                ThreadContext current = ThreadContext.FromCurrent();
                lock(current) {                    
                    current.enterModalHandler -= value;
                }
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.Idle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs when the application is about to leave a modal state
        ///    </para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static event EventHandler LeaveThreadModal {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            add {
                ThreadContext current = ThreadContext.FromCurrent();
                lock(current) {                    
                    current.leaveModalHandler += value;
                }
            }
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            remove {
                ThreadContext current = ThreadContext.FromCurrent();
                lock(current) {                    
                    current.leaveModalHandler -= value;
                }
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadException"]/*' />
        /// <devdoc>
        ///    <para>Occurs when an untrapped thread exception is thrown.</para>
        /// </devdoc>
        public static event ThreadExceptionEventHandler ThreadException {
            add {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "AffectThreadBehavior Demanded");
                IntSecurity.AffectThreadBehavior.Demand();

                ThreadContext current = ThreadContext.FromCurrent();
                lock(current) {                    
                    current.threadExceptionHandler = value;
                }
            }
            remove {
                ThreadContext current = ThreadContext.FromCurrent();
                lock(current) {
                    current.threadExceptionHandler -= value;
                }
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadExit"]/*' />
        /// <devdoc>
        ///    <para>Occurs when a thread is about to shut down.  When
        ///     the main thread for an application is about to be shut down,
        ///     this event will be raised first, followed by an ApplicationExit
        ///     event.</para>
        /// </devdoc>
        public static event EventHandler ThreadExit {
            add {
                AddEventHandler(EVENT_THREADEXIT, value);
            }
            remove {
                RemoveEventHandler(EVENT_THREADEXIT, value);
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.BeginModalMessageLoop"]/*' />
        /// <devdoc>
        ///     Called immediately before we begin pumping messages for a modal message loop.
        ///     Does not actually start a message pump; that's the caller's responsibility.
        /// </devdoc>
        /// <internalonly/>
        internal static void BeginModalMessageLoop() {
            ThreadContext.FromCurrent().BeginModalMessageLoop(null);
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.DoEvents"]/*' />
        /// <devdoc>
        ///    <para>Processes
        ///       all Windows messages currently in the message queue.</para>
        /// </devdoc>
        public static void DoEvents() {
            ThreadContext.FromCurrent().RunMessageLoop(NativeMethods.MSOCM.msoloopDoEvents, null);
        }

        /// <devdoc>
        /// </devdoc>
        internal static void DoEventsModal() {
            ThreadContext.FromCurrent().RunMessageLoop(NativeMethods.MSOCM.msoloopDoEventsModal, null);
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.EnableVisualStyles"]/*' />
        /// <devdoc>
        ///    <para>
        ///    Enables visual styles for all subsequent Application.Run() and CreateHandle() calls.
        ///    Uses the default theming manifest file shipped with the redist.
        ///    </para>
        /// </devdoc>
        public static void EnableVisualStyles() {
            string assemblyLoc = null;
            
            FileIOPermission fiop = new FileIOPermission(PermissionState.None);
            fiop.AllFiles = FileIOPermissionAccess.PathDiscovery;
            fiop.Assert();
            try {
                assemblyLoc = typeof(Application).Assembly.Location;
            }
            finally {
                CodeAccessPermission.RevertAssert();
            }
            // Pull manifest from our resources
            if (assemblyLoc != null) {
                // CSC embeds DLL manifests as resource ID 2
                // https://github.com/dotnet/roslyn/blob/fab7134296816fc80019c60b0f5bef7400cf23ea/src/Compilers/Core/Portable/CvtRes.cs#L562
                EnableVisualStylesInternal(assemblyLoc, 2);
            }
        }

        /// <devdoc>
        ///    Internal version ***WITHOUT SECURITY DEMAND***.
        /// </devdoc>
        private static void EnableVisualStylesInternal(string assemblyFileName, int nativeResourceID) {
            //Note that if the following call fails, we don't throw an exception.
            //Theming scope won't work, thats all.
            useVisualStyles = UnsafeNativeMethods.ThemingScope.CreateActivationContext(assemblyFileName, nativeResourceID);

            Debug.Assert(useVisualStyles, "Enable Visual Styles failed");
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.EndModalMessageLoop"]/*' />
        /// <devdoc>
        ///     Called immediately after we stop pumping messages for a modal message loop.
        ///     Does not actually end the message pump itself.
        /// </devdoc>
        /// <internalonly/>
        internal static void EndModalMessageLoop() {
            ThreadContext.FromCurrent().EndModalMessageLoop(null);
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.Exit"]/*' />
        /// <devdoc>
        ///    <para>Overload of Exit that does not care about e.Cancel.</para>
        /// </devdoc>
        public static void Exit() {
	        Exit(null);
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.Exit2"]/*' />
        /// <devdoc>
        ///    <para>Informs all message pumps that they are to terminate and
        ///       then closes all application windows after the messages have been processed.
        ///       e.Cancel indicates whether any of the open forms cancelled the exit call.</para>
        /// </devdoc>
        [
            EditorBrowsable(EditorBrowsableState.Advanced),
            SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")
        ]
        public static void Exit(CancelEventArgs e) {
            Assembly entryAssembly = Assembly.GetEntryAssembly();
            Assembly callingAssembly = Assembly.GetCallingAssembly();
            if (entryAssembly == null || callingAssembly == null || !entryAssembly.Equals(callingAssembly))
            {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "AffectThreadBehavior Demanded");
                IntSecurity.AffectThreadBehavior.Demand();
            }
            bool cancelExit = ExitInternal();
            if (e != null) {
                e.Cancel = cancelExit;
            }
        }

        /// <devdoc>
        ///    <para>Private version of Exit which does not do any security checks.</para>
        /// </devdoc>
        private static bool ExitInternal() {
            bool cancelExit = false;
            lock (internalSyncObject) {
                if (exiting) {
                    return false;
                }
                exiting = true;

                try
                {
                    // Raise the FormClosing and FormClosed events for each open form
                    if (forms != null) {
                        foreach (Form f in OpenFormsInternal) {
                            if (f.RaiseFormClosingOnAppExit()) {
                                cancelExit = true;
                                break; // quit the loop as soon as one form refuses to close
                            }
                        }
                    }
                    if (!cancelExit) {
                        if (forms != null) {
                            while (OpenFormsInternal.Count > 0) {
                                OpenFormsInternal[0].RaiseFormClosedOnAppExit(); // OnFormClosed removes the form from the FormCollection
                            }
                        }
                        ThreadContext.ExitApplication();
                    }
                }
                finally {
                    exiting = false;
                }
            }
            return cancelExit;
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ExitThread"]/*' />
        /// <devdoc>
        ///    <para>Exits the message loop on the
        ///       current thread and closes all windows on the thread.</para>
        /// </devdoc>
        public static void ExitThread() {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "AffectThreadBehavior Demanded");
            IntSecurity.AffectThreadBehavior.Demand();

            ExitThreadInternal();
        }

        private static void ExitThreadInternal() {
            ThreadContext context = ThreadContext.FromCurrent();
            if (context.ApplicationContext != null) {
                context.ApplicationContext.ExitThread();
            }
            else {
                context.Dispose(true);
            }
        }

        // When a Form receives a WM_ACTIVATE message, it calls this method so we can do the
        // appropriate MsoComponentManager activation magic
        internal static void FormActivated(bool modal, bool activated) {
            if (modal) {
                return;
            }

            ThreadContext.FromCurrent().FormActivated(activated);
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.GetAppFileVersionInfo"]/*' />
        /// <devdoc>
        ///     Retrieves the FileVersionInfo associated with the main module for
        ///     the application.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        private static FileVersionInfo GetAppFileVersionInfo() {
            lock (internalSyncObject) {
                if (appFileVersion == null) {
                    Type t = GetAppMainType();
                    if (t != null) {
                        // 


                        FileIOPermission fiop = new FileIOPermission( PermissionState.None );
                        fiop.AllFiles = FileIOPermissionAccess.PathDiscovery | FileIOPermissionAccess.Read;
                        fiop.Assert();

                        try {
                            appFileVersion = FileVersionInfo.GetVersionInfo(t.Module.FullyQualifiedName);
                        }
                        finally {
                            CodeAccessPermission.RevertAssert();
                        }
                    }
                    else {
                        appFileVersion = FileVersionInfo.GetVersionInfo(ExecutablePath);
                    }
                }
            }

            return(FileVersionInfo)appFileVersion;
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.GetAppMainType"]/*' />
        /// <devdoc>
        ///     Retrieves the Type that contains the "Main" method.
        /// </devdoc>
        private static Type GetAppMainType() {
            lock(internalSyncObject) {
                if (mainType == null) {
                    Assembly exe = Assembly.GetEntryAssembly();

                    // Get Main type...This doesn't work in MC++ because Main is a global function and not
                    // a class static method (it doesn't belong to a Type).
                    if (exe != null) {
                        mainType = exe.EntryPoint.ReflectedType;
                    }
                }
            }

            return mainType;
        }

        /// <devdoc>
        ///     Locates a thread context given a window handle.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        private static ThreadContext GetContextForHandle(HandleRef handle) {

            int pid;
            int id = SafeNativeMethods.GetWindowThreadProcessId(handle, out pid);
            ThreadContext cxt = ThreadContext.FromId(id);
            Debug.Assert(cxt != null, "No thread context for handle.  This is expected if you saw a previous assert about the handle being invalid.");
            return cxt;
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.GetDataPath"]/*' />
        /// <devdoc>
        ///     Returns a string that is the combination of the
        ///     basePath + CompanyName + ProducName + ProductVersion. This
        ///     will also create the directory if it doesn't exist.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        private static string GetDataPath(String basePath) {
            string template = @"{0}\{1}\{2}\{3}";

            string company = CompanyName;
            string product = ProductName;
            string version = ProductVersion;

            string path = string.Format(CultureInfo.CurrentCulture, template, new object[] {basePath, company, product, version});
            lock(internalSyncObject) {
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
            }
            return path;
        }


        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.RaiseExit"]/*' />
        /// <devdoc>
        ///     Called by the last thread context before it shuts down.
        /// </devdoc>
        private static void RaiseExit() {
            if (eventHandlers != null) {
                Delegate exit = eventHandlers[EVENT_APPLICATIONEXIT];
                if (exit != null)
                    ((EventHandler)exit)(null, EventArgs.Empty);
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.RaiseThreadExit"]/*' />
        /// <devdoc>
        ///     Called by the each thread context before it shuts down.
        /// </devdoc>
        private static void RaiseThreadExit() {
            if (eventHandlers != null) {
                Delegate exit = eventHandlers[EVENT_THREADEXIT];
                if (exit != null) {
                    ((EventHandler)exit)(null, EventArgs.Empty);
                }
            }
        }


        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ParkHandle"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///     "Parks" the given HWND to a temporary HWND.  This allows WS_CHILD windows to
        ///     be parked.
        /// </devdoc>
        internal static void ParkHandle(HandleRef handle, DpiAwarenessContext dpiAwarenessContext = DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED) {
            Debug.Assert(UnsafeNativeMethods.IsWindow(handle), "Handle being parked is not a valid window handle");
            Debug.Assert(((int)UnsafeNativeMethods.GetWindowLong(handle, NativeMethods.GWL_STYLE) & NativeMethods.WS_CHILD) != 0, "Only WS_CHILD windows should be parked.");

            ThreadContext cxt = GetContextForHandle(handle);
            if (cxt != null) {
                cxt.GetParkingWindow(dpiAwarenessContext).ParkHandle(handle);
            }
        }

        /// <summary>
        /// Park control handle on a parkingwindow that has matching DpiAwareness.
        /// </summary>
        /// <param name="cp"> create params for control handle</param>
        /// <param name="dpiContext"> dpi awareness</param>
        internal static void ParkHandle(CreateParams cp, DpiAwarenessContext dpiAwarenessContext = DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED) {

            ThreadContext cxt = ThreadContext.FromCurrent();
            if (cxt != null) {
                cp.Parent = cxt.GetParkingWindow(dpiAwarenessContext).Handle;
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.OleRequired"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes OLE on the current thread.
        ///    </para>
        /// </devdoc>
        public static System.Threading.ApartmentState OleRequired() {
            return ThreadContext.FromCurrent().OleRequired();
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.OnThreadException"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Application.ThreadException'/> event.</para>
        /// </devdoc>
        public static void OnThreadException(Exception t) {
            ThreadContext.FromCurrent().OnThreadException(t);
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.UnparkHandle"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///     "Unparks" the given HWND to a temporary HWND.  This allows WS_CHILD windows to
        ///     be parked.
        /// </devdoc>
        internal static void UnparkHandle(HandleRef handle, DpiAwarenessContext context) {
            ThreadContext cxt = GetContextForHandle(handle);
            if (cxt != null) {
                cxt.GetParkingWindow(context).UnparkHandle(handle);
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.RaiseIdle"]/*' />
        /// <devdoc>
        ///     Raises the Idle event.
        /// </devdoc>
        [
            EditorBrowsable(EditorBrowsableState.Advanced),
            SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode),
            SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers"),
            SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")
        ]
        public static void RaiseIdle(EventArgs e) {
            ThreadContext current = ThreadContext.FromCurrent();
            if (current.idleHandler != null) {
                current.idleHandler(Thread.CurrentThread, e);
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.RemoveMessageFilter"]/*' />
        /// <devdoc>
        ///    <para>Removes a message
        ///       filter from the application's message pump.</para>
        /// </devdoc>
        public static void RemoveMessageFilter(IMessageFilter value) {
            ThreadContext.FromCurrent().RemoveMessageFilter(value);
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.Restart"]/*' />
        /// <devdoc>
        ///    <para>Restarts the application.</para>
        /// </devdoc>
        [
            SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
            SecurityPermission(SecurityAction.Demand, Flags=SecurityPermissionFlag.UnmanagedCode)
        ]
        public static void Restart()
        {
            if (Assembly.GetEntryAssembly() == null)
            {
                throw new NotSupportedException(SR.RestartNotSupported);
            }

            bool hrefExeCase = false;

            Process process = Process.GetCurrentProcess();
            Debug.Assert(process != null);
            if (String.Equals(process.MainModule.ModuleName, IEEXEC, StringComparison.OrdinalIgnoreCase))
            {
                string clrPath = string.Empty;

                // 


                new FileIOPermission(PermissionState.Unrestricted).Assert();
                try
                {
                    clrPath = Path.GetDirectoryName(typeof(object).Module.FullyQualifiedName);
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }
                if (String.Equals(clrPath + "\\" + IEEXEC, process.MainModule.FileName, StringComparison.OrdinalIgnoreCase))
                {
                    // HRef exe case
                    hrefExeCase = true;
                    ExitInternal();
                    string launchUrl = AppDomain.CurrentDomain.GetData("APP_LAUNCH_URL") as string;
                    if (launchUrl != null)
                    {
                        Process.Start(process.MainModule.FileName, launchUrl);
                    }
                }
            }

            if (!hrefExeCase)
            {
#if CLICKONCE
                if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed)
                {
                    // ClickOnce app case
                    string appFullName = System.Deployment.Application.ApplicationDeployment.CurrentDeployment.UpdatedApplicationFullName;
                    UInt32 hostType = (UInt32) Application.ClickOnceUtility.GetHostTypeFromMetaData(appFullName);
                    ExitInternal();
                    UnsafeNativeMethods.CorLaunchApplication(hostType, appFullName, 0, null, 0, null, new UnsafeNativeMethods.PROCESS_INFORMATION());
                }
                else
#endif
                {
                    // Regular app case
                    String[] arguments = Environment.GetCommandLineArgs();
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
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.Run"]/*' />
        /// <devdoc>
        ///    <para>Begins running a
        ///       standard
        ///       application message loop on the current thread,
        ///       without a form.</para>
        /// </devdoc>
        public static void Run() {
            ThreadContext.FromCurrent().RunMessageLoop(NativeMethods.MSOCM.msoloopMain, new ApplicationContext());
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.Run1"]/*' />
        /// <devdoc>
        ///    <para>Begins running a standard application message loop on the current
        ///       thread, and makes the specified form visible.</para>
        /// </devdoc>
        public static void Run(Form mainForm) {
            ThreadContext.FromCurrent().RunMessageLoop(NativeMethods.MSOCM.msoloopMain, new ApplicationContext(mainForm));
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.Run2"]/*' />
        /// <devdoc>
        ///    <para>Begins running a
        ///       standard
        ///       application message loop on the current thread,
        ///       without a form.</para>
        /// </devdoc>
        public static void Run(ApplicationContext context) {
            ThreadContext.FromCurrent().RunMessageLoop(NativeMethods.MSOCM.msoloopMain, context);
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.RunDialog"]/*' />
        /// <devdoc>
        ///     Runs a modal dialog.  This starts a special type of message loop that runs until
        ///     the dialog has a valid DialogResult.  This is called internally by a form
        ///     when an application calls System.Windows.Forms.Form.ShowDialog().
        /// </devdoc>
        /// <internalonly/>
        internal static void RunDialog(Form form) {
            ThreadContext.FromCurrent().RunMessageLoop(NativeMethods.MSOCM.msoloopModalForm, new ModalApplicationContext(form));
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.SetCompatibleTextRenderingDefault"]/*' />
        /// <devdoc>
        ///     Sets the static UseCompatibleTextRenderingDefault field on Control to the value passed in. 
        ///     This switch determines the default text rendering engine to use by some controls that support 
        ///     switching rendering engine.
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        public static void SetCompatibleTextRenderingDefault(bool defaultValue)
        {
            if (NativeWindow.AnyHandleCreated)
            {
                throw new InvalidOperationException(SR.Win32WindowAlreadyCreated);
            }
            Control.UseCompatibleTextRenderingDefault = defaultValue;
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.SetSuspendState"]/*' />
        /// <devdoc>
        ///     Sets the suspend/hibernate state of the machine.
        ///     Returns true if the call succeeded, else false.
        /// </devdoc>
        public static bool SetSuspendState(PowerState state, bool force, bool disableWakeEvent) {
            IntSecurity.AffectMachineState.Demand();
            return UnsafeNativeMethods.SetSuspendState(state == PowerState.Hibernate, force, disableWakeEvent);
        }

        /// <devdoc>
        ///      Overload version of SetUnhandledExceptionMode that sets the UnhandledExceptionMode 
        ///      mode at the current thread level.
        /// </devdoc>
        public static void SetUnhandledExceptionMode(UnhandledExceptionMode mode)
        {
            SetUnhandledExceptionMode(mode, true /*threadScope*/);
        }

        /// <devdoc>
        ///     This method can be used to modify the exception handling behavior of
        ///     NativeWindow.  By default, NativeWindow will detect if an application
        ///     is running under a debugger, or is running on a machine with a debugger
        ///     installed.  In this case, an unhandled exception in the NativeWindow's
        ///     WndProc method will remain unhandled so the debugger can trap it.  If
        ///     there is no debugger installed NativeWindow will trap the exception
        ///     and route it to the Application class's unhandled exception filter.
        ///
        ///     You can control this behavior via a config file, or directly through
        ///     code using this method.  Setting the unhandled exception mode does
        ///     not change the behavior of any NativeWindow objects that are currently
        ///     connected to window handles; it only affects new handle connections.
        /// 
        ///     The parameter threadScope defines the scope of the setting: either
        ///     the current thread or the application.
        ///     When a thread exception mode isn't UnhandledExceptionMode.Automatic, it takes 
        ///     precedence over the application exception mode.
        /// </devdoc>
        public static void SetUnhandledExceptionMode(UnhandledExceptionMode mode, bool threadScope) {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "AffectThreadBehavior Demanded");
            IntSecurity.AffectThreadBehavior.Demand();

            NativeWindow.SetUnhandledExceptionModeInternal(mode, threadScope);
        }
        
        /*
        // Exposes GetHostTypeFromMetaData provided by the ClickOnce team.
        // Used to restart ClickOnce applications in Application.Restart().
        private class ClickOnceUtility
        {
            public enum HostType
            {
                Default = 0x0,
                AppLaunch = 0x1,
                CorFlag = 0x2
            }

            private ClickOnceUtility()
            {
            }

            public static HostType GetHostTypeFromMetaData(string appFullName)
            {
                HostType ht = HostType.Default;
                try
                {
                    // Convert into IDefinitionAppId.
                    IDefinitionAppId defAppId = IsolationInterop.AppIdAuthority.TextToDefinition(0, appFullName);
                    bool isFullTrust = GetPropertyBoolean(defAppId, "IsFullTrust");
                    ht = isFullTrust ? HostType.CorFlag : HostType.AppLaunch;
                }
                catch
                {
                    // Eating exceptions. IsFullTrust metadata is not present.
                }
                return ht;
            }

            // Get metadata property as boolean.
            private static bool GetPropertyBoolean(IDefinitionAppId appId, string propName)
            {
                string boolStr = GetPropertyString(appId, propName);
                if (string.IsNullOrEmpty(boolStr))
                {
                    return false;
                }
                try
                {
                    return Convert.ToBoolean(boolStr, CultureInfo.InvariantCulture);
                }
                catch
                {
                    return false;
                }
            }

            // Get metadata property as string.
            private static string GetPropertyString(IDefinitionAppId appId, string propName)
            {
                // Retrieve property and convert to string.
                byte[] bytes = IsolationInterop.UserStore.GetDeploymentProperty(0, appId,
                    InstallReference, new Guid("2ad613da-6fdb-4671-af9e-18ab2e4df4d8"), propName);

                // Check for valid Unicode string. Must end with L'\0'.
                int length = bytes.Length;
                if ((length == 0) || ((bytes.Length % 2) != 0) ||
                    (bytes[length - 2] != 0) || (bytes[length - 1] != 0))
                {
                    return null;
                }
                return Encoding.Unicode.GetString(bytes, 0, length - 2);
            }

            // Get the ClickOnce-specific constant install reference.
            private static StoreApplicationReference InstallReference
            {
                get
                {
                    return new StoreApplicationReference(
                        IsolationInterop.GUID_SXS_INSTALL_REFERENCE_SCHEME_OPAQUESTRING,
                        "{3f471841-eef2-47d6-89c0-d028f03a4ad5}", null);
                }
            }
        }
        */

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ComponentManager"]/*' />
        /// <devdoc>
        ///      This is our implementation of the MSO ComponentManager.  The Componoent Manager is
        ///      an object that is responsible for handling all message loop activity in a process.
        ///      The idea is that someone in the process implements the component manager and then
        ///      anyone who wants access to the message loop can get to it.  We implement this
        ///      so we have good interop with office and VS.  The first time we need a
        ///      component manager, we search the OLE message filter for one.  If that fails, we
        ///      create our own and install it in the message filter.
        ///
        ///      This class is not used when running inside the Visual Studio shell.
        /// </devdoc>
        private class ComponentManager : UnsafeNativeMethods.IMsoComponentManager
        {

            // ComponentManager instance data.
            //
            private class ComponentHashtableEntry {
                public UnsafeNativeMethods.IMsoComponent component;
                public NativeMethods.MSOCRINFOSTRUCT componentInfo;
            }

            private Hashtable oleComponents;
            private int cookieCounter = 0;
            private UnsafeNativeMethods.IMsoComponent activeComponent = null;
            private UnsafeNativeMethods.IMsoComponent trackingComponent = null;
            private int currentState = 0;

            private Hashtable OleComponents {
                get {
                    if (oleComponents == null) {
                        oleComponents = new Hashtable();
                        cookieCounter = 0;
                    }

                    return oleComponents;
                }
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ComponentManager.UnsafeNativeMethods.IMsoComponentManager.QueryService"]/*' />
            /// <devdoc>
            ///      Return in *ppvObj an implementation of interface iid for service
            ///      guidService (same as IServiceProvider::QueryService).
            ///      Return NOERROR if the requested service is supported, otherwise return
            ///      NULL in *ppvObj and an appropriate error (eg E_FAIL, E_NOINTERFACE).
            /// </devdoc>
            int UnsafeNativeMethods.IMsoComponentManager.QueryService(
                                                 ref Guid guidService,
                                                 ref Guid iid,
                                                 out object ppvObj) {

                ppvObj = null;
                return NativeMethods.E_NOINTERFACE;

            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ComponentManager.UnsafeNativeMethods.IMsoComponentManager.FDebugMessage"]/*' />
            /// <devdoc>
            ///      Standard FDebugMessage method.
            ///      Since IMsoComponentManager is a reference counted interface,
            ///      MsoDWGetChkMemCounter should be used when processing the
            ///      msodmWriteBe message.
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponentManager.FDebugMessage(
                                                   IntPtr hInst,
                                                   int msg,
                                                   IntPtr wparam,
                                                   IntPtr lparam) {

                return true;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ComponentManager.UnsafeNativeMethods.IMsoComponentManager.FRegisterComponent"]/*' />
            /// <devdoc>
            ///      Register component piComponent and its registration info pcrinfo with
            ///      this component manager.  Return in *pdwComponentID a cookie which will
            ///      identify the component when it calls other IMsoComponentManager
            ///      methods.
            ///      Return TRUE if successful, FALSE otherwise.
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponentManager.FRegisterComponent(UnsafeNativeMethods.IMsoComponent component,
                                                         NativeMethods.MSOCRINFOSTRUCT pcrinfo,
                                                         out IntPtr dwComponentID) {

                // Construct Hashtable entry for this component
                //
                ComponentHashtableEntry entry = new ComponentHashtableEntry();
                entry.component = component;
                entry.componentInfo = pcrinfo;
                OleComponents.Add(++cookieCounter, entry);

                // Return the cookie
                //
                dwComponentID = (IntPtr)cookieCounter;
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager: Component registered.  ID: " + cookieCounter.ToString(CultureInfo.InvariantCulture));
                return true;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ComponentManager.UnsafeNativeMethods.IMsoComponentManager.FRevokeComponent"]/*' />
            /// <devdoc>
            ///      Undo the registration of the component identified by dwComponentID
            ///      (the cookie returned from the FRegisterComponent method).
            ///      Return TRUE if successful, FALSE otherwise.
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponentManager.FRevokeComponent(IntPtr dwComponentID) {
                int dwLocalComponentID = unchecked((int)(long)dwComponentID);

                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager: Revoking component " + dwLocalComponentID.ToString(CultureInfo.InvariantCulture));

                ComponentHashtableEntry entry = (ComponentHashtableEntry)OleComponents[dwLocalComponentID];
                if (entry == null) {
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "Compoenent not registered.");
                    return false;
                }

                if (entry.component == activeComponent) {
                    activeComponent = null;
                }
                if (entry.component == trackingComponent) {
                    trackingComponent = null;
                }

                OleComponents.Remove(dwLocalComponentID);

                return true;

            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ComponentManager.UnsafeNativeMethods.IMsoComponentManager.FUpdateComponentRegistration"]/*' />
            /// <devdoc>
            ///      Update the registration info of the component identified by
            ///      dwComponentID (the cookie returned from FRegisterComponent) with the
            ///      new registration information pcrinfo.
            ///      Typically this is used to update the idle time registration data, but
            ///      can be used to update other registration data as well.
            ///      Return TRUE if successful, FALSE otherwise.
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponentManager.FUpdateComponentRegistration(
                                                                  IntPtr dwComponentID,
                                                                  NativeMethods.MSOCRINFOSTRUCT info
                                                                  ) {
                int dwLocalComponentID = unchecked((int)(long)dwComponentID);
                // Update the registration info
                //
                ComponentHashtableEntry entry = (ComponentHashtableEntry)OleComponents[dwLocalComponentID];
                if (entry == null) {
                    return false;
                }

                entry.componentInfo = info;

                return true;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ComponentManager.UnsafeNativeMethods.IMsoComponentManager.FOnComponentActivate"]/*' />
            /// <devdoc>
            ///      Notify component manager that component identified by dwComponentID
            ///      (cookie returned from FRegisterComponent) has been activated.
            ///      The active component gets the chance to process messages before they
            ///      are dispatched (via IMsoComponent::FPreTranslateMessage) and typically
            ///      gets first chance at idle time after the host.
            ///      This method fails if another component is already Exclusively Active.
            ///      In this case, FALSE is returned and SetLastError is set to
            ///      msoerrACompIsXActive (comp usually need not take any special action
            ///      in this case).
            ///      Return TRUE if successful.
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponentManager.FOnComponentActivate(IntPtr dwComponentID) {

                int dwLocalComponentID = unchecked((int)(long)dwComponentID);
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager: Component activated.  ID: " + dwLocalComponentID.ToString(CultureInfo.InvariantCulture));

                ComponentHashtableEntry entry = (ComponentHashtableEntry)OleComponents[dwLocalComponentID];
                if (entry == null) {
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "*** Component not registered ***");
                    return false;
                }

                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "New active component is : " + entry.component.ToString());
                activeComponent = entry.component;
                return true;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ComponentManager.UnsafeNativeMethods.IMsoComponentManager.FSetTrackingComponent"]/*' />
            /// <devdoc>
            ///      Called to inform component manager that  component identified by
            ///      dwComponentID (cookie returned from FRegisterComponent) wishes
            ///      to perform a tracking operation (such as mouse tracking).
            ///      The component calls this method with fTrack == TRUE to begin the
            ///      tracking operation and with fTrack == FALSE to end the operation.
            ///      During the tracking operation the component manager routes messages
            ///      to the tracking component (via IMsoComponent::FPreTranslateMessage)
            ///      rather than to the active component.  When the tracking operation ends,
            ///      the component manager should resume routing messages to the active
            ///      component.
            ///      Note: component manager should perform no idle time processing during a
            ///              tracking operation other than give the tracking component idle
            ///              time via IMsoComponent::FDoIdle.
            ///      Note: there can only be one tracking component at a time.
            ///      Return TRUE if successful, FALSE otherwise.
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponentManager.FSetTrackingComponent(IntPtr dwComponentID, bool fTrack) {

                int dwLocalComponentID = unchecked((int)(long)dwComponentID);
                ComponentHashtableEntry entry = (ComponentHashtableEntry)OleComponents[dwLocalComponentID];
                if (entry == null) {
                    return false;
                }

                if (entry.component == trackingComponent ^ fTrack) {
                    return false;
                }

                if (fTrack) {
                    trackingComponent = entry.component;
                }
                else {
                    trackingComponent = null;
                }

                return true;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ComponentManager.UnsafeNativeMethods.IMsoComponentManager.OnComponentEnterState"]/*' />
            /// <devdoc>
            ///      Notify component manager that component identified by dwComponentID
            ///      (cookie returned from FRegisterComponent) is entering the state
            ///      identified by uStateID (msocstateXXX value).  (For convenience when
            ///      dealing with sub CompMgrs, the host can call this method passing 0 for
            ///      dwComponentID.)
            ///      Component manager should notify all other interested components within
            ///      the state context indicated by uContext (a msoccontextXXX value),
            ///      excluding those within the state context of a CompMgr in rgpicmExclude,
            ///      via IMsoComponent::OnEnterState (see "Comments on State Contexts",
            ///      above).
            ///      Component Manager should also take appropriate action depending on the
            ///      value of uStateID (see msocstate comments, above).
            ///      dwReserved is reserved for future use and should be zero.
            ///
            ///      rgpicmExclude (can be NULL) is an array of cpicmExclude CompMgrs (can
            ///      include root CompMgr and/or sub CompMgrs); components within the state
            ///      context of a CompMgr appearing in this     array should NOT be notified of
            ///      the state change (note: if uContext        is msoccontextMine, the only
            ///      CompMgrs in rgpicmExclude that are checked for exclusion are those that
            ///      are sub CompMgrs of this Component Manager, since all other CompMgrs
            ///      are outside of this Component Manager's state context anyway.)
            ///
            ///      Note: Calls to this method are symmetric with calls to
            ///      FOnComponentExitState.
            ///      That is, if n OnComponentEnterState calls are made, the component is
            ///      considered to be in the state until n FOnComponentExitState calls are
            ///      made.  Before revoking its registration a component must make a
            ///      sufficient number of FOnComponentExitState calls to offset any
            ///      outstanding OnComponentEnterState calls it has made.
            ///
            ///      Note: inplace objects should not call this method with
            ///      uStateID == msocstateModal when entering modal state. Such objects
            ///      should call IOleInPlaceFrame::EnableModeless instead.
            /// </devdoc>
            void UnsafeNativeMethods.IMsoComponentManager.OnComponentEnterState(
                                                           IntPtr dwComponentID,
                                                           int uStateID,
                                                           int uContext,
                                                           int cpicmExclude,
                                                           int rgpicmExclude,          // IMsoComponentManger**
                                                           int dwReserved) {

                int dwLocalComponentID = unchecked((int)(long)dwComponentID);
                currentState |= uStateID;

                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager: Component enter state.  ID: " + dwLocalComponentID.ToString(CultureInfo.InvariantCulture) + " state: " + uStateID.ToString(CultureInfo.InvariantCulture));

                if (uContext == NativeMethods.MSOCM.msoccontextAll || uContext == NativeMethods.MSOCM.msoccontextMine) {

                    Debug.Indent();

                    // We should notify all components we contain that the state has changed.
                    //
                    foreach (ComponentHashtableEntry entry in OleComponents.Values) {
                        Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "Notifying " + entry.component.ToString());
                        entry.component.OnEnterState(uStateID, true);
                    }

                    Debug.Unindent();
                }
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ComponentManager.UnsafeNativeMethods.IMsoComponentManager.FOnComponentExitState"]/*' />
            /// <devdoc>
            ///      Notify component manager that component identified by dwComponentID
            ///      (cookie returned from FRegisterComponent) is exiting the state
            ///      identified by uStateID (a msocstateXXX value).  (For convenience when
            ///      dealing with sub CompMgrs, the host can call this method passing 0 for
            ///      dwComponentID.)
            ///      uContext, cpicmExclude, and rgpicmExclude are as they are in
            ///      OnComponentEnterState.
            ///      Component manager  should notify all appropriate interested components
            ///      (taking into account uContext, cpicmExclude, rgpicmExclude) via
            ///      IMsoComponent::OnEnterState (see "Comments on State Contexts", above).
            ///      Component Manager should also take appropriate action depending on
            ///      the value of uStateID (see msocstate comments, above).
            ///      Return TRUE if, at the end of this call, the state is still in effect
            ///      at the root of this component manager's state context
            ///      (because the host or some other component is still in the state),
            ///      otherwise return FALSE (ie. return what FInState would return).
            ///      Caller can normally ignore the return value.
            ///
            ///      Note: n calls to this method are symmetric with n calls to
            ///      OnComponentEnterState (see OnComponentEnterState comments, above).
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponentManager.FOnComponentExitState(
                                                           IntPtr dwComponentID,
                                                           int uStateID,
                                                           int uContext,
                                                           int cpicmExclude,
                                                           int rgpicmExclude       // IMsoComponentManager**
                                                           ) {
                int dwLocalComponentID = unchecked((int)(long)dwComponentID);
                currentState &= ~uStateID;

                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager: Component exit state.  ID: " + dwLocalComponentID.ToString(CultureInfo.InvariantCulture) + " state: " + uStateID.ToString(CultureInfo.InvariantCulture));

                if (uContext == NativeMethods.MSOCM.msoccontextAll || uContext == NativeMethods.MSOCM.msoccontextMine) {

                    Debug.Indent();

                    // We should notify all components we contain that the state has changed.
                    //
                    foreach (ComponentHashtableEntry entry in OleComponents.Values) {
                        Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "Notifying " + entry.component.ToString());
                        entry.component.OnEnterState(uStateID, false);
                    }

                    Debug.Unindent();
                }

                return false;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ComponentManager.UnsafeNativeMethods.IMsoComponentManager.FInState"]/*' />
            /// <devdoc>
            ///      Return TRUE if the state identified by uStateID (a msocstateXXX value)
            ///      is in effect at the root of this component manager's state context,
            ///      FALSE otherwise (see "Comments on State Contexts", above).
            ///      pvoid is reserved for future use and should be NULL.
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponentManager.FInState(int uStateID, IntPtr pvoid) {
                return(currentState & uStateID) != 0;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ComponentManager.UnsafeNativeMethods.IMsoComponentManager.FContinueIdle"]/*' />
            /// <devdoc>
            ///      Called periodically by a component during IMsoComponent::FDoIdle.
            ///      Return TRUE if component can continue its idle time processing,
            ///      FALSE if not (in which case component returns from FDoIdle.)
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponentManager.FContinueIdle() {

                // Essentially, if we have a message on queue, then don't continue
                // idle processing.
                //
                NativeMethods.MSG msg = new NativeMethods.MSG();
                return !UnsafeNativeMethods.PeekMessage(ref msg, NativeMethods.NullHandleRef, 0, 0, NativeMethods.PM_NOREMOVE);
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ComponentManager.UnsafeNativeMethods.IMsoComponentManager.FPushMessageLoop"]/*' />
            /// <devdoc>
            ///      Component identified by dwComponentID (cookie returned from
            ///      FRegisterComponent) wishes to push a message loop for reason uReason.
            ///      uReason is one the values from the msoloop enumeration (above).
            ///      pvLoopData is data private to the component.
            ///      The component manager should push its message loop,
            ///      calling IMsoComponent::FContinueMessageLoop(uReason, pvLoopData)
            ///      during each loop iteration (see IMsoComponent::FContinueMessageLoop
            ///      comments).  When IMsoComponent::FContinueMessageLoop returns FALSE, the
            ///      component manager terminates the loop.
            ///      Returns TRUE if component manager terminates loop because component
            ///      told it to (by returning FALSE from IMsoComponent::FContinueMessageLoop),
            ///      FALSE if it had to terminate the loop for some other reason.  In the
            ///      latter case, component should perform any necessary action (such as
            ///      cleanup).
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponentManager.FPushMessageLoop(
                                                      IntPtr dwComponentID,
                                                      int reason,
                                                      int pvLoopData          // PVOID
                                                      ) {

                int dwLocalComponentID = unchecked((int)(long)dwComponentID);
                // Hold onto old state to allow restore before we exit...
                //
                int currentLoopState = currentState;
                bool continueLoop = true;

                if (!OleComponents.ContainsKey(dwLocalComponentID)) {
                    return false;
                }

                UnsafeNativeMethods.IMsoComponent prevActive = this.activeComponent;

                try {
                    // Execute the message loop until the active component tells us to stop.
                    //
                    NativeMethods.MSG msg = new NativeMethods.MSG();
                    NativeMethods.MSG[] rgmsg = new NativeMethods.MSG[] {msg};
                    bool unicodeWindow = false;
                    UnsafeNativeMethods.IMsoComponent requestingComponent;

                    ComponentHashtableEntry entry = (ComponentHashtableEntry)OleComponents[dwLocalComponentID];
                    if (entry == null) {
                        return false;
                    }



                    requestingComponent = entry.component;

                    this.activeComponent = requestingComponent;

                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : Pushing message loop " + reason.ToString(CultureInfo.InvariantCulture));
                    Debug.Indent();

                    while (continueLoop) {

                        // Determine the component to route the message to
                        //
                        UnsafeNativeMethods.IMsoComponent component;

                        if (trackingComponent != null) {
                            component = trackingComponent;
                        }
                        else if (activeComponent != null) {
                            component = activeComponent;
                        }
                        else {
                            component = requestingComponent;
                        }

                        bool peeked = UnsafeNativeMethods.PeekMessage(ref msg, NativeMethods.NullHandleRef, 0, 0, NativeMethods.PM_NOREMOVE);

                        if (peeked) {

                            rgmsg[0] = msg;
                            continueLoop = component.FContinueMessageLoop(reason, pvLoopData, rgmsg);

                            // If the component wants us to process the message, do it.
                            // The component manager hosts windows from many places.  We must be sensitive
                            // to ansi / Unicode windows here.
                            //
                            if (continueLoop) {
                                if (msg.hwnd != IntPtr.Zero && SafeNativeMethods.IsWindowUnicode(new HandleRef(null, msg.hwnd))) {
                                    unicodeWindow = true;
                                    UnsafeNativeMethods.GetMessageW(ref msg, NativeMethods.NullHandleRef, 0, 0);
                                }
                                else {
                                    unicodeWindow = false;
                                    UnsafeNativeMethods.GetMessageA(ref msg, NativeMethods.NullHandleRef, 0, 0);
                                }

                                if (msg.message == NativeMethods.WM_QUIT) {
                                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : Normal message loop termination");

                                    Application.ThreadContext.FromCurrent().DisposeThreadWindows();

                                    if (reason != NativeMethods.MSOCM.msoloopMain) {
                                        UnsafeNativeMethods.PostQuitMessage((int)msg.wParam);
                                    }

                                    continueLoop = false;
                                    break;
                                }

                                // Now translate and dispatch the message.
                                //
                                // Reading through the rather sparse documentation,
                                // it seems we should only call FPreTranslateMessage
                                // on the active component.
                                if (!component.FPreTranslateMessage(ref msg)) {
                                    UnsafeNativeMethods.TranslateMessage(ref msg);
                                    if (unicodeWindow) {
                                        UnsafeNativeMethods.DispatchMessageW(ref msg);
                                    }
                                    else {
                                        UnsafeNativeMethods.DispatchMessageA(ref msg);
                                    }
                                }
                            }
                        }
                        else {

                            // If this is a DoEvents loop, then get out.  There's nothing left
                            // for us to do.
                            //
                            if (reason == NativeMethods.MSOCM.msoloopDoEvents ||
                                reason == NativeMethods.MSOCM.msoloopDoEventsModal) {
                                break;
                            }

                            // Nothing is on the message queue.  Perform idle processing
                            // and then do a WaitMessage.
                            //
                            bool continueIdle = false;

                            if (OleComponents != null) {
                                IEnumerator enumerator = OleComponents.Values.GetEnumerator();

                                while (enumerator.MoveNext()) {
                                    ComponentHashtableEntry idleEntry = (ComponentHashtableEntry)enumerator.Current;
                                    continueIdle |= idleEntry.component.FDoIdle(-1);
                                }
                            }

                            // give the component one more chance to terminate the
                            // message loop.
                            //
                            continueLoop = component.FContinueMessageLoop(reason, pvLoopData, null);

                            if (continueLoop) {
                                if (continueIdle) {
                                    // If someone has asked for idle time, give it to them.  However,
                                    // don't cycle immediately; wait up to 100ms.  Why?  Because we don't
                                    // want someone to attach to idle, forget to detach, and then cause
                                    // CPU to end up in race condition.  For Windows Forms this generally isn't an issue because
                                    // our component always returns false from its idle request
                                    UnsafeNativeMethods.MsgWaitForMultipleObjectsEx(0, IntPtr.Zero, 100, NativeMethods.QS_ALLINPUT, NativeMethods.MWMO_INPUTAVAILABLE);
                                }
                                else {
                                    // We should call GetMessage here, but we cannot because
                                    // the component manager requires that we notify the
                                    // active component before we pull the message off the
                                    // queue.  This is a bit of a problem, because WaitMessage
                                    // waits for a NEW message to appear on the queue.  If a
                                    // message appeared between processing and now WaitMessage
                                    // would wait for the next message.  We minimize this here
                                    // by calling PeekMessage.
                                    //
                                    if (!UnsafeNativeMethods.PeekMessage(ref msg, NativeMethods.NullHandleRef, 0, 0, NativeMethods.PM_NOREMOVE)) {
                                        UnsafeNativeMethods.WaitMessage();
                                    }
                                }
                            }
                        }
                    }

                    Debug.Unindent();
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : message loop " + reason.ToString(CultureInfo.InvariantCulture) + " complete.");
                }
                finally {
                    currentState = currentLoopState;
                    this.activeComponent = prevActive;
                }

                return !continueLoop;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ComponentManager.UnsafeNativeMethods.IMsoComponentManager.FCreateSubComponentManager"]/*' />
            /// <devdoc>
            ///      Cause the component manager to create a "sub" component manager, which
            ///      will be one of its children in the hierarchical tree of component
            ///      managers used to maintiain state contexts (see "Comments on State
            ///      Contexts", above).
            ///      piunkOuter is the controlling unknown (can be NULL), riid is the
            ///      desired IID, and *ppvObj returns   the created sub component manager.
            ///      piunkServProv (can be NULL) is a ptr to an object supporting
            ///      IServiceProvider interface to which the created sub component manager
            ///      will delegate its IMsoComponentManager::QueryService calls.
            ///      (see objext.h or docobj.h for definition of IServiceProvider).
            ///      Returns TRUE if successful.
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponentManager.FCreateSubComponentManager(
                                                                object punkOuter,
                                                                object punkServProv,
                                                                ref Guid riid,
                                                                out IntPtr ppvObj) {

                // We do not support sub component managers.
                //
                ppvObj = IntPtr.Zero;
                return false;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ComponentManager.UnsafeNativeMethods.IMsoComponentManager.FGetParentComponentManager"]/*' />
            /// <devdoc>
            ///      Return in *ppicm an AddRef'ed ptr to this component manager's parent
            ///      in the hierarchical tree of component managers used to maintain state
            ///      contexts (see "Comments on State   Contexts", above).
            ///      Returns TRUE if the parent is returned, FALSE if no parent exists or
            ///      some error occurred.
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponentManager.FGetParentComponentManager(out UnsafeNativeMethods.IMsoComponentManager ppicm) {
                ppicm = null;
                return false;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ComponentManager.UnsafeNativeMethods.IMsoComponentManager.FGetActiveComponent"]/*' />
            /// <devdoc>
            ///      Return in *ppic an AddRef'ed ptr to the current active or tracking
            ///      component (as indicated by dwgac (a msogacXXX value)), and
            ///      its registration information in *pcrinfo.  ppic and/or pcrinfo can be
            ///      NULL if caller is not interested these values.  If pcrinfo is not NULL,
            ///      caller should set pcrinfo->cbSize before calling this method.
            ///      Returns TRUE if the component indicated by dwgac exists, FALSE if no
            ///      such component exists or some error occurred.
            ///      dwReserved is reserved for future use and should be zero.
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponentManager.FGetActiveComponent(
                                                         int dwgac,
                                                         UnsafeNativeMethods.IMsoComponent[] ppic,
                                                         NativeMethods.MSOCRINFOSTRUCT info,
                                                         int dwReserved) {

                UnsafeNativeMethods.IMsoComponent component = null;

                if (dwgac == NativeMethods.MSOCM.msogacActive) {
                    component = activeComponent;
                }
                else if (dwgac == NativeMethods.MSOCM.msogacTracking) {
                    component = trackingComponent;
                }
                else if (dwgac == NativeMethods.MSOCM.msogacTrackingOrActive) {
                    if (trackingComponent != null) {
                        component = trackingComponent;
                    }
                    else {
                        component = activeComponent;
                    }
                }
                else {
                    Debug.Fail("Unknown dwgac in FGetActiveComponent");
                }

                if (ppic != null) {
                    ppic[0] = component;
                }
                if (info != null && component != null) {
                    foreach(ComponentHashtableEntry entry in OleComponents.Values) {
                        if (entry.component == component) {
                            info = entry.componentInfo;
                            break;
                        }
                    }
                }

                return component != null;
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext"]/*' />
        /// <devdoc>
        ///     This class is the embodiment of TLS for windows forms.  We do not expose this to end users because
        ///     TLS is really just an unfortunate artifact of using Win 32.  We want the world to be free
        ///     threaded.
        /// </devdoc>
        /// <internalonly/>
        internal sealed class ThreadContext : MarshalByRefObject, UnsafeNativeMethods.IMsoComponent {

            private const int STATE_OLEINITIALIZED       = 0x00000001;
            private const int STATE_EXTERNALOLEINIT      = 0x00000002;
            private const int STATE_INTHREADEXCEPTION    = 0x00000004;
            private const int STATE_POSTEDQUIT           = 0x00000008;
            private const int STATE_FILTERSNAPSHOTVALID  = 0x00000010;
            private const int STATE_TRACKINGCOMPONENT    = 0x00000020;
            private const int INVALID_ID = unchecked((int)0xFFFFFFFF);

            private static Hashtable        contextHash = new Hashtable();

            // When this gets to zero, we'll invoke a full garbage
            // collect and check for root/window leaks.
            //
            private static object           tcInternalSyncObject = new object();

            private static int              totalMessageLoopCount;
            private static int              baseLoopReason;

            [ ThreadStatic ]
            private static ThreadContext    currentThreadContext;

            internal ThreadExceptionEventHandler threadExceptionHandler;
            internal EventHandler           idleHandler;
            internal EventHandler           enterModalHandler;
            internal EventHandler           leaveModalHandler;
            private ApplicationContext      applicationContext;

            // Parking window list
            private List<ParkingWindow> parkingWindows = new List<ParkingWindow>();
            private Control                 marshalingControl;
            private CultureInfo             culture;
            private ArrayList               messageFilters;
            private IMessageFilter[]        messageFilterSnapshot;
            private int                     inProcessFilters = 0;
            private IntPtr                  handle;
            private int                     id;
            private int                     messageLoopCount;
            private int                     threadState;
            private int                     modalCount;

            // used for correct restoration of focus after modality
            private WeakReference           activatingControlRef;                  
            
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

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.ThreadContext"]/*' />
            /// <devdoc>
            ///     Creates a new thread context object.
            /// </devdoc>
            [ResourceExposure(ResourceScope.Process)]
            [ResourceConsumption(ResourceScope.Process)]
            public ThreadContext() {
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

            public ApplicationContext ApplicationContext {
                get {
                    return applicationContext;
                }
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.ComponentManager"]/*' />
            /// <devdoc>
            ///      Retrieves the component manager for this process.  If there is no component manager
            ///      currently installed, we install our own.
            /// </devdoc>
            internal UnsafeNativeMethods.IMsoComponentManager ComponentManager {
                get {

                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "Application.ComponentManager.Get:");

                    if (componentManager == null) {

                        // The CLR is a good COM citizen and will pump messages when things are waiting.
                        // This is nice; it keeps the world responsive.  But, it is also very hard for
                        // us because most of the code below causes waits, and the likelihood that
                        // a message will come in and need a component manager is very high.  Recursing
                        // here is very very bad, and will almost certainly lead to application failure
                        // later on as we come out of the recursion.  So, we guard it here and return
                        // null.  EVERYONE who accesses the component manager must handle a NULL return!
                        //
                        if (fetchingComponentManager) {
                            return null;
                        }

                        fetchingComponentManager = true;
                        try {
                            UnsafeNativeMethods.IMsoComponentManager msocm = null;
                            Application.OleRequired();

                            // Attempt to obtain the Host Application MSOComponentManager
                            //
                            IntPtr msgFilterPtr = (IntPtr)0;

                            if (NativeMethods.Succeeded(UnsafeNativeMethods.CoRegisterMessageFilter(NativeMethods.NullHandleRef, ref msgFilterPtr)) && msgFilterPtr != (IntPtr)0) {

                                IntPtr dummy = (IntPtr)0;
                                UnsafeNativeMethods.CoRegisterMessageFilter(new HandleRef(null, msgFilterPtr), ref dummy);

                                object msgFilterObj = Marshal.GetObjectForIUnknown(msgFilterPtr);
                                Marshal.Release(msgFilterPtr);

                                UnsafeNativeMethods.IOleServiceProvider sp = msgFilterObj as UnsafeNativeMethods.IOleServiceProvider;
                                if (sp != null) {
                                    try {
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

                                        if (NativeMethods.Succeeded(hr) && retval != IntPtr.Zero) {

                                            // Now query for hte message filter.

                                            IntPtr pmsocm;

                                            try {
                                                Guid IID_IMsoComponentManager = typeof(UnsafeNativeMethods.IMsoComponentManager).GUID;
                                                hr = Marshal.QueryInterface(retval, ref IID_IMsoComponentManager, out pmsocm);
                                            }
                                            finally {
                                                Marshal.Release(retval);
                                            }

                                            if (NativeMethods.Succeeded(hr) && pmsocm != IntPtr.Zero) {

                                                // Ok, we have a native component manager.  Hand this over to
                                                // our broker object to get a proxy we can use
                                                try {
                                                    msocm = ComponentManagerBroker.GetComponentManager(pmsocm);
                                                }
                                                finally {
                                                    Marshal.Release(pmsocm);
                                                }
                                            }

                                            if (msocm != null) {

                                                // If the resulting service is the same pUnk as the
                                                // message filter (a common implementation technique),
                                                // then we want to null msgFilterObj at this point so
                                                // we don't call RelaseComObject on it below.  That would
                                                // also release the RCW for the component manager pointer.
                                                if (msgFilterPtr == retval) {
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
                                    catch {
                                    }
                                }

                                if (msgFilterObj != null && Marshal.IsComObject(msgFilterObj)) {
                                    Marshal.ReleaseComObject(msgFilterObj);
                                }
                            }

                            // Otherwise, we implement component manager ourselves
                            //
                            if (msocm == null) {
                                msocm = new ComponentManager();
                                externalComponentManager = false;

                                // We must also store this back into the message filter for others
                                // to use.
                                //
                                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "Using our own component manager");
                            }

                            if (msocm != null && componentID == INVALID_ID) {
                                // Finally, if we got a compnent manager, register ourselves with it.
                                //
                                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "Registering MSO component with the component manager");
                                NativeMethods.MSOCRINFOSTRUCT info = new NativeMethods.MSOCRINFOSTRUCT();
                                info.cbSize = Marshal.SizeOf(typeof(NativeMethods.MSOCRINFOSTRUCT));
                                info.uIdleTimeInterval = 0;
                                info.grfcrf = NativeMethods.MSOCM.msocrfPreTranslateAll | NativeMethods.MSOCM.msocrfNeedIdleTime;
                                info.grfcadvf = NativeMethods.MSOCM.msocadvfModal;

                                IntPtr localComponentID;
                                bool result = msocm.FRegisterComponent(this, info, out localComponentID);
                                componentID = unchecked((int)(long)localComponentID);
                                Debug.Assert(componentID != INVALID_ID, "Our ID sentinel was returned as a valid ID");

                                if (result && !(msocm is ComponentManager)) {
                                    messageLoopCount++;
                                }

                                Debug.Assert(result, "Failed to register WindowsForms with the ComponentManager -- DoEvents and modal dialogs will be broken. size: " + info.cbSize);
                                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager.FRegisterComponent returned " + result.ToString());
                                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager.FRegisterComponent assigned a componentID == [0x" + Convert.ToString(componentID, 16) + "]");
                                componentManager = msocm;
                            }
                        }
                        finally {
                            fetchingComponentManager = false;
                        }
                    }

                    return componentManager;
                }
            }

            internal bool CustomThreadExceptionHandlerAttached {
                get {             
                     return threadExceptionHandler != null;
                }
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.ParkingWindow"]/*' />
            /// <devdoc>
            ///     Retrieves the actual parking form.  This will demand create the parking window
            ///     if it needs to.
            /// </devdoc>
            /// <internalonly/>
            internal ParkingWindow GetParkingWindow(DpiAwarenessContext context) {

                // Locking 'this' here is ok since this is an internal class.
                lock(this) {
                    var parkingWindow = GetParkingWindowForContext(context);
                    if (parkingWindow == null) {
#if DEBUG
                        // if we use Debug.WriteLine instead of "if", we need extra security permissions
                        // to get the stack trace!
                        if (CoreSwitches.PerfTrack.Enabled) {
                            Debug.WriteLine("Creating parking form!");
                            Debug.WriteLine(CoreSwitches.PerfTrack.Enabled, Environment.StackTrace);
                        }
#endif





                        IntSecurity.ManipulateWndProcAndHandles.Assert();
                        try {
                            using (DpiHelper.EnterDpiAwarenessScope(context)) {
                                parkingWindow = new ParkingWindow();
                            }

                            parkingWindows.Add(parkingWindow);
                        }
                        finally {
                            CodeAccessPermission.RevertAssert();
                        }
                    }
                    return parkingWindow;
                }
            }

            /// <summary>
            /// Returns parking window that matches dpi awareness context. return null if not found.
            /// </summary>
            /// <returns>return matching parking window from list. returns null if not found</returns>
            internal ParkingWindow GetParkingWindowForContext(DpiAwarenessContext context) {

                if (parkingWindows.Count == 0) {
                    return null;
                }

                // Legacy OS/target framework scenario where ControlDpiContext is set to DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_UNSPECIFIED
                // because of 'ThreadContextDpiAwareness' API unavailability or this feature is not enabled.

                if (!DpiHelper.IsScalingRequirementMet || CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(context, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED)) {

                    Debug.Assert(parkingWindows.Count == 1, "parkingWindows count can not be > 1 for legacy OS/target framework versions");
                    return parkingWindows[0];
                }

                // Supported OS scenario.
                foreach (var p in parkingWindows) {
                    if (CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(p.DpiAwarenessContext, context)) {
                        return p;
                    }
                }

                // parking window is not yet created for the requested DpiAwarenessContext
                return null;
            }

            internal Control ActivatingControl {
               get { 
                    if ((activatingControlRef != null) && (activatingControlRef.IsAlive)) {
                        return activatingControlRef.Target as Control;
                    }
                    return null;
                }
                set {
                    if (value != null) {
                        activatingControlRef = new WeakReference(value);
                    }
                    else {
                        activatingControlRef = null;
                    }
                }
            }


            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.MarshalingControl"]/*' />
            /// <devdoc>
            ///     Retrieves the actual parking form.  This will demand create the MarshalingControl window
            ///     if it needs to.
            /// </devdoc>
            /// <internalonly/>
            internal Control MarshalingControl {
                get {
                    lock (this) {
                        if (marshalingControl == null) {
#if DEBUG
                            // if we use Debug.WriteLine instead of "if", we need extra security permissions
                            // to get the stack trace!
                            if (CoreSwitches.PerfTrack.Enabled) {
                                Debug.WriteLine("Creating marshalling control!");
                                Debug.WriteLine(CoreSwitches.PerfTrack.Enabled, Environment.StackTrace);
                            }
#endif

                            marshalingControl = new System.Windows.Forms.Application.MarshalingControl();
                        }
                        return marshalingControl;
                    }
                }
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.AddMessageFilter"]/*' />
            /// <devdoc>
            ///     Allows you to setup a message filter for the application's message pump.  This
            ///     installs the filter on the current thread.
            /// </devdoc>
            /// <internalonly/>
            internal void AddMessageFilter(IMessageFilter f) {
                if (messageFilters == null) {
                    messageFilters = new ArrayList();
                }
                if (f != null) {
                    SetState(STATE_FILTERSNAPSHOTVALID, false);
                    if (messageFilters.Count > 0 && f is IMessageModifyAndFilter) {
                        // insert the IMessageModifyAndFilter filters first
                        messageFilters.Insert(0, f);
                    }
                    else {
                        messageFilters.Add(f);
                    }
                }
            }

            // Called immediately before we begin pumping messages for a modal message loop.
            internal void BeginModalMessageLoop(ApplicationContext context) {
#if DEBUG
                debugModalCounter++;
#endif
                // Set the ourModalLoop flag so that the "IMSOComponent.OnEnterState" is a NOOP since we started the ModalMessageLoop.
                bool wasOurLoop = ourModalLoop;
                ourModalLoop = true;
                try
                {
                    UnsafeNativeMethods.IMsoComponentManager cm = ComponentManager;
                    if (cm != null) {
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
                
                if (enterModalHandler != null && modalCount == 1) {
                    enterModalHandler(Thread.CurrentThread, EventArgs.Empty);
                }
                
            }

            // Disables windows in preparation of going modal.  If parameter is true, we disable all
            // windows, if false, only windows forms windows (i.e., windows controlled by this MsoComponent).
            // See also IMsoComponent.OnEnterState.
            internal void DisableWindowsForModalLoop(bool onlyWinForms, ApplicationContext context) {
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : Entering modal state");
                ThreadWindows old = threadWindows;
                threadWindows = new ThreadWindows(onlyWinForms);
                threadWindows.Enable(false);
                threadWindows.previousThreadWindows = old;

                ModalApplicationContext modalContext = context as ModalApplicationContext;
                if (modalContext != null) {
                    modalContext.DisableThreadWindows(true, onlyWinForms);
                }
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.Dispose"]/*' />
            /// <devdoc>
            ///     Disposes this thread context object.  Note that this will marshal to the owning thread.
            /// </devdoc>
            /// <internalonly/>
            internal void Dispose(bool postQuit) {

                // need to avoid multiple threads coming in here or we'll leak the thread
                // handle.
                //
                lock (this) {
                    try {
                        if (disposeCount++ == 0) {  // make sure that we are not reentrant
                            // Unravel our message loop.  this will marshal us over to
                            // the right thread, making the dispose() method async.
                            if (messageLoopCount > 0 && postQuit) {
                                PostQuit();
                            }
                            else {
                                bool ourThread = SafeNativeMethods.GetCurrentThreadId() == id;

                                try {
                                    // We can only clean up if we're being called on our
                                    // own thread.
                                    //
                                    if (ourThread) {

                                        // If we had a component manager, detach from it.
                                        //
                                        if (componentManager != null) {
                                            RevokeComponent();
                                        }

                                       // DisposeAssociatedComponents();
                                        DisposeThreadWindows();

                                        try {
                                            Application.RaiseThreadExit();
                                        }
                                        finally {
                                            if (GetState(STATE_OLEINITIALIZED) && !GetState(STATE_EXTERNALOLEINIT)) {
                                                SetState(STATE_OLEINITIALIZED, false);
                                                UnsafeNativeMethods.OleUninitialize();
                                            }
                                        }
                                    }
                                }
                                finally {
                                    // We can always clean up this handle, though
                                    //
                                    if (handle != IntPtr.Zero) {
                                        UnsafeNativeMethods.CloseHandle(new HandleRef(this, handle));
                                        handle = IntPtr.Zero;
                                    }

                                    try {
                                        if (totalMessageLoopCount == 0) {
                                            Application.RaiseExit();
                                        }
                                    }
                                    finally {
                                        lock (tcInternalSyncObject) {
                                            contextHash.Remove((object)id);
                                        }
                                        if (currentThreadContext == this) {
                                            currentThreadContext = null;
                                        }
                                    }
                                }
                            }

                            GC.SuppressFinalize(this);
                        }
                    }
                    finally {
                        disposeCount--;
                    }
                }
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.DisposeParkingWindow"]/*' />
            /// <devdoc>
            ///     Disposes of this thread's parking form.
            /// </devdoc>
            /// <internalonly/>
            private void DisposeParkingWindow() {
                if (parkingWindows.Count != 0) {

                    // We take two paths here.  If we are on the same thread as
                    // the parking window, we can destroy its handle.  If not,
                    // we just null it and let it GC.  When it finalizes it
                    // will disconnect its handle and post a WM_CLOSE.
                    //
                    // It is important that we just call DestroyHandle here
                    // and do not call Dispose.  Otherwise we would destroy
                    // controls that are living on the parking window.
                    //
                    int pid;
                    int hwndThread = SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(parkingWindows[0], parkingWindows[0].Handle), out pid);
                    int currentThread = SafeNativeMethods.GetCurrentThreadId();

                    for(int i=0; i< parkingWindows.Count; i++) {
                        if (hwndThread == currentThread) {
                            parkingWindows[i].Destroy();
                        }
                        else {
                            parkingWindows[i] = null;
                        }
                    }
                    parkingWindows.Clear();
                }
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.DisposeThreadWindows"]/*' />
            /// <devdoc>
            ///     Gets rid of all windows in this thread context.  Nulls out
            ///     window objects that we hang on to.
            /// </devdoc>
            internal void DisposeThreadWindows() {

                // We dispose the main window first, so it can perform any
                // cleanup that it may need to do.
                //
                try {
                    if (applicationContext != null) {
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
                catch {
                }
            }

            // Enables windows in preparation of stopping modal.  If parameter is true, we enable all windows,
            // if false, only windows forms windows (i.e., windows controlled by this MsoComponent).
            // See also IMsoComponent.OnEnterState.
            internal void EnableWindowsForModalLoop(bool onlyWinForms, ApplicationContext context) {
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : Leaving modal state");
                if (threadWindows != null) {
                    threadWindows.Enable(true);
                    Debug.Assert(threadWindows != null, "OnEnterState recursed, but it's not supposed to be reentrant");
                    threadWindows = threadWindows.previousThreadWindows;
                }

                ModalApplicationContext modalContext = context as ModalApplicationContext;
                if (modalContext != null) {
                    modalContext.DisableThreadWindows(false, onlyWinForms);
                }
            }

            // Called immediately after we end pumping messages for a modal message loop.
            internal void EndModalMessageLoop(ApplicationContext context) {
#if DEBUG
                debugModalCounter--;
                Debug.Assert(debugModalCounter >= 0, "Mis-matched calls to Application.BeginModalMessageLoop() and Application.EndModalMessageLoop()");
#endif
                // This will re-enable the windows...
                EnableWindowsForModalLoop(false, context); // onlyWinForms = false

                bool wasOurLoop = ourModalLoop;
                ourModalLoop = true;
                try {

                    // If We started the ModalMessageLoop .. this will call us back on the IMSOComponent.OnStateEnter and not do anything ...
                    UnsafeNativeMethods.IMsoComponentManager cm = ComponentManager;
                    if (cm != null) {
                        cm.FOnComponentExitState((IntPtr)componentID, NativeMethods.MSOCM.msocstateModal, NativeMethods.MSOCM.msoccontextAll, 0, 0);
                    }
                }
                finally
                {
                    // Reset the flag since we are exiting out of a ModalMesaageLoop..
                    ourModalLoop = wasOurLoop;
                }

                modalCount--;

                if (leaveModalHandler != null && modalCount == 0) {
                    leaveModalHandler(Thread.CurrentThread, EventArgs.Empty);
                }
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.ExitApplication"]/*' />
            /// <devdoc>
            ///     Exits the program by disposing of all thread contexts and message loops.
            /// </devdoc>
            /// <internalonly/>
            internal static void ExitApplication() {
                ExitCommon(true /*disposing*/);
            }

            private static void ExitCommon(bool disposing) {
                lock(tcInternalSyncObject) {
                    if (contextHash != null) {
                        ThreadContext[] ctxs = new ThreadContext[contextHash.Values.Count];
                        contextHash.Values.CopyTo(ctxs, 0);
                        for (int i = 0; i < ctxs.Length; ++i) {
                            if (ctxs[i].ApplicationContext != null) {
                                ctxs[i].ApplicationContext.ExitThread();
                            }
                            else {
                                ctxs[i].Dispose(disposing);
                            }
                        }
                    }
                }
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.ExitDomain"]/*' />
            /// <devdoc>
            ///     Exits the program by disposing of all thread contexts and message loops.
            /// </devdoc>
            /// <internalonly/>
            internal static void ExitDomain() {
                ExitCommon(false /*disposing*/);
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.Finalize"]/*' />
            /// <devdoc>
            ///     Our finalization.  Minimal stuff... this shouldn't be called... We should always be disposed.
            /// </devdoc>
            /// <internalonly/>
            ~ThreadContext() {

                // We used to call OleUninitialize() here if we were
                // still STATE_OLEINITIALIZED, but that's never the correct thing to do.
                // At this point we're on the wrong thread and we should never have been
                // called here in the first place.

                // We can always clean up this handle, though
                //
                if (handle != IntPtr.Zero) {
                    UnsafeNativeMethods.CloseHandle(new HandleRef(this, handle));
                    handle = IntPtr.Zero;
                }
            }

            // When a Form receives a WM_ACTIVATE message, it calls this method so we can do the
            // appropriate MsoComponentManager activation magic
            internal void FormActivated(bool activate) {
                if (activate) {
                    UnsafeNativeMethods.IMsoComponentManager cm = ComponentManager;
                    if (cm != null && !(cm is ComponentManager)) {
                        cm.FOnComponentActivate((IntPtr)componentID);
                    }
                }
            }

            // Sets this component as the tracking component - trumping any active component 
            // for message filtering.
            internal void TrackInput(bool track) {

                // protect against double setting, as this causes asserts in the VS component manager.
                if (track != GetState(STATE_TRACKINGCOMPONENT)) {
                    UnsafeNativeMethods.IMsoComponentManager cm = ComponentManager;
                    if (cm != null && !(cm is ComponentManager)) {
                        cm.FSetTrackingComponent((IntPtr)componentID, track);
                        SetState(STATE_TRACKINGCOMPONENT, track);
                    }                    
                }
            }
            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.FromCurrent"]/*' />
            /// <devdoc>
            ///     Retrieves a ThreadContext object for the current thread
            /// </devdoc>
            /// <internalonly/>
            [ResourceExposure(ResourceScope.Process)]
            [ResourceConsumption(ResourceScope.Process)]
            internal static ThreadContext FromCurrent() {
                ThreadContext context = currentThreadContext;

                if (context == null) {
                    context = new ThreadContext();
                }

                return context;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.FromId"]/*' />
            /// <devdoc>
            ///     Retrieves a ThreadContext object for the given thread ID
            /// </devdoc>
            /// <internalonly/>
            [ResourceExposure(ResourceScope.Process)]
            [ResourceConsumption(ResourceScope.Process)]
            internal static ThreadContext FromId(int id) {
                ThreadContext context = (ThreadContext)contextHash[(object)id];
                if (context == null && id == SafeNativeMethods.GetCurrentThreadId()) {
                    context = new ThreadContext();
                }

                return context;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.GetAllowQuit"]/*' />
            /// <devdoc>
            ///      Determines if it is OK to allow an application to quit and shutdown
            ///      the runtime.  We only allow this if we own the base message pump.
            /// </devdoc>
            internal bool GetAllowQuit() {
                return totalMessageLoopCount > 0 && baseLoopReason == NativeMethods.MSOCM.msoloopMain;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.GetHandle"]/*' />
            /// <devdoc>
            ///     Retrieves the handle to this thread.
            /// </devdoc>
            /// <internalonly/>
            internal IntPtr GetHandle() {
                return handle;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.GetId"]/*' />
            /// <devdoc>
            ///     Retrieves the ID of this thread.
            /// </devdoc>
            /// <internalonly/>
            internal int GetId() {
                return id;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.GetCulture"]/*' />
            /// <devdoc>
            ///     Retrieves the culture for this thread.
            /// </devdoc>
            /// <internalonly/>
            [ResourceExposure(ResourceScope.AppDomain)]
            [ResourceConsumption(ResourceScope.AppDomain)]
            internal CultureInfo GetCulture() {
                if (culture == null || culture.LCID != SafeNativeMethods.GetThreadLocale())
                    culture = new CultureInfo(SafeNativeMethods.GetThreadLocale());
                return culture;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.GetMessageLoop"]/*' />
            /// <devdoc>
            ///     Determines if a message loop exists on this thread.
            /// </devdoc>
            internal bool GetMessageLoop() {
                return GetMessageLoop(false);
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.GetMessageLoop"]/*' />
            /// <devdoc>
            ///     Determines if a message loop exists on this thread.
            /// </devdoc>
            internal bool GetMessageLoop(bool mustBeActive) {

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
                if (ComponentManager != null && externalComponentManager) {
                    if (mustBeActive == false) {
                        return true;
                    }

                    UnsafeNativeMethods.IMsoComponent[] activeComponents = new UnsafeNativeMethods.IMsoComponent[1];
                    if (ComponentManager.FGetActiveComponent(NativeMethods.MSOCM.msogacActive, activeComponents, null, 0) &&
                        activeComponents[0] == this) {
                        return true;
                    }
                }

                // Finally, check if a message loop has been registered 
                MessageLoopCallback callback = messageLoopCallback;
                if (callback != null) {
                    return callback();
                }

                // Otherwise, we do not have a loop running.
                //
                return false;
            }

            private bool GetState(int bit) {
                return(threadState & bit) != 0;
            }

            /// <devdoc>
            ///     Keep the object alive forever.
            /// </devdoc>
            public override object InitializeLifetimeService() {
                return null;
            }

            /// <summary>
            /// A method of determining whether we are handling messages that does not demand register
            /// the componentmanager
            /// </summary>
            /// <returns></returns>
            internal bool IsValidComponentId() {
                return (componentID != INVALID_ID);
            }

            internal System.Threading.ApartmentState OleRequired() {
                Thread current = Thread.CurrentThread;
                if (!GetState(STATE_OLEINITIALIZED)) {

                    int ret = UnsafeNativeMethods.OleInitialize();

#if false
                    if (!(ret == NativeMethods.S_OK || ret == NativeMethods.S_FALSE || ret == NativeMethods.RPC_E_CHANGED_MODE)) {
                        Debug.Assert(ret == NativeMethods.S_OK || ret == NativeMethods.S_FALSE || ret == NativeMethods.RPC_E_CHANGED_MODE,
                                     "OLE Failed to Initialize!. RetCode: 0x" + Convert.ToString(ret, 16) +
                                     " LastError: " + Marshal.GetLastWin32Error().ToString());
                    }
#endif

                    SetState(STATE_OLEINITIALIZED, true);
                    if (ret == NativeMethods.RPC_E_CHANGED_MODE) {
                        // This could happen if the thread was already initialized for MTA
                        // and then we call OleInitialize which tries to initialized it for STA
                        // This currently happens while profiling...
                        SetState(STATE_EXTERNALOLEINIT, true);
                    }

                }

                if ( GetState( STATE_EXTERNALOLEINIT )) {
                    return System.Threading.ApartmentState.MTA;
                }
                else {
                    return System.Threading.ApartmentState.STA;
                }
            }

            private void OnAppThreadExit(object sender, EventArgs e) {
                Dispose(true);
            }

            /// <devdoc>
            ///     Revokes our component if needed.
            /// </devdoc>
            [PrePrepareMethod]
            private void OnDomainUnload(object sender, EventArgs e) {
                RevokeComponent();
                ExitDomain();
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.OnThreadException"]/*' />
            /// <devdoc>
            ///     Called when an untrapped exception occurs in a thread.  This allows the
            ///     programmer to trap these, and, if left untrapped, throws a standard error
            ///     dialog.
            /// </devdoc>
            /// <internalonly/>
            internal void OnThreadException(Exception t) {
                if (GetState(STATE_INTHREADEXCEPTION)) return;

                SetState(STATE_INTHREADEXCEPTION, true);
                try {
                    if (threadExceptionHandler != null) {
                        threadExceptionHandler(Thread.CurrentThread, new ThreadExceptionEventArgs(t));
                    }
                    else {
                        if (SystemInformation.UserInteractive) {
                            ThreadExceptionDialog td = new ThreadExceptionDialog(t);
                            DialogResult result = DialogResult.OK;

                            // 



                            IntSecurity.ModifyFocus.Assert();
                            try {
                                result = td.ShowDialog();
                            }
                            finally {
                                CodeAccessPermission.RevertAssert();
                                td.Dispose();
                            }
                            switch (result) {
                                case DialogResult.Abort:
                                    // 









                                    Application.ExitInternal();

                                    // 


                                    new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Assert();
                                    Environment.Exit(0);
                                    break;
                                case DialogResult.Yes:
                                    WarningException w = t as WarningException;
                                    if (w != null) {
                                        Help.ShowHelp(null, w.HelpUrl, w.HelpTopic);
                                    }
                                    break;
                            }
                        }
                        else {
                            // Ignore unhandled thread exceptions. The user can
                            // override if they really care.
                            //
                        }

                    }
                }
                finally {
                    SetState(STATE_INTHREADEXCEPTION, false);
                }
            }

            internal void PostQuit() {
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
                UnsafeNativeMethods.PostThreadMessage(id, NativeMethods.WM_QUIT, IntPtr.Zero, IntPtr.Zero);
                SetState(STATE_POSTEDQUIT, true);
            }

            // Allows the hosting environment to register a callback 
            internal void RegisterMessageLoop(MessageLoopCallback callback) {
                messageLoopCallback = callback;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.RemoveMessageFilter"]/*' />
            /// <devdoc>
            ///     Removes a message filter previously installed with addMessageFilter.
            /// </devdoc>
            /// <internalonly/>
            internal void RemoveMessageFilter(IMessageFilter f) {
                if (messageFilters != null) {
                    SetState(STATE_FILTERSNAPSHOTVALID, false);
                    messageFilters.Remove(f);
                }
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.RunMessageLoop"]/*' />
            /// <devdoc>
            ///     Starts a message loop for the given reason.
            /// </devdoc>
            /// <internalonly/>
            internal void RunMessageLoop(int reason, ApplicationContext context) {
                // Ensure that we attempt to apply theming before doing anything
                // that might create a window.

                IntPtr userCookie = IntPtr.Zero;
                if (useVisualStyles) {
                    userCookie = UnsafeNativeMethods.ThemingScope.Activate();
                }

                try {
                    RunMessageLoopInner(reason, context);                    
                }
                finally {
                    UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
                }
            }    

            private void RunMessageLoopInner(int reason, ApplicationContext context) {

                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ThreadContext.PushMessageLoop {");
                Debug.Indent();

                if (reason == NativeMethods.MSOCM.msoloopModalForm && !SystemInformation.UserInteractive) {
                    throw new InvalidOperationException(SR.CantShowModalOnNonInteractive);
                }

                // if we've entered because of a Main message loop being pushed
                // (different than a modal message loop or DoEVents loop)
                // then clear the QUIT flag to allow normal processing.
                // this flag gets set during loop teardown for another form.
                if (reason == NativeMethods.MSOCM.msoloopMain) {
                    SetState(STATE_POSTEDQUIT, false);
                }

                if (totalMessageLoopCount++ == 0) {
                    baseLoopReason = reason;
                }

                messageLoopCount++;

                if (reason == NativeMethods.MSOCM.msoloopMain) {
                    // If someone has tried to push another main message loop on this thread, ignore
                    // it.
                    if (messageLoopCount != 1) {
                        throw new InvalidOperationException(SR.CantNestMessageLoops);
                    }

                    applicationContext = context;

                    applicationContext.ThreadExit += new EventHandler(OnAppThreadExit);

                    if (applicationContext.MainForm != null) {
                        applicationContext.MainForm.Visible = true;
                    }

                    DpiHelper.InitializeDpiHelperForWinforms();

                    AccessibilityImprovements.ValidateLevels();
                }

                Form oldForm = currentForm;
                if (context != null) {
                    currentForm = context.MainForm;
                }

                bool fullModal = false;
                bool localModal = false;
                HandleRef hwndOwner = new HandleRef(null, IntPtr.Zero);

                if (reason == NativeMethods.MSOCM.msoloopDoEventsModal) {
                    localModal = true;
                }

                if (reason == NativeMethods.MSOCM.msoloopModalForm || reason == NativeMethods.MSOCM.msoloopModalAlert) {
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
                    if (hwndOwner.Handle != IntPtr.Zero) {
                        if (SafeNativeMethods.IsWindowEnabled(hwndOwner)) {
                            SafeNativeMethods.EnableWindow(hwndOwner, false);
                        }
                        else {
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
                        SafeNativeMethods.IsWindowEnabled(new HandleRef(currentForm, currentForm.Handle)) != modalEnabled) {
                        SafeNativeMethods.EnableWindow(new HandleRef(currentForm, currentForm.Handle), modalEnabled);
                    }
                }

                try {
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "[0x" + Convert.ToString(componentID, 16) + "] Calling ComponentManager.FPushMessageLoop...");
                    bool result;

                    // Register marshaller for background tasks.  At this point,
                    // need to be able to successfully get the handle to the
                    // parking window.  Only do it when we're entering the first
                    // message loop for this thread.
                    if (messageLoopCount == 1) {
                        WindowsFormsSynchronizationContext.InstallIfNeeded();
                    }

                    //need to do this in a try/finally.  Also good to do after we installed the synch context.
                    if (fullModal && currentForm != null) {
                        currentForm.Visible = true;
                    }

                    if ((!fullModal && !localModal) || ComponentManager is ComponentManager) {
                        result = ComponentManager.FPushMessageLoop((IntPtr)componentID, reason, 0);
                    }
                    else if (reason == NativeMethods.MSOCM.msoloopDoEvents ||
                             reason == NativeMethods.MSOCM.msoloopDoEventsModal) {
                        result = LocalModalMessageLoop(null);
                    }
                    else {
                        result = LocalModalMessageLoop(currentForm);
                    }

                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "[0x" + Convert.ToString(componentID, 16) + "] ComponentManager.FPushMessageLoop returned " + result.ToString());
                }
                finally {

                    if (fullModal) {
                        Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "[0x" + Convert.ToString(componentID, 16) + "] Notifying component manager that we are exiting a modal loop");
                        EndModalMessageLoop(context);

                        // Again, if the hwndOwner was valid and disabled above, re-enable it.
                        if (hwndOwner.Handle != IntPtr.Zero) {
                            SafeNativeMethods.EnableWindow(hwndOwner, true);
                        }
                    }

                    currentForm = oldForm;
                    totalMessageLoopCount--;
                    messageLoopCount--;

                    if (messageLoopCount == 0) {
                        // If last message loop shutting down, install the
                        // previous op sync context in place before we started the first
                        // message loop.
                        WindowsFormsSynchronizationContext.Uninstall(false);
                    }

                    if (reason == NativeMethods.MSOCM.msoloopMain) {
                        Dispose(true);
                    }
                    else if (messageLoopCount == 0 && componentManager != null) {
                        // If we had a component manager, detach from it.
                        //
                        RevokeComponent();
                    }
                }

                Debug.Unindent();
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "}");
            }

            private bool LocalModalMessageLoop(Form form) {
                try {
                    // Execute the message loop until the active component tells us to stop.
                    //
                    NativeMethods.MSG msg = new NativeMethods.MSG();
                    bool unicodeWindow = false;
                    bool continueLoop = true;

                    while (continueLoop) {

                        bool peeked = UnsafeNativeMethods.PeekMessage(ref msg, NativeMethods.NullHandleRef, 0, 0, NativeMethods.PM_NOREMOVE);

                        if (peeked) {

                            // If the component wants us to process the message, do it.
                            // The component manager hosts windows from many places.  We must be sensitive
                            // to ansi / Unicode windows here.
                            //
                            if (msg.hwnd != IntPtr.Zero && SafeNativeMethods.IsWindowUnicode(new HandleRef(null, msg.hwnd))) {
                                unicodeWindow = true;
                                if (!UnsafeNativeMethods.GetMessageW(ref msg, NativeMethods.NullHandleRef, 0, 0)) {
                                    continue;
                                }

                            }
                            else {
                                unicodeWindow = false;
                                if (!UnsafeNativeMethods.GetMessageA(ref msg, NativeMethods.NullHandleRef, 0, 0)) {
                                    continue;
                                }
                            }

                            if (!PreTranslateMessage(ref msg)) {
                                UnsafeNativeMethods.TranslateMessage(ref msg);
                                if (unicodeWindow) {
                                    UnsafeNativeMethods.DispatchMessageW(ref msg);
                                }
                                else {
                                    UnsafeNativeMethods.DispatchMessageA(ref msg);
                                }
                            }

                            if (form != null) {
                                continueLoop = !form.CheckCloseDialog(false);
                            }
                        }
                        else if (form == null) {
                            break;
                        }
                        else if (!UnsafeNativeMethods.PeekMessage(ref msg, NativeMethods.NullHandleRef, 0, 0, NativeMethods.PM_NOREMOVE)) {
                            UnsafeNativeMethods.WaitMessage();
                        }
                    }
                    return continueLoop;
                }
                catch {
                    return false;
                }
            }

            internal bool ProcessFilters(ref NativeMethods.MSG msg, out bool modified) {
                bool filtered = false;

                modified = false;

                // Account for the case where someone removes a message filter
                // as a result of PreFilterMessage.  the message filter will be 
                // removed from _the next_ message.
                // If message filter is added or removed inside the user-provided PreFilterMessage function,
                // and user code pumps messages, we might re-enter ProcessFilter on the same stack, we
                // should not update the snapshot until the next message.
                if (messageFilters != null && !GetState(STATE_FILTERSNAPSHOTVALID) && (LocalAppContextSwitches.DontSupportReentrantFilterMessage || inProcessFilters == 0)) {
                    if (messageFilters.Count > 0) {
                        messageFilterSnapshot = new IMessageFilter[messageFilters.Count];
                        messageFilters.CopyTo(messageFilterSnapshot);
                    }
                    else {
                        messageFilterSnapshot = null;
                    }
                    SetState(STATE_FILTERSNAPSHOTVALID, true);
                }

                inProcessFilters++;
                try {
                    if (messageFilterSnapshot != null) {
                        IMessageFilter f;
                        int count = messageFilterSnapshot.Length;

                        Message m = Message.Create(msg.hwnd, msg.message, msg.wParam, msg.lParam);

                        for (int i = 0; i < count; i++) {
                            f = (IMessageFilter)messageFilterSnapshot[i];
                            bool filterMessage = f.PreFilterMessage(ref m);
                            // make sure that we update the msg struct with the new result after the call to
                            // PreFilterMessage.
                            if (f is IMessageModifyAndFilter) {
                                msg.hwnd = m.HWnd;
                                msg.message = m.Msg;
                                msg.wParam = m.WParam;
                                msg.lParam = m.LParam;
                                modified = true;
                            }

                            if (filterMessage) {
                                filtered = true;
                                break;
                            }
                        }
                    }
                }
                finally {
                    inProcessFilters--;
                }

                return filtered;
            }


            /// <devdoc>
            ///     Message filtering routine that is called before dispatching a message.
            ///     If this returns true, the message is already processed.  If it returns
            ///     false, the message should be allowed to continue through the dispatch
            ///     mechanism.
            /// </devdoc>
            internal bool PreTranslateMessage(ref NativeMethods.MSG msg) {
                bool modified = false;
                if (ProcessFilters(ref msg, out modified)) {
                    return true;
                }
                
                if (msg.message >= NativeMethods.WM_KEYFIRST
                        && msg.message <= NativeMethods.WM_KEYLAST) {
                    if (msg.message == NativeMethods.WM_CHAR) {
                        int breakLParamMask = 0x1460000; // 1 = extended keyboard, 46 = scan code
                        if (unchecked( (int) (long)msg.wParam) == 3 && ( unchecked( (int) (long)msg.lParam) & breakLParamMask) == breakLParamMask) { // ctrl-brk
                            // wParam is the key character, which for ctrl-brk is the same as ctrl-C.
                            // So we need to go to the lparam to distinguish the two cases.
                            // You might also be able to do this with WM_KEYDOWN (again with wParam=3)

                            if (Debugger.IsAttached) {
                                Debugger.Break();
                            }
                        }
                    }
                    Control target = Control.FromChildHandleInternal(msg.hwnd);
                    bool retValue = false;

                    Message m = Message.Create(msg.hwnd, msg.message, msg.wParam, msg.lParam);

                    if (target != null)
                    {
                        if (NativeWindow.WndProcShouldBeDebuggable) {
                            // we don't want to do a catch in the debuggable case.
                            //
                            if (Control.PreProcessControlMessageInternal(target, ref m) == PreProcessControlState.MessageProcessed) {
                                retValue = true;
                            }                            
                        }
                        else {
                            try
                            {
                                if (Control.PreProcessControlMessageInternal(target, ref m) == PreProcessControlState.MessageProcessed) {
                                    retValue = true;
                                }
                            }
                            catch (Exception e) {
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

                    if (retValue) {
                        return true;
                    }
                }

                return false;
            }

            /// <devdoc>
            ///     Revokes our component from the active component manager.  Does
            ///     nothing if there is no active component manager or we are
            ///     already invoked.
            /// </devdoc>
            private void RevokeComponent() {
                if (componentManager != null && componentID != INVALID_ID) {
                    int id = componentID;
                    UnsafeNativeMethods.IMsoComponentManager msocm = componentManager;

                    try {
                        msocm.FRevokeComponent((IntPtr)id);
                        if (Marshal.IsComObject(msocm)) {
                            Marshal.ReleaseComObject(msocm);
                        }
                    }
                    finally {
                        componentManager = null;
                        componentID = INVALID_ID;
                    }
                }
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.SetCulture"]/*' />
            /// <devdoc>
            ///     Sets the culture for this thread.
            /// </devdoc>
            /// <internalonly/>
            [ResourceExposure(ResourceScope.AppDomain)]
            [ResourceConsumption(ResourceScope.AppDomain)]
            internal void SetCulture(CultureInfo culture) {
                if (culture != null && culture.LCID != SafeNativeMethods.GetThreadLocale()) {
                    SafeNativeMethods.SetThreadLocale(culture.LCID);
                }
            }

            private void SetState(int bit, bool value) {
                if (value) {
                    threadState |= bit;
                }
                else {
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

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.UnsafeNativeMethods.IMsoComponent.FDebugMessage"]/*' />
            /// <devdoc>
            ///      Standard FDebugMessage method.
            ///      Since IMsoComponentManager is a reference counted interface,
            ///      MsoDWGetChkMemCounter should be used when processing the
            ///      msodmWriteBe message.
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponent.FDebugMessage(IntPtr hInst, int msg, IntPtr wparam, IntPtr lparam)
            {

                return false;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.UnsafeNativeMethods.IMsoComponent.FPreTranslateMessage"]/*' />
            /// <devdoc>
            ///      Give component a chance to process the message pMsg before it is
            ///      translated and dispatched. Component can do TranslateAccelerator
            ///      do IsDialogMessage, modify pMsg, or take some other action.
            ///      Return TRUE if the message is consumed, FALSE otherwise.
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponent.FPreTranslateMessage(ref NativeMethods.MSG msg) {
                return PreTranslateMessage(ref msg);
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.UnsafeNativeMethods.IMsoComponent.OnEnterState"]/*' />
            /// <devdoc>
            ///      Notify component when app enters or exits (as indicated by fEnter)
            ///      the state identified by uStateID (a value from olecstate enumeration).
            ///      Component should take action depending on value of uStateID
            ///       (see olecstate comments, above).
            ///
            ///      Note: If n calls are made with TRUE fEnter, component should consider
            ///      the state to be in effect until n calls are made with FALSE fEnter.
            ///
            ///     Note: Components should be aware that it is possible for this method to
            ///     be called with FALSE fEnter more    times than it was called with TRUE
            ///     fEnter (so, for example, if component is maintaining a state counter
            ///     (incremented when this method is called with TRUE fEnter, decremented
            ///     when called with FALSE fEnter), the counter should not be decremented
            ///     for FALSE fEnter if it is already at zero.)
            /// </devdoc>
            void UnsafeNativeMethods.IMsoComponent.OnEnterState(int uStateID, bool fEnter) {

                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : OnEnterState(" + uStateID + ", " + fEnter + ")");

                // Return if our (WINFORMS) Modal Loop is still running.
                if (ourModalLoop)
                {
                    return;
                }
                if (uStateID == NativeMethods.MSOCM.msocstateModal) {
                    // We should only be messing with windows we own.  See the "ctrl-shift-N" test above.
                    if (fEnter) {
                        DisableWindowsForModalLoop(true, null); // WinFormsOnly = true
                    }
                    else {
                        EnableWindowsForModalLoop(true, null); // WinFormsOnly = true
                    }
                }
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.UnsafeNativeMethods.IMsoComponent.OnAppActivate"]/*' />
            /// <devdoc>
            ///      Notify component when the host application gains or loses activation.
            ///      If fActive is TRUE, the host app is being activated and dwOtherThreadID
            ///      is the ID of the thread owning the window being deactivated.
            ///      If fActive is FALSE, the host app is being deactivated and
            ///      dwOtherThreadID is the ID of the thread owning the window being
            ///      activated.
            ///      Note: this method is not called when both the window being activated
            ///      and the one being deactivated belong to the host app.
            /// </devdoc>
            void UnsafeNativeMethods.IMsoComponent.OnAppActivate(bool fActive, int dwOtherThreadID) {
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.UnsafeNativeMethods.IMsoComponent.OnLoseActivation"]/*' />
            /// <devdoc>
            ///      Notify the active component that it has lost its active status because
            ///      the host or another component has become active.
            /// </devdoc>
            void UnsafeNativeMethods.IMsoComponent.OnLoseActivation() {
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : Our component is losing activation.");
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.UnsafeNativeMethods.IMsoComponent.OnActivationChange"]/*' />
            /// <devdoc>
            ///      Notify component when a new object is being activated.
            ///      If pic is non-NULL, then it is the component that is being activated.
            ///      In this case, fSameComponent is TRUE if pic is the same component as
            ///      the callee of this method, and pcrinfo is the reg info of pic.
            ///      If pic is NULL and fHostIsActivating is TRUE, then the host is the
            ///      object being activated, and pchostinfo is its host info.
            ///      If pic is NULL and fHostIsActivating is FALSE, then there is no current
            ///      active object.
            ///
            ///      If pic is being activated and pcrinfo->grf has the
            ///      olecrfExclusiveBorderSpace bit set, component should hide its border
            ///      space tools (toolbars, status bars, etc.);
            ///      component should also do this if host is activating and
            ///      pchostinfo->grfchostf has the olechostfExclusiveBorderSpace bit set.
            ///      In either of these cases, component should unhide its border space
            ///      tools the next time it is activated.
            ///
            ///      if pic is being activated and pcrinfo->grf has the
            ///      olecrfExclusiveActivation bit is set, then pic is being activated in
            ///      "ExclusiveActive" mode.
            ///      Component should retrieve the top frame window that is hosting pic
            ///      (via pic->HwndGetWindow(olecWindowFrameToplevel, 0)).
            ///      If this window is different from component's own top frame window,
            ///         component should disable its windows and do other things it would do
            ///         when receiving OnEnterState(olecstateModal, TRUE) notification.
            ///      Otherwise, if component is top-level,
            ///         it should refuse to have its window activated by appropriately
            ///         processing WM_MOUSEACTIVATE (but see WM_MOUSEACTIVATE NOTE, above).
            ///      Component should remain in one of these states until the
            ///      ExclusiveActive mode ends, indicated by a future call to
            ///      OnActivationChange with ExclusiveActivation bit not set or with NULL
            ///      pcrinfo.
            /// </devdoc>
            void UnsafeNativeMethods.IMsoComponent.OnActivationChange(UnsafeNativeMethods.IMsoComponent component, bool fSameComponent,
                                                  int pcrinfo,
                                                  bool fHostIsActivating,
                                                  int pchostinfo,
                                                  int dwReserved) {
                Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : OnActivationChange");
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.UnsafeNativeMethods.IMsoComponent.FDoIdle"]/*' />
            /// <devdoc>
            ///      Give component a chance to do idle time tasks.  grfidlef is a group of
            ///      bit flags taken from the enumeration of oleidlef values (above),
            ///      indicating the type of idle tasks to perform.
            ///      Component may periodically call IOleComponentManager::FContinueIdle;
            ///      if this method returns FALSE, component should terminate its idle
            ///      time processing and return.
            ///      Return TRUE if more time is needed to perform the idle time tasks,
            ///      FALSE otherwise.
            ///      Note: If a component reaches a point where it has no idle tasks
            ///      and does not need FDoIdle calls, it should remove its idle task
            ///      registration via IOleComponentManager::FUpdateComponentRegistration.
            ///      Note: If this method is called on while component is performing a
            ///      tracking operation, component should only perform idle time tasks that
            ///      it deems are appropriate to perform during tracking.
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponent.FDoIdle(int grfidlef) {
                 if (idleHandler != null) {
                     idleHandler(Thread.CurrentThread, EventArgs.Empty);
                 }
                 return false;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.UnsafeNativeMethods.IMsoComponent.FContinueMessageLoop"]/*' />
            /// <devdoc>
            ///      Called during each iteration of a message loop that the component
            ///      pushed. uReason and pvLoopData are the reason and the component private
            ///      data that were passed to IOleComponentManager::FPushMessageLoop.
            ///      This method is called after peeking the next message in the queue
            ///      (via PeekMessage) but before the message is removed from the queue.
            ///      The peeked message is passed in the pMsgPeeked param (NULL if no
            ///      message is in the queue).  This method may be additionally called when
            ///      the next message has already been removed from the queue, in which case
            ///      pMsgPeeked is passed as NULL.
            ///      Return TRUE if the message loop should continue, FALSE otherwise.
            ///      If FALSE is returned, the component manager terminates the loop without
            ///      removing pMsgPeeked from the queue.
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponent.FContinueMessageLoop(int reason, int pvLoopData, NativeMethods.MSG[] msgPeeked) {

                bool continueLoop = true;

                // If we get a null message, and we have previously posted the WM_QUIT message,
                // then someone ate the message...
                //
                if (msgPeeked == null && GetState(STATE_POSTEDQUIT)) {
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : Abnormal loop termination, no WM_QUIT received");
                    continueLoop = false;
                }
                else {
                    switch (reason) {
                        case NativeMethods.MSOCM.msoloopFocusWait:

                            // For focus wait, check to see if we are now the active application.
                            //
                            int pid;
                            SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(null, UnsafeNativeMethods.GetActiveWindow()), out pid);
                            if (pid == SafeNativeMethods.GetCurrentProcessId()) {
                                continueLoop = false;
                            }
                            break;

                        case NativeMethods.MSOCM.msoloopModalAlert:
                        case NativeMethods.MSOCM.msoloopModalForm:

                            // For modal forms, check to see if the current active form has been
                            // dismissed.  If there is no active form, then it is an error that
                            // we got into here, so we terminate the loop.
                            //
                            if (currentForm == null || currentForm.CheckCloseDialog(false)) {
                                continueLoop = false;
                            }
                            break;

                        case NativeMethods.MSOCM.msoloopDoEvents:
                        case NativeMethods.MSOCM.msoloopDoEventsModal:
                            // For DoEvents, just see if there are more messages on the queue.
                            //
                            if (!UnsafeNativeMethods.PeekMessage(ref tempMsg, NativeMethods.NullHandleRef, 0, 0, NativeMethods.PM_NOREMOVE)) {
                                continueLoop = false;
                            }

                            break;
                    }
                }

                return continueLoop;
            }


            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.UnsafeNativeMethods.IMsoComponent.FQueryTerminate"]/*' />
            /// <devdoc>
            ///      Called when component manager wishes to know if the component is in a
            ///      state in which it can terminate.  If fPromptUser is FALSE, component
            ///      should simply return TRUE if it can terminate, FALSE otherwise.
            ///      If fPromptUser is TRUE, component should return TRUE if it can
            ///      terminate without prompting the user; otherwise it should prompt the
            ///      user, either 1.) asking user if it can terminate and returning TRUE
            ///      or FALSE appropriately, or 2.) giving an indication as to why it
            ///      cannot terminate and returning FALSE.
            /// </devdoc>
            bool UnsafeNativeMethods.IMsoComponent.FQueryTerminate(bool fPromptUser) {
                return true;
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.UnsafeNativeMethods.IMsoComponent.Terminate"]/*' />
            /// <devdoc>
            ///      Called when component manager wishes to terminate the component's
            ///      registration.  Component should revoke its registration with component
            ///      manager, release references to component manager and perform any
            ///      necessary cleanup.
            /// </devdoc>
            void UnsafeNativeMethods.IMsoComponent.Terminate() {
                if (this.messageLoopCount > 0 && !(ComponentManager is ComponentManager)) {
                    this.messageLoopCount--;
                }

                Dispose(false);
            }

            /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadContext.UnsafeNativeMethods.IMsoComponent.HwndGetWindow"]/*' />
            /// <devdoc>
            ///      Called to retrieve a window associated with the component, as specified
            ///      by dwWhich, a olecWindowXXX value (see olecWindow, above).
            ///      dwReserved is reserved for future use and should be zero.
            ///      Component should return the desired window or NULL if no such window
            ///      exists.
            /// </devdoc>
            IntPtr UnsafeNativeMethods.IMsoComponent.HwndGetWindow(int dwWhich, int dwReserved) {
                return IntPtr.Zero;
            }
        }

        /// <devdoc>
        ///     This class allows us to handle sends/posts in our winformssynchcontext on the correct thread via
        ///  control.invoke().
        /// </devdoc>
        /// <internalonly/>
        internal sealed class MarshalingControl : Control {
            internal MarshalingControl()
                : base(false) {
                Visible = false;
                SetState2(STATE2_INTERESTEDINUSERPREFERENCECHANGED, false);
                SetTopLevel(true);
                CreateControl();
                CreateHandle();
            }

            protected override CreateParams CreateParams {
                get {
                    CreateParams cp = base.CreateParams;

                    // Message only windows are cheaper and have fewer issues than
                    // full blown invisible windows.  But, they are only supported
                    // on NT.
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                        cp.Parent = (IntPtr)NativeMethods.HWND_MESSAGE;
                    }
                    return cp;
                }
            }

            protected override void OnLayout(LayoutEventArgs levent) {
            }

            protected override void OnSizeChanged(EventArgs e) {

                // don't do anything here -- small perf game of avoiding layout, etc.
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ParkingWindow"]/*' />
        /// <devdoc>
        ///     This class embodies our parking window, which we create when the
        ///     first message loop is pushed onto the thread.
        /// </devdoc>
        /// <internalonly/>
        internal sealed class ParkingWindow : ContainerControl, IArrangedElement {

            // WHIDBEY CHANGES
            //   in whidbey we now aggressively tear down the parking window
            //   when the last control has been removed off of it.  

            private const int WM_CHECKDESTROY = NativeMethods.WM_USER + 0x01;

            private int childCount = 0;

            [
                SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // The parking window is invisible.
                                                                                                            // So we don't have to localize its Text.
            ]
            public ParkingWindow() {
                SetState2(STATE2_INTERESTEDINUSERPREFERENCECHANGED, false);
                SetState(STATE_TOPLEVEL, true);
                Text = "WindowsFormsParkingWindow";
                Visible = false;
            }

            protected override CreateParams CreateParams {
                get {
                    CreateParams cp = base.CreateParams;

                    // Message only windows are cheaper and have fewer issues than
                    // full blown invisible windows.  But, they are only supported
                    // on NT.
                    if (Environment.OSVersion.Platform == PlatformID.Win32NT) {
                        cp.Parent = (IntPtr)NativeMethods.HWND_MESSAGE;
                    }
                    return cp;
                }
            }

            internal override void AddReflectChild() {
                if (childCount < 0) {
                    Debug.Fail("How did parkingwindow childcount go negative???");
                    childCount = 0;
                }
                childCount++;
            }

            internal override void RemoveReflectChild() {
                childCount--;
                if (childCount < 0) {
                    Debug.Fail("How did parkingwindow childcount go negative???");
                    childCount = 0;
                }
                if (childCount == 0) {
                    if (IsHandleCreated) {
                        //check to see if we are running on the thread that owns the parkingwindow.
                        //if so, we can destroy immediately.
                        //This is important for scenarios where apps leak controls until after the
                        //messagepump is gone and then decide to clean them up.  We should clean
                        //up the parkingwindow in this case and a postmessage won't do it.
                        int lpdwProcessId;  //unused
                        int id = SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(this, HandleInternal), out lpdwProcessId);
                        Application.ThreadContext ctx = Application.ThreadContext.FromId(id);

                        //We only do this if the ThreadContext tells us that we are currently
                        //handling a window message.
                        if (ctx == null ||
                            !Object.ReferenceEquals(ctx, Application.ThreadContext.FromCurrent())) {
                            UnsafeNativeMethods.PostMessage(new HandleRef(this, HandleInternal), WM_CHECKDESTROY, IntPtr.Zero, IntPtr.Zero);
                        }
                        else {
                            CheckDestroy();
                        }
                    }
                }
            }
            
            private void CheckDestroy() {
                if (childCount == 0) {
                    IntPtr hwndChild = UnsafeNativeMethods.GetWindow(new HandleRef(this, Handle), NativeMethods.GW_CHILD);
                    if (hwndChild == IntPtr.Zero) {
                        DestroyHandle();
                    }
                }
            }

            public void Destroy() {
                DestroyHandle();
            }

            /// <devdoc>
            ///     "Parks" the given HWND to a temporary HWND.  This allows WS_CHILD windows to
            ///     be parked.
            /// </devdoc>
            internal void ParkHandle(HandleRef handle) {
                if (!IsHandleCreated) {
                    CreateHandle();
                }

                UnsafeNativeMethods.SetParent(handle, new HandleRef(this, Handle));
            }

            /// <devdoc>
            ///     "Unparks" the given HWND to a temporary HWND.  This allows WS_CHILD windows to
            ///     be parked.
            /// </devdoc>
            internal void UnparkHandle(HandleRef handle) {
                if (IsHandleCreated) {
                    Debug.Assert(UnsafeNativeMethods.GetParent(handle) != Handle, "Always set the handle's parent to someone else before calling UnparkHandle");
                    // If there are no child windows in this handle any longer, destroy the parking window.
                    CheckDestroy();
                }
            }

            // Do nothing on layout to reduce the calls into the LayoutEngine while debugging.
            protected override void OnLayout(LayoutEventArgs levent) {}
            void IArrangedElement.PerformLayout(IArrangedElement affectedElement, string affectedProperty) {}

            protected override void WndProc(ref Message m) {
                if (m.Msg != NativeMethods.WM_SHOWWINDOW) {
                    base.WndProc(ref m);
                    if (m.Msg == NativeMethods.WM_PARENTNOTIFY) {
                        if (NativeMethods.Util.LOWORD(unchecked( (int) (long)m.WParam)) == NativeMethods.WM_DESTROY) {
                            UnsafeNativeMethods.PostMessage(new HandleRef(this, Handle), WM_CHECKDESTROY, IntPtr.Zero, IntPtr.Zero);
                        }
                    }
                    else if (m.Msg == WM_CHECKDESTROY) {
                        CheckDestroy();
                    }
                }
            }
        }

        /// <include file='doc\Application.uex' path='docs/doc[@for="Application.ThreadWindows"]/*' />
        /// <devdoc>
        ///     This class enables or disables all windows in the current thread.  We use this to
        ///     disable other windows on the thread when a modal dialog is to be shown.  It can also
        ///     be used to dispose all windows in a thread, which we do before returning from a message
        ///     loop.
        /// </devdoc>
        /// <internalonly/>
        private sealed class ThreadWindows {
            private IntPtr[] windows;
            private int windowCount;
            private IntPtr activeHwnd;
            private IntPtr focusedHwnd;
            internal ThreadWindows previousThreadWindows;
            private bool onlyWinForms = true;

            internal ThreadWindows(bool onlyWinForms) {
                windows = new IntPtr[16];
                this.onlyWinForms = onlyWinForms;
                UnsafeNativeMethods.EnumThreadWindows(SafeNativeMethods.GetCurrentThreadId(),
                                                new NativeMethods.EnumThreadWindowsCallback(this.Callback),
                                                NativeMethods.NullHandleRef);
            }

            private bool Callback(IntPtr hWnd, IntPtr lparam) {

                // We only do visible and enabled windows.  Also, we only do top level windows.
                // Finally, we only include windows that are DNA windows, since other MSO components
                // will be responsible for disabling their own windows.
                //
                if (SafeNativeMethods.IsWindowVisible(new HandleRef(null, hWnd)) && SafeNativeMethods.IsWindowEnabled(new HandleRef(null, hWnd))) {
                    bool add = true;

                    if (onlyWinForms) {
                        Control c = Control.FromHandleInternal(hWnd);
                        if (c == null) add = false;
                    }

                    if (add) {
                        if (windowCount == windows.Length) {
                            IntPtr[] newWindows = new IntPtr[windowCount * 2];
                            Array.Copy(windows, 0, newWindows, 0, windowCount);
                            windows = newWindows;
                        }
                        windows[windowCount++] = hWnd;
                    }
                }
                return true;
            }

            // Disposes all top-level Controls on this thread
            internal void Dispose() {
                for (int i = 0; i < windowCount; i++) {
                    IntPtr hWnd = windows[i];
                    if (UnsafeNativeMethods.IsWindow(new HandleRef(null, hWnd))) {
                        Control c = Control.FromHandleInternal(hWnd);
                        if (c != null) {
                            c.Dispose();
                        }
                    }
                }
            }

            // Enables/disables all top-level Controls on this thread
            internal void Enable(bool state) {

                if (!onlyWinForms && !state) {
                    activeHwnd = UnsafeNativeMethods.GetActiveWindow();
                    Control activatingControl = Application.ThreadContext.FromCurrent().ActivatingControl;
                    if (activatingControl != null) {
                        focusedHwnd = activatingControl.Handle;
                    }
                    else {
                        focusedHwnd = UnsafeNativeMethods.GetFocus();
                    }
                }

                for (int i = 0; i < windowCount; i++) {
                    IntPtr hWnd = windows[i];
                    Debug.WriteLineIf(CompModSwitches.MSOComponentManager.TraceInfo, "ComponentManager : Changing enabled on window: " + hWnd.ToString() + " : " + state.ToString());
                    if (UnsafeNativeMethods.IsWindow(new HandleRef(null, hWnd))) SafeNativeMethods.EnableWindow(new HandleRef(null, hWnd), state);
                }

                // OpenFileDialog is not returning the focus the way other dialogs do.
                // Important that we re-activate the old window when we are closing
                // our modal dialog.
                //
                // edit mode forever with Excel application
                // But, DON'T change other people's state when we're simply
                // responding to external MSOCM events about modality.  When we are,
                // we are created with a TRUE for onlyWinForms.
                if (!onlyWinForms && state) {
                    if (activeHwnd != IntPtr.Zero && UnsafeNativeMethods.IsWindow(new HandleRef(null, activeHwnd))) {
                        UnsafeNativeMethods.SetActiveWindow(new HandleRef(null, activeHwnd));
                    }

                    if (focusedHwnd != IntPtr.Zero && UnsafeNativeMethods.IsWindow(new HandleRef(null, focusedHwnd))) {
                        UnsafeNativeMethods.SetFocus(new HandleRef(null, focusedHwnd));
                    }
                }
            }
        }

        private class ModalApplicationContext : ApplicationContext {

            private ThreadContext parentWindowContext;

            private delegate void ThreadWindowCallback(ThreadContext context, bool onlyWinForms);
            
            public ModalApplicationContext(Form modalForm) : base(modalForm) {
                }

            public void DisableThreadWindows(bool disable, bool onlyWinForms) {

                Control parentControl = null;
                
                // Get ahold of the parent HWND -- if it's a different thread we need to do 
                // do the disable over there too.  Note we only do this if we're parented by a Windows Forms
                // parent.
                //
                if (MainForm != null && MainForm.IsHandleCreated) {

                    // get ahold of the parenting control
                    //
                    IntPtr parentHandle = UnsafeNativeMethods.GetWindowLong(new HandleRef(this, MainForm.Handle), NativeMethods.GWL_HWNDPARENT);

                    parentControl = Control.FromHandleInternal(parentHandle);

                    if (parentControl != null && parentControl.InvokeRequired) {
                        parentWindowContext = GetContextForHandle(new HandleRef(this, parentHandle));                            
                    }
                    else {
                        parentWindowContext = null;
                    }
                }

                // if we got a thread context, that means our parent is in a different thread, make the call on that thread.
                //
                if (parentWindowContext != null) {

                    // in case we've already torn down, ask the context for this.
                    //
                    if (parentControl == null) {

                        parentControl = parentWindowContext.ApplicationContext.MainForm;
                    }
                    
                    if (disable) {
                        parentControl.Invoke(new ThreadWindowCallback(DisableThreadWindowsCallback), new object[]{parentWindowContext, onlyWinForms});
                    }
                    else {
                        parentControl.Invoke(new ThreadWindowCallback(EnableThreadWindowsCallback), new object[]{parentWindowContext, onlyWinForms});
                    }                    
                }
            }

            private void DisableThreadWindowsCallback(ThreadContext context, bool onlyWinForms) {
                context.DisableWindowsForModalLoop(onlyWinForms, this);                
            }

            private void EnableThreadWindowsCallback(ThreadContext context, bool onlyWinForms) {
                context.EnableWindowsForModalLoop(onlyWinForms, this);                
            }


            protected override void ExitThreadCore() {
                // do nothing... modal dialogs exit by setting dialog result
            }
        }

    }
}


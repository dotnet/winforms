// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms.Analyzers.Diagnostics;
using System.Windows.Forms.VisualStyles;
using Microsoft.Office;
using Microsoft.Win32;
using Directory = System.IO.Directory;

namespace System.Windows.Forms;

/// <summary>
///  Provides <see langword="static"/> methods and properties to manage an application, such as methods
///  to run and quit an application, to process Windows messages, and properties to get information about an application.
///  This class cannot be inherited.
/// </summary>
public sealed partial class Application
{
    /// <summary>
    ///  Hash table for our event list
    /// </summary>
    private static EventHandlerList? s_eventHandlers;
    private static Font? s_defaultFont;
    /// <summary>
    ///  Scaled version of non system <see cref="s_defaultFont"/>.
    /// </summary>
    private static Font? s_defaultFontScaled;
    private static string? s_startupPath;
    private static string? s_executablePath;
    private static FileVersionInfo? s_appFileVersion;
    private static Type? s_mainType;
    private static string? s_companyName;
    private static string? s_productName;
    private static string? s_productVersion;
    private static string? s_safeTopLevelCaptionSuffix;
    private static bool s_comCtlSupportsVisualStylesInitialized;
    private static bool s_comCtlSupportsVisualStyles;
    private static FormCollection? s_forms;
    private static readonly Lock s_internalSyncObject = new();
    private static bool s_useWaitCursor;

#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    private static SystemColorMode? s_colorMode;
#pragma warning restore WFO5001

    private const string DarkModeKeyPath = "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Themes\\Personalize";
    private const string DarkModeKey = "AppsUseLightTheme";
    private const int SystemDarkModeDisabled = 1;

    /// <summary>
    ///  Events the user can hook into
    /// </summary>
    private static readonly object s_eventApplicationExit = new();
    private static readonly object s_eventThreadExit = new();

    // Defines a new callback delegate type
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public delegate bool MessageLoopCallback();

    // Used to avoid recursive exit
    private static bool s_exiting;

    private static bool s_parkingWindowCreated;

    /// <summary>
    ///  This class is static, there is no need to ever create it.
    /// </summary>
    private Application()
    {
    }

    /// <summary>
    ///  Determines if the caller should be allowed to quit the application. This will return false,
    ///  for example, if being called from a windows forms control being hosted within a web browser. The
    ///  windows forms control should not attempt to quit the application.
    /// </summary>
    public static bool AllowQuit => ThreadContext.GetAllowQuit();

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

    private static unsafe bool InitializeComCtlSupportsVisualStyles()
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
        var hModule = PInvoke.GetModuleHandle(Libraries.Comctl32);
        fixed (byte* ptr = "ImageList_WriteEx\0"u8)
        {
            if (!hModule.IsNull)
            {
                return PInvoke.GetProcAddress(hModule, (PCSTR)ptr) != 0;
            }
        }

        // Load comctl since GetModuleHandle failed to find it
        nint ninthModule = PInvoke.LoadComctl32(StartupPath);
        if (ninthModule == 0)
        {
            return false;
        }

        fixed (byte* ptr = "ImageList_WriteEx\0"u8)
        {
            return PInvoke.GetProcAddress(hModule, (PCSTR)ptr) != 0;
        }
    }

    /// <summary>
    ///  Gets the registry key for the application data that is shared among all users.
    /// </summary>
    public static RegistryKey CommonAppDataRegistry
        => Registry.LocalMachine.CreateSubKey(CommonAppDataRegistryKeyName);

    internal static string CommonAppDataRegistryKeyName
        => $"Software\\{CompanyName}\\{ProductName}\\{ProductVersion}";

    /// <summary>
    ///  Gets the path for the application data that is shared among all users.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Don't obsolete these. GetDataPath isn't on SystemInformation, and it provides
    ///   the Windows logo required adornments to the directory (Company\Product\Version).
    ///  </para>
    /// </remarks>
    public static string CommonAppDataPath
        => GetDataPath(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData));

    /// <summary>
    ///  Gets the company name associated with the application.
    /// </summary>
    public static string? CompanyName
    {
        get
        {
            lock (s_internalSyncObject)
            {
                if (s_companyName is null)
                {
                    // Custom attribute
                    Assembly? entryAssembly = Assembly.GetEntryAssembly();
                    if (entryAssembly is not null)
                    {
                        object[] attrs = entryAssembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                        if (attrs is not null && attrs.Length > 0)
                        {
                            s_companyName = ((AssemblyCompanyAttribute)attrs[0]).Company;
                        }
                    }

                    // Win32 version
                    if (s_companyName is null || s_companyName.Length == 0)
                    {
                        s_companyName = GetAppFileVersionInfo().CompanyName;
                        if (s_companyName is not null)
                        {
                            s_companyName = s_companyName.Trim();
                        }
                    }

                    // fake it with a namespace
                    // won't work with MC++ see GetAppMainType.
                    if (s_companyName is null || s_companyName.Length == 0)
                    {
                        Type? type = GetAppMainType();

                        if (type is not null)
                        {
                            string? ns = type.Namespace;

                            if (!string.IsNullOrEmpty(ns))
                            {
                                int firstDot = ns.IndexOf('.');
                                s_companyName = firstDot != -1 ? ns[..firstDot] : ns;
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
    ///  Gets the default color mode (dark mode) for the application.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is the <see cref="SystemColorMode"/> which either has been set by <see cref="SetColorMode(SystemColorMode)"/>
    ///   or its default value <see cref="SystemColorMode.Classic"/>. If it has been set to <see cref="SystemColorMode.System"/>,
    ///   then the actual color mode is determined by the system settings (which can be retrieved by the
    ///   static (shared in VB) <see cref="SystemColorMode"/> property.
    ///  </para>
    /// </remarks>
    [Experimental(DiagnosticIDs.ExperimentalDarkMode, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static SystemColorMode ColorMode => s_colorMode ?? SystemColorMode.Classic;

    /// <summary>
    ///  True if the <see cref="ColorMode"/> has been set at least once.
    /// </summary>
    internal static bool ColorModeSet => s_colorMode is not null;

    /// <summary>
    ///  Sets the default color mode (dark mode) for the application.
    /// </summary>
    /// <param name="systemColorMode">The application's default color mode (dark mode) to set.</param>
    /// <remarks>
    ///  <para>
    ///   You should use this method to set the default color mode (dark mode) for the application. Set it,
    ///   before creating any UI elements, to ensure that the correct color mode is used. You can set it to
    ///   dark mode (<see cref="SystemColorMode.Dark"/>), light mode (<see cref="SystemColorMode.Classic"/>)
    ///   or to the system setting (<see cref="SystemColorMode.System"/>).
    ///  </para>
    ///  <para>
    ///   If you set it to <see cref="SystemColorMode.System"/>, the actual color mode is determined by the
    ///   Windows system settings. If the system setting is changed, the application will not automatically
    ///   adapt to the new setting.
    ///  </para>
    ///  <para>
    ///   Note that the dark color mode is only available from Windows 11 on or later versions. If the system
    ///   is set to a high contrast mode, the dark mode is not available.
    ///  </para>
    ///  <para>
    ///   <b>Note for Visual Basic:</b> If you are using the Visual Basic Application Framework, you should set the
    ///   color mode by handling the Application Events (see "WindowsFormsApplicationBase.ApplyApplicationDefaults").
    ///  </para>
    /// </remarks>
    [Experimental(DiagnosticIDs.ExperimentalDarkMode, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static void SetColorMode(SystemColorMode systemColorMode)
    {
        try
        {
            // Can't use the Generator here, since it cannot deal with [Experimental].
            _ = systemColorMode switch
            {
                SystemColorMode.Classic => systemColorMode,
                SystemColorMode.System => systemColorMode,
                SystemColorMode.Dark => systemColorMode,
                _ => throw new ArgumentOutOfRangeException(nameof(systemColorMode))
            };

            if (systemColorMode == s_colorMode)
            {
                return;
            }

            s_colorMode = systemColorMode;
        }
        finally
        {
            bool useAlternateColorSet = SystemColors.UseAlternativeColorSet;
            bool darkModeEnabled = IsDarkModeEnabled;

            if (useAlternateColorSet != darkModeEnabled)
            {
                SystemColors.UseAlternativeColorSet = darkModeEnabled;
                NotifySystemEventsOfColorChange();
            }
        }

        static void NotifySystemEventsOfColorChange()
        {
            string s_systemTrackerWindow = $".NET-BroadcastEventWindow.{AppDomain.CurrentDomain.GetHashCode():x}.0";

            HWND hwnd = PInvoke.FindWindow(s_systemTrackerWindow, s_systemTrackerWindow);
            if (hwnd.IsNull)
            {
                // Haven't created the window yet, so no need to notify.
                return;
            }

            bool complete = false;
            bool success = PInvoke.SendMessageCallback(hwnd, PInvokeCore.WM_SYSCOLORCHANGE + MessageId.WM_REFLECT, () => complete = true);
            Debug.Assert(success);

            if (!success)
            {
                return;
            }

            while (!complete)
            {
                DoEvents();
                Thread.Yield();
            }
        }
    }

    internal static Font DefaultFont => s_defaultFontScaled ?? s_defaultFont!;

    /// <summary>
    ///  Gets the system color mode setting of the OS system environment.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The color setting is determined based on the operating system version and its system settings.
    ///   It returns <see cref="SystemColorMode.Dark"/> if the dark mode is enabled in the system settings,
    ///   <see cref="SystemColorMode.Classic"/> if the color mode equals the light, standard color setting.
    ///  </para>
    ///  <para>
    ///   SystemColorMode is supported on Windows 11 or later versions.
    ///  </para>
    ///  <para>
    ///   SystemColorModes is not supported, if the Windows OS <c>High Contrast Mode</c> has been enabled in the system settings.
    ///  </para>
    /// </remarks>
    [Experimental(DiagnosticIDs.ExperimentalDarkMode, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static SystemColorMode SystemColorMode =>
        GetSystemColorModeInternal() == 0
            ? SystemColorMode.Dark
            : SystemColorMode.Classic;

    // Returns 0 if dark mode is enabled in the system, otherwise -1 (SystemDarkModeDisabled)
    private static int GetSystemColorModeInternal()
    {
        if (!IsSystemDarkModeAvailable)
        {
            return SystemDarkModeDisabled;
        }

        int systemColorMode = SystemDarkModeDisabled;

        try
        {
            // 0 for dark mode and |1| for light mode.
            systemColorMode = Math.Abs((Registry.GetValue(
                keyName: DarkModeKeyPath,
                valueName: DarkModeKey,
                defaultValue: SystemDarkModeDisabled) as int?) ?? systemColorMode);
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
        }

        return systemColorMode;
    }

    private static bool IsSystemDarkModeAvailable =>
        !SystemInformation.HighContrast && OsVersion.IsWindows11_OrGreater();

    /// <summary>
    ///  Gets a value indicating whether the application is running in a dark system color context.
    ///  Note: In a high contrast mode, this will always return <see langword="false"/>.
    /// </summary>
    [Experimental(DiagnosticIDs.ExperimentalDarkMode, UrlFormat = DiagnosticIDs.UrlFormat)]
    public static bool IsDarkModeEnabled =>
        !SystemInformation.HighContrast
        && (ColorMode == SystemColorMode.Dark
            || (ColorMode == SystemColorMode.System && SystemColorMode == SystemColorMode.Dark));

    /// <summary>
    ///  Gets the path for the executable file that started the application.
    /// </summary>
    public static string ExecutablePath =>
        s_executablePath ??= PInvoke.GetModuleFileNameLongPath(HINSTANCE.Null);

    /// <summary>
    ///  Gets the current <see cref="HighDpiMode"/> mode for the process.
    /// </summary>
    /// <value>One of the enumeration values that indicates the high DPI mode.</value>
    public static HighDpiMode HighDpiMode => ScaleHelper.GetThreadHighDpiMode();

    /// <summary>
    ///  Gets the path for the application data specific to a local, non-roaming user.
    /// </summary>
    /// <remarks>
    ///  <para>Don't obsolete these. GetDataPath isn't on SystemInformation, and it provides
    ///  the Windows logo required adornments to the directory (Company\Product\Version)</para>
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
    public static FormCollection OpenForms => s_forms ??= [];

    /// <summary>
    ///  Gets
    ///  the product name associated with this application.
    /// </summary>
    public static string? ProductName
    {
        get
        {
            if (!string.IsNullOrEmpty(s_productName))
            {
                return s_productName;
            }

            lock (s_internalSyncObject)
            {
                if (s_productName is not null)
                {
                    return s_productName;
                }

                // Custom attribute
                if (Assembly.GetEntryAssembly() is { } entryAssembly)
                {
                    object[] attrs = entryAssembly.GetCustomAttributes(typeof(AssemblyProductAttribute), inherit: false);
                    if (attrs is not null && attrs.Length > 0)
                    {
                        s_productName = ((AssemblyProductAttribute)attrs[0]).Product;
                    }
                }

                // Win32 version info
                if (string.IsNullOrEmpty(s_productName) && GetAppFileVersionInfo().ProductName is { } productName)
                {
                    s_productName = productName.Trim();
                }

                // Try using the namespace
                if (string.IsNullOrEmpty(s_productName) && GetAppMainType() is { } type)
                {
                    string? ns = type.Namespace;

                    if (!string.IsNullOrEmpty(ns))
                    {
                        int lastDot = ns.LastIndexOf('.');
                        s_productName = lastDot != -1 && lastDot < ns.Length - 1 ? ns[(lastDot + 1)..] : ns;
                    }
                    else
                    {
                        // Final fallback, use the main type.
                        s_productName = type.Name;
                    }
                }

                return s_productName;
            }
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
                    Assembly? entryAssembly = Assembly.GetEntryAssembly();
                    if (entryAssembly is not null)
                    {
                        object[] attrs = entryAssembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                        if (attrs is not null && attrs.Length > 0)
                        {
                            s_productVersion = ((AssemblyInformationalVersionAttribute)attrs[0]).InformationalVersion;
                        }
                    }

                    // Win32 version info
                    if (s_productVersion is null || s_productVersion.Length == 0)
                    {
                        s_productVersion = GetAppFileVersionInfo().ProductVersion;
                        if (s_productVersion is not null)
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
    public static void RegisterMessageLoop(MessageLoopCallback? callback)
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
            s_safeTopLevelCaptionSuffix ??= SR.SafeTopLevelCaptionFormat; // 0 - original, 1 - zone, 2 - site

            return s_safeTopLevelCaptionSuffix;
        }
        set
        {
            value ??= string.Empty;

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
            // StringBuilder sb = UnsafeNativeMethods.GetModuleFileNameLongPath(NativeMethods.NullHandleRef);
            // startupPath = Path.GetDirectoryName(sb.ToString());
            s_startupPath ??= AppContext.BaseDirectory;

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
    ///  <para>Don't obsolete these. GetDataPath isn't on SystemInformation, and it provides
    ///  the Windows logo required adornments to the directory (Company\Product\Version)</para>
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
    ///  <para>
    ///   The visual styles can be enabled by calling <see cref="EnableVisualStyles"/>.
    ///   The visual styles will not be enabled if the OS does not support them, or theming is disabled at the OS level.
    ///  </para>
    /// </remarks>
    public static bool UseVisualStyles { get; private set; }

    /// <remarks>
    ///  <para>
    ///   Don't never ever change this name, since the window class and partner teams
    ///   dependent on this. Changing this will introduce breaking changes.
    ///   If there is some reason need to change this, notify any partner teams affected.
    ///  </para>
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

            VisualStyleState vState = (VisualStyleState)PInvoke.GetThemeAppProperties();
            return vState;
        }
        set
        {
            if (VisualStyleInformation.IsSupportedByOS)
            {
                PInvoke.SetThemeAppProperties((SET_THEME_APP_PROPERTIES_FLAGS)value);

                // 248887 we need to send a WM_THEMECHANGED to the top level windows of this application.
                // We do it this way to ensure that we get all top level windows -- whether we created them or not.
                PInvokeCore.EnumWindows(SendThemeChanged);
            }
        }
    }

    /// <summary>
    ///  This helper broadcasts out a WM_THEMECHANGED to appropriate top level windows of this app.
    /// </summary>
    private static unsafe BOOL SendThemeChanged(HWND hwnd)
    {
        uint processId;
        PInvoke.GetWindowThreadProcessId(hwnd, &processId);
        if (processId == PInvoke.GetCurrentProcessId() && PInvoke.IsWindowVisible(hwnd))
        {
            SendThemeChangedRecursive(hwnd);
            PInvoke.RedrawWindow(
                hwnd,
                lprcUpdate: (RECT*)null,
                HRGN.Null,
                REDRAW_WINDOW_FLAGS.RDW_INVALIDATE
                    | REDRAW_WINDOW_FLAGS.RDW_FRAME
                    | REDRAW_WINDOW_FLAGS.RDW_ERASE
                    | REDRAW_WINDOW_FLAGS.RDW_ALLCHILDREN);
        }

        return true;
    }

    /// <summary>
    ///  This helper broadcasts out a WM_THEMECHANGED this window and all children.
    ///  It is assumed at this point that the handle belongs to the current process
    ///  and has a visible top level window.
    /// </summary>
    private static BOOL SendThemeChangedRecursive(HWND handle)
    {
        // First send to all children.
        PInvokeCore.EnumChildWindows(handle, SendThemeChangedRecursive);

        // Then send to ourself.
        PInvokeCore.SendMessage(handle, PInvokeCore.WM_THEMECHANGED);

        return true;
    }

    /// <summary>
    ///  Occurs when the application is about to shut down.
    /// </summary>
    public static event EventHandler? ApplicationExit
    {
        add => AddEventHandler(s_eventApplicationExit, value);
        remove => RemoveEventHandler(s_eventApplicationExit, value);
    }

    private static void AddEventHandler(object key, Delegate? value)
    {
        lock (s_internalSyncObject)
        {
            s_eventHandlers ??= new EventHandlerList();

            s_eventHandlers.AddHandler(key, value);
        }
    }

    private static void RemoveEventHandler(object key, Delegate? value)
    {
        lock (s_internalSyncObject)
        {
            if (s_eventHandlers is null)
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
    public static void AddMessageFilter(IMessageFilter? value)
        => ThreadContext.FromCurrent().AddMessageFilter(value);

    /// <summary>
    ///  Processes all message filters for given message
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static bool FilterMessage(ref Message message)
    {
        // Create copy of MSG structure
        MSG msg = message.ToMSG();
        bool processed = ThreadContext.FromCurrent().ProcessFilters(ref msg, out bool modified);
        if (modified)
        {
            message.HWnd = msg.hwnd;
            message.MsgInternal = (MessageId)msg.message;
            message.WParamInternal = msg.wParam;
            message.LParamInternal = msg.lParam;
        }

        return processed;
    }

    /// <summary>
    ///  Occurs when the application has finished processing and is about to enter the
    ///  idle state.
    /// </summary>
    public static event EventHandler? Idle
    {
        add
        {
            ThreadContext current = ThreadContext.FromCurrent();
            lock (current)
            {
                current._idleHandler += value;
                current.EnsureReadyForIdle();
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
    public static event EventHandler? EnterThreadModal
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
    public static event EventHandler? LeaveThreadModal
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
    ///  Occurs when an un-trapped thread exception is thrown.
    /// </summary>
    public static event ThreadExceptionEventHandler? ThreadException
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
    ///  Occurs when a thread is about to shut down. When the main thread for an
    ///  application is about to be shut down, this event will be raised first,
    ///  followed by an <see cref="ApplicationExit"/> event.
    /// </summary>
    public static event EventHandler? ThreadExit
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
        => ThreadContext.FromCurrent().RunMessageLoop(msoloop.DoEvents, null);

    internal static void DoEventsModal()
        => ThreadContext.FromCurrent().RunMessageLoop(msoloop.DoEventsModal, null);

    /// <summary>
    ///  Enables visual styles for all subsequent <see cref="Run()"/> and <see cref="Control.CreateHandle"/> calls.
    ///  Uses the default theming manifest file shipped with the redist.
    /// </summary>
    [UnconditionalSuppressMessage("SingleFile", "IL3002", Justification = "Single-file case is handled")]
    public static void EnableVisualStyles()
    {
        // Pull manifest from our resources
        Module module = typeof(Application).Module;
        var moduleHandle = PInvoke.GetModuleHandle(module.Name);

        if (moduleHandle != 0)
        {
            // We have a native module, point to our native embedded manifest resource.
            // CSC embeds DLL manifests as native resource ID 2
            UseVisualStyles = ThemingScope.CreateActivationContext(moduleHandle, nativeResourceManifestID: 2);
        }
        else
        {
            // We couldn't grab the module handle, likely we're running from a single file package.
            // Extract the manifest from managed resources.
            using Stream? stream = module.Assembly.GetManifestResourceStream(
                "System.Windows.Forms.XPThemes.manifest");
            if (stream is not null)
            {
                UseVisualStyles = ThemingScope.CreateActivationContext(stream);
            }
        }

        Debug.Assert(UseVisualStyles, "Enable Visual Styles failed");

        s_comCtlSupportsVisualStylesInitialized = false;
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
    ///  whether any of the open forms canceled the exit call.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    public static void Exit(CancelEventArgs? e)
    {
        lock (s_internalSyncObject)
        {
            if (s_exiting)
            {
                // Recursive call to Exit
                if (e is not null)
                {
                    e.Cancel = false;
                }

                return;
            }

            s_exiting = true;

            try
            {
                // Raise the FormClosing and FormClosed events for each open form
                if (s_forms?.Count > 0)
                {
                    HashSet<Form> processedForms = new(s_forms.Count);
                    int version = s_forms.AddVersion;
                    // We need to iterate in backward order to not violate MDI closing events rules
                    for (int i = s_forms.Count - 1; i > -1; i--)
                    {
                        Form? form = s_forms[i];
                        if (form is null || processedForms.Contains(form))
                        {
                            continue;
                        }

                        processedForms.Add(form);
                        // Here user can remove existing forms or add new
                        if (form.RaiseFormClosingOnAppExit())
                        {
                            // A form refused to close
                            if (e is not null)
                            {
                                e.Cancel = true;
                            }

                            processedForms.Clear();
                            return;
                        }

                        if (version != s_forms.AddVersion) // A new form was added, we need to iterate again
                        {
                            version = s_forms.AddVersion;
                            i = s_forms.Count;
                        }
                        else
                        {
                            i = Math.Min(i, s_forms.Count); // Form can be removed from the collection, we need to check it
                        }
                    }

                    processedForms.Clear();
                    while (s_forms.Count > 0)
                    {
                        // We need to iterate in backward order to not violate MDI closing events rules
                        Form? form = s_forms[^1];
                        if (form is not null)
                        {
                            // OnFormClosed removes the form from the FormCollection
                            form.RaiseFormClosedOnAppExit();
                        }
                        else
                        {
                            s_forms.RemoveAt(s_forms.Count - 1);
                        }
                    }
                }

                ThreadContext.ExitApplication();
                if (e is not null)
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
        if (context.ApplicationContext is not null)
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
    [UnconditionalSuppressMessage("SingleFile", "IL3002", Justification = "Single-file case is handled")]
    private static FileVersionInfo GetAppFileVersionInfo()
    {
        if (s_appFileVersion is { } fileVersion)
        {
            return fileVersion;
        }

        lock (s_internalSyncObject)
        {
            if (s_appFileVersion is null)
            {
                Type? type = GetAppMainType();

                // In a single-file, "Location" will be empty and it will fall back to ExecutablePath,
                // which gives the desired result.
#pragma warning disable IL3000 // Avoid accessing Assembly file path when publishing as a single file
                s_appFileVersion = type is not null && type.Assembly.Location.Length > 0
                    ? FileVersionInfo.GetVersionInfo(type.Module.FullyQualifiedName)
                    : FileVersionInfo.GetVersionInfo(ExecutablePath);
#pragma warning restore IL3000
            }
        }

        return s_appFileVersion;
    }

    /// <summary>
    ///  Retrieves the Type that contains the "Main" method.
    /// </summary>
    private static Type? GetAppMainType()
    {
        lock (s_internalSyncObject)
        {
            if (s_mainType is null)
            {
                Assembly? exe = Assembly.GetEntryAssembly();

                // Get Main type...This doesn't work in MC++ because Main is a global function and not
                // a class static method (it doesn't belong to a Type).
                if (exe is not null)
                {
                    s_mainType = exe.EntryPoint?.ReflectedType;
                }
            }
        }

        return s_mainType;
    }

    /// <summary>
    ///  Locates a thread context given a window handle.
    /// </summary>
    internal static unsafe ThreadContext GetContextForHandle<T>(T handle) where T : IHandle<HWND>
    {
        ThreadContext? threadContext = ThreadContext.FromId(PInvoke.GetWindowThreadProcessId(handle.Handle, null));
        Debug.Assert(
            threadContext is not null,
            "No thread context for handle. This is expected if you saw a previous assert about the handle being invalid.");

        GC.KeepAlive(handle);
        return threadContext;
    }

    /// <summary>
    ///  Returns a string that is the combination of the basePath + CompanyName + ProductName + ProductVersion. This
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
        if (s_eventHandlers is not null)
        {
            Delegate? exit = s_eventHandlers[s_eventApplicationExit];
            if (exit is not null)
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
        if (s_eventHandlers is not null)
        {
            Delegate? exit = s_eventHandlers[s_eventThreadExit];
            if (exit is not null)
            {
                ((EventHandler)exit)(null, EventArgs.Empty);
            }
        }
    }

    /// <summary>
    ///  "Parks" the given HWND to a temporary HWND. This allows WS_CHILD windows to be parked.
    /// </summary>
    internal static void ParkHandle(HandleRef<HWND> handle, DPI_AWARENESS_CONTEXT dpiAwarenessContext)
    {
        Debug.Assert(PInvoke.IsWindow(handle), "Handle being parked is not a valid window handle");
        Debug.Assert(
            ((WINDOW_STYLE)PInvokeCore.GetWindowLong(handle.Handle, WINDOW_LONG_PTR_INDEX.GWL_STYLE)).HasFlag(WINDOW_STYLE.WS_CHILD),
            "Only WS_CHILD windows should be parked.");

        GetContextForHandle(handle)?.GetParkingWindow(dpiAwarenessContext).ParkHandle(handle);

        GC.KeepAlive(handle);
    }

    /// <summary>
    ///  Park control handle on a parking window that has matching DpiAwareness.
    /// </summary>
    /// <param name="cp">Create params for control handle.</param>
    /// <param name="dpiAwarenessContext">DPI awareness.</param>
    internal static void ParkHandle(CreateParams cp, DPI_AWARENESS_CONTEXT dpiAwarenessContext)
    {
        ThreadContext threadContext = ThreadContext.FromCurrent();
        if (threadContext is not null)
        {
            cp.Parent = threadContext.GetParkingWindow(dpiAwarenessContext).Handle;
        }
    }

    /// <summary>
    ///  Initializes OLE on the current thread.
    /// </summary>
    public static ApartmentState OleRequired()
        => ThreadContext.FromCurrent().OleRequired();

    /// <summary>
    ///  Raises the <see cref="ThreadException"/> event.
    /// </summary>
    public static void OnThreadException(Exception t)
        => ThreadContext.FromCurrent().OnThreadException(t);

    /// <summary>
    ///  "Unparks" the given HWND to a temporary HWND. This allows WS_CHILD windows to
    ///  be parked.
    /// </summary>
    internal static void UnparkHandle(IHandle<HWND> handle, DPI_AWARENESS_CONTEXT context)
    {
        ThreadContext threadContext = GetContextForHandle(handle);
        threadContext?.GetParkingWindow(context).UnparkHandle(handle);
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
        Debug.Assert(process is not null);

        if (!hrefExeCase)
        {
            // Regular app case
            string[] arguments = Environment.GetCommandLineArgs();
            Debug.Assert(arguments is not null && arguments.Length > 0);

            ProcessStartInfo currentStartInfo = new()
            {
                FileName = ExecutablePath
            };

            if (arguments.Length >= 2)
            {
                StringBuilder sb = new((arguments.Length - 1) * 16);
                for (int argumentIndex = 1; argumentIndex < arguments.Length; argumentIndex++)
                {
                    sb.Append($"\"{arguments[argumentIndex]}\" ");
                }

                currentStartInfo.Arguments = sb.ToString(0, sb.Length - 1);
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
        => ThreadContext.FromCurrent().RunMessageLoop(msoloop.Main, new ApplicationContext());

    /// <summary>
    ///  Begins running a standard application message loop on the current
    ///  thread, and makes the specified form visible.
    /// </summary>
    public static void Run(Form mainForm)
        => ThreadContext.FromCurrent().RunMessageLoop(msoloop.Main, new ApplicationContext(mainForm));

    /// <summary>
    ///  Begins running a standard application message loop on the current thread,
    ///  without a form.
    /// </summary>
    public static void Run(ApplicationContext context)
        => ThreadContext.FromCurrent().RunMessageLoop(msoloop.Main, context);

    /// <summary>
    ///  Runs a modal dialog. This starts a special type of message loop that runs until
    ///  the dialog has a valid DialogResult. This is called internally by a form
    ///  when an application calls System.Windows.Forms.Form.ShowDialog().
    /// </summary>
    internal static void RunDialog(Form form)
        => ThreadContext.FromCurrent().RunMessageLoop(msoloop.ModalForm, new ModalApplicationContext(form));

    /// <summary>
    ///  Sets the static UseCompatibleTextRenderingDefault field on Control to the value passed in.
    ///  This switch determines the default text rendering engine to use by some controls that support
    ///  switching rendering engine.
    /// </summary>
    public static void SetCompatibleTextRenderingDefault(bool defaultValue)
    {
        if (NativeWindow.AnyHandleCreated)
        {
            throw new InvalidOperationException(string.Format(SR.Win32WindowAlreadyCreated, nameof(SetCompatibleTextRenderingDefault)));
        }

        Control.UseCompatibleTextRenderingDefault = defaultValue;
    }

    /// <summary>
    ///  Sets the default <see cref="Font"/> for process.
    /// </summary>
    /// <param name="font">The font to be used as a default across the application.</param>
    /// <exception cref="ArgumentNullException"><paramref name="font"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    ///  You can only call this method before the first window is created by your Windows Forms application.
    /// </exception>
    /// <remarks>
    /// <para>
    ///  The system text scale factor will be applied to the font, i.e. if the default font is set to "Calibri, 11f"
    ///  and the text scale factor is set to 150% the resulting default font will be set to "Calibri, 16.5f".
    /// </para>
    /// <para>
    ///  Users can adjust text scale with the Make text bigger slider on the Settings -> Ease of Access -> Vision/Display screen.
    /// </para>
    /// </remarks>
    /// <seealso href="https://docs.microsoft.com/windows/uwp/design/input/text-scaling">Windows Text scaling</seealso>
    public static void SetDefaultFont(Font font)
    {
        ArgumentNullException.ThrowIfNull(font);

        if (NativeWindow.AnyHandleCreated)
            throw new InvalidOperationException(string.Format(SR.Win32WindowAlreadyCreated, nameof(SetDefaultFont)));

        s_defaultFont = font;
        ScaleDefaultFont();
    }

    /// <summary>
    ///  Scale <see cref="s_defaultFont"/> or <see cref="s_defaultFontScaled"/> if needed.
    /// </summary>
    internal static void ScaleDefaultFont()
    {
        if (s_defaultFont is null)
        {
            return;
        }

        if (s_defaultFont.IsSystemFont)
        {
            s_defaultFontScaled?.Dispose();
            s_defaultFontScaled = null;
            // Recreating the SystemFont will have it scaled to the right size for the current setting. This could be
            // done more efficiently by querying the OS to see if this is necessary for the specific font.
            //
            // This should never return null.
            Font newSystemFont = SystemFonts.GetFontByName(s_defaultFont.SystemFontName)!;
            if (s_defaultFont.Equals(newSystemFont))
            {
                // No point in keeping an identical one, free the resource.
                newSystemFont.Dispose();
            }
            else
            {
                s_defaultFont = newSystemFont;
            }
        }
        else // non system Font
        {
            Font? font = ScaleHelper.ScaleToSystemTextSize(s_defaultFont);
            if (font is null || !font.Equals(s_defaultFontScaled)) // change s_defaultFontScaled only if needed
            {
                s_defaultFontScaled?.Dispose();
                s_defaultFontScaled = font;
            }
            else
            {
                font.Dispose();
            }
        }
    }

    /// <summary>
    ///  Sets the <see cref="HighDpiMode"/> mode for process.
    /// </summary>
    /// <param name="highDpiMode">One of the enumeration values that specifies the high DPI mode to set.</param>
    /// <returns><see langword="true" /> if the high DPI mode was set; otherwise, <see langword="false" />.</returns>
    public static bool SetHighDpiMode(HighDpiMode highDpiMode)
    {
        SourceGenerated.EnumValidator.Validate(highDpiMode, nameof(highDpiMode));
        return !s_parkingWindowCreated && ScaleHelper.SetProcessHighDpiMode(highDpiMode);
    }

    /// <summary>
    ///  Sets the suspend/hibernate state of the machine.
    ///  Returns true if the call succeeded, else false.
    /// </summary>
    public static bool SetSuspendState(PowerState state, bool force, bool disableWakeEvent)
        => PInvoke.SetSuspendState((state == PowerState.Hibernate), force, disableWakeEvent);

    /// <summary>
    ///  Overload version of SetUnhandledExceptionMode that sets the UnhandledExceptionMode
    ///  mode at the current thread level.
    /// </summary>
    public static void SetUnhandledExceptionMode(UnhandledExceptionMode mode)
        => SetUnhandledExceptionMode(mode, true /*threadScope*/);

    /// <summary>
    ///  This method can be used to modify the exception handling behavior of
    ///  NativeWindow. By default, NativeWindow will detect if an application
    ///  is running under a debugger, or is running on a machine with a debugger
    ///  installed. In this case, an unhandled exception in the NativeWindow's
    ///  WndProc method will remain unhandled so the debugger can trap it. If
    ///  there is no debugger installed NativeWindow will trap the exception
    ///  and route it to the Application class's unhandled exception filter.
    ///
    ///  You can control this behavior via a config file, or directly through
    ///  code using this method. Setting the unhandled exception mode does
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

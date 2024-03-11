﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.Versioning;

namespace System.Windows.Forms.Primitives;

// Borrowed from https://github.com/dotnet/runtime/blob/main/src/libraries/Common/src/System/LocalAppContextSwitches.Common.cs
internal static partial class LocalAppContextSwitches
{
    // Enabling switches in Core is different from Framework. See https://learn.microsoft.com/dotnet/core/runtime-config/
    // for details on how to set switches.

    // Switch names declared internal below are used in unit/integration tests. Refer to
    // https://github.com/dotnet/winforms/blob/tree/main/docs/design/anchor-layout-changes-in-net80.md
    // for more details on how to enable these switches in the application.
    private const string ScaleTopLevelFormMinMaxSizeForDpiSwitchName = "System.Windows.Forms.ScaleTopLevelFormMinMaxSizeForDpi";
    internal const string AnchorLayoutV2SwitchName = "System.Windows.Forms.AnchorLayoutV2";
    internal const string ApplyParentFontToMenusSwitchName = "System.Windows.Forms.ApplyParentFontToMenus";
    internal const string ServicePointManagerCheckCrlSwitchName = "System.Windows.Forms.ServicePointManagerCheckCrl";
    internal const string TrackBarModernRenderingSwitchName = "System.Windows.Forms.TrackBarModernRendering";
    private const string DoNotCatchUnhandledExceptionsSwitchName = "System.Windows.Forms.DoNotCatchUnhandledExceptions";
    internal const string DataGridViewUIAStartRowCountAtZeroSwitchName = "System.Windows.Forms.DataGridViewUIAStartRowCountAtZero";
    internal const string NoClientNotificationsSwitchName = "Switch.System.Windows.Forms.AccessibleObject.NoClientNotifications";
    internal const string EnableMsoComponentManagerSwitchName = "Switch.System.Windows.Forms.EnableMsoComponentManager";

    private static int s_scaleTopLevelFormMinMaxSizeForDpi;
    private static int s_anchorLayoutV2;
    private static int s_applyParentFontToMenus;
    private static int s_servicePointManagerCheckCrl;
    private static int s_trackBarModernRendering;
    private static int s_doNotCatchUnhandledExceptions;
    private static int s_dataGridViewUIAStartRowCountAtZero;
    private static int s_noClientNotifications;
    private static int s_enableMsoComponentManager;

    private static FrameworkName? s_targetFrameworkName;

    /// <summary>
    ///  When there is no exception handler registered for a thread, rethrows the exception. The exception will
    ///  not be presented in a dialog or swallowed when not in interactive mode. This is always opt-in and is
    ///  intended for scenarios where setting handlers for threads isn't practical.
    /// </summary>
    public static bool DoNotCatchUnhandledExceptions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(DoNotCatchUnhandledExceptionsSwitchName, ref s_doNotCatchUnhandledExceptions);
    }

    /// <summary>
    ///  The <see cref="TargetFrameworkAttribute"/> value for the entry assembly, if any.
    /// </summary>
    public static FrameworkName? TargetFrameworkName
    {
        get
        {
            s_targetFrameworkName ??= AppContext.TargetFrameworkName is { } name ? new(name) : null;
            return s_targetFrameworkName;
        }
    }

    private static bool GetCachedSwitchValue(string switchName, ref int cachedSwitchValue)
    {
        // The cached switch value has 3 states: 0 - unknown, 1 - true, -1 - false
        if (cachedSwitchValue < 0)
        {
            return false;
        }

        if (cachedSwitchValue > 0)
        {
            return true;
        }

        return GetSwitchValue(switchName, ref cachedSwitchValue);
    }

    private static bool GetSwitchValue(string switchName, ref int cachedSwitchValue)
    {
        bool hasSwitch = AppContext.TryGetSwitch(switchName, out bool isSwitchEnabled);
        if (!hasSwitch)
        {
            isSwitchEnabled = GetSwitchDefaultValue(switchName);
        }

        AppContext.TryGetSwitch("TestSwitch.LocalAppContext.DisableCaching", out bool disableCaching);
        if (!disableCaching)
        {
            cachedSwitchValue = isSwitchEnabled ? 1 /*true*/ : -1 /*false*/;
        }

        return isSwitchEnabled;

        static bool GetSwitchDefaultValue(string switchName)
        {
            if (TargetFrameworkName is not { } framework)
            {
                return false;
            }

            if (switchName == NoClientNotificationsSwitchName)
            {
                return false;
            }

            if (framework.Version.Major >= 8)
            {
                // Behavior changes added in .NET 8

                if (switchName == ScaleTopLevelFormMinMaxSizeForDpiSwitchName)
                {
                    return true;
                }

                if (switchName == TrackBarModernRenderingSwitchName)
                {
                    return true;
                }

                if (switchName == ServicePointManagerCheckCrlSwitchName)
                {
                    return true;
                }
            }

            return false;
        }
    }

    /// <summary>
    ///  Indicates whether AnchorLayoutV2 feature is enabled.
    /// </summary>
    /// <devdoc>
    ///  Returns AnchorLayoutV2 switch value from runtimeconfig.json. Defaults to false.
    ///  Refer to
    ///  https://github.com/dotnet/winforms/blob/tree/main/docs/design/anchor-layout-changes-in-net80.md for more details.
    /// </devdoc>
    public static bool AnchorLayoutV2
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(AnchorLayoutV2SwitchName, ref s_anchorLayoutV2);
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the parent font (as set by <see cref="Forms.Application.SetDefaultFont(Font)" />
    ///  or by the parent control or form's font) is applied to menus.
    /// </summary>
    public static bool ApplyParentFontToMenus
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(ApplyParentFontToMenusSwitchName, ref s_applyParentFontToMenus);
    }

    public static bool ScaleTopLevelFormMinMaxSizeForDpi
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(ScaleTopLevelFormMinMaxSizeForDpiSwitchName, ref s_scaleTopLevelFormMinMaxSizeForDpi);
    }

    public static bool TrackBarModernRendering
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(TrackBarModernRenderingSwitchName, ref s_trackBarModernRendering);
    }

    /// <summary>
    ///  Indicates whether certificates are checked against the certificate authority revocation list.
    ///  If true, revoked certificates will not be accepted by WebRequests and WebClients as valid.
    ///  Otherwise, revoked certificates will be accepted as valid.
    /// </summary>
    public static bool ServicePointManagerCheckCrl
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(ServicePointManagerCheckCrlSwitchName, ref s_servicePointManagerCheckCrl);
    }

    public static bool DataGridViewUIAStartRowCountAtZero
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(DataGridViewUIAStartRowCountAtZeroSwitchName, ref s_dataGridViewUIAStartRowCountAtZero);
    }

    public static bool NoClientNotifications
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(NoClientNotificationsSwitchName, ref s_noClientNotifications);
    }

    /// <summary>
    ///  If <see langword="true"/> Windows Forms threads will register with existing IMsoComponentManager instances.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   See <see href="https://learn.microsoft.com/previous-versions/office/developer/office-2007/ff518974(v=office.12)">
    ///   Component API Reference for the 2007 Office System</see> for more information on the IMsoComponentManager.
    ///  </para>
    /// </remarks>
    public static bool EnableMsoComponentManager
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => GetCachedSwitchValue(EnableMsoComponentManagerSwitchName, ref s_enableMsoComponentManager);
    }

    internal static void SetLocalAppContextSwitchValue(string switchName, bool value)
    {
        if (switchName == NoClientNotificationsSwitchName)
        {
            s_noClientNotifications = value ? 1 : 0;
        }

        if (switchName == DataGridViewUIAStartRowCountAtZeroSwitchName)
        {
            s_dataGridViewUIAStartRowCountAtZero = value ? 1 : 0;
        }

        if (switchName == ApplyParentFontToMenusSwitchName)
        {
            s_applyParentFontToMenus = value ? 1 : 0;
        }
    }

    internal static bool GetCachedSwitchValue(string switchName)
    {
        int cachedSwitchValue = 0;
        return GetCachedSwitchValue(switchName, ref cachedSwitchValue);
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public sealed partial class Application
{
    private static readonly object s_eventSystemVisualSettingsChanged = new();

    /// <summary>
    ///  Gets the current Windows visual and accessibility settings snapshot.
    /// </summary>
    public static SystemVisualSettings SystemVisualSettings
        => SystemVisualSettingsTracker.CurrentSettings;

    /// <summary>
    ///  Occurs when Windows visual or accessibility settings change.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   WinForms raises this event once for each settings transition, normalizing
    ///   <c>WM_SETTINGCHANGE</c>, <c>WM_DWMCOLORIZATIONCOLORCHANGED</c>,
    ///   <c>WM_THEMECHANGED</c>, and <c>WM_SYSCOLORCHANGE</c>. Handlers should use
    ///   <see cref="SystemVisualSettingsChangedEventArgs.Changed"/> to early-out when
    ///   their category did not change.
    ///  </para>
    ///  <para>
    ///   This static event is for application-lifetime consumers such as theming engines and
    ///   services. Components whose lifetime is shorter than the application must unsubscribe.
    ///   Controls and forms should use <see cref="Control.SystemVisualSettingsChanged"/> or
    ///   override <see cref="Control.OnSystemVisualSettingsChanged(SystemVisualSettingsChangedEventArgs)"/>
    ///   instead; that instance path requires no unsubscription.
    ///  </para>
    /// </remarks>
    public static event SystemVisualSettingsChangedEventHandler? SystemVisualSettingsChanged
    {
        add => AddEventHandler(s_eventSystemVisualSettingsChanged, value);
        remove => RemoveEventHandler(s_eventSystemVisualSettingsChanged, value);
    }

    internal static void OnSystemVisualSettingsChanged(SystemVisualSettingsChangedEventArgs e)
    {
        SystemVisualSettingsChangedEventHandler? handler;

        lock (s_internalSyncObject)
        {
            handler = s_eventHandlers?[s_eventSystemVisualSettingsChanged] as SystemVisualSettingsChangedEventHandler;
        }

        handler?.Invoke(null, e);
    }
}

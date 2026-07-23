// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using Microsoft.Win32;

namespace System.Windows.Forms;

public sealed partial class Application
{
#if NET11_0_OR_GREATER
    private static readonly object s_eventSystemTextSizeChanged = new();

    private static SystemTextSizeAwareness s_systemTextSizeAwareness;
    private static double s_lastSystemTextSize = 1.0;
    private static bool s_systemTextSizeNotificationsInitialized;

    /// <summary>
    ///  Gets the current Windows Accessibility text-scale factor
    ///  (Settings → Accessibility → Text size), as a multiplier in the range 1.0–2.25.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Live getter — re-reads the underlying system value on every access; does not cache.
    ///  </para>
    ///  <para>
    ///   This property is orthogonal to display DPI; see <see cref="HighDpiMode"/> for the DPI-scaling story.
    ///  </para>
    ///  <para>
    ///   On operating systems earlier than Windows 10 version 1507, this property returns <c>1.0</c>.
    ///  </para>
    /// </remarks>
    public static double SystemTextSize => ScaleHelper.GetSystemTextScaleFactor();

    /// <summary>
    ///  Gets the application's mode for reacting to changes in the Windows Accessibility text-scale setting.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Use <see cref="SetSystemTextSizeAwareness(SystemTextSizeAwareness)"/> to change this value.
    ///  </para>
    /// </remarks>
    public static SystemTextSizeAwareness SystemTextSizeAwareness => s_systemTextSizeAwareness;

    /// <summary>
    ///  Occurs once per process when the Windows Accessibility text-scale setting changes,
    ///  while <see cref="SystemTextSizeAwareness"/> is <see cref="SystemTextSizeAwareness.Notify"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   WinForms detects relevant changes by re-reading the underlying text-scale factor when Windows reports an
    ///   accessibility preference change. Because there is no dedicated text-scale notification, unrelated accessibility
    ///   changes do not raise this event unless the factor actually changed.
    ///  </para>
    ///  <para>
    ///   The framework listens through a single internal <see cref="SystemEvents.UserPreferenceChanged"/> subscription.
    ///   Individual <see cref="Form"/> instances receive their own <see cref="Form.SystemTextSizeChanged"/>
    ///   notifications from their window procedures and do not subscribe to this static event, avoiding framework-managed
    ///   static-event rooting of forms.
    ///  </para>
    /// </remarks>
    public static event EventHandler? SystemTextSizeChanged
    {
        add => AddEventHandler(s_eventSystemTextSizeChanged, value);
        remove => RemoveEventHandler(s_eventSystemTextSizeChanged, value);
    }

    /// <summary>
    ///  Sets the application's mode for reacting to changes in the Windows Accessibility text-scale setting.
    /// </summary>
    /// <param name="awareness">
    ///  One of the enumeration values that specifies how the application reacts to Accessibility text-scale changes.
    /// </param>
    /// <exception cref="InvalidEnumArgumentException">
    ///  <paramref name="awareness"/> is not a valid <see cref="SystemTextSizeAwareness"/> value.
    /// </exception>
    public static void SetSystemTextSizeAwareness(SystemTextSizeAwareness awareness)
    {
        SourceGenerated.EnumValidator.Validate(awareness, nameof(awareness));

        lock (s_internalSyncObject)
        {
            EnsureSystemTextSizeNotificationsInitialized();
            s_systemTextSizeAwareness = awareness;
        }
    }

    private static void EnsureSystemTextSizeNotificationsInitialized()
    {
        lock (s_internalSyncObject)
        {
            if (s_systemTextSizeNotificationsInitialized)
            {
                return;
            }

            s_lastSystemTextSize = ScaleHelper.GetSystemTextScaleFactor();
            SystemEvents.UserPreferenceChanged += OnSystemTextSizeUserPreferenceChanged;
            s_systemTextSizeNotificationsInitialized = true;
        }
    }

    private static void OnSystemTextSizeUserPreferenceChanged(object? sender, UserPreferenceChangedEventArgs e)
    {
        if (e.Category != UserPreferenceCategory.Accessibility
            || !ScaleHelper.TryGetSystemTextScaleFactor(out double systemTextSize))
        {
            return;
        }

        EventHandler? handler = null;

        lock (s_internalSyncObject)
        {
            if (systemTextSize == s_lastSystemTextSize)
            {
                return;
            }

            s_lastSystemTextSize = systemTextSize;

            if (s_systemTextSizeAwareness == SystemTextSizeAwareness.Notify)
            {
                handler = s_eventHandlers?[s_eventSystemTextSizeChanged] as EventHandler;
            }
        }

        handler?.Invoke(null, EventArgs.Empty);
    }
#endif
}

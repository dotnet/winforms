// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.Win32;

namespace System.Windows.Forms;

public sealed partial class Application
{
#if NET11_0_OR_GREATER
    private static readonly object s_eventSystemTextSizeChanged = new();

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
    ///  Occurs once per process when the Windows Accessibility text-scale setting changes.
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
    ///  <para>
    ///   On operating systems earlier than Windows 10 version 1507, this event is not raised.
    ///  </para>
    /// </remarks>
    public static event EventHandler? SystemTextSizeChanged
    {
        add
        {
            if (value is null)
            {
                return;
            }

            lock (s_internalSyncObject)
            {
                EnsureSystemTextSizeNotificationsInitialized();
                AddEventHandler(s_eventSystemTextSizeChanged, value);
            }
        }
        remove
        {
            if (value is null)
            {
                return;
            }

            lock (s_internalSyncObject)
            {
                RemoveEventHandler(s_eventSystemTextSizeChanged, value);

                if (s_systemTextSizeNotificationsInitialized
                    && s_eventHandlers?[s_eventSystemTextSizeChanged] is null)
                {
                    SystemEvents.UserPreferenceChanged -= OnSystemTextSizeUserPreferenceChanged;
                    s_systemTextSizeNotificationsInitialized = false;
                }
            }
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

        EventHandler? handler;

        lock (s_internalSyncObject)
        {
            if (systemTextSize == s_lastSystemTextSize)
            {
                return;
            }

            s_lastSystemTextSize = systemTextSize;
            handler = s_eventHandlers?[s_eventSystemTextSizeChanged] as EventHandler;
        }

        handler?.Invoke(null, EventArgs.Empty);
    }
#endif
}

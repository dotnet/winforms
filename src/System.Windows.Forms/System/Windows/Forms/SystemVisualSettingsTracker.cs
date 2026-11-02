// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.WinRT;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.UI.ViewManagement;
using static Windows.Win32.UI.WindowsAndMessaging.SYSTEM_PARAMETERS_INFO_ACTION;

namespace System.Windows.Forms;

/// <summary>
///  Maintains the process-wide system visual settings snapshot.
/// </summary>
internal static class SystemVisualSettingsTracker
{
    private static Func<SystemVisualSettings> s_snapshotProvider = GetNativeSnapshot;
    private static SystemVisualSettings s_currentSettings = GetNativeSnapshot();
    private static long s_transitionVersion;

    /// <summary>
    ///  Gets the current immutable snapshot.
    /// </summary>
    internal static SystemVisualSettings CurrentSettings
        => Volatile.Read(ref s_currentSettings);

    /// <summary>
    ///  Gets the version of the latest settings transition.
    /// </summary>
    internal static long TransitionVersion
        => Interlocked.Read(ref s_transitionVersion);

    /// <summary>
    ///  Reads the settings source and publishes an event for an effective transition.
    /// </summary>
    internal static SystemVisualSettings Refresh()
    {
        while (true)
        {
            SystemVisualSettings current = CurrentSettings;
            SystemVisualSettings next = Volatile.Read(ref s_snapshotProvider).Invoke();
            SystemVisualSettingsCategories changed = GetChangedCategories(current, next);

            if (changed == SystemVisualSettingsCategories.None)
            {
                return current;
            }

            if (ReferenceEquals(Interlocked.CompareExchange(ref s_currentSettings, next, current), current))
            {
                Interlocked.Increment(ref s_transitionVersion);
                Application.OnSystemVisualSettingsChanged(
                    new SystemVisualSettingsChangedEventArgs(current, next, changed));

                return next;
            }
        }
    }

    /// <summary>
    ///  Gets the categories that differ between two snapshots.
    /// </summary>
    internal static SystemVisualSettingsCategories GetChangedCategories(
        SystemVisualSettings oldSettings,
        SystemVisualSettings newSettings)
    {
        SystemVisualSettingsCategories changed = SystemVisualSettingsCategories.None;

        if (oldSettings.AccentColor != newSettings.AccentColor)
        {
            changed |= SystemVisualSettingsCategories.AccentColor;
        }

        if (oldSettings.TextScaleFactor != newSettings.TextScaleFactor)
        {
            changed |= SystemVisualSettingsCategories.TextScale;
        }

        if (oldSettings.HighContrastEnabled != newSettings.HighContrastEnabled)
        {
            changed |= SystemVisualSettingsCategories.HighContrast;
        }

        if (oldSettings.ClientAreaAnimationEnabled != newSettings.ClientAreaAnimationEnabled)
        {
            changed |= SystemVisualSettingsCategories.ClientAreaAnimations;
        }

        if (oldSettings.KeyboardCuesVisible != newSettings.KeyboardCuesVisible)
        {
            changed |= SystemVisualSettingsCategories.KeyboardCues;
        }

        if (oldSettings.FocusBorderMetrics != newSettings.FocusBorderMetrics)
        {
            changed |= SystemVisualSettingsCategories.FocusMetrics;
        }

        return changed;
    }

    /// <summary>
    ///  Creates a snapshot from values returned by the native settings source.
    /// </summary>
    internal static SystemVisualSettings CreateSnapshot(SystemVisualSettingsNativeValues values)
        => new(
            values.AccentColor,
            values.TextScaleFactor,
            values.HighContrastEnabled,
            values.ClientAreaAnimationEnabled,
            values.KeyboardCuesVisible,
            values.FocusBorderMetrics);

    /// <summary>
    ///  Sets a deterministic settings source for unit tests.
    /// </summary>
    internal static void ResetForTesting(
        SystemVisualSettings currentSettings,
        Func<SystemVisualSettings>? snapshotProvider = null)
    {
        ArgumentNullException.ThrowIfNull(currentSettings);

        Volatile.Write(ref s_snapshotProvider, snapshotProvider ?? GetNativeSnapshot);
        Interlocked.Exchange(ref s_currentSettings, currentSettings);
        Interlocked.Exchange(ref s_transitionVersion, 0);
    }

    /// <summary>
    ///  Restores the native settings source after a unit test.
    /// </summary>
    internal static void ResetForTesting()
    {
        SystemVisualSettings nativeSettings = GetNativeSnapshot();
        ResetForTesting(nativeSettings);
    }

    private static SystemVisualSettings GetNativeSnapshot()
    {
        HIGHCONTRASTW highContrast = default;
        bool highContrastEnabled = PInvokeCore.SystemParametersInfo(ref highContrast)
            && highContrast.dwFlags.HasFlag(HIGHCONTRASTW_FLAGS.HCF_HIGHCONTRASTON);

        SystemVisualSettingsNativeValues values = new(
            GetAccentColor(),
            (float)ScaleHelper.GetSystemTextScaleFactor(),
            highContrastEnabled,
            PInvokeCore.SystemParametersInfoBool(SPI_GETCLIENTAREAANIMATION),
            PInvokeCore.SystemParametersInfoBool(SPI_GETKEYBOARDCUES),
            new Size(
                PInvokeCore.SystemParametersInfoInt(SPI_GETFOCUSBORDERWIDTH),
                PInvokeCore.SystemParametersInfoInt(SPI_GETFOCUSBORDERHEIGHT)));

        return CreateSnapshot(values);
    }

    private static Color GetAccentColor()
    {
        try
        {
            return GetAccentColorCore();
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
            Debug.Fail($"Unable to query the Windows accent color: {ex.Message}");
            return SystemColors.Highlight;
        }
    }

    private static unsafe Color GetAccentColorCore()
    {
        HSTRING className = default;

        fixed (char* pClassName = "Windows.UI.ViewManagement.UISettings")
        {
            PInvokeCore.WindowsCreateString(
                sourceString: (PCWSTR)pClassName,
                length: 36u,
                @string: &className).ThrowOnFailure();
        }

        try
        {
            using ComScope<IInspectable> inspectable = new(null);
            PInvokeCore.RoActivateInstance(className, inspectable).ThrowOnFailure();

            using ComScope<IUISettings3> settings = inspectable.TryQuery<IUISettings3>(out HRESULT hr);
            hr.ThrowOnFailure();

            UIColor color;
            settings.Value->GetColorValue(UIColorType.Accent, &color).ThrowOnFailure();

            return Color.FromArgb(color.A, color.R, color.G, color.B);
        }
        finally
        {
            PInvokeCore.WindowsDeleteString(className);
        }
    }
}

/// <summary>
///  Contains values obtained from the native Windows settings APIs.
/// </summary>
internal readonly record struct SystemVisualSettingsNativeValues(
    Color AccentColor,
    float TextScaleFactor,
    bool HighContrastEnabled,
    bool ClientAreaAnimationEnabled,
    bool KeyboardCuesVisible,
    Size FocusBorderMetrics);

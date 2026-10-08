// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Represents an immutable snapshot of visual and accessibility settings that Windows owns.
/// </summary>
/// <remarks>
///  <para>
///   Applications must not override these values. Renderers can combine this snapshot with their
///   own state to honor the user's Windows personalization and accessibility preferences.
///  </para>
/// </remarks>
public sealed class SystemVisualSettings
{
    internal SystemVisualSettings(
        Color accentColor,
        float textScaleFactor,
        bool highContrastEnabled,
        bool clientAreaAnimationEnabled,
        bool keyboardCuesVisible,
        Size focusBorderMetrics)
    {
        AccentColor = accentColor;
        TextScaleFactor = textScaleFactor;
        HighContrastEnabled = highContrastEnabled;
        ClientAreaAnimationEnabled = clientAreaAnimationEnabled;
        KeyboardCuesVisible = keyboardCuesVisible;
        FocusBorderMetrics = focusBorderMetrics;
    }

    /// <summary>
    ///  Gets the user's current Windows accent color.
    /// </summary>
    public Color AccentColor { get; }

    /// <summary>
    ///  Gets the Windows Accessibility text-scale factor.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The factor is independent of display DPI and is in the range 1.0 through 2.25.
    ///  </para>
    /// </remarks>
    public float TextScaleFactor { get; }

    /// <summary>
    ///  Gets a value indicating whether the user has enabled Windows high-contrast mode.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   When this value is <see langword="true"/>, controls use the classic effective
    ///   <see cref="VisualStylesMode"/> so custom modern chrome does not bypass the High Contrast palette.
    ///  </para>
    /// </remarks>
    public bool HighContrastEnabled { get; }

    /// <summary>
    ///  Gets a value indicating whether Windows enables client-area animations.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   When this value changes to <see langword="false"/>, active client-area animations complete immediately.
    ///   Re-enabling animations affects later transitions only.
    ///  </para>
    /// </remarks>
    public bool ClientAreaAnimationEnabled { get; }

    /// <summary>
    ///  Gets a value indicating whether the system default displays keyboard cues.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is the system default. A window can temporarily override the effective cue state
    ///   through <c>WM_UPDATEUISTATE</c>.
    ///  </para>
    /// </remarks>
    public bool KeyboardCuesVisible { get; }

    /// <summary>
    ///  Gets the Windows focus-border width and height, in pixels.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Renderers can use these metrics as a baseline for border and focus prominence, scaling
    ///   them for DPI and <see cref="TextScaleFactor"/> rather than relying on fixed constants.
    ///  </para>
    /// </remarks>
    public Size FocusBorderMetrics { get; }
}

/// <summary>
///  Specifies the categories that changed in a <see cref="SystemVisualSettings"/> transition.
/// </summary>
[Flags]
public enum SystemVisualSettingsCategories
{
    /// <summary>
    ///  No visual settings changed.
    /// </summary>
    None = 0,

    /// <summary>
    ///  The Windows accent color changed.
    /// </summary>
    AccentColor = 1 << 0,

    /// <summary>
    ///  The Windows Accessibility text-scale factor changed.
    /// </summary>
    TextScale = 1 << 1,

    /// <summary>
    ///  The Windows high-contrast setting changed.
    /// </summary>
    HighContrast = 1 << 2,

    /// <summary>
    ///  The Windows client-area animation setting changed.
    /// </summary>
    ClientAreaAnimations = 1 << 3,

    /// <summary>
    ///  The Windows keyboard-cue default changed.
    /// </summary>
    KeyboardCues = 1 << 4,

    /// <summary>
    ///  The Windows focus-border metrics changed.
    /// </summary>
    FocusMetrics = 1 << 5
}

/// <summary>
///  Provides data for a <see cref="Application.SystemVisualSettingsChanged"/> or
///  <see cref="Control.SystemVisualSettingsChanged"/> event.
/// </summary>
public class SystemVisualSettingsChangedEventArgs : EventArgs
{
    internal SystemVisualSettingsChangedEventArgs(
        SystemVisualSettings oldSettings,
        SystemVisualSettings newSettings,
        SystemVisualSettingsCategories changed)
    {
        OldSettings = oldSettings;
        NewSettings = newSettings;
        Changed = changed;
    }

    /// <summary>
    ///  Gets the settings before the transition.
    /// </summary>
    public SystemVisualSettings OldSettings { get; }

    /// <summary>
    ///  Gets the settings after the transition.
    /// </summary>
    public SystemVisualSettings NewSettings { get; }

    /// <summary>
    ///  Gets the categories that changed in the transition.
    /// </summary>
    public SystemVisualSettingsCategories Changed { get; }
}

/// <summary>
///  Represents the method that handles a system visual settings change.
/// </summary>
/// <param name="sender">
///  The source of the event, or <see langword="null"/> for
///  <see cref="Application.SystemVisualSettingsChanged"/>.
/// </param>
/// <param name="e">The event data.</param>
public delegate void SystemVisualSettingsChangedEventHandler(object? sender, SystemVisualSettingsChangedEventArgs e);

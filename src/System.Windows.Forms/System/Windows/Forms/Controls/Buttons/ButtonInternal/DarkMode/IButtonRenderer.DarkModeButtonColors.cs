// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Cache of colors for different button states.
/// </summary>
internal static class DarkModeButtonColors
{
    // Normal Button (non-default)

    /// <summary>
    ///  Button background color for normal state (#2B2B2B).
    /// </summary>
    public static Color NormalBackgroundColor => Color.FromArgb(43, 43, 43); // #2B2B2B

    /// <summary>
    ///  Button background color for hover state (#3B3B3B).
    /// </summary>
    public static Color HoverBackgroundColor => Color.FromArgb(59, 59, 59); // #3B3B3B

    /// <summary>
    ///  Button background color for pressed state (#4B4B4B).
    /// </summary>
    public static Color PressedBackgroundColor => Color.FromArgb(75, 75, 75); // #4B4B4B

    /// <summary>
    ///  Button background color for disabled state (#252525).
    /// </summary>
    public static Color DisabledBackgroundColor => Color.FromArgb(37, 37, 37); // #252525

    // Default Button

    /// <summary>
    ///  Default button background color (#2B2B2B).
    /// </summary>
    public static Color DefaultBackgroundColor => NormalBackgroundColor; // #2B2B2B

    /// <summary>
    ///  Default button hover background color (#3B3B3B).
    /// </summary>
    public static Color DefaultHoverBackgroundColor => HoverBackgroundColor; // #3B3B3B

    /// <summary>
    ///  Default button pressed background color (#4B4B4B).
    /// </summary>
    public static Color DefaultPressedBackgroundColor => PressedBackgroundColor; // #4B4B4B

    /// <summary>
    ///  Default button disabled background color (#252525).
    /// </summary>
    public static Color DefaultDisabledBackgroundColor => DisabledBackgroundColor; // #252525

    // Text Colors

    /// <summary>
    ///  Normal text color (#E0E0E0).
    /// </summary>
    public static Color NormalTextColor => Color.FromArgb(224, 224, 224); // #E0E0E0

    /// <summary>
    ///  Default button text color (#FFFFFF).
    /// </summary>
    public static Color DefaultTextColor => Color.White; // #FFFFFF

    /// <summary>
    ///  Disabled text color (#606060, ~40% opacity).
    /// </summary>
    public static Color DisabledTextColor => Color.FromArgb(96, 96, 96); // #606060

    /// <summary>
    ///  Gets the single border color for a button in dark mode (#969696).
    /// </summary>
    public static Color SingleBorderColor => Color.FromArgb(150, 150, 150); // #969696

    /// <summary>
    ///  Gets the single border color for a default button in dark mode (#D2D2D2).
    /// </summary>
    public static Color DefaultSingleBorderColor => Color.FromArgb(210, 210, 210); // #D2D2D2

    /// <summary>
    ///  Gets the single border color for a pressed button in dark mode (#DCDCDC).
    /// </summary>
    public static Color PressedSingleBorderColor => Color.FromArgb(220, 220, 220); // #DCDCDC

    /// <summary>
    ///  Button top-left border color (#555555).
    /// </summary>
    public static Color TopLeftBorderColor => Color.FromArgb(85, 85, 85); // #555555

    /// <summary>
    ///  Button bottom-right border color (#222222).
    /// </summary>
    public static Color BottomRightBorderColor => Color.FromArgb(34, 34, 34); // #222222

    // Focus Colors

    /// <summary>
    ///  Focus indicator color (#F0F0F0).
    /// </summary>
    public static Color FocusIndicatorColor => Color.FromArgb(240, 240, 240); // #F0F0F0

    /// <summary>
    ///  Default button focus indicator color (#FFFFFF).
    /// </summary>
    public static Color DefaultFocusIndicatorColor => Color.White; // #FFFFFF

    // Shadow and Highlight Colors for 3D effects

    /// <summary>
    ///  Shadow color for dark areas (#282828).
    /// </summary>
    public static Color ShadowDarkColor { get; } = Color.FromArgb(40, 40, 40); // #282828

    /// <summary>
    ///  Shadow color for mid-tone areas (#3C3C3C).
    /// </summary>
    public static Color ShadowColor { get; } = Color.FromArgb(60, 60, 60); // #3C3C3C

    /// <summary>
    ///  Highlight color for button edges (#6E6E6E).
    /// </summary>
    public static Color HighlightColor { get; } = Color.FromArgb(110, 110, 110); // #6E6E6E

    /// <summary>
    ///  Bright highlight color for button edges (#828282).
    /// </summary>
    public static Color HighlightBrightColor { get; } = Color.FromArgb(130, 130, 130); // #828282

    /// <summary>
    ///  Disabled border dark color (#2D2D2D).
    /// </summary>
    public static Color DisabledBorderDarkColor { get; } = Color.FromArgb(45, 45, 45); // #2D2D2D

    /// <summary>
    ///  Disabled border light color (#373737).
    /// </summary>
    public static Color DisabledBorderLightColor { get; } = Color.FromArgb(55, 55, 55); // #373737

    /// <summary>
    ///  Disabled border mid color (#323232).
    /// </summary>
    public static Color DisabledBorderMidColor { get; } = Color.FromArgb(50, 50, 50); // #323232
}

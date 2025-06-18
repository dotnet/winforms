// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Cache of colors for different button states in dark mode.
/// </summary>
internal class DarkModeButtonColors
{
    private static DarkModeButtonColors? s_defaultColors;

    /// <summary>
    ///  Gets the default <see cref="DarkModeButtonColors"/> instance.
    /// </summary>
    public static DarkModeButtonColors DefaultColors { get; }
        = s_defaultColors ??= new();

    /// <summary>
    ///  Gets the default accept (default) button focus indicator back color (#A0A0A0).
    /// </summary>
    public virtual Color DefaultAcceptFocusIndicatorBackColor =>
        Color.FromArgb(0xA0, 0xA0, 0xA0);

    /// <summary>
    ///  Gets the default button background color (#333333).
    /// </summary>
    protected virtual Color DefaultBackColor =>
        Color.FromArgb(0x33, 0x33, 0x33);

    /// <summary>
    ///  Gets the default button bottom-right border color (#222222).
    /// </summary>
    protected virtual Color DefaultBottomRightBorderColor =>
        Color.FromArgb(0x22, 0x22, 0x22);

    /// <summary>
    ///  Gets the default disabled button background color (#333333).
    /// </summary>
    protected virtual Color DefaultDisabledBackColor =>
        Color.FromArgb(0x33, 0x33, 0x33);

    /// <summary>
    ///  Gets the default disabled border dark color (#2D2D2D).
    /// </summary>
    protected virtual Color DefaultDisabledBorderDarkColor =>
        Color.FromArgb(0x2D, 0x2D, 0x2D);

    /// <summary>
    ///  Gets the default disabled border light color (#373737).
    /// </summary>
    protected virtual Color DefaultDisabledBorderLightColor =>
        Color.FromArgb(0x37, 0x37, 0x37);

    /// <summary>
    ///  Gets the default disabled border mid color (#323232).
    /// </summary>
    protected virtual Color DefaultDisabledBorderMidColor =>
        Color.FromArgb(0x32, 0x32, 0x32);

    /// <summary>
    ///  Gets the default disabled text color (#CCCCCC).
    /// </summary>
    protected virtual Color DefaultDisabledTextColor =>
        Color.FromArgb(0xCC, 0xCC, 0xCC);

    /// <summary>
    ///  Gets the default focus back color (#333333).
    /// </summary>
    protected virtual Color DefaultFocusBackColor =>
        Color.FromArgb(0x33, 0x33, 0x33);

    /// <summary>
    ///  Gets the default focus border color (#FFFFFF).
    /// </summary>
    protected virtual Color DefaultFocusBorderColor =>
        Color.FromArgb(0xFF, 0xFF, 0xFF);

    /// <summary>
    ///  Gets the default focus indicator back color (#A0A0A0).
    /// </summary>
    public virtual Color DefaultFocusIndicatorBackColor =>
        Color.FromArgb(0xA0, 0xA0, 0xA0);

    /// <summary>
    ///  Gets the default highlight bright color for button edges (#828282).
    /// </summary>
    protected virtual Color DefaultHighlightBrightColor =>
        Color.FromArgb(0x82, 0x82, 0x82);

    /// <summary>
    ///  Gets the default highlight color for button edges (#6E6E6E).
    /// </summary>
    protected virtual Color DefaultHighlightColor =>
        Color.FromArgb(0x6E, 0x6E, 0x6E);

    /// <summary>
    ///  Gets the default hover back color (#454545).
    /// </summary>
    protected virtual Color DefaultHoverBackgroundColor =>
        Color.FromArgb(0x45, 0x45, 0x45);

    /// <summary>
    ///  Gets the default pressed back color (#666666).
    /// </summary>
    protected virtual Color DefaultPressedBackgroundColor =>
        Color.FromArgb(0x66, 0x66, 0x66);

    /// <summary>
    ///  Gets the default pressed single border color (#A0A0A0).
    /// </summary>
    protected virtual Color DefaultPressedSingleBorderColor =>
        Color.FromArgb(0xA0, 0xA0, 0xA0);

    /// <summary>
    ///  Gets the default shadow color for mid-tone areas (#3C3C3C).
    /// </summary>
    protected virtual Color DefaultShadowColor =>
        Color.FromArgb(0x3C, 0x3C, 0x3C);

    /// <summary>
    ///  Gets the default shadow dark color for dark areas (#282828).
    /// </summary>
    protected virtual Color DefaultShadowDarkColor =>
        Color.FromArgb(0x28, 0x28, 0x28);

    /// <summary>
    ///  Gets the default single border color for a button in dark mode (#9B9B9B).
    /// </summary>
    protected virtual Color DefaultSingleBorderColor =>
        Color.FromArgb(0x9B, 0x9B, 0x9B);

    /// <summary>
    ///  Gets the default button text color (#F0F0F0).
    /// </summary>
    public virtual Color AcceptButtonTextColor =>
        Color.FromArgb(0xF0, 0xF0, 0xF0);

    /// <summary>
    ///  Gets the default button top-left border color (#555555).
    /// </summary>
    protected virtual Color DefaultTopLeftBorderColor =>
        Color.FromArgb(0x55, 0x55, 0x55);

    /// <summary>
    ///  Gets the accept (default) button focus indicator back color.
    /// </summary>
    public Color AcceptFocusIndicatorBackColor =>
        DefaultAcceptFocusIndicatorBackColor;

    /// <summary>
    ///  Gets the button bottom-right border color.
    /// </summary>
    public Color BottomRightBorderColor =>
        DefaultBottomRightBorderColor;

    /// <summary>
    ///  Gets the disabled button background color.
    /// </summary>
    public Color DisabledBackColor =>
        DefaultDisabledBackColor;

    /// <summary>
    ///  Gets the disabled border dark color.
    /// </summary>
    public Color DisabledBorderDarkColor =>
        DefaultDisabledBorderDarkColor;

    /// <summary>
    ///  Gets the disabled border light color.
    /// </summary>
    public Color DisabledBorderLightColor =>
        DefaultDisabledBorderLightColor;

    /// <summary>
    ///  Gets the disabled border mid color.
    /// </summary>
    public Color DisabledBorderMidColor =>
        DefaultDisabledBorderMidColor;

    /// <summary>
    ///  Gets the disabled text color.
    /// </summary>
    public Color DisabledTextColor =>
        DefaultDisabledTextColor;

    /// <summary>
    ///  Gets the focus border color.
    /// </summary>
    public Color FocusBorderColor =>
        DefaultFocusBorderColor;

    /// <summary>
    ///  Gets the focus indicator back color.
    /// </summary>
    public Color FocusIndicatorBackColor =>
        DefaultFocusIndicatorBackColor;

    /// <summary>
    ///  Gets the focused button background color.
    /// </summary>
    public Color FocusedBackColor =>
        DefaultFocusBackColor;

    /// <summary>
    ///  Gets the highlight bright color for button edges.
    /// </summary>
    public Color HighlightBrightColor =>
        DefaultHighlightBrightColor;

    /// <summary>
    ///  Gets the highlight color for button edges.
    /// </summary>
    public Color HighlightColor =>
        DefaultHighlightColor;

    /// <summary>
    ///  Gets the hover button background color.
    /// </summary>
    public Color HoverBackColor =>
        DefaultHoverBackgroundColor;

    /// <summary>
    ///  Gets the normal button background color.
    /// </summary>
    public Color StandardBackColor =>
        DefaultBackColor;

    /// <summary>
    ///  Gets the normal button text color.
    /// </summary>
    public Color NormalTextColor =>
        AcceptButtonTextColor;

    /// <summary>
    ///  Gets the pressed button background color.
    /// </summary>
    public Color PressedBackColor =>
        DefaultPressedBackgroundColor;

    /// <summary>
    ///  Gets the pressed single border color.
    /// </summary>
    public Color PressedSingleBorderColor =>
        DefaultPressedSingleBorderColor;

    /// <summary>
    ///  Gets the shadow color for mid-tone areas.
    /// </summary>
    public Color ShadowColor =>
        DefaultShadowColor;

    /// <summary>
    ///  Gets the shadow dark color for dark areas.
    /// </summary>
    public Color ShadowDarkColor =>
        DefaultShadowDarkColor;

    /// <summary>
    ///  Gets the single border color for a button in dark mode.
    /// </summary>
    public Color SingleBorderColor =>
        DefaultSingleBorderColor;

    /// <summary>
    ///  Gets the button top-left border color.
    /// </summary>
    public Color TopLeftBorderColor =>
        DefaultTopLeftBorderColor;
}

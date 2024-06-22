// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  Represents a class that provides themed system colors.
/// </summary>
public abstract class ThemedSystemColors
{
    /// <summary>
    ///  Gets the active border color.
    /// </summary>
    public virtual Color ActiveBorder { get; } = SystemColors.ActiveBorder;

    /// <summary>
    ///  Gets the window color.
    /// </summary>
    public virtual Color Window { get; } = SystemColors.Window;

    /// <summary>
    ///  Gets the scroll bar color.
    /// </summary>
    public virtual Color ScrollBar { get; } = SystemColors.ScrollBar;

    /// <summary>
    ///  Gets the menu text color.
    /// </summary>
    public virtual Color MenuText { get; } = SystemColors.MenuText;

    /// <summary>
    ///  Gets the menu highlight color.
    /// </summary>
    public virtual Color MenuHighlight { get; } = SystemColors.MenuHighlight;

    /// <summary>
    ///  Gets the menu bar color.
    /// </summary>
    public virtual Color MenuBar { get; } = SystemColors.MenuBar;

    /// <summary>
    ///  Gets the menu color.
    /// </summary>
    public virtual Color Menu { get; } = SystemColors.Menu;

    /// <summary>
    ///  Gets the info text color.
    /// </summary>
    public virtual Color InfoText { get; } = SystemColors.InfoText;

    /// <summary>
    ///  Gets the info color.
    /// </summary>
    public virtual Color Info { get; } = SystemColors.Info;

    /// <summary>
    ///  Gets the inactive caption text color.
    /// </summary>
    public virtual Color InactiveCaptionText { get; } = SystemColors.InactiveCaptionText;

    /// <summary>
    ///  Gets the inactive caption color.
    /// </summary>
    public virtual Color InactiveCaption { get; } = SystemColors.InactiveCaption;

    /// <summary>
    ///  Gets the inactive border color.
    /// </summary>
    public virtual Color InactiveBorder { get; } = SystemColors.InactiveBorder;

    /// <summary>
    ///  Gets the hot track color.
    /// </summary>
    public virtual Color HotTrack { get; } = SystemColors.HotTrack;

    /// <summary>
    ///  Gets the highlight text color.
    /// </summary>
    public virtual Color HighlightText { get; } = SystemColors.HighlightText;

    /// <summary>
    ///  Gets the highlight color.
    /// </summary>
    public virtual Color Highlight { get; } = SystemColors.Highlight;

    /// <summary>
    ///  Gets the window frame color.
    /// </summary>
    public virtual Color WindowFrame { get; } = SystemColors.WindowFrame;

    /// <summary>
    ///  Gets the gray text color.
    /// </summary>
    public virtual Color GrayText { get; } = SystemColors.GrayText;

    /// <summary>
    ///  Gets the gradient active caption color.
    /// </summary>
    public virtual Color GradientActiveCaption { get; } = SystemColors.GradientActiveCaption;

    /// <summary>
    ///  Gets the desktop color.
    /// </summary>
    public virtual Color Desktop { get; } = SystemColors.Desktop;

    /// <summary>
    ///  Gets the control text color.
    /// </summary>
    public virtual Color ControlText { get; } = SystemColors.ControlText;

    /// <summary>
    ///  Gets the control light light color.
    /// </summary>
    public virtual Color ControlLightLight { get; } = SystemColors.ControlLightLight;

    /// <summary>
    ///  Gets the control light color.
    /// </summary>
    public virtual Color ControlLight { get; } = SystemColors.ControlLight;

    /// <summary>
    ///  Gets the control dark dark color.
    /// </summary>
    public virtual Color ControlDarkDark { get; } = SystemColors.ControlDarkDark;

    /// <summary>
    ///  Gets the control dark color.
    /// </summary>
    public virtual Color ControlDark { get; } = SystemColors.ControlDark;

    /// <summary>
    ///  Gets the control color.
    /// </summary>
    public virtual Color Control { get; } = SystemColors.Control;

    /// <summary>
    ///  Gets the button shadow color.
    /// </summary>
    public virtual Color ButtonShadow { get; } = SystemColors.ButtonShadow;

    /// <summary>
    ///  Gets the button highlight color.
    /// </summary>
    public virtual Color ButtonHighlight { get; } = SystemColors.ButtonHighlight;

    /// <summary>
    ///  Gets the button face color.
    /// </summary>
    public virtual Color ButtonFace { get; } = SystemColors.ButtonFace;

    /// <summary>
    ///  Gets the application workspace color.
    /// </summary>
    public virtual Color AppWorkspace { get; } = SystemColors.AppWorkspace;

    /// <summary>
    ///  Gets the active caption text color.
    /// </summary>
    public virtual Color ActiveCaptionText { get; } = SystemColors.ActiveCaptionText;

    /// <summary>
    ///  Gets the active caption color.
    /// </summary>
    public virtual Color ActiveCaption { get; } = SystemColors.ActiveCaption;

    /// <summary>
    ///  Gets the gradient inactive caption color.
    /// </summary>
    public virtual Color GradientInactiveCaption { get; } = SystemColors.GradientActiveCaption;

    /// <summary>
    ///  Gets the window text color.
    /// </summary>
    public virtual Color WindowText { get; } = SystemColors.WindowText;
}

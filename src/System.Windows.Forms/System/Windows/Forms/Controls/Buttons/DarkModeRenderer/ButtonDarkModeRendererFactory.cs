// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Factory class for creating button dark mode renderers based on FlatStyle.
/// </summary>
internal static class ButtonDarkModeRendererFactory
{
    /// <summary>
    ///  Gets the appropriate renderer for the specified FlatStyle.
    /// </summary>
    public static IButtonDarkModeRenderer GetRenderer(FlatStyle flatStyle) =>
        flatStyle switch
        {
            FlatStyle.Flat => ButtonDarkModeRenderer.FlatRenderer,
            FlatStyle.Popup => ButtonDarkModeRenderer.PopupRenderer,
            FlatStyle.System => ButtonDarkModeRenderer.SystemRenderer,
            _ => ButtonDarkModeRenderer.StandardRenderer // FlatStyle.Standard is default
        };
}

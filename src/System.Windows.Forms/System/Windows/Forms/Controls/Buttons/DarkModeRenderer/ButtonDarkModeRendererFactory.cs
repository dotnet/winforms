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
    public static IButtonDarkModeRenderer GetRenderer(FlatStyle flatStyle)
    {
        return flatStyle switch
        {
            FlatStyle.Flat => GetFlatRenderer(),
            FlatStyle.Popup => GetPopupRenderer(),
            FlatStyle.System => GetSystemRenderer(),
            _ => GetStandardRenderer() // FlatStyle.Standard is default
        };
    }

    /// <summary>
    /// Gets the renderer for FlatStyle.Standard.
    /// </summary>
    private static IButtonDarkModeRenderer GetStandardRenderer()
    {
        // Use ThreadStatic field to cache renderer instances
        return ButtonDarkModeRenderer.StandardRenderer;
    }

    /// <summary>
    /// Gets the renderer for FlatStyle.Flat.
    /// </summary>
    private static IButtonDarkModeRenderer GetFlatRenderer()
    {
        return ButtonDarkModeRenderer.FlatRenderer;
    }

    /// <summary>
    /// Gets the renderer for FlatStyle.Popup.
    /// </summary>
    private static IButtonDarkModeRenderer GetPopupRenderer()
    {
        return ButtonDarkModeRenderer.PopupRenderer;
    }

    /// <summary>
    /// Gets the renderer for FlatStyle.System.
    /// </summary>
    private static IButtonDarkModeRenderer GetSystemRenderer()
    {
        return ButtonDarkModeRenderer.SystemRenderer;
    }
}

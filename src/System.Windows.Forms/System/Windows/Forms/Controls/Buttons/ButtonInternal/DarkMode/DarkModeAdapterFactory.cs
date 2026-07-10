// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.ButtonInternal;

internal static class DarkModeAdapterFactory
{
    // The owner-drawn dark/modern adapter is used when dark mode is enabled (conservative renderer) or when
    // the control opts into the modern .NET 11 visual styles (modern renderer, in either dark or light scheme).
    private static bool UseOwnerDrawnAdapter(ButtonBase control)
    {
        return Application.IsDarkModeEnabled || control.EffectiveVisualStylesModeInternal >= VisualStylesMode.Net11;
    }

    public static ButtonBaseAdapter CreateFlatAdapter(ButtonBase control) =>
        UseOwnerDrawnAdapter(control)
            ? new ButtonDarkModeAdapter(control)
            : new ButtonFlatAdapter(control);

    public static ButtonBaseAdapter CreateStandardAdapter(ButtonBase control) =>
        UseOwnerDrawnAdapter(control)
            ? new ButtonDarkModeAdapter(control)
            : new ButtonStandardAdapter(control);

    public static ButtonBaseAdapter CreatePopupAdapter(ButtonBase control) =>
        UseOwnerDrawnAdapter(control)
            ? new ButtonDarkModeAdapter(control)
            : new ButtonPopupAdapter(control);
}

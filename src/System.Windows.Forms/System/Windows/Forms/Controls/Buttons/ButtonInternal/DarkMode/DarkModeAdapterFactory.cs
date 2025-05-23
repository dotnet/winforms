// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.ButtonInternal;

internal static class DarkModeAdapterFactory
{
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

    public static ButtonBaseAdapter CreateFlatAdapter(ButtonBase control) =>
        Application.IsDarkModeEnabled
            ? new ButtonDarkModeAdapter(control)
            : new ButtonFlatAdapter(control);

    public static ButtonBaseAdapter CreateStandardAdapter(ButtonBase control) =>
        Application.IsDarkModeEnabled
            ? new ButtonDarkModeAdapter(control)
            : new ButtonStandardAdapter(control);

    public static ButtonBaseAdapter CreatePopupAdapter(ButtonBase control) =>
        Application.IsDarkModeEnabled
            ? new ButtonDarkModeAdapter(control)
            : new ButtonPopupAdapter(control);

#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
}

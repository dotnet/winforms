// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public partial class DataGridViewButtonCell
{
    private static (Rectangle ContentBounds, Color TextColor) DrawButton(
        Graphics graphics,
        Rectangle bounds,
        PushButtonState state,
        bool isDefault,
        FlatStyle flatStyle,
        int deviceDpi)
        => DataGridViewButtonCellRenderer.DrawButton(
            graphics,
            bounds,
            state,
            isDefault,
            flatStyle,
            deviceDpi);

    private static Rectangle GetButtonContentBounds(
        Graphics graphics,
        Rectangle bounds,
        FlatStyle flatStyle,
        int deviceDpi)
        => DataGridViewButtonCellRenderer.GetContentBounds(graphics, bounds, flatStyle, deviceDpi);

    private static class DataGridViewButtonCellRenderer
    {
        private static VisualStyleRenderer? s_visualStyleRenderer;
        [ThreadStatic]
        private static FlatButtonDarkModeRenderer? s_flatButtonDarkModeRenderer;
        [ThreadStatic]
        private static SystemButtonDarkModeRenderer? s_systemButtonDarkModeRenderer;

        public static VisualStyleRenderer DataGridViewButtonRenderer
        {
            get
            {
                s_visualStyleRenderer ??= new VisualStyleRenderer(s_buttonElement);

                return s_visualStyleRenderer;
            }
        }

        public static (Rectangle ContentBounds, Color TextColor) DrawButton(
            Graphics g,
            Rectangle bounds,
            PushButtonState state,
            bool isDefault,
            FlatStyle flatStyle,
            int deviceDpi)
        {
            if (Application.IsDarkModeEnabled
                && AppContextSwitches.DataGridViewDarkModeTheming
                && !SystemInformation.HighContrast)
            {
                ButtonDarkModeRendererBase renderer = GetDarkModeRenderer(flatStyle);
                renderer.DeviceDpi = deviceDpi;
                Color backColor = renderer.GetBackgroundColor(state, isDefault, customBaseColor: Color.Empty);
                using (new GraphicsStateScope(g))
                {
                    renderer.DrawButtonBackground(
                        g,
                        bounds,
                        state,
                        isDefault,
                        focused: false,
                        backColor);
                }

                return (
                    GetContentBounds(g, bounds, flatStyle, deviceDpi),
                    GetTextColor(renderer, state, isDefault, flatStyle, backColor));
            }

            PushButtonState visualStyleState = isDefault && state == PushButtonState.Normal
                ? PushButtonState.Default
                : state;
            DataGridViewButtonRenderer.SetParameters(
                s_buttonElement.ClassName,
                s_buttonElement.Part,
                (int)visualStyleState);
            DataGridViewButtonRenderer.DrawBackground(g, bounds, Rectangle.Truncate(g.ClipBounds));
            return (
                GetContentBounds(g, bounds, flatStyle, deviceDpi),
                DataGridViewButtonRenderer.GetColor(ColorProperty.TextColor));
        }

        public static Rectangle GetContentBounds(
            Graphics g,
            Rectangle bounds,
            FlatStyle flatStyle,
            int deviceDpi)
        {
            if (Application.IsDarkModeEnabled
                && AppContextSwitches.DataGridViewDarkModeTheming
                && !SystemInformation.HighContrast)
            {
                ButtonDarkModeRendererBase renderer = GetDarkModeRenderer(flatStyle);
                renderer.DeviceDpi = deviceDpi;
                return Rectangle.Inflate(bounds, -3, -3);
            }

            return DataGridViewButtonRenderer.GetBackgroundContentRectangle(g, bounds);
        }

        public static Color GetDarkModeTextColor(
            PushButtonState state,
            bool isDefault,
            FlatStyle flatStyle)
        {
            ButtonDarkModeRendererBase renderer = GetDarkModeRenderer(flatStyle);
            Color backColor = renderer.GetBackgroundColor(state, isDefault, customBaseColor: Color.Empty);
            return GetTextColor(renderer, state, isDefault, flatStyle, backColor);
        }

        public static Color GetDarkModeBackgroundColor(
            PushButtonState state,
            bool isDefault,
            FlatStyle flatStyle)
        {
            ButtonDarkModeRendererBase renderer = GetDarkModeRenderer(flatStyle);
            return renderer.GetBackgroundColor(state, isDefault, customBaseColor: Color.Empty);
        }

        private static ButtonDarkModeRendererBase GetDarkModeRenderer(FlatStyle flatStyle)
            => flatStyle switch
            {
                FlatStyle.Standard => s_flatButtonDarkModeRenderer ??= new FlatButtonDarkModeRenderer(),
                FlatStyle.System => s_systemButtonDarkModeRenderer ??= new SystemButtonDarkModeRenderer(),
                _ => throw new ArgumentOutOfRangeException(nameof(flatStyle))
            };

        private static Color GetTextColor(
            ButtonDarkModeRendererBase renderer,
            PushButtonState state,
            bool isDefault,
            FlatStyle flatStyle,
            Color backColor)
            => renderer.GetTextColor(state, isDefault && flatStyle != FlatStyle.System, backColor);
    }
}

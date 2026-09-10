// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms;

public partial class DataGridViewButtonCell
{
    private static class DataGridViewButtonCellRenderer
    {
        private static VisualStyleRenderer? s_visualStyleRenderer;
        [ThreadStatic]
        private static ButtonDarkModeRendererBase? s_flatButtonDarkModeRenderer;
        [ThreadStatic]
        private static ButtonDarkModeRendererBase? s_modernButtonDarkModeRenderer;
        [ThreadStatic]
        private static ButtonDarkModeRendererBase? s_systemButtonDarkModeRenderer;

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
            int deviceDpi,
            bool useModernRenderer)
        {
            if (Application.IsDarkModeEnabled
                && AppContextSwitches.DataGridViewDarkModeTheming
                && !SystemInformation.HighContrast)
            {
                ButtonDarkModeRendererBase renderer = GetDarkModeRenderer(flatStyle, useModernRenderer);
                renderer.DeviceDpi = deviceDpi;
                Color backColor = renderer.GetBackgroundColor(state, isDefault, customBaseColor: Color.Empty);
                Rectangle contentBounds;
                using (new GraphicsStateScope(g))
                {
                    contentBounds = renderer.DrawButtonBackground(
                        g,
                        bounds,
                        state,
                        isDefault,
                        focused: false,
                        backColor);
                }

                return (contentBounds, renderer.GetTextColor(state, isDefault, backColor));
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
                DataGridViewButtonRenderer.GetBackgroundContentRectangle(g, bounds),
                DataGridViewButtonRenderer.GetColor(ColorProperty.TextColor));
        }

        public static Color GetDarkModeTextColor(
            PushButtonState state,
            bool isDefault,
            FlatStyle flatStyle,
            bool useModernRenderer)
        {
            ButtonDarkModeRendererBase renderer = GetDarkModeRenderer(flatStyle, useModernRenderer);
            Color backColor = renderer.GetBackgroundColor(state, isDefault, customBaseColor: Color.Empty);
            return renderer.GetTextColor(state, isDefault, backColor);
        }

        private static ButtonDarkModeRendererBase GetDarkModeRenderer(FlatStyle flatStyle, bool useModernRenderer)
            => flatStyle switch
            {
                FlatStyle.Standard => useModernRenderer
                    ? s_modernButtonDarkModeRenderer ??= new ModernButtonDarkModeRenderer()
                    : s_flatButtonDarkModeRenderer ??= new FlatButtonDarkModeRenderer(),
                FlatStyle.System => s_systemButtonDarkModeRenderer ??= new SystemButtonDarkModeRenderer(),
                _ => throw new ArgumentOutOfRangeException(nameof(flatStyle))
            };
    }
}

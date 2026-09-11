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
                Rectangle paddedBounds = ApplyRendererPadding(renderer, bounds);
                Rectangle contentBounds;
                using (new GraphicsStateScope(g))
                {
                    contentBounds = renderer.DrawButtonBackground(
                        g,
                        paddedBounds,
                        state,
                        isDefault,
                        focused: false,
                        backColor);
                }

                return (contentBounds, GetTextColor(renderer, state, isDefault, flatStyle, backColor));
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
            FlatStyle flatStyle)
        {
            ButtonDarkModeRendererBase renderer = GetDarkModeRenderer(flatStyle);
            Color backColor = renderer.GetBackgroundColor(state, isDefault, customBaseColor: Color.Empty);
            return GetTextColor(renderer, state, isDefault, flatStyle, backColor);
        }

        private static ButtonDarkModeRendererBase GetDarkModeRenderer(FlatStyle flatStyle)
            => flatStyle switch
            {
                FlatStyle.Standard => s_flatButtonDarkModeRenderer ??= new FlatButtonDarkModeRenderer(),
                FlatStyle.System => s_systemButtonDarkModeRenderer ??= new SystemButtonDarkModeRenderer(),
                _ => throw new ArgumentOutOfRangeException(nameof(flatStyle))
            };

        private static Rectangle ApplyRendererPadding(ButtonDarkModeRendererBase renderer, Rectangle bounds)
        {
            Padding padding = renderer.GetPreferredSizePadding();

            return new Rectangle(
                bounds.X + padding.Left,
                bounds.Y + padding.Top,
                bounds.Width - padding.Horizontal,
                bounds.Height - padding.Vertical);
        }

        private static Color GetTextColor(
            ButtonDarkModeRendererBase renderer,
            PushButtonState state,
            bool isDefault,
            FlatStyle flatStyle,
            Color backColor)
            => renderer.GetTextColor(state, isDefault && flatStyle != FlatStyle.System, backColor);
    }
}

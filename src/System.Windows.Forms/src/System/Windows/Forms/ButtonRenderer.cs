// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This is a rendering class for the Button control. It works downlevel too (obviously
    ///  without visual styles applied.)
    /// </summary>
    public static class ButtonRenderer
    {
        //Make this per-thread, so that different threads can safely use these methods.
        [ThreadStatic]
        private static VisualStyleRenderer t_visualStyleRenderer = null;
        private static readonly VisualStyleElement s_buttonElement = VisualStyleElement.Button.PushButton.Normal;

        /// <summary>
        ///  If this property is true, then the renderer will use the setting from Application.RenderWithVisualStyles to
        ///  determine how to render.
        ///  If this property is false, the renderer will always render with visualstyles.
        /// </summary>
        public static bool RenderMatchingApplicationState { get; set; } = true;

        private static bool RenderWithVisualStyles
        {
            get
            {
                return (!RenderMatchingApplicationState || Application.RenderWithVisualStyles);
            }
        }

        /// <summary>
        ///  Returns true if the background corresponding to the given state is partially transparent, else false.
        /// </summary>
        public static bool IsBackgroundPartiallyTransparent(PushButtonState state)
        {
            if (RenderWithVisualStyles)
            {
                InitializeRenderer((int)state);

                return t_visualStyleRenderer.IsBackgroundPartiallyTransparent();
            }
            else
            {
                return false; //for downlevel, this is false
            }
        }

        /// <summary>
        ///  This is just a convenience wrapper for VisualStyleRenderer.DrawThemeParentBackground. For downlevel,
        ///  this isn't required and does nothing.
        /// </summary>
        public static void DrawParentBackground(Graphics g, Rectangle bounds, Control childControl)
            => DrawParentBackground((IDeviceContext)g, bounds, childControl);

        internal static void DrawParentBackground(IDeviceContext dc, Rectangle bounds, Control childControl)
        {
            if (RenderWithVisualStyles)
            {
                InitializeRenderer(0);
                t_visualStyleRenderer.DrawParentBackground(dc, bounds, childControl);
            }
        }

        /// <summary>
        ///  Renders a Button control.
        /// </summary>
        public static void DrawButton(Graphics g, Rectangle bounds, PushButtonState state)
            => DrawButton((IDeviceContext)g, bounds, state);

        internal static void DrawButton(IDeviceContext deviceContext, Rectangle bounds, PushButtonState state)
        {
            if (RenderWithVisualStyles)
            {
                InitializeRenderer((int)state);
                t_visualStyleRenderer.DrawBackground(deviceContext, bounds);
            }
            else
            {
                Graphics graphics = deviceContext.TryGetGraphics(create: true);
                ControlPaint.DrawButton(graphics, bounds, ConvertToButtonState(state));
            }
        }

        /// <summary>
        ///  Method to draw visualstyle themes in case of per-monitor scenarios where Hwnd is necessary
        /// </summary>
        /// <param name="hwnd"> handle to the control</param>
        internal static void DrawButtonForHandle(
            IDeviceContext deviceContext,
            Rectangle bounds,
            bool focused,
            PushButtonState state,
            IntPtr hwnd)
        {
            Rectangle contentBounds;

            Graphics g = deviceContext.TryGetGraphics(create: true);

            if (RenderWithVisualStyles)
            {
                InitializeRenderer((int)state);

                using var hdc = new DeviceContextHdcScope(deviceContext);
                t_visualStyleRenderer.DrawBackground(hdc, bounds, hwnd);
                contentBounds = t_visualStyleRenderer.GetBackgroundContentRectangle(hdc, bounds);
            }
            else
            {
                Graphics graphics = deviceContext.TryGetGraphics(create: true);
                ControlPaint.DrawButton(graphics, bounds, ConvertToButtonState(state));
                contentBounds = Rectangle.Inflate(bounds, -3, -3);
            }

            if (focused)
            {
                Graphics graphics = deviceContext.TryGetGraphics(create: true);
                ControlPaint.DrawFocusRectangle(graphics, contentBounds);
            }
        }

        /// <summary>
        ///  Renders a Button control.
        /// </summary>
        public static void DrawButton(Graphics g, Rectangle bounds, bool focused, PushButtonState state)
            => DrawButtonForHandle(g, bounds, focused, state, IntPtr.Zero);

        /// <summary>
        ///  Renders a Button control.
        /// </summary>
        public static void DrawButton(Graphics g, Rectangle bounds, string buttonText, Font font, bool focused, PushButtonState state)
        {
            DrawButton(
                g,
                bounds,
                buttonText,
                font,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine,
                focused,
                state);
        }

        /// <summary>
        ///  Renders a Button control.
        /// </summary>
        public static void DrawButton(Graphics g, Rectangle bounds, string buttonText, Font font, TextFormatFlags flags, bool focused, PushButtonState state)
        {
            Rectangle contentBounds;
            Color textColor;

            if (RenderWithVisualStyles)
            {
                InitializeRenderer((int)state);

                t_visualStyleRenderer.DrawBackground(g, bounds);
                contentBounds = t_visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
                textColor = t_visualStyleRenderer.GetColor(ColorProperty.TextColor);
            }
            else
            {
                ControlPaint.DrawButton(g, bounds, ConvertToButtonState(state));
                contentBounds = Rectangle.Inflate(bounds, -3, -3);
                textColor = SystemColors.ControlText;
            }

            TextRenderer.DrawText(g, buttonText, font, contentBounds, textColor, flags);

            if (focused)
            {
                ControlPaint.DrawFocusRectangle(g, contentBounds);
            }
        }

        /// <summary>
        ///  Renders a Button control.
        /// </summary>
        public static void DrawButton(Graphics g, Rectangle bounds, Image image, Rectangle imageBounds, bool focused, PushButtonState state)
        {
            Rectangle contentBounds;

            if (RenderWithVisualStyles)
            {
                InitializeRenderer((int)state);

                t_visualStyleRenderer.DrawBackground(g, bounds);
                t_visualStyleRenderer.DrawImage(g, imageBounds, image);
                contentBounds = t_visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
            }
            else
            {
                ControlPaint.DrawButton(g, bounds, ConvertToButtonState(state));
                g.DrawImage(image, imageBounds);
                contentBounds = Rectangle.Inflate(bounds, -3, -3);
            }

            if (focused)
            {
                ControlPaint.DrawFocusRectangle(g, contentBounds);
            }
        }

        /// <summary>
        ///  Renders a Button control.
        /// </summary>
        public static void DrawButton(
            Graphics g,
            Rectangle bounds,
            string buttonText,
            Font font,
            Image image,
            Rectangle imageBounds,
            bool focused,
            PushButtonState state)
        {
            DrawButton(
                g,
                bounds,
                buttonText,
                font,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine,
                image,
                imageBounds,
                focused,
                state);
        }

        /// <summary>
        ///  Renders a Button control.
        /// </summary>
        public static void DrawButton(
            Graphics g,
            Rectangle bounds,
            string buttonText,
            Font font,
            TextFormatFlags flags,
            Image image,
            Rectangle imageBounds,
            bool focused,
            PushButtonState state)
            => DrawButton((IDeviceContext)g, bounds, buttonText, font, flags, image, imageBounds, focused, state);

        internal static void DrawButton(
            IDeviceContext deviceContext,
            Rectangle bounds,
            string buttonText,
            Font font,
            TextFormatFlags flags,
            Image image,
            Rectangle imageBounds,
            bool focused,
            PushButtonState state)
        {
            Rectangle contentBounds;
            Color textColor;

            Graphics graphics = deviceContext.TryGetGraphics(create: true);

            if (RenderWithVisualStyles)
            {
                InitializeRenderer((int)state);

                t_visualStyleRenderer.DrawBackground(deviceContext, bounds);
                t_visualStyleRenderer.DrawImage(graphics, imageBounds, image);
                contentBounds = t_visualStyleRenderer.GetBackgroundContentRectangle(deviceContext, bounds);
                textColor = t_visualStyleRenderer.GetColor(ColorProperty.TextColor);
            }
            else
            {
                ControlPaint.DrawButton(graphics, bounds, ConvertToButtonState(state));
                graphics.DrawImage(image, imageBounds);
                contentBounds = Rectangle.Inflate(bounds, -3, -3);
                textColor = SystemColors.ControlText;
            }

            TextRenderer.DrawText(deviceContext, buttonText, font, contentBounds, textColor, flags);

            if (focused)
            {
                ControlPaint.DrawFocusRectangle(graphics, contentBounds);
            }
        }

        internal static ButtonState ConvertToButtonState(PushButtonState state) => state switch
        {
            PushButtonState.Pressed => ButtonState.Pushed,
            PushButtonState.Disabled => ButtonState.Inactive,
            _ => ButtonState.Normal,
        };

        private static void InitializeRenderer(int state)
        {
            if (t_visualStyleRenderer is null)
            {
                t_visualStyleRenderer = new VisualStyleRenderer(s_buttonElement.ClassName, s_buttonElement.Part, state);
            }
            else
            {
                t_visualStyleRenderer.SetParameters(s_buttonElement.ClassName, s_buttonElement.Part, state);
            }
        }
    }
}

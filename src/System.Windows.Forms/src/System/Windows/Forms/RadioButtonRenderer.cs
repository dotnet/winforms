﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This is a rendering class for the RadioButton control. It works downlevel too (obviously
    ///  without visual styles applied.)
    /// </summary>
    public static class RadioButtonRenderer
    {
        // Make this per-thread, so that different threads can safely use these methods.
        [ThreadStatic]
        private static VisualStyleRenderer? t_visualStyleRenderer;

        private static readonly VisualStyleElement s_radioElement = VisualStyleElement.Button.RadioButton.UncheckedNormal;

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
                return !RenderMatchingApplicationState || Application.RenderWithVisualStyles;
            }
        }

        /// <summary>
        ///  Returns true if the background corresponding to the given state is partially transparent, else false.
        /// </summary>
        public static bool IsBackgroundPartiallyTransparent(RadioButtonState state)
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
        {
            if (RenderWithVisualStyles)
            {
                InitializeRenderer(0);

                t_visualStyleRenderer.DrawParentBackground(g, bounds, childControl);
            }
        }

        /// <summary>
        ///  Renders a RadioButton control.
        /// </summary>
        public static void DrawRadioButton(Graphics g, Point glyphLocation, RadioButtonState state)
        {
            DrawRadioButton(g, glyphLocation, state, HWND.Null);
        }

        internal static void DrawRadioButtonWithVisualStyles(
            HDC hdc,
            Point glyphLocation,
            RadioButtonState state,
            HWND hwnd)
        {
            InitializeRenderer((int)state);
            Rectangle glyphBounds = new Rectangle(glyphLocation, GetGlyphSize(hdc, state, hwnd));
            t_visualStyleRenderer.DrawBackground(hdc, glyphBounds, hwnd);
        }

        internal static void DrawRadioButton(
            Graphics graphics,
            Point glyphLocation,
            RadioButtonState state,
            HWND hwnd)
        {
            Rectangle glyphBounds;
            if (RenderWithVisualStyles)
            {
                using var hdc = new DeviceContextHdcScope(graphics);
                DrawRadioButtonWithVisualStyles(hdc, glyphLocation, state, hwnd);
            }
            else
            {
                using (var hdc = new DeviceContextHdcScope(graphics))
                {
                    glyphBounds = new Rectangle(glyphLocation, GetGlyphSize(hdc, state, hwnd));
                }

                ControlPaint.DrawRadioButton(graphics, glyphBounds, ConvertToButtonState(state));
            }
        }

        /// <summary>
        ///  Renders a RadioButton control.
        /// </summary>
        public static void DrawRadioButton(
            Graphics g,
            Point glyphLocation,
            Rectangle textBounds,
            string? radioButtonText,
            Font? font,
            bool focused,
            RadioButtonState state)
        {
            DrawRadioButton(g, glyphLocation, textBounds, radioButtonText, font,
                       TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine,
                       focused, state);
        }

        /// <summary>
        ///  Renders a RadioButton control.
        /// </summary>
        public static void DrawRadioButton(
            Graphics g,
            Point glyphLocation,
            Rectangle textBounds,
            string? radioButtonText,
            Font? font,
            TextFormatFlags flags,
            bool focused,
            RadioButtonState state)
        {
            DrawRadioButton(g, glyphLocation, textBounds, radioButtonText, font, flags, focused, state, HWND.Null);
        }

        internal static void DrawRadioButton(
            Graphics g,
            Point glyphLocation,
            Rectangle textBounds,
            string? radioButtonText,
            Font? font,
            TextFormatFlags flags,
            bool focused,
            RadioButtonState state,
            HWND hwnd)
        {
            Rectangle glyphBounds;
            using (var hdc = new DeviceContextHdcScope(g))
            {
                glyphBounds = new Rectangle(glyphLocation, GetGlyphSize(hdc, state, hwnd));
            }

            Color textColor;

            if (RenderWithVisualStyles)
            {
                InitializeRenderer((int)state);

                t_visualStyleRenderer.DrawBackground(g, glyphBounds);
                textColor = t_visualStyleRenderer.GetColor(ColorProperty.TextColor);
            }
            else
            {
                ControlPaint.DrawRadioButton(g, glyphBounds, ConvertToButtonState(state));
                textColor = SystemColors.ControlText;
            }

            TextRenderer.DrawText(g, radioButtonText, font, textBounds, textColor, flags);

            if (focused)
            {
                ControlPaint.DrawFocusRectangle(g, textBounds);
            }
        }

        /// <summary>
        ///  Renders a RadioButton control.
        /// </summary>
        public static void DrawRadioButton(
            Graphics g,
            Point glyphLocation,
            Rectangle textBounds,
            string? radioButtonText,
            Font? font,
            Image image,
            Rectangle imageBounds,
            bool focused,
            RadioButtonState state) => DrawRadioButton(
                g,
                glyphLocation,
                textBounds,
                radioButtonText,
                font,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine,
                image,
                imageBounds,
                focused,
                state);

        /// <summary>
        ///  Renders a RadioButton control.
        /// </summary>
        public static void DrawRadioButton(
            Graphics g,
            Point glyphLocation,
            Rectangle textBounds,
            string? radioButtonText,
            Font? font,
            TextFormatFlags flags,
            Image image,
            Rectangle imageBounds,
            bool focused,
            RadioButtonState state) => DrawRadioButton(
                g,
                glyphLocation,
                textBounds,
                radioButtonText,
                font,
                flags,
                image,
                imageBounds,
                focused,
                state,
                HWND.Null);

        internal static void DrawRadioButton(
            Graphics g,
            Point glyphLocation,
            Rectangle textBounds,
            string? radioButtonText,
            Font? font,
            TextFormatFlags flags,
            Image image,
            Rectangle imageBounds,
            bool focused,
            RadioButtonState state,
            HWND hwnd)
        {
            Rectangle glyphBounds;
            using (var hdc = new DeviceContextHdcScope(g))
            {
                glyphBounds = new Rectangle(glyphLocation, GetGlyphSize(hdc, state, hwnd));
            }

            Color textColor;

            if (RenderWithVisualStyles)
            {
                InitializeRenderer((int)state);

                // Keep this drawing order! It matches default drawing order.
                t_visualStyleRenderer.DrawImage(g, imageBounds, image);
                t_visualStyleRenderer.DrawBackground(g, glyphBounds);
                textColor = t_visualStyleRenderer.GetColor(ColorProperty.TextColor);
            }
            else
            {
                g.DrawImage(image, imageBounds);
                ControlPaint.DrawRadioButton(g, glyphBounds, ConvertToButtonState(state));
                textColor = SystemColors.ControlText;
            }

            TextRenderer.DrawText(g, radioButtonText, font, textBounds, textColor, flags);

            if (focused)
            {
                ControlPaint.DrawFocusRectangle(g, textBounds);
            }
        }

        /// <summary>
        ///  Returns the size of the RadioButton glyph.
        /// </summary>
        public static Size GetGlyphSize(Graphics g, RadioButtonState state)
        {
            using var hdc = new DeviceContextHdcScope(g);
            return GetGlyphSize(hdc, state, HWND.Null);
        }

        internal static Size GetGlyphSize(HDC hdc, RadioButtonState state, HWND hwnd)
        {
            if (RenderWithVisualStyles)
            {
                InitializeRenderer((int)state);

                return t_visualStyleRenderer.GetPartSize(hdc, ThemeSizeType.Draw, hwnd);
            }

            return new Size(13, 13);
        }

        internal static ButtonState ConvertToButtonState(RadioButtonState state) => state switch
        {
            RadioButtonState.CheckedNormal or RadioButtonState.CheckedHot => ButtonState.Checked,
            RadioButtonState.CheckedPressed => ButtonState.Checked | ButtonState.Pushed,
            RadioButtonState.CheckedDisabled => ButtonState.Checked | ButtonState.Inactive,
            RadioButtonState.UncheckedPressed => ButtonState.Pushed,
            RadioButtonState.UncheckedDisabled => ButtonState.Inactive,
            _ => ButtonState.Normal,
        };

        internal static RadioButtonState ConvertFromButtonState(ButtonState state, bool isHot)
        {
            if ((state & ButtonState.Checked) == ButtonState.Checked)
            {
                if ((state & ButtonState.Pushed) == ButtonState.Pushed)
                {
                    return RadioButtonState.CheckedPressed;
                }
                else if ((state & ButtonState.Inactive) == ButtonState.Inactive)
                {
                    return RadioButtonState.CheckedDisabled;
                }
                else if (isHot)
                {
                    return RadioButtonState.CheckedHot;
                }

                return RadioButtonState.CheckedNormal;
            }
            else
            {
                // Unchecked
                if ((state & ButtonState.Pushed) == ButtonState.Pushed)
                {
                    return RadioButtonState.UncheckedPressed;
                }
                else if ((state & ButtonState.Inactive) == ButtonState.Inactive)
                {
                    return RadioButtonState.UncheckedDisabled;
                }
                else if (isHot)
                {
                    return RadioButtonState.UncheckedHot;
                }

                return RadioButtonState.UncheckedNormal;
            }
        }

        [MemberNotNull(nameof(t_visualStyleRenderer))]
        private static void InitializeRenderer(int state)
        {
            RadioButtonState radioButtonState = (RadioButtonState)state;
            int part = s_radioElement.Part;
            if (SystemInformation.HighContrast
                && (radioButtonState == RadioButtonState.CheckedDisabled || radioButtonState == RadioButtonState.UncheckedDisabled)
                && VisualStyleRenderer.IsCombinationDefined(s_radioElement.ClassName, VisualStyleElement.Button.RadioButton.HighContrastDisabledPart))
            {
                part = VisualStyleElement.Button.RadioButton.HighContrastDisabledPart;
            }

            if (t_visualStyleRenderer is null)
            {
                t_visualStyleRenderer = new VisualStyleRenderer(s_radioElement.ClassName, part, state);
            }
            else
            {
                t_visualStyleRenderer.SetParameters(s_radioElement.ClassName, part, state);
            }
        }
    }
}

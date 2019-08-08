// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This is a rendering class for the CheckBox control. It works downlevel too (obviously
    ///  without visual styles applied.)
    /// </summary>
    public sealed class CheckBoxRenderer
    {
        //Make this per-thread, so that different threads can safely use these methods.
        [ThreadStatic]
        private static VisualStyleRenderer visualStyleRenderer = null;
        private static readonly VisualStyleElement CheckBoxElement = VisualStyleElement.Button.CheckBox.UncheckedNormal;
        private static bool renderMatchingApplicationState = true;

        //cannot instantiate
        private CheckBoxRenderer()
        {
        }

        /// <summary>
        ///  If this property is true, then the renderer will use the setting from Application.RenderWithVisualStyles to
        ///  determine how to render.
        ///  If this property is false, the renderer will always render with visualstyles.
        /// </summary>
        public static bool RenderMatchingApplicationState
        {
            get
            {
                return renderMatchingApplicationState;
            }
            set
            {
                renderMatchingApplicationState = value;
            }
        }

        private static bool RenderWithVisualStyles
        {
            get
            {
                return (!renderMatchingApplicationState || Application.RenderWithVisualStyles);
            }
        }

        /// <summary>
        ///  Returns true if the background corresponding to the given state is partially transparent, else false.
        /// </summary>
        public static bool IsBackgroundPartiallyTransparent(CheckBoxState state)
        {
            if (RenderWithVisualStyles)
            {
                InitializeRenderer((int)state);

                return visualStyleRenderer.IsBackgroundPartiallyTransparent();
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

                visualStyleRenderer.DrawParentBackground(g, bounds, childControl);
            }
        }

        /// <summary>
        ///  Renders a CheckBox control.
        /// </summary>
        public static void DrawCheckBox(Graphics g, Point glyphLocation, CheckBoxState state)
        {
            DrawCheckBox(g, glyphLocation, state, IntPtr.Zero);
        }

        internal static void DrawCheckBox(Graphics g, Point glyphLocation, CheckBoxState state, IntPtr hWnd)
        {
            Rectangle glyphBounds = new Rectangle(glyphLocation, GetGlyphSize(g, state, hWnd));

            if (RenderWithVisualStyles)
            {
                InitializeRenderer((int)state);

                visualStyleRenderer.DrawBackground(g, glyphBounds, hWnd);
            }
            else
            {
                if (IsMixed(state))
                {
                    ControlPaint.DrawMixedCheckBox(g, glyphBounds, ConvertToButtonState(state));
                }
                else
                {
                    ControlPaint.DrawCheckBox(g, glyphBounds, ConvertToButtonState(state));
                }
            }

        }

        /// <summary>
        ///  Renders a CheckBox control.
        /// </summary>
        public static void DrawCheckBox(Graphics g, Point glyphLocation, Rectangle textBounds, string checkBoxText, Font font, bool focused, CheckBoxState state)
        {
            DrawCheckBox(g, glyphLocation, textBounds, checkBoxText, font,
                       TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine,
                       focused, state);
        }

        /// <summary>
        ///  Renders a CheckBox control.
        /// </summary>
        public static void DrawCheckBox(Graphics g, Point glyphLocation, Rectangle textBounds, string checkBoxText, Font font, TextFormatFlags flags, bool focused, CheckBoxState state)
        {
            DrawCheckBox(g, glyphLocation, textBounds, checkBoxText, font, flags, focused, state, IntPtr.Zero);
        }

        internal static void DrawCheckBox(Graphics g, Point glyphLocation, Rectangle textBounds, string checkBoxText, Font font, TextFormatFlags flags, bool focused, CheckBoxState state, IntPtr hWnd)
        {
            Rectangle glyphBounds = new Rectangle(glyphLocation, GetGlyphSize(g, state, hWnd));
            Color textColor;

            if (RenderWithVisualStyles)
            {
                InitializeRenderer((int)state);

                visualStyleRenderer.DrawBackground(g, glyphBounds);
                textColor = visualStyleRenderer.GetColor(ColorProperty.TextColor);
            }
            else
            {
                if (IsMixed(state))
                {
                    ControlPaint.DrawMixedCheckBox(g, glyphBounds, ConvertToButtonState(state));
                }
                else
                {
                    ControlPaint.DrawCheckBox(g, glyphBounds, ConvertToButtonState(state));
                }

                textColor = SystemColors.ControlText;
            }

            TextRenderer.DrawText(g, checkBoxText, font, textBounds, textColor, flags);

            if (focused)
            {
                ControlPaint.DrawFocusRectangle(g, textBounds);
            }
        }

        /// <summary>
        ///  Renders a CheckBox control.
        /// </summary>
        public static void DrawCheckBox(Graphics g, Point glyphLocation, Rectangle textBounds, string checkBoxText, Font font, Image image, Rectangle imageBounds, bool focused, CheckBoxState state)
        {
            DrawCheckBox(g, glyphLocation, textBounds, checkBoxText, font,
                       TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine,
                       image, imageBounds, focused, state);
        }

        /// <summary>
        ///  Renders a CheckBox control.
        /// </summary>
        public static void DrawCheckBox(Graphics g, Point glyphLocation, Rectangle textBounds, string checkBoxText, Font font, TextFormatFlags flags, Image image, Rectangle imageBounds, bool focused, CheckBoxState state)
        {
            Rectangle glyphBounds = new Rectangle(glyphLocation, GetGlyphSize(g, state));
            Color textColor;

            if (RenderWithVisualStyles)
            {
                InitializeRenderer((int)state);

                //Keep this drawing order! It matches default drawing order.
                visualStyleRenderer.DrawImage(g, imageBounds, image);
                visualStyleRenderer.DrawBackground(g, glyphBounds);
                textColor = visualStyleRenderer.GetColor(ColorProperty.TextColor);
            }
            else
            {
                g.DrawImage(image, imageBounds);
                if (IsMixed(state))
                {
                    ControlPaint.DrawMixedCheckBox(g, glyphBounds, ConvertToButtonState(state));
                }
                else
                {
                    ControlPaint.DrawCheckBox(g, glyphBounds, ConvertToButtonState(state));
                }

                textColor = SystemColors.ControlText;
            }

            TextRenderer.DrawText(g, checkBoxText, font, textBounds, textColor, flags);

            if (focused)
            {
                ControlPaint.DrawFocusRectangle(g, textBounds);
            }
        }

        /// <summary>
        ///  Returns the size of the CheckBox glyph.
        /// </summary>
        public static Size GetGlyphSize(Graphics g, CheckBoxState state)
        {
            return GetGlyphSize(g, state, IntPtr.Zero);
        }

        internal static Size GetGlyphSize(Graphics g, CheckBoxState state, IntPtr hWnd)
        {
            if (RenderWithVisualStyles)
            {
                InitializeRenderer((int)state);

                return visualStyleRenderer.GetPartSize(g, ThemeSizeType.Draw, hWnd);
            }

            return new Size(13, 13);
        }

        internal static ButtonState ConvertToButtonState(CheckBoxState state)
        {
            switch (state)
            {

                case CheckBoxState.CheckedNormal:
                case CheckBoxState.CheckedHot:
                    return ButtonState.Checked;
                case CheckBoxState.CheckedPressed:
                    return (ButtonState.Checked | ButtonState.Pushed);
                case CheckBoxState.CheckedDisabled:
                    return (ButtonState.Checked | ButtonState.Inactive);

                case CheckBoxState.UncheckedPressed:
                    return ButtonState.Pushed;
                case CheckBoxState.UncheckedDisabled:
                    return ButtonState.Inactive;

                //Downlevel mixed drawing works only if ButtonState.Checked is set
                case CheckBoxState.MixedNormal:
                case CheckBoxState.MixedHot:
                    return ButtonState.Checked;
                case CheckBoxState.MixedPressed:
                    return (ButtonState.Checked | ButtonState.Pushed);
                case CheckBoxState.MixedDisabled:
                    return (ButtonState.Checked | ButtonState.Inactive);

                default:
                    return ButtonState.Normal;
            }
        }

        internal static CheckBoxState ConvertFromButtonState(ButtonState state, bool isMixed, bool isHot)
        {
            if (isMixed)
            {
                if ((state & ButtonState.Pushed) == ButtonState.Pushed)
                {
                    return CheckBoxState.MixedPressed;
                }
                else if ((state & ButtonState.Inactive) == ButtonState.Inactive)
                {
                    return CheckBoxState.MixedDisabled;
                }
                else if (isHot)
                {
                    return CheckBoxState.MixedHot;
                }

                return CheckBoxState.MixedNormal;
            }
            else if ((state & ButtonState.Checked) == ButtonState.Checked)
            {
                if ((state & ButtonState.Pushed) == ButtonState.Pushed)
                {
                    return CheckBoxState.CheckedPressed;
                }
                else if ((state & ButtonState.Inactive) == ButtonState.Inactive)
                {
                    return CheckBoxState.CheckedDisabled;
                }
                else if (isHot)
                {
                    return CheckBoxState.CheckedHot;
                }

                return CheckBoxState.CheckedNormal;
            }
            else
            { //unchecked
                if ((state & ButtonState.Pushed) == ButtonState.Pushed)
                {
                    return CheckBoxState.UncheckedPressed;
                }
                else if ((state & ButtonState.Inactive) == ButtonState.Inactive)
                {
                    return CheckBoxState.UncheckedDisabled;
                }
                else if (isHot)
                {
                    return CheckBoxState.UncheckedHot;
                }

                return CheckBoxState.UncheckedNormal;
            }
        }

        private static bool IsMixed(CheckBoxState state)
        {
            switch (state)
            {

                case CheckBoxState.MixedNormal:
                case CheckBoxState.MixedHot:
                case CheckBoxState.MixedPressed:
                case CheckBoxState.MixedDisabled:
                    return true;

                default:
                    return false;
            }
        }

        private static bool IsDisabled(CheckBoxState state)
        {
            switch (state)
            {
                case CheckBoxState.CheckedDisabled:
                case CheckBoxState.UncheckedDisabled:
                case CheckBoxState.MixedDisabled:
                    return true;

                default:
                    return false;
            }
        }

        private static void InitializeRenderer(int state)
        {
            int part = CheckBoxElement.Part;
            if (SystemInformation.HighContrast
                && IsDisabled((CheckBoxState)state)
                && VisualStyleRenderer.IsCombinationDefined(CheckBoxElement.ClassName, VisualStyleElement.Button.CheckBox.HighContrastDisabledPart))
            {
                part = VisualStyleElement.Button.CheckBox.HighContrastDisabledPart;
            }

            if (visualStyleRenderer == null)
            {
                visualStyleRenderer = new VisualStyleRenderer(CheckBoxElement.ClassName, part, state);
            }
            else
            {
                visualStyleRenderer.SetParameters(CheckBoxElement.ClassName, part, state);
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This is a rendering class for the TextBox control.
    /// </summary>
    public sealed class TextBoxRenderer
    {
        //Make this per-thread, so that different threads can safely use these methods.
        [ThreadStatic]
        private static VisualStyleRenderer visualStyleRenderer = null;
        private static readonly VisualStyleElement TextBoxElement = VisualStyleElement.TextBox.TextEdit.Normal;

        //cannot instantiate
        private TextBoxRenderer()
        {
        }

        /// <summary>
        ///  Returns true if this class is supported for the current OS and user/application settings,
        ///  otherwise returns false.
        /// </summary>
        public static bool IsSupported
        {
            get
            {
                return VisualStyleRenderer.IsSupported; // no downlevel support
            }
        }

        private static void DrawBackground(Graphics g, Rectangle bounds, TextBoxState state)
        {
            visualStyleRenderer.DrawBackground(g, bounds);
            if (state != TextBoxState.Disabled)
            {
                Color windowColor = visualStyleRenderer.GetColor(ColorProperty.FillColor);
                if (windowColor != SystemColors.Window)
                {
                    Rectangle fillRect = visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
                    //then we need to re-fill the background.
                    using (SolidBrush brush = new SolidBrush(SystemColors.Window))
                    {
                        g.FillRectangle(brush, fillRect);
                    }
                }
            }
        }

        /// <summary>
        ///  Renders a TextBox control.
        /// </summary>
        public static void DrawTextBox(Graphics g, Rectangle bounds, TextBoxState state)
        {
            InitializeRenderer((int)state);
            DrawBackground(g, bounds, state);
        }

        /// <summary>
        ///  Renders a TextBox control.
        /// </summary>
        public static void DrawTextBox(Graphics g, Rectangle bounds, string textBoxText, Font font, TextBoxState state)
        {
            DrawTextBox(g, bounds, textBoxText, font, TextFormatFlags.TextBoxControl, state);
        }

        /// <summary>
        ///  Renders a TextBox control.
        /// </summary>
        public static void DrawTextBox(Graphics g, Rectangle bounds, string textBoxText, Font font, Rectangle textBounds, TextBoxState state)
        {
            DrawTextBox(g, bounds, textBoxText, font, textBounds, TextFormatFlags.TextBoxControl, state);
        }

        /// <summary>
        ///  Renders a TextBox control.
        /// </summary>
        public static void DrawTextBox(Graphics g, Rectangle bounds, string textBoxText, Font font, TextFormatFlags flags, TextBoxState state)
        {
            InitializeRenderer((int)state);
            Rectangle textBounds = visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
            textBounds.Inflate(-2, -2);
            DrawTextBox(g, bounds, textBoxText, font, textBounds, flags, state);
        }

        /// <summary>
        ///  Renders a TextBox control.
        /// </summary>
        public static void DrawTextBox(Graphics g, Rectangle bounds, string textBoxText, Font font, Rectangle textBounds, TextFormatFlags flags, TextBoxState state)
        {
            InitializeRenderer((int)state);

            DrawBackground(g, bounds, state);
            Color textColor = visualStyleRenderer.GetColor(ColorProperty.TextColor);
            TextRenderer.DrawText(g, textBoxText, font, textBounds, textColor, flags);
        }

        private static void InitializeRenderer(int state)
        {
            if (visualStyleRenderer == null)
            {
                visualStyleRenderer = new VisualStyleRenderer(TextBoxElement.ClassName, TextBoxElement.Part, state);
            }
            else
            {
                visualStyleRenderer.SetParameters(TextBoxElement.ClassName, TextBoxElement.Part, state);
            }
        }
    }
}

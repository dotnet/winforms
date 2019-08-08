// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This is a rendering class for the ComboBox control.
    /// </summary>
    public sealed class ComboBoxRenderer
    {
        //Make this per-thread, so that different threads can safely use these methods.
        [ThreadStatic]
        private static VisualStyleRenderer visualStyleRenderer = null;
        private static readonly VisualStyleElement ComboBoxElement = VisualStyleElement.ComboBox.DropDownButton.Normal;
        private static readonly VisualStyleElement TextBoxElement = VisualStyleElement.TextBox.TextEdit.Normal;

        //cannot instantiate
        private ComboBoxRenderer()
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

        private static void DrawBackground(Graphics g, Rectangle bounds, ComboBoxState state)
        {
            visualStyleRenderer.DrawBackground(g, bounds);
            //for disabled comboboxes, comctl does not use the window backcolor, so
            // we don't refill here in that case.
            if (state != ComboBoxState.Disabled)
            {
                Color windowColor = visualStyleRenderer.GetColor(ColorProperty.FillColor);
                if (windowColor != SystemColors.Window)
                {
                    Rectangle fillRect = visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
                    fillRect.Inflate(-2, -2);
                    //then we need to re-fill the background.
                    g.FillRectangle(SystemBrushes.Window, fillRect);
                }
            }
        }

        /// <summary>
        ///  Renders the textbox part of a ComboBox control.
        /// </summary>
        public static void DrawTextBox(Graphics g, Rectangle bounds, ComboBoxState state)
        {
            if (visualStyleRenderer == null)
            {
                visualStyleRenderer = new VisualStyleRenderer(TextBoxElement.ClassName, TextBoxElement.Part, (int)state);
            }
            else
            {
                visualStyleRenderer.SetParameters(TextBoxElement.ClassName, TextBoxElement.Part, (int)state);
            }

            DrawBackground(g, bounds, state);
        }

        /// <summary>
        ///  Renders the textbox part of a ComboBox control.
        /// </summary>
        public static void DrawTextBox(Graphics g, Rectangle bounds, string comboBoxText, Font font, ComboBoxState state)
        {
            DrawTextBox(g, bounds, comboBoxText, font, TextFormatFlags.TextBoxControl, state);
        }

        /// <summary>
        ///  Renders the textbox part of a ComboBox control.
        /// </summary>
        public static void DrawTextBox(Graphics g, Rectangle bounds, string comboBoxText, Font font, Rectangle textBounds, ComboBoxState state)
        {
            DrawTextBox(g, bounds, comboBoxText, font, textBounds, TextFormatFlags.TextBoxControl, state);
        }

        /// <summary>
        ///  Renders the textbox part of a ComboBox control.
        /// </summary>
        public static void DrawTextBox(Graphics g, Rectangle bounds, string comboBoxText, Font font, TextFormatFlags flags, ComboBoxState state)
        {
            if (visualStyleRenderer == null)
            {
                visualStyleRenderer = new VisualStyleRenderer(TextBoxElement.ClassName, TextBoxElement.Part, (int)state);
            }
            else
            {
                visualStyleRenderer.SetParameters(TextBoxElement.ClassName, TextBoxElement.Part, (int)state);
            }

            Rectangle textBounds = visualStyleRenderer.GetBackgroundContentRectangle(g, bounds);
            textBounds.Inflate(-2, -2);
            DrawTextBox(g, bounds, comboBoxText, font, textBounds, flags, state);
        }

        /// <summary>
        ///  Renders the textbox part of a ComboBox control.
        /// </summary>
        public static void DrawTextBox(Graphics g, Rectangle bounds, string comboBoxText, Font font, Rectangle textBounds, TextFormatFlags flags, ComboBoxState state)
        {
            if (visualStyleRenderer == null)
            {
                visualStyleRenderer = new VisualStyleRenderer(TextBoxElement.ClassName, TextBoxElement.Part, (int)state);
            }
            else
            {
                visualStyleRenderer.SetParameters(TextBoxElement.ClassName, TextBoxElement.Part, (int)state);
            }

            DrawBackground(g, bounds, state);
            Color textColor = visualStyleRenderer.GetColor(ColorProperty.TextColor);
            TextRenderer.DrawText(g, comboBoxText, font, textBounds, textColor, flags);
        }

        /// <summary>
        ///  Renders a ComboBox drop-down button.
        /// </summary>
        public static void DrawDropDownButton(Graphics g, Rectangle bounds, ComboBoxState state)
        {
            DrawDropDownButtonForHandle(g, bounds, state, IntPtr.Zero);
        }

        /// <summary>
        ///  Renders a ComboBox drop-down button in per-monitor scenario.
        /// </summary>
        /// <param name="g">graphics object</param>
        /// <param name="bounds">dropdown button bounds</param>
        /// <param name="state"> state</param>
        /// <param name="handle"> handle of the control</param>
        internal static void DrawDropDownButtonForHandle(Graphics g, Rectangle bounds, ComboBoxState state, IntPtr handle)
        {
            if (visualStyleRenderer == null)
            {
                visualStyleRenderer = new VisualStyleRenderer(ComboBoxElement.ClassName, ComboBoxElement.Part, (int)state);
            }
            else
            {
                visualStyleRenderer.SetParameters(ComboBoxElement.ClassName, ComboBoxElement.Part, (int)state);
            }

            visualStyleRenderer.DrawBackground(g, bounds, handle);
        }
    }
}

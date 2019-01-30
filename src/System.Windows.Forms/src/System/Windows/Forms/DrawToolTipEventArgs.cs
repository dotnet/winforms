// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// This class contains the information a user needs to paint the ToolTip.
    /// </devdoc>
    public class DrawToolTipEventArgs : EventArgs
    {
        private readonly Color _backColor;
        private readonly Color _foreColor;

        /// <devdoc>
        /// Creates a new DrawToolTipEventArgs with the given parameters.
        /// </devdoc>
        public DrawToolTipEventArgs(Graphics graphics, IWin32Window associatedWindow, Control associatedControl, Rectangle bounds,
                                    string toolTipText, Color backColor, Color foreColor, Font font)
        {
            Graphics = graphics;
            AssociatedWindow = associatedWindow;
            AssociatedControl = associatedControl;
            Bounds = bounds;
            ToolTipText = toolTipText;
            _backColor = backColor;
            _foreColor = foreColor;
            Font = font;
        }

        /// <devdoc>
        /// Graphics object with which painting should be done.
        /// </devdoc>
        public Graphics Graphics { get; }

        /// <devdoc>
        /// The window for which the tooltip is being painted.
        /// </devdoc>
        public IWin32Window AssociatedWindow { get; }

        /// <devdoc>
        /// The control for which the tooltip is being painted.
        /// </devdoc>
        public Control AssociatedControl { get; }

        /// <devdoc>
        ///  The rectangle outlining the area in which the painting should be done.
        /// </devdoc>
        public Rectangle Bounds { get; }

        /// <devdoc>
        /// The text that should be drawn.
        /// </devdoc>
        public string ToolTipText { get; }

        /// <devdoc>
        /// The font used to draw tooltip text.
        /// </devdoc>
        public Font Font { get; }

        /// <devdoc>
        /// Draws the background of the ToolTip.
        /// </devdoc>
        public void DrawBackground()
        {
            using (var backBrush = new SolidBrush(_backColor))
            {
                Graphics.FillRectangle(backBrush, Bounds);
            }
        }

        /// <devdoc>
        /// Draws the text (overloaded)
        /// </devdoc>
        public void DrawText()
        {
            // Pass in a set of flags to mimic default behavior
            DrawText(TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.HidePrefix);
        }

        /// <devdoc>
        /// Draws the text (overloaded) - takes a TextFormatFlags argument.
        /// </devdoc>
        public void DrawText(TextFormatFlags flags)
        {
            TextRenderer.DrawText(Graphics, ToolTipText, Font, Bounds, _foreColor, flags);
        }

        /// <devdoc>
        /// Draws a border for the ToolTip similar to the default border.
        /// </devdoc>
        public void DrawBorder()
        {
            ControlPaint.DrawBorder(Graphics, Bounds, SystemColors.WindowFrame, ButtonBorderStyle.Solid);
        }
    }
}

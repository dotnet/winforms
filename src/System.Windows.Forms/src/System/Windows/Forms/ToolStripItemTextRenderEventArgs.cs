// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// This class represents all the information to render the winbar
    /// </devdoc>
    public class ToolStripItemTextRenderEventArgs : ToolStripItemRenderEventArgs
    {
        private ContentAlignment _textAlignment;
        private Color _textColor = SystemColors.ControlText;
        private bool _textColorChanged = false;

        /// <devdoc>
        /// This class represents all the information to render the winbar
        /// </devdoc>
        public ToolStripItemTextRenderEventArgs(Graphics g, ToolStripItem item, string text, Rectangle textRectangle, Color textColor, Font textFont, TextFormatFlags format) : base(g, item)
        {
            Text = text;
            TextRectangle = textRectangle;
            DefaultTextColor = textColor;
            TextFont = textFont;
            _textAlignment = item.TextAlign;
            TextFormat = format;
            TextDirection = item.TextDirection;
        }

        /// <devdoc>
        /// This class represents all the information to render the winbar
        /// </devdoc>
        public ToolStripItemTextRenderEventArgs(Graphics g, ToolStripItem item, string text, Rectangle textRectangle, Color textColor, Font textFont, ContentAlignment textAlign) : base(g, item)
        {
            Text = text;
            TextRectangle = textRectangle;
            DefaultTextColor = textColor;
            TextFont = textFont;
            TextFormat = ToolStripItemInternalLayout.ContentAlignToTextFormat(textAlign, item.RightToLeft == RightToLeft.Yes);
            TextFormat = (item.ShowKeyboardCues) ? TextFormat : TextFormat | TextFormatFlags.HidePrefix;
            TextDirection = item.TextDirection;
        }


        /// <devdoc>
        /// The string to draw
        /// </devdoc>
        public string Text { get; set; }

        /// <devdoc>
        /// The color to draw the text
        /// </devdoc>
        public Color TextColor
        {
            get => _textColorChanged ? _textColor : DefaultTextColor;
            set
            {
                _textColor = value;
                _textColorChanged=true;
            }
        }

        internal Color DefaultTextColor { get; set; }

        /// <devdoc>
        /// The font to draw the text
        /// </devdoc>
        public Font TextFont { get; set; }

        /// <devdoc>
        /// The rectangle to draw the text in
        /// </devdoc>
        public Rectangle TextRectangle { get; set; }

        /// <devdoc>
        /// The rectangle to draw the text in
        /// </devdoc>
        public TextFormatFlags TextFormat { get; set; }

        /// <devdoc>
        /// The angle at which the text should be drawn in tenths of degrees.
        /// </devdoc>
        public ToolStripTextDirection TextDirection { get; set; }
    }
}

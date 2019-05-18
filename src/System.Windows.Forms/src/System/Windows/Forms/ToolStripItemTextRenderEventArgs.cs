﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// This class represents all the information to render the ToolStrip
    /// </summary>
    public class ToolStripItemTextRenderEventArgs : ToolStripItemRenderEventArgs
    {
        private ContentAlignment _textAlignment;
        private Color _textColor = SystemColors.ControlText;
        private bool _textColorChanged = false;

        /// <summary>
        /// This class represents all the information to render the ToolStrip
        /// </summary>
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

        /// <summary>
        /// This class represents all the information to render the ToolStrip
        /// </summary>
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


        /// <summary>
        /// The string to draw
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The color to draw the text
        /// </summary>
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

        /// <summary>
        /// The font to draw the text
        /// </summary>
        public Font TextFont { get; set; }

        /// <summary>
        /// The rectangle to draw the text in
        /// </summary>
        public Rectangle TextRectangle { get; set; }

        /// <summary>
        /// The rectangle to draw the text in
        /// </summary>
        public TextFormatFlags TextFormat { get; set; }

        /// <summary>
        /// The angle at which the text should be drawn in tenths of degrees.
        /// </summary>
        public ToolStripTextDirection TextDirection { get; set; }
    }
}

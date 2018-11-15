// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.Windows.Forms.ButtonInternal;
    
    /// <include file='doc\ToolStripItemTextRenderEventArgs.uex' path='docs/doc[@for="ToolStripItemTextRenderEventArgs"]/*' />
    /// <devdoc>
    /// This class represents all the information to render the winbar
    /// </devdoc>
    public class ToolStripItemTextRenderEventArgs : ToolStripItemRenderEventArgs {

        private string                           text             = null;
        private Rectangle                        textRectangle    = Rectangle.Empty;
        private Color                            textColor        = SystemColors.ControlText;
        private Font                             textFont         = null;
        private ContentAlignment                 textAlignment;
        private ToolStripTextDirection           textDirection    = ToolStripTextDirection.Horizontal;
        private TextFormatFlags                  textFormat       = TextFormatFlags.Default;
        private Color                            defaultTextColor = SystemColors.ControlText;
        private bool                             textColorChanged = false;
        
        /// <include file='doc\ToolStripItemTextRenderEventArgs.uex' path='docs/doc[@for="ToolStripItemTextRenderEventArgs.ToolStripItemTextRenderEventArgs"]/*' />
        /// <devdoc>
        /// This class represents all the information to render the winbar
        /// </devdoc>
        public ToolStripItemTextRenderEventArgs(Graphics g, ToolStripItem item, string text, Rectangle textRectangle, Color textColor, Font textFont, TextFormatFlags format) : base(g, item) {
            this.text = text;
            this.textRectangle = textRectangle;
            this.defaultTextColor = textColor;
            this.textFont = textFont;
            this.textAlignment = item.TextAlign;
            this.textFormat = format;
            textDirection = item.TextDirection;

        }


        /// <include file='doc\ToolStripItemTextRenderEventArgs.uex' path='docs/doc[@for="ToolStripItemTextRenderEventArgs.ToolStripItemTextRenderEventArgs"]/*' />
        /// <devdoc>
        /// This class represents all the information to render the winbar
        /// </devdoc>
        public ToolStripItemTextRenderEventArgs(Graphics g, ToolStripItem item, string text, Rectangle textRectangle, Color textColor, Font textFont, ContentAlignment textAlign) : base(g, item) {
            this.text = text;
            this.textRectangle = textRectangle;
            this.defaultTextColor = textColor;
            this.textFont = textFont;
            this.textFormat = ToolStripItemInternalLayout.ContentAlignToTextFormat(textAlign, item.RightToLeft == RightToLeft.Yes); 
            
            // in 2K and XP++ hide underlined &File unless ALT is pressed
            this.textFormat = (item.ShowKeyboardCues) ? textFormat : textFormat | TextFormatFlags.HidePrefix;
            textDirection = item.TextDirection;
        }


        /// <include file='doc\ToolStripItemTextRenderEventArgs.uex' path='docs/doc[@for="ToolStripItemTextRenderEventArgs.Text"]/*' />
        /// <devdoc>
        /// the string to draw
        /// </devdoc>
        public string Text  {
            get {
                return text;    
            }
            set {
                text = value;
            }
        }

        /// <include file='doc\ToolStripItemTextRenderEventArgs.uex' path='docs/doc[@for="ToolStripItemTextRenderEventArgs.TextColor"]/*' />
        /// <devdoc>
        /// the color to draw the text
        /// </devdoc>
        public Color TextColor { 
            get {
                if (textColorChanged) {
                    return textColor;
                }
                return DefaultTextColor;
            }
            set {
                textColor = value;
                textColorChanged=true;
            }
        }

        // 


        internal Color DefaultTextColor { 
           get {
               return defaultTextColor;
           }
           set {
               defaultTextColor = value;
           }
       }

        /// <include file='doc\ToolStripItemTextRenderEventArgs.uex' path='docs/doc[@for="ToolStripItemTextRenderEventArgs.TextFont"]/*' />
        /// <devdoc>
        /// the font to draw the text
        /// </devdoc>
        public Font TextFont { 
            get {
                return textFont;
            }
            set {
                textFont = value;
            }
        }

        /// <include file='doc\ToolStripItemTextRenderEventArgs.uex' path='docs/doc[@for="ToolStripItemTextRenderEventArgs.TextRectangle"]/*' />
        /// <devdoc>
        /// the rectangle to draw the text in 
        /// </devdoc>
        public Rectangle TextRectangle { 
            get {
                return textRectangle;
            }
            set {
                textRectangle = value;
            }
        }

        /// <include file='doc\ToolStripItemTextRenderEventArgs.uex' path='docs/doc[@for="ToolStripItemTextRenderEventArgs.TextRectangle"]/*' />
        /// <devdoc>
        /// the rectangle to draw the text in 
        /// </devdoc>
        public TextFormatFlags TextFormat { 
            get {
                return textFormat;
            }
            set {
                textFormat = value;
            }
        }


        /// <include file='doc\ToolStripItemTextRenderEventArgs.uex' path='docs/doc[@for="ToolStripItemTextRenderEventArgs.TextDirection"]/*' />
        /// <devdoc>
        /// the angle at which the text should be drawn in tenths of degrees.
        /// </devdoc>
        public ToolStripTextDirection TextDirection { 
            get {
                return textDirection;
            }
            set {
                textDirection = value;
            }
        }


    }
}

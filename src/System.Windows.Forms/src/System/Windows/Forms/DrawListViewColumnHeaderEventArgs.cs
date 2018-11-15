// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms 
{

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using Microsoft.Win32;
    using System.Windows.Forms.VisualStyles;

    /// <include file='doc\DrawListViewColumnHeaderEventArgs.uex' path='docs/doc[@for="DrawListViewColumnHeaderEventArgs"]/*' />
    /// <devdoc>
    ///     This class contains the information a user needs to paint ListView column header (Details view only).
    /// </devdoc>
    public class DrawListViewColumnHeaderEventArgs : EventArgs 
    {

        private readonly Graphics graphics;
        private readonly Rectangle bounds;
        private readonly int columnIndex;
        private readonly ColumnHeader header;
        private readonly ListViewItemStates state;
        private readonly Color foreColor;
        private readonly Color backColor;
        private readonly Font font;
        private bool     drawDefault;

        /// <include file='doc\DrawListViewColumnHeaderEventArgs.uex' path='docs/doc[@for="DrawListViewColumnHeaderEventArgs.DrawListViewColumnHeaderEventArgs"]/*' />
        /// <devdoc>
        ///     Creates a new DrawListViewColumnHeaderEventArgs with the given parameters.
        /// </devdoc>
        public DrawListViewColumnHeaderEventArgs(Graphics graphics, Rectangle bounds, int columnIndex, 
                                                 ColumnHeader header, ListViewItemStates state,
                                                 Color foreColor, Color backColor, Font font) {
            this.graphics = graphics;
            this.bounds = bounds;
            this.columnIndex = columnIndex;
            this.header = header; 
            this.state = state;
            this.foreColor = foreColor;
            this.backColor = backColor;
            this.font = font;
        }

        
        /// <include file='doc\DrawListViewColumnHeaderEventArgs.uex' path='docs/doc[@for="DrawListViewColumnHeaderEventArgs.DrawDefault"]/*' />
        /// <devdoc>
        ///     Causes the item do be drawn by the system instead of owner drawn.
        /// </devdoc>        
        public bool DrawDefault {
            get  {
                return drawDefault;
            }
            set {
                drawDefault = value;
            }
        }
        
        /// <include file='doc\DrawListViewColumnHeaderEventArgs.uex' path='docs/doc[@for="DrawListViewColumnHeaderEventArgs.Graphics"]/*' />
        /// <devdoc>
        ///     Graphics object with which painting should be done.
        /// </devdoc>
        public Graphics Graphics {
            get {
                return graphics;
            }
        }


        /// <include file='doc\DrawListViewColumnHeaderEventArgs.uex' path='docs/doc[@for="DrawListViewColumnHeaderEventArgs.Bounds"]/*' />
        /// <devdoc>
        ///     The rectangle outlining the area in which the painting should be done.
        /// </devdoc>
        public Rectangle Bounds {
            get {
                return bounds;
            }
        }

        /// <include file='doc\DrawListViewColumnHeaderEventArgs.uex' path='docs/doc[@for="DrawListViewColumnHeaderEventArgs.ColumnIndex"]/*' />
        /// <devdoc>
        ///     The index of this column. 
        /// </devdoc>
        public int ColumnIndex {
            get {
                return columnIndex;
            }
        }

        /// <include file='doc\DrawListViewColumnHeaderEventArgs.uex' path='docs/doc[@for="DrawListViewColumnHeaderEventArgs.Header"]/*' />
        /// <devdoc>
        ///    The header object.
        /// </devdoc>
        public ColumnHeader Header {
            get {
                return header;
            }
        }

        /// <include file='doc\DrawListViewColumnHeaderEventArgs.uex' path='docs/doc[@for="DrawListViewColumnHeaderEventArgs.State"]/*' />
        /// <devdoc>
        ///     State information pertaining to the header.
        /// </devdoc>
        public ListViewItemStates State {
            get {
                return state;
            }
        }

        /// <include file='doc\DrawListViewColumnHeaderEventArgs.uex' path='docs/doc[@for="DrawListViewColumnHeaderEventArgs.ForeColor"]/*' />
        /// <devdoc>
        ///     Color used to draw the header's text.
        /// </devdoc>
        public Color ForeColor {
            get {
                return foreColor;
            }
        }

        /// <include file='doc\DrawListViewColumnHeaderEventArgs.uex' path='docs/doc[@for="DrawListViewColumnHeaderEventArgs.BackColor"]/*' />
        /// <devdoc>
        ///     Color used to draw the header's background.
        /// </devdoc>
        public Color BackColor {
            get {
                return backColor;
            }
        }

        /// <include file='doc\DrawListViewColumnHeaderEventArgs.uex' path='docs/doc[@for="DrawListViewColumnHeaderEventArgs.Font"]/*' />
        /// <devdoc>
        ///     Font used to render the header's text.
        /// </devdoc>
        public Font Font {
            get {
                return font;
            }
        }

        /// <include file='doc\DrawListViewColumnHeaderEventArgs.uex' path='docs/doc[@for="DrawListViewColumnHeaderEventArgs.DrawBackground"]/*' />
        /// <devdoc>
        ///     Draws the header's background.
        /// </devdoc>
        public void DrawBackground() {
            if (Application.RenderWithVisualStyles) {
                VisualStyleRenderer vsr = new VisualStyleRenderer(VisualStyleElement.Header.Item.Normal);
                vsr.DrawBackground(graphics, bounds);
            }
            else {
                using (Brush backBrush = new SolidBrush(backColor)) {
                    graphics.FillRectangle(backBrush, bounds);
                }
                
                // draw the 3d header
                //
                Rectangle r = this.bounds;
                
                r.Width -= 1;
                r.Height -= 1;
    
                // draw the dark border around the whole thing
                //
                graphics.DrawRectangle(SystemPens.ControlDarkDark, r);
                
                r.Width -= 1;
                r.Height -= 1;
                
                // draw the light 3D border
                //
                graphics.DrawLine(SystemPens.ControlLightLight, r.X, r.Y, r.Right, r.Y);
                graphics.DrawLine(SystemPens.ControlLightLight, r.X, r.Y, r.X, r.Bottom);
    
                // draw the dark 3D Border
                //
                graphics.DrawLine(SystemPens.ControlDark, r.X + 1, r.Bottom, r.Right, r.Bottom);
                graphics.DrawLine(SystemPens.ControlDark, r.Right, r.Y + 1, r.Right, r.Bottom);
            }
        }

        /// <include file='doc\DrawListViewColumnHeaderEventArgs.uex' path='docs/doc[@for="DrawListViewColumnHeaderEventArgs.DrawText"]/*' />
        /// <devdoc>
        ///     Draws the header's text (overloaded) 
        /// </devdoc>
        public void DrawText() {
            HorizontalAlignment hAlign = header.TextAlign;
            TextFormatFlags flags = (hAlign == HorizontalAlignment.Left) ? TextFormatFlags.Left : 
                                    ((hAlign == HorizontalAlignment.Center) ? TextFormatFlags.HorizontalCenter : 
                                     TextFormatFlags.Right);
            flags |= TextFormatFlags.WordEllipsis;

            DrawText(flags);
        }

        /// <include file='doc\DrawListViewColumnHeaderEventArgs.uex' path='docs/doc[@for="DrawListViewColumnHeaderEventArgs.DrawText1"]/*' />
        /// <devdoc>
        ///     Draws the header's text (overloaded) - takes a TextFormatFlags argument.
        /// </devdoc>
        [
            SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // We want to measure the size of blank space.
                                                                                                        // So we don't have to localize it.
        ]
        public void DrawText(TextFormatFlags flags) {
            string text  = header.Text;
            int padding = TextRenderer.MeasureText(" ", font).Width;
            Rectangle newBounds = Rectangle.Inflate(bounds, -padding, 0);

            TextRenderer.DrawText(graphics, text, font, newBounds, foreColor, flags);
        }
    }
}

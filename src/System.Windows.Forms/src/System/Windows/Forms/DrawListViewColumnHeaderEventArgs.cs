﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms 
{
    /// <devdoc>
    /// This class contains the information a user needs to paint ListView
    /// column header (Details view only).
    /// </devdoc>
    public class DrawListViewColumnHeaderEventArgs : EventArgs 
    {
        /// <devdoc>
        /// Creates a new DrawListViewColumnHeaderEventArgs with the given parameters.
        /// </devdoc>
        public DrawListViewColumnHeaderEventArgs(Graphics graphics, Rectangle bounds, int columnIndex, 
                                                 ColumnHeader header, ListViewItemStates state,
                                                 Color foreColor, Color backColor, Font font)
        {
            Graphics = graphics;
            Bounds = bounds;
            ColumnIndex = columnIndex;
            Header = header; 
            State = state;
            ForeColor = foreColor;
            BackColor = backColor;
            Font = font;
        }

        /// <devdoc>
        /// Graphics object with which painting should be done.
        /// </devdoc>
        public Graphics Graphics { get; }

        /// <devdoc>
        /// The rectangle outlining the area in which the painting should be done.
        /// </devdoc>
        public Rectangle Bounds { get; }

        /// <devdoc>
        /// The index of this column. 
        /// </devdoc>
        public int ColumnIndex { get; }

        /// <devdoc>
        /// The header object.
        /// </devdoc>
        public ColumnHeader Header { get; }

        /// <devdoc>
        /// State information pertaining to the header.
        /// </devdoc>
        public ListViewItemStates State { get; }

        /// <devdoc>
        /// Color used to draw the header's text.
        /// </devdoc>
        public Color ForeColor { get; }

        /// <devdoc>
        /// Color used to draw the header's background.
        /// </devdoc>
        public Color BackColor { get; }

        /// <devdoc>
        /// Font used to render the header's text.
        /// </devdoc>
        public Font Font { get; }
        
        /// <devdoc>
        /// Causes the item do be drawn by the system instead of owner drawn.
        /// </devdoc>
        public bool DrawDefault { get; set; }

        /// <devdoc>
        /// Draws the header's background.
        /// </devdoc>
        public void DrawBackground()
        {
            if (Application.RenderWithVisualStyles)
            {
                var vsr = new VisualStyleRenderer(VisualStyleElement.Header.Item.Normal);
                vsr.DrawBackground(Graphics, Bounds);
            }
            else
            {
                using (var backBrush = new SolidBrush(BackColor))
                {
                    Graphics.FillRectangle(backBrush, Bounds);
                }
                
                // Draw the 3d header
                Rectangle r = Bounds;
                
                r.Width -= 1;
                r.Height -= 1;
    
                // Draw the dark border around the whole thing
                Graphics.DrawRectangle(SystemPens.ControlDarkDark, r);
                
                r.Width -= 1;
                r.Height -= 1;
                
                // Draw the light 3D border
                Graphics.DrawLine(SystemPens.ControlLightLight, r.X, r.Y, r.Right, r.Y);
                Graphics.DrawLine(SystemPens.ControlLightLight, r.X, r.Y, r.X, r.Bottom);
    
                // Draw the dark 3D Border
                Graphics.DrawLine(SystemPens.ControlDark, r.X + 1, r.Bottom, r.Right, r.Bottom);
                Graphics.DrawLine(SystemPens.ControlDark, r.Right, r.Y + 1, r.Right, r.Bottom);
            }
        }

        /// <devdoc>
        /// Draws the header's text (overloaded) 
        /// </devdoc>
        public void DrawText()
        {
            HorizontalAlignment hAlign = Header.TextAlign;
            TextFormatFlags flags = (hAlign == HorizontalAlignment.Left) ? TextFormatFlags.Left : 
                                    ((hAlign == HorizontalAlignment.Center) ? TextFormatFlags.HorizontalCenter : 
                                     TextFormatFlags.Right);
            flags |= TextFormatFlags.WordEllipsis;

            DrawText(flags);
        }

        /// <devdoc>
        /// Draws the header's text (overloaded) - takes a TextFormatFlags argument.
        /// </devdoc>
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")] // We want to measure the size of blank space so we don't have to localize it.
        public void DrawText(TextFormatFlags flags)
        {
            string text  = Header.Text;
            int padding = TextRenderer.MeasureText(" ", Font).Width;
            Rectangle newBounds = Rectangle.Inflate(Bounds, -padding, 0);

            TextRenderer.DrawText(Graphics, text, Font, newBounds, ForeColor, flags);
        }
    }
}

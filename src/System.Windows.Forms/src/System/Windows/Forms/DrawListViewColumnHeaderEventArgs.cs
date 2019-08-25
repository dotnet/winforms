// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.VisualStyles;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class contains the information a user needs to paint ListView
    ///  column header (Details view only).
    /// </summary>
    public class DrawListViewColumnHeaderEventArgs : EventArgs
    {
        /// <summary>
        ///  Creates a new DrawListViewColumnHeaderEventArgs with the given parameters.
        /// </summary>
        public DrawListViewColumnHeaderEventArgs(Graphics graphics, Rectangle bounds, int columnIndex,
                                                 ColumnHeader header, ListViewItemStates state,
                                                 Color foreColor, Color backColor, Font font)
        {
            Graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
            Bounds = bounds;
            ColumnIndex = columnIndex;
            Header = header;
            State = state;
            ForeColor = foreColor;
            BackColor = backColor;
            Font = font;
        }

        /// <summary>
        ///  Graphics object with which painting should be done.
        /// </summary>
        public Graphics Graphics { get; }

        /// <summary>
        ///  The rectangle outlining the area in which the painting should be done.
        /// </summary>
        public Rectangle Bounds { get; }

        /// <summary>
        ///  The index of this column.
        /// </summary>
        public int ColumnIndex { get; }

        /// <summary>
        ///  The header object.
        /// </summary>
        public ColumnHeader Header { get; }

        /// <summary>
        ///  State information pertaining to the header.
        /// </summary>
        public ListViewItemStates State { get; }

        /// <summary>
        ///  Color used to draw the header's text.
        /// </summary>
        public Color ForeColor { get; }

        /// <summary>
        ///  Color used to draw the header's background.
        /// </summary>
        public Color BackColor { get; }

        /// <summary>
        ///  Font used to render the header's text.
        /// </summary>
        public Font Font { get; }

        /// <summary>
        ///  Causes the item do be drawn by the system instead of owner drawn.
        /// </summary>
        public bool DrawDefault { get; set; }

        /// <summary>
        ///  Draws the header's background.
        /// </summary>
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

        /// <summary>
        ///  Draws the header's text (overloaded)
        /// </summary>
        public void DrawText()
        {
            HorizontalAlignment hAlign = Header?.TextAlign ?? HorizontalAlignment.Left;
            TextFormatFlags flags = (hAlign == HorizontalAlignment.Left) ? TextFormatFlags.Left :
                                    ((hAlign == HorizontalAlignment.Center) ? TextFormatFlags.HorizontalCenter :
                                     TextFormatFlags.Right);
            flags |= TextFormatFlags.WordEllipsis;

            DrawText(flags);
        }

        /// <summary>
        ///  Draws the header's text (overloaded) - takes a TextFormatFlags argument.
        /// </summary>
        public void DrawText(TextFormatFlags flags)
        {
            string text = Header?.Text;
            int padding = TextRenderer.MeasureText(" ", Font).Width;
            Rectangle newBounds = Rectangle.Inflate(Bounds, -padding, 0);

            TextRenderer.DrawText(Graphics, text, Font, newBounds, ForeColor, flags);
        }
    }
}

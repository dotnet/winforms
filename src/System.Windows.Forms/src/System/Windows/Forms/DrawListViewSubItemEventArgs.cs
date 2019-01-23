// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// This class contains the information a user needs to paint ListView sub-items (Details view only).
    /// </devdoc>
    public class DrawListViewSubItemEventArgs : EventArgs
    {
        /// <devdoc>
        /// Creates a new DrawListViewSubItemEventArgs with the given parameters.
        /// </devdoc>
        public DrawListViewSubItemEventArgs(Graphics graphics, Rectangle bounds, ListViewItem item,
                        ListViewItem.ListViewSubItem subItem, int itemIndex, int columnIndex,
                        ColumnHeader header, ListViewItemStates itemState)
        {
            Graphics = graphics;
            Bounds = bounds;
            Item = item;
            SubItem = subItem;
            ItemIndex = itemIndex;
            ColumnIndex = columnIndex;
            Header = header;
            ItemState = itemState;
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
        /// The parent item.
        /// </devdoc>
        public ListViewItem Item { get; }

        /// <devdoc>
        /// The parent item.
        /// </devdoc>
        public ListViewItem.ListViewSubItem SubItem { get; }

        /// <devdoc>
        /// The index in the ListView of the parent item.
        /// </devdoc>
        public int ItemIndex { get; }

        /// <devdoc>
        /// The column index of this sub-item.
        /// </devdoc>
        public int ColumnIndex { get; }

        /// <devdoc>
        /// The header of this sub-item's column
        /// </devdoc>
        public ColumnHeader Header { get; }

        /// <devdoc>
        /// Miscellaneous state information pertaining to the parent item.
        /// </devdoc>
        public ListViewItemStates ItemState { get; }

        /// <devdoc>
        /// Causes the item do be drawn by the system instead of owner drawn.
        /// </devdoc>
        public bool DrawDefault { get; set; }

        /// <devdoc>
        /// Draws the sub-item's background.
        /// </devdoc>
        public void DrawBackground()
        {
            Color backColor = (ItemIndex == -1) ? Item.BackColor : SubItem.BackColor;
            using (var backBrush = new SolidBrush(backColor))
            {
                Graphics.FillRectangle(backBrush, Bounds);
            }
        }

        /// <devdoc>
        /// Draws a focus rectangle in the given bounds, if the item has focus.
        /// </devdoc>
        public void DrawFocusRectangle(Rectangle bounds)
        {
            if ((ItemState & ListViewItemStates.Focused) == ListViewItemStates.Focused)
            {
                ControlPaint.DrawFocusRectangle(Graphics, Rectangle.Inflate(bounds, -1, -1), Item.ForeColor, Item.BackColor);
            }
        }

        /// <devdoc>
        /// Draws the sub-item's text (overloaded)
        /// </devdoc>
        public void DrawText()
        {
            // Map the ColumnHeader::TextAlign to the TextFormatFlags.
            HorizontalAlignment hAlign = Header.TextAlign;
            TextFormatFlags flags = (hAlign == HorizontalAlignment.Left) ? TextFormatFlags.Left :
                                                   ((hAlign == HorizontalAlignment.Center) ? TextFormatFlags.HorizontalCenter :
                                                   TextFormatFlags.Right);
            flags |= TextFormatFlags.WordEllipsis;

            DrawText(flags);
        }

        /// <devdoc>
        /// Draws the sub-item's text (overloaded) - takes a TextFormatFlags argument.
        /// </devdoc>
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")] // We want to measure the size of blank spaces o we don't have to localize it.
        public void DrawText(TextFormatFlags flags)
        {
            string text  = (ItemIndex == -1) ? Item.Text      : SubItem.Text;
            Font   font  = (ItemIndex == -1) ? Item.Font      : SubItem.Font;
            Color  color = (ItemIndex == -1) ? Item.ForeColor : SubItem.ForeColor;
            int padding = TextRenderer.MeasureText(" ", font).Width;
            Rectangle newBounds = Rectangle.Inflate(Bounds, -padding, 0);

            TextRenderer.DrawText(Graphics, text, font, newBounds, color, flags);
        }
    }
}

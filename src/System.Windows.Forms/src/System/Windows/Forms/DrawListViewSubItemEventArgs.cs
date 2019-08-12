// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class contains the information a user needs to paint ListView sub-items (Details view only).
    /// </summary>
    public class DrawListViewSubItemEventArgs : EventArgs
    {
        /// <summary>
        ///  Creates a new DrawListViewSubItemEventArgs with the given parameters.
        /// </summary>
        public DrawListViewSubItemEventArgs(Graphics graphics, Rectangle bounds, ListViewItem item,
                        ListViewItem.ListViewSubItem subItem, int itemIndex, int columnIndex,
                        ColumnHeader header, ListViewItemStates itemState)
        {
            if (graphics == null)
            {
                throw new ArgumentNullException(nameof(graphics));
            }
            if (itemIndex == -1)
            {
                if (item == null)
                {
                    throw new ArgumentNullException(nameof(item));
                }
            }
            else if (subItem == null)
            {
                throw new ArgumentNullException(nameof(subItem));
            }

            Graphics = graphics;
            Bounds = bounds;
            Item = item;
            SubItem = subItem;
            ItemIndex = itemIndex;
            ColumnIndex = columnIndex;
            Header = header;
            ItemState = itemState;
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
        ///  The parent item.
        /// </summary>
        public ListViewItem Item { get; }

        /// <summary>
        ///  The parent item.
        /// </summary>
        public ListViewItem.ListViewSubItem SubItem { get; }

        /// <summary>
        ///  The index in the ListView of the parent item.
        /// </summary>
        public int ItemIndex { get; }

        /// <summary>
        ///  The column index of this sub-item.
        /// </summary>
        public int ColumnIndex { get; }

        /// <summary>
        ///  The header of this sub-item's column
        /// </summary>
        public ColumnHeader Header { get; }

        /// <summary>
        ///  Miscellaneous state information pertaining to the parent item.
        /// </summary>
        public ListViewItemStates ItemState { get; }

        /// <summary>
        ///  Causes the item do be drawn by the system instead of owner drawn.
        /// </summary>
        public bool DrawDefault { get; set; }

        /// <summary>
        ///  Draws the sub-item's background.
        /// </summary>
        public void DrawBackground()
        {
            Color backColor = (ItemIndex == -1) ? Item.BackColor : SubItem.BackColor;
            using (var backBrush = new SolidBrush(backColor))
            {
                Graphics.FillRectangle(backBrush, Bounds);
            }
        }

        /// <summary>
        ///  Draws a focus rectangle in the given bounds, if the item has focus.
        /// </summary>
        public void DrawFocusRectangle(Rectangle bounds)
        {
            if (Item == null)
            {
                return;
            }

            if ((ItemState & ListViewItemStates.Focused) == ListViewItemStates.Focused)
            {
                ControlPaint.DrawFocusRectangle(Graphics, Rectangle.Inflate(bounds, -1, -1), Item.ForeColor, Item.BackColor);
            }
        }

        /// <summary>
        ///  Draws the sub-item's text (overloaded)
        /// </summary>
        public void DrawText()
        {
            // Map the ColumnHeader.TextAlign to the TextFormatFlags.
            HorizontalAlignment hAlign = Header?.TextAlign ?? HorizontalAlignment.Left;
            TextFormatFlags flags = (hAlign == HorizontalAlignment.Left) ? TextFormatFlags.Left :
                                                   ((hAlign == HorizontalAlignment.Center) ? TextFormatFlags.HorizontalCenter :
                                                   TextFormatFlags.Right);
            flags |= TextFormatFlags.WordEllipsis;

            DrawText(flags);
        }

        /// <summary>
        ///  Draws the sub-item's text (overloaded) - takes a TextFormatFlags argument.
        /// </summary>
        public void DrawText(TextFormatFlags flags)
        {
            string text = (ItemIndex == -1) ? Item.Text : SubItem.Text;
            Font font = (ItemIndex == -1) ? Item.Font : SubItem.Font;
            Color color = (ItemIndex == -1) ? Item.ForeColor : SubItem.ForeColor;
            int padding = TextRenderer.MeasureText(" ", font).Width;
            Rectangle newBounds = Rectangle.Inflate(Bounds, -padding, 0);

            TextRenderer.DrawText(Graphics, text, font, newBounds, color, flags);
        }
    }
}

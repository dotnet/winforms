// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    /// This class contains the information a user needs to paint ListView items.
    /// </devdoc>
    public class DrawListViewItemEventArgs : EventArgs
    {
        /// <summary>
        /// Creates a new DrawListViewItemEventArgs with the given parameters.
        /// </devdoc>
        public DrawListViewItemEventArgs(Graphics graphics, ListViewItem item, Rectangle bounds, int itemIndex, ListViewItemStates state)
        {
            Graphics = graphics;
            Item = item;
            Bounds = bounds;
            ItemIndex = itemIndex;
            State = state;
        }

        /// <summary>
        /// Graphics object with which painting should be done.
        /// </devdoc>
        public Graphics Graphics { get; }

        /// <summary>
        /// The item to be painted.
        /// </devdoc>
        public ListViewItem Item { get; }

        /// <summary>
        /// The rectangle outlining the area in which the painting should be done.
        /// </devdoc>
        public Rectangle Bounds { get; }

        /// <summary>
        /// The index of the item that should be painted.
        /// </devdoc>
        public int ItemIndex { get; }

        /// <summary>
        /// Miscellaneous state information.
        /// </devdoc>
        public ListViewItemStates State { get; }

        /// <summary>
        /// Causes the item do be drawn by the system instead of owner drawn.
        /// </devdoc>
        public bool DrawDefault { get; set; }

        /// <summary>
        /// Draws the item's background.
        /// </devdoc>
        public void DrawBackground()
        {
            using (var backBrush = new SolidBrush(Item.BackColor))
            {
                Graphics.FillRectangle(backBrush, Bounds);
            }
        }

        /// <summary>
        /// Draws a focus rectangle in the given bounds, if the item is focused. In Details View, if FullRowSelect is
        /// true, the rectangle is drawn around the whole item, else around the first sub-item's text area.
        /// </devdoc>
        public void DrawFocusRectangle()
        {
            if ((State & ListViewItemStates.Focused) == ListViewItemStates.Focused)
            {
                ControlPaint.DrawFocusRectangle(Graphics, UpdateBounds(Bounds, drawText: false), Item.ForeColor, Item.BackColor);
            }
        }

        /// <summary>
        /// Draws the item's text (overloaded) - useful only when View != View.Details
        /// </devdoc>
        public void DrawText()
        {
            DrawText(TextFormatFlags.Left);
        }

        /// <summary>
        /// Draws the item's text (overloaded) - useful only when View != View.Details - takes a TextFormatFlags argument.
        /// </devdoc>
        public void DrawText(TextFormatFlags flags)
        {
            TextRenderer.DrawText(Graphics, Item.Text, Item.Font, UpdateBounds(Bounds, drawText: true), Item.ForeColor, flags);
        }

        private Rectangle UpdateBounds(Rectangle originalBounds, bool drawText)
        {
            Rectangle resultBounds = originalBounds;
            if (Item.ListView.View == View.Details)
            {
                // Note: this logic will compute the bounds so they align w/ the system drawn bounds only
                // for the default font.
                if (!Item.ListView.FullRowSelect && Item.SubItems.Count > 0)
                {
                    ListViewItem.ListViewSubItem subItem = Item.SubItems[0];
                    Size textSize = TextRenderer.MeasureText(subItem.Text, subItem.Font);

                    resultBounds = new Rectangle(originalBounds.X, originalBounds.Y, textSize.Width, textSize.Height);

                    // Add some padding so we paint like the system control paints.
                    resultBounds.X += 4;
                    resultBounds.Width++;
                }
                else
                {
                    resultBounds.X += 4;
                    resultBounds.Width -= 4;
                }

                if (drawText)
                {
                    resultBounds.X--;
                }
            }

            return resultBounds;
        }
    }
}

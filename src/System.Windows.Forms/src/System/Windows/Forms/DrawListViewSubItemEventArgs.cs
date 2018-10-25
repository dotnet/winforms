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

    /// <include file='doc\DrawListViewSubItemEventArgs.uex' path='docs/doc[@for="DrawListViewSubItemEventArgs"]/*' />
    /// <devdoc>
    ///     This class contains the information a user needs to paint ListView sub-items (Details view only).
    /// </devdoc>
    public class DrawListViewSubItemEventArgs : EventArgs 
    {

        private readonly Graphics graphics;
        private readonly Rectangle bounds;
        private readonly ListViewItem item;
        private readonly ListViewItem.ListViewSubItem subItem;
        private readonly int itemIndex;
        private readonly int columnIndex;
        private readonly ColumnHeader header;
        private readonly ListViewItemStates itemState;
        private bool     drawDefault;

        /// <include file='doc\DrawListViewSubItemEventArgs.uex' path='docs/doc[@for="DrawListViewSubItemEventArgs.DrawListViewSubItemEventArgs"]/*' />
        /// <devdoc>
        ///     Creates a new DrawListViewSubItemEventArgs with the given parameters.
        /// </devdoc>
        public DrawListViewSubItemEventArgs(Graphics graphics, Rectangle bounds, ListViewItem item, 
                        ListViewItem.ListViewSubItem subItem, int itemIndex, int columnIndex, 
                        ColumnHeader header, ListViewItemStates itemState) 
        {
            this.graphics = graphics;
            this.bounds = bounds;
            this.item = item;
            this.subItem = subItem;
            this.itemIndex = itemIndex;
            this.columnIndex = columnIndex;
            this.header = header; 
            this.itemState = itemState;
        }

        
        /// <include file='doc\DrawListViewSubItemEventArgs.uex' path='docs/doc[@for="DrawListViewSubItemEventArgs.DrawDefault"]/*' />
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
        
        /// <include file='doc\DrawListViewSubItemEventArgs.uex' path='docs/doc[@for="DrawListViewSubItemEventArgs.Graphics"]/*' />
        /// <devdoc>
        ///     Graphics object with which painting should be done.
        /// </devdoc>
        public Graphics Graphics 
        {
            get 
            {
                return graphics;
            }
        }


        /// <include file='doc\DrawListViewSubItemEventArgs.uex' path='docs/doc[@for="DrawListViewSubItemEventArgs.Bounds"]/*' />
        /// <devdoc>
        ///     The rectangle outlining the area in which the painting should be done.
        /// </devdoc>
        public Rectangle Bounds 
        {
            get 
            {
                return bounds;
            }
        }

        /// <include file='doc\DrawListViewSubItemEventArgs.uex' path='docs/doc[@for="DrawListViewSubItemEventArgs.Item"]/*' />
        /// <devdoc>
        ///     The parent item. 
        /// </devdoc>
        public ListViewItem Item 
        {
            get 
            {
                return item;
            }
        }	

        /// <include file='doc\DrawListViewSubItemEventArgs.uex' path='docs/doc[@for="DrawListViewSubItemEventArgs.SubItem"]/*' />
        /// <devdoc>
        ///     The parent item. 
        /// </devdoc>
        public ListViewItem.ListViewSubItem SubItem 
        {
            get 
            {
                return subItem;
            }
        }

        /// <include file='doc\DrawListViewSubItemEventArgs.uex' path='docs/doc[@for="DrawListViewSubItemEventArgs.ItemIndex"]/*' />
        /// <devdoc>
        ///     The index in the ListView of the parent item.
        /// </devdoc>
        public int ItemIndex 
        {
            get 
            {
                return itemIndex;
            }
        }

        /// <include file='doc\DrawListViewSubItemEventArgs.uex' path='docs/doc[@for="DrawListViewSubItemEventArgs.ColumnIndex"]/*' />
        /// <devdoc>
        ///     The column index of this sub-item. 
        /// </devdoc>
        public int ColumnIndex 
        {
            get 
            {
                return columnIndex;
            }
        }

        /// <include file='doc\DrawListViewSubItemEventArgs.uex' path='docs/doc[@for="DrawListViewSubItemEventArgs.Header"]/*' />
        /// <devdoc>
        ///    The header of this sub-item's column 
        /// </devdoc>
        public ColumnHeader Header 
        {
            get 
            {
                return header;
            }
        }

        /// <include file='doc\DrawListViewSubItemEventArgs.uex' path='docs/doc[@for="DrawListViewSubItemEventArgs.ItemState"]/*' />
        /// <devdoc>
        ///     Miscellaneous state information pertaining to the parent item.
        /// </devdoc>
        public ListViewItemStates ItemState 
        {
            get 
            {
                return itemState;
            }
        }

        /// <include file='doc\DrawListViewSubItemEventArgs.uex' path='docs/doc[@for="DrawListViewSubItemEventArgs.DrawBackground"]/*' />
        /// <devdoc>
        ///     Draws the sub-item's background.
        /// </devdoc>
        public void DrawBackground() 
        {
            Color backColor = (itemIndex == -1) ? item.BackColor : subItem.BackColor;
            using (Brush backBrush = new SolidBrush(backColor)) {
                Graphics.FillRectangle(backBrush, bounds);
            }
        }

        /// <include file='doc\DrawListViewSubItemEventArgs.uex' path='docs/doc[@for="DrawListViewSubItemEventArgs.DrawFocusRectangle"]/*' />
        /// <devdoc>
        ///     Draws a focus rectangle in the given bounds, if the item has focus.
        /// </devdoc>
        public void DrawFocusRectangle(Rectangle bounds) 
        {
            if((itemState & ListViewItemStates.Focused) == ListViewItemStates.Focused) 
            {
                ControlPaint.DrawFocusRectangle(graphics, Rectangle.Inflate(bounds, -1, -1), item.ForeColor, item.BackColor);
            }                
        }

        /// <include file='doc\DrawListViewSubItemEventArgs.uex' path='docs/doc[@for="DrawListViewSubItemEventArgs.DrawText"]/*' />
        /// <devdoc>
        ///     Draws the sub-item's text (overloaded) 
        /// </devdoc>
        public void DrawText() 
        {
            // Map the ColumnHeader::TextAlign to the TextFormatFlags.
            HorizontalAlignment hAlign = header.TextAlign;
            TextFormatFlags flags = (hAlign == HorizontalAlignment.Left) ? TextFormatFlags.Left : 
                                                   ((hAlign == HorizontalAlignment.Center) ? TextFormatFlags.HorizontalCenter :
                                                   TextFormatFlags.Right);
            flags |= TextFormatFlags.WordEllipsis;

            DrawText(flags);
        }

        /// <include file='doc\DrawListViewSubItemEventArgs.uex' path='docs/doc[@for="DrawListViewSubItemEventArgs.DrawText1"]/*' />
        /// <devdoc>
        ///     Draws the sub-item's text (overloaded) - takes a TextFormatFlags argument.
        /// </devdoc>
        [
            SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // We want to measure the size of blank space.
                                                                                                        // So we don't have to localize it.
        ]
        public void DrawText(TextFormatFlags flags) 
        {
            string text  = (itemIndex == -1) ? item.Text      : subItem.Text;
            Font   font  = (itemIndex == -1) ? item.Font      : subItem.Font;
            Color  color = (itemIndex == -1) ? item.ForeColor : subItem.ForeColor;
            int padding = TextRenderer.MeasureText(" ", font).Width;
            Rectangle newBounds = Rectangle.Inflate(bounds, -padding, 0);

            TextRenderer.DrawText(graphics, text, font, newBounds, color, flags);
        }
    }
}

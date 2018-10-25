// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using Microsoft.Win32;
    using System.Windows.Forms.VisualStyles;

    /// <include file='doc\DrawListViewItemEventArgs.uex' path='docs/doc[@for="DrawListViewItemEventArgs"]/*' />
    /// <devdoc>
    ///     This class contains the information a user needs to paint ListView items.
    /// </devdoc>
    public class DrawListViewItemEventArgs : EventArgs {

        private readonly Graphics graphics;
        private readonly ListViewItem item;
        private readonly Rectangle bounds;
        private readonly int itemIndex;
        private readonly ListViewItemStates state;
        private bool drawDefault;

        /// <include file='doc\DrawListViewItemEventArgs.uex' path='docs/doc[@for="DrawListViewItemEventArgs.DrawListViewItemEventArgs"]/*' />
        /// <devdoc>
        ///     Creates a new DrawListViewItemEventArgs with the given parameters.
        /// </devdoc>
        public DrawListViewItemEventArgs(Graphics graphics, ListViewItem item, Rectangle bounds,
										int itemIndex, ListViewItemStates state) 
        {
            this.graphics = graphics;
            this.item = item;
            this.bounds = bounds;
            this.itemIndex = itemIndex;
            this.state = state;
            this.drawDefault = false;
        }

        /// <include file='doc\DrawListViewItemEventArgs.uex' path='docs/doc[@for="DrawListViewItemEventArgs.DrawDefault"]/*' />
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
        
        /// <include file='doc\DrawListViewItemEventArgs.uex' path='docs/doc[@for="DrawListViewItemEventArgs.Graphics"]/*' />
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
	
        /// <include file='doc\DrawListViewItemEventArgs.uex' path='docs/doc[@for="DrawListViewItemEventArgs.Item"]/*' />
        /// <devdoc>
        ///     The item to be painted. 
        /// </devdoc>
        public ListViewItem Item 
        {
            get 
            {
                return item;
            }
        }
	
        /// <include file='doc\DrawListViewItemEventArgs.uex' path='docs/doc[@for="DrawListViewItemEventArgs.Bounds"]/*' />
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


        /// <include file='doc\DrawListViewItemEventArgs.uex' path='docs/doc[@for="DrawListViewItemEventArgs.ItemIndex"]/*' />
        /// <devdoc>
        ///     The index of the item that should be painted.
        /// </devdoc>
        public int ItemIndex 
        {
            get 
            {
                return itemIndex;
            }
        }

        /// <include file='doc\DrawListViewItemEventArgs.uex' path='docs/doc[@for="DrawListViewItemEventArgs.State"]/*' />
        /// <devdoc>
        ///     Miscellaneous state information.
        /// </devdoc>
        public ListViewItemStates State 
        {
            get 
            {
                return state;
            }
        }

        /// <include file='doc\DrawListViewItemEventArgs.uex' path='docs/doc[@for="DrawListViewItemEventArgs.DrawBackground"]/*' />
        /// <devdoc>
        ///     Draws the item's background.
        /// </devdoc>
        public void DrawBackground() 
        {
            Brush backBrush = new SolidBrush(item.BackColor);
            Graphics.FillRectangle(backBrush, bounds);
            backBrush.Dispose();
        }

		/// <include file='doc\DrawListViewItemEventArgs.uex' path='docs/doc[@for="DrawListViewSubItemEventArgs.DrawFocusRectangle"]/*' />
        /// <devdoc>
        ///     Draws a focus rectangle in the given bounds, if the item is focused. In Details View, if FullRowSelect is
        ///     true, the rectangle is drawn around the whole item, else around the first sub-item's text area.
        /// </devdoc>
        public void DrawFocusRectangle() 
        {
            if ((state & ListViewItemStates.Focused) == ListViewItemStates.Focused) {

                Rectangle focusBounds = bounds;
                ControlPaint.DrawFocusRectangle(graphics, UpdateBounds(focusBounds, false /*drawText*/), item.ForeColor, item.BackColor);
            }
        }                

        /// <include file='doc\DrawListViewItemEventArgs.uex' path='docs/doc[@for="DrawListViewItemEventArgs.DrawText"]/*' />
        /// <devdoc>
        ///     Draws the item's text (overloaded) - useful only when View != View.Details
        /// </devdoc>
        public void DrawText() 
        {
            DrawText(TextFormatFlags.Left);
        }
	
        /// <include file='doc\DrawListViewItemEventArgs.uex' path='docs/doc[@for="DrawListViewItemEventArgs.DrawText1"]/*' />
        /// <devdoc>
        ///     Draws the item's text (overloaded) - useful only when View != View.Details - takes a TextFormatFlags argument.
        /// </devdoc>
        public void DrawText(TextFormatFlags flags) 
        {
            TextRenderer.DrawText(graphics, item.Text, item.Font, UpdateBounds(bounds, true /*drawText*/), item.ForeColor, flags);
        }

        private Rectangle UpdateBounds(Rectangle originalBounds, bool drawText) {
            Rectangle resultBounds = originalBounds;
            if (item.ListView.View == View.Details) {

                // Note: this logic will compute the bounds so they align w/ the system drawn bounds only 
                // for the default font.
                if (!item.ListView.FullRowSelect && item.SubItems.Count > 0) {
                    ListViewItem.ListViewSubItem subItem = item.SubItems[0];
                    Size textSize = TextRenderer.MeasureText(subItem.Text, subItem.Font);

                    resultBounds = new Rectangle(originalBounds.X, originalBounds.Y, textSize.Width, textSize.Height);

                    // Add some padding so we paint like the system control paints.
                    resultBounds.X += 4;
                    resultBounds.Width ++;
                }
                else
                {
                    resultBounds.X +=4;
                    resultBounds.Width -= 4;
                }

                if (drawText) {
                    resultBounds.X --;
                }
            }

            return resultBounds;
        }
    }
}

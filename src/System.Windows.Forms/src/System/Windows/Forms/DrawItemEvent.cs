// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;

    /// <devdoc>
    ///     This event is fired by owner draw Controls, such as ListBoxes and
    ///     ComboBoxes. It contains all the information needed for the user to
    ///     paint the given item, including the item index, the Rectangle in which
    ///     the drawing should be done, and the Graphics object with which the drawing
    ///     should be done.
    /// </devdoc>
    public class DrawItemEventArgs : EventArgs {

        /// <devdoc>
        ///     The backColor to paint each menu item with.
        /// </devdoc>
        private Color backColor;

        /// <devdoc>
        ///     The foreColor to paint each menu item with.
        /// </devdoc>
        private Color foreColor;

        /// <devdoc>
        ///     The font used to draw the item's string.
        /// </devdoc>
        private Font font;
        
        /// <devdoc>
        ///     The graphics object with which the drawing should be done.
        /// </devdoc>
        private readonly System.Drawing.Graphics graphics;

        /// <devdoc>
        ///     The index of the item that should be painted.
        /// </devdoc>
        private readonly int index;

        /// <devdoc>
        ///     The rectangle outlining the area in which the painting should be
        ///     done.
        /// </devdoc>
        private readonly Rectangle rect;

        /// <devdoc>
        ///     Miscellaneous state information, such as whether the item is
        ///     "selected", "focused", or some other such information.  ComboBoxes
        ///     have one special piece of information which indicates if the item
        ///     being painted is the editable portion of the ComboBox.
        /// </devdoc>
        private readonly DrawItemState state;

        /// <devdoc>
        ///     Creates a new DrawItemEventArgs with the given parameters.
        /// </devdoc>
        public DrawItemEventArgs(Graphics graphics, Font font, Rectangle rect,
                             int index, DrawItemState state) {
            this.graphics = graphics;
            this.font = font;
            this.rect = rect;
            this.index = index;
            this.state = state;
            this.foreColor = SystemColors.WindowText;
            this.backColor = SystemColors.Window;
        }
        
        /// <devdoc>
        ///     Creates a new DrawItemEventArgs with the given parameters, including the foreColor and backColor of the control.
        /// </devdoc>
        public DrawItemEventArgs(Graphics graphics, Font font, Rectangle rect,
                             int index, DrawItemState state, Color foreColor, Color backColor) {
            this.graphics = graphics;
            this.font = font;
            this.rect = rect;
            this.index = index;
            this.state = state;
            this.foreColor = foreColor;
            this.backColor = backColor;
        }

        public Color BackColor {
            get {
                if ((state & DrawItemState.Selected) == DrawItemState.Selected) {
                    return SystemColors.Highlight;
                }
                return backColor;
            }
        }
        
        

        /// <devdoc>
        ///     The rectangle outlining the area in which the painting should be
        ///     done.
        /// </devdoc>
        public Rectangle Bounds {
            get {
                return rect;
            }
        }

        /// <devdoc>
        ///     A suggested font, usually the parent control's Font property.
        /// </devdoc>
        public Font Font {
            get {
                return font;
            }
        }

        /// <devdoc>
        ///     A suggested color drawing: either SystemColors.WindowText or SystemColors.HighlightText,
        ///     depending on whether this item is selected.
        /// </devdoc>
        public Color ForeColor {
            get {
                if ((state & DrawItemState.Selected) == DrawItemState.Selected) {
                    return SystemColors.HighlightText;
                }
                return foreColor;
            }
        }              

        /// <devdoc>
        ///     Graphics object with which painting should be done.
        /// </devdoc>
        public Graphics Graphics {
            get {
                return graphics;
            }
        }

        /// <devdoc>
        ///     The index of the item that should be painted.
        /// </devdoc>
        public int Index {
            get {
                return index;
            }
        }

        /// <devdoc>
        ///     Miscellaneous state information, such as whether the item is
        ///     "selected", "focused", or some other such information.  ComboBoxes
        ///     have one special piece of information which indicates if the item
        ///     being painted is the editable portion of the ComboBox.
        /// </devdoc>
        public DrawItemState State {
            get {
                return state;
            }
        }

        /// <devdoc>
        ///     Draws the background of the given rectangle with the color returned from the BackColor property.
        /// </devdoc>
        public virtual void DrawBackground() {
            Brush backBrush = new SolidBrush(BackColor);
            Graphics.FillRectangle(backBrush, rect);
            backBrush.Dispose();
        }

        /// <devdoc>
        ///     Draws a handy focus rect in the given rectangle.
        /// </devdoc>
        public virtual void DrawFocusRectangle() {
            if ((state & DrawItemState.Focus) == DrawItemState.Focus
                && (state & DrawItemState.NoFocusRect) != DrawItemState.NoFocusRect)
                ControlPaint.DrawFocusRectangle(Graphics, rect, ForeColor, BackColor);
        }                
    }
}

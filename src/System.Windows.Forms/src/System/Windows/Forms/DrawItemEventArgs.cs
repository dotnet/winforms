// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// This event is fired by owner draw Controls, such as ListBoxes and
    /// ComboBoxes. It contains all the information needed for the user to
    /// paint the given item, including the item index, the Rectangle in which
    /// the drawing should be done, and the Graphics object with which the drawing
    /// should be done.
    /// </devdoc>
    public class DrawItemEventArgs : EventArgs
    {
        /// <devdoc>
        /// The backColor to paint each menu item with.
        /// </devdoc>
        private Color _backColor;

        /// <devdoc>
        /// The foreColor to paint each menu item with.
        /// </devdoc>
        private Color _foreColor;

        /// <devdoc>
        /// Creates a new DrawItemEventArgs with the given parameters.
        /// </devdoc>
        public DrawItemEventArgs(Graphics graphics, Font font, Rectangle rect,
                                 int index, DrawItemState state)
        {
            Graphics = graphics;
            Font = font;
            Bounds = rect;
            Index = index;
            State = state;
            _foreColor = SystemColors.WindowText;
            _backColor = SystemColors.Window;
        }

        /// <devdoc>
        /// Creates a new DrawItemEventArgs with the given parameters, including the foreColor and backColor of the control.
        /// </devdoc>
        public DrawItemEventArgs(Graphics graphics, Font font, Rectangle rect,
                                 int index, DrawItemState state, Color foreColor, Color backColor)
        {
            Graphics = graphics;
            Font = font;
            Bounds = rect;
            Index = index;
            State = state;
            _foreColor = foreColor;
            _backColor = backColor;
        }

        /// <include file='doc\DrawItemEvent.uex' path='docs/doc[@for="DrawItemEventArgs.Graphics"]/*' />
        /// <devdoc>
        /// Graphics object with which painting should be done.
        /// </devdoc>
        public Graphics Graphics { get; }

        /// <devdoc>
        /// A suggested font, usually the parent control's Font property.
        /// </devdoc>
        public Font Font { get; }

        /// <include file='doc\DrawItemEvent.uex' path='docs/doc[@for="DrawItemEventArgs.Bounds"]/*' />
        /// <devdoc>
        /// The rectangle outlining the area in which the painting should be  done.
        /// </devdoc>
        public Rectangle Bounds { get; }

        /// <devdoc>
        /// The index of the item that should be painted.
        /// </devdoc>
        public int Index { get; }

        /// <devdoc>
        /// Miscellaneous state information, such as whether the item is
        /// "selected", "focused", or some other such information.  ComboBoxes
        /// have one special piece of information which indicates if the item
        /// being painted is the editable portion of the ComboBox.
        /// </devdoc>
        public DrawItemState State { get; }

        /// <devdoc>
        /// A suggested color drawing: either SystemColors.WindowText or SystemColors.HighlightText,
        /// depending on whether this item is selected.
        /// </devdoc>
        public Color ForeColor
        {
            get
            {
                if ((State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    return SystemColors.HighlightText;
                }
                return _foreColor;
            }
        }

        public Color BackColor
        {
            get
            {
                if ((State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    return SystemColors.Highlight;
                }
                return _backColor;
            }
        }

        /// <devdoc>
        /// Draws the background of the given rectangle with the color returned from the BackColor property.
        /// </devdoc>
        public virtual void DrawBackground()
        {
            Brush backBrush = new SolidBrush(BackColor);
            Graphics.FillRectangle(backBrush, Bounds);
            backBrush.Dispose();
        }

        /// <devdoc>
        /// Draws a handy focus rect in the given rectangle.
        /// </devdoc>
        public virtual void DrawFocusRectangle()
        {
            if ((State & DrawItemState.Focus) == DrawItemState.Focus &&
                (State & DrawItemState.NoFocusRect) != DrawItemState.NoFocusRect)
            {
                ControlPaint.DrawFocusRectangle(Graphics, Bounds, ForeColor, BackColor);
            }
        }
    }
}

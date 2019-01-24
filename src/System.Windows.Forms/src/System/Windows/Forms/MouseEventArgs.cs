// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see langword='MouseUp'/>, <see langword='MouseDown'/>, and
    /// <see langword='MouseMove '/> events.
    /// </devdoc>
    [ComVisible(true)]
    public class MouseEventArgs : EventArgs
    {
        /// <devdoc>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.MouseEventArgs'/> class.
        /// </devdoc>
        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
        {
            Button = button;
            Clicks = clicks;
            X = x;
            Y = y;
            Delta = delta;
        }

        /// <devdoc>
        /// Gets which mouse button was pressed.
        /// </devdoc>
        public MouseButtons Button { get; }

        /// <devdoc>
        /// Gets the number of times the mouse button was pressed and released.
        /// </devdoc>
        public int Clicks { get; }

        /// <devdoc>
        /// Gets the x-coordinate of a mouse click.
        /// </devdoc>
        public int X { get; }

        /// <devdoc>
        /// Gets the y-coordinate of a mouse click.
        /// </devdoc>
        public int Y { get; }

        /// <devdoc>
        /// Gets a signed count of the number of detents the mouse wheel has rotated.
        /// </devdoc>
        public int Delta { get; }

        /// <devdoc>
        /// Gets the location of the mouse during MouseEvent.
        /// </devdoc>
        public Point Location => new Point(X, Y);
    }
}

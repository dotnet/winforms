// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for splitter events.
    /// </devdoc>
    [ComVisible(true)]
    public class SplitterEventArgs : EventArgs
    {
        /// <devdoc>
        /// Initializes an instance of the <see cref='System.Windows.Forms.SplitterEventArgs'/> class with the specified coordinates
        /// of the mouse pointer and the upper-left corner of the <see cref='System.Windows.Forms.Splitter'/>.
        /// </devdoc>
        public SplitterEventArgs(int x, int y, int splitX, int splitY)
        {
            X = x;
            Y = y;
            SplitX = splitX;
            SplitY = splitY;
        }

        /// <devdoc>
        /// Gets the x-coordinate of the mouse pointer (in client coordinates).
        /// </devdoc>
        public int X { get; }

        /// <devdoc>
        /// Gets the y-coordinate of the mouse pointer (in client coordinates).
        /// </devdoc>
        public int Y { get; }

        /// <devdoc>
        /// Gets the x-coordinate of the upper-left corner of the <see cref='System.Windows.Forms.Splitter'/> (in client coordinates).
        /// </devdoc>
        public int SplitX { get; set; }

        /// <devdoc>
        /// Gets the y-coordinate of the upper-left corner of the <see cref='System.Windows.Forms.Splitter'/> (in client coordinates).
        /// </devdoc>
        public int SplitY { get; set; }
    }
}

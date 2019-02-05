// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for splitter events.
    /// </devdoc>
    public class SplitterCancelEventArgs : CancelEventArgs
    {
        /// <devdoc>
        /// Initializes an instance of the <see cref='System.Windows.Forms.SplitterCancelEventArgs'/> class with the specified coordinates
        /// of the mouse pointer and the upper-left corner of the <see cref='System.Windows.Forms.SplitContainer'/>.
        /// </devdoc>
        public SplitterCancelEventArgs(int mouseCursorX, int mouseCursorY, int splitX, int splitY)  : base (false)
        {
            MouseCursorX = mouseCursorX;
            MouseCursorY = mouseCursorY;
            SplitX = splitX;
            SplitY = splitY;
        }

        /// <devdoc>
        /// Gets the x-coordinate of the mouse pointer (in client coordinates).
        /// </devdoc>
        public int MouseCursorX { get; }

        /// <devdoc>
        /// Gets the y-coordinate of the mouse pointer (in client coordinates).
        /// </devdoc>
        public int MouseCursorY { get; }
        
        /// <devdoc>
        /// Gets the x-coordinate of the upper-left corner of the <see cref='System.Windows.Forms.SplitContainer'/> (in client coordinates).
        /// </devdoc>
        public int SplitX { get; set; }

        /// <devdoc>
        /// Gets the y-coordinate of the upper-left corner of the <see cref='System.Windows.Forms.SplitContainer'/> (in client coordinates).
        /// </devdoc>
        public int SplitY { get; set; }
    }
}

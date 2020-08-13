// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for splitter events.
    /// </summary>
    public class SplitterCancelEventArgs : CancelEventArgs
    {
        /// <summary>
        ///  Initializes an instance of the <see cref='SplitterCancelEventArgs'/> class with the specified coordinates
        ///  of the mouse pointer and the upper-left corner of the <see cref='SplitContainer'/>.
        /// </summary>
        public SplitterCancelEventArgs(int mouseCursorX, int mouseCursorY, int splitX, int splitY) : base(false)
        {
            MouseCursorX = mouseCursorX;
            MouseCursorY = mouseCursorY;
            SplitX = splitX;
            SplitY = splitY;
        }

        /// <summary>
        ///  Gets the x-coordinate of the mouse pointer (in client coordinates).
        /// </summary>
        public int MouseCursorX { get; }

        /// <summary>
        ///  Gets the y-coordinate of the mouse pointer (in client coordinates).
        /// </summary>
        public int MouseCursorY { get; }

        /// <summary>
        ///  Gets the x-coordinate of the upper-left corner of the <see cref='SplitContainer'/> (in client coordinates).
        /// </summary>
        public int SplitX { get; set; }

        /// <summary>
        ///  Gets the y-coordinate of the upper-left corner of the <see cref='SplitContainer'/> (in client coordinates).
        /// </summary>
        public int SplitY { get; set; }
    }
}

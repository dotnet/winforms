// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for splitter events.
    /// </summary>
    [ComVisible(true)]
    public class SplitterEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes an instance of the <see cref='SplitterEventArgs'/> class with the specified coordinates
        ///  of the mouse pointer and the upper-left corner of the <see cref='Splitter'/>.
        /// </summary>
        public SplitterEventArgs(int x, int y, int splitX, int splitY)
        {
            X = x;
            Y = y;
            SplitX = splitX;
            SplitY = splitY;
        }

        /// <summary>
        ///  Gets the x-coordinate of the mouse pointer (in client coordinates).
        /// </summary>
        public int X { get; }

        /// <summary>
        ///  Gets the y-coordinate of the mouse pointer (in client coordinates).
        /// </summary>
        public int Y { get; }

        /// <summary>
        ///  Gets the x-coordinate of the upper-left corner of the <see cref='Splitter'/> (in client coordinates).
        /// </summary>
        public int SplitX { get; set; }

        /// <summary>
        ///  Gets the y-coordinate of the upper-left corner of the <see cref='Splitter'/> (in client coordinates).
        /// </summary>
        public int SplitY { get; set; }
    }
}

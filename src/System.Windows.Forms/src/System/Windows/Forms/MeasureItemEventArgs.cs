// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This event is sent by controls such as the ListBox or ComboBox that need users
    ///  to tell them how large a given item is to be.
    /// </summary>
    public class MeasureItemEventArgs : EventArgs
    {
        public MeasureItemEventArgs(Graphics graphics, int index) : this(graphics, index, 0)
        {
        }

        public MeasureItemEventArgs(Graphics graphics, int index, int itemHeight)
        {
            Graphics = graphics;
            Index = index;
            ItemHeight = itemHeight;
            ItemWidth = 0;
        }

        /// <summary>
        ///  A Graphics object to measure relative to.
        /// </summary>
        public Graphics Graphics { get; }

        /// <summary>
        ///  The index of item for which the height/width is needed.
        /// </summary>
        public int Index { get; }

        /// <summary>
        ///  Where the recipient of the event should put the height of the item specified by
        ///  the index.
        /// </summary>
        public int ItemHeight { get; set; }

        /// <summary>
        ///  Where the recipient of the event should put the width of the item specified by
        ///  the index.
        /// </summary>
        public int ItemWidth { get; set; }
    }
}

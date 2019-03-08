﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// This event is sent by controls such as the ListBox or ComboBox that need users
    /// to tell them how large a given item is to be.
    /// </devdoc>
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
        
        /// <devdoc>
        /// A Graphics object to measure relative to.
        /// </devdoc>
        public Graphics Graphics { get; }

        /// <devdoc>
        /// The index of item for which the height/width is needed.
        /// </devdoc>
        public int Index { get; }

        /// <devdoc>
        /// Where the recipient of the event should put the height of the item specified by
        /// the index.
        /// </devdoc>
        public int ItemHeight { get; set; }

        /// <devdoc>
        /// Where the recipient of the event should put the width of the item specified by
        /// the index.
        /// </devdoc>
        public int ItemWidth { get; set; }
    }
}

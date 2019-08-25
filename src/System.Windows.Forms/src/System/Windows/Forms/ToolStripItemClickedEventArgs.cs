// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public class ToolStripItemClickedEventArgs : EventArgs
    {
        /// <summary>
        ///  This class represents event args a ToolStrip can use when an item has been clicked.
        /// </summary>
        public ToolStripItemClickedEventArgs(ToolStripItem clickedItem)
        {
            ClickedItem = clickedItem;
        }

        /// <summary>
        ///  Represents the item that was clicked on the toolStrip.
        /// </summary>
        public ToolStripItem ClickedItem { get; }
    }
}

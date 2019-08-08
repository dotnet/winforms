// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    public class ToolStripItemRenderEventArgs : EventArgs
    {
        /// <summary>
        ///  This class represents all the information to render the ToolStrip
        /// </summary>
        public ToolStripItemRenderEventArgs(Graphics g, ToolStripItem item)
        {
            Graphics = g;
            Item = item;
        }

        /// <summary>
        ///  The graphics object to draw with
        /// </summary>
        public Graphics Graphics { get; }

        /// <summary>
        ///  The item to draw
        /// </summary>
        public ToolStripItem Item { get; }

        /// <summary>
        ///  The toolstrip the item is currently parented to
        /// </summary>
        public ToolStrip ToolStrip => Item?.ParentInternal;
    }
}

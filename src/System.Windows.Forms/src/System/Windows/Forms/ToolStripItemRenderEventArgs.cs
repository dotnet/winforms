// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    public class ToolStripItemRenderEventArgs : EventArgs
    {
        /// <devdoc>
        /// This class represents all the information to render the ToolStrip
        /// </devdoc>
        public ToolStripItemRenderEventArgs(Graphics g, ToolStripItem item)
        {
            Graphics = g;
            Item = item;
        }

        /// <devdoc>
        /// The graphics object to draw with
        /// </devdoc>
        public Graphics Graphics { get; }

        /// <devdoc>
        /// The item to draw
        /// </devdoc>
        public ToolStripItem Item { get; }
        
	    /// <devdoc>
	    /// The toolstrip the item is currently parented to
	    /// </devdoc>
        public ToolStrip ToolStrip => Item.ParentInternal;
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Drawing;

    /// <devdoc/>
    public class ToolStripItemRenderEventArgs : EventArgs {

        private ToolStripItem             item             = null;
        private Graphics               graphics         = null;

        /// <devdoc>
        /// This class represents all the information to render the winbar
        /// </devdoc>
        public ToolStripItemRenderEventArgs(Graphics g, ToolStripItem item) {
            this.item = item;
            this.graphics = g;
        }


        /// <devdoc>
        /// the graphics object to draw with
        /// </devdoc>
        public Graphics Graphics {
            get {
                return graphics;    
            }
        }

        /// <devdoc>
        /// the item to draw
        /// </devdoc>
        public ToolStripItem Item { 
            get {
                return item;
            }
        }
        
	    /// <devdoc>
	    /// The toolstrip the item is currently parented to
	    /// </devdoc>
        public ToolStrip ToolStrip { 
            get {
                return item.ParentInternal;
            }
        }
    }
}

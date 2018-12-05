// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Drawing;
    /// <devdoc>
    /// This class represents all the information to render the winbar
    /// </devdoc>
    public class ToolStripSeparatorRenderEventArgs : ToolStripItemRenderEventArgs {

        private bool vertical = false;

        /// <devdoc>
        /// This class represents all the information to render the winbar
        /// </devdoc>
        public ToolStripSeparatorRenderEventArgs(Graphics g, ToolStripSeparator separator, bool vertical) : base(g, separator) {
            this.vertical = vertical;
        }

        /// <devdoc>
        /// the graphics object to draw with
        /// </devdoc>
        public bool Vertical  {
            get {
                return vertical;    
            }
        }
        
    }
}

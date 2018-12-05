// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Drawing;

    /// <devdoc/>
    public class ToolStripGripRenderEventArgs : ToolStripRenderEventArgs {

        /// <devdoc>
        /// This class represents all the information to render the toolStrip
        /// </devdoc>
        public ToolStripGripRenderEventArgs(Graphics g, ToolStrip toolStrip) : base(g, toolStrip) {

        }

        /// <devdoc>
        /// the graphics object to draw with
        /// </devdoc>
        public Rectangle GripBounds  {
            get {
                return ToolStrip.GripRectangle;    
            }
        }


        /// <devdoc>
        /// vertical or horizontal
        /// </devdoc>
        public ToolStripGripDisplayStyle GripDisplayStyle {
            get {
                return ToolStrip.GripDisplayStyle;
            }
        }
        
        /// <devdoc>
        /// visible or not
        /// </devdoc>
        public ToolStripGripStyle GripStyle {
            get {
                return ToolStrip.GripStyle;
            }
        }

    }
}

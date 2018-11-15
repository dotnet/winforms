// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Drawing;

    /// <include file='doc\ToolStripGripRenderEventArgs.uex' path='docs/doc[@for="ToolStripGripRenderEventArgs"]/*' />
    /// <devdoc/>
    public class ToolStripGripRenderEventArgs : ToolStripRenderEventArgs {

        /// <include file='doc\ToolStripGripRenderEventArgs.uex' path='docs/doc[@for="ToolStripGripRenderEventArgs.ToolStripGripRenderEventArgs"]/*' />
        /// <devdoc>
        /// This class represents all the information to render the toolStrip
        /// </devdoc>
        public ToolStripGripRenderEventArgs(Graphics g, ToolStrip toolStrip) : base(g, toolStrip) {

        }

        /// <include file='doc\ToolStripGripRenderEventArgs.uex' path='docs/doc[@for="ToolStripGripRenderEventArgs.GripBounds"]/*' />
        /// <devdoc>
        /// the graphics object to draw with
        /// </devdoc>
        public Rectangle GripBounds  {
            get {
                return ToolStrip.GripRectangle;    
            }
        }


        /// <include file='doc\ToolStripGripRenderEventArgs.uex' path='docs/doc[@for="ToolStripGripRenderEventArgs.GripDisplayStyle"]/*' />
        /// <devdoc>
        /// vertical or horizontal
        /// </devdoc>
        public ToolStripGripDisplayStyle GripDisplayStyle {
            get {
                return ToolStrip.GripDisplayStyle;
            }
        }
        
        /// <include file='doc\ToolStripGripRenderEventArgs.uex' path='docs/doc[@for="ToolStripGripRenderEventArgs.GripStyle"]/*' />
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

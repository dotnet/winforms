// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    public class ToolStripGripRenderEventArgs : ToolStripRenderEventArgs
    {
        /// <devdoc>
        /// This class represents all the information to render the toolStrip
        /// </devdoc>
        public ToolStripGripRenderEventArgs(Graphics g, ToolStrip toolStrip) : base(g, toolStrip)
        {
        }

        /// <devdoc>
        /// The graphics object to draw with
        /// </devdoc>
        public Rectangle GripBounds => ToolStrip.GripRectangle;

        /// <devdoc>
        /// Vertical or horizontal
        /// </devdoc>
        public ToolStripGripDisplayStyle GripDisplayStyle => ToolStrip.GripDisplayStyle;

        /// <devdoc>
        /// Visible or hidden
        /// </devdoc>
        public ToolStripGripStyle GripStyle => ToolStrip.GripStyle;
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    public class ToolStripGripRenderEventArgs : ToolStripRenderEventArgs
    {
        /// <summary>
        ///  This class represents all the information to render the toolStrip
        /// </summary>
        public ToolStripGripRenderEventArgs(Graphics g, ToolStrip toolStrip) : base(g, toolStrip)
        {
        }

        /// <summary>
        ///  The graphics object to draw with
        /// </summary>
        public Rectangle GripBounds => ToolStrip.GripRectangle;

        /// <summary>
        ///  Vertical or horizontal
        /// </summary>
        public ToolStripGripDisplayStyle GripDisplayStyle => ToolStrip.GripDisplayStyle;

        /// <summary>
        ///  Visible or hidden
        /// </summary>
        public ToolStripGripStyle GripStyle => ToolStrip.GripStyle;
    }
}

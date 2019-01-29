// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    public class ToolStripPanelRenderEventArgs : EventArgs
    {
        /// <devdoc>
        ///  This class represents all the information to render the toolStrip
        /// </devdoc>
        public ToolStripPanelRenderEventArgs(Graphics g, ToolStripPanel toolStripPanel)
        {
            Graphics = g;
            ToolStripPanel = toolStripPanel;
        }

        /// <devdoc>
        /// The graphics object to draw with
        /// </devdoc>
        public Graphics Graphics { get; }

        /// <devdoc>
        /// Represents which toolStrip was affected by the click
        /// </devdoc>
        public ToolStripPanel ToolStripPanel { get; }

        public bool Handled { get; set; }
    }
}

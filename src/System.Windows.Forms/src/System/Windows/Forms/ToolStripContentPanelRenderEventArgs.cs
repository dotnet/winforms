// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    public class ToolStripContentPanelRenderEventArgs : EventArgs
    {
        /// <devdoc>
        /// This class represents all the information to render the toolStrip
        /// </devdoc>        
        public ToolStripContentPanelRenderEventArgs(Graphics g, ToolStripContentPanel contentPanel)
        {
            Graphics = g;
            ToolStripContentPanel = contentPanel;
        }

        /// <devdoc>
        /// The graphics object to draw with
        /// </devdoc>
        public Graphics Graphics { get; }

        /// <devdoc>
        /// Represents which toolStrip was affected by the click
        /// </devdoc>
        public ToolStripContentPanel ToolStripContentPanel { get; }

        public bool Handled { get; set; }
    }
}

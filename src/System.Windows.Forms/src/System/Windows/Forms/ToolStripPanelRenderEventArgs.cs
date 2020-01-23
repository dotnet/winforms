// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms
{
    public class ToolStripPanelRenderEventArgs : EventArgs
    {
        /// <summary>
        ///  This class represents all the information to render the toolStrip
        /// </summary>
        public ToolStripPanelRenderEventArgs(Graphics g, ToolStripPanel toolStripPanel)
        {
            Graphics = g;
            ToolStripPanel = toolStripPanel;
        }

        /// <summary>
        ///  The graphics object to draw with
        /// </summary>
        public Graphics Graphics { get; }

        /// <summary>
        ///  Represents which toolStrip was affected by the click
        /// </summary>
        public ToolStripPanel ToolStripPanel { get; }

        public bool Handled { get; set; }
    }
}

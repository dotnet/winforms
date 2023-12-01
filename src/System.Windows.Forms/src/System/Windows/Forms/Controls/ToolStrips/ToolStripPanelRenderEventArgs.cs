// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public class ToolStripPanelRenderEventArgs : EventArgs
{
    /// <summary>
    ///  This class represents all the information to render the toolStrip
    /// </summary>
    public ToolStripPanelRenderEventArgs(Graphics g, ToolStripPanel toolStripPanel)
    {
        Graphics = g.OrThrowIfNull();
        ToolStripPanel = toolStripPanel.OrThrowIfNull();
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

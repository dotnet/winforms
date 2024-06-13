// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace Accessibility_Core_App;

public partial class ToolStripContainer : Form
{
    public ToolStripContainer()
    {
        InitializeComponent();
        toolStripContainer2.TopToolStripPanel.TabStop = true;
        toolStripContainer2.TopToolStripPanel.TabIndex = 0;
        toolStripContainer2.ContentPanel.TabIndex = 1;
    }
}

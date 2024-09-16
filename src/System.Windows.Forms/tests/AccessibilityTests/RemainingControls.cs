// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace Accessibility_Core_App;

public partial class RemainingControls : Form
{
    public RemainingControls()
    {
        InitializeComponent();
        foreach (ToolStripItem item in ((ToolStrip)propertyGrid1.Controls[3]).Items)
        {
            if (item is ToolStripButton)
            {
                item.AutoSize = false;
                item.Width = item.Width < 24 ? 24 : item.Width;
                item.Height = item.Height < 24 ? 24 : item.Height;
            }
        }

        foreach (ToolStripItem item in ((ToolStrip)propertyGrid2.Controls[3]).Items)
        {
            if (item is ToolStripButton)
            {
                item.AutoSize = false;
                item.Width = item.Width < 24 ? 24 : item.Width;
                item.Height = item.Height < 24 ? 24 : item.Height;
            }
        }
    }

    private void Form1_Load(object sender, EventArgs e)
    {
        propertyGrid1.SelectedObject = domainUpDown1;
        propertyGrid2.SelectedObject = trackBar1;
    }
}

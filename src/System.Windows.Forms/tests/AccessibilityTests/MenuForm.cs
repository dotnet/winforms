// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace Accessibility_Core_App;

public partial class MenuForm : Form
{
    public MenuForm()
    {
        InitializeComponent();
    }

    private void Button1_Click(object sender, EventArgs e)
    {
        Menu_Toolbars_controls stripControls = new();
        stripControls.Show();
    }

    private void Button2_Click(object sender, EventArgs e)
    {
        ToolStripContainer toolStripContainer = new();
        toolStripContainer.Show();
    }
}

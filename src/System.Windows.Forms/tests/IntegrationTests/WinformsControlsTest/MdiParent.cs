// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class MdiParent : Form
{
    private readonly MenuStrip _menuStrip;

    public MdiParent()
    {
        InitializeComponent();

        Text = RuntimeInformation.FrameworkDescription;

        ToolStripMenuItem menu = new() { Text = "Open new child" };
        menu.Click += (s, e) =>
        {
            Form child = new()
            {
                MdiParent = this,
                WindowState = FormWindowState.Maximized
            };
            child.Show();
        };

        _menuStrip = new MenuStrip();
        _menuStrip.Items.Add(menu);

        for (int i = 1; i < 7; i++)
        {
            ToolStripMenuItem item = new()
            {
                Alignment = i < 4 ? ToolStripItemAlignment.Left : ToolStripItemAlignment.Right,
                Text = $"Item{i}"
            };
            _menuStrip.Items.Add(item);
        }
    }

    public MenuStrip MainMenu => _menuStrip;

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        MdiChild frm = new()
        {
            MdiParent = this,
            WindowState = FormWindowState.Maximized
        };
        frm.Show();
    }
}

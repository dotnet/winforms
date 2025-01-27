// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class MdiChild : Form
{
    private readonly MenuStrip _menuStrip;

    public MdiChild()
    {
        InitializeComponent();

        _menuStrip = new MenuStrip();
        _menuStrip.Items.Add(new ToolStripMenuItem { Text = "Child1" });
        _menuStrip.Items.Add(new ToolStripMenuItem
        {
            Alignment = ToolStripItemAlignment.Right,
            Text = "Child2",
        });
    }

    private MdiParent MyParent => (MdiParent)MdiParent;

    private void btnOpenChild_Click(object sender, EventArgs e)
    {
        Form frm = new()
        {
            MdiParent = MdiParent,
            WindowState = FormWindowState.Maximized
        };
        frm.Show();
    }

    private void chkSetMenustrip_CheckedChanged(object sender, EventArgs e)
    {
        if (chkSetMenustrip.Checked)
        {
            MainMenuStrip = _menuStrip;
        }
        else
        {
            MainMenuStrip = null;
        }
    }

    private void chkSetParentMenustrip_CheckedChanged(object sender, EventArgs e)
    {
        if (chkSetParentMenustrip.Checked)
        {
            MyParent.MainMenuStrip = MyParent.MainMenu;
        }
        else
        {
            MyParent.MainMenuStrip = null;
        }
    }

    private void chkAddMenustrip_CheckedChanged(object sender, EventArgs e)
    {
        if (chkAddMenustrip.Checked)
        {
            Controls.Add(_menuStrip);
        }
        else
        {
            Controls.Remove(_menuStrip);
        }
    }

    private void chkAddParentMenustrip_CheckedChanged(object sender, EventArgs e)
    {
        if (chkAddParentMenustrip.Checked)
        {
            MyParent.Controls.Add(MyParent.MainMenu);
        }
        else
        {
            MyParent.Controls.Remove(MyParent.MainMenu);
        }
    }

    private void panel1_DoubleClick(object sender, EventArgs e)
    {
        WindowState = FormWindowState.Normal;
    }

    private void chkChildAlign_CheckedChanged(object sender, EventArgs e)
    {
        MyParent.MdiChildrenMinimizedAnchorBottom = !chkChildAlign.Checked;
    }

    private void chkRightToLeft_CheckedChanged(object sender, EventArgs e)
    {
        if (MyParent.MainMenuStrip is not null)
        {
            MyParent.MainMenuStrip.RightToLeft = chkRightToLeft.Checked ? RightToLeft.Yes : RightToLeft.No;
        }
    }
}

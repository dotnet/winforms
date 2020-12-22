// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class MdiParent : Form
    {
        private readonly MenuStrip _menuStrip;

        public MdiParent()
        {
            InitializeComponent();

            Text = RuntimeInformation.FrameworkDescription;

            _menuStrip = new MenuStrip();
            _menuStrip.Items.Add(new ToolStripMenuItem { Text = "Parent" });
        }

        public MenuStrip MainMenu => _menuStrip;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            MdiChild frm = new MdiChild();
            frm.MdiParent = this;
            frm.Show();
        }
    }
}

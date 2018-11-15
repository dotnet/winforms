// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class MDIParent : Form
    {
        public MDIParent()
        {
            InitializeComponent();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Form form = new Form();
            form.MdiParent = this;
            form.DpiChangedAfterParent += Form_DpiChangedAfterParent;
        }

        private void Form_DpiChangedAfterParent(object sender, EventArgs e)
        {
            Form form = sender as Form;
        }

        private void newChildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form child = new MdiChild();
            child.MdiParent = this;
            child.WindowState = FormWindowState.Maximized;
            child.Show();
        }
    }
}

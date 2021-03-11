// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class Dialogs : Form
    {
        private readonly ToolStripButton _btnOpen;

        public Dialogs()
        {
            InitializeComponent();

            _btnOpen = new("Open dialog")
            {
                Image = (System.Drawing.Bitmap)(resources.GetObject("OpenDialog")),
                Enabled = false
            };

            _btnOpen.Click += (s, e) =>
            {
                if (propertyGrid1.SelectedObject is CommonDialog dialog)
                {
                    dialog.ShowDialog(this);
                    return;
                }

                if (propertyGrid1.SelectedObject is Form form)
                {
                    form.ShowDialog(this);
                    return;
                }
            };

            ToolStrip toolbar = GetToolbar();
            toolbar.Items.Add(new ToolStripSeparator { Visible = true });
            toolbar.Items.Add(_btnOpen);
        }

        private ToolStrip GetToolbar()
        {
            foreach (Control control in propertyGrid1.Controls)
            {
                ToolStrip? toolStrip = control as ToolStrip;
                if (toolStrip is not null)
                {
                    return toolStrip;
                }
            }

            throw new MissingMemberException("Unable to find the toolstrip in the PropertyGrid.");
        }

        private void btnOpenFileDialog_Click(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = openFileDialog1;
        }

        private void btnFolderBrowserDialog_Click(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = folderBrowserDialog1;
        }

        private void btnPrintDialog_Click(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = printDialog1;
        }

        private void btnThreadExceptionDialog_Click(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = null;

            using ThreadExceptionDialog dialog = new(new Exception("Really long exception description string, because we want to see if it properly wraps around or is truncated."));
            dialog.ShowDialog(this);
        }

        private void propertyGrid1_SelectedObjectsChanged(object sender, EventArgs e)
        {
            _btnOpen.Enabled = propertyGrid1.SelectedObject is not null;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.Collections;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using static System.ComponentModel.TypeConverter;

namespace WinformsControlsTest
{
    public partial class MessageBoxes : Form
    {
        private readonly ToolStripButton _btnOpen;
        private readonly MessgageBoxProxy _messgageBoxProxy = new MessgageBoxProxy();

        public MessageBoxes()
        {
            InitializeComponent();

            _btnOpen = new("Show MessageBox")
            {
                Image = (System.Drawing.Bitmap?)(resources.GetObject("OpenDialog")),
                Enabled = false
            };

            _btnOpen.Click += (s, e) =>
            {
                MessageBox.Show(this, _messgageBoxProxy.Text, _messgageBoxProxy.Caption,
                    _messgageBoxProxy.Buttons, _messgageBoxProxy.Icon,
                    _messgageBoxProxy.DefaultButton, _messgageBoxProxy.Options);
            };

            ToolStrip toolbar = GetToolbar();
            toolbar.Items.Add(new ToolStripSeparator { Visible = true });
            toolbar.Items.Add(_btnOpen);

            propertyGrid1.SelectedObject = _messgageBoxProxy;
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

        private void propertyGrid1_SelectedObjectsChanged(object sender, EventArgs e)
        {
            _btnOpen.Enabled = propertyGrid1.SelectedObject is not null;
        }

        private class MessgageBoxProxy
        {
            public string Caption { get; set; } = "My Caption";
            public string Text { get; set; } = "Opps, I did it again...";
            public MessageBoxButtons Buttons { get; set; }
            public MessageBoxIcon Icon { get; set; }
            public MessageBoxDefaultButton DefaultButton { get; set; }
            public MessageBoxOptions Options { get; set; }
        }
    }
}

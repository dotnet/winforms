// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class Buttons : Form
    {
        public Buttons()
        {
            InitializeComponent();
        }

        private readonly FlatStyle[] styles = { FlatStyle.Flat, FlatStyle.Popup, FlatStyle.Standard, FlatStyle.System };
        private void Test1_Load(object sender, EventArgs e)
        {
            RadioButton r;
            for (int i = 0; i < styles.Length; i++)
            {
                r = new RadioButton
                {
                    Location = new System.Drawing.Point(20, 20 + 60 * i),
                    AutoSize = false,
                    Size = new System.Drawing.Size(100, 20),
                    FlatStyle = styles[i],
                    Text = styles[i].ToString(),
                    Checked = true
                };
                Controls.Add(r);
            }

            CheckBox c;
            for (int i = 0; i < styles.Length; i++)
            {
                c = new CheckBox
                {
                    Location = new System.Drawing.Point(120, 20 + 60 * i),
                    AutoSize = false,
                    Size = new System.Drawing.Size(100, 20),
                    FlatStyle = styles[i],
                    Text = styles[i].ToString(),
                    Checked = true
                };
                Controls.Add(c);
            }

            Button b;
            for (int i = 0; i < styles.Length; i++)
            {
                b = new Button
                {
                    Location = new System.Drawing.Point(220, 20 + 60 * i),
                    AutoSize = false,
                    Size = new System.Drawing.Size(100, 20),
                    FlatStyle = styles[i],
                    Text = styles[i].ToString()
                };
                Controls.Add(b);
            }
        }
    }
}

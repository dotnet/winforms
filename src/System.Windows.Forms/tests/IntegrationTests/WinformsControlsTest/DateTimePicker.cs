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
    public partial class DateTimePicker : Form
    {
        public DateTimePicker()
        {
            InitializeComponent();

            var checkBox = new CheckBox();
            checkBox.Checked = true;
            checkBox.Location = new Point(27, 60);
            checkBox.Click += CheckBox_Click;
            checkBox.Text = "Enabled";
            this.Controls.Add(checkBox);
        }

        private void CheckBox_Click(object sender, EventArgs e)
        {
            dateTimePicker1.Enabled = !dateTimePicker1.Enabled;
        }
    }
}

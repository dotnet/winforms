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
    public partial class DataGridViewHeaders : Form
    {
        public DataGridViewHeaders()
        {
            InitializeComponent();
        }

        private void DataGridView_Load(object sender, EventArgs e)
        {
            currentDPILabel1.Text = DeviceDpi.ToString();
        }

        private void DataGridView_DpiChanged(object sender, DpiChangedEventArgs e)
        {
            currentDPILabel1.Text = DeviceDpi.ToString();
        }
    }
}

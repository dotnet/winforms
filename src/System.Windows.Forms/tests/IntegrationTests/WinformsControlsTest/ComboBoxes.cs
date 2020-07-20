// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using System.Windows.Forms.IntegrationTests.Common;

namespace WinformsControlsTest
{
    public partial class ComboBoxes : Form
    {
        public ComboBoxes()
        {
            InitializeComponent();

            comboBox1.SelectedIndex = 0;

            dataBoundComboBox.DataSource = TestDataSources.GetPersons();
            dataBoundComboBox.DisplayMember = TestDataSources.PersonDisplayMember;
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            label1.Text = comboBox1.Text;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class ListBoxes : Form
    {
        public ListBoxes()
        {
            InitializeComponent();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            string text = textBox.Text;
            listBox.Items.Add(text);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            listBox.Items.Clear();
        }
    }
}

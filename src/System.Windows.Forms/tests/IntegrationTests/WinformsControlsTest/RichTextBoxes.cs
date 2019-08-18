// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class RichTextBoxes : Form
    {
        public RichTextBoxes()
        {
            InitializeComponent();

            richTextBox1.LoadFile(File.OpenRead(@"Data\example.rtf"), RichTextBoxStreamType.RichText);

        }

        private void RichTextBox1_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            MessageBox.Show(this, e.LinkText, "link clicked");
        }
    }
}

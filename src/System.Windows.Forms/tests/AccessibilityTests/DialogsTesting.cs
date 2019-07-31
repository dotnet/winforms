// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;

namespace AccessibilityTests
{
    public partial class DialogsTesting : Form
    {
        public DialogsTesting()
        {
            InitializeComponent();
        }

        private void ColorDialog_Click(object sender, EventArgs e)
        {
            ColorDialog colordialog = new ColorDialog();
            colordialog.ShowDialog();
        }

        private void FontDialog_Click(object sender, EventArgs e)
        {
            FontDialog fontdialog = new FontDialog();
            fontdialog.ShowDialog();
        }
        private void FolderBrowserDialog_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowDialog();
        }

        private void OpenFileDialog_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();

        }

        private void SaveFileDialog_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.ShowDialog();
        }
    }
}


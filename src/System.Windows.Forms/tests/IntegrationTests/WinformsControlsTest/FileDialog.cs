// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class FileDialog : Form
    {
        public FileDialog()
        {
            InitializeComponent();
        }

        private void Test1_Load(object sender, EventArgs e)
        {
            var openButtonVista = new Button
            {
                Location = new System.Drawing.Point(20, 20),
                AutoSize = false,
                Size = new System.Drawing.Size(250, 25),
                Text = "Show Open File Dialog (Vista)"
            };
            openButtonVista.Click += ShowOpenFileDialogVista_Click;
            Controls.Add(openButtonVista);

            var openButtonNotVista = new Button
            {
                Location = new System.Drawing.Point(20, 20 + 30 * 1),
                AutoSize = false,
                Size = new System.Drawing.Size(250, 25),
                Text = "Show Open File Dialog (Not Vista)"
            };
            openButtonNotVista.Click += ShowOpenFileDialogNotVista_Click;
            Controls.Add(openButtonNotVista);

            var saveButtonVista = new Button
            {
                Location = new System.Drawing.Point(20, 20 + 30 * 2),
                AutoSize = false,
                Size = new System.Drawing.Size(250, 25),
                Text = "Show Save File Dialog (Vista)"
            };
            saveButtonVista.Click += ShowSaveFileDialogVista_Click;
            Controls.Add(saveButtonVista);

            var saveButtonNotVista = new Button
            {
                Location = new System.Drawing.Point(20, 20 + 30 * 3),
                AutoSize = false,
                Size = new System.Drawing.Size(250, 25),
                Text = "Show Save File Dialog (Not Vista)"
            };
            saveButtonNotVista.Click += ShowSaveFileDialogNotVista_Click;
            Controls.Add(saveButtonNotVista);

            var saveButtonFiltered = new Button
            {
                Location = new System.Drawing.Point(20, 20 + 30 * 4),
                AutoSize = false,
                Size = new System.Drawing.Size(250, 25),
                Text = "Show Save File Dialog (Filtered)"
            };
            saveButtonFiltered.Click += ShowSaveFileDialogFiltered_Click;
            Controls.Add(saveButtonFiltered);

            var folderBrowserButtonVista = new Button
            {
                Location = new System.Drawing.Point(20, 20 + 30 * 5),
                AutoSize = false,
                Size = new System.Drawing.Size(250, 25),
                Text = "Show Folder Browser Dialog (Vista)"
            };
            folderBrowserButtonVista.Click += ShowFolderBrowserDialogVista_Click;
            Controls.Add(folderBrowserButtonVista);

            var folderBrowserButtonNotVista = new Button
            {
                Location = new System.Drawing.Point(20, 20 + 30 * 6),
                AutoSize = false,
                Size = new System.Drawing.Size(250, 25),
                Text = "Show Folder Browser Dialog (Not Vista)"
            };
            folderBrowserButtonNotVista.Click += ShowFolderBrowserDialogNotVista_Click;
            Controls.Add(folderBrowserButtonNotVista);
        }

        private void ShowOpenFileDialogVista_Click(object sender, EventArgs e)
        {
            using var d = new OpenFileDialog();
            d.ShowDialog();
        }

        private void ShowOpenFileDialogNotVista_Click(object sender, EventArgs e)
        {
            using var d = new OpenFileDialog
            {
                AutoUpgradeEnabled = false
            };
            d.ShowDialog();
        }

        private void ShowSaveFileDialogVista_Click(object sender, EventArgs e)
        {
            using var d = new SaveFileDialog();
            d.ShowDialog();
        }

        private void ShowSaveFileDialogNotVista_Click(object sender, EventArgs e)
        {
            using var d = new SaveFileDialog
            {
                AutoUpgradeEnabled = false
            };
            d.ShowDialog();
        }

        private void ShowSaveFileDialogFiltered_Click(object sender, EventArgs e)
        {
            using var d = new SaveFileDialog
            {
                Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*"
            };
            d.ShowDialog();
        }

        private void ShowFolderBrowserDialogVista_Click(object sender, EventArgs e)
        {
            using var d = new FolderBrowserDialog();
            d.ShowDialog();
        }

        private void ShowFolderBrowserDialogNotVista_Click(object sender, EventArgs e)
        {
            using var d = new FolderBrowserDialog
            {
                AutoUpgradeEnabled = false
            };
            d.ShowDialog();
        }
    }
}

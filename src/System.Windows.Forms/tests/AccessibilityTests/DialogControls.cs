// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace Accessibility_Core_App;

public partial class DialogControls : Form
{
    public DialogControls()
    {
        InitializeComponent();
    }

    private void ColorDialog_Click(object sender, EventArgs e)
    {
        ColorDialog colorDialog = new();
        colorDialog.ShowDialog();
    }

    private void FolderBrowserDialog_Click(object sender, EventArgs e)
    {
        FolderBrowserDialog folderBrowserDialog = new();
        folderBrowserDialog.ShowDialog();
    }

    private void OpenFileDialog_Click(object sender, EventArgs e)
    {
        OpenFileDialog openFileDialog = new();
        openFileDialog.ShowDialog();
    }

    private void SaveFileDialog_Click(object sender, EventArgs e)
    {
        SaveFileDialog saveFileDialog = new();
        saveFileDialog.ShowDialog();
    }
}

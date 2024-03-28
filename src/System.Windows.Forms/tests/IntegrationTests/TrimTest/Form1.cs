// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace ScratchProject;

// As we can't currently design in VS in the runtime solution, mark as "Default" so this opens in code view by default.
[DesignerCategory("Default")]
public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }

    private void textBox1_DragOver(object sender, DragEventArgs e)
    {
        if (e.Data is { } dataObject && dataObject.GetDataPresent(DataFormats.UnicodeText))
        {
            e.Effect = DragDropEffects.Copy;
        }
    }

    private void textBox1_DragEnter(object sender, DragEventArgs e)
    {
        if (e.Data is { } dataObject && dataObject.GetDataPresent(DataFormats.UnicodeText))
        {
            e.Effect = DragDropEffects.Copy;
        }
    }

    private void textBox1_DragDrop(object sender, DragEventArgs e)
    {
        if (e.Data is { } dataObject && dataObject.GetData(DataFormats.UnicodeText) is string text)
        {
            textBox1.Text = text;
        }
    }

    private void button1_Click(object sender, EventArgs e)
    {
        string text = comboBox1.SelectedItem as string ?? comboBox1.Text ?? string.Empty;
        Clipboard.SetText(text);
    }
}

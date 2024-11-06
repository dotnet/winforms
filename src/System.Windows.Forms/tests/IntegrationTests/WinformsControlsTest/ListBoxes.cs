// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class ListBoxes : Form
{
    public ListBoxes()
    {
        InitializeComponent();
    }

    private void addButton_Click(object sender, EventArgs e)
    {
        var control = (Control)sender;
        var listBox = (ListBox)((object[])control.Tag)[0];
        var textBox = (TextBox)((object[])control.Tag)[1];
        listBox.Items.Add(textBox.Text);
    }

    private void deleteButton_Click(object sender, EventArgs e)
    {
        var control = (Control)sender;
        var listBox = (ListBox)control.Tag;
        listBox.Items.Clear();
    }

    private void ListBox_DrawItem(object sender, DrawItemEventArgs e)
    {
        var listBox = (ListBox)sender;

        Brush customBrush;

        if (e.Index % 2 == 0)
        {
            customBrush = Brushes.Green;
        }
        else
        {
            customBrush = Brushes.Red;
        }

        e.Graphics.DrawString(listBox.Items[e.Index].ToString(),
            e.Font, customBrush, e.Bounds, StringFormat.GenericDefault);

        e.DrawFocusRectangle();
    }

    private void ListBox_MeasureItem(object sender, MeasureItemEventArgs e)
    {
        if (e.Index % 2 == 0)
        {
            e.ItemHeight += 10;
        }
        else
        {
            e.ItemHeight += 5;
        }
    }
}

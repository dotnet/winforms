// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class ErrorProviderTest : Form
{
    public ErrorProviderTest()
    {
        InitializeComponent();
    }

    private void submitButton_Click(object sender, EventArgs e)
    {
        if (textBox1.TextLength is < 5 or > 10)
        {
            errorProvider1.SetError(textBox1, "The length of the testbox is invalid!");
        }
        else
        {
            errorProvider1.Clear();
            MessageBox.Show("All right!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        if (textBox2.TextLength is < 5 or > 20)
        {
            errorProvider2.SetError(textBox2, "The length of the testbox is invalid!");
        }
        else
        {
            errorProvider2.Clear();
            MessageBox.Show("All right!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

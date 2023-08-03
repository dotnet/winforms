// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinformsControlsTest;

public partial class ErrorProviderTest : Form
{
    public ErrorProviderTest()
    {
        InitializeComponent();
    }

    private void submitButton_Click(object sender, EventArgs e)
    {
        if (textBox1.TextLength < 5 || textBox1.TextLength > 10)
        {
            errorProvider1.SetError(textBox1, "The length of the testbox is invalid!");
        }
        else
        {
            errorProvider1.Clear();
            MessageBox.Show("All right!", "OK", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}

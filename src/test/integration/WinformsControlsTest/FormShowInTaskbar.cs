// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public class FormShowInTaskbar : Form
{
    public FormShowInTaskbar()
    {
        Width = 600;
        Height = 460;
        StartPosition = FormStartPosition.CenterScreen;

        Button btnTest = new()
        {
            Text = "Click here to open new Form",
            Location = new Point(10, 10),
            Height = 60,
            AutoSize = true,
        };

        btnTest.Click += BtnTest_Click;
        Controls.Add(btnTest);
    }

    private void BtnTest_Click(object sender, EventArgs e)
    {
        using Form form = new()
        {
            Width = 680,
            Height = 400,
            StartPosition = FormStartPosition.CenterScreen,
        };

        Button btnTest = new()
        {
            Text = $"Click here to test ShowInTaskbar.{Environment.NewLine}If the test result is failed, this dialog will automatically close, or it will throw an exception.",
            Location = new Point(10, 10),
            Height = 60,
            AutoSize = true,
        };

        btnTest.Click += (object sender, EventArgs e) =>
        {
            IntPtr formHandle = form.Handle;
            form.ShowInTaskbar = !form.ShowInTaskbar;

            if (!form.IsHandleCreated)
                throw new InvalidOperationException();

            if (formHandle == form.Handle)
                throw new InvalidOperationException();
        };

        form.Controls.Add(btnTest);
        form.ShowDialog(this);
    }
}

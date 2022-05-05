﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public class FormShowInTaskbar : Form
    {
        public FormShowInTaskbar()
        {
            Width = 600;
            Height = 460;
            StartPosition = FormStartPosition.CenterScreen;

            var btnTest = new Button()
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
            using var form = new Form()
            {
                Width = 680,
                Height = 400,
                StartPosition = FormStartPosition.CenterScreen,
            };

            var btnTest = new Button()
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

                if (form.IsHandleCreated == false)
                    throw new Exception();

                if (formHandle == form.Handle)
                    throw new Exception();
            };

            form.Controls.Add(btnTest);
            form.ShowDialog(this);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms;

namespace TestConsole;

public partial class MainForm
{
    internal class MyUserControl : UserControl
    {
        public MyUserControl()
        {
            InitializeComponent();
        }

        public void InitializeComponent()
        {
            VScroll = false;
            AutoScrollMinSize = new(1, 1);
            HScroll = false;
            BackColor = Color.LightBlue;

            TextBox textBox = new()
            {
                Text = "UserControl TextBox 1",
                Dock = DockStyle.Top,
                Size = new(30, 30)
            };

            TextBox textBox1 = new()
            {
                Text = "UserControl TextBox 2",
                Dock = DockStyle.Top,
                Size = new(30, 30)
            };

            TextBox textBox2 = new()
            {
                Text = "UserControl TextBox 3",
                Dock = DockStyle.Bottom,
                Size = new(30, 30)
            };

            Controls.AddRange(textBox2, textBox1, textBox);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
internal class CustomComCtl32Button : Form
{
    public CustomComCtl32Button()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        ClientSize = new Size(400, 100);

        var controls = new Control[]
        {
            new CheckBox()
            {
                FlatStyle = FlatStyle.Standard,
                CheckState = CheckState.Unchecked,
                Appearance = Appearance.Button,

                AutoSize = true,
                Location = new Point(1, 0)
            },

            new CheckBox()
            {
                FlatStyle = FlatStyle.Standard,
                Appearance = Appearance.Button,
                CheckState = CheckState.Checked,

                AutoSize = true,
                Location = new Point(1, 30)
            },

            new Button()
            {
                FlatStyle = FlatStyle.Standard,
                Text = $"Real Button Standard Not Pressed",

                AutoSize = true,
                Location = new Point(1, 60)
            },
       };

        foreach (Control control in controls)
        {
            control.Paint += (sender, e) => DrawRoundBorder((Control)sender, e.Graphics);

            if (control.GetType().Name == "Button")
            {
                var button = (Button)control;

                button.MouseDown += (sender, e) =>
                {
                    button.Text = $"Real Button Standard Pressed";
                };

                button.MouseUp += (sender, e) =>
                {
                    button.Text = $"Real Button Standard Not Pressed";
                };
            }

            Controls.Add(control);
        }

        static void DrawRoundBorder(Control sender, Graphics g)
        {
            bool isPressed = false;
            if (sender.GetType().Name == "CheckBox")
            {
                CheckBox checkbox = (CheckBox)sender;
                isPressed = checkbox.CheckState == CheckState.Checked;
                checkbox.Text = $"CheckBox (Button appearance) Standard {(isPressed ? "Checked" : "Unchecked")}";
            }

            if (sender.GetType().Name == "Button")
            {
                if (sender.Text == "Real Button Standard Pressed")
                    isPressed = true;
            }

            // Test possible approach to https://github.com/dotnet/winforms/issues/6514
            if (Application.RenderWithVisualStyles && !isPressed)
            {
                GraphicsPath path = new();
                Pen pen = new(SystemColors.ControlDarkDark, 1);

                int x = sender.ClientRectangle.X + 1;
                int y = sender.ClientRectangle.Y + 1;
                int width = sender.ClientRectangle.Width - 10;
                int height = sender.ClientRectangle.Height - 10;
                const int radius = 8;

                // Top-left corner
                path.AddArc(new Rectangle(x, y, radius, radius), 180, 90);

                // Top-right corner
                path.AddArc(new Rectangle(width, y, radius, radius), 270, 90);

                // Bottom-right corner
                path.AddArc(new Rectangle(width, height, radius, radius), 0, 90);

                // Bottom-left corner
                path.AddArc(new Rectangle(x, height, radius, radius), 90, 90);

                // Back to Top-left corner
                path.AddArc(new Rectangle(x, y, radius, radius), 180, 90);

                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.DrawPath(pen, path);
            }
        }
    }
}

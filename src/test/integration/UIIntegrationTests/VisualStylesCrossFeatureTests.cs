// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Drawing.Drawing2D;

namespace System.Windows.Forms.UITests;

public class VisualStylesCrossFeatureTests : ControlTestBase
{
    public VisualStylesCrossFeatureTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task VisualStyles_InheritedControls_RenderAgainstPatternedParentAcrossModesAndDpiAsync()
    {
        List<Control> samples = [];

        await RunFormWithoutControlAsync(
            () =>
            {
                Form form = new()
                {
                    AutoScaleMode = AutoScaleMode.Dpi,
                    RightToLeft = RightToLeft.Yes,
                    RightToLeftLayout = true,
                    Size = new Size(900, 600)
                };

                PatternedGradientPanel parent = new()
                {
                    Dock = DockStyle.Fill,
                    Padding = new Padding(8),
                    RightToLeft = RightToLeft.Yes,
                    VisualStylesMode = VisualStylesMode.Classic
                };
                form.Controls.Add(parent);

                TableLayoutPanel table = new()
                {
                    AutoScroll = true,
                    BackColor = Color.Transparent,
                    ColumnCount = 3,
                    Dock = DockStyle.Fill,
                    RowCount = 11
                };
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
                table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
                parent.Controls.Add(table);

                table.Controls.Add(new Label { Text = "Control / font", AutoSize = true }, 0, 0);
                table.Controls.Add(new Label { Text = "AutoSize", AutoSize = true }, 1, 0);
                table.Controls.Add(new Label { Text = "Fixed small", AutoSize = true }, 2, 0);

                string[] controlKinds = ["TextBox", "MaskedTextBox", "RichTextBox", "NumericUpDown", "DomainUpDown"];
                int row = 1;
                foreach (float fontSize in new[] { 9f, 11f })
                {
                    foreach (string controlKind in controlKinds)
                    {
                        table.Controls.Add(new Label { Text = $"{controlKind} ({fontSize:0} pt)", AutoSize = true }, 0, row);
                        Control autoSized = CreateSampleControl(controlKind, fontSize, autoSize: true);
                        Control fixedSmall = CreateSampleControl(controlKind, fontSize, autoSize: false);
                        samples.Add(autoSized);
                        samples.Add(fixedSmall);
                        table.Controls.Add(autoSized, 1, row);
                        table.Controls.Add(fixedSmall, 2, row);
                        row++;
                    }
                }

                return form;
            },
            async form =>
            {
                Panel parent = (Panel)form.Controls[0];
                Assert.Equal(form.DeviceDpi, parent.DeviceDpi);
                Assert.All(samples, sample => Assert.Equal(RightToLeft.Yes, sample.RightToLeft));

                foreach (Control sample in samples)
                {
                    Assert.Equal(VisualStylesMode.Inherit, sample.VisualStylesMode);
                    Assert.True(sample.Width > 0);
                    Assert.True(sample.Height > 0);
                    using Bitmap bitmap = new(sample.Width, sample.Height);
                    sample.DrawToBitmap(bitmap, new Rectangle(Point.Empty, sample.Size));
                }

                parent.VisualStylesMode = VisualStylesMode.Net11;
                form.PerformLayout();
                Assert.All(
                    samples,
                    sample => Assert.Equal(VisualStylesMode.Inherit, sample.VisualStylesMode));

                foreach (Control sample in samples)
                {
                    using Bitmap bitmap = new(sample.Width, sample.Height);
                    sample.DrawToBitmap(bitmap, new Rectangle(Point.Empty, sample.Size));
                }

                parent.VisualStylesMode = VisualStylesMode.Classic;
                await Task.Yield();
                Assert.All(
                    samples,
                    sample => Assert.Equal(VisualStylesMode.Inherit, sample.VisualStylesMode));
            });
    }

    [WinFormsFact]
    public async Task VisualStyles_StandardSystemAndPopup_RenderFocusedDefaultAndPressedStatesAsync()
    {
        await RunFormWithoutControlAsync(
            () =>
            {
                Form form = new() { Size = new Size(500, 220) };
                FlowLayoutPanel panel = new()
                {
                    Dock = DockStyle.Fill,
                    RightToLeft = RightToLeft.Yes,
                    FlowDirection = FlowDirection.LeftToRight
                };
                form.Controls.Add(panel);
                form.RightToLeft = RightToLeft.Yes;
                form.RightToLeftLayout = true;
                form.AutoScaleMode = AutoScaleMode.Dpi;

                Button standard = CreateButton(FlatStyle.Standard, "Standard");
                standard.NotifyDefault(true);
                form.AcceptButton = standard;
                panel.Controls.Add(standard);
                foreach (FlatStyle style in new[] { FlatStyle.System, FlatStyle.Popup })
                {
                    Button button = CreateButton(style, style.ToString());
                    button.NotifyDefault(true);
                    panel.Controls.Add(button);
                }

                return form;
            },
            async form =>
            {
                FlowLayoutPanel panel = (FlowLayoutPanel)form.Controls[0];
                foreach (Button button in panel.Controls.OfType<Button>())
                {
                    Assert.Equal(VisualStylesMode.Net11, button.VisualStylesMode);
                    button.Select();
                    button.Focus();
                    using Bitmap focused = new(button.Width, button.Height);
                    button.DrawToBitmap(focused, new Rectangle(Point.Empty, button.Size));

                    bool clicked = false;
                    button.Click += (_, _) => clicked = true;
                    button.PerformClick();
                    Assert.True(clicked);
                }
            });
    }

    private static Button CreateButton(FlatStyle style, string text)
        => new()
        {
            FlatStyle = style,
            Text = text,
            VisualStylesMode = VisualStylesMode.Net11,
            Size = new Size(130, 40)
        };

    private static Control CreateSampleControl(string controlKind, float fontSize, bool autoSize)
    {
        Control control = controlKind switch
        {
            "TextBox" => new TextBox { Text = "Text sample" },
            "MaskedTextBox" => new MaskedTextBox { Mask = "00000", Text = "12345" },
            "RichTextBox" => new RichTextBox { Text = "Rich text", Multiline = true, ScrollBars = RichTextBoxScrollBars.Vertical },
            "NumericUpDown" => new NumericUpDown { Minimum = 0, Maximum = 100, Value = 42 },
            "DomainUpDown" => new DomainUpDown { Items = { "First", "Second", "Third" }, SelectedIndex = 1 },
            _ => throw new ArgumentOutOfRangeException(nameof(controlKind))
        };

        control.AutoSize = autoSize;
        control.Font = new Font(Control.DefaultFont.FontFamily, fontSize);
        control.RightToLeft = RightToLeft.Inherit;
        control.VisualStylesMode = VisualStylesMode.Inherit;
        control.Margin = new Padding(4);

        if (autoSize)
        {
            control.Width = 150;
        }
        else
        {
            control.Size = new Size(120, 14);
        }

        return control;
    }

    private sealed class PatternedGradientPanel : Panel
    {
        public PatternedGradientPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (ClientSize.Width == 0 || ClientSize.Height == 0)
            {
                return;
            }

            using LinearGradientBrush gradient = new(
                ClientRectangle,
                Color.FromArgb(38, 72, 126),
                Color.FromArgb(126, 76, 128),
                LinearGradientMode.Horizontal);
            e.Graphics.FillRectangle(gradient, ClientRectangle);

            using HatchBrush pattern = new(
                HatchStyle.DiagonalCross,
                Color.FromArgb(32, Color.White),
                Color.Transparent);
            e.Graphics.FillRectangle(pattern, ClientRectangle);
        }
    }
}
